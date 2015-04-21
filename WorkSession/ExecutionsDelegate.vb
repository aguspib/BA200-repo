Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.BL

    Partial Public Class ExecutionsDelegate
        Private NullElementID As Integer = -1 'RH 07/04/2011

#Region "Public Methods"

        ''' <summary>
        ''' Process volume missing
        ''' UPdate position as DEPLETED. If no more volume available lock pending executions (different business)
        ''' Also used when a reagents bottle is LOCKED due to INVALID REFILL
        ''' 'Business:
        ''' Reagent volume missing: Apply lock using the same by ElementID
        ''' Sample volume missing: Apply special business depending sample class
        ''' When Bottle Is locked then bottle status change to LOCKED in case not then is DEPLETED
        ''' When the bottle is locked then need to inform the Real Volume and the Test left to update on table
        ''' other winse in case is DEPLETED O not LOCKED then the Real Volume And The Test left are not needed. the value
        ''' will be 0.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pPreparationID">preparation identifier</param>
        ''' <param name="pRotorType">rotor type</param>
        ''' <param name="pCellNumber">cell number</param>
        ''' <param name="pPreparationIDPerformedOK" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pIsBottleLocked">Optional parammeter to indicate if the bottle status is locked other wise is depleted</param>
        ''' <param name="pRealVolume">IF the bottle is locked then need to recive the realvolume to be updated</param>
        ''' <param name="pTestLeft">IF the bottle is locked then need to recive the TestLeft to be updated</param>
        ''' 
        ''' <returns >Returns a globalDataTO with setData as ExecutionsDS with the new locked executions list</returns>
        ''' <remarks>
        ''' Created by:  DL 05/01/2011
        ''' AG 30/03/2011 - add pPreparationIDPerformedOK parameter (means the pPreparationID has been prepared OK and we have not to change his status)
        '''                 Clean and comment code. Also develop some corrections over the DL original code.
        ''' Modified by: TR 28/09/2012 -Add optional parameter IsBottleLocked to indicate if the bottle status is locked, this
        '''                             is for implementation of ReagentsOnBoard Locked bottle.
        ''' Modified by AG 04/10/2012 - add byref parameter pTurnToPendingFlag (used in ProcessArmStatusRecevied, when return TRUE means the Sw has to search next preparation to be sent again)
        ''' AG 13/06/2014 #1662 (add also protection against empty position causing method returns a wrong datatype!!!)
        '''                     (if not element in position but preparationID value different than zero -- then lock the executions linked with this preparationID)
        ''' </remarks>
        Public Function ProcessVolumeMissing(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer, _
                                             ByVal pRotorType As String, ByVal pCellNumber As Integer, _
                                             ByVal pPreparationIDPerformedOK As Boolean, ByVal pWorkSessionID As String, _
                                             ByVal pAnalyzerID As String, ByRef pTurnToPendingFlag As Boolean, Optional ByVal pIsBottleLocked As Boolean = False, _
                                             Optional ByVal pRealVolume As Single = 0, Optional ByVal pTestLeft As Integer = 0) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim myReturnValue As String = "LOCKED"

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myBottleStatus As String = ""
                        If (pIsBottleLocked) Then
                            myBottleStatus = "LOCKED"
                        Else
                            myBottleStatus = "DEPLETED"
                            pRealVolume = 0 'Set value to Cero for security.
                            pTestLeft = 0 'Set value to Cero for security.
                        End If

                        Dim myWSRotorContentByPosition As New WSRotorContentByPositionDelegate
                        'TR 01/10/2012 -Implement the optional parammeters RealVolume and TestLeft.
                        resultData = myWSRotorContentByPosition.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, _
                                                                                               pCellNumber, myBottleStatus, pRealVolume, pTestLeft, False, False)

                        '1.Update position to DEPLETED and reset RealVolume and remaining test to zero
                        'resultData = myWSRotorContentByPosition.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, _
                        '                                                                       pCellNumber, myBottleStatus, 0, 0, False, False)
                        'TR 01/10/2012 -END.
                        If (Not resultData.HasError) Then
                            '1b.Get elementid, multitubeitem
                            resultData = myWSRotorContentByPosition.ReadByRotorTypeAndCellNumber(dbConnection, pRotorType, pCellNumber, pWorkSessionID, pAnalyzerID)

                            Dim myWSRotorContentByPositionDS As New WSRotorContentByPositionDS
                            myWSRotorContentByPositionDS = CType(resultData.SetDatos, WSRotorContentByPositionDS)

                            'If (Not resultData.HasError) Then
                            If (Not resultData.HasError) AndAlso myWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0 Then 'AG 08/01/2014 - add this line to avoid exception (esporadic)
                                Dim myElementID As Integer = 0
                                Dim lockedExecutionsSet1 As New ExecutionsDS 'Executions locked before start the new lock process

                                If Not myWSRotorContentByPositionDS.twksWSRotorContentByPosition.First.IsElementIDNull Then
                                    myElementID = myWSRotorContentByPositionDS.twksWSRotorContentByPosition.First.ElementID

                                    '2 Exist other position with volume?
                                    resultData = myWSRotorContentByPosition.ExistOtherPosition(dbConnection, myElementID, pWorkSessionID, pAnalyzerID)
                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                        myWSRotorContentByPositionDS = CType(resultData.SetDatos, WSRotorContentByPositionDS)

                                        If myWSRotorContentByPositionDS.twksWSRotorContentByPosition.Count > 0 Then
                                            '2a. YES: Do not apply locks
                                            myReturnValue = "PENDING"
                                            pTurnToPendingFlag = True 'Inform the execution status has changed from INPROCESS to PENDING

                                        ElseIf myWSRotorContentByPositionDS.twksWSRotorContentByPosition.Count = 0 Then
                                            '2b.  NO: Apply locks
                                            myReturnValue = "LOCKED"

                                            'AG 02/01/2012 - Get the current LOCKED executions set (before starts the new lock process)
                                            resultData = GetExecutionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, "LOCKED", False)
                                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                lockedExecutionsSet1 = CType(resultData.SetDatos, ExecutionsDS)
                                            End If
                                            'AG 02/01/2012

                                        End If

                                        'Get the execution, sample class, ... related to the preparation ID
                                        Dim myExecutionDS As New ExecutionsDS

                                        'PreparationID may be 0 for other actions different from test preparation (for instance contamination washings)
                                        If pPreparationID > 0 Then
                                            resultData = GetExecutionByPreparationID(dbConnection, pPreparationID, pWorkSessionID, pAnalyzerID)
                                            If Not resultData.HasError Then
                                                myExecutionDS = CType(resultData.SetDatos, ExecutionsDS)

                                                If Not pPreparationIDPerformedOK Then
                                                    '2c. Update status for the preparation ID (LOCKED or PENDING) depending if more volume is available or not
                                                    Dim myExecutionID As Integer = 0 ' Init DL 19/01/2011

                                                    For Each myExecutionRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
                                                        If Not myExecutionRow.IsExecutionIDNull Then
                                                            'Update status for my Execution 
                                                            myExecutionID = myExecutionRow.ExecutionID
                                                            resultData = UpdateStatusByExecutionID(dbConnection, myReturnValue, myExecutionID, pWorkSessionID, pAnalyzerID)
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        End If

                                        Dim mySampleClass As String = ""
                                        Dim myCurrentExecStatus As String = ""
                                        Dim myMultiItemNumber As Integer = 1 'AG 03/10/2012
                                        Dim myReplicateNumber As Integer = 1 'AG 03/10/2012
                                        Dim myOrderTest As Integer = -1 'AG 03/10/2012
                                        If myExecutionDS.twksWSExecutions.Rows.Count > 0 Then
                                            If Not myExecutionDS.twksWSExecutions(0).IsSampleClassNull Then mySampleClass = myExecutionDS.twksWSExecutions(0).SampleClass
                                            If Not myExecutionDS.twksWSExecutions(0).IsExecutionStatusNull Then myCurrentExecStatus = myExecutionDS.twksWSExecutions(0).ExecutionStatus 'execution Status previous to be locked 

                                            'AG 03/10/2012 - add information for multipoint calibrators case (ReplicateNumber, MultiItemNumber and OrderTestID)
                                            If Not myExecutionDS.twksWSExecutions(0).IsReplicateNumberNull Then myReplicateNumber = myExecutionDS.twksWSExecutions(0).ReplicateNumber
                                            If mySampleClass = "CALIB" AndAlso Not myExecutionDS.twksWSExecutions(0).IsMultiItemNumberNull AndAlso Not myExecutionDS.twksWSExecutions(0).IsOrderTestIDNull Then
                                                myMultiItemNumber = myExecutionDS.twksWSExecutions(0).MultiItemNumber
                                                myOrderTest = myExecutionDS.twksWSExecutions(0).OrderTestID
                                            End If
                                            'AG 03/10/2012
                                        End If

                                        Dim dataToReturn As New ExecutionsDS
                                        'Only lock executions where no error, no more volume available and the execution failed not is still locked 
                                        If Not resultData.HasError AndAlso myReturnValue = "LOCKED" AndAlso myCurrentExecStatus <> "LOCKED" Then

                                            'AG 02/01/2012 - 
                                            ''AG 19/04/2011 - Get current LOCKED executions set (before starts the new lock process)
                                            'Dim lockedExecutionsSet1 As New ExecutionsDS
                                            'resultData = GetExecutionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, "LOCKED", False)
                                            'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            '    lockedExecutionsSet1 = CType(resultData.SetDatos, ExecutionsDS)
                                            '    'AG 19/04/2011
                                            If Not resultData.HasError Then
                                                'AG 02/01/2012

                                                '3. Update ElementID as NOPOS
                                                Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                                                resultData = myRequiredElementsDelegate.UpdateStatus(dbConnection, myElementID, "NOPOS")

                                                If (Not resultData.HasError) Then
                                                    '4a. Get ElementID information
                                                    resultData = myRequiredElementsDelegate.GetRequiredElementData(dbConnection, myElementID)
                                                    Dim req_ElementDS As New WSRequiredElementsDS
                                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                                        req_ElementDS = CType(resultData.SetDatos, WSRequiredElementsDS)
                                                        If req_ElementDS.twksWSRequiredElements.Rows.Count > 0 Then
                                                            Dim myTubeContent As String = ""
                                                            Dim mySolutionCode As String = ""
                                                            If Not req_ElementDS.twksWSRequiredElements(0).IsTubeContentNull Then myTubeContent = req_ElementDS.twksWSRequiredElements(0).TubeContent
                                                            If Not req_ElementDS.twksWSRequiredElements(0).IsSolutionCodeNull Then mySolutionCode = req_ElementDS.twksWSRequiredElements(0).SolutionCode

                                                            '4b. Different business depending the TubeContent and SampleClass
                                                            resultData = ProcessVolumeMissingBySampleClass(dbConnection, pWorkSessionID, pAnalyzerID, myElementID, mySampleClass, myTubeContent, mySolutionCode, _
                                                                                                           pPreparationID, myReplicateNumber, myMultiItemNumber, myOrderTest, myBottleStatus)
                                                        End If
                                                    End If
                                                End If

                                                'AG 19/04/2011 - Now get the new LOCKED executions (after new process has been performed)
                                                'The locked executions in due the current alarm call = lockedExecSet2 - lockedExecSet1
                                                Dim lockedExecutionsSet2 As New ExecutionsDS
                                                resultData = GetExecutionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, "LOCKED", False)
                                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                    lockedExecutionsSet2 = CType(resultData.SetDatos, ExecutionsDS)

                                                    Dim myLinq As New List(Of ExecutionsDS.twksWSExecutionsRow)
                                                    For Each row As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsSet2.twksWSExecutions
                                                        'If row.ExecutionID exists in lockedExSet1
                                                        myLinq = (From a As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsSet1.twksWSExecutions _
                                                                  Where a.ExecutionID = row.ExecutionID Select a).ToList

                                                        'If not exists add to the dataToReturn
                                                        If myLinq.Count = 0 Then
                                                            Dim executionRow As ExecutionsDS.twksWSExecutionsRow
                                                            executionRow = dataToReturn.twksWSExecutions.NewtwksWSExecutionsRow
                                                            executionRow = row
                                                            'TR 22/09/2011 -Change Add for Import because add produce error.
                                                            dataToReturn.twksWSExecutions.ImportRow(executionRow)
                                                        End If
                                                    Next
                                                    dataToReturn.AcceptChanges()
                                                End If
                                                'AG 19/04/2011

                                            End If
                                        End If
                                        resultData.SetDatos = dataToReturn

                                    End If
                                End If

                                'AG 16/06/2014 #1662 - if user has changed or removed bottle after TEST/PTEST/ISETEST had been sent and there is empty position
                                'then LOCK executions using this preparationID
                            ElseIf pPreparationID <> 0 Then
                                resultData = GetExecutionByPreparationID(dbConnection, pPreparationID, pWorkSessionID, pAnalyzerID)
                                If Not resultData.HasError Then
                                    Dim myExecutionDS As New ExecutionsDS
                                    myExecutionDS = CType(resultData.SetDatos, ExecutionsDS)

                                    If Not pPreparationIDPerformedOK Then
                                        '2c. Update status for the preparation ID (LOCKED or PENDING) depending if more volume is available or not
                                        myReturnValue = "LOCKED"
                                        For Each myExecutionRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
                                            If Not myExecutionRow.IsExecutionIDNull Then
                                                'Update status for my Execution 
                                                resultData = UpdateStatusByExecutionID(dbConnection, myReturnValue, myExecutionRow.ExecutionID, pWorkSessionID, pAnalyzerID)
                                            End If
                                        Next
                                    End If
                                    resultData.SetDatos = myExecutionDS 'Return executions locked!!!

                                End If
                                'AG 16/06/2014 #1662

                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                        'AG 13/06/2014 #1662 (add also protection against empty position causing method returns a wrong datatype!!!)
                        If Not resultData.HasError AndAlso Not TypeOf (resultData.SetDatos) Is ExecutionsDS Then
                            Dim temp As New ExecutionsDS
                            resultData.SetDatos = temp
                        End If
                        'AG 13/06/2014 #1662

                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ProcessVolumeMissing", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return (resultData)

        End Function

        ''' <summary>
        ''' Get the list of Executions with data to be monitored
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the results (view vwksWSExecutionsMonitor)
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL - 30/11/2010
        ''' </remarks>
        Public Function GetByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pAnalyzerID As String, _
                                         ByVal pWorkSessionID As String, _
                                         ByVal pOrderTestID As Integer) As GlobalDataTO
            'ByVal pExecutionID As Integer) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetByOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID) ', pExecutionID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetByOrderTestID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get basic data of the specified Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks></remarks>
        Public Function GetExecution(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer, _
                                     Optional ByVal pAnalyzerID As String = "", Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.GetExecution(dbConnection, pExecutionID, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecution", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Executions for the specified Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier></param>
        ''' <param name="pWorkSessionID">Work Session Identifier></param>
        ''' <param name="pFlagOnlyPrepSTD"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetExecutionByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String, Optional ByVal pFlagOnlyPrepSTD As Boolean = False) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.GetExecutionByWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pFlagOnlyPrepSTD)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecution", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the Calibrator to which corresponds the specified Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS containing the 
        '''          data of the Calibrator to which corresponds the specified Execution</returns>
        ''' <remarks>
        ''' Created by : DL 23/02/2010
        ''' Modified by: SA 30/08/2010 - Function name changed from GetCalibratorID to GetCalibratorData.
        '''                              After get the Calibrator data, for Special Calibrators, verify if 
        '''                              there are Special Settings for the requested Test/SampleType and in
        '''                              this case, set the real Number of Calibrator used for the Test/SampleID
        '''              AG 31/08/2010 - If the Calibrator is marked as special but for the Test/SampleType there 
        '''                              are not Calibration Settings defined in tfmwSpecialTestsSettings table,
        '''                              return the DS returned from GetCalibratorData 
        '''              SA 01/09/2010 - Add the DS returned from GetCalibratorData also when the Calibrator is not 
        '''                              marked as special: AG changed is moved to the inmediate external IF (in fact,
        '''                              currently it is returned, it is only as preventive measure)
        '''              AG 02/09/2010 - Added optional parameters (TestID, SampleType). If not informed, use original code
        '''                              getting data (TestSampleCalibratorDS) from ExecutionID, else (if optional parameters informed) 
        '''                              get data (TestSampleCalibratorDS) from TestID and SampleType 
        '''              AG 12/07/2012 - When parameters for Test/SampleType are informed and function GetCalibratorDataForCalculations is
        '''                              called, if the CalibratorType is FACTOR, then a DS with value of the CalibratorFactor is returned;
        '''                              then, validation of Special Calibrator should be done only when the CalibratorType is EXPERIMENT 
        '''                              (field SpecialCalib is not NULL)
        ''' </remarks>
        Public Function GetCalibratorData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer, _
                                          Optional ByVal pTestID As Integer = -1, Optional ByVal pSampleType As String = Nothing) _
                                          As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'AG 02/09/2010 - Get data from Executions or TestCalibrators depending if optional parameters
                        'are informed or not
                        If (pTestID < 0 OrElse pSampleType = Nothing) Then
                            'Original Code (get data searching by ExecutionID)
                            Dim mytwksWSExecutions As New twksWSExecutionsDAO
                            resultData = mytwksWSExecutions.GetCalibratorData(pDBConnection, pExecutionID)

                        Else
                            'New Code (get data from TestCalibrators or TestSamples, depending on the CalibratorType)
                            Dim myTestCalibDelegate As New TestCalibratorsDelegate
                            resultData = myTestCalibDelegate.GetCalibratorDataForCalculations(dbConnection, pTestID, pSampleType)
                        End If
                        'END AG 02/09/2010

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim calibratorDataDS As TestSampleCalibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                            If (calibratorDataDS.tparTestCalibrators.Rows.Count = 1) Then
                                If (Not calibratorDataDS.tparTestCalibrators(0).IsSpecialCalibNull AndAlso calibratorDataDS.tparTestCalibrators(0).SpecialCalib) Then
                                    'For Special Calibrators, verify if there are Special Settings for the requested Test/SampleType
                                    Dim mySpecialTestsSettingsDelegate As New SpecialTestsSettingsDelegate
                                    resultData = mySpecialTestsSettingsDelegate.Read(dbConnection, calibratorDataDS.tparTestCalibrators(0).TestID, _
                                                                                     calibratorDataDS.tparTestCalibrators(0).SampleType, _
                                                                                     GlobalEnumerates.SpecialTestsSettings.TOTAL_CAL_POINTS.ToString)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim mySpTestSettingsDS As New SpecialTestsSettingsDS
                                        mySpTestSettingsDS = DirectCast(resultData.SetDatos, SpecialTestsSettingsDS)

                                        If (mySpTestSettingsDS.tfmwSpecialTestsSettings.Rows.Count = 1) Then
                                            'There are Special Settings for the requested Test/SampleType. Set real Number of Calibrators used for it
                                            calibratorDataDS.tparTestCalibrators(0).BeginEdit()
                                            calibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators = Convert.ToInt32(mySpTestSettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
                                            calibratorDataDS.tparTestCalibrators(0).EndEdit()

                                            'AG 31/08/2010
                                            'resultData.SetDatos = calibratorDataDS
                                            'resultData.HasError = False
                                        End If
                                    End If
                                End If

                                'AG 31/08/2010
                                resultData.SetDatos = calibratorDataDS
                                resultData.HasError = False
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetCalibratorData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Executions for the specified Preparation Identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparationID">Preparation Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pExcludePendingOrLocked">Optional parameter. When its value is TRUE, Executions with status PENDING 
        '''                                       or LOCKED will not be returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of all Executions for the
        '''          specified Preparation Identifier</returns>
        ''' <remarks>
        ''' Created by: GDS 27/04/2010
        ''' Modified by: AG 31/03/2011 - Added parameters WorkSessionID and AnalyzerID
        '''              SA 03/07/2012 - Added optional parameter pExcludePendingOrLocked. When TRUE, Executions with status PENDING
        '''                              or LOCKED will not be returned
        ''' </remarks>
        Public Function GetExecutionByPreparationID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer, _
                                                    ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                    Optional ByVal pExcludePendingOrLocked As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.GetExecutionByPreparationID(dbConnection, pPreparationID, pWorkSessionID, pAnalyzerID, pExcludePendingOrLocked)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecutionByPreparationID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Linear kinetic
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun number</param>
        ''' <param name="pMultiItemNumber" ></param>
        ''' <param name="pExecutionStatus" ></param>
        ''' <param name="pMode" ></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by: DL - 06/05/2010
        ''' Modified by AG 24/02/2011 - add pMode parameter: CURVE_RESULTS_MULTIPLE or RESULTS_MULTIPLE (with execution status = CLOSED or CLOSEDNOK), WSSTATES_RESULTS_MULTIPLE (with execution status = CLOSED or CLOSEDNOK or INPROCESS)
        ''' </remarks>
        Public Function ReadByOrderTestIDandRerunNumber(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                        ByVal pOrderTestID As Integer, _
                                                        ByVal pRerunNumber As Integer, _
                                                        Optional ByVal pMultiItemNumber As Integer = -1, _
                                                        Optional ByVal pExecutionStatus As String = "", _
                                                        Optional ByVal pMode As GlobalEnumerates.GraphicalAbsScreenCallMode = GlobalEnumerates.GraphicalAbsScreenCallMode.NONE) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO

                        resultData = mytwksWSExecutionsDAO.ReadByOrderTestIDAndRerunNumber(dbConnection, pOrderTestID, pRerunNumber, pMultiItemNumber, pExecutionStatus, pMode)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ReadByOrderTestIDandRerunNumber", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get all different order testid with executions closed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by: DL - 14/02/2011
        ''' </remarks> 
        Public Function GetOrderTestWithExecutionStatus(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                        ByVal pSourceForm As GlobalEnumerates.ScreenCallsGraphical, _
                                                        ByVal pRerun As Integer, _
                                                        ByVal pOrderTestID As Integer, _
                                                        ByVal pAnalyzerID As String, _
                                                        ByVal pWorkSessionID As String, _
                                                        Optional ByVal pExecutionID As Integer = -1) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.GetOrderTestWithExecutionStatus(dbConnection, pSourceForm, pRerun, pOrderTestID, pAnalyzerID, pWorkSessionID, pExecutionID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetOrderTestWithExecutionStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get results depending on input parameters (orderTestId and replicateID)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pMultiItemNum"></param>
        ''' <param name="pReplicateID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <returns>Global Data To with set data as ExecutionsDS</returns>
        ''' <remarks>
        ''' Created by AG 01/03/2010 (Tested OK)
        ''' Modified by AG 04/08/2010 - add pRerunNumber
        ''' </remarks>
        Public Function GetResultsOrderTestReplicate(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                     ByVal pAnalyzerID As String, _
                                                     ByVal pWorkSessionID As String, _
                                                     ByVal pOrderTestID As Integer, _
                                                     ByVal pMultiItemNum As Integer, _
                                                     ByVal pReplicateID As Integer, _
                                                     ByVal pRerunNumber As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO

                        resultData = mytwksWSExecutionsDAO.ReadOrderTestReplicate(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pMultiItemNum, pReplicateID, pRerunNumber)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetResultsOrderTestReplicate", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Save data of the specified Executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionsDS">Typed DataSet ExecutionsDS containing all data of the Executions to save</param>
        ''' <param name="pRecalculusFlag">Optional parameter with default value FALSE. When it is informed as TRUE, fields ABS_Initial, 
        '''                               ABS_MainFilter, ABS_WorkReagent, SubstrateDepletion, rKinetics, KineticsInitialValue, KineticsSlope 
        '''                               and KineticsLinear are not updated (to avoid the losing of these values)</param>
        ''' <param name="pMultipointCalib">Optional parameter with default value FALSE. When it is informed as TRUE, fields SubstrateDepletion, 
        '''                                rKinetics, KineticsInitialValue, KineticsSlope and KineticsLinear are updated only for the last Replicate
        '''                                to avoid loss the values for the previous Replicates</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 01/03/2010
        ''' Modified by: SA 03/12/2014 - BA-1616 ==> Added new optional parameters pRecalculusFlag and pMultipointCalib. They are used as 
        '''                                          parameters when calling function SaveExecutionsResults
        ''' </remarks>
        Public Function SaveExecutionsResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS, _
                                              Optional ByVal pRecalculusFlag As Boolean = False, _
                                              Optional ByVal pMultipointCalib As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'BA-1616: Inform parameters pRecalculusFlag and pMultipointCalib when call function SaveExecutionsResults
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.SaveExecutionsResults(dbConnection, pExecutionsDS, pRecalculusFlag, pMultipointCalib)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.SaveExecutionsResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update executions table with the PreparationID fields (if informed) and the ExecutionStatus field (if informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by:  AG 20/04/2010 
        ''' Modified by: SA 10/05/2010 - Changed OpenDBConnection by OpenDBTransaction due to this method is an Update
        '''              SA 06/07/2012 - Updation is executed only if all Executions in the DS have field Status informed (this logic has 
        '''                              been moved here from the DAO function, but I do not known if it is possible have NULL status in the DS!!)
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'Verify all Executions have the Status informed before calling the function to update the status
                Dim resLinq As List(Of ExecutionsDS.twksWSExecutionsRow) = (From a In pExecutionsDS.twksWSExecutions _
                                                                           Where a.IsExecutionStatusNull Select a).ToList

                If (resLinq.Count = 0) Then
                    resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                        If (Not dbConnection Is Nothing) Then
                            Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                            resultData = mytwksWSExecutionsDAO.UpdateStatus(dbConnection, pExecutionsDS)

                            If (Not resultData.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update executions table with the PreparationID fields (if informed) and the ExecutionStatus field (if informed)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewExecutionStatus" ></param>
        ''' <param name="pExecutionID" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by:  DL 05/01/2011
        ''' AG 31/03/2011 - add pWorkSessionID and pAnalyzerID parameters
        ''' </remarks>
        Public Function UpdateStatusByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                  ByVal pNewExecutionStatus As String, _
                                                  ByVal pExecutionID As Integer, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.UpdateStatusByExecutionID(dbConnection, pNewExecutionStatus, pExecutionID, pWorkSessionID, pAnalyzerID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateStatusByExecutionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Update executions by OrderTest (change on status by new status)
        ''' (when pPreparationIDLocked informed) also lock the inprocess ordertest executions with PreparationID > pPreparationIDLocked) - AG 31/01/2012
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <param name="pNewExecutionStatus"></param> 
        ''' <param name="pOrderTestID"></param> 
        ''' <param name="pExecutionStatus"></param> 
        ''' <param name="pLockPreparationIDHigherThanThis"></param>
        ''' <param name="pLockMultiItemWithReplicateNumber"></param>
        ''' <remarks>
        ''' Created by:  dl 05/01/2011
        ''' AG 06/04/2011 use the proper template
        ''' AG 31/01/2012 - new optional parameter used for lock process (pLockPreparationIDHigherThanThis)
        ''' AG 03/10/2012 - new optional parameter used for lock process (pLockMultiItemWithReplicateNumber) used only in multiitem calibrators
        ''' </remarks>
        Public Function UpdateStatusByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                ByVal pNewExecutionStatus As String, _
                                                ByVal pOrderTestID As Integer, _
                                                ByVal pExecutionStatus As String, _
                                                Optional ByVal pLockPreparationIDHigherThanThis As Integer = -1, _
                                                Optional ByVal pLockMultiItemWithReplicateNumber As Integer = -1) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        'If pPreparationID <> -1 and newexecutionstatus is LOCKED pass this parameters to DAO method, else do not
                        Dim localPrepID As Integer = -1
                        If pLockPreparationIDHigherThanThis <> -1 AndAlso pNewExecutionStatus = "LOCKED" Then
                            localPrepID = pLockPreparationIDHigherThanThis
                        End If

                        'AG 03/10/2012
                        Dim localReplicateNumber As Integer = -1
                        If pLockMultiItemWithReplicateNumber <> -1 AndAlso pNewExecutionStatus = "LOCKED" Then
                            localReplicateNumber = pLockMultiItemWithReplicateNumber
                            localPrepID = -1
                        End If
                        'AG 03/10/2012

                        resultData = myDAO.UpdateStatusByOrderTest(dbConnection, pNewExecutionStatus, pOrderTestID, pExecutionStatus, localPrepID, localReplicateNumber)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateStatusByOrderTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' Modified by: TR 25/07/2011 - Once the Executions have been deleted, remove also all paused OrderTests
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)

                        If (Not resultData.HasError) Then
                            'Remove all paused Order Tests
                            Dim myWSPausedOrderTestsDelegate As New WSPausedOrderTestsDelegate
                            resultData = myWSPausedOrderTestsDelegate.DeleteByAnalyzerIDAndWorkSessionID(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Only for biochemistry tests!!!
        ''' Calculates the Contamination Number of an Executions group
        ''' LOW Contaminations (system liquid) -> Persistance contamination cycles: 1
        ''' HIHG Contaminations (wash bottle) -> Persistance contamination cycles: 2
        ''' </summary>
        ''' <param name="pContaminationsDS" ></param>
        ''' <param name="pExecutions">List of Order Tests</param>
        ''' <param name="pHighContaminationPersistance" ></param>
        ''' <returns>
        ''' Returns the Contamination Number
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH - 10/06/2010
        ''' AG 16/09/2011 - Add clause (AndAlso pExecutions(myIndex).ExecutionStatus = "PENDING" AndAlso AndAlso pExecutions(myIndex + 1).ExecutionStatus = "PENDING")
        '''                 due the pending and locked executions are grouped by sample
        ''' AG 25/11/2011 - add the high contamination persistance functionality
        ''' AG 15/12/2011 - define as public to use it in SearchNextPreparation process
        ''' </remarks>
        Public Shared Function GetContaminationNumber(ByVal pContaminationsDS As ContaminationsDS, _
                                                ByVal pExecutions As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow), _
                                                Optional ByVal pHighContaminationPersistance As Integer = 0) As Integer

            Dim ContaminationNumber As Integer = 0
            Dim myIndex As Integer

            For i As Integer = 0 To pExecutions.Count - 2
                myIndex = i

                'AG 16/09/2011
                'Dim contaminations = (From wse In pContaminationsDS.tparContaminations _
                '                      Where wse.ReagentContaminatorID = pExecutions(myIndex).ReagentID _
                '                        AndAlso wse.ReagentContaminatedID = pExecutions(myIndex + 1).ReagentID _
                '                      Select wse).ToList()

                'Search for contamination (LOW or HIGH level contamination)
                Dim contaminations = (From wse In pContaminationsDS.tparContaminations _
                                      Where wse.ReagentContaminatorID = pExecutions(myIndex).ReagentID _
                                        AndAlso wse.ReagentContaminatedID = pExecutions(myIndex + 1).ReagentID _
                                        AndAlso pExecutions(myIndex).ExecutionStatus = "PENDING" _
                                        AndAlso pExecutions(myIndex).ExecutionType = "PREP_STD" _
                                        AndAlso pExecutions(myIndex + 1).ExecutionStatus = "PENDING" _
                                        AndAlso pExecutions(myIndex + 1).ExecutionType = "PREP_STD" _
                                      Select wse).ToList()
                'AG 16/09/2011

                'If contaminations.Count > 0 Then ContaminationNumber += 1
                If contaminations.Count > 0 Then
                    ContaminationNumber += 1
                ElseIf pHighContaminationPersistance > 0 Then
                    'If not low contaminations exists then evaluate if a high contamination exits (only if optional parameter informed)
                    'Search for contamination (only HIGH level contamination)
                    For highIndex As Integer = pHighContaminationPersistance - 1 To 1
                        Dim auxHighIndex = highIndex
                        If (myIndex - auxHighIndex) >= 0 Then 'Avoid overflow
                            contaminations = (From wse In pContaminationsDS.tparContaminations _
                                              Where wse.ReagentContaminatorID = pExecutions(myIndex - auxHighIndex).ReagentID _
                                              AndAlso wse.ReagentContaminatedID = pExecutions(myIndex + 1).ReagentID _
                                              AndAlso Not wse.IsWashingSolutionR1Null _
                                              AndAlso pExecutions(myIndex - auxHighIndex).ExecutionStatus = "PENDING" _
                                              AndAlso pExecutions(myIndex - auxHighIndex).ExecutionType = "PREP_STD" _
                                              AndAlso pExecutions(myIndex + 1).ExecutionStatus = "PENDING" _
                                              AndAlso pExecutions(myIndex + 1).ExecutionType = "PREP_STD" _
                                              Select wse).ToList()
                            If contaminations.Count > 0 Then
                                ContaminationNumber += 1
                                Exit For
                            End If
                        End If
                    Next

                End If

            Next i

            Return ContaminationNumber
        End Function

        ''' <summary>
        ''' Gets the list of Execution's Element groups sorted by ReadingCycle.
        ''' </summary>
        ''' <param name="pExecutions">Dataset with structure of view vwksWSExecutions</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the sorted data (view vwksWSExecutions)
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH - 08/06/2010
        ''' </remarks>
        Public Function SortWSExecutionsByElementGroupTime(ByVal pExecutions As ExecutionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim returnDS As New ExecutionsDS

            Try
                Dim qOrders As List(Of ExecutionsDS.twksWSExecutionsRow)
                Dim Index = 0

                While Index < pExecutions.twksWSExecutions.Rows.Count
                    Dim StatFlag = pExecutions.twksWSExecutions(Index).StatFlag
                    Dim SampleClass = pExecutions.twksWSExecutions(Index).SampleClass

                    qOrders = (From wse In pExecutions.twksWSExecutions _
                           Where wse.StatFlag = StatFlag AndAlso wse.SampleClass = SampleClass _
                           Select wse).ToList()

                    Index += qOrders.Count

                    If SampleClass <> "PATIENT" Then
                        SortByExecutionTime(qOrders, returnDS)
                    Else
                        'When SampleClass = 'PATIENT' do not sort
                        For Each wse In qOrders
                            returnDS.twksWSExecutions.ImportRow(wse)
                        Next
                    End If
                End While

                qOrders = Nothing 'AG 19/02/2014 - #1514
                resultData.SetDatos = returnDS

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.SortWSExecutionsByElementGroupTime", EventLogEntryType.Error, False)

            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests Results from the Executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not.
        '''          If succeed, returns an ExecutionsDS dataset with the results (view vwksWSExecutionsResults)</returns>
        ''' <remarks>
        ''' Created by:  RH 15/07/2010
        ''' Modified by: AG 14/06/2013 - In order to not change the view we use another query + linq to inform the barcode values
        '''              SA 19/06/2013 - When field SpecimenIDList has to be informed for an Execution, if it contains more than one value (there are 
        '''                              several tubes for the same Patient), the CarriageReturn used as list separator has to be replaced by a comma
        ''' </remarks>
        Public Function GetWSExecutionsResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)

                        'AG 14/06/2013
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim executionDataDS As New ExecutionsDS
                            executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            Dim reqElementsDlg As New WSRequiredElementsDelegate
                            resultData = reqElementsDlg.GetLISPatientElements(dbConnection, pWorkSessionID)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim reqElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                Dim linqRes As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
                                Dim patsampleID As String = String.Empty
                                For Each row As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElementsDS.twksWSRequiredElements
                                    'Apply next business only for the patient / sample ID with barcode informed
                                    If Not row.IsSpecimenIDListNull AndAlso row.SpecimenIDList <> String.Empty Then
                                        If Not row.IsPatientIDNull Then
                                            patsampleID = row.PatientID
                                        ElseIf Not row.IsSampleIDNull Then
                                            patsampleID = row.SampleID
                                        Else
                                            patsampleID = String.Empty
                                        End If

                                        If patsampleID <> String.Empty Then
                                            linqRes = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In executionDataDS.vwksWSExecutionsResults _
                                                       Where a.SampleType = row.SampleType _
                                                       AndAlso (Not a.IsPatientIDNull AndAlso a.PatientID = patsampleID) _
                                                       Select a).ToList

                                            If linqRes.Count > 0 Then
                                                For Each exrow As ExecutionsDS.vwksWSExecutionsResultsRow In linqRes
                                                    exrow.BeginEdit()
                                                    If exrow.IsSpecimenIDListNull Then
                                                        'In case the field contains more than one SpecimenID, replace the Carriage Return by a comma
                                                        exrow.SpecimenIDList = row.SpecimenIDList.Replace(CChar(vbCrLf), ", ")
                                                    Else
                                                        exrow.SpecimenIDList &= ", " & row.SpecimenIDList
                                                    End If
                                                    exrow.EndEdit()
                                                Next
                                                executionDataDS.AcceptChanges()
                                            End If
                                        End If
                                    End If
                                Next
                                linqRes = Nothing
                            End If
                            resultData.SetDatos = executionDataDS
                        End If
                        'AG 14/06/2013
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetWSExecutionsResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 25/09/2013 - CF v2.1.1 - Original code from GetWSExecutionsResults function. Modified to filter by a list of OrderIDs
        ''' Get the list of Order Tests Results from the Executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not.
        '''          If succeed, returns an ExecutionsDS dataset with the results (view vwksWSExecutionsResults)</returns>
        ''' <remarks>
        ''' Created by:  RH 15/07/2010
        ''' Modified by: AG 14/06/2013 - In order to not change the view we use another query + linq to inform the barcode values
        '''              SA 19/06/2013 - When field SpecimenIDList has to be informed for an Execution, if it contains more than one value (there are 
        '''                              several tubes for the same Patient), the CarriageReturn used as list separator has to be replaced by a comma
        ''' </remarks>
        Public Function GetWSExecutionsResultsByOrderIDList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pWorkSessionID As String, ByVal pOrderList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetWSExecutionsResultsByOrderIDList(dbConnection, pAnalyzerID, pWorkSessionID, pOrderList)

                        'AG 14/06/2013
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim executionDataDS As New ExecutionsDS
                            executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            Dim reqElementsDlg As New WSRequiredElementsDelegate
                            resultData = reqElementsDlg.GetLISPatientElements(dbConnection, pWorkSessionID)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim reqElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                Dim linqRes As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
                                Dim patsampleID As String = String.Empty
                                For Each row As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElementsDS.twksWSRequiredElements
                                    'Apply next business only for the patient / sample ID with barcode informed
                                    If Not row.IsSpecimenIDListNull AndAlso row.SpecimenIDList <> String.Empty Then
                                        If Not row.IsPatientIDNull Then
                                            patsampleID = row.PatientID
                                        ElseIf Not row.IsSampleIDNull Then
                                            patsampleID = row.SampleID
                                        Else
                                            patsampleID = String.Empty
                                        End If

                                        If patsampleID <> String.Empty Then
                                            linqRes = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In executionDataDS.vwksWSExecutionsResults _
                                                       Where a.SampleType = row.SampleType _
                                                       AndAlso (Not a.IsPatientIDNull AndAlso a.PatientID = patsampleID) _
                                                       Select a).ToList

                                            If linqRes.Count > 0 Then
                                                For Each exrow As ExecutionsDS.vwksWSExecutionsResultsRow In linqRes
                                                    exrow.BeginEdit()
                                                    If exrow.IsSpecimenIDListNull Then
                                                        'In case the field contains more than one SpecimenID, replace the Carriage Return by a comma
                                                        exrow.SpecimenIDList = row.SpecimenIDList.Replace(CChar(vbCrLf), ", ")
                                                    Else
                                                        exrow.SpecimenIDList &= ", " & row.SpecimenIDList
                                                    End If
                                                    exrow.EndEdit()
                                                Next
                                                executionDataDS.AcceptChanges()
                                            End If
                                        End If
                                    End If
                                Next
                                linqRes = Nothing
                            End If
                            resultData.SetDatos = executionDataDS
                        End If
                        'AG 14/06/2013
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetWSExecutionsResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
        ''' <summary>
        ''' Get all the Execution Result Alarms (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the Execution Result Alarms (view vwksWSExecutionsAlarms)
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH - 19/07/2010
        ''' </remarks>
        Public Function GetWSExecutionResultAlarms(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetWSExecutionResultAlarms(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetWSExecutionResultAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Executions with data to be monitored
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the results (view vwksWSExecutionsMonitor)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 30/11/2010
        ''' Modified by: AG 14/06/2013 - In order to not change the view we use another query + linq to inform the barcode values
        '''              SA 19/06/2013 - When field SpecimenIDList has to be informed for an Execution, if it contains more than one value (there are 
        '''                              several tubes for the same Patient), the Carriage Return used as list separator has to be replaced by a comma 
        ''' </remarks> 
        Public Function GetWSExecutionsMonitor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetWSExecutionsMonitor(dbConnection, pAnalyzerID, pWorkSessionID)

                        'AG 14/06/2013
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim executionDataDS As New ExecutionsDS
                            executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            Dim reqElementsDlg As New WSRequiredElementsDelegate
                            resultData = reqElementsDlg.GetLISPatientElements(dbConnection, pWorkSessionID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim reqElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                Dim linqRes As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)
                                Dim patsampleID As String = String.Empty
                                For Each row As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElementsDS.twksWSRequiredElements
                                    'Apply next business only for the patient / sample ID with barcode informed
                                    If Not row.IsSpecimenIDListNull AndAlso row.SpecimenIDList <> String.Empty Then
                                        If Not row.IsPatientIDNull Then
                                            patsampleID = row.PatientID
                                        ElseIf Not row.IsSampleIDNull Then
                                            patsampleID = row.SampleID
                                        Else
                                            patsampleID = String.Empty
                                        End If

                                        If patsampleID <> String.Empty Then
                                            linqRes = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In executionDataDS.vwksWSExecutionsMonitor _
                                                       Where a.SampleType = row.SampleType _
                                                       AndAlso ((Not a.IsSampleIDNull AndAlso a.SampleID = patsampleID) _
                                                       OrElse (Not a.IsPatientIDNull AndAlso a.PatientID = patsampleID)) _
                                                       Select a).ToList

                                            If linqRes.Count > 0 Then
                                                For Each exrow As ExecutionsDS.vwksWSExecutionsMonitorRow In linqRes
                                                    exrow.BeginEdit()
                                                    If (exrow.IsSpecimenIDListNull) Then
                                                        'In case the field contains more than one SpecimenID, replace the Carriage Return by a comma
                                                        exrow.SpecimenIDList = row.SpecimenIDList.Replace(CChar(vbCrLf), ", ")
                                                    Else
                                                        exrow.SpecimenIDList &= ", " & row.SpecimenIDList
                                                    End If
                                                    exrow.EndEdit()
                                                Next
                                                executionDataDS.AcceptChanges()
                                            End If
                                        End If
                                    End If
                                Next
                                linqRes = Nothing
                            End If
                            resultData.SetDatos = executionDataDS
                        End If
                        'AG 14/06/2013
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetWSExecutionsMonitor", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Paused field into the twksWSExecutions table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by: RH 09/12/2010
        ''' Modified by: TR 24/07/2011 -When an element is marked as paused insert record on table twksWSPausedOrderTests and 
        '''                             when the element is unmarked then removed from table twksWSPausedOrderTests.
        '''             RH 03/05/2012 Process all rows in pExecutionsDS. Assumes all rows in it should be updated.
        ''' </remarks>
        Public Function UpdatePaused(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        'TR 25/07/2011
                        Dim myPausedOrderTestsDS As New WSPausedOrderTestsDS
                        Dim myPausedOrderTestsRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow
                        Dim myPausedOrderTestsDelegate As New WSPausedOrderTestsDelegate
                        'TR 25/07/2011 -END 

                        For Each row As ExecutionsDS.vwksWSExecutionsMonitorRow In pExecutionsDS.vwksWSExecutionsMonitor
                            resultData = myDAO.UpdatePaused(dbConnection, row.Paused, row.ExecutionID)

                            If resultData.HasError Then Exit For

                            ''TR 25/07/2011
                            If row.Paused Then
                                'Validate if element exist. in twksWSPausedOrderTests
                                resultData = myPausedOrderTestsDelegate.Read(dbConnection, row.AnalyzerID, row.WorkSessionID, _
                                                                                                 row.OrderTestID, row.RerunNumber)
                                If resultData.HasError Then Exit For

                                If Not resultData.SetDatos Is Nothing Then
                                    If DirectCast(resultData.SetDatos, WSPausedOrderTestsDS).twksWSPausedOrderTests.Count = 0 Then
                                        'Create new row if ok. in twksWSPausedOrderTests
                                        myPausedOrderTestsRow = myPausedOrderTestsDS.twksWSPausedOrderTests.NewtwksWSPausedOrderTestsRow()
                                        myPausedOrderTestsRow.AnalyzerID = row.AnalyzerID
                                        myPausedOrderTestsRow.WorkSessionID = row.WorkSessionID
                                        myPausedOrderTestsRow.OrderTestID = row.OrderTestID
                                        myPausedOrderTestsRow.RerunNumber = row.RerunNumber
                                        myPausedOrderTestsDS.twksWSPausedOrderTests.AddtwksWSPausedOrderTestsRow(myPausedOrderTestsRow)
                                        resultData = myPausedOrderTestsDelegate.Create(dbConnection, myPausedOrderTestsDS)
                                    End If
                                End If

                            Else
                                'Delete row from table twksWSPausedOrderTests
                                resultData = myPausedOrderTestsDelegate.Delete(dbConnection, row.AnalyzerID, row.WorkSessionID, _
                                                                                                 row.OrderTestID, row.RerunNumber)
                            End If

                            If resultData.HasError Then Exit For

                            'Clear dataset
                            myPausedOrderTestsDS.twksWSPausedOrderTests.Clear()
                            'TR 25/07/2011 -END
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdatePaused", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Updates the ThermoWarningFlag field into the twksWSExecutions table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 22/03/2011</remarks>
        Public Function UpdateThermoWarningFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions
                            resultData = myDAO.UpdateThermoWarningFlag(dbConnection, row.ThermoWarningFlag, row.ExecutionID)
                            If resultData.HasError Then Exit For
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateThermoWarningFlag", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Updates the ClotValue field into the twksWSExecutions table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 22/03/2011</remarks>
        Public Function UpdateClotValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions
                            If Not row.IsExecutionIDNull Then
                                resultData = myDAO.UpdateClotValue(dbConnection, row.ClotValue, row.ExecutionID)
                                If resultData.HasError Then Exit For
                            End If
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateClotValue", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get information for refresh UI after a Execution changes his status
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>GlobalDataTO with Data as UIRefreshDS (ExecutionStatusChanged)</returns>
        ''' <remarks></remarks>
        Public Function GetExecutionStatusChangeInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetExecutionOrderTestOrderStatus(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecutionStatusChangeInfo", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the PENDING executions and calculate the next ISE test for send
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>Integer inside a GlobalDataTO</returns>
        ''' <remarks>AG 18/01/2011</remarks>
        Public Function GetNextPendingISEPatientExecution(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim iseExecutionID As Integer = GlobalConstants.NO_PENDING_PREPARATION_FOUND

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim myDAO As New twksWSExecutionsDAO

                        'AG 30/11/2011 - get all pending executions (std and ise)
                        'resultData = myDAO.GetPendingPatientExecutions(dbConnection, pAnalyzerID, pWorkSessionID)
                        resultData = myDAO.GetPendingExecutionForSendNextProcess(dbConnection, pAnalyzerID, pWorkSessionID, False)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then '(3)
                            Dim myExDS As New ExecutionsDS
                            myExDS = CType(resultData.SetDatos, ExecutionsDS)
                            If myExDS.twksWSExecutions.Rows.Count > 0 Then '(4)

                                'Check if there are some PREP_ISE pending
                                Dim iseResLinq As List(Of ExecutionsDS.twksWSExecutionsRow) = _
                                (From a In myExDS.twksWSExecutions Where a.ExecutionType = "PREP_ISE" _
                                 Select a).ToList

                                If iseResLinq.Count > 0 Then
                                    iseExecutionID = iseResLinq(0).ExecutionID

                                    'AG 30/11/2011 - ISE tests are the first preparations when a patient starts but they has lower priority over blanks, calibrator and controls
                                    If iseExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myStatFlag As Boolean = False
                                        If Not iseResLinq(0).IsStatFlagNull Then myStatFlag = iseResLinq(0).StatFlag

                                        'Search if exists some BLANK or CALIBRATOR or CONTROL pending execution with the same or higher Stat level and with executionID < iseExecutionID
                                        'if found then iseExecutionID has to wait, it is not his moment. Assign iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                        Dim noPatientPreviousExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)
                                        If myStatFlag Then
                                            noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions Where a.StatFlag = myStatFlag AndAlso a.SampleClass <> "PATIENT" _
                                                                           AndAlso a.ExecutionID < iseExecutionID Select a).ToList
                                        Else
                                            noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions Where a.SampleClass <> "PATIENT" _
                                                                           AndAlso a.ExecutionID < iseExecutionID Select a).ToList
                                        End If

                                        If noPatientPreviousExecutions.Count > 0 Then
                                            iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                        End If
                                        noPatientPreviousExecutions = Nothing
                                    End If
                                    'AG 30/11/2011

                                    'If the first PATIENT execution in myExDS has the same OrderID OR SampleType that the found ISE execution ... send the ISE
                                    'otherwise Sw has to finish the previous patient - sample type before send ISE
                                    If iseExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myOrderID As String = ""
                                        Dim mySampleType As String = ""

                                        If Not iseResLinq(0).IsOrderIDNull AndAlso Not iseResLinq(0).IsSampleTypeNull Then
                                            myOrderID = iseResLinq(0).OrderID
                                            mySampleType = iseResLinq(0).SampleType
                                        End If

                                        Dim patientResLinq As List(Of ExecutionsDS.twksWSExecutionsRow) = (From a In myExDS.twksWSExecutions Where a.SampleClass = "PATIENT" _
                                                                                                          Select a).ToList
                                        If patientResLinq.Count > 0 Then
                                            If patientResLinq(0).ExecutionID <> iseExecutionID Then
                                                If Not patientResLinq(0).IsOrderIDNull AndAlso Not patientResLinq(0).IsSampleTypeNull Then

                                                    If patientResLinq(0).OrderID <> myOrderID OrElse patientResLinq(0).SampleType <> mySampleType Then
                                                        iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                                    End If 'If patientResLinq(0).OrderID <> myOrderID OrElse patientResLinq(0).SampleType <> mySampleType Then

                                                End If 'If Not patientResLinq(0).IsOrderIDNull AndAlso Not patientResLinq(0).IsSampleTypeNull Then
                                            End If 'If patientResLinq(0).ExecutionID <> iseExecutionID Then
                                        End If
                                        patientResLinq = Nothing
                                    End If


                                End If 'If iseResLinq.Count > 0 Then
                                iseResLinq = Nothing

                            End If 'If myExDS.twksWSExecutions.Rows.Count > 0 Then '(4)
                        End If 'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then '(3)

                    End If 'If (Not dbConnection Is Nothing) Then '(2)
                End If 'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)

                resultData.SetDatos = iseExecutionID

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetNextPendingISEPatientExecution", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the PENDING executions and calculate the next ISE test for send
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>Integer inside a GlobalDataTO</returns>
        ''' <remarks>AG 18/01/2011
        ''' AG 11/07/2012 - adapted for control ise (copied and adapted from GetNextPendingISEPatientExecution)</remarks>
        Public Function GetNextPendingISEExecutionNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim iseExecutionID As Integer = GlobalConstants.NO_PENDING_PREPARATION_FOUND
            Dim returnDS As New ExecutionsDS

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim myDAO As New twksWSExecutionsDAO

                        'AG 30/11/2011 - get all pending executions (std and ise)
                        'resultData = myDAO.GetPendingPatientExecutions(dbConnection, pAnalyzerID, pWorkSessionID)
                        resultData = myDAO.GetPendingExecutionForSendNextProcess(dbConnection, pAnalyzerID, pWorkSessionID, False)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then '(3)
                            Dim myExDS As New ExecutionsDS
                            myExDS = CType(resultData.SetDatos, ExecutionsDS)
                            If myExDS.twksWSExecutions.Rows.Count > 0 Then '(4)

                                'Check if there are some PREP_ISE pending
                                Dim iseResLinq As List(Of ExecutionsDS.twksWSExecutionsRow) = _
                                (From a In myExDS.twksWSExecutions Where a.ExecutionType = "PREP_ISE" _
                                 Select a).ToList

                                If iseResLinq.Count > 0 Then
                                    iseExecutionID = iseResLinq(0).ExecutionID
                                    returnDS.twksWSExecutions.ImportRow(iseResLinq(0))

                                    Dim mySampleClass As String = "CTRL"

                                    'AG 30/11/2011 - ISE tests are the first preparations when a patient starts but they has lower priority over blanks, calibrator and controls
                                    If iseExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myStatFlag As Boolean = False
                                        If Not iseResLinq(0).IsStatFlagNull Then myStatFlag = iseResLinq(0).StatFlag
                                        If Not iseResLinq(0).IsSampleClassNull Then mySampleClass = iseResLinq(0).SampleClass

                                        'Search if exists some BLANK or CALIBRATOR pending execution with the same or higher Stat level and with executionID < iseExecutionID
                                        'if found then iseExecutionID has to wait, it is not his moment. Assign iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                        Dim noPatientPreviousExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)
                                        If myStatFlag Then
                                            If mySampleClass = "CTRL" Then
                                                noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions Where a.StatFlag = myStatFlag AndAlso a.SampleClass <> "CTRL" _
                                                                               AndAlso a.SampleClass <> "PATIENT" _
                                                                               AndAlso a.ExecutionID < iseExecutionID Select a).ToList
                                            Else 'Patient
                                                noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions Where a.StatFlag = myStatFlag AndAlso a.SampleClass <> "PATIENT" _
                                                                               AndAlso a.ExecutionID < iseExecutionID Select a).ToList
                                            End If

                                        Else
                                            If mySampleClass = "CTRL" Then
                                                noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions Where a.SampleClass <> "CTRL" _
                                                                               AndAlso a.SampleClass <> "PATIENT" _
                                                                               AndAlso a.ExecutionID < iseExecutionID Select a).ToList

                                            Else 'Patient
                                                noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions Where a.SampleClass <> "PATIENT" _
                                                                               AndAlso a.ExecutionID < iseExecutionID Select a).ToList
                                            End If
                                        End If

                                        If noPatientPreviousExecutions.Count > 0 Then
                                            iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                        End If
                                        noPatientPreviousExecutions = Nothing
                                    End If
                                    'AG 30/11/2011

                                    'If the first PATIENT execution in myExDS has the same OrderID OR SampleType that the found ISE execution ... send the ISE
                                    'otherwise Sw has to finish the previous patient - sample type before send ISE
                                    If iseExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myOrderID As String = ""
                                        Dim mySampleType As String = ""

                                        If Not iseResLinq(0).IsOrderIDNull AndAlso Not iseResLinq(0).IsSampleTypeNull Then
                                            myOrderID = iseResLinq(0).OrderID
                                            mySampleType = iseResLinq(0).SampleType
                                        End If

                                        Dim ctrlPatientResLinq As List(Of ExecutionsDS.twksWSExecutionsRow) = (From a In myExDS.twksWSExecutions Where a.SampleClass = mySampleClass _
                                                                                                               Select a).ToList
                                        If ctrlPatientResLinq.Count > 0 Then
                                            If ctrlPatientResLinq(0).ExecutionID <> iseExecutionID Then
                                                If Not ctrlPatientResLinq(0).IsOrderIDNull AndAlso Not ctrlPatientResLinq(0).IsSampleTypeNull Then

                                                    If ctrlPatientResLinq(0).OrderID <> myOrderID OrElse ctrlPatientResLinq(0).SampleType <> mySampleType Then
                                                        iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                                    End If 'If patientResLinq(0).OrderID <> myOrderID OrElse patientResLinq(0).SampleType <> mySampleType Then

                                                End If 'If Not patientResLinq(0).IsOrderIDNull AndAlso Not patientResLinq(0).IsSampleTypeNull Then
                                            End If 'If patientResLinq(0).ExecutionID <> iseExecutionID Then
                                        End If
                                        ctrlPatientResLinq = Nothing
                                    End If


                                End If 'If iseResLinq.Count > 0 Then
                                iseResLinq = Nothing

                            End If 'If myExDS.twksWSExecutions.Rows.Count > 0 Then '(4)
                        End If 'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then '(3)

                    End If 'If (Not dbConnection Is Nothing) Then '(2)
                End If 'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)

                If iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                    returnDS.Clear()
                End If
                resultData.SetDatos = returnDS

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetNextPendingISEExecutionNEW", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the PENDING executions and calculate the next ISE test for send
        ''' Based on previous GetNextPendingISEPatientExecution()
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>An ExecutionsDS inside a GlobalDataTO with the data found</returns>
        ''' <remarks>
        ''' Created by: RH 26/06/2012
        ''' AG 11/07/2012 create a new versionGetNextPendingISEExecutionNEW 
        ''' </remarks>
        Public Function GetNextPendingISEExecution(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                   ByVal pAnalyzerID As String, _
                                                   ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim iseExecutionID As Integer = GlobalConstants.NO_PENDING_PREPARATION_FOUND
            Dim returnDS As New ExecutionsDS

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim myDAO As New twksWSExecutionsDAO

                        'AG 30/11/2011 - get all pending executions (std and ise)
                        'resultData = myDAO.GetPendingPatientExecutions(dbConnection, pAnalyzerID, pWorkSessionID)
                        resultData = myDAO.GetPendingExecutionForSendNextProcess(dbConnection, pAnalyzerID, pWorkSessionID, False)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then '(3)
                            Dim myExDS As ExecutionsDS
                            myExDS = CType(resultData.SetDatos, ExecutionsDS)

                            If myExDS.twksWSExecutions.Rows.Count > 0 Then '(4)

                                'Check if there are some PREP_ISE pending
                                Dim iseResLinq As List(Of ExecutionsDS.twksWSExecutionsRow)
                                iseResLinq = (From a In myExDS.twksWSExecutions _
                                              Where a.ExecutionType = "PREP_ISE" _
                                              Select a).ToList()

                                If iseResLinq.Count > 0 Then
                                    iseExecutionID = iseResLinq(0).ExecutionID
                                    returnDS.twksWSExecutions.ImportRow(iseResLinq(0))

                                    'AG 30/11/2011 - ISE tests are the first preparations when a patient starts but they has lower priority over blanks and calibrator
                                    If iseExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myStatFlag As Boolean = False

                                        If Not iseResLinq(0).IsStatFlagNull Then myStatFlag = iseResLinq(0).StatFlag

                                        'Search if exists some BLANK or CALIBRATOR pending execution with the same or higher Stat level and with executionID < iseExecutionID
                                        'if found then iseExecutionID has to wait, it is not the moment to go. Assign iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                        Dim noPatientPreviousExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)

                                        If myStatFlag Then
                                            noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions _
                                                                           Where a.StatFlag = myStatFlag _
                                                                           AndAlso a.SampleClass <> "PATIENT" _
                                                                           AndAlso a.SampleClass <> "CTRL" _
                                                                           AndAlso a.ExecutionID < iseExecutionID _
                                                                           Select a).ToList()
                                        Else
                                            noPatientPreviousExecutions = (From a In myExDS.twksWSExecutions _
                                                                           Where a.SampleClass <> "PATIENT" _
                                                                           AndAlso a.SampleClass <> "CTRL" _
                                                                           AndAlso a.ExecutionID < iseExecutionID _
                                                                           Select a).ToList()
                                        End If

                                        If noPatientPreviousExecutions.Count > 0 Then
                                            iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                            returnDS.twksWSExecutions.Clear()
                                        End If

                                        noPatientPreviousExecutions = Nothing
                                    End If
                                    'AG 30/11/2011

                                    'If the first PATIENT or CONTROL execution in myExDS has the same OrderID OR SampleType that the found ISE execution ... send the ISE
                                    'otherwise Sw has to finish the previous patient/control - sample type before send ISE
                                    If iseExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myOrderID As String = ""
                                        Dim mySampleType As String = ""

                                        If Not iseResLinq(0).IsOrderIDNull AndAlso Not iseResLinq(0).IsSampleTypeNull Then
                                            myOrderID = iseResLinq(0).OrderID
                                            mySampleType = iseResLinq(0).SampleType
                                        End If

                                        Dim patientResLinq As List(Of ExecutionsDS.twksWSExecutionsRow)
                                        patientResLinq = (From a In myExDS.twksWSExecutions _
                                                          Where a.SampleClass = "PATIENT" _
                                                          OrElse a.SampleClass = "CTRL" _
                                                          Select a).ToList()

                                        If patientResLinq.Count > 0 Then
                                            If patientResLinq(0).ExecutionID <> iseExecutionID Then
                                                If Not patientResLinq(0).IsOrderIDNull AndAlso Not patientResLinq(0).IsSampleTypeNull Then

                                                    If patientResLinq(0).OrderID <> myOrderID OrElse patientResLinq(0).SampleType <> mySampleType Then
                                                        iseExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                                        returnDS.twksWSExecutions.Clear()
                                                    End If

                                                End If
                                            End If
                                        End If

                                        patientResLinq = Nothing
                                    End If

                                End If

                                iseResLinq = Nothing

                            End If
                        End If

                    End If
                End If

                resultData.SetDatos = returnDS

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetNextPendingISEExecution", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Returns additonal information for inform the TEST preparation send and required for apply the send preparation algorithm
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>AnalyzerManagerDS.searchNext inside GlobalDataTo</returns>
        ''' <remarks>AG 18/01/2011</remarks>
        Public Function GetSendPreparationDataByExecution(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                          ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetSendPreparationDataByExecution(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetSendPreparationDataByExecution", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the PENDING executions and calculate the next STD or STD and ISE test for send
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pOnlyStdTestFlag"></param>
        ''' <returns>ExecutionsDS.twksWSExecutions inside a GlobalDataTO</returns>
        ''' <remarks>AG 18/01/2011
        ''' AG 30/11/2011 add pOnlyStdTestFlag</remarks>
        Public Function GetPendingExecutionForSendNextProcess(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String, ByVal pOnlyStdTestFlag As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetPendingExecutionForSendNextProcess(dbConnection, pAnalyzerID, pWorkSessionID, pOnlyStdTestFlag)

                    End If 'If (Not dbConnection Is Nothing) Then '(2)
                End If 'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetPendingExecutionForSendNextProcess", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Due to the way of working of ISE Module, all ISE Tests requested for each Patient or Control with the same SampleType will be 
        ''' executed together; that means all ISE Executions having the same OrderID/SampleType and ReplicateNumber will have the same 
        ''' PreparationID and become INPROCESS at the same time.
        ''' This functions returns all affected ISE Executions 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pSampleClass">Sample Class: CTRL or PATIENT</param>
        ''' <param name="pOnlyPending">Optional parameter; when value is TRUE, only PENDING ISE Executions are returned</param>
        ''' <returns>GlobalDataTO containing a typed DS ExecutionsDS with all affected ISE Executions</returns>
        ''' <remarks>
        ''' Created by:  AG 03/02/2011
        ''' Modified by: SA 02/07/2012 - Added optional parameter pOnlyPending; when its value is TRUE, it means the function
        '''                              has been called from the process of mark Preparations as accepted, and in this case, 
        '''                              only PENDING ISE Executions are returned
        '''              SA 10/07/2012 - Added optional parameter for the SampleClass (CTRL or PATIENT): For Patients, all ISE Executions using the same 
        '''                              SampleType tube are obtained, while for Controls, all ISE Executions using the same Control are obtained.
        ''' </remarks>
        Public Function GetAffectedISEExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                 ByVal pExecutionID As Integer, Optional ByVal pSampleClass As String = "", _
                                                 Optional ByVal pOnlyPending As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        If (pSampleClass = String.Empty) Then
                            'If the parameter is not informed, search the SampleClass of the informed Execution
                            resultData = myDAO.GetExecution(dbConnection, pExecutionID, pAnalyzerID, pWorkSessionID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                pSampleClass = DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions.First.SampleClass
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            resultData = myDAO.GetAffectedISEExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, pSampleClass, pOnlyPending)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetAffectedISEExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get if the ExecutionID has programmed contamination cuvette
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>AnalyzerManagerDS.searchNext inside a GlobalDataTO</returns>
        ''' <remarks>AG 07/02/2011 - Tested OK</remarks>
        Public Function GetExecutionContaminationCuvette(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                      ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetExecutionContaminationCuvette(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID)

                    End If 'If (Not dbConnection Is Nothing) Then '(2)
                End If 'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecutionContaminationCuvette", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Calculate if the current bottle levels are sufficient or not for performed the worksession 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWashSolutionPercentage"></param>
        ''' <param name="pHighContaminationWastePercentage"></param>
        ''' <returns>Global data to with set data as Boolean (TRUE: sufficent, FALSE: no sufficient </returns>
        ''' <remarks>
        ''' AG Creation 04/04/2011 - Not business is performed
        ''' </remarks>
        Public Function CalculateIfBottleLevelsAreSufficientForWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                         ByVal pWorkSessionID As String, ByVal pWashSolutionPercentage As Single, _
                                                         ByVal pHighContaminationWastePercentage As Single) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim sufficientLevel As Boolean = True

                        'Get all PENDING executions for the pAnalyzerid and pWorkSessionID
                        resultData = GetExecutionByWorkSession(dbConnection, pAnalyzerID, pWorkSessionID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim pendingExecutionsNr As Integer = 0
                            Dim myDS As New ExecutionsDS

                            myDS = CType(resultData.SetDatos, ExecutionsDS)
                            If myDS.twksWSExecutions.Rows.Count > 0 Then
                                pendingExecutionsNr = (From a As ExecutionsDS.twksWSExecutionsRow In myDS.twksWSExecutions _
                                                       Where a.ExecutionStatus = "PENDING" Select a).Count
                            End If

                            'TODO 
                            'Calculate if the current Wash Solution percentage is sufficient for this number of executions, contaminations,... - PENDING SPEC
                            'By now I suppose we will calculated it using a linear regression aproximation: N tests <-> M wash solution %

                            'TODO
                            'Calculate if the current High contamination waste percentage is sufficient for this number of executions, contaminations,... - PENDING SPEC
                            'By now I suppose we will calculated it using a linear regression aproximation: N tests <-> P high contamination waste %

                        End If

                        resultData.SetDatos = sufficientLevel
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CalculateIfBottleLevelsAreSufficientForWS", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the executions by execution status
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionStatus"></param>
        ''' <param name="pRejectPausedExecutions" ></param>
        ''' <param name="pExecutionType">When "" alll execution types, when informed only get the informed execution type</param>
        ''' <returns>GlobalDataTo (ExecutionsDS.twksWSExecutions)</returns>
        ''' <remarks>AG 19/04/2011
        ''' AG 19/07/2011 add pRejectPausedExecutions if TRUE modify Where clausule ... And Paused = 0
        ''' AG 23/03/2012 - add optional parameter pExecutionType
        ''' </remarks>
        Public Function GetExecutionsByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                            ByVal pWorkSessionID As String, ByVal pExecutionStatus As String, ByVal pRejectPausedExecutions As Boolean, _
                                            Optional ByVal pExecutionType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetExecutionsByStatus(dbConnection, pWorkSessionID, pAnalyzerID, pExecutionStatus, pRejectPausedExecutions)

                    End If 'If (Not dbConnection Is Nothing) Then '(2)
                End If 'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecutionsByStatus", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Creates the data related to an execution locked due to volume missing alarm 
        ''' (Based on Ax5)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>GlobalDataTo (String)</returns>
        ''' <remarks>
        ''' Created By RH: 03/02/2012 New version without IsFieldNull validation, because every field is String type (Null = Empty)
        '''      This method encodes the additional information due to a Locked execution.
        '''      To decode this information use the method WSAnalyzerAlarmsDelegate.DecodeAdditionalInfo
        ''' modified by: TR 27/03/2014 -Add new funct. to get the washing solution name and values. Mor info see BT#1558.
        ''' </remarks>
        Public Function EncodeAdditionalInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                                       ByVal pWorkSessionID As String, _
                                                                       ByVal pExecutionID As Integer, _
                                                                       ByVal pRotorPosition As Integer, _
                                                                       ByVal pAlarmCode As Alarms, _
                                                                       ByVal pReagentNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetDataForAlarmAdditionalInfo(dbConnection, pAnalyzerID, _
                                                                         pWorkSessionID, pExecutionID, pAlarmCode, pReagentNumber)

                        Dim additionalInfoDS As WSAnalyzerAlarmsDS
                        additionalInfoDS = CType(resultData.SetDatos, WSAnalyzerAlarmsDS)

                        'TR 27/03/2014- BT #1558- Get the correct information when the tube contents is a Washing solution
                        If pExecutionID = -1 Then
                            'Get the info, base on rotor position.
                            Dim myRotorContentByPosDelegate As New WSRotorContentByPositionDelegate
                            resultData = myRotorContentByPosDelegate.ReadByCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "REAGENTS", pRotorPosition)
                            If Not resultData.HasError Then
                                Dim myWSRotorContentByPositionDS As New WSRotorContentByPositionDS
                                myWSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                'Validate if the tube content is Washing Solution to continue the process
                                If myWSRotorContentByPositionDS.twksWSRotorContentByPosition.Count > 0 AndAlso _
                                    myWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeContent = "WASH_SOL" Then
                                    'Complete the required information for the solution
                                    resultData = myRotorContentByPosDelegate.GetPositionInfo(Nothing, myWSRotorContentByPositionDS)
                                    If (Not resultData.HasError) Then
                                        'Get the returned information and update the new values for the additionalinfoDS()
                                        Dim myCellPosInfoDS As CellPositionInformationDS = DirectCast(resultData.SetDatos, CellPositionInformationDS)
                                        additionalInfoDS.AdditionalInfoPrepLocked(0).TestName = myCellPosInfoDS.Reagents(0).ReagentName
                                        additionalInfoDS.AdditionalInfoPrepLocked(0).TubeContent = myWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).TubeContent
                                        If Not myWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).IsSolutionCodeNull Then
                                            additionalInfoDS.AdditionalInfoPrepLocked(0).SolutionCode = myWSRotorContentByPositionDS.twksWSRotorContentByPosition(0).SolutionCode
                                        End If
                                    End If
                                End If
                                'Free not in use elements
                                myWSRotorContentByPositionDS = Nothing
                            End If
                            'Free not in use elements 
                            myRotorContentByPosDelegate = Nothing
                        End If
                        'TR 27/03/2014- BT #1558- END.

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim separator As String = GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR

                            Dim code As String = String.Empty

                            'TO CONFIRM
                            'Ax5 saves: SAMPLECLASS, SAMPLEID, TESTNAME, REPLICATE
                            'Ax00 saves: SAMPLECLASS sep SAMPLE NAME sep TESTNAME sep REPLICATE
                            '            Where SAMPLE NAME depends on sampleclass:
                            '            "BLANK": Sample Name = Test Name
                            '            "CALIB": Sample Name = Calibrator Name
                            '            "CTRL": Sample Name = Control Name
                            '            "PATIENT": Sample Name = PatientID or SampleID



                            If additionalInfoDS.AdditionalInfoPrepLocked.Rows.Count > 0 Then
                                With additionalInfoDS.AdditionalInfoPrepLocked(0)
                                    code = String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}", separator, _
                                                         .SampleClass, .Name, .TestName, .ReplicateNumber, _
                                                         pRotorPosition, pReagentNumber, .NumberOfCalibrators, _
                                                         .MultiItemNumber, .TubeContent, .SolutionCode)
                                End With
                            End If

                            resultData.SetDatos = code
                        End If

                    End If 'If (Not dbConnection Is Nothing) Then '(2)
                End If 'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.EncodeAdditionalInfo", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Get execution information by OrderTestID - MultiItemNumber
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pMultiItemNumber"></param>
        ''' <returns></returns>
        ''' <remarks>AG 13/05/2011</remarks>
        Public Function GetByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                            ByVal pAnalyzerID As String, ByVal pOrderTestID As Integer, Optional ByVal pMultiItemNumber As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetByOrderTest(dbConnection, pWorkSessionID, pAnalyzerID, pOrderTestID, pMultiItemNumber)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetByOrderTest", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Get execution information by ElementID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pElementID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 16/07/2011</remarks>
        Public Function GetByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                    ByVal pAnalyzerID As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetByElementID(dbConnection, pWorkSessionID, pAnalyzerID, pElementID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetByElementID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Count how many executions are linked with the adjustbaselineID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo (setData as integer)</returns>
        ''' <remarks>AG 08/07/2011</remarks>
        Public Function CountExecutionsUsingAdjustBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, _
                             ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim resultCount As Integer = 0
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.GetExecutionsUsingAdjustBaseLine(dbConnection, pAnalyzerID, pWorkSessionID)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myDS As New ExecutionsDS
                            myDS = CType(resultData.SetDatos, ExecutionsDS)
                            resultCount = myDS.twksWSExecutions.Rows.Count
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CountExecutionsUsingAdjustBaseLine", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            resultData.SetDatos = resultCount
            Return resultData
        End Function

        ''' <summary>
        ''' Get the number of Pending, Locked and/or InProcess Executions in the specified Analyzer WorkSession for the informed Reagent 
        ''' (identified with the ID of the required Element in the WorkSession). This function is used to calculate the remaining needed 
        ''' volume of Reagents used in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier for the Reagent in the active WorkSession</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Pending, Locked and/or InProcess Executions in the specified
        '''          Analyzer WorkSession for the informed required Reagent</returns>
        ''' <remarks>
        ''' Created by:  SA 08/02/2012
        ''' </remarks>
        Public Function CountNotClosedSTDExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                    ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.CountNotClosedSTDExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pElementID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CountNotClosedSTDExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        '''  For all Pending and InProcess Executions corresponding to STD Tests for the informed WorkSessionID and AnalyzerID, get following data:
        ''' ** Execution data: ID, Status and OrderTestID
        ''' ** Test data: ID and TestCycles (the maximum value between FirstReadingCycle and SecondReadingCycle)
        ''' ** Readings: the Number of the last Reading the Analyzer has sent for the Execution (zero if not Readings have been sent)
        ''' ** Preparations: the SendingTime of the Preparation containing the Instruction sent to the Analyzer for the Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCycleMachine">Time in seconds for an Analyzer Cycle</param>
        ''' <returns>GlobalDataTO containing a Single value with the maximum remaining time for all Pending and InProcess STD Executions for the 
        '''          informed Analyzer and WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 24/10/2011 
        ''' Modified by: SA 31/05/2012 - Called new function GetMaxReadingNumber to get the last reading cycle for each Standard
        '''                              Preparation, and for each returned execution, search it in the DS containing the data needed
        '''                              for time estimation calculation and inform field LastReadingCycle (change due to bad performance
        '''                              of DAO function GetSTDTestsTimeEstimation)
        ''' </remarks>
        Public Function GetSTDTestsTimeEstimation(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                  ByVal pCycleMachine As Single) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim postR1Cycles As Integer = 0
                        Dim predilutionCycles As Integer = 0

                        Dim myParametersDS As New ParametersDS
                        Dim mySWParametersDelegate As New SwParametersDelegate

                        'Get the number of fixed Cycles the Analyzer needs for automatic predilutions
                        resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, _
                                                                                   GlobalEnumerates.SwParameters.PREDILUTION_CYCLES.ToString, True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                            If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then predilutionCycles = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                        End If

                        If (Not resultData.HasError) Then
                            'Get the number of fixed Analyzer Cycles needed after R1 is dispensed
                            resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, _
                                                                                       GlobalEnumerates.SwParameters.WAITING_CYCLES_AFTER_R1.ToString, True)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                                If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then postR1Cycles = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                            End If
                        End If

                        Dim offSet As Integer = 0
                        Dim remainingTime As Single = 0
                        Dim previousTestID As Integer = -1
                        Dim myReadingNumbersDS As TimeEstimationDS = Nothing
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO

                        If (Not resultData.HasError) Then
                            'Get the last Reading Cycle for all Standard Pending/InProcess Preparations 
                            resultData = mytwksWSExecutionsDAO.GetMaxReadingNumber(dbConnection, pWorkSessionID, pAnalyzerID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myReadingNumbersDS = DirectCast(resultData.SetDatos, TimeEstimationDS)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'Get the rest of the data needed to calculate the remaining time for each Standard Preparation
                            resultData = mytwksWSExecutionsDAO.GetSTDTestsTimeEstimation(dbConnection, pWorkSessionID, pAnalyzerID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim mySTDRemainingTimesDS As TimeEstimationDS = DirectCast(resultData.SetDatos, TimeEstimationDS)

                                'Inform LastReadingCycle for each Standard Preparation
                                Dim mySTDTestTimeEstList As List(Of TimeEstimationDS.TestTimeValuesRow)
                                For Each execution As TimeEstimationDS.TestTimeValuesRow In myReadingNumbersDS.TestTimeValues
                                    mySTDTestTimeEstList = (From a As TimeEstimationDS.TestTimeValuesRow In mySTDRemainingTimesDS.TestTimeValues _
                                                           Where a.ExecutionID = execution.ExecutionID _
                                                          Select a).ToList()

                                    If (mySTDTestTimeEstList.Count > 0) Then
                                        mySTDTestTimeEstList.First.BeginEdit()
                                        mySTDTestTimeEstList.First.LastReadingCycle = execution.LastReadingCycle
                                        mySTDTestTimeEstList.First.EndEdit()
                                    End If
                                Next

                                For Each stdPreparation As TimeEstimationDS.TestTimeValuesRow In mySTDRemainingTimesDS.TestTimeValues
                                    If (stdPreparation.ExecutionStatus = "PENDING") Then
                                        'Only for Patient Samples: verify if the Test needs an automatic predilution
                                        If (stdPreparation.SampleClass = "PATIENT") Then
                                            If (stdPreparation.PredilutionUseFlag AndAlso stdPreparation.PredilutionMode = "INST") Then
                                                'An automatic predilution is needed: the Analyzer uses a number of fixed Cycles that has to be added
                                                'to the total remaining time of the Test
                                                remainingTime = (offSet + postR1Cycles + predilutionCycles + stdPreparation.TestCycles) * pCycleMachine
                                                offSet += predilutionCycles
                                            Else
                                                'An automatic predilution is not needed
                                                remainingTime = (offSet + postR1Cycles + stdPreparation.TestCycles) * pCycleMachine
                                                offSet += 1
                                            End If
                                        Else
                                            'For Blanks, Calibrators and Controls
                                            remainingTime = (offSet + postR1Cycles + stdPreparation.TestCycles) * pCycleMachine
                                            offSet += 1
                                        End If

                                        'Verify if the first Reagent of the previous Test contaminates the first Reagent of the current Test
                                        '(only if it is a different TestID, for Replicates this verification has not sense)
                                        If (previousTestID <> -1) AndAlso (previousTestID <> stdPreparation.TestID) Then
                                            Dim myContaminationsDelegate As New ContaminationsDelegate

                                            resultData = myContaminationsDelegate.GetContaminationsBetweenTests(dbConnection, previousTestID, stdPreparation.TestID)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                If (DirectCast(resultData.SetDatos, Boolean)) Then
                                                    'An additional Cycle for Washing is added to the Test remaining time
                                                    remainingTime += pCycleMachine
                                                    offSet += 1
                                                End If
                                            Else
                                                'Error verifying the Contaminations between Reagents of the previous and current Test
                                                Exit For
                                            End If

                                        End If

                                    ElseIf (stdPreparation.ExecutionStatus = "INPROCESS") Then
                                        If (stdPreparation.LastReadingCycle > 0) Then
                                            'The Analyzer has already sent Readings for this Standard Preparation: the remaining time is based on
                                            'the number of Readings pending to receive
                                            remainingTime = (stdPreparation.TestCycles - stdPreparation.LastReadingCycle) * pCycleMachine
                                            'TEST RESULT :FOR TEST DOCUMENT ONLY UNCOMMENTED
                                            'Debug.Print("Remaining time With predilutions: " & remainingTime.ToString())
                                            'Debug.Print("Test Cycles = " & (stdPreparation.TestCycles).ToString())
                                            'Debug.Print("Cycle Machine: " & pCycleMachine)
                                            'Debug.Print("Last Reading Cycle: " & stdPreparation.LastReadingCycle.ToString())

                                            'TEST RESULT -END.
                                        Else
                                            'No Readings have been received yet.... 

                                            'Only for Patient Samples: verify if the Test needs an automatic predilution
                                            If (stdPreparation.SampleClass = "PATIENT") Then
                                                'TR 11/06/2012 -Validate the sendingTime is not Null.
                                                If (stdPreparation.PredilutionUseFlag AndAlso stdPreparation.PredilutionMode = "INST") AndAlso _
                                                                                                        Not stdPreparation.IsSendingTimeNull Then
                                                    'An automatic predilution is needed:
                                                    '...The time has passed until the Standard Preparation was sent to the Analyzer is deducted from the
                                                    'theorical total time required [(Cycles after R1 is dispensed + Predilution Cycles + Test Cycles) * Machine Cycle Time]
                                                    remainingTime = ((postR1Cycles + predilutionCycles + _
                                                                      stdPreparation.TestCycles) * pCycleMachine) - _
                                                                      DateDiff(DateInterval.Second, stdPreparation.SendingTime, Now)

                                                    'TEST RESULT :FOR TEST DOCUMENT ONLY UNCOMMENTED
                                                    'Debug.Print("Remaining time With predilutions: " & remainingTime.ToString())
                                                    'Debug.Print("Test Cycles = " & (postR1Cycles + predilutionCycles + _
                                                    '                                stdPreparation.TestCycles).ToString())
                                                    'Debug.Print("Cycle Machine: " & pCycleMachine)
                                                    'Debug.Print("Last Reading Cycle: " & (DateDiff(DateInterval.Second, _
                                                    '                                stdPreparation.SendingTime, Now)).ToString())

                                                    'TEST RESULT -END.

                                                Else
                                                    'TR 29/06/2012 -Validate the sending time is not null
                                                    If Not stdPreparation.IsSendingTimeNull Then
                                                        'An automatic predilution is not needed:
                                                        '...The time has passed until the Standard Preparation was sent to the Analyzer is deducted from the
                                                        'theorical total time required [(Cycles after R1 is dispensed + Test Cycles) * Machine Cycle Time]
                                                        remainingTime = ((postR1Cycles + stdPreparation.TestCycles) * pCycleMachine) - _
                                                                            DateDiff(DateInterval.Second, stdPreparation.SendingTime, Now)
                                                    End If
                                                    'TR 29/06/2012 -END.
                                                End If
                                            Else
                                                'TR 29/06/2012 -Validate the sending time is not null.
                                                If Not stdPreparation.IsSendingTimeNull Then
                                                    'For Blanks, Calibrators and Controls:
                                                    '...The time has passed until the Standard Preparation was sent to the Analyzer is deducted from the
                                                    'theorical total time required [(Cycles after R1 is dispensed + Test Cycles) * Machine Cycle Time]
                                                    remainingTime = ((postR1Cycles + stdPreparation.TestCycles) * pCycleMachine) - _
                                                                            DateDiff(DateInterval.Second, stdPreparation.SendingTime, Now)
                                                End If
                                                'TR 29/06/2012 -END.
                                            End If
                                        End If
                                    End If

                                    'Update field remaining time in the DS for the processed Test
                                    stdPreparation.BeginEdit()
                                    stdPreparation.RemainingTime = remainingTime
                                    stdPreparation.EndEdit()

                                    'Save the TestID to verify Contaminations with the next one
                                    previousTestID = stdPreparation.TestID
                                Next

                                If (Not resultData.HasError) Then
                                    If (mySTDRemainingTimesDS.TestTimeValues.Rows.Count > 0) Then
                                        'Get the maximum Remaining Time between all the calculated
                                        resultData.SetDatos = (Aggregate a In mySTDRemainingTimesDS.TestTimeValues _
                                                               Into Max(a.RemainingTime))
                                    Else
                                        resultData.SetDatos = 0
                                    End If

                                    resultData.HasError = False
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetSTDTestsTimeEstimation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For all Pending and InProcess Executions corresponding to ISE Tests for the informed WorkSessionID and AnalyzerID, get following data:
        ''' ** Execution data: ID, Status and OrderTestID
        ''' ** ISE Test data: ID 
        ''' ** Preparations: the SendingTime of the Preparation containing the Instruction sent to the Analyzer for the Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCycleMachine">Time in seconds for an Analyzer Cycle</param>
        ''' <returns>GlobalDataTO containing a Single value with the maximum remaining time for all Pending and InProcess ISE Executions for the 
        '''          informed Analyzer and WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 24/10/2011 
        ''' </remarks>
        Public Function GetISETestsTimeEstimation(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                  ByVal pCycleMachine As Single) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim remainingTime As Single = 0
                        Dim iseExecutionTime As Integer = 0

                        'TR 04/06/2012 
                        Dim ISE_EXECUTION_TIME_SER As Single = 0
                        Dim ISE_EXECUTION_TIME_URI As Single = 0
                        Dim WAITING_CYCLES_AFTER_R1 As Single = 0

                        Dim myParametersDS As New ParametersDS
                        Dim mySWParametersDelegate As New SwParametersDelegate

                        resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, _
                                                                                   GlobalEnumerates.SwParameters.WAITING_CYCLES_AFTER_R1.ToString, True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                            ' TR 05/06/2012 -Get the number of cycles
                            If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                                WAITING_CYCLES_AFTER_R1 = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                            End If
                            'Calculate the Cycles
                            WAITING_CYCLES_AFTER_R1 = WAITING_CYCLES_AFTER_R1 * pCycleMachine

                        End If

                        If Not resultData.HasError Then
                            resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, _
                                                                                   GlobalEnumerates.SwParameters.ISE_EXECUTION_TIME_SER.ToString, True)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                                ' TR 05/06/2012 -Get the number of cycles
                                If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                                    ISE_EXECUTION_TIME_SER = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                                End If
                                'Calculate the Cycles
                                ISE_EXECUTION_TIME_SER = ISE_EXECUTION_TIME_SER * pCycleMachine
                            End If
                        End If

                        If Not resultData.HasError Then
                            resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, _
                                                                                   GlobalEnumerates.SwParameters.ISE_EXECUTION_TIME_URI.ToString, True)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                                ' TR 05/06/2012 -Get the number of cycles
                                If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                                    ISE_EXECUTION_TIME_URI = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                                End If
                                'Calculate the Cycles
                                ISE_EXECUTION_TIME_URI = ISE_EXECUTION_TIME_URI * pCycleMachine
                            End If
                        End If
                        'TR 04/06/2012 -END.

                        'TR 05/06/2012 -Commented This code is no implemente 'cause calculation change.
                        'Get the constant time needed to execute an ISE Test
                        'resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, GlobalEnumerates.SwParameters.ISE_EXECUTION_TIME.ToString, True)
                        'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        '    myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                        '    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then iseExecutionTime = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                        'End If
                        'TR 05/06/2012 -END.

                        If (Not resultData.HasError) Then
                            Dim myVWSOrderTestDelegate As New WSOrderTestsDelegate

                            'Get all data needed to calculate the remaining time for each ISE Preparation
                            '1) Get all pending or in process execution that has not been send. 
                            resultData = myVWSOrderTestDelegate.GetNotSendISEPreparations(dbConnection, pAnalyzerID, pWorkSessionID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myISERemainingTimesDS As TimeEstimationDS = DirectCast(resultData.SetDatos, TimeEstimationDS)
                                If myISERemainingTimesDS.TestTimeValues.Rows.Count > 0 Then
                                    remainingTime = WAITING_CYCLES_AFTER_R1
                                End If

                                For Each IsePreparationRow As TimeEstimationDS.TestTimeValuesRow In myISERemainingTimesDS.TestTimeValues.Rows

                                    Select Case IsePreparationRow.SampleType
                                        Case "SER", "PLM"
                                            remainingTime += ISE_EXECUTION_TIME_SER * IsePreparationRow.NumPreparations
                                            Exit Select
                                        Case "URI"
                                            remainingTime += ISE_EXECUTION_TIME_URI * IsePreparationRow.NumPreparations
                                            Exit Select
                                    End Select
                                Next
                                'Debug.Print("ISE PENDING time: " & remainingTime.ToString())
                                'Debug.Print("TOTAL PENDING: " & myISERemainingTimesDS.TestTimeValues.Count.ToString())

                                '2) Get all  Executions with sending time.
                                resultData = GetSendISETestsForTimeCalculation(dbConnection, pWorkSessionID, pAnalyzerID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myISERemainingTimesDS = DirectCast(resultData.SetDatos, TimeEstimationDS)

                                    If myISERemainingTimesDS.TestTimeValues.Rows.Count > 0 AndAlso remainingTime = 0 Then
                                        remainingTime += WAITING_CYCLES_AFTER_R1
                                    End If
                                    For Each IsePreparationRow As TimeEstimationDS.TestTimeValuesRow In myISERemainingTimesDS.TestTimeValues.Rows

                                        Select Case IsePreparationRow.SampleType
                                            Case "SER", "PLM"
                                                remainingTime += CSng(ISE_EXECUTION_TIME_SER - (Now.TimeOfDay.TotalSeconds _
                                                                                         - IsePreparationRow.SendingTime.TimeOfDay.TotalSeconds))
                                                Exit Select
                                            Case "URI"
                                                remainingTime += CSng(ISE_EXECUTION_TIME_URI - (Now.TimeOfDay.TotalSeconds _
                                                                                         - IsePreparationRow.SendingTime.TimeOfDay.TotalSeconds))
                                                Exit Select
                                        End Select
                                        'TR 25/07/2012 -Implemted like STD
                                        IsePreparationRow.RemainingTime = remainingTime
                                    Next
                                    If myISERemainingTimesDS.TestTimeValues.Count > 0 Then

                                        'Debug.Print("ISE SEND time: " & remainingTime.ToString())
                                        'Debug.Print("Total Send: " & myISERemainingTimesDS.TestTimeValues.Count.ToString())
                                    End If

                                End If

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Return the calculated remaining time
                                    resultData.SetDatos = remainingTime
                                Else
                                    resultData.SetDatos = 0
                                End If


                                'If (myISERemainingTimesDS.TestTimeValues.Rows.Count > 0) Then
                                '    'Filter the Executions for ISE Tests to get all of them with Status PENDING
                                '    Dim lstPendingISETests As List(Of TimeEstimationDS.TestTimeValuesRow)
                                '    lstPendingISETests = (From a As TimeEstimationDS.TestTimeValuesRow In myISERemainingTimesDS.TestTimeValues _
                                '                         Where a.ExecutionStatus = "PENDING" _
                                '                        Select a).ToList

                                '    'NEED to get the amount the ise Test Type by Test Type new query CREATED BY SA.
                                '    'CALCULATE the time for each Type SER, URI, PLM.

                                '    'Calculate the time for SER type
                                '    'remainingTime = lstPendingISETests.Where(Function(a) a.SampleType = "SER").Count() * 30

                                '    'remainingTime += lstPendingISETests.Where(Function(a) a.SampleType = "URI").Count() * 44

                                '    'Calculate the time needed to execute PENDING ISE Tests: one cycle for Preparation + the constant time for ISE executions
                                '    'multiplied by the total number of Pending ISE Tests
                                '    'remainingTime = (pCycleMachine + iseExecutionTime) * lstPendingISETests.Count

                                '    'Filter the Executions for ISE Tests to get all of them with Status IN PROCESS
                                '    'AG 30/11/2011 add this condition (AndAlso Not a.IsSendingTimeNull) due when pause in running it causes system error
                                '    Dim lstInProcessISETests As List(Of TimeEstimationDS.TestTimeValuesRow)
                                '    lstInProcessISETests = (From a As TimeEstimationDS.TestTimeValuesRow In myISERemainingTimesDS.TestTimeValues _
                                '                         Where a.ExecutionStatus = "INPROCESS" AndAlso Not a.IsSendingTimeNull _
                                '                        Select a).ToList

                                '    'Calculate the time needed to execute each IN PROCESS ISE Test:
                                '    'For Each inProcessISE As TimeEstimationDS.TestTimeValuesRow In lstInProcessISETests
                                '    '    If Not inProcessISE.IsSendingTimeNull Then 'AG 30/11/2011 add this condition due in running this field can be null
                                '    '        remainingTime += iseExecutionTime - DateDiff(DateInterval.Second, Now, inProcessISE.SendingTime)
                                '    '    End If
                                '    'Next inProcessISE

                                '    lstPendingISETests = Nothing
                                '    lstInProcessISETests = Nothing

                                '    'Return the calculated remaining time
                                '    resultData.SetDatos = remainingTime
                                'Else
                                '    resultData.SetDatos = 0
                                'End If
                                'resultData.HasError = False
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetISETestsTimeEstimation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information of the WS Element required for the informed Execution
        ''' 
        ''' By default return only information about sample tube elements, but if parameter alsoReagentElementInfo = true return both: sample tube elements and reagent elements information
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pAlsoReagentElementInfo"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with data of the WS Element required
        '''          for the informed Execution</returns>
        ''' <remarks>
        ''' Created by: SA 18/01/2012
        ''' Modified by: AG 24/04/2012 - Take care about special test HBTOTAL (calibration)
        ''' </remarks>
        Public Function GetElementInfoByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                    ByVal pWorkSessionID As String, ByVal pExecutionID As Integer, _
                                                    ByVal pAlsoReagentElementInfo As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        'AG 24/04/2012 - check if is a special test (calib HbTotal)
                        'resultData = myDAO.GetElementInfoByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, pAlsoReagentElementInfo)
                        Dim myRealMultiItemNumber As Integer = -1 'Only used for special test calibrators (HbTotal)
                        resultData = myDAO.GetExecution(dbConnection, pExecutionID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim wsExecDS As New ExecutionsDS
                            wsExecDS = CType(resultData.SetDatos, ExecutionsDS)
                            If wsExecDS.twksWSExecutions.Rows.Count > 0 Then
                                If wsExecDS.twksWSExecutions(0).SampleClass.Trim = "CALIB" Then
                                    Dim mySettingsDelegate As New SpecialTestsSettingsDelegate
                                    resultData = mySettingsDelegate.Read(dbConnection, wsExecDS.twksWSExecutions(0).TestID, wsExecDS.twksWSExecutions(0).SampleType, GlobalEnumerates.SpecialTestsSettings.CAL_POINT_USED.ToString)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim mySettingsDS As New SpecialTestsSettingsDS
                                        mySettingsDS = DirectCast(resultData.SetDatos, SpecialTestsSettingsDS)
                                        If mySettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0 Then
                                            myRealMultiItemNumber = CInt(mySettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        resultData = myDAO.GetElementInfoByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, pAlsoReagentElementInfo, myRealMultiItemNumber)
                        'AG 24/04/2012
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetElementInfoByExecutionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pExecutionType"></param>
        ''' <param name="pCurrentStatus"></param>
        ''' <param name="pNewStatus"></param>
        ''' <param name="pAffectedExecutionList"></param>
        ''' <returns></returns>
        ''' <remarks>AG 18/01/2012
        ''' AG 07/03/2012 - optional parameter pAffectedExecutionList</remarks>
        Public Function UpdateStatusByExecutionTypeAndStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                         ByVal pExecutionType As String, ByVal pCurrentStatus As String, ByVal pNewStatus As String, _
                                         Optional ByVal pAffectedExecutionList As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, pExecutionType, pCurrentStatus, pNewStatus, pAffectedExecutionList)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateStatusByExecutionTypeAndStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the status of all ISE Executions in the specified Analyzer WorkSession when they fulfill following conditions:
        '''  ** Their current status is the specified
        '''  ** They belong to OrderTests requested for the specified ISE Test
        ''' This function is used mainly to lock ISE Executions of ISE Electrodes with wrong or pending calibration
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pISEElectrode">Code of the ISE Electrode (electrode name in ISE Module -> value of field ISE_ResultID 
        '''                             in tparISETests table)</param>
        ''' <param name="pCurrentStatus">Current status should have the ISE Executions to update</param>
        ''' <param name="pNewStatus">Status to assign to the ISE Executions to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 25/07/2012
        ''' Modified by: SA  07/09/2012 - Changed name of parameter pISETestType by pISEElectrode
        ''' </remarks>
        Public Function UpdateStatusByISETestType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pAnalyzerID As String, ByVal pISEElectrode As String, ByVal pCurrentStatus As String, _
                                                  ByVal pNewStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.UpdateStatusByISETestType(dbConnection, pWorkSessionID, pAnalyzerID, pISEElectrode, pCurrentStatus, pNewStatus)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateStatusByISETestType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Mark as PENDING or LOCKED all inprocess executions depending if his required elements are positioned or not
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ChangeINPROCESSStatusByCollision(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pWorkSessionID As String, Optional ByVal pExecutionID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        Dim inProcessExecutionsDS As New ExecutionsDS
                        Dim execRow As ExecutionsDS.twksWSExecutionsRow

                        '1) Get all INPROCESS executions
                        If pExecutionID = -1 Then
                            resultData = myDAO.GetExecutionsByStatus(dbConnection, pWorkSessionID, pAnalyzerID, "INPROCESS", False)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                inProcessExecutionsDS = CType(resultData.SetDatos, ExecutionsDS)
                            End If
                            'Get ONLY the execution in parameter
                        Else
                            execRow = inProcessExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
                            execRow.ExecutionID = pExecutionID
                            inProcessExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(execRow)
                            inProcessExecutionsDS.AcceptChanges()
                        End If

                        '2) For each execution evaluate his new status PENDING or LOCKED (depending if his R1, S or R2 elements are POS or NOPOS)
                        Dim toMarkAsPending As String = "" ' Executions to mark as PENDING
                        Dim toMarkAsLocked As String = "" ' Executions to mark as LOCKED

                        If Not resultData.HasError Then
                            Dim rcpDS As New WSRotorContentByPositionDS
                            Dim linqRes As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                            For Each execRow In inProcessExecutionsDS.twksWSExecutions
                                'AG 24/04/2012 - Do not call the DAO method, call the delegate because new business is performed in it
                                'resultData = myDAO.GetElementInfoByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, execRow.ExecutionID, True)
                                resultData = GetElementInfoByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, execRow.ExecutionID, True)

                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    rcpDS = CType(resultData.SetDatos, WSRotorContentByPositionDS)

                                    linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rcpDS.twksWSRotorContentByPosition _
                                               Where a.ElementStatus = "NOPOS" Select a).ToList

                                    If linqRes.Count > 0 Then
                                        'Execution to be LOCKED
                                        If toMarkAsLocked = "" Then
                                            toMarkAsLocked = execRow.ExecutionID.ToString
                                        Else
                                            toMarkAsLocked += ", " & execRow.ExecutionID.ToString
                                        End If

                                    Else
                                        'Execution to be PENDING
                                        If toMarkAsPending = "" Then
                                            toMarkAsPending = execRow.ExecutionID.ToString
                                        Else
                                            toMarkAsPending += ", " & execRow.ExecutionID.ToString
                                        End If
                                    End If

                                Else
                                    Exit For
                                End If
                            Next
                            linqRes = Nothing
                        End If

                        '3) Update INPROCESS executions new status (PENDING or LOCKED)
                        If Not resultData.HasError Then
                            If toMarkAsPending <> "" Then
                                resultData = myDAO.UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "", "INPROCESS", "PENDING", toMarkAsPending)
                            End If

                            If toMarkAsLocked <> "" AndAlso Not resultData.HasError Then
                                resultData = myDAO.UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "", "INPROCESS", "LOCKED", toMarkAsLocked)
                            End If
                        End If

                        '4) Apply LOCK when related executions are locked
                        '4.1)STD_PREP: Lock pending executions when their related blank or calib are locked 
                        If Not resultData.HasError Then
                            resultData = LockRelatedExecutions(dbConnection, pWorkSessionID, pAnalyzerID, "BLANK")
                            If Not resultData.HasError Then
                                resultData = LockRelatedExecutions(dbConnection, pWorkSessionID, pAnalyzerID, "CALIB")
                            End If
                        End If

                        If Not resultData.HasError Then
                            'Get all required elements in worksession
                            Dim reqElementsDlg As New WSRequiredElementsDelegate
                            resultData = reqElementsDlg.ReadAll(dbConnection, pWorkSessionID)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim reqElmDS As New WSRequiredElementsDS
                                Dim linqRes As List(Of WSRequiredElementsDS.twksWSRequiredElementsRow)

                                reqElmDS = CType(resultData.SetDatos, WSRequiredElementsDS)

                                '4.2)STD_PREP: Lock all pending patient executions using predilutions if the required dilluent solution is NOPOS
                                If Not resultData.HasError Then
                                    'Using Linq search SPEC_SOL with status NOPOS
                                    linqRes = (From a As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElmDS.twksWSRequiredElements _
                                               Where a.TubeContent = "SPEC_SOL" AndAlso a.ElementStatus = "NOPOS" Select a).ToList

                                    For Each row As WSRequiredElementsDS.twksWSRequiredElementsRow In linqRes
                                        If Not row.IsSolutionCodeNull Then
                                            resultData = LockPatientPredilutionsByDiluentCode(dbConnection, pWorkSessionID, pAnalyzerID, row.SolutionCode)
                                        End If
                                    Next
                                End If

                                'AG 03/06/2014 - #1519 Do not lock any contaminator executions when WASH SOL bottles are over
                                ''4.3)STD_PREP: Lock all pending executions it the required washing solution is NOPOS
                                'If Not resultData.HasError Then
                                '    'Using Linq search WASH_SOL with status NOPOS
                                '    linqRes = (From a As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElmDS.twksWSRequiredElements _
                                '               Where a.TubeContent = "WASH_SOL" AndAlso a.ElementStatus = "NOPOS" Select a).ToList

                                '    For Each row As WSRequiredElementsDS.twksWSRequiredElementsRow In linqRes
                                '        If Not row.IsSolutionCodeNull Then
                                '            resultData = myDAO.UpdateStatusByContamination(dbConnection, "LOCKED", "PENDING", row.SolutionCode, pWorkSessionID, pAnalyzerID)
                                '        End If
                                '    Next
                                'End If

                                '4.4)ISE_PREP: Lock all pending executions if ise wash solution is NOPOS
                                If Not resultData.HasError Then
                                    'Using Linq search TUBE_WASH_SOL with status NOPOS
                                    linqRes = (From a As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElmDS.twksWSRequiredElements _
                                               Where a.TubeContent = "TUBE_WASH_SOL" AndAlso a.ElementStatus = "NOPOS" Select a).ToList
                                    If linqRes.Count > 0 Then
                                        resultData = myDAO.UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "PREP_ISE", "PENDING", "LOCKED")
                                    End If
                                End If
                                linqRes = Nothing

                            End If

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ChangeINPROCESSStatusByCollision", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the ValidReadings and CompleteReadings in twksWSExecutions
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 02/07/2012</remarks>
        Public Function UpdateValidAndCompleteReadings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.UpdateValidAndCompleteReadings(dbConnection, pExecutionsDS)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateValidAndCompleteReadings", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Get all Executions for the specified OrderTest/RerunNumber (due to its Results have been exported to LIMS)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pReRunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the group of affected Executions</returns>
        ''' <remarks>
        ''' Created by:  TR 12/07/2012
        ''' Modified by: SA 01/08/2012 - Added parameters for OrderTestID and RerunNumber
        ''' Modified by AG 30/07/2014 - #1887 be sure all CALC/OFFS tests appear in data to export
        ''' </remarks>
        Public Function GetExportedExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                              ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetExportedExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pRerunNumber)

                        'AG 30/07/20014 #1887 
                        'previous query sometime fails for calculated tests that are form by another calc test (uses the view in monitor WS in v1.0.0)
                        'when NOTHING found use the new view developed v300 for CALC/OFF tests
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            If DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions.Rows.Count = 0 Then
                                resultData = myDAO.GetExportedExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pRerunNumber, True)
                            End If
                        End If
                        'AG 30/07/2014


                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExportedExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the last Rerun requested for the Order Test is LockedByLIS in Executions table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing if there are a lock</returns>
        ''' <remarks>
        ''' Created by: XB 21/03/2013
        ''' </remarks>
        Public Function VerifyLockedByLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetLockedByLIS(dbConnection, pOrderTestID, pRerunNumber)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.VerifyLockedByLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' UnLock executions because of LIS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID" ></param>
        ''' <param name="pRerunNumber"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XB 21/03/2013
        ''' </remarks>
        Public Function UnlockLISExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        If Not resultData.HasError Then
                            resultData = myDAO.UnlockLISExecutions(dbConnection, pOrderTestID, pRerunNumber)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UnlockLISExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the last Rerun requested for the Order Test was requested by LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing the result</returns>
        ''' <remarks>
        ''' Created by:  XB 21/03/2013
        ''' </remarks>
        Public Function GetMaxRerunNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetMaxRerunNumber(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetMaxRerunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Evaluate if there are some test in process that are critical in pause mode
        ''' In this case returns TRUE, else returns FALSE
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerId"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pWorksessionId"></param>
        ''' <returns>GlobalDataTo (data as boolean)</returns>
        ''' <remarks>AG 22/11/2013 - Creation, task #1391</remarks>
        Public Function ExistCriticalPauseTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerId As String, ByVal pAnalyzerModel As String, ByVal pWorksessionId As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim valueToReturn As Boolean = False 'No critical in pause tests in process
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim specialTestDlg As New SpecialTestsSettingsDelegate
                        resultData = specialTestDlg.ExistsCriticalPauseTests(dbConnection, pAnalyzerId, pWorksessionId)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim execDS As New ExecutionsDS
                            execDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            'If some item execute proper business
                            If execDS.twksWSExecutions.Rows.Count > 0 Then
                                'Read Software limits and parameters required
                                Dim internalReadingsOffset As Integer = 0
                                Dim defaultR2IsAdded As Integer = 0
                                Dim maxReadingsWithR2 As Integer = 0

                                Dim myParamDlg As New SwParametersDelegate
                                resultData = myParamDlg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SW_READINGSOFFSET.ToString, pAnalyzerModel)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    If (CType(resultData.SetDatos, ParametersDS).tfmwSwParameters.Rows.Count > 0 AndAlso _
                                        Not CType(resultData.SetDatos, ParametersDS).tfmwSwParameters(0).IsValueNumericNull) Then
                                        internalReadingsOffset = CInt(DirectCast(resultData.SetDatos, ParametersDS).tfmwSwParameters(0).ValueNumeric)
                                    End If
                                End If

                                If Not resultData.HasError Then
                                    Dim myLimitsDlg As New FieldLimitsDelegate
                                    resultData = myLimitsDlg.GetList(dbConnection, GlobalEnumerates.FieldLimitsEnum.READING2_CYCLES, pAnalyzerModel)
                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                        Dim auxLimitsDS As New FieldLimitsDS
                                        auxLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)
                                        If auxLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                                            defaultR2IsAdded = CInt(auxLimitsDS.tfmwFieldLimits(0).MinValue) - internalReadingsOffset
                                            maxReadingsWithR2 = CInt(auxLimitsDS.tfmwFieldLimits(0).MaxValue) - CInt(auxLimitsDS.tfmwFieldLimits(0).MinValue) + 1
                                        End If
                                    End If
                                End If

                                Dim realR2IsAdded As Integer = 0
                                Dim delayedR2Cycles As Integer = 0
                                Dim myReadingsDelegate As New WSReadingsDelegate
                                Dim execReadingsDS As New twksWSReadingsDS

                                If Not resultData.HasError Then
                                    For Each row As ExecutionsDS.twksWSExecutionsRow In execDS.twksWSExecutions
                                        delayedR2Cycles = 0
                                        realR2IsAdded = 0
                                        execReadingsDS.Clear()

                                        'Get the readings for this execution
                                        resultData = myReadingsDelegate.Read(dbConnection, pAnalyzerId, pWorksessionId, row.ExecutionID, True)
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            execReadingsDS = DirectCast(resultData.SetDatos, twksWSReadingsDS)
                                        End If

                                        If Not resultData.HasError Then
                                            'In case the execution has readings call the method that evaluates if the R2 has been added or not
                                            If execReadingsDS.twksWSReadings.Count > 0 Then
                                                'Search the R2 dispensing delay
                                                resultData = myReadingsDelegate.GetR2DelayedCycles(dbConnection, pAnalyzerId, pAnalyzerModel, pWorksessionId, _
                                                                                                 row.ExecutionID, defaultR2IsAdded, maxReadingsWithR2, internalReadingsOffset, _
                                                                                                 realR2IsAdded, row.TestID, execReadingsDS)
                                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                                    'delayedR2Cycles = DirectCast(resultData.SetDatos, Integer)

                                                    'If few readings that the required for r2 to be add to worksession -> return true
                                                    If execReadingsDS.twksWSReadings.Rows.Count < realR2IsAdded Then
                                                        valueToReturn = True
                                                        Exit For
                                                    End If
                                                End If
                                            End If
                                        End If

                                    Next
                                End If

                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ExistCriticalPauseTests", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            resultData.SetDatos = valueToReturn
            Return resultData
        End Function


        ''' <summary>
        ''' Get data for fill screen monitor tab WS (tests STD, ISE, OFFS, CALC)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pTestsWithExecutionsFlag">TRUE to get data for STD and ISE Tests
        '''                                        FALSE to get data for OFFS and CALC Tests</param>
        ''' <returns>GlobalDataTO containing a typed DS ExecutionsDS.vwksWSExecutionsMonitor with data obtained (according value of flag pTestsWithExecutionsFlag)</returns>
        ''' <remarks>
        ''' Created by:  AG 27/02/2014 - BT #1524
        ''' Modified by: SA 04/04/2014 - BT #1524 ==> For Patient Samples that need only an additional tube for a manual predilution (the full tube is not needed), the called 
        '''                                           function does not return fields from table twksWSRequiredElements (due to field PredilutionFactor is not NULL and the view
        '''                                           used applies a filter by PredilutionFactor=NULL). To solve this problem, it is verified if there are Executions without 
        '''                                           information from table of twksWSRequiredElements and in this case, funtion ReadAllPatientDilutions in WSRequiredElementsDelegate 
        '''                                           is called to get the missing data for each one of them
        ''' </remarks>
        Public Function GetDataForMonitorTabWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pTestsWithExecutionsFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        If (pTestsWithExecutionsFlag) Then
                            'Get Executions for Standard and ISE Tests
                            resultData = myDAO.GetDataSTDISEForMonitorTabWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        Else
                            'Get "fake" Executions for Calculated and OffSystem Tests
                            resultData = myDAO.GetDataOFFSCALCForMonitorTabWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'SA 04/04/2014 - BT #1524
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myExecutionsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            'Check if there are Executions without information from table of WS Required Elements
                            Dim lstMonitorExecutions As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow) = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In myExecutionsDS.vwksWSExecutionsMonitor _
                                                                                                           Where a.SampleClass = "PATIENT" _
                                                                                                         AndAlso a.IsSpecimenIDListNull _
                                                                                                         AndAlso a.IsElementFinishedNull _
                                                                                                          Select a).ToList()

                            If (lstMonitorExecutions.Count > 0) Then
                                'Get all Patient Samples for manual predilutions having field SpecimenIDList informed
                                Dim myWSReqElemDS As WSRequiredElementsDS
                                Dim myWSReqElemDelegate As New WSRequiredElementsDelegate
                                Dim lstWSReqElement As List(Of WSRequiredElementsDS.twksWSRequiredElementsRow)

                                resultData = myWSReqElemDelegate.ReadAllPatientDilutions(dbConnection)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myWSReqElemDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                    If (myWSReqElemDS.twksWSRequiredElements.Rows.Count > 0) Then
                                        For Each patientRow As ExecutionsDS.vwksWSExecutionsMonitorRow In lstMonitorExecutions
                                            'Search fields SpecimenIDList and ElementFinished for the Patient Sample
                                            lstWSReqElement = (From a As WSRequiredElementsDS.twksWSRequiredElementsRow In myWSReqElemDS.twksWSRequiredElements _
                                                              Where a.PatientID = patientRow.ElementName _
                                                            AndAlso a.SampleType = patientRow.SampleType _
                                                             Select a).ToList()

                                            If (lstWSReqElement.Count > 0) Then
                                                'Inform fields in the ExecutionsDS to return
                                                patientRow.BeginEdit()
                                                patientRow.WorkSessionID = lstWSReqElement.First.WorkSessionID
                                                patientRow.ElementFinished = lstWSReqElement.First.ElementFinished
                                                If (Not lstWSReqElement.First.IsSpecimenIDListNull) Then patientRow.SpecimenIDList = lstWSReqElement.First.SpecimenIDList
                                                patientRow.EndEdit()
                                            End If
                                        Next
                                    End If
                                End If
                                lstWSReqElement = Nothing

                                'Return the ExecutionsDS with the applied changes
                                resultData.SetDatos = myExecutionsDS
                                resultData.HasError = False
                            End If
                            lstMonitorExecutions = Nothing
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetDataForMonitorTabWS", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Count executions in current WS by ExecutionStatus
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pStatus"></param>
        ''' <returns></returns>
        ''' <remarks>AG 28/02/2014 - #1524</remarks>
        Public Function CountByExecutionStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                            ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.CountByExecutionStatus(dbConnection, pAnalyzerID, pWorkSessionID, pStatus)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CountByExecutionStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Signature #2
        ''' Update flag paused in executions table by OrderTestID - RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <param name="pNewValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 03/03/2014 - #1524
        ''' AG 20/03/2014 - #1545 also update table twksWSPausedOrderTests (add new parameters AnalyzerID and WorkSessionID is required</remarks>
        Public Function UpdatePaused(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As List(Of Integer), ByVal pRerunNumber As List(Of Integer), ByVal pNewValue As Boolean, _
                                     ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If pOrderTestID.Count = pRerunNumber.Count Then
                            Dim myDAO As New twksWSExecutionsDAO
                            resultData = myDAO.UpdatePaused(dbConnection, pOrderTestID, pRerunNumber, pNewValue)

                            'AG 20/03/2014 - #1545 update table of paused order tests
                            If Not resultData.HasError Then
                                Dim tempPausedOT As New WSPausedOrderTestsDelegate

                                'Read current contents of table
                                resultData = tempPausedOT.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    Dim pausedOTsDS As New WSPausedOrderTestsDS
                                    pausedOTsDS = DirectCast(resultData.SetDatos, WSPausedOrderTestsDS)

                                    If pNewValue Then
                                        'Add orderTest-Rerun to table (if not exists yet)
                                        Dim orderTestsToAddDS As New WSPausedOrderTestsDS
                                        Dim newRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow
                                        Dim existsLnq As List(Of WSPausedOrderTestsDS.twksWSPausedOrderTestsRow)
                                        For i As Integer = 0 To pOrderTestID.Count - 1
                                            Dim aux_i = i
                                            If pOrderTestID.Count >= aux_i OrElse pRerunNumber.Count >= aux_i Then
                                                existsLnq = (From a As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow In pausedOTsDS.twksWSPausedOrderTests _
                                                             Where a.OrderTestID = pOrderTestID(aux_i) AndAlso a.RerunNumber = pRerunNumber(aux_i) Select a).ToList
                                                If existsLnq.Count = 0 Then 'Add
                                                    'Create new row if ok. in twksWSPausedOrderTests
                                                    newRow = orderTestsToAddDS.twksWSPausedOrderTests.NewtwksWSPausedOrderTestsRow()
                                                    newRow.AnalyzerID = pAnalyzerID
                                                    newRow.WorkSessionID = pWorkSessionID
                                                    newRow.OrderTestID = pOrderTestID(aux_i)
                                                    newRow.RerunNumber = pRerunNumber(aux_i)
                                                    orderTestsToAddDS.twksWSPausedOrderTests.AddtwksWSPausedOrderTestsRow(newRow)
                                                End If
                                            End If
                                        Next
                                        orderTestsToAddDS.twksWSPausedOrderTests.AcceptChanges()
                                        existsLnq = Nothing
                                        If orderTestsToAddDS.twksWSPausedOrderTests.Rows.Count > 0 Then
                                            resultData = tempPausedOT.Create(dbConnection, orderTestsToAddDS)
                                        End If

                                    Else
                                        'Delete orderTest-Rerun from table
                                        resultData = tempPausedOT.DeleteList(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pRerunNumber)
                                    End If
                                End If


                            End If
                            'AG 20/03/2014 - #1545

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdatePaused #2", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When the informed Execution belongs to a CALIBRATOR Order Test for a Multipoint Experimental Calibrator, this function searches and
        ''' returns data of Executions for each one of the Calibrators points. NOTE: There is a NEW version of this function used in Calculations module,
        ''' but this function is still used in WSRotorContentByPositionDelegate in function UpdateSamplePositionStatus
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of all Executions for an specific Multipoint Calibrator</returns>
        ''' <remarks>
        ''' Created by:          
        ''' </remarks>
        Public Function GetExecutionMultititem(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.GetExecutionsMultiItem(dbConnection, pExecutionID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecutionMultititem", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 04/06/2012</remarks>
        Private Function GetSendISETestsForTimeCalculation(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, _
                                                          ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSExecutionsDAO As New twksWSExecutionsDAO
                        myGlobalDataTO = myWSExecutionsDAO.GetSendISETestsForTimeCalculation(pDBConnection, pWorkSessionID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetSendISETestsForTimeCalculation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified Test and SampleType, verify the type of the needed Calibrator and if it is  locked or not:
        ''' ** If CalibratorType = EXPERIMENTAL, verify if the Calibrator defined for the Test and SampleType is locked
        ''' ** If CalibratorType = FACTOR, returns the Calibrator is unlocked 
        ''' ** If CalibratorType = ALTERNATIVE:
        '''       If the Calibrator needed for the Test and the Alternative SampleType is EXPERIMENTAL, verify if it is locked 
        '''       If the Calibrator needed for the Test and the Alternative SampleType is FACTOR, returns that it is unlocked 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pAlternativeST">When the Calibrator needed for the specified Test and SampleType is an Alternative one,
        '''                              the Alternative SampleType is returned in this parameter. Optional parameter needed only
        '''                              when the verification is done to set the status of Executions for Patient Samples</param>
        ''' <returns>GlobalDataTO containing a boolean value: True if the Calibrator needed for the specified Test and 
        '''          SampleType is locked; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2010
        ''' Modified by: SA 01/09/2010 - Before verify if the needed Calibrator is positioned in the Analyzer Rotor, verify if it
        '''                              has to be executed in the active WS or if a previous result will be used; in case of reusing
        '''                              a result, it is not needed verifying if the Calibrator is positioned                            
        ''' </remarks>
        Private Function VerifyLockedCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                ByVal pTestID As Integer, ByVal pSampleType As String, Optional ByRef pAlternativeST As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '...Verify the type of Calibrator required for the Test and SampleType
                        Dim sampleTypeToVerify As String = ""
                        Dim verifyCalibLocked As Boolean = False
                        Dim myTestCalibratorDelegate As New TestCalibratorsDelegate

                        resultData = myTestCalibratorDelegate.GetTestCalibratorData(dbConnection, pTestID, pSampleType)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myTestSampleCalibratorDS As TestSampleCalibratorDS
                            myTestSampleCalibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                            If (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count = 1) Then
                                verifyCalibLocked = (myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorType = "EXPERIMENT")
                                sampleTypeToVerify = pSampleType
                            ElseIf (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count > 1) Then
                                'Calibrator is Alternative...verify if the SampleType Alternative needs an Experimental Calibrator
                                verifyCalibLocked = (myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorType = "EXPERIMENT")
                                sampleTypeToVerify = myTestSampleCalibratorDS.tparTestCalibrators(0).SampleType

                                'Set value of the parameter used to return the Alternative SampleType
                                pAlternativeST = myTestSampleCalibratorDS.tparTestCalibrators(0).SampleType
                            End If

                            '...If the Calibrator needed is Experimental, first verify it has to be executed in the current WorkSession
                            '(if a previous result was selected to be used it is not needed verify if the Calibrator is positioned)
                            Dim reqElemNoPos As Boolean = False
                            If (verifyCalibLocked) Then
                                Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                                resultData = myWSOrderTestsDelegate.VerifyToSendFlag(dbConnection, pAnalyzerID, pWorkSessionID, "CALIB", _
                                                                                     pTestID, sampleTypeToVerify)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    verifyCalibLocked = Convert.ToBoolean(resultData.SetDatos)

                                    '...If the Calibrator needed is Experimental and has to be executed,then verify if it is Locked
                                    If (verifyCalibLocked) Then
                                        Dim myExecutionsDAO As New twksWSExecutionsDAO
                                        resultData = myExecutionsDAO.VerifyUnlockedExecution(dbConnection, pAnalyzerID, pWorkSessionID, "CALIB", _
                                                                                             pTestID, sampleTypeToVerify)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            reqElemNoPos = (Not Convert.ToBoolean(resultData.SetDatos))
                                        End If
                                    End If
                                End If
                            End If

                            If (Not resultData.HasError) Then resultData.SetDatos = reqElemNoPos
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.VerifyLockedCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Test and SampleType, verify if it is required or not
        ''' When required verify if it is locked or not:
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a boolean value: True if the Calibrator needed for the specified Test and 
        '''          SampleType is locked; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  AG 20/04/2012 - Based on VerifyLockedCalibrator
        ''' </remarks>
        Private Function VerifyLockedBlank(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim verifyBlankLocked As Boolean = False

                        '...BLANK, first verify it has to be executed in the current WorkSession
                        '(if a previous result was selected to be used it is not needed verify if the Blank is positioned)
                        Dim reqElemNoPos As Boolean = False
                        Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                        resultData = myWSOrderTestsDelegate.VerifyToSendFlag(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", _
                                                                             pTestID, pSampleType)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            verifyBlankLocked = Convert.ToBoolean(resultData.SetDatos)

                            '...If the Calibrator needed is Experimental and has to be executed,then verify if it is Locked
                            If (verifyBlankLocked) Then
                                Dim myExecutionsDAO As New twksWSExecutionsDAO
                                resultData = myExecutionsDAO.VerifyUnlockedExecution(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", _
                                                                                     pTestID, pSampleType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    reqElemNoPos = (Not Convert.ToBoolean(resultData.SetDatos))
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then resultData.SetDatos = reqElemNoPos
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.VerifyLockedBlank", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a new repetition (manual or automatic) is become to be generated we inform the executions with the 
        ''' postdilution type they are generated
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pSentPostdilutionType"></param>
        ''' <returns></returns>
        ''' <remarks>AG 09/03/2011 - Tested OK</remarks>
        Public Function UpdateNewRerunSentPostdilutionType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                           ByVal pOrderTestID As Integer, ByVal pSentPostdilutionType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.UpdateSentNewRerunPostdilution(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pSentPostdilutionType)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateNewRerunSentPostdilutionType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Process volume missing implementing different business depending the SampleClass and TubeContent conbination
        ''' This method is used in 2 cases: 
        ''' a) No volume detected and NO other possible positions (original design case)
        ''' b) Reagent bottle locked because invalid refill and NO other possible positions (October 2012 - reagents on board)
        ''' 
        ''' Apply affected executions lock:
        ''' 1) Always locks the PENDING executions using this elementID
        ''' 2) Only in case a) (no volume detection) also the inprocess executions with prepID > pPreparationID are also locked
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pElementID"></param>
        ''' <param name="pSampleClass"></param>
        ''' <param name="pTubeContent"></param>
        ''' <param name="pSolutionCode"></param>
        ''' <param name="pPreparationID" > added 31/01/2012</param>
        ''' <param name="pReplicateNumber"></param>
        ''' <param name="pMultiItemNumber"></param>
        ''' <param name="pMultiItemOrderTest"></param>
        ''' <param name="pPositionStatus">added 03/10/2012 AG, values DEPLETED or LOCKED</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 05/04/2011 - creation
        ''' AG 31/01/2012 - add pPreparationID parameter
        ''' AG 03/10/2012 - add pReplicateNumber + pMultiItemNumber + pMultiItemOrderTest parameters, when his value higher than 1 Sw has to lock also the executions with same replicatenumber and MultiItemNumber lower than value
        '''                 add pPositionStatus parameter (used to differenciate when this method is called from NO VOLUME DETECTION [positionStatus = 'DEPLETED'] or INVALID REFILL DETECTION [positionStatus = 'LOCKED'])
        '''                 a) When DEPLETED lock affected pending executions + inprocess executions with prepID > pPreparationID
        '''                 b) When LOCKED lock only affected pending executions
        ''' </remarks>
        Private Function ProcessVolumeMissingBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                           ByVal pElementID As Integer, ByVal pSampleClass As String, _
                                                           ByVal pTubeContent As String, ByVal pSolutionCode As String, ByVal pPreparationID As Integer, _
                                                           ByVal pReplicateNumber As Integer, ByVal pMultiItemNumber As Integer, ByVal pMultiItemOrderTest As Integer, ByVal pPositionStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'AG 03/10/2012 - when position status LOCKED only the affected pending executions must be locked
                        Dim affectedPendingExecutions As Integer = pPreparationID
                        If pPositionStatus = "LOCKED" Then
                            affectedPendingExecutions = -1
                        End If
                        'AG 03/10/2012

                        Dim myRequiredElemByOrderTest As New WSRequiredElemByOrderTestDelegate

                        'REAGENT volume missing OR CTRL tube volume missing or PATIENT tube volume missing (OK tested 05/04/2011)
                        If (pTubeContent = "REAGENT" OrElse pTubeContent = "CTRL" OrElse pTubeContent = "PATIENT") Then
                            resultData = myRequiredElemByOrderTest.ReadOrderTestByElementID(dbConnection, pElementID)

                            If (Not resultData.HasError) Then
                                Dim myReqElementsDS As New WSRequiredElemByOrderTestDS
                                myReqElementsDS = CType(resultData.SetDatos, WSRequiredElemByOrderTestDS)

                                For Each Row As WSRequiredElemByOrderTestDS.twksWSRequiredElemByOrderTestRow In myReqElementsDS.Tables(0).Rows
                                    resultData = UpdateStatusByOrderTest(dbConnection, "LOCKED", Row.OrderTestID, "PENDING", affectedPendingExecutions) 'pPreparationID) AG 03/10/2012
                                Next Row

                                'AG 03/10/2012 - multiitem calibrators lock all executions with the same replicate number with the same OrderTestID as the one that failed
                                If pSampleClass = "CALIB" AndAlso pMultiItemNumber > 1 Then
                                    resultData = UpdateStatusByOrderTest(dbConnection, "LOCKED", pMultiItemOrderTest, "PENDING", -1, pReplicateNumber)
                                End If
                                'AG 03/10/2012

                            End If


                            'SAMPLE tube volume missing for BLANK (SPEC_SOL) or CALIB (calib tube) (OK tested 05/04/2011)
                            ''SPEC_SOL belonging a patient (predilution) fails (new case) (OK tested 07/04/2011)
                        ElseIf pSampleClass = "BLANK" OrElse pSampleClass = "CALIB" _
                                OrElse (pTubeContent = "SPEC_SOL" AndAlso pSampleClass = "PATIENT") Then

                            'AG 20/06/2011 - Now, these 2 lines are not needed due blanks not use SPEC_SOL bottle (they use tubes)
                            'Dim mySampleClass As String = pSampleClass
                            'If (pTubeContent = "SPEC_SOL" AndAlso pSampleClass = "PATIENT") Then mySampleClass = "BLANK"

                            resultData = myRequiredElemByOrderTest.ReadOrderTestByElementIDAndSampleClass(dbConnection, pElementID, pSampleClass)

                            If (Not resultData.HasError) Then
                                Dim myReqElementsDS As New WSRequiredElemByOrderTestDS
                                myReqElementsDS = CType(resultData.SetDatos, WSRequiredElemByOrderTestDS)

                                For Each Row As WSRequiredElemByOrderTestDS.twksWSRequiredElemByOrderTestRow In myReqElementsDS.Tables(0).Rows
                                    'Locking by ordertest solves the case multicalibrators
                                    resultData = UpdateStatusByOrderTest(dbConnection, "LOCKED", Row.OrderTestID, "PENDING", affectedPendingExecutions) 'pPreparationID) AG 03/10/2012
                                Next Row

                                'AG 03/10/2012 - multiitem calibrators lock all executions with the same replicate number with the same OrderTestID as the one that failed
                                If pSampleClass = "CALIB" AndAlso pMultiItemNumber > 1 Then
                                    resultData = UpdateStatusByOrderTest(dbConnection, "LOCKED", pMultiItemOrderTest, "PENDING", -1, pReplicateNumber)
                                End If
                                'AG 03/10/2012

                            End If

                            ' .... for all BLANKS with rerun = replicate = 1 LOCKED: Lock his related (TestID) CALIBS, CTRLS, PATIENTS  
                            ' ... for all CALIB with rerun = replicate = 1 LOCKED: Lock his related (TestID-SampleType) CTRLS, PATIENTS
                            If pSampleClass = "BLANK" OrElse pSampleClass = "CALIB" Then
                                resultData = LockRelatedExecutions(dbConnection, pWorkSessionID, pAnalyzerID, pSampleClass, affectedPendingExecutions) 'pPreparationID) AG 03/10/2012
                            End If


                            'Lock PENDING patients using SPEC_SOL as predilution
                            If pSampleClass = "BLANK" OrElse (pTubeContent = "SPEC_SOL" AndAlso pSampleClass = "PATIENT") Then
                                resultData = LockPatientPredilutionsByDiluentCode(dbConnection, pWorkSessionID, pAnalyzerID, pSolutionCode, affectedPendingExecutions) 'pPreparationID) AG 03/10/2012
                            End If


                            'WASH solution bottle is empty (OK tested 06/04/2011)
                        ElseIf pTubeContent = "WASH_SOL" Then
                            'AG 03/06/2014 - #1519 Do not lock any contaminator executions when WASH SOL bottles are over
                            ''LOCK all executions that uses reagents contaminators that requires the WASH_SOL - pSolutionCode
                            ''See ExecutionsByWashCode.sql
                            'Dim myDAO As New twksWSExecutionsDAO
                            'resultData = myDAO.UpdateStatusByContamination(dbConnection, "LOCKED", "PENDING", pSolutionCode, pWorkSessionID, pAnalyzerID)

                        ElseIf pTubeContent = "TUBE_WASH_SOL" Then
                            'ISE washing solution
                            'Lock all pending ISE test preparations
                            Dim myDAO As New twksWSExecutionsDAO
                            resultData = myDAO.UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "PREP_ISE", "PENDING", "LOCKED")
                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ProcessVolumeMissingBySampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the executions locked by SampleClass, Rerun and replicate number criteria
        ''' Then apply locks in other sampleclasses
        '''      a) BLANKS may lock CALIB, CTRLS and PATIENT
        '''      b) CALIBS may lock CTRLS and PATIENT
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pSampleClass">sample class</param>
        ''' <param name="pLockPreparationIDHigherThanThis">The Inprocess executions related with preparationID higher than this parameter will be also locked</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 04/01/2011
        ''' AG modified completely 04/04/2011
        ''' AG 29/02/2012 - add pLockPreparationIDHigherThanThis parameter and when informed lock also all ordertest INPROCESS executions where preparationID > parameter pPreparationID
        ''' </remarks>
        Private Function LockRelatedExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                               ByVal pSampleClass As String, Optional ByVal pLockPreparationIDHigherThanThis As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        'Get all executions LOCKED with RerunNumber = ReplicateNumber = 1 and SampleClass = pSampleClass
                        resultData = myDAO.ReadLockRExecutionsBySampleClass(dbConnection, pWorkSessionID, pAnalyzerID, pSampleClass, 1, 1)
                        If (Not resultData.HasError) Then
                            Dim myExecutionsDS As New ExecutionsDS
                            myExecutionsDS = CType(resultData.SetDatos, ExecutionsDS)

                            If myExecutionsDS.twksWSExecutions.Rows.Count > 0 Then
                                Dim testID As Integer = 0
                                Dim sampleType As String = ""
                                For Each myRow As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions
                                    If Not myRow.IsTestIDNull Then
                                        testID = myRow.TestID
                                        sampleType = ""
                                        If pSampleClass = "BLANK" Then
                                            'Lock all PENDING CALIBs using the TestID
                                            resultData = myDAO.LockRelatedExecutions(dbConnection, pWorkSessionID, pAnalyzerID, "CALIB", myRow.TestID, sampleType, "STD", pLockPreparationIDHigherThanThis)
                                        Else
                                            If Not myRow.IsSampleTypeNull Then sampleType = myRow.SampleType
                                        End If
                                    End If

                                    'Lock all PENDING CTRLs using the TestID (and optionally the SampleType)
                                    If Not resultData.HasError Then
                                        resultData = myDAO.LockRelatedExecutions(dbConnection, pWorkSessionID, pAnalyzerID, "CTRL", myRow.TestID, sampleType, "STD", pLockPreparationIDHigherThanThis)
                                    End If


                                    'Lock all PENDING PATIENTs using the TestID (and optionally the SampleType)
                                    If Not resultData.HasError Then
                                        resultData = myDAO.LockRelatedExecutions(dbConnection, pWorkSessionID, pAnalyzerID, "PATIENT", myRow.TestID, sampleType, "STD", pLockPreparationIDHigherThanThis)
                                    End If

                                Next myRow
                            End If

                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.LockRelatedExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Locks the Executions cancelled by LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID" ></param>
        ''' <param name="pExeType"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function LockExecutionsByLIS(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pOrderTestID As Integer, _
                                            ByVal pExeType As String, _
                                            ByVal pRerunNumber As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        'Lock all PENDING CTRLs using the TestID (and optionally the SampleType)
                        If Not resultData.HasError Then
                            resultData = myDAO.LockExecutionsByLIS(dbConnection, pOrderTestID, pExeType, pRerunNumber)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.LockExecutionsByLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' All pending executions that are PREDILUTIONS and required the diluent solution with no volume are LOCKED
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pDiluentCode ">sample class</param>
        ''' <param name="pLockPreparationIDHigherThanThis"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 06/04/2011
        ''' AG 29/02/2012 - add pLockPreparationIDHigherThanThis parameter and when informed lock also all ordertest INPROCESS executions where preparationID > parameter pPreparationID
        ''' </remarks>
        Private Function LockPatientPredilutionsByDiluentCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                               ByVal pDiluentCode As String, Optional ByVal pLockPreparationIDHigherThanThis As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO

                        'Get all executions LOCKED with RerunNumber = ReplicateNumber = 1 and SampleClass = pSampleClass
                        resultData = myDAO.LockPredilutionsByDiluentCode(dbConnection, pWorkSessionID, pAnalyzerID, pDiluentCode, pLockPreparationIDHigherThanThis)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.LockPatientPredilutionsByDiluentCode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the list of Executions sorted by ReadingCycle
        ''' </summary>
        ''' <param name="pExecutions">List of twksWSExecutionsRow to be sorted</param>
        ''' <param name="returnDS">ExecutionsDS with sorted rows</param>
        ''' <remarks>
        ''' Created by: RH 09/06/2010
        ''' AG 16/09/2011 SORT CRITERIA: pending (by time) and then locked (by time)
        ''' </remarks>
        Private Sub SortByExecutionTime(ByVal pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), ByRef returnDS As ExecutionsDS)
            While pExecutions.Count > 0

                'AG 16/09/2011 - add order criteria first by ExecutionStatus
                'Dim wseMaxReadingCycle = (From wse In pExecutions _
                '       Order By wse.ReadingCycle Descending _
                '       Select wse).ToList()(0)

                Dim wseMaxReadingCycle = (From wse In pExecutions _
                                          Order By wse.ExecutionStatus Descending, wse.ReadingCycle Descending _
                                          Select wse).ToList()(0)
                'AG 16/09/2011

                Dim wseSelected = (From wse In pExecutions _
                       Where wse.ElementID = wseMaxReadingCycle.ElementID _
                       Select wse).ToList()

                For Each wse In wseSelected
                    returnDS.twksWSExecutions.ImportRow(wse)
                Next

                For Each wse In wseSelected
                    pExecutions.Remove(wse)
                Next
            End While
        End Sub

        ''' <summary>
        ''' To avoid contamination between previous element group in WS executions last reagent and the fist in the next element group in WS executions
        ''' Search an OrderTest in new element group not contaminated by last OrderTest in previous element group
        ''' Take into account LOW and HIGH contamination level
        ''' 
        ''' No Try - Catch implemented due the caller method implements it
        ''' </summary>
        ''' <param name="pContaminationsDS"></param>
        ''' <param name="pPreviousReagentID">(the nearest reagents use the higher indexs)</param> 
        ''' <param name="pPreviousReplicatesNumber">(the nearest reagents use the higher indexs)</param>
        ''' <param name="pHighContaminationPersistance" ></param>
        ''' <param name="pExecutions"></param>
        ''' <param name="originalorderchanged"></param>
        ''' <param name="addContaminationBetweenGroups"></param>
        ''' <remarks>AG 08/11/2011</remarks>
        Private Sub MoveToAvoidContaminationBetweenElements(ByVal pContaminationsDS As ContaminationsDS, ByVal pPreviousReagentID As List(Of Integer), ByVal pPreviousReplicatesNumber As List(Of Integer), _
                                                            ByVal pHighContaminationPersistance As Integer, ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), ByRef originalorderchanged As Boolean, ByRef addContaminationBetweenGroups As Integer)

            If pPreviousReagentID.Count > 0 Then
                Dim myOTListLinq As List(Of Integer)
                myOTListLinq = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                               Select a.OrderTestID Distinct).ToList

                Dim ReagentContaminatorID = pPreviousReagentID(pPreviousReagentID.Count - 1) 'Last Reagent in previous element group (reverse order)
                Dim ReagentContaminatedID As Integer = -1
                Dim myExecLinqByOT As List(Of ExecutionsDS.twksWSExecutionsRow)
                Dim contaminations As List(Of ContaminationsDS.tparContaminationsRow) = Nothing

                If myOTListLinq.Count > 1 Then
                    Dim itera As Integer = 0
                    Dim insertPosition As Integer = 0

                    '1) Search test for FIRST POSITION: Evaluate contaminations between:
                    'LastReagent(Last) -> Next Reagent(0)
                    'LastReagent(Last-1) -> Next Reagent(0) (special)
                    'If contamination ... search one test not contaminated to be place in FIRST position
                    For Each myOrderTest As Integer In myOTListLinq
                        myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                          Where a.OrderTestID = myOrderTest AndAlso a.ExecutionStatus = "PENDING" _
                                          Select a).ToList

                        If myExecLinqByOT.Count > 0 Then
                            ReagentContaminatedID = myExecLinqByOT(0).ReagentID
                            For jj = pPreviousReagentID.Count - 1 To pPreviousReagentID.Count - pHighContaminationPersistance Step -1
                                If jj >= 0 Then
                                    ReagentContaminatorID = pPreviousReagentID(jj)

                                    If jj = pPreviousReagentID.Count - 1 Then 'search for contamination (low or high level)
                                        contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                            Where wse.ReagentContaminatorID = ReagentContaminatorID _
                                                            AndAlso wse.ReagentContaminatedID = ReagentContaminatedID _
                                                            Select wse).ToList()
                                    Else 'search for contamination (only high level)
                                        contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                            Where wse.ReagentContaminatorID = ReagentContaminatorID _
                                                            AndAlso wse.ReagentContaminatedID = ReagentContaminatedID _
                                                            AndAlso Not wse.IsWashingSolutionR1Null _
                                                            Select wse).ToList()
                                    End If

                                    If contaminations.Count > 0 Then Exit For

                                    'Evaluate only HIGH contamination persistance when OrderTest that uses reagent (pPreviousReagentID.Count - 1) has MaxReplicates < pHighContaminationPersistance
                                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                                    If jj = pPreviousReagentID.Count - 1 AndAlso pPreviousReplicatesNumber(pPreviousReplicatesNumber.Count - 1) >= pHighContaminationPersistance Then
                                        Exit For 'Do not evaluate high contamination persistance
                                    End If

                                Else
                                    Exit For
                                End If
                            Next

                            If contaminations.Count = 0 Then
                                addContaminationBetweenGroups = 0
                                If itera > 0 Then
                                    'New FIRST position test executions
                                    For Each currentExecution In myExecLinqByOT
                                        pExecutions.Remove(currentExecution) 'First remove from pExecutions (original position)
                                        pExecutions.Insert(insertPosition, currentExecution) 'Second add into pExecutions at the begining
                                        insertPosition += 1
                                    Next
                                    originalorderchanged = True
                                End If
                                Exit For 'For Each myOrderTest As ....

                            Else
                                addContaminationBetweenGroups = 1
                            End If
                            'contaminations = Nothing

                        Else
                            addContaminationBetweenGroups = 1
                        End If
                        itera += 1
                    Next

                    If originalorderchanged Then
                        '2) Search test for SECOND POSITION: Evaluate contaminations between:
                        'Next Reagent(0) -> Next Reagent(1) 
                        'LastReagent(Last) -> Next Reagent(1) (special)
                        'If contamination ... search one test not contaminated to be place in SECOND position
                        itera = 0
                        Dim firstPositionContaminatorID As Integer = 0
                        myOTListLinq = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                        Select a.OrderTestID Distinct).ToList 'New query over the new sort

                        For Each myOrderTest As Integer In myOTListLinq
                            myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                              Where a.OrderTestID = myOrderTest AndAlso a.ExecutionStatus = "PENDING" _
                                              Select a).ToList

                            'Define the insert position for the SECOND test
                            If myExecLinqByOT.Count > 0 Then
                                If itera = 0 Then
                                    insertPosition = myExecLinqByOT.Count
                                    firstPositionContaminatorID = myExecLinqByOT(0).ReagentID

                                    'Evaluate only HIGH contamination persistance when OrderTest in FIRST position has MaxReplicates < pHighContaminationPersistance
                                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                                    If myExecLinqByOT.Count >= pHighContaminationPersistance Then
                                        Exit For '(do not evaluate high contamination persistance)
                                    End If
                                End If

                                If itera > 0 AndAlso myExecLinqByOT.Count > 0 Then
                                    ReagentContaminatedID = myExecLinqByOT(0).ReagentID

                                    'search for contamination (low or high level) between FIRST position test (low or high level)
                                    contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                        Where wse.ReagentContaminatorID = firstPositionContaminatorID _
                                                        AndAlso wse.ReagentContaminatedID = ReagentContaminatedID _
                                                        Select wse).ToList()

                                    'If no contamination with the FIRST position test then evaluate the HIGH level contamination with the last reagent
                                    If contaminations.Count = 0 Then
                                        contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                            Where wse.ReagentContaminatorID = pPreviousReagentID(pPreviousReagentID.Count - 1) _
                                                            AndAlso wse.ReagentContaminatedID = ReagentContaminatedID _
                                                            AndAlso Not wse.IsWashingSolutionR1Null _
                                                            Select wse).ToList()
                                    End If


                                    If contaminations.Count = 0 Then
                                        addContaminationBetweenGroups = 0
                                        If itera > 1 Then
                                            'New SECOND position test executions
                                            For Each currentExecution In myExecLinqByOT
                                                pExecutions.Remove(currentExecution) 'First remove from pExecutions (original position)
                                                pExecutions.Insert(insertPosition, currentExecution) 'Second add into pExecutions at the begining
                                                insertPosition += 1
                                            Next
                                            originalorderchanged = True
                                        End If
                                        Exit For 'For Each myOrderTest As ....

                                    Else
                                        addContaminationBetweenGroups = 1
                                    End If
                                    'contaminations = Nothing
                                End If
                            End If
                            itera += 1
                        Next
                    End If


                ElseIf myOTListLinq.Count = 1 Then 'If myOTListLinq.Count > 1 Then (only one test, no movement is possible)
                    myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                      Where a.OrderTestID = myOTListLinq(0) AndAlso a.ExecutionStatus = "PENDING" _
                      Select a).ToList

                    If myExecLinqByOT.Count > 0 Then
                        ReagentContaminatedID = myExecLinqByOT(0).ReagentID

                        'AG 20/12/2011 - Evaluate low and high contamination levels
                        'Dim contaminations = (From wse In pContaminationsDS.tparContaminations _
                        '                     Where wse.ReagentContaminatorID = ReagentContaminatorID _
                        '                     AndAlso wse.ReagentContaminatedID = ReagentContaminatedID _
                        '                     Select wse).ToList()

                        For jj = pPreviousReagentID.Count - 1 To pPreviousReagentID.Count - pHighContaminationPersistance Step -1
                            If jj >= 0 Then
                                ReagentContaminatorID = pPreviousReagentID(jj)

                                If jj = pPreviousReagentID.Count - 1 Then 'search for contamination (low or high level)
                                    contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                        Where wse.ReagentContaminatorID = ReagentContaminatorID _
                                                        AndAlso wse.ReagentContaminatedID = ReagentContaminatedID _
                                                        Select wse).ToList()
                                Else 'search for contamination (only high level)
                                    contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                        Where wse.ReagentContaminatorID = ReagentContaminatorID _
                                                        AndAlso wse.ReagentContaminatedID = ReagentContaminatedID _
                                                        AndAlso Not wse.IsWashingSolutionR1Null _
                                                        Select wse).ToList()
                                End If

                                If contaminations.Count > 0 Then Exit For

                                'Evaluate only HIGH contamination persistance when OrderTest that uses reagent (pPreviousReagentID.Count - 1) has MaxReplicates < pHighContaminationPersistance
                                'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                                If jj = pPreviousReagentID.Count - 1 AndAlso pPreviousReplicatesNumber(pPreviousReplicatesNumber.Count - 1) >= pHighContaminationPersistance Then
                                    Exit For 'Do not evaluate high contamination persistance
                                End If

                            Else
                                Exit For
                            End If
                        Next
                        'AG 20/12/2011 

                        If contaminations.Count > 0 Then
                            addContaminationBetweenGroups = 1
                        End If
                    End If

                End If

                contaminations = Nothing
                myOTListLinq = Nothing
                myExecLinqByOT = Nothing
            End If

        End Sub
#End Region

#Region "CREATE EXECUTIONS - NEW 19-03-2014"

        ''' <summary>
        ''' Create all Executions for the specified Analyzer WorkSession
        ''' (Divide into multiple transactions instead of a unique one) - Avoiding deadlocks
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pWorkInRunningMode">When True, indicates the Analyzer is connected and running a WorkSession</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test for which a Rerun has been requested. Optional parameter</param>
        ''' <param name="pPostDilutionType">Type of post Dilution to apply when a Rerun has been requested. Optional parameter</param>
        ''' <param name="pIsISEModuleReady">Flag indicating if the ISE Module is ready to be used. Optional parameter</param>
        ''' <param name="pISEElectrodesList">String list containing ISE Electrodes (ISE_ResultID) with wrong/pending calibration</param>
        ''' <param name="pPauseMode"></param>
        ''' <param name="pManualRerunFlag">Always TRUE except when autoreruns are triggered</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2011
        ''' Modified by: TR 29/08/2011 - Declarations inside loops have been removed an put outside them to improve the memory use
        '''              AG 15/09/2011 - NO different executions tables: Pending Table + Locked Table: only one table with executions 
        '''                              grouped by patient (pending, locked)
        '''              AG 19/09/2011 - Added new parameter pWorkInRunningMode
        '''              SA 09/11/2011 - For Patient Samples, inform ElementID = CreationOrder (from allOrderTestsDS)
        '''              SA 31/01/2012 - When function is called to manage creation of Executions of a requested Rerun (an OrderTestID is specified)
        '''                              do not update the status of the related Rotor Positions, due to that process is done in function 
        '''                              Manage Repetitions in RepetitionsDelegate Class
        '''              SA 08/02/2012 - Do not update the status of cells in Samples Rotor containing tubes marked as DEPLETED or with FEW volume
        '''              SA 31/05/2012 - Changed the way of searching and deleting PENDING or LOCKED executions
        '''              SA 20/06/2012 - Lock of Controls due to locked needed Blanks or Calibrators is applied only for STD preparations 
        '''                            - Removed filter by TestType=STD when searching STAT Patient request; instead of it, get all different 
        '''                              TestType/TestID/SampleType requested for STAT and filter by each specific TestType when searching the 
        '''                              needed Blanks, Calibrators and Controls to mark them as STAT 
        '''              SA 26/07/2012 - Added optional parameter to indicate if the ISE Module is ready to be used; when it is not ready, the status 
        '''                              of all pending ISE Executions is changed to LOCKED
        '''              SA 31/07/2012 - When the function is called in STANDBY (pWorkInRunningMode = False), before deleting affected Executions, delete 
        '''                              their Readings (to avoid System Error due to violation of FK when delete records in twksWSExecutions)
        '''              SA 07/09/2012 - Added optional parameter pISEElectrodesList to inform the list of ISE Electrodes with wrong/pending calibration. 
        '''                              When this parameter is informed, all Executions for the ISE Tests contained in the list will be locked
        '''              AG 25/03/2013 - Before modify the current executions table get those ordertests locked by LIS
        '''                              After generate the new executions set ExecutionStatus = LOCKED and LockedByLIS = True for the executions of these ordertests
        '''              AG 19/02/2014 - #1514 improvements memory app/sql
        '''              AG 19/03/2014 - #1545 (adapt the CreateWSExecutions active until 19/03/2014 and adapt it for use multiples)
        '''                              New method does not open transaction, dbConnection is nothing unless it has been received from parameter
        '''              AG 30/05/2014 - #1584 new parameter pPauseMode (for recalculate status for executions LOCKED and also PENDING)!!! (in normal running only the LOCKED are recalculated)
        '''              AG 02/06/2014 - #1644 new optional parameter pManualRerunFlag (when FALSE Software cannot use the semaphore because it has been set to busy when ANSPHR started to be processed)
        ''' </remarks> 
        Public Function CreateWSExecutionsMultipleTransactions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                           ByVal pWorkInRunningMode As Boolean, Optional ByVal pOrderTestID As Integer = -1, _
                                           Optional ByVal pPostDilutionType As String = "", Optional ByVal pIsISEModuleReady As Boolean = False, _
                                           Optional ByVal pISEElectrodesList As List(Of String) = Nothing, Optional ByVal pPauseMode As Boolean = False, _
                                           Optional ByVal pManualRerunFlag As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                'Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'AG 02/06/2014 #1644 - Set the semaphore to busy value (EXCEPT when called from auto rerun business)
                If GlobalConstants.CreateWSExecutionsWithSemaphore AndAlso pManualRerunFlag Then
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Waiting (timeout = " & GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS.ToString & ")", "AnalyzerManager.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                    GlobalSemaphores.createWSExecutionsSemaphore.WaitOne(GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS)
                    GlobalSemaphores.createWSExecutionsQueue = 1 'Only 1 thread is allowed, so set to 1 instead of increment ++1 'GlobalSemaphores.createWSExecutionsQueue += 1
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Passed through, semaphore busy", "AnalyzerManager.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                End If

                'AG 19/03/2014 - #1545 - Do not open transaction, use the parameter as connection
                'resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then
                dbConnection = pDBConnection
                'AG 19/03/2014 - #1545

                Dim calledForRerun As Boolean = (pOrderTestID <> -1)
                StartTime = Now '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'Delete all Executions with status PENDING or LOCKED belonging OrderTests not started!!!
                'If an OrderTestID is not informed, only Pending and Locked Executions for Rerun=1 are deleted
                Dim myDAO As New twksWSExecutionsDAO

                'AG 25/03/2013 - Get the current distinct ordertests locked by lis
                resultData = myDAO.GetOrderTestsLockedByLIS(dbConnection, pAnalyzerID, pWorkSessionID, True)
                Dim orderTestLockedByLISList As New List(Of Integer)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    For Each row As ExecutionsDS.twksWSExecutionsRow In DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions
                        If Not row.IsOrderTestIDNull AndAlso Not orderTestLockedByLISList.Contains(row.OrderTestID) Then orderTestLockedByLISList.Add(row.OrderTestID)
                    Next
                End If
                'AG 25/03/2013

                'AJG ADDED BECAUSE THE ANALYZER MODEL IS NEEDED BECAUSE MANAGING CONTAMINATIONS IS ANALYZER DEPENDANT
                Dim activeAnalyzer As String = GetActiveAnalyzer(dbConnection)

                If (Not pWorkInRunningMode) Then 'AG 19/02/2014 - #1514 note that this parameter value is FALSE when called from create repetitions process!!!
                    'Search all Order Tests which Executions can be deleted: those having ALL Executions with status PENDING or LOCKED
                    resultData = myDAO.SearchNotInCourseExecutionsToDelete(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myOrderTestsToDelete As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                        If (myOrderTestsToDelete.twksOrderTests.Count > 0) Then
                            'Delete all Readings of the Executions for all OrderTests returned by the previous called function
                            Dim myReadingsDelegate As New WSReadingsDelegate
                            resultData = myReadingsDelegate.DeleteReadingsForNotInCourseExecutions(dbConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)

                            If (Not resultData.HasError) Then
                                'AG 19/03/2014 - #1545 call the delegate instead of the DAO (with this action we can use dbConnection = Nothing)
                                'Delete the Executions for all OrderTests returned by the previous called function
                                'resultData = myDAO.DeleteNotInCourseExecutionsNEW(dbConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)
                                resultData = DeleteNotInCourseExecutionsNEW(dbConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)

                            End If
                        End If
                    End If

                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    GlobalBase.CreateLogActivity("Not RUNNING: Search and Delete NOT IN COURSE " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                    "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                    StartTime = Now
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                End If

                'NEW 19/09/2011 - Lock / Unlock process:
                'pWorkInRunningMode = FALSE -> Recalculate status for all executions with RerunNumber > 1 with current status is PENDING or LOCKED
                '                              (note that in this case ALL the PENDING or LOCKED executions with RerunNumber = 1 have been deleted in mehtod DeleteNotInCurseExecutions)
                '
                'pWorkInRuuningMode = TRUE ->  Recalculate status for all existing executions with status PENDING or LOCKED
                'Table twksWSExecutions is updated at this point
                If (Not calledForRerun) Then

                    'AG 28/05/2014 - #1644 - Do not delete readings here!! They will be removed when the new preparation receives his 1st reading
                    ''In RUNNING Mode, delete all Readings of all LOCKED Executions 
                    'Dim myReadingsDelegate As New WSReadingsDelegate
                    'resultData = myReadingsDelegate.DeleteReadingsForNotInCourseExecutions(dbConnection, pAnalyzerID, pWorkSessionID, Nothing)

                    'AG 30/05/2014 - #1644 new parameter pPauseMode required
                    resultData = RecalculateStatusForNotDeletedExecutionsNEW(dbConnection, pAnalyzerID, pWorkSessionID, pWorkInRunningMode, pPauseMode)

                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    GlobalBase.CreateLogActivity("Recalculate Status " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                    "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                    StartTime = Now
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                End If
                'AG 19/09/2011

                'Now we can create executions for all ordertests not started (with no executions in twksWSExecutions table) ... <the initial process>
                If (Not resultData.HasError) Then
                    'Get detailed information of all Order Tests to be executed in the WS
                    Dim allOrderTestsDS As OrderTestsForExecutionsDS
                    Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

                    resultData = myWSOrderTestsDelegate.GetInfoOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        allOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsForExecutionsDS)

                        If (allOrderTestsDS.OrderTestsForExecutionsTable.Rows.Count > 0) Then
                            'Get all executions for BLANKS included in the WorkSession
                            Dim myBlankExecutionsDS As New ExecutionsDS
                            resultData = CreateBlankExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myBlankExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                Dim myOrderTestID As Integer = -1
                                Dim blankInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                For Each rowBlank As ExecutionsDS.twksWSExecutionsRow In myBlankExecutionsDS.twksWSExecutions
                                    If (rowBlank.OrderTestID <> myOrderTestID) Then
                                        myOrderTestID = rowBlank.OrderTestID
                                        'Search information for the Blank
                                        blankInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                    Where a.OrderTestID = myOrderTestID _
                                                   Select a).ToList()
                                    End If

                                    '...and complete fields for the Blank
                                    If (blankInfo.Count = 1) Then
                                        rowBlank.BeginEdit()
                                        rowBlank.TestID = blankInfo(0).TestID
                                        rowBlank.ReadingCycle = blankInfo(0).ReadingCycle
                                        rowBlank.ReagentID = blankInfo(0).ReagentID
                                        rowBlank.OrderID = blankInfo(0).OrderID
                                        rowBlank.ElementID = NullElementID     'rowBlank.ReagentID 'RH 29/09/2011
                                        rowBlank.EndEdit()
                                    End If
                                Next
                                myBlankExecutionsDS.AcceptChanges()
                                blankInfo = Nothing 'AG 19/02/2014 - #1514
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Get Executions For BLANKS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                            'Get all executions for CALIBRATORS included in the WorkSession
                            Dim myCalibratorExecutionsDS As ExecutionsDS = Nothing

                            If (Not resultData.HasError) Then
                                resultData = CreateCalibratorExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myCalibratorExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                    Dim myOrderTestID As Integer = -1
                                    Dim calibInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                    For Each rowCalib As ExecutionsDS.twksWSExecutionsRow In myCalibratorExecutionsDS.twksWSExecutions
                                        'Dim calibInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                                        If (rowCalib.OrderTestID <> myOrderTestID) Then
                                            myOrderTestID = rowCalib.OrderTestID
                                            'Search information for the Calibrator
                                            calibInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                        Where a.OrderTestID = myOrderTestID _
                                                       Select a).ToList()
                                        End If

                                        '...and complete fields for the Calibrator
                                        If (calibInfo.Count = 1) Then
                                            rowCalib.BeginEdit()
                                            rowCalib.TestID = calibInfo(0).TestID
                                            rowCalib.SampleType = calibInfo(0).SampleType
                                            rowCalib.ReadingCycle = calibInfo(0).ReadingCycle
                                            rowCalib.ReagentID = calibInfo(0).ReagentID
                                            rowCalib.OrderID = calibInfo(0).OrderID
                                            rowCalib.ElementID = calibInfo(0).ElementID
                                            rowCalib.EndEdit()
                                        End If
                                    Next
                                    myCalibratorExecutionsDS.AcceptChanges()
                                    calibInfo = Nothing 'AG 19/02/2014 - #1514
                                End If
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Get Executions For CALIBRATORS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                            'Get all executions for CONTROLS included in the WorkSession
                            Dim myControlExecutionsDS As ExecutionsDS = Nothing

                            If (Not resultData.HasError) Then
                                resultData = CreateControlExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myControlExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                    Dim myOrderTestID As Integer = -1
                                    Dim controlInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                    For Each rowControl As ExecutionsDS.twksWSExecutionsRow In myControlExecutionsDS.twksWSExecutions
                                        'Dim controlInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                                        If (rowControl.OrderTestID <> myOrderTestID) Then
                                            myOrderTestID = rowControl.OrderTestID
                                            'Search information for the Control
                                            controlInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                          Where a.OrderTestID = myOrderTestID _
                                                         Select a).ToList()
                                        End If

                                        '...and complete fields for the Control (more than one record is possible when several
                                        'controls are used for the TestType/Test/Sample Type)
                                        If (controlInfo.Count >= 1) Then
                                            rowControl.BeginEdit()
                                            rowControl.TestID = controlInfo(0).TestID
                                            rowControl.SampleType = controlInfo(0).SampleType
                                            rowControl.ReadingCycle = controlInfo(0).ReadingCycle
                                            rowControl.ReagentID = controlInfo(0).ReagentID
                                            rowControl.OrderID = controlInfo(0).OrderID
                                            rowControl.ElementID = controlInfo(0).ElementID

                                            If orderTestLockedByLISList.Contains(rowControl.OrderTestID) Then rowControl.ExecutionStatus = "LOCKED" 'AG 25/03/2013 - Locked by LIS not by volume missing
                                            rowControl.EndEdit()
                                        End If
                                    Next
                                    controlInfo = Nothing 'AG 19/02/2014 - #1514
                                End If
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Get Executions For CONTROLS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                            'Get all executions for PATIENT SAMPLES included in the WorkSession
                            Dim myPatientExecutionsDS As ExecutionsDS = Nothing

                            If (Not resultData.HasError) Then
                                resultData = CreatePatientExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myPatientExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                    Dim myOrderTestID As Integer = -1
                                    Dim patientInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                    For Each rowPatient As ExecutionsDS.twksWSExecutionsRow In myPatientExecutionsDS.twksWSExecutions
                                        'Dim patientInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                                        If (rowPatient.OrderTestID <> myOrderTestID) Then
                                            myOrderTestID = rowPatient.OrderTestID

                                            'Search information for the Patient Sample
                                            patientInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                          Where a.OrderTestID = myOrderTestID _
                                                         Select a).ToList()
                                        End If

                                        '...and complete fields for the Patient Sample
                                        If (patientInfo.Count = 1) Then
                                            rowPatient.BeginEdit()
                                            rowPatient.TestID = patientInfo(0).TestID
                                            rowPatient.SampleType = patientInfo(0).SampleType
                                            rowPatient.ReadingCycle = patientInfo(0).ReadingCycle
                                            rowPatient.ReagentID = patientInfo(0).ReagentID
                                            rowPatient.OrderID = patientInfo(0).OrderID

                                            'AG 27/04/2012 Activate this line again (RH 08/03/2012 Remove this line)
                                            rowPatient.ElementID = patientInfo(0).CreationOrder 'Convert.ToInt32(patientInfo(0).OrderID.Substring(8, 4))
                                            If orderTestLockedByLISList.Contains(rowPatient.OrderTestID) Then rowPatient.ExecutionStatus = "LOCKED" 'AG 25/03/2013 - Locked by LIS not by volume missing
                                            rowPatient.EndEdit()
                                        End If
                                    Next
                                    patientInfo = Nothing 'AG 19/02/2014 - #1514
                                End If
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Get Executions For PATIENTS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                            If (Not resultData.HasError) Then
                                'Search all locked BLANKS to lock also all Calibrators, Control and Patient Samples
                                'for the same Standard Test
                                Dim lstLockedBlanks As List(Of Integer)
                                lstLockedBlanks = (From a In myBlankExecutionsDS.twksWSExecutions _
                                                  Where a.ExecutionStatus = "LOCKED" _
                                                 Select a.TestID Distinct).ToList()

                                Dim myTestID As Integer
                                Dim lstLockedCtrls As List(Of ExecutionsDS.twksWSExecutionsRow)
                                Dim lstLockedCalibs As List(Of ExecutionsDS.twksWSExecutionsRow)
                                Dim lstLockedPatients As List(Of ExecutionsDS.twksWSExecutionsRow)

                                For Each lockedBlank In lstLockedBlanks
                                    myTestID = lockedBlank
                                    '...LOCK the correspondent Calibrators
                                    lstLockedCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                                                      Where a.TestID = myTestID _
                                                    AndAlso a.ExecutionStatus <> "LOCKED" _
                                                     Select a).ToList()

                                    For Each lockedCalib In lstLockedCalibs
                                        lockedCalib.BeginEdit()
                                        lockedCalib.ExecutionStatus = "LOCKED"
                                        lockedCalib.EndEdit()
                                    Next

                                    '...LOCK the correspondent Controls - Apply only to STD Preparations
                                    lstLockedCtrls = (From a In myControlExecutionsDS.twksWSExecutions _
                                                      Where a.ExecutionType = "PREP_STD" _
                                                    AndAlso a.TestID = myTestID _
                                                    AndAlso a.ExecutionStatus <> "LOCKED" _
                                                     Select a).ToList()

                                    For Each lockedCtrl In lstLockedCtrls
                                        lockedCtrl.BeginEdit()
                                        lockedCtrl.ExecutionStatus = "LOCKED"
                                        lockedCtrl.EndEdit()
                                    Next

                                    '...LOCK the correspondent Patients
                                    lstLockedPatients = (From a In myPatientExecutionsDS.twksWSExecutions _
                                                        Where a.ExecutionType = "PREP_STD" _
                                                      AndAlso a.TestID = myTestID _
                                                      AndAlso a.ExecutionStatus <> "LOCKED" _
                                                       Select a).ToList()

                                    For Each lockedPatient In lstLockedPatients
                                        lockedPatient.BeginEdit()
                                        lockedPatient.ExecutionStatus = "LOCKED"
                                        lockedPatient.EndEdit()
                                    Next
                                Next

                                myCalibratorExecutionsDS.AcceptChanges()
                                myControlExecutionsDS.AcceptChanges()
                                myPatientExecutionsDS.AcceptChanges()

                                'AG 19/02/2014 - #1514
                                lstLockedBlanks = Nothing
                                lstLockedCtrls = Nothing
                                lstLockedCalibs = Nothing
                                lstLockedPatients = Nothing
                                'AG 19/02/2014 - #1514
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Locks for BLANKS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                            If (Not resultData.HasError) Then
                                'Search all locked CALIBRATORS to lock also all Controls and Patient Samples
                                'for the same Standard Test and SampleType
                                Dim lstLockedCalibs As List(Of ExecutionsDS.twksWSExecutionsRow)
                                lstLockedCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                                                  Where a.ExecutionStatus = "LOCKED" _
                                                 Select a Order By a.OrderTestID).ToList()

                                Dim myOrderTestID As Integer = -1
                                Dim lstAlternativeST As List(Of String)
                                Dim myTestID As Integer
                                Dim mySampleType As String
                                Dim lstLockedCtrls As New List(Of ExecutionsDS.twksWSExecutionsRow)
                                Dim lstLockedPatients As List(Of ExecutionsDS.twksWSExecutionsRow)

                                For Each lockedCalib In lstLockedCalibs
                                    If (myOrderTestID <> lockedCalib.OrderTestID) Then
                                        'First verify if the Calibrator is used as alternative of another Sample Types for the same Test
                                        myOrderTestID = lockedCalib.OrderTestID
                                        lstAlternativeST = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                           Where a.SampleClass = "CALIB" _
                                                         AndAlso Not a.IsAlternativeOrderTestIDNull _
                                                         AndAlso a.AlternativeOrderTestID = myOrderTestID _
                                                          Select a.SampleType Distinct).ToList

                                        For i As Integer = 0 To (lstAlternativeST.Count)
                                            myTestID = lockedCalib.TestID
                                            mySampleType = lockedCalib.SampleType

                                            If (i > 0) Then mySampleType = lstAlternativeST(i - 1)

                                            '...LOCK the correspondent Controls - Apply only to STD Preparations
                                            lstLockedCtrls = (From a In myControlExecutionsDS.twksWSExecutions _
                                                              Where a.ExecutionType = "PREP_STD" _
                                                            AndAlso a.TestID = myTestID _
                                                            AndAlso a.SampleType = mySampleType _
                                                            AndAlso a.ExecutionStatus <> "LOCKED" _
                                                             Select a).ToList()

                                            For Each lockedCtrl In lstLockedCtrls
                                                lockedCtrl.BeginEdit()
                                                lockedCtrl.ExecutionStatus = "LOCKED"
                                                lockedCtrl.EndEdit()
                                            Next

                                            '...LOCK the correspondent Patients.
                                            lstLockedPatients = (From a In myPatientExecutionsDS.twksWSExecutions _
                                                                Where a.ExecutionType = "PREP_STD" _
                                                              AndAlso a.TestID = myTestID _
                                                              AndAlso a.SampleType = mySampleType _
                                                              AndAlso a.ExecutionStatus <> "LOCKED" _
                                                               Select a).ToList()

                                            For Each lockedPatient In lstLockedPatients
                                                lockedPatient.BeginEdit()
                                                lockedPatient.ExecutionStatus = "LOCKED"
                                                lockedPatient.EndEdit()
                                            Next
                                        Next
                                    End If
                                Next

                                myControlExecutionsDS.AcceptChanges()
                                myPatientExecutionsDS.AcceptChanges()

                                'AG 19/02/2014 - #1514
                                lstLockedCalibs = Nothing
                                lstAlternativeST = Nothing
                                lstLockedCtrls = Nothing
                                lstLockedPatients = Nothing
                                'AG 19/02/2014 - #1514
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Locks for CALIBRATORS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                            If (Not resultData.HasError) Then
                                If (Not calledForRerun) Then
                                    'Search all Standard TestType/TestID/SampleType of all Patients requested for STAT, to mark as STAT
                                    'all the needed Blanks, Calibrators and Controls
                                    Dim lstSTATS As List(Of String)
                                    lstSTATS = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                               Where a.SampleClass = "PATIENT" _
                                             AndAlso a.StatFlag = True _
                                              Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList

                                    Dim pos1 As Integer
                                    Dim pos2 As Integer
                                    Dim myTestID As Integer
                                    Dim myTestType As String
                                    Dim mySampleType As String
                                    Dim myOrderTestID As Integer
                                    Dim lstBlanks As List(Of ExecutionsDS.twksWSExecutionsRow)
                                    Dim lstBlkCalibCtrls As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                                    Dim lstCalibs As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
                                    Dim lstCtrls As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514

                                    'SA 19/06/2012 - Filter linq also by TestType
                                    For Each statPatient In lstSTATS
                                        pos1 = statPatient.IndexOf("|")
                                        myTestType = statPatient.Substring(0, pos1)

                                        pos2 = statPatient.LastIndexOf("|")
                                        mySampleType = statPatient.Substring(pos2 + 1)
                                        myTestID = Convert.ToInt32(statPatient.Substring(pos1 + 1, pos2 - pos1 - 1))


                                        'Search all Controls, Calibrators and Blanks for the TestType/TestID/SampleType
                                        lstBlkCalibCtrls = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                            Where a.TestType = myTestType _
                                                          AndAlso a.TestID = myTestID _
                                                          AndAlso ((a.SampleClass = "BLANK") _
                                                           OrElse (a.SampleClass = "CALIB" AndAlso a.SampleType = mySampleType) _
                                                           OrElse (a.SampleClass = "CTRL" AndAlso a.SampleType = mySampleType)) _
                                                           Select a Order By a.SampleClass).ToList()

                                        For Each blkCalibCtrlRow In lstBlkCalibCtrls
                                            myOrderTestID = blkCalibCtrlRow.OrderTestID
                                            If (blkCalibCtrlRow.SampleClass = "CALIB") AndAlso (Not blkCalibCtrlRow.IsAlternativeOrderTestIDNull) Then
                                                myOrderTestID = blkCalibCtrlRow.AlternativeOrderTestID
                                            End If

                                            'Search the OrderTestID to update the StatFlag
                                            If (blkCalibCtrlRow.SampleClass = "BLANK") Then
                                                lstBlanks = (From a In myBlankExecutionsDS.twksWSExecutions _
                                                            Where a.OrderTestID = myOrderTestID _
                                                           Select a).ToList()

                                                For Each blank In lstBlanks
                                                    blank.StatFlag = True
                                                Next
                                            ElseIf (blkCalibCtrlRow.SampleClass = "CALIB") Then
                                                'Dim lstCalibs As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
                                                lstCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                                                            Where a.OrderTestID = myOrderTestID _
                                                           Select a).ToList()

                                                For Each calibrator In lstCalibs
                                                    calibrator.StatFlag = True
                                                Next
                                            ElseIf (blkCalibCtrlRow.SampleClass = "CTRL") Then
                                                'Dim lstCtrls As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
                                                lstCtrls = (From a In myControlExecutionsDS.twksWSExecutions _
                                                            Where a.OrderTestID = myOrderTestID _
                                                           Select a).ToList()

                                                For Each control In lstCtrls
                                                    control.StatFlag = True
                                                Next
                                            End If
                                        Next
                                        myBlankExecutionsDS.AcceptChanges()
                                        myCalibratorExecutionsDS.AcceptChanges()
                                        myControlExecutionsDS.AcceptChanges()
                                    Next

                                    'AG 19/02/2014 - #1514
                                    lstSTATS = Nothing
                                    lstBlanks = Nothing
                                    lstBlkCalibCtrls = Nothing
                                    lstCalibs = Nothing
                                    lstCtrls = Nothing
                                    'AG 19/02/2014 - #1514
                                End If
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Mark STATS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***


                            'Unify all locked Executions and all pending Executions (all types) 
                            Dim lockedExecutionsDS As New ExecutionsDS
                            Dim pendingExecutionsDS As New ExecutionsDS

                            If (Not resultData.HasError) Then
                                'Move executions for BLANKS
                                For Each blank As ExecutionsDS.twksWSExecutionsRow In myBlankExecutionsDS.twksWSExecutions
                                    If (blank.ExecutionStatus = "LOCKED") Then
                                        lockedExecutionsDS.twksWSExecutions.ImportRow(blank)
                                    ElseIf (blank.ExecutionStatus = "PENDING") Then
                                        pendingExecutionsDS.twksWSExecutions.ImportRow(blank)
                                    End If
                                Next

                                'AG 19/02/2014 - #1514 use merge instead of loops
                                ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                'Next
                                pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                lockedExecutionsDS.Clear()
                                'AG 15/09/2011

                                'Move executions for CALIBRATORS
                                For Each calib As ExecutionsDS.twksWSExecutionsRow In myCalibratorExecutionsDS.twksWSExecutions
                                    If (calib.ExecutionStatus = "LOCKED") Then
                                        lockedExecutionsDS.twksWSExecutions.ImportRow(calib)
                                    ElseIf (calib.ExecutionStatus = "PENDING") Then
                                        pendingExecutionsDS.twksWSExecutions.ImportRow(calib)
                                    End If
                                Next

                                'AG 19/02/2014 - #1514 use merge instead of loops
                                ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                'Next
                                pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                lockedExecutionsDS.Clear()
                                'AG 15/09/2011

                                'Move executions for CONTROLS
                                For Each ctrl As ExecutionsDS.twksWSExecutionsRow In myControlExecutionsDS.twksWSExecutions
                                    If (ctrl.ExecutionStatus = "LOCKED") Then
                                        lockedExecutionsDS.twksWSExecutions.ImportRow(ctrl)
                                    ElseIf (ctrl.ExecutionStatus = "PENDING") Then
                                        pendingExecutionsDS.twksWSExecutions.ImportRow(ctrl)
                                    End If
                                Next

                                'AG 19/02/2014 - #1514 use merge instead of loops
                                ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                'Next
                                pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                lockedExecutionsDS.Clear()
                                'AG 15/09/2011

                                'Move executions for PATIENTS
                                For Each patient As ExecutionsDS.twksWSExecutionsRow In myPatientExecutionsDS.twksWSExecutions
                                    If (patient.ExecutionStatus = "LOCKED") Then
                                        lockedExecutionsDS.twksWSExecutions.ImportRow(patient)
                                    ElseIf (patient.ExecutionStatus = "PENDING") Then
                                        pendingExecutionsDS.twksWSExecutions.ImportRow(patient)
                                    End If
                                Next

                                'AG 19/02/2014 - #1514 use merge instead of loops
                                ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                'Next
                                pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                lockedExecutionsDS.Clear()
                                'AG 15/09/2011

                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                GlobalBase.CreateLogActivity("Unify all Pending and Locked " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                                StartTime = Now
                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                                If (Not calledForRerun) Then
                                    'Sort PENDING executions 
                                    If (pendingExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                        'AG 30/11/2011 - NEW: When a patient has Ise & std test executions the ISE executions are the first

                                        'AG 15/09/2011 - Locked executions at the botton his sample
                                        'pendingExecutionsDS.twksWSExecutions.DefaultView.Sort = _
                                        '        "StatFlag DESC, SampleClass, ElementID, SampleType, " & _
                                        '        "ExecutionType DESC, ReadingCycle DESC"
                                        'pendingExecutionsDS.twksWSExecutions.DefaultView.Sort = _
                                        '    "StatFlag DESC, SampleClass, ElementID, SampleType, " & _
                                        '    "ExecutionType DESC, ExecutionStatus DESC, ReadingCycle DESC"
                                        ''AG 15/09/2011

                                        pendingExecutionsDS.twksWSExecutions.DefaultView.Sort = "StatFlag DESC, SampleClass, ElementID, SampleType, " & _
                                                                                                "ExecutionType, ExecutionStatus DESC, ReadingCycle DESC"
                                        'AG 30/11/2011 - NEW: When a patient has Ise & std test executions the ISE executions are the first

                                        Dim executionDataDS As New ExecutionsDS
                                        'AG 12/07/2012
                                        'For Each pendingExecution As DataRow In pendingExecutionsDS.twksWSExecutions.DefaultView.ToTable.Rows
                                        '    executionDataDS.twksWSExecutions.ImportRow(pendingExecution)
                                        'Next
                                        executionDataDS = CType(pendingExecutionsDS, ExecutionsDS)
                                        'AG 12/07/2012

                                        'AG 28/11/2011 - Code for evaluate method execution time (comment after the evaluation has been performed)
                                        'Dim startTime As DateTime = Now

                                        'Sort by Contamination
                                        Dim sorter = New WSSorter(executionDataDS, activeAnalyzer)
                                        If sorter.SortWSExecutionsByContamination(dbConnection) Then
                                            resultData.SetDatos = sorter.Executions
                                            executionDataDS = sorter.Executions
                                        Else
                                            resultData.SetDatos = Nothing
                                            resultData.HasError = True
                                        End If

                                        If (Not resultData.HasError AndAlso Not executionDataDS Is Nothing) Then

                                            'Sort Orders by ReadingCycle
                                            resultData = SortWSExecutionsByElementGroupTime(executionDataDS)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                                'Sort Orders by Contamination
                                                'AG 07/11/2011
                                                'resultData = SortWSExecutionsByElementGroupContamination(dbConnection, executionDataDS) 'RH 29092011
                                                resultData = SortWSExecutionsByElementGroupContaminationNew(activeAnalyzer, dbConnection, executionDataDS) 'AG 07/11/2011

                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                                End If

                                            End If
                                        End If

                                        'AG 28/11/2011 - Code for evaluate method execution time (comment after the evaluation has been performed)
                                        'Dim elapsedTime As Double = 0
                                        'elapsedTime = Now.Subtract(startTime).TotalMilliseconds

                                        'Finally, save the sorted PENDING executions
                                        If (Not resultData.HasError) Then
                                            'AG 19/03/2014 - #1545 - call method in delegate instead of in DAO
                                            'resultData = myDAO.Create(dbConnection, executionDataDS)
                                            resultData = Create(dbConnection, executionDataDS)
                                        End If
                                    End If

                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    GlobalBase.CreateLogActivity("Sort and create all PENDING " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                    "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                                    StartTime = Now
                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                Else
                                    'Save PENDING executions  
                                    'AG 19/03/2014 - #1545 - call method in delegate instead of in DAO
                                    'resultData = myDAO.Create(dbConnection, pendingExecutionsDS)
                                    resultData = Create(dbConnection, pendingExecutionsDS)
                                End If

                                'Save LOCKED executions
                                If (lockedExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                    If (Not resultData.HasError) Then
                                        'AG 19/03/2014 - #1545 - call method in delegate instead of in DAO
                                        'resultData = myDAO.Create(dbConnection, lockedExecutionsDS)
                                        resultData = Create(dbConnection, lockedExecutionsDS)
                                    End If
                                End If

                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                GlobalBase.CreateLogActivity("Save all LOCKED " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                                StartTime = Now
                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                                If (Not resultData.HasError) Then
                                    'AG 19/09/2011 - Update of the Paused flag is performed only when no Running mode
                                    '              - When working in Running mode, no pending executions are deleted, then SW has only to update status 
                                    '                (business already performed)
                                    If (Not calledForRerun AndAlso Not pWorkInRunningMode) Then
                                        Dim tempExecutionDS As New ExecutionsDS
                                        Dim myWSPausedOrderTestsDS As New WSPausedOrderTestsDS
                                        Dim myWSPausedOrderTestsDelegate As New WSPausedOrderTestsDelegate

                                        resultData = myWSPausedOrderTestsDelegate.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID)
                                        If (Not resultData.HasError) Then
                                            myWSPausedOrderTestsDS = DirectCast(resultData.SetDatos, WSPausedOrderTestsDS)
                                            'AG 19/03/2014 - #1545 - avoid loop and call UpdatePaused (2on signature)
                                            'For Each pausedOTRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow In myWSPausedOrderTestsDS.twksWSPausedOrderTests.Rows
                                            '    resultData = myDAO.ReadByOrderTestIDAndRerunNumber(dbConnection, pausedOTRow.OrderTestID, pausedOTRow.RerunNumber)
                                            '    If resultData.HasError Then Exit For

                                            '    tempExecutionDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                            '    If (tempExecutionDS.twksWSExecutions.Count > 0) Then
                                            '        'If found, then update
                                            '        For Each executionRow As ExecutionsDS.twksWSExecutionsRow In tempExecutionDS.twksWSExecutions.Rows
                                            '            resultData = myDAO.UpdatePaused(dbConnection, True, executionRow.ExecutionID)
                                            '            If (resultData.HasError) Then Exit For
                                            '        Next
                                            '    End If
                                            'Next
                                            Dim myOT As New List(Of Integer)
                                            Dim myRerun As New List(Of Integer)
                                            For Each auxRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow In myWSPausedOrderTestsDS.twksWSPausedOrderTests
                                                If Not auxRow.IsOrderTestIDNull AndAlso Not myOT.Contains(auxRow.OrderTestID) Then
                                                    myOT.Add(auxRow.OrderTestID)
                                                    myRerun.Add(auxRow.RerunNumber)
                                                End If
                                            Next
                                            If myOT.Count > 0 AndAlso myOT.Count = myRerun.Count Then
                                                resultData = UpdatePaused(dbConnection, myOT, myRerun, True, pAnalyzerID, pWorkSessionID)
                                            End If
                                            myOT.Clear()
                                            myRerun.Clear()
                                            myOT = Nothing
                                            myRerun = Nothing
                                            'AG 19/03/2014 - #1545
                                        End If
                                    End If
                                    'AG 19/09/2011
                                End If

                                If (Not resultData.HasError) Then
                                    If (Not calledForRerun) Then
                                        'Do nothing ... business has been already performed in previous call to RecalculateStatusForNotDeletedExecutions method
                                    Else
                                        'If function was called for a Rerun, change the Status of the Order Test to PENDING
                                        Dim myOrderTestDelegate As New OrderTestsDelegate
                                        resultData = myOrderTestDelegate.UpdateStatusByOrderTestID(dbConnection, pOrderTestID, "PENDING")
                                    End If

                                    'AG 25/03/2013 - Update executions locked by lis
                                    If Not resultData.HasError AndAlso orderTestLockedByLISList.Count > 0 Then
                                        'AG 19/03/2014 - #1545 - call method in delegate instead of in DAO
                                        'resultData = myDAO.UpdateLockedByLIS(dbConnection, orderTestLockedByLISList, True)
                                        resultData = UpdateLockedByLIS(dbConnection, orderTestLockedByLISList, True)
                                    End If
                                    'AG 25/03/2013

                                End If
                            End If
                        End If
                    End If
                End If

                'AG 20/09/2011 - When the WorkSession is modified, all samples rotor status can change 
                '                when new tests are added using a tube with status finished (finished -> pending)
                If (Not resultData.HasError AndAlso Not calledForRerun) Then
                    'Read the current content of all positions in Samples Rotor
                    Dim rcp_del As New WSRotorContentByPositionDelegate
                    resultData = rcp_del.ReadByCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", -1)

                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                        Dim samplesRotorDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                        'Get only positions with tubes for required WorkSession Elements not marked as Depleted or with not enough volume
                        Dim linqRes As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                        linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In samplesRotorDS.twksWSRotorContentByPosition _
                              Where Not a.IsElementIDNull _
                                AndAlso a.Status <> "DEPLETED" _
                                AndAlso a.Status <> "FEW" _
                                 Select a).ToList

                        Dim newStatus As String = ""
                        For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In linqRes
                            newStatus = row.Status
                            resultData = rcp_del.UpdateSamplePositionStatus(dbConnection, -1, pWorkSessionID, pAnalyzerID, row.ElementID, _
                                                                            row.TubeContent, 1, newStatus, row.CellNumber)
                        Next
                        linqRes = Nothing 'AG 19/02/2014 - #1514

                    End If
                End If
                'AG 20/09/2011

                If (Not resultData.HasError) Then

                    'check if there are Electrodes with wrong Calibration
                    If (Not pISEElectrodesList Is Nothing AndAlso pISEElectrodesList.Count > 0) Then
                        For Each electrode As String In pISEElectrodesList
                            resultData = UpdateStatusByISETestType(dbConnection, pWorkSessionID, pAnalyzerID, electrode, "PENDING", "LOCKED")
                            If (resultData.HasError) Then Exit For
                        Next
                    ElseIf (Not pIsISEModuleReady) Then
                        'ISE Module cannot be used; all pending ISE Preparations are LOCKED
                        resultData = UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "PREP_ISE", "PENDING", "LOCKED")
                    End If

                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("Final Processing " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                StartTime = Now
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'AG 19/03/2014 - #1545 - New method does not open transaction, so do not close it
                'If (Not resultData.HasError) Then
                '    'When the Database Connection was opened locally, then the Commit is executed
                '    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                'Else
                '    'When the Database Connection was opened locally, then the Rollback is executed
                '    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                'End If
                'AG 19/03/2014 - #1545

                orderTestLockedByLISList = Nothing 'AG 19/02/2014 - #1514

                'AG 19/03/2014 - #1545
                '    End If
                'End If
                'AG 19/03/2014 - #1545

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                'If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection) 'AG 19/03/2014 - #1545 - New method does not open transaction, so do not close it

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Error, False)
            Finally
                'If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close() 'AG 19/03/2014 - #1545 - New method does not open transaction, so do not close it

                'AG 02/06/2014 #1644 - Set the semaphore to free value (EXCEPT when called from auto rerun business)
                If GlobalConstants.CreateWSExecutionsWithSemaphore AndAlso pManualRerunFlag Then
                    GlobalSemaphores.createWSExecutionsSemaphore.Release()
                    GlobalSemaphores.createWSExecutionsQueue = 0 'Only 1 thread is allowed, so reset to 0 instead of decrement --1 'GlobalSemaphores.createWSExecutionsQueue -= 1
                    'Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Released, semaphore free", "AnalyzerManager.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
                End If

            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the active analyzer model
        ''' </summary>
        ''' <param name="dbConnection">Connection to db</param>
        ''' <returns>The analyzer model that is currently connected to</returns>
        ''' <remarks>
        ''' Created on 19/03/2015 by AJG
        ''' </remarks>
        Public Function GetActiveAnalyzer(ByVal dbConnection As SqlConnection) As String
            Dim resultData As GlobalDataTO = Nothing
            Dim myAnalyzerDAO As New tcfgAnalyzersDAO
            Dim activeAnalyzer As String = ""
            resultData = myAnalyzerDAO.ReadByAnalyzerActive(dbConnection)
            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                Dim myDS = CType(resultData.SetDatos, AnalyzersDS)
                activeAnalyzer = myDS.tcfgAnalyzers(0).Item("AnalyzerModel").ToString()
            End If
            Return activeAnalyzer
        End Function

        ''' <summary>
        ''' Gets the active analyzer model
        ''' </summary>
        ''' <param name="dbConnection">Connection to db</param>
        ''' <returns>The analyzer model that is currently connected to</returns>
        ''' <remarks>
        ''' Created on 19/03/2015 by AJG
        ''' </remarks>
        Public Function GetActiveAnalyzerId(ByVal dbConnection As SqlConnection) As String
            Dim resultData As GlobalDataTO = Nothing
            Dim myAnalyzerDAO As New tcfgAnalyzersDAO
            Dim activeAnalyzer As String = ""
            resultData = myAnalyzerDAO.ReadByAnalyzerActive(dbConnection)
            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                Dim myDS = CType(resultData.SetDatos, AnalyzersDS)
                activeAnalyzer = myDS.tcfgAnalyzers(0).Item("AnalyzerId").ToString()
            End If
            Return activeAnalyzer
        End Function


        ''' <summary>
        ''' Delete all Executions for a group of Order Tests that fulfill the following condition: ALL their Executions 
        ''' have Execution Status PENDING or LOCKED 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsListDS">Typed DataSet OrderTestsDS containing the list of OrderTestIDs which Executions can be 
        '''                                 deleted (because all of them have status PENDING or LOCKED)</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' AG 19/03/2014 - create the delegate - #1545
        ''' </remarks>
        Public Function DeleteNotInCourseExecutionsNEW(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                      ByVal pOrderTestsListDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.DeleteNotInCourseExecutionsNEW(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestsListDS)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.DeleteNotInCourseExecutionsNEW", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Create a group of Executions in table twksWSExecutions
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pExecution">Typed Dataset ExecutionsDS with all Executions to create</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' AG 19/03/2014 - Creation #1545
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecution As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.Create(dbConnection, pExecution)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.Create", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "OUT OF DATE 19-03-2014 - CREATE EXECUTIONS - NEW 15-09-2011"

        ''' <summary>
        ''' Create all Executions for the specified Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pWorkInRunningMode">When True, indicates the Analyzer is connected and running a WorkSession</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test for which a Rerun has been requested. Optional parameter</param>
        ''' <param name="pPostDilutionType">Type of post Dilution to apply when a Rerun has been requested. Optional parameter</param>
        ''' <param name="pIsISEModuleReady">Flag indicating if the ISE Module is ready to be used. Optional parameter</param>
        ''' <param name="pISEElectrodesList">String list containing ISE Electrodes (ISE_ResultID) with wrong/pending calibration</param>
        ''' <param name="pPauseMode"></param>
        ''' <param name="pManualRerunFlag">Always TRUE except when autoreruns are triggered</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2011
        ''' Modified by: TR 29/08/2011 - Declarations inside loops have been removed an put outside them to improve the memory use
        '''              AG 15/09/2011 - NO different executions tables: Pending Table + Locked Table: only one table with executions 
        '''                              grouped by patient (pending, locked)
        '''              AG 19/09/2011 - Added new parameter pWorkInRunningMode
        '''              SA 09/11/2011 - For Patient Samples, inform ElementID = CreationOrder (from allOrderTestsDS)
        '''              SA 31/01/2012 - When function is called to manage creation of Executions of a requested Rerun (an OrderTestID is specified)
        '''                              do not update the status of the related Rotor Positions, due to that process is done in function 
        '''                              Manage Repetitions in RepetitionsDelegate Class
        '''              SA 08/02/2012 - Do not update the status of cells in Samples Rotor containing tubes marked as DEPLETED or with FEW volume
        '''              SA 31/05/2012 - Changed the way of searching and deleting PENDING or LOCKED executions
        '''              SA 20/06/2012 - Lock of Controls due to locked needed Blanks or Calibrators is applied only for STD preparations 
        '''                            - Removed filter by TestType=STD when searching STAT Patient request; instead of it, get all different 
        '''                              TestType/TestID/SampleType requested for STAT and filter by each specific TestType when searching the 
        '''                              needed Blanks, Calibrators and Controls to mark them as STAT 
        '''              SA 26/07/2012 - Added optional parameter to indicate if the ISE Module is ready to be used; when it is not ready, the status 
        '''                              of all pending ISE Executions is changed to LOCKED
        '''              SA 31/07/2012 - When the function is called in STANDBY (pWorkInRunningMode = False), before deleting affected Executions, delete 
        '''                              their Readings (to avoid System Error due to violation of FK when delete records in twksWSExecutions)
        '''              SA 07/09/2012 - Added optional parameter pISEElectrodesList to inform the list of ISE Electrodes with wrong/pending calibration. 
        '''                              When this parameter is informed, all Executions for the ISE Tests contained in the list will be locked
        '''              AG 25/03/2013 - Before modify the current executions table get those ordertests locked by LIS
        '''                              After generate the new executions set ExecutionStatus = LOCKED and LockedByLIS = True for the executions of these ordertests
        '''              AG 19/02/2014 - #1514 improvements memory app/sql
        '''              AG 20/03/2014 - #1545 call create WS executions method with multiple transactions
        '''              AG 30/05/2014 - #1584 new parameter pPauseMode (for recalculate status for executions LOCKED and also PENDING)!!! (in normal running only the LOCKED are recalculated)
        '''              AG 02/06/2014 - #1644 new optional parameter pManualRerunFlag (when FALSE Software cannot use the semaphore because it has been set to busy when ANSPHR started to be processed)
        ''' </remarks> 
        Public Function CreateWSExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                           ByVal pWorkInRunningMode As Boolean, Optional ByVal pOrderTestID As Integer = -1, _
                                           Optional ByVal pPostDilutionType As String = "", Optional ByVal pIsISEModuleReady As Boolean = False, _
                                           Optional ByVal pISEElectrodesList As List(Of String) = Nothing, Optional ByVal pPauseMode As Boolean = False, _
                                           Optional ByVal pManualRerunFlag As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try

                'AG 20/03/2014 - #1545 - call the new create execution method that uses multiple transactions
                If (GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                    resultData = CreateWSExecutionsMultipleTransactions(pDBConnection, pAnalyzerID, pWorkSessionID, pWorkInRunningMode, _
                                                       pOrderTestID, pPostDilutionType, pIsISEModuleReady, pISEElectrodesList, pPauseMode, pManualRerunFlag)
                    Exit Try 'Exit Try and not execute the old code
                End If
                'AG 20/03/2014 - #1545

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                'Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'AG 02/06/2014 #1644 - Set the semaphore to busy value (EXCEPT when called from auto rerun business)
                If GlobalConstants.CreateWSExecutionsWithSemaphore AndAlso pManualRerunFlag Then
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Waiting (timeout = " & GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS.ToString & ")", "AnalyzerManager.CreateWSExecutions", EventLogEntryType.Information, False)
                    GlobalSemaphores.createWSExecutionsSemaphore.WaitOne(GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS)
                    GlobalSemaphores.createWSExecutionsQueue = 1 'Only 1 thread is allowed, so set to 1 instead of increment ++1 'GlobalSemaphores.createWSExecutionsQueue += 1
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Passed through, semaphore busy", "AnalyzerManager.CreateWSExecutions", EventLogEntryType.Information, False)
                End If

                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calledForRerun As Boolean = (pOrderTestID <> -1)

                        StartTime = Now '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                        'Delete all Executions with status PENDING or LOCKED belonging OrderTests not started!!!
                        'If an OrderTestID is not informed, only Pending and Locked Executions for Rerun=1 are deleted
                        Dim myDAO As New twksWSExecutionsDAO

                        'AG 25/03/2013 - Get the current distinct ordertests locked by lis
                        resultData = myDAO.GetOrderTestsLockedByLIS(dbConnection, pAnalyzerID, pWorkSessionID, True)
                        Dim orderTestLockedByLISList As New List(Of Integer)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            For Each row As ExecutionsDS.twksWSExecutionsRow In DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions
                                If Not row.IsOrderTestIDNull AndAlso Not orderTestLockedByLISList.Contains(row.OrderTestID) Then orderTestLockedByLISList.Add(row.OrderTestID)
                            Next
                        End If
                        'AG 25/03/2013

                        'AJG ADDED BECAUSE THE ANALYZER MODEL IS NEEDED BECAUSE MANAGING CONTAMINATIONS IS ANALYZER DEPENDANT
                        Dim activeAnalyzer As String = GetActiveAnalyzer(dbConnection)

                        If (Not pWorkInRunningMode) Then 'AG 19/02/2014 - #1514 note that this parameter value is FALSE when called from create repetitions process!!!
                            'Search all Order Tests which Executions can be deleted: those having ALL Executions with status PENDING or LOCKED
                            resultData = myDAO.SearchNotInCourseExecutionsToDelete(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myOrderTestsToDelete As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                                If (myOrderTestsToDelete.twksOrderTests.Count > 0) Then
                                    'Delete all Readings of the Executions for all OrderTests returned by the previous called function
                                    Dim myReadingsDelegate As New WSReadingsDelegate
                                    resultData = myReadingsDelegate.DeleteReadingsForNotInCourseExecutions(dbConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)

                                    If (Not resultData.HasError) Then
                                        'Delete the Executions for all OrderTests returned by the previous called function
                                        resultData = myDAO.DeleteNotInCourseExecutionsNEW(dbConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)
                                    End If
                                End If
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Not RUNNING: Search and Delete NOT IN COURSE " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        End If

                        'NEW 19/09/2011 - Lock / Unlock process:
                        'pWorkInRunningMode = FALSE -> Recalculate status for all executions with RerunNumber > 1 with current status is PENDING or LOCKED
                        '                              (note that in this case ALL the PENDING or LOCKED executions with RerunNumber = 1 have been deleted in mehtod DeleteNotInCurseExecutions)
                        '
                        'pWorkInRuuningMode = TRUE ->  Recalculate status for all existing executions with status PENDING or LOCKED
                        'Table twksWSExecutions is updated at this point
                        If (Not calledForRerun) Then

                            'AG 28/05/2014 - #1644 - Do not delete readings here!! They will be removed when the new preparation receives his 1st reading
                            ''In RUNNING Mode, delete all Readings of all LOCKED Executions 
                            'Dim myReadingsDelegate As New WSReadingsDelegate
                            'resultData = myReadingsDelegate.DeleteReadingsForNotInCourseExecutions(dbConnection, pAnalyzerID, pWorkSessionID, Nothing)

                            'AG 30/05/2014 - #1644 new parameter pPauseMode required
                            resultData = RecalculateStatusForNotDeletedExecutionsNEW(dbConnection, pAnalyzerID, pWorkSessionID, pWorkInRunningMode, pPauseMode)

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            GlobalBase.CreateLogActivity("Recalculate Status " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                            StartTime = Now
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        End If
                        'AG 19/09/2011

                        'Now we can create executions for all ordertests not started (with no executions in twksWSExecutions table) ... <the initial process>
                        If (Not resultData.HasError) Then
                            'Get detailed information of all Order Tests to be executed in the WS
                            Dim allOrderTestsDS As OrderTestsForExecutionsDS
                            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

                            resultData = myWSOrderTestsDelegate.GetInfoOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                allOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsForExecutionsDS)

                                'Get all executions for BLANKS included in the WorkSession
                                Dim myBlankExecutionsDS As New ExecutionsDS
                                'Get all executions for CALIBRATORS included in the WorkSession
                                Dim myCalibratorExecutionsDS As ExecutionsDS = Nothing
                                'Get all executions for CONTROLS included in the WorkSession
                                Dim myControlExecutionsDS As ExecutionsDS = Nothing
                                'Get all executions for PATIENT SAMPLES included in the WorkSession
                                Dim myPatientExecutionsDS As ExecutionsDS = Nothing

                                If (allOrderTestsDS.OrderTestsForExecutionsTable.Rows.Count > 0) Then
                                    resultData = CreateBlankExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myBlankExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                    End If
                                    resultData = CreateCalibratorExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myCalibratorExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                    End If
                                    resultData = CreateControlExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myControlExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                    End If
                                    resultData = CreatePatientExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPatientExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                    End If

                                    Dim listTask As New List(Of Task)

                                    listTask.Add(Task.Factory.StartNew(Sub()
                                                                           Dim myOrderTestID As Integer = -1
                                                                           Dim blankInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                                                           For Each rowBlank As ExecutionsDS.twksWSExecutionsRow In myBlankExecutionsDS.twksWSExecutions
                                                                               If (rowBlank.OrderTestID <> myOrderTestID) Then
                                                                                   myOrderTestID = rowBlank.OrderTestID
                                                                                   'Search information for the Blank
                                                                                   blankInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                                                               Where a.OrderTestID = myOrderTestID _
                                                                                              Select a).ToList()
                                                                               End If

                                                                               '...and complete fields for the Blank
                                                                               If (blankInfo.Count = 1) Then
                                                                                   rowBlank.BeginEdit()
                                                                                   rowBlank.TestID = blankInfo(0).TestID
                                                                                   rowBlank.ReadingCycle = blankInfo(0).ReadingCycle
                                                                                   rowBlank.ReagentID = blankInfo(0).ReagentID
                                                                                   rowBlank.OrderID = blankInfo(0).OrderID
                                                                                   rowBlank.ElementID = NullElementID     'rowBlank.ReagentID 'RH 29/09/2011
                                                                                   rowBlank.EndEdit()
                                                                               End If
                                                                           Next
                                                                           myBlankExecutionsDS.AcceptChanges()
                                                                           blankInfo = Nothing 'AG 19/02/2014 - #1514
                                                                           'End If
                                                                           GlobalBase.CreateLogActivity("Get Executions For BLANKS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                                                           "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                                                       End Sub))


                                    listTask.Add(Task.Factory.StartNew(Sub()
                                                                           Dim myOrderTestID As Integer = -1
                                                                           Dim calibInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                                                           For Each rowCalib As ExecutionsDS.twksWSExecutionsRow In myCalibratorExecutionsDS.twksWSExecutions
                                                                               'Dim calibInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                                                                               If (rowCalib.OrderTestID <> myOrderTestID) Then
                                                                                   myOrderTestID = rowCalib.OrderTestID
                                                                                   'Search information for the Calibrator
                                                                                   calibInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                                                               Where a.OrderTestID = myOrderTestID _
                                                                                              Select a).ToList()
                                                                               End If

                                                                               '...and complete fields for the Calibrator
                                                                               If (calibInfo.Count = 1) Then
                                                                                   rowCalib.BeginEdit()
                                                                                   rowCalib.TestID = calibInfo(0).TestID
                                                                                   rowCalib.SampleType = calibInfo(0).SampleType
                                                                                   rowCalib.ReadingCycle = calibInfo(0).ReadingCycle
                                                                                   rowCalib.ReagentID = calibInfo(0).ReagentID
                                                                                   rowCalib.OrderID = calibInfo(0).OrderID
                                                                                   rowCalib.ElementID = calibInfo(0).ElementID
                                                                                   rowCalib.EndEdit()
                                                                               End If
                                                                           Next
                                                                           'myCalibratorExecutionsDS.AcceptChanges()
                                                                           calibInfo = Nothing 'AG 19/02/2014 - #1514
                                                                           'End If
                                                                           GlobalBase.CreateLogActivity("Get Executions For CALIBRATORS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                                                           "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                                                       End Sub))


                                    listTask.Add(Task.Factory.StartNew(Sub()
                                                                           Dim myOrderTestID As Integer = -1
                                                                           Dim controlInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                                                           For Each rowControl As ExecutionsDS.twksWSExecutionsRow In myControlExecutionsDS.twksWSExecutions
                                                                               'Dim controlInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                                                                               If (rowControl.OrderTestID <> myOrderTestID) Then
                                                                                   myOrderTestID = rowControl.OrderTestID
                                                                                   'Search information for the Control
                                                                                   controlInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                                                                 Where a.OrderTestID = myOrderTestID _
                                                                                                Select a).ToList()
                                                                               End If

                                                                               '...and complete fields for the Control (more than one record is possible when several
                                                                               'controls are used for the TestType/Test/Sample Type)
                                                                               If (controlInfo.Count >= 1) Then
                                                                                   rowControl.BeginEdit()
                                                                                   rowControl.TestID = controlInfo(0).TestID
                                                                                   rowControl.SampleType = controlInfo(0).SampleType
                                                                                   rowControl.ReadingCycle = controlInfo(0).ReadingCycle
                                                                                   rowControl.ReagentID = controlInfo(0).ReagentID
                                                                                   rowControl.OrderID = controlInfo(0).OrderID
                                                                                   rowControl.ElementID = controlInfo(0).ElementID

                                                                                   If orderTestLockedByLISList.Contains(rowControl.OrderTestID) Then rowControl.ExecutionStatus = "LOCKED" 'AG 25/03/2013 - Locked by LIS not by volume missing
                                                                                   rowControl.EndEdit()
                                                                               End If
                                                                           Next
                                                                           controlInfo = Nothing 'AG 19/02/2014 - #1514
                                                                           'End If
                                                                           GlobalBase.CreateLogActivity("Get Executions For CONTROLS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                                                           "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                                                       End Sub))


                                    listTask.Add(Task.Factory.StartNew(Sub()
                                                                           Dim myOrderTestID As Integer = -1
                                                                           Dim patientInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

                                                                           For Each rowPatient As ExecutionsDS.twksWSExecutionsRow In myPatientExecutionsDS.twksWSExecutions
                                                                               'Dim patientInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                                                                               If (rowPatient.OrderTestID <> myOrderTestID) Then
                                                                                   myOrderTestID = rowPatient.OrderTestID

                                                                                   'Search information for the Patient Sample
                                                                                   patientInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                                                                 Where a.OrderTestID = myOrderTestID _
                                                                                                Select a).ToList()
                                                                               End If

                                                                               '...and complete fields for the Patient Sample
                                                                               If (patientInfo.Count = 1) Then
                                                                                   rowPatient.BeginEdit()
                                                                                   rowPatient.TestID = patientInfo(0).TestID
                                                                                   rowPatient.SampleType = patientInfo(0).SampleType
                                                                                   rowPatient.ReadingCycle = patientInfo(0).ReadingCycle
                                                                                   rowPatient.ReagentID = patientInfo(0).ReagentID
                                                                                   rowPatient.OrderID = patientInfo(0).OrderID

                                                                                   'AG 27/04/2012 Activate this line again (RH 08/03/2012 Remove this line)
                                                                                   rowPatient.ElementID = patientInfo(0).CreationOrder 'Convert.ToInt32(patientInfo(0).OrderID.Substring(8, 4))
                                                                                   If orderTestLockedByLISList.Contains(rowPatient.OrderTestID) Then rowPatient.ExecutionStatus = "LOCKED" 'AG 25/03/2013 - Locked by LIS not by volume missing
                                                                                   rowPatient.EndEdit()
                                                                               End If
                                                                           Next
                                                                           patientInfo = Nothing 'AG 19/02/2014 - #1514
                                                                           'End If
                                                                           GlobalBase.CreateLogActivity("Get Executions For PATIENTS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                                                           "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                                                       End Sub))


                                    Task.WaitAll(listTask.ToArray())

                                    listTask.Clear()

                                    If (Not resultData.HasError) Then
                                        'Search all locked BLANKS to lock also all Calibrators, Control and Patient Samples
                                        'for the same Standard Test
                                        Dim lstLockedBlanks As List(Of Integer)
                                        lstLockedBlanks = (From a In myBlankExecutionsDS.twksWSExecutions _
                                                          Where a.ExecutionStatus = "LOCKED" _
                                                         Select a.TestID Distinct).ToList()

                                        'AJG
                                        Dim lstLockedCtrls As List(Of ExecutionsDS.twksWSExecutionsRow)
                                        Dim lstLockedCalibs As List(Of ExecutionsDS.twksWSExecutionsRow)
                                        Dim lstLockedPatients As List(Of ExecutionsDS.twksWSExecutionsRow)

                                        listTask.Add(task.Factory.StartNew(Sub()
                                                                               For Each lockedBlank In lstLockedBlanks
                                                                                   Dim myOwnTestId = lockedBlank
                                                                                   '...LOCK the correspondent Calibrators
                                                                                   lstLockedCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                                                                                                     Where a.TestID = myOwnTestId _
                                                                                                   AndAlso a.ExecutionStatus <> "LOCKED" _
                                                                                                    Select a).ToList()

                                                                                   For Each lockedCalib In lstLockedCalibs
                                                                                       lockedCalib.BeginEdit()
                                                                                       lockedCalib.ExecutionStatus = "LOCKED"
                                                                                       lockedCalib.EndEdit()
                                                                                   Next
                                                                               Next
                                                                           End Sub))

                                        listTask.Add(task.Factory.StartNew(Sub()
                                                                               For Each lockedBlank In lstLockedBlanks
                                                                                   Dim myOwnTestId = lockedBlank
                                                                                   '...LOCK the correspondent Controls - Apply only to STD Preparations
                                                                                   lstLockedCtrls = (From a In myControlExecutionsDS.twksWSExecutions _
                                                                                                     Where a.ExecutionType = "PREP_STD" _
                                                                                                   AndAlso a.TestID = myOwnTestId _
                                                                                                   AndAlso a.ExecutionStatus <> "LOCKED" _
                                                                                                    Select a).ToList()

                                                                                   For Each lockedCtrl In lstLockedCtrls
                                                                                       lockedCtrl.BeginEdit()
                                                                                       lockedCtrl.ExecutionStatus = "LOCKED"
                                                                                       lockedCtrl.EndEdit()
                                                                                   Next
                                                                               Next
                                                                           End Sub))

                                        listTask.Add(task.Factory.StartNew(Sub()
                                                                               For Each lockedBlank In lstLockedBlanks
                                                                                   Dim myOwnTestId = lockedBlank
                                                                                   '...LOCK the correspondent Patients
                                                                                   lstLockedPatients = (From a In myPatientExecutionsDS.twksWSExecutions _
                                                                                                       Where a.ExecutionType = "PREP_STD" _
                                                                                                     AndAlso a.TestID = myOwnTestId _
                                                                                                     AndAlso a.ExecutionStatus <> "LOCKED" _
                                                                                                      Select a).ToList()

                                                                                   For Each lockedPatient In lstLockedPatients
                                                                                       lockedPatient.BeginEdit()
                                                                                       lockedPatient.ExecutionStatus = "LOCKED"
                                                                                       lockedPatient.EndEdit()
                                                                                   Next
                                                                               Next
                                                                           End Sub))


                                        task.WaitAll(listTask.ToArray())
                                        listTask.Clear()

                                        myCalibratorExecutionsDS.AcceptChanges()
                                        myControlExecutionsDS.AcceptChanges()
                                        myPatientExecutionsDS.AcceptChanges()

                                        'AG 19/02/2014 - #1514
                                        lstLockedBlanks = Nothing
                                        lstLockedCtrls = Nothing
                                        lstLockedCalibs = Nothing
                                        lstLockedPatients = Nothing
                                        'AG 19/02/2014 - #1514
                                    End If

                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    GlobalBase.CreateLogActivity("Locks for BLANKS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                    "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                    StartTime = Now
                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                                    If (Not resultData.HasError) Then
                                        'Search all locked CALIBRATORS to lock also all Controls and Patient Samples
                                        'for the same Standard Test and SampleType
                                        Dim lstLockedCalibs As List(Of ExecutionsDS.twksWSExecutionsRow)
                                        lstLockedCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                                                          Where a.ExecutionStatus = "LOCKED" _
                                                         Select a Order By a.OrderTestID).ToList()

                                        Dim myOrderTestID As Integer = -1

                                        Parallel.ForEach(lstLockedCalibs, Sub(lockedCalib)
                                                                              If (myOrderTestID <> lockedCalib.OrderTestID) Then
                                                                                  'First verify if the Calibrator is used as alternative of another Sample Types for the same Test
                                                                                  myOrderTestID = lockedCalib.OrderTestID
                                                                                  Dim lstAlternativeST = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                                                                     Where a.SampleClass = "CALIB" _
                                                                                                   AndAlso Not a.IsAlternativeOrderTestIDNull _
                                                                                                   AndAlso a.AlternativeOrderTestID = myOrderTestID _
                                                                                                    Select a.SampleType Distinct).ToList

                                                                                  For i As Integer = 0 To (lstAlternativeST.Count)
                                                                                      Dim myTestID = lockedCalib.TestID
                                                                                      Dim mySampleType = lockedCalib.SampleType

                                                                                      If (i > 0) Then mySampleType = lstAlternativeST(i - 1)

                                                                                      '...LOCK the correspondent Controls - Apply only to STD Preparations
                                                                                      Dim lstLockedCtrls = (From a In myControlExecutionsDS.twksWSExecutions _
                                                                                                        Where a.ExecutionType = "PREP_STD" _
                                                                                                      AndAlso a.TestID = myTestID _
                                                                                                      AndAlso a.SampleType = mySampleType _
                                                                                                      AndAlso a.ExecutionStatus <> "LOCKED" _
                                                                                                       Select a).ToList()

                                                                                      Parallel.ForEach(lstLockedCtrls, Sub(lockedCtrl)
                                                                                                                           lockedCtrl.BeginEdit()
                                                                                                                           lockedCtrl.ExecutionStatus = "LOCKED"
                                                                                                                           lockedCtrl.EndEdit()
                                                                                                                       End Sub)

                                                                                      '...LOCK the correspondent Patients.
                                                                                      Dim lstLockedPatients = (From a In myPatientExecutionsDS.twksWSExecutions _
                                                                                                          Where a.ExecutionType = "PREP_STD" _
                                                                                                        AndAlso a.TestID = myTestID _
                                                                                                        AndAlso a.SampleType = mySampleType _
                                                                                                        AndAlso a.ExecutionStatus <> "LOCKED" _
                                                                                                         Select a).ToList()

                                                                                      Parallel.ForEach(lstLockedPatients, Sub(lockedPatient)
                                                                                                                              lockedPatient.BeginEdit()
                                                                                                                              lockedPatient.ExecutionStatus = "LOCKED"
                                                                                                                              lockedPatient.EndEdit()
                                                                                                                          End Sub)
                                                                                  Next
                                                                              End If
                                                                          End Sub)

                                        myControlExecutionsDS.AcceptChanges()
                                        myPatientExecutionsDS.AcceptChanges()

                                        'AG 19/02/2014 - #1514
                                        lstLockedCalibs = Nothing
                                        'AG 19/02/2014 - #1514
                                    End If

                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    GlobalBase.CreateLogActivity("Locks for CALIBRATORS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                    "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                    StartTime = Now
                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                                    If (Not resultData.HasError) Then
                                        If (Not calledForRerun) Then
                                            'Search all Standard TestType/TestID/SampleType of all Patients requested for STAT, to mark as STAT
                                            'all the needed Blanks, Calibrators and Controls
                                            Dim lstSTATS As List(Of String)
                                            lstSTATS = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                       Where a.SampleClass = "PATIENT" _
                                                     AndAlso a.StatFlag = True _
                                                      Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList

                                            Dim pos1 As Integer
                                            Dim pos2 As Integer
                                            Dim myTestID As Integer
                                            Dim myTestType As String
                                            Dim mySampleType As String
                                            Dim lstBlkCalibCtrls As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)

                                            'SA 19/06/2012 - Filter linq also by TestType
                                            For Each statPatient In lstSTATS
                                                pos1 = statPatient.IndexOf("|")
                                                myTestType = statPatient.Substring(0, pos1)

                                                pos2 = statPatient.LastIndexOf("|")
                                                mySampleType = statPatient.Substring(pos2 + 1)
                                                myTestID = Convert.ToInt32(statPatient.Substring(pos1 + 1, pos2 - pos1 - 1))


                                                'Search all Controls, Calibrators and Blanks for the TestType/TestID/SampleType
                                                lstBlkCalibCtrls = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                                    Where a.TestType = myTestType _
                                                                  AndAlso a.TestID = myTestID _
                                                                  AndAlso ((a.SampleClass = "BLANK") _
                                                                   OrElse (a.SampleClass = "CALIB" AndAlso a.SampleType = mySampleType) _
                                                                   OrElse (a.SampleClass = "CTRL" AndAlso a.SampleType = mySampleType)) _
                                                                   Select a Order By a.SampleClass).ToList()


                                                Parallel.ForEach(lstBlkCalibCtrls, Sub(blkCalibCtrlRow)
                                                                                       Dim myOrderTestID = blkCalibCtrlRow.OrderTestID
                                                                                       If (blkCalibCtrlRow.SampleClass = "CALIB") AndAlso (Not blkCalibCtrlRow.IsAlternativeOrderTestIDNull) Then
                                                                                           myOrderTestID = blkCalibCtrlRow.AlternativeOrderTestID
                                                                                       End If

                                                                                       'Search the OrderTestID to update the StatFlag
                                                                                       If (blkCalibCtrlRow.SampleClass = "BLANK") Then
                                                                                           Dim lstBlanks = (From a In myBlankExecutionsDS.twksWSExecutions _
                                                                                                       Where a.OrderTestID = myOrderTestID _
                                                                                                      Select a).ToList()

                                                                                           For Each blank In lstBlanks
                                                                                               blank.StatFlag = True
                                                                                           Next
                                                                                       ElseIf (blkCalibCtrlRow.SampleClass = "CALIB") Then
                                                                                           'Dim lstCalibs As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
                                                                                           Dim lstCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                                                                                                       Where a.OrderTestID = myOrderTestID _
                                                                                                      Select a).ToList()

                                                                                           For Each calibrator In lstCalibs
                                                                                               calibrator.StatFlag = True
                                                                                           Next
                                                                                       ElseIf (blkCalibCtrlRow.SampleClass = "CTRL") Then
                                                                                           'Dim lstCtrls As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
                                                                                           Dim lstCtrls = (From a In myControlExecutionsDS.twksWSExecutions _
                                                                                                       Where a.OrderTestID = myOrderTestID _
                                                                                                      Select a).ToList()

                                                                                           For Each control In lstCtrls
                                                                                               control.StatFlag = True
                                                                                           Next
                                                                                       End If
                                                                                   End Sub)

                                                myBlankExecutionsDS.AcceptChanges()
                                                myCalibratorExecutionsDS.AcceptChanges()
                                                myControlExecutionsDS.AcceptChanges()
                                            Next

                                            'AG 19/02/2014 - #1514
                                            lstSTATS = Nothing
                                            lstBlkCalibCtrls = Nothing
                                            'AG 19/02/2014 - #1514
                                        End If
                                    End If

                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    GlobalBase.CreateLogActivity("Mark STATS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                    "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                    StartTime = Now
                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***


                                    'Unify all locked Executions and all pending Executions (all types) 
                                    Dim lockedExecutionsDS As New ExecutionsDS
                                    Dim pendingExecutionsDS As New ExecutionsDS

                                    If (Not resultData.HasError) Then
                                        'Move executions for BLANKS
                                        For Each blank As ExecutionsDS.twksWSExecutionsRow In myBlankExecutionsDS.twksWSExecutions
                                            If (blank.ExecutionStatus = "LOCKED") Then
                                                lockedExecutionsDS.twksWSExecutions.ImportRow(blank)
                                            ElseIf (blank.ExecutionStatus = "PENDING") Then
                                                pendingExecutionsDS.twksWSExecutions.ImportRow(blank)
                                            End If
                                        Next

                                        'AG 19/02/2014 - #1514 use merge instead of loops
                                        ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                        'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                        '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                        'Next
                                        pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                        lockedExecutionsDS.Clear()
                                        'AG 15/09/2011

                                        'Move executions for CALIBRATORS
                                        For Each calib As ExecutionsDS.twksWSExecutionsRow In myCalibratorExecutionsDS.twksWSExecutions
                                            If (calib.ExecutionStatus = "LOCKED") Then
                                                lockedExecutionsDS.twksWSExecutions.ImportRow(calib)
                                            ElseIf (calib.ExecutionStatus = "PENDING") Then
                                                pendingExecutionsDS.twksWSExecutions.ImportRow(calib)
                                            End If
                                        Next

                                        'AG 19/02/2014 - #1514 use merge instead of loops
                                        ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                        'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                        '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                        'Next
                                        pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                        lockedExecutionsDS.Clear()
                                        'AG 15/09/2011

                                        'Move executions for CONTROLS
                                        For Each ctrl As ExecutionsDS.twksWSExecutionsRow In myControlExecutionsDS.twksWSExecutions
                                            If (ctrl.ExecutionStatus = "LOCKED") Then
                                                lockedExecutionsDS.twksWSExecutions.ImportRow(ctrl)
                                            ElseIf (ctrl.ExecutionStatus = "PENDING") Then
                                                pendingExecutionsDS.twksWSExecutions.ImportRow(ctrl)
                                            End If
                                        Next

                                        'AG 19/02/2014 - #1514 use merge instead of loops
                                        ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                        'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                        '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                        'Next
                                        pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                        lockedExecutionsDS.Clear()
                                        'AG 15/09/2011

                                        'Move executions for PATIENTS
                                        For Each patient As ExecutionsDS.twksWSExecutionsRow In myPatientExecutionsDS.twksWSExecutions
                                            If (patient.ExecutionStatus = "LOCKED") Then
                                                lockedExecutionsDS.twksWSExecutions.ImportRow(patient)
                                            ElseIf (patient.ExecutionStatus = "PENDING") Then
                                                pendingExecutionsDS.twksWSExecutions.ImportRow(patient)
                                            End If
                                        Next

                                        'AG 19/02/2014 - #1514 use merge instead of loops
                                        ''AG 15/09/2011 - Add locked executions at the botton of the pending ones
                                        'For Each lockedEx As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions
                                        '    pendingExecutionsDS.twksWSExecutions.ImportRow(lockedEx)
                                        'Next
                                        pendingExecutionsDS.twksWSExecutions.Merge(lockedExecutionsDS.twksWSExecutions)
                                        lockedExecutionsDS.Clear()
                                        'AG 15/09/2011

                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        GlobalBase.CreateLogActivity("Unify all Pending and Locked " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                        "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                        StartTime = Now
                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                                        If (Not calledForRerun) Then
                                            'Sort PENDING executions 
                                            If (pendingExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                                'AG 30/11/2011 - NEW: When a patient has Ise & std test executions the ISE executions are the first

                                                'AG 15/09/2011 - Locked executions at the botton his sample
                                                'pendingExecutionsDS.twksWSExecutions.DefaultView.Sort = _
                                                '        "StatFlag DESC, SampleClass, ElementID, SampleType, " & _
                                                '        "ExecutionType DESC, ReadingCycle DESC"
                                                'pendingExecutionsDS.twksWSExecutions.DefaultView.Sort = _
                                                '    "StatFlag DESC, SampleClass, ElementID, SampleType, " & _
                                                '    "ExecutionType DESC, ExecutionStatus DESC, ReadingCycle DESC"
                                                ''AG 15/09/2011

                                                pendingExecutionsDS.twksWSExecutions.DefaultView.Sort = "StatFlag DESC, SampleClass, ElementID, SampleType, " & _
                                                                                                        "ExecutionType, ExecutionStatus DESC, ReadingCycle DESC"
                                                'AG 30/11/2011 - NEW: When a patient has Ise & std test executions the ISE executions are the first

                                                Dim executionDataDS As New ExecutionsDS
                                                'AG 12/07/2012
                                                'For Each pendingExecution As DataRow In pendingExecutionsDS.twksWSExecutions.DefaultView.ToTable.Rows
                                                '    executionDataDS.twksWSExecutions.ImportRow(pendingExecution)
                                                'Next
                                                executionDataDS = CType(pendingExecutionsDS, ExecutionsDS)
                                                'AG 12/07/2012

                                                'AG 28/11/2011 - Code for evaluate method execution time (comment after the evaluation has been performed)
                                                'Dim startTime As DateTime = Now

                                                'Sort by Contamination

                                                'MANEL
                                                Dim sorter = New WSSorter(executionDataDS, activeAnalyzer)
                                                If sorter.SortWSExecutionsByContamination(dbConnection) Then
                                                    resultData.SetDatos = sorter.Executions
                                                    executionDataDS = sorter.Executions
                                                Else
                                                    resultData.SetDatos = Nothing
                                                    resultData.HasError = True
                                                End If
                                                '/MANEL

                                                If (Not resultData.HasError AndAlso Not executionDataDS Is Nothing) Then
                                                    'executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS) 'Already done

                                                    'Sort Orders by ReadingCycle
                                                    resultData = SortWSExecutionsByElementGroupTime(executionDataDS)
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                                        resultData = SortWSExecutionsByElementGroupContaminationNew(activeAnalyzer, dbConnection, executionDataDS) 'AG 07/11/2011

                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            executionDataDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                                        End If

                                                    End If
                                                End If

                                                'AG 28/11/2011 - Code for evaluate method execution time (comment after the evaluation has been performed)
                                                'Dim elapsedTime As Double = 0
                                                'elapsedTime = Now.Subtract(startTime).TotalMilliseconds

                                                'Finally, save the sorted PENDING executions
                                                If (Not resultData.HasError) Then
                                                    resultData = myDAO.Create(dbConnection, executionDataDS)
                                                End If
                                            End If

                                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                            GlobalBase.CreateLogActivity("Sort and create all PENDING " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                            StartTime = Now
                                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        Else
                                            'Save PENDING executions  
                                            resultData = myDAO.Create(dbConnection, pendingExecutionsDS)
                                        End If

                                        'Save LOCKED executions
                                        If (lockedExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                            If (Not resultData.HasError) Then
                                                resultData = myDAO.Create(dbConnection, lockedExecutionsDS)
                                            End If
                                        End If

                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        GlobalBase.CreateLogActivity("Save all LOCKED " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                        "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                                        StartTime = Now
                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                                        If (Not resultData.HasError) Then
                                            'AG 19/09/2011 - Update of the Paused flag is performed only when no Running mode
                                            '              - When working in Running mode, no pending executions are deleted, then SW has only to update status 
                                            '                (business already performed)
                                            If (Not calledForRerun AndAlso Not pWorkInRunningMode) Then
                                                Dim tempExecutionDS As New ExecutionsDS
                                                Dim myWSPausedOrderTestsDS As New WSPausedOrderTestsDS
                                                Dim myWSPausedOrderTestsDelegate As New WSPausedOrderTestsDelegate

                                                resultData = myWSPausedOrderTestsDelegate.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID)
                                                If (Not resultData.HasError) Then
                                                    myWSPausedOrderTestsDS = DirectCast(resultData.SetDatos, WSPausedOrderTestsDS)

                                                    For Each pausedOTRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow In myWSPausedOrderTestsDS.twksWSPausedOrderTests.Rows
                                                        resultData = myDAO.ReadByOrderTestIDAndRerunNumber(dbConnection, pausedOTRow.OrderTestID, pausedOTRow.RerunNumber)
                                                        If resultData.HasError Then Exit For

                                                        tempExecutionDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                                        If (tempExecutionDS.twksWSExecutions.Count > 0) Then
                                                            'If found, then update
                                                            For Each executionRow As ExecutionsDS.twksWSExecutionsRow In tempExecutionDS.twksWSExecutions.Rows
                                                                resultData = myDAO.UpdatePaused(dbConnection, True, executionRow.ExecutionID)
                                                                If (resultData.HasError) Then Exit For
                                                            Next
                                                        End If
                                                    Next
                                                End If
                                            End If
                                            'AG 19/09/2011
                                        End If

                                        If (Not resultData.HasError) Then
                                            If (Not calledForRerun) Then
                                                'Do nothing ... business has been already performed in previous call to RecalculateStatusForNotDeletedExecutions method
                                            Else
                                                'If function was called for a Rerun, change the Status of the Order Test to PENDING
                                                Dim myOrderTestDelegate As New OrderTestsDelegate
                                                resultData = myOrderTestDelegate.UpdateStatusByOrderTestID(dbConnection, pOrderTestID, "PENDING")
                                            End If

                                            'AG 25/03/2013 - Update executions locked by lis
                                            If Not resultData.HasError AndAlso orderTestLockedByLISList.Count > 0 Then
                                                resultData = myDAO.UpdateLockedByLIS(dbConnection, orderTestLockedByLISList, True)
                                            End If
                                            'AG 25/03/2013

                                        End If
                                    End If
                                End If
                            End If
                        End If

                        'AG 20/09/2011 - When the WorkSession is modified, all samples rotor status can change 
                        '                when new tests are added using a tube with status finished (finished -> pending)
                        If (Not resultData.HasError AndAlso Not calledForRerun) Then
                            'Read the current content of all positions in Samples Rotor
                            Dim rcp_del As New WSRotorContentByPositionDelegate
                            resultData = rcp_del.ReadByCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", -1)

                            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                Dim samplesRotorDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                'Get only positions with tubes for required WorkSession Elements not marked as Depleted or with not enough volume
                                Dim linqRes As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In samplesRotorDS.twksWSRotorContentByPosition _
                                      Where Not a.IsElementIDNull _
                                        AndAlso a.Status <> "DEPLETED" _
                                        AndAlso a.Status <> "FEW" _
                                         Select a).ToList

                                Dim newStatus As String = ""
                                For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In linqRes
                                    newStatus = row.Status
                                    resultData = rcp_del.UpdateSamplePositionStatus(dbConnection, -1, pWorkSessionID, pAnalyzerID, row.ElementID, _
                                                                                    row.TubeContent, 1, newStatus, row.CellNumber)
                                Next
                                linqRes = Nothing 'AG 19/02/2014 - #1514
                            End If
                        End If
                        'AG 20/09/2011

                        If (Not resultData.HasError) Then

                            'check if there are Electrodes with wrong Calibration
                            If (Not pISEElectrodesList Is Nothing AndAlso pISEElectrodesList.Count > 0) Then
                                For Each electrode As String In pISEElectrodesList
                                    resultData = UpdateStatusByISETestType(dbConnection, pWorkSessionID, pAnalyzerID, electrode, "PENDING", "LOCKED")
                                    If (resultData.HasError) Then Exit For
                                Next
                            ElseIf (Not pIsISEModuleReady) Then
                                'ISE Module cannot be used; all pending ISE Preparations are LOCKED
                                resultData = UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "PREP_ISE", "PENDING", "LOCKED")
                            End If

                        End If

                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        GlobalBase.CreateLogActivity("Final Processing " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                        "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
                        StartTime = Now
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                        orderTestLockedByLISList = Nothing 'AG 19/02/2014 - #1514
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

                'AG 02/06/2014 #1644 - Set the semaphore to free value (EXCEPT when called from auto rerun business)
                If GlobalConstants.CreateWSExecutionsWithSemaphore AndAlso pManualRerunFlag Then
                    GlobalSemaphores.createWSExecutionsSemaphore.Release()
                    GlobalSemaphores.createWSExecutionsQueue = 0 'Only 1 thread is allowed, so reset to 0 instead of decrement --1 'GlobalSemaphores.createWSExecutionsQueue -= 1
                    'Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Released, semaphore free", "AnalyzerManager.CreateWSExecutions", EventLogEntryType.Information, False)
                End If

            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the list of Executions sorted by Contamination.
        ''' </summary>
        ''' <param name="pExecutions">Dataset with structure of view vwksWSExecutions</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the ordered data (view vwksWSExecutions)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 08/06/2010
        ''' Modified by: SA            - Changed function called to get the R1 Contaminations for a one in the Delegated Class
        '''              RH 08/04/2011 - Changed order field from OrderID to ElementID
        '''              AG 27/04/2011 - In list of SampleClass, CALIB has to be before CTRL
        '''              AG 10/11/2011 - Apply policy only when improve the original solution
        '''              AG 25/11/2011 - Added the high contamination persistance functionality
        '''              AG 20/06/2012 - Executions for CONTROLS have to be processed in the same way than Executions for PATIENT Samples
        '''              AG 27/05/2013 - Add the new sample types LIQ, SEM
        '''              AJ 19/03/2015 - Added the activeAnalyzer parameter. Needed for solving contaminations by AnalyzerModel
        ''' </remarks>
        <Obsolete("Use the new WSSorter class instead.")>
        Public Function SortWSExecutionsByContamination(ByVal activeAnalyzer As String, ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutions As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim bestContaminationNumber As Integer = Integer.MaxValue
            'Dim currentContaminationNumber As Integer
            Dim contaminationsDataDS As ContaminationsDS = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all R1 Contaminations 
                        Dim myContaminationsDelegate As New ContaminationsDelegate
                        resultData = ContaminationsDelegate.GetContaminationsByType(dbConnection, "R1")

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            contaminationsDataDS = DirectCast(resultData.SetDatos, ContaminationsDS)

                            Dim highContaminationPersitance As Integer = 0

                            resultData = SwParametersDelegate.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS.ToString, Nothing)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                highContaminationPersitance = CInt(resultData.SetDatos)
                            End If

                            Dim Stats() As Boolean = {True, False}
                            Dim SampleClasses() As String = {"BLANK", "CALIB", "CTRL", "PATIENT"}

                            'TR 27/05/2013 -Get a list of sample types separated by commas
                            Dim SampleTypes() As String = Nothing
                            Dim myMasterDataDelegate As New MasterDataDelegate
                            resultData = MasterDataDelegate.GetSampleTypes(dbConnection)
                            If Not resultData.HasError Then
                                SampleTypes = resultData.SetDatos.ToString.Split(CChar(","))
                            End If

                            'Dim SampleTypes() As String = {"SER", "URI", "PLM", "WBL", "CSF", "LIQ", "SEM"}
                            Dim stdOrderTestsCount As Integer = 0

                            'Different Stat, SampleClasses and SampleTypes in WorkSession
                            Dim differentStatValues As List(Of Boolean) = (From wse In pExecutions.twksWSExecutions Select wse.StatFlag Distinct).ToList
                            Dim differentSampleClassValues As List(Of String) = (From wse In pExecutions.twksWSExecutions Select wse.SampleClass Distinct).ToList
                            Dim differentSampleTypeValues As List(Of String) = (From wse In pExecutions.twksWSExecutions Select wse.SampleType Distinct).ToList

                            Dim returnDS As New ExecutionsDS
                            For Each StatFlag In Stats
                                If differentStatValues.Contains(StatFlag) Then 'Do not perform business if not necessary

                                    For Each SampleClass In SampleClasses
                                        If differentSampleClassValues.Contains(SampleClass) Then 'Do not perform business if not necessary

                                            'AG 27/04/2012 AG + RH - Search the elementid for each sampleClass (the elementid code can be repeated from different sampleclasses)
                                            'Dim Elements = (From wse In pExecutions.twksWSExecutions _
                                            '              Select wse.ElementID Distinct).ToList()
                                            Dim Elements = (From wse In pExecutions.twksWSExecutions _
                                                            Where wse.SampleClass = SampleClass _
                                                            Select wse.ElementID Distinct).ToList()
                                            'AG 27/04/2012

                                            For Each elementID In Elements
                                                Dim ID = elementID
                                                Dim Stat As Boolean = StatFlag
                                                Dim SClass As String = SampleClass

                                                For Each sortedSampleType In SampleTypes
                                                    'AG 30/08/2012 - add OrElse differentSampleTypeValues.Contains("") because when create a ws only with blanks there not exists sampletype!!
                                                    If differentSampleTypeValues.Contains(sortedSampleType) OrElse _
                                                       (differentSampleClassValues.Count = 1 AndAlso differentSampleClassValues.Contains("BLANK")) Then

                                                        Dim SType As String = sortedSampleType

                                                        Dim AllTestTypeOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) 'All test type order tests executions
                                                        Dim OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) = Nothing  'Only STD test order tests executions

                                                        'NEW: When a patient OR CTRL has Ise & std test executions the ISE executions are the first
                                                        If SClass = "PATIENT" OrElse SClass = "CTRL" Then 'Apply OrderBy sample type order {"SER", "URI", "PLM", "WBL", "CSF"} (case one PATIENT with several sample types) + execution type
                                                            AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                             Where wse.StatFlag = Stat AndAlso _
                                                                             wse.SampleClass = SClass AndAlso _
                                                                             wse.SampleType = SType AndAlso _
                                                                             wse.ElementID = ID _
                                                                             Select wse Order By wse.ExecutionType).ToList()

                                                            If AllTestTypeOrderTests.Count > 0 Then
                                                                'Look for the STD orderTests
                                                                OrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                              Where wse.StatFlag = Stat AndAlso _
                                                                              wse.SampleClass = SClass AndAlso _
                                                                              wse.SampleType = SType AndAlso _
                                                                              wse.ElementID = ID AndAlso _
                                                                              wse.ExecutionType = "PREP_STD" _
                                                                              Select wse).ToList()
                                                                stdOrderTestsCount = OrderTests.Count
                                                            Else
                                                                stdOrderTestsCount = 0
                                                            End If

                                                        Else 'Do not apply OrderBy & do not take care about sample type order inside the same SAMPLE
                                                            'AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                            '                         Where wse.StatFlag = Stat AndAlso _
                                                            '                         wse.SampleClass = SClass AndAlso _
                                                            '                         wse.ElementID = ID _
                                                            '                         Select wse).ToList()

                                                            'RH 16/05/2012 Sort non PATIENT executions by time (ReadingCycle Descending)
                                                            AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                                     Where wse.StatFlag = Stat AndAlso _
                                                                                     wse.SampleClass = SClass AndAlso _
                                                                                     wse.ElementID = ID _
                                                                                     Select wse Order By wse.ReadingCycle Descending).ToList()
                                                            If AllTestTypeOrderTests.Count > 0 Then
                                                                'Look for the STD orderTests
                                                                OrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                              Where wse.StatFlag = Stat AndAlso _
                                                                              wse.SampleClass = SClass AndAlso _
                                                                              wse.ElementID = ID AndAlso _
                                                                              wse.ExecutionType = "PREP_STD" _
                                                                              Select wse).ToList()
                                                                stdOrderTestsCount = OrderTests.Count
                                                            Else
                                                                stdOrderTestsCount = 0
                                                            End If

                                                        End If

                                                        Dim OrderContaminationNumber As Integer = 0
                                                        If stdOrderTestsCount > 0 Then
                                                            OrderContaminationNumber = GetContaminationNumber(contaminationsDataDS, OrderTests, highContaminationPersitance)
                                                        End If

                                                        ManageContaminations(activeAnalyzer, dbConnection, returnDS, contaminationsDataDS, highContaminationPersitance, OrderTests, AllTestTypeOrderTests, OrderContaminationNumber)

                                                        If SClass <> "PATIENT" AndAlso SClass <> "CTRL" Then Exit For 'For blank, calib do not take care about the sample type inside the same SAMPLE

                                                        'AG 19/02/2014 - #1514
                                                        AllTestTypeOrderTests = Nothing
                                                        OrderTests = Nothing
                                                        'AG 19/02/2014 - #1514

                                                    End If
                                                Next 'For each mySampleType
                                            Next 'For each elementID

                                        End If
                                    Next 'For each SampleClass

                                End If
                            Next 'For each StatFlag

                            'AG 19/02/2014 - #1514
                            differentStatValues = Nothing
                            differentSampleClassValues = Nothing
                            differentSampleTypeValues = Nothing
                            'AG 19/02/2014 - #1514

                            resultData.SetDatos = returnDS
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.SortWSExecutionsByContamination", EventLogEntryType.Error, False)

            End Try

            'AG 19/02/2014 - #1514
            bestResult = Nothing
            currentResult = Nothing
            'AG 19/02/2014 - #1514

            Return resultData
        End Function

        ''' <summary>
        ''' Get all Blanks requested for the specified Analyzer WorkSession and needed to create the 
        ''' corresponding executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test for which a Rerun has been requested. Optional parameter</param>
        ''' <param name="pPostDilutionType">Type of post Dilution to apply when a Rerun has been requested. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of executions for requested Blanks</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2011
        ''' Modified by: TR 29/08/2011 - Declarations moved outside the loops
        ''' </remarks>
        Public Function CreateBlankExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                              ByVal pWorkSessionID As String, Optional ByVal pOrderTestID As Integer = -1, _
                                              Optional ByVal pPostDilutionType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calledForRerun As Boolean = (pOrderTestID <> -1)

                        Dim myWSOrderTests As New WSOrderTestsDelegate
                        resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", pOrderTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim orderTestsForExecDS As WSOrderTestsForExecutionsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                            Dim blankExecutionsDS As New ExecutionsDS
                            If (orderTestsForExecDS.WSOrderTestsForExecutions.Rows.Count > 0) Then
                                'Get all different OrderTestIDs returned (more than one record can be returned for a Blank Order Test: one 
                                'for each Reagent needed plus one for the Special Solution needed to execute the Blank)
                                Dim lstWSOTs As List(Of Integer)
                                lstWSOTs = (From a In orderTestsForExecDS.WSOrderTestsForExecutions _
                                          Select a.OrderTestID Distinct).ToList()

                                Dim reRunNumber As Integer
                                Dim numReplicates As Integer
                                Dim reqElemNoPos As Boolean = False
                                Dim currentOrderTestID As Integer
                                Dim myExecutionsDAO As New twksWSExecutionsDAO
                                Dim executionRow As ExecutionsDS.twksWSExecutionsRow
                                Dim noPosElementByOrderTest As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)
                                Dim lstWSReqElements As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow) 'AG 19/02/2014 - #1514

                                For Each blankOT As Integer In lstWSOTs
                                    currentOrderTestID = blankOT

                                    'Get all Elements required for the Blank currently processed
                                    'Dim lstWSReqElements As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow) 'AG 19/02/2014 - #1514
                                    lstWSReqElements = (From b As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                       Where b.OrderTestID = currentOrderTestID _
                                                      Select b).ToList()

                                    reqElemNoPos = False
                                    For numElem As Integer = 0 To lstWSReqElements.Count - 1
                                        If (numElem = 0) Then
                                            'Get number of replicates requested for the Blank currently processed
                                            numReplicates = lstWSReqElements(numElem).ReplicatesNumber

                                            reRunNumber = 1
                                            If (calledForRerun) Then
                                                'Get the rerun number for the Blank currently processed
                                                resultData = myExecutionsDAO.GetOrderTestRerunNumber(dbConnection, pAnalyzerID, pWorkSessionID, currentOrderTestID)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    reRunNumber = DirectCast(resultData.SetDatos, Integer)
                                                Else
                                                    'Error getting the rerun number
                                                    Exit For
                                                End If
                                            End If
                                        End If

                                        'Verify if the required Element is positioned
                                        noPosElementByOrderTest = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                                  Where a.OrderTestID = currentOrderTestID _
                                                                AndAlso a.ElementStatus = "NOPOS" _
                                                                 Select a).ToList
                                        reqElemNoPos = (noPosElementByOrderTest.Count > 0)

                                        'If it is the last required Element for the Blank
                                        If (numElem = lstWSReqElements.Count - 1) Then
                                            'Add one Execution for each requested Blank replicate
                                            For i As Integer = 1 To numReplicates
                                                executionRow = blankExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow()
                                                executionRow.AnalyzerID = pAnalyzerID
                                                executionRow.WorkSessionID = pWorkSessionID
                                                executionRow.OrderTestID = currentOrderTestID
                                                executionRow.MultiItemNumber = 1
                                                executionRow.RerunNumber = reRunNumber
                                                executionRow.ExecutionStatus = IIf(reqElemNoPos, "LOCKED", "PENDING").ToString
                                                executionRow.ExecutionType = "PREP_STD"
                                                executionRow.ReplicateNumber = i
                                                executionRow.SampleClass = "BLANK"

                                                If (Not calledForRerun) Then
                                                    executionRow.StatFlag = False
                                                Else
                                                    executionRow.StatFlag = True   'Reruns are always urgent
                                                    executionRow.PostDilutionType = pPostDilutionType
                                                End If
                                                blankExecutionsDS.twksWSExecutions.Rows.Add(executionRow)
                                            Next
                                        End If
                                    Next
                                Next

                                'AG 19/02/2014 - #1514
                                lstWSOTs = Nothing
                                noPosElementByOrderTest = Nothing
                                lstWSReqElements = Nothing
                                'AG 19/02/2014 - #1514

                            End If

                            'Return all Executions for requested BLANKS
                            resultData.SetDatos = blankExecutionsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CreateBlankExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (resultData)
        End Function

        ''' <summary>
        ''' Get all Calibrators requested for the specified Analyzer WorkSession and needed to create the 
        ''' corresponding executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test for which a Rerun has been requested. Optional parameter</param>
        ''' <param name="pPostDilutionType">Type of post Dilution to apply when a Rerun has been requested. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of executions for requested Calibrators</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2011
        ''' Modified by: TR 29/08/2011 - Declarations moved outside the loops
        '''              SA 18/04/2012 - Filter required Elements by TubeContent = CALIB to avoid returning also Reagents
        ''' </remarks>
        Public Function CreateCalibratorExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                   ByVal pWorkSessionID As String, Optional ByVal pOrderTestID As Integer = -1, _
                                                   Optional ByVal pPostDilutionType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calledForRerun As Boolean = (pOrderTestID <> -1)

                        Dim myWSOrderTests As New WSOrderTestsDelegate
                        resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, "CALIB", pOrderTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim orderTestsForExecDS As WSOrderTestsForExecutionsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                            Dim calibExecutionsDS As New ExecutionsDS
                            If (orderTestsForExecDS.WSOrderTestsForExecutions.Rows.Count > 0) Then
                                'Get all different OrderTestIDs returned (more than one record can be returned for a Calibrator 
                                'Order Test for Multipoint Calibrators)
                                Dim lstWSOTs As List(Of Integer) = (From a In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                                   Where a.TubeContent = "CALIB" _
                                                                  Select a.OrderTestID Distinct).ToList()

                                Dim reRunNumber As Integer
                                Dim numReplicates As Integer
                                Dim currentOrderTestID As Integer
                                Dim reqElemNoPos As Boolean = False
                                Dim myExecutionsDAO As New twksWSExecutionsDAO
                                Dim executionRow As ExecutionsDS.twksWSExecutionsRow
                                Dim lstWSReqElements As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)
                                Dim noPosElementByOrderTest As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)

                                For Each calibOT As Integer In lstWSOTs
                                    currentOrderTestID = calibOT

                                    'Get all Elements required for the Calibrator currently processed
                                    lstWSReqElements = (From b As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                       Where b.OrderTestID = currentOrderTestID _
                                                     AndAlso b.TubeContent = "CALIB" _
                                                 AndAlso Not b.IsMultiItemNumberNull _
                                                    Order By b.MultiItemNumber _
                                                      Select b).ToList()
                                    reqElemNoPos = False

                                    'Get Number of Replicates and Rerun Number
                                    If (lstWSReqElements.Count > 0) Then
                                        'Get number of replicates requested for the Calibrator currently processed
                                        numReplicates = lstWSReqElements(0).ReplicatesNumber

                                        reRunNumber = 1
                                        If (calledForRerun) Then
                                            'Get the rerun number for the Calibrator currently processed
                                            resultData = myExecutionsDAO.GetOrderTestRerunNumber(dbConnection, pAnalyzerID, pWorkSessionID, currentOrderTestID)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                reRunNumber = DirectCast(resultData.SetDatos, Integer)
                                            Else
                                                'Error getting the rerun number
                                                Exit For
                                            End If
                                        End If
                                    End If

                                    'Insert the executions for the Calibrator (for each Replicate all the Calibrator points)
                                    For i As Integer = 1 To numReplicates
                                        For numElem As Integer = 0 To lstWSReqElements.Count - 1
                                            'When there is only one Point for the processed Calibrator but the MultiItemNumber in the 
                                            'correspondent RequiredElement is greater than one, set MultiItemNumber to 1 (Calculations 
                                            'module needs the Execution created in that way)
                                            If (lstWSReqElements.Count = 1) AndAlso (lstWSReqElements(0).MultiItemNumber > 1) Then
                                                lstWSReqElements(0).MultiItemNumber = 1
                                            End If

                                            'Verify if the required Element is positioned
                                            noPosElementByOrderTest = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                                      Where a.OrderTestID = currentOrderTestID _
                                                                    AndAlso a.ElementStatus = "NOPOS" _
                                                                     Select a).ToList
                                            reqElemNoPos = (noPosElementByOrderTest.Count > 0)

                                            'Add the replicate for the currently processed Calibrator point
                                            executionRow = calibExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow()
                                            executionRow.AnalyzerID = pAnalyzerID
                                            executionRow.WorkSessionID = pWorkSessionID
                                            executionRow.OrderTestID = currentOrderTestID
                                            executionRow.MultiItemNumber = lstWSReqElements(numElem).MultiItemNumber
                                            executionRow.RerunNumber = reRunNumber
                                            executionRow.ExecutionStatus = IIf(reqElemNoPos, "LOCKED", "PENDING").ToString
                                            executionRow.ExecutionType = "PREP_STD"
                                            executionRow.ReplicateNumber = i
                                            executionRow.SampleClass = "CALIB"

                                            If (Not calledForRerun) Then
                                                executionRow.StatFlag = False
                                            Else
                                                executionRow.StatFlag = True  'Reruns are always Urgent
                                                executionRow.PostDilutionType = pPostDilutionType
                                            End If
                                            calibExecutionsDS.twksWSExecutions.Rows.Add(executionRow)
                                        Next
                                    Next
                                Next

                                'AG 19/02/2014 - #1514
                                lstWSOTs = Nothing
                                lstWSReqElements = Nothing
                                noPosElementByOrderTest = Nothing
                                'AG 19/02/2014 - #1514
                            End If

                            'Return all Executions for requested CALIBRATORS
                            resultData.SetDatos = calibExecutionsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CreateCalibratorExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Controls requested for the specified Analyzer WorkSession and needed to create the corresponding executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test for which a Rerun has been requested. Optional parameter</param>
        ''' <param name="pPostDilutionType">Type of post Dilution to apply when a Rerun has been requested. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of executions for requested Controls</returns>
        ''' <remarks>
        ''' Created by:  SA 10/02/2011
        ''' Modified by: AG 20/06/2011 - Added code to avoid creation of duplicate Executions due the query in GetOrderTestsForExecutions now 
        '''                              returns also the SPEC_SOL for Dilutions
        '''              TR 29/08/2011 - Declarations moved outside the loops
        '''              SA 19/06/2012 - Value of ExecutionType is set according the TestType of each CTRL OrderTest: if STD then PREP_STD,
        '''                              and if ISE then PREP_ISE    
        '''              SA 28/03/2014 - BT #1535 - Changes to avoid creation of repeated Executions. Function GetOrderTestsForExecutions returns
        '''                                         several records for each Control Order Test (one for each Element required to execute the Control:
        '''                                         REAGENTS, CONTROL SAMPLE and DILUTION SOLUTIONS), and the current code assumes all records are 
        '''                                         sorted by OrderTestID, but this is not true in all cases, and when Order Tests are disordered, as 
        '''                                         many Executions as required Elements are created for each Order Test and this is not correct  
        ''' </remarks>
        Public Function CreateControlExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pWorkSessionID As String, Optional ByVal pOrderTestID As Integer = -1, _
                                                Optional ByVal pPostDilutionType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calledForRerun As Boolean = (pOrderTestID <> -1)

                        Dim myWSOrderTests As New WSOrderTestsDelegate
                        resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, "CTRL", pOrderTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim orderTestsForExecDS As WSOrderTestsForExecutionsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                            Dim ctrlExecutionsDS As New ExecutionsDS
                            If (orderTestsForExecDS.WSOrderTestsForExecutions.Rows.Count > 0) Then
                                'BT #1535 - ctrlExecutionsDS is loaded in a different way to avoid creation of repeated Executions
                                Dim controlSamplesList As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)
                                controlSamplesList = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                     Where a.TubeContent = "CTRL" _
                                                    Select a).ToList()

                                Dim reRunNumber As Integer = 1
                                Dim reqElemNoPos As Boolean = False
                                Dim myExecutionsDAO As New twksWSExecutionsDAO
                                Dim executionRow As ExecutionsDS.twksWSExecutionsRow
                                Dim noPosElementByOrderTest As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)

                                For Each controlRow As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In controlSamplesList
                                    'Get the rerun number for the Control currently processed
                                    reRunNumber = 1
                                    If (calledForRerun) Then
                                        resultData = myExecutionsDAO.GetOrderTestRerunNumber(dbConnection, pAnalyzerID, pWorkSessionID, controlRow.OrderTestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            reRunNumber = DirectCast(resultData.SetDatos, Integer)
                                        Else
                                            'Error getting the rerun number for the Control
                                            Exit For
                                        End If
                                    End If

                                    'Get the Execution Status... verify if at least one of the Elements required for the Control is not positioned 
                                    noPosElementByOrderTest = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                              Where a.OrderTestID = controlRow.OrderTestID _
                                                            AndAlso a.ElementStatus = "NOPOS" _
                                                             Select a).ToList
                                    reqElemNoPos = (noPosElementByOrderTest.Count > 0)

                                    'Add one Execution of the Control for each requested Control replicate
                                    For i As Integer = 1 To controlRow.ReplicatesNumber
                                        executionRow = ctrlExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow()
                                        executionRow.AnalyzerID = pAnalyzerID
                                        executionRow.WorkSessionID = pWorkSessionID
                                        executionRow.OrderTestID = controlRow.OrderTestID
                                        executionRow.MultiItemNumber = 1
                                        executionRow.RerunNumber = reRunNumber
                                        executionRow.ExecutionStatus = IIf(reqElemNoPos, "LOCKED", "PENDING").ToString
                                        executionRow.ExecutionType = IIf(controlRow.TestType = "STD", "PREP_STD", "PREP_ISE").ToString
                                        executionRow.ReplicateNumber = i
                                        executionRow.SampleClass = "CTRL"

                                        If (Not calledForRerun) Then
                                            executionRow.StatFlag = False
                                        Else
                                            executionRow.StatFlag = True  'Reruns are always Urgent
                                            executionRow.PostDilutionType = pPostDilutionType
                                        End If
                                        ctrlExecutionsDS.twksWSExecutions.Rows.Add(executionRow)
                                    Next
                                Next
                                controlSamplesList = Nothing
                                noPosElementByOrderTest = Nothing

                                'Dim reRunNumber As Integer
                                'Dim reqElemNoPos As Boolean
                                'Dim otTreated As Integer = -1
                                'Dim flagOtTreated As Boolean = False
                                'Dim myExecutionsDAO As New twksWSExecutionsDAO
                                'Dim executionRow As ExecutionsDS.twksWSExecutionsRow
                                'Dim noPosElementByOrderTest As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)

                                'For Each ctrlRow As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions
                                '    'Get the rerun number for the Control currently processed
                                '    reRunNumber = 1

                                '    flagOtTreated = True
                                '    If (ctrlRow.OrderTestID <> otTreated) Then
                                '        otTreated = ctrlRow.OrderTestID
                                '        flagOtTreated = False
                                '    End If

                                '    If (calledForRerun) Then
                                '        resultData = myExecutionsDAO.GetOrderTestRerunNumber(dbConnection, pAnalyzerID, pWorkSessionID, ctrlRow.OrderTestID)
                                '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                '            reRunNumber = DirectCast(resultData.SetDatos, Integer)
                                '        Else
                                '            'Error getting the rerun number for the Control
                                '            Exit For
                                '        End If
                                '    End If

                                '    If (Not flagOtTreated) Then
                                '        'Get the Execution Status... verify if the Control is not positioned 
                                '        noPosElementByOrderTest = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                '                                  Where a.OrderTestID = ctrlRow.OrderTestID _
                                '                                AndAlso a.ElementStatus = "NOPOS" _
                                '                                 Select a).ToList
                                '        reqElemNoPos = (noPosElementByOrderTest.Count > 0)

                                '        'Add one Execution of the Control for each requested Control replicate
                                '        For i As Integer = 1 To ctrlRow.ReplicatesNumber
                                '            executionRow = ctrlExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow()
                                '            executionRow.AnalyzerID = pAnalyzerID
                                '            executionRow.WorkSessionID = pWorkSessionID
                                '            executionRow.OrderTestID = ctrlRow.OrderTestID
                                '            executionRow.MultiItemNumber = 1
                                '            executionRow.RerunNumber = reRunNumber
                                '            executionRow.ExecutionStatus = IIf(reqElemNoPos, "LOCKED", "PENDING").ToString
                                '            executionRow.ExecutionType = IIf(ctrlRow.TestType = "STD", "PREP_STD", "PREP_ISE").ToString
                                '            executionRow.ReplicateNumber = i
                                '            executionRow.SampleClass = "CTRL"

                                '            If (Not calledForRerun) Then
                                '                executionRow.StatFlag = False
                                '            Else
                                '                executionRow.StatFlag = True  'Reruns are always Urgent
                                '                executionRow.PostDilutionType = pPostDilutionType
                                '            End If
                                '            ctrlExecutionsDS.twksWSExecutions.Rows.Add(executionRow)
                                '        Next
                                '    End If
                                'Next

                                'noPosElementByOrderTest = Nothing 'AG 19/02/2014 - #1514

                            End If

                            'Return all Executions for requested CONTROLS
                            resultData.SetDatos = ctrlExecutionsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CreateControlExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Patient Samples requested for the specified Analyzer WorkSession and needed to create the 
        ''' corresponding executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test for which a Rerun has been requested. Optional parameter</param>
        ''' <param name="pPostDilutionType">Type of post Dilution to apply when a Rerun has been requested. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of executions for requested Patient Samples</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2011
        ''' Modified by: SA 19/04/2011 - Field ElementID has to be also informed in the ExecutionsDS to return
        '''              AG 20/06/2011 - Added code to avoid creation of duplicate Executions due the query in GetOrderTestsForExecutions now 
        '''                              returns also the SPEC_SOL for Dilutions
        '''              SA 28/03/2014 - BT #1535 - Changes to avoid creation of repeated Executions. Function GetOrderTestsForExecutions returns
        '''                                         several records for each Patient Order Test (one for each Element required for the Patient Sample:
        '''                                         REAGENTS, PATIENT SAMPLE and DILUTION SOLUTIONS), and the current code assumes all records are 
        '''                                         sorted by OrderTestID, but this is not true in all cases, and when Order Tests are disordered, as 
        '''                                         many Executions as required Elements are created for each Order Test and this is not correct 
        ''' </remarks>
        Public Function CreatePatientExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pWorkSessionID As String, Optional ByVal pOrderTestID As Integer = -1, _
                                                Optional ByVal pPostDilutionType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calledForRerun As Boolean = (pOrderTestID <> -1)

                        Dim myWSOrderTests As New WSOrderTestsDelegate
                        resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, "PATIENT", pOrderTestID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim orderTestsForExecDS As WSOrderTestsForExecutionsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                            Dim patientExecutionsDS As New ExecutionsDS
                            If (orderTestsForExecDS.WSOrderTestsForExecutions.Rows.Count > 0) Then
                                'BT #1535 - patientExecutionsDS is loaded in a different way to avoid creation of repeated Executions
                                Dim patientSamplesList As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)
                                patientSamplesList = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                     Where a.TubeContent = "PATIENT" _
                                                    Select a).ToList()

                                Dim reRunNumber As Integer = 1
                                Dim reqElemNoPos As Boolean = False
                                Dim myExecutionsDAO As New twksWSExecutionsDAO
                                Dim executionRow As ExecutionsDS.twksWSExecutionsRow
                                Dim noPosElementByOrderTest As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)

                                For Each patientRow As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In patientSamplesList
                                    'Get the rerun number for the Patient Sample currently processed 
                                    reRunNumber = 1
                                    If (calledForRerun) Then
                                        resultData = myExecutionsDAO.GetOrderTestRerunNumber(dbConnection, pAnalyzerID, pWorkSessionID, patientRow.OrderTestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            reRunNumber = DirectCast(resultData.SetDatos, Integer)
                                        Else
                                            'Error getting the rerun number for the Patient Sample
                                            Exit For
                                        End If
                                    End If

                                    'Get the Execution Status... verify if at least one of the Elements required for the Patient Sample is not positioned 
                                    noPosElementByOrderTest = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                                              Where a.OrderTestID = patientRow.OrderTestID _
                                                            AndAlso a.ElementStatus = "NOPOS" _
                                                             Select a).ToList
                                    reqElemNoPos = (noPosElementByOrderTest.Count > 0)

                                    'Add one Execution of the Patient Sample for each requested Patient replicate
                                    For i As Integer = 1 To patientRow.ReplicatesNumber
                                        executionRow = patientExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow()
                                        executionRow.AnalyzerID = pAnalyzerID
                                        executionRow.WorkSessionID = pWorkSessionID
                                        executionRow.OrderTestID = patientRow.OrderTestID
                                        executionRow.MultiItemNumber = 1
                                        executionRow.ElementID = patientRow.ElementID
                                        executionRow.RerunNumber = reRunNumber
                                        executionRow.ExecutionStatus = IIf(reqElemNoPos, "LOCKED", "PENDING").ToString

                                        If (patientRow.TestType = "STD") Then
                                            executionRow.ExecutionType = "PREP_STD"

                                        ElseIf (patientRow.TestType = "ISE") Then
                                            executionRow.ExecutionType = "PREP_ISE"
                                        End If

                                        executionRow.ReplicateNumber = i
                                        executionRow.SampleClass = "PATIENT"

                                        If (Not calledForRerun) Then
                                            executionRow.StatFlag = patientRow.StatFlag
                                        Else
                                            executionRow.StatFlag = True  'Reruns are always Urgent
                                            executionRow.PostDilutionType = pPostDilutionType
                                        End If
                                        patientExecutionsDS.twksWSExecutions.Rows.Add(executionRow)
                                    Next
                                Next
                                patientSamplesList = Nothing
                                noPosElementByOrderTest = Nothing

                                'Dim otTreated As Integer = -1
                                'Dim reRunNumber As Integer = 1
                                'Dim reqElemNoPos As Boolean = False
                                'Dim flagOtTreated As Boolean = False
                                'Dim myExecutionsDAO As New twksWSExecutionsDAO
                                'Dim executionRow As ExecutionsDS.twksWSExecutionsRow
                                'Dim noPosElementByOrderTest As List(Of WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow)

                                'For Each patientRow As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions
                                '    reRunNumber = 1

                                '    flagOtTreated = True
                                '    If (patientRow.OrderTestID <> otTreated) Then
                                '        otTreated = patientRow.OrderTestID
                                '        flagOtTreated = False
                                '    End If

                                '    'Get the rerun number for the Patient Sample currently processed (OrderTestID is informed when a manual
                                '    'repetition has been requested)
                                '    If (calledForRerun) Then
                                '        resultData = myExecutionsDAO.GetOrderTestRerunNumber(dbConnection, pAnalyzerID, pWorkSessionID, patientRow.OrderTestID)
                                '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                '            reRunNumber = DirectCast(resultData.SetDatos, Integer)
                                '        Else
                                '            'Error getting the rerun number for the Patient Sample
                                '            Exit For
                                '        End If
                                '    End If

                                '    If (Not flagOtTreated) Then
                                '        'Get the Execution Status... verify if the Patient Sample is not positioned 
                                '        noPosElementByOrderTest = (From a As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In orderTestsForExecDS.WSOrderTestsForExecutions _
                                '                                  Where a.OrderTestID = patientRow.OrderTestID _
                                '                                AndAlso a.ElementStatus = "NOPOS" _
                                '                                 Select a).ToList
                                '        reqElemNoPos = (noPosElementByOrderTest.Count > 0)

                                '        'Add one Execution of the Patient Sample for each requested Patient replicate
                                '        For i As Integer = 1 To patientRow.ReplicatesNumber
                                '            executionRow = patientExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow()
                                '            executionRow.AnalyzerID = pAnalyzerID
                                '            executionRow.WorkSessionID = pWorkSessionID
                                '            executionRow.OrderTestID = patientRow.OrderTestID
                                '            executionRow.MultiItemNumber = 1
                                '            executionRow.ElementID = patientRow.ElementID
                                '            executionRow.RerunNumber = reRunNumber
                                '            executionRow.ExecutionStatus = IIf(reqElemNoPos, "LOCKED", "PENDING").ToString

                                '            If (patientRow.TestType = "STD") Then
                                '                executionRow.ExecutionType = "PREP_STD"

                                '            ElseIf (patientRow.TestType = "ISE") Then
                                '                executionRow.ExecutionType = "PREP_ISE"
                                '            End If

                                '            executionRow.ReplicateNumber = i
                                '            executionRow.SampleClass = "PATIENT"

                                '            If (Not calledForRerun) Then
                                '                executionRow.StatFlag = patientRow.StatFlag
                                '            Else
                                '                executionRow.StatFlag = True  'Reruns are always Urgent
                                '                executionRow.PostDilutionType = pPostDilutionType
                                '            End If
                                '            patientExecutionsDS.twksWSExecutions.Rows.Add(executionRow)
                                '        Next
                                '    End If
                                'Next
                                'noPosElementByOrderTest = Nothing 'AG 19/02/2014 - #1514
                            End If

                            'Return all Executions for requested PATIENT SAMPLES
                            resultData.SetDatos = patientExecutionsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CreatePatientExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the list of Execution's Element groups sorted by Contamination
        ''' The previous group have no changes, this algorithm changes the current group executions to minimize the number of contaminations
        ''' between [Previous Group (last execution) -> Current Group (First execution) + Current Group All his executions]
        ''' 
        ''' </summary>
        ''' <param name="pExecutions">Dataset with structure of view vwksWSExecutions</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the ordered data (view vwksWSExecutions)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH - 08/06/2010
        ''' Modified by: SA            - Changed function called to get the R1 Contaminations for a one in the Delegated Class
        '''              RH 08/04/2011 - Changed order field from OrderID to ElementID
        '''              RH 29/09/2011 - Sort PATIENT executions. Sort inter SampleClass executions. Keep locked exections in place.
        '''              AG 09/11/2011 - Change the whole method business (the old method SortWSExecutionsByElementGroupContamination is kept)
        '''                              this is an adapted version of the ols SortWSExecutionsByContamination method
        '''              AG 25/11/2011 - Added the high contamination persistance functionality
        '''              AG 20/06/2012 - Executions for CONTROLS have to be processed in the same way than Executions for PATIENT Samples
        '''              AG 27/05/2013 - Add new samples types LIQ and SER
        '''              AJ 19/03/2015 - Added the activeAnalyzer parameter. Needed for solving contaminations by AnalyzerModel
        ''' </remarks>
        Public Function SortWSExecutionsByElementGroupContaminationNew(ByVal activeAnalyzer As String, ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutions As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim bestContaminationNumber As Integer = Integer.MaxValue
            'Dim currentContaminationNumber As Integer
            Dim contaminationsDataDS As ContaminationsDS = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all R1 Contaminations 
                        resultData = ContaminationsDelegate.GetContaminationsByType(dbConnection, "R1")

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            contaminationsDataDS = DirectCast(resultData.SetDatos, ContaminationsDS)

                            Dim highContaminationPersitance As Integer = 0

                            resultData = SwParametersDelegate.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS.ToString, Nothing)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                highContaminationPersitance = CInt(resultData.SetDatos)
                            End If

                            Dim Stats() As Boolean = {True, False}
                            Dim SampleClasses() As String = {"BLANK", "CALIB", "CTRL", "PATIENT"}
                            'TR 27/05/2013 -Get a list of sample types separated by commas
                            Dim SampleTypes() As String = Nothing
                            Dim myMasterDataDelegate As New MasterDataDelegate

                            resultData = MasterDataDelegate.GetSampleTypes(dbConnection)
                            If Not resultData.HasError Then
                                SampleTypes = resultData.SetDatos.ToString.Split(CChar(","))
                            End If
                            'Dim SampleTypes() As String = {"SER", "URI", "PLM", "WBL", "CSF", "LIQ", "SEM"}
                            Dim stdOrderTestsCount As Integer = 0

                            'Different Stat, SampleClasses and SampleTypes in WorkSession
                            Dim differentStatValues As List(Of Boolean) = (From wse In pExecutions.twksWSExecutions Select wse.StatFlag Distinct).ToList
                            Dim differentSampleClassValues As List(Of String) = (From wse In pExecutions.twksWSExecutions Select wse.SampleClass Distinct).ToList
                            Dim differentSampleTypeValues As List(Of String) = (From wse In pExecutions.twksWSExecutions Select wse.SampleType Distinct).ToList


                            Dim returnDS As New ExecutionsDS
                            Dim previousElementLastReagentID As Integer = -1
                            Dim PreviousReagentsIDList As New List(Of Integer) 'List of previous reagents sent before the current previousElementLastReagentID, 
                            '                                                   remember this information in order to check the high contamination persistance
                            '                                                   (One Item for each different OrderTest)

                            Dim previousElementLastMaxReplicates As Integer = 1
                            Dim previousOrderTestMaxReplicatesList As New List(Of Integer) 'AG 19/12/2011 - Same item number as previous list, indicates the replicate number for each item in previous list

                            Dim OrderContaminationNumber As Integer = 0

                            For Each StatFlag In Stats
                                If differentStatValues.Contains(StatFlag) Then

                                    For Each SampleClass In SampleClasses
                                        If differentSampleClassValues.Contains(SampleClass) Then

                                            'AG 27/04/2012 AG + RH - Search the elementid for each sampleClass (the elementid code can be repeated from different sampleclasses)
                                            'Dim Elements = (From wse In pExecutions.twksWSExecutions _
                                            '              Select wse.ElementID Distinct).ToList()
                                            Dim Elements = (From wse In pExecutions.twksWSExecutions _
                                                            Where wse.SampleClass = SampleClass _
                                                            Select wse.ElementID Distinct).ToList()
                                            'AG 27/04/2012

                                            For Each elementID In Elements
                                                Dim ID = elementID
                                                Dim Stat As Boolean = StatFlag
                                                Dim SClass As String = SampleClass

                                                For Each sortedSampleType In SampleTypes
                                                    'AG 30/08/2012 - add OrElse differentSampleTypeValues.Contains("") because when create a ws only with blanks there not exists sampletype!!
                                                    If differentSampleTypeValues.Contains(sortedSampleType) OrElse _
                                                       (differentSampleClassValues.Count = 1 AndAlso differentSampleClassValues.Contains("BLANK")) Then

                                                        Dim SType As String = sortedSampleType

                                                        Dim AllTestTypeOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) 'All test type order tests
                                                        Dim OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) = Nothing  'Only STD test order tests

                                                        'NEW: When a patient or ctrl has Ise & std test executions the ISE executions are the first
                                                        If SClass = "PATIENT" OrElse SClass = "CTRL" Then 'Apply OrderBy sample type order {"SER", "URI", "PLM", "WBL", "CSF"} (case one PATIENT with several sample types) + execution type
                                                            AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                             Where wse.StatFlag = Stat AndAlso _
                                                                             wse.SampleClass = SClass AndAlso _
                                                                             wse.SampleType = SType AndAlso _
                                                                             wse.ElementID = ID _
                                                                             Select wse Order By wse.ExecutionType).ToList()

                                                            If AllTestTypeOrderTests.Count > 0 Then
                                                                'Look for the STD orderTests
                                                                OrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                         Where wse.StatFlag = Stat AndAlso _
                                                                         wse.SampleClass = SClass AndAlso _
                                                                         wse.SampleType = SType AndAlso _
                                                                         wse.ElementID = ID AndAlso _
                                                                         wse.ExecutionType = "PREP_STD" _
                                                                         Select wse).ToList()
                                                                stdOrderTestsCount = OrderTests.Count
                                                            Else
                                                                stdOrderTestsCount = 0
                                                            End If

                                                        Else 'Do not apply OrderBy & do not take care about sample type order inside the same SAMPLE
                                                            AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                                     Where wse.StatFlag = Stat AndAlso _
                                                                                     wse.SampleClass = SClass AndAlso _
                                                                                     wse.ElementID = ID _
                                                                                     Select wse).ToList()

                                                            If AllTestTypeOrderTests.Count > 0 Then
                                                                'Look for the STD orderTests
                                                                OrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                              Where wse.StatFlag = Stat AndAlso _
                                                                              wse.SampleClass = SClass AndAlso _
                                                                              wse.ElementID = ID AndAlso _
                                                                              wse.ExecutionType = "PREP_STD" _
                                                                              Select wse).ToList()
                                                                stdOrderTestsCount = OrderTests.Count
                                                            Else
                                                                stdOrderTestsCount = 0
                                                            End If
                                                        End If


                                                        If stdOrderTestsCount > 0 Then
                                                            OrderContaminationNumber = 0

                                                            'Only move the contaminated tests, so no changes into first element tests and also when the
                                                            'last reagent on previous element do not contaminates the first reagent in current element
                                                            If previousElementLastReagentID <> -1 Then
                                                                Dim pendingOrderTestInNewElement = (From wse In OrderTests _
                                                                                                            Where wse.ExecutionStatus = "PENDING" _
                                                                                                            Select wse).ToList()
                                                                If pendingOrderTestInNewElement.Count > 0 Then
                                                                    'Search contamination between Elements
                                                                    Dim existContamination = (From wse In contaminationsDataDS.tparContaminations _
                                                                                                Where wse.ReagentContaminatorID = previousElementLastReagentID _
                                                                                                AndAlso wse.ReagentContaminatedID = pendingOrderTestInNewElement(0).ReagentID _
                                                                                                Select wse).ToList()

                                                                    If existContamination.Count > 0 Then
                                                                        'Calculate the contaminations inside the current Element + 1 (contamination between last and next elementID)
                                                                        OrderContaminationNumber = 1 + GetContaminationNumber(contaminationsDataDS, OrderTests, highContaminationPersitance)

                                                                    ElseIf highContaminationPersitance > 0 Then
                                                                        'If no LOW contamination exists between consecutive executions take care about the previous due the high contamination
                                                                        'has persistance > 1 (Evaluate only when last OrderTest has MaxReplicates < pHighContaminationPersistance)
                                                                        If previousElementLastMaxReplicates < highContaminationPersitance Then

                                                                            'Evaluate if the last reagents sent contaminates (HIGH contamination) the first pending to be sent
                                                                            For highIndex As Integer = PreviousReagentsIDList.Count - highContaminationPersitance To PreviousReagentsIDList.Count - 2
                                                                                Dim auxHighIndex = highIndex
                                                                                If auxHighIndex >= 0 Then 'Avoid overflow
                                                                                    existContamination = (From wse In contaminationsDataDS.tparContaminations _
                                                                                                      Where wse.ReagentContaminatorID = PreviousReagentsIDList(auxHighIndex) _
                                                                                                      AndAlso wse.ReagentContaminatedID = pendingOrderTestInNewElement(0).ReagentID _
                                                                                                      AndAlso Not wse.IsWashingSolutionR1Null _
                                                                                                      Select wse).ToList()
                                                                                    If existContamination.Count > 0 Then
                                                                                        Exit For
                                                                                    End If
                                                                                End If
                                                                            Next

                                                                            'If previous step has no contamination then evaluate if the LAST reagent sent contaminates (HIGH contamination) the second, third,... reagent pending to be sent
                                                                            If existContamination.Count = 0 Then
                                                                                Dim newPendingOrderTestMaxReplicates As Integer = 1
                                                                                newPendingOrderTestMaxReplicates = (From wse In pendingOrderTestInNewElement _
                                                                                                                    Select wse.ReplicateNumber).Max
                                                                                If newPendingOrderTestMaxReplicates < highContaminationPersitance Then
                                                                                    For i = 1 To highContaminationPersitance - 1
                                                                                        Dim aux_i = i
                                                                                        If aux_i <= pendingOrderTestInNewElement.Count - 1 Then 'Avoid overflow
                                                                                            existContamination = (From wse In contaminationsDataDS.tparContaminations _
                                                                                                              Where wse.ReagentContaminatorID = PreviousReagentsIDList(PreviousReagentsIDList.Count - 1) _
                                                                                                              AndAlso wse.ReagentContaminatedID = pendingOrderTestInNewElement(aux_i).ReagentID _
                                                                                                              AndAlso Not wse.IsWashingSolutionR1Null _
                                                                                                              Select wse).ToList()
                                                                                            If existContamination.Count > 0 Then
                                                                                                Exit For
                                                                                            End If
                                                                                        End If
                                                                                    Next
                                                                                End If
                                                                            End If

                                                                            If existContamination.Count > 0 Then
                                                                                'Calculate the contaminations inside the current Element + 1 (contamination between last and next elementID)
                                                                                OrderContaminationNumber = 1 + GetContaminationNumber(contaminationsDataDS, OrderTests, highContaminationPersitance)
                                                                            End If

                                                                        End If 'If previousElementLastMaxReplicates < highContaminationPersitance Then
                                                                    End If ' If existContamination.Count > 0 Then

                                                                End If 'If pendingOrderTestInNewElement.Count > 0 Then
                                                            End If 'If previousElementLastReagentID <> -1 Then

                                                            ManageContaminations(activeAnalyzer, dbConnection, returnDS, contaminationsDataDS, highContaminationPersitance, OrderTests, AllTestTypeOrderTests, OrderContaminationNumber, PreviousReagentsIDList, previousOrderTestMaxReplicatesList)

                                                            'AG 07/11/2011 - search the last reagentID of the current Element before change the ElementID
                                                            OrderTests = (From wse In returnDS.twksWSExecutions _
                                                                            Where wse.StatFlag = Stat AndAlso _
                                                                            wse.SampleClass = SClass AndAlso _
                                                                            wse.ElementID = ID AndAlso _
                                                                            wse.ExecutionStatus = "PENDING" _
                                                                            Select wse).ToList()

                                                            If OrderTests.Count > 0 Then
                                                                'AG 19/12/2011 - Inform the list of reagents and replicates using the executions of the last element group
                                                                'The last reagentID used has the higher indexes
                                                                Dim maxReplicates As Integer
                                                                For item = 0 To OrderTests.Count - 1
                                                                    Dim itemIndex = item
                                                                    maxReplicates = (From wse In returnDS.twksWSExecutions _
                                                                                                        Where wse.OrderTestID = OrderTests(itemIndex).OrderTestID _
                                                                                                        Select wse.ReplicateNumber).Max

                                                                    If PreviousReagentsIDList.Count = 0 Then
                                                                        PreviousReagentsIDList.Add(OrderTests(itemIndex).ReagentID)
                                                                        previousOrderTestMaxReplicatesList.Add(maxReplicates)

                                                                        'When reagent changes
                                                                    ElseIf PreviousReagentsIDList(PreviousReagentsIDList.Count - 1) <> OrderTests(itemIndex).ReagentID Then
                                                                        PreviousReagentsIDList.Add(OrderTests(itemIndex).ReagentID)
                                                                        previousOrderTestMaxReplicatesList.Add(maxReplicates)
                                                                    End If

                                                                    If itemIndex = OrderTests.Count - 1 Then
                                                                        previousElementLastReagentID = OrderTests(itemIndex).ReagentID
                                                                        previousElementLastMaxReplicates = maxReplicates
                                                                    End If
                                                                    'AG 19/12/2011
                                                                Next
                                                            Else
                                                                'Do nothing, the sentence previousElementLastReagentID = -1 is not allowed due
                                                                'WS could contain a Element LOCKED completely
                                                            End If
                                                            'AG 07/11/2011
                                                        Else
                                                            'AG 14/12/2011 - Different test types
                                                            For Each wse In AllTestTypeOrderTests
                                                                returnDS.twksWSExecutions.ImportRow(wse)
                                                            Next
                                                            'AG 14/12/2011
                                                        End If

                                                        If SClass <> "PATIENT" AndAlso SClass <> "CTRL" Then Exit For 'For blank, calib do not take care about the sample type inside the same SAMPLE

                                                        'AG 20/02/2014 - #1514
                                                        AllTestTypeOrderTests = Nothing
                                                        OrderTests = Nothing
                                                        'AG 20/02/2014 - #1514

                                                    End If
                                                Next 'For Each mySampleType

                                            Next 'For each elementID


                                        End If
                                    Next 'For each SampleClass

                                End If
                            Next 'For each StatFlag

                            resultData.SetDatos = returnDS
                            'AG 20/02/2014 - #1514
                            differentStatValues = Nothing
                            differentSampleClassValues = Nothing
                            differentSampleTypeValues = Nothing
                            PreviousReagentsIDList = Nothing
                            previousOrderTestMaxReplicatesList = Nothing
                            'AG 20/02/2014 - #1514
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.SortWSExecutionsByElementGroupContaminationNew", EventLogEntryType.Error, False)

            End Try
            'AG 19/02/2014 - #1514
            bestResult = Nothing
            currentResult = Nothing
            'AG 19/02/2014 - #1514

            Return resultData
        End Function

#End Region

#Region "METHODS REPLACED FOR NEW ONES DUE TO PERFORMANCE ISSUES"
        '        ''' <summary>
        '        ''' Find the CLOSED Executions having the maximum ReplicateNumber for the same OrderTestID and RerunNumber. Executions of all MultiItemNumbers
        '        ''' are returned excepting when parameter pOnlyMaxReplicate is TRUE, in which case, only the maximum MultiItemNumber for the Replicate is returned 
        '        ''' of the informed ExecutionID
        '        ''' </summary>
        '        ''' <param name="pDBConnection">Open DB Connection</param>
        '        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        '        ''' <param name="pExecutionID">Execution Identifier</param>
        '        ''' <param name="pOnlyMaxReplicate">Optional parameter. When TRUE, only the maximum MultiItemNumber for the Replicate is returned</param>
        '        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of all obtained Executions</returns>
        '        ''' <remarks>
        '        ''' Created by:  AG 23/07/2010
        '        ''' </remarks>
        '        Public Function GetClosedExecutionsRelated(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                                   ByVal pExecutionID As Integer, Optional ByVal pOnlyMaxReplicate As Boolean = False) As GlobalDataTO
        '            Dim resultData As GlobalDataTO = Nothing
        '            Dim dbConnection As SqlClient.SqlConnection = Nothing

        '            Try
        '                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '                    If (Not dbConnection Is Nothing) Then
        '                        Dim myDAO As New twksWSExecutionsDAO
        '                        resultData = myDAO.GetClosedExecutionsRelated(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, pOnlyMaxReplicate)

        '                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                            Dim myEx_DS As New ExecutionsDS
        '                            myEx_DS = CType(resultData.SetDatos, ExecutionsDS)
        '                            resultData.SetDatos = myEx_DS
        '                        End If
        '                    End If
        '                End If
        '            Catch ex As Exception
        '                resultData = New GlobalDataTO()
        '                resultData.HasError = True
        '                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '                'Dim myLogAcciones As New ApplicationLogManager()
        '                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetClosedExecutionsRelated", EventLogEntryType.Error, False)
        '            Finally
        '                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '            End Try
        '            Return resultData
        '        End Function

        '        ''' <summary>
        '        ''' Get number of multiitem 
        '        ''' </summary>
        '        ''' <param name="pDBConnection">Open Database Connection</param>
        '        ''' <param name="pExecutionID">execution identifier></param>
        '        ''' <returns></returns>
        '        ''' <remarks></remarks>
        '        Public Function GetNumberOfMultititem(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                              ByVal pExecutionID As Integer) As GlobalDataTO

        '            Dim resultData As New GlobalDataTO
        '            Dim dbConnection As New SqlClient.SqlConnection

        '            Try
        '                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

        '                If (Not resultData.HasError) Then
        '                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '                    If (Not dbConnection Is Nothing) Then
        '                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO

        '                        resultData = mytwksWSExecutionsDAO.GetNumberOfMultititem(dbConnection, pExecutionID)
        '                    End If
        '                End If

        '            Catch ex As Exception
        '                resultData.HasError = True
        '                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '                'Dim myLogAcciones As New ApplicationLogManager()
        '                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetNumberOfMultititem", EventLogEntryType.Error, False)

        '            Finally
        '                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '            End Try

        '            Return resultData

        '        End Function

        '        ' ''' <summary>
        '        ' ''' Search the affected CLOSED executions for the new blank or calibrator result
        '        ' ''' </summary>
        '        ' ''' <param name="pDBConnection"></param>
        '        ' ''' <param name="pAnalyzerID"></param>
        '        ' ''' <param name="pWorkSessionID"></param>
        '        ' ''' <param name="pSampleClass"></param>
        '        ' ''' <param name="pTestID"></param>
        '        ' ''' <param name="pSampleType"></param>
        '        ' ''' <returns>GlobalDataTo (ExecutionsDS)</returns>
        '        ' ''' <remarks>Created by AG 23/07/2010</remarks>
        '        'Public Function ReadAffectedExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '        '                               ByVal pSampleClass As String, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '        '    Dim resultData As New GlobalDataTO
        '        '    Dim dbConnection As New SqlClient.SqlConnection

        '        '    Try
        '        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)

        '        '        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
        '        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '        '            If (Not dbConnection Is Nothing) Then
        '        '                Dim myDAO As New twksWSExecutionsDAO

        '        '                Select Case pSampleClass
        '        '                    Case "BLANK"
        '        '                        resultData = myDAO.GetExecutionsAffectedByNewBlank(dbConnection, pAnalyzerID, pWorkSessionID, pSampleClass, pTestID)

        '        '                    Case "CALIB"
        '        '                        resultData = myDAO.GetExecutionsAffectedByNewCalib(dbConnection, pAnalyzerID, pWorkSessionID, pSampleClass, pTestID, pSampleType)

        '        '                    Case Else
        '        '                End Select
        '        '            End If
        '        '        End If

        '        '    Catch ex As Exception
        '        '        resultData.HasError = True
        '        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ReadAffectedExecutions", EventLogEntryType.Error, False)
        '        '    Finally
        '        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '        '    End Try

        '        '    Return resultData
        '        'End Function

        '        ''' <summary>
        '        ''' Recalculate status for all not deleted existing executions with status PENDING or LOCKED
        '        ''' </summary>
        '        ''' <param name="pDBConnection">Open DB Connection</param>
        '        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        '        ''' <param name="pWorkInRunningMode">Flag indicating if the function is executed when a WorkSession is running in the Analyzer
        '        '''                                  It is not necessary just now, but it is defined for future use if it is finally needed</param>
        '        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        '        ''' <remarks>
        '        ''' Created by:  SA 25/08/2010
        '        ''' Modified by: SA 15/02/2011 - Sent all updates in the DataSet to the UpdateStatus function instead of sent them
        '        '''                              one by one (for speed improvement)
        '        '''              AG 19/09/2011 - Function renamed from RecalculateStatusForRerunExecutions to RecalculateStatusForNotDeletedExecutions, 
        '        '''                              and added parameter pWorkInRunningMode to adapt the function for use working in running mode or not
        '        '''              AG 22/12/2011 - Verification of locked Blanks and Calibrators have to be done only for Standard Tests
        '        '''              AG 07/02/2012 - Original algorithm failed when we try to unlock a BLANKS, CALIBS (require BLANK), CTRLS or PATIENTS (require BLANK and CALIB)
        '        '''                              ** The 1st time BLANKS were unlocked (OK), but not CALIBS, CTRLS or PATIENTS because the status of the BLANK was queried in 
        '        '''                                 the Database before it has been updated 
        '        '''                              ** The 2nd time, CALIBS were unlocked (OK) but not CTRLS or PATIENTS for a similar reason, the status of the CALIB was queried
        '        '''                                 in the Database before it has been updated
        '        '''                              ** The 3rd time, CTRLS and PATIENTS were unlocked (OK)
        '        '''                              Solution: the new status is calculate in the same way as now, but the call to twksWSExecutionsDAO.Update has to be executed  
        '        '''                                        for each different SampleClass and not at the end of the loop
        '        '''              SA 09/03/2012 - Executions depending of required Elements marked as INCOMPLETE (there are volume in the Rotor, but the total quantity is not
        '        '''                              enough for all the pending work); changed the function template  
        '        '''              AG 20/04/2012 - Call method verifyBlankLocked (executionsDelegate) instead calling the ExecutionsDAO.VerifyUnlockedExecution
        '        '''                              because if a previous result was selected to be used the Blank it could not be positioned)
        '        ''' </remarks>
        '        Public Function RecalculateStatusForNotDeletedExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                                                 ByVal pWorkSessionID As String, ByVal pWorkInRunningMode As Boolean) As GlobalDataTO
        '            Dim resultData As GlobalDataTO = Nothing
        '            Dim dbConnection As SqlClient.SqlConnection = Nothing

        '            Try
        '                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '                    If (Not dbConnection Is Nothing) Then
        '                        Dim newExecStatus As String = ""
        '                        Dim rerunExecutions As New twksWSExecutionsDAO

        '                        'Get all Pending and Locked Executions created for Reruns in the Analyzer WorkSession
        '                        resultData = rerunExecutions.GetNotDeletedPendingExecutionDuringWSCreation(dbConnection, pAnalyzerID, pWorkSessionID, pWorkInRunningMode)
        '                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                            Dim myExecutionsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

        '                            'Variables to control the loop and DataSet used to call Update function for all pending and locked Executions of each different SampleClass
        '                            Dim currentOT As Integer = -1
        '                            Dim currentSampleClass As String = String.Empty
        '                            Dim updateBySampleClassExecutionsDS As New ExecutionsDS

        '                            For Each rerunExec As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions
        '                                'AG 07/02/2012 - When the SampleClass changes, the status of Executions of the previous one is updated
        '                                If (currentSampleClass <> String.Empty AndAlso currentSampleClass <> rerunExec.SampleClass) Then
        '                                    'Update Status of all Executions of the previous SampleClass 
        '                                    If (updateBySampleClassExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
        '                                        resultData = UpdateStatus(dbConnection, updateBySampleClassExecutionsDS)
        '                                    End If

        '                                    updateBySampleClassExecutionsDS.Clear()
        '                                    currentSampleClass = rerunExec.SampleClass
        '                                End If
        '                                'AG 07/02/2012

        '                                If (rerunExec.OrderTestID <> currentOT) Then
        '                                    currentOT = rerunExec.OrderTestID
        '                                    currentSampleClass = rerunExec.SampleClass

        '                                    Dim myWSOrderTests As New WSOrderTestsDelegate
        '                                    resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, rerunExec.SampleClass, currentOT)

        '                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                        Dim reqElementsDS As WSOrderTestsForExecutionsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

        '                                        newExecStatus = "PENDING"
        '                                        For Each reqElement As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In reqElementsDS.WSOrderTestsForExecutions
        '                                            If (reqElement.ElementStatus = "NOPOS") Then
        '                                                'If a required Element is NO POSITIONED, then the Execution is LOCKED
        '                                                newExecStatus = "LOCKED"
        '                                            Else
        '                                                'If a required Element is POSITIONED or INCOMPLETE and the Execution is for an Standard Test, then it is verified
        '                                                'if all elements for the needed Blank and Calibrator are also POSITIONED
        '                                                If (rerunExec.ExecutionType = "PREP_STD") Then
        '                                                    If (rerunExec.SampleClass = "CALIB") Then
        '                                                        'The Calibrator is positioned, verify if the elements needed for the Blank are also positioned

        '                                                        'AG 20/04/2012
        '                                                        'resultData = rerunExecutions.VerifyUnlockedExecution(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", _
        '                                                        '                                                     reqElement.TestID, reqElement.SampleType)
        '                                                        'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                                        '    If (Not DirectCast(resultData.SetDatos, Boolean)) Then newExecStatus = "LOCKED"
        '                                                        'Else
        '                                                        '    'Error verifying if the required Blank Elements are locked
        '                                                        '    Exit For
        '                                                        'End If
        '                                                        resultData = VerifyLockedBlank(dbConnection, pWorkSessionID, pAnalyzerID, reqElement.TestID, reqElement.SampleType)
        '                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                                            If (DirectCast(resultData.SetDatos, Boolean)) Then newExecStatus = "LOCKED"
        '                                                        Else
        '                                                            'Error verifying if the required Blank Elements are locked
        '                                                            Exit For
        '                                                        End If
        '                                                        'AG 20/04/2012

        '                                                    ElseIf (rerunExec.SampleClass = "CTRL" OrElse rerunExec.SampleClass = "PATIENT") Then
        '                                                        'The Control or Patient is positioned, verify if the required Calibrator is positioned
        '                                                        resultData = VerifyLockedCalibrator(dbConnection, pWorkSessionID, pAnalyzerID, reqElement.TestID, reqElement.SampleType)
        '                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                                            If (DirectCast(resultData.SetDatos, Boolean)) Then
        '                                                                newExecStatus = "LOCKED"
        '                                                            Else
        '                                                                'The Calibrator is positioned, verify if the elements needed for the Blank are also positioned

        '                                                                'AG 20/04/2012
        '                                                                'resultData = rerunExecutions.VerifyUnlockedExecution(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", _
        '                                                                '                                                     reqElement.TestID, reqElement.SampleType)
        '                                                                'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                                                '    If (Not DirectCast(resultData.SetDatos, Boolean)) Then newExecStatus = "LOCKED"
        '                                                                'Else
        '                                                                '    'Error verifying if the required Blank Elements are locked
        '                                                                '    Exit For
        '                                                                'End If

        '                                                                resultData = VerifyLockedBlank(dbConnection, pWorkSessionID, pAnalyzerID, reqElement.TestID, reqElement.SampleType)
        '                                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                                                    If (DirectCast(resultData.SetDatos, Boolean)) Then newExecStatus = "LOCKED"
        '                                                                Else
        '                                                                    'Error verifying if the required Blank Elements are locked
        '                                                                    Exit For
        '                                                                End If
        '                                                                'AG 20/04/2012
        '                                                            End If
        '                                                        Else
        '                                                            'Error verifying if the required Calibrator Elements are locked
        '                                                            Exit For
        '                                                        End If
        '                                                    End If
        '                                                End If
        '                                            End If

        '                                            If (newExecStatus = "LOCKED") Then Exit For
        '                                        Next

        '                                        rerunExec.BeginEdit()
        '                                        rerunExec.ExecutionStatus = newExecStatus
        '                                        rerunExec.EndEdit()
        '                                    End If
        '                                Else
        '                                    rerunExec.BeginEdit()
        '                                    rerunExec.ExecutionStatus = newExecStatus
        '                                    rerunExec.EndEdit()
        '                                End If

        '                                'AG 07/02/2012 - Add row into executionsDS grouped by sampleClass to update 
        '                                updateBySampleClassExecutionsDS.twksWSExecutions.ImportRow(rerunExec)
        '                                updateBySampleClassExecutionsDS.AcceptChanges()
        '                                'AG 07/02/2012
        '                            Next

        '                            'AG 07/02/2012 - now we update executions status grouped by sample class not at the loop ending
        '                            'when the loop is endind we has to update only the updateBySampleClassExecutionsDS contents
        '                            'Update the Status of the Reruns Executions
        '                            'If (myExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
        '                            '    resultData = UpdateStatus(dbConnection, myExecutionsDS)
        '                            'End If
        '                            If (updateBySampleClassExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
        '                                resultData = UpdateStatus(dbConnection, updateBySampleClassExecutionsDS)
        '                            End If
        '                            'AG 07/02/2012
        '                        End If

        '                        If (Not resultData.HasError) Then
        '                            'When the Database Connection was opened locally, then the Commit is executed
        '                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                        Else
        '                            'When the Database Connection was opened locally, then the Rollback is executed
        '                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                        End If
        '                    End If
        '                End If

        '            Catch ex As Exception
        '                'When the Database Connection was opened locally, then the Rollback is executed
        '                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '                resultData = New GlobalDataTO()
        '                resultData.HasError = True
        '                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '                'Dim myLogAcciones As New ApplicationLogManager()
        '                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.RecalculateStatusForNotDeletedExecutions", EventLogEntryType.Error, False)
        '            Finally
        '                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '            End Try
        '            Return resultData
        '        End Function

        '        ''' <summary>
        '        ''' Update inuse flag
        '        ''' </summary>
        '        ''' <param name="pDBConnection"></param>
        '        ''' <param name="pAnalyzerID"></param>
        '        ''' <param name="pWorkSessionID "></param>
        '        ''' <param name="pExecutionID"></param>
        '        ''' <param name="pInUse"></param>
        '        ''' <returns></returns>
        '        ''' <remarks>Created by AG 23/07/2010</remarks>
        '        Public Function UpdateInUse(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                             ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                             ByVal pExecutionID As Integer, ByVal pInUse As Boolean) As GlobalDataTO
        '            Dim resultData As New GlobalDataTO
        '            Dim dbConnection As New SqlClient.SqlConnection

        '            Try
        '                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '                If (Not resultData.HasError) Then
        '                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '                    If (Not dbConnection Is Nothing) Then

        '                        'Create DS
        '                        Dim myExecutionsDS As New ExecutionsDS
        '                        Dim myEx_Row As ExecutionsDS.twksWSExecutionsRow
        '                        myEx_Row = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow

        '                        myEx_Row.BeginEdit()
        '                        With myEx_Row
        '                            .AnalyzerID = pAnalyzerID
        '                            .WorkSessionID = pWorkSessionID
        '                            .ExecutionID = pExecutionID
        '                            .InUse = pInUse
        '                        End With
        '                        myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(myEx_Row)
        '                        myExecutionsDS.AcceptChanges()

        '                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
        '                        resultData = mytwksWSExecutionsDAO.UpdateInUse(dbConnection, myExecutionsDS)

        '                        If (Not resultData.HasError) Then
        '                            'When the Database Connection was opened locally, then the Commit is executed
        '                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                        Else
        '                            'When the Database Connection was opened locally, then the Rollback is executed
        '                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                        End If
        '                    End If
        '                End If

        '            Catch ex As Exception
        '                'When the Database Connection was opened locally, then the Rollback is executed
        '                If (pDBConnection Is Nothing) AndAlso Not (dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '                resultData.HasError = True
        '                resultData.ErrorCode = "SYSTEM_ERROR"
        '                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '                'Dim myLogAcciones As New ApplicationLogManager()
        '                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateInUse", EventLogEntryType.Error, False)
        '            Finally
        '                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '            End Try
        '            Return resultData
        '        End Function

        '        ''' <summary>
        '        ''' Update the fields BaseLineID, WellUsed and RotorTurnNumber
        '        ''' </summary>
        '        ''' <param name="pDBConnection">Open DB Connection</param>
        '        ''' <param name="pExecutionsDS">Executions DataSet with the following fields informed (ExecutionID, BaseLineID, WellUsed, RotorTurnNumber)</param>
        '        ''' <returns>GlobalDataTO containing success/error information</returns>
        '        ''' <remarks>
        '        ''' Created by: GDS - 27/04/2010
        '        ''' </remarks>
        '        Public Function UpdateReadingsFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
        '            Dim resultData As New GlobalDataTO
        '            Dim dbConnection As New SqlClient.SqlConnection

        '            Try
        '                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
        '                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '                    If (Not dbConnection Is Nothing) Then
        '                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
        '                        resultData = mytwksWSExecutionsDAO.UpdateReadingsFields(dbConnection, pExecutionsDS)

        '                        If (Not resultData.HasError) Then
        '                            'When the Database Connection was opened locally, then the Commit is executed
        '                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                        Else
        '                            'When the Database Connection was opened locally, then the Rollback is executed
        '                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                        End If
        '                    End If
        '                End If

        '            Catch ex As Exception
        '                'When the Database Connection was opened locally, then the Rollback is executed
        '                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '                resultData.HasError = True
        '                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '                'Dim myLogAcciones As New ApplicationLogManager()
        '                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateReadingsFields", EventLogEntryType.Error, False)
        '            Finally
        '                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '            End Try
        '            Return resultData
        '        End Function

        ' ''' <summary>
        ' '''1) Update Execution status as CLOSEDNOK 
        ' ''' 2) If Execution.ReplicateNumber = OrderTest.MaxReplicates then
        ' ''' 2.1)____ OrderTestDelegate.UpdateStatusByOrderTestID(OrderTest, CLOSED)
        ' ''' </summary>
        ' ''' <param name="pDBConnection"></param>
        ' ''' <param name="pAnalyzerID"></param>
        ' ''' <param name="pWorkSessionID"></param>
        ' ''' <param name="pExecutionID"></param>
        ' ''' <param name="pOrderTestID" ></param>
        ' ''' <param name="pOrderTestMaxReplicates" ></param>
        ' ''' <param name="pSTDPrepFlag"></param>
        ' ''' <returns>GlobalDataTo with error or not</returns>
        ' ''' <remarks>AG 24/02/2011 - Tested ok
        ' ''' AG 21/10/2011 - prepare business for shown closednok executions into results screen
        ' ''' AG 19/03/2012 - add parameter pSTDPrepFlag (when TRUE (STD) alarms are added, when FALSE (ISE) no alarms are added)
        ' ''' AG 25/06/2012 - ResultsDS informs also AnalyzerID and WorkSessionID
        ' ''' </remarks>
        'Public Function UpdateStatusClosedNOK(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                      ByVal pWorkSessionID As String, ByVal pExecutionID As Integer, _
        '                                      ByVal pOrderTestID As Integer, ByVal pOrderTestMaxReplicates As Integer, _
        '                                      ByVal pSTDPrepFlag As Boolean) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                'Get all execution, with several MultiItemNumber, using the same OrderTestID - RerunNumber
        '                resultData = GetExecutionMultititem(dbConnection, pExecutionID)
        '                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                    Dim myExecutionsDS As New ExecutionsDS
        '                    myExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

        '                    If myExecutionsDS.twksWSExecutions.Rows.Count > 0 Then
        '                        'Update EXECUTIONSTATUS to CLOSEDNOK + Add execution alarm (finished with optical errors)
        '                        Dim execAlarmDlg As New WSExecutionAlarmsDelegate
        '                        Dim execAlarmDS As New WSExecutionAlarmsDS
        '                        Dim execAlarmRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow

        '                        For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
        '                            row.BeginEdit()
        '                            row.ExecutionStatus = "CLOSEDNOK"
        '                            row.InUse = False
        '                            row.ResultDate = DateTime.Now
        '                            row.EndEdit()

        '                            If pSTDPrepFlag Then 'STD executions add remark Finished with optical errors
        '                                execAlarmRow = execAlarmDS.twksWSExecutionAlarms.NewtwksWSExecutionAlarmsRow
        '                                execAlarmRow.ExecutionID = row.ExecutionID
        '                                execAlarmRow.AlarmID = GlobalEnumerates.Alarms.ABS_REMARK13.ToString
        '                                execAlarmRow.AlarmDateTime = DateTime.Now
        '                                execAlarmDS.twksWSExecutionAlarms.AddtwksWSExecutionAlarmsRow(execAlarmRow)
        '                            Else 'ISE executions only mark as CLOSEDNOK but no alarm

        '                            End If

        '                        Next
        '                        myExecutionsDS.AcceptChanges()
        '                        execAlarmDS.twksWSExecutionAlarms.AcceptChanges()
        '                        resultData = UpdateStatus(dbConnection, myExecutionsDS)

        '                        'AG 21/10/2011
        '                        If Not resultData.HasError Then
        '                            If execAlarmDS.twksWSExecutionAlarms.Rows.Count > 0 Then
        '                                resultData = execAlarmDlg.Add(dbConnection, execAlarmDS)
        '                            End If
        '                        End If

        '                        'Exists results for OrderTest-MultiPointNumber-RerunNumber: Yes: Do nothing, No: Create empty result
        '                        Dim createAvgResult As Boolean = False
        '                        If Not resultData.HasError Then
        '                            Dim resultsDlg As New ResultsDelegate
        '                            Dim myResults As New ResultsDS
        '                            resultData = resultsDlg.GetAcceptedResults(dbConnection, pOrderTestID, True)
        '                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                                myResults = CType(resultData.SetDatos, ResultsDS)
        '                            End If

        '                            If myResults.twksResults.Rows.Count = 0 Then
        '                                createAvgResult = True

        '                                Dim newResultsRow As ResultsDS.twksResultsRow
        '                                For Each executionsRow As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
        '                                    newResultsRow = myResults.twksResults.NewtwksResultsRow
        '                                    newResultsRow.OrderTestID = pOrderTestID
        '                                    newResultsRow.MultiPointNumber = executionsRow.MultiItemNumber
        '                                    newResultsRow.RerunNumber = executionsRow.RerunNumber
        '                                    newResultsRow.AcceptedResultFlag = False
        '                                    newResultsRow.ManualResultFlag = False
        '                                    newResultsRow.ValidationStatus = "CLOSEDNOK"
        '                                    newResultsRow.ResultDateTime = DateTime.Now
        '                                    newResultsRow.AnalyzerID = pAnalyzerID
        '                                    newResultsRow.WorkSessionID = pWorkSessionID
        '                                    myResults.twksResults.AddtwksResultsRow(newResultsRow)
        '                                Next
        '                                myResults.twksResults.AcceptChanges()
        '                                resultData = resultsDlg.SaveResults(dbConnection, myResults)
        '                            End If

        '                            If Not resultData.HasError Then
        '                                'When the average result is created: Add average alarm (finished with optical errors)
        '                                If createAvgResult Then
        '                                    Dim resultAlarmDlg As New ResultAlarmsDelegate
        '                                    Dim resultAlarmDS As New ResultAlarmsDS
        '                                    Dim resultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow

        '                                    If pSTDPrepFlag Then 'STD executions add remark Finished with optical errors
        '                                        For Each executionsRow As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
        '                                            resultAlarmRow = resultAlarmDS.twksResultAlarms.NewtwksResultAlarmsRow
        '                                            resultAlarmRow.OrderTestID = pOrderTestID
        '                                            resultAlarmRow.MultiPointNumber = executionsRow.MultiItemNumber
        '                                            resultAlarmRow.RerunNumber = executionsRow.RerunNumber
        '                                            resultAlarmRow.AlarmID = GlobalEnumerates.Alarms.ABS_REMARK13.ToString
        '                                            resultAlarmRow.AlarmDateTime = DateTime.Now
        '                                            resultAlarmDS.twksResultAlarms.AddtwksResultAlarmsRow(resultAlarmRow)
        '                                        Next

        '                                    Else 'ISE executions only mark as CLOSEDNOK but no alarm

        '                                    End If
        '                                    resultAlarmDS.twksResultAlarms.AcceptChanges()
        '                                    If resultAlarmDS.twksResultAlarms.Rows.Count > 0 Then
        '                                        resultData = resultAlarmDlg.Add(dbConnection, resultAlarmDS)
        '                                    End If
        '                                End If
        '                            End If

        '                        End If
        '                        'AG 21/10/2011

        '                        If Not resultData.HasError Then
        '                            'Finally update OrderTest and Order status
        '                            If myExecutionsDS.twksWSExecutions(0).ReplicateNumber = pOrderTestMaxReplicates Then
        '                                Dim ot_delegate As New OrderTestsDelegate
        '                                resultData = ot_delegate.UpdateStatusByOrderTestID(dbConnection, pOrderTestID, "CLOSED")
        '                            End If
        '                        End If
        '                    End If

        '                End If


        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateStatusClosedNOK", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region

#Region "NEW METHODS - FOR PERFORMANCE IMPROVEMENTS"
        ''' <summary>
        ''' Find all the CLOSED Executions for the same OrderTestID and RerunNumber. Executions of all MultiItemNumbers are returned excepting 
        ''' when parameter pOnlyMaxReplicate is TRUE, in which case, only the Execution for the maximum MultiItemNumber and ReplicateNumber is returned 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pOnlyMaxReplicate">Optional parameter. When TRUE, only the Execution for the maximum MultiItemNumber and ReplicateNumbet is returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS (subtable vwksWSExecutionsResults) with data of all obtained Executions</returns>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Based in GetClosedExecutionsRelated but changing the entry parameter ExecutionID by OrderTestID and RerunNumber
        '''                              and calling the new DAO function
        ''' </remarks>
        Public Function GetClosedExecutionsRelatedNEW(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                      ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                                      Optional ByVal pOnlyMaxReplicate As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetClosedExecutionsRelatedNEW(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pRerunNumber, pOnlyMaxReplicate)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetClosedExecutionsRelatedNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the group of Executions for the informed OrderTestID, RerunNumber and ReplicateNumber. Only one Execution will be returned
        ''' excepting when the informed OrderTestID corresponds to a multipoint Calibrator 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pReplicateNumber">Replicate Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with all the Executions for the informed OrderTestID, RerunNumber 
        '''          and ReplicateNumber</returns>
        ''' <remarks>
        ''' Created by: SA 03/07/2012 - Same as GetExecutionMultititem but call function GetExecutionMultiItemsNEW in the DAO class instead
        '''                             of function GetExecutionMultiItem 
        ''' </remarks>
        Public Function GetExecutionMultiItemsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                                  ByVal pReplicateNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.GetExecutionMultiItemsNEW(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pRerunNumber, pReplicateNumber)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSExecutionsDelegate.GetExecutionMultiItemsNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified OrderTestID/RerunNumber, get the maximum MultiItemNumber between all its Executions
        ''' (using of this function is only for OrderTestID corresponding to a CALIBRATOR Order)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''  <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing an integer value with the maximum MultiItemNumber</returns>
        ''' <remarks>
        ''' Created  by: SA 11/07/2012
        ''' </remarks>
        Public Function GetNumberOfMultitItemNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                 ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetNumberOfMultitItemNEW(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pRerunNumber)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDele.GetNumberOfMultiItemNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of CALIB Executions affected for a new BLANK Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of affected Executions</returns>
        ''' <remarks>
        ''' Created by:  SA 10/07/2012
        ''' </remarks>
        Public Function ReadAffectedCalibExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                    ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        Dim myFinalExecutionsDS As New ExecutionsDS
                        Dim myPartialExecutionsDS As New ExecutionsDS

                        'Get the list of different OrderTestID/MAX(MultiItemNumber)/MAX(ReplicateNumber) with Calibrator Executions for the specified Test
                        resultData = myDAO.ReadAffectedOTCalibrators(dbConnection, pAnalyzerID, pWorkSessionID, pTestID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim calibExecDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            Dim myNumOfCalibratorOTs As Integer = calibExecDS.vwksWSExecutionsResults.Rows.Count
                            For Each calibExec As ExecutionsDS.vwksWSExecutionsResultsRow In calibExecDS.vwksWSExecutionsResults
                                'Get the list of affected Calibrator Executions for the OrderTestID/MAX(MultiItemNumber)/MAX(ReplicateNumber)
                                resultData = myDAO.ReadAffectedCalibExecutions(dbConnection, pAnalyzerID, pWorkSessionID, calibExec.OrderTestID, _
                                                                               calibExec.MultiItemNumber, calibExec.ReplicateNumber)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    If (myNumOfCalibratorOTs > 1) Then
                                        myPartialExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                        'Move all affected Calibrator Executions to the final DataSet
                                        For Each row As ExecutionsDS.vwksWSExecutionsResultsRow In myPartialExecutionsDS.vwksWSExecutionsResults
                                            myFinalExecutionsDS.vwksWSExecutionsResults.ImportRow(row)
                                        Next
                                    Else
                                        myFinalExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                    End If
                                Else
                                    'Error getting the list of affected Calibrator Executions
                                    Exit For
                                End If
                            Next
                        End If

                        resultData.SetDatos = myFinalExecutionsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ReadAffectedExecutionsNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of CTRL and PATIENT Executions affected for a new BLANK Result, or the list of affected CTRL and PATIENT
        ''' Executions affected for a new CALIB Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">Sample Type Code. Informed only when the new Result if for a Calibrator</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of affected Executions</returns>
        ''' <remarks>
        ''' Created by:  SA 13/07/2012
        ''' </remarks>
        Public Function ReadAffectedCtrlPatientExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                          ByVal pTestID As Integer, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetExecutionsAffectedByNewBlankOrCalib(dbConnection, pAnalyzerID, pWorkSessionID, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ReadAffectedCtrlPatientExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Recalculate status for all not deleted existing executions with status PENDING or LOCKED (standBy or pause mode)
        ''' In running normal mode: Do not evaluate the possible status change PENDING to LOCKED because in running remove rotor position is not allowed
        ''' Summary
        ''' PENDING to LOCKED: Only when pWorkInRunningMode = FALSE
        ''' LOCKED to PENDING: Always
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWorkInRunningMode">Flag indicating if the function is executed when a WorkSession is running in the Analyzer
        '''                                  It is not necessary just now, but it is defined for future use if it is finally needed</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/07/2012 - Based in RecalculateStatusForNotDeletedExecutions; changes to improve the function perfomance
        ''' Modified by AG 25/03/2013 - When the ordertest has been locked by lis assign LOCKED value instead of PENDING, otherwise although 
        '''                             the final result is OK the sort by contaminations can be affected
        ''' AG 30/05/2014 - #1644 add parameter pPauseMode
        ''' </remarks>
        Public Function RecalculateStatusForNotDeletedExecutionsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                                    ByVal pWorkSessionID As String, ByVal pWorkInRunningMode As Boolean, ByVal pPauseMode As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myExecutionsDAO As New twksWSExecutionsDAO

                        'Get all Pending and Locked Executions in the Analyzer WorkSession
                        resultData = myExecutionsDAO.GetPendingAndLockedGroupedExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pWorkInRunningMode, pPauseMode)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myExecutionsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            Dim newExecStatus As String
                            Dim noPOSElements As Integer
                            Dim myWSOrderTests As New WSOrderTestsDelegate
                            Dim reqElementsDS As New WSOrderTestsForExecutionsDS
                            Dim lstSampleClassExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)
                            Dim lstToPENDING As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
                            Dim lstToLOCKED As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514

                            If (myExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                'Get all BLANK Order Tests having Pending and/or Locked Executions
                                lstSampleClassExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions _
                                                           Where a.SampleClass = "BLANK" _
                                                          Select a).ToList()

                                If (lstSampleClassExecutions.Count > 0) Then
                                    For Each blankOT As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions
                                        'Get the list of Elements required for the Blank Order Test 
                                        resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, blankOT.SampleClass, blankOT.OrderTestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            reqElementsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                                            'Verify if at least one of the required Elements is not positioned
                                            noPOSElements = (From b As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In reqElementsDS.WSOrderTestsForExecutions _
                                                            Where b.ElementStatus = "NOPOS" _
                                                           Select b).ToList.Count()

                                            'The Executions for the Blank Order Test will be marked as LOCKED if there are not positioned elements
                                            blankOT.ExecutionStatus = IIf(noPOSElements > 0, "LOCKED", "PENDING").ToString
                                        Else
                                            Exit For
                                        End If
                                    Next

                                    'Finally, update the status of the Executions for each Blank OrderTest
                                    'AG 19/02/2014 - #1514
                                    'If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstSampleClassExecutions)
                                    lstToPENDING = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList
                                    lstToLOCKED = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "LOCKED" Select a).ToList
                                    If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstToPENDING, lstToLOCKED)
                                    'AG 19/02/2014 - #1514

                                End If

                                If (Not resultData.HasError) Then
                                    'Get all CALIBRATOR Order Tests having Pending and/or Locked Executions
                                    lstSampleClassExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions _
                                                               Where a.SampleClass = "CALIB" _
                                                              Select a).ToList()

                                    If (lstSampleClassExecutions.Count > 0) Then
                                        For Each calibOT As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions
                                            'Get the list of Elements required for the Calibrator Order Test 
                                            resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, calibOT.SampleClass, calibOT.OrderTestID)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                reqElementsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                                                'Verify if at least one of the required Elements is not positioned
                                                noPOSElements = (From b As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In reqElementsDS.WSOrderTestsForExecutions _
                                                                Where b.ElementStatus = "NOPOS" _
                                                               Select b).ToList.Count()

                                                newExecStatus = "PENDING"
                                                If (noPOSElements > 0) Then
                                                    'There are required Elements not positioned; the Executions of the Order Test will be marked as locked
                                                    newExecStatus = "LOCKED"
                                                Else
                                                    'Elements required for the Calibrator are positioned, verify if the elements needed for the Blank are also positioned
                                                    resultData = VerifyLockedBlank(dbConnection, pWorkSessionID, pAnalyzerID, reqElementsDS.WSOrderTestsForExecutions.First.TestID, _
                                                                                   reqElementsDS.WSOrderTestsForExecutions.First.SampleType)
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        If (DirectCast(resultData.SetDatos, Boolean)) Then newExecStatus = "LOCKED"
                                                    Else
                                                        Exit For
                                                    End If
                                                End If
                                                calibOT.ExecutionStatus = newExecStatus
                                            Else
                                                Exit For
                                            End If
                                        Next

                                        'Finally, update the status of the Executions for each Calibrator OrderTest
                                        'AG 19/02/2014 - #1514
                                        'If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstSampleClassExecutions)
                                        lstToPENDING = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList
                                        lstToLOCKED = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "LOCKED" Select a).ToList
                                        If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstToPENDING, lstToLOCKED)
                                        'AG 19/02/2014 - #1514
                                    End If
                                End If

                                If (Not resultData.HasError) Then
                                    'Get all CONTROL and PATIENT Order Tests having Pending and/or Locked Executions for ISE Tests 
                                    lstSampleClassExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions _
                                                               Where (a.SampleClass = "CTRL" OrElse a.SampleClass = "PATIENT") _
                                                             AndAlso a.ExecutionType = "PREP_ISE" _
                                                              Select a).ToList()

                                    If (lstSampleClassExecutions.Count > 0) Then
                                        For Each sampleIseOT As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions
                                            'Get the list of Elements required for the Control or Patient ISE Order Test 
                                            resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, sampleIseOT.SampleClass, sampleIseOT.OrderTestID)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                reqElementsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                                                'Verify if at least one of the required Elements is not positioned
                                                noPOSElements = (From b As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In reqElementsDS.WSOrderTestsForExecutions _
                                                                Where b.ElementStatus = "NOPOS" _
                                                               Select b).ToList.Count()

                                                'The Executions for the Control or Patient ISE Order Test will be marked as LOCKED if there are not positioned elements
                                                sampleIseOT.ExecutionStatus = IIf(noPOSElements > 0, "LOCKED", "PENDING").ToString
                                                If Not sampleIseOT.IsLockedByLISNull AndAlso sampleIseOT.LockedByLIS Then sampleIseOT.ExecutionStatus = "LOCKED" 'AG 25/03/2013 - Locked by LIS not by volume missing
                                            Else
                                                Exit For
                                            End If
                                        Next

                                        'Finally, update the status of the Executions for each Control or Patient ISE Order Test
                                        'AG 19/02/2014 - #1514
                                        'If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstSampleClassExecutions)
                                        lstToPENDING = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList
                                        lstToLOCKED = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "LOCKED" Select a).ToList
                                        If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstToPENDING, lstToLOCKED)
                                        'AG 19/02/2014 - #1514
                                    End If

                                    If (Not resultData.HasError) Then
                                        'Get all CONTROL and PATIENT Order Tests having Pending and/or Locked Executions for STANDARD Tests 
                                        lstSampleClassExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions _
                                                                   Where (a.SampleClass = "CTRL" OrElse a.SampleClass = "PATIENT") _
                                                                 AndAlso a.ExecutionType = "PREP_STD" _
                                                                  Select a).ToList()

                                        If (lstSampleClassExecutions.Count > 0) Then
                                            For Each sampleStdOT As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions
                                                'Get the list of Elements required for the Control or Patient STANDARD Order Test 
                                                resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, sampleStdOT.SampleClass, sampleStdOT.OrderTestID)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    reqElementsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                                                    'Verify if at least one of the required Elements is not positioned
                                                    noPOSElements = (From b As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In reqElementsDS.WSOrderTestsForExecutions _
                                                                    Where b.ElementStatus = "NOPOS" _
                                                                   Select b).ToList.Count()

                                                    newExecStatus = "PENDING"
                                                    If Not sampleStdOT.IsLockedByLISNull AndAlso sampleStdOT.LockedByLIS Then newExecStatus = "LOCKED" 'AG 25/03/2013 - Locked by LIS not by volume missing

                                                    If (noPOSElements > 0) Then
                                                        'There are required Elements not positioned; the Executions of the Order Test will be marked as locked
                                                        newExecStatus = "LOCKED"
                                                    ElseIf newExecStatus = "PENDING" Then 'Else 'AG 25/03/2013 - improve change Else for Else PENDING and reduce queries
                                                        'Elements required for the Control or Patient are positioned, verify if the elements needed for the Calibrator are also positioned
                                                        resultData = VerifyLockedCalibrator(dbConnection, pWorkSessionID, pAnalyzerID, reqElementsDS.WSOrderTestsForExecutions.First.TestID, _
                                                                                            reqElementsDS.WSOrderTestsForExecutions.First.SampleType)
                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            If (DirectCast(resultData.SetDatos, Boolean)) Then
                                                                newExecStatus = "LOCKED"
                                                            Else
                                                                'Verify if the elements needed for the Blank are also positioned
                                                                resultData = VerifyLockedBlank(dbConnection, pWorkSessionID, pAnalyzerID, reqElementsDS.WSOrderTestsForExecutions.First.TestID, _
                                                                                               reqElementsDS.WSOrderTestsForExecutions.First.SampleType)

                                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                    If (DirectCast(resultData.SetDatos, Boolean)) Then newExecStatus = "LOCKED"
                                                                Else
                                                                    Exit For
                                                                End If
                                                            End If
                                                        Else
                                                            Exit For
                                                        End If
                                                    End If
                                                    sampleStdOT.ExecutionStatus = newExecStatus
                                                Else
                                                    Exit For
                                                End If
                                            Next

                                            'Finally, update the status of the Executions for each Control or Patient ISE Order Test
                                            'AG 19/02/2014 - #1514
                                            'If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstSampleClassExecutions)
                                            lstToPENDING = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList
                                            lstToLOCKED = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "LOCKED" Select a).ToList
                                            If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstToPENDING, lstToLOCKED)
                                            'AG 19/02/2014 - #1514
                                        End If
                                    End If
                                End If
                            End If
                            lstSampleClassExecutions = Nothing
                            lstToPENDING = Nothing  'AG 19/02/2014 - #1514
                            lstToLOCKED = Nothing 'AG 19/02/2014 - #1514

                            If (Not resultData.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.RecalculateStatusForNotDeletedExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified ExecutionID, update the InUse flag
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID ">WorkSession Identifier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pInUse">New value for the InUse flag</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Based in UpdateInUse but calling the new DAO function
        ''' </remarks>
        Public Function UpdateInUseNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                       ByVal pExecutionID As Integer, ByVal pInUse As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.UpdateInUseNEW(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, pInUse)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateInUse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For each Execution in the entry DS, update value of fields WellUsed, BaseLineID, AdjustBaseLineID, HasReadings, ThermoWarningFlag,
        ''' ClotValue, ValidReadings and CompleteReadings
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionsDS">Typed DataSet ExecutionsDS containing the group of Executions to update with all the needed data</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/07/2012 - Same as UpdateReadingsFields but call function UpdateReadingsFieldsNEW in the DAO class instead
        '''                              of function UpdateReadingsFields
        ''' </remarks>
        Public Function UpdateReadingsFieldsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSExecutionsDAO As New twksWSExecutionsDAO
                        resultData = mytwksWSExecutionsDAO.UpdateReadingsFieldsNEW(dbConnection, pExecutionsDS)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateReadingsFieldsNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For all received Executions (more than one is received only for MultiPoint Calibrators):
        ''' ** Set ExecutionStatus = CLOSED
        ''' ** If the Execution corresponds to an Standard Preparation, add to it a remark "Finished with optical errors"
        ''' ** If there are not accepted results for the OrderTestID (it is the same for all Executions in the DS), create an 
        '''    empty result (for Multipoint Calibrators, create an empty result for each point)
        ''' ** If the Execution correspond to an Standard Preparation, add to the created result a remark "Finished with optical errors"
        ''' 
        ''' Finally, if the Execution is for the last Replicate requested for the Test, then set to CLOSED the Status of the 
        ''' corresponding OrderTest and Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionsDS">Typed DataSet ExecutionsDS containing the Execution to update or, for MultiPoint Calibrators, 
        '''                             the Executions of all Calibrator Points (because all of them have to be updated)</param>
        ''' <param name="pSTDPrepFlag">When TRUE, it indicates the Execution is for an STANDARD Test
        '''                            When FALSE, it indicates the Execution is for an ISE Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 03/07/2012 - Based in UpdateStatusClosedNOK; changes to improve the function perfomance
        ''' </remarks>
        Public Function UpdateStatusClosedNOK_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS, _
                                                  ByVal pSTDPrepFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update field ExecutionStatus and, for STD Executions, add alarm "Finished with optical errors"
                        Dim execAlarmDS As New WSExecutionAlarmsDS
                        Dim execAlarmRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow

                        For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions.Rows
                            row.BeginEdit()
                            row.ExecutionStatus = "CLOSEDNOK"
                            row.InUse = False
                            row.ResultDate = DateTime.Now
                            row.EndEdit()

                            'For STD Executions, add remark "Finished with optical errors"
                            If (pSTDPrepFlag) Then
                                execAlarmRow = execAlarmDS.twksWSExecutionAlarms.NewtwksWSExecutionAlarmsRow
                                execAlarmRow.ExecutionID = row.ExecutionID
                                execAlarmRow.AlarmID = Alarms.ABS_REMARK13.ToString
                                execAlarmRow.AlarmDateTime = DateTime.Now
                                execAlarmDS.twksWSExecutionAlarms.AddtwksWSExecutionAlarmsRow(execAlarmRow)
                            Else
                                'For ISE Executions no alarm is added; nothing to do
                            End If
                        Next
                        pExecutionsDS.AcceptChanges()
                        execAlarmDS.twksWSExecutionAlarms.AcceptChanges()

                        'Update all Executions in the DS
                        resultData = UpdateStatus(dbConnection, pExecutionsDS)
                        If (Not resultData.HasError AndAlso execAlarmDS.twksWSExecutionAlarms.Rows.Count > 0) Then
                            'Add all Alarms in the DS
                            Dim execAlarmDlg As New WSExecutionAlarmsDelegate
                            resultData = execAlarmDlg.Add(dbConnection, execAlarmDS)
                        End If

                        'Verify if there are accepted results for the OrderTestID (it is the same for all Executions in the DS): 
                        ' ** Yes -> Do nothing 
                        ' ** No  -> Create an empty result (for Multipoint Calibrators, create an empty result for each point)
                        If (Not resultData.HasError) Then
                            Dim resultsDlg As New ResultsDelegate
                            resultData = resultsDlg.GetAcceptedResults(dbConnection, pExecutionsDS.twksWSExecutions.First.OrderTestID, True)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myResults As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                                If (myResults.twksResults.Rows.Count = 0) Then
                                    Dim resultAlarmDS As New ResultAlarmsDS
                                    Dim resultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow

                                    Dim newResultsRow As ResultsDS.twksResultsRow
                                    For Each executionsRow As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions.Rows
                                        newResultsRow = myResults.twksResults.NewtwksResultsRow
                                        newResultsRow.OrderTestID = executionsRow.OrderTestID
                                        newResultsRow.MultiPointNumber = executionsRow.MultiItemNumber
                                        newResultsRow.RerunNumber = executionsRow.RerunNumber
                                        newResultsRow.AcceptedResultFlag = False
                                        newResultsRow.ManualResultFlag = False
                                        newResultsRow.ValidationStatus = "CLOSEDNOK"
                                        newResultsRow.ResultDateTime = DateTime.Now
                                        newResultsRow.AnalyzerID = executionsRow.AnalyzerID
                                        newResultsRow.WorkSessionID = executionsRow.WorkSessionID
                                        myResults.twksResults.AddtwksResultsRow(newResultsRow)

                                        'For STD Executions, add remark "Finished with optical errors"
                                        If (pSTDPrepFlag) Then
                                            resultAlarmRow = resultAlarmDS.twksResultAlarms.NewtwksResultAlarmsRow
                                            resultAlarmRow.OrderTestID = executionsRow.OrderTestID
                                            resultAlarmRow.MultiPointNumber = executionsRow.MultiItemNumber
                                            resultAlarmRow.RerunNumber = executionsRow.RerunNumber
                                            resultAlarmRow.AlarmID = Alarms.ABS_REMARK13.ToString
                                            resultAlarmRow.AlarmDateTime = DateTime.Now
                                            resultAlarmDS.twksResultAlarms.AddtwksResultAlarmsRow(resultAlarmRow)
                                        Else
                                            'For ISE Executions no alarm is added; nothing to do
                                        End If
                                    Next
                                    myResults.twksResults.AcceptChanges()
                                    resultAlarmDS.twksResultAlarms.AcceptChanges()

                                    resultData = resultsDlg.SaveResults(dbConnection, myResults)
                                    If (Not resultData.HasError AndAlso resultAlarmDS.twksResultAlarms.Rows.Count > 0) Then
                                        'Add all Alarms in the DS
                                        Dim resultAlarmDlg As New ResultAlarmsDelegate
                                        resultData = resultAlarmDlg.Add(dbConnection, resultAlarmDS)
                                    End If
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'If the Execution is for the last Replicate requested for the Test, then set to CLOSED the Status of the 
                            'corresponding OrderTest and Order
                            If (pExecutionsDS.twksWSExecutions.First.ReplicateNumber = pExecutionsDS.twksWSExecutions.First.ReplicatesTotalNum) Then
                                Dim ot_delegate As New OrderTestsDelegate
                                resultData = ot_delegate.UpdateStatusByOrderTestID(dbConnection, pExecutionsDS.twksWSExecutions.First.OrderTestID, "CLOSED")
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateStatusClosedNOK_NEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Prepare ISE executions after ISE conditionings (this method is called during the START WS process instead of calling CreateWSExecutions again)
        ''' Code copied from CreateWSExecutions
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pIsISEModuleReady">Flag indicating if the ISE Module is ready to be used. Optional parameter</param>
        ''' <param name="pISEElectrodesList">String list containing ISE Electrodes (ISE_ResultID) with wrong/pending calibration</param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by  AG 31/03/2014 - #1565
        ''' Modified by XB 01/04/2014 - #1565
        ''' </remarks>
        Public Function PrepareISEExecutionsAfterConditioning(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                              ByVal pIsISEModuleReady As Boolean, ByVal pISEElectrodesList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        ' XB 01/04/2014
                        If (pIsISEModuleReady) Then
                            'ISE Module is ready; all LOCKED ISE Preparations are pending again
                            resultData = UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "PREP_ISE", "LOCKED", "PENDING")
                        End If

                        'check if there are Electrodes with wrong Calibration
                        If (Not pISEElectrodesList Is Nothing AndAlso pISEElectrodesList.Count > 0) Then
                            For Each electrode As String In pISEElectrodesList
                                resultData = UpdateStatusByISETestType(dbConnection, pWorkSessionID, pAnalyzerID, electrode, "PENDING", "LOCKED")
                                If (resultData.HasError) Then Exit For
                            Next
                        ElseIf (Not pIsISEModuleReady) Then
                            'ISE Module cannot be used; all pending ISE Preparations are LOCKED
                            resultData = UpdateStatusByExecutionTypeAndStatus(dbConnection, pWorkSessionID, pAnalyzerID, "PREP_ISE", "PENDING", "LOCKED")
                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.PrepareISEExecutionsAfterConditioning", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "METHODS FOR MOVING DATA TO HISTORICS MODULE"
        ''' <summary>
        ''' Get all Executions to export to the Historic Module: those corresponding to accepted and validated Blank, 
        ''' Calibrator and Patient Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identfier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistWSExecutionsDS with all Executions that have to be moved to
        '''          Historic Module</returns>
        ''' <remarks>
        ''' Created by: TR 22/06/2012
        ''' </remarks>
        Public Function GetExecutionsForHistoricTable(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, _
                                                      ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSExecutionsDAO As New twksWSExecutionsDAO
                        myGlobalDataTO = myWSExecutionsDAO.GetExecutionsForHistoricTable(pDBConnection, pWorkSessionID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetExecutionsForHistoricTable", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "New methods for LIS with Embedded Synapse"

        ''' <summary>
        ''' Get all distinct ordertests that have been locked by lis
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pLockedValue"></param>
        ''' <returns>GlobalDataTo (executionsDS)</returns>
        ''' <remarks>AG 25/03/2013</remarks>
        Public Function GetOrderTestsLockedByLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pLockedValue As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetOrderTestsLockedByLIS(dbConnection, pAnalyzerID, pWorkSessionID, pLockedValue)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.GetOrderTestsLockedByLIS", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update the executions table for the entry ordertests id informed (affects only the executions with current status PENDING or LOCKED)
        ''' LockedValue = TRUE -> ExecutionStatus = LOCKED, LockedByLIS = 1
        ''' LockedValue = FALSE -> ExecutionStatus = PENDING, LockedByLIS = 0
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAffectedOrderTests"></param>
        ''' <param name="pNewLockedValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/03/2013</remarks>
        Public Function UpdateLockedByLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAffectedOrderTests As List(Of Integer), ByVal pNewLockedValue As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.UpdateLockedByLIS(dbConnection, pAffectedOrderTests, pNewLockedValue)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.UpdateLockedByLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Search for executions (with status PENDING or LOCKED) and belonging to OrderTests received from LIS
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo (setDatos as boolean)</returns>
        ''' <remarks>AG 16/07/2013</remarks>
        Public Function ExistsLISWorkordersPendingToExecute(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                Dim valueToReturn As Boolean = False

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDao As New twksWSExecutionsDAO
                        resultData = myDao.ExistsLISWorkordersPendingToExecute(dbConnection)

                        'Check if any result
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            If DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions.Rows.Count > 0 Then
                                valueToReturn = True
                            End If
                        End If
                    End If
                End If
                resultData.SetDatos = valueToReturn

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.ExistsLISWorkordersPendingToExecute", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "NEW FUNCTIONS TO IMPROVE PERFORMANCE OF MONITOR SCREEN"

        ''' <summary>
        ''' Get all data (headers and childs) that have to be shown in Samples grid in Monitor Screen 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBlankLabelText">Text used for the Blanks header in Monitor grid (in the active language)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS (subtable vwksWSExecutionsMonitorRow) with all data
        '''          (headers and childs) that have to be shown in Monitor Screen (Samples grid)</returns>
        ''' <remarks>
        ''' Created by: SA 11/03/2014 - BT #1524
        ''' </remarks>
        Public Function GetDataForMonitorTabWS_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pBlankLabelText As String) As GlobalDataTO
            Dim myFinalWSDS As New ExecutionsDS
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get information for STD and ISE Order Tests
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.GetDataSTDISEForMonitorTabWS(dbConnection, pAnalyzerID, pWorkSessionID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim stdIseTestsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            'Get information for CALC and OFFS Order Tests
                            resultData = myDAO.GetDataOFFSCALCForMonitorTabWS(dbConnection, pAnalyzerID, pWorkSessionID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim offsCalcTestsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                Dim previousElementName As String = String.Empty
                                Dim previousSampleClass As String = String.Empty

                                'Get the list of elements with OFFS / CALC tests
                                Dim headerWithOFFSCALCList As New List(Of String)
                                If (offsCalcTestsDS.vwksWSExecutionsMonitor.Rows.Count > 0) Then
                                    headerWithOFFSCALCList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In offsCalcTestsDS.vwksWSExecutionsMonitor
                                                         Where Not a.IsElementNameNull Select a.ElementName Distinct).ToList
                                End If

                                'Get full path for the Icons defined for each Sample Class
                                Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                                Dim freeCellImageBytes As Byte() = preloadedDataConfig.GetIconImage("FREECELL")
                                Dim pauseImageBytes As Byte() = preloadedDataConfig.GetIconImage("EXEC_PAUSED")
                                Dim graphImageBytes As Byte() = preloadedDataConfig.GetIconImage("ABS_GRAPH")
                                Dim exportedImageBytes As Byte() = preloadedDataConfig.GetIconImage("MANUAL_EXP")
                                Dim printedImageBytes As Byte() = preloadedDataConfig.GetIconImage("PRINTL")
                                Dim blankImageBytes As Byte() = preloadedDataConfig.GetIconImage("BLANK")
                                Dim calibImageBytes As Byte() = preloadedDataConfig.GetIconImage("CALIB")
                                Dim ctrlImageBytes As Byte() = preloadedDataConfig.GetIconImage("CTRL")
                                Dim statImageBytes As Byte() = preloadedDataConfig.GetIconImage("STATS")
                                Dim routineImageBytes As Byte() = preloadedDataConfig.GetIconImage("ROUTINES")

                                Dim childRowColor As String = String.Empty
                                Dim numPENDINGChilds As Integer = 0
                                Dim numINPROCESSChilds As Integer = 0
                                Dim numCLOSEDChilds As Integer = 0

                                'Move the content of the two DataSets to the final ExecutionsDS, adding the Header rows and also all Icons
                                Dim myCurrentHeaderIndex As Integer = -1
                                Dim myCurrentChildIndex As Integer = -1
                                Dim myCurrentHeaderPaused As Boolean = False
                                Dim myCurrentHeaderGraph As Boolean = False
                                Dim myCurrentHeaderExported As Boolean = False
                                Dim myCurrentHeaderPrinted As Boolean = False
                                Dim myCurrentHeaderRow As ExecutionsDS.vwksWSExecutionsMonitorRow
                                Dim myCurrentChildRow As ExecutionsDS.vwksWSExecutionsMonitorRow

                                Dim myCalcOffSList As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)
                                For Each row As ExecutionsDS.vwksWSExecutionsMonitorRow In stdIseTestsDS.vwksWSExecutionsMonitor
                                    'Check if it is required to add a new Header Row 
                                    If (row.ElementName <> previousElementName) Then
                                        'Before adding the new Header row, if the previous block was for a Patient, check if there are Calculated and/or OffSystem Tests to add to the block
                                        If (previousElementName <> String.Empty AndAlso previousSampleClass = "PATIENT") Then
                                            If (headerWithOFFSCALCList.Count > 0 AndAlso headerWithOFFSCALCList.Contains(previousElementName)) Then
                                                'Get all Calculated and OffSystem Tests for the ElementName
                                                myCalcOffSList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In offsCalcTestsDS.vwksWSExecutionsMonitor _
                                                                 Where a.ElementName = previousElementName _
                                                                Select a Distinct Order By a.TestType Ascending).ToList

                                                For Each calcOffRow As ExecutionsDS.vwksWSExecutionsMonitorRow In myCalcOffSList
                                                    myFinalWSDS.vwksWSExecutionsMonitor.ImportRow(calcOffRow)
                                                    myCurrentChildIndex = myFinalWSDS.vwksWSExecutionsMonitor.Rows.Count - 1
                                                    myCurrentChildRow = myFinalWSDS.vwksWSExecutionsMonitor.Last()

                                                    'Calculate the background color for the Child Row
                                                    childRowColor = CalculateChildRowColor(row)
                                                    If (childRowColor = "YELLOW") Then
                                                        numPENDINGChilds += 1
                                                    ElseIf (childRowColor = "ORANGE") Then
                                                        numINPROCESSChilds += 1
                                                    ElseIf (childRowColor = "GREEN") Then
                                                        numCLOSEDChilds += 1
                                                    End If

                                                    'Inform all fields needed in the final DS Child Row 
                                                    ConfigureChildRow(calcOffRow, myCurrentChildRow, myCurrentHeaderIndex, myCurrentChildIndex, childRowColor, _
                                                                      freeCellImageBytes, pauseImageBytes, printedImageBytes, exportedImageBytes, graphImageBytes)

                                                    'Mark this row as deleted to avoid processing it again in the final block
                                                    calcOffRow.DeletedRow = True
                                                Next
                                                headerWithOFFSCALCList.Remove(previousElementName)
                                            End If
                                        End If

                                        'If it is not the first Header, calculate the row color of the previous one
                                        If (previousElementName <> String.Empty) Then
                                            myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).BeginEdit()
                                            If (numINPROCESSChilds > 0) Then
                                                myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).RowColor = "ORANGE"
                                            ElseIf (numPENDINGChilds > 0) Then
                                                myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).RowColor = "YELLOW"
                                            Else
                                                myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).RowColor = "GREEN"
                                            End If
                                            myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).EndEdit()
                                        End If

                                        '*********************************************'
                                        '** NEW HEADER: add the row to the final DS **'
                                        '*********************************************'
                                        myFinalWSDS.vwksWSExecutionsMonitor.ImportRow(row)
                                        myCurrentHeaderIndex = myFinalWSDS.vwksWSExecutionsMonitor.Rows.Count - 1
                                        myCurrentHeaderRow = myFinalWSDS.vwksWSExecutionsMonitor.Last()

                                        'Initialize control variables for the new block
                                        numPENDINGChilds = 0
                                        numINPROCESSChilds = 0
                                        numCLOSEDChilds = 0

                                        myCurrentHeaderGraph = False
                                        myCurrentHeaderPaused = False
                                        myCurrentHeaderExported = False
                                        myCurrentHeaderPrinted = False

                                        'Inform all fields needed in the final DS Header Row 
                                        ConfigureHeaderRow(row, myCurrentHeaderRow, myCurrentHeaderIndex, freeCellImageBytes, _
                                                           blankImageBytes, pBlankLabelText, calibImageBytes, ctrlImageBytes, routineImageBytes, _
                                                           statImageBytes)


                                        'Inform the variables used for loop control with the new values
                                        previousElementName = row.ElementName
                                        previousSampleClass = row.SampleClass
                                    End If

                                    '********************************************'
                                    '** NEW CHILD: add the row to the final DS **'
                                    '********************************************'
                                    myFinalWSDS.vwksWSExecutionsMonitor.ImportRow(row)
                                    myCurrentChildIndex = myFinalWSDS.vwksWSExecutionsMonitor.Rows.Count - 1
                                    myCurrentChildRow = myFinalWSDS.vwksWSExecutionsMonitor.Last()

                                    'Calculate the background color for the Child Row
                                    childRowColor = CalculateChildRowColor(row)
                                    If (childRowColor = "YELLOW") Then
                                        numPENDINGChilds += 1
                                    ElseIf (childRowColor = "ORANGE") Then
                                        numINPROCESSChilds += 1
                                    ElseIf (childRowColor = "GREEN") Then
                                        numCLOSEDChilds += 1
                                    End If

                                    'Inform all fields needed in the final DS Child Row 
                                    ConfigureChildRow(row, myCurrentChildRow, myCurrentHeaderIndex, myCurrentChildIndex, childRowColor, _
                                                      freeCellImageBytes, pauseImageBytes, printedImageBytes, exportedImageBytes, graphImageBytes)

                                    'Update the related Header when needed...
                                    myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).BeginEdit()
                                    If (row.TestType = "STD" AndAlso Not myCurrentHeaderGraph) Then
                                        'If one of the Child rows is an Standard Test, the ABS Graph Icon is shown also in the Header row
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).GraphIcon = graphImageBytes
                                        myCurrentHeaderGraph = True
                                    End If
                                    If (row.Paused AndAlso Not myCurrentHeaderPaused) Then
                                        'If one of the Child rows is Paused, the Pause Icon is shown also in the Header row
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).Paused = True
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).PauseIcon = pauseImageBytes
                                        myCurrentHeaderPaused = True
                                    End If
                                    If (row.SampleClass = "PATIENT" OrElse row.SampleClass = "CTRL") AndAlso _
                                       (row.ExportStatus = "SENT" AndAlso childRowColor = "GREEN") AndAlso (Not myCurrentHeaderExported) Then
                                        'If one of the Child rows has been Exported to LIS, the Exported Icon is shown also in the Header row
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).ExportStatus = "SENT"
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).ExportedIcon = exportedImageBytes
                                        myCurrentHeaderExported = True
                                    End If
                                    If row.IsPrintedNull Then row.Printed = False 'AG 19/03/2014 - Protection against DBNULL
                                    If (row.SampleClass = "PATIENT" AndAlso Not row.Printed AndAlso childRowColor = "GREEN") Then
                                        'If one of the Child is available for printing, the Print Available Icon is shown also in the Header row
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).Printed = False
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).PrintedIcon = printedImageBytes
                                        myCurrentHeaderPrinted = True
                                    End If
                                    myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).EndEdit()
                                Next

                                'Check if the last block was for a Patient, and in this case, check if there are Calculated and/or OffSystem Tests to add to the block
                                If (previousElementName <> String.Empty AndAlso previousSampleClass = "PATIENT") Then
                                    If (headerWithOFFSCALCList.Count > 0 AndAlso headerWithOFFSCALCList.Contains(previousElementName)) Then
                                        'Get all Calculated and OffSystem Tests for the ElementName
                                        myCalcOffSList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In offsCalcTestsDS.vwksWSExecutionsMonitor _
                                                         Where a.ElementName = previousElementName _
                                                        Select a Distinct Order By a.TestType Ascending).ToList

                                        For Each calcOffRow As ExecutionsDS.vwksWSExecutionsMonitorRow In myCalcOffSList
                                            myFinalWSDS.vwksWSExecutionsMonitor.ImportRow(calcOffRow)
                                            myCurrentChildIndex = myFinalWSDS.vwksWSExecutionsMonitor.Rows.Count - 1
                                            myCurrentChildRow = myFinalWSDS.vwksWSExecutionsMonitor.Last()

                                            'Calculate the background color for the Child Row
                                            childRowColor = CalculateChildRowColor(calcOffRow)
                                            If (childRowColor = "YELLOW") Then
                                                numPENDINGChilds += 1
                                            ElseIf (childRowColor = "ORANGE") Then
                                                numINPROCESSChilds += 1
                                            ElseIf (childRowColor = "GREEN") Then
                                                numCLOSEDChilds += 1
                                            End If

                                            'Inform all fields needed in the final DS Child Row 
                                            ConfigureChildRow(calcOffRow, myCurrentChildRow, myCurrentHeaderIndex, myCurrentChildIndex, childRowColor, _
                                                              freeCellImageBytes, pauseImageBytes, printedImageBytes, exportedImageBytes, graphImageBytes)

                                            'Mark this row as deleted to avoid processing it again in the final block
                                            calcOffRow.DeletedRow = True
                                        Next

                                        headerWithOFFSCALCList.Remove(previousElementName)
                                    End If
                                End If

                                'Finally, calculate the color of the last added header row
                                'If it is not the first Header, calculate the row color of the previous one
                                If (previousElementName <> String.Empty) Then
                                    myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).BeginEdit()
                                    If (numINPROCESSChilds > 0) Then
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).RowColor = "ORANGE"
                                    ElseIf (numPENDINGChilds > 0) Then
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).RowColor = "YELLOW"
                                    Else
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).RowColor = "GREEN"
                                    End If
                                    myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).EndEdit()
                                End If

                                'If the WS has Patients that have only OffSystem Tests, add all of them to the final DS
                                If (headerWithOFFSCALCList.Count > 0) Then
                                    Dim offSystemTestsList As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow) = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In offsCalcTestsDS.vwksWSExecutionsMonitor _
                                                                                                                 Where a.DeletedRow = False _
                                                                                                               AndAlso a.TestType = "OFFS" _
                                                                                                                Select a).ToList

                                    previousElementName = String.Empty
                                    For Each row As ExecutionsDS.vwksWSExecutionsMonitorRow In offSystemTestsList
                                        If (row.ElementName <> previousElementName) Then
                                            '*********************************************'
                                            '** NEW HEADER: add the row to the final DS **'
                                            '*********************************************'
                                            myFinalWSDS.vwksWSExecutionsMonitor.ImportRow(row)
                                            myCurrentHeaderIndex = myFinalWSDS.vwksWSExecutionsMonitor.Rows.Count - 1
                                            myCurrentHeaderRow = myFinalWSDS.vwksWSExecutionsMonitor.Last()

                                            'Inform all fields needed in the final DS Header Row 
                                            ConfigureHeaderRow(row, myCurrentHeaderRow, myCurrentHeaderIndex, freeCellImageBytes, _
                                                               blankImageBytes, pBlankLabelText, calibImageBytes, ctrlImageBytes, routineImageBytes, _
                                                               statImageBytes)

                                            'Inform the variables used for loop control with the new values
                                            previousElementName = row.ElementName
                                            myCurrentHeaderExported = False
                                        End If

                                        '********************************************'
                                        '** NEW CHILD: add the row to the final DS **'
                                        '********************************************'
                                        myFinalWSDS.vwksWSExecutionsMonitor.ImportRow(row)
                                        myCurrentChildIndex = myFinalWSDS.vwksWSExecutionsMonitor.Rows.Count - 1
                                        myCurrentChildRow = myFinalWSDS.vwksWSExecutionsMonitor.Last()

                                        If (row.OrderTestStatus = "PENDING") Then
                                            childRowColor = "YELLOW"
                                        Else
                                            childRowColor = "GREEN"
                                        End If

                                        'Inform all fields needed in the final DS Child Row 
                                        ConfigureChildRow(row, myCurrentChildRow, myCurrentHeaderIndex, myCurrentChildIndex, childRowColor, _
                                                          freeCellImageBytes, pauseImageBytes, printedImageBytes, exportedImageBytes, graphImageBytes)

                                        'Update the related Header when needed...
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).BeginEdit()
                                        If (row.ExportStatus = "SENT" AndAlso Not myCurrentHeaderExported) Then
                                            'If one of the Child rows has been Exported to LIS, the Exported Icon is shown also in the Header row
                                            myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).ExportedIcon = exportedImageBytes
                                            myCurrentHeaderExported = True
                                        End If
                                        If row.IsPrintedNull Then row.Printed = False 'AG 19/03/2014 - Protection against DBNULL
                                        If (Not row.Printed AndAlso row.OrderTestStatus = "CLOSED" AndAlso Not myCurrentHeaderPrinted) Then
                                            'If one of the Child rows is available for printing, the Print Available Icon is shown also in the Header row
                                            myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).PrintedIcon = printedImageBytes
                                            myCurrentHeaderPrinted = True
                                        End If
                                        myFinalWSDS.vwksWSExecutionsMonitor(myCurrentHeaderIndex).EndEdit()
                                    Next
                                    offSystemTestsList = Nothing
                                End If

                                'Accept all changes made in the final DataSet
                                myFinalWSDS.vwksWSExecutionsMonitor.AcceptChanges()

                                'Set all lists and arrays to Nothing to free the used memory
                                myCalcOffSList = Nothing
                                headerWithOFFSCALCList = Nothing
                                freeCellImageBytes = Nothing
                                pauseImageBytes = Nothing
                                graphImageBytes = Nothing
                                blankImageBytes = Nothing
                                calibImageBytes = Nothing
                                ctrlImageBytes = Nothing
                                statImageBytes = Nothing
                                routineImageBytes = Nothing
                                myCurrentHeaderRow = Nothing
                                myCurrentChildRow = Nothing

                                'Return the final DataSet 
                                resultData.SetDatos = myFinalWSDS
                                resultData.HasError = False
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExecutionsDelegate.GetDataForMonitorTabWS_NEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Calculate the color that should be assigned to the OrderTest in Monitor grid according its current status. 
        ''' </summary>
        ''' <param name="pExecutionRow">Row of typed DataSet ExecutionsDS (subtable vwksWSExecutionsMonitor) containing data of
        '''                             the OrderTest for which the </param>
        ''' <returns>String with the color that should be assigned to the OrderTest in Monitor grid according its current status.
        '''          Value returned can be YELLOW, ORANGE or GREEN</returns>
        ''' <remarks>
        ''' Created by: SA 13/03/2014 - BT #1524
        ''' </remarks>
        Private Function CalculateChildRowColor(ByVal pExecutionRow As ExecutionsDS.vwksWSExecutionsMonitorRow) As String
            Dim rowColor As String = String.Empty

            Try
                Dim checkStatusForOtherReplicates As Boolean = False

                If (pExecutionRow.OrderTestStatus = "CLOSED") Then
                    rowColor = "GREEN"

                ElseIf (pExecutionRow.TestType = "STD" OrElse pExecutionRow.TestType = "ISE") Then
                    Select Case (pExecutionRow.ExecutionStatus)
                        Case "INPROCESS"
                            rowColor = "ORANGE"
                        Case "PENDING"
                            rowColor = "YELLOW"
                        Case Else
                            'First replicate is CLOSED but the OT is NOT CLOSED ==> It could means that not all replicates are CLOSED or that a rerun has been added
                            checkStatusForOtherReplicates = True
                    End Select
                Else
                    'Calculated and OffSystem Tests are always PENDING when they are not CLOSED
                    rowColor = "YELLOW"
                End If

                If (checkStatusForOtherReplicates) Then
                    'Check the Status for the rest of Replicates for the OrderTest
                    Dim resultData As GlobalDataTO
                    resultData = GetByOrderTest(Nothing, pExecutionRow.WorkSessionID, pExecutionRow.AnalyzerID, pExecutionRow.OrderTestID)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim otExecutionsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                        'Get the total number of Replicates for the OrderTest
                        Dim totalReplicates As Integer = (From a As ExecutionsDS.twksWSExecutionsRow In otExecutionsDS.twksWSExecutions _
                                                         Where a.OrderTestID = pExecutionRow.OrderTestID _
                                                       AndAlso a.RerunNumber = pExecutionRow.RerunNumber _
                                                        Select a).Count

                        'Get how many Replicates for the OrderTest are PENDING or LOCKED 
                        Dim totalByStatus As Integer = (From a As ExecutionsDS.twksWSExecutionsRow In otExecutionsDS.twksWSExecutions _
                                                       Where a.OrderTestID = pExecutionRow.OrderTestID _
                                                     AndAlso a.RerunNumber = pExecutionRow.RerunNumber _
                                                     AndAlso (a.ExecutionStatus = "PENDING" OrElse a.ExecutionStatus = "LOCKED") _
                                                      Select a).Count

                        If (totalByStatus = totalReplicates) Then
                            rowColor = "YELLOW"
                        Else
                            'Get how many Replicates for the OrderTest are INPROCESS
                            totalByStatus = (From a As ExecutionsDS.twksWSExecutionsRow In otExecutionsDS.twksWSExecutions _
                                            Where a.OrderTestID = pExecutionRow.OrderTestID _
                                          AndAlso a.RerunNumber = pExecutionRow.RerunNumber _
                                          AndAlso a.ExecutionStatus = "INPROCESS" _
                                           Select a).Count

                            If (totalByStatus > 0) Then
                                rowColor = "ORANGE"
                            Else
                                'All Replicates are CLOSED...
                                rowColor = "GREEN"
                            End If
                        End If
                    End If
                    resultData.SetDatos = Nothing
                End If
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExecutionsDelegate.CalculateChildRowColor", EventLogEntryType.Error, False)
            End Try
            Return rowColor
        End Function

        ''' <summary>
        ''' Build a Header Row for Samples grid in Monitor Screen
        ''' </summary>
        ''' <param name="pRowFromDB">Row of typed DataSet ExecutionsDS.vwksWSExecutionsMonitorRow containing the data needed to built the Header Row</param>
        ''' <param name="pHeaderRow">Row of typed DataSet ExecutionsDS.vwksWSExecutionsMonitorRow to return with all data needed for the Header</param>
        ''' <param name="pHeaderRowIndex">Index of the Header Row in the ExecutionsDS DataSet</param>
        ''' <param name="pFreeCellImageBytes">Icon for empty cells</param>
        ''' <param name="pBlankImageBytes">Icon for Blanks</param>
        ''' <param name="pBlankLabelText">Text for Blank Header Title in the active Language</param>
        ''' <param name="pCalibImageBytes">Icon for Calibrators</param>
        ''' <param name="pCtrlImageBytes">Icon for Controls</param>
        ''' <param name="pRoutineImageBytes">Icon for Routine Patient Samples</param>
        ''' <param name="pStatImageBytes">Icon for Stat Patient Samples</param>
        ''' <remarks>
        ''' Created by: SA 17/03/2014
        ''' </remarks>
        Private Sub ConfigureHeaderRow(ByVal pRowFromDB As ExecutionsDS.vwksWSExecutionsMonitorRow, ByRef pHeaderRow As ExecutionsDS.vwksWSExecutionsMonitorRow, _
                                       ByVal pHeaderRowIndex As Integer, ByVal pFreeCellImageBytes As Byte(), _
                                       ByVal pBlankImageBytes As Byte(), ByVal pBlankLabelText As String, ByVal pCalibImageBytes As Byte(), _
                                       ByVal pCtrlImageBytes As Byte(), ByVal pRoutineImageBytes As Byte(), ByVal pStatImageBytes As Byte())
            Try
                pHeaderRow.BeginEdit()
                If (pRowFromDB.SampleClass = "BLANK") Then
                    pHeaderRow.SampleClassIcon = pBlankImageBytes
                    pHeaderRow.ElementNameToShown = pBlankLabelText
                    pHeaderRow.SampleType = String.Empty
                ElseIf (pRowFromDB.SampleClass = "CALIB") Then
                    pHeaderRow.SampleClassIcon = pCalibImageBytes
                    pHeaderRow.ElementNameToShown = pRowFromDB.ElementName
                    pHeaderRow.SampleType = String.Empty
                ElseIf (pRowFromDB.SampleClass = "CTRL") Then
                    pHeaderRow.SampleClassIcon = pCtrlImageBytes
                    pHeaderRow.ElementNameToShown = pRowFromDB.ElementName
                    pHeaderRow.SampleType = String.Empty
                ElseIf (pRowFromDB.SampleClass = "PATIENT") Then
                    If (pRowFromDB.StatFlag) Then
                        pHeaderRow.SampleClassIcon = pStatImageBytes
                    Else
                        pHeaderRow.SampleClassIcon = pRoutineImageBytes
                    End If

                    If (Not pRowFromDB.IsSpecimenIDListNull) Then
                        pHeaderRow.ElementNameToShown = pRowFromDB.SpecimenIDList & " (" & pRowFromDB.ElementName & ") "
                    Else
                        pHeaderRow.ElementNameToShown = pRowFromDB.ElementName
                    End If
                    If (pRowFromDB.RerunNumber > 1) Then pHeaderRow.ElementNameToShown &= " (" & pRowFromDB.RerunNumber.ToString & ") "

                    If (pRowFromDB.TestType = "OFFS") Then
                        If (pRowFromDB.OrderStatus = "PENDING") Then
                            pHeaderRow.RowColor = "YELLOW"
                        Else
                            pHeaderRow.RowColor = "GREEN"
                        End If
                        pHeaderRow.Paused = False
                    End If
                End If
                pHeaderRow.PauseIcon = pFreeCellImageBytes
                pHeaderRow.Alarms = pFreeCellImageBytes
                pHeaderRow.PrintedIcon = pFreeCellImageBytes
                pHeaderRow.ExportedIcon = pFreeCellImageBytes
                pHeaderRow.GraphIcon = pFreeCellImageBytes
                pHeaderRow.IsHeader = True
                pHeaderRow.Index = pHeaderRowIndex
                pHeaderRow.EndEdit()
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExecutionsDelegate.ConfigureHeaderRow", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Build a Child Row for Samples grid in Monitor Screen
        ''' </summary>
        ''' <param name="pRowFromDB">Row of typed DataSet ExecutionsDS.vwksWSExecutionsMonitorRow containing the data needed to built the Child Row</param>
        ''' <param name="pChildRow">Row of typed DataSet ExecutionsDS.vwksWSExecutionsMonitorRow to return with all data needed for the Child</param>
        ''' <param name="pHeaderRowIndex">Index of the Header Row in the ExecutionsDS DataSet</param>
        ''' <param name="pChildRowIndex">Index of the Child Row in the ExecutionsDS DataSet</param>
        ''' <param name="pChildRowColor">Backcolor for the row according its status</param>
        ''' <param name="pFreeCellImageBytes">Icon for empty cells</param>
        ''' <param name="pPauseImageBytes">Icon for Paused preparation</param>
        ''' <param name="pPrintedImageBytes">Icon for Print Available status</param>
        ''' <param name="pExportedImageBytes">Icon for Exported to LIS</param>
        ''' <param name="pGraphImageBytes">Icon for ABS Graph</param>
        ''' <remarks>
        ''' Created by:  SA 18/03/2014
        ''' </remarks>
        Private Sub ConfigureChildRow(ByVal pRowFromDB As ExecutionsDS.vwksWSExecutionsMonitorRow, ByRef pChildRow As ExecutionsDS.vwksWSExecutionsMonitorRow, _
                                      ByVal pHeaderRowIndex As Integer, ByVal pChildRowIndex As Integer, ByVal pChildRowColor As String, _
                                      ByVal pFreeCellImageBytes As Byte(), ByVal pPauseImageBytes As Byte(), ByVal pPrintedImageBytes As Byte(), _
                                      ByVal pExportedImageBytes As Byte(), ByVal pGraphImageBytes As Byte())
            Try
                pChildRow.BeginEdit()
                pChildRow.RowColor = pChildRowColor
                pChildRow.SampleClassIcon = pFreeCellImageBytes
                If (Not pRowFromDB.IsPausedNull AndAlso pRowFromDB.Paused) Then
                    pChildRow.PauseIcon = pPauseImageBytes
                Else
                    pRowFromDB.Paused = False 'Just in case it is NULL
                    pChildRow.Paused = False
                    pChildRow.PauseIcon = pFreeCellImageBytes
                End If
                pChildRow.ElementNameToShown = pRowFromDB.TestName
                If (Not pRowFromDB.IsRerunNumberNull AndAlso pRowFromDB.RerunNumber > 1) Then pChildRow.ElementNameToShown &= " (" & pRowFromDB.RerunNumber.ToString & ") "
                If (pRowFromDB.SampleClass = "BLANK") Then pChildRow.SampleType = String.Empty
                pChildRow.Alarms = pFreeCellImageBytes

                If pRowFromDB.IsPrintedNull Then pRowFromDB.Printed = False 'AG 19/03/2014 - Protection against DBNULL
                If (pRowFromDB.SampleClass = "PATIENT" AndAlso Not pRowFromDB.Printed AndAlso pChildRowColor = "GREEN") Then
                    pChildRow.PrintedIcon = pPrintedImageBytes
                Else
                    pChildRow.PrintedIcon = pFreeCellImageBytes
                End If
                If (pRowFromDB.SampleClass = "PATIENT" OrElse pRowFromDB.SampleClass = "CTRL") Then
                    If (pRowFromDB.ExportStatus = "SENT") Then
                        pChildRow.ExportedIcon = pExportedImageBytes
                    Else
                        pChildRow.ExportedIcon = pFreeCellImageBytes
                    End If
                Else
                    pChildRow.ExportedIcon = pFreeCellImageBytes
                End If

                If (pRowFromDB.TestType = "STD") Then
                    pChildRow.GraphIcon = pGraphImageBytes
                Else
                    pChildRow.GraphIcon = pFreeCellImageBytes
                End If
                pChildRow.IsHeader = False
                pChildRow.Index = pChildRowIndex
                pChildRow.HeaderRowIndex = pHeaderRowIndex
                pChildRow.EndEdit()
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExecutionsDelegate.ConfigureChildRow", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

 
        ''' <summary>
        ''' This code executes the 4 different optimizations for solving contaminations, that were duplicated on the sort methods with contaminations.
        ''' </summary>
        ''' <param name="activeAnalyzer">Needed analyzer model, because optimizations are model dependent</param>
        ''' <param name="pConn">Connection to database</param>
        ''' <param name="returnDS">Sorted executions as result of optimizations</param>
        ''' <param name="contaminationsDataDS">List of existing contaminations</param>
        ''' <param name="highContaminationPersitance">value that defines high contamination persistence</param>
        ''' <param name="OrderTests">List of the order tests to treat</param>
        ''' <param name="AllTestTypeOrderTests">List of all the order tests</param>
        ''' <param name="OrderContaminationNumber">Number of previous existing contaminations</param>
        ''' <param name="pPreviousReagentID">Optional parameter</param>
        ''' <param name="pPreviousReagentIDMaxReplicates">Optional parameter</param>
        ''' <remarks>
        ''' Created on 19/03/2015 by AJG
        ''' </remarks>
        Private Sub ManageContaminations(ByVal activeAnalyzer As String,
                                         ByVal pConn As SqlConnection,
                                         ByRef returnDS As ExecutionsDS,
                                         ByVal contaminationsDataDS As ContaminationsDS,
                                         ByVal highContaminationPersitance As Integer,
                                         ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                                         ByVal AllTestTypeOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                                         ByVal OrderContaminationNumber As Integer,
                                         Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                                         Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)

            Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            'Dim bestContaminationNumber As Integer = Integer.MaxValue
            Dim currentContaminationNumber As Integer

            If (OrderContaminationNumber > 0) Then

                currentContaminationNumber = OrderContaminationNumber
                currentResult = OrderTests.ToList()
                bestResult = ManageContaminationsForRunningAndStatic(activeAnalyzer, pConn, contaminationsDataDS, currentResult, highContaminationPersitance, currentContaminationNumber, pPreviousReagentID, pPreviousReagentIDMaxReplicates)

                'A last try, if the order tests only have 2 tests that are contaminating between them, why not to interchange them?
                If currentContaminationNumber > 0 Then
                    If OrderTests.Count = 2 Then
                        'Okay, if there are contaminations, why not to try interchange them?
                        currentResult.Clear()
                        For z = OrderTests.Count - 1 To 0 Step -1
                            currentResult.Add(OrderTests(z))
                        Next
                        currentContaminationNumber = GetContaminationNumber(contaminationsDataDS, currentResult, highContaminationPersitance)
                        If currentContaminationNumber = 0 Then
                            bestResult = currentResult
                        End If
                    End If
                End If

                Dim stdPrepFlag As Boolean = False
                For Each wse In AllTestTypeOrderTests
                    If wse.ExecutionType <> "PREP_STD" Then
                        returnDS.twksWSExecutions.ImportRow(wse)
                    ElseIf Not stdPrepFlag Then
                        stdPrepFlag = True
                        'Add all std test executions using bestResult
                        For Each wseStdTest In bestResult
                            returnDS.twksWSExecutions.ImportRow(wseStdTest)
                        Next
                    End If
                Next
            Else
                For Each wse In AllTestTypeOrderTests
                    returnDS.twksWSExecutions.ImportRow(wse)
                Next
            End If

        End Sub

        ''' <summary>
        ''' Applies the 4 different optimizations for solving contaminations between order tests
        ''' </summary>
        ''' <param name="ActiveAnalyzer"></param>
        ''' <param name="pConn"></param>
        ''' <param name="contaminationsDataDS"></param>
        ''' <param name="OrderTests"></param>
        ''' <param name="highContaminationPersistance"></param>
        ''' <param name="currentContaminationNumber"></param>
        ''' <param name="pPreviousReagentID"></param>
        ''' <param name="pPreviousReagentIDMaxReplicates"></param>
        ''' <returns>
        ''' the order test optimized in execution order, if it has been possible
        ''' </returns>
        ''' <remarks>
        ''' Created on 19/03/2015 by AJG
        ''' </remarks>
        Public Shared Function ManageContaminationsForRunningAndStatic(ByVal ActiveAnalyzer As String,
                                                                ByVal pConn As SqlConnection,
                                                                ByVal contaminationsDataDS As ContaminationsDS,
                                                                ByRef OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                                                                ByVal highContaminationPersistance As Integer,
                                                                ByRef currentContaminationNumber As Integer,
                                                                Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                                                                Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing) As List(Of ExecutionsDS.twksWSExecutionsRow)


            Dim myContaminationManager As New ContaminationManager(pConn, ActiveAnalyzer, currentContaminationNumber, highContaminationPersistance, contaminationsDataDS, OrderTests, pPreviousReagentID, pPreviousReagentIDMaxReplicates)

            'Apply Optimization Policy A. (move contaminated OrderTest down until it becomes no contaminated)
            myContaminationManager.ApplyOptimizations(New OptimizationAPolicyApplier(pConn, ActiveAnalyzer), OrderTests)

            'Apply Optimization Policy B. (move contaminated OrderTest up until it becomes no contaminated)
            myContaminationManager.ApplyOptimizations(New OptimizationBPolicyApplier(pConn, ActiveAnalyzer), OrderTests)

            'Apply Optimization Policy C. (move contaminator OrderTest down until it no contaminates)
            myContaminationManager.ApplyOptimizations(New OptimizationCPolicyApplier(pConn, ActiveAnalyzer), OrderTests)

            'Apply Optimization Policy D. (move contaminator OrderTest up until it no contaminates)
            myContaminationManager.ApplyOptimizations(New OptimizationDPolicyApplier(pConn, ActiveAnalyzer), OrderTests)

            ''Apply Optimization using Backtracking algorithm. If exists it'll return an optimal solution with no contaminations
            'myContaminationManager.ApplyOptimizations(New OptimizationBacktrackingApplier(pConn, ActiveAnalyzer), OrderTests)

            currentContaminationNumber = myContaminationManager.currentContaminationNumber
            Return myContaminationManager.bestResult

        End Function

    End Class
End Namespace



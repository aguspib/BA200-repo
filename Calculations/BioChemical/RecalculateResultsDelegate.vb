Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Calculations
    Partial Public Class RecalculateResultsDelegate

#Region "Declarations"
        Private Structure RecalculationsData
            Dim Initialized As Boolean  'Flag (true when structure is initialized, false else)

            Dim AnalyzerID As String
            Dim WorkSessionID As String
            Dim OrderTestID As Integer
            Dim RerunNumber As Integer
            Dim SampleClass As String
            Dim TestID As Integer
            Dim SampleType As String
            Dim Execution As Integer  'The maximum CLOSED replicate belongs to the OrderTestID-RerunNumber (also MultiItem = MAX(OrderTestID multiitem) execution
            Dim MaxItemsNumber As Integer   'OrderTestID - RerunNumber: MAX(MultiItemNumber) // Multiitemnumber required for this OrderTestID
            Dim MaxReplicates As Integer    'OrderTestID - RerunNumber: MAX(ReplicateNumber) // replicates programmed for this OrderTestID
            Dim AcceptedResult As Boolean   'OrderTestID - RerunNumber: only recalculate affected results if the changes blank or calibrator is the accepted one
            Dim ExecutionType As String    'AG 26/11/2010
            Dim ExecutionStatus As String  'AG 18/05/2011
            Dim OrderTestStatus As String  'AG 21/05/2012

            'New fields needed to search last Blank/Calibrators in Historic Module
            Dim PreviousWSID As String
            Dim PreviousOrderTestID As Integer
        End Structure


        ' This structure is initialized with 2 different ways
        ' - Public init: called when Ax00 send new readings and Sw calculate new blank or calibrator (init structure using properties)
        ' - Private init: called when the recalculations is due to an user action (init structure using properties and private init method)
        Dim myRecalData As New RecalculationsData
        Dim myAnalyzerModel As String = "" 'AG 03/07/2012


#End Region

#Region "Properties"
        Public WriteOnly Property AnalyzerModel() As String 'AG 03/07/2012
            Set(ByVal value As String)
                myAnalyzerModel = value
            End Set
        End Property

        Public Property InitializedFlag() As Boolean
            Get
                Return myRecalData.Initialized
            End Get

            Set(ByVal value As Boolean)
                myRecalData.Initialized = value
            End Set
        End Property

        Public Property AnalyzerID() As String
            Get
                Return myRecalData.AnalyzerID
            End Get

            Set(ByVal value As String)
                myRecalData.AnalyzerID = value
            End Set
        End Property

        Public Property WorkSessionID() As String
            Get
                Return myRecalData.WorkSessionID
            End Get

            Set(ByVal value As String)
                myRecalData.WorkSessionID = value
            End Set
        End Property

        Public Property OrderTestID() As Integer
            Get
                Return myRecalData.OrderTestID
            End Get

            Set(ByVal value As Integer)
                myRecalData.OrderTestID = value
            End Set
        End Property

        Public Property RerunNumber() As Integer
            Get
                Return myRecalData.RerunNumber
            End Get

            Set(ByVal value As Integer)
                myRecalData.RerunNumber = value
            End Set
        End Property

        Public Property SampleClass() As String
            Get
                Return myRecalData.SampleClass
            End Get

            Set(ByVal value As String)
                myRecalData.SampleClass = value
            End Set
        End Property

        Public Property TestID() As Integer
            Get
                Return myRecalData.TestID
            End Get

            Set(ByVal value As Integer)
                myRecalData.TestID = value
            End Set
        End Property

        Public Property SampleType() As String
            Get
                Return myRecalData.SampleType
            End Get

            Set(ByVal value As String)
                myRecalData.SampleType = value
            End Set
        End Property

        Public Property ExecutionID() As Integer
            Get
                Return myRecalData.Execution
            End Get

            Set(ByVal value As Integer)
                myRecalData.Execution = value
            End Set
        End Property

        Public Property MaxItemsNumber() As Integer
            Get
                Return myRecalData.MaxItemsNumber
            End Get

            Set(ByVal value As Integer)
                myRecalData.MaxItemsNumber = value
            End Set
        End Property

        Public Property MaxReplicates() As Integer
            Get
                Return myRecalData.MaxReplicates
            End Get

            Set(ByVal value As Integer)
                myRecalData.MaxReplicates = value
            End Set
        End Property

#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Recalculate the Average Result of an ISE OrderTestID/RerunNumber when one of the Replicates are deactivated or activated in 
        ''' Current Results Screen.
        ''' NEW VERSION OF FUNCTION RecalculateAverageValue
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Idenfier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing succes/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2014 - BT #1660
        ''' Modified by: AG 30/07/2014 #1887 (recalculate the ExportStatus after manual recalculations)
        ''' AG 15/10/2014 BA-2011 - Update properly the OrderToExport field when the recalculated result is an accepted one
        ''' </remarks>
        Public Function RecalculateISEAverageValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                   ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the OrderTestID And RerunNumber for the informed ExecutionID
                        Dim exec_delg As New ExecutionsDelegate
                        resultData = exec_delg.GetExecution(dbConnection, pExecutionID, pAnalyzerID, pWorkSessionID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            If (myDS.twksWSExecutions.Rows.Count > 0) Then
                                Dim myOT As Integer = myDS.twksWSExecutions.First.OrderTestID
                                Dim myRerun As Integer = myDS.twksWSExecutions.First.RerunNumber
                                Dim mySampleClass As String = myDS.twksWSExecutions.First.SampleClass

                                'If it is a Control Result, get value of the ControlID from the ExecutionsDS
                                Dim myControlID As Integer = -1
                                If (mySampleClass = "CTRL") Then myControlID = myDS.twksWSExecutions.First.ControlID

                                'Get the current Result value for the OrderTestID/RerunNumber
                                Dim res_DS As New ResultsDS
                                Dim myResultsDelegate As New ResultsDelegate
                                Dim myResultAlarmsDelegate As New ResultAlarmsDelegate

                                resultData = myResultsDelegate.ReadByOrderTestIDandRerunNumber(dbConnection, myOT, myRerun)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    res_DS = DirectCast(resultData.SetDatos, ResultsDS)

                                    If (res_DS.twksResults.Rows.Count > 0) Then
                                        'Calculate the new Result Average value
                                        resultData = GetAverageConcentrationValue(dbConnection, myOT, myRerun)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myAverage As Single = CType(resultData.SetDatos, Single)

                                            'Prepare the DataSet to update the Result Value
                                            res_DS.twksResults(0).BeginEdit()
                                            res_DS.twksResults(0).CONC_Value = myAverage
                                            res_DS.twksResults(0).AnalyzerID = pAnalyzerID
                                            res_DS.twksResults(0).WorkSessionID = pWorkSessionID
                                            res_DS.twksResults(0).SampleClass = mySampleClass

                                            'AG 30/07/2014 #1887 - Update ExportStatus after recalculations and set OrderToExport = TRUE after manual recalculations
                                            resultData = myResultsDelegate.RecalculateExportStatusValue(dbConnection, myOT, myRerun)
                                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                res_DS.twksResults(0).ExportStatus = CType(resultData.SetDatos, String)
                                            End If
                                            'AG 30/07/2014 #1887

                                            res_DS.twksResults(0).AcceptChanges()

                                            'Update the Result Value
                                            resultData = myResultsDelegate.SaveResults(dbConnection, res_DS)

                                            'AG 15/10/2014 BA-2011 - Update properly the OrderToExport field when the recalculated result is an accepted one
                                            If Not resultData.HasError AndAlso res_DS.twksResults(0).AcceptedResultFlag Then
                                                Dim orders_dlg As New OrdersDelegate
                                                resultData = orders_dlg.SetNewOrderToExportValue(dbConnection, , myOT)
                                            End If
                                            'AG 15/10/2014 BA-2011

                                            If (Not resultData.HasError) Then
                                                'Delete all Alarms saved previously for the Average Result
                                                resultData = myResultAlarmsDelegate.DeleteAll(dbConnection, myOT, myRerun, res_DS.twksResults(0).MultiPointNumber)
                                            End If

                                            If (Not resultData.HasError) Then
                                                'Check if there are Reference Ranges Alarms for the new Result
                                                Dim myResultAlarmsDS As New ResultAlarmsDS
                                                Dim myResultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow

                                                resultData = ValidateISERefRanges(dbConnection, mySampleClass, myOT, myDS.twksWSExecutions.First.TestID, _
                                                                                  myDS.twksWSExecutions.First.SampleType, myAverage, myControlID)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    Dim validationResult As String = resultData.SetDatos.ToString

                                                    If (validationResult <> String.Empty) Then
                                                        'Load the Alarm found for the average Result in a row of a typed DataSet ResultAlarmsDS
                                                        myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
                                                        myResultAlarmRow.OrderTestID = myOT
                                                        myResultAlarmRow.RerunNumber = myRerun
                                                        myResultAlarmRow.MultiPointNumber = 1
                                                        myResultAlarmRow.AlarmID = validationResult
                                                        myResultAlarmRow.AlarmDateTime = Now
                                                        myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)
                                                        myResultAlarmsDS.AcceptChanges()
                                                    End If

                                                    'Get the list of different Alarms for all active Executions for the OrderTestID/RerunNumber (excepting the Reference Ranges
                                                    'Alarms: CONC_REMARK7 and CONC_REMARK8)
                                                    Dim myExecutionAlarmsDelegate As New WSExecutionAlarmsDelegate
                                                    resultData = myExecutionAlarmsDelegate.ReadAlarmsForAverageResult(dbConnection, pAnalyzerID, pWorkSessionID, myOT, myRerun)

                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        Dim myAlarmsDS As AlarmsDS = DirectCast(resultData.SetDatos, AlarmsDS)

                                                        'Load all Alarms in the typed DataSet ResultAlarmsDS for the Average Result
                                                        For Each execAlarm As AlarmsDS.tfmwAlarmsRow In myAlarmsDS.tfmwAlarms.Rows
                                                            myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
                                                            myResultAlarmRow.OrderTestID = myOT
                                                            myResultAlarmRow.RerunNumber = myRerun
                                                            myResultAlarmRow.MultiPointNumber = 1
                                                            myResultAlarmRow.AlarmID = execAlarm.AlarmID
                                                            myResultAlarmRow.AlarmDateTime = Now
                                                            myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)
                                                            myResultAlarmsDS.AcceptChanges()
                                                        Next
                                                    End If

                                                    'Finally, if there are Alarms for the Average Result, save them
                                                    If (Not resultData.HasError AndAlso myResultAlarmsDS.twksResultAlarms.Count > 0) Then
                                                        resultData = myResultAlarmsDelegate.Add(dbConnection, myResultAlarmsDS)
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.RecalculateISEAverageValue", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all accepted concentration values for specific OrderTestID and Rerun Number and calculate the Average 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a Single value with the calculate Average CONCENTRATION</returns>
        ''' <remarks>
        ''' Created by:  TR 05/01/2010
        ''' </remarks>
        Public Function GetAverageConcentrationValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                     ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim newAverage As Single = 0
                        Dim myExecDelegate As New ExecutionsDelegate

                        myGlobalDataTO = myExecDelegate.ReadByOrderTestIDandRerunNumber(dbConnection, pOrderTestID, pRerunNumber)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim execDS As ExecutionsDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)

                            Dim itemNumber As Integer = 0
                            For Each row As ExecutionsDS.twksWSExecutionsRow In execDS.twksWSExecutions
                                If (Not row.IsInUseNull AndAlso row.InUse) Then
                                    newAverage += row.CONC_Value
                                    itemNumber += 1
                                End If
                            Next

                            If (itemNumber > 0) Then newAverage = newAverage / itemNumber
                        End If

                        myGlobalDataTO.SetDatos = newAverage
                        myGlobalDataTO.HasError = False
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.GetAverageConcentrationValue", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the informed ISETest/Sample Type, search if there is a Normality Range defined for it.  The searching is different depending on the SampleClass:
        ''' ** For PATIENT results, search if there is a GENERIC or DETAILED Reference Range defined for the ISE Test/SampleType. For DETAILED Ranges, check which 
        '''    of them applies according the Patient Demographics of the specified Order Test
        ''' ** For CONTROL results, search the theoretical Normality range defined for the ISE Test/Sample Type for the specified Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleClass">Sample Class: PATIENT or CTRL</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pTestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCONC_Value">Calculated concentration value for the ISE Test/Sample Type</param>
        ''' <param name="pControlID">Control Identifier. Optional parameter that has to be informed when funtion is called for SampleClass CTRL</param>
        ''' <returns>GlobalDataTO containing a string value with one of the following values:
        '''          ** Empty String: if there is not a Reference Range defined for the ISETest/SampleType or there is one but the result is not out of the limits
        '''          ** CONC_REMARK7: the result is lesser than the lower limit
        '''          ** CONC_REMARK8: the result is greater than the upper limit
        ''' </returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2014 - BT #1660
        ''' </remarks>
        Public Function ValidateISERefRanges(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleClass As String, ByVal pOrderTestID As Integer, _
                                             ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pCONC_Value As Single, _
                                             Optional ByVal pControlID As Integer = -1) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim validationResult As String = String.Empty
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim lowerLimit As Single = -1
                        Dim upperLimit As Single = -1

                        If (pSampleClass = "PATIENT") Then
                            'Check if there are Reference Ranges defined for the ISE Test/Sample Type and in this case, get the RangeType: GENERIC or DETAILED
                            Dim myISETestSampleDelegate As New ISETestSamplesDelegate
                            resultData = myISETestSampleDelegate.GetListByISETestID(dbConnection, pTestID, pSampleType)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myISETestSampleDS As ISETestSamplesDS = DirectCast(resultData.SetDatos, ISETestSamplesDS)

                                If (myISETestSampleDS.tparISETestSamples.Count > 0) Then
                                    If (Not myISETestSampleDS.tparISETestSamples.First.IsActiveRangeTypeNull) AndAlso _
                                       (myISETestSampleDS.tparISETestSamples.First.ActiveRangeType <> String.Empty) Then
                                        'Get the Reference Range Interval defined for the ISE Test/SampleType
                                        Dim myOrderTestsDelegate As New OrderTestsDelegate
                                        resultData = myOrderTestsDelegate.GetReferenceRangeInterval(dbConnection, pOrderTestID, "ISE", pTestID, pSampleType, _
                                                                                                    myISETestSampleDS.tparISETestSamples.First.ActiveRangeType)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myTestRefRangesDS As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                                            If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
                                                lowerLimit = myTestRefRangesDS.tparTestRefRanges.First.NormalLowerLimit
                                                upperLimit = myTestRefRangesDS.tparTestRefRanges.First.NormalUpperLimit
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                        ElseIf (pSampleClass = "CTRL") Then
                            'Get the theoretical range limits for the Control
                            Dim myTestControlsDelegate As New TestControlsDelegate
                            resultData = myTestControlsDelegate.GetControlsNEW(dbConnection, "ISE", pTestID, pSampleType, pControlID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myTestControlsDS As TestControlsDS = DirectCast(resultData.SetDatos, TestControlsDS)

                                If (myTestControlsDS.tparTestControls.Rows.Count > 0) Then
                                    lowerLimit = myTestControlsDS.tparTestControls.First.MinConcentration
                                    upperLimit = myTestControlsDS.tparTestControls.First.MaxConcentration
                                End If
                            End If
                        End If

                        'If a Normality Range was found for the ISE Test/Sample Type, validate the Concentration value
                        If (lowerLimit <> -1 AndAlso upperLimit <> -1) Then
                            If (pCONC_Value < lowerLimit) Then
                                validationResult = GlobalEnumerates.Alarms.CONC_REMARK7.ToString
                            ElseIf (pCONC_Value > upperLimit) Then
                                validationResult = GlobalEnumerates.Alarms.CONC_REMARK8.ToString
                            End If
                        End If
                    End If
                End If

                resultData.SetDatos = validationResult
                resultData.HasError = False

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.ValidateISERefRanges", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NEW METHODS FOR PERFORMANCE IMPROVEMENTS"
        ''' <summary>
        ''' Change the accepted result when there are several reruns. Apply to all Sample Classes excepting CONTROLS (for Controls it is 
        ''' possible to accept several Reruns)
        ''' </summary>
        ''' <param name="pSelectedExecRow"></param>
        ''' <param name="pExportStatus"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 16/07/2012 - Based in ChangeAcceptedResult
        ''' AG 16/10/2014 BA-2011
        '''     Inform new required parameters + add pExportStatus parameter
        '''     #1 Change Accepted result for a PATIENT or CONTROL do not launch RecalculateResultsNEW (but in case of PATIENTs recalculate affected calculated tests ExecuteCalculatedTest)
        '''     #2 Set OrderToExport = TRUE when the new accepted result belongs a patient or a control and his export status is different than SENT
        ''' </remarks>
        Public Function ChangeAcceptedResultNEW(ByVal pSelectedExecRow As ExecutionsDS.vwksWSExecutionsResultsRow, ByVal pExportStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'Set AcceptedResultFlag = TRUE for the result having the informed OrderTestID and RerunNumber
                Dim myResDelegate As New ResultsDelegate
                resultData = myResDelegate.UpdateAcceptedResult(dbConnection, pSelectedExecRow.OrderTestID, pSelectedExecRow.RerunNumber, True, _
                                                                pSelectedExecRow.AnalyzerID, pSelectedExecRow.WorkSessionID)

                If (Not resultData.HasError) Then
                    'For BLANKS, CALIBRATORS AND PATIENT RESULTS (all TestTypes), set to False the AcceptedResultFlag of the previous accepted Rerun 
                    If (pSelectedExecRow.SampleClass <> "CTRL") Then
                        resultData = myResDelegate.ResetAcceptedResultFlag(dbConnection, pSelectedExecRow.OrderTestID, pSelectedExecRow.RerunNumber, _
                                                                           pSelectedExecRow.AnalyzerID, pSelectedExecRow.WorkSessionID)
                    End If
                End If

                If (Not resultData.HasError) Then
                    'For Results of STANDARD Tests, recalculate all Results affected for the new accepted Rerun
                    'AG 16/10/2014 BA-2011 #2 Recalculate only for blank and calib
                    'If (pSelectedExecRow.TestType = "STD") Then
                    If (pSelectedExecRow.TestType = "STD") AndAlso (pSelectedExecRow.SampleClass = "BLANK" OrElse pSelectedExecRow.SampleClass = "CALIB") Then
                        'Recalculate all the affected Results - The row with all data of the Execution to recalculate is sent as selected 
                        'Execution and also as Execution to recalculate

                        'AG 15/10/2014 BA-2011 inform the new required parameters
                        resultData = RecalculateResultsNEW(Nothing, pSelectedExecRow, pSelectedExecRow, True, False)
                    End If


                    'AG 16/10/2014 BA-2011 
                    '#1 recalculate calculated tests
                    If (pSelectedExecRow.SampleClass = "PATIENT") Then
                        Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                        myCalcTestsDelegate.AnalyzerID = pSelectedExecRow.AnalyzerID
                        myCalcTestsDelegate.WorkSessionID = pSelectedExecRow.WorkSessionID
                        resultData = myCalcTestsDelegate.ExecuteCalculatedTest(Nothing, pSelectedExecRow.OrderTestID, True)
                    End If

                    '#2 set OrderToExport = TRUE (except if the test is not mapped)
                    If (pSelectedExecRow.SampleClass = "CTRL" OrElse pSelectedExecRow.SampleClass = "PATIENT") AndAlso (pExportStatus <> "SENT") Then
                        Dim orders_dlg As New OrdersDelegate
                        resultData = orders_dlg.SetNewOrderToExportValue(dbConnection, , pSelectedExecRow.OrderTestID)
                    End If
                    'AG 16/10/2014 BA-2011

                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.ChangeAcceptedResultNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' ** When pNewInUseValue = TRUE: the InUse flag is set to TRUE and recalculation process is executed
        ''' ** When pNewInUseValue = FALSE (a Replicate is annulled): get all CLOSED Executions for the OrderTestID/RerunNumber, sorted by
        '''    MultiItemNumber DESC and ReplicateNumber DESC; if there is at least one, then the InUse flag is set to TRUE and recalculation 
        '''    process is executed
        ''' </summary>
        ''' <param name="pSelectedExecRow">Row of a typed Dataset ExecutionsDS containing all data of an specific Execution</param>
        ''' <param name="pNewInUseValue">When TRUE, the InUse flag is updated to TRUE and recalculations are executed
        '''                              When FALSE, if there are more than one CLOSED and InUse Execution for the informed OrderTestID and RerunNumber,
        '''                              the InUse flag is updated to TRUE and recalculations are executed
        ''' </param>
        ''' <returns>GlobalDataTO </returns>
        ''' <remarks>
        ''' Created by: SA 09/07/2012 - Based in ChangeInUseFlagReplicate
        ''' AG 15/10/2014 BA-2011 inform the new required parameters
        ''' </remarks>
        Public Function ChangeInUseFlagReplicateNEW(ByVal pSelectedExecRow As ExecutionsDS.vwksWSExecutionsResultsRow, _
                                                    ByVal pNewInUseValue As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myResult As Boolean = False
                Dim myExecutionsDelegate As New ExecutionsDelegate
                resultData = myExecutionsDelegate.GetClosedExecutionsRelatedNEW(Nothing, pSelectedExecRow.AnalyzerID, pSelectedExecRow.WorkSessionID, _
                                                                                pSelectedExecRow.OrderTestID, pSelectedExecRow.RerunNumber)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myExecutionsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                    If (myExecutionsDS.vwksWSExecutionsResults.Rows.Count > 0) Then
                        Dim maxMultiItemNumber As Integer = myExecutionsDS.vwksWSExecutionsResults.First.MultiItemNumber

                        If (pNewInUseValue) Then
                            'A result can be included always
                            myResult = True
                        Else
                            'Exclusion is allowed only when there is at least two InUse Executions for the same MultiItem Number
                            Dim inUseReplicates As Integer = 0
                            inUseReplicates = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In myExecutionsDS.vwksWSExecutionsResults _
                                              Where a.MultiItemNumber = pSelectedExecRow.MultiItemNumber _
                                            AndAlso a.InUse = True _
                                            AndAlso a.IsABS_ErrorNull _
                                             Select a).Count
                            myResult = (inUseReplicates > 1)
                        End If

                        If (myResult) Then
                            'Update the InUse flag to the value indicated by entry parameter pNewInUseValue
                            resultData = myExecutionsDelegate.UpdateInUseNEW(Nothing, pSelectedExecRow.AnalyzerID, pSelectedExecRow.WorkSessionID, _
                                                                             pSelectedExecRow.ExecutionID, pNewInUseValue)
                            If (Not resultData.HasError) Then
                                'Only for Calibrators, the experimental results are recovered
                                If (pSelectedExecRow.SampleClass = "CALIB" AndAlso pSelectedExecRow.ExecutionType = "PREP_STD") Then
                                    Dim resultsDelegate As New ResultsDelegate
                                    'AG 16/10/2014 BA-2011 inform ExportStatus as NOTSENT
                                    resultData = resultsDelegate.UpdateManualResult(Nothing, False, "QUANTIVE", 1, "", pSelectedExecRow.OrderTestID, maxMultiItemNumber, _
                                                                                    pSelectedExecRow.RerunNumber, "NOTSENT")
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'Execute recalculations excluding the annuled Replicate
                                'AG 15/10/2014 BA-2011 inform the new required parameters
                                resultData = RecalculateResultsNEW(Nothing, pSelectedExecRow, myExecutionsDS.vwksWSExecutionsResults.First, True, False)
                            End If
                        End If


                    End If
                End If
                resultData.SetDatos = myResult
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.ChangeInUseFlagReplicateNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute recalculations of all results affected for the new Result of an specific Execution 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSelectedExecRow">Row of a typed Dataset ExecutionsDS containing all data of the Execution to which corresponds the new Result</param>
        ''' <param name="pExecToRecalculateRow">Row of a typed DataSet ExecutionsDS contained all data of the Execution that has to be recalculated 
        '''                                     (the one with the maximum MultiItemNumber and ReplicateNumber for the selected OrderTestID/RerunNumber</param>
        ''' <param name="pManualRecalculationFlag">When TRUE, it indicates recalculations are executed due to an User's action (include/exclude a 
        '''                                        Replicate in Results o Results Calibration screens OR change the Accepted Result in Results screen). 
        '''                                        When FALSE, recalculations are executed due to new Results are received from the Analyzer</param>
        ''' <param name="pManualFactorEvent">Optional parameter. When TRUE, it means recalculations are called from ManualFactor functionality</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Based in RecalculateResults
        ''' Modified by: SA 12/06/2014 - BT# 1660 ==> When the Result to recalculate is for an  ISETest/SampleType, new function RecalculateISEAverageValue is
        '''                                           called instead of the previous version function (RecalculateAverageValue)
        '''              AG 15/10/2014 BA-2011 do not use optional parameters
        '''              XB 28/11/2014 - recalculates calculated tests also for ISE tests - BA-1867
        ''' </remarks>
        Public Function RecalculateResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSelectedExecRow As ExecutionsDS.vwksWSExecutionsResultsRow, _
                                              ByVal pExecToRecalculateRow As ExecutionsDS.vwksWSExecutionsResultsRow, ByVal pManualRecalculationFlag As Boolean, _
                                              ByVal pManualFactorEvent As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                'Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                Dim maxMultiItemNumber As Integer = pExecToRecalculateRow.MultiItemNumber
                Dim maxReplicatesNumber As Integer = pExecToRecalculateRow.ReplicateNumber

                If (Not myRecalData.Initialized) Then
                    resultData = InitNEW(dbConnection, pSelectedExecRow, maxMultiItemNumber, maxReplicatesNumber)
                Else
                    resultData = New GlobalDataTO
                End If

                If (myRecalData.ExecutionType = "PREP_STD" AndAlso myRecalData.ExecutionStatus = "CLOSED") Then
                    If (Not resultData.HasError) Then
                        'The recalculation will be executed for the Execution with Max(MultiItemNumber) and Max(ReplicateNumber) for the OrderTestID/RerunNumber
                        myRecalData.Execution = pExecToRecalculateRow.ExecutionID

                        If (pManualRecalculationFlag AndAlso Not pManualFactorEvent) Then
                            Dim myCalcDelegate As New CalculationsDelegate()
                            myCalcDelegate.AnalyzerModel = myAnalyzerModel
                            resultData = myCalcDelegate.CalculateExecutionNEW(dbConnection, myRecalData.AnalyzerID, myRecalData.WorkSessionID, myRecalData.Execution, _
                                                                              True, myRecalData.SampleClass, pManualRecalculationFlag, Nothing)
                        End If

                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        GlobalBase.CreateLogActivity("Recalculate selected result: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                        "RecalculateResultsDelegate.RecalculateResults NEW", EventLogEntryType.Information, False)
                        StartTime = Now
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    End If

                    If (Not resultData.HasError) Then
                        If (myRecalData.AcceptedResult) Then
                            Dim ex_deleg As New ExecutionsDelegate

                            'TR 26/07/2012 -Declare Ouside the for
                            Dim myCalcDelegate As New CalculationsDelegate()

                            If (myRecalData.SampleClass = "BLANK") Then
                                'Search all affected Calibrators to recalculate their Results
                                resultData = ex_deleg.ReadAffectedCalibExecutions(dbConnection, myRecalData.AnalyzerID, myRecalData.WorkSessionID, myRecalData.TestID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim executionsList As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                    For Each ex_row As ExecutionsDS.twksWSExecutionsRow In executionsList.twksWSExecutions
                                        'Dim myCalcDelegate As New CalculationsDelegate()
                                        myCalcDelegate.AnalyzerModel = myAnalyzerModel
                                        resultData = myCalcDelegate.CalculateExecutionNEW(dbConnection, myRecalData.AnalyzerID, myRecalData.WorkSessionID, ex_row.ExecutionID, _
                                                                                          True, myRecalData.SampleClass, pManualRecalculationFlag, ex_row)
                                        If (resultData.HasError) Then Exit For
                                    Next

                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    GlobalBase.CreateLogActivity("Recalculate affected CALIBs: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                    "RecalculateResultsDelegate.RecalculateResults NEW", EventLogEntryType.Information, False)
                                    StartTime = Now
                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'Search all affected Control and Patient Executions to recalculate their Results
                                If (myRecalData.SampleClass = "BLANK" OrElse myRecalData.SampleClass = "CALIB") Then
                                    Dim mySampleType As String = IIf(myRecalData.SampleClass = "BLANK", String.Empty, myRecalData.SampleType).ToString

                                    resultData = ex_deleg.ReadAffectedCtrlPatientExecutions(dbConnection, myRecalData.AnalyzerID, myRecalData.WorkSessionID, myRecalData.TestID, _
                                                                                            mySampleType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim executionsList As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                        For Each ex_row As ExecutionsDS.twksWSExecutionsRow In executionsList.twksWSExecutions
                                            'Dim myCalcDelegate As New CalculationsDelegate()
                                            myCalcDelegate.AnalyzerModel = myAnalyzerModel

                                            resultData = myCalcDelegate.CalculateExecutionNEW(dbConnection, myRecalData.AnalyzerID, myRecalData.WorkSessionID, ex_row.ExecutionID, _
                                                                                              True, myRecalData.SampleClass, pManualRecalculationFlag, ex_row)
                                            If (resultData.HasError) Then Exit For
                                        Next

                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        GlobalBase.CreateLogActivity("Recalculate affected CTRLs and PATIENTs: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                        "RecalculateResultsDelegate.RecalculateResults NEW", EventLogEntryType.Information, False)
                                        StartTime = Now
                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    End If
                                End If
                            End If
                        End If
                    End If

                ElseIf (myRecalData.ExecutionType = "PREP_ISE" AndAlso myRecalData.ExecutionStatus = "CLOSED") Then
                    resultData = RecalculateISEAverageValue(Nothing, myRecalData.AnalyzerID, myRecalData.WorkSessionID, myRecalData.Execution)

                    ' XB 28/11/2014 - BA-1867
                    ' recalculate calculated tests
                    If (pSelectedExecRow.SampleClass = "PATIENT") Then
                        Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                        myCalcTestsDelegate.AnalyzerID = pSelectedExecRow.AnalyzerID
                        myCalcTestsDelegate.WorkSessionID = pSelectedExecRow.WorkSessionID
                        resultData = myCalcTestsDelegate.ExecuteCalculatedTest(Nothing, pSelectedExecRow.OrderTestID, True)
                    End If
                    ' XB 28/11/2014 - BA-1867

                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    GlobalBase.CreateLogActivity("Recalculate selected ISE result: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                    "RecalculateResultsDelegate.RecalculateResults NEW", EventLogEntryType.Information, False)
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.RecalculateResultsNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change Experimental Calibration for a manual Calibration Factor (ManualFactorFlag = TRUE and FactorValue informed) OR
        ''' Change a manual Calibration Factor for the Experimental Calibration Factor (ManualFactorFlag = FALSE) 
        ''' </summary>
        ''' <param name="pSelectedExecRow">Row of typed DataSet ExecutionsDS containing the information of the Execution for the 
        '''                                last ReplicateNumber of a single point Calibrator</param>
        ''' <param name="pManualFactorFlag">When TRUE, it indicates the Calibration will be done using a Manual Factor
        '''                                 When FALSE, it indicates the Experimental Calibration will be used</param>
        ''' <param name="pFactorValue">When ManualFactorFlag = TRUE, it indicates the theorical Factor Value</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/07/2012
        ''' AG 15/10/2014 BA-2011 inform the new required parameters
        ''' </remarks>
        Public Function UpdateManualCalibrationFactorNEW(ByVal pSelectedExecRow As ExecutionsDS.vwksWSExecutionsResultsRow, _
                                                         ByVal pManualFactorFlag As Boolean, ByVal pFactorValue As Single) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'Update the result setting ManualResultFlag = TRUE and ManualResult = the entered Factor value
                Dim myResults As New ResultsDelegate
                'AG 16/10/2014 BA-2011 inform ExportStatus as NOTSENT
                resultData = myResults.UpdateManualResult(Nothing, pManualFactorFlag, "QUANTIVE", pFactorValue, String.Empty, _
                                                          pSelectedExecRow.OrderTestID, pSelectedExecRow.MultiItemNumber, pSelectedExecRow.RerunNumber, "NOTSENT")

                If (Not resultData.HasError) Then
                    'Special code for ManualFactor recalculations from WS Preparation screen
                    If (pSelectedExecRow.ExecutionID = -1) Then
                        With myRecalData
                            .AnalyzerID = pSelectedExecRow.AnalyzerID
                            .Execution = pSelectedExecRow.ExecutionID
                            .WorkSessionID = pSelectedExecRow.WorkSessionID
                            .OrderTestID = pSelectedExecRow.OrderTestID
                            .RerunNumber = pSelectedExecRow.RerunNumber
                            .SampleClass = "CALIB"
                            .TestID = pSelectedExecRow.TestID
                            .SampleType = pSelectedExecRow.SampleType
                            .ExecutionType = "PREP_STD"
                            .ExecutionStatus = pSelectedExecRow.ExecutionStatus
                            .MaxItemsNumber = 1
                            .MaxReplicates = 1
                            .AcceptedResult = True
                            .Initialized = True
                        End With
                    End If

                    'AG 15/10/2014 BA-2011 inform the new required parameters
                    resultData = RecalculateResultsNEW(Nothing, pSelectedExecRow, pSelectedExecRow, True, True)
                End If
                resultData.SetDatos = (Not resultData.HasError)

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.UpdateManualCalibrationFactorNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Initializes the re-calculation structures (global variable class myRecalData)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing succes/error information</returns>
        ''' <remarks>
        ''' Created by: SA 09/07/2012 
        ''' </remarks>
        Private Function InitNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSelectedExecRow As ExecutionsDS.vwksWSExecutionsResultsRow, _
                                 ByVal pMaxMultiItemNum As Integer, ByVal pMaxReplicateNum As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        With myRecalData
                            .AnalyzerID = pSelectedExecRow.AnalyzerID
                            .WorkSessionID = pSelectedExecRow.WorkSessionID
                            .Execution = pSelectedExecRow.ExecutionID
                            .OrderTestID = pSelectedExecRow.OrderTestID
                            If (Not pSelectedExecRow.IsOrderTestStatusNull) Then .OrderTestStatus = pSelectedExecRow.OrderTestStatus
                            .RerunNumber = pSelectedExecRow.RerunNumber
                            .SampleClass = pSelectedExecRow.SampleClass
                            .TestID = pSelectedExecRow.TestID
                            .SampleType = pSelectedExecRow.SampleType
                            .ExecutionType = pSelectedExecRow.ExecutionType
                            .MaxItemsNumber = pMaxMultiItemNum
                            .MaxReplicates = pMaxReplicateNum
                            .AcceptedResult = True
                            .ExecutionStatus = pSelectedExecRow.ExecutionStatus
                            .Initialized = True
                        End With

                        'Affected Results will be recalculated only if the changed Blank or Calibrator is the accepted one
                        Dim res_delegate As New ResultsDelegate

                        resultData = res_delegate.GetResults(dbConnection, myRecalData.OrderTestID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim res_DS As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                            If (res_DS.vwksResults.Rows.Count > 1) Then
                                myRecalData.AcceptedResult = (From a As ResultsDS.vwksResultsRow In res_DS.vwksResults _
                                                             Where a.RerunNumber = myRecalData.RerunNumber _
                                                            Select a.AcceptedResultFlag).First
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RecalculateResultsDelegate.InitNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace


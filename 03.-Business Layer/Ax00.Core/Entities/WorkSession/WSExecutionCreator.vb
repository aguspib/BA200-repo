Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession

    ''' <summary>
    ''' Class WSExecutionCreator. Implements a class which creates the WSExecution. It's implements through the Singleton pattern, 
    ''' so it's guaranteed that only one instance can be running.
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class WSExecutionCreator
        Private Shared _instance As WSExecutionCreator = Nothing

        'Parameters for create the Work Session Executions

        Private pDBConnection As SqlConnection
        Private pAnalyzerID As String
        Private pWorkSessionID As String
        Private pWorkInRunningMode As Boolean
        Private pOrderTestID As Integer = -1
        Private pPostDilutionType As String = ""
        Private pIsISEModuleReady As Boolean = False
        Private pISEElectrodesList As List(Of String) = Nothing
        Private pPauseMode As Boolean = False
        Private pManualRerunFlag As Boolean = True
        Private myDao As New twksWSExecutionsDAO()
        Private NullElementID As Integer = -1
        Private orderTestLockedByLISList As List(Of Integer)
        Private calledForRerun As Boolean

        'Get all executions for BLANKS included in the WorkSession
        Private myBlankExecutionsDS As New ExecutionsDS
        'Get all executions for CALIBRATORS included in the WorkSession
        Private myCalibratorExecutionsDS As ExecutionsDS
        'Get all executions for CONTROLS included in the WorkSession
        Private myControlExecutionsDS As ExecutionsDS
        'Get all executions for PATIENT SAMPLES included in the WorkSession
        Private myPatientExecutionsDS As ExecutionsDS
        'Get detailed information of all Order Tests to be executed in the WS
        Private allOrderTestsDS As OrderTestsForExecutionsDS

        Private pendingExecutionsDS As ExecutionsDS
        Private activeAnalyzer As String

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub New()

        End Sub

        ''' <summary>
        ''' Property which contains the only instance for this class
        ''' </summary>
        ''' <value></value>
        ''' <returns>The unique instance for this class</returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Instance As WSExecutionCreator
            Get
                Static myObject As Object = New Object()

                If IsNothing(_instance) Then
                    SyncLock myObject
                        If IsNothing(_instance) Then
                            _instance = New WSExecutionCreator()
                        End If
                    End SyncLock
                End If
                Return _instance
            End Get
        End Property

        Public ReadOnly Property AnalyzerID As String
            Get
                Return AnalyzerManager.GetCurrentAnalyzerManager.ActiveAnalyzer
            End Get
        End Property

        Public ReadOnly Property WorksesionID As String
            Get
                'Return pWorkSessionID
                Return AnalyzerManager.GetCurrentAnalyzerManager.ActiveWorkSession
            End Get
        End Property

        Public Property ContaminationsSpecification As IAnalyzerContaminationsSpecification

        ' ReSharper disable once UnusedMember.Global This method is USED by Reflection.
        Public Function CreateWS(ByVal ppDBConnection As SqlConnection, ppAnalyzerID As String, ByVal ppWorkSessionID As String, _
                                           ByVal ppWorkInRunningMode As Boolean, Optional ByVal ppOrderTestID As Integer = -1, _
                                           Optional ByVal ppPostDilutionType As String = "", Optional ByVal ppIsISEModuleReady As Boolean = False, _
                                           Optional ByVal ppISEElectrodesList As List(Of String) = Nothing, Optional ByVal ppPauseMode As Boolean = False, _
                                           Optional ByVal ppManualRerunFlag As Boolean = True) As GlobalDataTO 'Implements IWSExecutionCreator.CreateWS

            Me.ContaminationsSpecification = ContaminationsSpecification

            'Initialization from parameters
            pDBConnection = ppDBConnection
            pAnalyzerID = ppAnalyzerID
            pWorkSessionID = ppWorkSessionID
            pWorkInRunningMode = ppWorkInRunningMode
            pOrderTestID = ppOrderTestID
            pPostDilutionType = ppPostDilutionType
            pIsISEModuleReady = ppIsISEModuleReady
            pISEElectrodesList = ppISEElectrodesList
            pPauseMode = ppPauseMode
            pManualRerunFlag = ppManualRerunFlag

            Dim resultData As GlobalDataTO = Nothing
            Try
                pDBConnection = ResolveConnection()
                SetSemaphoreToBusy()
                calledForRerun = (pOrderTestID <> -1)
                orderTestLockedByLISList = GetOrderTestsLockedByLIS()
                activeAnalyzer = GetActiveAnalyzer(pDBConnection)

                If Not pWorkInRunningMode Then
                    resultData = SearchNotInCourseExecutionsToDelete()
                End If

                If Not calledForRerun Then
                    resultData = RecalculateStatusForNotDeletedExecutions()
                End If

                'Now we can create executions for all ordertests not started (with no executions in twksWSExecutions table) ... <the initial process>
                If (Not resultData.HasError) Then
                    resultData = CreateExecutionsOrderTestsNotStarted()
                End If

                If (Not resultData.HasError AndAlso Not calledForRerun) Then
                    resultData = UpdatePositionsRotorSample()
                End If

                If (Not resultData.HasError) Then
                    resultData = UpdateStatus()
                End If

                FinishTransaction(resultData, ppDBConnection)

                orderTestLockedByLISList = Nothing

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                    If (ppDBConnection Is Nothing AndAlso Not pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(pDBConnection)
                End If

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                GlobalBase.CreateLogActivity(ex) '.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Error, False)
            Finally
                If (ppDBConnection Is Nothing AndAlso Not pDBConnection Is Nothing) Then pDBConnection.Close()

                ReleaseSemaphoreToAvailable()
            End Try

            Return resultData
        End Function

        Private Function GetOrderTestsLockedByLIS() As List(Of Integer)
            Dim resultData = myDao.GetOrderTestsLockedByLIS(pDBConnection, pAnalyzerID, pWorkSessionID, True)
            Dim orderTestLockedByLISList As New List(Of Integer)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                For Each row As ExecutionsDS.twksWSExecutionsRow In DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions
                    If Not row.IsOrderTestIDNull AndAlso Not orderTestLockedByLISList.Contains(row.OrderTestID) Then orderTestLockedByLISList.Add(row.OrderTestID)
                Next
            End If

            Return orderTestLockedByLISList
        End Function

        Private Function ResolveConnection() As SqlConnection
            Dim dbConnection As New SqlConnection
            Dim resultData As GlobalDataTO

            If (GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                dbConnection = pDBConnection
            Else
                resultData = GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                End If
            End If

            Return dbConnection
        End Function

        ''' <summary>
        ''' Gets the active analyzer model
        ''' </summary>
        ''' <param name="dbConnection">Connection to db</param>
        ''' <returns>The analyzer model that is currently connected to</returns>
        ''' <remarks>
        ''' Created on 19/03/2015 by AJG
        ''' </remarks>
        Private Function GetActiveAnalyzer(ByVal dbConnection As SqlConnection) As String
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
        ''' Set the semaphore to busy (EXCEPT when called from auto rerun business)
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SetSemaphoreToBusy()
            If GlobalConstants.CreateWSExecutionsWithSemaphore AndAlso pManualRerunFlag Then
                GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Waiting (timeout = " & GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS.ToString & ")", "AnalyzerManager.CreateWSExecutions", EventLogEntryType.Information, False)
                GlobalSemaphores.createWSExecutionsSemaphore.WaitOne(GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS)
                GlobalSemaphores.createWSExecutionsQueue = 1 'Only 1 thread is allowed, so set to 1 instead of increment ++1 'GlobalSemaphores.createWSExecutionsQueue += 1
                GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Passed through, semaphore busy", "AnalyzerManager.CreateWSExecutions", EventLogEntryType.Information, False)
            End If
        End Sub

        Private Function Create(ByVal pDBConnection As SqlConnection, ByVal pExecution As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.Create(dbConnection, pExecution)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then RollbackTransaction(dbConnection)
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
        Private Function DeleteNotInCourseExecutionsNEW(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                      ByVal pOrderTestsListDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSExecutionsDAO
                        resultData = myDAO.DeleteNotInCourseExecutionsNEW(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestsListDS)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.DeleteNotInCourseExecutionsNEW", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        Private Function SearchNotInCourseExecutionsToDelete() As GlobalDataTO
            Dim StartTime = Now
            'Search all Order Tests which Executions can be deleted: those having ALL Executions with status PENDING or LOCKED
            Dim resultData = myDao.SearchNotInCourseExecutionsToDelete(pDBConnection, pAnalyzerID, pWorkSessionID, pOrderTestID)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myOrderTestsToDelete As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                If (myOrderTestsToDelete.twksOrderTests.Count > 0) Then
                    'Delete all Readings of the Executions for all OrderTests returned by the previous called function
                    Dim myReadingsDelegate As New WSReadingsDelegate
                    resultData = myReadingsDelegate.DeleteReadingsForNotInCourseExecutions(pDBConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)

                    If (Not resultData.HasError) Then
                        'Delete the Executions for all OrderTests returned by the previous called function
                        If GlobalConstants.CreateWSExecutionsWithMultipleTransactions Then
                            resultData = DeleteNotInCourseExecutionsNEW(pDBConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)
                        Else
                            resultData = myDao.DeleteNotInCourseExecutionsNEW(pDBConnection, pAnalyzerID, pWorkSessionID, myOrderTestsToDelete)
                        End If
                    End If
                End If
            End If
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Not RUNNING: Search and Delete NOT IN COURSE " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
            Return resultData
        End Function

        Private Function CreateExecutionsOrderTestsNotStarted() As GlobalDataTO
            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
            Dim resultData = myWSOrderTestsDelegate.GetInfoOrderTestsForExecutions(pDBConnection, pAnalyzerID, pWorkSessionID)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                allOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsForExecutionsDS)

                If (allOrderTestsDS.OrderTestsForExecutionsTable.Rows.Count > 0) Then

                    resultData = GetExecutionsForTests()

                    If Not resultData.HasError Then
                        LockAllTestsFromBlanks()
                        LockAllTestsFromCalibrators()
                        If (Not calledForRerun) Then
                            MarkAllTestsSTAT()
                        End If
                        pendingExecutionsDS = MovePendingAndLockedExecutions()
                        If (Not calledForRerun) Then
                            resultData = SortAndManageContaminations()
                        Else
                            resultData = SavePendingExecutions()
                        End If
                        If (Not resultData.HasError) Then
                            resultData = UpdatePausedFlag()
                        End If
                        If (Not resultData.HasError) Then
                            If (calledForRerun) Then
                                resultData = New OrderTestsDelegate().UpdateStatusByOrderTestID(pDBConnection, pOrderTestID, "PENDING")
                            End If
                            If (Not resultData.HasError AndAlso orderTestLockedByLISList.Count > 0) Then
                                resultData = UpdateLocksByLIS()
                            End If
                        End If
                    End If
                End If
            End If
            Return resultData
        End Function

        Private Sub GetExecutionsForBlanks()  'ByRef myBlankExecutionsDS As ExecutionsDS, ByVal allOrderTestsDS As OrderTestsForExecutionsDS)
            Dim StartTime = Now
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
                    rowBlank.ElementID = NullElementID
                    rowBlank.EndEdit()
                End If
            Next
            myBlankExecutionsDS.AcceptChanges()

            GlobalBase.CreateLogActivity("Get Executions For BLANKS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)

        End Sub

        Private Sub GetExecutionsForCalibrators()  'ByRef myCalibratorExecutionsDS As ExecutionsDS, ByVal allOrderTestsDS As OrderTestsForExecutionsDS)
            Dim StartTime = Now
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
            GlobalBase.CreateLogActivity("Get Executions For CALIBRATORS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)

        End Sub

        Private Sub GetExecutionsForControls()  'ByRef myControlExecutionsDS As ExecutionsDS, allOrderTestsDS As OrderTestsForExecutionsDS)
            Dim StartTime = Now
            Dim myOrderTestID As Integer = -1
            Dim controlInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

            For Each rowControl As ExecutionsDS.twksWSExecutionsRow In myControlExecutionsDS.twksWSExecutions
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
            GlobalBase.CreateLogActivity("Get Executions For CONTROLS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)

        End Sub

        Private Sub GetExecutionsForPatients()   'ByRef myPatientExecutionsDS As ExecutionsDS, ByVal allOrderTestsDS As OrderTestsForExecutionsDS)
            Dim StartTime = Now
            Dim myOrderTestID As Integer = -1
            Dim patientInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

            For Each rowPatient As ExecutionsDS.twksWSExecutionsRow In myPatientExecutionsDS.twksWSExecutions
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

                    rowPatient.ElementID = patientInfo(0).CreationOrder 'Convert.ToInt32(patientInfo(0).OrderID.Substring(8, 4))
                    If orderTestLockedByLISList.Contains(rowPatient.OrderTestID) Then rowPatient.ExecutionStatus = "LOCKED" 'AG 25/03/2013 - Locked by LIS not by volume missing
                    rowPatient.EndEdit()
                End If
            Next
            GlobalBase.CreateLogActivity("Get Executions For PATIENTS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
        End Sub

        Private Sub LockTestsFromBlanks(ByVal lstLockedBlanks As List(Of Integer), ByRef myExecutionDS As ExecutionsDS, ByVal IncludeExecution As Boolean)
            For Each lockedBlank In lstLockedBlanks
                Dim myOwnTestId = lockedBlank
                '...LOCK the correspondent OrderTests
                Dim lstLockedTests As List(Of ExecutionsDS.twksWSExecutionsRow)
                If IncludeExecution Then
                    lstLockedTests = (From a In myExecutionDS.twksWSExecutions _
                                      Where a.ExecutionType = "PREP_STD" _
                                    AndAlso a.TestID = myOwnTestId _
                                    AndAlso a.ExecutionStatus <> "LOCKED" _
                                     Select a).ToList()
                Else
                    lstLockedTests = (From a In myExecutionDS.twksWSExecutions _
                                      Where a.TestID = myOwnTestId _
                                    AndAlso a.ExecutionStatus <> "LOCKED" _
                                     Select a).ToList()
                End If

                For Each lockedTest In lstLockedTests
                    lockedTest.BeginEdit()
                    lockedTest.ExecutionStatus = "LOCKED"
                    lockedTest.EndEdit()
                Next
            Next
        End Sub

        Private Sub LockByCalibrators(ByVal lockedCalib As ExecutionsDS.twksWSExecutionsRow, ByVal myOrderTestID As Integer, ByRef myControlExecutionsDS As ExecutionsDS, ByRef myPatientExecutionsDS As ExecutionsDS, ByVal allOrderTestsDS As OrderTestsForExecutionsDS)
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
        End Sub

        Private Sub SetStatFlagToBlkCalCtrl(ByVal blkCalibCtrlRow As OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow,
                                            ByRef myBlankExecutionsDS As ExecutionsDS, ByRef myCalibratorExecutionsDS As ExecutionsDS,
                                            ByRef myControlExecutionsDS As ExecutionsDS)
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
                Dim lstCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                            Where a.OrderTestID = myOrderTestID _
                           Select a).ToList()

                For Each calibrator In lstCalibs
                    calibrator.StatFlag = True
                Next
            ElseIf (blkCalibCtrlRow.SampleClass = "CTRL") Then
                Dim lstCtrls = (From a In myControlExecutionsDS.twksWSExecutions _
                            Where a.OrderTestID = myOrderTestID _
                           Select a).ToList()

                For Each control In lstCtrls
                    control.StatFlag = True
                Next
            End If
        End Sub

        Private Function GetExecutionsForTests() As GlobalDataTO

            'Get all executions for BLANKS included in the WorkSession
            myBlankExecutionsDS = New ExecutionsDS()
            'Get all executions for CALIBRATORS included in the WorkSession
            myCalibratorExecutionsDS = Nothing
            'Get all executions for CONTROLS included in the WorkSession
            myControlExecutionsDS = Nothing
            'Get all executions for PATIENT SAMPLES included in the WorkSession
            myPatientExecutionsDS = Nothing

            Dim resultData = ExecutionsDelegate.CreateBlankExecutions(pDBConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myBlankExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
            End If
            resultData = ExecutionsDelegate.CreateCalibratorExecutions(pDBConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myCalibratorExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
            End If
            resultData = ExecutionsDelegate.CreateControlExecutions(pDBConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myControlExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
            End If
            resultData = ExecutionsDelegate.CreatePatientExecutions(pDBConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pPostDilutionType)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myPatientExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
            End If

            Dim listTask As New List(Of Task)

            listTask.Add(Task.Factory.StartNew(Sub() GetExecutionsForBlanks()))
            listTask.Add(Task.Factory.StartNew(Sub() GetExecutionsForCalibrators()))
            listTask.Add(Task.Factory.StartNew(Sub() GetExecutionsForControls()))
            listTask.Add(Task.Factory.StartNew(Sub() GetExecutionsForPatients()))

            Task.WaitAll(listTask.ToArray())
            listTask.Clear()

            Return resultData
        End Function

        Private Sub LockAllTestsFromBlanks()
            Dim StartTime = Now
            Dim listTask As New List(Of Task)
            Dim lstLockedBlanks As List(Of Integer)
            lstLockedBlanks = (From a In myBlankExecutionsDS.twksWSExecutions _
                              Where a.ExecutionStatus = "LOCKED" _
                             Select a.TestID Distinct).ToList()

            listTask.Add(Task.Factory.StartNew(Sub() LockTestsFromBlanks(lstLockedBlanks, myCalibratorExecutionsDS, False)))
            listTask.Add(Task.Factory.StartNew(Sub() LockTestsFromBlanks(lstLockedBlanks, myControlExecutionsDS, True)))
            listTask.Add(Task.Factory.StartNew(Sub() LockTestsFromBlanks(lstLockedBlanks, myPatientExecutionsDS, True)))

            task.WaitAll(listTask.ToArray())
            listTask.Clear()

            myCalibratorExecutionsDS.AcceptChanges()
            myControlExecutionsDS.AcceptChanges()
            myPatientExecutionsDS.AcceptChanges()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Locks for BLANKS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
        End Sub

        Private Sub LockAllTestsFromCalibrators()
            Dim StartTime = Now
            'Search all locked CALIBRATORS to lock also all Controls and Patient Samples
            'for the same Standard Test and SampleType
            Dim lstLockedCalibs As List(Of ExecutionsDS.twksWSExecutionsRow)
            lstLockedCalibs = (From a In myCalibratorExecutionsDS.twksWSExecutions _
                              Where a.ExecutionStatus = "LOCKED" _
                             Select a Order By a.OrderTestID).ToList()

            Dim myOrderTestID As Integer = -1

            Parallel.ForEach(lstLockedCalibs, Sub(lockedCalib) LockByCalibrators(lockedCalib, myOrderTestID, myControlExecutionsDS, myPatientExecutionsDS, allOrderTestsDS))

            myControlExecutionsDS.AcceptChanges()
            myPatientExecutionsDS.AcceptChanges()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Locks for CALIBRATORS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
        End Sub

        Private Sub MarkAllTestsSTAT()
            Dim StartTime = Now
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

                Parallel.ForEach(lstBlkCalibCtrls, Sub(blkCalibCtrlRow) SetStatFlagToBlkCalCtrl(blkCalibCtrlRow, myBlankExecutionsDS, myCalibratorExecutionsDS, myControlExecutionsDS))

                myBlankExecutionsDS.AcceptChanges()
                myCalibratorExecutionsDS.AcceptChanges()
                myControlExecutionsDS.AcceptChanges()
            Next

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Mark STATS " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
        End Sub

        Private Function MovePendingAndLockedExecutions() As ExecutionsDS
            Dim StartTime = Now
            Dim auxResult As ExecutionsDS
            Dim result As ExecutionsDS

            result = AppendLockedToPending(myBlankExecutionsDS)

            auxResult = AppendLockedToPending(myCalibratorExecutionsDS)
            result.twksWSExecutions.Merge(auxResult.twksWSExecutions)

            auxResult = AppendLockedToPending(myControlExecutionsDS)
            result.twksWSExecutions.Merge(auxResult.twksWSExecutions)

            auxResult = AppendLockedToPending(myPatientExecutionsDS)
            result.twksWSExecutions.Merge(auxResult.twksWSExecutions)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Unify all Pending and Locked " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
            Return result
        End Function

        Private Function AppendLockedToPending(ByVal myExecutionDS As ExecutionsDS) As ExecutionsDS
            Dim auxPending As New ExecutionsDS
            Dim auxLocked As New ExecutionsDS

            For Each elem In myExecutionDS.twksWSExecutions
                If (elem.ExecutionStatus = "PENDING") Then
                    auxPending.twksWSExecutions.ImportRow(elem)
                ElseIf (elem.ExecutionStatus = "LOCKED") Then
                    auxLocked.twksWSExecutions.ImportRow(elem)
                End If
            Next

            auxPending.twksWSExecutions.Merge(auxLocked.twksWSExecutions)
            auxLocked.Clear()

            Return auxPending
        End Function

        Private Function SortAndManageContaminations() As GlobalDataTO

            Dim StartTime = Now
            Dim resultData As New GlobalDataTO()
            If (pendingExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                pendingExecutionsDS.twksWSExecutions.DefaultView.Sort =
                    "StatFlag DESC, SampleClass, ElementID, SampleType, ExecutionType, ExecutionStatus DESC, ReadingCycle DESC"

                ' ReSharper disable once InconsistentNaming
                Dim executionDataDS = pendingExecutionsDS

                Dim sorter = New WSExecutionsSorter(executionDataDS, activeAnalyzer)

                Dim success = sorter.SortByContamination(pDBConnection)
                'If success Then success = sorter.SortByElementGroupTime()
                If success Then success = sorter.SortByGroupContamination(pDBConnection)

                executionDataDS = sorter.Executions

                'Finally, save the sorted PENDING executions
                If (success) Then
                    If (GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                        resultData = Create(pDBConnection, executionDataDS)
                    Else
                        resultData = myDao.Create(pDBConnection, executionDataDS)
                    End If
                Else
                    resultData = New GlobalDataTO
                    resultData.HasError = True
                    resultData.SetDatos = Nothing
                End If
            End If
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Sort and create all PENDING " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)
            Return resultData
        End Function

        Private Function UpdatePausedFlag() As GlobalDataTO
            ' - Update of the Paused flag is performed only when no Running mode
            ' - When working in Running mode, no pending executions are deleted, then SW has only to update status 
            '   (business already performed)
            Dim resultData As New GlobalDataTO

            If (Not calledForRerun AndAlso Not pWorkInRunningMode) Then
                Dim myWSPausedOrderTestsDS As WSPausedOrderTestsDS
                Dim myWSPausedOrderTestsDelegate As New WSPausedOrderTestsDelegate

                resultData = myWSPausedOrderTestsDelegate.ReadByAnalyzerAndWorkSession(pDBConnection, pAnalyzerID, pWorkSessionID)
                If (Not resultData.HasError) Then
                    myWSPausedOrderTestsDS = DirectCast(resultData.SetDatos, WSPausedOrderTestsDS)

                    If (GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                        resultData = UpdatePausedWithMultiTransaction(myWSPausedOrderTestsDS)
                    Else
                        resultData = UpdatePausedWithoutTransaction(myWSPausedOrderTestsDS)
                    End If
                End If
            End If

            Return resultData
        End Function

        Private Function UpdatePositionsRotorSample() As GlobalDataTO
            Dim resultData As GlobalDataTO
            'Read the current content of all positions in Samples Rotor
            Dim rcp_del As New WSRotorContentByPositionDelegate
            resultData = rcp_del.ReadByCellNumber(pDBConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", -1)

            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim samplesRotorDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                'Get only positions with tubes for required WorkSession Elements not marked as Depleted or with not enough volume                
                Dim linqRes = (From a In samplesRotorDS.twksWSRotorContentByPosition _
                      Where Not a.IsElementIDNull _
                        AndAlso a.Status <> "DEPLETED" _
                        AndAlso a.Status <> "FEW" _
                         Select a).ToList

                Dim newStatus As String = ""
                For Each row In linqRes
                    newStatus = row.Status
                    resultData = rcp_del.UpdateSamplePositionStatus(pDBConnection, -1, pWorkSessionID, pAnalyzerID, row.ElementID, _
                                                                    row.TubeContent, 1, newStatus, row.CellNumber)
                Next
            End If

            Return resultData
        End Function

        Private Function UpdateStatus() As GlobalDataTO
            Dim StartTime = Now
            Dim resultData As New GlobalDataTO
            'check if there are Electrodes with wrong Calibration
            If (Not pISEElectrodesList Is Nothing AndAlso pISEElectrodesList.Count > 0) Then
                For Each electrode As String In pISEElectrodesList
                    resultData = New ExecutionsDelegate().UpdateStatusByISETestType(pDBConnection, pWorkSessionID, pAnalyzerID, electrode, "PENDING", "LOCKED")
                    If (resultData.HasError) Then Exit For
                Next
            ElseIf (Not pIsISEModuleReady) Then
                'ISE Module cannot be used; all pending ISE Preparations are LOCKED
                resultData = New ExecutionsDelegate().UpdateStatusByExecutionTypeAndStatus(pDBConnection, pWorkSessionID, pAnalyzerID, "PREP_ISE", "PENDING", "LOCKED")
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Final Processing " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutions", EventLogEntryType.Information, False)

            Return resultData
        End Function

        Private Function RecalculateStatusForNotDeletedExecutions() As GlobalDataTO
            Dim StartTime = Now
            Dim resultData = RecalculateStatusForNotDeletedExecutionsNEW(pDBConnection, pAnalyzerID, pWorkSessionID, pWorkInRunningMode, pPauseMode)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Recalculate Status " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ExecutionsDelegate.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)

            Return resultData
        End Function

        Private Function SavePendingExecutions() As GlobalDataTO
            Dim resultData As GlobalDataTO
            If (GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                resultData = Create(pDBConnection, pendingExecutionsDS)
            Else
                resultData = myDao.Create(pDBConnection, pendingExecutionsDS)
            End If

            Return resultData
        End Function

        Private Function UpdatePausedWithMultiTransaction(ByVal myWSPausedOrderTestsDS As WSPausedOrderTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim myOT As New List(Of Integer)
            Dim myRerun As New List(Of Integer)
            For Each auxRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow In myWSPausedOrderTestsDS.twksWSPausedOrderTests
                If Not auxRow.IsOrderTestIDNull AndAlso Not myOT.Contains(auxRow.OrderTestID) Then
                    myOT.Add(auxRow.OrderTestID)
                    myRerun.Add(auxRow.RerunNumber)
                End If
            Next
            If myOT.Count > 0 AndAlso myOT.Count = myRerun.Count Then
                resultData = New ExecutionsDelegate().UpdatePaused(pDBConnection, myOT, myRerun, True, pAnalyzerID, pWorkSessionID)
            End If
            myOT.Clear()
            myRerun.Clear()
            Return resultData
        End Function

        Private Function UpdatePausedWithoutTransaction(ByVal myWSPausedOrderTestsDS As WSPausedOrderTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim tempExecutionDS As ExecutionsDS

            For Each pausedOTRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow In myWSPausedOrderTestsDS.twksWSPausedOrderTests.Rows
                resultData = myDao.ReadByOrderTestIDAndRerunNumber(pDBConnection, pausedOTRow.OrderTestID, pausedOTRow.RerunNumber)
                If resultData.HasError Then Exit For

                tempExecutionDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                If (tempExecutionDS.twksWSExecutions.Count > 0) Then
                    'If found, then update
                    For Each executionRow As ExecutionsDS.twksWSExecutionsRow In tempExecutionDS.twksWSExecutions.Rows
                        resultData = myDao.UpdatePaused(pDBConnection, True, executionRow.ExecutionID)
                        If (resultData.HasError) Then Exit For
                    Next
                End If
            Next
            Return resultData
        End Function

        Private Function UpdateLocksByLIS() As GlobalDataTO
            Dim resultData As GlobalDataTO

            If (GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                resultData = UpdateLockedByLIS(pDBConnection, orderTestLockedByLISList, True)
            Else
                resultData = myDao.UpdateLockedByLIS(pDBConnection, orderTestLockedByLISList, True)
            End If

            Return resultData
        End Function

        Private Sub FinishTransaction(ByVal resultData As GlobalDataTO, ByVal ppDBConnection As SqlConnection)
            If (Not GlobalConstants.CreateWSExecutionsWithMultipleTransactions) Then
                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (ppDBConnection Is Nothing) Then CommitTransaction(pDBConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    If (ppDBConnection Is Nothing) Then RollbackTransaction(pDBConnection)
                End If
            End If
        End Sub

        Private Sub ReleaseSemaphoreToAvailable()
            If GlobalConstants.CreateWSExecutionsWithSemaphore AndAlso pManualRerunFlag Then
                GlobalSemaphores.createWSExecutionsSemaphore.Release()
                GlobalSemaphores.createWSExecutionsQueue = 0 'Only 1 thread is allowed, so reset to 0 instead of decrement --1 'GlobalSemaphores.createWSExecutionsQueue -= 1
                GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Released, semaphore free", "AnalyzerManager.CreateWSExecutionsMultipleTransactions", EventLogEntryType.Information, False)
            End If
        End Sub

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
        ''' </remarks>
        Private Function RecalculateStatusForNotDeletedExecutionsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
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

                            resultData = New WSStatusRecalculatorForBlanks(myExecutionsDAO, myExecutionsDS, dbConnection, pAnalyzerID, pWorkSessionID).Recalculate()

                            If Not resultData.HasError Then
                                resultData = New WSStatusRecalculatorForCalibs(myExecutionsDAO, myExecutionsDS, dbConnection, pAnalyzerID, pWorkSessionID).Recalculate()
                            End If

                            If Not resultData.HasError Then
                                resultData = New WSStatusRecalculatorForCtrlPatientsWithISE(myExecutionsDAO, myExecutionsDS, dbConnection, pAnalyzerID, pWorkSessionID).Recalculate()
                            End If

                            If Not resultData.HasError Then
                                resultData = New WSStatusRecalculatorForCtrlPatientsSTD(myExecutionsDAO, myExecutionsDS, dbConnection, pAnalyzerID, pWorkSessionID).Recalculate()
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
        ''' Update the executions table for the entry ordertests id informed (affects only the executions with current status PENDING or LOCKED)
        ''' LockedValue = TRUE -> ExecutionStatus = LOCKED, LockedByLIS = 1
        ''' LockedValue = FALSE -> ExecutionStatus = PENDING, LockedByLIS = 0
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAffectedOrderTests"></param>
        ''' <param name="pNewLockedValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/03/2013</remarks>
        Private Function UpdateLockedByLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAffectedOrderTests As List(Of Integer), ByVal pNewLockedValue As Boolean) As GlobalDataTO
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

        Function GetContaminationNumber(calculateinrunning As Boolean, previousReagentID As List(Of Integer), ByVal orderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)) As Integer
            Dim contaminaNumber As Integer = 0
            Dim auxContext = New Context(ContaminationsSpecification)
            auxContext.Steps.Clear()
            If Not calculateinrunning AndAlso previousReagentID IsNot Nothing AndAlso previousReagentID.Any Then
                'Iterate throug last "persistence" elements of PreviousreagentID:
                For i As Integer = Math.Max(0, previousReagentID.Count - ContaminationsSpecification.HighContaminationPersistence) To previousReagentID.Count - 1
                    Dim curStep = New ContextStep(ContaminationsSpecification.DispensesPerStep)
                    curStep(1) = ContaminationsSpecification.CreateDispensing()
                    curStep(1).R1ReagentID = previousReagentID(i)
                    auxContext.Steps.Append(curStep)
                Next
            ElseIf Not calculateinrunning Then
                For i = auxContext.Steps.Range.Minimum To -1
                    Dim curStep = New ContextStep(ContaminationsSpecification.DispensesPerStep)
                    curStep(1) = ContaminationsSpecification.CreateDispensing()
                    curStep(1).KindOfLiquid = IDispensing.KindOfDispensedLiquid.Dummy
                    auxContext.Steps.Append(curStep)
                    'dispense.f()
                Next
            Else
                'Get contents from current REAL context
            End If

            For i As Integer = 0 To orderTests.Count + auxContext.Steps.Range.Maximum '- 1
                Dim myStep As New ContextStep(ContaminationsSpecification.DispensesPerStep)
                Dim dispense = ContaminationsSpecification.CreateDispensing()
                If i < orderTests.Count Then
                    dispense.R1ReagentID = orderTests(i).ReagentID
                Else
                    dispense.KindOfLiquid = IDispensing.KindOfDispensedLiquid.Dummy
                End If
                myStep(1) = dispense
                auxContext.Steps.Append(myStep)
                If auxContext.Steps.IsIndexValid(0) AndAlso auxContext.Steps(0) IsNot Nothing AndAlso auxContext.Steps(0)(1) IsNot Nothing Then
                    Dim result = auxContext.ActionRequiredForDispensing(auxContext.Steps(0)(1))
                    Select Case result.Action
                        Case IContaminationsAction.RequiredAction.Wash
                            contaminaNumber += 1
                        Case IContaminationsAction.RequiredAction.GoAhead, IContaminationsAction.RequiredAction.RemoveRequiredWashing, IContaminationsAction.RequiredAction.Skip
                            'Do nothing
                    End Select
                Else
                    Exit For
                End If
                auxContext.Steps.RemoveFirst()
            Next
            Return contaminaNumber
        End Function

    End Class
End Namespace


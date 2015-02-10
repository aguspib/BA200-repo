Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.BL

    Public Class WSReadingsDelegate

#Region "Methods"

        ''' <summary>
        ''' Return the preparation readings selecting by execution and reaction complete
        ''' 
        ''' In pause mode this method has to calculate the real cycle where R2 was added to preparation
        ''' and generate a readingsDS with:
        '''  o 33 first readings = choose valid readings in interval R1+S
        '''  o 35 last readings = choose valid readings in interval R1+S+R2
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalizerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pReactionComplete"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pUseFirstR1SampleReadings"></param>
        ''' <param name="pDefaultR2IsAdded"></param>
        ''' <param name="pNumberOfReadWithR2"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by DL
        ''' Modified by AG  25/02/2010 (add reactioncomplete parameter) Tested OK
        ''' Modified by GDS 04/03/2010 (add pAnalizerID and pWorkSessionID parameters) Tested OK
        ''' Modified AG 23/10/2013 Task #1347 (change method business and also add parameters AnalyzerModel, TestID, UserFirstR1SampleReadings,...
        ''' </remarks>
        Public Function GetReadingsByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pAnalizerID As String, _
                                                 ByVal pWorkSessionID As String, _
                                                 ByVal pExecutionID As Integer, _
                                                 ByVal pReactionComplete As Boolean, _
                                                 ByVal pAnalyzerModel As String, ByVal pTestID As Integer, ByVal pUseFirstR1SampleReadings As Integer, _
                                                 ByVal pDefaultR2IsAdded As Integer, ByVal pNumberOfReadWithR2 As Integer, ByVal pSwReadingsOffset As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'AG 23/10/2013 Task #1347 - Apply new business, not only read and return readings
                        'Dim mytwksWSReadings As New twksWSReadingsDAO
                        'resultData = mytwksWSReadings.GetReadingsByExecutionID(dbConnection, pAnalizerID, pWorkSessionID, pExecutionID, pReactionComplete)

                        '1. Get all execution readings
                        Dim allReadingsDS As New twksWSReadingsDS
                        If Not resultData.HasError Then
                            Dim mytwksWSReadings As New twksWSReadingsDAO
                            resultData = mytwksWSReadings.GetReadingsByExecutionID(dbConnection, pAnalizerID, pWorkSessionID, pExecutionID, pReactionComplete)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                allReadingsDS = DirectCast(resultData.SetDatos, twksWSReadingsDS)
                            End If
                        End If

                        '2. Calculate the delayed cycles where the R2 has to be added to preparation in 'normal' running mode
                        Dim r2DelayedCycles As Integer = 0 'Delay between the R2 real cycle and the not paused R2 cycle
                        Dim realReadingNumberR2Added As Integer = 0 'Real cycle where the R2 was added to preparation 'byRef parameter
                        If Not resultData.HasError Then
                            resultData = GetR2DelayedCycles(dbConnection, pAnalizerID, pAnalyzerModel, pWorkSessionID, pExecutionID, _
                                                            pDefaultR2IsAdded, pNumberOfReadWithR2, pSwReadingsOffset, realReadingNumberR2Added, pTestID, allReadingsDS)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                r2DelayedCycles = DirectCast(resultData.SetDatos, Integer)
                            End If
                        End If

                        '3. Generate the readings to be returned
                        If Not resultData.HasError Then
                            Dim startingIndex As Integer = 0
                            Dim endingIndex As Integer = 0
                            Dim finalReadingsDS As New twksWSReadingsDS

                            'No delayed due to 'pause' running mode. Return allReadings
                            If r2DelayedCycles = 0 Then
                                'AG 24/10/2013 - not copy because allReadings could contains lots of readings instead of 68
                                'Import only readings from 3 to RowsCount (with maximum as 70)
                                'resultData.SetDatos = allReadingsDS 
                                startingIndex = 0
                                endingIndex = allReadingsDS.twksWSReadings.Rows.Count - 1 '('-1' because index goes from 0 to N-1)
                                If endingIndex > startingIndex + pDefaultR2IsAdded + pNumberOfReadWithR2 - 2 Then
                                    endingIndex = startingIndex + pDefaultR2IsAdded + pNumberOfReadWithR2 - 2 '('-1' because index goes from 0 to N-1 + '-1' because the parameter informs the first cycle with R2)
                                End If

                                For index As Integer = startingIndex To endingIndex
                                    If index <= allReadingsDS.twksWSReadings.Rows.Count - 1 Then
                                        finalReadingsDS.twksWSReadings.ImportRow(allReadingsDS.twksWSReadings(index))
                                    Else
                                        Exit For
                                    End If
                                Next

                                'R2 has been delayed. Return the proper readings
                            Else
                                'Dim internalReadingsOffset As Integer = 2 'Related cycle machine with test programming cycles
                                'Dim paramsDelg As New SwParametersDelegate
                                'resultData = paramsDelg.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.SW_READINGSOFFSET.ToString, pAnalyzerModel)
                                'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                '    internalReadingsOffset = CInt(CType(resultData.SetDatos, ParametersDS).tfmwSwParameters(0).ValueNumeric)
                                'End If

                                If Not resultData.HasError Then
                                    'First import the readings for interval R1+S
                                    If pUseFirstR1SampleReadings = 1 Then
                                        'Get the first 33 readings for R1+S (ReadingNumber 3 to 35)
                                        startingIndex = 0
                                        endingIndex = pDefaultR2IsAdded - 2 '('-1' because index goes from 0 to N-1 + '-1' because the parameter informs the first cycle with R2)
                                    Else
                                        'Get the last 33 readings for R1+S (ReadingNumber (3 + R2Delayed) to (35+R2Delayed))
                                        startingIndex = r2DelayedCycles
                                        endingIndex = pDefaultR2IsAdded + r2DelayedCycles - 2 '('-1' because index goes from 0 to N-1 + '-1' because the parameter informs the first cycle with R2)
                                    End If

                                    For index As Integer = startingIndex To endingIndex
                                        If index <= allReadingsDS.twksWSReadings.Rows.Count - 1 Then
                                            finalReadingsDS.twksWSReadings.ImportRow(allReadingsDS.twksWSReadings(index))
                                        Else
                                            Exit For
                                        End If
                                    Next


                                    'Import the readings for interval R1+S+R2
                                    startingIndex = realReadingNumberR2Added - 1 '('-1' because index goes from 0 to N-1)
                                    endingIndex = allReadingsDS.twksWSReadings.Rows.Count - 1 '('-1' because index goes from 0 to N-1)
                                    If endingIndex > startingIndex + pNumberOfReadWithR2 - 1 Then
                                        endingIndex = startingIndex + pNumberOfReadWithR2 - 1 '('-1' because index goes from 0 to N-1)
                                    End If

                                    For index As Integer = startingIndex To endingIndex
                                        If index <= allReadingsDS.twksWSReadings.Rows.Count - 1 Then
                                            finalReadingsDS.twksWSReadings.ImportRow(allReadingsDS.twksWSReadings(index))
                                        Else
                                            Exit For
                                        End If
                                    Next
                                End If

                            End If

                            finalReadingsDS.twksWSReadings.AcceptChanges()
                            resultData.SetDatos = finalReadingsDS
                        End If
                        'AG 23/10/2013 Task #1347

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.GetReadingsByExecutionID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Save readings
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTO with set data as twksWSReadingsDS</returns>
        ''' <remarks>
        ''' Created by GDS 04/03/2010
        ''' Modified AG 22/11/2013 - Task #1397 - in case readings exist do nothing, do not call the Update method in DAO
        ''' </remarks>
        Public Function SaveReadings(ByVal pDBConnection As SqlClient.SqlConnection, _
                                     ByVal ptwksWSReadingsDS As twksWSReadingsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                Dim mytwksWSReadingsDAO As New twksWSReadingsDAO

                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        For Each myRow As twksWSReadingsDS.twksWSReadingsRow In ptwksWSReadingsDS.twksWSReadings.Rows
                            resultData = mytwksWSReadingsDAO.ExistsReading(dbConnection, myRow)

                            If (Not resultData.HasError) Then
                                If DirectCast(resultData.SetDatos, twksWSReadingsDS).twksWSReadings.Rows.Count > 0 Then
                                    'AG 22/11/2013 - Task #1397
                                    'resultData = mytwksWSReadingsDAO.Update(dbConnection, myRow)
                                Else
                                    resultData = mytwksWSReadingsDAO.Insert(dbConnection, myRow)
                                End If
                            End If

                            If resultData.HasError Then
                                Exit For
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
                If (pDBConnection Is Nothing) And _
                   (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.SaveReadings", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, _
                                ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReadingsDAO

                        resultData = myDAO.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.ResetWS", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo</returns>
        ''' <remarks>
        ''' Created by DL 27/05/2010 (tested pending)
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlConnection, _
                                         ByVal pWorkSessionID As String, _
                                         ByVal pAnalyzerID As String, _
                                         Optional ByVal pExecutionID As Integer = -1) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSReadingsDAO As New twksWSReadingsDAO
                        resultData = mytwksWSReadingsDAO.GetByWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.GetByWorkSession", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function


        ' ''' <summary>
        ' ''' Get all readings for an specific preparation and validate all readings are valid
        ' ''' (Valid readings means != SATURATED_READING and != READING_ERROR
        ' ''' 
        ' ''' (In case calibrator multiitem check readings for all executions belongs the same OrderTestID - RerunNumber)
        ' ''' </summary>
        ' ''' <param name="pDBConnection"></param>
        ' ''' <param name="pAnalyzerID"></param>
        ' ''' <param name="pWorkSessionID"></param>
        ' ''' <param name="pExecutionID"></param>
        ' ''' <returns>GlobalDataTO with Setdata as boolean</returns>
        ' ''' <remarks>
        ' ''' Created by AG 08/06/2010 (Tested OK)
        ' ''' Modified AG 23/10/2013 Method commented, it is not used. Task #1347
        ' ''' </remarks>
        'Public Function ValidateByExecutionID(ByVal pDBConnection As SqlConnection, _
        '                                     ByVal pAnalyzerID As String, _
        '                                     ByVal pWorkSessionID As String, _
        '                                     ByVal pExecutionID As Integer) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        Dim validateResult As Boolean = True

        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                '1st: Get (if exists) others related executions (multi item executions)
        '                Dim myExDelgt As New ExecutionsDelegate
        '                Dim executionsDS As New ExecutionsDS

        '                resultData = myExDelgt.GetExecutionMultititem(dbConnection, pExecutionID)
        '                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
        '                    executionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

        '                    For Each exeRow As ExecutionsDS.twksWSExecutionsRow In executionsDS.twksWSExecutions.Rows
        '                        'Get readings by Executions
        '                        Dim readingsDS As twksWSReadingsDS

        '                        resultData = Me.GetReadingsByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, exeRow.ExecutionID, True)
        '                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
        '                            readingsDS = DirectCast(resultData.SetDatos, twksWSReadingsDS)

        '                            'Check if there are some invalid read (SATURATED_READING or READING_ERROR)
        '                            For Each row As twksWSReadingsDS.twksWSReadingsRow In readingsDS.twksWSReadings.Rows
        '                                If row.MainCounts = GlobalConstants.SATURATED_READING Or row.MainCounts = GlobalConstants.READING_ERROR Then validateResult = False
        '                                If row.RefCounts = GlobalConstants.SATURATED_READING Or row.RefCounts = GlobalConstants.READING_ERROR Then validateResult = False
        '                                If Not validateResult Then Exit For
        '                            Next
        '                        End If
        '                        If Not validateResult Then Exit For
        '                    Next

        '                End If

        '            End If
        '        End If

        '        If resultData.HasError Then validateResult = False
        '        resultData.SetDatos = validateResult

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.ValidateByExecutionID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function


        Public Function GetPreviousReading(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReadingRow As twksWSReadingsDS.twksWSReadingsRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReadingsDAO
                        resultData = myDAO.ExistsReading(dbConnection, pReadingRow, True)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.GetPreviousReading", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Basic read
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pReactionComplete"></param>
        ''' <returns>GlobalDataTo (ReadingsDS as data)</returns>
        ''' <remarks>AG 22/11/2013 - Task #1391</remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pExecutionID As Integer, ByVal pReactionComplete As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReadingsDAO
                        resultData = myDAO.GetReadingsByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, pReactionComplete)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.Read", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "NEW METHODS"
        ''' <summary>
        ''' Insert all Readings received for an specific Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSReadingsDS">Typed DataSet twksWSReadingsDS containing all Readings to insert</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/07/2012 
        ''' </remarks>
        Public Function SaveReadingsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSReadingsDS As twksWSReadingsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSReadingsDAO As New twksWSReadingsDAO
                        resultData = mytwksWSReadingsDAO.InsertNEW(dbConnection, pWSReadingsDS)

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
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.SaveReadingsNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete Readings of all Executions for a group of Order Tests that fulfill the following condition: ALL their Executions 
        ''' have Execution Status PENDING or LOCKED 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsListDS">Typed DataSet OrderTestsDS containing the group of OrderTests having ALL their Executions 
        '''                                 with status PENDING or LOCKED. When it is not informed, readings for all LOCKED Executions are deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA  31/07/2012
        ''' </remarks>
        Public Function DeleteReadingsForNotInCourseExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                              ByVal pOrderTestsListDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSReadingsDAO As New twksWSReadingsDAO

                        If (Not pOrderTestsListDS Is Nothing) Then
                            'Delete Readings when the Analyzer is in STANDBY mode
                            resultData = mytwksWSReadingsDAO.DeleteReadingsForNotInCourseExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestsListDS)
                        Else
                            'Delete Readings when the Analyzer is in RUNNING mode
                            resultData = mytwksWSReadingsDAO.DeleteReadingsForLockedExecutions(dbConnection, pAnalyzerID, pWorkSessionID)
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
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.DeleteReadingsForNotInCourseExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete readings by Execution - Worksession - Analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>AG 02/10/2012</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pExecutionDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReadingsDAO
                        resultData = myDAO.Delete(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionDS)

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
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' This function determinates what is the first reading cycle with R2, we create method GetR2DelayedCycles (in WSReadingsDelegate)
        ''' In normal running mode there are 68 readings:
        ''' - 33 firsts (3 to 35) belongs to R1+S
        ''' - 35 lasts (36 to 70) belongs to R1+S+R2
        ''' 
        ''' In pause mode the executions could have more than 68 readings and Software has to decide which are taken into account for calculations and which are rejected
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pRealReadingNumberCycleR2IsAdded"></param>
        ''' <param name="pDefaultReadNumberR2IsAdded"></param>
        ''' <param name="pMaxR2Readings"></param>
        ''' <param name="pSwReadingsOffset"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pExecutionReadingsDS"></param>
        ''' <returns>GlobalDataTo (integer with the R2DelayOffset - n. of cycles the R2 was added to preparation later than the normal running mode</returns>
        ''' <remarks>AG 23/10/2013 Creation. Task #1347
        ''' Modified AG 04/11/2013. Task #1376
        ''' Modified AG 27/11/2013 - #1391 (in order to use this method into the critical reagents into pause mode we must reset variable pRealReadingNumberCycleR2IsAdded at the end of the function
        '''                                 for the monoreagent tests)
        ''' Modified AG 05/02/2014 - #1492 (when several pauses find the correct cycle where R2 was added, skip pause blocks when count cycles!!)</remarks>
        Public Function GetR2DelayedCycles(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String, ByVal pWorkSessionID As String, ByVal pExecutionID As Integer, _
                                           ByVal pDefaultReadNumberR2IsAdded As Integer, ByVal pMaxR2Readings As Integer, ByVal pSwReadingsOffset As Integer, ByRef pRealReadingNumberCycleR2IsAdded As Integer, _
                                           ByVal pTestID As Integer, ByVal pExecutionReadingsDS As twksWSReadingsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Define and initiate internal variables
                        Dim r2DelayCycles As Integer = 0
                        Dim realReadNumberR2Added As Integer = pDefaultReadNumberR2IsAdded 'Real cycle where the R2 was added to preparation

                        'If analysis mode not informed 
                        Dim analysisMode As String = ""

                        Dim testsDlg As New TestsDelegate
                        resultData = testsDlg.Read(dbConnection, pTestID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            analysisMode = DirectCast(resultData.SetDatos, TestsDS).tparTests.First.AnalysisMode
                        End If

                        Dim bireagentTestFlag As Boolean = True 'AG 27/11/2013 - #1391
                        If Not resultData.HasError Then
                            'Monoreagents analysis mode has no problem
                            Select Case analysisMode
                                Case "MREP", "MRFT", "MRK" 'Monoreagents analysis modes
                                    'Nothing to do, use the variables with their default values
                                    bireagentTestFlag = False 'AG 27/11/2013 - #1391

                                Case Else 'The bireagents

                                    'If readings not informed get them
                                    Dim myReadingsDS As New twksWSReadingsDS
                                    If pExecutionReadingsDS Is Nothing Then
                                        Dim myDAO As New twksWSReadingsDAO
                                        resultData = myDAO.GetReadingsByExecutionID(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, True)
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            myReadingsDS = DirectCast(resultData.SetDatos, twksWSReadingsDS)
                                        End If
                                    Else
                                        myReadingsDS = pExecutionReadingsDS
                                    End If

                                    'Search the first ReadingNumber with reading pause
                                    Dim lnqList As New List(Of twksWSReadingsDS.twksWSReadingsRow)
                                    Dim firstReadPaused As Integer = 0
                                    lnqList = (From a As twksWSReadingsDS.twksWSReadingsRow In myReadingsDS.twksWSReadings _
                                              Where a.Pause = True Select a Order By a.ReadingNumber Ascending).ToList

                                    Dim pausedCycles As Integer = lnqList.Count 'AG 05/02/2014 - BT #1492
                                    If lnqList.Count > 0 Then
                                        firstReadPaused = lnqList.First.ReadingNumber
                                    End If
                                    'lnqList = Nothing 'AG 05/02/2014 - BT #1492

                                    'ONLY If paused before R2 was added use next loop for calculate the real cycle were R2 was added to preparation
                                    If firstReadPaused > 0 AndAlso (firstReadPaused <= pDefaultReadNumberR2IsAdded + pSwReadingsOffset) Then
                                        'AG 05/02/2014 - BT #1492 - Comment the loop
                                        ''Define control variables
                                        'Dim totalCounter As Integer = 0
                                        'Dim r2Counter As Integer = 0
                                        'Dim pausedFlag As Boolean = False

                                        'For Each row As twksWSReadingsDS.twksWSReadingsRow In myReadingsDS.twksWSReadings
                                        '    totalCounter += 1
                                        '    If row.Pause Then
                                        '        pausedFlag = True 'the r2 counter will be increased just before return to not paused mode
                                        '    Else
                                        '        'If previous reading was in pause mode, update internal flag and increase the r2counter cycles
                                        '        If pausedFlag Then
                                        '            pausedFlag = False
                                        '            r2Counter += 1
                                        '        End If

                                        '        r2Counter += 1 'Increase R2counter cycles

                                        '        'Once we have group the readings in pause mode the r2counter matches the defaul cycle where R2 has to added
                                        '        If r2Counter > pDefaultReadNumberR2IsAdded Then 'AG 04/11/2013 - If r2Counter >= pDefaultReadNumberR2IsAdded Then
                                        '            realReadNumberR2Added = totalCounter
                                        '            Exit For
                                        '        End If
                                        '    End If
                                        'Next

                                        ''This case means the preparation is still in pause and R2 not added. Return the max value
                                        'If r2Counter <= pDefaultReadNumberR2IsAdded Then 'AG 04/11/2013 - If r2Counter < pDefaultReadNumberR2IsAdded Then
                                        '    realReadNumberR2Added = totalCounter
                                        'End If

                                        'BT #1492 - New code
                                        lnqList = (From a As twksWSReadingsDS.twksWSReadingsRow In myReadingsDS.twksWSReadings _
                                                   Where a.Pause = False Select a Order By a.ReadingNumber Ascending).ToList
                                        'Note condition must be '>=' 
                                        If lnqList.Count >= pDefaultReadNumberR2IsAdded Then
                                            realReadNumberR2Added = lnqList(pDefaultReadNumberR2IsAdded - 1).ReadingNumber - pSwReadingsOffset
                                        Else
                                            'This case means the preparation is still in pause and R2 not added. Return the max reading number + 1 (in order to not launch calculations)
                                            'realReadNumberR2Added = myReadingsDS.twksWSReadings.Rows.Count 'NOK, fails!!!!
                                            realReadNumberR2Added = myReadingsDS.twksWSReadings.Rows.Count + 1
                                        End If
                                        'AG 05/02/2014 - BT #1492

                                    End If

                                    lnqList = Nothing 'AG 05/02/2014 - BT #1492
                            End Select

                            'Update the parameters passed byRef
                            pRealReadingNumberCycleR2IsAdded = realReadNumberR2Added

                            'Calculate delayed cycles R2 was added
                            r2DelayCycles = realReadNumberR2Added - pDefaultReadNumberR2IsAdded
                            resultData.SetDatos = r2DelayCycles

                            'AG 27/11/2013 - #1391 - For the monoreagent tests set this local variable to 0 because it will be used when call from ExecutionsDelegate.ExistCriticalPauseTests
                            'Do it here once the variables to return have been calculated
                            If Not bireagentTestFlag Then pRealReadingNumberCycleR2IsAdded = 0

                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSReadingsDelegate.GetR2DelayedCycles", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


#End Region
    End Class

End Namespace

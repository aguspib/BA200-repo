Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class twksWSExecutionsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Create a group of Executions in table twksWSExecutions
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pExecution">Typed Dataset ExecutionsDS with all Executions to create</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 15/04/2010 - Tested: OK
        ''' Modified by: SA 21/04/2010 - Removed field SortCriteria
        '''              AG 03/05/2010 - RotorTurnNumber is deleted from database
        '''              RH 06/08/2010 - Added nextExecutionID
        '''              AG 03/01/2011 - Is not necessary add new field AdjustBaseLineID due has NULL default value
        '''              SA 09/02/2011 - Changed the function to send to DB several Executions in a script (to improve the speed of 
        '''                              process of executions creation)
        '''              TR 04/08/2011 - Implemented Using sentence; to clean the String Builder, instead of using Remove, set Lenght=0
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecution As ExecutionsDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO()
            Dim nextExecutionID As Integer

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()

                ElseIf (Not pExecution Is Nothing) Then
                    'Generate the next ID...
                    dataToReturn = GenerateExecutionID(pDBConnection)
                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                        nextExecutionID = DirectCast(dataToReturn.SetDatos, Integer)

                        Dim i As Integer = 0
                        Dim maxInserts As Integer = 300
                        Dim cmdText As New StringBuilder()
                        For Each row As ExecutionsDS.twksWSExecutionsRow In pExecution.twksWSExecutions.Rows
                            cmdText.Append("INSERT INTO twksWSExecutions")
                            cmdText.Append("(ExecutionID, WorkSessionID, AnalyzerID, OrderTestID, MultiItemNumber, RerunNumber, " & _
                                            "ExecutionStatus, ExecutionType, StatFlag, PostDilutionType, ReplicateNumber, BaseLineID, " & _
                                            "WellUsed, ABS_Value, ABS_Error, rKinetics, KineticsLinear, KineticsInitialValue, KineticsSlope, " & _
                                            "SubstrateDepletion, ABS_Initial, ABS_MainFilter, CONC_Value, CONC_CurveError, " & _
                                            "CONC_Error, InUse, ResultDate, SampleClass, PreparationID) VALUES(")

                            cmdText.AppendFormat("{0}, '{1}', '{2}', {3}, {4}, {5}, '{6}', '{7}', {8} ", nextExecutionID, row.WorkSessionID, row.AnalyzerID, _
                                                 row.OrderTestID, row.MultiItemNumber, row.RerunNumber, row.ExecutionStatus, row.ExecutionType, _
                                                 IIf(row.StatFlag, 1, 0))

                            'Control of values of not required fields
                            If (row.IsPostDilutionTypeNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", '{0}'", row.PostDilutionType)
                            End If

                            If (row.IsReplicateNumberNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", row.ReplicateNumber)
                            End If

                            If (row.IsBaseLineIDNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", row.BaseLineID)
                            End If

                            If (row.IsWellUsedNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", row.WellUsed)
                            End If

                            If (row.IsABS_ValueNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.ABS_Value))
                            End If

                            If (row.IsABS_ErrorNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", '{0}'", row.ABS_Error)
                            End If

                            If (row.IsrkineticsNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.rkinetics))
                            End If

                            If (row.IsKineticsLinearNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", IIf(row.KineticsLinear, 1, 0))
                            End If

                            If (row.IsKineticsInitialValueNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.KineticsInitialValue))
                            End If

                            If (row.IsKineticsSlopeNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.KineticsSlope))
                            End If

                            If (row.IsSubstrateDepletionNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", IIf(row.SubstrateDepletion, 1, 0))
                            End If

                            If (row.IsABS_InitialNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.ABS_Initial))
                            End If

                            If (row.IsABS_MainFilterNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.ABS_MainFilter))
                            End If

                            If (row.IsCONC_ValueNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.CONC_Value))
                            End If

                            If (row.IsCONC_CurveErrorNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", ReplaceNumericString(row.CONC_CurveError))
                            End If

                            If (row.IsCONC_ErrorNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", '{0}'", row.CONC_Error)
                            End If

                            If (row.IsInUseNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", IIf(row.InUse, 1, 0))
                            End If

                            If (row.IsResultDateNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", '{0}'", row.ResultDate.ToString("MM/dd/yyyy"))
                            End If

                            If (row.IsSampleClassNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", '{0}'", row.SampleClass.Trim())
                            End If

                            If (row.IsPreparationIDNull) Then
                                cmdText.Append(", NULL")
                            Else
                                cmdText.AppendFormat(", {0}", row.PreparationID)
                            End If

                            'Add the final parenthesis and increment the value for the next ExecutionID
                            cmdText.Append(") " & vbCrLf)
                            nextExecutionID += 1

                            'Increment the sentences counter and verify if the max has been reached
                            i += 1
                            If (i = maxInserts) Then
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                    dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                                    cmdText.Length = 0
                                    i = 0
                                End Using

                            End If
                        Next

                        If (Not dataToReturn.HasError) Then
                            If (cmdText.Length > 0) Then
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), pDBConnection)
                                    dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                                    cmdText.Length = 0
                                End Using
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update executions table with the PreparationID fields (if informed) and the ExecutionStatus field (if informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by:  AG 20/04/2010 
        ''' Modified by: AG 04/02/2011 - Implement DS.rows loop instead using only row 0 
        '''              AG 21/10/2011 - Update also fields InUse and ResultDate when they are informed
        '''              SA 06/07/2012 - Changes to sent all Updates together (add all updates to an StringBuilder and sent all to
        '''                              SQL Server in the same command
        ''' </remarks>
        ''' 
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As New StringBuilder()
                    For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions
                        cmdText.AppendFormat(" UPDATE twksWSExecutions SET ExecutionStatus = '{0}' ", row.ExecutionStatus)

                        If (Not row.IsPreparationIDNull) Then cmdText.AppendFormat(", PreparationID = {0} ", row.PreparationID)
                        If (Not row.IsInUseNull) Then cmdText.AppendFormat(", InUse = {0} ", IIf(row.InUse, 1, 0).ToString)
                        If (Not row.IsResultDateNull) Then cmdText.AppendFormat(", ResultDate = '{0:yyyyMMdd HH:mm:ss}' ", row.ResultDate)

                        cmdText.AppendFormat(" WHERE ExecutionID = {0} ", row.ExecutionID)
                        cmdText.Append(vbCrLf)
                    Next

                    If (cmdText.Length > 0) Then
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    End If
                    cmdText.Length = 0
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update executions table with the PreparationID fields (if informed) and the ExecutionStatus field (if informed)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewExecutionStatus">new execution status</param>
        ''' <param name="pOrderTestId">order test identifier</param>
        ''' <param name="pExecutionStatus">execution status</param>
        ''' 
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation DL 05/01/2011
        ''' AG 31/01/2012 - add pPreparationId parameter and when informed lock also all ordertest INPROCESS executions where preparationID > parameter pPreparationID
        ''' AG 03/10/2012 - new optional parameter used for lock process (pLockReplicateNumber) used only in multiitem calibrators 
        ''' </remarks>
        Public Function UpdateStatusByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                ByVal pNewExecutionStatus As String, _
                                                ByVal pOrderTestId As Integer, _
                                                ByVal pExecutionStatus As String, Optional ByVal pLockPreparationIDHigherThanThis As Integer = -1, _
                                                Optional ByVal pLockMultiItemWithReplicateNumber As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    Dim cmdText As String = ""
                    cmdText &= "UPDATE twkswsexecutions" & vbCrLf
                    cmdText &= "   SET ExecutionStatus = '" & pNewExecutionStatus & "'" & vbCrLf
                    cmdText &= " WHERE OrderTestID = " & pOrderTestId & vbCrLf
                    cmdText &= "   AND ( ExecutionStatus = '" & pExecutionStatus & "'"

                    'AG 31/01/2012
                    If pLockPreparationIDHigherThanThis <> -1 Then
                        cmdText &= "   OR ( ExecutionStatus = 'INPROCESS' AND PreparationID > " & pLockPreparationIDHigherThanThis & ")"
                    End If
                    'AG 31/01/2012

                    'AG 03/11/2012
                    If pLockMultiItemWithReplicateNumber <> -1 Then
                        cmdText &= "   OR ( ExecutionStatus = 'INPROCESS' AND ReplicateNumber = " & pLockMultiItemWithReplicateNumber & ")"
                    End If
                    'AG 03/10/2012

                    cmdText &= ") " 'AG 31/01/2012

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateStatusByOrderTest", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update executions table with the PreparationID fields (if informed) and the ExecutionStatus field (if informed)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewExecutionStatus">new execution status</param>
        ''' <param name="pExecutionId">order test identifier</param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation DL 05/01/2011
        ''' AG 31/03/2011 - add pWorkSessionID and pAnalyzerId parameters
        ''' </remarks>
        Public Function UpdateStatusByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                  ByVal pNewExecutionStatus As String, _
                                                  ByVal pExecutionId As Integer, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    Dim cmdText As String = ""
                    cmdText &= "UPDATE twkswsexecutions" & vbCrLf
                    cmdText &= "   SET ExecutionStatus = '" & pNewExecutionStatus & "'" & vbCrLf
                    cmdText &= " WHERE ExecutionID = " & pExecutionId.ToString
                    cmdText &= " AND WorkSessionID = '" & pWorkSessionID.ToString & "' "
                    cmdText &= " AND AnalyzerID = '" & pAnalyzerID.ToString & "' "

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        If (resultData.AffectedRecords = 1) Then
                            resultData.HasError = False
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        End If
                    End Using
                    'AG 25/07/2014
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateStatusByExecutionID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get the next Execution Identifier (by getting the last generated and adding one to it)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value with the next Execution Identifier</returns>
        ''' <remarks>
        ''' Created by:  SA 20/04/2010
        ''' Modified by: SA 24/10/2011 - Changed the function template
        ''' </remarks>
        Public Function GenerateExecutionID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search the last created ExecutionID
                        Dim cmdText As String = " SELECT MAX(ExecutionID) AS NextExecutionID " & vbCrLf & _
                                                " FROM   twksWSExecutions "

                        'Execute the SQL sentence 
                        Dim dbDataReader As SqlClient.SqlDataReader
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("NextExecutionID")) + 1
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GenerateExecutionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the informed OrderTestID included in an Analyzer WorkSession, get the next rerun number to 
        ''' insert as execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the rerun number to insert as Execution for the 
        '''          informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by:  SA 20/04/2010
        ''' Modified by: SA 24/10/2011 - Changed the function template
        ''' </remarks>
        Public Function GetOrderTestRerunNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search the last created ExecutionID
                        Dim cmdText As String = " SELECT MAX(RerunNumber) AS NextRerunNumber " & vbCrLf & _
                                                " FROM   twksWSExecutions " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    OrderTestID = " & pOrderTestID

                        'Execute the SQL sentence 
                        Dim dbDataReader As SqlClient.SqlDataReader
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("NextRerunNumber")) + 1
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetOrderTestRerunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is at least a not locked execution of the informed SampleClass for the specified 
        ''' TestID/SampleType in the Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class of the Execution to verify: BLANK or CALIB</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if there is at least a not locked execution of the informed
        '''          SampleClass for the specified TestID/SampleType in the Analyzer Work Session</returns>
        ''' <remarks>
        ''' Created by:  SA 20/04/2010
        ''' Modified by: SA 24/10/2011 - Changed the function template
        ''' </remarks>
        Public Function VerifyUnlockedExecution(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                ByVal pSampleClass As String, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT E.* " & vbCrLf & _
                                                " FROM   twksWSExecutions E INNER JOIN vwksWSOrderTests WSOT ON E.AnalyzerID = WSOT.AnalyzerID AND " & vbCrLf & _
                                                                                                              " E.WorkSessionID = WSOT.WorkSessionID AND " & vbCrLf & _
                                                                                                              " E.OrderTestID = WSOT.OrderTestID " & vbCrLf & _
                                                " WHERE  WSOT.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    WSOT.SampleClass = '" & pSampleClass & "' " & vbCrLf & _
                                                " AND    WSOT.TestID = " & pTestID & vbCrLf & _
                                                " AND    E.ExecutionStatus <> 'LOCKED' " & vbCrLf

                        If (pSampleClass <> "BLANK") Then
                            cmdText &= " AND WSOT.SampleType = '" & pSampleType & "' "
                        End If

                        'Execute the SQL sentence
                        Dim myExecutionDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myExecutionDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = (myExecutionDS.twksWSExecutions.Rows.Count > 0)
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.VerifyUnlockedExecution", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information of the WS Element required for the informed Execution. By default, this function returns only information about 
        ''' Sample Tube Elements, but it can return also information about the Reagents if parameter pAlsoReagentElementInfo is TRUE
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pAlsoReagentElementInfo">When TRUE, besides the Sample Tube Elements, the funcion returns also the required Reagents</param>
        ''' <param name="pRealMultiItemNumber">when informed (special test HbTotal) use the parameter value pRealMultiItemNumber instead of E.MultiItemNumber</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with data of the WS Element required for the informed Execution</returns>
        ''' <remarks>
        ''' Created by: SA 18/01/2012
        '''             AG 08/03/2012 - Changed the query to return also field ElementStatus; added new parameter pAlsoReagentElementInfo to allow return also 
        '''                             the required Reagents 
        '''             AG 24/04/2012 - Added optional parameter pRealMultiItemNumber; when informed (only for special Test HbTotal), the MultiItemNumber to get
        '''                             will be used instead of E.MultiItemNumber (when SampleClass = CALIB)
        ''' </remarks>
        Public Function GetElementInfoByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                    ByVal pExecutionID As Integer, ByVal pAlsoReagentElementInfo As Boolean, _
                                                    Optional ByVal pRealMultiItemNumber As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT E.SampleClass, E.OrderTestID, RE.ElementID, RE.ElementStatus, E.MultiItemNumber As MultiTubeNumber " & vbCrLf & _
                                                " FROM   twksWSExecutions E INNER JOIN twksWSRequiredElemByOrderTest REOT ON E.OrderTestID = REOT.OrderTestID " & vbCrLf & _
                                                                          " INNER JOIN twksWSRequiredElements RE ON REOT.ElementID = RE.ElementID " & vbCrLf & _
                                                " WHERE  E.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    E.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    E.ExecutionID = " & pExecutionID & vbCrLf & _
                                                " AND "

                        If (pAlsoReagentElementInfo) Then cmdText += " ( "
                        If (pRealMultiItemNumber = -1) Then
                            'Code for all Tests excepting the special ones
                            cmdText += "       (E.SampleClass <> 'BLANK' AND RE.TubeContent = E.SampleClass " & vbCrLf & _
                                       " OR     E.SampleClass = 'BLANK'  AND RE.TubeContent = 'TUBE_SPEC_SOL') " & vbCrLf & _
                                       " AND   (E.SampleClass = 'CALIB'  AND E.MultiItemNumber = RE.MultiItemNumber " & vbCrLf & _
                                       " OR     E.SampleClass <> 'CALIB' AND RE.MultiItemNumber IS NULL) " & vbCrLf
                        Else
                            'Code for special Tests
                            cmdText += "       (E.SampleClass <> 'BLANK' AND RE.TubeContent = E.SampleClass " & vbCrLf & _
                                       " OR     E.SampleClass = 'BLANK'  AND RE.TubeContent = 'TUBE_SPEC_SOL') " & vbCrLf & _
                                       " AND   (E.SampleClass = 'CALIB'  AND " & pRealMultiItemNumber & " = RE.MultiItemNumber " & vbCrLf & _
                                       " OR     E.SampleClass <> 'CALIB' AND RE.MultiItemNumber IS NULL) " & vbCrLf
                        End If
                        If (pAlsoReagentElementInfo) Then cmdText += " OR     RE.TubeContent = 'REAGENT' )"

                        Dim myElementInfoDS As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myElementInfoDS.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        resultData.SetDatos = myElementInfoDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetElementInfoByExecutionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the Calibrator to which corresponds the specified Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS containing data of the Calibrator to which corresponds 
        '''          the specified Execution</returns>
        ''' <remarks>
        ''' Created by : DL 23/02/2010
        ''' Modified by: DL 26/02/2010
        '''              SA 30/08/2010 - Function name changed from GetCalibratorID to GetCalibratorData; changed the query and the typed DataSet returned. 
        '''                              Besides the CalibratorID, the corresponding function in the Delegate needs fields NumberOfCalibrators, SpecialCalib, 
        '''                              TestID and SampleType
        '''              AG 31/08/2010 - Fix error in SA 30/08/2010 Query (not Executions ... is ExecutionID)
        '''              AG 08/11/2010 - SQL return also ExpirationDate
        '''              SA 09/03/2012 - Changed the function template
        ''' </remarks>
        Public Function GetCalibratorData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT C.CalibratorID, C.NumberOfCalibrators, C.SpecialCalib, TC.TestID, Tc.SampleType, C.ExpirationDate " & vbCrLf & _
                                                " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                            " INNER JOIN tparTestCalibrators TC ON OT.TestID = TC.TestID AND OT.SampleType = TC.SampleType " & vbCrLf & _
                                                                            " INNER JOIN tparCalibrators C ON TC.CalibratorID = C.CalibratorID " & vbCrLf & _
                                                " WHERE  WSE.ExecutionID = " & pExecutionID & vbCrLf

                        Dim calibratorDataDS As New TestSampleCalibratorDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(calibratorDataDS.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = calibratorDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetCalibratorData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Execution for the specified Preparation Identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparationID">Preparation Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pExcludePendingOrLocked">Optional parameter. When its value is TRUE, Executions with status PENDING 
        '''                                       or LOCKED will not be returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the Execution for the specified PreparationID</returns>
        ''' <remarks>
        ''' Created by: GDS 27/04/2010
        ''' Modified by: AG 03/01/2010 - Changed the query to get also field AdjustBaseLineID
        '''              AG 22/03/2011 - Changed the query to get also fields ClotValue and ThermoWarningFlag
        '''              AG 31/03/2011 - Added parameters WorkSessionID and AnalyzerID
        '''              SA 03/07/2012 - Added optional parameter pExcludePendingOrLocked. When TRUE, Executions with status PENDING
        '''                              or LOCKED will not be returned
        '''                            - Changed the query to get also fields AdjustBaseLineID, HasReadings, ValidReadings and CompleteReadings, 
        '''                              and also fields TestID, SampleType and ReplicatesNumber for the OrderTestID of the execution. Besides, if it is an 
        '''                              STANDARD Preparation, fields FirstReadingCycle and SecondReadingCycle are obtained from Tests table 
        '''              SA 26/07/2012 - Changed the query to get field ISE_ResultID from ISE Tests table if it is an ISE Preparation. Besides, get 
        '''                              field OrderID for the OrderTestID of the execution  
        '''              SA 14/06/2013 - Changed the query by adding an INNER JOIN with table twksWSOrderTests to get value of field CtrlsSendingGroup
        '''                              (needed to manage automatic reruns for Control Order Tests)
        '''              SA 06/06/2014 - BT #1660 ==> Changed the SQL Query to get also field ControlID from table twksOrderTests
        ''' </remarks>
        Public Function GetExecutionByPreparationID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer, _
                                                    ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                    Optional ByVal pExcludePendingOrLocked As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT E.ExecutionID, E.AnalyzerID, E.WorkSessionID, E.SampleClass, E.StatFlag, E.OrderTestID, E.MultiItemNumber, " & vbCrLf & _
                                                       " E.RerunNumber, E.ReplicateNumber, E.ExecutionStatus, E.ExecutionType, E.PostDilutionType, E.WellUsed, E.BaseLineID, " & vbCrLf & _
                                                       " E.AdjustBaseLineID, E.ABS_Value, E.ABS_Error, E.rKinetics, E.KineticsLinear, E.KineticsInitialValue, E.KineticsSlope, " & vbCrLf & _
                                                       " E.SubstrateDepletion, E.ABS_Initial, E.ABS_MainFilter, E.CONC_Value, E.CONC_CurveError, E.CONC_Error, E.InUse, E.ResultDate, " & vbCrLf & _
                                                       " E.PreparationID, E.ClotValue, E.ThermoWarningFlag, E.HasReadings, E.ValidReadings, E.CompleteReadings, " & vbCrLf & _
                                                       " OT.TestID, OT.SampleType, OT.ReplicatesNumber AS ReplicatesTotalNum, OT.OrderID, OT.ControlID, T.FirstReadingCycle, " & vbCrLf & _
                                                       " T.SecondReadingCycle, WOT.CtrlsSendingGroup, IT.ISE_ResultID " & vbCrLf & _
                                                " FROM   twksWSExecutions E INNER JOIN twksOrderTests OT ON E.OrderTestID = OT.OrderTestID AND E.AnalyzerID = OT.AnalyzerID " & vbCrLf & _
                                                                          " INNER JOIN twksWSOrderTests WOT ON E.OrderTestID = WOT.OrderTestID AND E.WorkSessionID = WOT.WorkSessionID " & vbCrLf & _
                                                                          " LEFT OUTER JOIN tparTests     T  ON OT.TestType = 'STD' AND OT.TestID = T.TestID " & vbCrLf & _
                                                                          " LEFT OUTER JOIN tparISETests IT  ON OT.TestType = 'ISE' AND OT.TestID = IT.ISETestID " & vbCrLf & _
                                                " WHERE  E.PreparationID = " & pPreparationID.ToString & vbCrLf & _
                                                " AND    E.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    E.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                        'When the optional parameter is TRUE, then exclude Pending or Locked Executions
                        If (pExcludePendingOrLocked) Then cmdText &= " AND E.ExecutionStatus <> 'PENDING' AND E.ExecutionStatus <> 'LOCKED' " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionByPreparationID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get a list of execution by ordertestid and rerunnumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunnumber">Rerun number</param>
        ''' <param name="pMultiItemNumber"></param>
        ''' <param name="pExecutionStatus"></param>
        ''' <param name="pMode"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by:  DL 06/05/2010
        ''' Modified by: AG 03/01/2010 - Add AdjustBaseLineID field
        '''              AG 24/02/2011 - Add pMode parameter: CURVE_RESULTS_MULTIPLE or RESULTS_MULTIPLE (with execution status = CLOSED or CLOSEDNOK), 
        '''                              WSSTATES_RESULTS_MULTIPLE (with execution status = CLOSED or CLOSEDNOK or INPROCESS)
        ''' </remarks>
        Public Function ReadByOrderTestIDAndRerunNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                        ByVal pRerunNumber As Integer, Optional ByVal pMultiItemNumber As Integer = -1, _
                                                        Optional ByVal pExecutionStatus As String = "", _
                                                        Optional ByVal pMode As GlobalEnumerates.GraphicalAbsScreenCallMode = GlobalEnumerates.GraphicalAbsScreenCallMode.NONE) _
                                                        As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ExecutionID, AnalyzerID, WorkSessionID, SampleClass, StatFlag, OrderTestID, MultiItemNumber, RerunNumber, " & vbCrLf & _
                                                       " ReplicateNumber, ExecutionStatus, ExecutionType, PostDilutionType, BaseLineID, WellUsed, ABS_Value, ABS_Error, " & vbCrLf & _
                                                       " rKinetics, KineticsLinear, KineticsInitialValue, KineticsSlope, SubstrateDepletion, ABS_Initial, ABS_MainFilter, " & vbCrLf & _
                                                       " CONC_Value, CONC_CurveError, CONC_Error, InUse, ResultDate, PreparationID, AdjustBaseLineID " & vbCrLf & _
                                                " FROM   twksWSExecutions" & vbCrLf & _
                                                " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    RerunNumber = " & pRerunNumber.ToString & vbCrLf

                        If (pMultiItemNumber > 0) Then cmdText &= " AND MultiItemNumber = " & pMultiItemNumber.ToString & vbCrLf

                        If (pExecutionStatus <> String.Empty) Then
                            cmdText &= " AND ExecutionStatus = '" & pExecutionStatus.Trim & "'" & vbCrLf
                        ElseIf (pMode <> GlobalEnumerates.GraphicalAbsScreenCallMode.NONE) Then
                            Select Case pMode
                                Case GlobalEnumerates.GraphicalAbsScreenCallMode.CURVE_RESULTS_MULTIPLE, GlobalEnumerates.GraphicalAbsScreenCallMode.RESULTS_MULTIPLE
                                    cmdText &= " AND (ExecutionStatus = 'CLOSED' OR ExecutionStatus = 'CLOSEDNOK') " & vbCrLf

                                Case GlobalEnumerates.GraphicalAbsScreenCallMode.WS_STATES_MULTIPLE
                                    cmdText &= " AND (ExecutionStatus = 'CLOSED' OR ExecutionStatus = 'CLOSEDNOK' OR ExecutionStatus = 'INPROCESS') " & vbCrLf
                            End Select
                        End If

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.ReadByOrderTestIDAndRerunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get a list of lock execution by SampleClass
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pSampleClass"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <param name="pReplicateNumber"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by:  DL 14/01/2011
        ''' Modified by: AG 04/04/2011 - Added pRerunNumber and pReplicateNumber and change query
        ''' </remarks>
        Public Function ReadLockRExecutionsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                         ByVal pAnalyzerID As String, ByVal pSampleClass As String, ByVal pRerunNumber As Integer, _
                                                         ByVal pReplicateNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT E.ExecutionID, E.AnalyzerID, E.WorkSessionID, E.SampleClass, " & vbCrLf & _
                                                       " E.StatFlag, E.OrderTestID, E.MultiItemNumber, E.RerunNumber, E.ReplicateNumber, " & vbCrLf & _
                                                       " E.ExecutionStatus, E.ExecutionType, OT.TestID, OT.SampleType " & vbCrLf & _
                                                " FROM   twksWSExecutions E INNER JOIN twksOrderTests OT ON E.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  E.SampleClass     = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    E.RerunNumber     = " & pRerunNumber.ToString & vbCrLf & _
                                                " AND    E.ReplicateNumber = " & pReplicateNumber.ToString & vbCrLf & _
                                                " AND    E.ExecutionStatus = 'LOCKED' " & vbCrLf & _
                                                " AND    E.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    E.AnalyzerID      = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                        Dim myExecutionDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myExecutionDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myExecutionDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.ReadLockRExecutionsBySampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Lock executions process (executions related with blank or calibrator)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pSampleClass" ></param>
        ''' <param name="pTestID" ></param>
        ''' <param name="pSampleType" ></param>
        ''' <param name="pTestType" ></param>
        ''' <param name="pLockPreparationIDHigherThanThis"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by:  DL 14/01/2011
        ''' Modified by: AG 04/04/2011 - Rename method to LockRelatedExecutions (old name UpdateStatusExecutions) and changes in code;
        '''                              use the proper template
        '''              AG 29/02/2012 - Add pLockPreparationIDHigherThanThis parameter and when informed lock also all OrderTest INPROCESS 
        '''                              executions where preparationID > parameter pPreparationID
        ''' </remarks>
        Public Function LockRelatedExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                              ByVal pSampleClass As String, ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pTestType As String, _
                                              Optional ByVal pLockPreparationIDHigherThanThis As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSExecutions SET ExecutionStatus = 'LOCKED' " & vbCrLf
                    cmdText &= " WHERE SampleClass = '" & pSampleClass & "' " & vbCrLf

                    'AG 29/02/2012
                    cmdText &= " AND (ExecutionStatus = 'PENDING' "
                    If (pLockPreparationIDHigherThanThis <> -1) Then
                        cmdText &= " OR (ExecutionStatus = 'INPROCESS' AND PreparationID > " & pLockPreparationIDHigherThanThis & ")"
                    End If
                    cmdText &= ") " & vbCrLf
                    'AG 29/02/2012

                    cmdText &= " AND WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                    cmdText &= " AND AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                    cmdText &= " AND OrderTestID IN (SELECT OrderTestID FROM twksOrderTests " & vbCrLf
                    cmdText &= "                     WHERE  TestID = " & pTestID & vbCrLf

                    If (pSampleType <> "") Then
                        cmdText &= " AND SampleType = '" & pSampleType & "' " & vbCrLf
                    End If
                    cmdText &= " AND TestType = '" & pTestType & "') "
                    'AG 04/04/2011

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.LockRelatedExecutions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Lock executions because of LIS cancelation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID" ></param>
        ''' <param name="pExeType"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' Modified by: XB 03/04/2013 - change status to search 'INPROCESS' by 'PENDING' into the SELECT
        ''' </remarks>
        Public Function LockExecutionsByLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pExeType As String, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSExecutions SET ExecutionStatus = 'LOCKED', LockedByLIS = 1 " & vbCrLf
                    cmdText &= " WHERE (ExecutionStatus = 'LOCKED' OR ExecutionStatus = 'PENDING' )" & vbCrLf
                    cmdText &= " AND OrderTestID = '" & pOrderTestID.ToString & "' " & vbCrLf
                    cmdText &= " AND RerunNumber = '" & pRerunNumber.ToString & "' " & vbCrLf
                    cmdText &= " AND ExecutionType = '" & pExeType & "' "
                    'AG 04/04/2011

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.LockExecutionsByLIS", EventLogEntryType.Error, False)
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
        ''' <remarks>
        ''' Created by:  DL 19/02/2010
        ''' Modified By: TR 14/04/2010 - Parameters pAnalyzerID and pWorkSessionID changed to Optional
        '''              AG 03/01/2010 - Get also field AdjustBaseLineID 
        '''              AG 22/03/2011 - Get also fields ThermoWarningFlag and ClotValue
        '''              TR 12/12/2011 - Get also field SampleType (from twksOrderTests) 
        '''              SA 11/01/2012 - Changed the query due to it didn't work for ISE Tests (TestID and TestName returned was always
        '''                              of an STANDARD Test); changed the function template
        '''              AG 02/07/2012 - Changed the query to get also fields CompleteReadings and ValidReadings from twksWSExecutions
        '''              SA 11/07/2012 - Changed the query to get also field ExecutionType from twksWSExecutions and TestType, ReplicatesNumber,
        '''                              OrderID and ControlID from twksOrderTests
        '''              XB 08/10/2014 - Get also KineticsLinear field to initialize preparation structure - BA-1970
        ''' </remarks>
        Public Function GetExecution(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer, _
                                     Optional ByVal pAnalyzerID As String = "", Optional ByVal pWorkSessionID As String = "") As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT  WSE.ExecutionID, WSE.ExecutionStatus,  WSE.WorkSessionID, WSE.AnalyzerID, WSE.BaseLineID, WSE.WellUsed, " & vbCrLf & _
                                                       "  WSE.OrderTestID, WSE.PostDilutionType, WSE.ReplicateNumber, WSE.RerunNumber, WSE.MultiItemNumber, " & vbCrLf & _
                                                       "  WSE.SampleClass, WSE.AdjustBaseLineID, WSE.ThermoWarningFlag, WSE.ClotValue, WSE.ExecutionType,  " & vbCrLf & _
                                                       " (CASE WHEN WSE.ExecutionType = 'PREP_STD' THEN T.TestID " & vbCrLf & _
                                                             " WHEN WSE.ExecutionType = 'PREP_ISE' THEN IT.ISETestID ELSE NULL END) AS TestID, " & vbCrLf & _
                                                       " (CASE WHEN WSE.ExecutionType = 'PREP_STD' THEN T.TestName " & vbCrLf & _
                                                       "       WHEN WSE.ExecutionType = 'PREP_ISE' THEN IT.ShortName ELSE NULL END) AS TestName, " & vbCrLf & _
                                                       "  WSE.ValidReadings, WSE.CompleteReadings, OT.TestType, OT.SampleType, OT.ReplicatesNumber AS ReplicatesTotalNum, " & vbCrLf & _
                                                       "  OT.ControlID, OT.OrderID, O.SampleID, WSE.KineticsLinear " & vbCrLf & _
                                                " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                            " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                            " LEFT JOIN tparTests T ON OT.TestID = T.TestID AND OT.TestType = 'STD' " & vbCrLf & _
                                                                            " LEFT JOIN tparISETests IT ON OT.TestID = IT.ISETestID AND OT.TestType = 'ISE' " & vbCrLf & _
                                                " WHERE  WSE.ExecutionID = " & pExecutionID & vbCrLf

                        'Add filters by optional parameters when they are informed
                        If (pWorkSessionID <> "") Then cmdText &= " AND WSE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf
                        If (pAnalyzerID <> "") Then cmdText &= " AND WSE.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecution", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Executions for the specified Analyzer and WorkSession (or only the Executions for STANDARD Tests if parameter
        ''' pFlagOnlyPrepSTD is TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work session Identifier</param>
        ''' <param name="pFlagOnlyPrepSTD">When TRUE, it indicates that only Executions for STANDARD Tests have to be returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by:  DL 19/02/2010
        ''' Modified by: TR 14/04/2010 - Added parameters for Analyzer and WorkSession identifiers
        '''              AG 16/11/2010 - Changed the query to get also field ExecutionStatus
        '''              AG 03/01/2011 - Changed the query to get also field AdjustBaseLineID; added the Order By clause
        '''              AG 05/01/2011 - Added parameter pFlagOnlyPrepSTD; when its value is TRUE the get only Executions for STANDARD Tests 
        '''              DL 12/01/2012 - Changed the query to get also field PreparationID
        ''' </remarks>
        Public Function GetExecutionByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String, ByVal pFlagOnlyPrepSTD As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ExecutionID, WorkSessionID, AnalyzerID, BaseLineID, WellUsed, OrderTestID, PostDilutionType, " & vbCrLf & _
                                                       " ReplicateNumber, RerunNumber, MultiItemNumber, SampleClass, ExecutionStatus, AdjustBaseLineID, " & vbCrLf & _
                                                       " PreparationID " & vbCrLf & _
                                                " FROM   twksWSExecutions " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                        If (pFlagOnlyPrepSTD) Then cmdText &= " AND ExecutionType = 'PREP_STD' " & vbCrLf
                        cmdText &= " ORDER BY AnalyzerID, WorkSessionID, ExecutionID "

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read executions record by OrderTestID - MultiItemNumber and ReplicateID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pMultiItemNum"></param>
        ''' <param name="pReplicateID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <returns>GlobalData to with set data as executionsDS</returns>
        ''' <remarks>
        ''' Created by AG 01/03/2010 (Tested OK)
        ''' Modified by AG 04/08/2010 - add RerunNumber into where clause
        ''' Modified by AG 03/07/2012 - improve query (get all replicates minor/equal the pReplicateID)
        ''' </remarks>
        Public Function ReadOrderTestReplicate(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pAnalyzerID As String, _
                                               ByVal pWorkSessionID As String, _
                                               ByVal pOrderTestID As Integer, _
                                               ByVal pMultiItemNum As Integer, _
                                               ByVal pReplicateID As Integer, _
                                               ByVal pRerunNumber As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim executionDataDS As New ExecutionsDS

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""

                        cmdText += "SELECT * "
                        cmdText += "FROM   twksWSExecutions "
                        cmdText += "WHERE  AnalyzerID = '" & pAnalyzerID & "'"
                        cmdText += "   and WorkSessionID = '" & pWorkSessionID & "'"
                        cmdText += "   and OrderTestID = " & pOrderTestID
                        cmdText += "   and MultiItemNumber = " & pMultiItemNum

                        'AG 03/07/2012
                        'cmdText += "   and ReplicateNumber = " & pReplicateID
                        cmdText += "   and ReplicateNumber <= " & pReplicateID

                        cmdText += "  and RerunNumber = " & pRerunNumber 'AG 04/08/2010

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.ReadOrderTestReplicate", EventLogEntryType.Error, False)

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
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSExecutions" & vbCrLf & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    'Dim cmd As SqlCommand
                    'cmd = pDBConnection.CreateCommand
                    'cmd.CommandText = cmdText
                    'cmd.ExecuteNonQuery()

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014

                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search all OrderTests in the specified Analyzer and WorkSession having ALL Executions with Status PENDING or LOCKED
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">OrderTest Identifier. Optional parameter. When informed, this function verifies if all Executions
        '''                            of the Order Test ID can be deleted</param>
        ''' <returns>GlobalDataTO containing a typed Dataset OrderTestsDS with the list of OrderTest Identifiers which 
        '''          Executions can be deleted</returns>
        ''' <remarks>
        ''' Created by:  SA 31/05/2012 - Created to improve performance of previous function DeleteNotInCurseExecutions
        ''' </remarks>
        Public Function SearchNotInCourseExecutionsToDelete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                           Optional ByVal pOrderTestID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim additionalFilter As String = " AND RerunNumber = 1 "
                        If (pOrderTestID <> -1) Then additionalFilter = " AND OrderTestID = " & pOrderTestID.ToString

                        Dim cmdText As String = " SELECT DISTINCT OrderTestID FROM twksWSExecutions " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    ExecutionStatus IN ('PENDING', 'LOCKED') " & vbCrLf & _
                                                additionalFilter & vbCrLf & _
                                                " AND    OrderTestID NOT IN (SELECT OrderTestID FROM twksWSExecutions " & vbCrLf & _
                                                                           " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                                           " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                                           " AND    ExecutionStatus NOT IN ('PENDING', 'LOCKED') " & vbCrLf & _
                                                                           additionalFilter & ") " & vbCrLf & _
                                                " ORDER BY OrderTestID " & vbCrLf

                        Dim myOTsToDeleteDS As New OrderTestsDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOTsToDeleteDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOTsToDeleteDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.SearchNotInCourseExecutionsToDelete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests Results from the Executions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the results (view vwksWSExecutionsResults)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 15/07/2010
        ''' Modified by: AG 01/12/2010 - Filter the query only by WorkSessionID (AnalyzerID is not used as filter)
        ''' </remarks>
        Public Function GetWSExecutionsResults(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Format(" SELECT * FROM vwksWSExecutionsResults " & _
                                                              " WHERE  WorkSessionID = '{0}' ", _
                                                              pWorkSessionID)

                        'Read from vwksWSExecutionsResults view
                        'cmdText = String.Format( _
                        '"SELECT * FROM vwksWSExecutionsResults " & _
                        '"WHERE AnalyzerID = '{0}' AND WorkSessionID = '{1}' ", _
                        'pAnalyzerID, pWorkSessionID)

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetWSExecutionsResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 25/09/2013 - CF - v2.1.1 Original code from GetWSExecutionsResults modified to add the OrderList Param. 
        ''' Gets the worksession executionsresults filtered by the supplied orderlist. 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="porderList"></param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns an ExecutionsDS dataset with the results (view vwksWSExecutionsResults)
        ''' </returns>
        ''' <remarks></remarks>
        Public Function GetWSExecutionsResultsByOrderIDList(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                          ByVal porderList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim sb As New StringBuilder
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Format(" SELECT * FROM vwksWSExecutionsResults " & _
                                                              " WHERE  WorkSessionID = '{0}'", _
                                                              pWorkSessionID)
                        Dim i As Integer = porderList.Count
                        For Each order As String In porderList
                            i -= 1
                            If i = 0 Then
                                sb.Append(String.Format("'{0}'", order))
                            Else
                                sb.Append(String.Format("'{0}',", order))
                            End If
                        Next
                        cmdText += String.Format(" AND OrderId in ({0})", sb.ToString)

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetWSExecutionsResultsByOrderID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' modified by: TR -06/06/2012 -Add column AlarmID.
        ''' AG 28/02/2014 - #1524 return also OrderTestID, RerunNumber (integrated in source control 11/03/2014)
        ''' </remarks>
        Public Function GetWSExecutionResultAlarms(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String

                        'Read from vwksWSExecutionsResults view
                        cmdText = "SELECT ExecutionID, Description, AlarmID, OrderTestID, RerunNumber  FROM vwksWSExecutionsAlarms"

                        Dim executionDataDS As New ExecutionsDS

                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            'Fill the DataSet to return
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsAlarms)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetWSExecutionResultAlarms", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

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
        ''' Created by: RH - 30/11/2010
        ''' </remarks>
        Public Function GetWSExecutionsMonitor(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                                   ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        'DL 20/09/2012
                        'Read from vwksWSExecutionsMonitor view
                        cmdText &= "  SELECT ExecutionID,AnalyzerID,WorkSessionID,SampleClass,StatFlag,OrderTestID,MultiItemNumber,RerunNumber" & vbCrLf
                        cmdText &= "        ,ReplicateNumber,ExecutionStatus,ExecutionType,OrderID,TestType,TestID,SampleType,OrderTestStatus" & vbCrLf
                        cmdText &= "        ,PatientName,PatientID,SampleID,OrderStatus,ExportStatus,TestName,Printed,Paused,ElementName,HasReadings" & vbCrLf
                        cmdText &= "    FROM vwksWSExecutionsMonitor" & vbCrLf
                        cmdText &= "   WHERE AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                        cmdText &= "     AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
                        cmdText &= "ORDER BY ExecutionID, StatFlag DESC, PatientID, SampleType"

                        'cmdText = String.Format( _
                        '   "SELECT * FROM vwksWSExecutionsMonitor " & _
                        '  "WHERE AnalyzerID = '{0}' AND WorkSessionID = '{1}' " & _
                        ' "ORDER BY ExecutionID, StatFlag DESC, PatientID, SampleType", _
                        'pAnalyzerID, pWorkSessionID)
                        'DL 20/09/2012

                        Dim executionDataDS As New ExecutionsDS

                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            'Fill the DataSet to return
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsMonitor)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetWSExecutionsMonitor", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Get the list of Executions with data to be monitored
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS (view vwksWSExecutionsMonitor) with data of all Executions
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by: DL 12/01/2012
        ''' </remarks>
        Public Function GetByOrderTestID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                         ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vwksWSExecutionsMonitor " & vbCrLf & _
                                                " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    OrderTestID   = " & pOrderTestID.ToString

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsMonitor)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Updates the Paused field into the twksOrderTests table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by RH 09/12/2010</remarks>
        Public Function UpdatePaused(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewValue As Boolean, _
                                     ByVal pExecutionID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String

                    cmdText = "UPDATE twksWSExecutions SET"
                    cmdText += " Paused = " & CStr(IIf(pNewValue, 1, 0))
                    cmdText += " WHERE ExecutionID = " & pExecutionID

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using

                    If (resultData.AffectedRecords >= 1) Then
                        resultData.HasError = False
                    Else
                        'resultData.HasError = True
                        'resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdatePaused", EventLogEntryType.Error, False)

            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Method 2on signature
        ''' Updates the Paused field into the Executions table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <param name="pNewValue"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 03/03/2014 - #1524</remarks>
        Public Function UpdatePaused(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As List(Of Integer), _
                                     ByVal pRerunNumber As List(Of Integer), ByVal pNewValue As Boolean) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    Dim auxText As String = ""

                    For i As Integer = 0 To pOrderTestID.Count - 1
                        If pOrderTestID.Count > 1 Then
                            If i < pOrderTestID.Count - 1 Then
                                auxText &= " (OrderTestID = " & pOrderTestID(i) & " AND RerunNumber = " & pRerunNumber(i) & " ) OR "
                            Else
                                auxText &= " (OrderTestID = " & pOrderTestID(i) & " AND RerunNumber = " & pRerunNumber(i) & " ) "
                            End If

                        Else
                            auxText &= " (OrderTestID = " & pOrderTestID(i) & " AND RerunNumber = " & pRerunNumber(i) & " ) "
                        End If
                    Next i

                    If auxText.Length > 0 Then
                        cmdText += "UPDATE twksWSExecutions SET"
                        cmdText += " Paused = " & CStr(IIf(pNewValue, 1, 0))
                        cmdText += " WHERE ( " & auxText & " ) "
                        cmdText += " AND (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED') "
                        cmdText += " AND Paused = " & CStr(IIf(pNewValue, 0, 1))


                        Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        End Using

                        If (resultData.AffectedRecords >= 1) Then
                            resultData.HasError = False
                        End If
                    End If

                End If


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdatePaused #2", EventLogEntryType.Error, False)

            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Updates the ThermoWarningFlag into the twksWSExecutions table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 22/03/2011 - Tested PENDING</remarks>
        Public Function UpdateThermoWarningFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewValue As Boolean, _
                                     ByVal pExecutionID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String

                    cmdText = "UPDATE twksWSExecutions SET"
                    cmdText += " ThermoWarningFlag = " & CStr(IIf(pNewValue, 1, 0))
                    cmdText += " WHERE ExecutionID = " & pExecutionID

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdateThermoWarningFlag", EventLogEntryType.Error, False)

            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Updates the ClotValue into the twksWSExecutions table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 22/03/2011 - Tested PENDING</remarks>
        Public Function UpdateClotValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewValue As String, _
                                     ByVal pExecutionID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""

                    cmdText = "UPDATE twksWSExecutions SET"

                    If Not pNewValue Is Nothing AndAlso pNewValue <> "" Then
                        cmdText += " ClotValue = '" & pNewValue & "' "
                    Else
                        cmdText += " ClotValue = NULL "
                    End If

                    cmdText += " WHERE ExecutionID = " & pExecutionID

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014


                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdateClotValue", EventLogEntryType.Error, False)

            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Updates the ClotValue into the twksWSExecutions table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 02/07/2012</remarks>
        Public Function UpdateValidAndCompleteReadings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""

                    'cmdText = "UPDATE twksWSExecutions SET"
                    'cmdText += " CompleteReadings = " & CStr(IIf(pCompleteReading, 1, 0))
                    'cmdText += " , ValidReadings = " & CStr(IIf(pValidReadings, 1, 0))
                    'cmdText += " WHERE ExecutionID = " & pExecutionID
                    'cmdText += " AND WorkSessionID = '" & pWorkSessionID.Trim & "' "
                    'cmdText += " AND Analyzer = '" & pAnalyzerID.Trim & "' "

                    For Each executionRow As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions
                        cmdText &= " UPDATE twksWSExecutions SET "
                        cmdText &= " CompleteReadings = " & CStr(IIf(executionRow.CompleteReadings, 1, 0))
                        cmdText &= " , ValidReadings = " & CStr(IIf(executionRow.ValidReadings, 1, 0))
                        cmdText &= " WHERE ExecutionID = " & executionRow.ExecutionID
                        cmdText &= " AND WorkSessionID = '" & executionRow.WorkSessionID.Trim & "' "
                        cmdText &= " AND AnalyzerID = '" & executionRow.AnalyzerID.Trim & "' "

                        cmdText &= String.Format("{0}", vbNewLine) 'insert line break
                    Next

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdateValidAndCompleteReadings", EventLogEntryType.Error, False)

            End Try

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
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS OpenExecutions " & vbCrLf & _
                                                " FROM   twksWSExecutions EX INNER JOIN twksWSRequiredElemByOrderTest REOT ON EX.OrderTestID = REOT.OrderTestID " & vbCrLf & _
                                                                                                                        " AND REOT.ElementID = " & pElementID.ToString & vbCrLf & _
                                                " WHERE  EX.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    EX.ExecutionType = 'PREP_STD' " & vbCrLf & _
                                                " AND    EX.ExecutionStatus NOT IN ('CLOSED', 'CLOSEDNOK') " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.CountNotClosedSTDExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        '''  For all not paused Pending and InProcess Executions corresponding to STD Tests for the informed WorkSessionID and AnalyzerID, get following data:
        ''' ** Execution data: ID, Status and OrderTestID
        ''' ** Test data: ID and TestCycles (the maximum value between FirstReadingCycle and SecondReadingCycle)
        ''' ** Preparations: the SendingTime of the Preparation containing the Instruction sent to the Analyzer for the Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TimeEstimationDS containing most of values needed to calculate the remaining
        '''          time for each Execution corresponding to STD Tests for the informed WorkSession and Analyzer (all values are obtained,
        '''          excepting the LastReadingCycle, that is calculated using new function GetMaxReadingNumber)</returns>
        ''' <remarks>
        ''' Created by:  AG 22/09/2010
        ''' Modified by: SA 21/10/2011 - Changed the function template; changed the SQL Query; removed parameter pExecutionType
        '''              SA 31/05/2012 - Changed the SQL Query (due to bad performance): removed the LEFT OUTER JOIN with twksWSReadings 
        '''              DL 20/09/2012 - Changed the SQL Query (Add clause WITH(NO LOCK): Avoid than update instruccions loked the Select clause
        ''' </remarks>
        Public Function GetSTDTestsTimeEstimation(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "  SELECT EX.ExecutionID, EX.AnalyzerID, EX.SampleClass, EX.OrderTestID, EX.ExecutionStatus, EX.ExecutionType, OT.TestID, " & vbCrLf
                        cmdText &= "        (CASE WHEN T.FirstReadingCycle < T.SecondReadingCycle THEN T.SecondReadingCycle ELSE T.FirstReadingCycle END) AS TestCycles, " & vbCrLf
                        cmdText &= "         TS.PredilutionUseFlag, TS.PredilutionMode, WSP.SendingTime, 0 As LastReadingCycle " & vbCrLf
                        cmdText &= "    FROM twksWSExecutions EX WITH(NOLOCK) INNER JOIN twksOrderTests OT WITH(NOLOCK) ON EX.OrderTestID = OT.OrderTestID " & vbCrLf
                        cmdText &= "               INNER JOIN tparTests T WITH(NOLOCK) ON OT.TestID = T.TestID " & vbCrLf
                        cmdText &= "               INNER JOIN tparTestSamples TS WITH(NOLOCK)ON OT.TestID = TS.TestID AND OT.SampleType = TS.SampleType " & vbCrLf
                        cmdText &= "               LEFT OUTER JOIN twksWSPreparations WSP WITH(NOLOCK) ON (EX.AnalyzerID = WSP.AnalyzerID AND EX.PreparationID = WSP.PreparationID) " & vbCrLf
                        cmdText &= "   WHERE EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                        cmdText &= "     AND EX.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf
                        cmdText &= "     AND EX.ExecutionType = 'PREP_STD' " & vbCrLf
                        cmdText &= "     AND EX.ExecutionStatus IN ('PENDING', 'INPROCESS') " & vbCrLf
                        cmdText &= "     AND EX.Paused = 0 " & vbCrLf
                        cmdText &= "GROUP BY EX.ExecutionID, EX.AnalyzerID, EX.SampleClass, EX.OrderTestID, EX.ExecutionStatus, EX.ExecutionType, OT.TestID, " & vbCrLf
                        cmdText &= "         T.FirstReadingCycle, T.SecondReadingCycle, TS.PredilutionUseFlag, TS.PredilutionMode, WSP.SendingTime " & vbCrLf
                        cmdText &= "ORDER BY EX.ExecutionID "

                        'Removed from previous query...
                        'MAX(ISNULL(WSR.ReadingNumber, 0)) As LastReadingCycle 
                        'LEFT OUTER JOIN twksWSReadings WSR ON (EX.AnalyzerID = WSR.AnalyzerID AND EX.ExecutionID = WSR.ExecutionID)

                        Dim timeEstimationDS As New TimeEstimationDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(timeEstimationDS.TestTimeValues)
                            End Using
                        End Using

                        resultData.SetDatos = timeEstimationDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetSTDTestsTimeEstimation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        '''  For all not paused Pending and InProcess Executions corresponding to STD Tests for the informed WorkSessionID and AnalyzerID, get following data:
        ''' ** Readings: the Number of the last Reading the Analyzer has sent for the Execution (zero if not Readings have been sent)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TimeEstimationDS containing the last reading cycle for each not paused
        '''          Pending and InProcess Executions corresponding to STD Tests for the informed WorkSession and Analyzer</returns>
        ''' <remarks>
        ''' Created by:  SA 31/05/2012
        ''' </remarks>
        Public Function GetMaxReadingNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT   EX.ExecutionID, MAX(WSR.ReadingNumber) As LastReadingCycle " & vbCrLf & _
                                                " FROM     twksWSExecutions EX INNER JOIN twksWSReadings WSR ON (EX.AnalyzerID = WSR.AnalyzerID AND EX.ExecutionID = WSR.ExecutionID) " & vbCrLf & _
                                                " WHERE    EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND      EX.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND      EX.ExecutionType = 'PREP_STD' " & vbCrLf & _
                                                " AND      EX.ExecutionStatus IN ('PENDING', 'INPROCESS') " & vbCrLf & _
                                                " AND      EX.Paused = 0 " & vbCrLf & _
                                                " GROUP BY EX.ExecutionID " & vbCrLf & _
                                                " ORDER BY EX.ExecutionID "

                        Dim timeEstimationDS As New TimeEstimationDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(timeEstimationDS.TestTimeValues)
                            End Using
                        End Using

                        resultData.SetDatos = timeEstimationDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetMaxReadingNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For all not paused Pending and InProcess Executions corresponding to ISE Tests for the informed WorkSessionID and AnalyzerID, get following data:
        ''' ** Execution data: ID, Status and OrderTestID
        ''' ** ISE Test data: ID 
        ''' ** Preparations: the SendingTime of the Preparation containing the Instruction sent to the Analyzer for the Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TimeEstimationDS containing all values needed to calculate the remaining
        '''          time for each Execution corresponding to ISE Tests for the informed WorkSession and Analyzer</returns>
        ''' <remarks>
        ''' Created by:  SA 24/10/2011 
        ''' </remarks>
        Public Function GetISETestsTimeEstimation(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT   EX.ExecutionID, EX.AnalyzerID, EX.SampleClass, EX.OrderTestID, EX.ExecutionStatus, EX.ExecutionType, OT.TestID, " & vbCrLf & _
                                                        "  OT.SampleType, WSP.SendingTime " & vbCrLf & _
                                                " FROM     twksWSExecutions EX INNER JOIN twksOrderTests OT ON EX.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                             " INNER JOIN tparISETests IT ON OT.TestID = IT.ISETestID " & vbCrLf & _
                                                                             " INNER JOIN tparISETestSamples ITS ON OT.TestID = ITS.ISETestID AND OT.SampleType = ITS.SampleType " & vbCrLf & _
                                                                             " LEFT OUTER JOIN twksWSPreparations WSP ON (EX.AnalyzerID = WSP.AnalyzerID AND EX.PreparationID = WSP.PreparationID) " & vbCrLf & _
                                                " WHERE    EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND      EX.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND      EX.ExecutionType = 'PREP_ISE' " & vbCrLf & _
                                                " AND      EX.ExecutionStatus IN ('PENDING', 'INPROCESS') " & vbCrLf & _
                                                " AND      EX.Paused = 0 " & vbCrLf & _
                                                " ORDER BY EX.ExecutionID "

                        Dim timeEstimationDS As New TimeEstimationDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(timeEstimationDS.TestTimeValues)
                            End Using
                        End Using

                        resultData.SetDatos = timeEstimationDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetISETestsTimeEstimation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the pending executons for send next ISE test calculation
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>AnalyzerManagerDS.searchNext inside a GlobalDataTO</returns>
        ''' <remarks>AG 18/01/2011</remarks>
        Public Function GetPendingPatientExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""
                        Dim executionDataDS As New AnalyzerManagerDS

                        cmdText = " SELECT E.ExecutionID, E.StatFlag, E.SampleClass, E.ExecutionType, "
                        cmdText += " E.ExecutionStatus, OT.OrderID,  OT.OrderTestID, OT.SampleType "
                        cmdText += " FROM twksWSExecutions E INNER JOIN twksOrderTests OT ON E.OrderTestID = OT.OrderTestID "
                        cmdText += " WHERE E.Paused = 'False'  AND E.SampleClass = 'PATIENT' "
                        cmdText += " AND E.ExecutionStatus = 'PENDING' "
                        cmdText += " AND E.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"
                        cmdText += " AND E.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"
                        cmdText += " ORDER BY E.StatFlag DESC, E.ExecutionID"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(executionDataDS.searchNext)

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetPendingPatientExecutions", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns additonal information for inform the STD TEST preparation send and required for apply the send preparation algorithm
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
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim executionDataDS As New AnalyzerManagerDS

                        cmdText = " SELECT E.ExecutionID, E.StatFlag, E.ExecutionType, E.SampleClass, "
                        cmdText += " OT.OrderID, OT.OrderTestID, OT.SampleType, TR.ReagentID "
                        cmdText += " FROM twksWSExecutions E INNER JOIN twksOrderTests OT ON E.OrderTestID = OT.OrderTestID "
                        cmdText += " INNER JOIN tparTestReagents TR ON OT.TestID = TR.TestID"
                        cmdText += " WHERE E.ExecutionID = " & pExecutionID
                        cmdText += " AND E.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"
                        cmdText += " AND E.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        ''Fill the DataSet to return 
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(executionDataDS.searchNext)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.searchNext)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False

                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetSendPreparationDataByExecution", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the pending STD and ISE executons for send. 
        ''' REJECTED 20/12/2011 - Return also reagent information to avoid contaminations (his reagent contaminators and his washing modes)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pOnlyStdTestFlag" ></param>
        ''' <returns>ExecutionsDS.twksWSExecutions inside a GlobalDataTO</returns>
        ''' <remarks>AG 24/01/2011
        ''' AG 30/11/2011 add parameter pOnlyStdTestFlag
        ''' AG 20/11/2011 change method name
        ''' AG 20/12/2011 return ExecutionsDS.twksWSExecutions instead of AnalyzerManagerDS.searchNext
        ''' AG 24/01/2012 - add replicatenumber into select
        ''' </remarks>
        Public Function GetPendingExecutionForSendNextProcess(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String, ByVal pOnlyStdTestFlag As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""
                        Dim executionDataDS As New ExecutionsDS 'AnalyzerManagerDS

                        cmdText = " SELECT DISTINCT E.ExecutionID, E.StatFlag, E.SampleClass, E.ExecutionType, E.ExecutionStatus, E.ReplicateNumber, "
                        'cmdText += " OT.OrderID,  OT.OrderTestID, OT.SampleType, TR.ReagentID, "
                        'cmdText += " TC.ContaminationID , TC.ReagentContaminatorID, TC.WashingSolutionR1 As WashingSolution1 "
                        cmdText += " OT.OrderID,  OT.OrderTestID, OT.SampleType, TR.ReagentID "

                        cmdText += " FROM twksWSExecutions E INNER JOIN twksOrderTests OT ON E.OrderTestID = OT.OrderTestID "
                        cmdText += " INNER JOIN tparTestReagents TR ON OT.TestID = TR.TestID "
                        cmdText += " LEFT OUTER JOIN tparContaminations TC ON TR.ReagentID = TC.ReagentContaminatedID "

                        'AG 30/11/2011
                        'cmdText += " WHERE E.Paused = 'False'  AND E.ExecutionType = 'PREP_STD' "
                        cmdText += " WHERE E.Paused = 'False' "
                        If pOnlyStdTestFlag Then
                            cmdText += " AND E.ExecutionType = 'PREP_STD' "
                        End If
                        'AG 30/11/2011

                        cmdText += " AND E.ExecutionStatus = 'PENDING' "
                        cmdText += " AND E.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"
                        cmdText += " AND E.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"
                        cmdText += " ORDER BY E.StatFlag DESC, E.SampleClass, E.ExecutionID"

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        ''Fill the DataSet to return 
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetPendingExecutionForSendNextProcess", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get if the ExecutionID has programmed contamination cuvette
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID" ></param>
        ''' <returns>AnalyzerManagerDS.searchNext inside a GlobalDataTO</returns>
        ''' <remarks>AG 07/02/2011 - tested OK
        ''' AG 24/11/2011 - Select also ot.TestID column</remarks>
        Public Function GetExecutionContaminationCuvette(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""
                        Dim myDataDS As New AnalyzerManagerDS

                        ''If TestID is saved when programm cuvette contamination use this command
                        cmdText = " SELECT ex.ExecutionID, ex.OrderTestID, c.ContaminationID, c.WashingSolutionR1 As WashingSolution1, c.WashingSolutionR2 As WashingSolution2, ot.TestID "
                        cmdText += " FROM twksWSExecutions ex INNER JOIN "
                        cmdText += " twksOrderTests ot ON ex.OrderTestID = ot.OrderTestID INNER JOIN "
                        cmdText += " tparContaminations c ON ot.TestID = c.TestContaminaCuvetteID  "
                        cmdText += " WHERE ex.ExecutionType = 'PREP_STD' "
                        cmdText += " AND ex.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"
                        cmdText += " AND ex.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"
                        cmdText += " AND ex.ExecutionID = " & pExecutionID


                        'If ReagentID is saved when programm cuvette contamination use this command
                        'cmdText = " SELECT ex.ExecutionID, ex.OrderTestID, tr.ReagentID, c.ContaminationID, c.WashingSolutionR1 As WashingSolution1, c.WashingSolutionR2 As WashingSolution2 "
                        'cmdText += " FROM twksWSExecutions ex INNER JOIN "
                        'cmdText += " twksOrderTests ot ON ex.OrderTestID = ot.OrderTestID INNER JOIN "
                        'cmdText += " tparTestReagents tr ON ot.TestID = tr.TestID INNER JOIN "
                        'cmdText += " tparContaminations c ON tr.ReagentID = c.TestContaminaCuvetteID  "
                        'cmdText += " WHERE ex.ExecutionType = 'PREP_STD' "
                        'cmdText += " AND ex.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"
                        'cmdText += " AND ex.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"
                        'cmdText += " AND ex.ExecutionID = " & pExecutionID

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myDataDS.searchNext)

                        resultData.SetDatos = myDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionContaminationCuvette", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' All pending executions wich reagent is contaminator that requires the wash solution with no volume are LOCKED
        ''' Update status for all PENDING executions wich reagent has defined as a contaminator that required the pWashSolutionCode
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewExecutionStatus"></param>
        ''' <param name="pCurrentStatus"></param>
        ''' <param name="pWashSolutionCode"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 06/04/2011 - created</remarks>
        Public Function UpdateStatusByContamination(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewExecutionStatus As String, _
                                                    ByVal pCurrentStatus As String, ByVal pWashSolutionCode As String, _
                                                    ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    cmdText = cmdText & "UPDATE twksWSExecutions SET ExecutionStatus = '" & pNewExecutionStatus & "' " & vbCrLf
                    cmdText = cmdText & " WHERE ExecutionStatus  = '" & pCurrentStatus & "' " & vbCrLf
                    cmdText = cmdText & " AND WorkSessionID  = '" & pWorkSessionID & "' " & vbCrLf
                    cmdText = cmdText & " AND AnalyzerID  = '" & pAnalyzerID & "' " & vbCrLf
                    cmdText = cmdText & " AND ExecutionID IN " & vbCrLf
                    cmdText = cmdText & " (SELECT E.ExecutionID FROM twksWSExecutions E INNER JOIN twksOrderTests OT " & vbCrLf
                    cmdText = cmdText & " ON E.OrderTestID = OT.OrderTestID INNER JOIN tparTestReagents TR " & vbCrLf
                    cmdText = cmdText & " ON OT.TestID = TR.TestID INNER JOIN tparContaminations CT " & vbCrLf
                    cmdText = cmdText & " ON TR.ReagentID = CT.ReagentContaminatorID " & vbCrLf
                    cmdText = cmdText & " WHERE E.ExecutionStatus = '" & pCurrentStatus & "' " & vbCrLf
                    cmdText = cmdText & " AND E.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                    cmdText = cmdText & " AND E.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                    cmdText = cmdText & " AND OT.TestType = 'STD' AND (CT.WashingSolutionR1 = '" & pWashSolutionCode & "' " & vbCrLf
                    cmdText = cmdText & " OR CT.WashingSolutionR2 = '" & pWashSolutionCode & "')) " & vbCrLf

                    'Execute the SQL sentence 
                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateStatusByContamination", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' GetOrderTestWithExecutionStatus
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="psSourceForm"></param>
        ''' <param name="pRerun"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by DL
        ''' Modified by AG 24/02/2011 - Add CLOSEDNOK in WHERE clausules and comment the AND RerunNumber = ... condition
        ''' Modified by DL 09/03/2011 - Add Condition by rerun
        ''' </remarks>
        Public Function GetOrderTestWithExecutionStatus(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                        ByVal psSourceForm As GlobalEnumerates.ScreenCallsGraphical, _
                                                        ByVal pRerun As Integer, _
                                                        ByVal pOrderTestId As Integer, _
                                                        ByVal pAnalyzerID As String, _
                                                        ByVal pWorkSessionID As String, _
                                                        Optional ByVal pExecutionID As Integer = -1) As GlobalDataTO


            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "  SELECT ExecutionID, AnalyzerID, WorkSessionID, SampleClass, SampleID, OrderTestID, MultiItemNumber, RerunNumber, " & vbCrLf
                        cmdText &= "         ReplicateNumber, ExecutionStatus, WellUsed, ABS_Value, TestType, SampleType, TestID, TestName, ReadingMode, MainWavelength, ReferenceWavelength"
                        cmdText &= "    FROM vwksWSAbsorbance"

                        'DL 15/05/2012
                        'Select Case psSourceForm
                        '    Case GlobalEnumerates.ScreenCallsGraphical.WS_STATES
                        '        cmdText &= "   WHERE (ExecutionStatus <> '') " & vbCrLf ' dl 17/11/2011 (ExecutionStatus = 'CLOSED' OR ExecutionStatus = 'INPROCESS' OR ExecutionStatus = 'CLOSEDNOK') " & vbCrLf

                        '    Case GlobalEnumerates.ScreenCallsGraphical.RESULTSFRM ', GlobalEnumerates.ScreenCallsGraphical.CURVEFRM
                        '        cmdText &= "   WHERE (ExecutionStatus = 'CLOSED' OR ExecutionStatus = 'CLOSEDNOK') " & vbCrLf

                        '    Case GlobalEnumerates.ScreenCallsGraphical.CURVEFRM
                        '        cmdText &= "   WHERE (ExecutionStatus <> '') " & vbCrLf ' dl 15/05/2012 cmdText &= "   WHERE (ExecutionStatus = 'CLOSED' OR ExecutionStatus = 'CLOSEDNOK' OR ExecutionStatus = 'INPROCESS') " & vbCrLf

                        '    Case Else
                        '        cmdText &= "   WHERE (ExecutionStatus = 'CLOSED' OR ExecutionStatus = 'CLOSEDNOK') " & vbCrLf
                        'End Select
                        cmdText &= "   WHERE (ExecutionStatus <> '') " & vbCrLf
                        'DL 15/05/2012

                        'cmdText &= "     AND RerunNumber = " & pRerun & vbCrLf
                        cmdText &= "     AND AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                        cmdText &= "     AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf

                        If pExecutionID > -1 Then
                            cmdText &= "     AND OrderTestID = '" & pOrderTestId & "'" & vbCrLf
                            cmdText &= "     AND ExecutionID = '" & pExecutionID & "'" & vbCrLf
                        End If

                        cmdText &= "ORDER BY ExecutionID"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New vwksWSAbsorbanceDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myDS.vwksWSAbsorbance)

                        resultData.SetDatos = myDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetOrderTestWithExecutionStatus", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Due to the way of working of ISE Module, all ISE Tests requested for each Patient or Control with the same SampleType will be 
        ''' executed together; that means all ISE Executions having the same OrderID/SampleType and ReplicateNumber will have the same 
        ''' PreparationID and become INPROCESS at the same time.
        ''' This function returns all affected ISE Executions 
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
        ''' Modified by: AG 30/11/2011 - Changed the SQL to get only not Paused Executions
        '''              SA 02/07/2012 - Added optional parameter pOnlyPending; when its value is TRUE, it means the function
        '''                              has been called from the process of mark Preparations as accepted, and in this case, 
        '''                              only PENDING ISE Executions are returned
        '''              SA 10/07/2012 - Added parameter for the SampleClass (CTRL or PATIENT): For Patients, all ISE Executions using the same 
        '''                              SampleType tube are obtained, while for Controls, all ISE Executions using the same Control are obtained
        ''' </remarks>
        Public Function GetAffectedISEExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                 ByVal pExecutionID As Integer, ByVal pSampleClass As String, Optional ByVal pOnlyPending As Boolean = False) _
                                                 As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim innerJoinBySampleClass As String = String.Empty
                        If (pSampleClass = "CTRL") Then
                            innerJoinBySampleClass = " INNER JOIN twksOrderTests OTB ON OTA.ControlID = OTB.ControlID " & vbCrLf & _
                                                                                  " AND OTB.TestType    = 'ISE' "
                        ElseIf (pSampleClass = "PATIENT") Then
                            innerJoinBySampleClass = " INNER JOIN twksOrderTests OTB ON OTA.OrderID = OTB.OrderID " & vbCrLf & _
                                                                                  " AND OTA.SampleType  = OTB.SampleType " & vbCrLf & _
                                                                                  " AND OTB.TestType    = 'ISE' "
                        End If

                        Dim cmdText As String = " SELECT EXB.* FROM twksWSExecutions EXA INNER JOIN twksOrderTests OTA ON EXA.OrderTestID = OTA.OrderTestID " & vbCrLf & _
                                                                                         innerJoinBySampleClass & vbCrLf & _
                                                                                       " INNER JOIN twksWSExecutions EXB ON OTB.OrderTestID     = EXB.OrderTestID " & vbCrLf & _
                                                                                                                      " AND EXA.ReplicateNumber = EXB.ReplicateNumber " & vbCrLf & _
                                                                                                                      " AND EXB.ExecutionType = 'PREP_ISE' " & vbCrLf & _
                                                                                                                      " AND EXB.Paused = 0 " & vbCrLf & _
                                                " WHERE  EXA.ExecutionID   = " & pExecutionID.ToString & vbCrLf & _
                                                " AND    EXA.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    EXA.AnalyzerID    = N'" & pAnalyzerID.Trim & "' " & vbCrLf

                        'When PreparationID is informed, then only PENDING ISE Executions are obtained
                        If (pOnlyPending) Then cmdText &= " AND EXB.ExecutionStatus = 'PENDING' " & vbCrLf

                        Dim myExecutionsDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myExecutionsDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myExecutionsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetAffectedISEExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the SentNewRerunPostdilution field into the twksWSExecutions table Once the new repetition has been sent
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="porderTestID "></param>
        ''' <param name="pPostDilutionType "></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>Created by AG 08/03/2011 - Tested OK</remarks>
        Public Function UpdateSentNewRerunPostdilution(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                       ByVal pWorkSessionID As String, ByVal porderTestID As Integer, _
                                                        ByVal pPostDilutionType As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""

                    cmdText = "UPDATE twksWSExecutions SET "
                    cmdText += " SentNewRerunPostdilution = '" & pPostDilutionType & "' " & vbCrLf
                    cmdText += " WHERE OrderTestID = " & porderTestID & " "
                    cmdText += " AND AnalyzerID = '" & pAnalyzerID & "' "
                    cmdText += " AND WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                    cmdText += " AND RerunNumber = (SELECT MAX(RerunNumber) FROM twksWSExecutions WHERE OrderTestID = " & porderTestID & " "
                    cmdText += " AND AnalyzerID = '" & pAnalyzerID & "' "
                    cmdText += " AND WorkSessionID = '" & pWorkSessionID & "' )" & vbCrLf

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdateSentNewRerunPostdilution", EventLogEntryType.Error, False)

            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get the SAMPLE position used for pExecutionId value (this method dont work for blanks!!!)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>GlobalDataTO (WSRotorContentByPositionDS)</returns>
        ''' <remarks>AG 13/05/2011</remarks>
        Public Function GetSampleRotorPositionByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                            ByVal pAnalyzerID As String, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT ex.OrderTestID, ex.SampleClass, ex.MultiItemNumber As MultiTubeNumber, re.ElementID, rcp.RingNumber , rcp.CellNumber, rcp.Status " & vbCrLf
                        cmdText &= " FROM twksWSExecutions ex INNER JOIN twksWSRequiredElemByOrderTest re ON ex.ordertestID = re.OrderTestID " & vbCrLf
                        cmdText &= " INNER JOIN twksWSRotorContentByPosition rcp ON re.ElementID = rcp.ElementID AND ex.WorkSessionID = rcp.WorkSessionID " & vbCrLf
                        cmdText &= " AND ex.AnalyzerID  = ex.AnalyzerID AND rcp.RotorType = 'SAMPLES' " & vbCrLf
                        cmdText &= " INNER JOIN twksWSRequiredElements re2 ON rcp.ElementID = re2.ElementID AND EX.SampleClass = RE2.TubeContent " & vbCrLf
                        cmdText &= " WHERE ex.ExecutionID = " & pExecutionID & " " & vbCrLf
                        cmdText &= " AND ex.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                        cmdText &= " AND ex.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                        cmdText &= " AND (EX.MultiItemNumber = RE2.MultiItemNumber OR RE2.MultiItemNumber IS NULL) "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myResultDataDS As New WSRotorContentByPositionDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myResultDataDS.twksWSRotorContentByPosition)

                        resultData.SetDatos = myResultDataDS
                        resultData.HasError = False

                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.GetSampleRotorPositionByExecutionID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Get data of all Executions for the informed OrderTestID and optionally, MultiItemNumber in the specified Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pMultiItemNumber">MultiItem Number. It is informed only when the OrderTest is for a multipoint Calibrator</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of all Executions for the informed OrderTestID
        '''          and optionally, MultiItemNumber in the specified Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:
        ''' </remarks>
        Public Function GetByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                       ByVal pOrderTestID As Integer, ByVal pMultiItemNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSExecutions " & vbCrLf & _
                                                " WHERE  OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf
                        If (pMultiItemNumber > 0) Then cmdText &= " AND  MultiItemNumber = " & pMultiItemNumber.ToString & vbCrLf

                        Dim myResultDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myResultDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetByOrderTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the Executions related with the specified Element 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of the Executions related with the specified Element</returns>
        ''' <remarks>
        ''' Created by:  AG 16/05/2011
        ''' Modified by: AG 08/09/2011 - Query changed  
        ''' </remarks>
        Public Function GetByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                       ByVal pAnalyzerID As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT EX.ExecutionID, EX.MultiItemNumber, EX.ExecutionStatus, EX.SampleClass " & vbCrLf & _
                                                " FROM   twksWSExecutions EX INNER JOIN twksWSRequiredElemByOrderTest RE ON EX.OrderTestID = RE.OrderTestID " & vbCrLf & _
                                                " WHERE  RE.ElementID = " & pElementID.ToString

                        Dim myResultDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myResultDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.GetByElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Executions with field AdjustBaseLine informed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with all Executions with field AdjustBaseLine informed</returns>
        ''' <remarks>
        ''' Created by:  AG 08/07/2011
        ''' </remarks>
        Public Function GetExecutionsUsingAdjustBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                         ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ExecutionID FROM twksWSExecutions " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    AdjustBaseLineID IS NOT NULL " & vbCrLf

                        Dim myResultDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myResultDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.GetExecutionsUsingAdjustBaseLine", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the executions table by execution type and status to a new status
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
        ''' AG 07/03/2012 - if pAffectedExecutionList informed then ignore the pExecutionType and use the pAffectedExecutionList</remarks>
        Public Function UpdateStatusByExecutionTypeAndStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                         ByVal pExecutionType As String, ByVal pCurrentStatus As String, ByVal pNewStatus As String, _
                                         Optional ByVal pAffectedExecutionList As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    cmdText = "UPDATE twksWSExecutions SET ExecutionStatus = '" & pNewStatus & "' " & vbCrLf
                    cmdText += " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                    cmdText += " AND AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                    cmdText += " AND ExecutionStatus = '" & pCurrentStatus & "' " & vbCrLf

                    'AG 07/03/2012
                    'cmdText += " AND ExecutionType = " & pExecutionType
                    If pAffectedExecutionList = "" Then
                        cmdText += " AND ExecutionType = '" & pExecutionType & "' "
                    Else
                        cmdText += " AND ExecutionID IN (" & pAffectedExecutionList & ")"
                    End If
                    'AG 07/03/2012

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdateStatusByExecutionTypeAndStatus", EventLogEntryType.Error, False)
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
        ''' Modified by: SA  07/09/2012 - Changed name of parameter pISETestType by pISEElectrode. Changed the subquery by replacing use of 
        '''                               LIKE sentence by direct value comparison in uppercase
        ''' </remarks>
        Public Function UpdateStatusByISETestType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pAnalyzerID As String, ByVal pISEElectrode As String, ByVal pCurrentStatus As String, _
                                                  ByVal pNewStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE twksWSExecutions SET ExecutionStatus = '" & pNewStatus.Trim & "' " & vbCrLf & _
                                            " WHERE  WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    AnalyzerID      = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    ExecutionType   = 'PREP_ISE' " & vbCrLf & _
                                            " AND    ExecutionStatus = '" & pCurrentStatus.Trim & "' " & vbCrLf & _
                                            " AND    OrderTestID IN (SELECT OrderTestID " & vbCrLf & _
                                                                   " FROM   twksOrderTests OT INNER JOIN tparISETests IT ON OT.TestID = IT.ISETestID " & vbCrLf & _
                                                                   " WHERE  OT.TestType = 'ISE' " & vbCrLf & _
                                                                   " AND    UPPER(IT.ISE_ResultID) = UPPER('" & pISEElectrode.Trim & "')) " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.UpdateStatusByISETestType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' All pending executions that are PREDILUTIONS and required the diluent solution with no volume are LOCKED
        ''' Lock executions process (pending patient predilution executions using the pDiluentCode are locked)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pDiluentCode " ></param>
        ''' <param name="pLockPreparationIDHigherThanThis"></param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>
        ''' Created by: AG - 06/04/2011
        ''' AG 29/02/2012 - add pLockPreparationIDHigherThanThis parameter and when informed lock also all ordertest INPROCESS executions where preparationID > parameter pPreparationID
        ''' </remarks>
        Public Function LockPredilutionsByDiluentCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                      ByVal pAnalyzerID As String, ByVal pDiluentCode As String, _
                                                      Optional ByVal pLockPreparationIDHigherThanThis As Integer = -1) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    'Write sentence
                    cmdText = cmdText & "UPDATE twksWSExecutions" & vbCrLf
                    cmdText = cmdText & "   SET ExecutionStatus = 'LOCKED'" & vbCrLf
                    cmdText = cmdText & " WHERE WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                    cmdText = cmdText & " AND AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                    cmdText = cmdText & " AND ExecutionID IN (SELECT E.ExecutionID  FROM twksWSExecutions E " & vbCrLf
                    cmdText = cmdText & " INNER JOIN twksOrderTests OD ON E.OrderTestID = OD.OrderTestID " & vbCrLf
                    cmdText = cmdText & " INNER JOIN tparTestSamples TS ON OD.TestID = TS.TestID AND OD.SampleType = TS.SampleType " & vbCrLf

                    'AG 29/02/2012
                    'cmdText = cmdText & " WHERE E.ExecutionStatus = 'PENDING' " & vbCrLf
                    cmdText = cmdText & " WHERE ( E.ExecutionStatus = 'PENDING' "
                    If pLockPreparationIDHigherThanThis <> -1 Then
                        cmdText &= "   OR (ExecutionStatus = 'INPROCESS' AND PreparationID > " & pLockPreparationIDHigherThanThis & ")"
                    End If
                    cmdText &= ") " & vbCrLf
                    'AG 29/02/2012

                    cmdText = cmdText & " AND E.SampleClass  = 'PATIENT' " & vbCrLf
                    cmdText = cmdText & " AND TS.PredilutionUseFlag = 'TRUE' AND TS.PredilutionMode = 'INST' " & vbCrLf
                    cmdText = cmdText & " AND TS.DiluentSolution = '" & pDiluentCode & "' " & vbCrLf
                    cmdText = cmdText & " AND E.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                    cmdText = cmdText & " AND E.AnalyzerID = '" & pAnalyzerID & "' ) " & vbCrLf

                    'Execute the SQL sentence 
                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.LockPredilutionsByDiluentCode", EventLogEntryType.Error, False)
            End Try
            Return resultData

        End Function


        ''' <summary>
        ''' Get data of all Executions in the specified Analyzer and WorkSession having the informed Status
        ''' Optionally, paused Executions are excluded and/or Executions having a type different of the informed one
        ''' are also excluded
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pExecutionStatus">Execution Status</param>
        ''' <param name="pRejectPausedExecutions">When TRUE, pause Executions are excluded</param>
        ''' <param name="pExecutionType">When informed, only Executions having the informed Type are obtained</param>
        ''' <returns>GlobalDataTo (ExecutionsDS.twksWSExecutions)</returns>
        ''' <remarks>
        ''' Created by:  AG 19/04/2011
        ''' Modified by: AG 19/07/2011 - Added parameter pRejectPausedExecutions; when TRUE, paused Executions are excluded
        '''              AG 23/03/2012 - Added optional parameter pExecutionType; when informed, also the Executions of this type are returned
        ''' </remarks>
        Public Function GetExecutionsByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                              ByVal pAnalyzerID As String, ByVal pExecutionStatus As String, ByVal pRejectPausedExecutions As Boolean, _
                                              Optional ByVal pExecutionType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT E.ExecutionID, E.AnalyzerID, E.WorkSessionID, E.SampleClass, E.OrderTestID, " & vbCrLf & _
                                                       " E.ReplicateNumber, E.ExecutionType, E.PreparationID, OT.OrderID, OT.TestType, OT.TestID, OT.SampleType " & vbCrLf & _
                                                " FROM   twksWSExecutions E INNER JOIN twksOrderTests OT ON E.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  E.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    E.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    E.ExecutionStatus = '" & pExecutionStatus.Trim & "' " & vbCrLf

                        If (pRejectPausedExecutions) Then cmdText &= " AND E.Paused = 0 " & vbCrLf
                        If (pExecutionType <> "") Then cmdText &= " AND E.ExecutionType = '" & pExecutionType.Trim & "' " & vbCrLf

                        Dim myExecutionDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myExecutionDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myExecutionDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionsByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets AdditionalInfo data for an Alarm
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pWorkSessionID" ></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pAlarmCode"></param>
        ''' <param name="pReagentNumber"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by RH: 03/02/2012
        '''             TR: 01/10/2012 - Add new alarm BOTTLE_LOCKED_WARN  
        ''' </remarks>
        Public Function GetDataForAlarmAdditionalInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                                       ByVal pWorkSessionID As String, ByVal pExecutionID As Integer, _
                                                                       ByVal pAlarmCode As GlobalEnumerates.Alarms, _
                                                                       ByVal pReagentNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty

                        'Reagents volume missing
                        If (pAlarmCode = GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN) OrElse (pAlarmCode = GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN) _
                            OrElse (pAlarmCode = GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN) Then
                            cmdText = GetCmdTextForAlarmAdditionalInfo(pAnalyzerID, pWorkSessionID, pExecutionID, pReagentNumber)

                            'Samples volume missing or clot detection (error or warning)
                        Else
                            cmdText = GetCmdTextForAlarmAdditionalInfo(pAnalyzerID, pWorkSessionID, pExecutionID)
                        End If

                        Dim myAdditionalInfoDS As New WSAnalyzerAlarmsDS

                        Using cmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(cmd)
                                'Fill the DataSet to return
                                dbDataAdapter.Fill(myAdditionalInfoDS.AdditionalInfoPrepLocked)
                            End Using
                        End Using

                        resultData.SetDatos = myAdditionalInfoDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetDataForAlarmAdditionalInfo", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Pending and In Process Executions to calculate the time.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 01/06/2012</remarks>
        Public Function GetSendISETestsForTimeCalculation(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, _
                                                          ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " "
                        cmdText &= "SELECT OT.OrderID, OT.SampleType, P.SendingTime  " & vbCrLf
                        cmdText &= "FROM   twksWSExecutions EX INNER JOIN twksOrderTests OT ON EX.OrderTestID = OT.OrderTestID AND OT.TestType = 'ISE' " & vbCrLf
                        cmdText &= "                           LEFT JOIN twksWSPreparations P ON EX.PreparationID = P.PreparationID " & vbCrLf
                        cmdText &= "WHERE  EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                        cmdText &= "AND    EX.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                        cmdText &= "AND    EX.ExecutionType = 'PREP_ISE' " & vbCrLf
                        cmdText &= "AND    EX.ExecutionStatus ='INPROCESS' " & vbCrLf
                        cmdText &= "AND    EX.Paused = 0 " & vbCrLf
                        cmdText &= "AND    (EX.PreparationID IS NOT NULL AND P.SendingTime IS NOT NULL) " & vbCrLf
                        cmdText &= "GROUP BY OT.OrderID, OT.SampleType, P.SendingTime " & vbCrLf
                        cmdText &= "ORDER BY OT.OrderID " & vbCrLf

                        Dim myTimeEstimationDS As New TimeEstimationDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTimeEstimationDS.TestTimeValues)
                            End Using
                        End Using

                        resultData.SetDatos = myTimeEstimationDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetSendISETestsForTimeCalculation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: ???
        ''' Modified b: AG 12/07/2010 (add Abs_MainFilter and Abs_WorkReagent) - OK
        ''' Modified by AG: 26/07/2010 (dont update date)
        ''' Modified by AG 11/08/2010 - Do loop for all rows in DS
        ''' </remarks>
        Public Function SaveExecutionsResults(ByVal pDBConnection As SqlClient.SqlConnection, _
                                              ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try

                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else

                    If pExecutionsDS.twksWSExecutions.Rows.Count < 1 Then
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString

                    Else
                        'AG 11/08/2010 - add loop and change (0) to (i)
                        For i As Integer = 0 To pExecutionsDS.twksWSExecutions.Rows.Count - 1
                            Dim cmdText As String = ""

                            cmdText += "UPDATE twkswsexecutions SET "
                            cmdText += "ABS_Value = "

                            'Verify if ABS_Value is informed
                            If pExecutionsDS.twksWSExecutions(i).IsABS_ValueNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                'cmdText = cmdText & pExecutionsDS.twksWSExecutions(0).ABS_Value.ToString.Replace(",", ".") & ", " & vbCrLf
                                ' modified by DL 12/03/2010
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).ABS_Value) & ", " & vbCrLf
                            End If

                            'AG 26/07/2010
                            cmdText += "                            ResultDate = "
                            'Verify if ResultDate is informed
                            If pExecutionsDS.twksWSExecutions(i).IsResultDateNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                cmdText = cmdText & " '" & CType(pExecutionsDS.twksWSExecutions(i).ResultDate.ToString(), DateTime).ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                            End If
                            'END AG 26/07/2010

                            cmdText += "                            ABS_Error = "

                            'Verify if ABS_Error is informed
                            If pExecutionsDS.twksWSExecutions(i).IsABS_ErrorNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                cmdText = cmdText & "'" & pExecutionsDS.twksWSExecutions(i).ABS_Error.ToString & "', " & vbCrLf
                            End If

                            cmdText += "                            rkinetics = "

                            'Verify if rkinetics is informed
                            If pExecutionsDS.twksWSExecutions(i).IsrkineticsNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                'cmdText = cmdText & pExecutionsDS.twksWSExecutions(0).rkinetics.ToString.Replace(",", ".") & ", " & vbCrLf
                                'modified by dl 12/03/2010
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).rkinetics) & ", " & vbCrLf
                            End If

                            cmdText += "                            KineticsInitialValue = "

                            'Verify if KineticsInitialValue is informed
                            If pExecutionsDS.twksWSExecutions(i).IsKineticsInitialValueNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                'cmdText = cmdText & pExecutionsDS.twksWSExecutions(0).KineticsInitialValue.ToString.Replace(",", ".") & ", " & vbCrLf
                                ' modified by dl 12/03/2010
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).KineticsInitialValue) & ", " & vbCrLf
                            End If

                            cmdText += "                            KineticsSlope = "

                            'Verify if KineticsSlope is informed
                            If pExecutionsDS.twksWSExecutions(i).IsKineticsSlopeNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                'cmdText = cmdText & pExecutionsDS.twksWSExecutions(0).KineticsSlope.ToString.Replace(",", ".") & ", " & vbCrLf
                                ' modified by dl 12/03/2010
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).KineticsSlope) & ", " & vbCrLf
                            End If

                            cmdText += "                            KineticsLinear = "

                            'Verify if KineticsLinear is informed
                            If pExecutionsDS.twksWSExecutions(i).IsKineticsLinearNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else

                                If pExecutionsDS.twksWSExecutions(i).KineticsLinear = True Then
                                    cmdText = cmdText & "1, " & vbCrLf

                                Else
                                    cmdText = cmdText & "0, " & vbCrLf
                                End If

                            End If

                            cmdText += "                            SubstrateDepletion = "

                            'Verify if SubstrateDepletion is informed
                            If pExecutionsDS.twksWSExecutions(i).IsSubstrateDepletionNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else

                                If pExecutionsDS.twksWSExecutions(i).SubstrateDepletion = True Then
                                    cmdText = cmdText & "1," & vbCrLf

                                Else
                                    cmdText = cmdText & "0," & vbCrLf
                                End If

                            End If

                            cmdText += "                            InUse = "

                            'Verify if InUse is informed
                            If pExecutionsDS.twksWSExecutions(i).IsInUseNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else

                                If pExecutionsDS.twksWSExecutions(i).InUse = True Then
                                    cmdText = cmdText & "1," & vbCrLf

                                Else
                                    cmdText = cmdText & "0," & vbCrLf
                                End If

                            End If

                            cmdText += "                            ABS_Initial = "

                            'Verify if ABS_Initial is informed
                            If pExecutionsDS.twksWSExecutions(i).IsABS_InitialNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                'cmdText = cmdText & pExecutionsDS.twksWSExecutions(0).ABS_Initial.ToString.Replace(",", ".") & ", " & vbCrLf
                                ' modified by dl 12/03/2010
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).ABS_Initial) & ", " & vbCrLf
                            End If

                            'AG 12/07/2010 - add Abs_MainFilter & Abs_WorkReagent
                            cmdText += " ABS_MainFilter = "
                            'Verify if ABS_MainFilter is informed
                            If pExecutionsDS.twksWSExecutions(i).IsABS_MainFilterNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf
                            Else
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).ABS_MainFilter) & ", " & vbCrLf
                            End If

                            cmdText += " ABS_WorkReagent = "
                            'Verify if ABS_WorkReagent is informed
                            If pExecutionsDS.twksWSExecutions(i).IsAbs_WorkReagentNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf
                            Else
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).Abs_WorkReagent) & ", " & vbCrLf
                            End If
                            'END AG 12/07/2010


                            cmdText += "                            CONC_Value = "

                            'Verify if CONC_Value is informed
                            If pExecutionsDS.twksWSExecutions(i).IsCONC_ValueNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                'cmdText = cmdText & pExecutionsDS.twksWSExecutions(0).CONC_Value.ToString.Replace(",", ".") & ", " & vbCrLf
                                ' modified by dl 12/03/2010
                                cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).CONC_Value) & ", " & vbCrLf
                            End If

                            cmdText += "                            CONC_Error = "

                            'Verify if CONC_Error is informed
                            If pExecutionsDS.twksWSExecutions(i).IsCONC_ErrorNull Then
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                cmdText = cmdText & "'" & pExecutionsDS.twksWSExecutions(i).CONC_Error & "', " & vbCrLf
                            End If

                            cmdText += "                            CONC_CurveError = "

                            'Verify if CONC_Error is informed
                            If pExecutionsDS.twksWSExecutions(i).IsCONC_CurveErrorNull Then
                                'AG 20/04/2010 - This is the last parameter
                                'cmdText = cmdText & " NULL, " & vbCrLf
                                cmdText = cmdText & " NULL, " & vbCrLf

                            Else
                                'AG 20/04/2010 - This is the last parameter
                                'cmdText = cmdText & ReplaceNumericString(pExecutionsDS.twksWSExecutions(0).CONC_CurveError) & "', " & vbCrLf
                                cmdText = cmdText & "'" & ReplaceNumericString(pExecutionsDS.twksWSExecutions(i).CONC_CurveError) & "', " & vbCrLf
                            End If

                            cmdText += "                            ExecutionStatus = "

                            'Verify if CONC_Error is informed
                            If pExecutionsDS.twksWSExecutions(i).IsExecutionStatusNull Then
                                cmdText = cmdText & " NULL " & vbCrLf

                            Else
                                cmdText = cmdText & "'" & pExecutionsDS.twksWSExecutions(i).ExecutionStatus & "' " & vbCrLf
                            End If

                            'AG 20/04/2010 - PreparationID isnt update during calculation results
                            'cmdText += "                            PreparationID = "

                            ''Verify if PreparationID is informed
                            'If pExecutionsDS.twksWSExecutions(i).IsPreparationIDNull Then
                            '    cmdText = cmdText & " NULL, " & vbCrLf

                            'Else
                            '    cmdText = cmdText & pExecutionsDS.twksWSExecutions(i).PreparationID.ToString() & vbCrLf
                            'End If

                            cmdText += "Where ExecutionID = " & pExecutionsDS.twksWSExecutions(i).ExecutionID

                            'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                                If resultData.AffectedRecords = 1 Then
                                    resultData.HasError = False
                                Else
                                    resultData.HasError = True
                                    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                                End If
                            End Using
                            'AG 25/07/2014

                            'AG 11/08/2010
                            If resultData.HasError Then Exit For
                        Next
                        'END AG 11/08/2010

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.SaveExecutionsResults", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get the first pending preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleClass">Execution Identifier</param>
        ''' <param name="pStatFlag">Analyzer Identifier</param>
        ''' <param name="pWorkSession" ></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <param name="pExecutionType" ></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by: GDS 19/04/2010
        ''' Modified AG 29/12/2010 - add paused = false condition
        ''' AG 17/01/2011 - add pExecutionType parameter, select executions usign the executiontype in parameter
        ''' </remarks> 
        Public Function GetNextPreparation(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pSampleClass As String, _
                                           ByVal pStatFlag As Boolean, _
                                           ByVal pWorkSession As String, _
                                           ByVal pAnalyzerID As String, ByVal pExecutionType As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim executionDataDS As New ExecutionsDS

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String

                        cmdText = "SELECT TOP 1 ExecutionID" & vbCrLf & _
                                  "  FROM twksWSExecutions" & vbCrLf & _
                                  "  WHERE ExecutionStatus = 'PENDING'" & vbCrLf & _
                                  "    AND Paused = 'False' " & _
                                  "    AND SampleClass = '" & pSampleClass & "'" & vbCrLf & _
                                  "    AND StatFlag = '" & IIf(pStatFlag, "True", "False").ToString & "'" & vbCrLf & _
                                  "    AND WorkSessionID = '" & pWorkSession.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                  "    AND AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                  "    AND ExecutionType = '" & pExecutionType.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                  "  ORDER BY ExecutionID"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetNextPreparation", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get all Executions with status PENDING or LOCKED in the specified Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWorkInRunningMode">Flag indicating if the function is executed when a WorkSession is running in the Analyzer
        '''                                  It is not necessary just now, but it is defined for future use if it is finally needed</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of Pending or Locked Executions in the Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 25/08/2010
        ''' Modified by: AG 19/09/2011 - Function renamed from GetRerunExecutions to GetNotDeletedPendingExecutionDuringWSCreation, 
        '''                              and added parameter pWorkInRunningMode to adapt the function for use working in running mode or not
        '''              SA 09/03/2012 - Changed the function template
        ''' </remarks>
        Public Function GetNotDeletedPendingExecutionDuringWSCreation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                                      ByVal pWorkSessionID As String, ByVal pWorkInRunningMode As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'AG 19/09/2011
                        Dim cmdText As String = " SELECT * FROM twksWSExecutions " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    ExecutionStatus IN ('PENDING', 'LOCKED') " & vbCrLf & _
                                                " ORDER BY SampleClass, OrderTestID, RerunNumber, MultiItemNumber, ReplicateNumber " & vbCrLf

                        'cmdText = " SELECT * FROM twksWSExecutions " & _
                        '          " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & _
                        '          " AND    WorkSessionID = '" & pWorkSessionID & "' " & _
                        '          " AND    ExecutionStatus IN ('PENDING', 'LOCKED') " & _
                        '          " AND    RerunNumber > 1 " & _
                        '          " ORDER BY SampleClass, OrderTestID, RerunNumber, MultiItemNumber, ReplicateNumber "
                        'If Not pWorkInRunningMode Then
                        '    cmdText &= " AND    RerunNumber > 1 "
                        'End If
                        'cmdText &= " ORDER BY SampleClass, OrderTestID, RerunNumber, MultiItemNumber, ReplicateNumber "
                        'AG 19/09/2011

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetNotDeletedPendingExecutionDuringWSCreation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetExecutionOrderTestOrderStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                         ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT EX.WorkSessionID, EX.AnalyzerID, EX.ExecutionID, EX.SampleClass, EX.OrderTestID, EX.RerunNumber, EX.ExecutionStatus, " & vbCrLf & _
                                                       " OT.OrderID, OT.OrderTestStatus, O.OrderStatus " & vbCrLf & _
                                                " FROM   twksWSExecutions AS EX INNER JOIN twksOrderTests AS OT ON EX.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                              " INNER JOIN twksOrders AS O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                " WHERE  EX.ExecutionID   = " & pExecutionID.ToString & vbCrLf & _
                                                " AND    EX.AnalyzerID    = '" & pAnalyzerID.ToString & "' " & vbCrLf & _
                                                " AND    EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf

                        Dim myDS As New UIRefreshDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDS.ExecutionStatusChanged)
                            End Using
                        End Using

                        resultData.SetDatos = myDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionOrderTestOrderStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Execution for the first Replicate of the specified OrderTest/RerunNumber (due to its Results have been exported to LIMS)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pReRunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the group of affected Executions</returns>
        ''' <remarks>
        ''' Created by:  TR 12/07/2012
        ''' Modified by: SA 01/08/2012 - Added parameters and filters by OrderTestID and RerunNumber; get data from view vwksWSExecutionsMonitor
        '''                              instead of from table twksWSExecutions to allow get also Calculated and OffSystem "fake" Executions.
        '''                              Get only the Execution for the first Replicate of the informed OrderTest/RerunNumber
        ''' Modified by: SG 06/03/2013 - Add Inner Join with twksOrderTests
        ''' Modified by AG 30/07/2014 - #1887 be sure all CALC/OFFS tests appear in data to export
        ''' </remarks>
        Public Function GetExportedExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                              ByVal pOrderTestID As Integer, ByVal pReRunNumber As Integer, Optional ByVal pUseCALCOFFviewFlag As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'AG 30/07/2014 - #1887 - Classical view
                        Dim cmdText As String = String.Empty
                        If Not pUseCALCOFFviewFlag Then
                            'SGM 06/03/2013 - inner join with order test table
                            cmdText = " SELECT * FROM vwksWSExecutionsMonitor VEM " & vbCrLf & _
                                                    " INNER JOIN twksOrderTests TOT ON VEM.OrderTestID = TOT.OrderTestID " & vbCrLf & _
                                                    " WHERE  VEM.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                    " AND    VEM.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                    " AND    VEM.OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                    " AND    VEM.RerunNumber = " & pReRunNumber.ToString & vbCrLf & _
                                                    " AND    VEM.ReplicateNumber = 1 " & vbCrLf

                            'OrderTest not found in classical view, use the new developed in v300
                        Else
                            cmdText = "SELECT 1 As RerunNumber, VOC.*, OT.*, O.SampleID, O.PatientID FROM vwksMonitorWSTabOFFSCALC VOC" & vbCrLf & _
                                      " INNER JOIN twksOrderTests OT ON VOC.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                      " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                      " WHERE VOC.OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                      " AND VOC.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                        End If
                        'AG 30/07/2014 - #1887

                        Dim myExecutionsDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myExecutionsDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myExecutionsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExportedExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created by:  XB 21/03/2013
        ''' </remarks>
        Public Function GetLockedByLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search the last created ExecutionID
                        Dim cmdText As String = " SELECT LockedByLIS  " & vbCrLf & _
                                                " FROM   twksWSExecutions  " & vbCrLf & _
                                                " WHERE  OrderTestID  = " & pOrderTestID & " " & vbCrLf & _
                                                " AND    RerunNumber  = " & pRerunNumber

                        'Execute the SQL sentence 
                        Dim dbDataReader As SqlClient.SqlDataReader
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = Convert.ToBoolean(dbDataReader.Item("LockedByLIS"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetLockedByLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSExecutions SET LockedByLIS = 0 " & vbCrLf
                    cmdText &= " WHERE OrderTestID = '" & pOrderTestID.ToString & "' " & vbCrLf
                    cmdText &= " AND RerunNumber = '" & pRerunNumber.ToString & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UnlockLISExecutions", EventLogEntryType.Error, False)
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
                        'Search the max one of Reruns for that OrderTestID
                        Dim cmdText As String = " SELECT MAX(RerunNumber)  " & vbCrLf & _
                                                " FROM   twksWSExecutions  " & vbCrLf & _
                                                " WHERE  OrderTestID  = " & pOrderTestID

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetMaxRerunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data for fill screen monitor tab WS (tests STD, ISE)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>ExecutionsDS.vwksWSExecutionsMonitor</returns>
        ''' <remarks>AG 2702/2014 - Created - #1524</remarks>
        Public Function GetDataSTDISEForMonitorTabWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = "SELECT * FROM vwksMonitorWSTabSTDISE " & vbCrLf
                        'cmdText &= " WHERE WorkSessionID = '" & pWorkSessionID.ToString & "' " & vbCrLf
                        'cmdText &= " AND AnalyzerID = '" & pAnalyzerID.ToString & "' " & vbCrLf
                        cmdText &= " ORDER BY ExecutionID ASC"

                        Dim myDataSet As New ExecutionsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.vwksWSExecutionsMonitor)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetDataSTDISEForMonitorTabWS", EventLogEntryType.Error, False)
            Finally
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data for fill screen monitor tab WS (tests OFFS, CALC)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>ExecutionsDS.vwksWSExecutionsMonitor</returns>
        ''' <remarks>AG 2702/2014 - Created - #1524</remarks>
        Public Function GetDataOFFSCALCForMonitorTabWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = "SELECT * FROM vwksMonitorWSTabOFFSCALC " & vbCrLf
                        'cmdText &= " WHERE WorkSessionID = '" & pWorkSessionID.ToString & "' " & vbCrLf
                        'cmdText &= " AND AnalyzerID = '" & pAnalyzerID.ToString & "' " & vbCrLf
                        cmdText &= " ORDER BY OrderID ASC"

                        Dim myDataSet As New ExecutionsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.vwksWSExecutionsMonitor)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetDataOFFSCALCForMonitorTabWS", EventLogEntryType.Error, False)
            Finally
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
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS OpenExecutions " & vbCrLf & _
                                                " FROM   twksWSExecutions EX " & vbCrLf & _
                                                " WHERE  EX.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf

                        If pStatus.Length > 0 Then
                            cmdText &= " AND    EX.ExecutionStatus = '" & pStatus & "' "
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.CountByExecutionStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


#End Region

#Region "Private Methods"
        Private Function GetCmdTextForAlarmAdditionalInfo(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As String
            Dim cmdText As String = ""

            cmdText &= " SELECT ex.SampleClass, "
            cmdText &= " (CASE WHEN ex.SampleClass  = 'BLANK' THEN te.BlankMode "
            cmdText &= "  WHEN EX.SampleClass = 'CALIB' THEN ca.CalibratorName "
            cmdText &= "  WHEN EX.SampleClass = 'CTRL' THEN co.ControlName "
            cmdText &= "  WHEN EX.SampleClass = 'PATIENT' THEN (CASE WHEN od.PatientID IS NOT NULL THEN od.PatientID ELSE od.SampleID END) END) As Name, "
            cmdText &= " (CASE WHEN ot.TestType = 'STD' THEN te.TestName ELSE IT.Name END) As TestName, "
            cmdText &= " EX.ReplicateNumber, ca.NumberOfCalibrators, ex.MultiItemNumber "
            cmdText &= " FROM twksWSExecutions ex INNER JOIN twksOrderTests ot ON ex.OrderTestID = ot.OrderTestID "
            cmdText &= "   INNER JOIN twksOrders od ON ot.OrderID = od.OrderID  "
            cmdText &= "   LEFT OUTER JOIN tparControls co ON ot.ControlID = co.ControlID "
            cmdText &= "   LEFT OUTER JOIN tparTestCalibrators tc ON ot.TestID = tc.TestID AND ot.SampleType = tc.SampleType "
            cmdText &= "   LEFT OUTER JOIN tparCalibrators ca ON tc.CalibratorID = ca.CalibratorID "
            cmdText &= "   LEFT OUTER JOIN tparTests  te ON ot.TestID = te.TestID "
            cmdText &= "   LEFT OUTER JOIN tparISETests it ON ot.TestID = it.ISETestID "
            cmdText &= " WHERE EX.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' "
            cmdText &= " AND EX.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

            If pExecutionID <> -1 Then
                cmdText &= " AND EX.ExecutionID = " & pExecutionID
            End If

            Return cmdText
        End Function

        Private Function GetCmdTextForAlarmAdditionalInfo(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                    ByVal pExecutionID As Integer, ByVal pReagentNumber As Integer) As String
            Dim cmdText As String = ""

            cmdText &= " WITH Data As ("
            cmdText &= "   SELECT ex.SampleClass, '' As Name, r.ReagentName As TestName, ex.ReplicateNumber, re.TubeContent, re.SolutionCode,"
            cmdText &= "          ROW_NUMBER() OVER(ORDER BY ex.SampleClass) AS RowNumber "
            cmdText &= "   FROM twksWSExecutions ex INNER JOIN twksOrderTests ot ON ex.OrderTestID = ot.OrderTestID "
            cmdText &= "                            INNER JOIN twksOrders od ON ot.OrderID = od.OrderID "
            cmdText &= "                            INNER JOIN tparTests  te ON ot.TestID = te.TestID "
            cmdText &= "                            INNER JOIN tparTestReagents tr ON te.TestID = tr.TestID "
            cmdText &= "                            INNER JOIN tparReagents r ON tr.ReagentID = r.ReagentID "
            cmdText &= "                            INNER JOIN twksWSRequiredElemByOrderTest reot ON ot.OrderTestID = reot.OrderTestID "
            cmdText &= "                            INNER JOIN twksWSRequiredElements re ON reot.ElementID = re.ElementID "
            cmdText &= "   WHERE ex.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' "
            cmdText &= "   AND ex.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

            If pExecutionID <> -1 Then
                cmdText &= " AND EX.ExecutionID = " & pExecutionID
            End If

            cmdText &= " ) "
            cmdText &= " SELECT SampleClass, Name, TestName, ReplicateNumber, TubeContent, SolutionCode "
            cmdText &= " FROM Data "
            cmdText &= " WHERE RowNumber = " & pReagentNumber

            Return cmdText
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
        Public Function GetExecutionsForHistoricTable(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                      ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT E.AnalyzerID, E.WorkSessionID, E.MultiItemNumber AS MultipointNumber, E.ReplicateNumber, " & vbCrLf & _
                                                       " E.PostDilutionType, E.ResultDate AS ResultDateTime, E.ABS_Value AS ABSValue, E.CONC_Value AS CONCValue, " & vbCrLf & _
                                                       " E.ABS_Initial AS ABSInitial, E.ABS_MainFilter AS ABSMainFilter, E.ABS_WorkReagent AS ABSWorkReagent, " & vbCrLf & _
                                                       " E.rKinetics, E.KineticsLinear, E.KineticsInitialValue, E.KineticsSlope, E.SubstrateDepletion, " & vbCrLf & _
                                                       " E.BaseLineID, E.WellUsed, E.AdjustBaseLineID, E.ExecutionID, E.OrderTestID " & vbCrLf & _
                                                " FROM twksWSExecutions E INNER JOIN twksResults R ON E.OrderTestID = R.OrderTestID " & vbCrLf & _
                                                                                                " AND E.RerunNumber = R.RerunNumber " & vbCrLf & _
                                                                                                " AND E.MultiItemNumber = R.MultiPointNumber " & vbCrLf & _
                                                                       "  INNER JOIN twksOrderTests OT ON E.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                    " AND E.AnalyzerID = OT.AnalyzerID " & vbCrLf & _
                                                " WHERE E.AnalyzerID       = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND   E.WorkSessionID    = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND   E.SampleClass     <> 'CTRL' " & vbCrLf & _
                                                " AND E.ExecutionStatus    = 'CLOSED' " & vbCrLf & _
                                                " AND R.ValidationStatus   = 'OK' " & vbCrLf & _
                                                " AND R.AcceptedResultFlag = 1 " & vbCrLf & _
                                                " AND OT.OrderTestStatus   = 'CLOSED' " & vbCrLf

                        Dim myHisWSExecutionsDS As New HisWSExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisWSExecutionsDS.thisWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myHisWSExecutionsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionsForHistoricTable", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "METHODS REPLACED FOR NEW ONES DUE TO PERFORMANCE ISSUES"
        ''' <summary>
        ''' Delete all Executions for a group of Order Tests that fulfill the following condition: ALL their Executions 
        ''' have Execution Status PENDING or LOCKED 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' 
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 22/04/2010
        ''' Modified by: DL  11/05/2010 - Added optional parameter to filter Executions by OrderTestID
        '''              DL  11/05/2010 - When an OrderTestID has not been informed, delete only Pending and/or Locked
        '''                               Executions that do not correspond to a Rerun request
        '''              AG  20/09/2011 - Deletion affects only OrderTests having all his executions PENDING or LOCKED
        '''              SA  29/11/2011 - Changed the SQL Query to improve the readibility 
        '''              SA  31/05/2012 - Implementation changed due to bad performance of the previous query: a DS with the list of 
        '''                               OrderTestIDs which executions can be deleted (because all of them have status PENDING or LOCKED)  
        '''                               is received as entry parameter and the SQL Query was changed to delete executions by OrderTestID
        ''' </remarks>
        Public Function DeleteNotInCurseExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pOrderTestsListDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0
                    Dim maxUpdates As Integer = 500
                    Dim cmdText As New StringBuilder()

                    For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In pOrderTestsListDS.twksOrderTests
                        cmdText.Append(" DELETE FROM twksWSExecutions ")
                        cmdText.Append(" WHERE  AnalyzerID = ")
                        cmdText.AppendFormat("'{0}'", pAnalyzerID.ToString)
                        cmdText.Append(" AND    WorkSessionID = ")
                        cmdText.AppendFormat("'{0}'", pWorkSessionID.ToString)
                        cmdText.Append(" AND    OrderTestID = ")
                        cmdText.AppendFormat("{0}", orderTestRow.OrderTestID)
                        cmdText.Append(vbCrLf)

                        i += 1
                        If (i = maxUpdates) Then
                            'Execute the SQL script
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using

                            'Initialize the counter and the StringBuilder
                            i = 0
                            cmdText.Remove(0, cmdText.Length)
                        End If
                    Next orderTestRow

                    If (Not resultData.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining deletes...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.DeleteNotInCurseExecutions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Find the CLOSED Executions having the maximum ReplicateNumber for the same OrderTestID and RerunNumber. Executions of all MultiItemNumbers
        ''' are returned excepting when parameter pOnlyMaxReplicate is TRUE, in which case, only the maximum MultiItemNumber for the Replicate is returned 
        ''' of the informed ExecutionID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pOnlyMaxReplicate">Optional parameter. When TRUE, only the maximum MultiItemNumber for the Replicate is returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of all obtained Executions</returns>
        ''' <remarks>
        ''' Created by:  AG 23/07/2010
        ''' Modified by: AG 21/10/2011 - Changed the SQL by adding a filter by ExecutionStatus = 'CLOSED'</remarks>
        Public Function GetClosedExecutionsRelated(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pExecutionId As Integer, Optional ByVal pOnlyMaxReplicate As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        If (pOnlyMaxReplicate) Then
                            cmdText = "SELECT TOP 1 " 'Only 1 record
                        Else
                            cmdText = "SELECT " 'All records
                        End If

                        cmdText += " b.* FROM vwksWSExecutionsResults a, vwksWSExecutionsResults b "
                        cmdText += " WHERE  a.ExecutionID = " & pExecutionId & " "
                        cmdText += " AND  a.AnalyzerID = '" & pAnalyzerID & "' "
                        cmdText += " AND a.WorkSessionID = '" & pWorkSessionID & "' "
                        cmdText += " AND a.ExecutionStatus = 'CLOSED' " 'AG 21/10/2011
                        cmdText += " AND a.OrderTestID = b.OrderTestID AND a.RerunNumber = b.RerunNumber "
                        cmdText += " AND a.ExecutionStatus = b.ExecutionStatus " 'AG 21/10/2011
                        cmdText += " ORDER BY b.MultiItemNumber DESC, b.ReplicateNumber DESC"

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, ".twksWSExecutionsDAO.GetClosedExecutionsRelated", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get a list with execution ids from multitem execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed execution</returns>
        ''' <remarks>
        ''' Created by:  DL 23/02/2010
        ''' Modified by: AG 03/08/2010 - change the SQL query (the old fails when there are calibrator multiitem reruns)
        ''' Modified by: AG 24/02/2011 - return also the ReplicateNumber field
        ''' AG 21/10/2011 - select also RerunNumber
        ''' AG 21/12/2011 - select also WellUsed
        ''' </remarks>
        Public Function GetExecutionsMultiItem(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pExecutionID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim executionDataDS As New ExecutionsDS

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim Cmd As New SqlClient.SqlCommand
                        Dim cmdText As String = ""

                        'AG 03/08/2010
                        'cmdText += "Select ExecutionID , WorkSessionID, AnalyzerID, OrdertestID, MultiItemNumber "
                        'cmdText += "From   twksWSExecutions "
                        'cmdText += "Where  OrderTestID in    (Select OrderTestID "
                        'cmdText += "                          From twksWSExecutions "
                        'cmdText += "                          Where ExecutionID = " & pExecutionID & ") "
                        'cmdText += "  and ReplicateNumber in (Select ReplicateNumber "
                        'cmdText += "                          From twksWSExecutions "
                        'cmdText += "                          Where ExecutionID = " & pExecutionID & ") "
                        'cmdText += "ORDER BY MultiItemNumber ASC"

                        cmdText += "SELECT a.ExecutionID , a.WorkSessionID, a.AnalyzerID, a.OrdertestID, a.MultiItemNumber, a.ReplicateNumber, a.RerunNumber, a.WellUsed "
                        cmdText += " FROM twksWSExecutions a, twksWSExecutions b "
                        cmdText += " WHERE b.ExecutionID = " & pExecutionID
                        cmdText += " AND a.OrderTestID = b.OrderTestID "
                        cmdText += " AND a.ReplicateNumber = b.ReplicateNumber"
                        cmdText += " AND a.RerunNumber = b.RerunNumber "
                        cmdText += " ORDER BY MultiItemNumber ASC"
                        'END AG 03/08/2010

                        'Cmd.Connection = dbConnection
                        'Cmd.CommandText = cmdText

                        ''Fill the DataSet to return 
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(Cmd)
                        'dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionsMultiItem", EventLogEntryType.Error, False)

            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Search the affected CLOSED executions for the new blank result
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pSampleClass"></param>
        ''' <param name="pTestID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by AG 23/07/2010
        ''' AG 04/01/2011 - add executiontype = prep_std
        ''' AG 21/10/2011 add condition into Where: And ExecutionStatus = 'CLOSED'
        ''' </remarks>
        Public Function GetExecutionsAffectedByNewBlank(ByVal pDBConnection As SqlClient.SqlConnection, _
                                             ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             ByVal pSampleClass As String, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText += "SELECT * FROM vwksWSExecutionsResults "
                        cmdText += " WHERE AnalyzerID = '" & pAnalyzerID & "' "
                        cmdText += " AND WorkSessionID = '" & pWorkSessionID & "' "
                        cmdText += " AND SampleClass <> '" & pSampleClass & "' "
                        cmdText += " AND TestID = " & pTestID & " "
                        cmdText += " AND ExecutionStatus = 'CLOSED' " 'AG 21/10/2011
                        cmdText += " AND ExecutionType = 'PREP_STD'"
                        cmdText += " ORDER BY SampleClass, ExecutionID"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim executionDataDS As New ExecutionsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionsAffectedByNewBlank", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Search all closed CTRL and PATIENT Executions affected by a new Calibrator result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pSampleClass"></param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">SampleType Code</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 23/07/2010
        ''' Modified by: AG 02/09/2010 - Changed the Query to filter data by SampleType or SampleTypeAlternative
        '''              AG 04/01/2011 - Changed the Query to get only Executions corresponding to Standard Tests (ExecutionType=PREP_STD) 
        '''              AG 21/10/2011 - Changed the Query to get only CLOSED Executions
        ''' </remarks>
        Public Function GetExecutionsAffectedByNewCalib(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                        ByVal pWorkSessionID As String, ByVal pSampleClass As String, ByVal pTestID As Integer, _
                                                        ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vwksWSExecutionsResults " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    ExecutionStatus = 'CLOSED' " & vbCrLf & _
                                                " AND    ExecutionType = 'PREP_STD' " & vbCrLf & _
                                                " AND    SampleClass <> 'BLANK' " & vbCrLf & _
                                                " AND    SampleClass <> '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND   (SampleType = '" & pSampleType.Trim & "' OR SampleTypeAlternative = '" & pSampleType.Trim & "') " & vbCrLf & _
                                                " ORDER BY SampleClass, ExecutionID " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionsAffectedByNewCalib", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the maximum MultiItem Number for the specified Execution 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of the specified ExecutionID</returns>
        ''' <remarks>
        ''' Created  by: DL 23/02/2010
        ''' Modified by: DL 24/02/2010
        '''              AG 26/04/2011 - Changed the query to ANSI format to improve the speed 
        '''              SA 09/03/2012 - Changed the function template
        ''' </remarks>''' 
        Public Function GetNumberOfMultititem(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT " & pExecutionID & " As ExecutionID, EX.WorkSessionID, EX.AnalyzerID, EX.OrderTestID, EX.RerunNumber, " & vbCrLf & _
                                                         " MAX(EX2.MultiItemNumber) as MultiItemNumber, EX.ExecutionStatus, EX.ExecutionType, EX.SampleClass " & vbCrLf & _
                                                " FROM     twksWSExecutions EX INNER JOIN twksWSExecutions EX2 ON EX.OrderTestID = EX2.OrderTestID  " & vbCrLf & _
                                                " WHERE    EX.ExecutionID = " & pExecutionID & vbCrLf & _
                                                " GROUP BY EX.WorkSessionID, EX.AnalyzerID, EX.OrderTestID, EX.RerunNumber, EX.ExecutionStatus, EX.ExecutionType, EX.SampleClass " & vbCrLf

                        Dim myExecutionDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myExecutionDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myExecutionDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetNumberOfMultiitem", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the InUse (flag)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 23/07/2010
        ''' </remarks>
        Public Function UpdateInUse(ByVal pDBConnection As SqlClient.SqlConnection, _
                              ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Try

                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If pExecutionsDS.twksWSExecutions.Rows.Count < 1 Then
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString

                    ElseIf pExecutionsDS.twksWSExecutions(0).IsInUseNull Then
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    Else
                        Dim cmdText As String = ""
                        cmdText += "UPDATE twkswsexecutions SET "

                        ''Verify if PreparationID is informed
                        If Not pExecutionsDS.twksWSExecutions(0).IsInUseNull Then
                            cmdText += " InUse = "
                            If pExecutionsDS.twksWSExecutions(0).InUse = True Then
                                cmdText += "1 "
                            Else
                                cmdText += "0 "
                            End If
                        End If

                        cmdText += " Where ExecutionID = " & pExecutionsDS.twksWSExecutions(0).ExecutionID
                        cmdText += " And AnalyzerID = '" & pExecutionsDS.twksWSExecutions(0).AnalyzerID & "' "
                        cmdText += " And WorkSessionID = '" & pExecutionsDS.twksWSExecutions(0).WorkSessionID & "' "

                        'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            If resultData.AffectedRecords = 1 Then
                                resultData.HasError = False
                            Else
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                            End If
                        End Using
                        'AG 25/07/2014

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateInUse", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Update the fields BaseLineID, WellUsed and RotorTurnNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionsDS">Executions DataSet with the following fields informed (ExecutionID, BaseLineID, WellUsed, RotorTurnNumber)</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS - 27/04/2010
        ''' Modified by AG: 03/05/2010 (RotorTurnNumber is deleted from database)
        ''' AG 03/01/2011 - update also AdjustBaseLineID field
        ''' AG 19/05/2011 - update also HasReadings field
        ''' </remarks>
        Public Function UpdateReadingsFields(ByVal pDBConnection As SqlClient.SqlConnection, _
                                             ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String

                    If pExecutionsDS.twksWSExecutions.Rows.Count = 1 Then
                        'AG 03/05/2010 - RotorTurnNumber field is deleted from database
                        'cmdText = "UPDATE twksWSExecutions" & vbCrLf & _
                        '          "  SET BaseLineID = " & pExecutionsDS.twksWSExecutions(0).BaseLineID & vbCrLf & _
                        '          "    , WellUsed = " & pExecutionsDS.twksWSExecutions(0).WellUsed & vbCrLf & _
                        '          "    , RotorTurnNumber = " & pExecutionsDS.twksWSExecutions(0).RotorTurnNumber & vbCrLf & _
                        '          " WHERE ExecutionID = " & pExecutionsDS.twksWSExecutions(0).ExecutionID
                        cmdText = "UPDATE twksWSExecutions" & vbCrLf & _
                                  "  SET BaseLineID = " & pExecutionsDS.twksWSExecutions(0).BaseLineID & vbCrLf & _
                                  "    , WellUsed = " & pExecutionsDS.twksWSExecutions(0).WellUsed & vbCrLf & _
                                  "    , AdjustBaseLineID = " & pExecutionsDS.twksWSExecutions(0).AdjustBaseLineID & vbCrLf & _
                                  "    , HasReadings = " & CStr(IIf(pExecutionsDS.twksWSExecutions(0).HasReadings, "1", "0")) & vbCrLf & _
                                  " WHERE ExecutionID = " & pExecutionsDS.twksWSExecutions(0).ExecutionID

                        'Execute the SQL sentence 
                        'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                        'AG 25/07/2014

                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateReadingsFields", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

#End Region

#Region "NEW METHODS FOR PERFORMANCE IMPROVEMENTS"

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
        ''' Created by:  DL 16/07/2012 - Optimization of function DeleteNotInCurseExecutions to improve the performance of the group
        '''                              of DELETES to execute
        ''' Modified by: XB 03/03/2014 - Task #1530 improvements memory app/sql 
        ''' </remarks>
        Public Function DeleteNotInCourseExecutionsNEW(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                      ByVal pOrderTestsListDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0

                    ' XB 03/03/2014
                    Dim maxUpdates As Integer = 100 ' 199
                    ' XB 03/03/2014

                    Dim cmdText As String = ""
                    Dim firstRow As Boolean = True

                    If (Not pOrderTestsListDS Is Nothing AndAlso pOrderTestsListDS.twksOrderTests.Rows.Count > 0) Then
                        For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In pOrderTestsListDS.twksOrderTests
                            ' XB 03/03/2014
                            'If (firstRow) Then
                            '    cmdText &= "DELETE FROM twksWSExecutions" & vbCrLf
                            '    cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                            '    cmdText &= "   AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
                            '    cmdText &= "   AND OrderTestID IN (" & orderTestRow.OrderTestID

                            '    firstRow = False
                            'Else
                            '    cmdText &= ", " & orderTestRow.OrderTestID
                            'End If

                            'i += 1
                            'If (i = maxUpdates) Then
                            '    cmdText &= ")"

                            '    'Execute the SQL script
                            '    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            '        resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                            '    End Using

                            '    'Initialize the counter and the StringBuilder
                            '    i = 0
                            '    cmdText = ""
                            '    firstRow = True
                            'End If

                            If (firstRow) Then
                                cmdText &= "DELETE FROM twksWSExecutions" & vbCrLf
                                cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                                cmdText &= "   AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
                                cmdText &= "   AND ( OrderTestID = " & orderTestRow.OrderTestID

                                firstRow = False
                            Else
                                cmdText &= " OR OrderTestID = " & orderTestRow.OrderTestID
                            End If

                            i += 1
                            If (i = maxUpdates) Then
                                cmdText &= ")"

                                'Execute the SQL script
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                    resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                                End Using

                                'Initialize the counter and the StringBuilder
                                i = 0
                                cmdText = ""
                                firstRow = True
                            End If
                            ' XB 03/03/2014
                        Next orderTestRow

                        If (Not resultData.HasError) Then
                            If (cmdText.Length > 0) Then
                                cmdText &= ")"
                                'Execute the remaining deletes...
                                Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                    resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                                End Using
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.DeleteNotInCourseExecutionsNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

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
        ''' Created by: SA 09/07/2012 - Based in GetClosedExecutionsRelated but changing the entry parameter ExecutionID by OrderTestID and RerunNumber
        '''                             and simplifying the query
        ''' </remarks>
        Public Function GetClosedExecutionsRelatedNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                      ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                                      Optional ByVal pOnlyMaxReplicate As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT "
                        If (pOnlyMaxReplicate) Then cmdText &= " TOP 1 " 'Only 1 record

                        cmdText &= " E.* " & vbCrLf & _
                                   " FROM     vwksWSExecutionsResults E " & vbCrLf & _
                                   " WHERE    E.OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                   " AND      E.RerunNumber = " & pRerunNumber.ToString & vbCrLf & _
                                   " AND      E.AnalyzerID  = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                   " AND      E.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                   " AND      E.ExecutionStatus = 'CLOSED' " & vbCrLf & _
                                   " ORDER BY E.MultiItemNumber DESC, E.ReplicateNumber DESC " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, ".twksWSExecutionsDAO.GetClosedExecutionsRelatedNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all closed CTRL and PATIENT Executions affected by a new Blank or Calibrator result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">SampleType Code. Informed only to search Executions affected for a new Calibrator result</param>
        ''' <returns>GlobalDataTO containing a typed DataSet with the list of affected Executions</returns>
        ''' <remarks>
        ''' Created by:  SA 13/07/2012 
        ''' </remarks>
        Public Function GetExecutionsAffectedByNewBlankOrCalib(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                               ByVal pWorkSessionID As String, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "") _
                                                               As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT  WSE.ExecutionID, WSE.ExecutionStatus,  WSE.WorkSessionID, WSE.AnalyzerID, WSE.BaseLineID, WSE.WellUsed, " & vbCrLf & _
                                                       "  WSE.OrderTestID, WSE.PostDilutionType, WSE.ReplicateNumber, WSE.RerunNumber, WSE.MultiItemNumber, " & vbCrLf & _
                                                       "  WSE.SampleClass, WSE.AdjustBaseLineID, WSE.ThermoWarningFlag, WSE.ClotValue, WSE.ExecutionType,  " & vbCrLf & _
                                                       "  T.TestID, T.TestName, WSE.ValidReadings, WSE.CompleteReadings, OT.TestType, OT.SampleType, " & vbCrLf & _
                                                       "  OT.ReplicatesNumber AS ReplicatesTotalNum, OT.ControlID, OT.OrderID, O.SampleID " & vbCrLf & _
                                                " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                            " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                            " INNER JOIN tparTests T ON OT.TestID = T.TestID AND OT.TestType = 'STD' " & vbCrLf & _
                                                " WHERE  WSE.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WSE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    OT.TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    WSE.ExecutionStatus = 'CLOSED' " & vbCrLf & _
                                                " AND    WSE.ExecutionType = 'PREP_STD' " & vbCrLf & _
                                                " AND   (WSE.SampleClass = 'CTRL' OR WSE.SampleClass = 'PATIENT') " & vbCrLf

                        If (pSampleType <> String.Empty) Then cmdText &= " AND (OT.SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                                         "  OR  OT.SampleType IN (SELECT TS.SampleType FROM tparTestSamples TS " & vbCrLf & _
                                                                                                " WHERE  TS.TestID = " & pTestID.ToString & vbCrLf & _
                                                                                                " AND    TS.SampleTypeAlternative = '" & pSampleType.Trim & "')) " & vbCrLf
                        cmdText &= " ORDER BY WSE.SampleClass, WSE.ExecutionID " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        'Dim cmdText As String = " SELECT * FROM vwksWSExecutionsResults " & vbCrLf & _
                        '                        " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                        '                        " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                        " AND    TestID = " & pTestID.ToString & vbCrLf & _
                        '                        " AND    ExecutionStatus = 'CLOSED' " & vbCrLf & _
                        '                        " AND    ExecutionType = 'PREP_STD' " & vbCrLf & _
                        '                        " AND   (SampleClass = 'CTRL' OR SampleClass = 'PATIENT') " & vbCrLf & _
                        '                        " ORDER BY SampleClass, ExecutionID " & vbCrLf

                        'Dim executionDataDS As New ExecutionsDS
                        'Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        '    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        '        dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)
                        '    End Using
                        'End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionsAffectedByNewBlankOrCalib", EventLogEntryType.Error, False)
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
        ''' Created by: SA 03/07/2012 - New implementation of function GetExecutionsMultiItem; changes to improve application perfomance
        ''' </remarks>
        Public Function GetExecutionMultiItemsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                                  ByVal pReplicateNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WSE.ExecutionID, WSE.ExecutionStatus,  WSE.WorkSessionID, WSE.AnalyzerID, WSE.BaseLineID, WSE.WellUsed, " & vbCrLf & _
                                                       " WSE.OrderTestID, WSE.PostDilutionType, WSE.ReplicateNumber, WSE.RerunNumber, WSE.MultiItemNumber, " & vbCrLf & _
                                                       " WSE.SampleClass, WSE.AdjustBaseLineID, WSE.ThermoWarningFlag, WSE.ClotValue, WSE.ExecutionType,  " & vbCrLf & _
                                                       " T.TestID, T.TestName, WSE.ValidReadings, WSE.CompleteReadings, OT.TestType, OT.SampleType, " & vbCrLf & _
                                                       " OT.ReplicatesNumber AS ReplicatesTotalNum, OT.ControlID, OT.OrderID, O.SampleID " & vbCrLf & _
                                                " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                            " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                            " INNER JOIN tparTests T ON OT.TestID = T.TestID AND OT.TestType = 'STD' " & vbCrLf & _
                                                " WHERE    WSE.AnalyzerID      = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND      WSE.WorkSessionID   =  '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND      WSE.OrderTestID     = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND      WSE.RerunNumber     = " & pRerunNumber.ToString & vbCrLf & _
                                                " AND      WSE.ReplicateNumber = " & pReplicateNumber.ToString & vbCrLf & _
                                                " ORDER BY WSE.MultiItemNumber " & vbCrLf

                        Dim myMultiItemExecutionsDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myMultiItemExecutionsDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myMultiItemExecutionsDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionMultiItemsNEW", EventLogEntryType.Error, False)
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
                        Dim cmdText As String = " SELECT MAX(MultiItemNumber) AS MultiItemNumber " & vbCrLf & _
                                                " FROM   twksWSExecutions " & vbCrLf & _
                                                " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    OrderTestID   = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    RerunNumber   = " & pRerunNumber.ToString & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetNumberOfMultiItemNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all distinct SampleClass / OrderTestID / RerunNumber having Executions with status PENDING or LOCKED in the specified Analyzer Work Session (when StandBy or pause mode)
        ''' Get all distinct SampleClass / OrderTestID / RerunNumber having Executions with status LOCKED in the specified Analyzer Work Session (when Running normal)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWorkInRunningMode"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of distinct SampleClass / OrderTestID / RerunNumber 
        '''          having Executions with status Pending or Locked in the Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 05/07/2012
        ''' AG 25/03/2013 - return also LockedByLIS
        ''' AG 30/05/2014 - #1644 add parameter pPauseMode
        ''' </remarks>
        Public Function GetPendingAndLockedGroupedExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                             ByVal pWorkSessionID As String, ByVal pWorkInRunningMode As Boolean, ByVal pPauseMode As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim cmdText As String = " SELECT DISTINCT AnalyzerID, WorkSessionID, SampleClass, OrderTestID, RerunNumber, ExecutionType " & vbCrLf & _
                        '                        " FROM   twksWSExecutions " & vbCrLf & _
                        '                        " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                        '                        " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                        " AND   (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED') " & vbCrLf & _
                        '                        " ORDER BY SampleClass, OrderTestID, RerunNumber " & vbCrLf

                        Dim cmdText As String = " SELECT DISTINCT AnalyzerID, WorkSessionID, SampleClass, OrderTestID, RerunNumber, ExecutionType, LockedByLIS " & vbCrLf & _
                                                " FROM   twksWSExecutions " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                        If Not pWorkInRunningMode OrElse pPauseMode Then 'AG 30/05/2014 - #1644 in standBy or in pause mode
                            cmdText += " AND   (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED') " & vbCrLf
                        Else 'In normal running
                            cmdText += " AND   (ExecutionStatus = 'LOCKED') " & vbCrLf
                        End If
                        cmdText += " ORDER BY SampleClass, OrderTestID, RerunNumber " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetPendingAndLockedGroupedExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Calibrator Executions for the specified Analyzer/WorkSession/OrderTestID/MultiItemNumber and ReplicateNumber 
        ''' (for all existing RerunNumbers)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pMultiItemNumber">MultiItem Number (the one for the last Calibrator Point)</param>
        ''' <param name="pReplicateNumber">Replicate Number (the maximum Replicate Number for the Calibrator when used for the Test/SampleType</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the list of Calibrator Executions for the specified 
        '''          Analyzer/WorkSession/OrderTestID/MultiItemNumber and ReplicateNumber (for all existing RerunNumbers)</returns>
        ''' <remarks>
        ''' Created by: SA 09/07/2012
        ''' </remarks>
        Public Function ReadAffectedCalibExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                    ByVal pOrderTestID As Integer, ByVal pMultiItemNumber As Integer, ByVal pReplicateNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT  WSE.ExecutionID, WSE.ExecutionStatus,  WSE.WorkSessionID, WSE.AnalyzerID, WSE.BaseLineID, WSE.WellUsed, " & vbCrLf & _
                                                       "  WSE.OrderTestID, WSE.PostDilutionType, WSE.ReplicateNumber, WSE.RerunNumber, WSE.MultiItemNumber, " & vbCrLf & _
                                                       "  WSE.SampleClass, WSE.AdjustBaseLineID, WSE.ThermoWarningFlag, WSE.ClotValue, WSE.ExecutionType,  " & vbCrLf & _
                                                       "  T.TestID, T.TestName, WSE.ValidReadings, WSE.CompleteReadings, OT.TestType, OT.SampleType, " & vbCrLf & _
                                                       "  OT.ReplicatesNumber AS ReplicatesTotalNum, OT.ControlID, OT.OrderID, O.SampleID " & vbCrLf & _
                                                " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                            " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                                            " INNER JOIN tparTests T ON OT.TestID = T.TestID AND OT.TestType = 'STD' " & vbCrLf & _
                                                " WHERE  WSE.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WSE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSE.OrderTestID = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    WSE.MultiItemNumber = " & pMultiItemNumber.ToString & vbCrLf & _
                                                " AND    WSE.ReplicateNumber = " & pReplicateNumber.ToString & vbCrLf & _
                                                " AND    WSE.ExecutionStatus = 'CLOSED' " & vbCrLf & _
                                                " AND    WSE.ExecutionType = 'PREP_STD' " & vbCrLf & _
                                                " AND    WSE.SampleClass = 'CALIB' " & vbCrLf & _
                                                " ORDER BY WSE.ExecutionID " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.ReadAffectedCalibExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer/WorkSession, search the Max MultiItemNumber and the Max ReplicateNumber for all SampleTypes with 
        ''' Calibrators requested for the informed STD TestID
        ''' </summary>
        '''<param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS containing the Max MultiItemNumber and the Max ReplicateNumber for
        '''          all SampleTypes with Calibrators requested for the informed TestID in the specified AnalyzerID and WorkSessionID</returns>
        ''' <remarks>
        ''' Created by: SA 09/07/2012
        ''' </remarks>
        Public Function ReadAffectedOTCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                  ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT OrderTestID, SampleType, MAX(MultiItemNumber) AS MultiItemNumber, MAX(ReplicateNumber) AS ReplicateNumber " & vbCrLf & _
                                                " FROM   vwksWSExecutionsResults " & vbCrLf & _
                                                " WHERE  AnalyzerID      = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    TestID          = " & pTestID.ToString & vbCrLf & _
                                                " AND    SampleClass     = 'CALIB' " & vbCrLf & _
                                                " AND    ExecutionType   = 'PREP_STD' " & vbCrLf & _
                                                " AND    ExecutionStatus = 'CLOSED' " & vbCrLf & _
                                                " GROUP BY AnalyzerID, WorkSessionID, OrderTestID, SampleType " & vbCrLf

                        Dim executionDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(executionDataDS.vwksWSExecutionsResults)
                            End Using
                        End Using

                        resultData.SetDatos = executionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.ReadAffectedOTCalibrators", EventLogEntryType.Error, False)
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
        ''' Created by:  SA 09/07/2012 - Based in UpdateInUse receiving all needed values as parameters instead in an ExecutionsDS. Implementation
        '''                              changed by removing unneeded code 
        ''' </remarks>
        Public Function UpdateInUseNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                       ByVal pExecutionID As Integer, ByVal pInUse As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSExecutions " & vbCrLf & _
                                            " SET    InUse         = " & IIf(pInUse, 1, 0).ToString & vbCrLf & _
                                            " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID =  '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    ExecutionID   = " & pExecutionID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateInUse", EventLogEntryType.Error, False)
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
        ''' Created by:  SA 03/07/2012 - New implementation of function UpdateReadingsFields; changes to improve application perfomance
        ''' </remarks>
        Public Function UpdateReadingsFieldsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As New StringBuilder
                    For Each executionRow As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions
                        cmdText.Append(" UPDATE twksWSExecutions SET ")
                        cmdText.AppendFormat(" WellUsed = {0}, ", executionRow.WellUsed.ToString)
                        cmdText.AppendFormat(" BaseLineID = {0}, ", executionRow.BaseLineID.ToString)
                        cmdText.AppendFormat(" AdjustBaseLineID = {0}, ", executionRow.AdjustBaseLineID.ToString)
                        cmdText.AppendFormat(" HasReadings = {0}, ", IIf(executionRow.HasReadings, 1, 0).ToString)
                        cmdText.AppendFormat(" ThermoWarningFlag = {0}, ", IIf(executionRow.ThermoWarningFlag, 1, 0).ToString)
                        If (Not executionRow.IsClotValueNull) Then cmdText.AppendFormat(" ClotValue = '{0}', ", executionRow.ClotValue)
                        cmdText.AppendFormat(" ValidReadings = {0}, ", IIf(executionRow.ValidReadings, 1, 0).ToString)
                        cmdText.AppendFormat(" CompleteReadings = {0} ", IIf(executionRow.CompleteReadings, 1, 0).ToString)
                        cmdText.AppendFormat(" WHERE ExecutionID = {0} ", executionRow.ExecutionID)
                        cmdText.Append(vbCrLf)
                    Next

                    If (cmdText.Length > 0) Then
                        'Execute all the UPDATES contained in the string 
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateReadingsFieldsNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the status of all Executions for the OrderTestID and RerunNumber that have currently status Pending or Locked
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pToPendingUpdateList ">List of rows of a typed DataSet ExecutionsDS containing for the new status to assign to all 
        '''                             currently Pending Executions for each OrderTestID and RerunNumber</param>
        ''' <param name="pToLockedUpdateList">Equal as above but for LOCKED</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 05/07/2012
        ''' Modified by: AG 19/02/2014 - #1514 -Parameters 2 lists (one for update to PENDING and other for update to LOCKED)
        '''              XB 20/03/2014 - Add parameter HResult into Try Catch section - #1548
        ''' </remarks>
        Public Function UpdateStatusByOTAndRerunNumber(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                       ByVal pToPendingUpdateList As List(Of ExecutionsDS.twksWSExecutionsRow), ByVal pToLockedUpdateList As List(Of ExecutionsDS.twksWSExecutionsRow)) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As New StringBuilder
                    'AG 19/02/2014 - #1514
                    'For Each executionRow As ExecutionsDS.twksWSExecutionsRow In pToUpdateList
                    '    cmdText.Append(" UPDATE twksWSExecutions ")
                    '    cmdText.AppendFormat(" SET   ExecutionStatus = '{0}' ", executionRow.ExecutionStatus)
                    '    cmdText.AppendFormat(" WHERE AnalyzerID    = N'{0}' ", executionRow.AnalyzerID.Trim.Replace("'", "''"))
                    '    cmdText.AppendFormat(" AND   WorkSessionID =  '{0}' ", executionRow.WorkSessionID.Trim)
                    '    cmdText.AppendFormat(" AND   OrderTestID   =   {0}  ", executionRow.OrderTestID.ToString)
                    '    cmdText.AppendFormat(" AND   RerunNumber   =   {0}  ", executionRow.RerunNumber.ToString)
                    '    cmdText.Append(" AND   (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED') ")
                    '    cmdText.Append(vbCrLf)
                    'Next

                    'Update executions to PENDING - 1 QUERY
                    If pToPendingUpdateList.Count > 0 Then
                        cmdText.Append(" UPDATE twksWSExecutions ")
                        cmdText.AppendFormat(" SET   ExecutionStatus = '{0}' ", pToPendingUpdateList(0).ExecutionStatus)
                        cmdText.AppendFormat(" WHERE AnalyzerID    = N'{0}' ", pToPendingUpdateList(0).AnalyzerID.Trim.Replace("'", "''"))
                        cmdText.AppendFormat(" AND   WorkSessionID =  '{0}' ", pToPendingUpdateList(0).WorkSessionID.Trim)
                        cmdText.Append(" AND   (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED') ")

                        Dim i As Integer = 0
                        Dim filterClause As String = ""
                        For Each executionRow As ExecutionsDS.twksWSExecutionsRow In pToPendingUpdateList
                            If pToPendingUpdateList.Count > 1 Then
                                Select Case i
                                    Case 0 'First item
                                        filterClause &= " ( (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") "
                                    Case pToPendingUpdateList.Count - 1 'Last item
                                        filterClause &= " OR (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") )"
                                    Case Else 'Middle item
                                        filterClause &= " OR (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") "
                                End Select
                            Else 'Only 1 item
                                filterClause &= " (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") "
                            End If
                            i += 1
                        Next
                        cmdText.AppendFormat(" AND {0} ", filterClause)
                        cmdText.Append(vbCrLf)
                    End If

                    'Update executions to LOCKED - 1 QUERY
                    If pToLockedUpdateList.Count > 0 Then
                        cmdText.Append(" UPDATE twksWSExecutions ")
                        cmdText.AppendFormat(" SET   ExecutionStatus = '{0}' ", pToLockedUpdateList(0).ExecutionStatus)
                        cmdText.AppendFormat(" WHERE AnalyzerID    = N'{0}' ", pToLockedUpdateList(0).AnalyzerID.Trim.Replace("'", "''"))
                        cmdText.AppendFormat(" AND   WorkSessionID =  '{0}' ", pToLockedUpdateList(0).WorkSessionID.Trim)
                        cmdText.Append(" AND   (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED') ")

                        Dim i As Integer = 0
                        Dim filterClause As String = ""
                        For Each executionRow As ExecutionsDS.twksWSExecutionsRow In pToLockedUpdateList
                            If pToLockedUpdateList.Count > 1 Then
                                Select Case i
                                    Case 0 'First item
                                        filterClause &= " ( (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") "
                                    Case pToLockedUpdateList.Count - 1 'Last item
                                        filterClause &= " OR (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") )"
                                    Case Else 'Middle item
                                        filterClause &= " OR (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") "
                                End Select
                            Else 'Only 1 item
                                filterClause &= " (OrderTestID = " & executionRow.OrderTestID.ToString & " AND RerunNumber = " & executionRow.RerunNumber.ToString & ") "
                            End If
                            i += 1
                        Next
                        cmdText.AppendFormat(" AND {0} ", filterClause)
                        cmdText.Append(vbCrLf)
                    End If
                    'AG 19/02/2014 - #1514

                    If (cmdText.Length > 0) Then
                        'Execute all the UPDATES contained in the string 
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                        cmdText.Length = 0
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksWSExecutionsDAO.UpdateStatusByOTAndRerunNumber", EventLogEntryType.Error, False)
            End Try
            Return resultData
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
        ''' <returns>GlobalDataTo (executionsDS - only with OrderTestID informed)</returns>
        ''' <remarks>AG 25/03/2013</remarks>
        Public Function GetOrderTestsLockedByLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pLockedValue As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT OrderTestID FROM twksWSExecutions " & vbCrLf
                        cmdText += " WHERE LockedByLIS = " & IIf(pLockedValue, 1, 0).ToString

                        Dim myDataSet As New ExecutionsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetOrderTestsLockedByLIS", EventLogEntryType.Error, False)

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
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    Dim newStatus As String = IIf(pNewLockedValue, "LOCKED", "PENDING").ToString
                    Dim newLockedByLIS As Integer = CInt(IIf(pNewLockedValue, 1, 0))
                    For Each item As Integer In pAffectedOrderTests

                        cmdText += " UPDATE twksWSExecutions SET ExecutionStatus = '" & newStatus & "', " & vbCrLf & _
                                  " LockedByLIS = " & newLockedByLIS & vbCrLf & _
                                  " WHERE OrderTestId = " & item & vbCrLf & _
                                  " AND (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED')"

                        cmdText += vbNewLine

                    Next

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                    'AG 25/07/2014
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateLockedByLIS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search for executions (with status PENDING or LOCKED) and belonging to OrderTests received from LIS
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo (ExecutionsDS)</returns>
        ''' <remarks>AG 16/07/2013</remarks>
        Public Function ExistsLISWorkordersPendingToExecute(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ex.ExecutionID , ex.OrderTestID , ex.ExecutionStatus FROM twksWSExecutions ex INNER JOIN twksOrderTests ot " & vbCrLf & _
                                                " ON ex.OrderTestID = ot.OrderTestID " & vbCrLf & _
                                                " WHERE  (ex.ExecutionStatus = 'PENDING' OR ex.ExecutionStatus = 'LOCKED') " & vbCrLf & _
                                                " AND ot.LISRequest = 1 "

                        Dim myResultDataDS As New ExecutionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultDataDS.twksWSExecutions)
                            End Using
                        End Using

                        resultData.SetDatos = myResultDataDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.ExistsLISWorkordersPendingToExecute", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "TO REVIEW - DELETE?"
        'Public Function GetOrderTestWithExecutionClosed(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                                ByVal pExecutionStatus As String, _
        '                                                ByVal pRerun As Integer, _
        '                                                ByVal pMultiItemNumber As Integer) As GlobalDataTO
        '    'ByVal pAnalyzerID As String, _
        '    'ByVal pWorkSessionID As String) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then

        '                Dim cmdText As String = ""
        '                cmdText &= "  SELECT OrderTestID, ExecutionStatus, RerunNumber, MultiItemNumber " & vbCrLf
        '                cmdText &= "    FROM twksWSExecutions" & vbCrLf
        '                cmdText &= "GROUP BY OrderTestID, ExecutionStatus, RerunNumber, MultiItemNumber" & vbCrLf ', AnalyzerID, WorkSessionID  " & vbCrLf
        '                cmdText &= "  HAVING ExecutionStatus = '" & pExecutionStatus & "'" & vbCrLf
        '                cmdText &= "     AND RerunNumber = " & pRerun
        '                cmdText &= "     AND MultiItemNumber = " & pMultiItemNumber & vbCrLf
        '                'cmdText &= "     AND AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
        '                'cmdText &= "     AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
        '                cmdText &= "ORDER BY OrderTestID"

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim myDS As New ExecutionsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(myDS.twksWSExecutions)

        '                resultData.SetDatos = myDS
        '                resultData.HasError = False

        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetOrderTestWithExecutionClosed", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed information of all Patient Executions (for Standard and ISE Tests)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSExecutionsInfoDS with detailed information 
        ''''          of all Patient Executions</returns>
        '''' <remarks>
        '''' Created by : SA 22/11/2010
        '''' </remarks>
        'Public Function GetPatientExecutionsInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                         ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT WSE.ExecutionID, WSE.SampleClass, WSE.StatFlag, WSE.MultiItemNumber, WSE.ReplicateNumber, WSE.ExecutionStatus, " & _
        '                                 " WSE.RerunNumber, OT.OrderTestID, OT.TestType, OT.SampleType, T.TestName, T.PreloadedTest, " & _
        '                                 " CASE WHEN O.PatientID IS NULL THEN O.SampleID ELSE O.PatientID END AS ElementName " & _
        '                          " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & _
        '                                                      " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & _
        '                                                      " INNER JOIN tparTests T ON OT.TestID = T.TestID " & _
        '                          " WHERE  WSE.WorkSessionID = '" & pWorkSessionID & "' " & _
        '                          " AND    WSE.AnalyzerID = '" & pAnalyzerID & "' " & _
        '                          " AND    WSE.SampleClass = 'PATIENT' " & _
        '                          " AND    OT.TestType = 'STD' " & _
        '                          " UNION " & _
        '                          " SELECT WSE.ExecutionID, WSE.SampleClass, WSE.StatFlag, WSE.MultiItemNumber, WSE.ReplicateNumber, WSE.ExecutionStatus, " & _
        '                                 " WSE.RerunNumber, OT.OrderTestID, OT.TestType, OT.SampleType, IT.[Name] AS TestName, 0 AS PreloadedTest, " & _
        '                                 " CASE WHEN O.PatientID IS NULL THEN O.SampleID ELSE O.PatientID END AS ElementName " & _
        '                          " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & _
        '                                                      " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & _
        '                                                      " INNER JOIN tparISETests IT ON OT.TestID = IT.ISETestID " & _
        '                          " WHERE  WSE.WorkSessionID = '" & pWorkSessionID & "' " & _
        '                          " AND    WSE.AnalyzerID = '" & pAnalyzerID & "' " & _
        '                          " AND    WSE.SampleClass = 'PATIENT' " & _
        '                          " AND    OT.TestType = 'ISE' " & _
        '                          " ORDER BY WSE.ExecutionID "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                Dim patientExecDS As New WSExecutionsInfoDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(patientExecDS.twksWSExecutions)

        '                resultData.SetDatos = patientExecDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetPatientExecutionsInfo", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed information of all Calibrator Executions (for Standard Tests)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSExecutionsInfoDS with detailed information 
        ''''          of all Calibrator Executions</returns>
        '''' <remarks>
        '''' Created by : SA 22/11/2010
        '''' </remarks>
        'Public Function GetCalibratorExecutionsInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                            ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT WSE.ExecutionID, WSE.SampleClass, WSE.StatFlag, WSE.MultiItemNumber, WSE.ReplicateNumber, WSE.ExecutionStatus, " & _
        '                                 " WSE.RerunNumber, OT.OrderTestID, OT.TestType, OT.SampleType, T.TestName, T.PreloadedTest, C.CalibratorName AS ElementName " & _
        '                          " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & _
        '                                                      " INNER JOIN tparTests T ON OT.TestID = T.TestID " & _
        '                                                      " INNER JOIN tparTestCalibrators TC ON OT.TestID = TC.TestID AND OT.SampleType = TC.SampleType " & _
        '                                                      " INNER JOIN tparCalibrators C ON TC.CalibratorID = C.CalibratorID " & _
        '                          " WHERE  WSE.WorkSessionID = '" & pWorkSessionID & "' " & _
        '                          " AND    WSE.AnalyzerID = '" & pAnalyzerID & "' " & _
        '                          " AND    WSE.SampleClass = 'CALIB' " & _
        '                          " AND    OT.TestType = 'STD' " & _
        '                          " ORDER BY WSE.ExecutionID "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                Dim calibExecDS As New WSExecutionsInfoDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(calibExecDS.twksWSExecutions)

        '                resultData.SetDatos = calibExecDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetCalibratorExecutionsInfo", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed information of all Control Executions (for Standard Tests)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSExecutionsInfoDS with detailed information 
        ''''          of all Control Executions</returns>
        '''' <remarks>
        '''' Created by : SA 22/11/2010
        '''' Modified by: SA 14/02/2011 - Added join condition by Control Number to avoid duplicated records when more
        ''''                              than one Control is used for a Test/Sample Type
        ''''              SA 20/04/2011 - Query changed due to it had syntax errors
        '''' </remarks>
        'Public Function GetControlExecutionsInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                         ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                ' DL 18/04/2011 Management of Controls in WS Preparation
        '                'cmdText = " SELECT WSE.ExecutionID, WSE.SampleClass, WSE.StatFlag, WSE.MultiItemNumber, WSE.ReplicateNumber, WSE.ExecutionStatus, " & _
        '                '                 " WSE.RerunNumber, OT.OrderTestID, OT.TestType, OT.SampleType, T.TestName, T.PreloadedTest, C.ControlName AS ElementName " & _
        '                '          " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & _
        '                '                                      " INNER JOIN tparTests T ON OT.TestID = T.TestID " & _
        '                '                                      " INNER JOIN tparTestControls TC ON TC.TestID = OT.TestID AND TC.SampleType = OT.SampleType AND TC.ControlNum = WSE.MultiItemNumber " & _
        '                '                                      " INNER JOIN tparControls C ON TC.ControlID = C.ControlID" & _
        '                '          " WHERE  WSE.WorkSessionID = '" & pWorkSessionID & "' " & _
        '                '          " AND    WSE.AnalyzerID = '" & pAnalyzerID & "' " & _
        '                '          " AND    WSE.SampleClass = 'CTRL' " & _
        '                '          " AND    OT.TestType = 'STD' " & _
        '                '          " ORDER BY WSE.ExecutionID "
        '                cmdText &= "  SELECT WSE.ExecutionID, WSE.SampleClass, WSE.StatFlag, WSE.MultiItemNumber, WSE.ReplicateNumber, WSE.ExecutionStatus, " & vbCrLf
        '                cmdText &= "         WSE.RerunNumber, OT.OrderTestID, OT.TestType, OT.SampleType, T.TestName, T.PreloadedTest, C.ControlName AS ElementName " & vbCrLf
        '                cmdText &= "    FROM twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & vbCrLf
        '                cmdText &= "                              INNER JOIN tparTests T ON OT.TestID = T.TestID " & vbCrLf
        '                cmdText &= "                              INNER JOIN tparControls C ON OT.ControlID = C.ControlID" & vbCrLf
        '                cmdText &= "   WHERE WSE.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
        '                cmdText &= "     AND WSE.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
        '                cmdText &= "     AND WSE.SampleClass = 'CTRL' " & vbCrLf
        '                cmdText &= "     AND OT.TestType = 'STD' " & vbCrLf
        '                cmdText &= "ORDER BY WSE.ExecutionID"
        '                ' End DL 18/04/2011

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                Dim ctrlExecDS As New WSExecutionsInfoDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(ctrlExecDS.twksWSExecutions)

        '                resultData.SetDatos = ctrlExecDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetControlExecutionsInfo", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed information of all Blank Executions (for Standard Tests)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSExecutionsInfoDS with detailed information 
        ''''          of all Blank Executions</returns>
        '''' <remarks>
        '''' Created by : SA 22/11/2010
        '''' </remarks>
        'Public Function GetBlankExecutionsInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                       ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT WSE.ExecutionID, WSE.SampleClass, WSE.StatFlag, WSE.MultiItemNumber, WSE.ReplicateNumber, WSE.ExecutionStatus, " & _
        '                                               " WSE.RerunNumber, OT.OrderTestID, OT.TestType, OT.SampleType, T.TestName, T.PreloadedTest, T.TestName AS ElementName " & _
        '                                        " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & _
        '                                                                    " INNER JOIN tparTests T ON OT.TestID = T.TestID " & _
        '                                        " WHERE  WSE.WorkSessionID = '" & pWorkSessionID & "' " & _
        '                                        " AND    WSE.AnalyzerID = '" & pAnalyzerID & "' " & _
        '                                        " AND    WSE.SampleClass = 'BLANK' " & _
        '                                        " AND    OT.TestType = 'STD' " & _
        '                                        " ORDER BY WSE.ExecutionID "

        '                Dim blankExecDS As New WSExecutionsInfoDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(blankExecDS.twksWSExecutions)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = blankExecDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetBlankExecutionsInfo", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' When a Stat Patient Order Test is included in a WorkSession, set StatFlag=True for executions of the required Blank, Calibrator 
        '''' and Controls, but only when the status of these executions is PENDING
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pAlternativeST">Code of the Alternative SampleType. Optional parameter, informed only when for the specified
        ''''                              Test and SampleType the Calibrator needed is an Alternative one</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 21/04/2010
        '''' Modified by: SA 10/05/2010 - Query changed to manages all posible cases (locked Executions and Alternative Calibrators were 
        ''''                              not managed in the previous one)
        ''''              SA 09/03/2012 - Implemented Using sentence
        '''' </remarks>
        'Public Function UpdateStatExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                     ByVal pTestID As Integer, ByVal pSampleType As String, Optional ByVal pAlternativeST As String = "") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE twksWSExecutions " & vbCrLf & _
        '                                    " SET    StatFlag = 1 " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
        '                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                    " AND   (ExecutionStatus = 'PENDING' OR ExecutionStatus = 'LOCKED') " & vbCrLf & _
        '                                    " AND    OrderTestID IN (SELECT OrderTestID FROM vwksWSOrderTests " & vbCrLf & _
        '                                                           " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
        '                                                           " AND   WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                                           " AND ((SampleClass = 'CTRL' AND TestID = " & pTestID.ToString & " AND SampleType = '" & pSampleType.Trim & "') " & vbCrLf

        '            If (pAlternativeST.Trim = "") Then
        '                cmdText &= " OR (SampleClass = 'CALIB' AND TestID = " & pTestID.ToString & " AND SampleType = '" & pSampleType.Trim & "') " & vbCrLf
        '            Else
        '                'Search Calibrator for the Alternative SampleType
        '                cmdText &= " OR (SampleClass = 'CALIB' AND TestID = " & pTestID.ToString & " AND SampleType = '" & pAlternativeST.Trim & "') " & vbCrLf
        '            End If

        '            cmdText &= " OR (SampleClass = 'BLANK' AND TestID = " & pTestID.ToString & "))) " & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.UpdateStatExecutions", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Counts the total number of Executions for the specified Analyzer and WorkSession
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <returns>GlobalDataTO containing an integer value with the total number of Executions for the specified 
        ''''          Analyzer and WorkSession</returns>
        '''' <remarks>
        '''' Created by:  RH 27/09/2011
        '''' </remarks>
        'Public Function CountExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = String.Format("SELECT COUNT(ExecutionID) AS ExecutionsNumber " & _
        '                                                      "FROM   twksWSExecutions " & _
        '                                                      "WHERE  AnalyzerID    = N'{0}' " & _
        '                                                      "AND    WorkSessionID = '{1}' ", _
        '                                                      pAnalyzerID.Trim.Replace("'", "''"), pWorkSessionID.Trim)

        '                Dim dbDataReader As SqlClient.SqlDataReader
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    dbDataReader = dbCmd.ExecuteReader()

        '                    If (dbDataReader.HasRows) Then
        '                        dbDataReader.Read()

        '                        If (dbDataReader.IsDBNull(0)) Then
        '                            resultData.SetDatos = 0
        '                        Else
        '                            resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("ExecutionsNumber"))
        '                        End If
        '                    End If
        '                    dbDataReader.Close()
        '                End Using
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.CountExecutions", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Calculate the Contamination Number of an Order Test
        '''' </summary>
        '''' <param name="pReagentIDs">List of ReagentIDs</param>
        '''' <returns>GlobalDataTO containing an Integer Value with the total number of Contaminations found</returns>
        '''' <remarks>
        '''' Created by:  RH 10/06/2010
        '''' </remarks>
        'Public Function GetContaminationNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentIDs As List(Of Integer)) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        If (pReagentIDs.Count = 0) Then
        '            resultData.SetDatos = 0
        '            Return resultData
        '        End If

        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myContaminationNumber As Integer = 0

        '                Dim dbDataReader As SqlClient.SqlDataReader
        '                Using dbCmd As New SqlClient.SqlCommand()
        '                    dbCmd.Connection = dbConnection

        '                    For i As Integer = 0 To pReagentIDs.Count - 2
        '                        dbCmd.CommandText = String.Format(" SELECT ContaminationID FROM tparContaminations " & _
        '                                                          " WHERE ReagentContaminatorID = {0} AND ReagentContaminatedID = {1} AND ContaminationType = 'R1' ", _
        '                                                          pReagentIDs(i), pReagentIDs(i + 1))

        '                        'Execute the SQL sentence
        '                        dbDataReader = dbCmd.ExecuteReader()
        '                        If (dbDataReader.HasRows) Then
        '                            myContaminationNumber += 1
        '                        End If

        '                        dbDataReader.Close()
        '                    Next
        '                End Using

        '                resultData.SetDatos = myContaminationNumber
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetContaminationNumber", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the list of pending executions (vwksWSExecutions)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO indicating if an error has occurred or not. If succeed, returns an 
        ''''          ExecutionsDS dataset with the data (view vwksWSExecutions)
        '''' </returns>
        '''' <remarks>
        '''' Created by:  RH 20/09/2010
        '''' </remarks>
        'Public Function GetWSPendingExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
        '                                           ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String

        '                'Read from vwksWSExecutions view
        '                cmdText = String.Format( _
        '                    "SELECT * FROM vwksWSExecutions " & _
        '                    "WHERE AnalyzerID = '{0}' AND WorkSessionID = '{1}'", _
        '                    pAnalyzerID, pWorkSessionID)

        '                Dim cmd As New SqlClient.SqlCommand
        '                cmd.Connection = dbConnection
        '                cmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim executionDataDS As New ExecutionsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(cmd)
        '                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

        '                resultData.SetDatos = executionDataDS
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetWSPendingExecutions", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the list of Order Tests sorted by StatFlag, SampleClass, OrderID, ReadingCycle and OrderTestID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO indicating if an error has occurred or not. If succeed, returns an 
        ''''          ExecutionsDS dataset with the sorted data (view vwksWSExecutions)
        '''' </returns>
        '''' <remarks>
        '''' Created by:  RH 06/08/2010
        '''' </remarks>
        'Public Function GetWSExecutionsOrderByTime(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
        '                                           ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String

        '                'Read from vwksWSExecutions view
        '                'cmdText = String.Format( _
        '                '    "SELECT * FROM vwksWSExecutions " & _
        '                '    "WHERE AnalyzerID = '{0}' AND WorkSessionID = '{1}' " & _
        '                '    "ORDER BY StatFlag DESC, SampleClass, OrderID, ReadingCycle DESC, OrderTestID", _
        '                '    pAnalyzerID, pWorkSessionID)

        '                'RH 19/01/2011 new sort criteria (ExecutionType and SampleType) 
        '                cmdText = String.Format( _
        '                    "SELECT * FROM vwksWSExecutions " & _
        '                    "WHERE AnalyzerID = '{0}' AND WorkSessionID = '{1}' " & _
        '                    "ORDER BY StatFlag DESC, SampleClass, SampleType, OrderID, ExecutionType DESC, ElementID, ReadingCycle DESC, OrderTestID", _
        '                    pAnalyzerID, pWorkSessionID)

        '                Dim cmd As New SqlClient.SqlCommand
        '                cmd.Connection = dbConnection
        '                cmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim executionDataDS As New ExecutionsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(cmd)
        '                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

        '                resultData.SetDatos = executionDataDS
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetWSExecutionsOrderByTime", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get all Executions for the specified Order Test 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pOrderTestID">Order Test Identifier</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier; optional parameter</param>
        '''' <param name="pWorkSessionID">Work Session Identifier; optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with all Executions
        ''''          for the specified Order Test</returns>
        '''' <remarks>
        '''' Created by:  DL 11/05/2010
        '''' Modified by: DL - Added the optional to parameters pAnalyzerID and pWorkSessionID, and the validation
        ''''                   to filter the SQL by these values when informed.
        '''' </remarks>
        'Public Function GetExecutionByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
        '                                        Optional ByVal pAnalyzerID As String = "", Optional ByVal pWorkSessionID As String = "") _
        '                                        As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""

        '                cmdText &= "SELECT ExecutionID, WorkSessionID, AnalyzerID, BaseLineID, WellUsed, OrderTestID, " & vbCrLf
        '                cmdText &= "       PostDilutionType, ReplicateNumber, RerunNumber, MultiItemNumber, SampleClass " & vbCrLf
        '                cmdText &= "FROM   twksWSExecutions " & vbCrLf
        '                cmdText &= "WHERE  OrderTestID = " & pOrderTestID & vbCrLf

        '                If (pAnalyzerID <> "") Then cmdText &= "      AND AnalyzerID = '" & pAnalyzerID & "'"
        '                If (pWorkSessionID <> "") Then cmdText &= "   AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim executionDataDS As New ExecutionsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

        '                resultData.SetDatos = executionDataDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionByOrderTest", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the list of locked executions
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet ExecutionDS with data of the informed executions</returns>
        '''' <remarks>
        '''' Created by: RH - 06/08/2010
        '''' </remarks>
        'Public Function GetLockedExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
        '                                           ByVal pWorkSessionID As String) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim executionDataDS As New ExecutionsDS

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim Cmd As New SqlClient.SqlCommand
        '                Dim cmdText As String

        '                cmdText = String.Format( _
        '                    "SELECT * FROM twksWSExecutions " & _
        '                    "WHERE AnalyzerID = '{0}' AND WorkSessionID = '{1}' AND ExecutionStatus = 'LOCKED'", _
        '                    pAnalyzerID, pWorkSessionID)

        '                Cmd.Connection = dbConnection
        '                Cmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(Cmd)
        '                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

        '                resultData.SetDatos = executionDataDS
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetLockedExecutions", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pExecutionID"></param>
        '''' <param name="pAnalyzerID"></param>
        '''' <param name="pWorkSessionID"></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created ???
        '''' Modified by AG 03/01/2010 - add AdjustBaseLineID field
        '''' </remarks>
        'Public Function GetExecutionWithSampleClass(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                            ByVal pExecutionID As Integer, _
        '                                            Optional ByVal pAnalyzerID As String = "", _
        '                                            Optional ByVal pWorkSessionID As String = "") As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim executionDataDS As New ExecutionsDS

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then

        '                Dim cmdText As String = ""

        '                'AG 03/05/2010 - Field rotorturnnumber is deleted from database
        '                'cmdText &= " SELECT E.ExecutionID, E.WorkSessionID, E.AnalyzerID, E.BaseLineID, E.WellUsed, E.RotorTurnNumber, E.OrderTestID, "
        '                'cmdText &= "       PostDilutionType, ReplicateNumber, RotorTurnNumber, RerunNumber, MultiItemNumber "
        '                'cmdText &= " FROM   twksWSExecutions E, twksOrderTests OT, twksOrders O "
        '                'cmdText &= " WHERE  ExecutionID = " & pExecutionID
        '                'cmdText &= " AND E.OrderTestID = OT.OrderTestID "
        '                'cmdText &= " AND OT.OrderID = O.OrderID "
        '                cmdText &= " SELECT E.ExecutionID, E.WorkSessionID, E.AnalyzerID, E.BaseLineID, E.WellUsed, E.OrderTestID, "
        '                cmdText &= "       PostDilutionType, ReplicateNumber, RerunNumber, MultiItemNumber, AdjustBaseLineID "
        '                cmdText &= " FROM   twksWSExecutions E, twksOrderTests OT, twksOrders O "
        '                cmdText &= " WHERE  ExecutionID = " & pExecutionID
        '                cmdText &= " AND E.OrderTestID = OT.OrderTestID "
        '                cmdText &= " AND OT.OrderID = O.OrderID "


        '                If pWorkSessionID <> "" Then cmdText += "   and WorkSessionID = '" & pWorkSessionID & "'"

        '                If pAnalyzerID <> "" Then cmdText += "   and AnalyzerID = '" & pAnalyzerID & "'"

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

        '                resultData.SetDatos = executionDataDS
        '                resultData.HasError = False
        '            End If

        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetExecutionWithSampleClass", EventLogEntryType.Error, False)

        '    Finally

        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the BLANK SAMPLE position used for pExecutionId value (this method works ONLY for blanks!!!)
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pWorkSessionID" ></param>
        '''' <param name="pAnalyzerID" ></param>
        '''' <param name="pExecutionID"></param>
        '''' <returns>GlobalDataTO (WSRotorContentByPositionDS)</returns>
        '''' <remarks>AG 13/05/2011</remarks>
        'Public Function GetBLANKSampleRotorPositionByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                                         ByVal pAnalyzerID As String, ByVal pExecutionID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT ex.OrderTestID, ex.SampleClass, ex.MultiItemNumber As MultiTubeNumber, re.ElementID, rcp.RingNumber , rcp.CellNumber, rcp.Status " & vbCrLf
        '                cmdText &= " FROM twksWSExecutions ex INNER JOIN twksWSRequiredElemByOrderTest re ON ex.ordertestID = re.OrderTestID " & vbCrLf
        '                cmdText &= " INNER JOIN twksWSRotorContentByPosition rcp ON re.ElementID = rcp.ElementID AND ex.WorkSessionID = rcp.WorkSessionID " & vbCrLf
        '                cmdText &= " AND ex.AnalyzerID  = ex.AnalyzerID AND rcp.RotorType = 'SAMPLES' " & vbCrLf
        '                cmdText &= " INNER JOIN twksWSRequiredElements re2 ON rcp.ElementID = re2.ElementID " & vbCrLf
        '                cmdText &= " WHERE ex.ExecutionID = " & pExecutionID & " " & vbCrLf
        '                cmdText &= " AND ex.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
        '                cmdText &= " AND ex.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
        '                cmdText &= " AND (EX.MultiItemNumber = RE2.MultiItemNumber OR RE2.MultiItemNumber IS NULL) "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim myResultDataDS As New WSRotorContentByPositionDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(myResultDataDS.twksWSRotorContentByPosition)

        '                resultData.SetDatos = myResultDataDS
        '                resultData.HasError = False

        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TwksWSExecutionsDAO.GetBLANKSampleRotorPositionByExecutionID", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData

        'End Function

        '''' <summary>
        '''' Get all related execution by the order test id
        '''' this methos is implemente for the ISE results. It get the 
        '''' Get the ISE ResultID and the testID
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pOrderTestID">Order Test ID</param>
        '''' <param name="pPreparationID" ></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' CREATE BY: TR 04/01/2011
        '''' Modified AG 30/11/2011
        '''' </remarks>
        'Public Function GetISEExecutionsByOrderTestAndPreparationID(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                            ByVal pOrderTestID As Integer, ByVal pPreparationID As Integer) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim executionDataDS As New ExecutionsDS

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim Cmd As New SqlClient.SqlCommand
        '                Dim cmdText As String = ""

        '                cmdText &= "SELECT twksOrderTests.TestID, twksOrderTests.SampleType, tparISETests.ISE_ResultID, twksWSExecutions.*  " & vbCrLf
        '                cmdText &= "  FROM twksWSExecutions INNER JOIN twksOrderTests ON twksWSExecutions.OrderTestID = twksOrderTests.OrderTestID INNER JOIN " & vbCrLf
        '                cmdText &= "       tparISETests ON twksOrderTests.TestID = tparISETests.ISETestID" & vbCrLf
        '                cmdText &= " WHERE twksWSExecutions.OrderTestID IN (SELECT twksOrderTests.OrderTestID FROM twksOrderTests "
        '                cmdText &= "                                        WHERE OrderID In (Select OrderID  From  twksOrderTests where twksOrderTests.OrderTestID =" & pOrderTestID & ")"
        '                cmdText &= "                                          AND TestType = 'ISE') "
        '                cmdText &= " AND twksOrderTests.ReplicatesNumber >= twksWSExecutions.ReplicateNumber " & vbCrLf
        '                cmdText &= " AND twksWSExecutions.ExecutionStatus = 'INPROCESS' " & vbCrLf 'AG 30/11/2011 - only the INPROCESS (not paused when were sent)
        '                cmdText &= " AND twksWSExecutions.PreparationID = " & pPreparationID.ToString & vbCrLf 'AG 30/11/2011

        '                Cmd.Connection = dbConnection
        '                Cmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(Cmd)
        '                dbDataAdapter.Fill(executionDataDS.twksWSExecutions)

        '                resultData.SetDatos = executionDataDS
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionsDAO.GetISEExecutionsByOrderTestAndPreparationID", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData

        'End Function

#End Region

    End Class
End Namespace

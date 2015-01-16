Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.DAL.DAO

    Public Class vwksWSOrderTestsDAO
        Inherits DAOBase

#Region "Public Methods"
        ''' <summary>
        ''' Count the number of OffSystem Tests requested in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of OffSystem Tests requested 
        '''          in the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 18/01/2011
        ''' Modified by: SA 12/04/2012 - Changed the function template
        ''' </remarks>
        Public Function CountWSOffSystemTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumOffSystemTests " & vbCrLf & _
                                                " FROM   vwksWSOrderTests " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    TestType = 'OFFS' " & vbCrLf

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.CountWSOffSystemTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Grouped by OrderID and SampleType, get the total number of ISE preparations that fulfill following conditions
        ''' ** Status is PENDING or Status is INPROCESS but the Preparation has not been still generated 
        ''' ** Status is INPROCESS and the Preparation has been generated but not sent (SendingTime is not informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TimeEstimationDS</returns>
        ''' <remarks>
        ''' Created by: TR 01/06/2012
        ''' </remarks>
        Public Function GetNotSendISEPreparations(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " "
                        cmdText &= "SELECT OrderID, SampleType, MAX(ReplicatesNumber) AS NumPreparations " & vbCrLf
                        cmdText &= "FROM vwksWSOrderTests " & vbCrLf
                        cmdText &= "WHERE TestType = 'ISE' " & vbCrLf
                        cmdText &= "AND   ToSendFlag = 1 " & vbCrLf
                        cmdText &= "AND   OrderTestID IN (SELECT OrderTestID FROM twksWSExecutions  " & vbCrLf
                        cmdText &= "                      WHERE WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
                        cmdText &= "                      AND   AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                        cmdText &= "                      AND   ExecutionType = 'PREP_ISE' " & vbCrLf
                        cmdText &= "                      AND  (ExecutionStatus = 'PENDING'  " & vbCrLf
                        cmdText &= "                      OR    (ExecutionStatus = 'INPROCESS'  " & vbCrLf
                        cmdText &= "                      AND    PreparatioNID IS NULL)) " & vbCrLf
                        cmdText &= "                      AND   Paused = 0) " & vbCrLf
                        cmdText &= "GROUP BY  OrderID, SampleType " & vbCrLf
                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "SELECT OrderID, SampleType, MAX(ReplicatesNumber) AS NumPreparations " & vbCrLf
                        cmdText &= "FROM vwksWSOrderTests " & vbCrLf
                        cmdText &= "WHERE TestType = 'ISE' " & vbCrLf
                        cmdText &= "AND   ToSendFlag = 1 " & vbCrLf
                        cmdText &= "AND   OrderTestID IN (SELECT EX.OrderTestID  " & vbCrLf
                        cmdText &= "                      FROM   twksWSExecutions EX LEFT JOIN twksWSPreparations P ON EX.PreparationID = P.PreparationID  " & vbCrLf
                        cmdText &= "                      WHERE  EX.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf
                        cmdText &= "                      AND    EX.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf
                        cmdText &= "                      AND    EX.ExecutionType = 'PREP_ISE' " & vbCrLf
                        cmdText &= "                      AND    EX.ExecutionStatus ='INPROCESS' " & vbCrLf
                        cmdText &= "                      AND    EX.Paused = 0 " & vbCrLf
                        cmdText &= "                      AND    (EX.PreparationID IS NOT NULL AND P.SendingTime IS NULL)) " & vbCrLf
                        cmdText &= "                      GROUP BY  OrderID, SampleType  " & vbCrLf

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.GetNotSendISEPreparations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' For the specified Analyzer WorkSession, get the list of Tests/SampleTypes for which a Calibrator was requested 
        ''' but executed for a different SampleType (those having informed field AlternativeOrderTestID)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS TestSamplesDS with the list of TestID / SampleType / Alternative SampleType
        '''          in the current Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 23/10/2012
        ''' </remarks>
        Public Function GetCalibratorsWithAlternative(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT OT.TestID, OT.SampleType, TS.SampleTypeAlternative, HTS.HistTestID, HTS.TestVersionNumber " & vbCrLf & _
                                                " FROM   vwksWSOrderTests OT INNER JOIN tparTestSamples TS ON OT.TestID      = TS.TestID " & vbCrLf & _
                                                                                                        " AND OT.SampleType  = TS.SampleType " & vbCrLf & _
                                                                           " INNER JOIN thisTestSamples HTS ON OT.TestID     = HTS.TestID " & vbCrLf & _
                                                                                                         " AND OT.SampleType = HTS.SampleType " & vbCrLf & _
                                                " WHERE  OT.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    OT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    OT.TestType      = 'STD' " & vbCrLf & _
                                                " AND    TS.SampleTypeAlternative IS NOT NULL " & vbCrLf & _
                                                " AND    TS.SampleTypeAlternative <> '' " & vbCrLf & _
                                                " AND    HTS.ClosedTestVersion = 0 " & vbCrLf & _
                                                " AND    HTS.ClosedTestSample  = 0 " & vbCrLf & _
                                                " ORDER BY OT.TestID, OT.SampleType " & vbCrLf

                        Dim myAlternativeOTsDS As New TestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAlternativeOTsDS.tparTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myAlternativeOTsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.GetCalibratorsWithAlternative", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer WorkSession, get the list of Blanks and Calibrators that were not executed due
        ''' to a previous value was used
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS HisWSOrderTestsDS with the list of Blanks and Calibrators that were 
        '''          not executed due to a previous value was used</returns>
        ''' <remarks>
        ''' Created by:  SA 31/08/2012
        ''' Modified by: SA 19/10/2012 - Changed the query to get also the Historic Identifiers for the previous Blanks 
        '''                              and Calibrators used in the specified Analyzer WorkSession, and also the used
        '''                              Calibration Factor (for Single Point Calibrators)
        '''              SA 23/10/2012 - Changed the type of the returned DS: from OrderTestsDS to HisWSOrderTestsDS. Changed the query 
        '''                              to get also fields HistTestID and TestVersionNumber from table thisWSOrderTests
        ''' </remarks>
        Public Function GetPreviousBlkCalibUsed(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT  OT.SampleClass, OT.TestID, OT.SampleType, HOT.HistTestID, HOT.TestVersionNumber, " & vbCrLf & _
                                                       "  R.HistWSID AS WorkSessionID, R.HistOTID AS HistOrderTestID, " & vbCrLf & _
                                                       " (CASE WHEN OT.SampleClass = 'BLANK' THEN NULL " & vbCrLf & _
                                                             " WHEN OT.SampleClass = 'CALIB' AND R.ManualResultFlag = 0 THEN R.CalibratorFactor " & vbCrLf & _
                                                             " WHEN OT.SampleClass = 'CALIB' AND R.ManualResultFlag = 1 THEN R.ManualResult " & vbCrLf & _
                                                             " ELSE NULL END) AS CalibrationFactor " & vbCrLf & _
                                                " FROM   vwksWSOrderTests OT INNER JOIN twksResults R ON OT.PreviousOrderTestID = R.OrderTestID " & vbCrLf & _
                                                                                                   " AND OT.AnalyzerID  = R.AnalyzerID " & vbCrLf & _
                                                                           " INNER JOIN thisWSOrderTests HOT ON R.HistOTID   = HOT.HistOrderTestID " & vbCrLf & _
                                                                                                          " AND R.HistWSID   = HOT.WorkSessionID " & vbCrLf & _
                                                                                                          " AND R.AnalyzerID = HOT.AnalyzerID " & vbCrLf & _
                                                " WHERE  OT.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    OT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    OT.ToSendFlag    = 0 " & vbCrLf & _
                                                " AND    OT.TestType      = 'STD' " & vbCrLf & _
                                                " AND    OT.PreviousOrderTestID IS NOT NULL " & vbCrLf & _
                                                " AND    R.HistWSID IS NOT NULL AND R.HistOTID IS NOT NULL " & vbCrLf & _
                                                " ORDER BY OT.SampleClass " & vbCrLf

                        Dim myOrderTestsDS As New HisWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.thisWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.GetPreviousBlkCalibUsed", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of OffSystem Tests requested in the specified WorkSession. Results informed for these OffSystem Tests are also returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsResultsDS with the list of OffSystem Tests requested in the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 18/01/2011
        ''' Modified by: SA 12/04/2012 - Changed the function template
        ''' </remarks>
        Public Function GetWSOffSystemTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WSOT.OrderTestID, (CASE WHEN WSOT.PatientID IS NULL THEN WSOT.SampleID ELSE WSOT.PatientID END) AS SampleID, " & vbCrLf & _
                                                       " WSOT.StatFlag, WSOT.SampleType, WSOT.TestID, OST.[Name] AS TestName, OST.ResultType, R.ManualResult AS ResultValue, " & vbCrLf & _
                                                       " R.ManualResultText, OST.Decimals AS AllowedDecimals, OSTS.ActiveRangeType, MD.FixedItemDesc AS Unit " & vbCrLf & _
                                                " FROM   vwksWSOrderTests WSOT INNER JOIN tparOffSystemTests OST ON WSOT.TestID = OST.OffSystemTestID " & vbCrLf & _
                                                                             " INNER JOIN tparOffSystemTestSamples OSTS ON WSOT.TestID = OSTS.OffSystemTestID " & vbCrLf & _
                                                                                                                     " AND WSOT.SampleType = OSTS.SampleType " & vbCrLf & _
                                                                             " LEFT OUTER JOIN twksResults R ON WSOT.OrderTestID = R.OrderTestID " & vbCrLf & _
                                                                             " LEFT OUTER JOIN tcfgMasterData MD ON OST.Units = MD.ItemID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " AND    WSOT.TestType = 'OFFS' " & vbCrLf & _
                                                " AND    OST.ResultType = 'QUANTIVE' " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT WSOT.OrderTestID, (CASE WHEN WSOT.PatientID IS NULL THEN WSOT.SampleID ELSE WSOT.PatientID END) AS SampleID, " & vbCrLf & _
                                                       " WSOT.StatFlag, WSOT.SampleType, WSOT.TestID, OST.[Name] AS TestName, OST.ResultType, R.ManualResult AS ResultValue, " & vbCrLf & _
                                                       " R.ManualResultText, OST.Decimals  AS AllowedDecimals, OSTS.ActiveRangeType, '' AS Unit " & vbCrLf & _
                                                " FROM   vwksWSOrderTests WSOT INNER JOIN tparOffSystemTests OST ON WSOT.TestID = OST.OffSystemTestID " & vbCrLf & _
                                                                             " INNER JOIN tparOffSystemTestSamples OSTS ON WSOT.TestID = OSTS.OffSystemTestID " & vbCrLf & _
                                                                                                                     " AND WSOT.SampleType = OSTS.SampleType " & vbCrLf & _
                                                                             " LEFT OUTER JOIN twksResults R ON WSOT.OrderTestID = R.OrderTestID " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.SampleClass = 'PATIENT' " & vbCrLf & _
                                                " AND    WSOT.TestType = 'OFFS' " & vbCrLf & _
                                                " AND    OST.ResultType = 'QUALTIVE' " & vbCrLf & _
                                                " ORDER BY SampleID, WSOT.StatFlag, WSOT.SampleType " & vbCrLf

                        Dim myOffSystemTestsResults As New OffSystemTestsResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOffSystemTestsResults.OffSystemTestsResults)
                            End Using
                        End Using

                        resultData.SetDatos = myOffSystemTestsResults
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.GetWSOffSystemTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all requested Calibrators for a Tests/SampleTypes using an Alternative Calibrator (field AlternativeOrderTestID is informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of TestID/SampleType and AlternativeOrderTestID</returns>
        ''' <remarks>
        ''' Created by:  SA 17/02/2011
        ''' Modified by: SA 12/04/2012 - Changed the function template
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function ReadAllAlternativeOTs(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WSOT.TestID, WSOT.SampleType, WSOT.AlternativeOrderTestID " & vbCrLf & _
                                                " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    WSOT.SampleClass   = 'CALIB' " & vbCrLf & _
                                                " AND    WSOT.TestType      = 'STD' " & vbCrLf & _
                                                " AND    WSOT.AlternativeOrderTestID IS NOT NULL " & vbCrLf

                        Dim myOrderTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.ReadAllAlternativeOTs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all Order Tests of the specified SampleClass and having the informed Status that have been requested in the 
        ''' active WorkSession.  Used to get data of Patients Samples and Controls for export them to LIMS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pOrderTestStatus"></param>
        ''' <returns>GlobalDataTo containing a typed DataSet ViewWSOrderTestsDS with the list of OrderTests that fulfill the 
        '''          specified searching criteria</returns>
        ''' <remarks>
        ''' Created by:  TR 14/05/2010
        ''' Modified by: SA 12/04/2012 - Changed the function template
        '''              AG 22/05/2012 - Added parameter for OrderTestStatus and change the query by adding a filter by this field
        ''' </remarks>
        Public Function ReadByWorkSessionAndSampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                        ByVal pSampleClass As String, ByVal pOrderTestStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vwksWSOrderTests " & vbCrLf & _
                                                " WHERE  WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    SampleClass     = '" & pSampleClass.Trim & "' " & vbCrLf & _
                                                " AND    OrderTestStatus = '" & pOrderTestStatus.Trim & "' " & vbCrLf

                        Dim myViewWSOrderTestsDS As New ViewWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myViewWSOrderTestsDS.vwksWSOrderTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myViewWSOrderTestsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.ReadByWorkSessionAndSampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all Order Tests requested for the specified Standard Test (and optionally, for the indicated SampleType
        ''' and SampleClass) in the active WorkSession. Used to get data of Blanks and Calibrators for export them to LIMS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTo containing a typed DataSet ViewWSOrderTestsDS</returns>
        ''' <remarks>
        ''' Created by:  TR 14/05/2010
        ''' Modified by: SA 12/04/2012 - Changed the function template
        '''              SA 19/04/2012 - Changed the query to filter data only for Standard Tests
        '''              AG 22/05/2012 - add optional parameter pOrderTestStatus
        ''' </remarks>
        Public Function ReadByWorkSessionAndTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   ByVal pTestID As Integer, Optional ByVal pSampleClass As String = "", _
                                                   Optional ByVal pSampleType As String = "", Optional ByVal pOrderTestStatus As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vwksWSOrderTests " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    TestType      = 'STD' " & vbCrLf & _
                                                " AND    TestID        = " & pTestID.ToString & vbCrLf

                        If (pSampleClass <> "") Then cmdText &= " AND SampleClass = '" & pSampleClass.Trim & "' " & vbCrLf
                        If (pSampleType <> "") Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf
                        If (pOrderTestStatus <> "") Then cmdText &= " AND OrderTestStatus = '" & pOrderTestStatus.Trim & "' " & vbCrLf 'AG 22/05/2012

                        Dim myViewWSOrderTestsDS As New ViewWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myViewWSOrderTestsDS.vwksWSOrderTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myViewWSOrderTestsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.ReadByWorkSessionAndTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Patient Samples and Controls requested for the specified Standard TestID and SampleType in the active Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestID">Standard Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ViewWSOrderTestsDS with the list of Patient Samples and Controls requested for 
        '''          the specified Standard TestID and SampleType in the active Work Session</returns>
        ''' <remarks>
        ''' Created by:  SA 12/04/2012
        ''' </remarks>
        Public Function ReadPatientsAndCtrlsByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                                  ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OrderTestID, StatFlag FROM vwksWSOrderTests " & vbCrLf & _
                                                " WHERE  SampleClass IN ('PATIENT', 'CTRL') " & vbCrLf & _
                                                " AND    WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    TestType        = 'STD' " & vbCrLf & _
                                                " AND    TestID          = " & pTestID.ToString & _
                                                " AND    SampleType      = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND    OrderTestStatus = 'OPEN' " & vbCrLf & _
                                                " AND    OpenOTFlag      = 0 " & vbCrLf & _
                                                " AND    ToSendFlag      = 1 " & vbCrLf

                        Dim myOrderTestsDS As New ViewWSOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.vwksWSOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.ReadPatientsAndCtrlsByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Special Tests (with SampleType) requested in the Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of TestID/SampleType corresponding to Special Tests</returns>
        ''' <remarks>
        ''' Created by:  SA 17/02/2011
        ''' Modified by: SA 19/04/2012 - Changed the query to filter data only for Standard Tests
        ''' </remarks>
        Public Function ReadSpecialTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT WSOT.TestID, WSOT.SampleType " & vbCrLf & _
                                                " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WSOT.TestType      = 'STD' " & vbCrLf & _
                                                " AND    WSOT.SpecialTest   = 1 "

                        Dim myOrderTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksWSOrderTestsDAO.ReadSpecialTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class

End Namespace

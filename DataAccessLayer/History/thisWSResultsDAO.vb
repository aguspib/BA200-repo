Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisWSResultsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a group of Results in the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSResultsDS">Typed DataSet HisWSResultsDS containing the group of Results to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 20/06/2012
        ''' Modified by: JB 04/10/2012 - Added column NoCalcCalib
        '''              JB 08/10/2012 - Added column ClosedResult 
        '''              SA 17/10/2012 - Added columns RelativeErrorCurve and ExportStatus
        '''              SA 18/10/2012 - Added columns BlankAbsorbanceLimit, KineticBlankLimit, MinFactorLimit and MaxFactorLimit
        '''              SA 22/10/2012 - Added columns ABSInitial, ABSWorkReagent and ABSMainFilter
        '''              SA 25/10/2012 - Added column RemarkAlert
        '''              AG 24/04/2013 - Added column LISMessageID
        '''              SA 08/09/2014 - BA-1919 ==> Added a call to Replace function for fields ManualResultText and UserComments
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSResultsDS As HisWSResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim HistBaseLineID As Integer = -1

                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection

                        For Each myHisWSResultsRow As HisWSResultsDS.thisWSResultsRow In pHisWSResultsDS.thisWSResults.Rows
                            cmdText.Append("  INSERT INTO thisWSResults ")
                            cmdText.Append(" (HistOrderTestID, AnalyzerID, WorkSessionID, MultiPointNumber, ResultDateTime, ABSValue, CONCValue, ")
                            cmdText.Append("  ManualResultFlag, ManualResult, ManualResultText, UserComment, CalibratorFactor, CalibratorBlankAbsUsed,")
                            cmdText.Append("  CurveSlope, CurveOffSet, CurveCorrelation, RelativeErrorCurve, ExportStatus, BlankAbsorbanceLimit, ")
                            cmdText.Append("  KineticBlankLimit, FactorLowerLimit, FactorUpperLimit, ABSInitial, ABSWorkReagent, ABSMainFilter, ")
                            cmdText.Append("  MinRefRange, MaxRefRange, RemarkAlert, AlarmList, NotCalcCalib, ClosedResult, LISMessageID) ")
                            cmdText.AppendFormat(" VALUES({0}, N'{1}', '{2}', {3}, '{4}'", myHisWSResultsRow.HistOrderTestID, _
                                                 myHisWSResultsRow.AnalyzerID.Trim.Replace("'", "''"), myHisWSResultsRow.WorkSessionID.Trim, _
                                                 myHisWSResultsRow.MultiPointNumber, myHisWSResultsRow.ResultDateTime.ToString("yyyyMMdd HH:mm:ss"))

                            If (Not myHisWSResultsRow.IsABSValueNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.ABSValue))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsCONCValueNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.CONCValue))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            cmdText.Append(", '" & myHisWSResultsRow.ManualResultFlag & "'")

                            If (Not myHisWSResultsRow.IsManualResultNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.ManualResult))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsManualResultTextNull AndAlso myHisWSResultsRow.ManualResultText.Trim <> String.Empty) Then
                                cmdText.Append(", N'" & myHisWSResultsRow.ManualResultText.Trim.Replace("'", "''") & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsUserCommentNull AndAlso myHisWSResultsRow.UserComment.Trim <> String.Empty) Then
                                cmdText.Append(", N'" & myHisWSResultsRow.UserComment.Trim.Replace("'", "''") & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsCalibratorFactorNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.CalibratorFactor))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsCalibratorBlankAbsUsedNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.CalibratorBlankAbsUsed))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsCurveSlopeNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.CurveSlope))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsCurveOffSetNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.CurveOffSet))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsCurveCorrelationNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.CurveCorrelation))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsRelativeErrorCurveNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.RelativeErrorCurve))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsExportStatusNull AndAlso myHisWSResultsRow.ExportStatus.Trim <> String.Empty) Then
                                cmdText.Append(", '" & myHisWSResultsRow.ExportStatus.Trim & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsBlankAbsorbanceLimitNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.BlankAbsorbanceLimit))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsKineticBlankLimitNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.KineticBlankLimit))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsFactorLowerLimitNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.FactorLowerLimit))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsFactorUpperLimitNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.FactorUpperLimit))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsABSInitialNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.ABSInitial))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsABSWorkReagentNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.ABSWorkReagent))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsABSMainFilterNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.ABSMainFilter))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsMinRefRangeNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.MinRefRange))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsMaxRefRangeNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSResultsRow.MaxRefRange))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsRemarkAlertNull AndAlso myHisWSResultsRow.RemarkAlert.Trim <> String.Empty) Then
                                cmdText.Append(", '" & myHisWSResultsRow.RemarkAlert.Trim & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsAlarmListNull AndAlso myHisWSResultsRow.AlarmList.Trim <> String.Empty) Then
                                cmdText.Append(", '" & myHisWSResultsRow.AlarmList.Trim & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsNotCalcCalibNull) Then
                                cmdText.Append(", " & IIf(myHisWSResultsRow.NotCalcCalib, "1", "0").ToString)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSResultsRow.IsClosedResultNull) Then
                                cmdText.Append(", " & IIf(myHisWSResultsRow.ClosedResult, "1", "0").ToString)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            'AG 24/04/2013
                            If (Not myHisWSResultsRow.IsLISMessageIDNull) Then
                                cmdText.Append(", '" & myHisWSResultsRow.LISMessageID.Trim & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If
                            'AG 24/04/2013

                            cmdText.Append(")")

                            dbCmd.CommandText = cmdText.ToString()
                            myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            cmdText.Length = 0
                        Next
                    End Using

                    myGlobalDataTO.SetDatos = pHisWSResultsDS
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Patient Results for the informed list of AnalyzerID / WorkSessionID / HistOrderTestID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSOrderTestsDS">Typed DataSet HisWSOrderTestsDS with the list of AnalyzerID / WorkSessionID / HistOrderTestID 
        '''                                  selected to be deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/07/2013
        ''' </remarks>
        Public Function DeleteResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()

                ElseIf (Not pHisWSOrderTestsDS Is Nothing) Then
                    Dim myGlobalBase As New GlobalBase
                    Dim myUserName As String = GlobalBase.GetSessionInfo().UserName().Trim

                    Dim i As Integer = 0
                    Dim maxDeletes As Integer = 10000

                    Debug.Print(DateTime.Now.ToString("H:mm:ss:fff") & " - start delete results")
                    For Each row As HisWSOrderTestsDS.thisWSOrderTestsRow In pHisWSOrderTestsDS.thisWSOrderTests
                        cmdText.Append(" DELETE FROM thisWSResults ")
                        cmdText.AppendFormat(" WHERE AnalyzerID = N'{0}'", row.AnalyzerID.Trim.Replace("'", "''"))
                        cmdText.AppendFormat(" AND   WorkSessionID = '{0}'", row.WorkSessionID.Trim)
                        cmdText.AppendFormat(" AND   HistOrderTestID = {0}", row.HistOrderTestID.ToString & vbCrLf)

                        'Increment the sentences counter and verify if the max has been reached
                        i += 1
                        If (i = maxDeletes) Then
                            'Execute the SQL script
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                dataToReturn.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using

                            'Initialize the counter and the StringBuilder
                            i = 0
                            cmdText.Remove(0, cmdText.Length)
                        End If
                    Next

                    If (Not dataToReturn.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining Deletes...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                dataToReturn.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If

                    If Not dataToReturn.HasError Then
                        Debug.Print(DateTime.Now.ToString("H:mm:ss:fff") & " - start delete history data (Store Procedure)")
                        cmdText.Clear()
                        cmdText.Append("DeleteHistPatientData")
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            dbCmd.CommandType = CommandType.StoredProcedure
                            dbCmd.CommandTimeout = Integer.MaxValue
                            dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                            dataToReturn.HasError = False
                        End Using
                    End If

                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.DeleteResults", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' After a result has been exported from historical results the field used for upload to LIS are cleared (free database space)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID">LIS Message Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 24/04/2013
        ''' </remarks>
        Public Function ClearIdentifiersForLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISMessageID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE thisWSResults " & vbCrLf
                    cmdText &= " SET   LISMessageID = NULL " & vbCrLf
                    cmdText &= " WHERE LISMessageID = '" & pLISMessageID & "' " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.ClearIdentifiersForLIS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' This function updates the field ClosedResult to 1 of old Blanks and Calibrators
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter, informed when the function is called from the process
        '''                           of moving results to Historic Module; when the function is called due to the current Test Version
        '''                           has been closed, it is not informed </param>
        ''' <param name="pSampleClass">Sample Class Code. Optional parameter; when not informed, both Blank and Calibrator results are closed</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  JB 10/10/2012
        ''' Modified by: SA 19/10/2012 - Do not filter Historic OrderTests by TestVersionNumber; OPEN Blanks can belongs to a 
        '''                              TestVersion already closed (it happens when Calibration data is changed for the Test/SampleType)
        ''' </remarks>
        Public Function CloseOLDBlankCalibResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, _
                                                  ByVal pSampleType As String, Optional ByVal pAnalyzerID As String = "", _
                                                  Optional ByVal pSampleClass As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else

                    Dim cmdText As String = " UPDATE thisWSResults SET ClosedResult = 1 " & vbCrLf

                    Dim sampleClassFilter As String = " IN ('BLANK', 'CALIB') "
                    If (String.Compare(pSampleClass.Trim, String.Empty, False) <> 0) Then sampleClassFilter = " = '" & pSampleClass.Trim & "' "

                    If (String.Compare(pAnalyzerID.Trim, String.Empty, False) = 0) Then
                        cmdText &= " WHERE ClosedResult = 0 " & vbCrLf & _
                                   " AND   HistOrderTestID IN (SELECT HistOrderTestID FROM thisWSOrderTests " & vbCrLf & _
                                                             " WHERE  SampleClass " & sampleClassFilter & vbCrLf & _
                                                             " AND    TestType   =  'STD' " & vbCrLf & _
                                                             " AND    HistTestID =  " & pHistTestID.ToString & vbCrLf

                    Else
                        cmdText &= " WHERE  AnalyzerID  = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                   " AND   ClosedResult = 0 " & vbCrLf & _
                                   " AND   HistOrderTestID IN (SELECT HistOrderTestID FROM thisWSOrderTests " & vbCrLf & _
                                                             " WHERE  AnalyzerID =  N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                             " AND    SampleClass " & sampleClassFilter & vbCrLf & _
                                                             " AND    TestType   =  'STD' " & vbCrLf & _
                                                             " AND    HistTestID =  " & pHistTestID.ToString & vbCrLf
                    End If

                    If (String.Compare(pSampleType.Trim, String.Empty, False) <> 0) Then cmdText &= " AND SampleType =  '" & pSampleType.Trim & "' " & vbCrLf
                    cmdText &= ")"

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.CloseOLDBlankCalibResults", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all the Historic Results for Several OrderTestID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">List of OrderTestIS separated by comma</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  DL 23/10/2012
        ''' </remarks>
        Public Function GetResultsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vhisWSResults WHERE HistOrderTestID IN(" & pOrderTestID & ")"

                        Dim lastResultsDS As New HisWSResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(lastResultsDS.vhisWSResults)
                            End Using
                        End Using

                        resultData.SetDatos = lastResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.GetResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the historic curve average results to generate a report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pHistOrderTestID">Historic order test identifier</param>
        ''' <param name="pDecimalsAllowed">Test decimals allowed configuration</param>
        ''' <returns>GlobalDataTo with dataset as ResultsDS.ReportCalibCurve</returns>
        ''' <remarks>
        ''' Created by XB 30/07/2014 - BT #1863
        ''' Modified by XB 26/11/2014 - Correction: add HR.CurveSlope , HR.CurveOffset, HR.CurveCorrelation fields - BA-2141
        ''' </remarks>
        Public Function GetResultsCalibCurveForReport(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                      ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String, _
                                                      ByVal pHistOrderTestID As Integer, _
                                                      ByVal pDecimalsAllowed As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "SELECT HOT.HistOrderTestID, HOT.SampleType,HR.MultipointNumber, " & _
                                  "       CAST(HR.ABSValue AS DECIMAL(20,4)) AS ABSValue, " & _
                                  "       CAST(HR.CONCValue AS DECIMAL(20," & pDecimalsAllowed & ")) AS CONC_Value, " & _
                                  "       CAST(HR.RelativeErrorCurve AS DECIMAL(20,4)) AS RelativeErrorCurve, " & _
                                  "       HR.CalibratorBlankAbsUsed, HTS.CurveGrowthType, HTS.CurveType, HTS.CurveAxisXType, " & _
                                  "       HTS.CurveAxisYType, HTS.TestLongName, HTCV.TheoreticalConcentration As TheoricalConcentration, " & _
                                  "       MD.FixedItemDesc As MeasureUnit, HR.CurveSlope , HR.CurveOffset, HR.CurveCorrelation " & _
                                  " FROM thisWSResults HR INNER JOIN thisWSOrderTests HOT ON HR.HistOrderTestID = HOT.HistOrderTestID " & _
                                  "                       INNER JOIN thisTestSamples HTS ON HOT.HistTestID = HTS.HistTestID AND HOT.SampleType = HTS.SampleType " & _
                                  "                                                     AND HOT.TestVersionNumber = HTS.TestVersionNumber " & _
                                  "                       INNER JOIN thisTestCalibratorsValues HTCV ON HOT.HistTestID = HTCV.HistTestID " & _
                                  "                                                                AND HOT.SampleType = HTCV.SampleType " & _
                                  "                                                                AND HOT.TestVersionNumber = HTCV.TestVersionNumber " & _
                                  "                                                                AND HOT.HistCalibratorID = HTCV.HistCalibratorID " & _
                                  "                                                                AND HR.MultiPointNumber = HTCV.CalibratorNum " & _
                                  "                       INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID AND MD.SubTableID = 'TEST_UNITS' " & _
                                  " WHERE HOT.HistOrderTestID = " & pHistOrderTestID & _
                                  " AND HOT.SampleClass = 'CALIB' " & _
                                  " AND HOT.TestType = 'STD' " & _
                                  " AND HR.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & _
                                  " AND HR.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

                        Dim myDataSet As New ResultsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.ReportCalibCurve)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.GetResultsCalibCurveForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "View to reuse IResultsCalibCurve screen in history"
        ''' <summary>
        ''' Get the historic curve average results (key parameters HistOrderTestID, AnalyzerID, WorkSessionID)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistOrderTestID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo with dataset as ResultsDS.vwksResults</returns>
        ''' <remarks>AG 17/10/2012 - Creation</remarks>
        Public Function GetAvgResultsForCalibCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As Integer, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "SELECT * " & _
                                  " FROM vhisWSCalibCurveAvgResults" & _
                                  " WHERE OrderTestID = " & pHistOrderTestID & _
                                  " AND AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & _
                                  " AND WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

                        Dim myDataSet As New ResultsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.vwksResults)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.GetAvgResultsForCalibCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region " Get Historical Results "
        ''' <summary>
        ''' Get all Historic Patient Results that fulfill the specified filters
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Initial Results Date</param>
        ''' <param name="pDateTo">Final Result Date</param>
        ''' <param name="pPatientData">Part of the Patient ID, LastName or FirstName</param>
        ''' <param name="pSampleTypes">List of Sample Types</param>
        ''' <param name="pStatFlag">Priority Flag</param>
        ''' <param name="pTestTypes">List of Test Types</param>
        ''' <param name="pTestName">Part of the Test Name</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSpecimenID">Part of the SpecimenID (Barcode)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSResultsDS.vhisWSResults with all the Patient results that fulfill the
        '''          specified search criteria</returns>
        ''' <remarks>
        ''' Created by:  JB 19/10/2012
        ''' Modified by: AG 29/10/2012 - Sort returned records by PatientID and ResultDateTime (DESC) 
        '''              JB 07/11/2012 - Fixed search query
        '''              SA 01/08/2014 - BA-1861 ==> - Added new optional parameter pSpecimenID and the corresponding filter in the SQL when it is informed
        '''                                          - Removed parameter pSampleClasses: it is not needed due to this function get only Patient Results
        '''                                          - Changed the use of parameter pPatientData: when it is informed, the query searches if it is part of 
        '''                                            fields PatientID, LastName or FirstName
        '''                                          - Changed the ORDER BY: sort returned data by PatientID, SpecimenID and ResultDateTime
        ''' </remarks>
        Public Function GetHistoricalResultsByFilter(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     Optional ByVal pDateFrom As Date = Nothing, Optional ByVal pDateTo As Date = Nothing, _
                                                     Optional ByVal pPatientData As String = "", Optional ByVal pSampleTypes As List(Of String) = Nothing, _
                                                     Optional ByVal pStatFlag As TriState = TriState.UseDefault, _
                                                     Optional ByVal pTestTypes As List(Of String) = Nothing, Optional ByVal pTestName As String = "", _
                                                     Optional ByVal pWorkSessionID As String = "", Optional ByVal pSpecimenID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdInFilter As String = String.Empty
                        Dim cmdText As String = " SELECT * FROM vhisWSResults " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    SampleClass = 'PATIENT' " & vbCrLf

                        '******************************************************************************
                        '* Add additional filters when the different optional parameters are informed *
                        '******************************************************************************

                        '(1) Range of dates
                        If (pDateFrom <> Nothing) Then cmdText &= " AND ResultDateTime >= '" & Format(pDateFrom, "yyyyMMdd") & "' "
                        If (pDateTo <> Nothing) Then cmdText &= " AND ResultDateTime <= '" & Format(pDateTo, "yyyyMMdd") & "' "

                        '(2) Patient data
                        If (Not String.IsNullOrEmpty(pPatientData)) Then
                            cmdText &= " AND (UPPER(PatientID) LIKE UPPER(N'%" & pPatientData.Trim.Replace("'", "''") & "%') " & vbCrLf & _
                                       " OR   UPPER(LastName) LIKE UPPER(N'%" & pPatientData.Trim.Replace("'", "''") & "%') " & vbCrLf & _
                                       " OR   UPPER(FirstName) LIKE UPPER(N'%" & pPatientData.Trim.Replace("'", "''") & "%')) " & vbCrLf
                        End If

                        '(3) SpecimenID (Barcode)
                        If (Not String.IsNullOrEmpty(pSpecimenID)) Then cmdText &= " AND SpecimenID LIKE UPPER(N'%" & pSpecimenID.Trim.Replace("'", "''") & "%') " & vbCrLf

                        '(4) Priority
                        If (pStatFlag <> TriState.UseDefault) Then cmdText &= " AND StatFlag = " & IIf(pStatFlag = TriState.False, 0, 1).ToString

                        '(5) List of Sample Types
                        If (Not pSampleTypes Is Nothing AndAlso pSampleTypes.Count > 0) Then
                            If (pSampleTypes.Count = 1) Then
                                cmdText &= " AND SampleType = '" & pSampleTypes.First & "' "
                            Else
                                For Each elem As String In pSampleTypes
                                    If (Not String.IsNullOrEmpty(cmdInFilter)) Then cmdInFilter &= ", "
                                    cmdInFilter &= " '" & elem & "' "
                                Next
                                cmdText &= " AND SampleType IN (" & cmdInFilter & ") "
                            End If
                        End If

                        '(6) List of Test Types
                        If (Not pTestTypes Is Nothing AndAlso pTestTypes.Count > 0) Then
                            If (pTestTypes.Count = 1) Then
                                cmdText &= " AND TestType = '" & pTestTypes.First & "' "
                            Else
                                cmdInFilter = String.Empty
                                For Each elem As String In pTestTypes
                                    If (Not String.IsNullOrEmpty(cmdInFilter)) Then cmdInFilter &= ", "
                                    cmdInFilter &= " '" & elem & "' "
                                Next
                                cmdText &= " AND TestType IN (" & cmdInFilter & ") "
                            End If
                        End If

                        '(7) Test Name
                        If (Not String.IsNullOrEmpty(pTestName)) Then cmdText &= " AND TestName LIKE N'%" & pTestName.Trim.replace("'", "''") & "%' "

                        '(8) Work Session Identifier
                        If (pWorkSessionID.Trim <> String.Empty) Then cmdText &= " AND WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

                        '*********************************************************************************
                        '* Sort returned data by PatientID, HistPatientID, SpecimenID and ResultDateTime *
                        '*********************************************************************************
                        cmdText &= " ORDER BY PatientID, HistPatientID, SpecimenID, ResultDateTime DESC "

                        Dim myDataSet As New HisWSResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.vhisWSResults)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.GetHistoricalResultsByFilter", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Historic Blank and Calibrator Results that fulfill the specified filters
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Initial Results Date</param>
        ''' <param name="pDateTo">Final Result Date</param>
        ''' <param name="pTestNameContains">Part of the Test Name</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSResultsDS.vhisWSResults with all the Blank and Calibrator results that 
        '''          fulfill the specified search criteria</returns>
        ''' <remarks>
        ''' Created by:  AG 22/10/2012
        ''' Modified by: AG 29/10/2012 - Order by TestName, ResultDateTime DESC (AG + EF meeting)
        '''              JB 07/11/2012 - Fixed search filter 
        ''' </remarks>
        Public Function GetHistoricalBlankCalibResultsByFilter(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                               Optional ByVal pDateFrom As Date = Nothing, Optional ByVal pDateTo As Date = Nothing, _
                                                               Optional ByVal pTestNameContains As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vhisWSBlankCalibResults " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' "

                        'Range of dates
                        If (pDateFrom <> Nothing) Then cmdText &= " AND ResultDateTime >= '" & Format(pDateFrom, "yyyyMMdd") & "' "
                        If (pDateTo <> Nothing) Then cmdText &= " AND ResultDateTime <= '" & Format(pDateTo, "yyyyMMdd") & "' "

                        'Test Name
                        If (Not String.IsNullOrEmpty(pTestNameContains)) Then cmdText &= " AND UPPER(TestName) LIKE UPPER(N'%" & pTestNameContains.Trim.Replace("'", "''") & "%') "

                        'Sort records by TestName and ResultDateTime (DESC)
                        cmdText &= " ORDER BY TestName, ResultDateTime DESC "

                        Dim myDataSet As New HisWSResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.vhisWSResults)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.GetHistoricalBlankCalibResultsByFilter", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region " Methods for export results to LIMS "
        ''' <summary>
        ''' Get Historical Patient and Historical Results to export in a ResultsDS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestID">The list of HistOrderTestId to be exported</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (subtable vwksResults) with the group of Results to export</returns>
        ''' <remarks>
        ''' Created by:  JB 22/10/2012
        ''' Modified by: SA 17/12/2012 - Changed the SQL to manage properly all test types to export; the current query does not get the name of non-
        '''                              standard Tests, exporting ISE, CALC and OFFS Tests with the name of the Standard Test having the same ID (if any)
        '''              SG 25/04/2013 - Changed the SQL to get also LIS fields
        '''              SA 09/07/2013 - Changed the SQL to get also field AnalyzerID
        '''              SA 15/01/2014 - BT #1453 ==> Changed the SQL to get also field WorkSessionID
        ''' </remarks>
        Public Function GetResultsToExportFromHIST(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HR.HistOrderTestID AS OrderTestID, 1 as RerunNumber, HR.MultiPointNumber, HR.ResultDateTime, HR.StatFlag, " & vbCrLf & _
                                                       " HR.LISRequest, HR.ESOrderID, HR.ESPatientID, HR.LISOrderID, HR.LISPatientID, " & vbCrLf & _
                                                       " HR.LISTestName, HR.LISSampleType, HR.LISUnits, " & vbCrLf & _
                                                      " (CASE WHEN StatFlag = 0 THEN 'N' ELSE 'U' END) AS SampleClass, " & vbCrLf & _
                                                      " (CASE WHEN HR.PatientID IS NOT NULL AND HR.HistPatientID IS NULL THEN HR.PatientID ELSE NULL END) AS PatientID, " & vbCrLf & _
                                                      " (CASE WHEN HR.HistPatientID IS NOT NULL THEN P.PatientID ELSE NULL END) AS SampleID, " & vbCrLf & _
                                                      "  HR.TestType, HR.TestName, HR.SampleType, '' AS TubeType, HR.CONCValue AS CONC_Value, " & vbCrLf & _
                                                      "  HR.MeasureUnit, HR.ManualResultFlag, HR.ManualResult, HR.ManualResultText, HR.AnalyzerID, " & vbCrLf & _
                                                      "  HR.WorkSessionID, '' AS ControlName, '' AS LotNumber " & vbCrLf & _
                                                " FROM   vhisWSResults HR LEFT JOIN thisPatients P ON P.HistPatientID = HR.HistPatientID " & vbCrLf & _
                                                " WHERE  HR.SampleClass = 'PATIENT' " & vbCrLf

                        Dim cmdOrderTests As String = ""
                        For Each elem As Integer In pHistOrderTestID
                            If (Not String.IsNullOrEmpty(cmdOrderTests)) Then cmdOrderTests &= ", "
                            cmdOrderTests &= elem.ToString
                        Next

                        If (Not String.IsNullOrEmpty(cmdOrderTests)) Then
                            cmdText &= " AND HR.HistOrderTestID IN (" & cmdOrderTests & ") "
                        End If

                        cmdText &= " ORDER BY SampleClass, SampleID, PatientID "

                        Dim myResultsDS As New ResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myResultsDS.vwksResults)
                            End Using
                        End Using

                        resultData.SetDatos = myResultsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.GetResultsToExportFromHIST", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields ExportStatus and ExportDataTime for the group of results sent to and external LIMS system
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the exported results</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  JB 22/10/2012
        ''' Modified by: SG 10/04/2013 - Add new parameter "pAlternativeStatus"
        '''              AG 17/04/2013 - Use pAlternativeStatus, SG only defined it but not business was done
        ''' </remarks>
        Public Function UpdateExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, ByVal pAlternativeStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As New StringBuilder
                    For Each myResultsRow As ResultsDS.vwksResultsRow In pResultsDS.vwksResults.Rows
                        cmdText.Append("UPDATE thisWSResults SET" & vbCrLf)

                        If pAlternativeStatus.Length > 0 Then
                            cmdText.Append("ExportStatus = '" & pAlternativeStatus.Trim & "' " & vbCrLf)
                        ElseIf (myResultsRow.IsExportStatusNull) Then 'If (myResultsRow.IsExportStatusNull) Then
                            cmdText.Append("ExportStatus = 'SENT'" & vbCrLf)
                        Else
                            cmdText.Append("ExportStatus = '" & myResultsRow.ExportStatus.Trim & "'" & vbCrLf)
                        End If

                        cmdText.Append("WHERE HistOrderTestID = " & myResultsRow.OrderTestID.ToString & vbCrLf)
                        cmdText.Append(Environment.NewLine)
                    Next

                    If cmdText.Length > 0 Then
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), pDBConnection)
                            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.UpdateExportStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update field ExportStatus for the group of Historical Order Test IDs sent to LIS using the ES Library
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestID">The list of HistOrderTestId to be exported</param>
        ''' <param name="pExportStatus">Value of the Export Status to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/01/2014
        ''' </remarks>
        Public Function UpdateLISExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As List(Of Integer), _
                                              ByVal pExportStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As New StringBuilder
                    For Each myOTID As Integer In pHistOrderTestID
                        cmdText.Append("UPDATE thisWSResults SET " & vbCrLf)
                        cmdText.Append("ExportStatus = '" & pExportStatus.Trim & "'" & vbCrLf)
                        cmdText.Append("WHERE HistOrderTestID = " & myOTID.ToString & vbCrLf)
                        cmdText.Append(Environment.NewLine)
                    Next

                    If (cmdText.Length > 0) Then
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), pDBConnection)
                            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.UpdateLISExportStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Updates the LISMessageID (requires ExportStatus to "SENDING")
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' Modified by: DL 24/04/2013. Updates the columns: ExportStatus (SENDING), LISMessageID (guid number) Where HistOrderTestID = OrderTest in DS parameter
        ''' AG 25/04/2013 - Set ExportStatus from DataSet, in WHERE clausule add SENDING
        ''' AG 14/02/2014 - #1505 (Remove clause add SENDING, because we have commented the code that set as SENDING in ExportDelegate methods) - ACTIVATED 24/03/2014 (PAUSED 17/02/2014)
        ''' </remarks>
        Public Function UpdateLISMessageID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pHistWSResultsDS As HisWSResultsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    For Each HistResultRow As HisWSResultsDS.thisWSResultsRow In pHistWSResultsDS.thisWSResults
                        Dim cmdText As String = ""

                        cmdText &= "UPDATE thisWSResults " & vbCrLf
                        cmdText &= "   SET ExportStatus = '" & HistResultRow.ExportStatus & "' " & vbCrLf
                        cmdText &= "      ,LISMessageID = '" & HistResultRow.LISMessageID & "' " & vbCrLf
                        cmdText &= " WHERE HistOrderTestID = " & HistResultRow.HistOrderTestID
                        'cmdText &= " AND ExportStatus = 'SENDING'" 'AG 24/03/2014 - AG 17/02/2014 this line must be COMMENTED when implement #1505 point 7 '(AG 14/02/2014 - #1505 comment this line)

                        'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                            If resultData.AffectedRecords > 0 Then
                                resultData.HasError = False
                                resultData.SetDatos = pHistWSResultsDS
                            Else
                                resultData.HasError = True
                                resultData.AffectedRecords = 0
                            End If
                        End Using
                        'AG 25/07/2014
                    Next

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.UpdateLISMessageID", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Updates the LISMessageID depnding on ExportStatus
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID"></param>
        ''' <param name="pExportStatus"></param>
        ''' <returns>GlobalDataTo returns the affectedREcords</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' Modified by: DL 24/04/2013. Updates ExportStatus = NewExportStatus for those historical results with LISMessageID = parameter LISMEssageID
        ''' AG 25/04/2013 - where ALSO exportstatus = SENDING
        ''' </remarks>
        Public Function UpdateExportStatusByMessageID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                      ByVal pLISMessageID As String, _
                                                      ByVal pExportStatus As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    cmdText &= "UPDATE thisWSResults" & vbCrLf
                    cmdText &= "   SET ExportStatus = '" & pExportStatus & "'" & vbCrLf
                    cmdText &= " WHERE LISMessageID = '" & pLISMessageID & "'"
                    cmdText &= " AND ExportStatus = 'SENDING' "

                    'AG 25/07/2014 #1886 - RQ00086 - improve memory usage
                    'Dim dbCmd As New SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText
                    'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.UpdateExportStatusByMessageID", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Delete all Results saved in Historic Module for the specified AnalyzerID / WorkSessionID / OrderTestID 
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestID">Identifier of the Order Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function DeleteByHistOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                        ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM thisWSResults " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                    " AND    HistOrderTestID = " & pHistOrderTestID.ToString & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.DeleteByHistOrderTestID", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' THIS FUNCTION IS NOT USED ANY MORE (SA 01/07/2013)
        ' ''' Delete the Historical Results from thisWSResults 
        ' ''' The function deletes all points of the OrderTests in the parameters
        ' ''' </summary>
        ' ''' <param name="pDBConnection"></param>
        ' ''' <param name="pAnalyzerId"></param>
        ' ''' <param name="pHistOrderTestList"></param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by: JB 19/10/2012
        ' ''' </remarks>
        'Public Function DeleteResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pHistOrderTestList As List(Of Integer)) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        If pDBConnection Is Nothing Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdWhere As String = ""
        '            For Each orderTest As Integer In pHistOrderTestList
        '                If Not String.IsNullOrEmpty(cmdWhere) Then cmdWhere &= " OR "
        '                cmdWhere &= " ( "
        '                cmdWhere &= " AnalyzerID = '" & pAnalyzerID & "'"
        '                cmdWhere &= " AND HistOrderTestID = " & orderTest.ToString
        '                cmdWhere &= " ) "
        '            Next
        '            Dim cmdText As String = ""
        '            cmdText &= " DELETE thisWSResults "
        '            cmdText &= " WHERE " & cmdWhere

        '            Using dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = pDBConnection
        '                dbCmd.CommandText = cmdText
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using

        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSResultsDAO.DeleteResults", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace


Option Strict On
Option Explicit On

Imports System.Text
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tqcResultsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Insert new QC Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' Modified by: SA 01/06/2011 - Replaced use of function ToSQLString in single values by function ReplaceNumericString to avoid loss of accuracy
        '''                            - Added N prefix and single quote replacement in string values ResultComment and TS_User
        '''                            - Boolean values are sent as 0 or 1 instead of False or True
        '''              SA 04/06/2012 - Changed the query to insert also new field AnalyzerID
        '''              TR 23/07/2012 - Add the new rows CtrlSendingGroup, SampleClass.
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim currentSession As New GlobalBase
                    For Each qcResultsRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults.Rows
                        cmdText &= " INSERT INTO tqcResults (QCTestSampleID, QCControlLotID, AnalyzerID, RunsGroupNumber, RunNumber, " & vbCrLf & _
                                                           " ResultDateTime, ManualResultFlag, ResultValue, ManualResultValue, ResultComment, " & vbCrLf & _
                                                           " ValidationStatus, Excluded, ClosedResult, CtrlsSendingGroup, SampleClass, TS_User, TS_DateTime) " & vbCrLf & _
                                   " VALUES (" & qcResultsRow.QCTestSampleID & ", " & vbCrLf & _
                                                 qcResultsRow.QCControlLotID & ", " & vbCrLf & _
                                         " N'" & qcResultsRow.AnalyzerID.Replace("'", "''").Trim & "', " & vbCrLf & _
                                                 qcResultsRow.RunsGroupNumber & ", " & vbCrLf & _
                                                 qcResultsRow.RunNumber & ", " & vbCrLf & _
                                          " '" & qcResultsRow.ResultDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                                 Convert.ToInt32(IIf(qcResultsRow.ManualResultFlag, 1, 0)) & ", " & vbCrLf

                        If (qcResultsRow.IsResultValueNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= ReplaceNumericString(qcResultsRow.ResultValue) & ", "
                        End If

                        If (qcResultsRow.IsManualResultValueNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= ReplaceNumericString(qcResultsRow.ManualResultValue) & ", "
                        End If

                        If (qcResultsRow.IsResultCommentNull) Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= " N'" & qcResultsRow.ResultComment.Replace("'", "''") & "', "
                        End If

                        cmdText &= " '" & qcResultsRow.ValidationStatus.Trim & "', "
                        cmdText &= Convert.ToInt32(IIf(qcResultsRow.Excluded, 1, 0)) & ", "
                        cmdText &= Convert.ToInt32(IIf(qcResultsRow.ClosedResult, 1, 0)) & ", "

                        'TR 23/07/2012 -Inser values for new rows
                        If qcResultsRow.IsCtrlsSendingGroupNull Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= qcResultsRow.CtrlsSendingGroup & ", "
                        End If

                        If qcResultsRow.IsSampleTypeNull Then
                            cmdText &= " NULL, "
                        Else
                            cmdText &= "'" & qcResultsRow.SampleType & "', "
                        End If
                        'TR 23/07/2012 - END

                        If (qcResultsRow.IsTS_UserNull) Then
                            cmdText &= " N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "', "
                        Else
                            cmdText &= " N'" & qcResultsRow.TS_User.Replace("'", "''") & "', " & vbCrLf
                        End If

                        If (qcResultsRow.IsTS_DateTimeNull) Then
                            cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                        Else
                            cmdText &= " '" & qcResultsRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                        End If
                        cmdText &= vbCrLf
                    Next

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete a group of QC Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/06/2011
        ''' Modified by: SA 04/06/2012 - Changed the query by adding a filter by field AnalyzerID
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    Dim currentSession As New GlobalBase
                    For Each qcResultsRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults.Rows
                        cmdText &= " DELETE FROM tqcResults " & vbCrLf & _
                                   " WHERE  QCTestSampleID = " & qcResultsRow.QCTestSampleID & vbCrLf & _
                                   " AND    QCControlLotID = " & qcResultsRow.QCControlLotID & vbCrLf & _
                                   " AND    AnalyzerID      = N'" & qcResultsRow.AnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                   " AND    RunsGroupNumber = " & qcResultsRow.RunsGroupNumber & vbCrLf & _
                                   " AND    RunNumber = " & qcResultsRow.RunNumber & vbCrLf
                    Next

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID and QCControlLotID, delete the QC Results included in the RunsGroup for 
        ''' the informed Cumulated Serie
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCumResultsNum">Number of the Cumulated Serie to be deleted for the QCTestSampleID and QCControlLotID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/06/2011 
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function DeleteByCumResultsNumNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                 ByVal pAnalyzerID As String, ByVal pCumResultsNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tqcResults " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND    RunsGroupNumber IN (SELECT RunsGroup FROM tqcRunsGroups " & vbCrLf & _
                                                                       " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                                       " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
                                                                       " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                                       " AND    CumResultsNum  = " & pCumResultsNum.ToString & ") " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.DeleteByCumResultsNum", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID and QCControlLotID, delete all the QC Results included in the informed RunsGroup
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNum">Number of the Runs Group in which the QC Results are included</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 27/06/2011 
        ''' Modified by: SA 19/12/2011 - Added filter by field IncludedInMean=FALSE, due to QC Results used to calculate statistic values 
        '''                              are not physically deleted when the RunsGroup is cumulated
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function DeleteByRunsGroupNumNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                ByVal pAnalyzerID As String, ByVal pRunsGroupNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tqcResults " & vbCrLf & _
                                            " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND    RunsGroupNumber = " & pRunsGroupNum.ToString & vbCrLf & _
                                            " AND    IncludedInMean  = 0 " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.DeleteByRunsGroupNum", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update fields ManualResultValue, ResultComment and/or Excluded flag for an specific QC Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing data of the QC Result to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 08/06/2011
        ''' Modified by: SA 04/06/2012 - Changed the query by adding a filter by field AnalyzerID 
        ''' </remarks>
        Public Function UpdateManualResultNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (pQCResultsDS.tqcResults.Count > 0) Then
                    Dim currentSession As New GlobalBase
                    Dim cmdText As String = " UPDATE tqcResults SET "

                    If (Not pQCResultsDS.tqcResults(0).IsResultCommentNull) Then
                        cmdText &= " ResultComment = N'" & pQCResultsDS.tqcResults(0).ResultComment.Replace("'", "''") & "' "
                    End If

                    If (Not pQCResultsDS.tqcResults(0).IsExcludedNull) Then
                        If (cmdText.Length > 0) Then
                            cmdText &= ", "
                        End If
                        cmdText &= " Excluded = " & Convert.ToInt32(IIf(pQCResultsDS.tqcResults(0).Excluded, 1, 0))
                    End If

                    If (Not pQCResultsDS.tqcResults(0).IsManualResultValueNull) Then
                        If (cmdText.Length > 0) Then
                            cmdText &= ", "
                        End If
                        cmdText &= " ManualResultValue = " & ReplaceNumericString(pQCResultsDS.tqcResults(0).ManualResultValue) & ", "
                        cmdText &= " ManualResultFlag  = 1 "
                    End If

                    cmdText &= " WHERE QCTestSampleID  = " & pQCResultsDS.tqcResults(0).QCTestSampleID
                    cmdText &= " AND   QCControlLotID  = " & pQCResultsDS.tqcResults(0).QCControlLotID
                    cmdText &= " AND   AnalyzerID      = N'" & pQCResultsDS.tqcResults(0).AnalyzerID.Replace("'", "''").Trim & "' "
                    cmdText &= " AND   RunsGroupNumber = " & pQCResultsDS.tqcResults(0).RunsGroupNumber
                    cmdText &= " AND   RunNumber       = " & pQCResultsDS.tqcResults(0).RunNumber

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.UpdateManualResult", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set ValidationStatus=OK for all results of the informed QCControlLotID/QCTestSampleID/RunsGroupNumber that are inside
        ''' the specified range of dates 
        ''' OR 
        ''' Set ValidationStatus=PENDING for all results of the informed QCControlLotID/QCTestSampleID/RunsGroupNumber that are 
        ''' outside the specified range of dates 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        ''' <param name="pInDateRange">Flag indicating which of the two Updates have to be executed: when TRUE, the function set 
        '''                            ValidationStatus=OK for all results inside the range of dates; when FALSE, the function set
        '''                            ValidationStatus=PENDING for all results outside the range of dates</param>
        ''' <param name="pDateFrom">Date From to search results</param>
        ''' <param name="pDateTo">Date To to search results</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2011
        ''' Modified by: SA 27/06/2011 - Changed filter by dates when pInDateRange=False; it does not work
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function UpdateValStatusByDateRangeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, ByVal pQCTestSampleID As Integer, _
                                                      ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer, ByVal pInDateRange As Boolean, _
                                                      ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    If (pInDateRange) Then
                        cmdText = " UPDATE tqcResults SET ValidationStatus = 'OK' " & vbCrLf & _
                                  " WHERE  QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                  " AND    QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                  " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                  " AND    RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf & _
                                  " AND    DATEDIFF(day, '" & pDateFrom.ToString("yyyyMMdd") & "', ResultDateTime) > = 0 " & vbCrLf & _
                                  " AND    DATEDIFF(day, ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') > = 0 " & vbCrLf
                    Else
                        cmdText = " UPDATE tqcResults SET ValidationStatus = 'PENDING' " & vbCrLf & _
                                  " WHERE  QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                  " AND    QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                  " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                  " AND    RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf & _
                                  " AND    (DATEDIFF(day, '" & pDateFrom.ToString("yyyyMMdd") & "', ResultDateTime) < 0 " & vbCrLf & _
                                  " OR      DATEDIFF(day, ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') < 0) " & vbCrLf
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.UpdateValStatusByDateRange", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the ValidationStatus with the specified value for a QC Result of the informed QCControlLotID/QCTestSampleID/RunsGroupNumber 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        ''' <param name="pRunNumber">Number of the execution that identify the specific Result which ValidationStatus has to be updated</param>
        ''' <param name="pValStatus">Value to assign as Validation Status of the Results</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function UpdateValStatusByResultNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, ByVal pQCTestSampleID As Integer, _
                                                   ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer, ByVal pValStatus As String, _
                                                   ByVal pRunNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcResults " & vbCrLf & _
                                            " SET    ValidationStatus = '" & pValStatus & "' " & vbCrLf & _
                                            " WHERE  QCControlLotID   = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND    QCTestSampleID   = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND    RunsGroupNumber  = " & pRunsGroupNumber.ToString() & vbCrLf & _
                                            " AND    RunNumber        = " & pRunNumber

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.UpdateValStatusByResult", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get the number of non cumulated and not excluded QC Results for the specified QCTestSampleID/QCControlLotID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of non cumulated and not excluded QC Results
        '''          for the specified QCTestSampleID/QCControlLotID/AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  SA 20/06/2011
        ''' Modified by: SA 20/12/2011 - Added filter by IncludedInMean=FALSE to avoid counting QC Results used to calculate 
        '''                              statistic values when the specified Test/SampleType has STATISTIC Calculation Mode  
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function CountNonCumulatedResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                    ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) FROM tqcResults " & vbCrLf & _
                                                " WHERE  QCControlLotID = " & pQCControlLotID & vbCrLf & _
                                                " AND    QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    ClosedResult   = 0 " & vbCrLf & _
                                                " AND    Excluded       = 0 " & vbCrLf & _
                                                " AND    IncludedInMean = 0 "

                        'Get the value and set it to the set of data to return
                        Dim nonCumulatedResults As Integer = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            nonCumulatedResults = Convert.ToInt32(dbCmd.ExecuteScalar())
                        End Using

                        myGlobalDataTO.SetDatos = nonCumulatedResults
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.CountNonCumulatedResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Return the number of results for and specific runsgroup where the included mean is true.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pQCTestSampleID"></param>
        ''' <param name="pQCControlLotID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pCumSerieNum"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR/SA</remarks>
        Public Function CountStatisticsSResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                    ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String, ByVal pCumSerieNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT R.RunsGroupNumber FROM tqcResults R " & vbCrLf & _
                                                "  INNER JOIN tqcRunsGroups RG ON R.QCTestSampleID = RG.QCTestSampleID " & vbCrLf & _
                                                                           " AND R.QCControlLotID = RG.QCControlLotID " & vbCrLf & _
                                                                           " AND R.AnalyzerID = RG.AnalyzerID " & vbCrLf & _
                                                                           " AND R.RunsGroupNumber = RG.RunsGroupNumber " & vbCrLf & _
                                                " WHERE  R.QCControlLotID = " & pQCControlLotID & vbCrLf & _
                                                " AND    R.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    R.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    R.IncludedInMean = 1 " & vbCrLf & _
                                                " AND    RG.CumResultsNum = " & pCumSerieNum & vbCrLf

                        'Get the value and set it to the set of data to return

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                myGlobalDataTO.SetDatos = CInt(dbDataReader.Item("RunsGroupNumber"))

                            Else
                                myGlobalDataTO.SetDatos = 0
                            End If
                            dbDataReader.Close()
                        End Using

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.CountStatisticsSResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCControlLotID/QCTestSampleID/AnalyzerID/RunsGroupNumber, count the number of non cumulated 
        ''' Results pending to review
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Results pending to review</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2011
        ''' Modified by: SA 20/12/2011 - Added filter by IncludedInMean=FALSE to avoid counting QC Results that will not be cumulated
        '''                              (those that are used to calculate statistic values for Test/SampleTypes having STATISTIC
        '''                              Calculation Mode) 
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function CountNotReviewedResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, _
                                                   ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS ReviewPending FROM tqcResults " & vbCrLf & _
                                                " WHERE  QCControlLotID   = " & pQCControlLotID & vbCrLf & _
                                                " AND    QCTestSampleID   = " & pQCTestSampleID & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    RunsGroupNumber  = " & pRunsGroupNumber & vbCrLf & _
                                                " AND    ValidationStatus = 'PENDING' " & vbCrLf & _
                                                " AND    ClosedResult     = 0 " & vbCrLf & _
                                                " AND    Excluded         = 0 " & vbCrLf & _
                                                " AND    IncludedInMean   = 0 "

                        'Get the value and set it to the set of data to return
                        Dim pendingResults As Integer = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            pendingResults = Convert.ToInt32(dbCmd.ExecuteScalar())
                        End Using

                        myGlobalDataTO.SetDatos = pendingResults
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.CountNotReviewedResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCControlLotID/QCTestSampleID/AnalyzerID/RunsGroupNumber, count the number of non cumulated 
        ''' Results having alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Results with alarms</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2011
        ''' Modified by: SA 20/12/2011 - Added filter by IncludedInMean=FALSE to avoid counting QC Results that will not be cumulated
        '''                              (those that are used to calculate statistic values for Test/SampleTypes having STATISTIC
        '''                              Calculation Mode) 
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function CountResultsWithAlarmsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, _
                                                  ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS WithAlarms FROM tqcResults " & vbCrLf & _
                                                " WHERE  QCControlLotID  = " & pQCControlLotID & vbCrLf & _
                                                " AND    QCTestSampleID  = " & pQCTestSampleID & vbCrLf & _
                                                " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    RunsGroupNumber = " & pRunsGroupNumber & vbCrLf & _
                                                " AND    ValidationStatus IN ('WARNING', 'ERROR') " & vbCrLf & _
                                                " AND    ClosedResult = 0 " & vbCrLf & _
                                                " AND    Excluded     = 0 " & vbCrLf

                        'Get the value and set it to the set of data to return
                        Dim resultWithAlarms As Integer = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultWithAlarms = Convert.ToInt32(dbCmd.ExecuteScalar())
                        End Using

                        myGlobalDataTO.SetDatos = resultWithAlarms
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.CountResultsWithAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all QC Results marked as included in calculation of statistical values for the informed QCTestSampleID/QCControlLotID/AnalyzerID
        ''' and optionally, RunsGroupNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Optional parameter. Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Optional parameter. Number of the open Runs Group for the QCTestSampleID/QCControlLotID/AnalyzerID</param>
        ''' <returns>GlobalDataTO containing success/error infomration</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 21/12/2011 - Parameter for the RunsGroupNumber changed to optional
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID; changed the query by adding a filter by this field when informed
        ''' </remarks>
        Public Function DeleteStatisticResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                  Optional ByVal pAnalyzerID As String = "", Optional ByVal pRunsGroupNumber As Integer = 0) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tqcResults " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND    IncludedInMean = 1 "

                    'Add the filter by optional parameters when they are informed
                    If (pRunsGroupNumber <> 0) Then cmdText &= " AND RunsGroupNumber = " & pRunsGroupNumber.ToString & vbCrLf
                    If (pAnalyzerID.Trim <> String.Empty) Then cmdText &= " AND AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.DeleteStatisticResults ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search in history table of QC Module for the specified AnalyzerID, all Controls/Lots having ClosedLot = FALSE and DeleteControl=FALSE 
        ''' and having for at least a Test/SampleType, enough QC Results pending to accumulate
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of all Controls/Lots in 
        '''          the history table in QC Module that have QC Results thar can be cumulated </returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2011
        ''' Modified by: SA 19/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        '''                              values (due to these values are not cumulated)
        '''              SA 02/02/2012 - Added a filter by field Excluded=FALSE to avoid including as data to cumulate QC Results that
        '''                              have been excluded  
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetControlsToCumulateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT   DISTINCT R.QCControlLotID, HCL.ControlName, HCL.LotNumber " & vbCrLf & _
                                                " FROM     tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                      " INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                " WHERE    R.AnalyzerID          = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND      R.ClosedResult        = 0 " & vbCrLf & _
                                                " AND      R.Excluded            = 0 " & vbCrLf & _
                                                " AND      R.IncludedInMean      = 0 " & vbCrLf & _
                                                " AND      HTS.DeletedSampleType = 0 " & vbCrLf & _
                                                " AND      HTS.DeletedTest       = 0 " & vbCrLf & _
                                                " AND      HCL.ClosedLot         = 0 " & vbCrLf & _
                                                " AND      HCL.DeletedControl    = 0 " & vbCrLf & _
                                                " GROUP BY R.QCControlLotID, HCL.ControlName, HCL.LotNumber " & vbCrLf & _
                                                " ORDER BY HCL.ControlName "

                        Dim myHistoryControlLotsDS As New HistoryControlLotsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryControlLotsDS.tqcHistoryControlLots)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHistoryControlLotsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetControlsToCumulate", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all not excluded QC Results for the specified QCTestSampleID/QCControlLotID/AnalyzerID and calculate all data needed to 
        ''' create a new cumulated serie
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param> 
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultsCalculationDS with the needed data</returns>
        ''' <remarks>
        ''' Created by:  TR 19/05/2011
        ''' Modified by: SA 15/06/2011 - Get also MIN(ResultDateTime), MAX(ResultDateTime) of the QC Results selected for the
        '''                              Test/SampleType and Control/Lot
        '''              SA 05/07/2011 - Changed the query to get also the SUMs of manual QC Results. Mean will be calculated in
        '''                              the function in the Delegate Class, calculation removed from the query 
        '''              SA 19/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        '''                              values (due to these values are not cumulated)
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetDataToCreateCumulateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                   ByVal pQCControlLotID As Integer, Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT R.AnalyzerID, R.RunsGroupNumber, COUNT(*) AS n, " & vbCrLf & _
                                                       " MIN(R.ResultDateTime) AS FirstRunDateTime, " & vbCrLf & _
                                                       " MAX(R.ResultDateTime) AS LastRunDateTime, " & vbCrLf & _
                                                       " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(R.ResultValue) ELSE SUM(R.ManualResultValue) END AS SumXi, " & vbCrLf & _
                                                       " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(POWER(R.ResultValue, 2)) ELSE SUM(POWER(R.ManualResultValue, 2)) END AS SumXi2, " & vbCrLf & _
                                                       " CASE WHEN (R.ManualResultFlag = 0) THEN POWER(SUM(R.ResultValue), 2) ELSE POWER(SUM(R.ManualResultValue), 2) END AS Sum2Xi " & vbCrLf & _
                                                " FROM  tqcResults R " & vbCrLf & _
                                                " WHERE R.QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
                                                " AND   R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
                                                " AND   R.ClosedResult   = 0 " & vbCrLf & _
                                                " AND   R.Excluded       = 0 " & vbCrLf & _
                                                " AND   R.IncludedInMean = 0 " & vbCrLf

                        If (pAnalyzerID.Trim <> String.Empty) Then cmdText &= " AND R.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf
                        cmdText &= " GROUP BY R.AnalyzerID, R.RunsGroupNumber, R.ManualResultFlag " & vbCrLf

                        Dim myQCResultsCalculationDS As New QCResultsCalculationDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myQCResultsCalculationDS.tQCResultCalculation)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myQCResultsCalculationDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetDataToCreateCumulate", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the more recent date of a non cumulated QC Result for the specified QCTestSampleID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing a datetime value with the maximum date</returns>
        ''' <remarks>
        ''' Created by:  SA 13/07/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetMaxResultDateTimeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(R.ResultDateTime) " & vbCrLf & _
                                                " FROM   tqcResults R " & vbCrLf & _
                                                " WHERE  R.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    R.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    R.ClosedResult   = 0 "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMaxResultDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/AnalyzerID, get the maximum Run Number of all its non cumulated QC Results (between all its 
        ''' linked Control/Lots)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the TestType/TestID/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing an integer value with the maximum Run Number between all non cumulated QC Results for
        '''          the specified QCTestSampleID/AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  SA 17/01/2012
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetMaxRunNumberByTestSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                    ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(RunNumber) AS MaxRunNumber FROM tqcResults " & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
                                                " AND    AnalyzerID   = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                        'Get the value and set it to the set of data to return
                        Dim myResult As Object
                        Dim maxRunNumber As Integer = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myResult = dbCmd.ExecuteScalar()
                            If (myResult Is DBNull.Value) Then myResult = 0

                            maxRunNumber = Convert.ToInt32(myResult)
                        End Using

                        myGlobalDataTO.SetDatos = maxRunNumber
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMaxRunNumberByTestSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the oldest date of a non cumulated QC Result for the specified QCTestSampleID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        ''' <remarks>
        ''' Created by:  TR 27/05/2011
        ''' Modified by: SA 01/06/2011 - Removed filter by Excluded=0; while not cumulated, all results have to be shown
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetMinResultDateTimeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MIN(R.ResultDateTime) " & vbCrLf & _
                                                " FROM   tqcResults R " & vbCrLf & _
                                                " WHERE  R.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    R.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    R.ClosedResult   = 0 "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMinResultDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all not cumulated QC Results for the specified QCTestSampleID/AnalyzerID and each QCControlLotID in the list, 
        ''' and optionally, in the informed interval of dates
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotIDList">List of identifiers of Controls/Lots in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Date From to filter results. Optional parameter</param>
        ''' <param name="pDateTo">Date To to filter results. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS with the list of not cumulated results
        '''          that fulfill the specified search criteria</returns>
        ''' <remarks>
        ''' Created by:  TR 01/06/2011
        ''' Modified by: SA 23/06/2011 - Date From/Date To parameters changed to optional 
        '''              SA 19/01/2012 - Changed the ORDER BY: sort data by ResultDateTime before RunNumber
        '''              SA 26/01/2012 - Changed the query to get also "ControlName (LotNumber)" AS ControlNameLotNum
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetNonCumulateResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotIDList As String, _
                                                 ByVal pAnalyzerID As String, Optional ByVal pDateFrom As DateTime = Nothing, _
                                                 Optional ByVal pDateTo As DateTime = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT R.*, HCL.ControlName, HCL.ControlName + ' (' + HCL.LotNumber + ')' AS ControlNameLotNum, " & vbCrLf & _
                                                      "  HCL.LotNumber, (MD.FixedItemDesc) AS MeasureUnit " & vbCrLf & _
                                                " FROM   tqcResults R INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                                    " INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                    " INNER JOIN tcfgMasterData MD ON MD.ItemID  = HTS.MeasureUnit " & vbCrLf & _
                                                " WHERE  R.QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                " AND    R.QCControlLotID IN (" & pQCControlLotIDList & ") " & vbCrLf & _
                                                " AND    R.AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    R.ClosedResult = 0 " & vbCrLf

                        'Add filters if optional parameters are informed
                        If (pDateFrom <> Nothing) Then cmdText &= " AND DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', R.ResultDateTime) >= 0 " & vbCrLf
                        If (pDateTo <> Nothing) Then cmdText &= " AND DATEDIFF(DAY, R.ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "')   >= 0 " & vbCrLf

                        'Sort Results by Control/Lot and RunNumber
                        cmdText &= " ORDER BY R.QCControlLotID, R.ResultDateTime, R.RunNumber " & vbCrLf

                        Dim myQCResultDS As New QCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myQCResultDS.tqcResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myQCResultDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetNonCumulateResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/QCControlLotID/AnalyzerID/RunsGroupNumber, get all QC Results to export to an
        ''' XML File (when a group of series are cumulated)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Identifier of the Group Number in QC Module</param> 
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS containing all QC Results to export to an
        '''          XML File (when a group of series are cumulated)</returns>
        ''' <remarks>
        ''' Created by:  DL 27/06/2011
        ''' Modified by: SA 27/06/2011 - Changed the SQL to get all QC Results, included the ones marked as Excluded; change the
        '''                              type of DS to return
        '''              SA 19/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        '''                              values (due to these values are not cumulated and not physically deleted)
        '''              SA 06/06/2012 - Added parameter for AnalyzerID and filter the query for this field
        ''' </remarks>
        Public Function GetQCResultsToExportNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tqcResults " & vbCrLf & _
                                                " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString & vbCrLf & _
                                                " AND    QCControlLotID  = " & pQCControlLotID.ToString & vbCrLf & _
                                                " AND    AnalyzerID      = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RunsGroupNumber = " & pRunsGroupNumber.ToString & vbCrLf & _
                                                " AND    IncludedInMean  = 0 " & vbCrLf

                        Dim myQCResultsDS As New QCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myQCResultsDS.tqcResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myQCResultsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetQCResultsToExport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/AnalyzerID, get all non cumulated QC Results in the informed range of dates for 
        ''' each one of the linked Control/Lots. When used for Test/Sample Type with CalculationMode = STATISTIC, then 
        ''' QC Results with IncludedInMean = TRUE are excluded from the query
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Date From to filter results</param>
        ''' <param name="pDateTo">Date To to filter results</param>
        ''' <param name="pForStatisticMode">When TRUE, it indicates that only QC Results having flag IncludedInMean=FALSE will be obtained</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OpenQCResultsDS with all Controls/Lots with not cumulated 
        '''          QC Results for the specified QCTestSampleID/AnalyzerID in the informed range of dates</returns>
        ''' <remarks>
        ''' Created by:  TR 31/05/2011
        ''' Modified by: SA 05/07/2011 - Changed the query to get also the SUMs of manual QC Results
        '''              SA 01/12/2011 - Changed function name from GetByQCTestSampleIDResultDateTime to GetResultsByControlLotForManualMode
        '''                              Removed fields CalculationMode and NumberOfSeries from the query
        '''                              Added optional parameter to get data only for the specified Control/Lot
        '''              SA 16/12/2011 - Changed function name from GetResultsByControlLotForManualMode to GetResultsByControlLot, due to now it
        '''                              is used for Manual and Statistics Calculation Mode, although for Statistic Mode, it will return only the
        '''                              not cumulated Results not included in the statistics calculation
        '''              SA 25/01/2012 - Changed the query to get also "ControlName (LotNumber)" AS ControlNameLotNum
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetResultsByControlLotNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String, _
                                                  ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime, ByVal pForStatisticMode As Boolean, _
                                                  Optional ByVal pQCControlLotID As Integer = -1, Optional ByVal pGetOnlyExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HTCL.WestgardControlNum, R.QCControlLotID, HCL.ControlName + ' (' + HCL.LotNumber + ')' AS ControlNameLotNum, " & vbCrLf & _
                                                       " HCL.ControlName, HCL.LotNumber, COUNT(R.ResultValue) AS n, " & vbCrLf & _
                                                       " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(R.ResultValue) ELSE SUM(R.ManualResultValue) END AS SumXi, " & vbCrLf & _
                                                       " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(POWER(R.ResultValue, 2)) ELSE SUM(POWER(R.ManualResultValue, 2)) END AS SumXi2, " & vbCrLf & _
                                                       " (MD.FixedItemDesc) AS MeasureUnit, R.RunsGroupNumber, HTS.RejectionCriteria, " & vbCrLf & _
                                                       " HTCL.MinConcentration AS MinRange, HTCL.MaxConcentration AS MaxRange, R.IncludedInMean " & vbCrLf & _
                                                " FROM tqcResults R INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                                  " INNER JOIN tqcHistoryTestControlLots HTCL ON R.QCTestSampleID = HTCL.QCTestSampleID AND R.QCControlLotID = HTCL.QCControlLotID " & vbCrLf & _
                                                                  " INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                  " INNER JOIN tcfgMasterData MD ON MD.ItemID  = HTS.MeasureUnit " & vbCrLf & _
                                                " WHERE  R.QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                " AND    R.AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', R.ResultDateTime) >= 0" & vbCrLf & _
                                                " AND    DATEDIFF(DAY, R.ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') >= 0" & vbCrLf & _
                                                " AND    R.ClosedResult = 0" & vbCrLf & _
                                                " AND    R.Excluded = " & IIf(Not pGetOnlyExcluded, 0, 1).ToString & vbCrLf

                        If (pForStatisticMode) Then cmdText &= " AND R.IncludedInMean = 0 " & vbCrLf
                        If (pQCControlLotID <> -1) Then cmdText &= " AND R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf

                        cmdText &= " GROUP BY HTCL.WestgardControlNum, R.QCControlLotID, HCL.ControlName, HCL.LotNumber, MD.FixedItemDesc, R.RunsGroupNumber, " & Environment.NewLine & _
                                            " HTS.RejectionCriteria, HTCL.MinConcentration, HTCL.MaxConcentration, R.ManualResultFlag, R.IncludedInMean " & Environment.NewLine & _
                                   " ORDER BY HCL.ControlName "

                        Dim myOpenQCResultDS As New OpenQCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOpenQCResultDS.tOpenResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myOpenQCResultDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetResultsByControlLot", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/AnalyzerID, get all QC Results not included in calculation of statistical values for each one of 
        ''' the linked Control/Lots (or for the specified one, when the optional parameter for the Control/Lot is informed)        
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS with all Controls/Lots with all included in Mean
        '''          QC Results for the specified QCTestSampleID/AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  SA 30/11/2011
        ''' Modified by: SA 25/01/2012 - Changed the query to get also "ControlName (LotNumber)" AS ControlNameLotNum
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetResultsByControlLotForStatisticsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                               ByVal pAnalyzerID As String, Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HTCL.WestgardControlNum, R.QCControlLotID, HCL.ControlName + ' (' + HCL.LotNumber + ')' AS ControlNameLotNum, " & vbCrLf & _
                                                       " HCL.ControlName, HCL.LotNumber, R.AnalyzerID, R.RunsGroupNumber, R.RunNumber, R.ResultDateTime, " & vbCrLf & _
                                                       " CASE WHEN (R.ManualResultFlag = 0) THEN R.ResultValue ELSE R.ManualResultValue END AS ResultValue, " & vbCrLf & _
                                                       " (MD.FixedItemDesc) AS MeasureUnit, HTS.RejectionCriteria, HTS.NumberOfSeries, R.IncludedInMean " & vbCrLf & _
                                                " FROM tqcResults R INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                                  " INNER JOIN tqcHistoryTestControlLots HTCL ON R.QCTestSampleID = HTCL.QCTestSampleID AND R.QCControlLotID = HTCL.QCControlLotID " & vbCrLf & _
                                                                  " INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                  " INNER JOIN tcfgMasterData MD ON MD.ItemID  = HTS.MeasureUnit " & vbCrLf & _
                                                " WHERE R.QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                " AND   R.AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND   R.ClosedResult = 0" & vbCrLf & _
                                                " AND   R.Excluded = 0 " & _
                                                " AND   R.IncludedInMean = 1 " & vbCrLf

                        If (pQCControlLotID <> -1) Then cmdText &= " AND R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf
                        cmdText &= " ORDER BY HCL.ControlName, R.ResultDateTime "

                        Dim myQCResultDS As New QCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myQCResultDS.tqcResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myQCResultDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetResultsByControlLotForStatistics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all open QC Results (of the specified Analyzer) for all Control/Lots linked to Test/SampleTypes with Calculation Mode 
        ''' defined as STATISTICS (the first NumberOfSeries results will be used to calculate the statistic values) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed Dataset QCResultsDS with the list of all QC Results (of the specified Analyzer) 
        '''          that will be used to calculate statistics for all Control/Lots linked to Test/SampleTypes with 
        '''          CalculationMode = STATISTIC</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetResultsForStatisticsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                   Optional ByVal pQCTestSampleID As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT R.*, HTS.NumberOfSeries " & vbCrLf & _
                                                " FROM   tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                " WHERE  R.AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    R.ClosedResult = 0 " & vbCrLf & _
                                                " AND    R.Excluded = 0 " & vbCrLf & _
                                                " AND    HTS.CalculationMode = 'STATISTIC' " & vbCrLf

                        If (pQCTestSampleID <> -1) Then cmdText &= " AND R.QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf
                        cmdText &= " ORDER BY R.QCTestSampleID, R.QCControlLotID, R.RunsGroupNumber, R.RunNumber "

                        Dim myQCResultDS As New QCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myQCResultDS.tqcResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myQCResultDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetResultsForStatistics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Max RunNumber for the QCTestSampleID, QCControlID and RunsGroupNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNum">Number of the currently opened Runs Group in which the Results are included</param>
        ''' <returns>GlobalDataTO containing an integer value with the last created RunNumber for the
        '''          specified QCTestSampleID, QCControlID and RunsGroupNumber</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' Modified by: SA 15/06/2012 - Added parameter for AnalyzerID and filter the query by this value
        ''' </remarks>
        Public Function GetMaxRunNumberNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                           ByVal pAnalyzerID As String, ByVal pRunsGroupNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(RunNumber) AS RunNumber " & vbCrLf & _
                                                " FROM   tqcResults " & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    QCControlLotID = " & pQCControlLotID & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    RunsGroupNumber = " & pRunsGroupNum

                        Dim myResult As Object
                        Dim myMaxRunNumber As Integer = 0

                        'Get the value and set it to the set data to return
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myResult = dbCmd.ExecuteScalar()
                            If (myResult Is DBNull.Value) Then myResult = 0

                            myMaxRunNumber = CInt(myResult)
                        End Using
                        myGlobalDataTO.SetDatos = myMaxRunNumber
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMaxRunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID, get the list of different Run Numbers of all its non cumulated QC Results (between all its 
        ''' linked Control/Lots)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the TestType/TestID/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS with the list of different Run Numbers of all its non cumulated 
        '''          QC Results for the specified TestType/TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 24/05/2012
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetRunNumberListByTestSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                     ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT RunNumber FROM tqcResults " & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " ORDER BY RunNumber " & vbCrLf

                        'Get the value and set it to the set of data to return
                        Dim runNumberListDS As New QCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(runNumberListDS.tqcResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = runNumberListDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetRunNumberListByTestSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set flag IncludedInMean = TRUE for all QC Results in the specified QCResultsDS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QC Results containing the group of QC Results that have to be marked 
        '''                            as included in Mean</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 04/06/2012 - Changed the query by adding a filter by field AnalyzerID
        ''' </remarks>
        Public Function MarkStatisticResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim cmdText As New StringBuilder()
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0
                    Dim maxUpdates As Integer = 500
                    For Each qcResultRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults
                        cmdText.Append(" UPDATE tqcResults SET IncludedInMean = 1 ")
                        cmdText.Append(vbCrLf)
                        cmdText.Append(" WHERE QCTestSampleID = ")
                        cmdText.AppendFormat("{0}", qcResultRow.QCTestSampleID)
                        cmdText.Append(vbCrLf)
                        cmdText.Append(" AND QCControlLotID = ")
                        cmdText.AppendFormat("{0}", qcResultRow.QCControlLotID)
                        cmdText.Append(vbCrLf)
                        cmdText.Append(" AND AnalyzerID = ")
                        cmdText.AppendFormat("N'{0}'", qcResultRow.AnalyzerID.Replace("'", "''").Trim)
                        cmdText.Append(vbCrLf)
                        cmdText.Append(" AND RunsGroupNumber = ")
                        cmdText.AppendFormat("{0}", qcResultRow.RunsGroupNumber)
                        cmdText.Append(vbCrLf)
                        cmdText.Append(" AND RunNumber = ")
                        cmdText.AppendFormat("{0}", qcResultRow.RunNumber)
                        cmdText.Append(vbCrLf)

                        i += 1
                        If (i = maxUpdates) Then
                            'Execute the SQL script 
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), pDBConnection)
                                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using

                            'Initialize the counter and the StringBuilder
                            i = 0
                            cmdText.Remove(0, cmdText.Length)
                        End If
                    Next

                    If (Not myGlobalDataTO.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining Updates...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), pDBConnection)
                                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.MarkStatisticResults ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        ''' informed QCTestSampleID/QCControlLotID/AnalyzerID/RunsGroupNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pNewRunsGroupNumber">Number of the open Runs Group  for the Test/SampleType and Control/Lot to which 
        '''                                   the QC Results have to be assigned</param>
        ''' <returns>GlobalDataTO containing success / error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field 
        ''' </remarks>
        Public Function MoveStatisticResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String, ByVal pNewRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcResults " & vbCrLf & _
                                            " SET    RunsGroupNumber = " & pNewRunsGroupNumber.ToString & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND    RunsGroupNumber = " & (pNewRunsGroupNumber - 1).ToString & vbCrLf & _
                                            " AND    IncludedInMean = 1 "

                    'Get the value and set it to the set of data to return
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.MoveStatisticResults ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Select all Tests/SampleTypes with not cumulated QC Results for the specified Control/Lot and Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing a typed DataSet OpenQCResultsDS with statistical QC data for all 
        '''          Tests/SampleTypes linked to the informed Control/Lot and Analyzer that have non cumulated QC Results</returns>
        ''' <remarks>
        ''' Created by:  SA 09/06/2011
        ''' Modified by: SA 05/07/2011 - Changed the query to get also the SUMs of manual QC Results 
        '''              SA 01/12/2011 - Changed the query to remove the SUMs of QC Results by Test/SampleType. Filter data by IncludedInMean=FALSE
        '''                              to get only Test/SampleType with enough Results to cumulate for the Test/SampleType
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field 
        '''                            - Get also field TestType from table tqcHistoryTestSamples
        ''' </remarks>
        Public Function ReadByQCControlLotIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, _
                                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT 1 AS Selected, R.QCTestSampleID, R.RunsGroupNumber, HTS.PreloadedTest, HTS.TestType, HTS.TestName, HTS.SampleType, " & vbCrLf & _
                                                       " HTS.DecimalsAllowed, HTS.RejectionCriteria, HTS.CalculationMode, HTS.NumberOfSeries, " & vbCrLf & _
                                                       " MD.FixedItemDesc AS MeasureUnit, COUNT(R.ResultValue) AS n, MIN(R.ResultDateTime) AS MinResultDateTime, " & vbCrLf & _
                                                       " MAX(R.ResultDateTime) AS MaxResultDateTime " & vbCrLf & _
                                                " FROM   tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                    " INNER JOIN tqcHistoryTestControlLots HTCL ON R.QCTestSampleID = HTCL.QCTestSampleID AND R.QCControlLotID = HTCL.QCControlLotID " & vbCrLf & _
                                                                    " INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID " & vbCrLf & _
                                                " WHERE  R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
                                                " AND    R.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    R.ClosedResult   = 0 " & vbCrLf & _
                                                " AND    R.Excluded       = 0 " & vbCrLf & _
                                                " AND    R.IncludedInMean = 0 " & vbCrLf & _
                                                " AND    MD.SubTableID    = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                                                " GROUP BY R.QCTestSampleID, R.RunsGroupNumber, HTS.PreloadedTest, HTS.TestType, HTS.TestName, HTS.SampleType, HTS.DecimalsAllowed, " & vbCrLf & _
                                                         " HTS.RejectionCriteria, HTS.CalculationMode, HTS.NumberOfSeries, MD.FixedItemDesc " & vbCrLf & _
                                                " ORDER BY HTS.TestType DESC, HTS.PreloadedTest DESC, HTS.TestName, HTS.SampleType " & vbCrLf

                        Dim myOpenQCResultDS As New OpenQCResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOpenQCResultDS.tOpenResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myOpenQCResultDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.ReadByQCControlLotID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there are QC Results pending to cumulate for the specified Control and Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pTestType">Test Type Code. Optional parameter</param>
        ''' <param name="pTestID">Test Identifier. Optional parameter</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Tests/SampleTypes
        '''          for which the informed Control has QC Results (of the informed Analyzer) pending to accumulate</returns>
        ''' <remarks>
        ''' Created by:  SA 16/05/2011
        ''' Modified by: SA 24/05/2011 - Added optional parameters to filter results also by TestID and SampleType
        '''              SA 20/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        '''                              values (due to these values are not cumulated)
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID; changed the query by adding a filter by this field when it is informed
        '''                            - Added optional parameter for TestType; changed the query by adding a filter by this field when it is informed
        ''' Modified by: RH 20/06/2012 - Solved bug: Incorrect syntax near 'SER'. Unclosed quotation mark after the character string ' '.
        ''' </remarks>
        Public Function SearchPendingResultsByControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                                         Optional ByVal pTestType As String = "", Optional ByVal pTestID As Integer = 0, _
                                                         Optional ByVal pSampleType As String = "", Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String

                        cmdText = " SELECT DISTINCT R.QCControlLotID, R.QCTestSampleID, HTS.TestType, HTS.TestID, HTS.SampleType, HTS.PreloadedTest, " & vbCrLf & _
                                  " HTS.TestName, HCL.ControlID, HCL.LotNumber, HCL.ControlName " & vbCrLf & _
                                  " FROM tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                  " INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                  " WHERE R.ClosedResult        = 0 " & vbCrLf & _
                                  " AND   R.Excluded            = 0 " & vbCrLf & _
                                  " AND   R.IncludedInMean      = 0 " & vbCrLf & _
                                  " AND   HCL.ControlID         = " & pControlID & vbCrLf & _
                                  " AND   HCL.ClosedLot         = 0 " & vbCrLf & _
                                  " AND   HCL.DeletedControl    = 0 " & vbCrLf & _
                                  " AND   HTS.DeletedSampleType = 0 " & vbCrLf & _
                                  " AND   HTS.DeletedTest       = 0 " & vbCrLf

                        If (pAnalyzerID.Trim <> String.Empty) Then cmdText &= " AND R.AnalyzerID = N '" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                        If (pTestType.Trim <> String.Empty AndAlso pTestID <> 0 AndAlso pSampleType.Trim <> String.Empty) Then
                            cmdText &= " AND HTS.TestType   = '" & pTestType.Trim & "' " & vbCrLf & _
                                       " AND HTS.TestID     = " & pTestID & vbCrLf & _
                                       " AND HTS.SampleType = '" & pSampleType.Trim & "' "
                        End If

                        Dim myHistoryQCInfoDS As New HistoryQCInformationDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryQCInfoDS.HistoryQCInfoTable)
                            End Using
                        End Using

                        resultData.SetDatos = myHistoryQCInfoDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.SearchPendingResultsByControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there are QC Results pending to cumulate for the specified TestType/TestID and optionally, Sample Type, for the 
        ''' informed Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Controls/Lots
        '''          for which the informed TestType/TestID/SampleType has QC Results pending to accumulate for the informed Analyzer</returns>
        ''' <remarks>
        ''' Created by:  TR 24/05/2011
        ''' Modified by: SA 20/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        '''                              values (due to these values are not cumulated). Get also field CalculationMode
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID; changed the query by adding a filter by this field when it is informed
        '''                            - Added parameter for TestType; changed the query by adding a filter by this field
        '''                            - Changed the SQL to get also field TestType from table tqcHistoryTestSamples
        ''' </remarks>
        Public Function SearchPendingResultsByTestIDSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, _
                                                                  ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                                                  Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT R.QCControlLotID, R.QCTestSampleID, R.AnalyzerID, HTS.TestType, HTS.TestID, HTS.SampleType, " & vbCrLf & _
                                                                " HTS.PreloadedTest, HTS.TestName, HTS.CalculationMode, HCL.ControlID, HCL.LotNumber, HCL.ControlName " & vbCrLf & _
                                                " FROM tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                  " INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                " WHERE R.ClosedResult   = 0 " & vbCrLf & _
                                                " AND   R.Excluded       = 0 " & vbCrLf & _
                                                " AND   R.IncludedInMean = 0 " & vbCrLf & _
                                                " AND   HTS.TestType     = '" & pTestType & "' " & vbCrLf & _
                                                " AND   HTS.TestID       = " & pTestID.ToString & vbCrLf

                        'Validate if AnalyzerID is informed to add it as filter
                        If (pAnalyzerID.Trim <> "") Then cmdText &= " AND R.AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                        'Validate if Sample Type is informed to add it as filter
                        If (pSampleType.Trim <> "") Then cmdText &= " AND HTS.SampleType= '" & pSampleType.Trim & "' " & vbCrLf

                        'Add the rest of query filters...
                        cmdText &= " AND HCL.ClosedLot         = 0 " & vbCrLf
                        cmdText &= " AND HCL.DeletedControl    = 0 " & vbCrLf
                        cmdText &= " AND HTS.DeletedSampleType = 0 " & vbCrLf
                        cmdText &= " AND HTS.DeletedTest       = 0 " & vbCrLf

                        Dim myHistoryQCInfoDS As New HistoryQCInformationDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryQCInfoDS.HistoryQCInfoTable)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHistoryQCInfoDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.SearchPendingResultsByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        ''' informed QCTestSampleID/AnalyzerID and optionally, a QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing the total number of QC Results marked as included in Mean for the specified QCTestSampleID/AnalyzerID 
        '''          and optionally, a QCControlLotID</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field 
        ''' </remarks>
        Public Function UnmarkStatisticResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                  ByVal pAnalyzerID As String, Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcResults " & vbCrLf & _
                                            " SET    IncludedInMean = 0 " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND    ClosedResult   = 0 " & vbCrLf & _
                                            " AND    IncludedInMean = 1 " & vbCrLf

                    If (pQCControlLotID <> -1) Then cmdText &= " AND QCControlLotID = " & pQCControlLotID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.UnmarkStatisticResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TEMPORARY - ONLY FOR QC RESULTS SIMULATOR"
        Public Function GetFirstDateTimeForResultsCreationNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pTestType As String, _
                                                              ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(R.ResultDateTime) AS FirstDateTime " & vbCrLf & _
                                                " FROM   tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                " WHERE  HTS.TestType   = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND    HTS.TestID     = " & pTestID.ToString & vbCrLf & _
                                                " AND    HTS.SampleType = '" & pSampleType & "' " & vbCrLf & _
                                                " AND    R.AnalyzerID   = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    R.ClosedResult = 0 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetFirstDateTimeForResultsCreation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' THIS METHOD IS FOR QC SIMULATOR ONLY (DO NOT IMPLEMENT IN OTHER APPLICATION AREAS).
        ''' For the specified Analyzer WorkSession get the maximum CtrlsSendingGroup created. If there is not any CtrlsSendingGroup
        ''' then it returns zero.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analizer ID.</param>
        ''' <returns>
        ''' GlobalDataTO containing an integer value with the maximum CtrlsSendingGroup in the specified
        ''' Analyzer WorkSession. If there is not any CtrlsSendingGroup, then it contains zero
        ''' </returns>
        ''' <remarks>
        ''' CREATED BY: TR 24/07/2012
        ''' </remarks>
        Public Function GetMaxCtrlsSendingGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX (CtrlsSendingGroup) AS NextCtrlsSendingGroup FROM tqcResults " & _
                                                " WHERE AnalyzerID = '" & pAnalyzerID & "'"

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    myGlobalDataTO.SetDatos = 0
                                Else
                                    myGlobalDataTO.SetDatos = CInt(dbDataReader.Item("NextCtrlsSendingGroup"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMaxCtrlsSendingGroup", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function



#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Insert new QC Results
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to add</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 17/05/2011
        '''' Modified by: SA 01/06/2011 - Replaced use of function ToSQLString in single values by function ReplaceNumericString to avoid loss of accuracy
        ''''                            - Added N prefix and single quote replacement in string values ResultComment and TS_User
        ''''                            - Boolean values are sent as 0 or 1 instead of False or True
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            Dim currentSession As New GlobalBase
        '            For Each qcResultsRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults.Rows
        '                cmdText &= " INSERT INTO tqcResults " & vbCrLf
        '                cmdText &= " (QCTestSampleID, QCControlLotID, RunsGroupNumber, RunNumber, ResultDateTime, " & vbCrLf
        '                cmdText &= "  ManualResultFlag, ResultValue, ManualResultValue, ResultComment, " & vbCrLf
        '                cmdText &= "  ValidationStatus, Excluded, ClosedResult, TS_User, TS_DateTime) " & vbCrLf
        '                cmdText &= " VALUES (" & vbCrLf
        '                cmdText &= qcResultsRow.QCTestSampleID & ", "
        '                cmdText &= qcResultsRow.QCControlLotID & ", "
        '                cmdText &= qcResultsRow.RunsGroupNumber & ", "
        '                cmdText &= qcResultsRow.RunNumber & ", "
        '                cmdText &= " '" & qcResultsRow.ResultDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
        '                cmdText &= Convert.ToInt32(IIf(qcResultsRow.ManualResultFlag, 1, 0)) & ", "

        '                If (qcResultsRow.IsResultValueNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= ReplaceNumericString(qcResultsRow.ResultValue) & ", "
        '                End If

        '                If (qcResultsRow.IsManualResultValueNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= ReplaceNumericString(qcResultsRow.ManualResultValue) & ", "
        '                End If

        '                If (qcResultsRow.IsResultCommentNull) Then
        '                    cmdText &= " NULL, "
        '                Else
        '                    cmdText &= " N'" & qcResultsRow.ResultComment.Replace("'", "''") & "', "
        '                End If

        '                cmdText &= " '" & qcResultsRow.ValidationStatus & "', "
        '                cmdText &= Convert.ToInt32(IIf(qcResultsRow.Excluded, 1, 0)) & ", "
        '                cmdText &= Convert.ToInt32(IIf(qcResultsRow.ClosedResult, 1, 0)) & ", "

        '                If (qcResultsRow.IsTS_UserNull) Then
        '                    cmdText &= " N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "', "
        '                Else
        '                    cmdText &= " N'" & qcResultsRow.TS_User.Replace("'", "''") & "', " & vbCrLf
        '                End If

        '                If (qcResultsRow.IsTS_DateTimeNull) Then
        '                    cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
        '                Else
        '                    cmdText &= " '" & qcResultsRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
        '                End If
        '                cmdText &= vbCrLf
        '            Next

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.Create ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete a group of QC Results
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to delete</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 10/06/2011
        '''' </remarks>
        'Public Function DeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            Dim currentSession As New GlobalBase
        '            For Each qcResultsRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults.Rows
        '                cmdText &= " DELETE FROM tqcResults " & Environment.NewLine
        '                cmdText &= " WHERE  QCTestSampleID = " & qcResultsRow.QCTestSampleID & Environment.NewLine
        '                cmdText &= " AND    QCControlLotID = " & qcResultsRow.QCControlLotID & Environment.NewLine
        '                cmdText &= " AND    RunsGroupNumber = " & qcResultsRow.RunsGroupNumber & Environment.NewLine
        '                cmdText &= " AND    RunNumber = " & qcResultsRow.RunNumber & Environment.NewLine
        '                cmdText &= Environment.NewLine
        '            Next

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.Delete ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCTestSampleID and QCControlLotID, delete the QC Results included in the RunsGroup for 
        '''' the informed Cumulated Serie
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pCumResultsNum">Number of the Cumulated Serie to be deleted for the QCTestSampleID and QCControlLotID</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 21/06/2011 
        '''' </remarks>
        'Public Function DeleteByCumResultsNumOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                         ByVal pCumResultsNum As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " DELETE tqcResults " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                    " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
        '                                    " AND    RunsGroupNumber IN (SELECT RunsGroup FROM tqcRunsGroups " & vbCrLf & _
        '                                                               " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                                               " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
        '                                                               " AND    CumResultsNum  = " & pCumResultsNum.ToString & ") "

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.DeleteByCumResultsNum", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCTestSampleID and QCControlLotID, delete all the QC Results included in the informed RunsGroup
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNum">Number of the Runs Group in which the QC Results are included</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 27/06/2011 
        '''' Modified by: SA 19/12/2011 - Added filter by field IncludedInMean=FALSE, due to QC Results used to calculate statistic values 
        ''''                              are not physically deleted when the RunsGroup is cumulated
        '''' </remarks>
        'Public Function DeleteByRunsGroupNumOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                        ByVal pRunsGroupNum As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " DELETE tqcResults " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                    " AND    QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
        '                                    " AND    RunsGroupNumber = " & pRunsGroupNum.ToString & vbCrLf & _
        '                                    " AND    IncludedInMean  = 0 " & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.DeleteByRunsGroupNum", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Update fields ManualResultValue, ResultComment and/or Excluded flag for an specific QC Result
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing data of the QC Result to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 08/06/2011
        '''' </remarks>
        'Public Function UpdateManualResultOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

        '        ElseIf (pQCResultsDS.tqcResults.Count > 0) Then
        '            Dim currentSession As New GlobalBase
        '            Dim cmdText As String = " UPDATE tqcResults SET "

        '            If (Not pQCResultsDS.tqcResults(0).IsResultCommentNull) Then
        '                cmdText &= " ResultComment = N'" & pQCResultsDS.tqcResults(0).ResultComment.Replace("'", "''") & "' "
        '            End If

        '            If (Not pQCResultsDS.tqcResults(0).IsExcludedNull) Then
        '                If cmdText.Length > 0 Then
        '                    cmdText &= ","
        '                End If
        '                cmdText &= " Excluded ='" & pQCResultsDS.tqcResults(0).Excluded & "'"
        '            End If

        '            If (Not pQCResultsDS.tqcResults(0).IsManualResultValueNull) Then
        '                If cmdText.Length > 0 Then
        '                    cmdText &= ","
        '                End If
        '                cmdText &= " ManualResultValue =" & ReplaceNumericString(pQCResultsDS.tqcResults(0).ManualResultValue) & ","
        '                cmdText &= " ManualResultFlag = 1 "
        '            End If

        '            cmdText &= " WHERE QCTestSampleID =" & pQCResultsDS.tqcResults(0).QCTestSampleID
        '            cmdText &= " AND   QCControlLotID =" & pQCResultsDS.tqcResults(0).QCControlLotID
        '            cmdText &= " AND   RunsGroupNumber = " & pQCResultsDS.tqcResults(0).RunsGroupNumber
        '            cmdText &= " AND   RunNumber = " & pQCResultsDS.tqcResults(0).RunNumber

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.UpdateValStatusByResult ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Set ValidationStatus=OK for all results of the informed QCControlLotID/QCTestSampleID/RunsGroupNumber that are inside
        '''' the specified range of dates 
        '''' OR 
        '''' Set ValidationStatus=PENDING for all results of the informed QCControlLotID/QCTestSampleID/RunsGroupNumber that are 
        '''' outside the specified range of dates 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        '''' <param name="pInDateRange">Flag indicating which of the two Updates have to be executed: when TRUE, the function set 
        ''''                            ValidationStatus=OK for all results inside the range of dates; when FALSE, the function set
        ''''                            ValidationStatus=PENDING for all results outside the range of dates</param>
        '''' <param name="pDateFrom">Date From to search results</param>
        '''' <param name="pDateTo">Date To to search results</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 06/06/2011
        '''' Modified by: SA 27/06/2011 - Changed filter by dates when pInDateRange=False; it does not work
        '''' </remarks>
        'Public Function UpdateValStatusByDateRangeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, ByVal pQCTestSampleID As Integer, _
        '                                              ByVal pRunsGroupNumber As Integer, ByVal pInDateRange As Boolean, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) _
        '                                              As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            If (pInDateRange) Then
        '                cmdText = " UPDATE tqcResults SET ValidationStatus = 'OK' " & vbCrLf & _
        '                          " WHERE  QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
        '                          " AND    QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                          " AND    RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf & _
        '                          " AND    DATEDIFF(day, '" & pDateFrom.ToString("yyyyMMdd") & "', ResultDateTime) > = 0 " & vbCrLf & _
        '                          " AND    DATEDIFF(day, ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') > = 0 " & vbCrLf
        '            Else
        '                cmdText = " UPDATE tqcResults SET ValidationStatus = 'PENDING' " & vbCrLf & _
        '                          " WHERE  QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
        '                          " AND    QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                          " AND    RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf & _
        '                          " AND    (DATEDIFF(day, '" & pDateFrom.ToString("yyyyMMdd") & "', ResultDateTime) < 0 " & vbCrLf & _
        '                          " OR      DATEDIFF(day, ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') < 0) " & vbCrLf
        '            End If

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.UpdateValStatusByDateRange ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Update the ValidationStatus with the specified value for a QC Result of the informed QCControlLotID/QCTestSampleID/RunsGroupNumber 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        '''' <param name="pRunNumber">Number of the execution that identify the specific Result which ValidationStatus has to be updated</param>
        '''' <param name="pValStatus">Value to assign as Validation Status of the Results</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 06/06/2011
        '''' </remarks>
        'Public Function UpdateValStatusByResultOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, ByVal pQCTestSampleID As Integer, _
        '                                           ByVal pRunsGroupNumber As Integer, ByVal pValStatus As String, ByVal pRunNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tqcResults " & vbCrLf & _
        '                                    " SET    ValidationStatus = '" & pValStatus & "' " & vbCrLf & _
        '                                    " WHERE  QCControlLotID   = " & pQCControlLotID.ToString() & vbCrLf & _
        '                                    " AND    QCTestSampleID   = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                    " AND    RunsGroupNumber  = " & pRunsGroupNumber.ToString() & vbCrLf & _
        '                                    " AND    RunNumber        = " & pRunNumber

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.UpdateValStatusByResult ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the number of non cumulated and not excluded QC Results for the specified QCTestSampleID and QCControlLotID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing an integer value with the number of non cumulated and not excluded QC Results
        ''''          for the specified QCTestSampleID and QCControlLotID</returns>
        '''' <remarks>
        '''' Created by:  SA 20/06/2011
        '''' Modified by: SA 20/12/2011 - Added filter by IncludedInMean=FALSE to avoid counting QC Results used to calculate 
        ''''                              statistic values when the specified Test/SampleType has STATISTIC Calculation Mode  
        '''' </remarks>
        'Public Function CountNonCumulatedResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                         ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT COUNT(*) FROM tqcResults " & vbCrLf & _
        '                                        " WHERE  QCControlLotID = " & pQCControlLotID & vbCrLf & _
        '                                        " AND    QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    ClosedResult   = 0 " & vbCrLf & _
        '                                        " AND    Excluded       = 0 " & vbCrLf & _
        '                                        " AND    IncludedInMean = 0 "

        '                'Get the value and set it to the set of data to return
        '                Dim nonCumulatedResults As Integer = 0
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    nonCumulatedResults = Convert.ToInt32(dbCmd.ExecuteScalar())
        '                End Using

        '                myGlobalDataTO.SetDatos = nonCumulatedResults
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.CountNonCumulatedResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCControlLotID/QCTestSampleID/RunsGroupNumber, count the number of non cumulated Results pending to review
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        '''' <returns>GlobalDataTO containing an integer value with the number of Results pending to review</returns>
        '''' <remarks>
        '''' Created by:  SA 06/06/2011
        '''' Modified by: SA 20/12/2011 - Added filter by IncludedInMean=FALSE to avoid counting QC Results that will not be cumulated
        ''''                              (those that are used to calculate statistic values for Test/SampleTypes having STATISTIC
        ''''                              Calculation Mode) 
        '''' </remarks>
        'Public Function CountNotReviewedResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, _
        '                                           ByVal pQCTestSampleID As Integer, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT COUNT(*) AS ReviewPending FROM tqcResults " & vbCrLf & _
        '                                        " WHERE  QCControlLotID   = " & pQCControlLotID & vbCrLf & _
        '                                        " AND    QCTestSampleID   = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    RunsGroupNumber  = " & pRunsGroupNumber & vbCrLf & _
        '                                        " AND    ValidationStatus = 'PENDING' " & vbCrLf & _
        '                                        " AND    ClosedResult     = 0 " & vbCrLf & _
        '                                        " AND    Excluded         = 0 " & vbCrLf & _
        '                                        " AND    IncludedInMean   = 0 "

        '                'Get the value and set it to the set of data to return
        '                Dim pendingResults As Integer = 0
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    pendingResults = Convert.ToInt32(dbCmd.ExecuteScalar())
        '                End Using

        '                myGlobalDataTO.SetDatos = pendingResults
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.CountNotReviewedResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCControlLotID/QCTestSampleID/RunsGroupNumber, count the number of non cumulated Results having alarms
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pRunsGroupNumber">Number of the currently opened Runs Group in which the Results are included</param>
        '''' <returns>GlobalDataTO containing an integer value with the number of Results with alarms</returns>
        '''' <remarks>
        '''' Created by:  SA 06/06/2011
        '''' Modified by: SA 20/12/2011 - Added filter by IncludedInMean=FALSE to avoid counting QC Results that will not be cumulated
        ''''                              (those that are used to calculate statistic values for Test/SampleTypes having STATISTIC
        ''''                              Calculation Mode) 
        '''' </remarks>
        'Public Function CountResultsWithAlarmsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, _
        '                                         ByVal pQCTestSampleID As Integer, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT COUNT(*) AS WithAlarms FROM tqcResults " & vbCrLf & _
        '                                        " WHERE  QCControlLotID  = " & pQCControlLotID & vbCrLf & _
        '                                        " AND    QCTestSampleID  = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    RunsGroupNumber = " & pRunsGroupNumber & vbCrLf & _
        '                                        " AND    ValidationStatus IN ('WARNING', 'ERROR') " & vbCrLf & _
        '                                        " AND    ClosedResult = 0 " & vbCrLf & _
        '                                        " AND    Excluded     = 0 " & vbCrLf

        '                'Get the value and set it to the set of data to return
        '                Dim resultWithAlarms As Integer = 0
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    resultWithAlarms = Convert.ToInt32(dbCmd.ExecuteScalar())
        '                End Using

        '                myGlobalDataTO.SetDatos = resultWithAlarms
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.CountResultsWithAlarms", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete all QC Results marked as included in calculation of statistical values for the 
        '''' informed Test/Sample Type, Control/Lot and Runs Group Number
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNumber">Optional parameter. Number of the open Runs Group for the Test/SampleType and Control/Lot</param>
        '''' <returns>GlobalDataTO containing success/error infomration </returns>
        '''' <remarks>
        '''' Created by:  SA 15/12/2011
        '''' Modified by: SA 21/12/2011 - Parameter for the RunsGroupNumber changed to optional
        '''' </remarks>
        'Public Function DeleteStatisticResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                          ByVal pQCControlLotID As Integer, Optional ByVal pRunsGroupNumber As Integer = 0) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " DELETE FROM tqcResults " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                    " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
        '                                    " AND    IncludedInMean = 1 "

        '            'Add the filter by RunsGroupNumber when the parameter is informed
        '            If (pRunsGroupNumber <> 0) Then cmdText &= " AND RunsGroupNumber = " & pRunsGroupNumber.ToString & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.DeleteStatisticResults ", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Search in history table of QC Module all Controls/Lots having ClosedLot = FALSE and DeleteControl=FALSE and having for at least 
        '''' a Test/SampleType, enough QC Results pending to accumulate
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of all Controls/Lots in 
        ''''          the history table in QC Module that have QC Results thar can be cumulated </returns>
        '''' <remarks>
        '''' Created by:  SA 01/07/2011
        '''' Modified by: SA 19/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        ''''                              values (due to these values are not cumulated)
        ''''              SA 02/02/2012 - Added a filter by field Excluded=FALSE to avoid including as data to cumulate QC Results that
        ''''                              have been excluded  
        '''' </remarks>
        'Public Function GetControlsToCumulateOLD(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT   DISTINCT R.QCControlLotID, HCL.ControlName, HCL.LotNumber " & vbCrLf & _
        '                                        " FROM     tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                              " INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                        " WHERE    R.ClosedResult        = 0 " & vbCrLf & _
        '                                        " AND      R.Excluded            = 0 " & vbCrLf & _
        '                                        " AND      R.IncludedInMean      = 0 " & vbCrLf & _
        '                                        " AND      HTS.DeletedSampleType = 0 " & vbCrLf & _
        '                                        " AND      HTS.DeletedTest       = 0 " & vbCrLf & _
        '                                        " AND      HCL.ClosedLot         = 0 " & vbCrLf & _
        '                                        " AND      HCL.DeletedControl    = 0 " & vbCrLf & _
        '                                        " GROUP BY R.QCControlLotID, HCL.ControlName, HCL.LotNumber " & vbCrLf & _
        '                                        " ORDER BY HCL.ControlName "

        '                Dim myHistoryControlLotsDS As New HistoryControlLotsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myHistoryControlLotsDS.tqcHistoryControlLots)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myHistoryControlLotsDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetControlsToCumulate", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all not excluded QC Results for the specified Test/SampleType and Control/Lot and calculate all data needed to 
        '''' create a new cumulated serie
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultsCalculationDS with the needed data</returns>
        '''' <remarks>
        '''' Created by:  TR 19/05/2011
        '''' Modified by: SA 15/06/2011 - Get also MIN(ResultDateTime), MAX(ResultDateTime) of the QC Results selected for the
        ''''                              Test/SampleType and Control/Lot
        ''''              SA 05/07/2011 - Changed the query to get also the SUMs of manual QC Results. Mean will be calculated in
        ''''                              the function in the Delegate Class, calculation removed from the query 
        ''''              SA 19/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        ''''                              values (due to these values are not cumulated)
        '''' </remarks>
        'Public Function GetDataToCreateCumulateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                           ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT R.RunsGroupNumber, COUNT(*) AS n, " & vbCrLf & _
        '                                               " MIN(R.ResultDateTime) AS FirstRunDateTime, " & vbCrLf & _
        '                                               " MAX(R.ResultDateTime) AS LastRunDateTime, " & vbCrLf & _
        '                                               " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(R.ResultValue) ELSE SUM(R.ManualResultValue) END AS SumXi, " & vbCrLf & _
        '                                               " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(POWER(R.ResultValue, 2)) ELSE SUM(POWER(R.ManualResultValue, 2)) END AS SumXi2, " & vbCrLf & _
        '                                               " CASE WHEN (R.ManualResultFlag = 0) THEN POWER(SUM(R.ResultValue), 2) ELSE POWER(SUM(R.ManualResultValue), 2) END AS Sum2Xi " & vbCrLf & _
        '                                        " FROM  tqcResults R " & vbCrLf & _
        '                                        " WHERE R.QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
        '                                        " AND   R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
        '                                        " AND   R.ClosedResult   = 0 " & vbCrLf & _
        '                                        " AND   R.Excluded       = 0 " & vbCrLf & _
        '                                        " AND   R.IncludedInMean = 0 " & vbCrLf & _
        '                                        " GROUP BY R.RunsGroupNumber, R.ManualResultFlag " & vbCrLf

        '                Dim myQCResultsCalculationDS As New QCResultsCalculationDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myQCResultsCalculationDS.tQCResultCalculation)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myQCResultsCalculationDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetDataToCreateCumulate", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the more recent date of a non cumulated QC Result for the specified QCTestSampleID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <returns>GlobalDataTO containing a datetime value with the maximum date</returns>
        '''' <remarks>
        '''' Created by:  SA 13/07/2011
        '''' </remarks>
        'Public Function GetMaxResultDateTimeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT MAX(R.ResultDateTime) " & vbCrLf & _
        '                                        " FROM   tqcResults R " & vbCrLf & _
        '                                        " WHERE  R.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    R.ClosedResult = 0 "

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
        '                    myGlobalDataTO.HasError = False
        '                End Using
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMaxResultDateTime", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the Max RunNumber for the QCTestSampleID, QCControlID and RunsGroupNumber
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNum">Number of the currently opened Runs Group in which the Results are included</param>
        '''' <returns>GlobalDataTO containing an integer value with the last created RunNumber for the
        ''''          specified QCTestSampleID, QCControlID and RunsGroupNumber</returns>
        '''' <remarks>
        '''' Created by: TR 17/05/2011
        '''' </remarks>
        'Public Function GetMaxRunNumberOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                   ByVal pRunsGroupNum As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT MAX(RunNumber) AS RunNumber " & Environment.NewLine & _
        '                                        " FROM   tqcResults " & Environment.NewLine & _
        '                                        " WHERE  QCTestSampleID = " & pQCTestSampleID & Environment.NewLine & _
        '                                        " AND    QCControlLotID = " & pQCControlLotID & Environment.NewLine & _
        '                                        " AND    RunsGroupNumber = " & pRunsGroupNum

        '                Dim myResult As Object
        '                Dim myMaxRunNumber As Integer = 0

        '                'Get the value and set it to the set data to return
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    myResult = dbCmd.ExecuteScalar()
        '                    If (myResult Is DBNull.Value) Then myResult = 0

        '                    myMaxRunNumber = CInt(myResult)
        '                End Using
        '                myGlobalDataTO.SetDatos = myMaxRunNumber
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMaxRunNumber", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the oldest date of a non cumulated QC Result for the specified QCTestSampleID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        '''' <remarks>
        '''' Created by:  TR 27/05/2011
        '''' Modified by: SA 01/06/2011 - Removed filter by Excluded=0; while not cumulated, all results have to be shown
        '''' </remarks>
        'Public Function GetMinResultDateTimeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT MIN(R.ResultDateTime) " & vbCrLf & _
        '                                        " FROM   tqcResults R " & vbCrLf & _
        '                                        " WHERE  R.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    R.ClosedResult = 0 "

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
        '                End Using
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetMinResultDateTime", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all not cumulated QC Results for the specified QCTestSampleID and each QCControlLotID in the list, 
        '''' and optionally, in the informed interval of dates
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotIDList">List of identifiers of Controls/Lots in QC Module</param>
        '''' <param name="pDateFrom">Date From to filter results. Optional parameter</param>
        '''' <param name="pDateTo">Date To to filter results. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS with the list of not cumulated results
        ''''          that fulfill the specified search criteria</returns>
        '''' <remarks>
        '''' Created by:  TR 01/06/2011
        '''' Modified by: SA 23/06/2011 - Date From/Date To parameters changed to optional 
        ''''              SA 19/01/2012 - Changed the ORDER BY: sort data by ResultDateTime before RunNumber
        ''''              SA 26/01/2012 - Changed the query to get also "ControlName (LotNumber)" AS ControlNameLotNum
        '''' </remarks>
        'Public Function GetNonCumulateResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotIDList As String, _
        '                                         Optional ByVal pDateFrom As DateTime = Nothing, Optional ByVal pDateTo As DateTime = Nothing) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT R.*, HCL.ControlName, HCL.ControlName + ' (' + HCL.LotNumber + ')' AS ControlNameLotNum, " & vbCrLf & _
        '                                              "  HCL.LotNumber, (MD.FixedItemDesc) AS MeasureUnit " & vbCrLf & _
        '                                        " FROM   tqcResults R INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                                            " INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                            " INNER JOIN tcfgMasterData MD ON MD.ItemID  = HTS.MeasureUnit " & vbCrLf & _
        '                                        " WHERE  R.QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                        " AND    R.QCControlLotID IN (" & pQCControlLotIDList & ") " & vbCrLf & _
        '                                        " AND    R.ClosedResult = 0 " & vbCrLf

        '                'Add filters if optional parameters are informed
        '                If (pDateFrom <> Nothing) Then cmdText &= " AND DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', R.ResultDateTime) >= 0 " & vbCrLf
        '                If (pDateTo <> Nothing) Then cmdText &= " AND DATEDIFF(DAY, R.ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "')   >= 0 " & vbCrLf

        '                'Sort Results by Control/Lot and RunNumber
        '                cmdText &= " ORDER BY R.QCControlLotID, R.ResultDateTime, R.RunNumber " & vbCrLf

        '                Dim myQCResultDS As New QCResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myQCResultDS.tqcResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myQCResultDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetNonCumulateResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all QC Results for the specified Test/SampleType, Control/Lot and RunsGroup
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNumber">Number of the Runs Group in which the QC Results are included</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS with all QC Results for the specified Test/SampleType, 
        ''''          Control/Lot and RunsGroup</returns>
        '''' <remarks>
        '''' Created by:  DL 27/06/2011
        '''' Modified by: SA 27/06/2011 - Changed the SQL to get all QC Results, included the ones marked as Excluded; change the
        ''''                              type of DS to return
        ''''              SA 19/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        ''''                              values (due to these values are not cumulated and not physically deleted)
        '''' </remarks>
        'Public Function GetQCResultsToExportOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                        ByVal pQCControlLotID As Integer, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * FROM tqcResults " & vbCrLf & _
        '                                        " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString & vbCrLf & _
        '                                        " AND    QCControlLotID  = " & pQCControlLotID.ToString & vbCrLf & _
        '                                        " AND    RunsGroupNumber = " & pRunsGroupNumber.ToString & vbCrLf & _
        '                                        " AND    IncludedInMean  = 0 " & vbCrLf

        '                Dim myQCResultsDS As New QCResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myQCResultsDS.tqcResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myQCResultsDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetQCResultsToExport", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified Test/SampleType, get all non cumulated QC Results in the informed range of dates for 
        '''' each one of the linked Control/Lots. When used for Test/Sample Type with CalculationMode = STATISTIC, then 
        '''' QC Results with IncludedInMean = TRUE are excluded from the query
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pDateFrom">Date From to filter results</param>
        '''' <param name="pDateTo">Date To to filter results</param>
        '''' <param name="pForStatisticMode">When TRUE, it indicates that only QC Results having flag IncludedInMean=FALSE will be obtained</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet OpenQCResultsDS with all Controls/Lots with not cumulated 
        ''''          QC Results for the specified Test/SampleType in the informed range of dates</returns>
        '''' <remarks>
        '''' Created by:  TR 31/05/2011
        '''' Modified by: SA 05/07/2011 - Changed the query to get also the SUMs of manual QC Results
        ''''              SA 01/12/2011 - Changed function name from GetByQCTestSampleIDResultDateTime to GetResultsByControlLotForManualMode
        ''''                              Removed fields CalculationMode and NumberOfSeries from the query
        ''''                              Added optional parameter to get data only for the specified Control/Lot
        ''''              SA 16/12/2011 - Changed function name from GetResultsByControlLotForManualMode to GetResultsByControlLot, due to now it
        ''''                              is used for Manual and Statistics Calculation Mode, although for Statistic Mode, it will return only the
        ''''                              not cumulated Results not included in the statistics calculation
        ''''              SA 25/01/2012 - Changed the query to get also "ControlName (LotNumber)" AS ControlNameLotNum
        '''' </remarks>
        'Public Function GetResultsByControlLotOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pDateFrom As DateTime, _
        '                                          ByVal pDateTo As DateTime, ByVal pForStatisticMode As Boolean, Optional ByVal pQCControlLotID As Integer = -1, _
        '                                          Optional ByVal pGetOnlyExcluded As Boolean = False) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT HTCL.WestgardControlNum, R.QCControlLotID, HCL.ControlName + ' (' + HCL.LotNumber + ')' AS ControlNameLotNum, " & vbCrLf & _
        '                                               " HCL.ControlName, HCL.LotNumber, COUNT(R.ResultValue) AS n, " & vbCrLf & _
        '                                               " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(R.ResultValue) ELSE SUM(R.ManualResultValue) END AS SumXi, " & vbCrLf & _
        '                                               " CASE WHEN (R.ManualResultFlag = 0) THEN SUM(POWER(R.ResultValue, 2)) ELSE SUM(POWER(R.ManualResultValue, 2)) END AS SumXi2, " & vbCrLf & _
        '                                               " (MD.FixedItemDesc) AS MeasureUnit, R.RunsGroupNumber, HTS.RejectionCriteria, " & vbCrLf & _
        '                                               " HTCL.MinConcentration AS MinRange, HTCL.MaxConcentration AS MaxRange, R.IncludedInMean " & vbCrLf & _
        '                                        " FROM tqcResults R INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                                          " INNER JOIN tqcHistoryTestControlLots HTCL ON R.QCTestSampleID = HTCL.QCTestSampleID AND R.QCControlLotID = HTCL.QCControlLotID " & vbCrLf & _
        '                                                          " INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                          " INNER JOIN tcfgMasterData MD ON MD.ItemID  = HTS.MeasureUnit " & vbCrLf & _
        '                                        " WHERE  R.QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                        " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', R.ResultDateTime) >= 0" & vbCrLf & _
        '                                        " AND    DATEDIFF(DAY, R.ResultDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') >= 0" & vbCrLf & _
        '                                        " AND    R.ClosedResult = 0" & vbCrLf & _
        '                                        " AND    R.Excluded = " & IIf(Not pGetOnlyExcluded, 0, 1).ToString & vbCrLf

        '                If (pForStatisticMode) Then cmdText &= " AND R.IncludedInMean = 0 " & vbCrLf
        '                If (pQCControlLotID <> -1) Then cmdText &= " AND R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf

        '                cmdText &= " GROUP BY HTCL.WestgardControlNum, R.QCControlLotID, HCL.ControlName, HCL.LotNumber, MD.FixedItemDesc, R.RunsGroupNumber, " & Environment.NewLine & _
        '                                    " HTS.RejectionCriteria, HTCL.MinConcentration, HTCL.MaxConcentration, R.ManualResultFlag, R.IncludedInMean " & Environment.NewLine & _
        '                           " ORDER BY HCL.ControlName "

        '                Dim myOpenQCResultDS As New OpenQCResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myOpenQCResultDS.tOpenResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myOpenQCResultDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetResultsByControlLot", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified Test/SampleType, get all QC Results not included in calculation of statistical values for each one of 
        '''' the linked Control/Lots (or for the specified one, when the optional parameter for the Control/Lot is informed)        
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS with all Controls/Lots with all included in Mean
        ''''          QC Results for the specified Test/SampleType</returns>
        '''' <remarks>
        '''' Created by:  SA 30/11/2011
        '''' Modified by: SA 25/01/2012 - Changed the query to get also "ControlName (LotNumber)" AS ControlNameLotNum
        '''' </remarks>
        'Public Function GetResultsByControlLotForStatisticsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                                       Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT HTCL.WestgardControlNum, R.QCControlLotID, HCL.ControlName + ' (' + HCL.LotNumber + ')' AS ControlNameLotNum, " & vbCrLf & _
        '                                               " HCL.ControlName, HCL.LotNumber, R.RunsGroupNumber, R.RunNumber, R.ResultDateTime, " & vbCrLf & _
        '                                               " CASE WHEN (R.ManualResultFlag = 0) THEN R.ResultValue ELSE R.ManualResultValue END AS ResultValue, " & vbCrLf & _
        '                                               " (MD.FixedItemDesc) AS MeasureUnit, HTS.RejectionCriteria, HTS.NumberOfSeries, R.IncludedInMean " & vbCrLf & _
        '                                        " FROM tqcResults R INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                                          " INNER JOIN tqcHistoryTestControlLots HTCL ON R.QCTestSampleID = HTCL.QCTestSampleID AND R.QCControlLotID = HTCL.QCControlLotID " & vbCrLf & _
        '                                                          " INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                          " INNER JOIN tcfgMasterData MD ON MD.ItemID  = HTS.MeasureUnit " & vbCrLf & _
        '                                        " WHERE R.QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                        " AND   R.ClosedResult = 0" & vbCrLf & _
        '                                        " AND   R.Excluded = 0 " & _
        '                                        " AND   R.IncludedInMean = 1 " & vbCrLf

        '                If (pQCControlLotID <> -1) Then cmdText &= " AND R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf
        '                cmdText &= " ORDER BY HCL.ControlName, R.ResultDateTime "

        '                Dim myQCResultDS As New QCResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myQCResultDS.tqcResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myQCResultDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetResultsByControlLotForStatistics", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all open QC Results for all Control/Lots linked to Test/SampleTypes with Calculation Mode defined as STATISTICS
        '''' (the first NumberOfSeries results will be used to calculate the statistic values)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <returns>GlobalDataTO containing a typed Dataset QCResultsDS with the list of all QC Results that will
        ''''          be used to calculate statistics for all Control/Lots linked to Test/SampleTypes with 
        ''''          CalculationMode = STATISTIC</returns>
        '''' <remarks>
        '''' Created by: SA 15/12/2011
        '''' </remarks>
        'Public Function GetResultsForStatisticsOLD(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pQCTestSampleID As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT R.*, HTS.NumberOfSeries " & vbCrLf & _
        '                                        " FROM   tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                        " WHERE  R.ClosedResult = 0 " & vbCrLf & _
        '                                        " AND    R.Excluded = 0 " & vbCrLf & _
        '                                        " AND    HTS.CalculationMode = 'STATISTIC' " & vbCrLf

        '                If (pQCTestSampleID <> -1) Then cmdText &= " AND R.QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf
        '                cmdText &= " ORDER BY R.QCTestSampleID, R.QCControlLotID, R.RunsGroupNumber, R.RunNumber "

        '                Dim myQCResultDS As New QCResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myQCResultDS.tqcResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myQCResultDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetResultsForStatistics", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Set flag IncludedInMean = TRUE for all QC Results in the specified QCResultsDS
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultsDS">Typed DataSet QC Results containing the group of QC Results that have to be marked 
        ''''                            as included in Mean</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: SA 15/12/2011
        '''' </remarks>
        'Public Function MarkStatisticResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
        '    Dim cmdText As New StringBuilder()
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim i As Integer = 0
        '            Dim maxUpdates As Integer = 500
        '            For Each qcResultRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults
        '                cmdText.Append(" UPDATE tqcResults SET IncludedInMean = 1 ")
        '                cmdText.Append(vbCrLf)
        '                cmdText.Append(" WHERE QCTestSampleID = ")
        '                cmdText.AppendFormat("{0}", qcResultRow.QCTestSampleID)
        '                cmdText.Append(vbCrLf)
        '                cmdText.Append(" AND QCControlLotID = ")
        '                cmdText.AppendFormat("{0}", qcResultRow.QCControlLotID)
        '                cmdText.Append(vbCrLf)
        '                cmdText.Append(" AND RunsGroupNumber = ")
        '                cmdText.AppendFormat("{0}", qcResultRow.RunsGroupNumber)
        '                cmdText.Append(vbCrLf)
        '                cmdText.Append(" AND RunNumber = ")
        '                cmdText.AppendFormat("{0}", qcResultRow.RunNumber)
        '                cmdText.Append(vbCrLf)

        '                i += 1
        '                If (i = maxUpdates) Then
        '                    'Execute the SQL script 
        '                    Dim dbCmd As New SqlClient.SqlCommand
        '                    dbCmd.Connection = pDBConnection
        '                    dbCmd.CommandText = cmdText.ToString()

        '                    myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()

        '                    'Initialize the counter and the StringBuilder
        '                    i = 0
        '                    cmdText.Remove(0, cmdText.Length)
        '                End If
        '            Next

        '            If (Not myGlobalDataTO.HasError) Then
        '                If (cmdText.Length > 0) Then
        '                    'Execute the remaining Updates...
        '                    Dim dbCmd As New SqlClient.SqlCommand
        '                    dbCmd.Connection = pDBConnection
        '                    dbCmd.CommandText = cmdText.ToString()

        '                    myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.MarkStatisticResults ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        '''' informed Test/Sample Type, Control/Lot and Runs Group Number
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pNewRunsGroupNumber">Number of the open Runs Group  for the Test/SampleType and Control/Lot to which 
        ''''                                   the QC Results have to be assigned</param>
        '''' <returns>GlobalDataTO containing success / error information</returns>
        '''' <remarks>
        '''' Created by: SA 15/12/2011
        '''' </remarks>
        'Public Function MoveStatisticResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                        ByVal pQCControlLotID As Integer, ByVal pNewRunsGroupNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tqcResults " & vbCrLf & _
        '                                    " SET    RunsGroupNumber = " & pNewRunsGroupNumber.ToString & vbCrLf & _
        '                                    " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                    " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
        '                                    " AND    RunsGroupNumber = " & (pNewRunsGroupNumber - 1).ToString & vbCrLf & _
        '                                    " AND    IncludedInMean = 1 "

        '            'Get the value and set it to the set of data to return
        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.MoveStatisticResults ", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Select all Tests/SampleTypes with not cumulated QC Results for the specified Control/Lot
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing a typed DataSet XX with statistical QC data for all 
        ''''          Tests/SampleTypes linked to the informed Control/Lot that have non cumulated QC Results</returns>
        '''' <remarks>
        '''' Created by:  SA 09/06/2011
        '''' Modified by: SA 05/07/2011 - Changed the query to get also the SUMs of manual QC Results 
        ''''              SA 01/12/2011 - Changed the query to remove the SUMs of QC Results by Test/SampleType. Filter data by IncludedInMean=FALSE
        ''''                              to get only Test/SampleType with enough Results to cumulate for the Test/SampleType
        '''' </remarks>
        'Public Function ReadByQCControlLotIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT 1 AS Selected, R.QCTestSampleID, R.RunsGroupNumber, HTS.PreloadedTest, HTS.TestName, HTS.SampleType, " & vbCrLf & _
        '                                               " HTS.DecimalsAllowed, HTS.RejectionCriteria, HTS.CalculationMode, HTS.NumberOfSeries, " & vbCrLf & _
        '                                               " MD.FixedItemDesc AS MeasureUnit, COUNT(R.ResultValue) AS n, MIN(R.ResultDateTime) AS MinResultDateTime, " & vbCrLf & _
        '                                               " MAX(R.ResultDateTime) AS MaxResultDateTime " & vbCrLf & _
        '                                        " FROM   tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                            " INNER JOIN tqcHistoryTestControlLots HTCL ON R.QCTestSampleID = HTCL.QCTestSampleID AND R.QCControlLotID = HTCL.QCControlLotID " & vbCrLf & _
        '                                                            " INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID " & vbCrLf & _
        '                                        " WHERE  R.QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
        '                                        " AND    R.ClosedResult   = 0 " & vbCrLf & _
        '                                        " AND    R.Excluded       = 0 " & vbCrLf & _
        '                                        " AND    R.IncludedInMean = 0 " & vbCrLf & _
        '                                        " AND    MD.SubTableID    = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
        '                                        " GROUP BY R.QCTestSampleID, R.RunsGroupNumber, HTS.PreloadedTest, HTS.TestName, HTS.SampleType, HTS.DecimalsAllowed, " & vbCrLf & _
        '                                                 " HTS.RejectionCriteria, HTS.CalculationMode, HTS.NumberOfSeries, MD.FixedItemDesc " & vbCrLf & _
        '                                        " ORDER BY HTS.PreloadedTest DESC, HTS.TestName, HTS.SampleType " & vbCrLf

        '                Dim myOpenQCResultDS As New OpenQCResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myOpenQCResultDS.tOpenResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myOpenQCResultDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.ReadByQCControlLotID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Verify if there are QC Results pending to cumulate for the specified Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlID">Control Identifier</param>
        '''' <param name="pTestID">Test Identifier. Optional parameter</param>
        '''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Tests/SampleTypes
        ''''          for which the informed Control has QC Results pending to accumulate</returns>
        '''' <remarks>
        '''' Created by:  SA 16/05/2011
        '''' Modified by: SA 24/05/2011 - Added optional parameters to filter results also by TestID and SampleType
        ''''              SA 20/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        ''''                              values (due to these values are not cumulated)
        '''' </remarks>
        'Public Function SearchPendingResultsByControlOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
        '                                                 Optional ByVal pTestID As Integer = 0, Optional ByVal pSampleType As String = "") As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT DISTINCT R.QCControlLotID, R.QCTestSampleID, HTS.TestID, HTS.SampleType, HTS.PreloadedTest, HTS.TestName, " & vbCrLf & _
        '                                                        " HCL.ControlID, HCL.LotNumber, HCL.ControlName " & vbCrLf & _
        '                                        " FROM tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                          " INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                        " WHERE R.ClosedResult        = 0 " & vbCrLf & _
        '                                        " AND   R.Excluded            = 0 " & vbCrLf & _
        '                                        " AND   R.IncludedInMean      = 0 " & vbCrLf & _
        '                                        " AND   HCL.ControlID         = " & pControlID & vbCrLf & _
        '                                        " AND   HCL.ClosedLot         = 0 " & vbCrLf & _
        '                                        " AND   HCL.DeletedControl    = 0 " & vbCrLf & _
        '                                        " AND   HTS.DeletedSampleType = 0 " & vbCrLf & _
        '                                        " AND   HTS.DeletedTest       = 0 " & vbCrLf

        '                If (pTestID <> 0 AndAlso pSampleType <> String.Empty) Then
        '                    cmdText &= " AND HTS.TestID = " & pTestID & vbCrLf & _
        '                               " AND HTS.SampleType = '" & pSampleType & "' "
        '                End If

        '                Dim myHistoryQCInfoDS As New HistoryQCInformationDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myHistoryQCInfoDS.HistoryQCInfoTable)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myHistoryQCInfoDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.SearchPendingResultsByControl", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Verify if there are QC Results pending to cumulate for the specified TestID and Sample Type
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Controls/Lots
        ''''          for which the informed Test/SampleType has QC Results pending to accumulate</returns>
        '''' <remarks>
        '''' Created by:  TR 24/05/2011
        '''' Modified by: SA 20/12/2011 - Added a filter by field IncludedInMean=FALSE to exclude QC Results used to calculate statistical 
        ''''                              values (due to these values are not cumulated). Get also field CalculationMode
        '''' </remarks>
        'Public Function SearchPendingResultsByTestIDSampleTypeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
        '                                                          ByVal pSampleType As String) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT DISTINCT R.QCControlLotID, R.QCTestSampleID, HTS.TestID, HTS.SampleType, HTS.PreloadedTest, " & vbCrLf & _
        '                                                        " HTS.TestName, HTS.CalculationMode, HCL.ControlID, HCL.LotNumber, HCL.ControlName " & vbCrLf & _
        '                                        " FROM tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                          " INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                        " WHERE R.ClosedResult   = 0 " & vbCrLf & _
        '                                        " AND   R.Excluded       = 0 " & vbCrLf & _
        '                                        " AND   R.IncludedInMean = 0 " & vbCrLf & _
        '                                        " AND   HTS.TestID       = " & pTestID & vbCrLf

        '                'Validate if Sample Type is informed to add it as filter
        '                If (pSampleType <> "") Then cmdText &= " AND HTS.SampleType= '" & pSampleType & "' " & vbCrLf

        '                'Add the rest of query filters...
        '                cmdText &= " AND HCL.ClosedLot         = 0 " & vbCrLf
        '                cmdText &= " AND HCL.DeletedControl    = 0 " & vbCrLf
        '                cmdText &= " AND HTS.DeletedSampleType = 0 " & vbCrLf
        '                cmdText &= " AND HTS.DeletedTest       = 0 " & vbCrLf

        '                Dim myHistoryQCInfoDS As New HistoryQCInformationDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myHistoryQCInfoDS.HistoryQCInfoTable)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myHistoryQCInfoDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.SearchPendingResultsByTestIDSampleType", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        '''' informed Test/Sample Type and optionally, the Control/Lot
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        '''' <returns>GlobalDataTO containing the total number of QC Results marked as included in Mean
        ''''          for the specified Test/SampleType and optionally, the Control/Lot</returns>
        '''' <remarks>
        '''' Created by: SA 15/12/2011
        '''' </remarks>
        'Public Function UnmarkStatisticResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                       Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tqcResults " & vbCrLf & _
        '                                    " SET    IncludedInMean = 0 " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                    " AND    ClosedResult = 0 " & vbCrLf & _
        '                                    " AND    IncludedInMean = 1 " & vbCrLf

        '            If (pQCControlLotID <> -1) Then cmdText &= " AND QCControlLotID = " & pQCControlLotID.ToString & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                myGlobalDataTO.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcResultsDAO.UnmarkStatisticResults ", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        'Public Function GetFirstDateTimeForResultsCreationOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                                     ByVal pControlID As Integer, ByVal pLotNumber As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT MAX(R.ResultDateTime) AS FirstDateTime " & vbCrLf & _
        '                                        " FROM   tqcResults R INNER JOIN tqcHistoryTestSamples HTS ON R.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                            " INNER JOIN tqcHistoryControlLots HCL ON R.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                        " WHERE  HTS.TestID     = " & pTestID.ToString & vbCrLf & _
        '                                        " AND    HTS.SampleType = '" & pSampleType & "' " & vbCrLf & _
        '                                        " AND    HCL.ControlID  = " & pControlID.ToString & vbCrLf & _
        '                                        " AND    HCL.LotNumber  = '" & pLotNumber & "' " & vbCrLf & _
        '                                        " AND    R.ClosedResult = 0 " & vbCrLf

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
        '                    myGlobalDataTO.HasError = False
        '                End Using
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tqcResultsDAO.GetFirstDateTimeForResultsCreation", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

#End Region

    End Class
End Namespace
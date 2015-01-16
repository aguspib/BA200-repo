Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tqcCumulatedResultsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create a new Cumulated Serie for a QCTestSampleID/QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCumulatedResultDS">Typed DataSet containing all data needed to create the new Cumulated Serie</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' Modified by: SA 02/06/2011 - Use function ReplaceNumericString instead ToSQLString to avoid loss precision in Single 
        '''                              and Double fields
        '''              SA 15/06/2011 - Included new fields FirstRunDateTime and LastRunDateTime in the INSERT sentence
        '''              SA 27/06/2011 - Included new field XMLFileName in the INSERT sentence; removed field SumResultsSQRD
        '''              SA 05/06/2012 - Changed the query to insert also new field AnalyzerID
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pCumulatedResultDS Is Nothing) Then
                    If (pCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
                        Dim cmdText As String = " INSERT INTO tqcCumulatedResults (QCTestSampleID, QCControlLotID, AnalyzerID, CumResultsNum, CumDateTime, TotalRuns, " & vbCrLf & _
                                                                                 " FirstRunDateTime, LastRunDateTime, SumResults, SumSQRDResults, Mean, SD, XMLFileName, " & vbCrLf & _
                                                                                 " TS_User, TS_DateTime) " & vbCrLf & _
                                                " VALUES (" & pCumulatedResultDS.tqcCumulatedResults(0).QCTestSampleID.ToString & ", " & vbCrLf & _
                                                              pCumulatedResultDS.tqcCumulatedResults(0).QCControlLotID.ToString & ", " & vbCrLf & _
                                                      " N'" & pCumulatedResultDS.tqcCumulatedResults(0).AnalyzerID.Replace("'", "''").Trim & "', " & vbCrLf & _
                                                              pCumulatedResultDS.tqcCumulatedResults(0).CumResultsNum.ToString & ", " & vbCrLf & _
                                                       " '" & pCumulatedResultDS.tqcCumulatedResults(0).CumDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                                              pCumulatedResultDS.tqcCumulatedResults(0).TotalRuns.ToString & ", " & vbCrLf & _
                                                       " '" & pCumulatedResultDS.tqcCumulatedResults(0).FirstRunDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                                       " '" & pCumulatedResultDS.tqcCumulatedResults(0).LastRunDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumResults) & ", " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumSQRDResults) & ", " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).Mean) & ", " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SD) & ", " & vbCrLf & _
                                                       " '" & pCumulatedResultDS.tqcCumulatedResults(0).XMLFileName.Trim & "', " & vbCrLf

                        If (pCumulatedResultDS.tqcCumulatedResults(0).IsTS_UserNull) Then
                            Dim myGlobalBase As New GlobalBase
                            cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                        Else
                            cmdText &= " N'" & pCumulatedResultDS.tqcCumulatedResults(0).TS_User.Replace("'", "''") & "', "
                        End If

                        If (pCumulatedResultDS.tqcCumulatedResults(0).IsTS_DateTimeNull) Then
                            cmdText &= " '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                        Else
                            cmdText &= " '" & pCumulatedResultDS.tqcCumulatedResults(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
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
                GlobalBase.CreateLogActivity(ex.Message, " tqcCumulatedResultsDAO.Create ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the informed Cumulated Serie for a QCTestSampleID and QCControlLot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCumResultsNum">Number of the Cumulated Serie to delete for the QCTestSampleID and QCControlLotID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                  ByVal pAnalyzerID As String, ByVal pCumResultsNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tqcCumulatedResults " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
                                            " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND    CumResultsNum  = " & pCumResultsNum.ToString

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
                GlobalBase.CreateLogActivity(ex.Message, " tqcCumulatedResultsDAO.Delete ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all saved Cumulated Series for the specified QCTestSampleID/QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS with all saved Cumulated Series for 
        '''          the specified QCTestSampleID/QCControlLotID</returns>
        ''' <remarks>
        ''' Created by:  TR 19/05/2011
        ''' Modified by: SA 29/06/2011 - Sort records by the number of Cumulated Serie
        '''              SA 05/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function ReadByQcTestSampleIDQCControlLotIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                              ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tqcCumulatedResults " & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    QCControlLotID = " & pQCControlLotID & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " ORDER BY CumResultsNum "

                        Dim myCumulatedResultDS As New CumulatedResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCumulatedResultDS.tqcCumulatedResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myCumulatedResultDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tqcCumulatedResultsDAO.ReadByQcTestSampleIDQCControlLotID ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' For the specified QCTestSampleID and QCControlLotID, decrement in one the CumResultsNum for all its 
        ''' Cumulated Series
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/06/2011 
        ''' Modified by: SA 05/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function DecrementCumResultsNumNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                  ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcCumulatedResults SET CumResultsNum = CumResultsNum - 1 " & vbCrLf & _
                                            " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.DecrementCumResultsNum", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Controls/Lots that have Cumulated Series for the specified Test/SampleType in the informed period of time
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        ''' <param name="pDateTo">Date To to search Cumulated Series</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCCumulatedSummaryDS containing not calculated data of all Controls/Lots
        '''          that have Cumulated Series for the specified Test/SampleType in the informed period of time</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: SA 03/01/2012 - Added also fields ClosedLot and DeletedControl from table tqcHistoryControlLots
        '''              SA 05/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetControlsLotsWithCumulatedSeriesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                              ByVal pAnalyzerID As String, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT 0 AS Selected, CR.QCControlLotID, HCL.ControlName, HCL.LotNumber, COUNT(*) AS N, " & vbCrLf & _
                                                       " MD.FixedItemDesc AS MeasureUnit, HTS.RejectionCriteria, MIN(CR.FirstRunDateTime) AS MinDate, " & vbCrLf & _
                                                       " MAX(CR.LastRunDateTime) AS MaxDate, (HCL.ClosedLot | HCL.DeletedControl) AS DeletedControlLot " & vbCrLf & _
                                                " FROM   tqcCumulatedResults CR INNER JOIN tqcHistoryControlLots HCL ON CR.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                                              " INNER JOIN tqcHistoryTestSamples HTS ON CR.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                              " INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID " & vbCrLf & _
                                                " WHERE  CR.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    CR.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', CR.CumDateTime) >= 0 " & vbCrLf & _
                                                " AND    DATEDIFF(DAY, CR.CumDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') >= 0 " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                                                " GROUP  BY CR.QCControlLotID, HCL.ControlName, HCL.LotNumber, MD.FixedItemDesc, HTS.RejectionCriteria, " & vbCrLf & _
                                                          " HCL.ClosedLot, HCL.DeletedControl " & vbCrLf & _
                                                " ORDER  BY HCL.ControlName, HCL.LotNumber "

                        Dim myCumulatedSummaryDS As New QCCumulatedSummaryDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCumulatedSummaryDS.QCCumulatedSummaryTable)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myCumulatedSummaryDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.GetControlsLotsWithCumulatedSeries", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get details of all Cumulated Series of the the specified Test/SampleType (all Controls/Lots) in the informed period of time
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        ''' <param name="pDateTo">Date To to search Cumulated Series</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS containing data of all Cumulated Series saved for 
        '''          the specified Test/SampleType (all Controls/Lots) in the informed period of time</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: SA 28/06/2011 - Removed field CR.SumResultsSQRD from the query
        '''              SA 05/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        '''              JV 06/11/2013 - issue #1185 avoid dividing by zero if Mean = 0 -> caused exception. In this case, we assign CV = 0
        ''' </remarks>
        Public Function GetCumulatedSeriesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                              ByVal pAnalyzerID As String, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim cmdText As String = " SELECT CR.QCTestSampleID, CR.QCControlLotID, HCL.ControlName, HCL.LotNumber, CR.CumResultsNum, CR.CumDateTime, " & vbCrLf & _
                        '                               " CR.TotalRuns, CR.SumResults, CR.SumSQRDResults, CR.XMLFileName, MD.FixedItemDesc AS MeasureUnit,  " & vbCrLf & _
                        '                               " CR.Mean, CR.SD, (CR.SD/CR.Mean)*100 AS CV, (CR.Mean - (HTS.RejectionCriteria * CR.SD)) AS MinRange, " & vbCrLf & _
                        '                               " (CR.Mean +(HTS.RejectionCriteria * CR.SD)) AS MaxRange " & vbCrLf & _
                        '                        " FROM   tqcCumulatedResults CR INNER JOIN tqcHistoryControlLots HCL ON CR.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                        '                                                      " INNER JOIN tqcHistoryTestSamples HTS ON CR.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                        '                                                      " INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID " & vbCrLf & _
                        '                        " WHERE  CR.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                        '                        " AND    CR.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                        '                        " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', CR.CumDateTime) >= 0 " & vbCrLf & _
                        '                        " AND    DATEDIFF(DAY, CR.CumDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') >= 0 " & vbCrLf & _
                        '                        " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                        '                        " ORDER BY HCL.ControlName, HCL.LotNumber, CR.CumDateTime "

                        Dim cmdText As String = " SELECT CR.QCTestSampleID, CR.QCControlLotID, HCL.ControlName, HCL.LotNumber, CR.CumResultsNum, CR.CumDateTime, " & vbCrLf & _
                                                       " CR.TotalRuns, CR.SumResults, CR.SumSQRDResults, CR.XMLFileName, MD.FixedItemDesc AS MeasureUnit,  " & vbCrLf & _
                                                       " CR.Mean, CR.SD, COALESCE(CR.SD/NULLIF(CR.Mean,0),0)*100 AS CV, (CR.Mean - (HTS.RejectionCriteria * CR.SD)) AS MinRange, " & vbCrLf & _
                                                       " (CR.Mean +(HTS.RejectionCriteria * CR.SD)) AS MaxRange " & vbCrLf & _
                                                " FROM   tqcCumulatedResults CR INNER JOIN tqcHistoryControlLots HCL ON CR.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
                                                                              " INNER JOIN tqcHistoryTestSamples HTS ON CR.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
                                                                              " INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID " & vbCrLf & _
                                                " WHERE  CR.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    CR.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', CR.CumDateTime) >= 0 " & vbCrLf & _
                                                " AND    DATEDIFF(DAY, CR.CumDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') >= 0 " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                                                " ORDER BY HCL.ControlName, HCL.LotNumber, CR.CumDateTime "

                        Dim myCumulatedSeriesDS As New CumulatedResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCumulatedSeriesDS.tqcCumulatedResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myCumulatedSeriesDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.GetCumulatedSeries", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the oldest date of a cumulated serie for the specified QCTestSampleID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function GetMinCumDateTimeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                             ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MIN(CumDateTime) FROM tqcCumulatedResults " & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.GetMinCumDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get the newest date of a cumulated serie for the specified QCTestSampleID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pQCTestSampleID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 02/07/2012 </remarks>
        Public Function GetMaxCumDateTime(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                             ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(CumDateTime) FROM tqcCumulatedResults " & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.GetMaxCumDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Create a new Cumulated Serie for a QCTestSampleID/QCControlLotID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pCumulatedResultDS">Typed DataSet containing all data needed to create the new Cumulated Serie</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 13/05/2011
        '''' Modified by: SA 02/06/2011 - Use function ReplaceNumericString instead ToSQLString to avoid loss precision in Single 
        ''''                              and Double fields
        ''''              SA 15/06/2011 - Included new fields FirstRunDateTime and LastRunDateTime in the INSERT sentence
        ''''              SA 27/06/2011 - Included new field XMLFileName in the INSERT sentence; removed field SumResultsSQRD
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

        '        ElseIf (Not pCumulatedResultDS Is Nothing) Then
        '            If (pCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
        '                Dim cmdText As String = " INSERT INTO tqcCumulatedResults (QCTestSampleID, QCControlLotID, CumResultsNum, CumDateTime, TotalRuns, FirstRunDateTime, " & vbCrLf & _
        '                                                                         " LastRunDateTime, SumResults, SumSQRDResults, Mean, SD, XMLFileName, " & vbCrLf & _
        '                                                                         " TS_User, TS_DateTime) " & vbCrLf & _
        '                                        " VALUES (" & pCumulatedResultDS.tqcCumulatedResults(0).QCTestSampleID & ", " & vbCrLf & _
        '                                                      pCumulatedResultDS.tqcCumulatedResults(0).QCControlLotID & ", " & vbCrLf & _
        '                                                      pCumulatedResultDS.tqcCumulatedResults(0).CumResultsNum & ", " & vbCrLf & _
        '                                               " '" & pCumulatedResultDS.tqcCumulatedResults(0).CumDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
        '                                                      pCumulatedResultDS.tqcCumulatedResults(0).TotalRuns & ", " & vbCrLf & _
        '                                               " '" & pCumulatedResultDS.tqcCumulatedResults(0).FirstRunDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
        '                                               " '" & pCumulatedResultDS.tqcCumulatedResults(0).LastRunDateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumResults) & ", " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumSQRDResults) & ", " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).Mean) & ", " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SD) & ", " & vbCrLf & _
        '                                               " '" & pCumulatedResultDS.tqcCumulatedResults(0).XMLFileName & "', " & vbCrLf

        '                If (pCumulatedResultDS.tqcCumulatedResults(0).IsTS_UserNull) Then
        '                    Dim myGlobalBase As New GlobalBase
        '                    cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
        '                Else
        '                    cmdText &= " N'" & pCumulatedResultDS.tqcCumulatedResults(0).TS_User.Replace("'", "''") & "', "
        '                End If

        '                If (pCumulatedResultDS.tqcCumulatedResults(0).IsTS_DateTimeNull) Then
        '                    cmdText &= " '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "') "
        '                Else
        '                    cmdText &= " '" & pCumulatedResultDS.tqcCumulatedResults(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
        '                End If

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                    myGlobalDataTO.HasError = False
        '                End Using
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, " tqcCumulatedResultsDAO.Create ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete the informed Cumulated Serie for a QCTestSampleID and QCControlLot
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        '''' <param name="pCumResultsNum">Number of the Cumulated Serie to delete for the QCTestSampleID and QCControlLotID</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' </remarks>
        'Public Function DeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                       ByVal pCumResultsNum As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " DELETE FROM tqcCumulatedResults " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
        '                                    " AND    QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
        '                                    " AND    CumResultsNum  = " & pCumResultsNum.ToString

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
        '        GlobalBase.CreateLogActivity(ex.Message, " tqcCumulatedResultsDAO.Delete ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all saved Cumulated Series for the specified QCTestSampleID/QCControlLotID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS with all saved Cumulated Series for 
        ''''          the specified QCTestSampleID/QCControlLotID</returns>
        '''' <remarks>
        '''' Created by:  TR 19/05/2011
        '''' Modified by: SA 29/06/2011 - Sort records by the number of Cumulated Serie
        '''' </remarks>
        'Public Function ReadByQcTestSampleIDQCControlLotIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                                      ByVal pQCControlLotID As Integer) As GlobalDataTO

        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing
        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * FROM tqcCumulatedResults " & vbCrLf & _
        '                                        " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    QCControlLotID = " & pQCControlLotID & vbCrLf & _
        '                                        " ORDER BY CumResultsNum "

        '                Dim myCumulatedResultDS As New CumulatedResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myCumulatedResultDS.tqcCumulatedResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myCumulatedResultDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, " tqcCumulatedResultsDAO.ReadByQcTestSampleIDQCControlLotID ", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCTestSampleID and QCControlLotID, decrement in one the CumResultsNum for all its 
        '''' Cumulated Series
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 21/06/2011 
        '''' </remarks>
        'Public Function DecrementCumResultsNumOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer) _
        '                                       As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tqcCumulatedResults SET CumResultsNum = CumResultsNum - 1 " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                    " AND    QCControlLotID  = " & pQCControlLotID.ToString()

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
        '        GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.DecrementCumResultsNum", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all Controls/Lots that have Cumulated Series for the specified Test/SampleType in the informed period of time
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        '''' <param name="pDateTo">Date To to search Cumulated Series</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCCumulatedSummaryDS containing not calculated data of all Controls/Lots
        ''''          that have Cumulated Series for the specified Test/SampleType in the informed period of time</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' Modified by: SA 03/01/2012 - Added also fields ClosedLot and DeletedControl from table tqcHistoryControlLots
        '''' </remarks>
        'Public Function GetControlsLotsWithCumulatedSeriesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                                   ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT 0 AS Selected, CR.QCControlLotID, HCL.ControlName, HCL.LotNumber, COUNT(*) AS N, " & vbCrLf & _
        '                                               " MD.FixedItemDesc AS MeasureUnit, HTS.RejectionCriteria, MIN(CR.FirstRunDateTime) AS MinDate, " & vbCrLf & _
        '                                               " MAX(CR.LastRunDateTime) AS MaxDate, (HCL.ClosedLot | HCL.DeletedControl) AS DeletedControlLot " & vbCrLf & _
        '                                        " FROM   tqcCumulatedResults CR INNER JOIN tqcHistoryControlLots HCL ON CR.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                                                      " INNER JOIN tqcHistoryTestSamples HTS ON CR.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                                      " INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID " & vbCrLf & _
        '                                        " WHERE  CR.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', CR.CumDateTime) >= 0 " & vbCrLf & _
        '                                        " AND    DATEDIFF(DAY, CR.CumDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') >= 0 " & vbCrLf & _
        '                                        " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
        '                                        " GROUP  BY CR.QCControlLotID, HCL.ControlName, HCL.LotNumber, MD.FixedItemDesc, HTS.RejectionCriteria, " & vbCrLf & _
        '                                                  " HCL.ClosedLot, HCL.DeletedControl " & vbCrLf & _
        '                                        " ORDER  BY HCL.ControlName, HCL.LotNumber "

        '                Dim myCumulatedSummaryDS As New QCCumulatedSummaryDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myCumulatedSummaryDS.QCCumulatedSummaryTable)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myCumulatedSummaryDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.GetControlsLotsWithCumulatedSeries", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get details of all Cumulated Series of the the specified Test/SampleType (all Controls/Lots) in the informed period of time
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        '''' <param name="pDateTo">Date To to search Cumulated Series</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS containing data of all Cumulated Series saved for 
        ''''          the specified Test/SampleType (all Controls/Lots) in the informed period of time</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' Modified by: SA 28/06/2011 - Removed field CR.SumResultsSQRD from the query
        '''' </remarks>
        'Public Function GetCumulatedSeriesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pDateFrom As DateTime, _
        '                                   ByVal pDateTo As DateTime) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT CR.QCTestSampleID, CR.QCControlLotID, HCL.ControlName, HCL.LotNumber, CR.CumResultsNum, CR.CumDateTime, " & vbCrLf & _
        '                                               " CR.TotalRuns, CR.SumResults, CR.SumSQRDResults, CR.XMLFileName, MD.FixedItemDesc AS MeasureUnit,  " & vbCrLf & _
        '                                               " CR.Mean, CR.SD, (CR.SD/CR.Mean)*100 AS CV, (CR.Mean - (HTS.RejectionCriteria * CR.SD)) AS MinRange, " & vbCrLf & _
        '                                               " (CR.Mean +(HTS.RejectionCriteria * CR.SD)) AS MaxRange " & vbCrLf & _
        '                                        " FROM   tqcCumulatedResults CR INNER JOIN tqcHistoryControlLots HCL ON CR.QCControlLotID = HCL.QCControlLotID " & vbCrLf & _
        '                                                                      " INNER JOIN tqcHistoryTestSamples HTS ON CR.QCTestSampleID = HTS.QCTestSampleID " & vbCrLf & _
        '                                                                      " INNER JOIN tcfgMasterData MD ON HTS.MeasureUnit = MD.ItemID " & vbCrLf & _
        '                                        " WHERE  CR.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                        " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', CR.CumDateTime) >= 0 " & vbCrLf & _
        '                                        " AND    DATEDIFF(DAY, CR.CumDateTime, '" & pDateTo.ToString("yyyyMMdd") & "') >= 0 " & vbCrLf & _
        '                                        " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
        '                                        " ORDER BY HCL.ControlName, HCL.LotNumber, CR.CumDateTime "

        '                Dim myCumulatedSeriesDS As New CumulatedResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myCumulatedSeriesDS.tqcCumulatedResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myCumulatedSeriesDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.GetCumulatedSeries", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the oldest date of a cumulated serie for the specified QCTestSampleID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' </remarks>
        'Public Function GetMinCumDateTimeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT MIN(CumDateTime) FROM tqcCumulatedResults " & vbCrLf & _
        '                                        " WHERE  QCTestSampleID = " & pQCTestSampleID

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
        '        GlobalBase.CreateLogActivity(ex.Message, "tqcCumulatedResultsDAO.GetMinCumDateTime", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region

    End Class
End Namespace


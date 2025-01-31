﻿Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tqcRunsGroupsDAO

#Region "CRUD Methods"
        ''' <summary>
        ''' Create a new Runs Group in QC Module for a QCTestSampleID/QCControlID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRunsGroupsDS">Typed DataSet RunsGroupDS containing data of the Runs Group to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' Modified by: SA 04/06/2012 - Changed the query to insert also new field AnalyzerID
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRunsGroupsDS As RunsGroupsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    For Each runsGroupRow As RunsGroupsDS.tqcRunsGroupsRow In pRunsGroupsDS.tqcRunsGroups.Rows
                        cmdText &= " INSERT INTO tqcRunsGroups (QCTestSampleID, QCControlLotID, AnalyzerID, RunsGroupNumber,ClosedRunsGroup,CumResultsNum) " & vbCrLf & _
                                   " VALUES (" & runsGroupRow.QCTestSampleID & ", " & vbCrLf & _
                                                 runsGroupRow.QCControlLotID.ToString() & ", " & vbCrLf & _
                                         " N'" & runsGroupRow.AnalyzerID.Replace("'", "''").Trim & "', " & vbCrLf & _
                                                 runsGroupRow.RunsGroupNumber.ToString() & ", " & vbCrLf & _
                                                 Convert.ToInt32(IIf(runsGroupRow.ClosedRunsGroup, 1, 0)) & ", " & vbCrLf

                        If (runsGroupRow.IsCumResultsNumNull) Then
                            cmdText &= " NULL "
                        Else
                            cmdText &= runsGroupRow.CumResultsNum.ToString()
                        End If
                        cmdText &= ") " & vbCrLf
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcRunsGroupsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID, QCControlLotID and AnalyzerID, delete the RunsGroup for the informed Cumulated Serie
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
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                  ByVal pAnalyzerID As String, ByVal pCumResultsNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tqcRunsGroups " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcRunsGroupsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Runs Groups that exists for the informed QCTestSampleID, QCControlLotID and AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RunsGroupDS with all Runs Groups that exist
        '''          for the informed QCTestSampleID, QCControlLotID and AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function ReadByQCTestSampleIDQCControlLotIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                              ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM  tqcRunsGroups " & vbCrLf & _
                                                " WHERE QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND   QCControlLotID = " & pQCControlLotID & vbCrLf & _
                                                " AND   AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                        Dim myRunsGroupsDS As New RunsGroupsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myRunsGroupsDS.tqcRunsGroups)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myRunsGroupsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcRunsGroupsDAO.ReadByQCTestSampleIDQCControlLotID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' When results included in a Runs Group of a QCTestSampleID/QCControlID, the Runs Group is updated marking it as 
        ''' Closed and informing the number of the Cumulated generated for the Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Runs Group Number</param>
        ''' <param name="pCumResultsNum">Number of the Cumulated Serie created for the Results</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 19/05/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function CloseRunsGroupNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                          ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer, ByVal pCumResultsNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcRunsGroups " & vbCrLf & _
                                            " SET    ClosedRunsGroup = 1, CumResultsNum = " & pCumResultsNum.ToString & vbCrLf & _
                                            " WHERE QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND   QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND   AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND   RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcRunsGroupsDAO.CloseRunsGroup", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID and QCControlLotID, decrement in one the CumResultsNum for all Closed Runs Groups 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/06/2011 
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function DecrementCumResultsNumNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                  ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcRunsGroups SET CumResultsNum = CumResultsNum - 1 " & vbCrLf & _
                                            " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                            " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                            " AND    ClosedRunsGroup = 1 "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcRunsGroupsDAO.DecrementCumResultsNum", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For an specific QCTestSampleID, check the Status of the RunsGroupNumber for each one of the linked Control/Lots with open QC Results, 
        ''' and return QCControlLotID and RunsGroupNumber of all Runs Groups with ClosedRunsGroup = TRUE.
        ''' 
        ''' Used when the Calculation Mode of a Test/Sample Type is changed from STATISTICS to MANUAL and the unique Open Results that exist for 
        ''' Control/Lot are those included in the Mean
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RunsGroupDS with all Closed Runs Groups with not Closed Results for the informed 
        '''          QCTestSampleID and AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  SA 30/01/2015 - BA-1098
        ''' </remarks>
        Public Function ReadClosedRunsGroupsWithOpenResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                            ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT RG.* " & vbCrLf & _
                                                " FROM tqcResults R INNER JOIN tqcRunsGroups RG ON R.QCTestSampleID  = RG.QCTestSampleID " & vbCrLf & _
                                                                                             " AND R.QCControlLotID  = RG.QCControlLotID " & vbCrLf & _
                                                                                             " AND R.AnalyzerID      = RG.AnalyzerID " & vbCrLf & _
                                                                                             " AND R.RunsGroupNumber = RG.RunsGroupNumber " & vbCrLf & _
                                                " WHERE R.QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND   R.AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND   RG.ClosedRunsGroup = 1 " & vbCrLf

                        Dim myRunsGroupsDS As New RunsGroupsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myRunsGroupsDS.tqcRunsGroups)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myRunsGroupsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "tqcRunsGroupsDAO.ReadClosedRunsGroupsWithOpenResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

    End Class
End Namespace

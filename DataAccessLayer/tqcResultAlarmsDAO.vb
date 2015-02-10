Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tqcResultAlarmsDAO

#Region "CRUD Methods"
        ''' <summary>
        ''' Insert Alarms for QC Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing the Alarms to insert</param>
        ''' <returns>GlobalDataSet containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 02/01/2011
        ''' Modified by: SA 04/06/2012 - Changed the query to insert also new field AnalyzerID
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultAlarmsDS As QCResultAlarms) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection

                    Dim cmdText As String = String.Empty
                    For Each resultAlarmRow As QCResultAlarms.tqcResultAlarmsRow In pQCResultAlarmsDS.tqcResultAlarms.Rows
                        cmdText = " INSERT INTO tqcResultAlarms (QCTestSampleID, QCControlLotID, AnalyzerID, RunsGroupNumber, RunNumber, AlarmID) " & vbCrLf & _
                                  " VALUES (" & resultAlarmRow.QCTestSampleID.ToString() & ", " & vbCrLf & _
                                                resultAlarmRow.QCControlLotID.ToString() & ", " & vbCrLf & _
                                        " N'" & resultAlarmRow.AnalyzerID.Replace("'", "''").Trim & "', " & vbCrLf & _
                                                resultAlarmRow.RunsGroupNumber.ToString() & ", " & vbCrLf & _
                                                resultAlarmRow.RunNumber.ToString & ", " & vbCrLf & _
                                         " '" & resultAlarmRow.AlarmID.Trim & "') " & vbCrLf

                        dbCmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcResultAlarmsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete Alarms for all results of the specified QCTestSampleID/QCControlLotID/AnalyzerID, and optionally, 
        ''' RunsGroupNumber and an specific result (RunNumber)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Optional parameter. Number of the open Runs Group for the specified
        '''                                QCTestSampleID/QCControlLotID/AnalyzerID</param>
        ''' <param name="pRunNumber">Optional parameter. Run Number of an specific Result</param>
        ''' <returns>GlobalDataSet containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 01/06/2011
        ''' Modified by: SA 06/07/2011 - Added optional parameter to delete only the Alarms of an specific result
        '''              SA 21/12/2011 - If parameter for the RunsGroupNumber is not informed, the value is obtained from table tqcResults
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID; changed the query by adding a filter by this field when informed
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                  Optional ByVal pAnalyzerID As String = "", Optional ByVal pRunsGroupNumber As Integer = 0, Optional ByVal pRunNumber As Integer = 0) _
                                  As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tqcResultAlarms " & vbCrLf & _
                                            " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                            " AND    QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf

                    'Add the filter by AnalyzerID when it has been specified
                    If (pAnalyzerID.Trim <> String.Empty) Then cmdText &= " AND AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                    'Add the filter by RunsGroupNumber
                    If (pRunsGroupNumber <> 0) Then
                        cmdText &= " AND RunsGroupNumber = " & pRunsGroupNumber
                    Else
                        cmdText &= " AND RunsGroupNumber = (SELECT DISTINCT RunsGroupNumber FROM tqcResults " & vbCrLf & _
                                                          " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
                                                          " AND    QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
                                                          " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "') " & vbCrLf
                    End If

                    'Add the filter by RunNumber when it has been specified
                    If (pRunNumber <> 0) Then cmdText &= " AND RunNumber = " & pRunNumber.ToString & vbCrLf

                    Using cmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcResultAlarmsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all Alarms for all QC Results included in the informed RunsGroup for the informed QCTestSampleID/QCControlLotID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Number of the Runs Group in which the results are included</param>
        ''' <returns>GlobalDataSet containing a typed DataSet QCResultAlarms with the list of Alarms for all QC Results
        '''          included in the informed RunsGroup for the Test/SampleType and Control/Lot</returns>
        ''' <remarks>
        ''' Created by:  SA 28/06/2011 
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function ReadByRunsGroupNumberNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                 ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tqcResultAlarms " & vbCrLf & _
                                                " WHERE  QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                " AND    QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                                " AND    AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf & _
                                                " ORDER BY RunNumber "

                        Dim myQCResultAlarmsDS As New QCResultAlarms
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myQCResultAlarmsDS.tqcResultAlarms)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myQCResultAlarmsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcResultsDAO.ReadByRunsGroupNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' For the informed QCTestSampleID/QCControlLotID/AnalyzerID, get all Results having alarms, getting also the description
        ''' of each Alarm in the current application Language
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCurrentLanguage">Code of the current Application Language</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultAlarms with all Results with Alarms for the specified
        '''          QCTestSampleID/QCControlLotID/AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  TR 02/06/2011
        ''' Modified by: SA 16/06/2011 - Added parameter for the RunsGroupNumber and used it as filter in both subqueries
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field        '''              
        ''' </remarks>
        Public Function GetAlarmsAndDescriptionsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                    ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer, ByVal pCurrentLanguage As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RA.RunNumber, RA.AlarmID, MR.ResourceText AS AlarmDesc " & vbCrLf & _
                                                " FROM   tqcResultAlarms RA LEFT JOIN tfmwAlarms A ON RA.AlarmID = A.AlarmID " & vbCrLf & _
                                                                          " INNER JOIN tfmwMultiLanguageResources MR ON A.DescResourceID = MR.ResourceID " & vbCrLf & _
                                                " WHERE RA.QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                " AND   RA.QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                                " AND   RA.AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND   RA.RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf & _
                                                " AND   MR.LanguageID      = '" & pCurrentLanguage.Trim & "' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT RA.RunNumber, RA.AlarmID, PMD.FixedItemDesc AS AlarmDesc " & vbCrLf & _
                                                " FROM   tqcResultAlarms RA LEFT JOIN tfmwPreloadedMasterData PMD ON RA.AlarmID = PMD.ItemID " & vbCrLf & _
                                                " WHERE  RA.QCTestSampleID  = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                " AND    RA.QCControlLotID  = " & pQCControlLotID.ToString() & vbCrLf & _
                                                " AND    RA.AnalyzerID      = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND    RA.RunsGroupNumber = " & pRunsGroupNumber.ToString() & vbCrLf & _
                                                " AND    PMD.SubTableID     = '" & GlobalEnumerates.PreloadedMasterDataEnum.QC_MULTIRULES.ToString & "' " & vbCrLf & _
                                                " ORDER BY RA.RunNumber "

                        Dim myQCResultAlarmsDS As New QCResultAlarms
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myQCResultAlarmsDS.tqcResultAlarms)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myQCResultAlarmsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcResultsDAO.GetAlarmsAndDescriptions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

    End Class
End Namespace

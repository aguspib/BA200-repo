Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tqcLastCumulatedValuesDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Create the last cumulated values for an specific QCTestSampleID/QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCumulatedResultDS">Typed DataSet CumulatedResultsDS containing a row with the cumulated data to insert</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 02/06/2011
        ''' Modified by: SA 28/06/2011 - Removed field SumResultsSQRD from the INSERT
        '''              SA 04/06/2012 - Changed the query to insert also new field AnalyzerID 
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If (pCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
                        Dim cmdText As String = " INSERT INTO tqcLastCumulatedValues (QCTestSampleID, QCControlLotID, AnalyzerID, TotalRuns, SumResults, " & vbCrLf & _
                                                                                    " SumSQRDResults, Mean, SD) " & vbCrLf & _
                                                " VALUES (" & pCumulatedResultDS.tqcCumulatedResults(0).QCTestSampleID & ", " & vbCrLf & _
                                                              pCumulatedResultDS.tqcCumulatedResults(0).QCControlLotID & ", " & vbCrLf & _
                                                      " N'" & pCumulatedResultDS.tqcCumulatedResults(0).AnalyzerID.Replace("'", "''").Trim & "', " & vbCrLf & _
                                                              pCumulatedResultDS.tqcCumulatedResults(0).TotalRuns & ", " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumResults) & ", " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumSQRDResults) & ", " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).Mean) & ", " & vbCrLf & _
                                                              ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SD) & ") "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcLastCumulatedValuesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the last cumulated values for a QCTestSampleID/QCControlLotID/AnalyzerID (needed when all the Cumulated Series for them are deleted)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/06/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field 
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                  ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE tqcLastCumulatedValues " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotID & vbCrLf & _
                                            " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tqcLastCumulatedValuesDAO.Delete ", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read the last calculated cumulated values for the specified QCTestSampleID/QCControlLotID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">QC Test/SampleType Identifier</param>
        ''' <param name="pQCControlLotID">QC Control/Lot Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS with the last calculated cumulated values for the 
        '''          specified QCTestSampleID/QCControlLotID/AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  TR 31/05/2011
        ''' Modified by: SA 02/06/2011 - Changed the type of DataSet to return: CumulatedResultsDS instead of LastCumulatedValuesDS
        '''              SA 06/07/2011 - Added a filter to get last cumulate values only when the number of saved Cumulated Series
        '''                              for the specified Test/SampleType is greater than one
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function ReadNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   tqcLastCumulatedValues " & vbCrLf & _
                                                " WHERE  QCTestSampleID =" & pQCTestSampleID.ToString() & vbCrLf & _
                                                " AND    QCControlLotID =" & pQCControlLotID.ToString() & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                " AND   (SELECT COUNT(*) FROM tqcCumulatedResults " & vbCrLf & _
                                                       " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
                                                       " AND    QCControlLotID = " & pQCControlLotID.ToString() & vbCrLf & _
                                                       " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "') > 1 "

                        Dim myLastCumulateValuesDS As New CumulatedResultsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myLastCumulateValuesDS.tqcCumulatedResults)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myLastCumulateValuesDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcLastCumulatedValuesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the last cumulated values for a QCTestSampleID/QCControlLotID/AnalyzerID that are calculated by adding data of the last
        ''' Cumulated Serie to the existing last cumulated values (TotalRuns, SumResults and SumSQRDResults)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCumulatedResultDS">Typed DataSet CumulatedResultsDS containing a row with the cumulated data to add to the 
        '''                                  existing ones to calculate the new values of Mean and SD</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 02/06/2011
        ''' Modified by: SA 04/06/2012 - Changed the query by adding a new filter by field AnalyzerID
        ''' </remarks>
        Public Function UpdateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If (pCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
                        Dim cmdText As String = " UPDATE tqcLastCumulatedValues " & vbCrLf & _
                                                " SET    TotalRuns      = TotalRuns      + " & pCumulatedResultDS.tqcCumulatedResults(0).TotalRuns & ", " & vbCrLf & _
                                                       " SumResults     = SumResults + " & ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumResults) & ", " & vbCrLf & _
                                                       " SumSQRDResults = SumSQRDResults + " & ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumSQRDResults) & vbCrLf & _
                                                " WHERE  QCTestSampleID = " & pCumulatedResultDS.tqcCumulatedResults(0).QCTestSampleID & vbCrLf & _
                                                " AND    QCControlLotID = " & pCumulatedResultDS.tqcCumulatedResults(0).QCControlLotID & vbCrLf & _
                                                " AND    AnalyzerID     = N'" & pCumulatedResultDS.tqcCumulatedResults(0).AnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf


                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcLastCumulatedValuesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the last cumulated values for a QCTestSampleID/QCControlLotID/AnalyzerID that are calculated based in values updated when 
        ''' data of the last Cumulated Serie was added to the existing last cumulated values (SumResultsSQRD, Mean and SD)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 02/06/2011
        ''' Modified by: SA 28/06/2011 - Removed field SumResultsSQRD from the UPDATE
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; changed the query by adding a filter by this field
        ''' </remarks>
        Public Function UpdateCalculatedFieldsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                  ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcLastCumulatedValues " & vbCrLf & _
                                            " SET    Mean           = SumResults/TotalRuns " & ", " & vbCrLf & _
                                                   " SD             = SQRT(((TotalRuns*SumSQRDResults)-POWER(SumResults, 2))/(TotalRuns*(TotalRuns-1))) " & vbCrLf & _
                                            " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString & vbCrLf & _
                                            " AND    QCControlLotID = " & pQCControlLotID.ToString & vbCrLf & _
                                            " AND    AnalyzerID     = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tqcLastCumulatedValuesDAO.UpdateCalculatedFields", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE - OLD METHODS"
        '''' <summary>
        '''' Create the last cumulated values for an specific QCTestSampleID/QCControlLotID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pCumulatedResultDS">Typed DataSet CumulatedResultsDS containing a row with the cumulated data to insert</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 02/06/2011
        '''' Modified by: SA 28/06/2011 - Removed field SumResultsSQRD from the INSERT
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            If (pCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
        '                Dim cmdText As String = " INSERT INTO tqcLastCumulatedValues (QCTestSampleID, QCControlLotID, TotalRuns, SumResults, " & vbCrLf & _
        '                                                                            " SumSQRDResults, Mean, SD) " & vbCrLf & _
        '                                        " VALUES (" & pCumulatedResultDS.tqcCumulatedResults(0).QCTestSampleID & ", " & vbCrLf & _
        '                                                      pCumulatedResultDS.tqcCumulatedResults(0).QCControlLotID & ", " & vbCrLf & _
        '                                                      pCumulatedResultDS.tqcCumulatedResults(0).TotalRuns & ", " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumResults) & ", " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumSQRDResults) & ", " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).Mean) & ", " & vbCrLf & _
        '                                                      ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SD) & ") "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = pDBConnection
        '                dbCmd.CommandText = cmdText

        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, " tqcLastCumulatedValuesDAO.Create ", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Delete the last cumulated values for a QCTestSampleID/QCControlLotID (needed when all the Cumulated Series for them are deleted)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 16/06/2011
        '''' </remarks>
        'Public Function DeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " DELETE tqcLastCumulatedValues " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                    " AND    QCControlLotID = " & pQCControlLotID

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            resultData.HasError = False
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, " tqcLastCumulatedValuesDAO.Delete ", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Read the last calculated cumulated values for the specified QCTestSampleID and QCControlLotID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">QC Test/SampleType Identifier</param>
        '''' <param name="pQCControlLotID">QC Control/Lot Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS with the last calculated
        ''''          cumulated values for the specified QCTestSampleID and QCControlLotID</returns>
        '''' <remarks>
        '''' Created by:  TR 31/05/2011
        '''' Modified by: SA 02/06/2011 - Changed the type of DataSet to return: CumulatedResultsDS instead of LastCumulatedValuesDS
        ''''              SA 06/07/2011 - Added a filter to get last cumulate values only when the number of saved Cumulated Series
        ''''                              for the specified Test/SampleType is greater than one
        '''' </remarks>
        'Public Function ReadOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * " & vbCrLf & _
        '                                        " FROM   tqcLastCumulatedValues " & vbCrLf & _
        '                                        " WHERE  QCTestSampleID =" & pQCTestSampleID.ToString() & vbCrLf & _
        '                                        " AND    QCControlLotID =" & pQCControlLotID.ToString() & vbCrLf & _
        '                                        " AND   (SELECT COUNT(*) FROM tqcCumulatedResults " & vbCrLf & _
        '                                               " WHERE  QCTestSampleID = " & pQCTestSampleID.ToString() & vbCrLf & _
        '                                               " AND    QCControlLotID =" & pQCControlLotID.ToString() & ") > 1 "

        '                Dim myLastCumulateValuesDS As New CumulatedResultsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myLastCumulateValuesDS.tqcCumulatedResults)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myLastCumulateValuesDS
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tqcLastCumulatedValuesDAO.Read", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Update the last cumulated values for a QCTestSampleID/QCControlLotID that are calculated by adding data of the last
        '''' Cumulated Serie to the existing last cumulated values (TotalRuns, SumResults and SumSQRDResults)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pCumulatedResultDS">Typed DataSet CumulatedResultsDS containing a row with the cumulated data to add to the 
        ''''                                  existing ones to calculate the new values of Mean and SD</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 02/06/2011
        '''' </remarks>
        'Public Function UpdateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            If (pCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
        '                Dim cmdText As String = " UPDATE tqcLastCumulatedValues " & vbCrLf & _
        '                                        " SET    TotalRuns      = TotalRuns      + " & pCumulatedResultDS.tqcCumulatedResults(0).TotalRuns & ", " & vbCrLf & _
        '                                               " SumResults     = SumResults + " & ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumResults) & ", " & vbCrLf & _
        '                                               " SumSQRDResults = SumSQRDResults + " & ReplaceNumericString(pCumulatedResultDS.tqcCumulatedResults(0).SumSQRDResults) & vbCrLf & _
        '                                        " WHERE  QCTestSampleID = " & pCumulatedResultDS.tqcCumulatedResults(0).QCTestSampleID & vbCrLf & _
        '                                        " AND    QCControlLotID = " & pCumulatedResultDS.tqcCumulatedResults(0).QCControlLotID

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = pDBConnection
        '                dbCmd.CommandText = cmdText

        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, " tqcLastCumulatedValuesDAO.Update ", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Update the last cumulated values for a QCTestSampleID/QCControlLotID that are calculated based in values updated when 
        '''' data of the last Cumulated Serie was added to the existing last cumulated values (SumResultsSQRD, Mean and SD)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 02/06/2011
        '''' Modified by: SA 28/06/2011 - Removed field SumResultsSQRD from the UPDATE
        '''' </remarks>
        'Public Function UpdateCalculatedFieldsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                       ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tqcLastCumulatedValues " & vbCrLf & _
        '                                    " SET    Mean           = SumResults/TotalRuns " & ", " & vbCrLf & _
        '                                           " SD             = SQRT(((TotalRuns*SumSQRDResults)-POWER(SumResults, 2))/(TotalRuns*(TotalRuns-1))) " & vbCrLf & _
        '                                    " WHERE  QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
        '                                    " AND    QCControlLotID = " & pQCControlLotID

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            resultData.HasError = False
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, " tqcLastCumulatedValuesDAO.UpdateCalculatedFields ", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class
End Namespace

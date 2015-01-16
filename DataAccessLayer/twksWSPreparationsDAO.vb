Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWSPreparationsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Creates one new Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pPreparation">Dataset with structure of table twksWSPreparations</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH  14/04/2010 - Tested: OK
        ''' Modified by: GDS 20/04/2010 - Added AnalizerID field
        '''              SA  25/10/2011 - Added field SendingTime
        '''              SA  02/07/2012 - Changed the function template 
        '''              AG 25/09/2012 - If informed, save also the WellUsed
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparation As WSPreparationsDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim row As WSPreparationsDS.twksWSPreparationsRow = pPreparation.twksWSPreparations(0)
                    'Dim cmdText As String = String.Format(" INSERT INTO twksWSPreparations (PreparationID, WorkSessionID, LAX00Data, PreparationStatus, " & _
                    '                                                                      " AnalyzerID, SendingTime) " & _
                    '                                      " VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}') ", row.PreparationID, row.WorkSessionID, row.LAX00Data, _
                    '                                                                                           row.PreparationStatus, row.AnalyzerID, _
                    '                                                                                           Now.ToString("yyyyMMdd HH:mm:ss"))
                    Dim cmdText As String = ""
                    If row.IsWellUsedNull Then
                        cmdText = String.Format(" INSERT INTO twksWSPreparations (PreparationID, WorkSessionID, LAX00Data, PreparationStatus, " & _
                                                  " AnalyzerID, SendingTime) " & _
                                                  " VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}') ", row.PreparationID, row.WorkSessionID, row.LAX00Data, _
                                                  row.PreparationStatus, row.AnalyzerID, _
                                                  Now.ToString("yyyyMMdd HH:mm:ss"))
                    Else
                        cmdText = String.Format(" INSERT INTO twksWSPreparations (PreparationID, WorkSessionID, LAX00Data, PreparationStatus, " & _
                                                  " AnalyzerID, SendingTime, WellUsed) " & _
                                                  " VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}', {6}) ", row.PreparationID, row.WorkSessionID, row.LAX00Data, _
                                                  row.PreparationStatus, row.AnalyzerID, _
                                                  Now.ToString("yyyyMMdd HH:mm:ss"), row.WellUsed)
                    End If


                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Gets the details of the specified Work Session Preparation 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparationID">Work Session Preparation Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSPreparationsDS with all data of the
        '''          specified Work Session Preparation</returns>
        ''' <remarks>
        ''' Created by:  RH 14/04/2010 
        ''' Modified by: SA 02/07/2012 - Changed the function template
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSPreparations " & vbCrLf & _
                                                " WHERE  PreparationID = " & pPreparationID

                        Dim resultData As New WSPreparationsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSPreparations)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Gets the details of all Work Session Preparations
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSPreparationsDS with all data
        '''           of all Work Session Preparations
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010 
        ''' Modified by: SA 02/07/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSPreparations "

                        Dim resultData As New WSPreparationsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSPreparations)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Updates the Status of the specified Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparationID">Work Session Preparation Identifier</param>
        ''' <param name="pNewPreparationStatus">New Work Session Preparation Status</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010 
        ''' Modified by: SA 02/07/2012 - Changed the function template
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer, _
                                     ByVal pNewPreparationStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Format(" UPDATE twksWSPreparations " & _
                                                          " SET    PreparationStatus = '{0}' " & _
                                                          " WHERE  PreparationID = {1} ", pNewPreparationStatus, pPreparationID)

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.UpdateStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deletes the specified Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010 
        ''' Modified by: SA 02/07/2012 - Changed the function template
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Format(" DELETE FROM twksWSPreparations WHERE PreparationID = {0} ", pPreparationID)

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using

                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Deletes all Work Session Preparations data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010 
        ''' Modified by: SA 02/07/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksWSPreparations "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Generates the next PreparationID for a specific Work Session Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <returns>GlobalDataTO containing the generated PreparationID</returns>
        ''' <remarks>
        ''' Created by: RH 14/04/2010 
        ''' Modified by: SA 02/07/2012 - Changed the function template
        ''' </remarks>
        Public Function GeneratePreparationID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search the last PreparationID
                        Dim cmdText As String = " SELECT MAX(PreparationID) AS NextPreparationID " & vbCrLf & _
                                                " FROM   twksWSPreparations "

                        'Execute the SQL sentence 
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = CInt(dbDataReader.Item("NextPreparationID")) + 1
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.GeneratePreparationID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Preparations for the informed Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' Modified by: SA 02/07/2012 - Changed the function template
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSPreparations " & vbCrLf & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the cuvettes contaminated during recovery results (communications failed during Running)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pMinPrepID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/09/2012 created</remarks>
        Public Function ReadCuvettesContaminatedAfterRecoveryResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMinPrepID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "SELECT p.PreparationID, p.WellUsed , e.ExecutionID , o.testID, c.WashingSolutionR1 ,c.WashingSolutionR2 " & vbCrLf & _
                                  " FROM twksWSPreparations p " & vbCrLf & _
                                  " INNER JOIN twksWSExecutions e ON p.PreparationID = e.PreparationID AND e.ExecutionType = 'PREP_STD' " & vbCrLf & _
                                  " INNER JOIN twksOrderTests o ON e.OrderTestID = o.OrderTestID " & vbCrLf & _
                                  " INNER JOIN tparContaminations c ON o.TestID = c.TestContaminaCuvetteID AND c.ContaminationType = 'CUVETTES' " & vbCrLf & _
                                  " WHERE p.PreparationID >= " & pMinPrepID.ToString

                        Dim myDataSet As New WSPreparationsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSPreparations)
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSPreparationsDAO.ReadCuvettesContaminatedAfterRecoveryResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


#End Region
    End Class
End Namespace

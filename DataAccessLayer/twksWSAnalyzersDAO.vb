Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksWSAnalyzersDAO
        Inherits DAOBase

#Region "CRUD"

        ''' <summary>
        ''' Add an Analyzer to a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSAnalyzersDS">DataSet containing the data needed to add an Analyzer to an existing Work Session</param>
        ''' <returns>GlobalDataTO containing succes/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 10/12/2009 
        ''' Modified by: VR 15/12/2009 
        '''              SA 02/01/2014 -  Implemented the Using Sentence to execute the query
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSAnalyzersDS As WSAnalyzersDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Open Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'SQL Sentence to insert data
                    Dim cmdText As String = ""
                    cmdText = " INSERT INTO twksWSAnalyzers(WorkSessionID, AnalyzerID, WSStatus) " & _
                              " VALUES ('" & pWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID & "', '" _
                                           & pWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID & "','" _
                                           & pWSAnalyzersDS.twksWSAnalyzers(0).WSStatus & "')"

                    'Execute the SQL sentence 
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Read the WSAnalyzer by the index (AnalyzerID, WorkSessionID)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzersDS with all data of the specified Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  TR 12/05/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                             ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT WorkSessionID, AnalyzerID, WSStatus " & _
                                  " FROM   twksWSAnalyzers " & _
                                  " WHERE  AnalyzerID = '" & pAnalyzerID & "'" & _
                                  " AND    WorkSessionID = '" & pWorkSessionID & "'"

                        Dim activeWorkSessionDS As New WSAnalyzersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(activeWorkSessionDS.twksWSAnalyzers)
                            End Using
                        End Using

                        resultData.SetDatos = activeWorkSessionDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Analyzer active for the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession ID </param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzersDS with all data of the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  DL 28/05/2010
        ''' </remarks>
        Public Function ReadByWorkSessionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WorkSessionID, AnalyzerID, WSStatus " & vbCrLf & _
                                                " FROM   twksWSAnalyzers " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' "

                        Dim activeWorkSessionDS As New WSAnalyzersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(activeWorkSessionDS.twksWSAnalyzers)
                            End Using
                        End Using

                        resultData.SetDatos = activeWorkSessionDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.ReadByWorkSessionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change the status of the informed Analyzer WorkSession from the specified current Status to the new one.  
        ''' If the current status of the specified Work Session is different of the indicated, the Status is not changed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pNewStatus">New Work Session Status</param>
        ''' <param name="pCurrentStatus">Current WorkSession Status; optional parameter, when informed it means that
        '''                              the WS Status has to be changed only when it has this current Status</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  SA 26/04/2010 
        ''' Modified by: SA 14/06/2010 - Parameter for current WS Status change to optional to allow reuse the function
        ''' </remarks>
        Public Function UpdateWSStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                       ByVal pNewStatus As String, Optional ByVal pCurrentStatus As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = " UPDATE twksWSAnalyzers " & vbCrLf & _
                                            " SET    WSStatus = '" & pNewStatus.Trim & "' " & vbCrLf & _
                                            " WHERE  AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                    'If the Current Status parameter is informed then following condition is linked to the query
                    If (pCurrentStatus.Trim <> "") Then
                        cmdText &= " AND WSStatus = '" & pCurrentStatus.Trim & "' " & vbCrLf
                    End If

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.UpdateWSStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change the Analyzer identifier of the informed Analyzer WorkSession.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 11/06/2012
        ''' </remarks>
        Public Function UpdateWSAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = " UPDATE twksWSAnalyzers " & vbCrLf & _
                                            " SET    AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                 
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Search if there is an active WorkSession for the specified Analyzer 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzersDS with the list of active WorkSessions
        '''          for the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 28/07/2010 - Changed the query to get also value of the Analyzer Model
        '''              XB 15/06/2012 - Changed pAnalyzerID to an optional parameter
        '''              SA 02/01/2014 - Implemented the Using Sentence to execute the query
        ''' </remarks>
        Public Function GetActiveWSByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TOP 1 WSA.WorkSessionID, WSA.AnalyzerID, WSA.WSStatus, A.AnalyzerModel " & vbCrLf & _
                                                " FROM   twksWSAnalyzers WSA INNER JOIN twksWorkSessions WS ON WSA.WorkSessionID = WS.WorkSessionID " & vbCrLf & _
                                                                           " INNER JOIN tcfgAnalyzers A ON WSA.AnalyzerID = A.AnalyzerID "

                        If (pAnalyzerID.Length > 0) Then cmdText &= " WHERE  WSA.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf
                        cmdText &= " ORDER BY WS.WSDateTime DESC "

                        Dim activeWorkSessionDS As New WSAnalyzersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(activeWorkSessionDS.twksWSAnalyzers)
                            End Using
                        End Using

                        resultData.SetDatos = activeWorkSessionDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.GetActiveWSByAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created by: GDS 21/04/2010
        ''' Modified by: SA 02/01/2014 - Implemented the Using Sentence to execute the query
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) _
                                As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " DELETE twksWSAnalyzers " & _
                              " WHERE  AnalyzerID    = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & _
                              " AND    WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"

                    'Execute the SQL sentence 
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
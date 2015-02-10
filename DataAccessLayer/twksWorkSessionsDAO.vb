Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWorkSessionsDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a new WorkSession in table twksWorkSessions
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSession">Dataset with structure of table twksWorkSessions</param>
        ''' <returns>Global object containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Modified by: SA 02/02/2010 - Added column WorkSessionDesc to the INSERT
        '''              SA 04/11/2010 - Added N preffix for multilanguage of field TS_USer
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSession As WorkSessionsDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO twksWorkSessions (WorkSessionID, WorkSessionDesc, WSDateTime, TS_DateTime, TS_User) " & vbCrLf & _
                                            " VALUES('" & pWorkSession.twksWorkSessions(0).WorkSessionID & "', " & vbCrLf & _
                                                  " N'" & pWorkSession.twksWorkSessions(0).WorkSessionDesc.Replace("'", "''").ToString & "', " & vbCrLf

                    'Value for fields WSDateTime and TS_DateTime
                    If (pWorkSession.twksWorkSessions(0).IsTS_DateTimeNull) Then
                        cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                   " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                    Else
                        cmdText &= " '" & pWorkSession.twksWorkSessions(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                   " '" & pWorkSession.twksWorkSessions(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf
                    End If

                    'Value for field TS_User
                    If (pWorkSession.twksWorkSessions(0).IsTS_UserNull) Then
                        'Dim myGlobalbase As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "') " & vbCrLf
                    Else
                        cmdText &= " N'" & pWorkSession.twksWorkSessions(0).TS_User.Replace("'", "''") & "') " & vbCrLf
                    End If

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWorkSessionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Set current date and time as start date time of the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: 
        ''' </remarks>
        Public Function UpdateStartDateTime(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWorkSessions SET StartDateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf & _
                                            " WHERE WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWorkSessionsDAO.UpdateStartDateTime", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Generate the next WorkSessionID. This ID is generated as current date with format YYYYMMDD plus a two digits sequence number.  
        ''' The method searches if there is a Work Session defined for the same day and in this case it increments in 1 the sequence number; 
        ''' if not, the sequence number is set as 1
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>A String value containing the generated WorkSessionID</returns>
        ''' <remarks></remarks>
        Public Function GenerateWorkSessionID(ByVal pDBConnection As SqlClient.SqlConnection) As String
            Dim nextWSID As String = ""
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim currentDateTime As Date = Now
                        Dim cmdText As String = " SELECT MAX(SequenceNumber) AS NextSequence " & vbCrLf & _
                                                " FROM   twksWorkSessions " & vbCrLf & _
                                                " WHERE  CONVERT(CHAR(10), WSDateTime, 112) = '" & currentDateTime.ToString("yyyyMMdd") & "' " & vbCrLf

                        Dim nextSeq As Integer = 1
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not DBNull.Value.Equals(dbDataReader.Item("NextSequence"))) Then
                                    nextSeq = CInt(dbDataReader.Item("NextSequence")) + 1
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                        
                        'Finally, create the next Work Session ID
                        nextWSID = Format(Now, "yyyyMMdd") & Format(nextSeq, "00")
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWorkSessionsDAO.GenerateWorkSessionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return nextWSID
        End Function

        ''' <summary>
        ''' Get data of the specified WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS WorkSessionsDS with all WorkSession definition data</returns>
        ''' <remarks>
        ''' Created by:  DL 27/05/2010
        ''' Modified by: AG 17/06/2011 - Changed the query to get also new field StartDateTime
        '''              SA 23/02/2012 - Changed the query to get also the Analyzer Identifier; changed the function template
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT WS.*, WSA.AnalyzerID " & vbCrLf & _
                                                " FROM   twksWorkSessions WS INNER JOIN twksWSAnalyzers WSA ON WS.WorkSessionID = WSA.WorkSessionID " & vbCrLf & _
                                                " WHERE  WS.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                        Dim myWorkSessionDS As New WorkSessionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWorkSessionDS.twksWorkSessions)
                            End Using
                        End Using

                        resultData.SetDatos = myWorkSessionDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWorkSessionsDAO.GetByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWorkSessions " & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWorkSessionsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

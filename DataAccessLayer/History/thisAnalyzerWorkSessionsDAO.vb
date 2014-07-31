Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports System.Text
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisAnalyzerWorkSessionsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a new Analyzer WorkSession to the Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionsDS">Typed DS containing data of the WorkSession to move to the Historics Module</param>
        ''' <returns>>GlobalDataTO containing a typed DataSet WorkSessionsDS with the information of the WorkSession created
        '''          in Historic Module</returns>
        ''' <remarks>
        ''' Created by:  RH 23/02/2012
        ''' Modified by: SA 23/02/2012 - Removed the For/Next; function does not return a DS, just return sucess/error information;
        '''                              changed the type of DS received as entry parameter
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionsDS As WorkSessionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Using dbCmd As New SqlClient.SqlCommand
                        Dim row As WorkSessionsDS.twksWorkSessionsRow = pWorkSessionsDS.twksWorkSessions(0)

                        Dim cmdText As New StringBuilder
                        cmdText.AppendLine(" INSERT INTO thisAnalyzerWorkSessions (AnalyzerID, WorkSessionID, StartDateTime) ")
                        cmdText.AppendFormat(" VALUES(N'{0}', N'{1}', '{2}') ", row.AnalyzerID, row.HistWorkSessionID, row.StartDateTime.ToSQLString())

                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText.ToString()

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.SetDatos = pWorkSessionsDS
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisAnalyzerWorkSessionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS WorkSessionsDS with all WorkSession definition data</returns>
        ''' <remarks>
        ''' Created by XB 30/07/2014 - BT #1863
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   thisAnalyzerWorkSessions " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                "   AND  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisAnalyzerWorkSessionsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' If there are WorkSessions saved in Historic Module for the specified Analyzer in the current date, search the last 
        ''' created sequence number (last two digits of field WorkSessionID), add one to it and return that value as next 
        ''' Sequence Number
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an Integer value with the next Sequence Number for the identifier of the WorkSession
        '''          to move to Historic Module</returns>
        ''' <remarks>
        ''' Created by: SA 04/09/2012
        ''' </remarks>
        Public Function GenerateNextSequenceNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT   MAX(CONVERT(int, RIGHT(WorkSessionID,2))) AS LastSequence " & vbCrLf & _
                                                " FROM     thisAnalyzerWorkSessions " & vbCrLf & _
                                                " WHERE    AnalyzerID = N'" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND      SUBSTRING(WorkSessionID,1,8) = '" & Now.ToString("yyyyMMdd") & "' " & _
                                                " GROUP BY SUBSTRING(WorkSessionID,1,8)"

                        Dim dbDataReader As SqlClient.SqlDataReader
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("LastSequence")) + 1
                                End If
                            Else
                                resultData.SetDatos = 1
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
                myLogAcciones.CreateLogActivity(ex.Message, "thisAnalyzerWorkSessionsDAO.GenerateNextSequenceNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Delete the specified AnalyzerID / WorkSessionID
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM thisAnalyzerWorkSessions " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "thisAnalyzerWorkSessionsDAO.Delete", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace

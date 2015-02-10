Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisAdjustBaseLinesDAO
          

#Region "Other Methods"
        ''' <summary>
        ''' Get the identifier of the last Adjustment BaseLine moved to Historic for the specified Analyzer 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the identifier of the last Adjustment BaseLine moved to Historic for
        '''          the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by:  SA 26/09/2012
        ''' </remarks>
        Public Function GetLastHistAdjustBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search the last created Adjustment BL Identifier for the specified Analyzer
                        Dim cmdText As String = " SELECT MAX(HistAdjustBaseLineID) AS LastAdjustBaseLineID " & vbCrLf & _
                                                " FROM   thisAdjustBaseLines " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                        Dim dbDataReader As SqlClient.SqlDataReader
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 0
                                Else
                                    resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("LastAdjustBaseLineID"))
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisAdjustBaseLinesDAO.GetLastHistAdjustBaseLine", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the next Adjust Base Line Identifier (by getting the last generated and adding one to it)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the ID for the next Adjustment BaseLine to create
        '''          in Historic Module for the specified AnalyzerID</returns>
        ''' <remarks>
        ''' Created by: SA 30/08/2012
        ''' </remarks>
        Public Function GenerateNextAdjustBaseLineID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search the last created Adjustment BL Identifier in the specified Analyzer
                        Dim cmdText As String = " SELECT MAX(HistAdjustBaseLineID) AS NextAdjustBaseLineID " & vbCrLf & _
                                                " FROM   thisAdjustBaseLines " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                        Dim dbDataReader As SqlClient.SqlDataReader
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = Convert.ToInt32(dbDataReader.Item("NextAdjustBaseLineID")) + 1
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisAdjustBaseLinesDAO.GenerateNextAdjustBaseLineID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified AnalyzerID and AdjustBaseLineID, get data of all WaveLengths and move all of them to Historic
        ''' Module, using the previously generated next HistAdjustBaseLineID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAdjustBaseLineID">Identifier of the Adjustment BaseLine in WorkSessions Module</param>
        ''' <param name="pHistAdjustBaseLineID">Identifier of the Adjustment BaseLine generated to add it to Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 30/08/2012
        ''' Modified by: SA 17/10/2012 - Changes to inform fields LedPosition and WaveLength: value of field WaveLength from BaseLines table will be the 
        '''                              LedPosition and value of field WaveLength in table of Analyzer Led Positions will be the WaveLentgh 
        ''' </remarks>
        Public Function InsertNewBaseLines(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAdjustBaseLineID As Integer, _
                                           ByVal pHistAdjustBaseLineID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " INSERT INTO thisAdjustBaseLines (AnalyzerID, HistAdjustBaseLineID, LedPosition, WaveLength, " & vbCrLf & _
                                                                             " MainDark, RefDark, BaseLineDateTime) " & vbCrLf & _
                                            " SELECT BL.AnalyzerID, " & pHistAdjustBaseLineID.ToString & " AS HistAdjustBaseLineID, " & vbCrLf & _
                                                   " BL.WaveLength AS LedPosition, LP.WaveLength, BL.MainDark, BL.RefDark, BL.[DateTime] AS BaseLineDateTime " & vbCrLf & _
                                            " FROM   twksWSBLines BL INNER JOIN tcfgAnalyzerLedPositions LP ON BL.AnalyzerID = LP.AnalyzerID " & vbCrLf & _
                                                                                                         " AND BL.WaveLength = LP.LedPosition " & vbCrLf & _
                                            " WHERE  BL.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    BL.BaseLineID = " & pAdjustBaseLineID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisAdjustBaseLinesDAO.InsertNewBaseLines", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' For the specified Analyzer, delete all Adjust Base Lines not linked to any Execution
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function DeleteNotInUseAdjustBL(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM thisAdjustBaseLines " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                    " AND    HistAdjustBaseLineID NOT IN (SELECT HistAdjustBaseLineID FROM thisWSExecutions " & vbCrLf & _
        '                                                                        " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "') " & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisAdjustBaseLinesDAO.DeleteNotInUseAdjustBL", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class
End Namespace
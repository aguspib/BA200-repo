Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    'AG 14/05/2010 - change name from twksWSBaseLinesDAO to twksWSBLinesDAO
    'Modify all query using new table name!!!
    Public Class twksWSBLinesDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Create an adjustment BaseLine
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBaseLinesDS">Typed DataSet BaseLinesDS containing all data of the adjustment BaseLine to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/05/2010
        ''' Modified by: AG 29/04/2011 - WorkSessionID is removed from table twksWSBLines
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pBaseLinesDS As BaseLinesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim cmd As SqlCommand

                    cmd = pDBConnection.CreateCommand

                    For Each myRow As BaseLinesDS.twksWSBaseLinesRow In pBaseLinesDS.twksWSBaseLines.Rows
                        cmdText = "INSERT INTO twksWSBLines" & vbCrLf & _
                              "  ( AnalyzerID" & vbCrLf & _
                              "  , BaseLineID" & vbCrLf & _
                              "  , Wavelength" & vbCrLf & _
                              "  , WellUsed" & vbCrLf & _
                              "  , MainLight" & vbCrLf & _
                              "  , RefLight" & vbCrLf & _
                              "  , MainDark" & vbCrLf & _
                              "  , RefDark" & vbCrLf & _
                              "  , IT" & vbCrLf & _
                              "  , DAC" & vbCrLf & _
                              "  , DateTime) " & vbCrLf & _
                              "VALUES" & vbCrLf & _
                              "  ( '" & myRow.AnalyzerID.Replace("'", "''").ToString & "'" & vbCrLf & _
                              "  ,  " & myRow.BaseLineID.ToString & vbCrLf & _
                              "  ,  " & myRow.Wavelength.ToString & vbCrLf & _
                              "  ,  " & myRow.WellUsed.ToString & vbCrLf & _
                              "  ,  " & myRow.MainLight.ToString & vbCrLf & _
                              "  ,  " & myRow.RefLight.ToString & vbCrLf & _
                              "  ,  " & myRow.MainDark.ToString & vbCrLf & _
                              "  ,  " & myRow.RefDark.ToString & vbCrLf & _
                              "  ,  " & myRow.IT.ToString & vbCrLf & _
                              "  ,  " & myRow.DAC.ToString & vbCrLf & _
                              "  , '" & myRow.DateTime.ToString("yyyyMMdd HH:mm:ss") & "')"

                        cmd.CommandText = cmdText
                        resultData.AffectedRecords = cmd.ExecuteNonQuery()
                    Next
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Return all Base Lines data for the specified Wave Length (light counts are get from twksWSBLinesByWell and dark counts 
        ''' are get from twksWSBLines)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAdjustBaseLineID">Identifier of an adjustment BaseLine</param>
        ''' <param name="pWaveLength">Wave length</param>
        ''' <param name="pWellBaseLineID">Identifier of a BaseLine by Well</param>
        ''' <param name="pWellUsed">Rotor Well Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with all data of the specified BaseLine/WaveLength</returns>
        ''' <remarks>
        ''' Created by:  DL 02/06/2010
        ''' Modified by: AG 04/01/2011
        '''              AG 29/04/2011 - WorkSessionID is removed from table twksWSBLines
        ''' </remarks>
        Public Function GetByWaveLength(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                        ByVal pAdjustBaseLineID As Integer, ByVal pWaveLength As Integer, ByVal pWellBaseLineID As Integer, _
                                        ByVal pWellUsed As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'cmdText &= " SELECT BLW.BaseLineID, BL.BaseLineID As AdjustBaseLineID, BL.Wavelength, BLW.MainLight , BLW.RefLight, BL.MainDark , BL.RefDark, BL.IT, " & vbCrLf
                        'cmdText &= " BL.DAC, BLW.DateTime, BLW.WellUsed FROM twksWSBLines BL INNER JOIN twksWSBLinesByWell BLW ON " & vbCrLf
                        'cmdText &= " BL.AnalyzerID = BLW.AnalyzerID AND BL.WorkSessionID = BLW.WorkSessionID  AND BL.Wavelength = BLW.Wavelenght " & vbCrLf
                        'cmdText &= " WHERE BL.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf
                        'cmdText &= " AND BL.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'" & vbCrLf
                        'cmdText &= " AND BL.Wavelength  = " & pWaveLength & vbCrLf
                        'cmdText &= " AND BLW.WellUsed = " & pWellUsed & vbCrLf
                        'cmdText &= " AND  BLW.BaseLineID = " & pWellBaseLineID & vbCrLf
                        'cmdText &= " AND  BL.BaseLineID = " & pAdjustBaseLineID & vbCrLf
                        'cmdText &= " ORDER BY BLW.BaseLineID, BL.Wavelength "

                        cmdText &= " SELECT BLW.BaseLineID, BL.BaseLineID As AdjustBaseLineID, BL.Wavelength, BLW.MainLight , BLW.RefLight, BL.MainDark , BL.RefDark, BL.IT, " & vbCrLf
                        cmdText &= "        BL.DAC, BLW.DateTime, BLW.WellUsed " & vbCrLf
                        cmdText &= " FROM   twksWSBLines BL INNER JOIN twksWSBLinesByWell BLW ON BL.AnalyzerID = BLW.AnalyzerID AND BL.Wavelength = BLW.Wavelenght " & vbCrLf
                        cmdText &= " WHERE  BL.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf
                        cmdText &= " AND    BLW.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf
                        cmdText &= " AND    BL.Wavelength  = " & pWaveLength.ToString & vbCrLf
                        cmdText &= " AND    BLW.WellUsed = " & pWellUsed.ToString & vbCrLf
                        cmdText &= " AND    BLW.BaseLineID = " & pWellBaseLineID.ToString & vbCrLf
                        cmdText &= " AND    BL.BaseLineID = " & pAdjustBaseLineID.ToString & vbCrLf
                        cmdText &= " ORDER BY BLW.BaseLineID, BL.Wavelength "

                        Dim myBaseLinesDS As New BaseLinesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBaseLinesDS.twksWSBaseLines)
                            End Using
                        End Using

                        resultData.SetDatos = myBaseLinesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.GetByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get values for all WaveLengths of all adjustment BaseLines for the Executions of the informed Analyzer and WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with all data of all adjustment BaseLines for the Executions
        '''          of the informed Analyzer and WorkSession</returns>
        ''' <remarks>
        ''' Created by:  DL 19/02/2010
        ''' Modified by: AG 29/04/2011 - WorkSessionID is removed from table twksWSBLines
        '''              AG 06/07/2011 - Changed the query
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'Query 1st version
                        'cmdText &= " SELECT   AnalyzerID, WorkSessionID, BaseLineID, Wavelength, WellUsed, MainLight, MainDark,  " & vbCrLf
                        'cmdText &= "          RefLight, RefDark, IT, DAC, DateTime" & vbCrLf
                        'cmdText &= " FROM     twksWSBLines" & vbCrLf
                        'cmdText &= " WHERE    AnalyzerID    = '" & pAnalyzerID.Trim & "'" & vbCrLf
                        'cmdText &= "   AND    WorkSessionID = '" & pWorkSessionID.Trim & "'" & vbCrLf
                        'cmdText &= " ORDER BY BaselineID, Wavelength"

                        'Query 2on version
                        'cmdText &= " SELECT   DISTINCT BL.BaseLineID, BL.AnalyzerID, EX.WorkSessionID, BL.Wavelength, BL.WellUsed, BL.MainLight, BL.MainDark,  " & vbCrLf
                        'cmdText &= "          BL.RefLight, BL.RefDark, BL.IT, BL.DAC, BL.DateTime" & vbCrLf
                        'cmdText &= " FROM     twksWSExecutions EX INNER JOIN twksWSBLines BL " & vbCrLf
                        'cmdText &= " ON EX.AnalyzerID = BL.AnalyzerID AND EX.AdjustBaseLineID = BL.BaseLineID " & vbCrLf
                        'cmdText &= " WHERE    EX.AnalyzerID    = '" & pAnalyzerID.Trim & "'" & vbCrLf
                        'cmdText &= "   AND    EX.WorkSessionID = '" & pWorkSessionID.Trim & "'" & vbCrLf
                        'cmdText &= " ORDER BY BaselineID, Wavelength"

                        cmdText &= " SELECT DISTINCT BL.BaseLineID, BL.AnalyzerID, EX.WorkSessionID, BL.Wavelength, BL.WellUsed, BL.MainLight, BL.MainDark,  " & vbCrLf
                        cmdText &= "                 BL.RefLight, BL.RefDark, BL.IT, BL.DAC, BL.DateTime " & vbCrLf
                        cmdText &= " FROM   twksWSExecutions EX INNER JOIN twksWSBLines BL ON EX.AnalyzerID = BL.AnalyzerID " & vbCrLf
                        cmdText &= " WHERE  EX.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf
                        cmdText &= " AND    EX.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf
                        cmdText &= " ORDER BY BaselineID, Wavelength "

                        Dim myBaseLinesDS As New BaseLinesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBaseLinesDS.twksWSBaseLines)
                            End Using
                        End Using

                        resultData.SetDatos = myBaseLinesDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.GetByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get values for all WaveLengths for the specified adjustment Base Line
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">NOT USED!!</param>
        ''' <param name="pBaseLineID">Identifier of the adjustment Base Line</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with all data for the informed BaseLine</returns>
        ''' <remarks>
        ''' Created by:  DL 19/02/2010
        ''' Modified by: AG 13/05/2010 - Changed the method name to READ and some field names 
        '''              AG 14/05/2010 - Added ORDER BY clause
        '''              AG 20/05/2010 - Added parameters AnalyzerID and WorkSessionID
        '''              AG 29/04/2011 - WorkSessionID is removed from table twksWSBLines
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                             ByVal pBaseLineID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AnalyzerID, BaseLineID, Wavelength, WellUsed, MainLight, MainDark, " & vbCrLf & _
                                                       " RefLight, RefDark, IT, DAC, DateTime " & vbCrLf & _
                                                " FROM   twksWSBLines " & vbCrLf & _
                                                " WHERE  BaseLineID = " & pBaseLineID.ToString & vbCrLf & _
                                                " AND    AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " ORDER BY Wavelength " & vbCrLf

                        Dim myBaseLinesDS As New BaseLinesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBaseLinesDS.twksWSBaseLines)
                            End Using
                        End Using
                        
                        resultData.SetDatos = myBaseLinesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified adjustment Base Line
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBaseLinesDS">Typed DataSet BaseLinesDS containing all data of the adjustment BaseLine to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/05/2010
        ''' Modified by: AG 29/04/2011 - WorkSessionID is removed from table twksWSBLines
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pBaseLinesDS As BaseLinesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    Dim cmd As SqlCommand
                    cmd = pDBConnection.CreateCommand

                    For Each myRow As BaseLinesDS.twksWSBaseLinesRow In pBaseLinesDS.twksWSBaseLines.Rows
                        cmdText = " UPDATE twksWSBLines SET "

                        If Not myRow.IsWellUsedNull Then
                            cmdText &= vbCrLf & "    WellUsed  =  " & myRow.WellUsed.ToString & ","
                        End If

                        If Not myRow.IsMainLightNull Then
                            cmdText &= vbCrLf & "    MainLight =  " & myRow.MainLight.ToString & ","
                        End If

                        If Not myRow.IsRefLightNull Then
                            cmdText &= vbCrLf & "    RefLight  =  " & myRow.RefLight.ToString & ","
                        End If

                        If Not myRow.IsMainDarkNull Then
                            cmdText &= vbCrLf & "    MainDark  =  " & myRow.MainDark.ToString & ","
                        End If

                        If Not myRow.IsRefDarkNull Then
                            cmdText &= vbCrLf & "    RefDark   =  " & myRow.RefDark.ToString & ","
                        End If

                        If Not myRow.IsITNull Then
                            cmdText &= vbCrLf & "    IT        =  " & myRow.IT.ToString & ","
                        End If

                        If Not myRow.IsDACNull Then
                            cmdText &= vbCrLf & "    DAC       =  " & myRow.DAC.ToString & ","
                        End If

                        If Not myRow.IsDateTimeNull Then
                            cmdText &= vbCrLf & "    DateTime  = '" & myRow.DateTime.ToString("yyyyMMdd HH:mm:ss") & "',"
                        End If

                        If cmdText.Chars(cmdText.Length - 1) = "," Then
                            cmdText = cmdText.Substring(0, cmdText.Length - 1) & vbCrLf & _
                                      "  WHERE AnalyzerID    = '" & myRow.AnalyzerID.Replace("'", "''").ToString & "'" & vbCrLf & _
                                      "    AND BaseLineID    =  " & myRow.BaseLineID.ToString & vbCrLf & _
                                      "    AND Wavelength    =  " & myRow.Wavelength.ToString

                            cmd.CommandText = cmdText
                            resultData.AffectedRecords = cmd.ExecuteNonQuery()
                        End If
                    Next
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Others Methods"
        ''' <summary>
        ''' Get all adjustment BaseLines for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS</returns>
        ''' <remarks>
        ''' Created by: AG 28/07/2011 - Used when the results EXCEL file is generated 
        ''' </remarks>
        Public Function GetByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AnalyzerID, BaseLineID, Wavelength, WellUsed, MainLight, MainDark, " & vbCrLf & _
                                                       " RefLight, RefDark, IT, DAC, DateTime " & vbCrLf & _
                                                " FROM   twksWSBLines " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " ORDER BY BaselineID, Wavelength "

                        Dim myBaseLinesDS As New BaseLinesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBaseLinesDS.twksWSBaseLines)
                            End Using
                        End Using

                        resultData.SetDatos = myBaseLinesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.GetByAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the identifier of the last executed adjustment BaseLine for the specified Analyzer (to inform this value for every WS Execution)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">NOT USED!!</param>
        ''' <returns>GlobalDataTO containing an integer value with the adjustment BaseLine Identifier</returns>
        ''' <remarks>
        ''' Created by:  AG 17/05/2010 
        ''' Modified by: AG 29/04/2011 - WorkSessionID is removed from table twksWSBLines
        ''' </remarks>
        Public Function GetCurrentBaseLineID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(BaseLineID) AS CurrentBaseLineID FROM twksWSBLines " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 0
                                Else
                                    resultData.SetDatos = CInt(dbDataReader.Item("CurrentBaseLineID"))
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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.GetCurrentBaseLineID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all WaveLengths for the last executed adjustment BaseLine for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with all WaveLengths for the last executed adjustment BaseLine for 
        '''          the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by:  AG 04/05/2011
        ''' Modified by: AG 06/09/2012 - Changed the query to assure the results returned belongs to the informed AnalyzerID. Old query failed when 
        '''                              several there were Base Lines for several Analyzers in the table
        ''' </remarks>
        Public Function GetCurrentBaseLineValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSBLines " & vbCrLf & _
                                                " WHERE BaseLineID IN (SELECT MAX(BaseLineID) FROM twksWSBLines " & vbCrLf & _
                                                                    "  WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "') " & vbCrLf

                        'AG 06/09/2012 - Get the adjust light results for the specified Analyzer!!!!
                        cmdText &= " AND AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' "
                        cmdText &= " ORDER BY Wavelength ASC "

                        Dim myBaseLinesDS As New BaseLinesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBaseLinesDS.twksWSBaseLines)
                            End Using
                        End Using

                        resultData.SetDatos = myBaseLinesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.GetCurrentBaseLineValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Return all Base Lines data needed for calculations (light counts are get from twksWSBLinesByWell and dark counts are get from twksWSBLines)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pBaseLineWellID">Identifier of a BaseLine by Well</param>
        ''' <param name="pWell">Rotor Well Number</param>
        ''' <param name="pBaseLineAdjustID">Identifier of an adjustment Base Line</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLineDS</returns>
        ''' <remarks>
        ''' Created by:  AG 04/01/2011
        ''' Modified by: AG 29/04/2011 - WorkSessionID was removed from table twksWSBLines; query changed
        ''' </remarks>
        Public Function ReadValuesForCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                  ByVal pBaseLineWellID As Integer, ByVal pWell As Integer, ByVal pBaseLineAdjustID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'cmdText = "SELECT BL.Wavelength , BLW.MainLight , BLW.RefLight, BL.MainDark , BL.RefDark, BL.IT, BL.DAC " & vbCrLf & _
                        '          " , BLW.DateTime, BLW.WellUsed FROM twksWSBLines BL INNER JOIN twksWSBLinesByWell BLW ON " & vbCrLf & _
                        '          " BL.AnalyzerID = BLW.AnalyzerID AND BL.WorkSessionID = BLW.WorkSessionID  AND BL.Wavelength = BLW.Wavelenght " & vbCrLf & _
                        '          " WHERE BL.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                        '          " AND BL.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                        '          " AND BL.BaseLineID = " & pBaseLineAdjustID & vbCrLf & _
                        '          " AND BLW.BaseLineID = " & pBaseLineWellID & vbCrLf & _
                        '          " AND BLW.WellUsed = " & pWell & vbCrLf & _
                        '          " ORDER BY BL.Wavelength"
                        cmdText = " SELECT BL.Wavelength , BLW.MainLight , BLW.RefLight, BL.MainDark , BL.RefDark, BL.IT, BL.DAC, " & vbCrLf & _
                                         " BLW.DateTime, BLW.WellUsed " & vbCrLf & _
                                  " FROM   twksWSBLines BL INNER JOIN twksWSBLinesByWell BLW ON BL.AnalyzerID = BLW.AnalyzerID " & vbCrLf & _
                                                                                          " AND BL.Wavelength = BLW.Wavelenght " & vbCrLf & _
                                  " WHERE BL.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                  " AND   BLW.WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                  " AND   BL.BaseLineID = " & pBaseLineAdjustID.ToString & vbCrLf & _
                                  " AND   BLW.BaseLineID = " & pBaseLineWellID.ToString & vbCrLf & _
                                  " AND   BLW.WellUsed = " & pWell.ToString & vbCrLf & _
                                  " ORDER BY BL.Wavelength "

                        Dim myBaseLinesDS As New BaseLinesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBaseLinesDS.twksWSBaseLines)
                            End Using
                        End Using

                        resultData.SetDatos = myBaseLinesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.ReadValuesForCalculations", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete of adjustment BaseLines for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' Modified by: AG 29/04/2011 - Due to field WorkSessionID was removed from table twksWSBLines, method is renamed from ResetWS to 
        '''                              ResetAdjustsBLines, parameter pWorkSessionID is also removed, and the query is changed to remove the filter
        ''' </remarks>
        Public Function ResetAdjustsBLines(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSBLines WHERE AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.SetDatos = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.ResetAdjustsBLines", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For all WaveLengths of the informed Analyzer/Adjustment Base Line, set field MovedToHistoric = TRUE to indicate the
        ''' adjustment BaseLine has been exported to Historic Module
        ''' </summary>
        '''<param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAdjustBaseLineID">Identifier of the adjustment Base Line</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 26/09/2012
        ''' </remarks>
        Public Function UpdateMovedToHistoric(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAdjustBaseLineID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSBLines SET MovedToHistoric = 1 " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    BaseLineID = " & pAdjustBaseLineID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.SetDatos = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.UpdateMovedToHistoric", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the adjustment Base Line has been already moved to Historic Module (if field MovedToHistoric is TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAdjustBaseLineID">Identifier of the adjustment Base Line</param>
        ''' <returns>GlobalDataTO containing an integer value indicating if the adjustment Base Line has been moved to Historic Module</returns>
        ''' <remarks>
        ''' Created by: SA 26/09/2012
        ''' </remarks>
        Public Function VerifyBLMovedToHistoric(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pAdjustBaseLineID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TOP 1 MovedToHistoric " & vbCrLf & _
                                                " FROM   twksWSBLines " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    BaseLineID = " & pAdjustBaseLineID.ToString & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            resultData.SetDatos = 0
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    If (Convert.ToBoolean(dbDataReader.Item("MovedToHistoric"))) Then
                                        resultData.SetDatos = 1
                                    End If
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesDAO.VerifyBLMovedToHistoric", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

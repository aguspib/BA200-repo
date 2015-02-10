Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    'AG 14/05/2010 - creation
    Public Class twksWSBLinesByWellDAO
          

#Region "CRUD"

        ''' <summary>
        ''' Create a group of BaseLines by Well
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBaseLinesDS">Typed DataSet BaseLinesDS containing all BaseLines to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/05/2010
        ''' Modified by: AG 02/05/2011 - Replaced fields MainDark and RefDark for ABSValue and IsMean
        '''              IT 03/11/2014 - BA-2067: Dynamic BaseLine Type   
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pBaseLinesDS As BaseLinesDS) As GlobalDataTO
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
                        cmdText = "INSERT INTO twksWSBLinesByWell" & vbCrLf & _
                                  "  ( AnalyzerID" & vbCrLf & _
                                  "  , WorkSessionID" & vbCrLf & _
                                  "  , BaseLineID" & vbCrLf & _
                                  "  , Wavelenght" & vbCrLf & _
                                  "  , WellUsed" & vbCrLf & _
                                  "  , Type" & vbCrLf



                        If Not myRow.IsMainLightNull Then
                            cmdText &= "  , MainLight" & vbCrLf
                        End If

                        If Not myRow.IsRefLightNull Then
                            cmdText &= "  , RefLight" & vbCrLf
                        End If

                        If Not myRow.IsABSvalueNull Then
                            cmdText &= "  , ABSvalue" & vbCrLf
                        End If

                        If Not myRow.IsIsMeanNull Then
                            cmdText &= "  , IsMean" & vbCrLf
                        End If

                        If Not myRow.IsDateTimeNull Then
                            cmdText &= "  , DateTime" & vbCrLf
                        End If

                        cmdText &= "  )" & vbCrLf
                        cmdText &= "VALUES" & vbCrLf & _
                                   "  ( '" & myRow.AnalyzerID.Replace("'", "''").ToString & "'" & vbCrLf & _
                                   "  , '" & myRow.WorkSessionID.Replace("'", "''").ToString & "'" & vbCrLf & _
                                   "  ,  " & myRow.BaseLineID.ToString & vbCrLf & _
                                   "  ,  " & myRow.Wavelength.ToString & vbCrLf & _
                                   "  ,  " & myRow.WellUsed.ToString & vbCrLf & _
                                   "  ,  '" & myRow.Type & "'" & vbCrLf

                        If Not myRow.IsMainLightNull Then
                            cmdText &= "  ,  " & myRow.MainLight.ToString & vbCrLf
                        End If

                        If Not myRow.IsRefLightNull Then
                            cmdText &= "  ,  " & myRow.RefLight.ToString & vbCrLf
                        End If

                        If Not myRow.IsABSvalueNull Then
                            cmdText &= "  ,  " & ReplaceNumericString(myRow.ABSvalue) & vbCrLf

                        End If

                        If Not myRow.IsIsMeanNull Then
                            cmdText &= "  ,  " & CStr(IIf(myRow.IsMean, 1, 0)) & vbCrLf
                        End If

                        If Not myRow.IsDateTimeNull Then
                            cmdText &= "  , '" & myRow.DateTime.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                        End If

                        cmdText &= "  )"

                        cmd.CommandText = cmdText
                        resultData.AffectedRecords = cmd.ExecuteNonQuery()
                    Next
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of all WaveLengths for the specified BaseLine by Well 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBaseLineID">Identifier of the Base Line</param>
        ''' <param name="pWell">Well Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with the group of BaseLines by Well</returns>
        ''' <remarks>
        ''' Created by:  AG 14/05/2010 
        ''' Modified by: AG 20/05/2010 - Added parameters AnalyzerID, WorkSessionID and WellNumber
        '''              AG 02/05/2011 - Replaced fields MainDark and RefDark for ABSValue and IsMean
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                             ByVal pBaseLineID As Integer, ByVal pWell As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AnalyzerID, WorkSessionID, BaseLineID, Wavelenght AS Wavelength, WellUsed, MainLight, " & vbCrLf & _
                                                       " RefLight, ABSvalue, IsMean, DateTime, Type " & vbCrLf & _
                                                " FROM   twksWSBLinesByWell" & vbCrLf & _
                                                " WHERE  BaseLineID    = " & pBaseLineID.ToString & vbCrLf & _
                                                " AND    AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WellUsed      = " & pWell.ToString & vbCrLf & _
                                                " ORDER BY Wavelenght " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update values of a group of BaseLines by Well
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBaseLinesDS">Typed DataSet BaseLinesDS containing all BaseLines to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/05/2010
        ''' Modified by: AG 02/05/2011 - Replaced fields MainDark and RefDark for ABSValue and IsMean
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pBaseLinesDS As BaseLinesDS) As GlobalDataTO
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
                        cmdText = "UPDATE twksWSBLinesByWell SET"

                        If Not myRow.IsMainLightNull Then
                            cmdText &= vbCrLf & "    MainLight =  " & myRow.MainLight.ToString & ","
                        End If

                        If Not myRow.IsRefLightNull Then
                            cmdText &= vbCrLf & "    RefLight  =  " & myRow.RefLight.ToString & ","
                        End If

                        If Not myRow.IsABSvalueNull Then
                            cmdText &= vbCrLf & "    ABSvalue  =  " & ReplaceNumericString(myRow.ABSvalue) & ","
                        End If

                        If Not myRow.IsIsMeanNull Then
                            cmdText &= vbCrLf & "    IsMean   =  " & CStr(IIf(myRow.IsMean, 1, 0)) & ","
                        End If

                        If Not myRow.IsDateTimeNull Then
                            cmdText &= vbCrLf & "    DateTime  = '" & myRow.DateTime.ToString("yyyyMMdd HH:mm:ss") & "',"
                        End If

                        If cmdText.Chars(cmdText.Length - 1) = "," Then
                            cmdText = cmdText.Substring(0, cmdText.Length - 1) & vbCrLf & _
                                      "  WHERE AnalyzerID    = '" & myRow.AnalyzerID.Replace("'", "''").ToString & "'" & vbCrLf & _
                                      "    AND WorkSessionID = '" & myRow.WorkSessionID.Replace("'", "''").ToString & "'" & vbCrLf & _
                                      "    AND BaseLineID    =  " & myRow.BaseLineID.ToString & vbCrLf & _
                                      "    AND Wavelenght    =  " & myRow.Wavelength.ToString & vbCrLf & _
                                      "    AND WellUsed      =  " & myRow.WellUsed.ToString

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get all Base Lines by Well used for an specific WorkSessionID and AnalyzerID
        ''' </summary>
        '''  <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with the group of BaseLines by Well</returns>
        ''' <remarks>
        ''' Created by:  DL 19/02/2010
        ''' Modified by: AG 04/01/2011 - Order by WellUsed and BaseLineID instead of by BaseLineID
        '''              AG 02/05/2011 - Replaced fields MainDark and RefDark for ABSValue and IsMean
        '''              IT 03/11/2014 - BA-2067: Added the Column Type
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pBaseLineType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT   AnalyzerID, WorkSessionID, BaseLineID, WaveLenght as [WaveLength], WellUsed, MainLight, RefLight, ABSvalue, IsMean, DateTime, Type " & vbCrLf
                        cmdText &= " FROM     twksWSBLinesByWell" & vbCrLf
                        cmdText &= " WHERE    AnalyzerID    = N'" & pAnalyzerID.Trim & "'" & vbCrLf
                        cmdText &= "   AND    WorkSessionID = N'" & pWorkSessionID.Trim & "'" & vbCrLf
                        cmdText &= "   AND    Type = '" & pBaseLineType.Trim & "' " & vbCrLf
                        cmdText &= " ORDER BY WellUsed, BaselineID, WaveLength"

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the ID of the last created BaseLine for the specified Analyzer, WorkSession and WellNumber in order to inform 
        ''' every WSExecution with his correct BaseLine
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWell">Well Number</param>
        ''' <returns>GlobalDataTO containing and integer value with the ID of the last created BaseLine for the specified 
        '''          Analyzer, WorkSession and WellNumber</returns>
        ''' <remarks>
        ''' Created by: AG 17/05/2010 
        ''' Modified by: TR 27/05/2012 - Added filter by WellNumber; added quotations marks for fields AnalyzerID and WorkSessionID
        '''                            - If a BaseLine is not found, returns 0 instead of 1
        ''' </remarks>
        Public Function GetCurrentBaseLineID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                             ByVal pWorkSessionID As String, ByVal pWell As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT MAX(BaseLineID) AS CurrentBaseLineID "
                        cmdText += " FROM   twksWSBLinesByWell "
                        cmdText += " WHERE  AnalyzerID = N'" & pAnalyzerID & "' "
                        cmdText += " AND WorkSessionID = N'" & pWorkSessionID & "' "
                        cmdText += " AND WellUsed = " & pWell

                        'Dont repeat the BaseLineId in different wells
                        'cmdText += " AND WellUsed = " & pWell

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.GetCurrentBaseLineID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search the last Well received for an Analyzer and WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing and integer value with the number of the last Well received for specified 
        '''          Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  AG 31/07/2012
        ''' </remarks>
        Public Function GetLastWellBaseLineReceived(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                    ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT TOP 1 WellUsed FROM twksWSBLinesByWell "
                        cmdText &= " WHERE AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").ToString & "'"
                        cmdText &= " AND WorkSessionID = N'" & pWorkSessionID.Replace("'", "''").ToString & "'"
                        cmdText &= " ORDER BY DateTime DESC"

                        Dim myDataSet As New BaseLinesDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBaseLines)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.GetLastWellBaseLineReceived", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all BaseLines by Well for the specified Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with the group of BaseLines by Well</returns>
        ''' <remarks>
        ''' Created by:  AG 04/05/2011
        ''' Modify by: AG 18/11/2014 - BA-2065: exclude worksession from the query
        '''            IT 03/11/2014 - BA-2067: Added the Column Type
        ''' </remarks>
        Public Function GetMeanWellBaseLineValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "SELECT AnalyzerID, WorkSessionID, BaseLineID, WaveLenght as [WaveLength], WellUsed, MainLight, RefLight, ABSvalue, IsMean, DateTime, Type " & vbCrLf & _
                                  " FROM twksWSBLinesByWell  WHERE IsMean = 1 " & vbCrLf & _
                                  " AND AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                  " ORDER BY BaseLineID, WellUsed , Wavelenght "

                        'AG 18/11/2014 BA-2065 removed after AND AnalyzerID ...
                        '" AND WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'" & vbCrLf & _

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.GetMeanWellBaseLineValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all BaseLines by Well for the specified Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 14/05/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSBLinesByWell " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID = N'" & pWorkSessionID.Trim.Replace("'", "''") & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field isMean
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBaseLineID">BaseLine Identifier</param>
        ''' <param name="pIsMeanValue">Value to set to field isMean</param>
        ''' <param name="pWellsString">List of Well Numbers divided by comma character</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 04/05/2011
        ''' Modified by: AG 15/02/2012 - Changed the where conditions to update properly the IsMean column when some Wells start a new rotor turn 
        '''                              but not these who has to update the IsMean column (NOTE the pBaseLineID value is always the current (new) 
        '''                              rotor turn) 
        ''' </remarks>
        Public Function UpdateIsMean(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                     ByVal pBaseLineID As Integer, ByVal pIsMeanValue As Boolean, ByVal pWellsString As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= "UPDATE twksWSBLinesByWell SET IsMean = " & CInt(IIf(pIsMeanValue, 1, 0)).ToString
                    cmdText &= " WHERE WellUsed IN (" & pWellsString & " )"
                    cmdText &= " AND AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").ToString & "'"
                    cmdText &= " AND WorkSessionID = N'" & pWorkSessionID.Replace("'", "''").ToString & "'"

                    'AG 15/02/2012
                    'cmdText &= " AND BaseLineID = " & pBaseLineID
                    If (Not pIsMeanValue) Then 'Update IsMean to False
                        cmdText &= " AND BaseLineID <= " & pBaseLineID
                    Else 'Update IsMean to True
                        cmdText &= " AND BaseLineID = " & pBaseLineID
                    End If
                    'AG 15/02/2012

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.UpdateIsMean", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the last information (max baselineID) for each well
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pType">DYNAMIC - STATIC - "" ALL</param>
        ''' <returns></returns>
        ''' <remarks>AG 17/11/2014 BA-2065
        ''' AG 21/11/2014 BA-2065 add paramter pType</remarks>
        Public Function GetAllWellsLastTurn(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT W1.wellused, MAX(W1.baselineID) AS BaselineID FROM twksWSBLinesByWell W1 " & vbCrLf
                        cmdText &= " INNER JOIN twksWSBLinesByWell W2 ON W1.WellUsed = W2.WellUsed " & vbCrLf
                        cmdText &= " WHERE W1.AnalyzerID = N'" & pAnalyzerID.Replace("'", "''").ToString & "'" & vbCrLf
                        cmdText &= " AND W1.WorksessionID = N'" & pWorkSessionID.Replace("'", "''").ToString & "'" & vbCrLf

                        If pType <> "" Then
                            cmdText &= " AND W1.Type = N'" & pType.Replace("'", "''").ToString & "'" & vbCrLf
                        End If

                        cmdText &= " GROUP BY W1.WellUsed ORDER BY WellUsed ASC "

                        Dim myDataSet As New BaseLinesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBaseLines)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksWSBLinesByWellDAO.FunctionName", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' If pBLType = STATIC then: Delete all the STATIC records, except the max baseLineID of each well (NOTE: the max baselineID can be STATIC or DYNAMIC)
        ''' If pBLType = DYNAMIC then: Delete all the DYNAMIC records, except the max baselineID of each well (max baselineID for DYNAMIC)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pLastValuesDS"></param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' AG 17/11/2014 BA-2065
        ''' AG 21/11/2014 BA-2065 remove only records with type STATIC
        ''' AG 08/01/2014 BA-2182 (step 4) new parameters and different query depending pBLType
        ''' </remarks>
        Public Function ResetWSForDynamicBL(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pBLType As String, _
                                            Optional ByVal pLastValuesDS As BaseLinesDS = Nothing, Optional ByVal pMaxDynamicID As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty

                    If pBLType = GlobalEnumerates.BaseLineType.STATIC.ToString AndAlso Not pLastValuesDS Is Nothing Then
                        For Each row As BaseLinesDS.twksWSBaseLinesRow In pLastValuesDS.twksWSBaseLines
                            cmdText &= " DELETE twksWSBLinesByWell " & vbCrLf & _
                                       " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                       " AND    WorkSessionID = N'" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                       " AND WellUsed = " & row.WellUsed & vbCrLf & _
                                       " AND Type = '" & GlobalEnumerates.BaseLineType.STATIC.ToString.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                       " AND BaseLineID <>" & row.BaseLineID & vbCrLf
                            cmdText &= vbNewLine
                        Next

                    ElseIf pBLType = GlobalEnumerates.BaseLineType.DYNAMIC.ToString AndAlso pMaxDynamicID > -1 Then
                        cmdText &= " DELETE twksWSBLinesByWell " & vbCrLf & _
                                   " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                   " AND    WorkSessionID = N'" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                   " AND Type = '" & GlobalEnumerates.BaseLineType.DYNAMIC.ToString.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                   " AND BaseLineID <" & pMaxDynamicID & vbCrLf

                    End If

                    If cmdText <> "" Then
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        End Using
                    End If

                    End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.ResetWSForDynamicBL", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the base line ID (filtering by Type) to a new value
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="NewBaseLineID"></param>
        ''' <param name="pType"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 21/11/2014 BA-2065
        ''' AG 08/01/2015 BA-2182 add pWorkSessionID
        ''' </remarks>
        Public Function UpdateBaseLineIDByType(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal NewBaseLineID As Integer, ByVal pType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    cmdText &= " UPDATE twksWSBLinesByWell SET BaseLineID = " & NewBaseLineID & vbCrLf & _
                               " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                               " AND  WorkSessionID = N'" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                               " AND  Type = '" & pType.Trim.Replace("'", "''") & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.UpdateBaseLineIDByType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the WorkSessionID
        ''' Required because using dynamic base line after reset this table can contain several records. We must update the WorkSessionID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pNewWorkSessionID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 24/11/2014 BA-2065</remarks>
        Public Function UpdateWorkSessionID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pNewWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    cmdText &= " UPDATE twksWSBLinesByWell SET WorkSessionID = N'" & pNewWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                               " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf


                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.UpdateWorkSessionID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Read distinct worksessions by AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadWorkSessions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT WorkSessionID " & vbCrLf & _
                                                " FROM   twksWSBLinesByWell" & vbCrLf & _
                                                " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " ORDER BY WorkSessionID DESC " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.ReadWorkSessions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete all records. Similar than ResetWS but here when informed the worksession in parameter is not deleted
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pExceptWorkSessionID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 24/11/2014 BA-2065</remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, Optional ByVal pExceptWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSBLinesByWell " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf

                    If pExceptWorkSessionID <> "" Then
                        cmdText &= " AND    WorkSessionID <> N'" & pExceptWorkSessionID.Trim.Replace("'", "''") & "' "
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete all records using pIDToDelete
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pIDToDelete"></param>
        ''' <returns></returns>
        ''' <remarks>AG 19/12/2014 BA-2182 (1)</remarks>
        Public Function DeleteByID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pIDToDelete As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSBLinesByWell " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND WorkSessionID = N'" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    BaseLineID = " & pIDToDelete.ToString

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.DeleteByID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update all records with pCurrentID to pNewID
        ''' (before update the method deletes all records (if any) that uses pNewID)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pCurrentID"></param>
        ''' <param name="pNewID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 19/12/2014 BA-2182 (1)</remarks>
        Public Function UpdateByID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pCurrentID As Integer, ByVal pNewID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    cmdText &= " UPDATE twksWSBLinesByWell SET BaseLineID = " & pNewID.ToString & vbCrLf & _
                               " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                               " AND  WorkSessionID = N'" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                               " AND  BaseLineID = " & pCurrentID.ToString

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.UpdateByID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Search the last baselineID (minor or equal) than pPreviousID with the same type as pType parameter
        ''' (this method is created because it is needed for excel results creation with dynamic base line)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pPreviousID"></param>
        ''' <param name="pWellUsed"></param>
        ''' <param name="pType"></param>
        ''' <returns></returns>
        ''' <remarks>AG 07/01/2015 BA-2182</remarks>
        Public Function GetPreviousBaseLineIDByType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                    ByVal pPreviousID As Integer, ByVal pWellUsed As Integer, ByVal pType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TOP 1 AnalyzerID, WorkSessionID, BaseLineID, Wavelenght AS Wavelength, WellUsed, MainLight, " & vbCrLf & _
                                                       " RefLight, ABSvalue, IsMean, DateTime, Type " & vbCrLf & _
                                                " FROM   twksWSBLinesByWell" & vbCrLf & _
                                                " WHERE  BaseLineID    <= " & pPreviousID.ToString & vbCrLf & _
                                                " AND    AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    WellUsed      = " & pWellUsed.ToString & vbCrLf & _
                                                " AND    Type      = '" & pType.ToString & "' " & vbCrLf & _
                                                " ORDER BY BaseLineID DESC " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSBLinesByWellDAO.GetPreviousBaseLineIDByType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace

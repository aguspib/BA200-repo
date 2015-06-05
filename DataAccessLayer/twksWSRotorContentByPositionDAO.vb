Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWSRotorContentByPositionDAO
          

#Region "CRUD"

        ''' <summary>
        ''' Create all Analyzer Rotor Positions when a WorkSession is created
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing the Rotor Positions to create</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SA 04/11/2010 - Added N preffix for multilanguage of field TS_User
        '''              XB 07/10/2014 - Add log traces to catch NULL wrong assignment on RealVolume field - BA-1978
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim NullAssigment As Boolean = False

                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmd As SqlCommand
                    cmd = pDBConnection.CreateCommand

                    Dim keys As String = " (AnalyzerID, RotorType, RingNumber, CellNumber, WorkSessionID, ElementID, MultiTubeNumber," & _
                                         "  TubeType, RealVolume, RemainingTestsNumber, Status, ScannedPosition, BarCodeInfo, BarcodeStatus, " & _
                                         "  TS_User, TS_DateTime) "

                    Dim values As String = " "
                    For Each twksWSRotorContentByPositionDR As DataRow In pWSRotorContentByPositionDS.twksWSRotorContentByPosition
                        values = ""
                        values &= " '" & twksWSRotorContentByPositionDR("AnalyzerID").ToString().Replace("'", "''") & "', "
                        values &= " '" & twksWSRotorContentByPositionDR("RotorType").ToString().Replace("'", "''") & "', "
                        values &= ReplaceNumericString(CSng(twksWSRotorContentByPositionDR("RingNumber"))) & ", "
                        values &= ReplaceNumericString(CSng(twksWSRotorContentByPositionDR("CellNumber"))) & ", "
                        values &= " '" & twksWSRotorContentByPositionDR("WorkSessionID").ToString().Replace("'", "''") & "', "

                        If twksWSRotorContentByPositionDR("ElementID") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= twksWSRotorContentByPositionDR("ElementID").ToString() & ", "
                        End If

                        If twksWSRotorContentByPositionDR("MultiTubeNumber") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(CSng(twksWSRotorContentByPositionDR("MultiTubeNumber"))) & ", "
                        End If

                        If twksWSRotorContentByPositionDR("TubeType") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= " '" & ReplaceNumericString(CSng(twksWSRotorContentByPositionDR("TubeType"))) & "', "
                        End If

                        If twksWSRotorContentByPositionDR("RealVolume") Is DBNull.Value Then
                            values &= " NULL, "
                            NullAssigment = True
                        Else
                            values &= ReplaceNumericString(CSng(twksWSRotorContentByPositionDR("RealVolume"))) & ", "
                        End If

                        If twksWSRotorContentByPositionDR("RemainingTestsNumber") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= twksWSRotorContentByPositionDR("RemainingTestsNumber").ToString() & ", "
                        End If

                        If twksWSRotorContentByPositionDR("Status") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= " '" & twksWSRotorContentByPositionDR("Status").ToString().Replace("'", "''") & "', "
                        End If

                        If twksWSRotorContentByPositionDR("ScannedPosition") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= " '" & twksWSRotorContentByPositionDR("ScannedPosition").ToString().Replace("'", "''") & "', "
                        End If

                        If twksWSRotorContentByPositionDR("BarCodeInfo") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= " '" & twksWSRotorContentByPositionDR("BarCodeInfo").ToString().Replace("'", "''") & "', "
                        End If

                        If twksWSRotorContentByPositionDR("BarcodeStatus") Is DBNull.Value Then
                            values &= " NULL, "
                        Else
                            values &= " '" & twksWSRotorContentByPositionDR("BarcodeStatus").ToString().Replace("'", "''") & "', "
                        End If

                        If twksWSRotorContentByPositionDR("TS_USER") Is DBNull.Value Then
                            'Dim myGlobalbase As New GlobalBase
                            values &= " N'" & GlobalBase.GetSessionInfo().UserName().Replace("'", "''") & "', "
                        Else
                            values &= " N'" & twksWSRotorContentByPositionDR("TS_User").ToString().Replace("'", "''") & "', "
                        End If

                        values &= " '" & CType(Now, DateTime).ToString("yyyyMMdd HH:mm:ss") & "' "

                        Dim cmdText As String = ""
                        cmdText = "INSERT INTO twksWSRotorContentByPosition  " & keys & " VALUES (" & values & ")"

                        If NullAssigment Then
                            ' Rules that RealVolume NULL value is not allowed
                            If twksWSRotorContentByPositionDR("RotorType").ToString() = "REAGENTS" Then
                                If Not twksWSRotorContentByPositionDR("Status") Is DBNull.Value Then
                                    If Not twksWSRotorContentByPositionDR("Status").ToString() = "FREE" Then
                                        'Dim myLogAccionesAux As New ApplicationLogManager()
                                        GlobalBase.CreateLogActivity("A not allowed NULL value it is performed on Real Volume field !", "twksWSRotorContentByPositionDAO.Create", EventLogEntryType.Error, False)
                                    End If
                                End If
                            End If
                        End If

                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Rotor Position (by AnalyzerID, RotorType, RingNumber, CellNumber and WorkSessionID)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet WSRotorContentByPositionDS containing the list of
        '''                                           Rotor Positions to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 19/11/2009
        ''' Modified by: SA 26/01/2010 - Removed return of error "NO RECORD DELETE"; this message does not exist
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmd As SqlCommand
                    cmd = pDBConnection.CreateCommand

                    Dim cmdText As String = ""
                    For Each twksWSRotorContentByPositionDR As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pWSRotorContentByPositionDS.twksWSRotorContentByPosition
                        cmdText = " DELETE twksWSRotorContentByPosition " & _
                                  " WHERE  AnalyzerID    = '" & twksWSRotorContentByPositionDR.AnalyzerID & "' " & _
                                  " AND    RotorType     = '" & twksWSRotorContentByPositionDR.RotorType & "' " & _
                                  " AND    RingNumber    = " & twksWSRotorContentByPositionDR.RingNumber & _
                                  " AND    CellNumber    = " & twksWSRotorContentByPositionDR.CellNumber & _
                                  " AND    WorkSessionID = '" & twksWSRotorContentByPositionDR.WorkSessionID.ToString().Replace("'", "''") & "' "

                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        If (Not myGlobalDataTO.AffectedRecords > 0) Then
                            myGlobalDataTO.HasError = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the content of an specific cell in an Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analizer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <param name="pWorkSessionID ">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all information of the 
        '''          Rotor Position
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 19/11/2009
        ''' Modified by: AG 03/12/2009 - Added pWorkSessionID optional parameter (tested pending)
        '''              TR 10/12/2009 - Remove optional to parameter WorkSessionID; this value is Mandatory
        '''              SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 10/05/2012 - Changed the function template; parameter pRingNumber changed to optional 
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                             ByVal pRotorType As String, ByVal pCellNumber As Integer, Optional ByVal pRingNumber As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.ToString() & "' " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType & "' " & vbCrLf & _
                                                " AND    CellNumber = " & pCellNumber.ToString() & vbCrLf

                        If (pRingNumber <> -1) Then cmdText &= " AND RingNumber = " & pRingNumber.ToString() & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.Read", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Analyzer and Rotor in which the informed required Element is positioned
        ''' for the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPosition with the Analyzer
        '''          and Rotor in which the informed required Work Session Element is positioned</returns>
        ''' <remarks>
        ''' Created by:  DL 04/01/2011
        ''' Modified by: SA 08/02/2012 - Changed the function template
        '''              TR 05/03/2012 - Get also field CellNumber 
        ''' </remarks>
        Public Function ReadByElementIDAndWorkSessionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                        ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT AnalyzerID, RotorType, CellNumber " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    ElementID = " & pElementID.ToString & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ReadByElementIDAndWorkSessionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get which of the informed Elements are positioned in the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pElementList">List of elements separated by (,)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of Rotor Positions in which the informed 
        '''          Elements are positioned</returns>
        ''' <remarks>
        ''' Created by:  TR 18/11/2009
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 10/05/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByElementIDList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                            ByVal pElementList As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  ElementID IN (" & pElementList & ") " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType.Trim & "' " & vbCrLf & _
                                                " AND    AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ReadByElementIDList", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information of the Tube positioned in the specified Rotor Cell
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPosition with the Analyzer
        '''          and Rotor in which the informed required Work Session Element is positioned</returns>
        ''' <remarks>
        ''' Created by:  DL 04/01/2011
        ''' Modified by: AG 12/04/2011 - Changed the query to get also field TubeContent from table twksWSRequiredElements
        ''' </remarks>
        Public Function ReadByRotorTypeAndCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorType As String, ByVal pCellNumber As Integer, _
                                                     ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, RCP.MultiTubeNumber, RCP.TubeType, " & vbCrLf & _
                                                       " RCP.RealVolume, RCP.RemainingTestsNumber, RCP.Status, RCP.ScannedPosition, RCP.BarCodeInfo, RCP.BarcodeStatus, RCP.TS_User, " & vbCrLf & _
                                                       " RCP.TS_DateTime, ELM.TubeContent " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSRequiredElements ELM ON RCP.ElementID = ELM.ElementID " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.RotorType     = '" & pRotorType.Trim & "' " & vbCrLf & _
                                                " AND    RCP.CellNumber    = " & pCellNumber.ToString & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ReadByRotorTypeAndCellNumber", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update records on table twksWSRotorContentByPositon 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSRotorContentByPositionDS">Typed DataSet WSRotorContentByPositionDS containing the list of Rotor
        '''                                           positions to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR - TESTED OK
        ''' Modified by: AG 14/01/2010 - Add new field VirtualRotorID (tested OK)
        '''              AG 21/01/2010 - Delete updating field VirtualRotorID
        '''              SA 26/01/2010 - Removed return of error "NO RECORD UPDATE"; this message does not exist. Added Exit For 
        '''                              when an update fails
        '''              SA 04/11/2010 - Added N preffix for multilanguage of field TS_User
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmd As SqlCommand
                    cmd = pDBConnection.CreateCommand

                    Dim cmdText As String = ""
                    Dim values As String = ""
                    For Each twksWSRotorContentByPositionDR As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows
                        values = ""
                        values &= "ElementID = "
                        If twksWSRotorContentByPositionDR.IsElementIDNull Then
                            values &= "NULL,"
                        Else
                            values &= twksWSRotorContentByPositionDR.ElementID.ToString() & ","
                        End If

                        values &= "MultiTubeNumber = "
                        If twksWSRotorContentByPositionDR.IsMultiTubeNumberNull Then
                            values &= "NULL,"
                        Else
                            values &= twksWSRotorContentByPositionDR.MultiTubeNumber.ToString() & ","
                        End If

                        values &= "TubeType = "
                        If twksWSRotorContentByPositionDR.IsTubeTypeNull Then
                            values &= "NULL,"
                        Else
                            values &= "'" & twksWSRotorContentByPositionDR.TubeType.Replace("'", "''") & "',"
                        End If

                        values &= "RealVolume = "
                        If twksWSRotorContentByPositionDR.IsRealVolumeNull Then
                            values &= "NULL,"
                        Else
                            values &= ReplaceNumericString(twksWSRotorContentByPositionDR.RealVolume) & ","
                        End If

                        values &= "RemainingTestsNumber = "
                        If twksWSRotorContentByPositionDR.IsRemainingTestsNumberNull Then
                            values &= "NULL,"
                        Else
                            values &= twksWSRotorContentByPositionDR.RemainingTestsNumber.ToString() & ","
                        End If

                        values &= "Status = "
                        If twksWSRotorContentByPositionDR.IsStatusNull Then
                            values &= "NULL,"
                        Else
                            values &= "'" & twksWSRotorContentByPositionDR.Status.Replace("'", "''") & "',"
                        End If

                        values &= "ScannedPosition = "
                        If twksWSRotorContentByPositionDR.IsScannedPositionNull Then
                            values &= "NULL,"
                        Else
                            values &= "'" & twksWSRotorContentByPositionDR.ScannedPosition.ToString() & "',"
                        End If

                        values &= "BarCodeInfo = "
                        If twksWSRotorContentByPositionDR.IsBarCodeInfoNull Then
                            values &= "NULL,"
                        Else
                            values &= "'" & twksWSRotorContentByPositionDR.BarCodeInfo.Replace("'", "''") & "',"
                        End If

                        values &= "BarcodeStatus = "
                        If twksWSRotorContentByPositionDR.IsBarcodeStatusNull Then
                            values &= "NULL,"
                        Else
                            values &= "'" & twksWSRotorContentByPositionDR.BarcodeStatus.ToString().Replace("'", "''") & "',"
                        End If

                        values &= "TS_User = "
                        If twksWSRotorContentByPositionDR.IsTS_UserNull Then
                            'Dim myGlobalbase As New GlobalBase
                            values &= " N'" & GlobalBase.GetSessionInfo().UserName & "', "
                        Else
                            values &= " N'" & twksWSRotorContentByPositionDR.TS_User.ToString().Replace("'", "''") & "',"
                        End If

                        values &= "TS_DateTime = "
                        If twksWSRotorContentByPositionDR.IsTS_DateTimeNull Then
                            values &= "'" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                        Else
                            values &= "'" & twksWSRotorContentByPositionDR.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'"
                        End If

                        cmdText = ""
                        cmdText = " UPDATE twksWSRotorContentByPosition SET " & values & _
                                  " WHERE  AnalyzerID = '" & twksWSRotorContentByPositionDR.AnalyzerID & "' " & _
                                  " AND    RotorType = '" & twksWSRotorContentByPositionDR.RotorType & "' " & _
                                  " AND    RingNumber = " & twksWSRotorContentByPositionDR.RingNumber & _
                                  " AND    CellNumber = " & twksWSRotorContentByPositionDR.CellNumber & _
                                  " AND    WorkSessionID = '" & twksWSRotorContentByPositionDR.WorkSessionID.ToString().Replace("'", "''") & "' "


                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        If (Not myGlobalDataTO.AffectedRecords > 0) Then
                            myGlobalDataTO.HasError = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Change the Analyzer identifier of the informed Analyzer WorkSession.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 12/06/2012
        ''' </remarks>
        Public Function UpdateWSAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = " UPDATE twksWSRotorContentByPosition " & vbCrLf & _
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Count how many cells in the specified Analyzer Rotor have the informed Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pRotorType">Type of Analyzer Rotor</param>
        ''' <param name="pStatus">Cell Status to search</param>
        ''' <param name="pRingNumber">Ring Number. Optional parameter</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of cells having the informed Status</returns>
        ''' <remarks>
        ''' Created by:  TR 28/01/2010 - TESTED: OK
        ''' Modified by: TR 01/02/2010 - Added new optional parameter pRingNumber
        '''              SA 12/01/2012 - Changed the function template
        '''              MR 04/06/2015 - Add OPTIONAL filter parameters (pNumCell, pNumMaxbyRotor)
        ''' </remarks>
        Public Function CountPositionsByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                               ByVal pRotorType As String, ByVal pStatus As String, Optional ByVal pRingNumber As Integer = 0, _
                                               Optional ByVal pNumCell As Integer = 0, Optional ByVal pNumMaxbyRotor As Integer = 0) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS TotalPositions " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND   WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND   RotorType = '" & pRotorType & "' " & vbCrLf & _
                                                " AND   Status = '" & pStatus & "' " & vbCrLf

                        If (pRingNumber > 0) Then cmdText &= " AND RingNumber = " & pRingNumber
                        If (pNumCell > 0 AndAlso pNumMaxbyRotor > 0) Then cmdText &= " AND  CellNumber BETWEEN '" & pNumCell & "' and '" & pNumMaxbyRotor & "'" & vbCrLf

                        cmdText &= " GROUP BY Status "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.CountPositionByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Count the number of positioned bottles on the Reagents Rotor for an specific Reagent or Additional Solution (ElementID)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzeID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with the number of different Bottles
        '''          placed for the specified Reagent in the Reagents Rotor</returns>
        ''' <remarks>
        ''' Created by:  TR 04/02/2010
        ''' Modified by: SA 07/02/2012 - Changed the function template
        '''              SA 08/02/2012 - Changed the query to exclude bottles that are depleted 
        '''              SA 14/02/2012 - Changed the query to get not only the number of bottles but also the total positioned volume of the Reagent
        ''' </remarks>
        Public Function CountPositionedReagentsBottlesByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzeID As String, _
                                                                  ByVal pWorkSessionID As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TubeType AS TubeCode, COUNT(*) AS NumOfBottles, SUM(RealVolume) AS RealVolume " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzeID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RotorType = 'REAGENTS' " & vbCrLf & _
                                                " AND    ElementID = " & pElementID & vbCrLf & _
                                                " AND    Status <> 'DEPLETED' " & vbCrLf & _
                                                " GROUP BY TubeType " & vbCrLf

                        Dim resultData As New ReagentTubeTypesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.ReagentTubeTypes)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.CountPositionedReagentsBottlesByElementID", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Count how many tubes there are in Samples Rotor for the active Work Session labelled with the specified 
        ''' Specimen Identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSpecimenID">Specimen Identifier to search</param>
        ''' <returns>GlobalDataTO containing a boolean value: TRUE when a tube with the specified SpecimenID exists as Barcode label
        '''          in Samples Rotor; otherwise FALSE</returns>
        ''' <remarks>
        ''' Created by:  SA 29/01/2014 - BT #1474
        ''' </remarks>
        Public Function ExistBarcodeInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                         ByVal pSpecimenID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RotorType     = 'SAMPLES' " & vbCrLf & _
                                                " AND    BarcodeInfo   = N'" & pSpecimenID.Trim.Replace("'", "''") & "' " & vbCrLf

                        Dim thereAreTubes As Boolean = False
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    thereAreTubes = CBool(dbDataReader.Item(0))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                        myGlobalDataTO.SetDatos = thereAreTubes
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ExistBarcodeInfo", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search all Rotor positions containing a not depleted tube of the informed Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPosition</returns>
        ''' <remarks>
        ''' Created by:  DL 05/01/2011 - PENDING: CONFIRM LOCKED STATUS FOR REAGENT BOTTLES WHEN THEY WERE REFILLED
        ''' </remarks>
        Public Function ExistOtherPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, _
                                           ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT AnalyzerID, RotorType, RingNumber, CellNumber, WorkSessionID, ElementID, MultiTubeNumber, TubeType, " & vbCrLf
                        cmdText &= "        RealVolume, RemainingTestsNumber, Status, ScannedPosition, BarCodeInfo, BarcodeStatus, TS_User, TS_DateTime " & vbCrLf
                        cmdText &= " FROM   twksWSRotorContentByPosition " & vbCrLf
                        cmdText &= " WHERE ((RotorType = 'REAGENTS' AND Status <> 'DEPLETED' AND Status <> 'LOCKED') " & vbCrLf
                        cmdText &= " OR     (RotorType = 'SAMPLES'  AND Status <> 'DEPLETED')) "
                        cmdText &= " AND    AnalyzerID = '" & pAnalyzerID & "'"
                        cmdText &= " AND    WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf
                        cmdText &= " AND    ElementID = " & pElementID & vbCrLf
                        cmdText &= " AND   (BarcodeStatus IS NULL OR BarcodeStatus <> 'ERROR')" 'AG 02/08/2011

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim resultData As New WSRotorContentByPositionDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ExistOtherPosition", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get from Samples Rotor (for the specified Analyzer and WorkSession) all scanned tubes containing Patient Samples, regardless their Status (complete
        ''' and incomplete Patient Samples are returned)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pExcludeDuplicates">When TRUE, it indicates that cells containing duplicated Barcodes will not be returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with all scanned Patient Samples in Samples Rotor (complete
        '''          and incomplete Patient Samples)</returns>
        ''' <remarks>
        ''' Created by:  SA 10/07/2013
        ''' Modified by: SA 16/07/2013 - Changed the SQL Sentence (both sub-queries) to exclude all duplicated Barcodes
        '''              SA 24/07/2013 - Changed the SQL Sentence (both sub-queries) to get also the current Status of each Rotor cell. Added new parameter 
        '''                              to indicate when duplicated tubes have to be excluded. Changed the first sub-query to obtain value of StatFlag from
        '''                              table twksWSRequiredElemByOrderTests   
        ''' </remarks>
        Public Function GetAllPatientTubesForHQ(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                ByVal pExcludeDuplicates As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'AJG
                        'Dim cmdText As String = " SELECT RCP.AnalyzerID, RCP.WorkSessionID, RCP.RotorType, RCP.CellNumber, " & vbCrLf & _
                        '                              " (CASE WHEN RE.PatientID IS NULL THEN RE.SampleID ELSE RE.PatientID END) AS ExternalPID, " & vbCrLf & _
                        '                              "  RE.SampleType, REOT.StatFlag, NULL AS PatientID, 1 AS CompletedFlag, 0 AS NotSampleType, " & vbCrLf & _
                        '                              "  'COMPLETED' AS LISStatus, RCP.BarCodeInfo, NULL AS MessageID, RCP.RingNumber, WSA.WSStatus, RCP.Status AS CellStatus " & vbCrLf & _
                        '                        " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSRequiredElements RE ON RCP.ElementID = RE.ElementID " & vbCrLf & _
                        '                                                                                                    " AND RCP.WorkSessionID = RE.WorkSessionID " & vbCrLf & _
                        '                                                                                                    " AND RE.TubeContent = 'PATIENT' " & vbCrLf & _
                        '                                                                " INNER JOIN twksWSRequiredElemByOrderTest REOT ON RE.ElementID = REOT.ElementID " & vbCrLf & _
                        '                                                                " INNER JOIN twksWSAnalyzers WSA ON RCP.AnalyzerID    = WSA.AnalyzerID " & vbCrLf & _
                        '                                                                                              " AND RCP.WorkSessionID = WSA.WorkSessionID " & vbCrLf & _
                        '                        " WHERE  RCP.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                        '                        " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                        " AND    RCP.RotorType     = 'SAMPLES' " & vbCrLf & _
                        '                        " AND    RCP.Status NOT IN ('FREE', 'NO_INUSE') " & vbCrLf & _
                        '                        " AND   (RCP.BarCodeInfo IS NOT NULL AND  RCP.BarCodeInfo <> '') " & vbCrLf & _
                        '                        " AND    RCP.BarcodeInfo IN (SELECT RC.BarcodeInfo FROM twksWSRotorContentByPosition RC " & vbCrLf & _
                        '                                                   " WHERE  RC.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                        '                                                   " AND    RC.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                                                   " AND    RC.RotorType     = 'SAMPLES' " & vbCrLf & _
                        '                                                   " AND    RC.Status NOT IN ('FREE', 'NO_INUSE') " & vbCrLf & _
                        '                                                   " AND   (RC.BarCodeInfo IS NOT NULL AND  RCP.BarCodeInfo <> '') " & vbCrLf

                        Dim cmdText As String = " SELECT RCP.AnalyzerID, RCP.WorkSessionID, RCP.RotorType, RCP.CellNumber, " & vbCrLf & _
                                                      " (CASE WHEN RE.PatientID IS NULL THEN RE.SampleID ELSE RE.PatientID END) AS ExternalPID, " & vbCrLf & _
                                                      "  RE.SampleType, REOT.StatFlag, NULL AS PatientID, 1 AS CompletedFlag, 0 AS NotSampleType, " & vbCrLf & _
                                                      "  'COMPLETED' AS LISStatus, RCP.BarCodeInfo, NULL AS MessageID, RCP.RingNumber, WSA.WSStatus, RCP.Status AS CellStatus " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSRequiredElements RE ON RCP.ElementID = RE.ElementID " & vbCrLf & _
                                                                                                                            " AND RCP.WorkSessionID = RE.WorkSessionID " & vbCrLf & _
                                                                                                                            " AND RE.TubeContent = 'PATIENT' " & vbCrLf & _
                                                                                        " INNER JOIN twksWSRequiredElemByOrderTest REOT ON RE.ElementID = REOT.ElementID " & vbCrLf & _
                                                                                        " INNER JOIN twksWSAnalyzers WSA ON RCP.AnalyzerID    = WSA.AnalyzerID " & vbCrLf & _
                                                                                                                      " AND RCP.WorkSessionID = WSA.WorkSessionID " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.RotorType     = 'SAMPLES' " & vbCrLf & _
                                                " AND    RCP.Status NOT IN ('FREE', 'NO_INUSE') " & vbCrLf & _
                                                " AND   (RCP.BarCodeInfo IS NOT NULL AND  RCP.BarCodeInfo <> '') " & vbCrLf & _
                                                " AND    EXISTS (SELECT RC.BarcodeInfo FROM twksWSRotorContentByPosition RC " & vbCrLf & _
                                                                " WHERE  RC.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                                " AND    RC.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                                " AND    RC.RotorType     = 'SAMPLES' " & vbCrLf & _
                                                                " AND    RC.Status NOT IN ('FREE', 'NO_INUSE') " & vbCrLf & _
                                                                " AND   (RC.BarCodeInfo IS NOT NULL AND  RCP.BarCodeInfo <> '') " & vbCrLf & _
                                                                " AND    RCP.BarcodeInfo = RC.BarcodeInfo " & vbCrLf

                        If (pExcludeDuplicates) Then cmdText &= " GROUP BY RC.BarcodeInfo HAVING COUNT(*) = 1 " & vbCrLf
                        'AJG
                        'cmdText &= " ) UNION " & vbCrLf & _
                        '           " SELECT BPW.*, RCP.RingNumber, WSA.WSStatus, RCP.Status AS CellStatus " & vbCrLf & _
                        '           " FROM   twksWSBarcodePositionsWithNoRequests BPW INNER JOIN twksWSRotorContentByPosition RCP ON BPW.AnalyzerID    = RCP.AnalyzerID " & vbCrLf & _
                        '                                                                                                      " AND BPW.WorkSessionID = RCP.WorkSessionID " & vbCrLf & _
                        '                                                                                                      " AND BPW.RotorType     = RCP.RotorType " & vbCrLf & _
                        '                                                                                                      " AND BPW.CellNumber    = RCP.CellNumber " & vbCrLf & _
                        '                                                           " INNER JOIN twksWSAnalyzers WSA ON BPW.AnalyzerID    = WSA.AnalyzerID " & vbCrLf & _
                        '                                                                                         " AND BPW.WorkSessionID = WSA.WorkSessionID " & vbCrLf & _
                        '           " WHERE  BPW.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                        '           " AND    BPW.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '           " AND    BPW.RotorType     = 'SAMPLES' " & vbCrLf & _
                        '           " AND    BPW.BarcodeInfo IN (SELECT BP.BarcodeInfo FROM twksWSBarcodePositionsWithNoRequests BP " & vbCrLf & _
                        '                                      " WHERE  BP.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                        '                                      " AND    BP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                                      " AND    BP.RotorType     = 'SAMPLES' " & vbCrLf

                        cmdText &= " ) UNION " & vbCrLf & _
                                   " SELECT BPW.*, RCP.RingNumber, WSA.WSStatus, RCP.Status AS CellStatus " & vbCrLf & _
                                   " FROM   twksWSBarcodePositionsWithNoRequests BPW INNER JOIN twksWSRotorContentByPosition RCP ON BPW.AnalyzerID    = RCP.AnalyzerID " & vbCrLf & _
                                                                                                                              " AND BPW.WorkSessionID = RCP.WorkSessionID " & vbCrLf & _
                                                                                                                              " AND BPW.RotorType     = RCP.RotorType " & vbCrLf & _
                                                                                                                              " AND BPW.CellNumber    = RCP.CellNumber " & vbCrLf & _
                                                                                   " INNER JOIN twksWSAnalyzers WSA ON BPW.AnalyzerID    = WSA.AnalyzerID " & vbCrLf & _
                                                                                                                 " AND BPW.WorkSessionID = WSA.WorkSessionID " & vbCrLf & _
                                   " WHERE  BPW.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                   " AND    BPW.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                   " AND    BPW.RotorType     = 'SAMPLES' " & vbCrLf & _
                                   " AND    EXISTS (SELECT BP.BarcodeInfo FROM twksWSBarcodePositionsWithNoRequests BP " & vbCrLf & _
                                                   " WHERE  BP.AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                   " AND    BP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                   " AND    BP.RotorType     = 'SAMPLES' " & vbCrLf & _
                                                   " AND    BPW.BarcodeInfo  = BP.BarcodeInfo " & vbCrLf

                        If (pExcludeDuplicates) Then cmdText &= " GROUP BY BP.BarcodeInfo HAVING COUNT(*) = 1 " & vbCrLf
                        cmdText &= " ) ORDER BY ExternalPID, SampleType " & vbCrLf

                        Dim myDataSet As New BarcodePositionsWithNoRequestsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSBarcodePositionsWithNoRequests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myDataSet
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetAllPatientTubesForHQ", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information of all positions of a specific Analyzer Rotor in which a required Work Session Element is positioned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of positions
        '''          in which required Work Session Elements are placed</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 06/02/2012 - Changed the function template 
        ''' </remarks>
        Public Function GetAllPositionedElementsToReset(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT ElementID, AnalyzerID, RotorType, RingNumber, CellNumber, WorkSessionID, " & vbCrLf & _
                                                                " MultiTubeNumber, TubeType, RealVolume, RemainingTestsNumber,Status, ScannedPosition, " & vbCrLf & _
                                                                " BarCodeInfo, BarcodeStatus, TS_User, TS_DateTime, 'NOPOS' AS ElementStatus " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType.Trim & "' " & vbCrLf & _
                                                " AND    ElementID IS NOT NULL " & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetAllPositionedElements", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <sumary>Search the minimum free Cell in the specified Analyzer Rotor Ring in the informed Work Session</sumary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <param name="pReferenceCellNumber">Optional parameter. When informed, it indicates that the function should
        '''                                    get the minimum free Cell in the Ring but after this one</param>
        ''' <returns>GlobalDataTO containing an Integer value with the number of the free Rotor Cell found</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template.
        '''              SA 10/01/2012 - Changed the function template
        '''  </remarks>
        Public Function GetMinFreeCellByRing(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                             ByVal pRotorType As String, ByVal pWorkSessionID As String, ByVal pRingNumber As Integer, _
                                             Optional ByVal pReferenceCellNumber As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MIN(CellNumber) AS CellNumber " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    RingNumber = " & pRingNumber.ToString() & vbCrLf & _
                                                " AND    Status = 'FREE' " & vbCrLf

                        If (pReferenceCellNumber > 0) Then cmdText &= " AND CellNumber > " & pReferenceCellNumber.ToString() & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetMinFreeCellByRing", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search the maximum free Cell in the specified Analyzer Rotor Ring in the informed Work Session 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <returns>GlobalDataTO containing an Integer value with the maximum free Cell Number in the
        '''          specified Analyzer Rotor Ring</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 06/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetMaxFreeCellByRing(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                             ByVal pWorkSessionID As String, ByVal pRingNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(CellNumber) As CellNumber " & vbCrLf & _
                                                " FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE AnalyzerID  = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND   RotorType     = '" & pRotorType.Trim & "' " & vbCrLf & _
                                                " AND   WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND   RingNumber = " & pRingNumber.ToString() & vbCrLf & _
                                                " AND   Status = 'FREE' " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetMaxFreeCellByRing", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the informed Element corresponds to a MultiPoint Calibrator and in that case return
        ''' the Rotor Positions in which the rest of the tubes of the Calibrator kit are placed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of Rotor Positions
        '''          when the tubes of the informed MultiPoint Calibrator are placed</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 06/02/2012 - Changed the function template and query format to ANSI SQL
        ''' </remarks>
        Public Function GetMultiPointCalibratorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                         ByVal pCalibratorID As Integer, ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT  RCP.*, RE.TubeContent " & vbCrLf & _
                                                " FROM    twksWSRequiredElements RE INNER JOIN twksWSRotorContentByPosition RCP ON RE.ElementID = RCP.ElementID " & vbCrLf & _
                                                                                                                             " AND RE.WorkSessionID = RCP.WorkSessionID " & vbCrLf & _
                                                " WHERE   RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND     RE.CalibratorID  = " & pCalibratorID.ToString() & _
                                                " AND     RE.ElementID    <> " & pElementID.ToString()

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetMultiPointCalibratorPositions", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the number of bottles of a required Work Session Element currently placed in the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Identifier of the required Element</param>
        ''' <returns>GlobalDataTO containing an Integer value with the total number of bottles in the Rotor</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 10/01/2012 - Changed the function template. Changed the query: get COUNT(*) instead of COUNT(MultiTubeNumber)
        ''' </remarks>
        Public Function GetPlacedTubesByElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                ByVal pWorkSessionID As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumOfPlacedTubes " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType & "' " & vbCrLf & _
                                                " AND    ElementID = " & pElementID & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = CType(dbCmd.ExecuteScalar(), Integer)
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetPlacedTubesByElement", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the number of positioned bottles and the total positioned volume by bottle of a Reagent required in the WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pElementID">Identifier of the required Element</param>
        ''' <returns>GlobalDataTO containing a typed DS ReagentTubeTypesDS with the number of positioned bottles and the total positioned volume by bottle
        '''          type for the specified required Reagent Element 
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 18/11/2009
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template.
        '''                              Fixed error in the name of the function in CreateLogActivity
        '''              AG 02/08/2011 - Changed the SQL Query to get only volume of not scanned Reagents or scanned Reagents but without error 
        '''              SA 06/02/2012 - Changed the function template
        '''              SA 27/02/2012 - Changed the query to get the SUM(RealVolume) and the Number of Positioned Bottles by Bottle Type; changed the return
        '''                              value from a single value (the total positioned volume) to a typed DS containing positioned volume and number of
        '''                              bottles by bottle type. Added a new filter to exclude DEPLETED Bottles 
        '''              SA 02/03/2012 - Changed the query by adding an INNER JOIN with table tfmwReagentTubeTypes to allow get also value of 
        '''                              the Bottle Section
        '''              TR 27/09/2012 - Added a new filter to exclude LOCKED Bottles.
        ''' </remarks>
        Public Function GetPositionedReagentVolume(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                   ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RCP.TubeType AS TubeCode, SUM(RCP.RealVolume) AS RealVolume, COUNT(*) AS NumOfBottles, RTT.Section " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN tfmwReagentTubeTypes RTT ON RCP.TubeType = RTT.TubeCode " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.RotorType = '" & pRotorType.Trim & "' " & vbCrLf & _
                                                " AND    RCP.ElementID = " & pElementID & vbCrLf & _
                                                " AND    RCP.Status <> 'DEPLETED' AND RCP.Status <> 'LOCKED' " & vbCrLf & _
                                                " AND   (RCP.BarcodeStatus IS NULL OR RCP.BarcodeStatus <> 'ERROR') " & vbCrLf & _
                                                " GROUP BY RCP.AnalyzerID, RCP.RotorType, RCP.ElementID, RCP.TubeType, RTT.Section "

                        Dim myReagentBottlesDS As New ReagentTubeTypesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myReagentBottlesDS.ReagentTubeTypes)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myReagentBottlesDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetPositionedReagentVolume", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        	''' <summary>
        ''' Get information of all non free positions in the Reagents Rotor for the active WorkSession, including those that contain
        ''' Not In Use elements. Used to save the rotor as an internal Virtual Rotor before reset the WS (due to the Reagents Rotor
        ''' is not phisically discharged after finishing each WS) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pVirtualRotorID">Identifier of the internal Virtual Rotor used to save the content of the Reagents Rotor</param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the content of all non-free
        '''          positions in the Reagents Rotor for the active WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 19/04/2011
        ''' Modified by: SA 27/09/2011 - Changed the query to get also field ScannedPosition
        '''              AG 03/02/2012 - Changed the query to get also the position status (field Status)
        '''              SA 06/02/2012 - Changed the function template; for NOT IN USE Positions, get field Status from table twksWSNotInUseRotorPositions
        '''                              instead of from table twksWSRotorContentByPosition
        '''              SA 08/01/2015 - BA-1999 ==> Changed the second subquery to remove the filter by Position Status = 'NO_INUSE', to allow to get also 
        '''                                          NOT IN USE Positions with Status DEPLETED, FEW or LOCKED
        '''              SA 09/01/2015 - BA-1999 ==> Added a new subquery to get also NOT IN USE Positions with Barcode Status UNKNOWN or ERROR
        ''' </remarks>
        Public Function GetReagentsRotorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT " & pVirtualRotorID.ToString & " AS VirtualRotorID, RCP.RingNumber, RCP.CellNumber, RE.TubeContent, RCP.TubeType, " & vbCrLf & _
                                                         " RCP.MultiTubeNumber, RE.ReagentID, RE.SolutionCode, RE.MultiItemNumber, RCP.RealVolume, RCP.Status, " & vbCrLf & _
                                                         " RCP.BarCodeInfo, RCP.BarcodeStatus, RCP.ScannedPosition, 'BIOSYSTEMS' AS TS_User, GETDATE() AS TS_DateTime " & vbCrLf & _
                                                " FROM    twksWSRotorContentByPosition RCP INNER JOIN twksWSRequiredElements RE ON RCP.ElementID = RE.ElementID " & vbCrLf & _
                                                " WHERE   RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND     RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND     RCP.RotorType     = 'REAGENTS' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT " & pVirtualRotorID.ToString & " AS VirtualRotorID, RCP.RingNumber, RCP.CellNumber, NIU.TubeContent, RCP.TubeType, " & vbCrLf & _
                                                         " RCP.MultiTubeNumber, NIU.ReagentID, NIU.SolutionCode, NIU.MultiItemNumber, RCP.RealVolume, NIU.Status, " & vbCrLf & _
                                                         " RCP.BarcodeInfo, RCP.BarcodeStatus, RCP.ScannedPosition, 'BIOSYSTEMS' AS TS_User, GETDATE() AS TS_DateTime " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSNotInUseRotorPositions NIU ON RCP.AnalyzerID    = NIU.AnalyzerID " & vbCrLf & _
                                                                                                                                   " AND RCP.WorkSessionID = NIU.WorkSessionID " & vbCrLf & _
                                                                                                                                   " AND RCP.RotorType     = NIU.RotorType " & vbCrLf & _
                                                                                                                                   " AND RCP.RingNumber    = NIU.RingNumber " & vbCrLf & _
                                                                                                                                   " AND RCP.CellNumber    = NIU.CellNumber " & vbCrLf & _
                                                " WHERE RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND   RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND   RCP.RotorType     = 'REAGENTS' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT " & pVirtualRotorID.ToString & " AS VirtualRotorID, RCP.RingNumber, RCP.CellNumber, NULL AS TubeContent, RCP.TubeType, " & vbCrLf & _
                                                         " RCP.MultiTubeNumber, NULL AS ReagentID, NULL AS SolutionCode, NULL AS MultiItemNumber, RCP.RealVolume, RCP.Status, " & vbCrLf & _
                                                         " RCP.BarcodeInfo, RCP.BarcodeStatus, RCP.ScannedPosition, 'BIOSYSTEMS' AS TS_User, GETDATE() AS TS_DateTime " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP " & vbCrLf & _
                                                " WHERE RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND   RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND   RCP.RotorType     = 'REAGENTS' " & vbCrLf & _
                                                " AND   RCP.BarcodeInfo IS NOT NULL " & vbCrLf & _
                                                " AND   RCP.BarcodeStatus IN ('UNKNOWN', 'ERROR') " & vbCrLf & _
                                                " ORDER BY RCP.RingNumber, RCP.CellNumber "

                        Dim myVRotorPosDS As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myVRotorPosDS.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = myVRotorPosDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetReagentsRotorPositions", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information of all Ring Cells (status and content) in all Rotors of an Analyzer included in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with information of all Cells in 
        '''          all the Analyzer Rotors
        ''' </returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified By: TR 25/01/2010 - Added the functionality to get the elements which status is "NO_INUSE" and get the tube content
        '''                              from the twksWSNotInUseRotorPositions table.
        '''              SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 30/03/2012 - Changed subquery for NOT IN USE positions: value of Barcode fields has to be get from table
        '''                              twksWSNotInUseRotorPositions instead of from table twksWSRotorContentByPosition
        '''              SA 08/11/2013 - BA-1358 ==> Changed both subqueries to get also field CalibratorID
        '''              JV 08/11/2013 - BA-1358 ==> Changed both subqueries to get also fields MultiItemNumber, ReagentID and SolutionCode
        '''              TR 18/11/2013 - BA-1359 ==> In the first subquery, added a Left Join with table twksWSRotorPositionsInProcess to allow inform
        '''                                          flag InProcessElement = TRUE for those elements that are still needed for the execution of the 
        '''                                          active Work Session. Only second Reagents and Washing Solutions needed to avoid R2 Well Contaminations
        '''                                          will have information in table twksWSRotorPositionsInProcess; for any other element (IN USE or NOT IN USE)
        '''                                          flag InProcessElement will be returned as FALSE
        '''              SA 08/01/2015 - BA-1999 ==> Changed the first subquery to get only Positions with ElementID different of NULL when Status is different
        '''                                          of NOT IN USE (to exclude NOT IN USE Positions with Status DEPLETED, FEW or LOCKED), and to add a new condition 
        '''                                          to get also FREE Positions (which include Positions with BarcodeStatus ERROR or UNKNOWN).. 
        '''                                          Changed the second subquery by removing the filter by Status (it is not needed due to an INNER JOIN is used) to 
        '''                                          allow also to get NOT IN USE Positions with Status DEPLETED, FEW or LOCKED); besides, return NO_INUSE as Status.  
        ''' </remarks>
        Public Function GetRotorContentPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) _
                                                 As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, " & vbCrLf & _
                                                       " RCP.MultiTubeNumber, RCP.TubeType, RCP.RealVolume, RCP.RemainingTestsNumber, RCP.Status, " & vbCrLf & _
                                                       " RCP.ScannedPosition, RCP.BarcodeInfo, RCP.BarcodeStatus, RE.TubeContent, RE.ElementStatus, RE.CalibratorID, " & vbCrLf & _
                                                       " RE.MultiItemNumber, RE.ReagentID, RE.SolutionCode, " & vbCrLf & _
                                                       "(CASE WHEN RIP.InProcessTestsNumber IS NULL THEN 0 ELSE 1 END) AS InProcessElement " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP LEFT OUTER JOIN twksWSRequiredElements RE " & vbCrLf & _
                                                                                                     " ON RCP.WorkSessionID = RE.WorkSessionID " & vbCrLf & _
                                                                                                    " AND RCP.ElementID     = RE.ElementID " & vbCrLf & _
                                                                                        " LEFT OUTER JOIN twksWSRotorPositionsInProcess RIP " & vbCrLf & _
                                                                                                     " ON RCP.RotorType  = RIP.RotorType " & vbCrLf & _
                                                                                                    " AND RCP.CellNumber = RIP.CellNumber " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND  ((RCP.Status       <> 'NO_INUSE' AND RCP.ElementID IS NOT NULL) " & vbCrLf & _
                                                " OR    (RCP.Status        = 'FREE')) " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT DISTINCT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, " & vbCrLf & _
                                                                " RCP.ElementID, RCP.MultiTubeNumber, RCP.TubeType, RCP.RealVolume, RCP.RemainingTestsNumber, " & vbCrLf & _
                                                                " 'NO_INUSE' AS Status, NU.ScannedPosition, NU.BarcodeInfo, NU.BarcodeStatus, NU.TubeContent, " & vbCrLf & _
                                                                " NULL as ElementStatus, NU.CalibratorID, NU.MultiItemNumber, NU.ReagentID, " & vbCrLf & _
                                                                " NU.SolutionCode, 0 AS InProcessElement  " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSNotInUseRotorPositions NU " & vbCrLf & _
                                                                                                " ON RCP.AnalyzerID    = NU.AnalyzerID " & vbCrLf & _
                                                                                               " AND RCP.RotorType     = NU.RotorType " & vbCrLf & _
                                                                                               " AND RCP.RingNumber    = NU.RingNumber " & vbCrLf & _
                                                                                               " AND RCP.CellNumber    = NU.CellNumber " & vbCrLf & _
                                                                                               " AND RCP.WorkSessionID = NU.WorkSessionID " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetRotorContentPositions", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information of all Ring Cells (status and content) in all Rotors of an Analyzer included in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with information of all Cells in 
        '''          all the Analyzer Rotors, without session.
        ''' </returns>
        ''' <remarks>
        ''' Created by:  JV 03/12/2013 - BA-1384
        ''' Modified by: SA 16/12/2014 - BA-1972 ==> Changed both sub-queries to get also field ControlID (positions with NOT IN USE Controls should indicate 
        '''                                          the ID of the Control)
        '''              SA 08/01/2015 - BA-1999 ==> Changed the first subquery to get only Positions with ElementID different of NULL when Status is different
        '''                                          of NOT IN USE (to exclude NOT IN USE Positions with Status DEPLETED or FEW), and to add a new condition 
        '''                                          to get also FREE Positions (which include Positions with BarcodeStatus ERROR or UNKNOWN). 
        '''                                          Changed the second subquery by removing the filter by Status (it is not needed due to an INNER JOIN is used) to 
        '''                                          allow also to get NOT IN USE Positions with Status DEPLETED or FEW)
        '''              SA 09/01/2015 - BA-1999 ==> Changed the first subquery to remove the LEFT OUTER JOIN with table twksWSRotorPositionsInProcess due to this 
        '''                                          function is called when the WorkSession Status is EMPTY or OPEN and there are not Positions In Process
        '''         ''' </remarks>
        Public Function GetRotorContentPositionsResetDone(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) _
                                                 As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, " & vbCrLf & _
                                                       " RCP.MultiTubeNumber, RCP.TubeType, RCP.RealVolume, RCP.RemainingTestsNumber, RCP.Status, " & vbCrLf & _
                                                       " RCP.ScannedPosition, RCP.BarcodeInfo, RCP.BarcodeStatus, RE.TubeContent, RE.ElementStatus, RE.CalibratorID, " & vbCrLf & _
                                                       " RE.MultiItemNumber, RE.ReagentID, RE.SolutionCode, RE.ControlID, 0 AS InProcessElement " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP LEFT OUTER JOIN twksWSRequiredElements RE " & vbCrLf & _
                                                                                                     " ON RCP.WorkSessionID = RE.WorkSessionID " & vbCrLf & _
                                                                                                    " AND RCP.ElementID     = RE.ElementID " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND  ((RCP.Status        <> 'NO_INUSE' AND RCP.ElementID IS NOT NULL) " & vbCrLf & _
                                                " OR    (RCP.Status        = 'FREE')) " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT DISTINCT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, " & vbCrLf & _
                                                                " RCP.ElementID, RCP.MultiTubeNumber, RCP.TubeType, RCP.RealVolume, RCP.RemainingTestsNumber, " & vbCrLf & _
                                                                " (CASE WHEN NU.Status IS NULL THEN RCP.Status ELSE NU.Status END), NU.ScannedPosition, " & vbCrLf & _
                                                                " NU.BarcodeInfo, NU.BarcodeStatus, NU.TubeContent, " & vbCrLf & _
                                                                " NULL as ElementStatus, NU.CalibratorID, NU.MultiItemNumber, NU.ReagentID, " & vbCrLf & _
                                                                " NU.SolutionCode, NU.ControlID, 0 AS InProcessElement  " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSNotInUseRotorPositions NU " & vbCrLf & _
                                                                                                " ON RCP.AnalyzerID    = NU.AnalyzerID " & vbCrLf & _
                                                                                               " AND RCP.RotorType     = NU.RotorType " & vbCrLf & _
                                                                                               " AND RCP.RingNumber    = NU.RingNumber " & vbCrLf & _
                                                                                               " AND RCP.CellNumber    = NU.CellNumber " & vbCrLf & _
                                                                                               " AND RCP.WorkSessionID = NU.WorkSessionID " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetRotorContentPositionsResetDone", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified OrderTestID, search Rotor Positions in which its required Elements are placed (the searching is done according value of 
        ''' entry parameters pOnlySamplesRotor and pOnlyExecutionSamplePosition
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pOnlySamplesRotor">When TRUE, it indicates the searching is done only in SAMPLES Rotor; otherwise, the searching is done in both
        '''                                 Rotors, Samples and Reagents</param>
        ''' <param name="pOnlyExecutionSamplePosition">When TRUE it indicates the searching is only for Rotor positions in which the Sample tube is placed;
        '''                                            otherwise, positions for all related required Elements are searched</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of Rotor Positions related with the informed Order Test</returns>
        ''' <remarks>
        ''' Created by:  AG 04/07/2011
        ''' Modified by: SA 30/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetRotorPositionsByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pOnlySamplesRotor As Boolean, _
                                                       ByVal pOnlyExecutionSamplePosition As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT REOT.OrderTestID , REOT.ElementID , RCP.CellNumber , RCP.RotorType , RCP.Status " & vbCrLf & _
                                                " FROM   twksWSExecutions EX INNER JOIN twksWSRequiredElemByOrderTest REOT ON EX.OrderTestID = REOT.OrderTestID " & vbCrLf & _
                                                                           " INNER JOIN twksWSRotorContentByPosition RCP ON REOT.ElementID = RCP.ElementID " & vbCrLf & _
                                                                           " INNER JOIN twksWSRequiredElements RE ON REOT.ElementID = RE.ElementID " & vbCrLf & _
                                                " WHERE EX.OrderTestID = " & pOrderTestID & vbCrLf

                        If (pOnlySamplesRotor) Then cmdText &= " AND RCP.RotorType = 'SAMPLES' " & vbCrLf
                        If (pOnlyExecutionSamplePosition) Then cmdText &= " AND ((EX.SampleClass = RE.TubeContent AND EX.SampleClass <> 'BLANK') " & vbCrLf & _
                                                                          " OR   (RE.TubeContent = 'TUBE_SPEC_SOL' AND EX.SampleClass = 'BLANK')) " & vbCrLf

                        Dim resultQuery As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultQuery.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        resultData.SetDatos = resultQuery
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetRotorPositionsByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information of all Ring Cells (status and content) in the specified Rotor of an Analyzer included in a Work Session
        ''' (Copied from GetRotorContentPositions and modified by adding it a filter by RotorType)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with information of 
        '''          all Cells in the specified Analyzer Rotor</returns>
        ''' <remarks>
        ''' Created by:  AG 25/01/2010 (Tested Pending)
        ''' Modified by: SA 26/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        ''' </remarks>
        Public Function GetRotorTypeContentPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     ByVal pWorkSessionID As String, ByVal pRotorType As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'TR - AG 25/01/2010 -Add the table twksWSNotInUseRotorPositions to get th information for the elements witch status is NO_INUSE.
                        'Add the RotorType in Where clauses
                        'SQL Sentence to get data
                        Dim cmdText As String
                        cmdText = " SELECT RCP.*, RE.TubeContent, RE.ElementStatus  " & _
                                  " FROM   twksWSRotorContentByPosition  RCP LEFT OUTER JOIN twksWSRequiredElements RE " & _
                                                                    " ON RCP.WorkSessionID = RE.WorkSessionID AND RCP.ElementID = RE.ElementID " & _
                                  " WHERE  RCP.AnalyzerID = '" & pAnalyzerID & "' " & _
                                  " AND    RCP.WorkSessionID = '" & pWorkSessionID & "' " & _
                                  " AND    RCP.RotorType = '" & pRotorType & "' " & _
                                  " AND    RCP.Status <> 'NO_INUSE' " & _
                                  " UNION " & _
                                  " SELECT DISTINCT RCP.*, NU.TubeContent, NULL as ElementStatus " & _
                                  " FROM   twksWSRotorContentByPosition  RCP, twksWSNotInUseRotorPositions NU " & _
                                  " WHERE  RCP.AnalyzerID = '" & pAnalyzerID & "' " & _
                                  " AND    RCP.WorkSessionID = '" & pWorkSessionID & "' " & _
                                  " AND    RCP.RotorType = '" & pRotorType & "' " & _
                                  " AND    RCP.Status = 'NO_INUSE' " & _
                                  " AND    NU.WorkSessionID = RCP.WorkSessionID  " & _
                                  " AND    NU.AnalyzerID = RCP.AnalyzerID " & _
                                  " AND    NU.RotorType = RCP.RotorType " & _
                                  " AND    NU.RingNumber = RCP.RingNumber " & _
                                  " AND    NU.CellNumber = RCP.CellNumber"
                        'TR - AG 25/01/2010 END

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim resultData As New WSRotorContentByPositionDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetRotorTypeContentPositions", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

       	''' <summary>
        ''' Get information of all non free positions in the Samples Rotor for the active WorkSession, including those that contain
        ''' Not In Use elements. Used to save the rotor as an internal Virtual Rotor before reset the WS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pVirtualRotorID"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet VirtualRotorPositionsDS with the content of all non-free
        '''          positions in the Samples Rotor for the active WorkSession</returns>
        ''' <remarks>
        ''' Created by:  DL 04/08/2011
        ''' Modified by: AG 03/02/2012 - Changed the query to get also the position status (field Status)
        '''              SA 06/02/2012 - Changed the function template; for NOT IN USE Positions, get field Status from table twksWSNotInUseRotorPositions
        '''                              instead of from table twksWSRotorContentByPosition
        '''              SA 08/01/2015 - BA-1999 ==> Changed the second subquery to remove the filter by Position Status = 'NO_INUSE', to allow to get also 
        '''                                          NOT IN USE Positions with Status DEPLETED
        ''' </remarks>
        Public Function GetSamplesRotorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String, ByVal pVirtualRotorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT " & pVirtualRotorID.ToString & " AS VirtualRotorID, RCP.RingNumber, RCP.CellNumber, RE.TubeContent, RCP.TubeType, " & vbCrLf & _
                                                         " RE.CalibratorID, RE.ControlID, RE.SolutionCode, RCP.BarCodeInfo, RCP.BarcodeStatus, RCP.ScannedPosition, RCP.Status, " & vbCrLf & _
                                                         " 'BIOSYSTEMS' AS TS_User, GETDATE() AS TS_DateTime, RE.MultiItemNumber " & vbCrLf & _
                                                " FROM    twksWSRotorContentByPosition RCP INNER JOIN twksWSRequiredElements RE ON RCP.ElementID = RE.ElementID " & vbCrLf & _
                                                " WHERE   RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND     RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND     RCP.RotorType     = 'SAMPLES' " & vbCrLf & _
                                                " AND     RE.TubeContent <> 'PATIENT' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT " & pVirtualRotorID.ToString & " AS VirtualRotorID, RCP.RingNumber, RCP.CellNumber, NIU.TubeContent, RCP.TubeType, " & vbCrLf & _
                                                         " NIU.CalibratorID, NIU.ControlID, NIU.SolutionCode, RCP.BarcodeInfo, RCP.BarcodeStatus, RCP.ScannedPosition, NIU.Status, " & vbCrLf & _
                                                         " 'BIOSYSTEMS' AS TS_User, GETDATE() AS TS_DateTime, NIU.MultiItemNumber " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSNotInUseRotorPositions NIU ON RCP.AnalyzerID    = NIU.AnalyzerID " & vbCrLf & _
                                                                                                                                   " AND RCP.WorkSessionID = NIU.WorkSessionID " & vbCrLf & _
                                                                                                                                   " AND RCP.RotorType     = NIU.RotorType " & vbCrLf & _
                                                                                                                                   " AND RCP.RingNumber    = NIU.RingNumber " & vbCrLf & _
                                                                                                                                   " AND RCP.CellNumber    = NIU.CellNumber " & vbCrLf & _
                                                " WHERE RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND   RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND   RCP.RotorType     = 'SAMPLES' " & vbCrLf & _
                                                " AND   NIU.TubeContent  <> 'PATIENT' " & vbCrLf & _
                                                " ORDER BY RCP.RingNumber, RCP.CellNumber "

                        Dim myVRotorPosDS As New VirtualRotorPosititionsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myVRotorPosDS.tparVirtualRotorPosititions)
                            End Using
                        End Using

                        resultData.SetDatos = myVRotorPosDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.GetSamplesRotorPositions", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the Rotor Cells containing bottles of the specified Washing Solution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSolutionCode">Code for the Washing Solution</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the list of Rotor Cells
        '''          containing bottles of the specified Washing Solution</returns>
        ''' <remarks>
        ''' Created by:  TR 28/01/2011
        ''' Modified by: RH 17/06/2011 - Added filter for Solution Code TUBE_WASH_SOL
        '''              AG 03/08/2011 - Get also field BarcodeStatus
        ''' </remarks>
        Public Function GetWashingSolutionPosInfoBySolutionCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSolutionCode As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RE.ElementID, RE.SolutionCode, " & vbCrLf & _
                                                       " RE.TubeContent, RCP.TubeType, RCP.Status, RCP.BarcodeStatus " & vbCrLf & _
                                                " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSRequiredElements RE ON RCP.ElementID = RE.ElementID " & vbCrLf & _
                                                " WHERE  RE.SolutionCode = '" & pSolutionCode.Trim & "' " & vbCrLf & _
                                                " AND   (RE.TubeContent = 'TUBE_WASH_SOL' OR RE.TubeContent = 'WASH_SOL') "

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ReadByElementIDAndWorkSessionID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the specified Rotor Cell or if a Cell is not informed, get data of all Cells in the Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">If -1 read by AnalyzerID, WorkSessionID and RotorType</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the information obtained</returns>
        ''' <remarks>
        ''' Created by:  AG 09/06/2011
        ''' Modified by: AG 05/09/2011 - Chanqed Query to also return TubeContent (different tables depending if InUse or NOT InUse)
        '''              SA 05/09/2011 - Changed the query to return also the Tube Content of Not InUse cells that already exist in table
        '''                              of NotInUseRotorPositions (those loaded from the previous saved Rotor). Removed parameter pNotInUsedCreatedFlag
        '''              SA 07/01/2015 - BA-1999 ==> When value of parameter pGetNotInUseStatus is FALSE (default case), all NOT IN USE Positions are returned
        '''                                          with Status = NO_INUSE (also for Rotor Positions saved as DEPLETED, LOCKED or FEW). Changed the condition
        '''                                          to get NOT IN USE Positions: (Status different of FREE AND ElementID = NULL) instead of Status = NO_INUSE. 
        '''                                          Added a new subquery to get only FREE Positions. Changed the second subquery to get only INUSE Positions
        '''                                          (searching Positions with Status different of FREE AND ElementID different of NULL)  
        ''' </remarks>
        Public Function ReadByCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                         ByVal pRotorType As String, ByVal pCellNumber As Integer, Optional ByVal pGetNotInUseStatus As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        ''Not INUSE
                        ''SA + JV - 12/12/2013 - #1384
                        'Dim notInUseStatus As String = String.Empty
                        'If (Not pGetNotInUseStatus) Then
                        '    notInUseStatus = "RCP.Status, "
                        'Else
                        '    notInUseStatus = "(CASE WHEN NU.Status IS NULL THEN RCP.Status ELSE NU.Status END) AS Status, "
                        'End If

                        'cmdText = " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, RCP.MultiTubeNumber, " & vbCrLf & _
                        '                 " RCP.TubeType, RCP.RealVolume, RCP.RemainingTestsNumber, " & notInUseStatus & vbCrLf & _
                        '                 " NU.TubeContent, RCP.ScannedPosition, RCP.BarcodeInfo, RCP.BarcodeStatus, " & vbCrLf & _
                        '                 " RCP.TS_User, RCP.TS_DateTime, NULL AS ElementStatus, NU.ReagentID, NU.SolutionCode, NU.CalibratorID, NU.ControlID, NU.MultiItemNumber " & vbCrLf & _
                        '              " FROM   twksWSRotorContentByPosition RCP LEFT OUTER JOIN twksWSNotInUseRotorPositions NU ON RCP.AnalyzerID = NU.AnalyzerID " & vbCrLf & _
                        '                                                                                                     " AND RCP.RotorType  = NU.RotorType " & vbCrLf & _
                        '                                                                                                     " AND RCP.RingNumber = NU.RingNumber " & vbCrLf & _
                        '                                                                                                     " AND RCP.CellNumber = NU.CellNumber  " & vbCrLf & _
                        '              " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                        '              " AND    RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                        '              " AND    RCP.RotorType     = '" & pRotorType & "' " & vbCrLf & _
                        '              " AND    RCP.Status        = 'NO_INUSE' " & vbCrLf

                        'If (pCellNumber <> -1) Then cmdText &= " AND   RCP.CellNumber = " & pCellNumber.ToString() & " " & vbCrLf
                        ''FIN 12/12/2013

                        ''OTHERS: INUSE, FREE, ...
                        'cmdText &= " UNION " & vbCrLf & _
                        '           " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, RCP.MultiTubeNumber, RCP.TubeType, " & vbCrLf & _
                        '                  " RCP.RealVolume, RCP.RemainingTestsNumber, RCP.Status, RE.TubeContent, RCP.ScannedPosition, RCP.BarcodeInfo, RCP.BarcodeStatus, " & vbCrLf & _
                        '                  " RCP.TS_User, RCP.TS_DateTime, RE.ElementStatus, RE.ReagentID, RE.SolutionCode, RE.CalibratorID, RE.ControlID, RE.MultiItemNumber " & vbCrLf & _
                        '           " FROM   twksWSRotorContentByPosition RCP LEFT OUTER JOIN twksWSRequiredElements RE ON RCP.ElementID = RE.ElementID " & vbCrLf & _
                        '           " WHERE  RCP.AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                        '           " AND    RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                        '           " AND    RCP.RotorType = '" & pRotorType & "' " & vbCrLf & _
                        '           " AND    RCP.Status <> 'NO_INUSE' "

                        'If (pCellNumber <> -1) Then cmdText &= " AND   RCP.CellNumber = " & pCellNumber.ToString() & " " & vbCrLf

                        'BA-1999: Changes in the way the Rotor Positions are obtained
                        Dim cmdText As String = String.Empty

                        '(1) GET NOT INUSE ROTOR POSITIONS ==> NOT FREE Positions with field ElementID NOT INFORMED
                        'For NOT IN USE Positions, the Status of the Rotor Position will be obtained in the following way:
                        '** If the Status of the active WS is EMPTY or OPEN (pGetNotInUseStatus is FALSE): 
                        '    - The NO_INUSE Status is returned (instead of field Status from twksWSRotorContentByPosition due to it can be FEW, DEPLETED or LOCKED)
                        '** Else (pGetNotInUseStatus is TRUE): 
                        '    - Field Status from table twksWSNotInUseRotorPositions if it is informed (only when the Not In Use Position is FEW, DEPLETED or LOCKED)
                        '    - Otherwise, field Status from table twksWSRotorContentByPosition (in this case it will be always NO_INUSE)
                        Dim notInUseStatus As String = String.Empty
                        If (Not pGetNotInUseStatus) Then
                            'NOT IN USE Positions can have Status LOCKED, FEW or DEPLETED; in this case, the NO_INUSE Status has to be returned 
                            'due to the Status of the active WorkSession is PENDING, INPROCESS or CLOSED
                            notInUseStatus = "'NO_INUSE' AS Status, "
                        Else
                            notInUseStatus = "(CASE WHEN NU.Status IS NULL THEN RCP.Status ELSE NU.Status END) AS Status, "
                        End If

                        cmdText = " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, RCP.MultiTubeNumber, " & vbCrLf & _
                                         " RCP.TubeType, RCP.RealVolume, RCP.RemainingTestsNumber, " & notInUseStatus & vbCrLf & _
                                         " NU.TubeContent, RCP.ScannedPosition, RCP.BarcodeInfo, RCP.BarcodeStatus, " & vbCrLf & _
                                         " RCP.TS_User, RCP.TS_DateTime, NULL AS ElementStatus, NU.ReagentID, NU.SolutionCode, NU.CalibratorID, NU.ControlID, NU.MultiItemNumber " & vbCrLf & _
                                  " FROM   twksWSRotorContentByPosition RCP LEFT OUTER JOIN twksWSNotInUseRotorPositions NU ON RCP.AnalyzerID = NU.AnalyzerID " & vbCrLf & _
                                                                                                                         " AND RCP.RotorType  = NU.RotorType " & vbCrLf & _
                                                                                                                         " AND RCP.RingNumber = NU.RingNumber " & vbCrLf & _
                                                                                                                         " AND RCP.CellNumber = NU.CellNumber  " & vbCrLf & _
                                  " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                  " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                  " AND    RCP.RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                  " AND   (RCP.Status <> 'FREE' AND RCP.ElementID IS NULL) " & vbCrLf
                        If (pCellNumber <> -1) Then cmdText &= " AND   RCP.CellNumber = " & pCellNumber.ToString() & " " & vbCrLf

                        '(2) GET INUSE ROTOR POSITIONS ==> NOT FREE Positions with field ElementID INFORMED (INNER JOIN with table twksWSRequiredElements)
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, RCP.MultiTubeNumber, RCP.TubeType, " & vbCrLf & _
                                          " RCP.RealVolume, RCP.RemainingTestsNumber, RCP.Status, RE.TubeContent, RCP.ScannedPosition, RCP.BarcodeInfo, RCP.BarcodeStatus, " & vbCrLf & _
                                          " RCP.TS_User, RCP.TS_DateTime, RE.ElementStatus, RE.ReagentID, RE.SolutionCode, RE.CalibratorID, RE.ControlID, RE.MultiItemNumber " & vbCrLf & _
                                   " FROM   twksWSRotorContentByPosition RCP INNER JOIN twksWSRequiredElements RE ON RCP.ElementID = RE.ElementID " & vbCrLf & _
                                   " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                   " AND    RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                   " AND    RCP.RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                   " AND    RCP.Status       <> 'NO_INUSE' " & vbCrLf
                        If (pCellNumber <> -1) Then cmdText &= " AND   RCP.CellNumber = " & pCellNumber.ToString() & " " & vbCrLf

                        '(3) GET FREE ROTOR POSITIONS
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT RCP.AnalyzerID, RCP.RotorType, RCP.RingNumber, RCP.CellNumber, RCP.WorkSessionID, RCP.ElementID, RCP.MultiTubeNumber, RCP.TubeType, " & vbCrLf & _
                                          " RCP.RealVolume, RCP.RemainingTestsNumber, RCP.Status, NULL AS TubeContent, RCP.ScannedPosition, RCP.BarcodeInfo, RCP.BarcodeStatus, " & vbCrLf & _
                                          " RCP.TS_User, RCP.TS_DateTime, NULL AS ElementStatus, NULL AS ReagentID, NULL AS SolutionCode, NULL AS CalibratorID, NULL AS ControlID, NULL AS MultiItemNumber " & vbCrLf & _
                                " FROM   twksWSRotorContentByPosition RCP " & vbCrLf & _
                                " WHERE  RCP.AnalyzerID    = '" & pAnalyzerID & "' " & vbCrLf & _
                                " AND    RCP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                " AND    RCP.RotorType     = '" & pRotorType & "' " & vbCrLf & _
                                " AND    RCP.Status        = 'FREE' " & vbCrLf
                        If (pCellNumber <> -1) Then cmdText &= " AND   RCP.CellNumber = " & pCellNumber.ToString() & " " & vbCrLf

                        '(4) SORT ROTOR POSITIONS BY CELL NUMBER
                        cmdText &= " ORDER BY RCP.CellNumber " & vbCrLf

                        Dim resultData As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ReadByCellNumber", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Clear (or delete) all positions of the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier, defined as optinal in order to change less code as possible</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all Elements that were positioned 
        '''          in the reseted Rotor with the updated ElementStatus to NOPOS</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: AG 03/12/2009 - Added pWorkSessionID optional parameter (tested pending)
        '''              SA 26/01/2010 - Removed return of error "NO RECORD UPDATE"; this message does not exist
        '''                              Removed use and declaration of variable dbConnection and removed Finally section; 
        '''                              these are not needed for this type of functions
        ''' </remarks>
        Public Function ResetRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                   ByVal pRotorType As String, Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmd As SqlCommand
                    cmd = pDBConnection.CreateCommand

                    Dim cmdText As String = " UPDATE twksWSRotorContentByPosition " & _
                                            " SET    Status = 'FREE', " & _
                                            "        ElementID = NULL, " & _
                                            "        MultiTubeNumber = NULL, " & _
                                            "        RealVolume = NULL, " & _
                                            "        RemainingTestsNumber = NULL, " & _
                                            "        BarcodeInfo = NULL, " & _
                                            "        BarCodeStatus = NULL, " & _
                                            "        TubeType = NULL, " & _
                                            "        ScannedPosition = NULL " & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & _
                                            " AND    RotorType = '" & pRotorType & "' "

                    If pWorkSessionID.Trim <> "" Then
                        cmdText = cmdText & " AND WorkSessionID = '" & pWorkSessionID.ToString() & "'"
                    End If

                    cmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()

                    If Not myGlobalDataTO.AffectedRecords > 0 Then
                        myGlobalDataTO.HasError = True
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ResetRotor", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Rotor Positions for the specified Analyzer and Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " DELETE twksWSRotorContentByPosition " & _
                              " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & _
                              " AND    WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

                    Dim cmd As SqlCommand
                    cmd = pDBConnection.CreateCommand
                    cmd.CommandText = cmdText

                    resultData.AffectedRecords = cmd.ExecuteNonQuery()
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set Status = 'FREE' for Scanned positions in Samples Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: TR 26/04/2013
        '''</remarks>
        Public Function SetScannedPositionToFREE(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                 ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand

                    dbCmd.Connection = pDBConnection
                    cmdText &= " UPDATE twksWSRotorContentByPosition "
                    cmdText &= " SET "
                    cmdText &= " STATUS = 'FREE', "
                    cmdText &= " BarCodeInfo = NULL, "
                    cmdText &= " BarcodeStatus = NULL, "
                    cmdText &= " ScannedPosition = NULL, "
                    cmdText &= " MultiTubeNumber = NULL "
                    cmdText &= " WHERE RotorType = '" & GlobalEnumerates.Rotors.SAMPLES.ToString() & "' "
                    cmdText &= " AND (Status = 'NO_INUSE' "
                    'AJG
                    'cmdText &= " AND CellNumber NOT IN (SELECT CellNumber FROM twksWSNotInUseRotorPositions "
                    cmdText &= " AND NOT EXISTS (SELECT CellNumber FROM twksWSNotInUseRotorPositions "
                    cmdText &= "                WHERE AnalyzerID = '" & pAnalyzerID & "' "
                    'AJG
                    cmdText &= "                AND twksWSRotorContentByPosition.CellNumber = CellNumber"
                    cmdText &= "                AND WorkSessionID = '" & pWorkSessionID & "' "
                    cmdText &= "                AND RotorType = '" & GlobalEnumerates.Rotors.SAMPLES.ToString() & "')) "
                    cmdText &= " OR BarcodeStatus = 'ERROR' "

                    If (cmdText <> "") Then
                        dbCmd.CommandText = cmdText
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.SetScannedPositionToFREE", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update ScannedPosition and the rest of Barcode fields of a Rotor position with the data read from Analyzer (BarCodeInfo and BarCodeStatus)
        ''' When parameter pAdditionalFieldsFlag is TRUE: 
        '''  ** Fields ElementID, MultiTubeNumber and TubeType are also informed
        '''  ** Fields RealVolume and Status are also informed but only when they are informed in the entry DS (that is to say, they are not set to NULL)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeDS">Typed DataSet WSRotorContentByPositionDS with information of the Rotor position to update</param>
        ''' <param name="pAdditionalFieldsFlag">When FALSE, only Barcode fields are updated (BarcodeStatus, BarcodeInfo, ScannedPosition)
        '''                                     When TRUE, besides Barcode fields, update also fields ElementID, MultiTubeNumber, TubeType, 
        '''                                     RealVolume and Status)</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 22/06/2011 - tested OK
        ''' Modified by: AG 05/10/2011 - If length of field BarcodeInfo is greater than 100 characters, use only the first 100, that is the maximum
        '''                              length allowed for the field in DB 
        '''              SA 02/04/2012 - When pAdditionalFieldsFlags is TRUE, update also the RealVolume field
        '''              TR 13/06/2012 - Added the update of Remaining Tests number into the update
        '''              JB 08/11/2012 - Fixed query with codeBar with ' char and non-ANSI chars
        '''              SA 15/11/2012 - Changed the SQL to avoid errors when updating field BarcodeInfo: validation BarcodeInfo = String.Empty
        '''                              has to be inside the condition "Not IsBarcodeInfoNULL" instead of in the Else; otherwise, an error is
        '''                              raised because BarcodeInfo is NULL 
        ''' </remarks>
        Public Function UpdateBarCodeFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeDS As WSRotorContentByPositionDS, _
                                            ByVal pAdditionalFieldsFlag As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pBarCodeDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim cmdAddComma As String = ""
                    Dim STR_NULL As String = " NULL "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pBarCodeDS.twksWSRotorContentByPosition
                        If (Not row.IsBarCodeInfoNull Or Not row.IsBarcodeStatusNull) Then
                            cmdText &= " UPDATE twksWSRotorContentByPosition "
                            cmdText &= " SET "

                            If (Not row.IsBarcodeStatusNull) Then
                                cmdText &= " BarcodeStatus = " & String.Format("'{0}'", row.BarcodeStatus)
                                cmdAddComma = ", "
                            End If

                            If (Not row.IsBarCodeInfoNull) Then
                                If (row.BarCodeInfo = String.Empty) Then
                                    cmdText &= cmdAddComma & " BarCodeInfo = " & STR_NULL
                                Else
                                    'Length of field BarcodeInfo cannot be greater than 100 characters
                                    If (row.BarCodeInfo.Length > 100) Then
                                        cmdText &= cmdAddComma & " BarCodeInfo = " & String.Format(" N'{0}'", row.BarCodeInfo.Substring(0, 100).Replace("'", "''"))
                                    Else
                                        cmdText &= cmdAddComma & " BarCodeInfo = " & String.Format(" N'{0}'", row.BarCodeInfo.Replace("'", "''"))
                                    End If
                                End If
                                cmdAddComma = ", "

                                'ElseIf (row.BarCodeInfo = String.Empty) Then
                                '    cmdText &= cmdAddComma & " BarCodeInfo = " & STR_NULL
                                '    cmdAddComma = " , "
                            Else
                                'This condition do nothing due some methods call it with BarCodeInfo as NULL but it is informed and it can be deleted
                                'cmdText &= " , BarCodeInfo = " & STR_NULL
                            End If

                            If (Not row.IsScannedPositionNull) Then
                                cmdText &= cmdAddComma & " ScannedPosition = " & CStr(IIf(row.ScannedPosition, 1, 0))
                            Else
                                cmdText &= cmdAddComma & " ScannedPosition = " & STR_NULL
                            End If
                            cmdAddComma = " , "

                            'Additional fields to update during barcode management business process
                            If (pAdditionalFieldsFlag) Then
                                'ElementID
                                If (Not row.IsElementIDNull) Then
                                    cmdText &= " , ElementID = " & row.ElementID
                                Else
                                    cmdText &= " , ElementID = " & STR_NULL
                                End If

                                'MultiTubeNumber
                                If (Not row.IsMultiTubeNumberNull) Then
                                    cmdText &= " , MultiTubeNumber = " & row.MultiTubeNumber
                                Else
                                    cmdText &= " , MultiTubeNumber = " & STR_NULL
                                End If

                                'TubeType
                                If (Not row.IsTubeTypeNull) Then
                                    cmdText &= " , TubeType = " & String.Format("'{0}'", row.TubeType)
                                Else
                                    cmdText &= " , TubeType = " & STR_NULL
                                End If

                                'RealVolume 
                                If (Not row.IsRealVolumeNull) Then
                                    cmdText &= " , RealVolume = " & ReplaceNumericString(row.RealVolume)
                                End If

                                'Remaining Tests Number
                                If (Not row.IsRemainingTestsNumberNull) Then
                                    cmdText &= " , RemainingTestsNumber = " & ReplaceNumericString(row.RemainingTestsNumber)
                                End If

                                'Status
                                If (Not row.IsStatusNull) Then
                                    cmdText &= " , Status = " & String.Format("'{0}'", row.Status)
                                End If
                            End If

                            cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", row.AnalyzerID)
                            cmdText &= " AND WorksessionID = " & String.Format("'{0}'", row.WorkSessionID)
                            cmdText &= " AND CellNumber = " & row.CellNumber
                            cmdText &= " AND RotorType = " & String.Format("'{0}'", row.RotorType)

                            'Insert line break 
                            cmdText &= String.Format("{0}", vbNewLine)
                        End If
                    Next

                    If (cmdText <> "") Then
                        dbCmd.CommandText = cmdText
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.UpdateBarCodeFields", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Status of the specified Rotor Position and, if parameter pUpdateOnlyStatusFlag=FALSE, update also fields
        ''' RealVolume and RemainingTestsNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID" >Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <param name="pStatus">Status to set to the Rotor Cell</param>
        ''' <param name="pRealVolume">Reagent Volume</param>
        ''' <param name="pTestLeft">Number of Tests that can be executed with the Reagent Volume</param>
        ''' <param name="pUpdateOnlyStatusFlag">When TRUE, indicates that only the Status field has to be updated</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 04/01/2011 
        ''' Modified by: AG 30/03/2011 - Added parameters for Real Volume and Number of Remaining Tests 
        '''              AG 13/05/2011 - If RotorType = 'SAMPLES', set to NULL fields RealVolume and RemainingTestNumber
        '''              AG 26/07/2011 - Added parameter pOnlyUpdateStatusFlag
        '''              XB 20/03/2014 - Add parameter HResult into Try Catch section - #1548
        ''' </remarks>
        Public Function UpdateByRotorTypeAndCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                       ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pStatus As String, ByVal pRealVolume As Single, _
                                                       ByVal pTestLeft As Integer, ByVal pUpdateOnlyStatusFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSRotorContentByPosition " & vbCrLf & _
                                            " SET    Status = '" & pStatus & "' " & vbCrLf

                    If (Not pUpdateOnlyStatusFlag) Then
                        If (pRotorType = "REAGENTS") Then
                            cmdText &= " , RealVolume = " & ReplaceNumericString(pRealVolume) & vbCrLf & _
                                       " , RemainingTestsNumber = " & ReplaceNumericString(pTestLeft) & vbCrLf
                        Else
                            cmdText &= " , RealVolume = NULL " & vbCrLf & _
                                       " , RemainingTestsNumber = NULL " & vbCrLf
                        End If
                    End If

                    cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "' " & vbCrLf & _
                               " AND   WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                               " AND   RotorType     = '" & pRotorType & "' " & vbCrLf & _
                               " AND   CellNumber    = " & pCellNumber

                    Dim cmd As SqlCommand = pDBConnection.CreateCommand
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    myGlobalDataTO.HasError = False
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksWSRotorContentByPositionDAO.UpdateByRotorTypeAndCellNumber", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update TubeType for all points of the specified Calibrator positioned in SAMPLES Rotor. The Calibrator can be IN USE or NOT IN USE in the
        ''' active WorkSession. For IN USE Calibrators, field Status is also updated
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pCalibratorID">Element Identifier</param>
        ''' <param name="pNewTubeType">New TubeType for all Calibrator Tube</param>
        ''' <param name="pNewElementStatus">Optional parameter. For IN USE Calibrators, it is the new Element Status to assign to all Calibrator Elements</param>
        ''' <returns>GlobalDataTO containing succes/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 01/08/2011
        ''' Modified by: SA 10/02/2012 - Implementation changed due to, NOT IN USE Calibrators have to be also updated
        ''' </remarks>
        Public Function UpdateCalibStatusAndTubeType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                     ByVal pCalibratorID As Integer, ByVal pNewTubeType As String, Optional ByVal pNewElementStatus As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE twksWSRotorContentByPosition " & vbCrLf
                    If (pNewElementStatus <> String.Empty) Then
                        'It is an IN USE Calibrator, update fields Status and TubeType for all Calibrator kit positions - search them in table of required WS Elements
                        'AJG
                        'cmdText &= " SET    TubeType = '" & pNewTubeType.Trim & "', " & vbCrLf & _
                        '                  " Status   = '" & pNewElementStatus.Trim & "' " & vbCrLf & _
                        '           " WHERE (ElementID IN (SELECT ElementID FROM twksWSRequiredElements " & vbCrLf & _
                        '                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                                " AND    TubeContent   = 'CALIB' " & vbCrLf & _
                        '                                " AND    CalibratorID  = " & pCalibratorID.ToString & ")) " & vbCrLf

                        cmdText &= " SET    TubeType = '" & pNewTubeType.Trim & "', " & vbCrLf & _
                                          " Status   = '" & pNewElementStatus.Trim & "' " & vbCrLf & _
                                   " WHERE EXISTS (SELECT ElementID FROM twksWSRequiredElements " & vbCrLf & _
                                                        " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                        " AND    TubeContent   = 'CALIB' " & vbCrLf & _
                                                        " AND    CalibratorID  = " & pCalibratorID.ToString & " AND twksWSRotorContentByPosition.ElementID = ElementID) " & vbCrLf
                    Else
                        'It is a NOT IN USE Calibrator, update field TubeType for all Calibrator kit positions - search them in table of not in use WS Elements
                        'AJG
                        'cmdText &= " SET    TubeType = '" & pNewTubeType.Trim & "' " & vbCrLf & _
                        '           " WHERE  (CellNumber IN (SELECT CellNumber FROM twksWSNotInUseRotorPositions " & vbCrLf & _
                        '                                  " WHERE AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                        '                                  " AND   WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                                  " AND   RotorType     = 'SAMPLES' " & vbCrLf & _
                        '                                  " AND   TubeContent   = 'CALIB' " & vbCrLf & _
                        '                                  " AND   CalibratorID  = " & pCalibratorID.ToString & ")) " & vbCrLf

                        cmdText &= " SET    TubeType = '" & pNewTubeType.Trim & "' " & vbCrLf & _
                                   " WHERE  EXISTS (SELECT CellNumber FROM twksWSNotInUseRotorPositions " & vbCrLf & _
                                                          " WHERE AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                          " AND   WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                          " AND   RotorType     = 'SAMPLES' " & vbCrLf & _
                                                          " AND   TubeContent   = 'CALIB' " & vbCrLf & _
                                                          " AND   CalibratorID  = " & pCalibratorID.ToString & " AND twksWSRotorContentByPosition.CellNumber = CellNumber) " & vbCrLf
                    End If

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.UpdateCalibStatusAndTubeType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update values of  group of Not In Use Positions when a required WorkSession Element is found for them.  
        ''' Fields ElementID and Status are updated, and the Rotor Position is removed from the table of Not In Use positions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pNotInUsePositionDS">DataSet containing information of all Not In Use Rotor Positions that have to be 
        '''                                   updated due to a required Work Session Element was found for them</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 25/01/2010 - (Tested : OK) Tested for REAGENTS flow
        ''' Modified by: SA 26/01/2010 - Removed return of error "NO RECORD DELETE"; this message does not exist
        '''                              Removed use and declaration of variable dbConnection and removed Finally section; 
        '''                              these are not needed for this type of functions. Function moved to Other Methods Region
        ''' </remarks>
        Public Function UpdateNotInUseRotorPosition(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                    ByVal pNotInUsePositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    For Each rotorPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pNotInUsePositionDS.twksWSRotorContentByPosition
                        cmdText = " UPDATE twksWSRotorContentByPosition " & vbCrLf & _
                                  " SET    ElementID = " & rotorPos.ElementID & ", " & vbCrLf & _
                                  "        Status = '" & rotorPos.Status & "' " & vbCrLf & _
                                  " WHERE AnalyzerID = '" & rotorPos.AnalyzerID.Trim & "' " & vbCrLf & _
                                  " AND   WorkSessionID = '" & rotorPos.WorkSessionID.Trim & "' " & vbCrLf & _
                                  " AND   RotorType = '" & rotorPos.RotorType.Trim & "' " & vbCrLf & _
                                  " AND   RingNumber = " & rotorPos.RingNumber.ToString & vbCrLf & _
                                  " AND   CellNumber = " & rotorPos.CellNumber.ToString & vbCrLf

                        Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                            myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                        End Using
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.UpdateNotInUseRotorPosition", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified ElementID, which corresponds to an Element required in the Analyzer WorkSession that is marked as not positioned,
        ''' verify if there are positioned tubes (only those with status different of DEPLETED if parameter pExcludeDepleted is TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Type of Rotor</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <param name="pExcludeDepleted">When True, all Rotor positions containing not depleted bottles/tubes of the specified Element are returned
        '''                                When False, all Rotor positions containing bottles/tubes of the specified Element are returned (depleted or not)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with the Rotor Position in which the tubes of the informed 
        '''          Element are positioned</returns>
        ''' <remarks>
        ''' Created by:  SA 10/02/2012
        ''' </remarks>
        Public Function VerifyTubesByElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             ByVal pRotorType As String, ByVal pElementID As Integer, ByVal pExcludeDepleted As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RotorType = '" & pRotorType.Trim & "' " & vbCrLf & _
                                                " AND    ElementID  = " & pElementID.ToString & vbCrLf
                        If (pExcludeDepleted) Then cmdText &= " AND  Status <> 'DEPLETED' "

                        Dim myRotorPositionsDS As New WSRotorContentByPositionDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myRotorPositionsDS.twksWSRotorContentByPosition)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myRotorPositionsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.VerifyTubesByElement", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO REVIEW - DELETE"
        'AG 07/12/2011 - comment this method and move into ReactionsRotorDelegate
        '''' <summary>
        '''' Get the content of an specific cell in an Analyzer Rotor
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analizer Identifier</param>
        '''' <param name="pRotorType">Rotor Type</param>
        '''' <param name="pRingNumber">Ring Number</param>
        '''' <param name="pCellNumber">Cell Number</param>
        '''' <param name="pWorkSessionID ">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSRotorContentByPositionDS with all information of the 
        ''''          Rotor Position
        '''' </returns>
        '''' <remarks>
        '''' Created by:  DL 02/06/2011
        '''' </remarks>
        'Public Function ReadReactions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                     ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText &= "SELECT TS.SampleType, WSE.SampleClass, WSE.RerunNumber, T.TestName, T.ReplicatesNumber, O.SampleID, WSE.ExecutionID, WSE.MultiItemNumber, OT.OrderTestID  " & vbCrLf
        '                cmdText &= "  FROM twksWSReactionsRotor WSRR INNER JOIN twksWSExecutions WSE ON (WSRR.ExecutionID = WSE.ExecutionID)" & vbCrLf
        '                cmdText &= "                                 INNER JOIN twksOrderTests OT ON (OT.OrderTestID = WSE.OrderTestID)" & vbCrLf
        '                cmdText &= "                                 INNER JOIN tparTests T ON (OT.TestID = T.TestID)" & vbCrLf
        '                cmdText &= "                                 LEFT  JOIN twksOrders O ON (OT.OrderID = O.OrderID)" & vbCrLf
        '                cmdText &= "                                 LEFT  JOIN tparTestSamples TS ON (T.TestID = TS.TestID)" & vbCrLf
        '                cmdText &= " WHERE WSRR.AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
        '                cmdText &= "   AND WSRR.RotorTurn = " & pRingNumber.ToString
        '                cmdText &= "   AND WSRR.WellNumber = " & pCellNumber.ToString

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim resultData As New ReactionRotorDetailsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(resultData.ReactionsRotorDetails)

        '                myGlobalDataTO.SetDatos = resultData
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSRotorContentByPositionDAO.ReadReactions", EventLogEntryType.Error, False)
        '    Finally
        '        'When Database Connection was opened locally, it has to be closed 
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
        'AG 07/12/2011 - comment this method and move into ReactionsRotorDelegate
#End Region
    End Class

End Namespace

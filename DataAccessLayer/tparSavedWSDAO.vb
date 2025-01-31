﻿Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparSavedWSDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Create a new Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSDS">Typed DataSet SavedWSDS containing the data of the WS to save</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet SavedWSDS after informing the ID of the 
        '''          Saved Work Session (automatically generated by the DB)</returns>
        ''' <remarks>
        ''' Created by:  GDS 30/03/2010
        ''' Modified by: SA  13/09/2010 - Added new field FromLIMS to the Insert 
        '''              SA  27/10/2010 - Added N preffix for multilanguage of field TS_User
        '''              SA  18/04/2012 - Changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSDS As SavedWSDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pSavedWSDS Is Nothing) Then
                    Dim cmdText As String = " INSERT INTO tparSavedWS (SavedWSName, FromLIMS, TS_User, TS_DateTime) " & vbCrLf & _
                                            " VALUES (N'" & pSavedWSDS.tparSavedWS(0).SavedWSName.Trim.Replace("'", "''") & "', " & vbCrLf & _
                                                      Convert.ToInt32(IIf(pSavedWSDS.tparSavedWS(0).FromLIMS, 1, 0)) & ", " & vbCrLf

                    'Audit fields are always informed 
                    If (pSavedWSDS.tparSavedWS(0).IsTS_UserNull) Then
                        'Dim objGlobal As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " N'" & pSavedWSDS.tparSavedWS(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pSavedWSDS.tparSavedWS(0).IsTS_DateTimeNull) Then
                        cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                    Else
                        cmdText &= " '" & pSavedWSDS.tparSavedWS(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                    End If
                    cmdText &= " SELECT SCOPE_IDENTITY() " & vbCrLf

                    'Execute the SQL sentence 
                    Dim nextSavedWSID As Integer = 0
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        nextSavedWSID = CType(dbCmd.ExecuteScalar(), Integer)
                    End Using

                    If (nextSavedWSID > 0) Then
                        'Get the generated WS ID and update the correspondent field in the DataSet
                        pSavedWSDS.tparSavedWS(0).BeginEdit()
                        pSavedWSDS.tparSavedWS(0).SavedWSID = nextSavedWSID
                        pSavedWSDS.tparSavedWS(0).EndEdit()

                        dataToReturn.SetDatos = pSavedWSDS
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete the specified WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Work Session ID</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 26/05/2010
        ''' Modified by: SA 18/04/2012 - Changed the function template
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparSavedWS WHERE SavedWSID = " & pSavedWSID

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
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete the specified Saved WS if it is empty (it does not have any Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Saved Work Session ID</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 23/04/2013 
        ''' </remarks>
        Public Function DeleteEmptySavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'AJG
                    'Dim cmdText As String = " DELETE FROM tparSavedWS " & vbCrLf & _
                    '                        " WHERE SavedWSID = " & pSavedWSID & vbCrLf & _
                    '                        " AND   SavedWSID NOT IN (SELECT DISTINCT SavedWSID FROM tparSavedWSOrderTests " & vbCrLf & _
                    '                                                 " WHERE SavedWSID = " & pSavedWSID & ") " & vbCrLf

                    Dim cmdText As String = " DELETE FROM tparSavedWS " & vbCrLf & _
                                            " WHERE SavedWSID = " & pSavedWSID & vbCrLf & _
                                            " AND   NOT EXISTS (SELECT DISTINCT SavedWSID FROM tparSavedWSOrderTests " & vbCrLf & _
                                                               " WHERE SavedWSID = " & pSavedWSID & " AND tparSavedWS.SavedWSID = SavedWSID) " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSDAO.DeleteEmptySavedWS", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the list of Saved Work Sessions
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFromLIS">Optional parameter to filter the returned Saved WorkSession to avoid shown the
        '''                        ones created for the Import from LIMS process (Files) or the LIS Orders Download process (ES)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSDS with the list of all Saved WorkSessions</returns>
        ''' <remarks>
        ''' Created by:  GDS 30/03/2010
        ''' Modified by: SA  13/09/2010 - Added new optional parameter pFromLIMS; changed the SQL to filter the SavedWS 
        '''                               according the value of field FromLIMS
        '''              SA  18/04/2012 - Changed the function template
        '''              DL  08/05/2013 - Added new optional parameter pAll
        '''              SA  14/05/2013 - Removed optional parameter pAll. Besides, when pFromLIS = TRUE, sort data by SavedWSID instead
        '''                               of by SavedWSName
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pFromLIS As Boolean = False) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparSavedWS " & vbCrLf & _
                                                " WHERE FromLIMS = " & Convert.ToInt32(IIf(pFromLIS, 1, 0)) & vbCrLf

                        If (pFromLIS) Then
                            cmdText &= "ORDER BY SavedWSID "
                        Else
                            cmdText &= "ORDER BY SavedWSName "
                        End If

                        Dim mySavedWSDS As New SavedWSDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSDS.tparSavedWS)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is already a Saved Work Session with the specified name 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSName">Name to be verified</param>
        ''' <param name="pFromLIMS">Optional parameter to filter the returned Saved WorkSession to avoid include in 
        '''                         the verification the ones created for the Import from LIMS process</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSDS with information of a WS for the specified Name</returns>
        ''' <remarks>
        ''' Created by:  GDS 30/03/2010
        ''' Modified by: SA  27/05/2010 - Changed the query to compare the Names using the same case 
        '''              SA  13/09/2010 - Added new optional parameter pFromLIMS; changed the SQL to filter the SavedWS 
        '''                               according the value of field FromLIMS
        '''              SA  19/10/2010 - Fixed error in SQL query: value of pFromLIMS has to be convert to Int32, not to Boolean
        '''              SA  27/10/2010 - Added N preffix for multilanguage of field SavedWSName when it is used in the WHERE clause
        '''              SA  18/04/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadBySavedWSName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSName As String, _
                                          Optional ByVal pFromLIMS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SavedWSID FROM tparSavedWS " & vbCrLf & _
                                                " WHERE  UPPER(SavedWSName) = UPPER(N'" & pSavedWSName.Trim.Replace("'", "''") & "') " & vbCrLf & _
                                                " AND    FromLIMS = " & Convert.ToInt32(IIf(pFromLIMS, 1, 0)) & vbCrLf

                        Dim mySavedWSDS As New SavedWSDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSDS.tparSavedWS)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSDAO.ReadBySavedWSName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all join with tparSavedWSOrderTests by SavedWSID
        ''' (return all records with mark fromLIS = 1)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSDS with information of a WS for the specified Name</returns>
        ''' <remarks>
        ''' Created by:  DL 26/04/2013
        ''' Modified by: AG 13/05/201 - Renamed method from "ReadBySavedWSID" to "ReadLISSavedWS", also change DAO query
        ''' </remarks>
        Public Function ReadLISSavedWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT SWS.SavedWSID, SWS.SavedWSName, SWS.FromLIMS, SWS.TS_User, SWS.TS_DateTime " & vbCrLf
                        cmdText &= "  FROM tparSavedWS SWS inner join tparSavedWSOrderTests SWSOT on SWS.SavedWSID = SWSOT.SavedWSID " & vbCrLf
                        cmdText &= "  WHERE SWS.FromLIMS = 1 "

                        Dim mySavedWSDS As New SavedWSDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySavedWSDS.tparSavedWS)
                            End Using
                        End Using

                        resultData.SetDatos = mySavedWSDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparSavedWSDAO.ReadLISSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class

End Namespace
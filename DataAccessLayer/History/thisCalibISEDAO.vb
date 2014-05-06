Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class thisCalibISEDAO

        Inherits DAOBase


#Region "CRUD Methods"

       

        ''' <summary>
        ''' Get ISE Calibrations History related to the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  SGM 24/01/2012
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   thisCalibISE " & _
                                  " WHERE  AnalyzerID = '" & pAnalyzerID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim HistoryISE As New HistoryISECalibrationsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(HistoryISE.thisCalibISE)

                        resultData.SetDatos = HistoryISE
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalibISEDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        'JB 25/07/2012 - Changed function name and column name
        '              - Added filter parameters
        Public Function ReadByConditioningTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pTypes As GlobalEnumerates.ISEConditioningTypes(), _
                                                Optional ByVal pDateFrom As DateTime = Nothing, Optional ByVal pDateTo As DateTime = Nothing) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim strTypes As String = ""
                        For Each elem As GlobalEnumerates.ISEConditioningTypes In pTypes
                            If Not String.IsNullOrEmpty(strTypes) Then strTypes &= ","
                            strTypes &= "'" & elem.ToString & "'"
                        Next
                        Dim cmdText As String
                        cmdText = " SELECT * FROM thisCalibISE " & _
                                  " WHERE AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' "

                        If Not String.IsNullOrEmpty(strTypes) Then cmdText &= " AND ConditioningType IN (" & strTypes & ")"
                        If (pDateFrom <> Nothing) Then cmdText &= " AND DATEDIFF(DAY, '" & Format(pDateFrom, "yyyyMMdd") & "', CalibrationDate) >= 0"
                        If (pDateTo <> Nothing) Then cmdText &= " AND DATEDIFF(DAY, CalibrationDate, '" & Format(pDateTo, "yyyyMMdd") & "')   >= 0"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim HistoryISE As New HistoryISECalibrationsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(HistoryISE.thisCalibISE)

                        resultData.SetDatos = HistoryISE
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalibISEDAO.ReadByConditioningTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        Public Function ReadByCalibrationID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pISECalibrationID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   thisCalibISE " & _
                                  " WHERE  AnalyzerID = '" & pAnalyzerID & "'" & _
                                  " AND  CalibrationID = '" & pISECalibrationID.ToString & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim HistoryISE As New HistoryISECalibrationsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(HistoryISE.thisCalibISE)

                        resultData.SetDatos = HistoryISE
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalibISEDAO.ReadByCalibrationID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        'JB: 25/07/2012 - Changed column CalibrationType to ConditioningType
        '               - Deleted column ActionType
        'JB: 20/09/2012 - Add LiEnabled column
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibrationDS As HistoryISECalibrationsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim cmdText As String = ""
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    Dim i As Integer = 0
                    Dim recordOK As Boolean = True

                    For Each C As HistoryISECalibrationsDS.thisCalibISERow In pCalibrationDS.thisCalibISE.Rows

                        cmdText = ""
                        cmdText = " INSERT INTO thisCalibISE(AnalyzerID, ConditioningType, CalibrationDate, ResultsString, LiEnabled, ErrorsString) " & _
                                  " VALUES (N'" & C.AnalyzerID.Trim & "', " & _
                                          " N'" & C.ConditioningType.Trim & "', " & _
                                          " N'" & CDate(C.CalibrationDate).ToString("yyyyMMdd HH:mm:ss").Trim & "', " & _
                                          " N'" & C.ResultsString.Replace("'", "''").Trim & "', " & _
                                          " " & CStr(IIf(C.LiEnabled, 1, 0)) & ", " & _
                                          " N'" & C.ErrorsString.Replace("'", "''").Trim & "') "

                        'Finally, get the automatically generated ID for the created Calculated Test
                        cmdText &= " SELECT SCOPE_IDENTITY() "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        Dim newCalibrationID As Integer
                        newCalibrationID = CType(dbCmd.ExecuteScalar(), Integer)
                        If (newCalibrationID > 0) Then
                            'C.BeginEdit()
                            'C.SetField("CalibrationID", newCalibrationID)
                            'C.EndEdit()

                            resultData.HasError = False
                            resultData.AffectedRecords += 1
                        Else
                            resultData.HasError = True
                            resultData.AffectedRecords = 0
                            Exit For
                        End If

                    Next C

                    If Not resultData.HasError Then
                        resultData.SetDatos = pCalibrationDS
                    Else
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        resultData.AffectedRecords = 0
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalibISEDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISECalibrationID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim cmd As New SqlCommand

                    cmdText &= " DELETE FROM  thisCalibISE "
                    cmdText &= " WHERE CalibrationID = " & pISECalibrationID.ToString

                    cmd.CommandText = cmdText
                    cmd.Connection = pDBConnection

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()

                    If myGlobalDataTO.AffectedRecords > 0 Then
                        myGlobalDataTO.HasError = False
                    Else
                        myGlobalDataTO.HasError = True
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalibISEDAO.Delete", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function
#End Region

    End Class

End Namespace

Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksCurveResultsDAO
        Inherits DAOBase

        Public Function FindLastCurveID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim dataReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                dataReturn = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not dataReturn.HasError) And (Not dataReturn.SetDatos Is Nothing) Then

                    dbConnection = CType(dataReturn.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim Cmd As New SqlClient.SqlCommand
                        Dim cmdText As String = ""

                        cmdText = "SELECT MAX(CurveResultsID) from twksCurveResults"

                        Cmd.CommandText = cmdText
                        Cmd.Connection = dbConnection
                        dataReturn.SetDatos = Cmd.ExecuteScalar()
                    End If
                End If

            Catch ex As Exception
                dataReturn.HasError = True
                dataReturn.ErrorCode = "SYSTEM_ERROR"
                dataReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.FindLastCurveID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return dataReturn

        End Function

        Public Function InsertCurve(ByVal pDBConnection As SqlClient.SqlConnection, _
                                    ByVal ptwksCurveResultsRow As CurveResultsDS.twksCurveResultsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"
                Else
                    Dim cmdText As String

                    cmdText = "INSERT INTO twksCurveResults " & vbCrLf & _
                              "( CurveResultsID" & vbCrLf & _
                              ", CurvePoint" & vbCrLf & _
                              ", ABSValue" & vbCrLf & _
                              ", CONCValue)" & vbCrLf & _
                              " VALUES " & vbCrLf & _
                              "( " & ptwksCurveResultsRow.CurveResultsID.ToString & vbCrLf

                    If ptwksCurveResultsRow.IsCurvePointNull Then
                        cmdText += ", NULL" & vbCrLf
                    Else
                        cmdText += ", " & ptwksCurveResultsRow.CurvePoint.ToString & vbCrLf
                    End If

                    If ptwksCurveResultsRow.IsABSValueNull Then
                        cmdText += ", NULL" & vbCrLf
                    Else
                        'cmdText += ", " & ptwksCurveResultsRow.ABSValue.ToString.Replace(",", ".") & vbCrLf
                        ' modified by dl 12/03/2010
                        cmdText += ", " & ReplaceNumericString(ptwksCurveResultsRow.ABSValue) & vbCrLf
                    End If

                    If ptwksCurveResultsRow.IsCONCValueNull Then
                        cmdText += ", NULL" & vbCrLf
                    Else
                        'cmdText += ", " & ptwksCurveResultsRow.CONCValue.ToString.Replace(",", ".") & vbCrLf
                        ' modified by dl 12/03/2010
                        cmdText += ", " & ReplaceNumericString(ptwksCurveResultsRow.CONCValue) & vbCrLf
                    End If

                    cmdText += ")"

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False

                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = "SYSTEM_ERROR"
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.InsertCurve", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        Public Function UpdateCurve(ByVal pDBConnection As SqlClient.SqlConnection, _
                                    ByVal ptwksCurveResultsRow As CurveResultsDS.twksCurveResultsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"

                Else
                    Dim cmdText As String

                    cmdText = "UPDATE twksCurveResults SET" & vbCrLf

                    If ptwksCurveResultsRow.IsABSValueNull Then
                        cmdText += "ABSValue = NULL" & vbCrLf
                    Else
                        'cmdText += "ABSValue = " & ptwksCurveResultsRow.ABSValue.ToString.Replace(",", ".") & vbCrLf
                        ' modified by dl 12/03/2010
                        cmdText += "ABSValue = " & ReplaceNumericString(ptwksCurveResultsRow.ABSValue) & vbCrLf

                    End If

                    If ptwksCurveResultsRow.IsCONCValueNull Then
                        cmdText += ", CONCValue = NULL" & vbCrLf
                    Else
                        'cmdText += ", CONCValue = " & ptwksCurveResultsRow.CONCValue.ToString.Replace(",", ".") & vbCrLf
                        ' modified by dl 12/03/2010
                        cmdText += ", CONCValue = " & ReplaceNumericString(ptwksCurveResultsRow.CONCValue) & vbCrLf
                    End If

                    cmdText += "WHERE CurveResultsID = " & ptwksCurveResultsRow.CurveResultsID.ToString & vbCrLf & _
                               "  AND CurvePoint = " & ptwksCurveResultsRow.CurvePoint.ToString

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False

                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = "SYSTEM_ERROR"
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.UpdateCurve", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        Public Function ExistsCurveResult(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal ptwksCurveResultsRow As CurveResultsDS.twksCurveResultsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String

                        cmdText = "SELECT 1" & vbCrLf & _
                                  "  FROM twksCurveResults" & vbCrLf & _
                                  "  WHERE CurveResultsID = " & ptwksCurveResultsRow.CurveResultsID & vbCrLf & _
                                  "    AND CurvePoint     = " & ptwksCurveResultsRow.CurvePoint

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet
                        Dim existsCurveResultsDS As New CurveResultsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(existsCurveResultsDS.twksCurveResults)

                        resultData.HasError = False
                        resultData.SetDatos = existsCurveResultsDS
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.ExistsCurveResult", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        Public Function ReadCurve(ByVal pDBConnection As SqlClient.SqlConnection, _
                                  ByVal pCurveResultsID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String

                        cmdText = "SELECT CurveResultsID" & _
                                       ", CurvePoint" & _
                                       ", ABSValue" & _
                                       ", CONCValue" & _
                                  " FROM twksCurveResults" & _
                                  " WHERE CurveResultsID = " & pCurveResultsID & _
                                  " ORDER BY CurvePoint ASC"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet
                        Dim curveResultsDS As New CurveResultsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(curveResultsDS.twksCurveResults)

                        resultData.HasError = False
                        resultData.SetDatos = curveResultsDS
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.ReadCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Delete curve results by OrderTestId
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>Globaldatato indicating if operation succeed or fails</returns>
        ''' <remarks>
        ''' Created by:  AG 09/03/2010 (Tested pending)
        ''' Modified by: SA 16/04/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteResultsByOrderTestId(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = " DELETE from twksCurveResults " & vbCrLf & _
                                            " WHERE CurveResultsID IN (SELECT DISTINCT CurveResultsID " & vbCrLf & _
                                                                     " FROM   twksResults " & vbCrLf & _
                                                                     " WHERE  OrderTestID = " & pOrderTestID.ToString & ") " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.DeleteResultsByOrderTestId", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete curve results 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCurveResultsID"></param>        
        ''' <returns>GlobalDataTo informing if execution succeed or fails</returns>
        ''' <remarks>Created by AG 22/03/2010 (Tested pending)</remarks>
        Public Function DeleteCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCurveResultsID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"

                Else
                    Dim cmdText As String

                    cmdText = "DELETE from twksCurveResults where CurveResultsID = " & pCurveResultsID

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.DeleteCurve", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete curve results for all NOTCALC Calibrators
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo informing if execution succeed or fails</returns>
        ''' <remarks>
        ''' Created by:  JB 10/10/2012
        ''' </remarks>
        Public Function DeleteForNOTCALCCalibrators(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"
                Else
                    Dim cmdText As String
                    cmdText = "DELETE twksCurveResults " & _
                              "  WHERE CurveResultsID NOT IN (" & _
                              "       SELECT DISTINCT CurveResultsID " & _
                              "         FROM twksResults " & _
                              "         WHERE ValidationStatus = 'OK' " & _
                              "           AND AcceptedResultFlag = 1 " & _
                              "           AND CurveResultsID is not null" & _
                              "  )"

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksCurveResultsDAO.DeleteForNOTCALCCalibrators", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

    End Class

End Namespace

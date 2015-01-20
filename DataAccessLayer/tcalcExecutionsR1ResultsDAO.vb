Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcalcExecutionsR1ResultsDAO
        Inherits DAOBase

        Public Function InsertResult(ByVal pDBConnection As SqlClient.SqlConnection, _
                                     ByVal ptcalcExecutionsR1Results As ExecutionsR1ResultsDS.tcalcExecutionsR1ResultsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"
                Else
                    Dim cmdText As String

                    cmdText = "INSERT INTO tcalcExecutionsR1Results " & vbCrLf & _
                              "( AnalyzerID" & vbCrLf & _
                              ", WorkSessionID" & vbCrLf & _
                              ", ExecutionID" & vbCrLf & _
                              ", ABS_Value" & vbCrLf & _
                              ", ReadingNumber" & vbCrLf & _
                              ", DateTime)" & vbCrLf & _
                              " VALUES " & vbCrLf & _
                              "( '" & ptcalcExecutionsR1Results.AnalyzerID.ToString & "'" & vbCrLf & _
                              ", '" & ptcalcExecutionsR1Results.WorkSessionID.ToString & "'" & vbCrLf & _
                              ", " & ptcalcExecutionsR1Results.ExecutionID.ToString & vbCrLf

                    If ptcalcExecutionsR1Results.IsABS_ValueNull Then
                        cmdText += ", 0" & vbCrLf
                    Else
                        'cmdText += ", " & ptcalcExecutionsR1Results.ABS_Value.ToString.Replace(",", ".") & vbCrLf
                        ' Modified by : DL 12/03/2010
                        cmdText += ", " & ReplaceNumericString(ptcalcExecutionsR1Results.ABS_Value) & vbCrLf
                    End If

                    If ptcalcExecutionsR1Results.IsReadingNumberNull Then
                        cmdText += ", 0" & vbCrLf
                    Else
                        cmdText += ", " & ptcalcExecutionsR1Results.ReadingNumber.ToString & vbCrLf
                    End If

                    If ptcalcExecutionsR1Results.IsDateTimeNull Then
                        cmdText += ", '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    Else
                        cmdText += ", '" & Convert.ToDateTime(ptcalcExecutionsR1Results.DateTime.ToString).ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
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
                GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsR1ResultsDAO.InsertResult", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        Public Function UpdateResults(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal ptcalcExecutionsR1Results As ExecutionsR1ResultsDS.tcalcExecutionsR1ResultsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"

                Else
                    Dim cmdText As String

                    cmdText = "UPDATE tcalcExecutionsR1Results SET" & vbCrLf

                    If ptcalcExecutionsR1Results.IsABS_ValueNull Then
                        cmdText += "  ABS_Value = 0" & vbCrLf
                    Else
                        'cmdText += "  ABS_Value = " & ptcalcExecutionsR1Results.ABS_Value.ToString.Replace(",", ".") & vbCrLf
                        ' Modified by : DL 12/03/2010
                        cmdText += "  ABS_Value = " & ReplaceNumericString(ptcalcExecutionsR1Results.ABS_Value) & vbCrLf
                    End If

                    If ptcalcExecutionsR1Results.IsReadingNumberNull Then
                        cmdText += ", ReadingNumber = 0" & vbCrLf
                    Else
                        cmdText += ", ReadingNumber = " & ptcalcExecutionsR1Results.ReadingNumber.ToString & vbCrLf
                    End If

                    If ptcalcExecutionsR1Results.IsDateTimeNull Then
                        cmdText += ", DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    Else
                        cmdText += ", DateTime = '" & Convert.ToDateTime(ptcalcExecutionsR1Results.DateTime.ToString).ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    End If

                    cmdText += "WHERE AnalyzerID = '" & ptcalcExecutionsR1Results.AnalyzerID.ToString & "'" & vbCrLf & _
                               "  AND WorkSessionID = '" & ptcalcExecutionsR1Results.WorkSessionID.ToString & "'" & vbCrLf & _
                               "  AND ExecutionID = " & ptcalcExecutionsR1Results.ExecutionID.ToString

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
                GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsR1ResultsDAO.UpdateResults", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        Public Function ExistsResult(ByVal pDBConnection As SqlClient.SqlConnection, _
                                     ByVal ptcalcExecutionsR1Results As ExecutionsR1ResultsDS.tcalcExecutionsR1ResultsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String

                        cmdText = "SELECT 1" & vbCrLf & _
                                  "  FROM tcalcExecutionsR1Results" & vbCrLf & _
                                  "  WHERE AnalyzerID = '" & ptcalcExecutionsR1Results.AnalyzerID.ToString & "'" & vbCrLf & _
                                  "    AND WorkSessionID = '" & ptcalcExecutionsR1Results.WorkSessionID.ToString & "'" & vbCrLf & _
                                  "    AND ExecutionID = " & ptcalcExecutionsR1Results.ExecutionID.ToString

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet
                        Dim existsExecutionsR1Results As New ExecutionsR1ResultsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(existsExecutionsR1Results.tcalcExecutionsR1Results)

                        resultData.HasError = False
                        resultData.SetDatos = existsExecutionsR1Results
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsR1ResultsDAO.ExistsResult", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, _
                                ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"

                Else
                    Dim cmdText As String
                    cmdText = "DELETE tcalcExecutionsR1Results" & vbCrLf & _
                              "  WHERE AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                              "  AND   WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"

                    Dim cmd As SqlCommand

                    cmd = pDBConnection.CreateCommand
                    cmd.CommandText = cmdText

                    cmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsR1ResultsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

    End Class

End Namespace

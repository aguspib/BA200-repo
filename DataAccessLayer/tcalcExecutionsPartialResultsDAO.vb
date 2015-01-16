Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcalcExecutionsPartialResultsDAO
        Inherits DAOBase

        'Public Function InsertResult(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                             ByVal ptcalcExecutionsPartialResults As ExecutionsPartialResultsDS.tcalcExecutionsPartialResultsRow) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = "DB_CONNECTION_ERROR"
        '        Else
        '            Dim cmdText As String

        '            cmdText = "INSERT INTO tcalcExecutionsPartialResults " & vbCrLf & _
        '                      "( AnaylzerID" & vbCrLf & _
        '                      ", WorkSessionID" & vbCrLf & _
        '                      ", ExecutionID" & vbCrLf & _
        '                      ", PartialABS_Value" & vbCrLf & _
        '                      ", PartialNumber)" & vbCrLf & _
        '                      " VALUES " & vbCrLf & _
        '                      "( '" & ptcalcExecutionsPartialResults.AnaylzerID.ToString & "'" & vbCrLf & _
        '                      ", '" & ptcalcExecutionsPartialResults.WorkSessionID.ToString & "'" & vbCrLf & _
        '                      ", " & ptcalcExecutionsPartialResults.ExecutionID.ToString & vbCrLf

        '            If ptcalcExecutionsPartialResults.IsPartialABS_ValueNull Then
        '                cmdText += ", 0" & vbCrLf
        '            Else
        '                'cmdText += ", " & ptcalcExecutionsPartialResults.PartialABS_Value.ToString.Replace(",", ".") & vbCrLf
        '                ' Modified by : DL 12/03/2010
        '                cmdText += ", " & ReplaceNumericString(ptcalcExecutionsPartialResults.PartialABS_Value) & vbCrLf
        '            End If

        '            If ptcalcExecutionsPartialResults.IsPartialNumberNull Then
        '                cmdText += ", 0" & vbCrLf
        '            Else
        '                cmdText += ", " & ptcalcExecutionsPartialResults.PartialNumber.ToString & vbCrLf
        '            End If

        '            cmdText += ")"

        '            Dim dbCmd As New SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

        '            If (resultData.AffectedRecords = 1) Then
        '                resultData.HasError = False

        '            Else
        '                resultData.HasError = True
        '                resultData.ErrorCode = "SYSTEM_ERROR"
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsPartialResultsDAO.InsertResult", EventLogEntryType.Error, False)
        '    End Try

        '    Return resultData

        'End Function

        'Public Function UpdateResults(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                              ByVal ptcalcExecutionsPartialResults As ExecutionsPartialResultsDS.tcalcExecutionsPartialResultsRow) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = "DB_CONNECTION_ERROR"

        '        Else
        '            Dim cmdText As String

        '            cmdText = "UPDATE tcalcExecutionsPartialResults SET" & vbCrLf

        '            If ptcalcExecutionsPartialResults.IsPartialABS_ValueNull Then
        '                cmdText += "PartialABS_Value = 0" & vbCrLf
        '            Else
        '                'cmdText += "PartialABS_Value = " & ptcalcExecutionsPartialResults.PartialABS_Value.ToString.Replace(",", ".") & vbCrLf
        '                ' Modified by : DL 12/03/2010
        '                cmdText += "PartialABS_Value = " & ReplaceNumericString(ptcalcExecutionsPartialResults.PartialABS_Value) & vbCrLf
        '            End If

        '            If ptcalcExecutionsPartialResults.IsPartialNumberNull Then
        '                cmdText += ", PartialNumber = 0" & vbCrLf
        '            Else
        '                cmdText += ", PartialNumber = " & ptcalcExecutionsPartialResults.PartialNumber.ToString & vbCrLf
        '            End If

        '            cmdText += "WHERE AnaylzerID = '" & ptcalcExecutionsPartialResults.AnaylzerID.ToString & "'" & vbCrLf & _
        '                       "  AND WorkSessionID = '" & ptcalcExecutionsPartialResults.WorkSessionID.ToString & "'" & vbCrLf & _
        '                       "  AND ExecutionID = " & ptcalcExecutionsPartialResults.ExecutionID.ToString

        '            Dim dbCmd As New SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

        '            If (resultData.AffectedRecords = 1) Then
        '                resultData.HasError = False

        '            Else
        '                resultData.HasError = True
        '                resultData.ErrorCode = "SYSTEM_ERROR"
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsPartialResultsDAO.UpdateResults", EventLogEntryType.Error, False)
        '    End Try

        '    Return resultData

        'End Function

        'Public Function ExistsResult(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                             ByVal ptcalcExecutionsPartialResults As ExecutionsPartialResultsDS.tcalcExecutionsPartialResultsRow) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String

        '                cmdText = "SELECT 1" & vbCrLf & _
        '                          "  FROM tcalcExecutionsPartialResults" & vbCrLf & _
        '                          "  WHERE AnaylzerID = '" & ptcalcExecutionsPartialResults.AnaylzerID.ToString & "'" & vbCrLf & _
        '                          "    AND WorkSessionID = '" & ptcalcExecutionsPartialResults.WorkSessionID.ToString & "'" & vbCrLf & _
        '                          "    AND ExecutionID = " & ptcalcExecutionsPartialResults.ExecutionID.ToString

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet
        '                Dim existsExecutionsPartialResults As New ExecutionsPartialResultsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(existsExecutionsPartialResults.tcalcExecutionsPartialResults)

        '                resultData.HasError = False
        '                resultData.SetDatos = existsExecutionsPartialResults
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsPartialResultsDAO.ExistsResult", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData

        'End Function

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
                    cmdText = "DELETE tcalcExecutionsPartialResults" & vbCrLf & _
                              "  WHERE AnaylzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
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
                GlobalBase.CreateLogActivity(ex.Message, "tcalcExecutionsPartialResultsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

    End Class

End Namespace



Option Explicit On
Option Strict On


Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class tcfgAnalyzerLedPositionsDAO
        Inherits DAOBase



#Region "CRUD"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerLedPositionsDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 16/12/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerLedPositionsDS As AnalyzerLedPositionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                ElseIf (Not pAnalyzerLedPositionsDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    Dim STR_NULL As String = " NULL, "

                    For Each localRow As AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow In pAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions
                        cmdText &= " INSERT INTO tcfgAnalyzerLedPositions "
                        cmdText &= " (AnalyzerID, LedPosition, WaveLength, Status) VALUES ("

                        cmdText &= String.Format("'{0}', ", localRow.AnalyzerID) 'Required
                        cmdText &= String.Format("{0}, ", localRow.LedPosition) 'Required
                        cmdText &= String.Format("'{0}', ", localRow.WaveLength) 'Required
                        cmdText &= String.Format("{0} ", IIf(localRow.Status, 1, 0)) 'Required but not comma is added after CurrentValue

                        cmdText &= String.Format("){0}", vbNewLine) 'insert line break

                    Next

                    dbCmd.CommandText = cmdText
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerLedPositionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData

        End Function

#End Region


#Region "READ BY"

        ''' <summary>
        ''' Get all WaveLength Items
        ''' </summary>
        ''' <param name="pAnalyzerID">Identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks>
        ''' Created by XBC 24/02/2011
        ''' Modified by RH 29/02/2012 Code optimization. Using statement. Short circuit evaluation.
        ''' </remarks>
        Public Function GetAllWaveLengths(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal pAnalyzerID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDS
                        Dim cmdText As New Text.StringBuilder()

                        cmdText.AppendLine("  SELECT AnalyzerID, LedPosition, WaveLength, Status")
                        cmdText.AppendLine("  FROM  tcfgAnalyzerLedPositions")
                        cmdText.AppendFormat("WHERE AnalyzerID = '{0}'", pAnalyzerID)
                        cmdText.AppendFormat("{0}ORDER BY LedPosition", vbCrLf)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAnalyzerLedPositions.tcfgAnalyzerLedPositions)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myAnalyzerLedPositions
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = False
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerLedPositionsDAO.GetAllWaveLengths", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Get value of a specific Item in the indicated Preloaded Master Data Sub Table
        ''' </summary>
        ''' <param name="pAnalyzerID">Identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <param name="pWaveLength">Identifier of the specific Item which value will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks></remarks>
        Public Function GetByWaveLength(ByVal pDBConnection As SqlClient.SqlConnection, _
                                        ByVal pAnalyzerID As String, _
                                        ByVal pWaveLength As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim dbCmd As New SqlClient.SqlCommand
                        Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDS
                        Dim cmdText As String = ""

                        cmdText += "SELECT AnalyzerID, LedPosition, WaveLength, Status "
                        cmdText += "FROM  tcfgAnalyzerLedPositions "
                        cmdText += "WHERE AnalyzerID = '" & pAnalyzerID & "'"
                        cmdText += "  AND WaveLength = '" & pWaveLength & "'"

                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(myAnalyzerLedPositions.tcfgAnalyzerLedPositions)
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAnalyzerLedPositions.tcfgAnalyzerLedPositions)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myAnalyzerLedPositions
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = False
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerLedPositionsDAO.GetByWaveLength", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Get value of a specific Item in the indicated Preloaded Master Data Sub Table
        ''' </summary>
        ''' <param name="pAnalyzerID">Identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <param name="pLedPosition">Identifier of the specific Item which value will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks>
        ''' Modified by RH 27/02/2012 Code optimization. Using statement.
        ''' </remarks>
        Public Function GetByLedPosition(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pAnalyzerID As String, _
                                         ByVal pLedPosition As Integer) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDS
                        Dim cmdText As New System.Text.StringBuilder()

                        cmdText.AppendLine("SELECT AnalyzerID, LedPosition, WaveLength, Status")
                        cmdText.AppendLine("FROM  tcfgAnalyzerLedPositions")
                        cmdText.AppendFormat("WHERE AnalyzerID = '{0}' ", pAnalyzerID)
                        cmdText.AppendFormat("AND LedPosition = {0}", pLedPosition)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAnalyzerLedPositions.tcfgAnalyzerLedPositions)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myAnalyzerLedPositions
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = False
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerLedPositionsDAO.GetByLedPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function

#End Region 'READ BY


    End Class

End Namespace


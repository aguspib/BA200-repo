Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Class AnalyzerLedPositionsDelegate

#Region "CRUD"

        ''' <summary>
        ''' Create the analyzer LED POSITIONS for a new analyzerID (serial number)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerLedPositionsDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 16/12/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerLedPositionsDS As AnalyzerLedPositionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If Not pAnalyzerLedPositionsDS Is Nothing AndAlso pAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                            Dim myDAO As New tcfgAnalyzerLedPositionsDAO
                            resultData = myDAO.Create(dbConnection, pAnalyzerLedPositionsDS)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerLedPositionsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

#End Region

#Region "OTHER METHODS"

        Public Function GetAllWaveLengths(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByVal pAnalyzerID As String) As GlobalDataTO

            Dim returnedData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not returnedData.HasError) AndAlso (Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = CType(returnedData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzerLedPositions As New tcfgAnalyzerLedPositionsDAO
                        returnedData = mytcfgAnalyzerLedPositions.GetAllWaveLengths(dbConnection, pAnalyzerID)
                    End If
                End If

            Catch ex As Exception
                returnedData = New GlobalDataTO()
                returnedData.HasError = True
                returnedData.ErrorCode = "SYSTEM_ERROR"
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerLedPositionsDelegate.GetAllWaveLengths", EventLogEntryType.Error, False)
            Finally

                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return returnedData
        End Function

        Public Function GetByWaveLength(ByVal pDBConnection As SqlClient.SqlConnection, _
                                        ByVal pAnalyzerID As String, _
                                        ByVal pWavelength As String) As GlobalDataTO

            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError) And (Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = CType(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzerLedPositions As New tcfgAnalyzerLedPositionsDAO

                        returnedData = mytcfgAnalyzerLedPositions.GetByWaveLength(dbConnection, pAnalyzerID, pWavelength)
                    End If
                End If

            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = "SYSTEM_ERROR"
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerLedPositionsDelegate.GetByWaveLength", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return returnedData
        End Function

        ' Modified by RH 27/02/2012 Code optimization.
        Public Function GetByLedPosition(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pAnalyzerID As String, _
                                         ByVal pLedPosition As Integer) As GlobalDataTO

            Dim returnedData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not returnedData.HasError) AndAlso (Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = CType(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytcfgAnalyzerLedPositions As New tcfgAnalyzerLedPositionsDAO
                        returnedData = mytcfgAnalyzerLedPositions.GetByLedPosition(dbConnection, pAnalyzerID, pLedPosition)
                    End If
                End If

            Catch ex As Exception
                returnedData = New GlobalDataTO()
                returnedData.HasError = True
                returnedData.ErrorCode = "SYSTEM_ERROR"
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerLedPositionsDelegate.GetByLedPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return returnedData
        End Function

#End Region

    End Class

End Namespace
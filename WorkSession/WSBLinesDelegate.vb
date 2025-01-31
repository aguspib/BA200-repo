﻿Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    'AG 14/05/2010 - change name WSBaseLinesDelegate for WSBLinesDelegate

    Public Class WSBLinesDelegate
        Implements IWSBLinesDelegateCRUD(Of BaseLinesDS)
        Implements IWSBLinesDelegateValuesDeleter

#Region "CRUD"
        ''' <summary>
        ''' Create an adjustment BaseLine
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBaseLinesDS">Typed DataSet BaseLinesDS containing all data of the adjustment BaseLine to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/05/2010
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBaseLinesDS As BaseLinesDS) As GlobalDataTO Implements IWSBLinesDelegateCRUD(Of BaseLinesDS).Create
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        resultData = myDAO.Create(dbConnection, pBaseLinesDS)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (resultData)
        End Function


        ''' <summary>
        ''' Verify if exists the specified BaseLineID in table twksWSBLines
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBaseLineID">Identifier of an adjustment Base Line</param>
        ''' <param name="pLed"></param>
        ''' <param name="pType">STATIC or DYNAMIC</param>
        ''' <returns>GlobalDataTO containing a boolean value: TRUE if the adjustment BL exists; otherwise FALSE</returns>
        ''' <remarks>
        ''' Created by:  AG 20/05/2010 
        ''' AG 29/10/2014 BA-2057 parameter pLed, pType
        ''' </remarks>
        Public Function Exists(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                               ByVal pBaseLineID As Integer, ByVal pLed As Integer, ByVal pType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Dim existBaseline As Boolean = False

            Try
                resultData.SetDatos = existBaseline

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        resultData = myDAO.Read(dbConnection, pAnalyzerID, pWorkSessionID, pBaseLineID, pLed, pType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myBaseLineDS As BaseLinesDS = DirectCast(resultData.SetDatos, BaseLinesDS)
                            If (myBaseLineDS.twksWSBaseLines.Rows.Count > 0) Then
                                existBaseline = True
                            End If
                        End If
                    End If
                End If

                resultData.SetDatos = existBaseline
                resultData.HasError = False
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.Exists", EventLogEntryType.Error, False)
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
        ''' <param name="pWellUsed"></param>
        ''' <param name="pType">STATIC or DYNAMIC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with all data for the informed BaseLine</returns>
        ''' <remarks>
        ''' Created by:  DL 19/02/2010
        ''' Modified by: AG 13/05/2010 - Changed the method name to READ and some field names 
        '''              AG 20/05/2010 - Added parameters AnalyzerID and WorkSessionID
        ''' AG 29/10/2014 BA-2057 parameter pWellUsed, pType
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                             ByVal pBaseLineID As Integer, ByVal pWellUsed As Integer, ByVal pType As String) As TypedGlobalDataTo(Of BaseLinesDS) Implements IWSBLinesDelegateCRUD(Of BaseLinesDS).Read
            Dim resultData As TypedGlobalDataTo(Of BaseLinesDS) = Nothing
            Dim connection As TypedGlobalDataTo(Of SqlConnection) = Nothing
            Try
                connection = GetSafeOpenDBConnection(pDBConnection)
                If (connection IsNot Nothing AndAlso connection.SetDatos IsNot Nothing) Then
                    Dim mytwksWSBaseLines As New twksWSBLinesDAO
                    Dim result = mytwksWSBaseLines.Read(connection.SetDatos, pAnalyzerID, pWorkSessionID, pBaseLineID, pWellUsed, pType)
                    resultData = New TypedGlobalDataTo(Of BaseLinesDS)
                    resultData.SetDatos = TryCast(result.SetDatos, BaseLinesDS)
                    resultData.HasError = result.HasError
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message
                CreateLogActivity(ex)
            Finally
                If (pDBConnection Is Nothing) Then CloseConnection(connection)
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
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pBaseLinesDS As BaseLinesDS) As GlobalDataTO Implements IWSBLinesDelegateCRUD(Of BaseLinesDS).Update
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        resultData = myDAO.Update(dbConnection, pBaseLinesDS)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.GetByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        Public Function Delete(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                     ByVal pBaseLineID As Integer, ByVal pWellUsed As Integer, ByVal pType As String) As TypedGlobalDataTo(Of BaseLinesDS) Implements IWSBLinesDelegateCRUD(Of BaseLinesDS).Delete
            Return New TypedGlobalDataTo(Of BaseLinesDS)() With {.HasError = True, .ErrorMessage = "Not implemented"}
        End Function


#End Region

#Region "Read for calculations"
        ''' <summary>
        ''' Return all Base Lines data needed for calculations
        ''' STATIC base line: Read ligth values from twksWSBLinesByWell, read dark and adjust (TI, DAC) values from twksWSBLines
        ''' DYNAMIC base line: Read ligth values from twksWSBLines (DYNAMIC), read dark and adjust (TI, DAC) values from twksWSBLines (STATIC with adjust)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pBaseLineWellID">Identifier of a BaseLine by Well</param>
        ''' <param name="pWell">Rotor Well Number</param>
        ''' <param name="pBaseLineAdjustID">Identifier of an adjustment Base Line</param>
        ''' <param name="pType">STATIC or DYNAMIC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLineDS</returns>
        ''' <remarks>
        ''' Created by:  AG 04/01/2011
        ''' AG 29/10/2014 BA-2064 adapt for static or dynamic base lines (new parameter pType) (renamed, old name GetBaseLineValues)
        ''' AG 19/11/2014 BA-2064 reinterprete the formula and get the correct information for calculations using dynamic base line
        ''' </remarks>
        Public Function ReadValuesForCalculations(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                  ByVal pBaseLineWellID As Integer, ByVal pWell As Integer, ByVal pBaselineAdjustID As Integer, ByVal pType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        If pType = GlobalEnumerates.BaseLineType.STATIC.ToString Then
                            resultData = myDAO.ReadValuesForCalculations(dbConnection, pAnalyzerID, pWorkSessionID, pBaseLineWellID, pWell, pBaselineAdjustID, pType)
                        ElseIf pType = GlobalEnumerates.BaseLineType.DYNAMIC.ToString Then
                            resultData = myDAO.ReadValuesForCalculationsDYNAMIC(dbConnection, pAnalyzerID, pWell, pBaselineAdjustID)
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.GetCurrentBaseLineID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Several Get by ..."
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
        ''' <param name="pBLType">Base line type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with all data of the specified BaseLine/WaveLength</returns>
        ''' <remarks>
        ''' Created by:  DL 02/06/2010
        ''' AG 19/11/2014 BA-2067 add parameter for base line type
        ''' </remarks>
        Public Function GetByWaveLength(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                        ByVal pAdjustBaseLineID As Integer, ByVal pWaveLength As Integer, ByVal pWellBaseLineID As Integer, _
                                        ByVal pWellUsed As Integer, ByVal pBLType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSBLines As New twksWSBLinesDAO
                        If pBLType = GlobalEnumerates.BaseLineType.STATIC.ToString Then
                            resultData = myWSBLines.GetByWaveLength(dbConnection, pAnalyzerID, pWorkSessionID, pAdjustBaseLineID, pWaveLength, pWellBaseLineID, pWellUsed)
                        ElseIf pBLType = GlobalEnumerates.BaseLineType.DYNAMIC.ToString Then
                            resultData = myWSBLines.GetByWaveLengthDYNAMIC(dbConnection, pAnalyzerID, pAdjustBaseLineID, pWaveLength, pWellUsed, pBLType)
                        End If

                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.GetCurrentBaseLineValues", EventLogEntryType.Error, False)
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
        ''' Modified by: IT 03/11/2014 - BA-2067: Dynamic BaseLine
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pBaseLineType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSBLines As New twksWSBLinesDAO

                        If (pWorkSessionID.Trim <> "") Then
                            resultData = myWSBLines.GetByWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pBaseLineType) 'BA-2067
                        Else
                            resultData = myWSBLines.GetByAnalyzer(dbConnection, pAnalyzerID, pBaseLineType) 'BA-2067
                        End If

                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.IsComplete", EventLogEntryType.Error, False)
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
        ''' <param name="pWellUsed"></param>
        ''' <param name="pType">STATIC or DYNAMIC</param>
        ''' <returns>GlobalDataTO containing an integer value with the adjustment BaseLine Identifier</returns>
        ''' <remarks>
        ''' Created by:  AG 17/05/2010
        ''' AG 29/10/2014 BA-20562 adapt method to read the static or dynamic base line(parameter pWellUsed, pType)
        ''' </remarks>
        Public Function GetCurrentBaseLineID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             ByVal pWellUsed As Integer, ByVal pType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        resultData = myDAO.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, pWellUsed, pType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.Read", EventLogEntryType.Error, False)
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
        ''' <param name="pType">STATIC or DYNAMIC or ALL (if "")</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with all WaveLengths for the last executed adjustment BaseLine for 
        '''          the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by:  AG 04/05/2011
        '''              AG 04/11/2014 BA-2065 (adapt well rejection for STATIC or DYNAMIC base line)
        ''' </remarks>
        Public Function GetCurrentBaseLineValues(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        resultData = myDAO.GetCurrentBaseLineValues(dbConnection, pAnalyzerID, pType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.ReadValuesForCalculations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


#End Region

#Region "Reset (new results or reset WS)"
        ''' <summary>
        ''' Delete of adjustment BaseLines for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pType">IF ="" delete ALL, if different delete by all by Type</param>
        ''' <param name="pBaseLineTypeForCalculation"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' Modified by: AG 29/04/2011 - Due to field WorkSessionID was removed from table twksWSBLines, method is renamed from ResetWS to 
        '''                              ResetAdjustsBLines, parameter pWorkSessionID is also removed, and the query is changed to remove the filter
        ''' Modified by: AG 31/10/2014 BA-2057 new parameter pType
        ''' AG 16/11/2014 BA-2065 rename method from ResetAdjustsBLines to ResetBLinesValues
        ''' AG 21/11/2014 BA-2062 when Reset the ALIGHT results reset also the table twksWSBLinesByWell
        ''' AG 15/01/2015 BA-2212 skip the process for delete not used baseline records when pBaseLineTypeForCalculation = DYNAMIC and parameter RDI_CUMULATE_ALL_BASELINES = 1
        ''' </remarks>
        Public Function DeleteBLinesValues(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, ByVal pType As String, ByVal pBaseLineTypeForCalculation As String) As GlobalDataTO Implements IWSBLinesDelegateValuesDeleter.DeleteBLinesValues
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'AG 15/01/2015 BA-2212
                        Dim skipBaseLinesDeletionProcess As Integer = 0 'By default execute the process for delete the not used baseline records
                        If pBaseLineTypeForCalculation = GlobalEnumerates.BaseLineType.DYNAMIC.ToString Then
                            Dim myParams As New SwParametersDelegate
                            resultData = myParams.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.RDI_CUMULATE_ALL_BASELINES.ToString, Nothing)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim auxDataSet As ParametersDS
                                auxDataSet = DirectCast(resultData.SetDatos, ParametersDS)
                                If auxDataSet.tfmwSwParameters.Rows.Count > 0 AndAlso Not auxDataSet.tfmwSwParameters.First.IsValueNumericNull Then
                                    skipBaseLinesDeletionProcess = CInt(auxDataSet.tfmwSwParameters.First.ValueNumeric)
                                End If
                            End If
                        End If

                        'When value = 1 skip the process that evaluates if previous baselines can be deleted or not
                        If skipBaseLinesDeletionProcess <> 1 Then
                            'AG 15/01/2015 BA-2212

                            Dim myExecDel As New ExecutionsDelegate
                            resultData = myExecDel.CountExecutionsUsingAdjustBaseLine(dbConnection, pAnalyzerID, pWorkSessionID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim counter As Integer = CType(resultData.SetDatos, Integer)
                                If (counter = 0) Then
                                    'Remove ALIGHT/FLIGHT results (or both) only when no executions are using these adjust base line identifiers
                                    Dim myDAO As New twksWSBLinesDAO
                                    resultData = myDAO.DeleteBLinesValues(dbConnection, pAnalyzerID, pType)

                                    'AG 21/11/2014 BA-2062 if pType = "" (ALL) also delete the twksWSBLinesByWell table (inform the model is not required then)
                                    If Not resultData.HasError AndAlso pType = "" Then
                                        Dim myBLByWellDlgte As New WSBLinesByWellDelegate
                                        resultData = myBLByWellDlgte.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID, "", True)
                                    End If

                                End If
                            End If
                        End If 'AG 15/01/2015 BA-2212

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.ResetAdjustsBLines", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        Public Function VerifyBLMovedToHistoric(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pAdjustBaseLineID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        resultData = myDAO.VerifyBLMovedToHistoric(dbConnection, pAnalyzerID, pAdjustBaseLineID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesDAO
                        resultData = myDAO.UpdateMovedToHistoric(dbConnection, pAnalyzerID, pAdjustBaseLineID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.UpdateMovedToHistoric", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete all records in table except the last baseLineID for both types: STATIC and DYNAMIC
        ''' (process for delete baseline records MUST BE EXECUTED ONLY when pBaseLineTypeForCalculation = DYNAMIC and parameter RDI_CUMULATE_ALL_BASELINES = 1)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <returns></returns>
        ''' <remarks>AG 15/01/2015 BA-2212</remarks>
        Public Function ResetWSForDynamicBL(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Read parameters and evaluate if deletion process must be or not executed (by default NOT)
                        Dim myParams As New SwParametersDelegate
                        Dim auxDataSet As ParametersDS
                        Dim deletionProcess As Integer = 0

                        resultData = myParams.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.BL_TYPE_FOR_CALCULATIONS.ToString, pAnalyzerModel)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            auxDataSet = DirectCast(resultData.SetDatos, ParametersDS)
                            If auxDataSet.tfmwSwParameters.Rows.Count > 0 AndAlso Not auxDataSet.tfmwSwParameters.First.IsValueTextNull _
                                AndAlso auxDataSet.tfmwSwParameters.First.ValueText = GlobalEnumerates.BaseLineType.DYNAMIC.ToString Then

                                resultData = myParams.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.RDI_CUMULATE_ALL_BASELINES.ToString, Nothing)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    auxDataSet = DirectCast(resultData.SetDatos, ParametersDS)
                                    If auxDataSet.tfmwSwParameters.Rows.Count > 0 AndAlso Not auxDataSet.tfmwSwParameters.First.IsValueNumericNull Then
                                        deletionProcess = CInt(auxDataSet.tfmwSwParameters.First.ValueNumeric)
                                    End If
                                End If

                            End If
                        End If

                        If Not resultData.HasError AndAlso deletionProcess = 1 Then
                            'Get the maximum BaseLineID with type STATIC
                            Dim myDAO As New twksWSBLinesDAO
                            Dim myDS As New BaseLinesDS
                            Dim skippedBaseLineID As Integer = 0

                            'STATIC records: Delete ALL except those with the maximum BaseLineID
                            resultData = myDAO.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, 1, GlobalEnumerates.BaseLineType.STATIC.ToString)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                skippedBaseLineID = DirectCast(resultData.SetDatos, Integer)
                                resultData = myDAO.DeleteByType(dbConnection, pAnalyzerID, GlobalEnumerates.BaseLineType.STATIC.ToString, skippedBaseLineID)
                            End If

                            'DYNAMIC records: Delete ALL except those with the maximum BaseLineID
                            If Not resultData.HasError Then
                                resultData = myDAO.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, 1, GlobalEnumerates.BaseLineType.DYNAMIC.ToString)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    skippedBaseLineID = DirectCast(resultData.SetDatos, Integer)
                                    resultData = myDAO.DeleteByType(dbConnection, pAnalyzerID, GlobalEnumerates.BaseLineType.DYNAMIC.ToString, skippedBaseLineID)
                                End If
                            End If

                            'Update remaining STATIC records with BaseLineID = 1
                            If Not resultData.HasError Then
                                resultData = myDAO.UpdateBaseLineIDByType(dbConnection, pAnalyzerID, 1, GlobalEnumerates.BaseLineType.STATIC.ToString)
                            End If

                            'Update remaining DYNAMIC records with BaseLineID = 2
                            If Not resultData.HasError Then
                                resultData = myDAO.UpdateBaseLineIDByType(dbConnection, pAnalyzerID, 2, GlobalEnumerates.BaseLineType.DYNAMIC.ToString)
                            End If

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.VerifyBLMovedToHistoric", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class

    Public Interface IWSBLinesDelegateValuesDeleter
        Function DeleteBLinesValues(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, ByVal pType As String, ByVal pBaseLineTypeForCalculation As String) As GlobalDataTO
    End Interface

End Namespace

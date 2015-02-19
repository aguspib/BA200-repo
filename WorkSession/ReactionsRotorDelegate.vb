Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL


    Partial Public Class ReactionsRotorDelegate
      
#Region "C R U D"

        ''' <summary>
        ''' Create a reaction rotors records
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReactionRotorDS">Reaction rotor data set</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 08/02/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pReactionRotorDS As ReactionsRotorDS) As GlobalDataTO

            'Dim result As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSReactionRotorDAO As New twksWSReactionsRotorDAO
                        myGlobalDataTO = myWSReactionRotorDAO.Create(dbConnection, pReactionRotorDS)
                    End If
                End If

                If Not myGlobalDataTO.HasError Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        '''' <summary>
        '''' Read reaction rotor records by the index key.
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pReactionRotorDS"></param>
        '''' <returns></returns>
        '''' <remarks>CREATED BY: TR 08/02/2011</remarks>
        'Public Function Read(ByVal pDBConnection As SqlConnection, ByVal pReactionRotorDS As ReactionsRotorDS) As GlobalDataTO

        '    'Dim result As Boolean = False
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myWSReactionRotorDAO As New twksWSReactionsRotorDAO
        '                myGlobalDataTO = myWSReactionRotorDAO.Read(dbConnection, pReactionRotorDS)
        '            End If
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.Read", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return myGlobalDataTO

        'End Function

        ''' <summary>
        ''' Update reactiion rotor records 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReactionRotorDS"></param>
        ''' <param name="pUpdateOnlyLastRotorTurn" ></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 08/02/2011
        ''' AG 12/05/2011 add parameter pUpdateOnlyLastRotorTurn</remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pReactionRotorDS As ReactionsRotorDS, _
                               ByVal pUpdateOnlyLastRotorTurn As Boolean) As GlobalDataTO

            'Dim result As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSReactionRotorDAO As New twksWSReactionsRotorDAO
                        If Not pUpdateOnlyLastRotorTurn Then
                            myGlobalDataTO = myWSReactionRotorDAO.Update(dbConnection, pReactionRotorDS)
                        Else
                            myGlobalDataTO = myWSReactionRotorDAO.UpdateMAXRotorTurnRecord(dbConnection, pReactionRotorDS)
                        End If

                    End If
                End If

                If Not myGlobalDataTO.HasError Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

#End Region

#Region "ALL & RESET"

        '''' <summary>
        '''' Read all records using the Work Session ID and the Analyzer ID
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID">Analyzer ID</param>
        '''' <returns></returns>
        '''' <remarks>
        '''' CREATED BY: TR 08/02/2011
        '''' </remarks>
        'Public Function ReadAll(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
        '    'Dim result As Boolean = False
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myWSReactionRotorDAO As New twksWSReactionsRotorDAO
        '                myGlobalDataTO = myWSReactionRotorDAO.ReadAll(dbConnection, pAnalyzerID)
        '            End If
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.ReadAll", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return myGlobalDataTO

        'End Function


        ''' <summary>
        ''' Delete all records in twksWSReactionsRotor table except the last rotorturn for each well number
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerId"></param>
        ''' <param name="pWorkSessionId"></param>
        ''' <returns></returns>
        ''' <remarks>AG 13/05/2011
        ''' AG 12/12/2011 - call also the method UpdateWellContentAndStatusAfterReset</remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionId As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReactionsRotorDAO

                        'resultData = myDAO.GetAllWellsLastTurn(dbConnection, pAnalyzerID)
                        'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        '    Dim myDS As New ReactionsRotorDS
                        '    myDS = CType(resultData.SetDatos, ReactionsRotorDS)

                        '    If myDS.twksWSReactionsRotor.Rows.Count > 0 Then
                        '        resultData = myDAO.Delete(dbConnection, myDS)
                        '    End If

                        '    If Not resultData.HasError Then
                        '        'Update the rotorTurn to 0
                        '        resultData = myDAO.UpdateRotorTurnAfterReset(dbConnection, pAnalyzerID, 0)
                        '    End If
                        'End If

                        'Delete all table contents but the last rotor turn
                        resultData = myDAO.Delete(dbConnection, pAnalyzerID)
                        If Not resultData.HasError Then
                            'Update the rotorTurn to 0
                            resultData = myDAO.UpdateRotorTurnAfterReset(dbConnection, pAnalyzerID, 0)

                            If Not resultData.HasError Then
                                'Update the WellContent (E or C) and WellStatus (R or X)
                                resultData = myDAO.UpdateWellContentAndStatusAfterReset(dbConnection, pAnalyzerID)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Used on change the reactions rotor
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerId"></param>
        ''' <param name="pWorkSessionId"></param>
        ''' <returns></returns>
        ''' <remarks>AG 13/05/2011</remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerId As String, _
                        ByVal pWorkSessionId As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReactionsRotorDAO
                        resultData = myDAO.DeleteAll(dbConnection, pAnalyzerId)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.DeleteAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Other public business methods"


        ''' <summary>
        ''' Read all records using pWellNumber in current WorkSession and Analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWellNumber"></param>
        ''' <param name="pAllRecordsFlag" ></param>
        ''' <returns>GlobalDataTO with data as ReactionsRotorDS</returns>
        ''' <remarks>AG 08/02/2011 - tested ok</remarks>
        Public Function ReadWellHistoricalUse(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                  ByVal pAnalyzerID As String, ByVal pWellNumber As Integer, _
                                  ByVal pAllRecordsFlag As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReactionsRotorDAO
                        resultData = myDAO.ReadWellHistoricalUse(dbConnection, pAnalyzerID, pWellNumber, pAllRecordsFlag)
                    End If

                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.ReadWellHistoricalUse", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Calculates the next value for the RotorNumber field in twksWSReactionsRotor table for the pWellNumber in parameter
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWellNumber"></param>
        ''' <returns>GlobalDataTO with data as integer</returns>
        ''' <remarks>AG 08/02/2011 - tested pending</remarks>
        Public Function GetNextRotorTurn(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                  ByVal pAnalyzerID As String, ByVal pWellNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReactionsRotorDAO
                        resultData = myDAO.ReadWellHistoricalUse(dbConnection, pAnalyzerID, pWellNumber, False)

                        If Not resultData.HasError Then
                            Dim nextRotorNumberValue As Integer = 1
                            Dim localDS As New ReactionsRotorDS
                            localDS = CType(resultData.SetDatos, ReactionsRotorDS)

                            If localDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                If Not localDS.twksWSReactionsRotor(0).IsRotorTurnNull Then
                                    nextRotorNumberValue = localDS.twksWSReactionsRotor(0).RotorTurn + 1
                                End If
                            End If
                            resultData.SetDatos = nextRotorNumberValue
                        End If
                    End If

                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.GetNextRotorTurn", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        Public Function GetAllWellsLastTurn(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReactionsRotorDAO
                        resultData = myDAO.GetAllWellsLastTurn(dbConnection, pAnalyzerID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.GetAllWellsLastTurn", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update the reactions rotor well status (pWellNumber) using the information sent by analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWellNumber"></param>
        ''' <param name="pWellStatus"></param>
        ''' <param name="pPrepID"></param>
        ''' <param name="pBAx00ActionCode" ></param>
        ''' <param name="pRotorName" >by now not is used but maybe it will be required for predilutions</param>
        ''' <returns>GlobalDataTO (data as ReactionsRotorDS)</returns>
        ''' <remarks>AG 03/06/2011
        ''' AG 24/11/2011 - add columns TestId, WashingSolutionR1, WashingSolutionR2
        ''' AG 03/07/2012 - use GET template method with nothing as connection </remarks>
        Public Function UpdateWellByArmStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                              ByVal pWorkSessionID As String, ByVal pWellNumber As Integer, _
                                              ByVal pWellStatus As String, ByVal pPrepID As Integer, _
                                              ByVal pBAx00ActionCode As GlobalEnumerates.AnalyzerManagerAx00Actions, _
                                              ByVal pRotorName As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim myReactionsDS As New ReactionsRotorDS

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wellContent As String = ""
                        Dim wellStatus As String = ""

                        Select Case pWellStatus
                            Case GlobalEnumerates.Ax00ArmWellStatusValues.R1.ToString
                                wellContent = "T"
                                wellStatus = "R1"

                            Case GlobalEnumerates.Ax00ArmWellStatusValues.PD.ToString
                                wellContent = "P"
                                wellStatus = "P"

                                'S1 (sample from samples rotor tube is dispensed into photometric rotor)
                                'PS (diluted sample from photometric rotor is dispensed into photometric rotor)
                            Case GlobalEnumerates.Ax00ArmWellStatusValues.S1.ToString, GlobalEnumerates.Ax00ArmWellStatusValues.PS.ToString
                                wellContent = "T"
                                wellStatus = "S"

                            Case GlobalEnumerates.Ax00ArmWellStatusValues.R2.ToString
                                wellContent = "T"
                                wellStatus = "R2"

                            Case GlobalEnumerates.Ax00ArmWellStatusValues.WS.ToString
                                'AG 13/12/2011 - When washing is performed no changes. Screen has to shown contaminated well
                                'wellContent = "W"
                                'wellStatus = "R"
                            Case GlobalEnumerates.Ax00ArmWellStatusValues.DU.ToString
                                'AG 13/12/2011 - When washing is performed no changes. Screen has to shown contaminated well
                                ''Fw indicates DU (dummy) when performes a Dummy and also when performes a WRun with System Water
                                ''Sw has to set wellContent = "W" and wellStatus = "R" only when WRun is performed (use the ActionCode)
                                'If pBAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_RUN_START Then
                                '    wellContent = "W"
                                '    wellStatus = "R"
                                'End If

                        End Select

                        'Add row into the DS and update database table
                        If wellContent <> "" AndAlso wellStatus <> "" Then
                            Dim executionID As Integer = -1
                            Dim execDelg As New ExecutionsDelegate
                            If pPrepID > 0 Then
                                resultData = execDelg.GetExecutionByPreparationID(dbConnection, pPrepID, pWorkSessionID, pAnalyzerID)
                                If Not resultData.HasError Then
                                    Dim execDS As New ExecutionsDS
                                    execDS = CType(resultData.SetDatos, ExecutionsDS)
                                    If execDS.twksWSExecutions.Rows.Count > 0 Then
                                        If Not execDS.twksWSExecutions(0).IsExecutionIDNull Then executionID = execDS.twksWSExecutions(0).ExecutionID
                                    End If
                                End If
                            End If

                            Dim newRow As ReactionsRotorDS.twksWSReactionsRotorRow
                            newRow = myReactionsDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                            With newRow
                                .AnalyzerID = pAnalyzerID
                                .WellNumber = pWellNumber
                                .WellContent = wellContent
                                .WellStatus = wellStatus
                                If executionID = -1 Then .SetExecutionIDNull() Else .ExecutionID = executionID
                                .SetTestIDNull() 'AG 24/11/2011
                                .SetWashingSolutionR1Null() 'AG 24/11/2011
                                .SetWashingSolutionR2Null() 'AG 24/11/2011

                                'Calculate field (WashRequiredFlag) using ExecutionID

                                'AG 07/02/2012 - This condition fails when R1 has no volume, so evaluate if wash cuvette is required when R1 or R2 dispensed (if no volume alarm it will be analyze twice)
                                'If executionID <> -1 AndAlso wellContent = "T" AndAlso wellStatus = "R1" Then
                                If executionID <> -1 AndAlso wellContent = "T" AndAlso (wellStatus = "R1" OrElse wellStatus = "R2") Then
                                    'AG 07/02/2012

                                    'Look if the last test prepared in this well is a well contaminator or not and his washing mode
                                    'Dim myExecutionsDlgte As New ExecutionsDelegate
                                    resultData = execDelg.GetExecutionContaminationCuvette(dbConnection, pAnalyzerID, pWorkSessionID, executionID)

                                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                        Dim myLocalDS As New AnalyzerManagerDS
                                        myLocalDS = CType(resultData.SetDatos, AnalyzerManagerDS)

                                        If myLocalDS.searchNext.Rows.Count > 0 Then
                                            If Not myLocalDS.searchNext(0).IsContaminationIDNull Then
                                                .WashRequiredFlag = True
                                                .WashedFlag = False
                                            End If

                                            'AG 24/11/2011
                                            If Not myLocalDS.searchNext(0).IsTestIDNull Then
                                                .TestID = myLocalDS.searchNext(0).TestID
                                            End If
                                            If Not myLocalDS.searchNext(0).IsWashingSolution1Null Then
                                                .WashingSolutionR1 = myLocalDS.searchNext(0).WashingSolution1
                                            End If
                                            If Not myLocalDS.searchNext(0).IsWashingSolution2Null Then
                                                .WashingSolutionR2 = myLocalDS.searchNext(0).WashingSolution2
                                            End If
                                            'AG 24/11/2011

                                        End If
                                    End If
                                End If
                            End With
                            myReactionsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(newRow)
                            myReactionsDS.AcceptChanges()
                            resultData = Update(Nothing, myReactionsDS, True)

                        End If

                    End If
                End If

                If Not resultData.HasError Then
                    resultData.SetDatos = myReactionsDS
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.UpdateWellByArmStatus", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try


            'Try
            '    resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
            '    If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
            '        dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
            '        If (Not dbConnection Is Nothing) Then
            '            Dim wellContent As String = ""
            '            Dim wellStatus As String = ""

            '            Select Case pWellStatus
            '                Case GlobalEnumerates.Ax00ArmWellStatusValues.R1.ToString
            '                    wellContent = "T"
            '                    wellStatus = "R1"

            '                Case GlobalEnumerates.Ax00ArmWellStatusValues.PD.ToString
            '                    wellContent = "P"
            '                    wellStatus = "P"

            '                Case GlobalEnumerates.Ax00ArmWellStatusValues.S1.ToString
            '                    wellContent = "T"
            '                    wellStatus = "S"

            '                Case GlobalEnumerates.Ax00ArmWellStatusValues.R2.ToString
            '                    wellContent = "T"
            '                    wellStatus = "R2"

            '                Case GlobalEnumerates.Ax00ArmWellStatusValues.WS.ToString
            '                    'AG 13/12/2011 - When washing is performed no changes. Screen has to shown contaminated well
            '                    'wellContent = "W"
            '                    'wellStatus = "R"
            '                Case GlobalEnumerates.Ax00ArmWellStatusValues.DU.ToString
            '                    'AG 13/12/2011 - When washing is performed no changes. Screen has to shown contaminated well
            '                    ''Fw indicates DU (dummy) when performes a Dummy and also when performes a WRun with System Water
            '                    ''Sw has to set wellContent = "W" and wellStatus = "R" only when WRun is performed (use the ActionCode)
            '                    'If pBAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_RUN_START Then
            '                    '    wellContent = "W"
            '                    '    wellStatus = "R"
            '                    'End If

            '            End Select

            '            'Add row into the DS and update database table
            '            If wellContent <> "" AndAlso wellStatus <> "" Then
            '                Dim executionID As Integer = -1
            '                Dim execDelg As New ExecutionsDelegate
            '                If pPrepID > 0 Then
            '                    resultData = execDelg.GetExecutionByPreparationID(dbConnection, pPrepID, pWorkSessionID, pAnalyzerID)
            '                    If Not resultData.HasError Then
            '                        Dim execDS As New ExecutionsDS
            '                        execDS = CType(resultData.SetDatos, ExecutionsDS)
            '                        If execDS.twksWSExecutions.Rows.Count > 0 Then
            '                            If Not execDS.twksWSExecutions(0).IsExecutionIDNull Then executionID = execDS.twksWSExecutions(0).ExecutionID
            '                        End If
            '                    End If
            '                End If

            '                Dim newRow As ReactionsRotorDS.twksWSReactionsRotorRow
            '                newRow = myReactionsDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
            '                With newRow
            '                    .AnalyzerID = pAnalyzerID
            '                    .WellNumber = pWellNumber
            '                    .WellContent = wellContent
            '                    .WellStatus = wellStatus
            '                    If executionID = -1 Then .SetExecutionIDNull() Else .ExecutionID = executionID
            '                    .SetTestIDNull() 'AG 24/11/2011
            '                    .SetWashingSolutionR1Null() 'AG 24/11/2011
            '                    .SetWashingSolutionR2Null() 'AG 24/11/2011

            '                    'Calculate field (WashRequiredFlag) using ExecutionID

            '                    'AG 07/02/2012 - This condition fails when R1 has no volume, so evaluate if wash cuvette is required when R1 or R2 dispensed (if no volume alarm it will be analyze twice)
            '                    'If executionID <> -1 AndAlso wellContent = "T" AndAlso wellStatus = "R1" Then
            '                    If executionID <> -1 AndAlso wellContent = "T" AndAlso (wellStatus = "R1" OrElse wellStatus = "R2") Then
            '                        'AG 07/02/2012

            '                        'Look if the last test prepared in this well is a well contaminator or not and his washing mode
            '                        'Dim myExecutionsDlgte As New ExecutionsDelegate
            '                        resultData = execDelg.GetExecutionContaminationCuvette(dbConnection, pAnalyzerID, pWorkSessionID, executionID)

            '                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
            '                            Dim myLocalDS As New AnalyzerManagerDS
            '                            myLocalDS = CType(resultData.SetDatos, AnalyzerManagerDS)

            '                            If myLocalDS.searchNext.Rows.Count > 0 Then
            '                                If Not myLocalDS.searchNext(0).IsContaminationIDNull Then
            '                                    .WashRequiredFlag = True
            '                                    .WashedFlag = False
            '                                End If

            '                                'AG 24/11/2011
            '                                If Not myLocalDS.searchNext(0).IsTestIDNull Then
            '                                    .TestID = myLocalDS.searchNext(0).TestID
            '                                End If
            '                                If Not myLocalDS.searchNext(0).IsWashingSolution1Null Then
            '                                    .WashingSolutionR1 = myLocalDS.searchNext(0).WashingSolution1
            '                                End If
            '                                If Not myLocalDS.searchNext(0).IsWashingSolution2Null Then
            '                                    .WashingSolutionR2 = myLocalDS.searchNext(0).WashingSolution2
            '                                End If
            '                                'AG 24/11/2011

            '                            End If
            '                        End If
            '                    End If
            '                End With
            '                myReactionsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(newRow)
            '                myReactionsDS.AcceptChanges()
            '                resultData = Update(dbConnection, myReactionsDS, True)
            '            End If

            '        End If

            '        If (Not resultData.HasError) Then
            '            'When the Database Connection was opened locally, then the Commit is executed
            '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
            '            resultData.SetDatos = myReactionsDS
            '        Else
            '            'When the Database Connection was opened locally, then the Rollback is executed
            '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
            '        End If
            '    End If

            'Catch ex As Exception
            '    'When the Database Connection was opened locally, then the Rollback is executed
            '    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

            '    resultData.HasError = True
            '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            '    resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

            '    'Dim myLogAcciones As New ApplicationLogManager()
            '    GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.UpdateWellByArmStatus", EventLogEntryType.Error, False)
            'Finally
            '    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            'End Try

            Return resultData
        End Function


        ''' <summary>
        ''' 1 For the current rotor turn add new record in table with well read
        ''' 2 Update all wells inside the washing station and the one who leave it
        ''' 3 After well baseline evaluation inform (if any) the rejected wells
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWellNumber"></param>
        ''' <param name="pRejectedWells"></param>
        ''' <param name="pInitializeBLWWell" ></param>
        ''' <param name="pReactionRotorMaxWells" ></param>
        ''' <param name="pWashStationInputWellOffset" ></param>
        ''' <param name="pWashStationOutputWellOffset" ></param>
        ''' <param name="pType"></param>
        ''' <returns>GlobalDataTo (ReactionsRotorDS)</returns>
        ''' <remarks>AG 08/06/2011
        ''' AG 24/11/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2
        ''' AG 18/02/2015 - BA-2285 - New status DX (dynamically rejected). Generate it when pType = DYNAMIC. Do not remove never when pType = STATIC
        ''' </remarks>
        Public Function InitializeNewRotorTurnWellStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                     ByVal pAnalyzerID As String, ByVal pWellNumber As Integer, ByVal pRejectedWells As String, _
                                     ByVal pInitializeBLWWell As Boolean, ByVal pReactionRotorMaxWells As Integer, _
                                     ByVal pWashStationOutputWellOffset As Integer, ByVal pWashStationInputWellOffset As Integer, _
                                     ByVal pType As GlobalEnumerates.BaseLineType) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                '' XBC 03/07/2012 - time estimation
                'Dim StartTime As DateTime = Now

                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myDAO As New twksWSReactionsRotorDAO
                        Dim myReactionsDS As New ReactionsRotorDS
                        Dim myRow As ReactionsRotorDS.twksWSReactionsRotorRow

                        If Not resultData.HasError Then

                            If pInitializeBLWWell Then 'Create records due the new ANSPHR instruction received

                                'FIRST: Set CurrentTurnFlag to False for the pWellNumber
                                resultData = myDAO.SetCurrentTurnFlagToFalse(dbConnection, pAnalyzerID, pWellNumber)

                                'SECOND: Create the pWellNumber with RotorTurn++
                                Dim nextRotorTurn As Integer = 1
                                Dim rejectedFlag As Boolean = False

                                resultData = GetNextRotorTurn(dbConnection, pWorkSessionID, pAnalyzerID, pWellNumber)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    nextRotorTurn = CType(resultData.SetDatos, Integer)
                                End If


                                'CASE: Treat the current well pWellNumber = ANSPHR.BLW
                                rejectedFlag = CBool(IIf(pRejectedWells.Contains(" " & pWellNumber.ToString & " "), True, False))

                                If Not resultData.HasError Then
                                    'Current well: ANSPHR.BLW
                                    myRow = myReactionsDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                                    With myRow
                                        .AnalyzerID = pAnalyzerID
                                        .WellNumber = pWellNumber
                                        .RotorTurn = nextRotorTurn
                                        .WellContent = "W" 'Washing station
                                        .SetExecutionIDNull()
                                        .CurrentTurnFlag = True
                                        .WashedFlag = False
                                        .SetWashRequiredFlagNull()
                                        .SetTestIDNull()
                                        .SetWashingSolutionR1Null()
                                        .SetWashingSolutionR2Null()

                                        If Not rejectedFlag Then
                                            .WellStatus = "R" 'Well status: Ready
                                            .RejectedFlag = False
                                        Else
                                            .WellStatus = "X" 'Well status: Rejected
                                            If pType = GlobalEnumerates.BaseLineType.DYNAMIC Then .WellStatus = "DX" 'AG 18/02/2015 - New status: Dynamically rejected

                                            'Temporally comment the true code and not reject never
                                            .RejectedFlag = True
                                            '.RejectedFlag = False
                                        End If

                                        'AG 12/12/2011 - Keep the cuvette contamination information only if not cuvette wash has been performed
                                        resultData = myDAO.ReadWellHistoricalUse(dbConnection, pAnalyzerID, pWellNumber, False) 'AG 12/12/2011 - read only last turn, not all well history

                                        If Not resultData.HasError Then
                                            Dim auxDS As New ReactionsRotorDS
                                            auxDS = CType(resultData.SetDatos, ReactionsRotorDS)
                                            If auxDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                                If auxDS.twksWSReactionsRotor(0).WashRequiredFlag AndAlso Not auxDS.twksWSReactionsRotor(0).WashedFlag Then
                                                    .WashedFlag = False
                                                    .WashRequiredFlag = True
                                                    If Not auxDS.twksWSReactionsRotor(0).IsTestIDNull Then .TestID = auxDS.twksWSReactionsRotor(0).TestID
                                                    If Not auxDS.twksWSReactionsRotor(0).IsWashingSolutionR1Null Then .WashingSolutionR1 = auxDS.twksWSReactionsRotor(0).WashingSolutionR1
                                                    If Not auxDS.twksWSReactionsRotor(0).IsWashingSolutionR2Null Then .WashingSolutionR2 = auxDS.twksWSReactionsRotor(0).WashingSolutionR2
                                                End If

                                                'AG 18/02/2015 BA-2285 the wells rejected by the dynamic base line cannot change their status until new dynamic base line is performed
                                                If pType = GlobalEnumerates.BaseLineType.STATIC AndAlso Not auxDS.twksWSReactionsRotor.First.IsWellStatusNull AndAlso auxDS.twksWSReactionsRotor.First.WellStatus = "DX" Then
                                                    .WellStatus = "DX"
                                                    .RejectedFlag = True
                                                End If
                                                'AG 18/02/2015
                                            End If
                                        End If
                                        'AG 12/12/2011

                                    End With
                                    myReactionsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(myRow)
                                    myReactionsDS.AcceptChanges()
                                    resultData = myDAO.Create(dbConnection, myReactionsDS)
                                End If

                                'Update all wells inside WashingStation with WellContent = "W"
                                Dim myWellList As String = ""
                                Dim wellRealValue As Integer = pWellNumber
                                For wellID = pWellNumber + pWashStationOutputWellOffset To pWellNumber + pWashStationInputWellOffset
                                    wellRealValue = GetRealWellNumber(wellID, pReactionRotorMaxWells)
                                    If wellRealValue <> pWellNumber Then
                                        myRow = myReactionsDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                                        With myRow
                                            .AnalyzerID = pAnalyzerID
                                            .WellNumber = wellRealValue
                                            .SetRotorTurnNull()
                                            .WellContent = "W" 'Washing station
                                        End With
                                        myReactionsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(myRow)

                                        If String.Equals(myWellList, "") Then
                                            myWellList &= wellRealValue.ToString
                                        Else
                                            myWellList &= ", " & wellRealValue.ToString
                                        End If
                                    End If
                                Next
                                myReactionsDS.AcceptChanges()
                                If Not String.Equals(myWellList, String.Empty) Then resultData = myDAO.SetValuesForWashingStationWells(dbConnection, pAnalyzerID, myWellList)

                                'Update the well who is leaving the WashingStation as WellContent = 'E' (empty) or 'C' (contaminated)
                                If Not resultData.HasError Then
                                    wellRealValue = GetRealWellNumber(pWellNumber + pWashStationOutputWellOffset - 1, pReactionRotorMaxWells)
                                    'resultData = myDAO.ReadWellHistoricalUse(dbConnection, pAnalyzerID, wellRealValue, True)
                                    resultData = myDAO.ReadWellHistoricalUse(dbConnection, pAnalyzerID, wellRealValue, False) 'AG 12/12/2011 - read only last turn, not all well history

                                    If Not resultData.HasError Then
                                        Dim auxDS As New ReactionsRotorDS
                                        auxDS = CType(resultData.SetDatos, ReactionsRotorDS)
                                        If auxDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                            'Search if exist mark for wash required
                                            Dim listWashRequired As New List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                                            listWashRequired = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In auxDS.twksWSReactionsRotor _
                                                                Where a.WashRequiredFlag = True Select a).ToList

                                            Dim myWellContent As String = "E"
                                            If listWashRequired.Count > 0 Then
                                                'Search if well has already washed or not
                                                Dim alreadyWashed As Boolean = False
                                                For Each row As ReactionsRotorDS.twksWSReactionsRotorRow In auxDS.twksWSReactionsRotor
                                                    If row.WashedFlag Then
                                                        alreadyWashed = True
                                                        Exit For
                                                    ElseIf row.WashRequiredFlag Then
                                                        Exit For
                                                    End If
                                                Next
                                                If Not alreadyWashed Then myWellContent = "C"
                                            End If

                                            Dim updateDS As New ReactionsRotorDS
                                            myRow = updateDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                                            With myRow
                                                .AnalyzerID = pAnalyzerID
                                                .WellNumber = wellRealValue
                                                .SetRotorTurnNull()
                                                .WellContent = myWellContent
                                                .SetWellStatusNull() 'No change WellStatus
                                            End With
                                            updateDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(myRow)
                                            updateDS.AcceptChanges()
                                            myReactionsDS.twksWSReactionsRotor.ImportRow(myRow)
                                            myReactionsDS.AcceptChanges()
                                            resultData = myDAO.UpdateMAXRotorTurnRecord(dbConnection, updateDS)

                                            listWashRequired = Nothing
                                        End If
                                    End If
                                End If


                            Else 'Records were created previously when the ANSPHR instruction was received. But NOW we are treating these results (when well rejections parameter FIFO becomes completed)

                                'CASE: Update the RejectedWells after the initialization phase has finished
                                If Not resultData.HasError Then
                                    If Not String.Equals(pRejectedWells, String.Empty) Then
                                        Dim rejectedWellList() As String
                                        If pRejectedWells.Contains(",") Then
                                            rejectedWellList = Split(pRejectedWells, ",")
                                        Else
                                            ReDim rejectedWellList(0)
                                            rejectedWellList(0) = pRejectedWells
                                        End If

                                        'AG 18/02/2015 BA-2285 - Read the current status for the reactions rotor
                                        Dim auxDS As New ReactionsRotorDS
                                        Dim linqResults As List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                                        If pType = GlobalEnumerates.BaseLineType.DYNAMIC AndAlso rejectedWellList.Length > 0 Then
                                            resultData = myDAO.GetAllWellsLastTurn(dbConnection, pAnalyzerID)
                                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                                auxDS = CType(resultData.SetDatos, ReactionsRotorDS)
                                            End If
                                        End If
                                        'AG 18/02/2015

                                        myReactionsDS.Clear()
                                        For i As Integer = 0 To rejectedWellList.Length - 1
                                            myRow = myReactionsDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                                            With myRow
                                                .AnalyzerID = pAnalyzerID
                                                .WellNumber = CInt(rejectedWellList(i))
                                                '.RotorTurn = nextRotorTurn
                                                .SetRotorTurnNull()
                                                .SetWellContentNull() 'No change WellContent
                                                .SetExecutionIDNull()
                                                .WellStatus = "X" 'Well status: Rejected

                                                'AG 18/02/2014 BA-2285 - If previous status of this well was DX (dynamically rejected) do not change it!!!
                                                If pType = GlobalEnumerates.BaseLineType.DYNAMIC AndAlso Not auxDS Is Nothing AndAlso auxDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                                    linqResults = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In auxDS.twksWSReactionsRotor _
                                                                   Where a.WellNumber = .WellNumber Select a).ToList
                                                    If linqResults.Count > 0 AndAlso Not linqResults.First.IsWellStatusNull AndAlso linqResults.First.WellStatus = "DX" Then
                                                        .WellStatus = "DX" 'Well status must remain to: Dynamically Rejected
                                                    End If
                                                End If
                                                'AG 18/02/2015

                                                'Temporally comment the true code and not reject never
                                                .RejectedFlag = True
                                                '.RejectedFlag = False
                                            End With
                                            myReactionsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(myRow)

                                        Next
                                        myReactionsDS.AcceptChanges()
                                        If myReactionsDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                            resultData = myDAO.UpdateMAXRotorTurnRecord(dbConnection, myReactionsDS)
                                        End If
                                        linqResults = Nothing 'AG 18/02/2015 BA-2285
                                    End If
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = myReactionsDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

                '' XBC 03/07/2012 - time estimation
                ''Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity("Inserting new wells (discarded) " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "ReactionsRotorDelegate.InitializeNewRotorTurnWellStatus", EventLogEntryType.Information, False)

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.InitializeNewRotorTurnWellStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' On create workSession asure the reactions rotor is created from wells 1 to 120
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pAnalyzerModel" ></param>
        ''' <param name="pSetWellContentToEmpty" >TRUE: All well has WellContent = 'E'. False not</param>
        ''' <returns></returns>
        ''' <remarks>AG dd/mm/yy - creation
        ''' AG 24/11/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2</remarks>
        Public Function CreateWSReactionsRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String, _
                                               ByVal pSetWellContentToEmpty As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Read the max wells in reactions rotor
                        Dim maxWells As Integer = 120
                        Dim paramsDelg As New SwParametersDelegate
                        resultData = paramsDelg.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.MAX_REACTROTOR_WELLS.ToString, Nothing)
                        If Not resultData.HasError Then
                            Dim paramsDS As New ParametersDS
                            paramsDS = CType(resultData.SetDatos, ParametersDS)
                            If paramsDS.tfmwSwParameters.Rows.Count > 0 Then
                                If Not paramsDS.tfmwSwParameters(0).IsValueNumericNull Then maxWells = CInt(paramsDS.tfmwSwParameters(0).ValueNumeric)
                            End If
                        End If

                        'Get the current reactions rotor
                        Dim myDAO As New twksWSReactionsRotorDAO
                        resultData = myDAO.GetAllWellsLastTurn(dbConnection, pAnalyzerID)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim currentReactionsDS As New ReactionsRotorDS
                            currentReactionsDS = CType(resultData.SetDatos, ReactionsRotorDS)

                            'Complete the reactions rotor if needed
                            Dim myRes As List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                            Dim createDS As New ReactionsRotorDS
                            Dim newRow As ReactionsRotorDS.twksWSReactionsRotorRow
                            For i = 1 To maxWells
                                Dim auxI = i
                                myRes = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In currentReactionsDS.twksWSReactionsRotor _
                                         Where a.WellNumber = auxI Select a).ToList

                                If myRes.Count = 0 Then
                                    newRow = createDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                                    newRow.AnalyzerID = pAnalyzerID
                                    newRow.WellNumber = auxI
                                    newRow.RotorTurn = 0
                                    newRow.WellContent = "E"
                                    newRow.WellStatus = "R"
                                    newRow.RejectedFlag = False
                                    newRow.SetCurrentTurnFlagNull()
                                    newRow.SetWashedFlagNull()
                                    newRow.SetExecutionIDNull()
                                    newRow.SetWashRequiredFlagNull()
                                    newRow.SetTestIDNull() 'AG 24/11/2011
                                    newRow.SetWashingSolutionR1Null()  'AG 24/11/2011
                                    newRow.SetWashingSolutionR2Null() 'AG 24/11/2011
                                    createDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(newRow)
                                End If
                            Next
                            createDS.AcceptChanges()

                            If createDS.twksWSReactionsRotor.Rows.Count > 0 Then resultData = myDAO.Create(dbConnection, createDS)

                        End If

                        If Not resultData.HasError And pSetWellContentToEmpty Then resultData = myDAO.SetToEmptyTheWellsInWashStation(dbConnection, pAnalyzerID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.CreateWSReactionsRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' This method creates a new reactions rotor after perform the new rotor utility
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ChangeRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        resultData = DeleteAll(dbConnection, pAnalyzerID, "")
                        If Not resultData.HasError Then
                            resultData = CreateWSReactionsRotor(dbConnection, pAnalyzerID, pAnalyzerModel, True)
                        End If

                        'Create the new reactions rotor configuration
                        If Not resultData.HasError Then
                            Dim AnReactionsRotor As New AnalyzerReactionsRotorDelegate
                            resultData = AnReactionsRotor.Delete(dbConnection, pAnalyzerID)
                            If Not resultData.HasError Then
                                Dim newRotorConfg As New AnalyzerReactionsRotorDS
                                Dim row As AnalyzerReactionsRotorDS.tcfgAnalyzerReactionsRotorRow
                                row = newRotorConfg.tcfgAnalyzerReactionsRotor.NewtcfgAnalyzerReactionsRotorRow

                                row.AnalyzerID = pAnalyzerID
                                row.InstallDate = Now
                                row.BLParametersRejected = False
                                row.WellsRejectedNumber = 0

                                newRotorConfg.tcfgAnalyzerReactionsRotor.AddtcfgAnalyzerReactionsRotorRow(row)
                                newRotorConfg.AcceptChanges()
                                resultData = AnReactionsRotor.Create(dbConnection, newRotorConfg)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.ChangeRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Set WellContent = E in all well whose WellContent is W
        ''' (this method is used when analyzer leaves Running and enter in StandBy due in this state the wash station doesnt work)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 29/06/2011 - Creation</remarks>
        Public Function SetToEmptyTheWellsInWashStation(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

            'Dim result As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSReactionRotorDAO As New twksWSReactionsRotorDAO
                        myGlobalDataTO = myWSReactionRotorDAO.SetToEmptyTheWellsInWashStation(dbConnection, pAnalyzerID)
                    End If
                End If

                If Not myGlobalDataTO.HasError Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.SetToEmptyTheWellsInWashStation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' After leave running mode some special business has to be performed
        ''' 1) The current well inside the washing station has to change his WellContent to "C" contaminated or "E" empty
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pReactionsRotorLastTurn"></param>
        ''' <returns>GlobalDataTo with ReactionsRotorDS that contains the contaminated wells currently in the washing station</returns>
        ''' <remarks>AG 13/12/2011
        ''' AG 26/09/2012 - assign WellContent = "C" for all well with WashRequiredFlag = True not only those inside washing station
        '''                 This change is for mark wells as contaminated also after ABORT work session</remarks>
        Public Function AsignFinalValuesAfterLeavingRunning(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                                            ByVal pReactionsRotorLastTurn As ReactionsRotorDS) As GlobalDataTO
            Dim newContaminatedWellsDS As New ReactionsRotorDS
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReactionsRotorDAO

                        'Search the contaminated wells and updated to WellContent = "C"
                        Dim myList As List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                        'AG 26/09/2012
                        'myList = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionsRotorLastTurn.twksWSReactionsRotor _
                        '            Where a.WellContent = "W" And a.WashRequiredFlag = True Select a).ToList
                        myList = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionsRotorLastTurn.twksWSReactionsRotor _
                                  Where a.WashRequiredFlag = True Select a).ToList
                        'AG 26/09/2012

                        If myList.Count > 0 Then
                            For Each item As ReactionsRotorDS.twksWSReactionsRotorRow In myList
                                If item.WellContent <> "C" Then 'AG 26/09/2012
                                    item.BeginEdit()
                                    'item.WellStatus = "C" 
                                    item.WellContent = "C" 'AG 26/09/2012 field wellcontent not wellstatus
                                    item.EndEdit()
                                    newContaminatedWellsDS.twksWSReactionsRotor.ImportRow(item)
                                End If
                            Next
                            newContaminatedWellsDS.AcceptChanges()

                            'Update only last turn
                            If newContaminatedWellsDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                resultData = Update(dbConnection, newContaminatedWellsDS, True)
                            End If
                        End If

                        'All wells with WellContent = 'W' changes to WellContent = 'E' or to 'C'
                        If (Not resultData.HasError) Then
                            resultData = myDAO.SetToEmptyTheWellsInWashStation(dbConnection, pAnalyzerID)
                        End If

                    End If
                End If

                If Not resultData.HasError Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    resultData.SetDatos = newContaminatedWellsDS
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.AsignFinalValuesAfterLeavingRunning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get the reactions rotor well details for show into the presentation layer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pRotorPositionDS"></param>
        ''' <returns>GlobalDataTo with setDatos as ReactionRotorDetailsDS</returns>
        ''' <remarks>AG 07/12/2011</remarks>
        Public Function GetPositionReactionsInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorPositionDS As WSRotorContentByPositionDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSReactionsRotorDAO
                        resultData = myDAO.ReadReactionsDetails(dbConnection, _
                                             pRotorPositionDS.twksWSRotorContentByPosition(0).AnalyzerID, _
                                             pRotorPositionDS.twksWSRotorContentByPosition(0).RingNumber, _
                                             pRotorPositionDS.twksWSRotorContentByPosition(0).CellNumber, _
                                             pRotorPositionDS.twksWSRotorContentByPosition(0).WorkSessionID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.GetPositionReactionsInfo", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Change the Analyzer identifier of the informed Reactions Rotor WorkSession.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerIDNew">current connected Analyzer Identifier</param>
        ''' <param name="pAnalyzerIDOld">old connected Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 11/06/2012
        ''' </remarks>
        Public Function UpdateWSAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerIDNew As String, ByVal pAnalyzerIDOld As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSReactionsRotorDAO As New twksWSReactionsRotorDAO
                        resultData = myWSReactionsRotorDAO.UpdateWSAnalyzerID(dbConnection, pAnalyzerIDNew, pAnalyzerIDOld)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' When analyzer enters in StandBy repaint the whole reactions rotor, not only the WashStation wells
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns>GlobalDataTo with ReactionsRotorDS</returns>
        ''' <remarks>AG 14/11/2014 BA-2065 REFACTORING
        '''          AG 18/02/2015 - BA-2285 - add new status DX (dynamically rejected)
        ''' </remarks>
        Public Function RepaintAllReactionsRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim newWellsDS As New ReactionsRotorDS

                        'Update the reactions rotor table (WellContent = 'E' or 'C' for all wells, we are in standby)
                        resultData = GetAllWellsLastTurn(dbConnection, pAnalyzerID)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim currentWellsDS As ReactionsRotorDS

                            currentWellsDS = CType(resultData.SetDatos, ReactionsRotorDS)
                            'All wells with WellContent = 'W' changes to WellContent = 'E' or to 'C'
                            'resultData = reactionsDelegate.SetToEmptyTheWellsInWashStation(dbConnection, AnalyzerIDAttribute)
                            Dim contaminatedWellsDS As New ReactionsRotorDS
                            resultData = AsignFinalValuesAfterLeavingRunning(dbConnection, pAnalyzerID, currentWellsDS)
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                contaminatedWellsDS = CType(resultData.SetDatos, ReactionsRotorDS)
                            End If

                            'Read again the complete current reactions rotor (last turn)
                            resultData = GetAllWellsLastTurn(dbConnection, pAnalyzerID)
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                currentWellsDS = CType(resultData.SetDatos, ReactionsRotorDS)

                                'Finally prepare DS for inform presentation with the wells inside Washing Station when Running has finished
                                For Each item As ReactionsRotorDS.twksWSReactionsRotorRow In currentWellsDS.twksWSReactionsRotor.Rows
                                    item.BeginEdit()
                                    'WellContent must be 'E' (empty) or 'C' (contaminated)
                                    If item.WellContent <> "E" AndAlso item.WellContent <> "C" Then item.WellContent = "E"

                                    'AG 18/02/2015 BA-2285 - WellStatus must be 'R' (ready) or 'X' (rejected) or 'DX' (dynamically rejected)
                                    If item.WellStatus <> "R" AndAlso item.WellStatus <> "X" AndAlso item.WellStatus <> "DX" Then item.WellStatus = "R"
                                    item.EndEdit()

                                    newWellsDS.twksWSReactionsRotor.ImportRow(item)
                                Next
                                newWellsDS.AcceptChanges()
                            End If

                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = newWellsDS
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.RepaintAllReactionsRotor", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' For a lineal value from -N .. N returns the circular value inside range 1 ... pMaxRotorWells (120)
        ''' 
        ''' No connection is needed
        ''' </summary>
        ''' <param name="pWellNumber"></param>
        ''' <param name="pMaxRotorWells"></param>
        ''' <returns></returns>
        ''' <remarks>AG 09/06/2011</remarks>
        Public Function GetRealWellNumber(ByVal pWellNumber As Integer, ByVal pMaxRotorWells As Integer) As Integer
            Dim myValue As Integer = pWellNumber
            Try
                If pWellNumber <= 0 Then
                    myValue = pMaxRotorWells + pWellNumber
                            If myValue = 0 Then myValue = pMaxRotorWells

                        ElseIf pWellNumber > pMaxRotorWells Then
                            myValue = pWellNumber - pMaxRotorWells
                        End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReactionsRotorDelegate.GetRealWellNumber", EventLogEntryType.Error, False)
            End Try
            Return myValue
        End Function


#End Region


    End Class

End Namespace

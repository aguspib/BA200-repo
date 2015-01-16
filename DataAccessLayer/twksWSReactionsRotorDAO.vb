Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class twksWSReactionsRotorDAO
        Inherits DAOBase



#Region "C R U D"

        ''' <summary>
        ''' Create a new record.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReactionRotorDS">Reaction Rotor Data Set</param>
        ''' <returns>On the global dataset the affected record result.</returns>
        ''' <remarks>CREATED BY: TR 08/02/2011
        ''' AG 15/02/2011 add RejectedFlag column
        ''' AG 12/05/2011 remove WorkSessionID and rename RotorNumber -> RotorTurn
        ''' AG 06/06/2011 - add fields CurrentTurnFlag, WashedFlag and WashRequiredFlag
        ''' AG 24/11/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2</remarks>
        Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pReactionRotorDS As ReactionsRotorDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pReactionRotorDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    Dim STR_NULL As String = " NULL, "

                    For Each reactionRotorRow As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionRotorDS.twksWSReactionsRotor
                        cmdText &= " INSERT INTO twksWSReactionsRotor "
                        cmdText &= " (AnalyzerID, WellNumber, RotorTurn, WellContent, WellStatus, ExecutionID, RejectedFlag "

                        'AG 24/11/2011 + AG 06/06/2011
                        If Not reactionRotorRow.IsWashRequiredFlagNull Then cmdText &= " , WashRequiredFlag "
                        If Not reactionRotorRow.IsCurrentTurnFlagNull Then cmdText &= " , CurrentTurnFlag "
                        If Not reactionRotorRow.IsWashedFlagNull Then cmdText &= " , WashedFlag "
                        If Not reactionRotorRow.IsTestIDNull Then cmdText &= " , TestID "
                        If Not reactionRotorRow.IsWashingSolutionR1Null Then cmdText &= " , WashingSolutionR1 "
                        If Not reactionRotorRow.IsWashingSolutionR2Null Then cmdText &= " , WashingSolutionR2 "
                        'AG 24/11/2011 + AG 06/06/2011

                        cmdText &= " ) VALUES ("

                        cmdText &= String.Format("'{0}',", reactionRotorRow.AnalyzerID) 'Required
                        cmdText &= reactionRotorRow.WellNumber & ", " 'Required
                        cmdText &= reactionRotorRow.RotorTurn & ", " 'Required

                        If Not reactionRotorRow.IsWellContentNull Then
                            cmdText &= String.Format("'{0}',", reactionRotorRow.WellContent)
                        Else
                            cmdText &= STR_NULL
                        End If

                        If Not reactionRotorRow.IsWellStatusNull Then
                            cmdText &= String.Format("'{0}',", reactionRotorRow.WellStatus)
                        Else
                            cmdText &= STR_NULL
                        End If

                        If Not reactionRotorRow.IsExecutionIDNull Then
                            cmdText &= String.Format("'{0}',", reactionRotorRow.ExecutionID)
                        Else
                            cmdText &= STR_NULL
                        End If

                        If Not reactionRotorRow.IsRejectedFlagNull Then
                            cmdText &= IIf(reactionRotorRow.RejectedFlag, 1, 0).ToString
                        Else
                            cmdText &= "0  " 'False
                        End If

                        'AG 24/11/2011 + AG 06/06/2011
                        If Not reactionRotorRow.IsWashRequiredFlagNull Then cmdText &= ", " & IIf(reactionRotorRow.WashRequiredFlag, 1, 0).ToString
                        If Not reactionRotorRow.IsCurrentTurnFlagNull Then cmdText &= ", " & IIf(reactionRotorRow.CurrentTurnFlag, 1, 0).ToString
                        If Not reactionRotorRow.IsWashedFlagNull Then cmdText &= ", " & IIf(reactionRotorRow.WashedFlag, 1, 0).ToString
                        If Not reactionRotorRow.IsTestIDNull Then cmdText &= ", " & String.Format("'{0}'", reactionRotorRow.TestID)
                        If Not reactionRotorRow.IsWashingSolutionR1Null Then cmdText &= ", " & String.Format("'{0}'", reactionRotorRow.WashingSolutionR1)
                        If Not reactionRotorRow.IsWashingSolutionR2Null Then cmdText &= ", " & String.Format("'{0}'", reactionRotorRow.WashingSolutionR2)
                        'AG 24/11/2011 + AG 06/06/2011

                        cmdText &= String.Format("){0}", vbNewLine) 'insert line break

                    Next

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        '''' <summary>
        '''' Read Record by index
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pReactionRotorDS"></param>
        '''' <returns></returns>
        '''' <remarks>CREATED BY: TR 08/02/2011
        '''' AG 15/02/2011 - add RejectedFlag column
        '''' AG 12/05/2011 remove WorkSessionID and rename RotorNumber -> RotorTurn
        '''' AG 06/06/2011 - add fields CurrentTurnFlag, WashedFlag and WashRequiredFlag
        '''' AG 24/11/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2</remarks>
        'Public Function Read(ByVal pDBConnection As SqlConnection, ByVal pReactionRotorDS As ReactionsRotorDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

        '        ElseIf (Not pReactionRotorDS Is Nothing) Then
        '            Dim cmdText As String = ""
        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection

        '            cmdText = " SELECT AnalyzerID, WellNumber, RotorTurn, WellContent, WellStatus, ExecutionID, RejectedFlag, CurrentTurnFlag, WashedFlag, WashRequiredFlag, TestID, WashingSolutionR1, WashingSolutionR2 "
        '            cmdText &= " FROM twksWSReactionsRotor WHERE "
        '            cmdText &= " AnalyzerID = '" & pReactionRotorDS.twksWSReactionsRotor(0).AnalyzerID & "' "
        '            cmdText &= " AND WellNumber = " & pReactionRotorDS.twksWSReactionsRotor(0).WellNumber
        '            cmdText &= " AND RotorTurn = " & pReactionRotorDS.twksWSReactionsRotor(0).RotorTurn

        '            dbCmd.CommandText = cmdText
        '            Dim myDataDS As New ReactionsRotorDS
        '            Dim dbDataAdapter As New SqlDataAdapter(dbCmd)
        '            dbDataAdapter.Fill(myDataDS.twksWSReactionsRotor)

        '            myGlobalDataTO.SetDatos = myDataDS

        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.Read", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        ''' <summary>
        ''' Update the records by the table index.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReactionRotorDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 08/02/2011
        ''' AG 15/02/2011 - add RejectedFlag column
        ''' AG 12/05/2011 remove WorkSessionID and rename RotorNumber -> RotorTurn
        ''' AG 06/06/2011 - add fields CurrentTurnFlag, WashedFlag and WashRequiredFlag
        ''' AG 24/11/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2</remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pReactionRotorDS As ReactionsRotorDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pReactionRotorDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    Dim STR_NULL As String = " NULL, "

                    For Each reactionRotorRow As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionRotorDS.twksWSReactionsRotor
                        cmdText &= " UPDATE twksWSReactionsRotor "
                        cmdText &= " SET "
                        
                        If Not reactionRotorRow.IsWellContentNull Then
                            cmdText &= " WellContent = " & String.Format(" '{0}',", reactionRotorRow.WellContent)
                        Else
                            'cmdText &= " WellContent = " & STR_NULL
                        End If

                        If Not reactionRotorRow.IsWellStatusNull Then
                            cmdText &= " WellStatus = " & String.Format("'{0}',", reactionRotorRow.WellStatus)
                        Else
                            'cmdText &= " WellStatus = " & STR_NULL
                        End If

                        If Not reactionRotorRow.IsRejectedFlagNull Then
                            cmdText &= " RejectedFlag = " & IIf(reactionRotorRow.RejectedFlag, 1, 0).ToString & ", "
                        End If

                        If Not reactionRotorRow.IsExecutionIDNull Then
                            cmdText &= " ExecutionID = " & reactionRotorRow.ExecutionID
                        Else
                            cmdText &= " ExecutionID = NULL "
                        End If

                        'AG 06/06/2011
                        If Not reactionRotorRow.IsCurrentTurnFlagNull Then
                            cmdText &= " , CurrentTurnFlag = " & CStr(IIf(reactionRotorRow.CurrentTurnFlag, 1, 0))
                        End If

                        If Not reactionRotorRow.IsWashedFlagNull Then
                            cmdText &= " , WashedFlag = " & CStr(IIf(reactionRotorRow.WashedFlag, 1, 0))
                        End If

                        If Not reactionRotorRow.IsWashRequiredFlagNull Then
                            cmdText &= " , WashRequiredFlag = " & CStr(IIf(reactionRotorRow.WashRequiredFlag, 1, 0))
                        End If
                        'AG 06/06/2011

                        'AG 24/11/2011
                        If Not reactionRotorRow.IsTestIDNull Then
                            cmdText &= " , TestID = " & String.Format("'{0}'", reactionRotorRow.TestID)
                        End If

                        If Not reactionRotorRow.IsWashingSolutionR1Null Then
                            cmdText &= " , WashingSolutionR1 = " & String.Format("'{0}'", reactionRotorRow.WashingSolutionR1)
                        End If

                        If Not reactionRotorRow.IsWashingSolutionR2Null Then
                            cmdText &= " , WashingSolutionR2 = " & String.Format("'{0}'", reactionRotorRow.WashingSolutionR2)
                        End If
                        'AG 24/11/2011

                        cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", reactionRotorRow.AnalyzerID) 'Required
                        cmdText &= " AND WellNumber = " & reactionRotorRow.WellNumber 'Required
                        cmdText &= " AND RotorTurn = " & reactionRotorRow.RotorTurn  'Required

                        cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    Next

                    If cmdText <> "" Then
                        dbCmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Delete all records but the last rotor turn (CurrentTurnFlag = 1)
        ''' Records with CurrentTurnFlag = 0 will be deleted
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 06/06/2011 - add fields CurrentTurnFlag, WashedFlag and WashRequiredFlag</remarks>
        Public Function Delete(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                    'ElseIf (Not pReactionRotorDS Is Nothing) Then
                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    'For Each reactionRotorRow As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionRotorDS.twksWSReactionsRotor
                    '    If reactionRotorRow.RotorTurn > 0 Then 'RotorTurn = 0 means is the previous worksession last turn
                    '        cmdText &= " DELETE FROM twksWSReactionsRotor "
                    '        cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", reactionRotorRow.AnalyzerID) 'Required
                    '        cmdText &= " AND WellNumber = " & reactionRotorRow.WellNumber   'Required
                    '        cmdText &= " AND RotorTurn < " & reactionRotorRow.RotorTurn  'Required

                    '        cmdText &= String.Format("{0}", vbNewLine) 'insert line break
                    '    End If
                    'Next
                    cmdText &= " DELETE FROM twksWSReactionsRotor "
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) 'Required
                    cmdText &= " AND CurrentTurnFlag = 0 "

                    If cmdText <> "" Then
                        dbCmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End If

                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "ALL & RESET"

        '''' <summary>
        '''' Read all records by the Analyzer ID.
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID">Analyzer ID</param>
        '''' <returns></returns>
        '''' <remarks>
        '''' CREATE BY: TR 08/02/2011
        '''' AG 15/02/2011 - add RejectedFlag column
        '''' AG 12/05/2011 remove WorkSessionID and rename RotorNumber -> RotorTurn
        '''' AG 06/06/2011 - add fields CurrentTurnFlag, WashedFlag and WashRequiredFlag
        '''' AG 24/11/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2</remarks>
        'Public Function ReadAll(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        ElseIf pAnalyzerID <> "" Then

        '            Dim cmdText As String = ""
        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection

        '            cmdText = " SELECT AnalyzerID, WellNumber, RotorTurn, WellContent, WellStatus, ExecutionID, RejectedFlag, CurrentTurnFlag, WashedFlag, WashRequiredFlag, TestID, WashingSolutionR1, WashingSolutionR2 "
        '            cmdText &= " FROM twksWSReactionsRotor WHERE "
        '            cmdText &= String.Format(" AnalyzerID = '{0}'", pAnalyzerID)

        '            dbCmd.CommandText = cmdText

        '            Dim myDataDS As New ReactionsRotorDS
        '            Dim dbDataAdapter As New SqlDataAdapter(dbCmd)
        '            dbDataAdapter.Fill(myDataDS.twksWSReactionsRotor)

        '            myGlobalDataTO.SetDatos = myDataDS

        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.ReadAll", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        ''' <summary>
        ''' Delete table by Analyzer and WorkSession identifiers
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 08/02/2011
        ''' AG 12/05/2011 remove WorkSessionID and rename RotorNumber -> RotorTurn</remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = "DELETE twksWSReactionsRotor" & vbCrLf & _
                              "  WHERE AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"


                    Dim cmd As SqlCommand
                    cmd = pDBConnection.CreateCommand
                    cmd.CommandText = cmdText
                    cmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the last turn for each well number in table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 13/05/2011
        ''' AG 06/06/2011 - add fields CurrentTurnFlag, WashedFlag and WashRequiredFlag
        ''' AG 24/11/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2</remarks>
        Public Function GetAllWellsLastTurn(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        'AG 06/06/2011
                        '' DL 01/06/2011
                        ''cmdText = " SELECT  MAX(rotorturn) As RotorTurn, Wellnumber, AnalyzerID, RejectedFlag  FROM twksWSReactionsRotor " & _
                        ''          " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & _
                        ''          " GROUP BY WellNumber, AnalyzerID, RejectedFlag "
                        'cmdText &= "  SELECT T1.AnalyzerID, T1.WellNumber, T1.RotorTurn, T1.WellContent, T1.WellStatus, T1.ExecutionID, T1.RejectedFlag" & vbCrLf
                        'cmdText &= "    FROM twksWSReactionsRotor T1, (SELECT MAX (RotorTurn) as MaxRotor, analyzerid, wellnumber" & vbCrLf
                        'cmdText &= "                                     FROM twksWSReactionsRotor" & vbCrLf
                        'cmdText &= "                                 GROUP BY Analyzerid, Wellnumber) T2" & vbCrLf
                        'cmdText &= "   WHERE (t1.AnalyzerID = t2.AnalyzerID And t1.WellNumber = t2.WellNumber And t1.RotorTurn = T2.MaxRotor)" & vbCrLf
                        'cmdText &= "ORDER BY wellnumber"
                        '' End DL 01/06/2011

                        cmdText = " SELECT AnalyzerID, WellNumber, RotorTurn, WellContent, WellStatus, ExecutionID, RejectedFlag, CurrentTurnFlag, WashedFlag, WashRequiredFlag, TestID, WashingSolutionR1, WashingSolutionR2 "
                        cmdText &= " FROM twksWSReactionsRotor WHERE "
                        cmdText &= String.Format(" AnalyzerID = '{0}'", pAnalyzerID)
                        cmdText &= " AND CurrentTurnFlag = 1 "
                        'AG 06/06/2011

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim reactions As New ReactionsRotorDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(reactions.twksWSReactionsRotor)

                        resultData.SetDatos = reactions
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.GetAllWellsLastTurn", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update the column rotor turn to value pPreviousWorkSessionRotorTurn
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pPreviousWorkSessionRotorTurn"></param>
        ''' <returns></returns>
        ''' <remarks>AG 16/05/2011</remarks>
        Public Function UpdateRotorTurnAfterReset(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pPreviousWorkSessionRotorTurn As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " UPDATE twksWSReactionsRotor SET RotorTurn = " & pPreviousWorkSessionRotorTurn & vbCrLf
                    cmdText &= " , ExecutionID = NULL "
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) & vbCrLf 'Required
                    cmdText &= " AND RotorTurn > " & pPreviousWorkSessionRotorTurn
                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.UpdateRotorTurnAfterReset", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the column WellContent and WellStatus after RESET WS is performed
        ''' For all well the WellContent column posible values are E (empty) or C (cuvette contaminated)
        ''' For all well the WellStatus column posible values are E (empty) or C (cuvette contaminated)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 12/12/2011</remarks>
        Public Function UpdateWellContentAndStatusAfterReset(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    'Update WellContent
                    cmdText &= " UPDATE twksWSReactionsRotor SET WellContent = 'E' " & vbCrLf
                    cmdText &= " , ExecutionID = NULL "
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) & vbCrLf 'Required
                    cmdText &= " AND WellContent <> 'C' "

                    cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    'Update WellStatus
                    cmdText &= " UPDATE twksWSReactionsRotor SET WellStatus = 'R' " & vbCrLf
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) & vbCrLf 'Required
                    cmdText &= " AND WellStatus <> 'X' "


                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.UpdateWellContentAndStatusAfterReset", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "Other business methods"

        ''' <summary>
        ''' Read all records using pWellNumber in current WorkSession and Analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWellNumber"></param>
        ''' <param name="pAllRecordsFlag" ></param>
        ''' <returns>GlobalDataTO with data as ReactionsRotorDS</returns>
        ''' <remarks>AG 07/02/2011 - tested Ok
        ''' AG 12/05/2011 remove WorkSessionID and rename RotorNumber -> RotorTurn</remarks>
        Public Function ReadWellHistoricalUse(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                   ByVal pWellNumber As Integer, ByVal pAllRecordsFlag As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""

                        If pAllRecordsFlag Then
                            cmdText &= " SELECT * FROM twksWSReactionsRotor " & vbCrLf
                        Else
                            cmdText &= " SELECT TOP 1 * FROM twksWSReactionsRotor " & vbCrLf
                        End If

                        cmdText &= " WHERE WellNumber = " & pWellNumber & " " & vbCrLf
                        cmdText &= " AND AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                        cmdText &= " ORDER BY RotorTurn DESC "

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        'Dim myDataDS As New ReactionsRotorDS
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(myDataDS.twksWSReactionsRotor)
                        Dim myDataDS As New ReactionsRotorDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataDS.twksWSReactionsRotor)
                            End Using
                        End Using


                        resultData.SetDatos = myDataDS
                        resultData.HasError = False

                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.ReadWellHistoricalUse", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Updates the record by AnalyzerID, WellNumber and RotorTurn = MAX (RotorTurn for this AnalyzerID, WellNumber)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReactionRotorDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 12/05/2011
        ''' AG 06/06/2011 - add fields CurrentTurnFlag, WashedFlag and WashRequiredFlag
        ''' AG 07/12/2011 - add columns TestID, WashingSolutionR1, WashingSolutionR2</remarks>
        Public Function UpdateMAXRotorTurnRecord(ByVal pDBConnection As SqlConnection, ByVal pReactionRotorDS As ReactionsRotorDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                '' XBC 03/07/2012 - time estimation
                'Dim StartTime As DateTime = Now

                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pReactionRotorDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    Dim STR_NULL As String = " NULL, "

                    For Each reactionRotorRow As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionRotorDS.twksWSReactionsRotor
                        cmdText &= " UPDATE twksWSReactionsRotor "
                        cmdText &= " SET "

                        If Not reactionRotorRow.IsWellContentNull Then
                            cmdText &= " WellContent = " & String.Format(" '{0}',", reactionRotorRow.WellContent)
                        Else
                            'cmdText &= " WellContent = " & STR_NULL
                        End If

                        If Not reactionRotorRow.IsWellStatusNull Then
                            cmdText &= " WellStatus = " & String.Format("'{0}',", reactionRotorRow.WellStatus)
                        Else
                            'cmdText &= " WellStatus = " & STR_NULL
                        End If

                        If Not reactionRotorRow.IsRejectedFlagNull Then
                            cmdText &= " RejectedFlag = " & IIf(reactionRotorRow.RejectedFlag, 1, 0).ToString & ", "
                        End If

                        If Not reactionRotorRow.IsExecutionIDNull Then
                            cmdText &= " ExecutionID = " & reactionRotorRow.ExecutionID
                        Else
                            cmdText &= " ExecutionID = NULL "
                        End If

                        'AG 06/06/2011
                        'If Not reactionRotorRow.IsCurrentTurnFlagNull Then
                        '    cmdText &= " , CurrentTurnFlag = " & CStr(IIf(reactionRotorRow.CurrentTurnFlag, 1, 0))
                        'End If

                        If Not reactionRotorRow.IsWashedFlagNull Then
                            cmdText &= " , WashedFlag = " & CStr(IIf(reactionRotorRow.WashedFlag, 1, 0))
                        End If

                        If Not reactionRotorRow.IsWashRequiredFlagNull Then
                            cmdText &= " , WashRequiredFlag = " & CStr(IIf(reactionRotorRow.WashRequiredFlag, 1, 0))
                        End If
                        'AG 06/06/2011

                        'AG 07/12/2011
                        If Not reactionRotorRow.IsTestIDNull Then
                            cmdText &= " , TestID = " & String.Format("'{0}'", reactionRotorRow.TestID)
                        End If

                        If Not reactionRotorRow.IsWashingSolutionR1Null Then
                            cmdText &= " , WashingSolutionR1 = " & String.Format("'{0}'", reactionRotorRow.WashingSolutionR1)
                        End If

                        If Not reactionRotorRow.IsWashingSolutionR2Null Then
                            cmdText &= " , WashingSolutionR2 = " & String.Format("'{0}'", reactionRotorRow.WashingSolutionR2)
                        End If
                        'AG 07/12/2011


                        cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", reactionRotorRow.AnalyzerID) 'Required
                        cmdText &= " AND WellNumber = " & reactionRotorRow.WellNumber 'Required

                        'AG 06/06/2011
                        'cmdText &= " AND RotorTurn IN ( SELECT MAX(RotorTurn) FROM twksWSReactionsRotor "
                        'cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", reactionRotorRow.AnalyzerID) 'Required
                        'cmdText &= " AND WellNumber = " & reactionRotorRow.WellNumber & " )" 'Required

                        cmdText &= " AND CurrentTurnFlag = 1"
                        'AG 06/06/2011

                        cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    Next

                    If cmdText <> "" Then
                        dbCmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End If
                End If


                '' XBC 03/07/2012 - time estimation
                ''Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity("Update Reactions Rotor " & _
                '                                Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "twksWSReactionsRotorDAO.UpdateMAXRotorTurnRecord", EventLogEntryType.Information, False)


            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.UpdateMAXRotorTurnRecord", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWellNumber"></param>
        ''' <returns></returns>
        ''' <remarks>AG 07/06/2011</remarks>
        Public Function SetCurrentTurnFlagToFalse(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWellNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " UPDATE twksWSReactionsRotor SET CurrentTurnFlag = 0 " & vbCrLf
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) & vbCrLf
                    cmdText &= " AND WellNumber = " & pWellNumber & vbCrLf
                    cmdText &= " AND CurrentTurnFlag = 1 "

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.SetCurrentTurnFlagToFalse", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' During Running all wells inside the washing station are mark with WellContent = 'W' (wash)
        ''' 13/12/2011 - And also set the ExecutionID to NULL
        ''' NOTE the WellStatus can NOT BE set to 'R' because this column value is set when receive the ANSPHR read
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWellList"></param>
        ''' <returns></returns>
        ''' <remarks>AG 07/06/2011 - created
        ''' AG 05/07/2011 - also update currentturnflag
        ''' AG 13/12/2011 - also set ExecutionID NULL</remarks>
        Public Function SetValuesForWashingStationWells(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWellList As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " UPDATE twksWSReactionsRotor SET WellContent = 'W', ExecutionID = NULL " & vbCrLf
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) & vbCrLf
                    cmdText &= " AND WellContent <> 'W' " & vbCrLf
                    cmdText &= " AND WellNumber IN (" & pWellList & ")"


                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.SetValuesForWashingStationWells", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Set WellContent = E in all well whose WellContent is W
        ''' Set WellStatus = R where CurrentTurnFlag = True and WellStatus different than R or X
        ''' (this method is used when analyzer leaves Running and enter in StandBy due in this state the wash station doesnt work)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 29/06/2011 - Creation
        ''' AG 13/12/2011 - update also wellstatus </remarks>
        Public Function SetToEmptyTheWellsInWashStation(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " UPDATE twksWSReactionsRotor SET WellContent = 'E' " & vbCrLf
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) & vbCrLf 'Required
                    cmdText &= " AND WellContent = 'W' "

                    cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    'AG 13/12/2011
                    cmdText &= " UPDATE twksWSReactionsRotor SET WellStatus = 'R' " & vbCrLf
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) & vbCrLf 'Required
                    cmdText &= " AND WellStatus <> 'R' AND WellStatus <> 'X' " & vbCrLf
                    cmdText &= " AND CurrentTurnFlag = 1 "
                    'AG 13/12/2011

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.SetToEmptyTheWellsInWashStation", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pRingNumber"></param>
        ''' <param name="pCellNumber"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by: XB 14/10/2013 - Correction on the query bug # 1326
        ''' </remarks>
        Public Function ReadReactionsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                     ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'AG 14/12/2011
                        'cmdText &= " SELECT DISTINCT RR.WellNumber, RR.ExecutionID, EX.SampleClass, EX.ReplicateNumber, EX.RerunNumber, EX.ExecutionStatus, " & vbCrLf
                        'cmdText &= " EX.OrderTestID,  EX.MultiItemNumber, OT.SampleType, TE.TestName, " & vbCrLf
                        'cmdText &= " (CASE OD.SampleID  WHEN NULL THEN OD.PatientID  ELSE OD.SampleID  END) As SampleID, RE.PredilutionFactor As DilutionFactor " & vbCrLf
                        'cmdText &= " FROM twksWSReactionsRotor RR INNER JOIN twksWSExecutions EX ON RR.ExecutionID = EX.ExecutionID " & vbCrLf
                        'cmdText &= " INNER JOIN twksOrderTests OT ON EX.OrderTestID = OT.OrderTestID " & vbCrLf
                        'cmdText &= " INNER JOIN tparTests TE ON OT.TestID = TE.TestID " & vbCrLf
                        'cmdText &= " INNER JOIN twksOrders OD ON OT.OrderID  = OD.OrderID " & vbCrLf
                        'cmdText &= " INNER JOIN twksWSRequiredElemByOrderTest REOT ON EX.OrderTestID = REOT.OrderTestID " & vbCrLf
                        'cmdText &= " LEFT OUTER JOIN twksWSRequiredElements RE ON (REOT.ElementID = RE.ElementID AND RE.TubeContent = 'PATIENT' AND RE.OnlyForISE = 0) " & vbCrLf
                        'cmdText &= " WHERE RR.AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                        'cmdText &= " AND RR.RotorTurn = " & pRingNumber.ToString & vbCrLf
                        'cmdText &= " AND RR.WellNumber = " & pCellNumber.ToString

                        cmdText &= " SELECT DISTINCT RR.WellNumber, RR.ExecutionID, EX.SampleClass, EX.ReplicateNumber, EX.RerunNumber, EX.ExecutionStatus, " & vbCrLf
                        cmdText &= " EX.OrderTestID,  EX.MultiItemNumber, OT.SampleType, TE.TestName, " & vbCrLf
                        'cmdText &= " (CASE OD.SampleID  WHEN NULL THEN OD.PatientID  ELSE OD.SampleID  END) As SampleID, TS.PredilutionFactor As DilutionFactor " & vbCrLf
                        cmdText &= " (CASE WHEN OD.SampleID  IS NULL THEN OD.PatientID  ELSE OD.SampleID  END) As SampleID, TS.PredilutionFactor As DilutionFactor " & vbCrLf
                        cmdText &= " FROM twksWSReactionsRotor RR INNER JOIN twksWSExecutions EX ON RR.ExecutionID = EX.ExecutionID " & vbCrLf
                        cmdText &= " INNER JOIN twksOrderTests OT ON EX.OrderTestID = OT.OrderTestID " & vbCrLf
                        cmdText &= " INNER JOIN tparTests TE ON OT.TestID = TE.TestID " & vbCrLf
                        cmdText &= " INNER JOIN twksOrders OD ON OT.OrderID  = OD.OrderID " & vbCrLf
                        cmdText &= " INNER JOIN tparTestSamples TS ON OT.TestID  = TS.TestID AND OT.SampleType = TS.SampleType " & vbCrLf
                        cmdText &= " WHERE RR.AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf
                        cmdText &= " AND RR.RotorTurn = " & pRingNumber.ToString & vbCrLf
                        cmdText &= " AND RR.WellNumber = " & pCellNumber.ToString
                        'AG 14/12/2011

                        Dim myDataSet As New ReactionRotorDetailsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.ReactionsRotorDetails)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSReactionsRotorDAO.ReadReactionsDetails", EventLogEntryType.Error, False)

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
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = " UPDATE twksWSReactionsRotor " & vbCrLf & _
                                            " SET    AnalyzerID = '" & pAnalyzerIDNew.Trim & "' " & vbCrLf & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerIDOld.Trim & "' " & vbCrLf


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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSAnalyzersDAO.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region


    End Class

End Namespace

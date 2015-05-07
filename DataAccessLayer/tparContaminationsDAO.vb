Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient
Imports System.Text

Partial Public Class tparContaminationsDAO
      

#Region "CRUD Methods"
    ''' <summary>
    ''' Create a new Contamination
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminationRow">Row containing all data of the new Contamination to add</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 22/02/2010 - Changed the way of opening the DB Connection/Transaction; it was bad implemented
    '''              SA 22/07/2010 - Remove the validation of the number of records afected
    '''              SA 27/10/2010 - Added N preffix for multilanguage of field TS_User
    '''              AG 15/12/2010 - Added columns WashingSolutionR1 and WashingSolutionR2
    '''              SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminationRow As ContaminationsDS.tparContaminationsRow) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                cmdText = " INSERT INTO tparContaminations (ReagentContaminatorID, ReagentContaminatedID, TestContaminaCuvetteID, ContaminationType, " & vbCrLf & _
                                                          " WashingSolutionR1, WashingSolutionR2, TS_User, TS_DateTime) " & vbCrLf & _
                          " VALUES ("

                If (pContaminationRow.ContaminationType = "CUVETTES") Then
                    cmdText &= "NULL, NULL, " & pContaminationRow.TestContaminaCuvetteID.ToString & ", " & _
                                          "'" & pContaminationRow.ContaminationType.ToString() & "', "

                Else 'Contamination Type is R1 
                    cmdText &= pContaminationRow.ReagentContaminatorID.ToString() & ", " & _
                               pContaminationRow.ReagentContaminatedID.ToString() & ", " & _
                               "NULL, '" & pContaminationRow.ContaminationType.ToString() & "', "
                End If

                If (Not pContaminationRow.IsWashingSolutionR1Null) Then
                    cmdText &= " N'" & pContaminationRow.WashingSolutionR1.Replace("'", "''") & "', "
                Else
                    cmdText &= "NULL,"
                End If

                If (Not pContaminationRow.IsWashingSolutionR2Null) Then
                    cmdText &= " N'" & pContaminationRow.WashingSolutionR2.Replace("'", "''") & "', "
                Else
                    cmdText &= "NULL,"
                End If

                If (Not pContaminationRow.IsTS_UserNull) Then
                    cmdText &= " N'" & pContaminationRow.TS_User.Replace("'", "''") & "', "
                Else
                    'If TS_User is not informed, then get value of the Current User
                    'Dim myGlobalbase As New GlobalBase
                    cmdText &= " N'" & GlobalBase.GetSessionInfo().UserName().Replace("'", "''") & "', "
                End If

                If (Not pContaminationRow.IsTS_DateTimeNull) Then
                    cmdText &= " '" & pContaminationRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "')"
                Else
                    'If TS_DateTime is not informed, then the current Date and Time is used
                    cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "')"
                End If

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
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.Create", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all Contaminations between Reagents defined for the informed Reagent
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminatorReagentID">Identifier of the Contaminator Reagent</param>
    ''' <param name="pContaminationType">Contamination Type = R1</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 22/02/2010 - Changed the way of opening the DB Connection/Transaction; it was bad implemented.
    '''              SA 22/07/2010 - Removed the validation of the number of records afected
    '''              SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorReagentID As Integer, ByVal pContaminationType As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                cmdText = " DELETE tparContaminations " & _
                          " WHERE  ReagentContaminatorID = " & pContaminatorReagentID & _
                          " AND    ContaminationType = '" & pContaminationType & "'"

                Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using

                ''Execute the SQL Sentence
                'Dim dbCmd As New SqlCommand
                'dbCmd.Connection = pDBConnection
                'dbCmd.CommandText = cmdText

                'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                'resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.Delete", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all Well Contaminations defined for the informed Test 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminatorTestID">Identifier of the Contaminator Test</param>
    ''' <param name="pContaminationType">Contamination Type = CUVETTES</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  AG 15/12/2010 
    ''' Modified by: SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function DeleteCuvettes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorTestID As Integer, _
                                   ByVal pContaminationType As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                cmdText = " DELETE tparContaminations " & _
                          " WHERE  TestContaminaCuvetteID = " & pContaminatorTestID & _
                          " AND    ContaminationType = '" & pContaminationType & "'"

                Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using

                ''Execute the SQL Sentence
                'Dim dbCmd As New SqlCommand
                'dbCmd.Connection = pDBConnection
                'dbCmd.CommandText = cmdText

                'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                'resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteCuvettes", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Search all the Contaminations of the specified type
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminationType">Type of Contamination</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations of the specified type</returns>
    ''' <remarks>
    ''' Created by:  DL 10/02/2010
    ''' Modified by: SA 22/02/2010 - Parameter ContaminationType was changed from Integer to String
    '''              SA 24/10/2011 - Changed the function template
    ''' </remarks>
    Public Function ReadByContaminationType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminationType As String) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT * FROM   tparContaminations " & vbCrLf & _
                                            " WHERE  ContaminationType = '" & pContaminationType & "'"

                    Dim contaminationsDataDS As New ContaminationsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByContaminationType", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function


    Public Shared Function GetAllContaminationsForAReagent(ReagentID As Integer) As TypedGlobalDataTo(Of ContaminationsDS.tparContaminationsDataTable)
        'Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            Dim connection = GetSafeOpenDBConnection(Nothing)
            dbConnection = connection.SetDatos
            If dbConnection Is Nothing Then

                Return New TypedGlobalDataTo(Of ContaminationsDS.tparContaminationsDataTable) _
                    With {.SetDatos = Nothing, .HasError = True, .ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString}

            Else
                Dim cmdText As String = "SELECT * FROM [Ax00].[dbo].[tparContaminations] Where ReagentContaminatorID=" & ReagentID & ";"

                Dim contaminationsDataDS As New ContaminationsDS
                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                    End Using
                End Using
                Dim resultData As New TypedGlobalDataTo(Of ContaminationsDS.tparContaminationsDataTable)
                resultData.SetDatos = contaminationsDataDS.tparContaminations

                resultData.HasError = False
                Return resultData
            End If

        Catch ex As Exception
            Dim resultData = New TypedGlobalDataTo(Of ContaminationsDS.tparContaminationsDataTable)
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex)
            Return resultData
        Finally
            If dbConnection IsNot Nothing Then dbConnection.Close()
        End Try

    End Function


#End Region

#Region "Other Methods"
    ''' <summary>
    ''' Delete all Contaminations defined for the specified Test
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' Modified by: SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function DeleteAllByTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                resultData.HasError = True
            Else
                'AJG
                'Dim cmdText As String = " DELETE FROM tparContaminations " & vbCrLf & _
                '                        " WHERE  (ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                '                        " AND     ReagentContaminatorID IN (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                '                                                          " WHERE  TestID = " & pTestID & ")) " & vbCrLf & _
                '                        " OR     (ContaminationType = 'CUVETTES' " & vbCrLf & _
                '                        " AND     TestContaminaCuvetteID = " & pTestID & ") "

                Dim cmdText As String = " DELETE FROM tparContaminations " & vbCrLf & _
                                        " WHERE  (ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                                        " AND EXISTS (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                                                     " WHERE  TestID = " & pTestID & " AND tparContaminations.ReagentContaminatorID = ReagentID)) " & vbCrLf & _
                                        " OR     (ContaminationType = 'CUVETTES' " & vbCrLf & _
                                        " AND     TestContaminaCuvetteID = " & pTestID & ") "

                Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using

                ''Execute the SQL Sentence
                'Dim dbCmd As New SqlCommand
                'dbCmd.Connection = pDBConnection
                'dbCmd.CommandText = cmdText

                'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                'resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteAllByTest", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all Contaminations in which the specified Reagent acts as contaminator or contaminated
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentID">Reagent Identifier</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  TR 18/05/2010
    ''' Modified by: SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function DeleteByReagentContaminatorOrContaminated(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparContaminations " & vbCrLf & _
                                        " WHERE  ReagentContaminatorID = " & pReagentID & vbCrLf & _
                                        " OR     ReagentContaminatedID = " & pReagentID & vbCrLf

                Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using

                ''Execute the SQL Sentence
                'Dim dbCmd As New SqlCommand
                'dbCmd.Connection = pDBConnection
                'dbCmd.CommandText = cmdText

                'resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                'resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteByReagentContaminatorOrContaminated", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get the list of Washing Solutions needed in the active WorkSession (to add them as required Work Session Elements). Only Washing Solutions that fulfill
    ''' one or more of the following conditions are returned:
    ''' ** The Washing Solution is needed to avoid Contaminations between Reagents that are both needed in the active Work Session
    ''' ** The Washing Solution is needed to avoid Well Contamination caused by a Test requested in the active Work Session
    ''' ** The Washing Solution is needed to remove remaining Well Contaminations from the previous Work Session 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pWorkSessionID">Work Session Identifier</param>
    ''' <param name="pAnalyzerID">Analyzer Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with the list of Washing Solutions needed in the active WS</returns>
    ''' <remarks>
    ''' Created by:  SA 27/05/2014 - BT #1519
    ''' </remarks>
    Public Function GetWSContaminationsWithWASH(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    '(1) Subquery to get Washing Solutions needed to avoid Contaminations between Reagents of Tests requested in the Work Session
                    Dim cmdText As String = " SELECT WashingSolutionR1, NULL AS WashingSolutionR2 " & vbCrLf & _
                                            " FROM   tparContaminations C INNER JOIN twksWSRequiredElements RE1 ON C.ReagentContaminatorID = RE1.ReagentID " & vbCrLf & _
                                                                        " INNER JOIN twksWSRequiredElements RE2 ON C.ReagentContaminatedID = RE2.ReagentID " & vbCrLf & _
                                            " WHERE  C.ContaminationType = 'R1' " & vbCrLf & _
                                            " AND    C.WashingSolutionR1 IS NOT NULL " & vbCrLf & _
                                            " AND    RE1.WorkSessionID = '" & pWorkSessionID.Trim & "' AND RE1.TubeContent = 'REAGENT' " & vbCrLf & _
                                            " AND    RE2.WorkSessionID = '" & pWorkSessionID.Trim & "' AND RE2.TubeContent = 'REAGENT' " & vbCrLf

                    '(2) Subquery to get Washing Solutions to avoid Well Contaminations caused by Tests requested in the Work Session 
                    'AJG
                    'cmdText &= " UNION " & vbCrLf & _
                    '           " SELECT WashingSolutionR1, WashingSolutionR2 " & vbCrLf & _
                    '           " FROM   tparContaminations C INNER JOIN twksOrderTests OT ON C.TestContaminaCuvetteID = OT.TestID " & vbCrLf & _
                    '           " WHERE  C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                    '           " AND   (C.WashingSolutionR1 IS NOT NULL OR C.WashingSolutionR2 IS NOT NULL) " & vbCrLf & _
                    '           " AND    OT.TestType = 'STD' " & vbCrLf & _
                    '           " AND    OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                    '           " AND    OT.OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                    '                                     " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                    '                                     " AND    OpenOTFlag    = 0 " & vbCrLf & _
                    '                                     " AND    ToSendFlag    = 1) " & vbCrLf

                    cmdText &= " UNION " & vbCrLf & _
                               " SELECT WashingSolutionR1, WashingSolutionR2 " & vbCrLf & _
                               " FROM   tparContaminations C INNER JOIN twksOrderTests OT ON C.TestContaminaCuvetteID = OT.TestID " & vbCrLf & _
                               " WHERE  C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                               " AND   (C.WashingSolutionR1 IS NOT NULL OR C.WashingSolutionR2 IS NOT NULL) " & vbCrLf & _
                               " AND    OT.TestType = 'STD' " & vbCrLf & _
                               " AND    OT.OrderTestStatus = 'OPEN' " & vbCrLf & _
                               " AND    EXISTS (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                               " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                               " AND    OpenOTFlag    = 0 " & vbCrLf & _
                                               " AND    ToSendFlag    = 1 AND OT.OrderTestID = OrderTestID) " & vbCrLf

                    '(3) Subquery to get Washing Solution needed to clean Wells that remain contaminated from the previous Work Session
                    cmdText &= " UNION " & vbCrLf & _
                               " SELECT WashingSolutionR1, WashingSolutionR2 FROM twksWSReactionsRotor " & vbCrLf & _
                               " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                               " AND    CurrentTurnFlag = 1 " & vbCrLf & _
                               " AND    WashRequiredFlag = 1 " & vbCrLf & _
                               " AND    WashedFlag = 0 " & vbCrLf

                    Dim myContaminationsDS As New ContaminationsDS
                    Using dbCmd As New SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myContaminationsDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = myContaminationsDS
                    resultData.HasError = False
                End If
            End If

        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.GetWSContaminationsWithWASH", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Search all Contaminations in which the specified Reagent acts as contaminator or contaminated
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentID">Reagent Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations in which the
    '''          specified Reagent acts as contaminator or contaminated</returns>
    ''' <remarks>
    ''' Created by:  TR 18/05/2010
    ''' Modified by: SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function ReadAllContaminationsByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT ContaminationID, ReagentContaminatorID, ReagentContaminatedID, TestContaminaCuvetteID, ContaminationType " & vbCrLf & _
                                            " FROM   tparContaminations " & vbCrLf & _
                                            " WHERE  ReagentContaminatorID = " & pReagentID.ToString() & vbCrLf & _
                                            " OR     ReagentContaminatedID = " & pReagentID.ToString() & vbCrLf

                    Dim contaminationsDataDS As New ContaminationsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False

                    'Dim dbCmd As New SqlClient.SqlCommand
                    'dbCmd.Connection = dbConnection
                    'dbCmd.CommandText = cmdText

                    ''Fill the DataSet to return 
                    'Dim contaminationsDataDS As New ContaminationsDS
                    'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    'dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    'resultData.SetDatos = contaminationsDataDS
                    'resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadAllContaminationsByReagentID", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Verify if there is a R1 Contamination between the informed Tests
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminatorTestID">Identifier of the Contaminator Test</param>
    ''' <param name="pContaminatedTestID">Identifier of the Contaminated Test</param>
    ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS with the ReagentID of the Contaminated Test
    '''          when exist a contamination between it and the Reagent of the specified Contaminator Test</returns>
    ''' <remarks>
    ''' Created by:  SA 24/10/2011
    ''' </remarks>
    Public Function ReadContaminationsBetweenTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorTestID As Integer, _
                                                   ByVal pContaminatedTestID As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'AJG
                    'Dim cmdText As String = " SELECT * FROM tparTestReagents TR " & vbCrLf & _
                    '                        " WHERE  TR.TestID = " & pContaminatorTestID & vbCrLf & _
                    '                        " AND    TR.ReagentID IN (SELECT C.ReagentContaminatedID " & vbCrLf & _
                    '                                                " FROM   tparContaminations C INNER JOIN tparTestReagents TR2 ON C.ReagentContaminatorID = TR2.ReagentID " & vbCrLf & _
                    '                                                " WHERE  TR2.TestID = " & pContaminatedTestID & vbCrLf & _
                    '                                                " AND    C.ContaminationType = 'R1') " & vbCrLf

                    Dim cmdText As String = " SELECT * FROM tparTestReagents TR " & vbCrLf & _
                                            " WHERE  TR.TestID = " & pContaminatorTestID & vbCrLf & _
                                            " AND    EXISTS (SELECT C.ReagentContaminatedID " & vbCrLf & _
                                                            " FROM   tparContaminations C INNER JOIN tparTestReagents TR2 ON C.ReagentContaminatorID = TR2.ReagentID " & vbCrLf & _
                                                            " WHERE  TR2.TestID = " & pContaminatedTestID & vbCrLf & _
                                                            " AND    C.ContaminationType = 'R1' AND TR.ReagentID = C.ReagentContaminatedID) " & vbCrLf

                    Dim myTestReagentsDS As New TestReagentsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myTestReagentsDS.tparTestReagents)
                        End Using
                    End Using

                    resultData.SetDatos = myTestReagentsDS
                    resultData.HasError = False
                End If
            End If

        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadContaminationsBetweenTests", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get all defined Cuvette Contaminations
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsAuxCuDS with all the defined CuvetteContaminations</returns>
    ''' <remarks>
    ''' Created by:  DL 21/12/2010
    ''' Modified by: TR 24/10/2011 - Changed the query to load the multilanguage desciption instead the Code for Washing Solutions
    '''              SA 07/11/2011 - Changed all LEFT JOIN for INNER JOIN; changed the function template
    '''              TR 14/12/2011 - Change Query because previous was wrong and do not return all the Well Contaminations.
    ''' </remarks>
    Public Function ReadTestContaminatorsCuv(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    ''Dim myLocalBase As New GlobalBase
                    Dim cmdText As New StringBuilder
                    'Dim cmdText As String = " SELECT DISTINCT T.Testname as Contaminators, C.WashingSolutionR1 as Step1,  C.WashingSolutionR2 as Step2, " & vbCrLf & _
                    '                                        " MLR1.ResourceText AS Step1Desc, MLR2.ResourceText AS Step2Desc " & vbCrLf & _
                    '                        " FROM tparContaminations C INNER JOIN tparTests T ON C.TestContaminaCuvetteID  = T.TestID " & vbCrLf & _
                    '                                                  " INNER JOIN tfmwPreloadedMasterData PMD1 ON C.WashingSolutionR1 = PMD1.ItemID " & vbCrLf & _
                    '                                                  " INNER JOIN tfmwMultiLanguageResources MLR1 ON MLR1.ResourceID = PMD1.ResourceID " & vbCrLf & _
                    '                                                  " INNER JOIN tfmwPreloadedMasterData PMD2 ON C.WashingSolutionR1 = PMD2.ItemID " & vbCrLf & _
                    '                                                  " INNER JOIN tfmwMultiLanguageResources MLR2 ON MLR2.ResourceID = PMD2.ResourceID " & vbCrLf & _
                    '                        " WHERE C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                    '                        " AND   PMD1.SubTableID = '" & GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS.ToString & "' " & vbCrLf & _
                    '                        " AND   MLR1.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf & _
                    '                        " AND   PMD2.SubTableID = '" & GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS.ToString & "' " & vbCrLf & _
                    '                        " AND   MLR2.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' "
                    'TR 14/12/2011 
                    cmdText.Append(" SELECT DISTINCT T.Testname as Contaminators, C.WashingSolutionR1 as Step1,  C.WashingSolutionR2 as Step2 ")
                    cmdText.Append(" FROM tparContaminations C INNER JOIN tparTests T ON C.TestContaminaCuvetteID  = T.TestID ")
                    cmdText.Append(" WHERE C.ContaminationType = 'CUVETTES' ")
                    'TR 14/12/2011 -END.

                    Dim contaminationsDataDS As New ContaminationsAuxCuDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadTestContaminatorsCuv", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get all defined Reagent Contaminations 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsAuxR1DS with all the defined Reagent Contaminations</returns>
    ''' <remarks>
    ''' Created by:  DL 21/12/2010
    ''' Modified by: TR 24/10/2011 - Changed the query to load the multilanguage desciption instead the Code for Washing Solutions
    '''              SA 07/11/2011 - Changed the SQL, it does not work when WashingSolution is NULL; changed the function template
    ''' </remarks>
    Public Function ReadTestContaminatorsR1(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Dim myLocalBase As New GlobalBase
                    Dim cmdText As String = " SELECT T.TestName AS Contaminator, T1.TestName AS Contaminated, C.WashingSolutionR1 AS Wash, NULL AS WashDesc " & vbCrLf & _
                                            " FROM   tparContaminations C INNER JOIN tparTestReagents TR ON C.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                                        " INNER JOIN tparTests T ON TR.TestID = T.TestID " & vbCrLf & _
                                                                        " INNER JOIN tparTestReagents TR1 ON C.ReagentContaminatedID = TR1.ReagentID " & vbCrLf & _
                                                                        " INNER JOIN tparTests T1 ON TR1.TestID = T1.TestID " & vbCrLf & _
                                            " WHERE C.ContaminationType = 'R1' " & vbCrLf & _
                                            " AND   C.WashingSolutionR1 IS NULL " & vbCrLf & _
                                            " UNION ALL " & vbCrLf & _
                                            " SELECT T.TestName AS Contaminator, T1.TestName AS Contaminated, C.WashingSolutionR1 AS Wash, MLR.ResourceText AS WashDesc " & vbCrLf & _
                                            " FROM   tparContaminations C INNER JOIN tparTestReagents TR ON C.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                                        " INNER JOIN tparTests T ON TR.TestID = T.TestID " & vbCrLf & _
                                                                        " INNER JOIN tparTestReagents TR1 ON C.ReagentContaminatedID = TR1.ReagentID " & vbCrLf & _
                                                                        " INNER JOIN tparTests T1 ON TR1.TestID = T1.TestID " & vbCrLf & _
                                                                        " INNER JOIN tfmwPreloadedMasterData PMD ON C.WashingSolutionR1 = PMD.ItemID " & vbCrLf & _
                                                                        " INNER JOIN tfmwMultiLanguageResources MLR ON MLR.ResourceID =  PMD.ResourceID " & vbCrLf & _
                                            " WHERE C.ContaminationType = 'R1' " & vbCrLf & _
                                            " AND   C.WashingSolutionR1 IS NOT NULL " & vbCrLf & _
                                            " AND   PMD.SubTableID = '" & GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS.ToString & "' " & vbCrLf & _
                                            " AND   MLR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf & _
                                            " ORDER BY Contaminator, Contaminated "

                    Dim contaminationsDataDS As New ContaminationsAuxR1DS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadTestContaminatorsR1", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get all Contaminations (of all types) defined for an specific Standard Tests 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations (all types) defined
    '''          for the specified Standard Test</returns>
    ''' <remarks>
    ''' Created by:  SA 29/11/2010
    ''' Modified by: AG 15/12/2010 - Do not use tparContaminationWashings table
    '''              SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function ReadTestRNAsContaminator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Dim cmdText As String = " SELECT C.ContaminationID, C.ContaminationType, C.ReagentContaminatorID, C.ReagentContaminatedID, " & vbCrLf & _
                    '                               " C.TestContaminaCuvetteID, C.WashingSolutionR1, C.WashingSolutionR2 " & vbCrLf & _
                    '                        " FROM   tparContaminations C " & vbCrLf & _
                    '                        " WHERE  (C.ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                    '                        " AND     C.ReagentContaminatorID IN (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                    '                                                            " WHERE  TestID = " & pTestID & ")) " & vbCrLf & _
                    '                        " OR     (C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                    '                        " AND     C.TestContaminaCuvetteID = " & pTestID & ") " & vbCrLf & _
                    '                        " ORDER BY C.ContaminationType "

                    Dim cmdText As String = " SELECT C.ContaminationID, C.ContaminationType, C.ReagentContaminatorID, C.ReagentContaminatedID, " & vbCrLf & _
                                                   " C.TestContaminaCuvetteID, C.WashingSolutionR1, C.WashingSolutionR2 " & vbCrLf & _
                                            " FROM   tparContaminations C " & vbCrLf & _
                                            " WHERE  (C.ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                                            " AND    EXISTS (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                                                            " WHERE  TestID = " & pTestID & " AND C.ReagentContaminatorID = ReagentID)) " & vbCrLf & _
                                            " OR     (C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                                            " AND     C.TestContaminaCuvetteID = " & pTestID & ") " & vbCrLf & _
                                            " ORDER BY C.ContaminationType "

                    Dim contaminationsDataDS As New ContaminationsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False

                    'Dim dbCmd As New SqlClient.SqlCommand
                    'dbCmd.Connection = dbConnection
                    'dbCmd.CommandText = cmdText

                    ''Fill the DataSet to return 
                    'Dim contaminationsDataDS As New ContaminationsDS
                    'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    'dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    'resultData.SetDatos = contaminationsDataDS
                    'resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadTestRNAsContaminator", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' NOTE: THIS FUNCTION IS EQUAL TO FUNCTION ReadTestRNAsContaminator!!
    ''' Get all Contaminations (of all types) defined for an specific Standard Tests 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations (all types) defined
    '''          for the specified Standard Test</returns>
    ''' <remarks>
    ''' Created by:  TR 13/12/2011
    ''' Modified by: SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Dim cmdText As String = " SELECT * FROM tparContaminations " & vbCrLf & _
                    '                        " WHERE  (ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                    '                        " AND     ReagentContaminatorID IN (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                    '                                                          " WHERE  TestID = " & pTestID & ")) " & vbCrLf & _
                    '                        " OR     (ContaminationType = 'CUVETTES' " & vbCrLf & _
                    '                        " AND     TestContaminaCuvetteID = " & pTestID & ") "

                    Dim cmdText As String = " SELECT * FROM tparContaminations " & vbCrLf & _
                                            " WHERE  (ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                                            " AND    EXISTS (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                                                            " WHERE  TestID = " & pTestID & " AND tparContaminations.ReagentContaminatorID = ReagentID)) " & vbCrLf & _
                                            " OR     (ContaminationType = 'CUVETTES' " & vbCrLf & _
                                            " AND     TestContaminaCuvetteID = " & pTestID & ") "

                    Dim contaminationsDataDS As New ContaminationsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False

                    'Dim dbCmd As New SqlClient.SqlCommand
                    'dbCmd.Connection = dbConnection
                    'dbCmd.CommandText = cmdText

                    ''Fill the DataSet to return 
                    'Dim contaminationsDataDS As New ContaminationsDS
                    'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    'dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    'resultData.SetDatos = contaminationsDataDS
                    'resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByTestID", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get Contaminantions between Reagents searching by the Contaminator Reagent Name and the Contaminated Reagent Name
    ''' NOTE: this function is used ONLY for the process of update preloaded data
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminatorReagtName">Contaminator Reagent Name</param>
    ''' <param name="pContaminatedReagName">Contaminated Reagent Name</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with data of all Contaminations between Reagents defined for the 
    '''          specified Reagents (Contaminator and Contaminated)</returns>
    ''' <remarks>
    ''' Created by:  TR 11/02/2013
    ''' Modified by: AG 13/03/2014 - BT #1538 ==> Changed the query to fix issue when Name contains char ' (use .Replace("'", "''"))
    '''              SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function ReadByReagentsName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorReagtName As String, _
                                       ByVal pContaminatedReagName As String) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT C.*, R.PreloadedReagent, R.ReagentName AS ContaminatorName, R1.ReagentName AS ContaminatedName " & vbCrLf & _
                                            " FROM Ax00.dbo.tparContaminations C, Ax00.dbo.tparReagents R, Ax00TEM.dbo.tparReagents R1 " & vbCrLf & _
                                            " WHERE R.ReagentName = N'" & pContaminatorReagtName.Replace("'", "''") & "'" & vbCrLf & _
                                            " AND R1.ReagentName  = N'" & pContaminatedReagName.Replace("'", "''") & "'" & vbCrLf & _
                                            " AND R.ReagentID     = C.ReagentContaminatorID " & vbCrLf & _
                                            " AND R1.ReagentID    = C.ReagentContaminatedID " & vbCrLf

                    Dim contaminationsDataDS As New ContaminationsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False

                    'Dim dbCmd As New SqlClient.SqlCommand
                    'dbCmd.Connection = dbConnection
                    'dbCmd.CommandText = cmdText

                    ''Fill the DataSet to return 
                    'Dim contaminationsDataDS As New ContaminationsDS
                    'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    'dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    'resultData.SetDatos = contaminationsDataDS
                    'resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByReagentsName", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get Well Contaminations searching by the TestName 
    ''' NOTE: this function is used ONLY for the process of update preloaded data
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestName">Test Name</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with data of the Well Contamination defined for the 
    '''          specified Test</returns>
    ''' <remarks>
    ''' Created by:  TR 11/02/2013
    ''' Modified by: AG 13/03/2014 - BT #1538 ==> Changed the query to fix issue when Name contains char ' (use .Replace("'", "''"))
    '''              SA 27/05/2014 - Added USING sentence to manage the SQLCommand
    ''' </remarks>
    Public Function ReadByTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT C.*, T.TestID, T.TestName " & vbCrLf & _
                                            " FROM   Ax00.dbo.tparContaminations C INNER JOIN tparTests T ON T.TestID = C.TestContaminaCuvetteID " & vbCrLf & _
                                            " WHERE T.TestName = N'" & pTestName.Replace("'", "''") & "'" & vbCrLf

                    Dim contaminationsDataDS As New ContaminationsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)
                        End Using
                    End Using

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False

                    'Dim dbCmd As New SqlClient.SqlCommand
                    'dbCmd.Connection = dbConnection
                    'dbCmd.CommandText = cmdText

                    ''Fill the DataSet to return 
                    'Dim contaminationsDataDS As New ContaminationsDS
                    'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    'dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    'resultData.SetDatos = contaminationsDataDS
                    'resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByTestName", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Returns Contaminations info to show in a Report
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="AppLang">Application Language</param>
    ''' <remarks>
    ''' Created by: RH 21/12/2011
    '''             Returns the same info as ReadTestContaminatorsR1() + ReadTestContaminatorsCuv()
    '''             but merged in the same Typed Dataset so, if any of the queries of these methods change,
    '''             the below queries should be updated.
    ''' </remarks>
    Public Function GetContaminationsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal AppLang As String) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = String.Format( _
                       " SELECT T.TestName AS Contaminator, T1.TestName AS Contaminated, C.WashingSolutionR1 AS Wash, NULL AS WashDesc" & _
                       " FROM   tparContaminations C INNER JOIN tparTestReagents TR ON C.ReagentContaminatorID = TR.ReagentID" & _
                       " INNER JOIN tparTests T ON TR.TestID = T.TestID" & _
                       " INNER JOIN tparTestReagents TR1 ON C.ReagentContaminatedID = TR1.ReagentID" & _
                       " INNER JOIN tparTests T1 ON TR1.TestID = T1.TestID" & _
                       " WHERE C.ContaminationType = 'R1'" & _
                       " AND   C.WashingSolutionR1 IS NULL" & _
                       " UNION ALL" & _
                       " SELECT T.TestName AS Contaminator, T1.TestName AS Contaminated, C.WashingSolutionR1 AS Wash, MLR.ResourceText AS WashDesc" & _
                       " FROM   tparContaminations C INNER JOIN tparTestReagents TR ON C.ReagentContaminatorID = TR.ReagentID" & _
                       " INNER JOIN tparTests T ON TR.TestID = T.TestID" & _
                       " INNER JOIN tparTestReagents TR1 ON C.ReagentContaminatedID = TR1.ReagentID" & _
                       " INNER JOIN tparTests T1 ON TR1.TestID = T1.TestID" & _
                       " INNER JOIN tfmwPreloadedMasterData PMD ON C.WashingSolutionR1 = PMD.ItemID" & _
                       " INNER JOIN tfmwMultiLanguageResources MLR ON MLR.ResourceID =  PMD.ResourceID" & _
                       " WHERE C.ContaminationType = 'R1'" & _
                       " AND   C.WashingSolutionR1 IS NOT NULL" & _
                       " AND   PMD.SubTableID = '{0}'" & _
                       " AND   MLR.LanguageID = '{1}'" & _
                       " ORDER BY Contaminator, Contaminated", GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, AppLang)

                    Dim myContaminationsDS As New ContaminationsDS

                    'Get first table
                    Using dbCmd As New SqlCommand(cmdText, dbConnection)
                        'Fill the DataSet to return 
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myContaminationsDS.ReagentsContaminations)
                        End Using
                    End Using

                    cmdText = String.Format( _
                       " SELECT DISTINCT T.Testname as Contaminators, MLR1.ResourceText AS Step1, MLR2.ResourceText AS Step2" & _
                       " FROM tparContaminations C INNER JOIN tparTests T ON C.TestContaminaCuvetteID  = T.TestID" & _
                       " INNER JOIN tfmwPreloadedMasterData PMD1 ON C.WashingSolutionR1 = PMD1.ItemID" & _
                       " INNER JOIN tfmwMultiLanguageResources MLR1 ON MLR1.ResourceID = PMD1.ResourceID" & _
                       " INNER JOIN tfmwPreloadedMasterData PMD2 ON C.WashingSolutionR2 = PMD2.ItemID" & _
                       " INNER JOIN tfmwMultiLanguageResources MLR2 ON MLR2.ResourceID = PMD2.ResourceID" & _
                       " WHERE C.ContaminationType = 'CUVETTES'" & _
                       " AND PMD1.SubTableID = '{0}'" & _
                       " AND MLR1.LanguageID = '{1}'" & _
                       " AND PMD2.SubTableID = '{0}'" & _
                       " AND MLR2.LanguageID = '{1}'", GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS, AppLang)

                    'Get second table
                    Using dbCmd As New SqlCommand(cmdText, dbConnection)
                        'Fill the DataSet to return 
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myContaminationsDS.CuvettesContaminations)
                            resultData.SetDatos = myContaminationsDS
                        End Using
                    End Using

                End If
            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.GetContaminationsForReport", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Returns the number of Well Contaminations that can happens in the active WorkSession and in which the informed Washing Solution is used 
    ''' in the second Step
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pWorkSessionID">Work Session Identifier</param>
    ''' <param name="pWashingSolution">Code of the Washing Solution used in the second step of Well Contaminations</param>
    ''' <returns>GlobalDataTO containing an Integer value with the number of Well Contaminations that can happens in the active WorkSession and
    '''          in which the informed Washing Solution is used in the second Step</returns>
    ''' <remarks>
    ''' Created by:  JV 08/11/2013 - BT #1358
    ''' </remarks>
    Public Function VerifyWashingSolutionR2(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pWashingSolution As String) As GlobalDataTO
        Dim myGlobalDataTO As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'AJG
                    'Dim cmdText As String = "SELECT COUNT(*) AS NumContaminations " & vbCrLf & _
                    '                        "FROM   tparContaminations " & vbCrLf & _
                    '                        "WHERE  ContaminationType = 'CUVETTES' " & vbCrLf & _
                    '                        "AND    WashingSolutionR2 = '" & pWashingSolution & "' " & vbCrLf & _
                    '                        "AND    TestContaminaCuvetteID IN (SELECT DISTINCT TestID FROM vwksWSOrderTests " & vbCrLf & _
                    '                                                         " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                    '                                                         " AND    TestType = 'STD' " & vbCrLf & _
                    '                                                         " AND    OpenOTFlag = 0) " & vbCrLf

                    Dim cmdText As String = "SELECT COUNT(*) AS NumContaminations " & vbCrLf & _
                                            "FROM   tparContaminations " & vbCrLf & _
                                            "WHERE  ContaminationType = 'CUVETTES' " & vbCrLf & _
                                            "AND    WashingSolutionR2 = '" & pWashingSolution & "' " & vbCrLf & _
                                            "AND    EXISTS (SELECT TestID FROM vwksWSOrderTests " & vbCrLf & _
                                                           " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                           " AND    TestType = 'STD' " & vbCrLf & _
                                                           " AND    OpenOTFlag = 0 AND tparContaminations.TestContaminaCuvetteID = TestID) " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            End If
        Catch ex As Exception
            myGlobalDataTO = New GlobalDataTO()
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.VerifyWashingSolutionR2", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return myGlobalDataTO
    End Function

#End Region

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS (NEW AND UPDATED FUNCTIONS)"
    ''' <summary>
    ''' Delete all R1 Contaminations where both Reagents, the Contaminator and the Contaminated, are used for preloaded 
    ''' Standard Tests --> Moved from CommonTasks section of TaskList.xml to make the Update Version process more clear  
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  SA 20/10/2014 - BA-1944 (SubTask BA-1986)
    ''' </remarks>
    Public Function DeleteAllPreloadedR1Contaminations(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                'AJG
                'Dim cmdText As String = " DELETE FROM tparContaminations " & vbCrLf & _
                '                        " WHERE  ContaminationType = 'R1' " & vbCrLf & _
                '                        " AND    ReagentContaminatorID IN (SELECT ReagentID FROM tparReagents WHERE PreloadedReagent = 1) " & vbCrLf & _
                '                        " AND    ReagentContaminatedID IN (SELECT ReagentID FROM tparReagents WHERE PreloadedReagent = 1) " & vbCrLf

                Dim cmdText As String = " DELETE FROM tparContaminations " & vbCrLf & _
                                        " WHERE  ContaminationType = 'R1' " & vbCrLf & _
                                        " AND    EXISTS (SELECT ReagentID FROM tparReagents WHERE PreloadedReagent = 1 AND tparContaminations.ReagentContaminatorID = ReagentID) " & vbCrLf & _
                                        " AND    EXISTS (SELECT ReagentID FROM tparReagents WHERE PreloadedReagent = 1 AND tparContaminations.ReagentContaminatedID = ReagentID) " & vbCrLf

                'Execute the SQL Sentence
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
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteAllPreloadedR1Contaminations", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all CUVETTES Contaminations for preloaded Standard Tests --> Moved from CommonTasks section of TaskList.xml to 
    ''' make the Update Version process more clear 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  SA 20/10/2014 - BA-1944 (SubTask BA-1986)
    ''' </remarks>
    Public Function DeleteAllPreloadedCuvettesContaminations(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                'AJG
                'Dim cmdText As String = " DELETE FROM tparContaminations " & vbCrLf & _
                '                        " WHERE  ContaminationType = 'CUVETTES' " & vbCrLf & _
                '                        " AND    TestContaminaCuvetteID IN (SELECT TestID FROM tparTests WHERE PreloadedTest = 1) " & vbCrLf

                Dim cmdText As String = " DELETE FROM tparContaminations " & vbCrLf & _
                                        " WHERE  ContaminationType = 'CUVETTES' " & vbCrLf & _
                                        " AND    EXISTS (SELECT TestID FROM tparTests WHERE PreloadedTest = 1 AND tparContaminations.TestContaminaCuvetteID = TestID) " & vbCrLf

                'Execute the SQL Sentence
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
            GlobalBase.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteAllPreloadedCuvettesContaminations", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function
#End Region

End Class
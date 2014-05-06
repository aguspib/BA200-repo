Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient
Imports System.Text

Partial Public Class tparContaminationsDAO
    Inherits DAOBase

#Region "CRUD Methods"

    ''' <summary>
    ''' Add a new record in table tparContaminations
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminationRow">Row of typed DataSet ContaminationsDS containing the Contamination 
    '''                                 information to add</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 22/02/2010 - Changed the way of opening the DB Connection/Transaction; it was bad implemented
    '''              SA 22/07/2010 - Remove the validation of the number of records afected
    '''              SA 27/10/2010 - Added N preffix for multilanguage of field TS_User
    '''              AG 15/12/2010 - Added columns WashingSolutionR1 and WashingSolutionR2
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

                Else 'Contamination Type is R1 or R2
                    cmdText &= pContaminationRow.ReagentContaminatorID.ToString() & ", " & _
                               pContaminationRow.ReagentContaminatedID.ToString() & ", " & _
                               "NULL, '" & pContaminationRow.ContaminationType.ToString() & "', "
                End If

                If Not pContaminationRow.IsWashingSolutionR1Null Then
                    cmdText &= " N'" & pContaminationRow.WashingSolutionR1.Replace("'", "''") & "', "
                Else
                    cmdText &= "NULL,"
                End If

                If Not pContaminationRow.IsWashingSolutionR2Null Then
                    cmdText &= " N'" & pContaminationRow.WashingSolutionR2.Replace("'", "''") & "', "
                Else
                    cmdText &= "NULL,"
                End If

                If (Not pContaminationRow.IsTS_UserNull) Then
                    cmdText &= " N'" & pContaminationRow.TS_User.Replace("'", "''") & "', "
                Else
                    'If TS_User is not informed, then get value of the Current User
                    Dim myGlobalBase As New GlobalBase
                    cmdText &= " N'" & myGlobalBase.GetSessionInfo().UserName().Replace("'", "''") & "', "
                End If

                If (Not pContaminationRow.IsTS_DateTimeNull) Then
                    cmdText &= " '" & pContaminationRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "')"
                Else
                    'If TS_DateTime is not informed, then the current Date and Time is used
                    cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "')"
                End If

                'Execute the SQL Sentence
                Dim dbCmd As New SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.Create", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all contaminations of the specified type that are led by the informed Reagent
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminatorReagentID">Identifier of the Contaminator Reagent to delete</param>
    ''' <param name="pContaminationType">Type of Contamination to delete for the informed Contaminator Reagent</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 22/02/2010 - Changed the way of opening the DB Connection/Transaction;
    '''                              it was bad implemented.
    '''              SA 22/07/2010 - Removed the validation of the number of records afected
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorReagentID As Integer, _
                           ByVal pContaminationType As String) As GlobalDataTO
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

                'Execute the SQL Sentence
                Dim dbCmd As New SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.Delete", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all cuvette contaminations of the specified type that are led by the informed Test
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminatorTestID">Identifier of the Contaminator Test to delete</param>
    ''' <param name="pContaminationType">Type of Contamination to delete for the informed Contaminator Test</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  AG 15/12/2010 (Based on Delete)
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

                'Execute the SQL Sentence
                Dim dbCmd As New SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteCuvettes", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get all Contaminations currently defined (both types R1 and CUVETTES)
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all Contaminations (R1 and CUVETTES)
    '''          currently defined</returns>
    ''' <remarks>
    ''' Created by:  TR 30/03/2011
    ''' Modified by: SA 24/10/2011 - Changed the function template
    ''' </remarks>
    Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT * FROM tparContaminations "

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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadAll", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByContaminationType", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Search if there is an R1 Contaminamination between the Reagents informed as Contaminator and Contaminated
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentContaminatorID">Identifier of the Contaminator Reagent</param>
    ''' <param name="pReagentContaminatedID">Identifier of the Contaminated Reagent</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with the Contamination between the informed Reagents</returns>
    ''' <remarks>
    ''' Created by:  TR 30/03/2011
    ''' Modified by: SA 24/10/2011 - Changed the function template
    ''' </remarks>
    Public Function ReadByContaminatorIDContaminatedID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentContaminatorID As Integer, _
                                                       ByVal pReagentContaminatedID As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT * FROM   tparContaminations " & vbCrLf & _
                                            " WHERE  ReagentContaminatorID = " & pReagentContaminatorID & vbCrLf & _
                                            " AND    ReagentContaminatedID = " & pReagentContaminatedID

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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByContaminatorIDContaminatedID", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function
#End Region

#Region "Other Methods"
    ''' <summary>
    ''' Search all Contaminations in which the specified Reagent acts as contaminator or contaminated
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentID">Reagent Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations in which the
    '''          specified Reagent acts as contaminator or contaminated</returns>
    ''' <remarks>
    ''' Created by:  TR 18/05/2010
    ''' </remarks>
    Public Function ReadAllContaminationsByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText = " SELECT ContaminationID, ReagentContaminatorID, ReagentContaminatedID, TestContaminaCuvetteID, ContaminationType "
                    cmdText &= " FROM   tparContaminations "
                    cmdText &= " WHERE  ReagentContaminatorID = " & pReagentID.ToString()
                    cmdText &= " OR     ReagentContaminatedID = " & pReagentID.ToString()


                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim contaminationsDataDS As New ContaminationsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadAllContaminationsByReagentID", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
    ''' </remarks>
    Public Function DeleteByReagentContaminatorOrContaminated(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                cmdText = " DELETE tparContaminations " & _
                          " WHERE  ReagentContaminatorID = " & pReagentID & _
                          " OR     ReagentContaminatedID = " & pReagentID

                'Execute the SQL Sentence
                Dim dbCmd As New SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteByReagentContaminatorOrContaminated", EventLogEntryType.Error, False)
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
                    Dim myLocalBase As New GlobalBase
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
                                            " AND   MLR.LanguageID = '" & myLocalBase.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf & _
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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadTestContaminatorsR1", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
    '''              TR 14/12/2011 - Change Query because previous was wrong and do not returt all the cuvettes contaminations.
    ''' </remarks>
    Public Function ReadTestContaminatorsCuv(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim myLocalBase As New GlobalBase
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
                    '                        " AND   MLR1.LanguageID = '" & myLocalBase.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf & _
                    '                        " AND   PMD2.SubTableID = '" & GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS.ToString & "' " & vbCrLf & _
                    '                        " AND   MLR2.LanguageID = '" & myLocalBase.GetSessionInfo.ApplicationLanguage & "' "
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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadTestContaminatorsCuv", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
    ''' </remarks>
    Public Function ReadTestRNAsContaminator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT C.ContaminationID, C.ContaminationType, C.ReagentContaminatorID, C.ReagentContaminatedID, " & vbCrLf & _
                                                   " C.TestContaminaCuvetteID, C.WashingSolutionR1, C.WashingSolutionR2 " & vbCrLf & _
                                            " FROM   tparContaminations C " & vbCrLf & _
                                            " WHERE  (C.ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                                            " AND     C.ReagentContaminatorID IN (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                                                                                " WHERE  TestID = " & pTestID & ")) " & vbCrLf & _
                                            " OR     (C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                                            " AND     C.TestContaminaCuvetteID = " & pTestID & ") " & vbCrLf & _
                                            " ORDER BY C.ContaminationType "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim contaminationsDataDS As New ContaminationsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadTestRNAsContaminator", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get all Standard Tests that contaminate the specified Standard Test
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all found Contaminations</returns>
    ''' <remarks>
    ''' Created by:  SA 29/11/2010
    ''' Modified by: AG 15/12/2010 - Do not use tparContaminationWashings table
    ''' </remarks>
    Public Function ReadTestRNAsContaminated(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT C.ContaminationType, T.TestID, T.TestName, T.PreloadedTest, C.ReagentContaminatorID, " & vbCrLf & _
                                                   " C.WashingSolutionR1, C.WashingSolutionR2 " & vbCrLf & _
                                            " FROM   tparContaminations C INNER JOIN tparTestReagents TR ON C.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                                        " INNER JOIN tparTests T ON TR.TestID = T.TestID " & vbCrLf & _
                                            " WHERE  C.ReagentContaminatedID IN (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                                                                               " WHERE  TestID = " & pTestID & ") " & vbCrLf & _
                                            " ORDER BY C.ContaminationType "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim contaminationsDataDS As New ContaminationsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadTestRNAsContaminated", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete all Contaminations defined for the specified Test
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  SA 01/12/2010
    ''' </remarks>
    Public Function DeleteAllByTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                resultData.HasError = True
            Else
                Dim cmdText As String = ""
                cmdText = " DELETE FROM tparContaminations " & vbCrLf & _
                          " WHERE  (ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                          " AND     ReagentContaminatorID IN (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                                                            " WHERE  TestID = " & pTestID & ")) " & vbCrLf & _
                          " OR     (ContaminationType = 'CUVETTES' " & vbCrLf & _
                          " AND     TestContaminaCuvetteID = " & pTestID & ") "

                'Execute the SQL Sentence
                Dim dbCmd As New SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteAllByTest", EventLogEntryType.Error, False)
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
                    Dim cmdText As String = " SELECT * FROM tparTestReagents TR " & vbCrLf & _
                                            " WHERE  TR.TestID = " & pContaminatorTestID & vbCrLf & _
                                            " AND    TR.ReagentID IN (SELECT C.ReagentContaminatedID " & vbCrLf & _
                                                                    " FROM   tparContaminations C INNER JOIN tparTestReagents TR2 ON C.ReagentContaminatorID = TR2.ReagentID " & vbCrLf & _
                                                                    " WHERE  TR2.TestID = " & pContaminatedTestID & vbCrLf & _
                                                                    " AND    C.ContaminationType = 'R1') " & vbCrLf

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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadContaminationsBetweenTests", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get Contaminations By the testid
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pTestID"></param>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 13/12/2011</remarks>
    Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText = " SELECT * FROM tparContaminations " & vbCrLf & _
                              " WHERE  (ContaminationType IN ('R1', 'R2') " & vbCrLf & _
                              " AND     ReagentContaminatorID IN (SELECT ReagentID FROM tparTestReagents " & vbCrLf & _
                                                                " WHERE  TestID = " & pTestID & ")) " & vbCrLf & _
                              " OR     (ContaminationType = 'CUVETTES' " & vbCrLf & _
                              " AND     TestContaminaCuvetteID = " & pTestID & ") "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim contaminationsDataDS As New ContaminationsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByTestID", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get the info by the contaminator Reagent Name and the contaminated reagent name.
    ''' this method is used for the update preloaded process (ONLY)
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pContaminatorReagtName">Contaminator Reagent Name.</param>
    ''' ''' <param name="pContaminatedReagName">Contaminated Reagent Name.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 11/02/2013
    ''' Modify AG 13/03/2014 - #1538 fix issue when name contains char ' (use .Replace("'", "''"))
    ''' </remarks>
    Public Function ReadByReagentsName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorReagtName As String, _
                                      pContaminatedReagName As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText &= " SELECT C.*, R.PreloadedReagent, R.ReagentName AS ContaminatorName, R1.ReagentName AS ContaminatedName " & vbCrLf
                    cmdText &= " FROM Ax00.dbo.tparContaminations C, Ax00.dbo.tparReagents R, Ax00TEM.dbo.tparReagents R1 " & vbCrLf
                    cmdText &= " WHERE R.ReagentName = N'" & pContaminatorReagtName.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " AND R1.ReagentName = N'" & pContaminatedReagName.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " AND R.ReagentID  = C.ReagentContaminatorID AND R1.ReagentID = C.ReagentContaminatedID " & vbCrLf

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim contaminationsDataDS As New ContaminationsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByReagentsName", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get the info by the TestName (TestContaminaCuvette)
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pTestName"></param>
    ''' <returns></returns>
    ''' <remarks>Modify AG 13/03/2014 - #1538 fix issue when name contains char ' (use .Replace("'", "''"))</remarks>
    Public Function ReadByTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = String.Empty
                    cmdText &= " SELECT C.*, T.TestID, T.TestName " & vbCrLf
                    cmdText &= " FROM         Ax00.dbo.tparContaminations C INNER JOIN " & vbCrLf
                    cmdText &= "              tparTests T ON T.TestID  = C.TestContaminaCuvetteID " & vbCrLf
                    cmdText &= " WHERE T.TestName = N'" & pTestName.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim contaminationsDataDS As New ContaminationsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByTestID", EventLogEntryType.Error, False)
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
    '''        
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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.GetContaminationsForReport", EventLogEntryType.Error, False)

        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

        End Try

        Return resultData
    End Function

    ''' <summary>
    ''' Returns the number of contaminations for a given pWashingSolution in a WS
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <param name="pWashingSolution"></param>
    ''' <returns></returns>
    ''' <remarks>JV #1358 08/11/2013</remarks>
    Public Function VerifyWashingSolutionR2(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pWashingSolution As String) As GlobalDataTO
        Dim myGlobalDataTO As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then

                    Dim cmdText As String = "SELECT COUNT(*) AS NumContaminations " & vbCrLf & _
                                            "FROM tparContaminations " & vbCrLf & _
                                            "WHERE  ContaminationType = 'CUVETTES' " & vbCrLf & _
                                            "AND    WashingSolutionR2 = '" & pWashingSolution & "' " & vbCrLf & _
                                            "AND    TestContaminaCuvetteID IN (SELECT DISTINCT TestID FROM vwksWSOrderTests " & vbCrLf & _
                                            "WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                            "AND    TestType = 'STD' AND    OpenOTFlag = 0)" & vbCrLf

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

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.VerifyWashingSolutionR2", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return myGlobalDataTO
    End Function

#End Region

#Region "TO REVIEW - DELETE - USED FOR THE PREVIOUS FORM OR NOT USED"
    ''' <summary>
    ''' USED FOR THE PREVIOUS CONTAMINATIONS FORM
    ''' Delete all contaminations of the specified type 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pContaminationType">Type of Contamination to delete</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 22/02/2010 - Changed the way of opening the DB Connection/Transaction; it was bad implemented.
    '''              TR 04/05/2010 - Removed the validation of the number of records afected
    ''' </remarks>
    Public Function DeleteByContaminationType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminationType As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                cmdText = " DELETE tparContaminations " & _
                          " WHERE  ContaminationType = '" & pContaminationType & "'"

                'Execute the SQL Sentence
                Dim dbCmd As New SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteByContaminationType", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' USED FOR THE PREVIOUS CONTAMINATIONS FORM
    ''' Search all the Contaminations of the informed type in which the specified Reagent acts as contaminator
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReagentContaminatorID"></param>
    ''' <param name="pContaminationType">Type of Contaminations</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations of the informed type 
    '''          in which the specified Reagent acts as contaminator</returns>
    ''' <remarks>
    ''' Created by:  DL 10/02/2010
    ''' Modified by: SA 22/02/2010 - Parameter ContaminationType was changed from Integer to String
    ''' </remarks>
    Public Function ReadByContaminatorIDAndType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentContaminatorID As Integer, _
                                                ByVal pContaminationType As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = ""
                    cmdText = " SELECT ContaminationID, ReagentContaminatorID, ReagentContaminatedID, TestContaminaCuvetteID, ContaminationType " & _
                              " FROM   tparContaminations " & _
                              " WHERE  ReagentContaminatorID = " & pReagentContaminatorID & _
                              " AND    ContaminationType = '" & pContaminationType & "'"

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim contaminationsDataDS As New ContaminationsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                    resultData.SetDatos = contaminationsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByContaminatorIDAndType", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    '''' <summary>
    '''' Delete the Contamination between the informed Reagents
    '''' </summary>
    '''' <param name="pDBConnection">Open DB Connection</param>
    '''' <param name="pContaminatorReagentID">Identifier of the Contaminator Reagent to delete</param>
    '''' <param name="pContaminatedReagentID">Identifier of the Contaminated Reagent to delete</param>
    '''' <returns>GlobalDataTO containing success/error information</returns>
    '''' <remarks>
    '''' Created by:  TR 30/03/2011
    '''' </remarks>
    'Public Function DeletebyContaminatorIDContamintedID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorReagentID As Integer, _
    '                                                                                      ByVal pContaminatedReagentID As Integer) As GlobalDataTO
    '    Dim resultData As New GlobalDataTO

    '    Try
    '        If (pDBConnection Is Nothing) Then
    '            resultData.HasError = True
    '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
    '        Else
    '            Dim cmdText As String = ""
    '            cmdText = " DELETE tparContaminations " & _
    '                      " WHERE  ReagentContaminatorID = " & pContaminatorReagentID & _
    '                      " AND    ReagentContaminatedID = '" & pContaminatedReagentID & "'"

    '            'Execute the SQL Sentence
    '            Dim dbCmd As New SqlCommand
    '            dbCmd.Connection = pDBConnection
    '            dbCmd.CommandText = cmdText

    '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
    '            resultData.HasError = False
    '        End If
    '    Catch ex As Exception
    '        resultData.HasError = True
    '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        resultData.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeletebyContaminatorIDContamintedID", EventLogEntryType.Error, False)
    '    End Try
    '    Return resultData
    'End Function

    '''' <summary>
    '''' Delete by the contamination type and the cuvette id  - SAME FUNCTIONALITY AS DeleteCuvettes!!
    '''' </summary>
    '''' <param name="pDBConnection"></param>
    '''' <param name="pContaminationType"></param>
    '''' <returns></returns>
    '''' <remarks>CREATED BY: TR 30/03/2011</remarks>
    'Public Function DeleteByContaminationTypeAndCuvetteID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestContaminaCuvetteID As Integer, _
    '                                                      ByVal pContaminationType As String) As GlobalDataTO
    '    Dim resultData As New GlobalDataTO

    '    Try
    '        If (pDBConnection Is Nothing) Then
    '            resultData.HasError = True
    '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
    '        Else
    '            Dim cmdText As String = ""
    '            cmdText = " DELETE tparContaminations "
    '            cmdText &= " WHERE  ContaminationType = '" & pContaminationType & "'"
    '            cmdText &= "AND TestContaminaCuvetteID = " & pTestContaminaCuvetteID

    '            'Execute the SQL Sentence
    '            Dim dbCmd As New SqlCommand
    '            dbCmd.Connection = pDBConnection
    '            dbCmd.CommandText = cmdText

    '            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
    '            resultData.HasError = False
    '        End If
    '    Catch ex As Exception
    '        resultData.HasError = True
    '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        resultData.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.DeleteByContaminationType", EventLogEntryType.Error, False)
    '    End Try
    '    Return resultData
    'End Function

    '''' <summary>
    '''' Search if the informed Test contaminates Cuvettes
    '''' </summary>
    '''' <param name="pDBConnection">Open DB Connection</param>
    '''' <param name="pTestContaminaCuvette">Identifier of the Contaminator Test</param>
    '''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with the Contamination defined for the informed TestID</returns>
    '''' <remarks>
    '''' Created by:  TR 30/03/2011
    '''' </remarks>
    'Public Function ReadByTestContaminaCuvetteID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestContaminaCuvette As Integer) As GlobalDataTO
    '    Dim resultData As New GlobalDataTO
    '    Dim dbConnection As New SqlClient.SqlConnection

    '    Try
    '        resultData = GetOpenDBConnection(pDBConnection)
    '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
    '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
    '            If (Not dbConnection Is Nothing) Then
    '                Dim cmdText As String = ""
    '                cmdText = "  SELECT * FROM tparContaminations "
    '                cmdText &= " WHERE  TestContaminaCuvetteID = " & pTestContaminaCuvette

    '                Dim dbCmd As New SqlClient.SqlCommand
    '                dbCmd.Connection = dbConnection
    '                dbCmd.CommandText = cmdText

    '                'Fill the DataSet to return 
    '                Dim contaminationsDataDS As New ContaminationsDS
    '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
    '                dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

    '                resultData.SetDatos = contaminationsDataDS
    '                resultData.HasError = False
    '            End If
    '        End If
    '    Catch ex As Exception
    '        resultData.HasError = True
    '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        resultData.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "tparContaminationsDAO.ReadByTestContaminaCuvetteID", EventLogEntryType.Error, False)
    '    Finally
    '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
    '    End Try
    '    Return resultData
    'End Function


#End Region

End Class
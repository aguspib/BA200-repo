Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO

    Public Class ContaminationsUpdateDAO
        Inherits DAOBase


        Public Function GetAffectedItemsFromFactory(ByVal pDBConnection As SqlClient.SqlConnection, Optional pGetCuvettesType As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        If pGetCuvettesType Then
                            'Get the values for cuvetes Types
                            cmdText &= "SELECT     C.*, T.TestID, T.TestName  " & vbCrLf
                            cmdText &= " FROM      Ax00TEM.dbo.tparContaminations C INNER JOIN" & vbCrLf
                            cmdText &= " tparTests T ON T.TestID  = C.TestContaminaCuvetteID " & vbCrLf
                            cmdText &= " EXCEPT " & vbCrLf
                            cmdText &= "SELECT     C.*, T.TestID, T.TestName  " & vbCrLf
                            cmdText &= " FROM      Ax00.dbo.tparContaminations C INNER JOIN" & vbCrLf
                            cmdText &= " tparTests T ON T.TestID  = C.TestContaminaCuvetteID" & vbCrLf

                        Else
                            'Get the values for R1 and R2 Types
                            cmdText &= " SELECT C.ContaminationID, R.ReagentName AS ContaminatorName ,C.ReagentContaminatorID, " & vbCrLf
                            cmdText &= " R1.ReagentName AS ContaminatedName, C.ReagentContaminatedID, C.TestContaminaCuvetteID, " & vbCrLf
                            cmdText &= " C.ContaminationType,C.WashingSolutionR1,C.WashingSolutionR2, R.PreloadedReagent " & vbCrLf
                            cmdText &= " FROM Ax00TEM.dbo.tparContaminations C, Ax00TEM.dbo.tparReagents R, Ax00TEM.dbo.tparReagents R1 " & vbCrLf
                            cmdText &= " WHERE R.ReagentID  = C.ReagentContaminatorID AND R1.ReagentID = C.ReagentContaminatedID  " & vbCrLf
                            cmdText &= " EXCEPT " & vbCrLf
                            cmdText &= " SELECT C.ContaminationID, R.ReagentName AS Contaminator ,C.ReagentContaminatorID, " & vbCrLf
                            cmdText &= " R1.ReagentName AS Contaminated, C.ReagentContaminatedID, C.TestContaminaCuvetteID, " & vbCrLf
                            cmdText &= " C.ContaminationType,C.WashingSolutionR1,C.WashingSolutionR2, R.PreloadedReagent " & vbCrLf
                            cmdText &= " FROM Ax00.dbo.tparContaminations C, Ax00.dbo.tparReagents R, Ax00.dbo.tparReagents R1 " & vbCrLf
                            cmdText &= " WHERE R.ReagentID  = C.ReagentContaminatorID AND R1.ReagentID = C.ReagentContaminatedID " & vbCrLf
                        End If


                        Dim resultData As New DataSet
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.GetAffectedItemsFromFactory", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Search on factorzDB the contamination bz the contamination ID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pContaminationID">Contaminator Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ContaminationsDS with all the Contaminations in which the
        '''          specified Reagent acts as contaminator or contaminated</returns>
        ''' <remarks>
        ''' Created by:  TR 11/02/2013
        ''' </remarks>
        Public Function GetContaminationsByContaminatonID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminationID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "  SELECT C.*, r.ReagentName AS contaminatorName, R1.ReagentName as ContaminatedName "
                        cmdText &= " FROM   Ax00TEM.dbo.tparContaminations  C, Ax00TEM.dbo.tparReagents R, Ax00TEM.dbo.tparReagents R1 "
                        cmdText &= " WHERE  ContaminationID = " & pContaminationID.ToString()
                        cmdText &= " AND r.ReagentID = c.ReagentContaminatorID "
                        cmdText &= " AND r1.ReagentID = c.ReagentContaminatedID "
                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim contaminationsDataDS As New ContaminationsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(contaminationsDataDS.tparContaminations)

                        myGlobalDataTO.SetDatos = contaminationsDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.GetContaminationsByContaminatorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Validate if the indicated reagent belong to factory test.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReagentID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 11/02/2013
        ''' </remarks>
        Public Function IsFactoryReagent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As Boolean
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim isFactoryValue As Boolean = False

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        cmdText &= " SELECT T.PreloadedTest  "
                        cmdText &= " FROM   tparTests T WITH (NOLOCK) INNER JOIN tparTestReagents TR WITH (NOLOCK) ON T.TestID = TR.TestID "
                        cmdText &= " WHERE TR.ReagentID = " & pReagentID

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        isFactoryValue = CBool(myGlobalDataTO.SetDatos)

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.IsFactoryReagent", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return isFactoryValue
        End Function

        ''' <summary>
        ''' Validate if the indicated Test belong to factory test.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 12/02/2013</remarks>
        Public Function IsFactoryTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As Boolean
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim isFactoryValue As Boolean = False

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        cmdText &= " SELECT T.PreloadedTest  "
                        cmdText &= " FROM   tparTests T  WITH (NOLOCK)"
                        cmdText &= " WHERE T.TestID= " & pTestID

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        isFactoryValue = CBool(myGlobalDataTO.SetDatos)

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.IsFactoryTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return isFactoryValue
        End Function


        ''' <summary>
        ''' Delete the contaminations of the specified type that are led by the ReagentContaminatorID and ReagentContaminatedID
        ''' (USE ON UPDATE PROCESS ONLY)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pContaminatorReagentID">Identifier of the Contaminator Reagent to delete</param>
        ''' <param name="pContaminationType">Type of Contamination to delete for the informed Contaminator Reagent</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 12/02/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pContaminatorReagentID As Integer, _
                               pContaminatedReagentID As Integer, ByVal pContaminationType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE tparContaminations " & _
                              " WHERE  ReagentContaminatorID = " & pContaminatorReagentID & _
                              " AND    ReagentContaminatedID = " & pContaminatedReagentID & _
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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.Delete", EventLogEntryType.Error, False)
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
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.DeleteCuvettes", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS"
        ''' <summary>
        ''' Search in FACTORY DB all R1 Contaminations defined for Preloaded STD TESTS that are not in the CUSTOMER DB or that are in CUSTOMER DB but with different Washing Solution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a ContaminationsDS with the group of R1 Contaminations obtained</returns>
        ''' <remarks>
        ''' Created by: SA 13/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Public Function GetNEWorUPDContaminationsR1(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOptionalFilters As String = String.Empty
                        Dim cmdText As String = " SELECT C.ContaminationType, T.TestID AS TestContaminatorID, T1.TestID AS TestContaminatedID, C.WashingSolutionR1 " & vbCrLf & _
                                                " FROM       " & GlobalBase.TemporalDBName & ".[dbo].[tparContaminations] C " & vbCrLf & _
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTestReagents] TR ON C.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] T ON TR.TestID = T.TestID " &
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTestReagents] TR1 ON C.ReagentContaminatedID = TR1.ReagentID " & vbCrLf & _
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] T1 ON TR1.TestID = T1.TestID " &
                                                " WHERE C.ContaminationType = 'R1' " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT C.ContaminationType, T.TestID AS TestContaminatorID, T1.TestID AS TestContaminatedID, C.WashingSolutionR1 " & vbCrLf & _
                                                " FROM       [Ax00].[dbo].[tparContaminations] C " & vbCrLf & _
                                                " INNER JOIN [Ax00].[dbo].[tparTestReagents] TR ON C.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                " INNER JOIN [Ax00].[dbo].[tparTests] T ON TR.TestID = T.TestID " &
                                                " INNER JOIN [Ax00].[dbo].[tparTestReagents] TR1 ON C.ReagentContaminatedID = TR1.ReagentID " & vbCrLf & _
                                                " INNER JOIN [Ax00].[dbo].[tparTests] T1 ON TR1.TestID = T1.TestID " &
                                                " WHERE C.ContaminationType = 'R1' " & vbCrLf

                        Dim r1ContaminationsDS As New ContaminationsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(r1ContaminationsDS.tparContaminations)
                            End Using
                        End Using

                        resultData.SetDatos = r1ContaminationsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.GetNEWorUPDContaminationsR1", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all CUVETTES Contaminations defined for Preloaded STD TESTS that are not in the CUSTOMER DB or that 
        ''' are in CUSTOMER DB but with different Washing Solution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a ContaminationsDS with the group of CUVETTES Contaminations obtained</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Public Function GetNEWorUPDContaminationsCUVETTES(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOptionalFilters As String = String.Empty
                        Dim cmdText As String = " SELECT C.ContaminationType, C.TestContaminaCuvetteID, C.WashingSolutionR1, C.WashingSolutionR2 " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparContaminations] C " & vbCrLf & _
                                                " WHERE  C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT C.ContaminationType, C.TestContaminaCuvetteID, C.WashingSolutionR1, C.WashingSolutionR2 " & vbCrLf & _
                                                " FROM   [Ax00].[dbo].[tparContaminations] C " & vbCrLf & _
                                                " WHERE  C.ContaminationType = 'CUVETTES' " & vbCrLf

                        Dim cuvettesContaminationsDS As New ContaminationsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(cuvettesContaminationsDS.tparContaminations)
                            End Using
                        End Using

                        resultData.SetDatos = cuvettesContaminationsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.GetNEWorUPDContaminationsCUVETTES", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in  CUSTOMER DB all  Contaminations of R1 type (for preloaded Tests) that have been deleted from FACTORY DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a ContaminationsDS with the group of R1 Contaminations to delete</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Public Function GetDELContaminationsR1(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOptionalFilters As String = String.Empty
                        Dim cmdText As String = " SELECT C.ContaminationType, T.TestID AS TestContaminatorID, T1.TestID AS TestContaminatedID " & vbCrLf & _
                                                " FROM       [Ax00].[dbo].[tparContaminations] C " & vbCrLf & _
                                                " INNER JOIN [Ax00].[dbo].[tparTestReagents] TR ON C.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                " INNER JOIN [Ax00].[dbo].[tparTests] T ON TR.TestID = T.TestID " &
                                                " INNER JOIN [Ax00].[dbo].[tparTestReagents] TR1 ON C.ReagentContaminatedID = TR1.ReagentID " & vbCrLf & _
                                                " INNER JOIN [Ax00].[dbo].[tparTests] T1 ON TR1.TestID = T1.TestID " &
                                                " WHERE C.ContaminationType = 'R1' " & vbCrLf & _
                                                " AND   T.PreloadedTest = 1 " & vbCrLf & _
                                                " AND   T1.PreloadedTest = 1 " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT C.ContaminationType, T.TestID AS TestContaminatorID, T1.TestID AS TestContaminatedID " & vbCrLf & _
                                                " FROM       " & GlobalBase.TemporalDBName & ".[dbo].[tparContaminations] C " & vbCrLf & _
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTestReagents] TR ON C.ReagentContaminatorID = TR.ReagentID " & vbCrLf & _
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] T ON TR.TestID = T.TestID " &
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTestReagents] TR1 ON C.ReagentContaminatedID = TR1.ReagentID " & vbCrLf & _
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] T1 ON TR1.TestID = T1.TestID " &
                                                " WHERE C.ContaminationType = 'R1' " & vbCrLf

                        Dim r1ContaminationsDS As New ContaminationsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(r1ContaminationsDS.tparContaminations)
                            End Using
                        End Using

                        resultData.SetDatos = r1ContaminationsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.GetDELContaminationsR1", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in  CUSTOMER DB all  Contaminations of CUVETTES type (for preloaded Tests) that have been deleted from FACTORY DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a ContaminationsDS with the group of CUVETTES Contaminations to delete</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Public Function GetDELContaminationsCUVETTES(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOptionalFilters As String = String.Empty
                        Dim cmdText As String = " SELECT C.ContaminationType, C.TestContaminaCuvetteID " & vbCrLf & _
                                                " FROM   [Ax00].[dbo].[tparContaminations] C INNER JOIN [Ax00].[dbo].[tparTests] T ON C.TestContaminaCuvetteID = T.TestID " & vbCrLf & _
                                                " WHERE  C.ContaminationType = 'CUVETTES' " & vbCrLf & _
                                                " AND    T.PreloadedTest = 1 " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT C.ContaminationType, C.TestContaminaCuvetteID " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparContaminations] C " & vbCrLf & _
                                                " WHERE  C.ContaminationType = 'CUVETTES' " & vbCrLf

                        Dim cuvettesContaminationsDS As New ContaminationsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(cuvettesContaminationsDS.tparContaminations)
                            End Using
                        End Using

                        resultData.SetDatos = cuvettesContaminationsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsUpdateDAO.GetDELContaminationsCUVETTES", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
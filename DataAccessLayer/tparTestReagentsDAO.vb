Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestReagentsDAO
        Inherits DAOBase

#Region "Other Methods"

        ''' <summary>
        ''' Create relations between Standard Tests and Reagents
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestReagentsDS">Typed DataSet TestReagentsDS with the list of relations Test-Reagent to create</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 24/02/2010
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestReagentsDS As TestReagentsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim keys As String = " (TestID, ReagentID, ReagentNumber) "
                    Dim values As String = ""
                    Dim cmdText As String = ""

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    For Each myTestReagentRow As TestReagentsDS.tparTestReagentsRow In pTestReagentsDS.tparTestReagents.Rows
                        values = ""
                        values &= myTestReagentRow.TestID.ToString() & ", "
                        values &= myTestReagentRow.ReagentID.ToString() & ", "
                        values &= myTestReagentRow.ReagentNumber.ToString() & ""

                        cmdText = " INSERT INTO tparTestReagents " & keys & " VALUES(" & values & ")"

                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()

                        If (myGlobalDataTO.AffectedRecords > 0) Then
                            myGlobalDataTO.HasError = False
                        Else
                            myGlobalDataTO.HasError = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete a relation between a Test and a Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <param name="pReagentNumber">Reagent Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        '''Created by:  TR 15/03/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pReagentID As Integer, _
                               ByVal pReagentNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparTestReagents " & vbCrLf & _
                                            " WHERE  TestID        = " & pTestID & vbCrLf & _
                                            " AND    ReagentID     = " & pReagentID & vbCrLf & _
                                            " AND    ReagentNumber = " & pReagentNumber

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords += cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Reagents required for an specific Test or, if parameter pReagentNumber is informed, get the ID of the Reagent 
        ''' linked to the Test as the specified Reagent Number
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pReagentNumber">Reagent Number. Optional parameter; when informed, only the ID of the Reagent linked to the Test as 
        '''                              this Reagent Number is obtained</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS with the list of Reagents required for the informed Test</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2010
        ''' Modified by: SA 07/10/2011 - Changed the function template amd the sintax of the SQL query
        '''              SA 18/10/2012 - Get also field PreloadedReagent from table tparReagents
        '''              SA 10/10/2014 - BA-1944 (SubTask BA-1984) ==> Get also field CodeTest from table tparReagents
        '''              SA 14/10/2014 - BA-1944 (SubTask BA-1986) ==> Added new optional parameter pReagentNumber and changed the SQL Query
        '''                                                            to filter it by ReagentNumber when the parameter is informed. Get also
        '''                                                            field ShortName from tparTests
        ''' </remarks>
        Public Function GetTestReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                        Optional ByVal pReagentNumber As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID, T.TestName, T.ShortName, T.ReagentsNumber, T.BlankReplicates, " & vbCrLf & _
                                                       " TR.ReagentID, TR.ReagentNumber, R.ReagentName, R.PreloadedReagent, R.CodeTest " & vbCrLf & _
                                                " FROM   tparTestReagents TR INNER JOIN tparReagents R ON TR.ReagentID = R.ReagentID " & vbCrLf & _
                                                                           " INNER JOIN tparTests T ON TR.TestID = T.TestID " & vbCrLf & _
                                                " WHERE  TR.TestID    = " & pTestID.ToString & vbCrLf

                        If (pReagentNumber <> -1) Then cmdText &= " AND TR.ReagentNumber = " & pReagentNumber.ToString & vbCrLf
                        cmdText &= " ORDER BY TR.ReagentNumber " & vbCrLf

                        Dim resultData As New TestReagentsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tparTestReagents)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.GetTestReagents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Tests using the specified Reagent
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS</returns>
        ''' <remarks>
        ''' Created by:  TR 18/05/2010
        ''' Modified by: SA 07/10/2011 - Change the query to get also the name of all Tests using the specified Reagent. Changed the function template
        ''' </remarks>
        Public Function GetTestReagentsByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TR.*, T.TestName " & vbCrLf & _
                                                " FROM   tparTestReagents TR INNER JOIN tparTests T ON TR.TestID = T.TestID " & vbCrLf & _
                                                " WHERE  TR.ReagentID =" & pReagentID.ToString()

                        Dim resultData As New TestReagentsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tparTestReagents)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.GetTestReagentsByReagentID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Standard Tests with their correspondent Reagent (R1 or R2, depending on the informed
        ''' Reagent Number). To load DS needed in ProgTestContaminations Screen
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentNum">Indicates the obtained Reagents will be the ones used as this Reagent number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ?? with the list of Tests and Reagents</returns>
        ''' <remarks>
        ''' Created by:  SA 30/11/2010
        ''' Modified by: SA 07/10/2011 - Changed the function template
        ''' </remarks>
        Public Function ReadByReagentNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentNum As Integer, _
                                            Optional ByVal pExcludedTestID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT 0 AS Selected, -1 AS ContaminationID, T.TestID, T.TestName, TR.ReagentID, R.IsShared, " & vbCrLf & _
                                                       " NULL AS WashingSolution, 1 AS Visible, 'N' AS AlreadySaved " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestReagents TR ON T.TestID = TR.TestID " & vbCrLf & _
                                                                   " INNER JOIN tparReagents R ON TR.ReagentID = R.ReagentID " & vbCrLf & _
                                                " WHERE  TR.ReagentNumber = " & pReagentNum & vbCrLf

                        If (pExcludedTestID <> -1) Then cmdText &= " AND T.TestID <> " & pExcludedTestID & vbCrLf
                        cmdText &= " ORDER BY T.TestPosition "

                        Dim contaminatedTestsDS As New TestContaminationsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(contaminatedTestsDS.tparContaminations)
                            End Using
                        End Using

                        resultData.SetDatos = contaminatedTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.ReadByReagentNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO REVIEW-DELETE"
        ''' <summary>
        ''' USED ONLY FOR THE OLD CONTAMINATIONS FORM
        ''' Get the list of Reagents by Reagent Number; or the list of Tests when the informed Type is 3 
        ''' (used by Contaminations - CUVETTES)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTypeReagents">Reagent Number (1 for R1/R3; 2 for R2/R4; 3 for obtain the list of Tests)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS with the list of obtained Reagents 
        '''          or Tests</returns>
        ''' <remarks>
        ''' Modified by:  DL 09/02/2010
        ''' 
        ''' </remarks>
        Public Function GetListReagentsByType(ByVal pDBConnection As SqlClient.SqlConnection, _
                                              ByVal pTypeReagents As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        Select Case pTypeReagents
                            Case 1   'R1/R3
                                cmdText &= " SELECT DISTINCT T.ReagentID, R.ReagentName, T.ReagentNumber, R.InUse, P.PreloadedTest " & vbCrLf
                                cmdText &= " FROM   tparTestReagents T INNER JOIN tparReagents R ON T.ReagentID = R.ReagentID " & vbCrLf
                                cmdText &= "                           INNER JOIN tparTests P ON T.TestID = P.TestID " & vbCrLf
                                cmdText &= " WHERE  T.ReagentNumber IN (1,3) " & vbCrLf
                                cmdText &= " ORDER BY R.ReagentName"

                            Case 2   'R2/R4
                                cmdText &= " SELECT DISTINCT T.ReagentID,R.ReagentName,T.ReagentNumber, R.InUse, P.PreloadedTest " & vbCrLf
                                cmdText &= " FROM   tparTestReagents T INNER JOIN tparReagents R ON T.ReagentID = R.ReagentID " & vbCrLf
                                cmdText &= "                           INNER JOIN tparTests P ON T.TestID = P.TestID " & vbCrLf
                                cmdText &= " WHERE  T.ReagentNumber IN (2,4)" & vbCrLf
                                cmdText &= " ORDER BY R.ReagentName"

                            Case Else   'CUVETTES 
                                cmdText &= " SELECT DISTINCT TestID, TestName, InUse, PreloadedTest " & vbCrLf
                                cmdText &= " FROM   tparTests " & vbCrLf
                                cmdText &= " ORDER BY TestName"
                        End Select

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim resultData As New TestReagentsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tparTestReagents)

                        myGlobalDataTO.HasError = False
                        myGlobalDataTO.SetDatos = resultData
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.GetListReagentsByType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        '''' <summary>
        '''' PENDING TO REVIEW - THIS FUNCTION HAS NO SENSE
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestReagentsDS">Typed DataSet TestReagentsDS with the list of relations Test-Reagent to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 24/02/2010
        '''' </remarks>
        'Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestReagentsDS As TestReagentsDS) As GlobalDataTO

        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            Dim values As String = " "
        '            Dim cmd As New SqlCommand
        '            cmd.Connection = pDBConnection

        '            For Each TestReagentRow As TestReagentsDS.tparTestReagentsRow In pTestReagentsDS.tparTestReagents.Rows
        '                values = ""
        '                values &= " TestID = " & TestReagentRow.TestID.ToString() & ", "
        '                values &= " ReagentID = " & TestReagentRow.ReagentID.ToString() & ", "
        '                values &= " ReagentsNumber = " & TestReagentRow.ReagentsNumber.ToString() & ", "

        '                cmdText = " UPDATE tparTests SET " & values & _
        '                          " WHERE  TestID = " & TestReagentRow.TestID.ToString() & _
        '                          " AND ReagentID = " & TestReagentRow.ReagentID & " " & _
        '                          " AND ReagentsNumber = " & TestReagentRow.ReagentsNumber & " "
        '            Next
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.Update", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the list of Reagents required for an specific Test - NOT USED
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pType">Test Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestReagentsDS with the list of Reagents
        ''''          required for the informed Test</returns>
        '''' <remarks>
        '''' Created by:  SA 09/02/2010
        '''' </remarks>
        'Public Function GetTestReagentsType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pType As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT T.TestID, T.TestName, T.ReagentsNumber, T.BlankReplicates, " & _
        '                                 " TR.ReagentID, R.ReagentName " & _
        '                          " FROM   tparTestReagents TR, tparReagents R, tparTests T " & _
        '                          " WHERE  T.ReagentsNumber = " & pType & _
        '                          " AND    TR.ReagentID = R.ReagentID " & _
        '                          " AND    TR.TestID    = T.TestID " & _
        '                          " ORDER BY TR.ReagentNumber "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                Dim resultData As New TestReagentsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(resultData.tparTestReagents)

        '                myGlobalDataTO.HasError = False
        '                myGlobalDataTO.SetDatos = resultData
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.GetTestReagents", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace
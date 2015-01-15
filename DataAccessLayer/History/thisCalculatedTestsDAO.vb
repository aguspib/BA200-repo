Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisCalculatedTestsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' A Calculated Test is closed in Historics Module (field ClosedCalcTest is set to TRUE) in following cases:
        '''   ** When the Calculated Test Formula is changed in Calculated Tests Programming Screen
        '''   ** When the Calculated Test is deleted in Calculated Tests Programming screen
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistCalcTestID">Calculated Test Identifier in Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 20/02/2012
        ''' Modified by: SA 24/02/2012 - Name of the function changed from Delete to CloseCalculatedTest
        ''' </remarks>
        Public Function CloseCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistCalcTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    cmdText.AppendLine(" UPDATE thisCalculatedTests SET ClosedCalcTest = 1 ")
                    cmdText.AppendFormat(" WHERE HistCalcTestID = {0} ", pHistCalcTestID)

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalculatedTestsDAO.CloseCalculatedTest", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a list of Calculated Tests to the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalculatedTestsDS">Typed DataSet HisCalculatedTestsDS containing all Calculated Tests to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisCalculatedTestsDS with all created Calculated Tests with the generated 
        '''          HistCalTestID</returns>
        ''' <remarks>
        ''' Created by:  RH 17/02/2012
        ''' Modified by: SA 24/02/2012 - Removed parameter OnlyTheFirstRow. Changed the query to remove field DeletedCalcTest (this field has a default
        '''                              value for inserts and its name has been changed to ClosedCalcTest)
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalculatedTestsDS As HisCalculatedTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim HistCalcTestID As Integer = -1

                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection

                        For Each row As HisCalculatedTestsDS.thisCalculatedTestsRow In pHisCalculatedTestsDS.thisCalculatedTests.Rows
                            cmdText.Append(" INSERT INTO thisCalculatedTests ")
                            cmdText.AppendLine(" (CalcTestID, CalcTestLongName, MeasureUnit, DecimalsAllowed, FormulaText) ")
                            cmdText.AppendFormat("VALUES({0}, N'{1}', N'{2}', '{3}', N'{4}')", row.CalcTestID, row.CalcTestLongName.Replace("'", "''"), _
                                                 row.MeasureUnit, row.DecimalsAllowed, row.FormulaText)
                            cmdText.AppendFormat("{0}SELECT SCOPE_IDENTITY()", vbCrLf)
                            dbCmd.CommandText = cmdText.ToString()

                            'Execute the SQL script 
                            HistCalcTestID = CType(dbCmd.ExecuteScalar(), Integer)
                            If (HistCalcTestID > 0) Then
                                row.HistCalcTestID = HistCalcTestID
                            End If

                            cmdText.Length = 0 'Instead of using Remove use the Lenght = 0 
                        Next
                    End Using

                    resultData.SetDatos = pHisCalculatedTestsDS
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalculatedTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all not in use closed Calculated Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisCalculatedTests " & vbCrLf & _
                                            " WHERE  ClosedCalcTest = 1 " & vbCrLf & _
                                            " AND    HistCalcTestID NOT IN (SELECT HistTestID  FROM thisWSOrderTests " & vbCrLf & _
                                                                                      " WHERE  TestType = 'CALC') " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalculatedTestsDAO.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if an open version of the Calculated Test already exists in Historics Module and in this case, get the its data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Calculated Test Identifier in Parameters Programming</param>
        ''' <returns>GlobalDataTO containing a typed DS HisCalculatedTestsDS with data of the Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  RH 21/02/2012
        ''' Modified by: SA 24/02/2012 - Query changed due to name of field DeletedCalcTest has been changed by ClosedCalcTest
        ''' </remarks>
        Public Function ReadByCalcTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As New StringBuilder
                        cmdText.AppendLine(" SELECT HistCalcTestID, CalcTestID, CalcTestLongName, MeasureUnit, DecimalsAllowed, FormulaText, ClosedCalcTest ")
                        cmdText.AppendLine(" FROM   thisCalculatedTests ")

                        cmdText.AppendFormat(" WHERE CalcTestID = {0} ", pCalcTestID)
                        cmdText.AppendFormat(" AND   ClosedCalcTest = 0 ")

                        Dim myDS As New HisCalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString(), dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDS.thisCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalculatedTestsDAO.ReadByHistCalcTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields CalcTestLongName, MeasureUnit and DecimalsAllowed for an specific Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalculatedTestsDS">Typed DS HisCalculatedTestsDS containing data of the Calculated Test to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 21/02/2012
        ''' Modified by: SA 24/02/2012 - Function has to return success/error information, not a typed DS HisCalculatedTestsDS
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalculatedTestsDS As HisCalculatedTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim row As HisCalculatedTestsDS.thisCalculatedTestsRow = pHisCalculatedTestsDS.thisCalculatedTests(0)

                    Using dbCmd As New SqlClient.SqlCommand()
                        cmdText.AppendLine(" UPDATE thisCalculatedTests ")
                        cmdText.AppendFormat(" SET CalcTestLongName = N'{0}', DecimalsAllowed = {1}, MeasureUnit = '{2}' " & vbCrLf, _
                                              row.CalcTestLongName, row.DecimalsAllowed, row.MeasureUnit)
                        cmdText.AppendFormat(" WHERE HistCalcTestID = {0} ", row.HistCalcTestID)

                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText.ToString()

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalculatedTestsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field FormulaText of a Calculated Test saved in Historic Module when the long name of a Standard, ISE, Off-System or Calculated Test included in its
        ''' formula is changed in the corresponding Programming Screen.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistCalcTestID">Identifier of the Calculated Test in Historic Module</param>
        ''' <param name="pNewFormulaText">New text of the Formula of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2012
        ''' Modified by: WE 11/11/2014 - RQ00035C (BA-1867) - Updated Summary description with ISE and Off-System as possible sources for changing its name.
        ''' </remarks>
        Public Function UpdateFormulaText(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistCalcTestID As Integer, ByVal pNewFormulaText As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE thisCalculatedTests " & vbCrLf & _
                                            " SET    FormulaText = N'" & pNewFormulaText.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " WHERE  HistCalcTestID = " & pHistCalcTestID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisCalculatedTestsDAO.UpdateFormulaText", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace

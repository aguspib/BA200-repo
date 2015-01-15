Option Strict On
Option Explicit On

Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksWSRotorPositionsInProcessDAO

#Region "CRUD"
        ''' <summary>
        ''' Create new record for an Analyzer, RotorType and Position. Default value for InProcessTestsNumber = 1
        ''' The position becomes in process
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pRotorInProcessDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorInProcessDS As RotorPositionsInProcessDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder()
                    Dim errorFlag As Boolean = False
                    For Each row As RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow In pRotorInProcessDS.twksWSRotorPositionsInProcess
                        If (row.IsAnalyzerIDNull) Then errorFlag = True
                        If (row.IsRotorTypeNull) Then errorFlag = True
                        If (row.IsCellNumberNull) Then errorFlag = True
                        If (row.IsInProcessTestsNumberNull) Then errorFlag = True

                        If Not errorFlag Then
                            cmdText.Append(" INSERT INTO twksWSRotorPositionsInProcess (AnalyzerID, RotorType, CellNumber, InProcessTestsNumber) ")
                            cmdText.Append(" VALUES(")
                            cmdText.AppendFormat("N'{0}', N'{1}', {2}, {3}) {4} ", _
                                                 row.AnalyzerID.Trim.Replace("'", "''"), row.RotorType.Trim.Replace("'", "''"), row.CellNumber.ToString, row.InProcessTestsNumber.ToString, vbCrLf)
                        Else
                            Exit For
                        End If
                    Next

                    If (cmdText.Length > 0) AndAlso Not errorFlag Then
                        'Execute all the INSERTS contained in the string 
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRotorPositionsInProcessDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified RotorType/CellNumber from table twksWSRotorPositionsInProcess, which means the affected Position is no longer In Process
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
        ''' <param name="pCellNumber">Rotor Position</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM twksWSRotorPositionsInProcess " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    RotorType  = '" & pRotorType.Trim & "' " & vbCrLf & _
                                            " AND    CellNumber = " & pCellNumber.ToString & " "

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRotorPositionsInProcessDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read contents for an Analyzer, RotorType and Position
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pRotorType"></param>
        ''' <param name="pCellNumberList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pCellNumberList As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing AndAlso pCellNumberList.Count > 0) Then
                        Dim listOfCells As String = String.Empty
                        For item As Integer = 0 To pCellNumberList.Count - 1
                            listOfCells += pCellNumberList(item).ToString
                            If item < pCellNumberList.Count - 1 Then
                                listOfCells += ", "
                            End If
                        Next

                        Dim cmdText As String = "SELECT AnalyzerID, RotorType, CellNumber, InProcessTestsNumber FROM twksWSRotorPositionsInProcess " & vbCrLf & _
                                                " WHERE AnalyzerID = N'" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND RotorType = N'" & pRotorType & "' " & vbCrLf

                        If pCellNumberList.Count = 1 Then
                            'Single item ... AND CellNumber = ...
                            cmdText += " AND CellNumber = " & listOfCells & " "
                        Else
                            'Multiple item ... AND CellNumber IN ( ... )
                            cmdText += " AND CellNumber IN ( " & listOfCells & " ) "
                        End If

                        Dim myDataSet As New RotorPositionsInProcessDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSRotorPositionsInProcess)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRotorPositionsInProcessDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update number of inprocess tests for this position in table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pRotorInProcessDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRotorInProcessDS As RotorPositionsInProcessDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder()
                    Dim errorFlag As Boolean = False
                    For Each row As RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow In pRotorInProcessDS.twksWSRotorPositionsInProcess
                        If (row.IsAnalyzerIDNull) Then errorFlag = True
                        If (row.IsRotorTypeNull) Then errorFlag = True
                        If (row.IsCellNumberNull) Then errorFlag = True
                        If (row.IsInProcessTestsNumberNull) Then errorFlag = True

                        If Not errorFlag Then
                            cmdText.Append(" UPDATE twksWSRotorPositionsInProcess SET InProcessTestsNumber = " & row.InProcessTestsNumber & " ")
                            cmdText.Append(" WHERE AnalyzerID = N'" & row.AnalyzerID.Trim.Replace("'", "''") & "' ")
                            cmdText.Append(" AND RotorType = N'" & row.RotorType & "' ")
                            cmdText.Append(" AND CellNumber = " & row.CellNumber & " ")
                        Else
                            Exit For
                        End If
                    Next

                    If cmdText.Length > 0 AndAlso Not errorFlag Then
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRotorPositionsInProcessDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get all Reagents Rotor positions that are currently In Process for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RotorPositionsInProcessDS with all pairs of RotorType/CellNumber 
        '''          that are currently In Process in Reagents Rotor</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2013
        ''' </remarks>
        Public Function ReadAllReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AnalyzerID, RotorType, CellNumber " & vbCrLf & _
                                                " FROM   twksWSRotorPositionsInProcess " & vbCrLf & _
                                                " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    RotorType = 'REAGENTS'"

                        Dim myDataSet As New RotorPositionsInProcessDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSRotorPositionsInProcess)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRotorPositionsInProcessDAO.ReadAllReagents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information about Rotor Positions In Process for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    cmdText &= " DELETE FROM twksWSRotorPositionsInProcess " & vbCrLf & _
                               " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' "

                    If cmdText <> "" Then
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRotorPositionsInProcessDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

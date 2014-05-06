Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksWSRepetitionsToAddDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a new Repetition for an Order Tests in a WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRepetitionsToAdd">Typed DataSet WSRepetitionsToAddDS with data of the repetition to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRepetitionsToAddDS with data of the
        '''          add Repetitions or error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/07/2010
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRepetitionsToAdd As WSRepetitionsToAddDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If (pRepetitionsToAdd.twksWSRepetitionsToAdd.Rows.Count > 0) Then
                        Dim myRepToAddRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow = pRepetitionsToAdd.twksWSRepetitionsToAdd.First

                        If (Not myRepToAddRow.IsAnalyzerIDNull AndAlso Not myRepToAddRow.IsWorkSessionIDNull AndAlso _
                            Not myRepToAddRow.IsOrderTestIDNull AndAlso Not myRepToAddRow.IsPostDilutionTypeNull) Then
                            Dim cmdText As String = " INSERT INTO twksWSRepetitionsToAdd(AnalyzerID, WorkSessionID, OrderTestID, PostDilutionType) " & vbCrLf & _
                                                    " VALUES (" & "'" & myRepToAddRow.AnalyzerID.Trim & "', " & vbCrLf & _
                                                                  "'" & myRepToAddRow.WorkSessionID.Trim & "', " & vbCrLf & _
                                                                        myRepToAddRow.OrderTestID.ToString & ", " & vbCrLf & _
                                                                  "'" & myRepToAddRow.PostDilutionType.Trim & "') "

                            Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                                resultData.HasError = False
                            End Using
                        Else
                            resultData.AffectedRecords = 0
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRepetitionsToAddDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete a Repetition for an Order Tests in a WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRepetitionsToAdd">Typed DataSet WSRepetitionsToAddDS with the Repetition to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/07/2010
        ''' Modified by: SA 20/07/2010 - Use GetOpenDBTransaction instead GetOpenDBConnection
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRepetitionsToAdd As WSRepetitionsToAddDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If (pRepetitionsToAdd.twksWSRepetitionsToAdd.Rows.Count > 0) Then
                        Dim myRepToAddRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow = pRepetitionsToAdd.twksWSRepetitionsToAdd.First
                        If (Not myRepToAddRow.IsAnalyzerIDNull AndAlso Not myRepToAddRow.IsWorkSessionIDNull AndAlso Not myRepToAddRow.IsOrderTestIDNull) Then
                            Dim cmdText As String = " DELETE FROM twksWSRepetitionsToAdd " & vbCrLf & _
                                                    " WHERE AnalyzerID = '" & myRepToAddRow.AnalyzerID.Trim & "' " & vbCrLf & _
                                                    " AND   WorkSessionID = '" & myRepToAddRow.WorkSessionID.Trim & "' " & vbCrLf & _
                                                    " AND   OrderTestID = " & myRepToAddRow.OrderTestID.ToString

                            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                                resultData.HasError = False
                            End Using
                        Else
                            resultData.AffectedRecords = 0
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRepetitionsToAddDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Repetitions requested in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/07/2010
        ''' Modified by: SA 20/07/2010 - Remove duplicated validations of informed parameters
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If (pAnalyzerID <> "" And pWorkSessionID <> "") Then
                        Dim cmdText As String = " DELETE FROM twksWSRepetitionsToAdd " & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using
                    Else
                        resultData.AffectedRecords = 0
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRepetitionsToAddDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get repetitions requested for an specific Order Test in a WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRepetitionsToAdd">Typed DataSet WSRepetitionsToAddDS with the Analyzer, WorkSession and 
        '''                                 Order Test for which the Repetitions will be read</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRepetitionsToAddDS with data of all repetitions
        '''          requested for the specific Order Test and WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SG 15/07/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRepetitionsToAdd As WSRepetitionsToAddDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pRepetitionsToAdd.twksWSRepetitionsToAdd.Rows.Count > 0) Then
                            Dim myRepetitionsToAdd As New WSRepetitionsToAddDS
                            Dim myRepToAddRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow = pRepetitionsToAdd.twksWSRepetitionsToAdd.First
                            If (Not myRepToAddRow.IsAnalyzerIDNull AndAlso Not myRepToAddRow.IsWorkSessionIDNull AndAlso Not myRepToAddRow.IsOrderTestIDNull) Then
                                Dim cmdText As String = " SELECT * FROM twksWSRepetitionsToAdd " & vbCrLf & _
                                                        " WHERE AnalyzerID = '" & myRepToAddRow.AnalyzerID.Trim & "' " & vbCrLf & _
                                                        " AND   WorkSessionID = '" & myRepToAddRow.WorkSessionID.Trim & "' " & vbCrLf & _
                                                        " AND   OrderTestID = " & myRepToAddRow.OrderTestID.ToString

                                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                        dbDataAdapter.Fill(myRepetitionsToAdd.twksWSRepetitionsToAdd)
                                    End Using
                                End Using

                                resultData.SetDatos = myRepetitionsToAdd
                                resultData.HasError = False
                            Else
                                'Return an empty DS
                                resultData.SetDatos = myRepetitionsToAdd
                                resultData.HasError = False
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRepetitionsToAddDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all the requested reruns for the specified Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRepetitionsToAddDS with data of all repetitions for the specified Analyzer Work Session</returns>
        ''' <remarks>
        ''' Created by:  SG 15/07/2010
        ''' Modified by: AG - When Analyzer and/or WorkSession is not informed, then return an empty DataSet
        '''              SA 10/07/2012 - Changed the SQL by adding an INNER JOIN with tables twksOrderTests and twksOrders to get value
        '''                              of field SampleClass for the specified OrderTestID
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorksessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRepetitionsToAdd As New WSRepetitionsToAddDS

                        If (pAnalyzerID <> "" AndAlso pWorksessionID <> "") Then
                            Dim cmdText As String = " SELECT WSRA.*, O.SampleClass " & vbCrLf & _
                                                    " FROM   twksWSRepetitionsToAdd WSRA INNER JOIN twksOrderTests OT ON WSRA.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                                   " AND WSRA.AnalyzerID  = OT.AnalyzerID " & vbCrLf & _
                                                                                       " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & vbCrLf & _
                                                    " WHERE  WSRA.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                    " AND    WSRA.WorkSessionID = '" & pWorksessionID.Trim & "' " & vbCrLf

                            Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                                Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                    dbDataAdapter.Fill(myRepetitionsToAdd.twksWSRepetitionsToAdd)
                                End Using
                            End Using

                            resultData.SetDatos = myRepetitionsToAdd
                            resultData.HasError = False
                        Else
                            'Return an empty DS
                            resultData.SetDatos = myRepetitionsToAdd
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRepetitionsToAddDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of a Repetition for an Order Tests in a WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRepetitionsToAdd">Typed DataSet WSRepetitionsToAddDS with data of the repetition to update</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRepetitionsToAddDS with data of the updated Repetition</returns>
        ''' <remarks>
        ''' Created by:  SG 15/07/2010
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRepetitionsToAdd As WSRepetitionsToAddDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    If (pRepetitionsToAdd.twksWSRepetitionsToAdd.Rows.Count > 0) Then
                        Dim myRepToAddRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow = pRepetitionsToAdd.twksWSRepetitionsToAdd.First
                        If (Not myRepToAddRow.IsAnalyzerIDNull AndAlso Not myRepToAddRow.IsWorkSessionIDNull AndAlso _
                            Not myRepToAddRow.IsOrderTestIDNull AndAlso Not myRepToAddRow.IsPostDilutionTypeNull) Then
                            Dim cmdText As String = " UPDATE twksWSRepetitionsToAdd SET PostDilutionType = '" & myRepToAddRow.PostDilutionType & "' " & vbCrLf & _
                                                    " WHERE  AnalyzerID = '" & myRepToAddRow.AnalyzerID.Trim & "' " & vbCrLf & _
                                                    " AND    WorkSessionID = '" & myRepToAddRow.WorkSessionID.Trim & "' " & vbCrLf

                            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                                resultData.SetDatos = pRepetitionsToAdd
                                resultData.HasError = False
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRepetitionsToAddDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

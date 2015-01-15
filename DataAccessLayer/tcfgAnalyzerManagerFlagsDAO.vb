Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Public Class tcfgAnalyzerManagerFlagsDAO
        Inherits DAOBase

        ''' <summary>
        ''' '
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pFlagID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Tested pending</remarks>
        Public Function ReadValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                             ByVal pFlagID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT Value " & vbCrLf & _
                                  " FROM   tcfgAnalyzerManagerFlags " & vbCrLf & _
                                  " WHERE  FlagID = '" & pFlagID & "'" & vbCrLf & _
                                  " AND  AnalyzerID = '" & pAnalyzerID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New AnalyzerManagerFlagsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myDS.tcfgAnalyzerManagerFlags)

                        resultData.SetDatos = myDS
                        resultData.HasError = False

                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.ReadValue", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - tested OK</remarks>
        Public Function ReadByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT AnalyzerID, FlagID, Value " & vbCrLf & _
                                  " FROM   tcfgAnalyzerManagerFlags " & vbCrLf & _
                                  " WHERE  AnalyzerID = '" & pAnalyzerID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New AnalyzerManagerFlagsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myDS.tcfgAnalyzerManagerFlags)

                        resultData.SetDatos = myDS
                        resultData.HasError = False

                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.ReadByAnalyzerID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerFlagsDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Tested OK</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerFlagsDS As AnalyzerManagerFlagsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pAnalyzerFlagsDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    For Each row As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow In pAnalyzerFlagsDS.tcfgAnalyzerManagerFlags
                        cmdText &= " INSERT INTO tcfgAnalyzerManagerFlags "
                        cmdText &= " (AnalyzerID, FlagID, Value) "
                        cmdText &= " VALUES ("

                        cmdText &= String.Format("'{0}', ", row.AnalyzerID) 'Required
                        cmdText &= String.Format("'{0}', ", row.FlagID) 'Required

                        If Not row.IsValueNull Then
                            cmdText &= String.Format("'{0}'", row.Value)
                        Else
                            cmdText &= " NULL "
                        End If

                        cmdText &= String.Format("){0}", vbNewLine) 'insert line break
                    Next

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerFlagsDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Tested pending</remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerFlagsDS As AnalyzerManagerFlagsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pAnalyzerFlagsDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    For Each flagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow In pAnalyzerFlagsDS.tcfgAnalyzerManagerFlags
                        cmdText &= " UPDATE tcfgAnalyzerManagerFlags "
                        cmdText &= " SET "

                        If Not flagRow.IsValueNull Then
                            cmdText &= " Value = " & String.Format(" '{0}'", flagRow.Value)
                        Else
                            cmdText &= " Value = NULL "
                        End If

                        cmdText &= " WHERE "
                        cmdText &= " AnalyzerID = " & String.Format("'{0}'", flagRow.AnalyzerID) 'Required
                        cmdText &= " AND FlagID = " & String.Format("'{0}' ", flagRow.FlagID) 'Required
                        cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    Next

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pFlagID"></param>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Tested pending</remarks>
        Public Function UpdateFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                   ByVal pFlagID As String, ByVal pValue As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString


                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " UPDATE tcfgAnalyzerManagerFlags "
                    cmdText &= " SET "

                    If Not pValue = "" Then
                        cmdText &= " Value = " & String.Format(" '{0}',", pValue)
                    Else
                        cmdText &= " Value = NULL "
                    End If

                    cmdText &= " WHERE "
                    cmdText &= " AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) 'Required
                    cmdText &= " AND FlagID = " & pFlagID 'Required
                    cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.UpdateFlag", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Reset all Software flags
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pLeaveConnectFlag" ></param>
        ''' <returns></returns>
        ''' <remarks>Modified AG 21/06/2012 - add parameter pLeaveConnectFlag</remarks>
        Public Function ResetFlags(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pLeaveConnectFlag As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " UPDATE tcfgAnalyzerManagerFlags "
                    cmdText &= " SET Value = NULL "

                    cmdText &= " WHERE "
                    cmdText &= " AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) 'Required

                    If pLeaveConnectFlag Then
                        cmdText &= " AND FlagID <> " & String.Format("'{0}'", GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString)
                    End If

                    dbCmd.CommandText = cmdText
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.ResetFlags", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Read all flags with status = pValue (if pReadWithSameValue = True)
        ''' Read all flags with status != pValue (if pReadWithSameValue = False)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pValue"></param>
        ''' <param name="pReadWithSameValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 09/03/2012</remarks>
        Public Function ReadByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pValue As String, _
                                     ByVal pReadWithSameValue As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT * " & vbCrLf & _
                                  " FROM   tcfgAnalyzerManagerFlags " & vbCrLf & _
                                  " WHERE  AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf

                        If pReadWithSameValue Then
                            If pValue <> "" Then
                                cmdText += " AND  Value = '" & pValue & "'"
                            Else
                                cmdText += " AND  Value IS NULL"
                            End If

                        Else
                            If pValue <> "" Then
                                cmdText += " AND  Value <> '" & pValue & "'"
                            Else
                                cmdText += " AND  Value IS NOT NULL"
                            End If

                        End If

                        Dim myDataSet As New AnalyzerManagerFlagsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.tcfgAnalyzerManagerFlags)
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
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.ReadByStatus", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete Flags from AnalyzerID specified
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XB 03/05/2013</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
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

                    cmdText &= " DELETE tcfgAnalyzerManagerFlags "
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) 'Required

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerManagerFlagsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


    End Class

End Namespace

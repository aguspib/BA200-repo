Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcfgAnalyzersDAO
        Inherits DAOBase

        ''' <summary>
        ''' Get all Analyzers defined in the DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: SA 11/01/2010 - Changed the way of opening the DB Conection to fulfill the new template.
        '''                              Added code to write in the application log in case of error
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   tcfgAnalyzers" & vbCrLf

                        Dim resultData As New AnalyzersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tcfgAnalyzers)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzersDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get all values of the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the Analyzer information</returns>
        ''' <remarks>
        ''' Created by:  DL 26/02/2010
        ''' </remarks>
        Public Function ReadByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   tcfgAnalyzers " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf

                        Dim myAnalyzersDS As New AnalyzersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAnalyzersDS.tcfgAnalyzers)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myAnalyzersDS
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzersDAO.ReadByAnalyzerID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Create a new Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzer">Analyzer to create</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 03/10/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzer As AnalyzersDS.tcfgAnalyzersRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " INSERT INTO tcfgAnalyzers "
                    cmdText &= " (AnalyzerID, AnalyzerModel, FirmwareVersion, Generic, Active ) "
                    cmdText &= " VALUES ("

                    cmdText &= " N'" & pAnalyzer.AnalyzerID.ToString.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " , N'" & pAnalyzer.AnalyzerModel.ToString.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " , N'" & pAnalyzer.FirmwareVersion.ToString.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " , 'False'" & vbCrLf
                    cmdText &= " , 'True' )" & vbCrLf

                    dbCmd.CommandText = cmdText
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    'Dim myLogAccionesAux As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("(Analyzer Change) Create Analyzer - AffectedRecords [" & resultData.AffectedRecords & " ]", "tcfgAnalyzersDAO.Create", EventLogEntryType.Information, False)

                    'If (resultData.AffectedRecords > 0) Then
                    '    Dim myAnalyzersDS As New AnalyzersDS
                    '    Dim myAnalyzersRow As AnalyzersDS.tcfgAnalyzersRow
                    '    myAnalyzersRow = myAnalyzersDS.tcfgAnalyzers.NewtcfgAnalyzersRow

                    '    myAnalyzersRow.AnalyzerID = pAnalyzer.AnalyzerID
                    '    myAnalyzersRow.AnalyzerModel = pAnalyzer.AnalyzerModel
                    '    myAnalyzersRow.FirmwareVersion = pAnalyzer.FirmwareVersion
                    '    myAnalyzersRow.Generic = False
                    '    myAnalyzersRow.Active = True
                    '    myAnalyzersDS.tcfgAnalyzers.Rows.Add(myAnalyzersRow)

                    '    resultData.SetDatos = myAnalyzersDS
                    '    resultData.HasError = False
                    'Else
                    '    resultData.HasError = True
                    '    resultData.ErrorCode = "SYSTEM_ERROR"
                    'End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzersDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all values of an specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pGeneric">Generic Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the Analyzer information</returns>
        ''' <remarks>
        ''' Created by:  XBC 06/06/2012
        ''' </remarks>
        Public Function ReadByAnalyzerGeneric(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pGeneric As Boolean) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   tcfgAnalyzers " & vbCrLf & _
                                                " WHERE  Generic = '" & pGeneric & "' " & vbCrLf

                        Dim myAnalyzersDS As New AnalyzersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAnalyzersDS.tcfgAnalyzers)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myAnalyzersDS
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzersDAO.ReadByAnalyzerGeneric", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get all values of the Active Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzersDS with the Analyzer information</returns>
        ''' <remarks>
        ''' Created by:  XBC 07/06/2012
        ''' </remarks>
        Public Function ReadByAnalyzerActive(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   tcfgAnalyzers " & vbCrLf & _
                                                " WHERE  Active = 'True' " & vbCrLf

                        Dim myAnalyzersDS As New AnalyzersDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAnalyzersDS.tcfgAnalyzers)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myAnalyzersDS
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzersDAO.ReadByAnalyzerActive", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete Las connected Analyzers
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 06/06/2012</remarks>
        Public Function DeleteConnectedAnalyzersNotActive(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    cmdText &= " DELETE FROM tcfgAnalyzers " & vbCrLf & _
                               " WHERE  Generic = 'False'" & vbCrLf & _
                               " AND    Active = 'False'"

                    dbCmd.CommandText = cmdText
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    'Dim myLogAccionesAux As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("(Analyzer Change) Delete Analyzer - AffectedRecords [" & resultData.AffectedRecords & " ]", "tcfgAnalyzersDAO.DeleteConnectedAnalyzersNotActive", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzersDAO.DeleteConnectedAnalyzers", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Analyzer specified as the Active Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 07/06/2012
        ''' </remarks>
        Public Function UpdateAnalyzerActive(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    Dim cmdText As String = ""
                    Dim myAffectedRecords As Integer
                    Dim dbCmd As New SqlClient.SqlCommand

                    ' First step : initializing setting all Analyzers Active to False
                    cmdText = " UPDATE tcfgAnalyzers SET [Active] = 'False' "

                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText
                    myAffectedRecords = dbCmd.ExecuteNonQuery()

                    ' Second step : set pAnalyzerID as the Analyzers Active 
                    cmdText = " UPDATE tcfgAnalyzers SET [Active] = 'True' " & _
                              " WHERE " & _
                              " AnalyzerID = '" & pAnalyzerID & "'"

                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText
                    myAffectedRecords += dbCmd.ExecuteNonQuery()

                    'Dim myLogAccionesAux As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("(Analyzer Change) Update Analyzer - AffectedRecords [" & myAffectedRecords & " ]", "tcfgAnalyzersDAO.UpdateAnalyzerActive", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.UpdateAnalyzerActive", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

    End Class

End Namespace
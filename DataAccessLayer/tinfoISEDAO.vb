Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tinfoISEDAO

        Inherits DAOBase

#Region "CRUD Methods"

        

        ''' <summary>
        ''' Get ISE information from the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISEInformationDS with data of the specified Analyzer identifier</returns>
        ''' <remarks>
        ''' Created by:  SGM 24/01/2012
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   tinfoISE " & _
                                  " WHERE  AnalyzerID = '" & pAnalyzerID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim InfoISE As New ISEInformationDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(InfoISE.tinfoISE)

                        resultData.SetDatos = InfoISE
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tinfoISEDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all ISE information
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISEInformationDS with data of the specified Analyzer identifier</returns>
        ''' <remarks>
        ''' Created by:  XB 10/05/2013
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   tinfoISE "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim InfoISE As New ISEInformationDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(InfoISE.tinfoISE)

                        resultData.SetDatos = InfoISE
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tinfoISEDAO.ReadAll (2)", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get ISE information from the specified Analyzer and ISE Setting ID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISEInformationDS with data of the specified Analyzer ID and ISE Setting ID</returns>
        ''' <remarks>
        ''' Created by:  SGM 24/01/2012
        ''' </remarks>
        Public Function ReadISESetting(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pISESetting As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   tinfoISE " & _
                                  " WHERE  AnalyzerID = '" & pAnalyzerID & "' " & _
                                  " AND ISESettingID = '" & pISESetting & "' "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim InfoISE As New ISEInformationDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(InfoISE.tinfoISE)

                        resultData.SetDatos = InfoISE
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tinfoISEDAO.ReadISESetting", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Creates a new ISE info master data with the informed dataset 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function CreateNewMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISEInfoDS As ISEInformationDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then


                        Dim i As Integer = 0
                        Dim recordOK As Boolean = True
                        For Each D As ISEInformationDS.tinfoISERow In pISEInfoDS.tinfoISE.Rows

                            Dim cmdText As String = ""
                            cmdText &= "INSERT INTO tinfoISE (AnalyzerID, ISESettingID,Value) " & vbCrLf
                            cmdText &= "VALUES ( N'" & D.AnalyzerID.ToString.Replace("'", "''") & "'"
                            cmdText &= " , N'" & D.ISESettingID.ToString.Replace("'", "''") & "'"
                            cmdText &= " , N'" & D.Value.ToString.Replace("'", "''") & "'"
                            cmdText &= ")"

                            'Execute the SQL Sentence
                            Dim dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText

                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            recordOK = (resultData.AffectedRecords = 1)
                            i += 1

                            If (Not recordOK) Then Exit For
                        Next


                        If (recordOK) Then
                            resultData.HasError = False
                            resultData.AffectedRecords = i
                            resultData.SetDatos = pISEInfoDS
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                            resultData.AffectedRecords = 0
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tinfoISEDAO.CreateNewMasterData", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pAnalyzerID As String, ByVal pISEInfoDS As ISEInformationDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                Dim recordOK As Boolean
                Dim j As Integer
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        For Each I As ISEInformationDS.tinfoISERow In pISEInfoDS.tinfoISE.Rows

                            Dim cmdText As String = ""

                            cmdText = " UPDATE tinfoISE SET [Value] = " & vbCrLf
                            cmdText &= "'" & I.Value.ToString & "'"
                            cmdText &= " WHERE ISESettingID = '" & I.ISESettingID & "' "
                            cmdText &= " AND AnalyzerID = '" & I.AnalyzerID & "' "

                            Dim dbCmd As New SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText

                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            recordOK = (resultData.AffectedRecords = 1)
                            j += 1

                            If (Not recordOK) Then
                                Exit For
                            End If

                        Next

                        If (recordOK) Then
                            resultData.HasError = False
                            resultData.AffectedRecords = j
                            resultData.SetDatos = pISEInfoDS
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                            resultData.AffectedRecords = 0
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tinfoISEDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update specified value
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analyzer identifier</param>
        ''' <param name="pSettingID">Setting identifier</param>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by: XB 10/05/2013</remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pAnalyzerID As String, _
                               ByVal pSettingID As String, _
                               ByVal pValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE tinfoISE " & vbCrLf & _
                                            " SET    [Value] = '" & pValue & "'" & vbCrLf & _
                                            " WHERE  ISESettingID = '" & pSettingID & "'" & vbCrLf & _
                                            " AND AnalyzerID = '" & pAnalyzerID & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tinfoISEDAO.Update (2)", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region


    End Class

End Namespace
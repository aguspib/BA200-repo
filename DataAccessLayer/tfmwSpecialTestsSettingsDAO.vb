Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwSpecialTestsSettingsDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Get the Settings defined for the specified Special Test/Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pSettingName">Optional parameter; when informed, only values of the
        '''                            specified Setting is returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SpecialTest</returns>
        ''' <remarks>
        ''' Created by:  DL 27/08/2010
        ''' Modified by: SA 30/08/2010 - When fill the defined DataAdapter, the name of the table inside
        '''                              the DataSet was missing and due to that, the function does not 
        '''                              return nothing
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                             ByVal pSampleType As String, Optional ByVal pSettingName As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= " SELECT TestID, SampleType, SettingName, SettingValue, Status  " & vbCrLf
                        cmdText &= " FROM   tfmwSpecialTestsSettings " & vbCrLf
                        cmdText &= " WHERE TestID = " & pTestID & vbCrLf
                        cmdText &= " AND   SampleType = '" & pSampleType & "' " & vbCrLf
                        cmdText &= " AND   Status = 1 "

                        If (pSettingName <> "") Then cmdText &= " AND SettingName = '" & pSettingName & "' "

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        ''Fill the DataSet to return 
                        'Dim mySpecialTestsSettingsDS As New SpecialTestsSettingsDS
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(mySpecialTestsSettingsDS.tfmwSpecialTestsSettings)
                        Dim mySpecialTestsSettingsDS As New SpecialTestsSettingsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mySpecialTestsSettingsDS.tfmwSpecialTestsSettings)
                            End Using
                        End Using


                        resultData.SetDatos = mySpecialTestsSettingsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwSpecialTestsSettingsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Read all executions in process whose test id are defined as special test because they are critical in pause mode
        ''' SpecialTestSetting - CRITICAL_PAUSEMODE
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pSpecialTestSetting"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo (ExecutionsDS)</returns>
        ''' <remarks>AG 22/11/2013 - Creation - Task #1391</remarks>
        Public Function ExistsCriticalPauseTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSpecialTestSetting As String, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        cmdText &= " SELECT ex.ExecutionID, ex.ExecutionStatus, ot.TestID, ex.StatFlag " & vbCrLf
                        cmdText &= " FROM   tfmwSpecialTestsSettings st INNER JOIN twksOrderTests ot " & vbCrLf
                        cmdText &= " ON st.TestID = ot.TestID INNER JOIN twksWSExecutions ex " & vbCrLf
                        cmdText &= " ON ot.OrderTestID = ex.OrderTestID WHERE st.SettingName = '" & pSpecialTestSetting & "' " & vbCrLf
                        cmdText &= " AND st.Status = 1 " & vbCrLf
                        cmdText &= " AND ex.ExecutionStatus = 'INPROCESS' " & vbCrLf
                        cmdText &= " AND ex.AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf
                        cmdText &= " AND ex.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                        Dim myDataSet As New ExecutionsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSExecutions)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwSpecialTestsSettingsDAO.ExistsCriticalPauseTests", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region
    End Class

End Namespace

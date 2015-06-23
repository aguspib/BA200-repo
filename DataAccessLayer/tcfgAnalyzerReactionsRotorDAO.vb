Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcfgAnalyzerReactionsRotorDAO
        Implements IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS)

#Region "C+R+U+D"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 17/05/2011 - </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As TypedGlobalDataTo(Of AnalyzerReactionsRotorDS) Implements IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS).Read
            'Dim resultData As New GlobalDataTO
            'Dim dbConnection As New SqlClient.SqlConnection
            Dim connection = DAOBase.GetSafeOpenDBConnection(pDBConnection)
            Try

                If (connection IsNot Nothing AndAlso connection.SetDatos IsNot Nothing) Then
                    Dim cmdText As String = ""
                    cmdText = " SELECT AnalyzerID, InstallDate, BLParametersRejected, WellsRejectedNumber " & vbCrLf & _
                              " FROM   tcfgAnalyzerReactionsRotor " & vbCrLf & _
                              " WHERE  AnalyzerID = '" & pAnalyzerID & "'"

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = connection.SetDatos
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim myDS As New AnalyzerReactionsRotorDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(myDS.tcfgAnalyzerReactionsRotor)

                    Return New TypedGlobalDataTo(Of AnalyzerReactionsRotorDS)() With {.SetDatos = myDS, .HasError = False}
                Else
                    Return New TypedGlobalDataTo(Of AnalyzerReactionsRotorDS)() With {.HasError = True}
                End If
            Catch ex As Exception
                Dim resultdata = New TypedGlobalDataTo(Of AnalyzerReactionsRotorDS)
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex)
                Return resultdata
            Finally
                If (pDBConnection Is Nothing) Then CloseConnection(connection)
            End Try
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pEntryDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 17/05/2011 created</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pEntryDS As AnalyzerReactionsRotorDS) As GlobalDataTO Implements IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS).Create
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pEntryDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection

                    For Each row As AnalyzerReactionsRotorDS.tcfgAnalyzerReactionsRotorRow In pEntryDS.tcfgAnalyzerReactionsRotor
                        cmdText &= " INSERT INTO tcfgAnalyzerReactionsRotor "
                        cmdText &= " (AnalyzerID, InstallDate, BLParametersRejected, WellsRejectedNumber ) "
                        cmdText &= " VALUES ("

                        cmdText &= String.Format("'{0}', ", row.AnalyzerID) 'Required
                        cmdText &= String.Format("'{0}', ", row.InstallDate.ToString("yyyyMMdd HH:mm:ss")) 'Required
                        cmdText &= CInt(IIf(row.BLParametersRejected, 1, 0)) & ", "
                        cmdText &= row.WellsRejectedNumber

                        cmdText &= String.Format(" ){0}", vbNewLine) 'insert line break
                    Next

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzerReactionsRotorDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pEntryDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 17/05/2011</remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pEntryDS As AnalyzerReactionsRotorDS) As GlobalDataTO Implements IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS).Update
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pEntryDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    Dim STR_NULL As String = " NULL, "

                    For Each Row As AnalyzerReactionsRotorDS.tcfgAnalyzerReactionsRotorRow In pEntryDS.tcfgAnalyzerReactionsRotor
                        cmdText &= " UPDATE tcfgAnalyzerReactionsRotor SET "

                        Dim cmdTextValues As String = ""
                        If Not Row.IsInstallDateNull Then
                            cmdTextValues &= " InstallDate = " & String.Format(" '{0}',", Row.InstallDate.ToString("yyyyMMdd HH:mm:ss"))
                        End If

                        If Not Row.IsBLParametersRejectedNull Then
                            If cmdTextValues <> "" Then cmdTextValues &= ", "
                            cmdTextValues &= " BLParametersRejected = " & CInt(IIf(Row.BLParametersRejected, 1, 0))
                        End If

                        If Not Row.IsWellsRejectedNumberNull Then
                            If cmdTextValues <> "" Then cmdTextValues &= ", "
                            cmdTextValues &= " WellsRejectedNumber = " & Row.WellsRejectedNumber
                        End If

                        cmdText &= cmdTextValues
                        cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", Row.AnalyzerID) 'Required
                        cmdText &= String.Format("{0}", vbNewLine) 'insert line break

                    Next

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzerReactionsRotor.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID" ></param>
        ''' <returns></returns>
        ''' <remarks>AG 17/05/2011 - tested pending</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO Implements IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS).Delete
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

                    cmdText &= " DELETE tcfgAnalyzerReactionsRotor "
                    cmdText &= " WHERE AnalyzerID = " & String.Format("'{0}'", pAnalyzerID) 'Required

                    dbCmd.CommandText = cmdText
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgAnalyzerReactionsRotor.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


#End Region

    End Class


End Namespace
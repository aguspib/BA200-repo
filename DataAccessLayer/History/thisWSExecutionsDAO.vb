Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisWSExecutionsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Insert a group of Executions in Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSExecutionsDS">Typed DataSet HisWSExecutionsDS containing data of all Executions to add to 
        '''                                  Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 20/06/2012
        ''' Modified by: SA 01/10/2012 - Changed the SQL due to changes in the table structure (new fields BLMainWL, BLMainWL_MainLight, 
        '''                              BLMainWL_RefLight, BLRefWL, BLRefWL_MainLight and BLRefWL_RefLight)
        '''              SA 17/10/2012 - Changed the SQL due to changes in the table structure (new fields BLMainLedPos and BLRefLedPos)
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSExecutionsDS As HisWSExecutionsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection

                        For Each myHisWSExecutionsRow As HisWSExecutionsDS.thisWSExecutionsRow In pHisWSExecutionsDS.thisWSExecutions.Rows
                            cmdText.Append(" INSERT INTO thisWSExecutions ")
                            cmdText.Append(" (HistOrderTestID, AnalyzerID, WorkSessionID, MultiPointNumber, ReplicateNumber, PostDilutionType, ")
                            cmdText.Append("  ResultDateTime, ABSValue, CONCValue, ABSInitial, ABSMainFilter, ABSWorkReagent, rKinetics, ")
                            cmdText.Append("  KineticsLinear, KineticsInitialValue, KineticsSlope, SubstrateDepletion, HistAdjustBaseLineID, ")
                            cmdText.Append("  WellUsed, BLMainLedPos, BLMainWL, BLMainWL_MainLight, BLMainWL_RefLight, ")
                            cmdText.Append("  BLRefLedPos, BLRefWL, BLRefWL_MainLight, BLRefWL_RefLight, AlarmList) ")
                            cmdText.AppendFormat(" VALUES({0}, N'{1}', '{2}', {3}, {4} ", _
                                                 myHisWSExecutionsRow.HistOrderTestID, _
                                                 myHisWSExecutionsRow.AnalyzerID.Trim.Replace("'", "''"), _
                                                 myHisWSExecutionsRow.WorkSessionID.Trim, _
                                                 myHisWSExecutionsRow.MultiPointNumber, _
                                                 myHisWSExecutionsRow.ReplicateNumber)

                            If (Not myHisWSExecutionsRow.IsPostDilutionTypeNull) Then
                                cmdText.Append(", '" & myHisWSExecutionsRow.PostDilutionType.Trim & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsResultDateTimeNull) Then
                                cmdText.Append(", '" & myHisWSExecutionsRow.ResultDateTime.ToString("yyyyMMdd HH:mm:ss") & "'")
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsABSValueNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.ABSValue))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsCONCValueNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.CONCValue))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsABSInitialNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.ABSInitial))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsABSMainFilterNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.ABSMainFilter))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsABSWorkReagentNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.ABSWorkReagent))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsrKineticsNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.rKinetics))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsKineticsLinearNull) Then
                                cmdText.Append(", " & IIf(myHisWSExecutionsRow.KineticsLinear, 1, 0).ToString)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsKineticsInitialValueNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.KineticsInitialValue))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsKineticsSlopeNull) Then
                                cmdText.Append(", " & ReplaceNumericString(myHisWSExecutionsRow.KineticsSlope))
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsSubstrateDepletionNull) Then
                                cmdText.Append(", " & IIf(myHisWSExecutionsRow.SubstrateDepletion, 1, 0).ToString)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsHistAdjustBaseLineIDNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.HistAdjustBaseLineID)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsWellUsedNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.WellUsed)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLMainLedPosNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLMainLedPos)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLMainWLNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLMainWL)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLMainWL_MainLightNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLMainWL_MainLight)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLMainWL_RefLightNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLMainWL_RefLight)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLRefLedPosNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLRefLedPos)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLRefWLNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLRefWL)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLRefWL_MainLightNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLRefWL_MainLight)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsBLRefWL_RefLightNull) Then
                                cmdText.Append(", " & myHisWSExecutionsRow.BLRefWL_RefLight)
                            Else
                                cmdText.Append(", NULL")
                            End If

                            If (Not myHisWSExecutionsRow.IsAlarmListNull) Then
                                cmdText.Append(", '" & myHisWSExecutionsRow.AlarmList.Trim & "')")
                            Else
                                cmdText.Append(", NULL)")
                            End If

                            dbCmd.CommandText = cmdText.ToString()
                            myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            cmdText.Length = 0
                        Next
                    End Using
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSExecutionsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "View to reuse IResultsCalibCurve screen in history"
        ''' <summary>
        ''' Get the historic curve replicate results (key parameters HistOrderTestID, AnalyzerID, WorkSessionID)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistOrderTestID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo with dataset as ExecutionsDS.vwksWSExecutionsResults</returns>
        ''' <remarks>AG 17/10/2012 - Creation</remarks>
        Public Function GetExecutionResultsForCalibCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As Integer, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "SELECT * " & _
                                  " FROM vhisWSCalibCurveExecutionResults" & _
                                  " WHERE OrderTestID = " & pHistOrderTestID & _
                                  " AND AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & _
                                  " AND WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

                        Dim myDataSet As New ExecutionsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.vwksWSExecutionsResults)
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
                GlobalBase.CreateLogActivity(ex.Message, "thisWSExecutionsDAO.GetExecutionResultsForCalibCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Delete all Executions saved in Historic Module for the specified AnalyzerID / WorkSessionID / OrderTestID
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestID">Identifier of the Order Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function DeleteByHistOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                        ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM thisWSExecutions " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                    " AND    HistOrderTestID = " & pHistOrderTestID.ToString & vbCrLf
        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSExecutionsDAO.DeleteByHistOrderTestID", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace


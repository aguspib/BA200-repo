Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports System.Globalization

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class ISEInformationUpdateData
        ''' <summary>
        ''' Performs update actions related to modified ISE data information in Factory DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XB - 10/05/2013
        ''' </remarks>
        Public Function UpdateDataFormatv1Tov2(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            'Dim myLogAcciones As New ApplicationLogManager()
            Dim myDataSet As DataSet = Nothing
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISEInfoDelegate As New ISEDelegate
                        Dim myDS As New ISEInformationDS
                        'Search elems affected
                        resultData = myISEInfoDelegate.ReadAllInfo(dbConnection)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myDS = CType(resultData.SetDatos, ISEInformationDS)

                            If myDS.tinfoISE.Rows.Count > 0 Then
                                For Each row As ISEInformationDS.tinfoISERow In myDS.tinfoISE.Rows
                                    'Check for values which has date time format
                                    If IsDateTime(row) Then
                                        Dim valueToSave As String
                                        'Extract date time value
                                        If row.Value.Length > 0 Then
                                            resultData = ExtractDateTime(row.Value)
                                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                valueToSave = CDate(resultData.SetDatos).ToString(CultureInfo.InvariantCulture)
                                            Else
                                                resultData.HasError = False
                                                ' can not recover the date so set an empty value
                                                valueToSave = ""
                                            End If
                                        Else
                                            valueToSave = ""
                                        End If

                                        'Save date time value with the invariant date time format
                                        myISEInfoDelegate.UpdateISEInfo(dbConnection, row.AnalyzerID, row.ISESettingID, valueToSave)
                                    End If
                                Next
                            End If

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex.Message, "ISEInformationUpdateData.UpdateDataFormatv1Tov2", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Returns if specified ISE field is Date time or not
        ''' </summary>
        ''' <param name="pISEInforow"></param>
        ''' <returns></returns>
        ''' <remarks>Created by : XB 10/05/2013</remarks>
        Public Function IsDateTime(ByVal pISEInforow As ISEInformationDS.tinfoISERow) As Boolean
            Dim resultData As Boolean = False
            Try
                Select Case pISEInforow.ISESettingID.ToString

                    Case GlobalEnumerates.ISEModuleSettings.REAGENTS_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.LI_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.NA_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.K_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.CL_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.REF_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.PUMP_TUBING_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.FLUID_TUBING_INSTALL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.LAST_CLEAN_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.LAST_CALB_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.LAST_PUMP_CAL_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.REAGENTS_EXPIRE_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.LAST_OPERATION_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.LAST_OPERATION_WS_DATE.ToString, _
                         GlobalEnumerates.ISEModuleSettings.LAST_BUBBLE_CAL_DATE.ToString

                        resultData = True
                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("ISE Info Update Error.", "ISEInformationUpdateData.IsDateTime", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        Public Function ExtractDateTime(ByVal pValue As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDateTimeValue As DateTime

                Dim ISEInfoDateFormat_v1 As String = "dd/MM/yyyy HH:mm:ss"
                myDateTimeValue = DateTime.ParseExact(pValue, ISEInfoDateFormat_v1, DateTimeFormatInfo.InvariantInfo)

                myGlobalDataTO.SetDatos = myDateTimeValue

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("ISE Info Update Error.", "ISEInformationUpdateData.ExtractDateTime", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

    End Class
End Namespace


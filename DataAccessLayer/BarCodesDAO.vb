Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class BarCodesDAO
          

#Region "CRUD"

        ''' <summary>
        ''' Updates the list of Samples Bar Codes Configuration values (view vwksSamplesBarCodesConfiguration)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSamplesBarCodesConfiguration">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 06/07/2011
        ''' Modified by: SA 19/04/2012 - Changed the function template for the one used for Insert/Update/Delete functions in 
        '''                              DAO Classes
        ''' </remarks>
        Public Function UpdateSamplesBarCodesConfiguration(ByVal pDBConnection As SqlConnection, ByVal pSamplesBarCodesConfiguration As BarCodesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty

                    Dim AffectedRecords As Integer = 0
                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                            For Each row As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In pSamplesBarCodesConfiguration.vcfgSamplesBarCodesConfiguration
                                cmdText = String.Format(" UPDATE tcfgBarCodesConfigurationTypes SET Status = {0}, CheckValue = {1} " + _
                                                        " WHERE CodeID = {2} AND RotorType = 'SAMPLES' ", _
                                                        IIf(row.Status, 1, 0), IIf(row.CheckValue, 1, 0), row.CodeID)

                                dbCmd.CommandText = cmdText
                                AffectedRecords += dbCmd.ExecuteNonQuery()
                            Next
                        End Using
                    End Using

                    resultData.SetDatos = AffectedRecords
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodesDAO.UpdateSamplesBarCodesConfiguration", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Gets the list of Samples Bar Codes Configuration values (view vwksSamplesBarCodesConfiguration)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns a BarCodesDS dataset with the Bar Codes configuration values (view vcfgSamplesBarCodesConfiguration)
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH - 06/07/2011
        ''' modified by: TR 30/08/2011 -Change the view name from vwksSamplesBarCodesConfiguration to vcfgSamplesBarCodesConfiguration
        ''' </remarks>
        Public Function GetSamplesBarCodesConfiguration(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = "SELECT * FROM vcfgSamplesBarCodesConfiguration "

                        Dim BarCodes As New BarCodesDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(BarCodes.vcfgSamplesBarCodesConfiguration)
                            End Using
                        End Using

                        resultData.SetDatos = BarCodes
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodesDAO.GetSamplesBarCodesConfiguration", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
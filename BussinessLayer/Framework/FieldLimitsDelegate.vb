Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Public Class FieldLimitsDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Get minimum and maximum allowed values for the specified Limit
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLimitID">Unique identifier of the limit id of FieldLimitsDelegate</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FieldLimitsDS with the minimum and 
        '''          maximum allowed values for the specified Limit</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 25/02/2010 - Return error Master Data Missing when there are not values for 
        '''                              the informed limit; changed datatype of parameter pLimitID
        '''              RH 08/09/2011 - Code Optimization. Short circuit evaluation. Remove unneeded and memory wasting "New" instructions.
        ''' </remarks>
        Public Function GetList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLimitID As FieldLimitsEnum, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myFieldLimits As New tfmwFieldLimitsDAO
                        resultData = myFieldLimits.Read(dbConnection, pLimitID, pAnalyzerModel)

                        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                            Dim myFieldLimitsDS As FieldLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)
                            If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
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
                GlobalBase.CreateLogActivity(ex.Message, "FieldLimitsDelegate.GetList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get minimum and maximum allowed values for all Limit
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FieldLimitsDS with the minimum and 
        '''          maximum allowed values for the specified Limit</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SG 01/09/2010 
        ''' </remarks>
        Public Function GetAllList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myFieldLimits As New tfmwFieldLimitsDAO
                        resultData = myFieldLimits.ReadAll(dbConnection, pAnalyzerModel)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myFieldLimitsDS As FieldLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)

                            If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
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
                GlobalBase.CreateLogActivity(ex.Message, "FieldLimitsDelegate.GetAllList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified Limits
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLimitsDS">Typed DS FieldLimitsDS containing the list of Limits to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 11/07/2011
        ''' </remarks>
        Public Function SaveLimits(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLimitsDS As FieldLimitsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myFieldLimits As New tfmwFieldLimitsDAO
                        For Each myLimitRow As FieldLimitsDS.tfmwFieldLimitsRow In pLimitsDS.tfmwFieldLimits.Rows
                            myGlobalDataTO = myFieldLimits.Update(dbConnection, myLimitRow)
                            If (myGlobalDataTO.HasError) Then Exit For
                        Next myLimitRow

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FieldLimitsDelegate.SaveLimits", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region
    End Class
End Namespace

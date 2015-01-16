Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL


    Partial Public Class HisReagentBottlesDelegate

#Region "C R U D"


        ''' <summary>
        ''' Read history reagent bottle by primary key.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistoryReagentBottlesDS"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: DL 03/08/2011</remarks>
        Public Function Read(ByVal pDBConnection As SqlConnection, ByVal pHistoryReagentBottlesDS As HisReagentsBottlesDS) As GlobalDataTO

            Dim result As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryReagentBottles As New thisReagentBottlesDAO
                        myGlobalDataTO = myHistoryReagentBottles.Read(dbConnection, pHistoryReagentBottlesDS)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryReagentBottlesDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Update history reagent bottles records 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistoryReagentBottlesDS"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: DL 03/08/2011
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlConnection, ByVal pHistoryReagentBottlesDS As HisReagentsBottlesDS) As GlobalDataTO

            Dim result As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryReagentBottles As New thisReagentBottlesDAO
                        myGlobalDataTO = myHistoryReagentBottles.Update(dbConnection, pHistoryReagentBottlesDS)
                    End If
                End If

                If Not myGlobalDataTO.HasError Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryReagentBottlesDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Read history reagent bottle by primary key.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pBarCode">BarCode</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 11/06/2012</remarks>
        Public Function ReadByBarCode(ByVal pDBConnection As SqlConnection, ByVal pBarCode As String) As GlobalDataTO

            Dim result As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryReagentBottles As New thisReagentBottlesDAO
                        myGlobalDataTO = myHistoryReagentBottles.ReadByBarCode(dbConnection, pBarCode)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryReagentBottlesDelegate.ReadByBarCode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Create a history reagent bottle records.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 12/06/2012</remarks>
        Public Function CreateReagentsBottle(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                                        ByVal phisReagentsBottlesDS As HisReagentsBottlesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If phisReagentsBottlesDS.thisReagentsBottles.Count > 0 Then
                            Dim myHistoryReagentBottles As New thisReagentBottlesDAO
                            myGlobalDataTO = myHistoryReagentBottles.Create(dbConnection, phisReagentsBottlesDS)
                        End If
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisReagentBottlesDelegate.CreateReagentsBottle", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return (myGlobalDataTO)

        End Function

#End Region



    End Class

End Namespace

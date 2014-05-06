Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
'Imports System.Data.SqlClient


Namespace Biosystems.Ax00.BL

    Public Class OrdersDemographicsDelegate

        ''' <summary>
        ''' Add values for one or more customized Order Demographics to an Order
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pCustomDemographics">DataSet containing values of the customized 
        '''                                   Order Demographics to add</param>
        ''' <returns>Global Object containing error information or the added data</returns>
        ''' <remarks></remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pCustomDemographics As OrderDemographicsDS) As GlobalDataTO
            Dim result As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                If (IsNothing(pDBConnection)) Then
                    'A local Database Connection is opened
                    dbConnection.ConnectionString = DAOBase.GetConnectionString
                    dbConnection.Open()

                    'A local Database Transaction is opened
                    DAOBase.BeginTransaction(dbConnection)
                Else
                    'The opened Database Connection is used
                    dbConnection = pDBConnection
                End If
            Catch ex As Exception
                'If a Database Connection was locally opened, then it is closed
                If (IsNothing(pDBConnection)) Then
                    DAOBase.RollbackTransaction(dbConnection)
                    dbConnection.Close()
                End If

                result.HasError = True
                result.ErrorCode = "DB_CONNECTION_ERROR"
                result.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrdersDemographicsDelegate.Create", EventLogEntryType.Error, False)
            End Try

            Try
                Dim myOrdersDemographics As New twksOrderDemographicsDAO
                result = myOrdersDemographics.Create(pDBConnection, pCustomDemographics)    'This is not from the GeneratedDAO

                If (Not result.HasError) Then
                    'If no error and a Database Connection was locally opened, the Commit is executed
                    If (IsNothing(pDBConnection)) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'If error and a Database Connection was locally opened, the Rollback is executed
                    If (IsNothing(pDBConnection)) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'If error and a Database Connection was locally opened, the Rollback is executed
                If (IsNothing(pDBConnection)) Then DAOBase.RollbackTransaction(dbConnection)

                result.HasError = True
                result.ErrorCode = "SYSTEM_ERROR"
                result.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrdersDemographicsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                'If a Database Connection was locally opened, then it is closed
                If (IsNothing(pDBConnection)) Then dbConnection.Close()
            End Try

            Return result
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderDemographicsDAO

                        resultData = myDAO.ResetWS(dbConnection)

                        If (Not resultData.HasError) Then
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrdersDemographicsDelegate.ResetWS", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

    End Class

End Namespace



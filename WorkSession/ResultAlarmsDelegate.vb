Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class ResultAlarmsDelegate

#Region "Other Methods"
        ''' <summary>
        ''' Get all Result Alarms that exist for the specified OrderTestID, RerunNumber and MultipointNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pMultiPointNumber">Multipoint Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultAlarmsDS with the list of Alarms</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                             ByVal pRerunNumber As Integer, ByVal pMultiPointNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultAlarms As New twksResultAlarmsDAO
                        resultData = myResultAlarms.Read(dbConnection, pOrderTestID, pRerunNumber, pMultiPointNumber)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultAlarmsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a group of Result Alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open BD Connection</param>
        ''' <param name="pResultAlarms">Typed DataSet ResultAlarmsDS with the list of Result Alarms to add</param>
        ''' <returns>GlobalDataTO containing the same entry DataSet</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultAlarms As ResultAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultAlarms As New twksResultAlarmsDAO
                        resultData = myResultAlarms.Add(dbConnection, pResultAlarms)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultAlarmsDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Alarms for the specified OrderTestID, RerunNumber, MultiPointNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pMultiPointNumber">Multipoint Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 08/06/2010 
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                  ByVal pRerunNumber As Integer, ByVal pMultiPointNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultAlarmsDAO
                        resultData = myDAO.DeleteAll(dbConnection, pOrderTestID, pRerunNumber, pMultiPointNumber)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultAlarmsDelegate.DeleteAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When the active WorkSession is reset, delete all result alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 04/07/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultAlarmsDAO
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultAlarmsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Result Alarms that exist for all Order Tests belonging to the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 25/01/2011
        ''' </remarks>
        Public Function DeleteResultAlarmsByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultAlarmsDAO
                        resultData = myDAO.DeleteResultAlarmsByOrderID(dbConnection, pOrderID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultAlarmsDelegate.DeleteResultAlarmsByOrderID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete  Result Alarms related to the informed Order Test Id
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function DeleteResultAlarmsByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultAlarmsDAO
                        resultData = myDAO.DeleteResultAlarmsByOrderTestID(dbConnection, pOrderTestID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultAlarmsDelegate.DeleteResultAlarmsByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "TO REVIEW"
        ''' <summary>
        ''' Delete a result alarm of the specified Order Test, rerun number and multipoint number
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pResultAlarms">DS with only one datarow specifier of the order test, rerun number and multipoint number </param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReturnAlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultAlarms As ResultAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultAlarms As New twksResultAlarmsDAO
                        resultData = myResultAlarms.Delete(dbConnection, pResultAlarms)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultAlarmsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region


    End Class

End Namespace


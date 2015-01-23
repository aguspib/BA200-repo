Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Public Class WSOrderTestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Adds a group of Order Tests to a Work Session, and update ToSendFlag and OpenOTFlag for a group of Order Tests
        ''' that are already linked to the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSOrderTestsList">List of Order Tests that have to be added to the Work Session</param>
        ''' <param name="pNewWorkSession">When True, it indicates a new Work Session is being created and all informed
        '''                               Order Tests have to be linked to it</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/01/2010 - Changed the way of open the DB Transaction to fulfill the new template
        '''              SA 26/04/2010 - Before creation, verify if the Order Test has been already linked to the WS, and in this
        '''                              case, if value of field OpenOTFlag has changed, update its value (which means the Order
        '''                              Test has been sent to positioning)
        '''              SA 01/09/2010 - When the Order Test has been already linked to the WS, update also value of field ToSendFlag,
        '''                              not only the OpenOTFlag; name of previous called method UpdateOpenOTFlag changed to UpdateWSOTFlags
        '''              SA 17/02/2011 - New parameter to indicate when the function was called for a new WorkSession
        '''              TR 19/07/2012 - Validate the CtrlsSendingGroup to update if informed on the UpdateWSOTFlags.
        '''              SA 24/02/2014 - BT #1528 ==> Added changes to improve performance of function AddWorkSession. Updates are prepared 
        '''                                           in a DS and sent later in blocks instead of one by one
        ''' </remarks>
        Public Function AddOrderTestsToWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSOrderTestsList As WSOrderTestsDS, _
                                          ByVal pNewWorkSession As Boolean) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pWSOrderTestsList.twksWSOrderTests.Rows.Count > 0) Then
                            Dim wsOrderTestData As New twksWSOrderTestsDAO

                            If (pNewWorkSession) Then
                                'New WorkSession, link all Order Tests to it 
                                dataToReturn = wsOrderTestData.Create(dbConnection, pWSOrderTestsList)
                            Else
                                'Updated WorkSession, verify which Order Tests are new and which have to be updated
                                Dim wsOTsToCreateDS As New WSOrderTestsDS
                                Dim wsOTsToUpdateDS As New WSOrderTestsDS

                                For Each wsOTRow As WSOrderTestsDS.twksWSOrderTestsRow In pWSOrderTestsList.twksWSOrderTests
                                    dataToReturn = wsOrderTestData.Read(dbConnection, wsOTRow.WorkSessionID, wsOTRow.OrderTestID)
                                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        Dim myWSOrderTests As WSOrderTestsDS = DirectCast(dataToReturn.SetDatos, WSOrderTestsDS)

                                        If (myWSOrderTests.twksWSOrderTests.Rows.Count = 0) Then
                                            'Add the WSOrderTest to the DS containing the list of new OTs to link to the WS
                                            wsOTsToCreateDS.twksWSOrderTests.ImportRow(wsOTRow)
                                        Else
                                            'BT #1528 - Add the WSOrderTest to the DS containing the list of OTs already linked 
                                            '           to the WS that have to be updated
                                            wsOTsToUpdateDS.twksWSOrderTests.ImportRow(wsOTRow)
                                        End If
                                    End If
                                Next

                                If (Not dataToReturn.HasError) Then
                                    'Create new records...
                                    If (wsOTsToCreateDS.twksWSOrderTests.Rows.Count > 0) Then
                                        dataToReturn = wsOrderTestData.Create(dbConnection, wsOTsToCreateDS)
                                    End If

                                    If (Not dataToReturn.HasError) Then
                                        'BT #1528 - Update old records in block...
                                        If (wsOTsToUpdateDS.twksWSOrderTests.Rows.Count > 0) Then
                                            dataToReturn = wsOrderTestData.UpdateWSOTFlags(dbConnection, wsOTsToUpdateDS)
                                        End If
                                    End If
                                End If
                            End If

                            If (Not dataToReturn.HasError) Then
                                'If the Connection was opened locally, then it is confirmed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'If the Connection was opened locally, then it is undone
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.AddOrderTestsToWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Counts the number of Order Tests with status different of CLOSED in the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOFFSOrderTest">Indicate if the order test belong to an off system test</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests in the specified Order that are still not closed</returns>
        ''' <remarks>
        ''' Created by:  SG 14/06/2010
        ''' Modified by: SA 01/09/2010 - Changed declaration of myWSOrderTestsDAO from twksOrderTestsDAO to twksWSOrderTestsDAO
        '''              SA 18/02/2011 - Removed parameter pOrderTestID; added new parameter pOrderID.
        '''                              Changed the returned type from DS to an integer value
        '''              TR 28/06/2013 - Add parameter POFFSOrderTest Validate if ordertest belong to and off system test to avoid the ToSendFlag and openOTFlag Filter.
        ''' </remarks>
        Public Function CountClosedOTsByOrder(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                              ByVal pAnalyzerID As String, ByVal pOrderID As String, Optional pOFFSOrderTest As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        dataToReturn = myWSOrderTestsDAO.CountClosedOTsByOrder(dbConnection, pWorkSessionID, pAnalyzerID, pOrderID, pOFFSOrderTest)
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.CountClosedOTsByOrder", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' Count the number of Controls or patients with LISRequest field to True
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID"></param> 
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 07/05/2013
        ''' </remarks>
        Public Function CountLISRequestActive(ByVal pDBConnection As SqlClient.SqlConnection, _
                                              ByVal pAnalyzerID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksOrderTests As New twksWSOrderTestsDAO
                        resultData = mytwksOrderTests.CountLISRequestActive(dbConnection, pAnalyzerID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.CountLISRequestActive", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Counts the number of Order Tests with status different of CLOSED in the 
        ''' specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests
        '''          in the specified Work Session that are still not closed</returns>
        ''' <remarks>
        ''' Created by:  SG 14/06/2010
        ''' Modified by: SA 01/09/2010 - Changed declaration of myWSOrderTestsDAO from twksOrderTestsDAO to
        '''                              twksWSOrderTestsDAO
        '''               TR 28/06/2013 - Add parameter POFFSOrderTest Validate if ordertest belong to and off system test to avoid the ToSendFlag and openOTFlag Filter.
        ''' </remarks>
        Public Function CountClosedOTsByWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                              ByVal pAnalyzerID As String, Optional pOFFSOrderTest As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        dataToReturn = myWSOrderTestsDAO.CountClosedOTsByWS(dbConnection, pWorkSessionID, pAnalyzerID, pOFFSOrderTest)
                    End If
                End If

            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.CountClosedOTsByWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Count the number of Order Tests in the specified WorkSession and related with the specified required Reagent, that have been 
        ''' sent to positioning but for which the Executions have not been still created (this can happen only when new Standard or ISE 
        ''' Tests are requested in the WorkSession and sent to positioning, but button Create Executions in Rotor Positioning Screen has 
        ''' not been still pressed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests without Executions</returns>
        ''' <remarks>
        ''' Created by:  SA 27/02/2012
        ''' Modified by: SA 06/03/2012 - Added parameter for the ElementID
        ''' </remarks>
        Public Function CountOTsWithoutExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.CountOTsWithoutExecutions(dbConnection, pWorkSessionID, pElementID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.CountOTsWithoutExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count patient order test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the total number of Patient Samples requested in the active WS.</returns>
        ''' <remarks>
        ''' Created by:  DL 01/09/2011
        ''' </remarks>
        Public Function CountPatientOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) AndAlso (Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsOrderTestsData As New twksWSOrderTestsDAO
                        dataToReturn = wsOrderTestsData.CountPatientOrderTests(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.CountPatientOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Count the number of OffSystem Tests requested in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of OffSystem Tests requested 
        '''          in the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 18/01/2011
        ''' </remarks>
        Public Function CountWSOffSystemTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myvwksWSOrderTestsDAO As New vwksWSOrderTestsDAO
                        resultData = myvwksWSOrderTestsDAO.CountWSOffSystemTests(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.CountWSOffSystemTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Order Tests linked to the specified WorkSession and that belong to the specified PATIENT Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 08/06/2010
        ''' </remarks>
        Public Function DeleteByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pOrderID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestDAO As New twksWSOrderTestsDAO()
                        resultData = myWSOrderTestDAO.DeleteByOrderID(dbConnection, pWorkSessionID, pOrderID)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.DeleteByOrderID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the informed Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Identifier</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function DeleteByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestDAO As New twksWSOrderTestsDAO()
                        resultData = myWSOrderTestDAO.DeleteByOrderTestID(dbConnection, pOrderTestID)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.DeleteByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified WorkSession, delete all OPEN Order Tests that are not included in the 
        ''' list of informed Order Test IDs 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test IDs that should remain in the WorkSession</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 26/04/2010
        ''' </remarks>
        Public Function DeleteNotInList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pOrderID As String, ByVal pOrderTestsList As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestDAO As New twksWSOrderTestsDAO()
                        resultData = myWSOrderTestDAO.DeleteNotInList(dbConnection, pWorkSessionID, pOrderID, pOrderTestsList)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.DeleteNotInList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Order Tests with status OPEN linked to the specified WorkSession and belonging to Orders 
        ''' of the specified Sample Class
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 25/02/2010
        ''' </remarks>
        Public Function DeleteWSOpenOTsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                     ByVal pSampleClass As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestDAO As New twksWSOrderTestsDAO()
                        resultData = myWSOrderTestDAO.DeleteWSOpenOTsBySampleClass(dbConnection, pWorkSessionID, pSampleClass)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.DeleteWSOpenOTsBySampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if for the informed PatientID/SampleType (or OrderID/SampleType when the patient is unknown) there is 
        ''' at least an Order Test that corresponds to an Stat Order included in the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">Patient Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCountNotPositioned">Optional parameter to indicate if the Stat Patient Samples to count are the ones corresponding
        '''                                   to OrderTestIDs still not sent to positioning in the Analyzer Rotor (default value) or the ones 
        '''                                   already sent</param>
        ''' <returns>True if there is an Stat Order Test requested for the Patient in the Work Session; otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: AG 23/11/2009 - Adapt code to template (Tested: OK 26/11/2009)
        '''              SA 11/01/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''                              Function moved to "Other Methods" Region, and Region "For testing-Provisional" was removed
        '''              SA 09/03/2010 - Added parameter for the SampleID
        '''              SA 14/02/2011 - Added optional parameter pCountNotPositioned to indicate if the Stat Patient Samples to count are
        '''                              the ones corresponding to OrderTestIDs still not sent to positioning in the Analyzer Rotor (default
        '''                              value) or the ones already sent
        ''' </remarks>
        Public Function ExistStatPatientSampleInWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   ByVal pPatientID As String, ByVal pSampleID As String, ByVal pOrderID As String, _
                                                   ByVal pSampleType As String, Optional ByVal pCountNotPositioned As Boolean = True) _
                                                   As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) AndAlso (Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsOrderTestsData As New twksWSOrderTestsDAO
                        dataToReturn = wsOrderTestsData.ExistStatPatientSampleInWS(dbConnection, pWorkSessionID, pPatientID, pSampleID, _
                                                                                   pOrderID, pSampleType, pCountNotPositioned)
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.ExistStatPatientSampleInWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' For the informed PatientID/SampleID and SampleType, search all requested Order Tests that have to be positioned (ToSendFlag = TRUE) 
        ''' but that have not been selected yet for it (OpenOTFlag = TRUE)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">Patient or Sample Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>Global object containing a typed DataSet WSOrderTestsDS with the list of Order Tests requested for the specified Patient 
        '''          and SampleType and having OpenOTFlag = TRUE (they have been not selected to send to positioning)</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2013
        ''' Modified by: SA 17/04/2013 - Deleted parameter pPatientExists
        ''' </remarks>
        Public Function GetByPatientAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pPatientID As String, _
                                                  ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Order Tests requested for the specified PatientID/SampleID and SampleType in the active WS and that have not been selected for positioning yet
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.GetByPatientAndSampleType(dbConnection, pWorkSessionID, pPatientID, pSampleType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myWSOrderTestsDS As WSOrderTestsDS = DirectCast(resultData.SetDatos, WSOrderTestsDS)

                            'Select all Order Tests corresponding to STD Tests 
                            Dim myAuxWSOrderTestsDS As New WSOrderTestsDS
                            Dim lstSTDOrderTests As List(Of WSOrderTestsDS.twksWSOrderTestsRow) = (From a As WSOrderTestsDS.twksWSOrderTestsRow In myWSOrderTestsDS.twksWSOrderTests _
                                                                                                  Where a.TestType = "STD" _
                                                                                                 Select a).ToList()

                            'Search for each STD Test/Sample Type, the Order Tests of the needed Blank and Calibrator if they have not been selected for positioning yet
                            For Each otRow As WSOrderTestsDS.twksWSOrderTestsRow In lstSTDOrderTests
                                resultData = myWSOrderTestsDAO.GetBlankAndCalibByTestAndSampleType(dbConnection, pWorkSessionID, otRow.TestID, pSampleType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myAuxWSOrderTestsDS = DirectCast(resultData.SetDatos, WSOrderTestsDS)

                                    'Add Blank and/or Calibrators to the final DS to return
                                    For Each auxRow As WSOrderTestsDS.twksWSOrderTestsRow In myAuxWSOrderTestsDS.twksWSOrderTests
                                        myWSOrderTestsDS.twksWSOrderTests.ImportRow(auxRow)
                                    Next
                                Else
                                    'Error
                                    Exit For
                                End If
                            Next

                            If (Not resultData.HasError) Then resultData.SetDatos = myWSOrderTestsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetByPatientAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer WorkSession, get the list of Tests/SampleTypes for which a Calibrator was requested 
        ''' but executed for a different SampleType (those having informed field AlternativeOrderTestID)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS TestSamplesDS with the list of TestID / SampleType / Alternative SampleType
        '''          in the current Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 23/10/2012
        ''' </remarks>
        Public Function GetCalibratorsWithAlternative(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVWSOrderTestsDAO As New vwksWSOrderTestsDAO
                        resultData = myVWSOrderTestsDAO.GetCalibratorsWithAlternative(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetCalibratorsWithAlternative", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified WorkSession and optionally, for the specified Order, get all OFF-SYSTEM Order Tests with status CLOSED 
        ''' (those having a result). Optionally, get only those OFF-SYSTEM Order Tests that are not included in the list of informed
        '''  OrderTestIDs 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderID">Order Identifier. Optional parameter</param>
        ''' <param name="pOrderTestsList">List of Order Test IDs that should remain in the Order. Optional parameter</param>
        ''' <returns>Global object containing a typed DataSet WSOrderTestsDS with the list of obtained OFF-SYSTEM OrderTests</returns>
        ''' <remarks>
        ''' Created by:  SA 20/01/2011
        ''' </remarks>
        Public Function GetClosedOffSystemOTs(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                              Optional ByVal pOrderID As String = "", Optional ByVal pOrderTestsList As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.GetClosedOffSystemOTs(dbConnection, pWorkSessionID, pOrderID, pOrderTestsList)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetClosedOffSystemOTs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer WorkSession get the maximum CtrlsSendingGroup created. If there is not any CtrlsSendingGroup
        ''' then it returns zero
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestID">Identifier of an Standard Test. Optional parameter</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing an integer value with the maximum CtrlsSendingGroup in the specified
        '''          Analyzer WorkSession. If there is not any CtrlsSendingGroup, then it contains zero</returns>
        ''' <remarks>
        ''' Created by:  TR 19/07/2012
        ''' Modified by: JC 14/06/2013 - Added optionals parameters for TestID and SampleTyp and pass them to the function in DAO class. 
        ''' </remarks>
        Public Function GetMaxCtrlsSendingGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional pTestID As Integer = 0, _
                                                Optional pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.GetMaxCtrlsSendingGroup(dbConnection, pWorkSessionID, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetMaxCtrlsSendingGroup", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For an OrderTestID corresponding to a Control requested in the specified WorkSession, get value of field
        ''' Control Sending Group for it 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with value of the Control Sending Group for the specified OrderTestID;
        '''          if the value it is not informed, then it contains zero
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 19/07/2012 
        ''' </remarks>
        Public Function GetControlSendingGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                               ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.Read(dbConnection, pWorkSessionID, pOrderTestID)

                        Dim myCtrlSendingGroupNumber As Integer = 0
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myWSOrderTestsDS As WSOrderTestsDS = DirectCast(resultData.SetDatos, WSOrderTestsDS)
                            If (myWSOrderTestsDS.twksWSOrderTests.Count > 0) Then
                                If (Not myWSOrderTestsDS.twksWSOrderTests(0).IsCtrlsSendingGroupNull) Then
                                    'If the field is informed, then its value is returned
                                    myCtrlSendingGroupNumber = myWSOrderTestsDS.twksWSOrderTests(0).CtrlsSendingGroup
                                End If
                            End If
                        End If
                        resultData.SetDatos = myCtrlSendingGroupNumber
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetControlSendingGroup", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all information needed to create the executions of all Order Tests to be executed in the 
        ''' informed Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsForExecutionsDS with all information of the 
        '''          Order Tests to be executed in the active Analyzer Work Session</returns>
        ''' <remarks>
        ''' Created by:  SA 10/02/2011
        ''' Modified by: AG/SA - For each patient get the minimum creation order and use it for all his tests (it will become ElementID in sort process)
        ''' </remarks>
        Public Function GetInfoOrderTestsForExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                       ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsOrderTestsData As New twksWSOrderTestsDAO
                        resultData = wsOrderTestsData.GetInfoOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myDS As OrderTestsForExecutionsDS = DirectCast(resultData.SetDatos, OrderTestsForExecutionsDS)

                            Dim myLinQ As List(Of String) = (From a As OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow In myDS.OrderTestsForExecutionsTable _
                                                            Where a.SampleClass = "PATIENT" _
                                                           Select a.OrderID Distinct).ToList

                            Dim myPatientInfoLinq As New List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)
                            For Each myRow As String In myLinQ
                                myPatientInfoLinq = (From a As OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow In myDS.OrderTestsForExecutionsTable _
                                                    Where a.OrderID = myRow _
                                                   Select a Order By a.CreationOrder Ascending).ToList

                                For index = 1 To myPatientInfoLinq.Count - 1
                                    myPatientInfoLinq(index).BeginEdit()
                                    myPatientInfoLinq(index).CreationOrder = myPatientInfoLinq(0).CreationOrder
                                    myPatientInfoLinq(index).EndEdit()
                                    myPatientInfoLinq(index).AcceptChanges()
                                Next
                            Next
                            myPatientInfoLinq = Nothing
                            myLinQ = Nothing

                            resultData.SetDatos = myDS
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetInfoOrderTestsForExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Grouped by OrderID and SampleType, get the total number of ISE preparations that fulfill following conditions
        ''' ** Status is PENDING or Status is INPROCESS but the Preparation has not been still generated 
        ''' ** Status is INPROCESS and the Preparation has been generated but not sent (SendingTime is not informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TimeEstimationDS</returns>
        ''' <remarks>
        ''' Created by: TR 01/06/2012
        ''' </remarks>
        Public Function GetNotSendISEPreparations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVWSOrderTestsDAO As New vwksWSOrderTestsDAO
                        myGlobalDataTO = myVWSOrderTestsDAO.GetNotSendISEPreparations(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetNotSendISEPreparations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Orders of the specified SampleClass currently linked to the specified WorkSession but that are not in the informed list of 
        ''' Orders that remains in the Work Session 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pListOfWSOrders">List of IDs of Orders included in the specified Work Session</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet with the list of Orders that have to be removed from
        '''          the WorkSession and physically deleted with all their Order Tests</returns>
        ''' <remarks>
        ''' Created by:  SA 08/06/2010
        ''' Modified by: SA 01/02/2012 - Added parameter for the SampleClass; function name changed to GetOrdersNotInWS due to now the 
        '''                              it is used for all different Sample Classes; changed the function template 
        ''' </remarks>
        Public Function GetOrdersNotInWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pListOfWSOrders As String, _
                                         ByVal pSampleClass As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsOrderTestsData As New twksWSOrderTestsDAO
                        dataToReturn = wsOrderTestsData.GetOrdersNotInWS(dbConnection, pWorkSessionID, pListOfWSOrders, pSampleClass)
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetOrdersNotInWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the list of Required Elements for each OrderTests included in the informed WorkSession and having the specified SampleClass
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pOrderTestID">Order Test Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSOrderTestsForExecutions with the list of Order Tests and required Elements by Order Tests
        '''          included in the WorkSession and corresponding to the informed Sample Class</returns>
        ''' <remarks>
        ''' Created by:  SA 20/04/2010
        ''' Modified by: DL 11/05/2010 - Added additional parameter to filter by an specific OrderTestID
        ''' </remarks>
        Public Function GetOrderTestsForExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pSampleClass As String, Optional ByVal pOrderTestID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsOrderTestsData As New twksWSOrderTestsDAO
                        resultData = wsOrderTestsData.GetOrderTestsForExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pSampleClass, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetOrderTestsForExecutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer WorkSession, get the list of Blanks and Calibrators that were not executed due
        ''' to a previous value was used
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS HisWSOrderTestsDS with the list of Blanks and Calibrators that were 
        '''          not executed due to a previous value was used</returns>
        ''' <remarks>
        ''' Created by:  SA 31/08/2012
        ''' Modified by: SA 19/10/2012 - Implementation changed
        '''              SA 23/10/2012 - Changed the type of the returned DS: from OrderTestsDS to HisWSOrderTestsDS. Code changed:
        '''                              just call the DAO function, it is not needed to get fields HistTestID and TestVersionNumber
        ''' </remarks>
        Public Function GetPreviousBlkCalibUsed(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myVWSOrderTestsDAO As New vwksWSOrderTestsDAO
                        resultData = myVWSOrderTestsDAO.GetPreviousBlkCalibUsed(dbConnection, pAnalyzerID, pWorkSessionID)

                        'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        '    Dim myOrderTestsDS As HisWSOrderTestsDS = DirectCast(resultData.SetDatos, HisWSOrderTestsDS)

                        '    Dim myHistWSOrderTestsDS As New HisWSOrderTestsDS
                        '    Dim myHisWSOrderTestsDelegate As New HisWSOrderTestsDelegate

                        '    For Each previousOT As HisWSOrderTestsDS.thisWSOrderTestsRow In myOrderTestsDS.thisWSOrderTests
                        '        resultData = myHisWSOrderTestsDelegate.Read(dbConnection, pAnalyzerID, previousOT.WorkSessionID, previousOT.HistOrderTestID)
                        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        '            myHistWSOrderTestsDS = DirectCast(resultData.SetDatos, HisWSOrderTestsDS)

                        '            If (myHistWSOrderTestsDS.thisWSOrderTests.Rows.Count > 0) Then
                        '                previousOT.BeginEdit()
                        '                previousOT.TestID = myHistWSOrderTestsDS.thisWSOrderTests.First.HistTestID
                        '                previousOT.TestVersionNumber = myHistWSOrderTestsDS.thisWSOrderTests.First.TestVersionNumber
                        '                previousOT.EndEdit()
                        '            End If
                        '        Else
                        '            Exit For
                        '        End If
                        '    Next

                        '    resultData.SetDatos = myOrderTestsDS
                        '    resultData.HasError = False
                        'End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetPreviousBlkCalibUsed", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the WorkSessionID and the AnalyzerID in which the specified Order Test is included
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test ID</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestWSAnalyzerDS with the obtained
        '''          WorkSessionID and AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  SG 14/06/2010
        ''' Modified by: SA 01/09/2010 - Changed declaration of myWSOrderTestsDAO from twksOrderTestsDAO to
        '''                              twksWSOrderTestsDAO
        ''' </remarks>
        Public Function GetWSAndAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        myGlobalDataTO = myWSOrderTestsDAO.GetWSAndAnalyzer(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetWSAndAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of OffSystem Tests requested in the specified WorkSession. Results informed for 
        ''' these OffSystem Tests are also returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsResultsDS with the list of OffSystem
        '''          Tests requested in the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 18/01/2011
        ''' </remarks>
        Public Function GetWSOffSystemTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myvwksWSOrderTestsDAO As New vwksWSOrderTestsDAO

                        resultData = myvwksWSOrderTestsDAO.GetWSOffSystemTests(dbConnection, pWorkSessionID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myOffSystemOTsDS As OffSystemTestsResultsDS = DirectCast(resultData.SetDatos, OffSystemTestsResultsDS)

                            'Get Icon Image byte array for Patient's priority 
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate

                            Dim bPatientStat As Byte() = Nothing
                            Dim bPatientNotStat As Byte() = Nothing
                            bPatientStat = preloadedDataConfig.GetIconImage("STATS")
                            bPatientNotStat = preloadedDataConfig.GetIconImage("ROUTINES")

                            'Complete information and return the final DS
                            Dim myFinalOffSystemOTsDS As New OffSystemTestsResultsDS
                            For Each reqOFFS As OffSystemTestsResultsDS.OffSystemTestsResultsRow In myOffSystemOTsDS.OffSystemTestsResults
                                Dim myOFFSTestResultRow As OffSystemTestsResultsDS.OffSystemTestsResultsRow
                                myOFFSTestResultRow = myFinalOffSystemOTsDS.OffSystemTestsResults.NewOffSystemTestsResultsRow

                                myOFFSTestResultRow.OrderTestID = reqOFFS.OrderTestID
                                myOFFSTestResultRow.SampleID = reqOFFS.SampleID

                                myOFFSTestResultRow.StatFlag = reqOFFS.StatFlag
                                If (reqOFFS.StatFlag) Then
                                    myOFFSTestResultRow.StatFlagIcon = bPatientStat
                                Else
                                    myOFFSTestResultRow.StatFlagIcon = bPatientNotStat
                                End If

                                myOFFSTestResultRow.SampleType = reqOFFS.SampleType
                                myOFFSTestResultRow.TestID = reqOFFS.TestID
                                myOFFSTestResultRow.TestName = reqOFFS.TestName
                                myOFFSTestResultRow.ResultType = reqOFFS.ResultType

                                If (reqOFFS.ResultType = "QUANTIVE") Then
                                    myOFFSTestResultRow.QuantitativeFlag = True
                                    myOFFSTestResultRow.Unit = reqOFFS.Unit
                                    myOFFSTestResultRow.AllowedDecimals = reqOFFS.AllowedDecimals
                                Else
                                    myOFFSTestResultRow.QuantitativeFlag = False
                                    myOFFSTestResultRow.Unit = ""
                                    myOFFSTestResultRow.AllowedDecimals = 0
                                End If

                                If (Not reqOFFS.IsResultValueNull) Then
                                    myOFFSTestResultRow.ResultValue = reqOFFS.ResultValue
                                ElseIf (Not reqOFFS("ManualResultText") Is System.DBNull.Value) Then
                                    myOFFSTestResultRow.ResultValue = reqOFFS("ManualResultText").ToString
                                Else
                                    myOFFSTestResultRow.SetResultValueNull()
                                End If

                                If (Not reqOFFS.IsActiveRangeTypeNull) Then
                                    myOFFSTestResultRow.ActiveRangeType = reqOFFS.ActiveRangeType
                                Else
                                    myOFFSTestResultRow.SetActiveRangeTypeNull()
                                End If
                                myFinalOffSystemOTsDS.OffSystemTestsResults.AddOffSystemTestsResultsRow(myOFFSTestResultRow)
                            Next
                            myFinalOffSystemOTsDS.AcceptChanges()

                            resultData.SetDatos = myFinalOffSystemOTsDS
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetWSOffSystemTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all requested Calibrators for a Tests/SampleTypes using an Alternative Calibrator (field AlternativeOrderTestID is informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of TestID/SampleType and AlternativeOrderTestID</returns>
        ''' <remarks>
        ''' Created by:  SA 17/02/2011
        ''' </remarks>
        Public Function ReadAllAlternativeOTs(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New vwksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.ReadAllAlternativeOTs(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.ReadAllAlternativeOTs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Patient Samples and Controls requested for the specified TestID and SampleType in the active Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestID">Standard Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ViewWSOrderTestsDS with the list of Patient Samples and Controls requested for 
        '''          the specified TestID and SampleType in the active Work Session</returns>
        ''' <remarks>
        ''' Created by:  SA 12/04/2012
        ''' </remarks>
        Public Function ReadPatientsAndCtrlsByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                                  ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New vwksWSOrderTestsDAO
                        resultData = myDAO.ReadPatientsAndCtrlsByTestIDAndSampleType(dbConnection, pWorkSessionID, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.ReadPatientsAndCtrlsByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Special Tests (with SampleType) requested in the Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of TestID/SampleType corresponding to Special Tests</returns>
        ''' <remarks>
        ''' Created by:  SA 17/02/2011
        ''' </remarks>
        Public Function ReadSpecialTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New vwksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.ReadSpecialTests(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.ReadSpecialTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSOrderTestsDAO
                        resultData = myDAO.ResetWS(dbConnection, pWorkSessionID)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For an specific OrderTesrtID corresponding to a CTRL requested in the WorkSession, update value of field CtrlsSendingGroup
        ''' when a new Rerun is requested 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pNextCtrlsSendingGroup">Value of CtrlsSendingGroup to update for the specified OrderTestID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 19/007/2012
        ''' </remarks>
        Public Function UpdateCtrlsSendingGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestID As Integer, _
                                                ByVal pNextCtrlsSendingGroup As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestDAO As New twksWSOrderTestsDAO()
                        resultData = myWSOrderTestDAO.UpdateCtrlsSendingGroup(dbConnection, pWorkSessionID, pOrderTestID, pNextCtrlsSendingGroup)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.UpdateCtrlsSendingGroup", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the Calibrator for the specified TestID/SampleType or the Blank for the specified TestID has to be executed 
        ''' in the WorkSession (ToSendFlag=1). ToSendFlag=0 means a previous result will be used, then when executions for Control 
        ''' or Patient Samples requested for the TestID/SampleType are created it is not needed verifying if the Calibrator is positioned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code: CALIB or BLANK</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code; for SampleClass=BLANK it is not needed to inform it</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if it is needed to verify if the Calibrator for the specified 
        '''          TestID/SampleType or the Blank for the specified TestID has been positioned</returns>
        ''' <remarks>
        ''' Created by:  SA 01/09/2010
        ''' </remarks>
        Public Function VerifyToSendFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                         ByVal pWorkSessionID As String, ByVal pSampleClass As String, ByVal pTestID As Integer, _
                                         ByVal pSampleType As String) _
                                         As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSOrderTestsDAO As New twksWSOrderTestsDAO
                        resultData = mytwksWSOrderTestsDAO.VerifyToSendFlag(dbConnection, pAnalyzerID, pWorkSessionID, pSampleClass, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.VerifyToSendFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the SampleTypes currently in use in the WS
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/04/2013</remarks>
        Public Function GetSampleTypesInWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSOrderTestsDAO As New twksWSOrderTestsDAO
                        resultData = myWSOrderTestsDAO.GetSampleTypesInWS(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.GetSampleTypesInWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO REVIEW - DELETE"
        'A new version of this function was created to improve performance of function AddWorkSession
        'Public Function AddOrderTestsToWS_OLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSOrderTestsList As WSOrderTestsDS, _
        '                                      ByVal pNewWorkSession As Boolean) As GlobalDataTO
        '    Dim dataToReturn As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                If (pWSOrderTestsList.twksWSOrderTests.Rows.Count > 0) Then
        '                    Dim wsOrderTestData As New twksWSOrderTestsDAO

        '                    If (pNewWorkSession) Then
        '                        'New WorkSession, link all Order Tests to it 
        '                        dataToReturn = wsOrderTestData.Create(dbConnection, pWSOrderTestsList)
        '                    Else
        '                        'Updated WorkSession, verify which Order Tests are new and which have to be updated
        '                        Dim wsOTsToCreateDS As New WSOrderTestsDS
        '                        For Each wsOTRow As WSOrderTestsDS.twksWSOrderTestsRow In pWSOrderTestsList.twksWSOrderTests
        '                            dataToReturn = wsOrderTestData.Read(dbConnection, wsOTRow.WorkSessionID, wsOTRow.OrderTestID)
        '                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
        '                                Dim myWSOrderTests As WSOrderTestsDS = DirectCast(dataToReturn.SetDatos, WSOrderTestsDS)

        '                                If (myWSOrderTests.twksWSOrderTests.Rows.Count = 0) Then
        '                                    'Add the WSOrderTest to the DS containing the list of new OTs to link to the WS
        '                                    wsOTsToCreateDS.twksWSOrderTests.ImportRow(wsOTRow)
        '                                Else
        '                                    'Validate the CtrlsSendingGroup to update if informed on the UpdateWSOTFlags
        '                                    If (wsOTRow.IsCtrlsSendingGroupNull) Then
        '                                        'Update values of flags OpenOTFlag and ToSendFlag
        '                                        dataToReturn = wsOrderTestData.UpdateWSOTFlags_OLD(dbConnection, wsOTRow.WorkSessionID, wsOTRow.OrderTestID, _
        '                                                                                           wsOTRow.OpenOTFlag, wsOTRow.ToSendFlag)
        '                                    Else
        '                                        'Update values of flags OpenOTFlag, ToSendFlag and CtrlsSendingGroup
        '                                        dataToReturn = wsOrderTestData.UpdateWSOTFlags_OLD(dbConnection, wsOTRow.WorkSessionID, wsOTRow.OrderTestID, _
        '                                                                                           wsOTRow.OpenOTFlag, wsOTRow.ToSendFlag, wsOTRow.CtrlsSendingGroup)
        '                                    End If
        '                                    If (dataToReturn.HasError) Then Exit For
        '                                End If
        '                            End If
        '                        Next

        '                        If (Not dataToReturn.HasError) Then
        '                            'Create new records...
        '                            If (wsOTsToCreateDS.twksWSOrderTests.Rows.Count > 0) Then
        '                                dataToReturn = wsOrderTestData.Create(dbConnection, wsOTsToCreateDS)
        '                            End If
        '                        End If
        '                    End If

        '                    If (Not dataToReturn.HasError) Then
        '                        'If the Connection was opened locally, then it is confirmed
        '                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    Else
        '                        'If the Connection was opened locally, then it is undone
        '                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        dataToReturn = New GlobalDataTO()
        '        dataToReturn.HasError = True
        '        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WSOrderTestsDelegate.AddOrderTestsToWS", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return dataToReturn
        'End Function
#End Region
    End Class
End Namespace

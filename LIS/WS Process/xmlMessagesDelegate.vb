'AG 13/03/2013 - Move from WorkSession project in order to avoid circular references

Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports System.Xml


Namespace Biosystems.Ax00.LISCommunications

    Partial Public Class xmlMessagesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Saves a new XML into twksXMLMessages table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <param name="pxmlDoc">XML Message</param>
        ''' <param name="pMsgStatus">Message Status</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: AG 27/02/2013
        ''' </remarks>
        Public Function AddMessage(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String, ByVal pXmlDoc As XmlDocument, _
                                   ByVal pMsgStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.Create(dbConnection, pMessageID, pXmlDoc, pMsgStatus)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.AddMessage", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified XML Message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Identifier of the Message to delete</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.Delete(dbConnection, pMessageID)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete All XML Messages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.DeleteAll(dbConnection)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.DeleteAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of the specified XML Message in table twksXmlMessages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <returns>GlobalDataTO containing an XMLMessageTO with all data of the specified Message</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.Read(dbConnection, pMessageID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all XML Messages in table twksXMLMessages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a list of XMLMessagesTO, which is an structure containing MessageID, Status and XML</returns>
        ''' <remarks>
        ''' Created by:  AG 27/02/2013
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.ReadAll(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all XML Messages in table twksXMLMessages having the informed status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatus">Status that should have the XML Messages to read</param>
        ''' <returns>GlobalDataTO containing a list of XMLMessagesTO, which is an structure containing MessageID, Status and XML</returns>
        ''' <remarks>
        ''' Created by:  AG 27/02/2013
        ''' </remarks>
        Public Function ReadByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.ReadByStatus(dbConnection, pStatus)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ReadByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete specified XML Messages depending on Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatus">Status that should have the Messages to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: XB 28/02/2013
        ''' </remarks>
        Public Function DeleteByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.DeleteByStatus(dbConnection, pStatus)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.DeleteByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the status of all XML Messages having the specified previous status 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreviousStatus">Previous Status</param>
        ''' <param name="pStatus">New Status</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: AG 27/03/2013
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, pPreviousStatus As String, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.UpdateStatus(dbConnection, pPreviousStatus, pStatus)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.UpdateStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        '''  Update the status of the specified Message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageId">Identifier of the Message to update</param>
        ''' <param name="pStatus">New status for the specified Message</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: AG 27/03/2013
        ''' </remarks>
        Public Function UpdateStatusByMessageId(ByVal pDBConnection As SqlClient.SqlConnection, pMessageId As String, ByVal pStatus As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksXmlMessagesDAO
                        resultData = myDAO.UpdateStatusByMessageId(dbConnection, pMessageId, pStatus)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.UpdateStatusByMessageId", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Extract and return the list of specimens included in the message
        ''' </summary>
        ''' <param name="pXMLDoc"></param>
        ''' <returns>GlobalDataTo (list of string)</returns>
        ''' <remarks>AG 23/07/2013</remarks>
        Public Function ExtractSpecimensFromMessage(ByVal pXMLDoc As XmlDocument) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim specimenExtracted As New List(Of String)
            Try
                'Verify if the XML Message can be processed
                If (pXMLDoc IsNot Nothing) Then
                    'The tag "error" is treated by means of a unique way for both protocols (HL7 & ASTM)
                    Dim myXmlHelper As xmlHelper
                    Dim ErrorsList As XmlNodeList

                    'Pending to solve STL errors with new driver 16/05/2013
                    myXmlHelper = New xmlHelper("udc", UDCSchema, "ci", ClinicalInfoSchema)
                    ErrorsList = myXmlHelper.QueryXmlNodeList(pXMLDoc, "udc:command/udc:body/udc:error")

                    If (ErrorsList IsNot Nothing AndAlso ErrorsList.Count = 0) Then
                        'For each tag Service search the specimen
                        Dim mySpecimenId As String = String.Empty
                        For Each myXmlNode As XmlNode In myXmlHelper.QueryXmlNodeList(pXMLDoc, "udc:command/udc:body/udc:message/ci:service")
                            mySpecimenId = String.Empty
                            mySpecimenId = myXmlHelper.QueryStringValue(myXmlNode, "ci:specimen/ci:id", String.Empty)
                            If (mySpecimenId.Length = 0) Then
                                'Invalid
                            Else
                                If Not specimenExtracted.Contains(mySpecimenId) Then
                                    specimenExtracted.Add(mySpecimenId)
                                End If
                            End If
                        Next
                    End If
                End If
                resultData.SetDatos = specimenExtracted

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ExtractSpecimensFromMessage", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


#End Region

#Region "Methods for processing Nodes in XML Messages"
        ''' <summary>
        ''' Cancels an AwosID previously requested by LIS when a request for cancel is received from LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAWOSID">Identifier of the AWOS to cancel</param>
        ''' <param name="pAnalyzerID">Current analyzer</param>
        ''' <param name="pWorkSessionID">Current worksession</param>
        ''' <returns>GlobalDataTO containing </returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' Modified by: TR 22/03/2013 - On some cases where Delete Order Tests LIS Info pass the optional parameter Update Lis Request value = True
        '''              SA 04/04/2013 - Open DB Transaction instead of a DB Connection; remove call to function OrderTestsDelegate.GetOrderTest, it is
        '''                              not needed, all fields required are returned by OrderTestsLISInfoDelegate.GetOrderTestInfoByAwosID; do not 
        '''                              pass the opened DB Transaction to functions used to query needed data
        '''              XB 04/04/2013 - Added for each OrderTestsLISInfo and repositioning open and commit/rollback of DB Transaction
        '''              SA 23/04/2013 - If the AwosID does not exist in the active WS, verify if it exists in the list of Order Tests of LIS Saved WS that
        '''                              have not been still processed and added to the active WS
        '''              AG 08/05/2013 - After LIS cancelation, update the test InUse field (this change requires adding of parameters AnalyzerID and WorkSessionID)
        '''              SA 09/05/2013 - Changes due to parameters of functions GetOrderTestsByWorkSession and UpdateInUseFlagOnLISCancellation have been changed; 
        '''                              AG code from 08/05/2013 moved inside the For/Next loop
        '''              AG 11/05/2013 - When the status of the Order Test to Cancel is PENDING, its Executions are marked as locked by LIS, but the Order Test
        '''                              is not deleted; in this case the IN USE flag of the corresponding Test should not be changed, due to it is still in the 
        '''                              active Work Session
        '''              TR 16/05/2013 - For OFF-SYSTEM Tests, call the new implementation of function DeleteByOrderTestID
        '''              SA 21/05/2013 - For ISE Tests, call the new implementation of function DeleteByOrderTestID
        '''             JCM 22/05/2013 - Added call to function CancelAWOSInLISSavedWS when the specified AwosID does not exist in the active WS (it is then searched
        '''                              in the list of created LIS Saved WS)
        ''' </remarks>
        Public Function CancelAWOSID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAWOSID As String, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'Verify if the informed AWOS ID exists in the active Work Session and get data of the related Order Test
                Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate

                resultData = myOrderTestsLISInfoDelegate.GetOrderTestInfoByAwosID(Nothing, pAWOSID)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myOrderTestLISInfoDS As OrderTestsLISInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                    If (myOrderTestLISInfoDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                        'XBC 04/04/2013 - Add For each + repositioning open and commit/rollback of DB Transaction
                        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                            If (Not dbConnection Is Nothing) Then
                                Dim checkInUseAndSavedWS As Boolean
                                Dim myOrdersDelegate As New OrdersDelegate
                                Dim myOrderTestsDelegate As New OrderTestsDelegate
                                Dim mySavedWSOrderTestDelegate As New SavedWSOrderTestsDelegate

                                'Same AWOS ID for more than one Order Test is possible only for internal Controls when the Test/SampleType has more than one
                                'active Control linked to it; for the rest of cases function GetOrderTestInfoByAwosID returns only one Order Test
                                For Each myLISInfoRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In myOrderTestLISInfoDS.twksOrderTestsLISInfo.Rows
                                    checkInUseAndSavedWS = True

                                    If (myLISInfoRow.TestType = "OFFS") Then
                                        'The related results are removed from twksResults
                                        If (myLISInfoRow.OrderTestStatus = "CLOSED") Then
                                            Dim myResultsDelegate As New ResultsDelegate
                                            resultData = myResultsDelegate.DeleteOFFSResultsByOrderTestID(dbConnection, myLISInfoRow.OrderTestID)
                                        End If

                                        'The related AwosId is removed from twksOrderTestLISInfo
                                        If (Not resultData.HasError) Then
                                            resultData = myOrderTestsLISInfoDelegate.Delete(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.RerunNumber)
                                        End If

                                        'Delete the related Order Test and also the Order if it does not have other Order Tests
                                        If (Not resultData.HasError) Then
                                            resultData = myOrderTestsDelegate.DeleteByOrderTestID(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.OrderID, _
                                                                                                  myLISInfoRow.SampleClass, myLISInfoRow.TestType, myLISInfoRow.TestID, _
                                                                                                  myLISInfoRow.SampleType)
                                        End If
                                    Else
                                        'For the rest of Test Types (STD, CALC, e ISE), the Cancel process depends on the Status of the Order Test
                                        Select Case (myLISInfoRow.OrderTestStatus)
                                            Case "OPEN"
                                                Select Case (myLISInfoRow.TestType)
                                                    Case "ISE"
                                                        'The related AwosId is removed from twksOrderTestLISInfo
                                                        resultData = myOrderTestsLISInfoDelegate.Delete(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.RerunNumber)

                                                        'Delete the related Order Test and also the Order if it does not have other Order Tests
                                                        If (Not resultData.HasError) Then
                                                            resultData = myOrderTestsDelegate.DeleteByOrderTestID(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.OrderID, _
                                                                                                                  myLISInfoRow.SampleClass, myLISInfoRow.TestType, myLISInfoRow.TestID, _
                                                                                                                  myLISInfoRow.SampleType)
                                                        End If

                                                    Case "STD"
                                                        'Verify if it is included in the formula of at least one requested Calculated Test
                                                        Dim myOrderCalculatedTestsDelegate As New OrderCalculatedTestsDelegate
                                                        resultData = myOrderCalculatedTestsDelegate.GetByOrderTestID(Nothing, myLISInfoRow.OrderTestID)

                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            Dim myOrderCalculatedTestsDS As OrderCalculatedTestsDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)

                                                            If (myOrderCalculatedTestsDS.twksOrderCalculatedTests.Rows.Count > 0) Then
                                                                'The related AwosId is removed from twksOrderTestLISInfo and Update Lis Request value
                                                                resultData = myOrderTestsLISInfoDelegate.Delete(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.RerunNumber, True)
                                                            Else
                                                                'The related AwosId is removed from twksOrderTestLISInfo
                                                                resultData = myOrderTestsLISInfoDelegate.Delete(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.RerunNumber)

                                                                'Delete the related Order Test and also the Order if it does not have other Order Tests
                                                                If (Not resultData.HasError) Then
                                                                    resultData = myOrderTestsDelegate.DeleteByOrderTestID(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.OrderID, _
                                                                                                                          myLISInfoRow.SampleClass, myLISInfoRow.TestType, myLISInfoRow.TestID, _
                                                                                                                          myLISInfoRow.SampleType)
                                                                End If
                                                            End If
                                                        End If

                                                    Case "CALC"
                                                        Dim myOrderTestDelegate As New OrderTestsDelegate()
                                                        resultData = myOrderTestDelegate.CancelAwosForOpenCalcTest(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.OrderID, _
                                                                                                                   myLISInfoRow.SampleClass, myLISInfoRow.TestID, myLISInfoRow.SampleType)
                                                End Select

                                            Case "PENDING"
                                                If (myLISInfoRow.TestType = "CALC") Then
                                                    'The related AwosId is removed from twksOrderTestLISInfo and LISRequest value is set to False
                                                    resultData = myOrderTestsLISInfoDelegate.Delete(dbConnection, myLISInfoRow.OrderTestID, myLISInfoRow.RerunNumber, True)
                                                Else
                                                    'Mark all Executions for the Order Test/Rerun Number as locked by LIS (ExecutionStatus=LOCKED + LockedByLIS=True) 
                                                    Dim myExecutionsDelegate As New ExecutionsDelegate
                                                    resultData = myExecutionsDelegate.LockExecutionsByLIS(dbConnection, myLISInfoRow.OrderTestID, "PREP_" & myLISInfoRow.TestType, _
                                                                                                          myLISInfoRow.RerunNumber)

                                                    'Do not update the IN USE flag because we have not removed  the OrderTest; Executions have been locked, but the Test is still IN USE!!
                                                    checkInUseAndSavedWS = False

                                                    'If the cancelled AWOS exists in one or more Saved WS created by Users, set to NULL all LIS fields 
                                                    If (Not resultData.HasError) Then
                                                        resultData = mySavedWSOrderTestDelegate.UpdateAsManualOrderTest(dbConnection, pAWOSID, myLISInfoRow.TestID, myLISInfoRow.TestType)
                                                    End If
                                                End If

                                            Case Else
                                                'The Order Test is CLOSED. Nothing to do in this case, it cannot be cancelled
                                                checkInUseAndSavedWS = False
                                        End Select

                                        'Update to FALSE the InUse field in those Tests deleted from the active WS
                                        If (Not resultData.HasError AndAlso checkInUseAndSavedWS) Then
                                            'Verify if in the active WS there are other Order Tests with the same TestType/TestID of the cancelled AWOS
                                            resultData = myOrderTestsDelegate.GetOrderTestsByWorkSession(dbConnection, pWorkSessionID, myLISInfoRow.TestType, myLISInfoRow.TestID)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                'If there are not other Order Tests for the same TestType/TestID, update to FALSE the INUSE flag  
                                                If (DirectCast(resultData.SetDatos, OrderTestsDS).twksOrderTests.Rows.Count = 0) Then
                                                    Dim wsDelegate As New WorkSessionsDelegate
                                                    resultData = wsDelegate.UpdateInUseFlagOnLISCancellation(dbConnection, myLISInfoRow.TestType, myLISInfoRow.TestID)
                                                End If
                                            End If

                                            'If the cancelled AWOS exists in one or more Saved WS created by Users, set to NULL all LIS fields 
                                            resultData = mySavedWSOrderTestDelegate.UpdateAsManualOrderTest(dbConnection, pAWOSID, myLISInfoRow.TestID, myLISInfoRow.TestType)
                                        End If
                                    End If
                                Next

                            End If

                            If (Not resultData.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    Else
                        'Verify if the informed AWOS ID exists in the list of created LIS Saved Work Sessions and in this case, execute the Cancel
                        Dim mySavedWSOrderTestsDelegate As New SavedWSOrderTestsDelegate
                        resultData = mySavedWSOrderTestsDelegate.CancelAWOSInLISSavedWS(Nothing, pAWOSID)
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.CancelAWOSID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Evaluates each Patient Node in pXmlDoc (just one by XML Message according last STL changes) and executes the proper business according the information received:
        ''' * If Patient Node could not be decoded, it is ignored
        ''' * If Patient Node was correctly decoded, it is added to the final PatientsDS to return and additionally:
        '''    ** If a Patient with the same identifier exists in tparPatients (the comparison is done with the LIS Patient ID in uppercase):
        '''       ** If Patient Demographics were informed by LIS (Name, Surname, Sex and/or DateOfBirth): 
        '''          *** Patient data is updated with the information received from LIS
        '''       ** ElseIf LIS sent only the Patient Identifier:
        '''          *** If the existing Patient is not IN USE in the active WS nor it is included in a LIS Saved WS, then it is deleted from tparPatients table and  
        '''              the received ID will be used as SampleID in the WS -- NOT IMPLEMENTED, DUE TO THE PROBLEM IN THE SCREEN OF HISTORIC RESULTS REVIEW (BugTracking
        '''              Number 952); IN CURRENT IMPLEMENTATION THE EXISTING PATIENT IS CONSIDERED THE SAME SENT BY LIS
        '''          *** ElseIf the existing Patient is IN USE in the active WS or it is included in a LIS Saved WS, it is considered the same Patient
        '''    ** ElseIf there is not a Patient in tparPatients with the same Identifier:
        '''       ** If Patient Demographics were informed by LIS (Name, Surname, Sex and/or DateOfBirth): 
        '''          *** Patient is created with the information received from LIS
        '''       ** ElseIf LIS sent only the Patient Identifier:
        '''          *** The received ID will be used as SampleID in the WS
        ''' Finally, a PatientsDS containing data of all Patient Nodes in the XML Message is returned
        ''' </summary>
        ''' <param name="pxmlDoc">LIS XML Message</param>
        ''' <param name="pChannelID">ES Channel Identifier</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <param name="pAnalyzerSN">Analyzer Serial Number</param>
        ''' <returns>GlobalDataTO containing a PatientsDS with data of all Patient Nodes in the XML Message</returns>
        ''' <remarks>
        ''' Created by:  AG 13/03/2013
        ''' Modified by: SA 02/04/2013 - Removed parameter for a DB Connection; changed call to function DecodeXMLPatientTag: an open DB Connection is not informed as parameter 
        '''              SA 10/05/2013 - Changed the function logic to implement all cases described in "summary" section
        '''              SA 21/05/2013 - Code for deleting existing Patients when LIS does not sent demographics for them has been commented due to BugTracking Number 952
        '''                              has not been solved yet.
        ''' </remarks>
        Public Function ProcessLISPatients(ByVal pXmlDoc As XmlDocument, ByVal pChannelID As String, ByVal pAnalyzerModel As String, ByVal pAnalyzerSN As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO  'Do not remove New declaration, is needed

            Try
                'Get all Patients currently saved in Patients table in DB
                Dim myPatientsDelegate As New PatientDelegate

                resultData = myPatientsDelegate.GetListWithFilters(Nothing, Nothing)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim allDbPatientsDS As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)

                    'Get all PATIENT Nodes in pXMLDocument
                    Dim myXmlHelper As xmlHelper = Nothing
                    myXmlHelper = New xmlHelper("udc", UDCSchema, "ci", ClinicalInfoSchema)

                    Dim auxDS As New PatientsDS
                    Dim patientToDBDS As New PatientsDS
                    Dim patientsToDeleteDS As New PatientsDS
                    Dim auxRow As PatientsDS.tparPatientsRow
                    Dim linqRes As List(Of PatientsDS.tparPatientsRow)
                    Dim mySavedWSOTsDelegate As New SavedWSOrderTestsDelegate
                    Dim xmlTranslator As New ESxmlTranslator(pChannelID, pAnalyzerModel, pAnalyzerSN)

                    Dim toReturnData As New PatientsDS
                    For Each myXmlNode As XmlNode In myXmlHelper.QueryXmlNodeList(pXmlDoc, "udc:command/udc:body/udc:message/ci:patient")
                        'Validate and decode all data informed in the tag
                        resultData = xmlTranslator.DecodeXMLPatientTag(myXmlNode)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            auxRow = DirectCast(resultData.SetDatos, PatientsDS.tparPatientsRow)

                            'If there is not a Patient Identifier, the Patient Node is ignored and the next Patient Node is processed
                            '(this case is possible when LIS sends requests for Internal Controls)
                            If (Not auxRow.IsPatientIDNull) Then
                                'Verify if the Patient already exists in the list of Patients saved in DB
                                linqRes = (From a As PatientsDS.tparPatientsRow In allDbPatientsDS.tparPatients _
                                          Where a.PatientID = auxRow.PatientID Select a).ToList

                                If (linqRes.Count = 0) Then
                                    '** THERE IS NOT A PATIENT WITH THE SAME ID IN TABLE tparPatients IN DB

                                    'Verify if PatientType = LIS, which means demographics were informed for the Patient
                                    If (Not auxRow.IsPatientTypeNull AndAlso auxRow.PatientType = "LIS") Then
                                        'Add data to an auxiliary PatientsDS
                                        patientToDBDS.Clear()
                                        patientToDBDS.tparPatients.ImportRow(auxRow)

                                        'Add the Patient to Patients table in DB
                                        resultData = myPatientsDelegate.Add(Nothing, patientToDBDS, True)

                                        'Add the new Patient to the local DS containing all Patients saved in DB
                                        If (Not resultData.HasError) Then
                                            allDbPatientsDS.tparPatients.ImportRow(auxRow)
                                            allDbPatientsDS.tparPatients.AcceptChanges()

                                            'Patient has been added to tparPatients table; set PatientType to DB and SampleID
                                            auxRow.PatientType = "DB"
                                        End If
                                    Else
                                        'Patient is not added to tparPatients table; set PatientType to MANUAL
                                        auxRow.PatientType = "MAN"
                                    End If
                                Else
                                    '** THERE IS A PATIENT WITH THE SAME ID IN TABLE tparPatients IN DB

                                    'Verify if PatientType = LIS, which means demographics were informed for the Patient
                                    If (Not auxRow.IsPatientTypeNull AndAlso auxRow.PatientType = "LIS") Then
                                        'Add data to an auxiliary PatientsDS
                                        patientToDBDS.Clear()
                                        patientToDBDS.tparPatients.ImportRow(auxRow)

                                        'Update patient data in Patients table in DB
                                        resultData = myPatientsDelegate.Modify(Nothing, patientToDBDS)

                                        'Patient exists in tparPatients table; set PatientType to DB
                                        auxRow.PatientType = "DB"
                                    Else
                                        'DO NOT DELETE; IT CAN BE IMPLEMENTED AFTER SOLVING BugTracking Number 952
                                        ''Verify is the Patient is IN USE in the active WS or if it is included in a LIS Saved Work Session pending to process
                                        'If (Not linqRes.First.IsInUseNull AndAlso Not linqRes.First.InUse) Then
                                        '    'The Patient is not IN USE in the active WorkSession; verify if it is included in in a LIS Saved WS pending to process
                                        '    resultData = mySavedWSOTsDelegate.CountBySampleID(Nothing, linqRes.First.PatientID)
                                        '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        '        If (Not CBool(resultData.SetDatos)) Then
                                        '            'The Patient is not IN USE in the active WorkSession nor in a LIS Saved WS pending to process; add it to the 
                                        '            'auxiliary DS containing Patients to delete
                                        '            patientsToDeleteDS.tparPatients.ImportRow(auxRow)

                                        '            'Patient will be deleted from tparPatients table; set PatientType to MANUAL
                                        '            auxRow.PatientType = "MAN"
                                        '        Else
                                        '            'Patient exists in tparPatients table; set PatientType to DB
                                        '            auxRow.PatientType = "DB"
                                        '        End If
                                        '    End If
                                        'Else
                                        '    'Patient exists in tparPatients table; set PatientType to DB
                                        '    auxRow.PatientType = "DB"
                                        'End If

                                        'Patient exists in tparPatients table; set PatientType to DB
                                        auxRow.PatientType = "DB"
                                    End If
                                End If
                            End If

                            'Add the Patient row to the final PatientsDS to return
                            toReturnData.tparPatients.ImportRow(auxRow)
                        Else
                            'Nothing to do: if there was an error decoding the Patient Node, it is ignored and the next Patient Node is processed
                        End If
                    Next
                    toReturnData.tparPatients.AcceptChanges()

                    'DO NOT DELETE; IT CAN BE IMPLEMENTED AFTER SOLVING BugTracking Number 952
                    'If (patientsToDeleteDS.tparPatients.Rows.Count > 0) Then
                    '    'Delete all Patients NOT IN USE for which LIS does not send demographics 
                    '    resultData = myPatientsDelegate.Delete(Nothing, patientsToDeleteDS)
                    'End If

                    'Finally, return the DS containing the content of all Patient Nodes in the XML Message
                    resultData.SetDatos = toReturnData
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ProcessLISPatients", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For each Patient Node correctly decoded, get all Order sub-Nodes contained in it and decode each one of them   
        ''' </summary>
        ''' <param name="pXmlDoc">LIS XML Message</param>
        ''' <param name="pPatientsDS">Typed DataSet PatientsDS containing data of all Patient Nodes in the XML that were correctly decoded</param>
        ''' <param name="pChannelID">ES Channel Identifier</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <param name="pAnalyzerSN">Analyzer Serial Number</param>
        ''' <returns>GlobalDataTO containing a PatientsDS with data of all Patient Order Nodes in the XML Message</returns>
        ''' <remarks>
        ''' Created by:  AG 13/03/2013
        ''' Modified by: SG 19/03/2013 - Decodes Orders included in Patients decoded previously
        '''              SA 02/04/2013 - Removed parameter for a DB Connection; for each valid Order, inform field PatientID in the DS to return with field ESPatientID;
        '''                              changed call to function DecodeXMLOrderTag: an open DB Connection is not informed as parameter   
        ''' </remarks>
        Public Function ProcessLISOrders(ByVal pXmlDoc As XmlDocument, ByVal pPatientsDS As PatientsDS, ByVal pChannelID As String, ByVal pAnalyzerModel As String, _
                                         ByVal pAnalyzerSN As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO  'Do not remove New declaration, is needed

            Try
                Dim myXmlHelper As xmlHelper = Nothing
                myXmlHelper = New xmlHelper("udc", UDCSchema, "ci", ClinicalInfoSchema)

                'Get the ESPatientID of all valid Patients contained in the XML Message
                Dim myAllPatientIDs As List(Of String) = (From p As PatientsDS.tparPatientsRow In pPatientsDS.tparPatients _
                                                          Select p.ExternalPID).ToList

                'From the XML, get all <patient> nodes in <body><message>
                Dim toReturnData As New OrdersDS
                Dim auxRow As OrdersDS.twksOrdersRow
                Dim xmlTranslator As New ESxmlTranslator(pChannelID, pAnalyzerModel, pAnalyzerSN)

                For Each myPatientNode As XmlNode In myXmlHelper.QueryXmlNodeList(pXmlDoc, "udc:command/udc:body/udc:message/ci:patient")
                    'Get the ID attribute in Patient node (ESPatientID)
                    Dim myESPatientID As String = myXmlHelper.QueryAttributeStringValue(myPatientNode, "id", "", String.Empty)
                    If (myESPatientID.Length > 0) Then
                        'Verify if the read ESPatientID is included in the list of valid Patients
                        If (myAllPatientIDs.Contains(myESPatientID)) Then
                            'Get all Order Nodes for the Patient 
                            For Each myOrderNode As XmlNode In myXmlHelper.QueryXmlNodeList(pXmlDoc, "udc:command/udc:body/udc:message/ci:patient/ci:order")
                                'Validate and decode all data informed in the Order tag
                                resultData = xmlTranslator.DecodeXMLOrderTag(myOrderNode)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    auxRow = CType(resultData.SetDatos, OrdersDS.twksOrdersRow)

                                    'Inform field PatientID in myOrderDS.Row with value of the ES Patient ID in process
                                    auxRow.BeginEdit()
                                    auxRow.PatientID = myESPatientID
                                    auxRow.EndEdit()

                                    'Finally add row to data to return variable
                                    toReturnData.twksOrders.ImportRow(auxRow)
                                End If
                            Next
                        End If
                    End If
                Next
                myAllPatientIDs = Nothing

                toReturnData.twksOrders.AcceptChanges()
                resultData.SetDatos = toReturnData
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ProcessLISOrders", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Service Nodes contained in the XML Message sent by LIS and decode and process each one of them depending the requested action (add or remove)
        ''' </summary>
        ''' <param name="pXmlDoc">LIS XML Message</param>
        ''' <param name="pPatientsDS">Typed DataSet PatientsDS containing data of all Patient Nodes in the XML that were correctly decoded</param>
        ''' <param name="pOrdersDS">Typed DataSet OrdersDS containing data of all Patient Order Nodes in the XML that were correctly decoded</param>
        ''' <param name="pRejectedOTLISInfoDS">Typed DataSet OrderTestsLISInfoDS used to return all rejected Service Nodes</param>
        ''' <param name="pListTestsMappingDS">Typed DataSet AllTestsByTypeDS containing all defined LIS Test Mappings</param>
        ''' <param name="pLISConfgMappingsDS">Typed DataSet LISMappingsDS containing all defined LIS Mappings for configuration values</param>
        ''' <param name="pDefaultPatientTube">Type of tube defined by default for Patients</param>
        ''' <param name="pDefaultControlTube">Type of tube defined by default for Controls</param>
        ''' <param name="pChannelID">ES Channel Identifier</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <param name="pAnalyzerSN">Analyzer Serial Number</param>
        ''' <param name="pMsgDateTime">Date and Time the Message was received from LIS</param>
        ''' <returns>GlobalDataTo (set data as integer (saved WS identifier))</returns>
        ''' <remarks>
        ''' Created by:  AG  15/03/2013
        ''' Modified by: SGM 19/03/2013 - Validate if there is data for the Order informed for the AWOS ID
        '''              SGM 25/03/2013 - Added pRejectedOTLISInfoDS parameter. Fill with rejected SavedWSOrderTestsDS
        '''              AG  27/03/2013 - Fixed system error when DecodeXmlService return TestID or TestName nulls
        '''              SA  02/04/2013 - Changed call to DecodeXMLServiceTag to remove the DB Connection parameter; removed all calls to function AddAwosRejected;
        '''                               when data in a Service Tag is rejected, added a row with MessageID, AwosID and ErrorCode to an ImportErrorsDS; activated call
        '''                               to function for cancel an Awos 
        '''              SA  04/04/2013 - Removed parameter for a DB Connection; changes due to function ValidationsForAddSTDorISEOrderTests: when SampleClass is CTRL and 
        '''                               the Test/SampleType is valid (it exists and has at least a linked Control with QC active), only one row is returned, and field 
        '''                               ControlID is not informed (it is not needed because the searching of the linked Controls is executed when the Saved Order Test 
        '''                               is added to the active WS)
        '''              SA  03/05/2013 - Call function SaveFromLIS in SavedWSDelegate when there is at least an Order Test in finalSavedDS
        '''              AG  06/05/2013 - Added NoInfo and LIS import errors treatment
        '''              AG  08/05/2013 - Added parameter pWorkSessionID, needed when calling function CancelAwosID
        '''              SA  10/05/2013 - When filling the SavedWSOrderTestsDS with data of each Service Node with SampleClass PATIENT, inform field LISPatientID with 
        '''                               value of the same field in pPatientsDS (the ID just as it was sent by LIS), and inform field SampleID with value of field 
        '''                               PatientID in pPatientsDS (the ID sent by LIS but in uppercase, excepting when the patient was not saved in tparPatients table,
        '''                               in which case, it has the same value than field LISPatientID)  
        '''              SA  14/05/2013 - Changed validations of Duplicated Request and Duplicated Specimen: instead using field ESPatientID to compare by Patient, use
        '''                               field SampleID if ExternalQC is FALSE and SpecimenID used as SampleID if ExternalQC is TRUE
        '''              SA  11/06/2013 - BA-1433 ==> When load data in an ImportErrorLogDS, inform field LineText as "MessageID | AwosID" instead of inform specific fields 
        '''                                           MessageID and AwosID in the DS (due to these fields have been deleted). Call function ImportErrorsLogDelegate.Add to 
        '''                                           save data loaded in the ImportErrorsLogDS in table twksImportErrorsLog
        '''              JC  27/06/2013 - BA-1205 ==> When the processed Service Tag contains a Patient request to add, if field LISPatientID is not informed for it, 
        '''                                           the AwosID is rejected. Patient Samples corresponding to an External QC are excluded from this validation
        '''              SA  19/12/2013 - BA-1433 ==> When field LISPatientID is not informed in the processed Service Tag, a row is added to an ImportErrorLogDS with 
        '''                                           error code LISPATIENTID_NOT_INFORMED
        '''              SA  17/02/2014 - BA-1510 ==> Code for adding rejected Order Tests to the entry DS pRejectedOTLISInfoDS has been commented to avoid the sending 
        '''                                           of rejecting messages to LIS and reduce message traffic. Exceptions: the rejections due to:
        '''                                           ** LIS_INVALID_FIELDNUM (a mandatory field is not informed in the message), returned by function DecodeXMLServiceTag
        '''                                           ** LIS_INVALID_TEST (the requested Test is not mapped), returned by function DecodeXMLServiceTag
        '''                                           ** LISPATIENTID_NOT_INFORMED (field LISPatientID is empty in the message) are still sent
        '''                                           ** LIS_DUPLICATE_SPECIMEN (there is already an Order Test for the same SampleClass and SpecimenID but different Patient) 
        '''                                           ** LIMS_INVALID_SAMPLETYPE (the Test/Sample Type does not exists in BAx00 or, for Controls, it exists but QC is not active 
        '''                                              or it is active but there are not active linked Controls), returned by function ApplyValidationsForAddOrderTests
        '''              SA  01/04/2014 - BA-1564 ==> Added new parameter for the Message Date and Time, and inform them when calling function SaveFromLIS in SavedWSDelegate
        '''              SA  29/01/2015 - BA-1610 ==> When function ApplyValidationsForAddOrderTests returns error INCOMPLETE_TESTSAMPLE (which means the Test/Sample Type has
        '''                                           been rejected due to its Calibration Programming in not complete), do not add a row in the DS of Rejected Order Tests, 
        '''                                           due to a Rejected Message has not to be sent to LIS in this case.  Instead, a row is added in the ApplicationLog informing 
        '''                                           the Test/SampleType and the error
        ''' </remarks>
        Public Function ProcessLISOrderTests(ByVal pXmlDoc As XmlDocument, ByVal pPatientsDS As PatientsDS, ByVal pOrdersDS As OrdersDS, ByRef pRejectedOTLISInfoDS As OrderTestsLISInfoDS, _
                                             ByVal pListTestsMappingDS As AllTestsByTypeDS, ByVal pLISConfgMappingsDS As LISMappingsDS, ByVal pDefaultPatientTube As String, _
                                             ByVal pDefaultControlTube As String, ByVal pChannelID As String, ByVal pAnalyzerModel As String, ByVal pAnalyzerSN As String, _
                                             ByVal pWorkSessionID As String, ByVal pMsgDateTime As Date) As GlobalDataTO
            Dim resultData As New GlobalDataTO  'Do not remove New declaration, it is needed

            Try
                Dim myXmlHelper As xmlHelper = Nothing
                myXmlHelper = New xmlHelper("udc", UDCSchema, "ci", ClinicalInfoSchema)

                'Get the Message ID
                Dim myMessageID As String = String.Empty
                For Each myXmlNode As XmlNode In myXmlHelper.QueryXmlNodeList(pXmlDoc, "udc:command/udc:header")
                    myMessageID = myXmlHelper.QueryStringValue(myXmlNode, "udc:id", "")
                    If (myMessageID.Length > 0) Then
                        Exit For
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                        Exit Try
                    End If
                Next

                'DS with processed data
                Dim lisImportErrorsDS As New ImportErrorsLogDS
                Dim notCancelledAwosDS As New SavedWSOrderTestsDS
                Dim finalSavedDS As New SavedWSOrderTestsDS
                Dim addedSavedDS As New SavedWSOrderTestsDS

                'Auxliary variables and lists
                Dim auxRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow
                Dim toChangeStatusSpecimenList As New List(Of String)
                Dim orderLinqRes As List(Of OrdersDS.twksOrdersRow)
                Dim patLinqRes As List(Of PatientsDS.tparPatientsRow)
                Dim savedWSLinqRes As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
                Dim myRejectedRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow

                Dim xmlTranslator As New ESxmlTranslator(pChannelID, pAnalyzerModel, pAnalyzerSN)
                For Each myXmlNode As XmlNode In myXmlHelper.QueryXmlNodeList(pXmlDoc, "udc:command/udc:body/udc:message/ci:service")
                    'Validate and Decode all data informed in the Service Node
                    resultData = xmlTranslator.DecodeXMLServiceTag(myXmlNode, pLISConfgMappingsDS, pListTestsMappingDS)

                    Dim validNode As Boolean = True
                    If (Not resultData.SetDatos Is Nothing) Then
                        auxRow = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)

                        If (resultData.HasError) Then
                            If (resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NO_INFO_AVAILABLE.ToString) Then
                                'Add Specimen to the string list containing the SpecimenIDs for LISStatus change 
                                AddSpecimenStatusChange(auxRow, toChangeStatusSpecimenList)
                            Else
                                validNode = False
                            End If
                        Else
                            'Verify: AwosID to Add or Cancel
                            Dim myAction As String = myXmlHelper.QueryStringValue(myXmlNode, "ci:test/ci:action", "")
                            If (myAction.Length > 0) Then
                                If (myAction = "add") Then
                                    'Search if the pair mySavedWSOrderTestDS.Row.ESOrderID and mySavedWSOrderTestDS.Row.ESPatientID exist 
                                    'in pOrdersDS (search by.ExternalOID and PatientID respectively)
                                    orderLinqRes = (From a As OrdersDS.twksOrdersRow In pOrdersDS.twksOrders _
                                                   Where a.ExternalOID = auxRow.ESOrderID _
                                                 AndAlso a.PatientID = auxRow.ESPatientID _
                                                  Select a).ToList

                                    If (orderLinqRes.Count = 0) Then
                                        validNode = False
                                    Else
                                        'If LIS Order ID is informed in OrdersDS, inform it in the SavedWSOrderTestsDS row
                                        If (Not orderLinqRes(0).IsOrderIDNull) Then auxRow.LISOrderID = orderLinqRes(0).OrderID Else auxRow.SetLISOrderIDNull()

                                        If (auxRow.SampleClass = "PATIENT") Then
                                            'Inform the default Tube Type used for Patient Samples
                                            auxRow.TubeType = pDefaultPatientTube

                                            'Validate if there is data for the Patient informed for the AWOS ID
                                            patLinqRes = (From a As PatientsDS.tparPatientsRow In pPatientsDS.tparPatients _
                                                         Where a.ExternalPID = auxRow.ESPatientID Select a).ToList

                                            If (patLinqRes.Count = 0) Then
                                                validNode = False
                                            Else
                                                If (auxRow.ExternalQC) Then
                                                    'The AwosID is an External QC; there is not a Patient for it and the SpecimenID is used as SampleID
                                                    auxRow.SetLISPatientIDNull()
                                                    auxRow.SampleID = auxRow.SpecimenID
                                                    auxRow.PatientIDType = "MAN"
                                                Else
                                                    'Reject the Service Tag if it is not an External QC and field LIS Patient ID is not informed
                                                    If (String.IsNullOrEmpty(patLinqRes(0).LISPatientID)) Then
                                                        validNode = False
                                                        resultData.ErrorCode = "LISPATIENTID_NOT_INFORMED" 'SA 19/12/2013 - BT #1433
                                                    Else
                                                        'The AwosID is a Patient; there is a Patient for it, and the LIS Patient ID is used as SampleID
                                                        auxRow.LISPatientID = patLinqRes(0).LISPatientID
                                                        auxRow.PatientIDType = patLinqRes(0).PatientType

                                                        If (patLinqRes(0).PatientType = "DB") Then
                                                            'The PatientID sent by LIS will be used as it is saved in tparPatients: all characters in uppercase
                                                            auxRow.SampleID = patLinqRes(0).PatientID
                                                        Else
                                                            'The PatientID sent by LIS will be used "as is", with the same case
                                                            auxRow.SampleID = patLinqRes(0).LISPatientID
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Else
                                            'The AwosID in an internal Control; inform the default Tube Type used for Controls
                                            auxRow.TubeType = pDefaultControlTube
                                        End If

                                        If (validNode) Then
                                            'Apply validations for ADD and get the rest of  needed data
                                            resultData = ApplyValidationsForAddOrderTests(Nothing, auxRow)

                                            If (resultData.HasError) Then
                                                validNode = False

                                            ElseIf (Not resultData.SetDatos Is Nothing) Then
                                                Dim validationDS As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                                                If (validationDS.tparSavedWSOrderTests.Rows.Count > 0) Then
                                                    'Verify if in finalSavedWSOrderTestsDS if there is already an Order Test for the same SampleClass, Patient, TestType, 
                                                    'TestID and SampleType
                                                    savedWSLinqRes = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In finalSavedDS.tparSavedWSOrderTests _
                                                                     Where a.SampleClass = validationDS.tparSavedWSOrderTests(0).SampleClass _
                                                                   AndAlso a.TestType = validationDS.tparSavedWSOrderTests(0).TestType _
                                                                   AndAlso a.TestID = validationDS.tparSavedWSOrderTests(0).TestID _
                                                                   AndAlso a.SampleType = validationDS.tparSavedWSOrderTests(0).SampleType _
                                                                   AndAlso ((a.ExternalQC = False And a.SampleID = validationDS.tparSavedWSOrderTests(0).SampleID) _
                                                                    OrElse (a.ExternalQC = True And a.SampleID = validationDS.tparSavedWSOrderTests(0).SpecimenID)) _
                                                                    Select a).ToList

                                                    If (savedWSLinqRes.Count > 0) Then
                                                        validNode = False
                                                        resultData.ErrorCode = "DUPLICATED_REQUEST"
                                                    Else
                                                        'Verify if in finalSavedWSOrderTestsDS if there is already an Order Test for the same SampleClass and SpecimenID 
                                                        'but different Patient
                                                        savedWSLinqRes = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In finalSavedDS.tparSavedWSOrderTests _
                                                                         Where a.SampleClass = validationDS.tparSavedWSOrderTests(0).SampleClass _
                                                                       AndAlso ((a.ExternalQC = False And a.SampleID <> validationDS.tparSavedWSOrderTests(0).SampleID) _
                                                                        OrElse (a.ExternalQC = True And a.SampleID <> validationDS.tparSavedWSOrderTests(0).SpecimenID)) _
                                                                       AndAlso a.SpecimenID = validationDS.tparSavedWSOrderTests(0).SpecimenID
                                                                        Select a).ToList

                                                        If (savedWSLinqRes.Count > 0) Then
                                                            validNode = False
                                                            resultData.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATE_SPECIMEN.ToString()
                                                        Else
                                                            'Inform the Number of Replicates assigned by default for the Test/SampleType
                                                            auxRow.ReplicatesNumber = validationDS.tparSavedWSOrderTests(0).ReplicatesNumber

                                                            If (auxRow.TestType = "CALC") Then
                                                                'Inform the Formula of the Calculated Test and move the row to the final SavedWSOrderTestsDS
                                                                auxRow.FormulaText = validationDS.tparSavedWSOrderTests(0).FormulaText
                                                                finalSavedDS.tparSavedWSOrderTests.ImportRow(auxRow)

                                                                'All Tests not requested by LIS, but added because they are included in the Formula 
                                                                'of a Calculated Test requested by LIS are moved to a temporary SavedWSOrderTestsDS and validated later
                                                                For i As Integer = 1 To validationDS.tparSavedWSOrderTests.Rows.Count - 1
                                                                    addedSavedDS.tparSavedWSOrderTests.ImportRow(validationDS.tparSavedWSOrderTests(i))
                                                                Next
                                                            Else
                                                                'For STD, ISE and/or OFFS Tests, just move the row to the final SavedWSOrderTestsDS
                                                                finalSavedDS.tparSavedWSOrderTests.ImportRow(auxRow)
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If

                                ElseIf (myAction = "remove") Then
                                    'Apply additional validations for CANCEL the AwosID
                                    resultData = CancelAWOSID(Nothing, auxRow.AwosID, pAnalyzerSN, pWorkSessionID)
                                    If (resultData.HasError AndAlso resultData.ErrorCode = Messages.LIS_AWOS_NOT_IN_WS.ToString) Then
                                        'Move the row to the local DS for not accepted Cancels
                                        notCancelledAwosDS.tparSavedWSOrderTests.ImportRow(auxRow)
                                        resultData.HasError = False 'SGM 23/04/2013 - reset error for further operations (SaveFromLIS)
                                    End If
                                Else
                                    validNode = False
                                    resultData.ErrorCode = "INVALID_LIS_ACTION"
                                End If
                            End If
                        End If

                        If (Not validNode AndAlso resultData.ErrorCode <> String.Empty) Then
                            'BA-1610: When a Test/Sample Type has been rejected due to its Calibration Programming in not complete, it is not 
                            '         added to the DS of Rejected Order Tests, due to a Rejected Message has not to be sent to LIS in this case.
                            '         Instead, a row is added in the ApplicationLog informing the Test/SampleType and the error
                            If (resultData.ErrorCode = GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString) Then
                                'Add Error, TestName and SampleType to the Application Log
                                Dim logTrace = resultData.ErrorCode & ": " & auxRow.TestName & " [" & auxRow.SampleType & "]"
                                GlobalBase.CreateLogActivity(logTrace, "xmlMessagesDelegate.ProcessLISOrderTests", EventLogEntryType.Information, False)
                            Else
                                'A Rejection Message is not sent for duplicated requests
                                If (resultData.ErrorCode <> "DUPLICATED_REQUEST") Then
                                    'For the rest of Error cases, add data in the Service Node to a row in the DS needed to send a Rejected Delayed Message
                                    myRejectedRow = pRejectedOTLISInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                                    With myRejectedRow
                                        .BeginEdit()
                                        .AwosID = auxRow.AwosID
                                        .SpecimenID = auxRow.SpecimenID
                                        .SampleClass = auxRow.LISSampleClass
                                        .StatFlagText = auxRow.LISStatFlag
                                        .SampleType = auxRow.LISSampleType
                                        .TestIDString = auxRow.LISTestID
                                        .ESPatientID = auxRow.ESPatientID
                                        .ESOrderID = auxRow.ESOrderID
                                        .CheckLISValues = False
                                        .EndEdit()
                                    End With

                                    pRejectedOTLISInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRejectedRow)
                                    pRejectedOTLISInfoDS.twksOrderTestsLISInfo.AcceptChanges()
                                End If

                                'Add MessageID, AwosID and ErrorCode to the local ImportErrorsDS used to write the XML file for Rejected Awos
                                Dim myLISRow As ImportErrorsLogDS.twksImportErrorsLogRow
                                myLISRow = lisImportErrorsDS.twksImportErrorsLog.NewtwksImportErrorsLogRow

                                myLISRow.BeginEdit()
                                myLISRow.ErrorCode = resultData.ErrorCode
                                myLISRow.LineText = myMessageID & " | " & auxRow.AwosID
                                myLISRow.EndEdit()

                                lisImportErrorsDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myLISRow)
                                lisImportErrorsDS.twksImportErrorsLog.AcceptChanges()
                            End If
                        End If
                    End If

                    'Accept changes in all the used DataSets
                    finalSavedDS.tparSavedWSOrderTests.AcceptChanges()
                    addedSavedDS.tparSavedWSOrderTests.AcceptChanges()
                    notCancelledAwosDS.tparSavedWSOrderTests.AcceptChanges()
                Next

                'Special process for AWOS that could not be cancelled due to they have not been included in the active WS yet. 
                'They have to be searched in the list of AWOS to add 
                For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In notCancelledAwosDS.tparSavedWSOrderTests
                    'Verify if the AwosID exists in finalSavedDS
                    savedWSLinqRes = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In finalSavedDS.tparSavedWSOrderTests _
                                     Where a.AwosID = row.AwosID Select a).ToList

                    If (savedWSLinqRes.Count > 0) Then
                        Dim removeFromFinalFlag As Boolean = False

                        If (savedWSLinqRes(0).TestType = "STD") Then
                            If (savedWSLinqRes(0).IsCalcTestIDsNull) Then removeFromFinalFlag = True

                        ElseIf (savedWSLinqRes(0).TestType = "CALC") Then
                            'Search in addedSavedWSOrderTestsDS all Order Tests linked to the Calculated Test
                            savedWSLinqRes = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In addedSavedDS.tparSavedWSOrderTests _
                                             Where a.SampleClass = row.SampleClass _
                                           AndAlso a.ESPatientID = row.ESPatientID _
                                           AndAlso a.CalcTestIDs = row.TestID.ToString _
                                            Select a).ToList

                            For Each item As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In savedWSLinqRes
                                'Delete all records found from addedSavedWSOrderTestsDS
                                addedSavedDS.tparSavedWSOrderTests.RemovetparSavedWSOrderTestsRow(item)
                            Next
                        Else
                            removeFromFinalFlag = True
                        End If

                        If (removeFromFinalFlag) Then
                            'Remove the awos from finalSavedDS
                            finalSavedDS.tparSavedWSOrderTests.RemovetparSavedWSOrderTestsRow(savedWSLinqRes(0))
                        End If
                    End If

                    'Accept changes
                    finalSavedDS.tparSavedWSOrderTests.AcceptChanges()
                    addedSavedDS.tparSavedWSOrderTests.AcceptChanges()
                Next

                'Special process for Calculated Tests: validation for Tests added because they are in the Formula of requested Calculated Tests 
                If (addedSavedDS.tparSavedWSOrderTests.Rows.Count > 0) Then
                    resultData = ValidateDuplicateRequest(finalSavedDS, addedSavedDS)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        finalSavedDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)
                    End If
                End If

                'Create the internal Saved WorkSession
                If (finalSavedDS.tparSavedWSOrderTests.Rows.Count > 0) Then
                    'BT #1564 - Inform also the Message Date and Time as function parameter
                    Dim saveWSDlg As New SavedWSDelegate
                    resultData = saveWSDlg.SaveFromLIS(Nothing, myMessageID, finalSavedDS, pMsgDateTime) 'Do not use current connection, use a new transaction
                End If

                If (lisImportErrorsDS.twksImportErrorsLog.Rows.Count > 0) Then
                    'Write the content of DataSet LISImportErrorDS to table twksImportErrorsLog 
                    'SA 16/12/2013 - BT #1433 - Save the rejection causes into database
                    Dim myImportErrorLog As New ImportErrorsLogDelegate
                    resultData = myImportErrorLog.Add(Nothing, lisImportErrorsDS)
                End If

                'Verify for each SpecimenID in the list if it exists in table of Barcode Positions with no request and in this case, update LISStatus = NOINFO
                If (toChangeStatusSpecimenList.Count > 0) Then
                    Dim notInUseDlg As New BarcodePositionsWithNoRequestsDelegate
                    resultData = notInUseDlg.UpdateLISStatus(Nothing, toChangeStatusSpecimenList, "NOINFO") 'Do not use current connection, use a new transaction
                End If

                'Release lists
                toChangeStatusSpecimenList = Nothing
                orderLinqRes = Nothing
                patLinqRes = Nothing
                savedWSLinqRes = Nothing
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ProcessLISOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Process XML Messages received from LIS and saved in the temporary buffer (table twksXMLMessages) 
        ''' </summary>
        ''' <param name="pChannel">ES Channel Identifier</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <param name="pAnalyzerSN">Analyzer Serial Number</param>
        ''' <returns>GlobalDataTO containing an OrderTestsLISInfoDS with the list of rejected Awos</returns>
        ''' <remarks>
        ''' Created by:  SGM 21/03/2013
        ''' Modified by: SA  26/03/2013 - Changed the code to remove DB Transaction opened in this function. Each XML will open and close its own DB Tran
        '''                               Fixed an error when searching the LIS Value for the Test Identifier of rejected Awos: value is an string that have
        '''                               to be assigned to field TestIDString in the DS 
        '''              AG  27/03/2013 - Fix Issue SA 26/03/2013 call DAO update with no connection - Solution is create a method in delegate and call 
        '''                               this method in delegate instead of call DAO method
        '''                             - Fix Issue config lis mapping call the wrong delegate
        '''                             - Fix Issue if validate error all messages were changed his status not only the wrong message
        '''              SA  04/04/2013 - Set the DB Connection of all called functions to Nothing; removed the DB Connection parameter
        '''              AG  08/05/2013 - Added parameter pWorkSessionID, needed in the call to function ValidateXmlMessage
        '''              SA  11/06/2013 - If there are new XML messages to process, delete current content of table twksImportErrorsLog
        '''              SA  04/07/2013 - If there are AWOS IDs that have been rejected, verify which of them correspond to Samples Barcodes with LIS Status = ASKING
        '''                               for which LIS did not send valid AWOS IDs. If this is the case, the LIS Status is changed to REJECTED to avoid the Barcode 
        '''                               Status remains permanently ASKING
        '''              SA  01/04/2014 - BT #1564 ==> Inform the Message Date and Time when calling function ValidateXmlMessage to allow identify old messages and to 
        '''                                            avoid manage them as LIS Rerun requests 
        ''' </remarks>
        Public Function ProcessXmlMessages(ByVal pChannel As String, ByVal pAnalyzerModel As String, ByVal pAnalyzerSN As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO  'Do not remove New declaration, is needed

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim TotalStartTime As DateTime = Now
                'Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'Update Status = INPROCESS for all XML Messages with Status = PENDING
                Dim myXmlMessagesEleDAO As New twksXmlMessagesDAO
                resultData = UpdateStatus(Nothing, "PENDING", "INPROCESS")

                If (Not resultData.HasError) Then
                    'Get all XML Messages with Status = INPROCESS
                    resultData = myXmlMessagesEleDAO.ReadByStatus(Nothing, "INPROCESS")
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myXMLMessages As List(Of XMLMessagesTO) = TryCast(resultData.SetDatos, List(Of XMLMessagesTO))

                        If (myXMLMessages.Count > 0) Then
                            'Delete current content of table twksImportErrorsLog
                            'SA 16/12/2013 - BT #1433 - Delete table before starting the process
                            Dim myImportErrorsLogDelegate As New ImportErrorsLogDelegate
                            resultData = myImportErrorsLogDelegate.DeleteAll(Nothing)

                            'Get all defined LIS Test Mappings
                            Dim testMappingDS As AllTestsByTypeDS
                            Dim myLISTestMappingsDelegate As New AllTestByTypeDelegate

                            resultData = myLISTestMappingsDelegate.ReadAll(Nothing)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                testMappingDS = DirectCast(resultData.SetDatos, AllTestsByTypeDS)

                                'Get the rest of defined LIS Mappings
                                Dim confMappingDS As LISMappingsDS
                                Dim myLISMappingsDelegate As New LISMappingsDelegate

                                resultData = myLISMappingsDelegate.ReadAll(Nothing)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    confMappingDS = DirectCast(resultData.SetDatos, LISMappingsDS)

                                    'Get default Tube Type for Patient Samples and Controls
                                    Dim myUsersSettingsDelegate As New UserSettingsDelegate

                                    '...for Patient Samples
                                    Dim myPatientTube As String = "T13"
                                    resultData = myUsersSettingsDelegate.GetDefaultSampleTube(Nothing, "PATIENT")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPatientTube = TryCast(resultData.SetDatos, String)
                                    End If

                                    '...for Controls
                                    Dim myControlTube As String = "PED"
                                    resultData = myUsersSettingsDelegate.GetDefaultSampleTube(Nothing, "CTRL")
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myControlTube = TryCast(resultData.SetDatos, String)
                                    End If

                                    'Process for each XML Message in the list of XMLMessagesTO
                                    Dim myRejectedDS As New OrderTestsLISInfoDS
                                    Dim MessageStartTime As DateTime = Now

                                    For Each M As XMLMessagesTO In myXMLMessages
                                        'BT #1564 - Inform also the Message Date and Time from the XMLMessageTO as function parameter
                                        resultData = ValidateXmlMessage(M.XMLMessage, myRejectedDS, pChannel, pAnalyzerModel, pAnalyzerSN, testMappingDS, confMappingDS, _
                                                                        myPatientTube, myControlTube, pWorkSessionID, M.MsgDateTime)

                                        If (resultData.HasError) Then
                                            'Mark the message as ERROR
                                            resultData = UpdateStatusByMessageId(Nothing, M.MessageID, "ERROR")
                                        End If

                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        GlobalBase.CreateLogActivity("Process of MessageID " & M.MessageID & " = " & Now.Subtract(MessageStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                        "XMLMessagesDelegate.ProcessXMLMessages", EventLogEntryType.Information, False)
                                        MessageStartTime = Now
                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    Next

                                    'Delete all "INPROCESS" Messages
                                    resultData = DeleteByStatus(Nothing, "INPROCESS")

                                    'Only if there are rejected Order Tests... 
                                    Dim qRejectedAwos As New List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                                    If (myRejectedDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                        'Get all Samples Barcodes with LIS Status = ASKING (a Host Query was sent to LIS for them) for which LIS has not
                                        'sent valid Order Tests (the Barcode does not exist in the LIS Saved Work Sessions that have been created)
                                        Dim myBCToUpdate As New List(Of String)
                                        Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate

                                        resultData = myBCPosWithNoRequestDelegate.ReadAskingBCNotSentByLIS(Nothing)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myBCPosWithNoRequestDS As BarcodePositionsWithNoRequestsDS = DirectCast(resultData.SetDatos, BarcodePositionsWithNoRequestsDS)

                                            For Each barcode As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests
                                                'Verify if the Barcode is one of the SpecimenIDs with Order Tests sent by LIS that have been rejected
                                                qRejectedAwos = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In myRejectedDS.twksOrderTestsLISInfo _
                                                                Where a.SpecimenID = barcode.BarCodeInfo.Trim _
                                                               Select a).ToList

                                                If (qRejectedAwos.Count > 0) Then
                                                    'Add the Barcode to the list of Barcodes to change the LIS Status from ASKING to PENDING...
                                                    myBCToUpdate.Add(barcode.BarCodeInfo.Trim)
                                                End If
                                            Next

                                            'If there are Barcodes for which the LIS Status has to be updated...
                                            If (myBCToUpdate.Count > 0) Then
                                                resultData = myBCPosWithNoRequestDelegate.UpdateLISStatus(Nothing, myBCToUpdate, "REJECTED")
                                            End If
                                        End If

                                        'If there are rejected AwosID with CheckLISValues=True, search the LIS Value for the TestID and SampleType 
                                        qRejectedAwos = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In myRejectedDS.twksOrderTestsLISInfo _
                                                        Where a.CheckLISValues = True _
                                                       Select a).ToList

                                        Dim myMappedTestId As String = ""
                                        Dim myMappedSampleType As String = ""

                                        For Each R As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In qRejectedAwos
                                            'Get the LIS Value for the Test Identifier
                                            resultData = myLISTestMappingsDelegate.GetLISTestID(testMappingDS, R.TestID, R.TestType)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                R.TestIDString = CStr(resultData.SetDatos)
                                            End If

                                            'Get the LIS Value for the Sample Type Code
                                            resultData = myLISMappingsDelegate.GetLISSampleType(confMappingDS, R.SampleType)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                R.SampleType = CStr(resultData.SetDatos)
                                            End If
                                        Next
                                    End If

                                    'Finally, return the DS with the list of rejected Awos
                                    resultData = New GlobalDataTO
                                    resultData.SetDatos = myRejectedDS
                                End If
                            End If
                        Else
                            'Return an empty myRejectedDS
                            Dim myRejectedDS As New OrderTestsLISInfoDS
                            resultData.SetDatos = myRejectedDS
                        End If
                    End If
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "XMLMessagesDelegate.ProcessXMLMessages", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ProcessXmlMessages", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Private methods"
        ''' <summary>
        ''' Validations according the TestType for Order Tests requested by LIS to be added into current WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pEntryRow">Row of a SavedWSOrderTestsDS containing the data of the Order Test requested by LIS</param>
        ''' <returns>GlobalDataTo (SavedWSOrderTestsDS)</returns>
        ''' <remarks>
        ''' Created by: AG 13/03/2013
        ''' </remarks>
        Private Function ApplyValidationsForAddOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                          ByVal pEntryRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Apply validations according the Test Type of the Order Test requested by LIS
                        Select Case (pEntryRow.TestType)
                            Case "STD", "ISE"
                                resultData = ValidationsForAddSTDorISEOrderTests(dbConnection, pEntryRow)
                            Case "CALC"
                                resultData = ValidationsForAddCALCOrderTests(dbConnection, pEntryRow)
                            Case "OFFS"
                                resultData = ValidationsForAddOFFSOrderTests(dbConnection, pEntryRow)
                        End Select
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ApplyValidationsForAddOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Validations for STD or ISE Order Tests requested by LIS to be added into current WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pEntryRow">Row of a SavedWSOrderTestsDS containing the data of the Order Test requested by LIS</param>
        ''' <returns>GlobalDataTO containing a SavedWSOrderTestsDS with the entry row updated, or an error indicating the 
        '''          requested Test/SampleType is not valid</returns>
        ''' <remarks>
        ''' Created by:  AG 13/03/2013
        ''' Modified by: SA 04/04/2013 - When SampleClass = CTRL, update the ControlReplicates fields for the received SaveWSOrderTestsDS row, but do not inform
        '''                              field ControlID nor add additional rows if there are several Controls linked to the Test/SampleType
        '''              SA 28/01/2015 - BA-1610 ==> If the STD Test/Sample Type requested by LIS has EnableStatus = FALSE (which means the Calibration programming  
        '''                                          is incompleted for it), it is not added to the WorkSession and error INCOMPLETE_TESTSAMPLE is returned   
        ''' </remarks>
        Private Function ValidationsForAddSTDorISEOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                             ByVal pEntryRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim toReturnData As New SavedWSOrderTestsDS

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim withErrorFlag As Boolean = True 'Default value with errors
                        Dim incompleteCalibProg As Boolean = False

                        'PATIENT Request
                        If (pEntryRow.SampleClass = "PATIENT") Then
                            If (pEntryRow.TestType = "STD") Then
                                'Verify if the STD Test/SampleType exists, and in this case, get the number of replicates defined by the Test
                                Dim myTestDlg As New TestSamplesDelegate
                                resultData = myTestDlg.GetDefinition(dbConnection, pEntryRow.TestID, pEntryRow.SampleType, True)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim localTestSamplesDS As TestSamplesDS = DirectCast(resultData.SetDatos, TestSamplesDS)

                                    If (localTestSamplesDS.tparTestSamples.Rows.Count > 0) Then
                                        'BA-1610: When field EnableStatus for the STD Test/Sample Type is False, it means the Calibration programming is 
                                        '         incompleted for it and then the STD Test/Sample Type cannot be added to the active Work Session
                                        If (Not localTestSamplesDS.tparTestSamples.First.EnableStatus) Then
                                            incompleteCalibProg = True
                                        Else
                                            withErrorFlag = False

                                            pEntryRow.BeginEdit()
                                            pEntryRow.ReplicatesNumber = localTestSamplesDS.tparTestSamples(0).ReplicatesNumber
                                            pEntryRow.EndEdit()
                                        End If
                                    End If
                                End If

                            Else
                                'Verify if the ISE Test/SampleType exists, and in this case, set the number of replicates to 1
                                Dim myISETestDlg As New ISETestSamplesDelegate
                                resultData = myISETestDlg.GetListByISETestID(dbConnection, pEntryRow.TestID, pEntryRow.SampleType)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim localISETestSamplesDS As ISETestSamplesDS = DirectCast(resultData.SetDatos, ISETestSamplesDS)

                                    If (localISETestSamplesDS.tparISETestSamples.Rows.Count > 0) Then
                                        withErrorFlag = False

                                        pEntryRow.BeginEdit()
                                        pEntryRow.ReplicatesNumber = 1
                                        pEntryRow.EndEdit()
                                    End If
                                End If
                            End If

                            'CONTROL Request
                        ElseIf (pEntryRow.SampleClass = "CTRL") Then
                            withErrorFlag = True

                            'Verify if QC is active for the Test/SampleType and there is at least a Control linked to it; in this case, 
                            'get the number of Control replicates defined for the Test/SampleType
                            Dim myCtrlDlg As New TestControlsDelegate
                            resultData = myCtrlDlg.GetControlsNEW(dbConnection, pEntryRow.TestType, pEntryRow.TestID, pEntryRow.SampleType, 0, True)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim localCtrlDS As TestControlsDS = CType(resultData.SetDatos, TestControlsDS)

                                If (localCtrlDS.tparTestControls.Rows.Count > 0) Then
                                    'BA-1610: When field EnableStatus for the STD Test/Sample Type is False, it means the Calibration programming is 
                                    '         incompleted for it and then the STD Test/Sample Type cannot be added to the active Work Session. 
                                    '         For ISE Tests, field EnableStatus is returned always as TRUE
                                    If (Not localCtrlDS.tparTestControls.First.EnableStatus) Then
                                        incompleteCalibProg = True
                                    Else
                                        withErrorFlag = False

                                        pEntryRow.BeginEdit()
                                        pEntryRow.ReplicatesNumber = localCtrlDS.tparTestControls(0).ControlReplicates
                                        pEntryRow.EndEdit()
                                    End If
                                End If
                            End If
                        End If

                        If (Not withErrorFlag) Then
                            'Copy the updated entry row to the DS to return
                            toReturnData.tparSavedWSOrderTests.ImportRow(pEntryRow)
                            toReturnData.tparSavedWSOrderTests.AcceptChanges()
                        Else
                            'BA-1610: a different error code is returned when STD Test/Sample Type exists but its Calibration Programming is incomplete
                            resultData.HasError = True
                            If (Not incompleteCalibProg) Then
                                'If SampleClass = PATIENT, the requested Test/Sample Type does not exist in the application
                                'If SampleClass = CTRL, the requested Test/Sample Type does not exist in the application or it exits but QC is not active
                                resultData.ErrorCode = GlobalEnumerates.Messages.LIMS_INVALID_SAMPLETYPE.ToString
                            Else
                                'Calibration programming for the STD Test/Sample Type is not complete
                                resultData.ErrorCode = GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString
                            End If
                        End If

                        resultData.SetDatos = toReturnData
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ValidationsForAddSTDorISEOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Validations for OFF System Order Tests requested by LIS to be added into current WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pEntryRow">Row of a SavedWSOrderTestsDS containing the data of the Order Test requested by LIS</param>
        ''' <returns>GlobalDataTO containing a SavedWSOrderTestsDS with the entry row updated, or an error indicating the 
        '''          requested Test/SampleType is not valid</returns>
        ''' <remarks>
        ''' Created by: AG 13/03/2013
        ''' </remarks>
        Private Function ValidationsForAddOFFSOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                         ByVal pEntryRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim toReturnData As New SavedWSOrderTestsDS
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Check for required data informed
                        Dim withErrorFlag As Boolean = True 'Default value with errors
                        If Not pEntryRow.IsSampleClassNull AndAlso Not pEntryRow.IsSampleTypeNull AndAlso Not pEntryRow.IsTestIDNull Then
                            'PATIENTS
                            If pEntryRow.SampleClass = "PATIENT" Then

                                Dim myOffTestDlg As New OffSystemTestSamplesDelegate
                                resultData = myOffTestDlg.GetListByOffSystemTestID(dbConnection, pEntryRow.TestID, pEntryRow.SampleType)

                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    Dim localDS As New OffSystemTestSamplesDS
                                    localDS = CType(resultData.SetDatos, OffSystemTestSamplesDS)
                                    If localDS.tparOffSystemTestSamples.Rows.Count > 0 Then
                                        withErrorFlag = False
                                        pEntryRow.BeginEdit()
                                        pEntryRow.ReplicatesNumber = 1
                                        pEntryRow.EndEdit()
                                    End If
                                End If
                            End If
                        End If

                        If Not withErrorFlag Then
                            toReturnData.tparSavedWSOrderTests.ImportRow(pEntryRow)
                            toReturnData.tparSavedWSOrderTests.AcceptChanges()
                            resultData.SetDatos = toReturnData
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.LIMS_INVALID_SAMPLETYPE.ToString
                        End If

                    End If 'If (Not dbConnection Is Nothing) Then
                End If 'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ValidationsForAddOFFSOrderTests", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Validations for CALC Order Tests requested by LIS to be added into current WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pEntryRow">Row of a SavedWSOrderTestsDS containing the data of the Order Test requested by LIS</param>
        ''' <returns>GlobalDataTO containing a SavedWSOrderTestsDS with the calculated test and all tests contained in its Formula</returns>
        ''' <remarks>
        ''' Created by:  AG 13/03/2013
        ''' Modified by: SA 02/04/2013 - Fixed error: search of Formula members should be done when the SampleType is the same, not when it is different 
        '''                              Changed call to TestSamplesDelegate.GetDefinition for a call to TestsDelegate.Read: the pair of fields needed are
        '''                              both in tparTests table, not in tparTestSamples; when Tests contained in the Formula are added to a SavedWSOrderTestsDS
        '''                              row, fields inherited from the row received as parameter have to be explicity assigned one by one to avoid modification 
        '''                              of values of the entry parameter row
        '''              SA 23/05/2013 - Fixed error: when calling function Read in TestsDelegate, pass the ID of the Standard Test as parameter instead of the 
        '''                              ID of the Calculated Test
        '''              XB 02/12/2014 - BA-1867 ==> Added functionality to include ISE and OFFS into CALC Tests
        '''              SA 28/01/2015 - BA-1610 ==> Added changes to verify if at least one of the STD Tests/Sample Types included in the Formula of the specified 
        '''                                          Calculated Test has field EnableStatus = False (which means the Calibration programming is incompleted for it), 
        '''                                          and in this case, return error INCOMPLETE_TESTSAMPLE (the Calculated Test is not added to the active WorkSession) 
        ''' </remarks>
        Private Function ValidationsForAddCALCOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                         ByVal pEntryRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim toReturnData As New SavedWSOrderTestsDS

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Check for required data informed
                        Dim withErrorFlag As Boolean = True 'Default value with errors
                        Dim incompleteCalibProg As Boolean = False

                        If (Not pEntryRow.IsSampleClassNull AndAlso Not pEntryRow.IsTestIDNull AndAlso Not pEntryRow.IsSampleTypeNull) Then
                            If (pEntryRow.SampleClass = "PATIENT") Then
                                Dim myCalcDlg As New CalculatedTestsDelegate
                                resultData = myCalcDlg.GetCalcTest(dbConnection, pEntryRow.TestID)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim localDS As CalculatedTestsDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                                    If (localDS.tparCalculatedTests.Rows.Count > 0 AndAlso Not localDS.tparCalculatedTests(0).IsUniqueSampleTypeNull _
                                        AndAlso localDS.tparCalculatedTests(0).UniqueSampleType) Then

                                        If (Not localDS.tparCalculatedTests(0).IsSampleTypeNull AndAlso localDS.tparCalculatedTests(0).SampleType = pEntryRow.SampleType) Then
                                            Dim myFormulasDlg As New FormulasDelegate
                                            resultData = myFormulasDlg.GetTestsInFormula(dbConnection, pEntryRow.TestID, True)

                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                Dim localFormDS As FormulasDS = DirectCast(resultData.SetDatos, FormulasDS)

                                                If (localFormDS.tparFormulas.Rows.Count > 0) Then
                                                    'BA-1610: If at least one of the STD Tests/Sample Types included in the Formula of the specified Calculated Test has field
                                                    '         EnableStatus = False (which means the Calibration programming is incompleted for it), the Calculated Test cannot
                                                    '         be added to the active Work Session
                                                    If (localFormDS.tparFormulas.ToList.Where(Function(a) Not a.EnableStatus).Count > 0) Then
                                                        incompleteCalibProg = True
                                                    Else
                                                        'No ERROR, the Calculated Test and all its components can be added to the WorkSession!!!
                                                        withErrorFlag = False

                                                        'Edit the entry parameter and add row into structure to return
                                                        pEntryRow.BeginEdit()
                                                        pEntryRow.ReplicatesNumber = 1
                                                        pEntryRow.FormulaText = localDS.tparCalculatedTests(0).FormulaText
                                                        pEntryRow.EndEdit()
                                                        toReturnData.tparSavedWSOrderTests.ImportRow(pEntryRow)

                                                        Dim newRowToReturn As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow
                                                        For Each row As FormulasDS.tparFormulasRow In localFormDS.tparFormulas.Rows
                                                            'Customize the entry parameter for each element in dataset and add into structure to return
                                                            newRowToReturn = toReturnData.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow

                                                            'Fields which values have to be obtained from the FormulasDS
                                                            newRowToReturn.TestType = row.TestType
                                                            newRowToReturn.TestID = CInt(row.Value)
                                                            newRowToReturn.SampleType = row.SampleType
                                                            newRowToReturn.CalcTestIDs = row.CalcTestID.ToString
                                                            newRowToReturn.CalcTestNames = row.TestName

                                                            'Fields which value have to be set to NULL
                                                            newRowToReturn.SetAwosIDNull()

                                                            'Fields whose value have to be get from the received SavedWSOrderTestsDS row
                                                            newRowToReturn.SampleClass = pEntryRow.SampleClass
                                                            newRowToReturn.StatFlag = pEntryRow.StatFlag
                                                            newRowToReturn.ExternalQC = pEntryRow.ExternalQC
                                                            newRowToReturn.SampleID = pEntryRow.SampleID
                                                            newRowToReturn.PatientIDType = pEntryRow.PatientIDType
                                                            newRowToReturn.SpecimenID = pEntryRow.SpecimenID
                                                            newRowToReturn.ESPatientID = pEntryRow.ESPatientID
                                                            newRowToReturn.LISPatientID = pEntryRow.LISPatientID
                                                            newRowToReturn.ESOrderID = pEntryRow.ESOrderID
                                                            If (Not pEntryRow.IsLISOrderIDNull) Then newRowToReturn.LISOrderID = pEntryRow.LISOrderID
                                                            newRowToReturn.TubeType = pEntryRow.TubeType

                                                            'Fields which value have to be search in DB
                                                            If (newRowToReturn.TestType = "CALC") Then
                                                                Dim linqRes As List(Of FormulasDS.tparFormulasRow)
                                                                linqRes = (From a As FormulasDS.tparFormulasRow In localFormDS.tparFormulas _
                                                                           Where a.CalcTestID = newRowToReturn.TestID Select a).ToList

                                                                If (linqRes.Count > 0) Then
                                                                    newRowToReturn.TestName = linqRes(0).TestName
                                                                    newRowToReturn.FormulaText = linqRes(0).FormulaText
                                                                    newRowToReturn.ReplicatesNumber = 1
                                                                End If
                                                                linqRes = Nothing

                                                                ' XB 02/12/2014 - BA-1867
                                                                'Else
                                                                '    Dim myTests As New TestsDelegate
                                                                '    resultData = myTests.Read(Nothing, newRowToReturn.TestID)

                                                                '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                '        Dim myTestsDS As TestsDS = DirectCast(resultData.SetDatos, TestsDS)

                                                                '        If (myTestsDS.tparTests.Rows.Count > 0) Then
                                                                '            newRowToReturn.TestName = myTestsDS.tparTests(0).TestName
                                                                '            newRowToReturn.ReplicatesNumber = myTestsDS.tparTests(0).ReplicatesNumber
                                                                '            newRowToReturn.SetFormulaTextNull()
                                                                '        End If
                                                                '    End If
                                                                'End If

                                                            ElseIf (newRowToReturn.TestType = "STD") Then
                                                                Dim myTests As New TestsDelegate
                                                                resultData = myTests.Read(Nothing, newRowToReturn.TestID)

                                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                    Dim myTestsDS As TestsDS = DirectCast(resultData.SetDatos, TestsDS)

                                                                    If (myTestsDS.tparTests.Rows.Count > 0) Then
                                                                        newRowToReturn.TestName = myTestsDS.tparTests(0).TestName
                                                                        newRowToReturn.ReplicatesNumber = myTestsDS.tparTests(0).ReplicatesNumber
                                                                        newRowToReturn.SetFormulaTextNull()
                                                                    End If
                                                                End If
                                                            ElseIf (newRowToReturn.TestType = "ISE") Then
                                                                Dim myISETests As New ISETestsDelegate
                                                                resultData = myISETests.Read(Nothing, newRowToReturn.TestID)

                                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                    Dim myISETestsDS As ISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)

                                                                    If (myISETestsDS.tparISETests.Rows.Count > 0) Then
                                                                        newRowToReturn.TestName = myISETestsDS.tparISETests(0).Name
                                                                        newRowToReturn.ReplicatesNumber = 1
                                                                        newRowToReturn.SetFormulaTextNull()
                                                                    End If
                                                                End If
                                                            ElseIf (newRowToReturn.TestType = "OFFS") Then
                                                                Dim myOFFSTests As New OffSystemTestsDelegate
                                                                resultData = myOFFSTests.Read(Nothing, newRowToReturn.TestID)

                                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                    Dim myOFFSTestsDS As OffSystemTestsDS = DirectCast(resultData.SetDatos, OffSystemTestsDS)

                                                                    If (myOFFSTestsDS.tparOffSystemTests.Rows.Count > 0) Then
                                                                        newRowToReturn.TestName = myOFFSTestsDS.tparOffSystemTests(0).Name
                                                                        newRowToReturn.ReplicatesNumber = 1
                                                                        newRowToReturn.SetFormulaTextNull()
                                                                    End If
                                                                End If
                                                            End If
                                                            'XB 02/12/2014 - BA-1867

                                                            toReturnData.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(newRowToReturn)
                                                        Next
                                                    End If
                                                End If
                                            End If

                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If (withErrorFlag) Then
                            'BA-1610: a different error code is returned when CALC Test/Sample Type exists and has an unique Sample Type but the 
                            '         Calibration Programming of at least one of the STD Test/SampleType included in its Formula is incomplete
                            resultData.HasError = True
                            If (Not incompleteCalibProg) Then
                                'The requested Test/Sample Type does not exist in the application
                                resultData.ErrorCode = GlobalEnumerates.Messages.LIMS_INVALID_SAMPLETYPE.ToString
                            Else
                                'At least one of the STD Tests included in the Formula of the Calculated Test has Calibration Programming incomplete
                                resultData.ErrorCode = GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString
                            End If
                        End If

                        If (toReturnData.tparSavedWSOrderTests.Rows.Count > 0) Then toReturnData.tparSavedWSOrderTests.AcceptChanges()
                        resultData.SetDatos = toReturnData
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ValidationsForAddCALCOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add the specimen into a list of change status specimens
        ''' Not try---catch the method calling this must implement it
        ''' </summary>
        ''' <param name="pEntryRow"></param>
        ''' <param name="pListSpecimenStatusChange"></param>
        ''' <remarks>
        ''' Created by:  AG 15/03/2013
        ''' </remarks>
        Private Sub AddSpecimenStatusChange(ByVal pEntryRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, ByRef pListSpecimenStatusChange As List(Of String))
            If (Not pListSpecimenStatusChange.Contains(pEntryRow.SpecimenID)) Then pListSpecimenStatusChange.Add(pEntryRow.SpecimenID)
        End Sub

        ''' <summary>
        ''' When a Calculated Test is requested by LIS, all Tests included in its Formula are automatically added; this function verify for each one of
        ''' these additional Tests if they have been already added to the list of final accepted Order Tests
        ''' </summary>
        ''' <param name="pFinal">Typed DataSet SavedWSOrderTestsDS containing all Order Tests requested by LIS that have been validated and accepted</param>
        ''' <param name="pAddedOrderTests">Typed DataSet SavedWSOrderTestsDS containing Order Tests added when a Calculated Test is requested by LIS 
        '''                                (in this case, )</param>
        ''' <returns>GlobalDataTo (setDatos as SavedWSOrderTestsDS)</returns>
        ''' <remarks>
        ''' Created by:  AG 14/03/2013
        ''' Modified by: SA 04/04/2013 - Removed the DB Connection parameter
        '''              SA 23/05/2013 - Before add a CalculatedTestID to the list, verify if it is already included to avoid duplicates
        ''' </remarks>
        Private Function ValidateDuplicateRequest(ByVal pFinal As SavedWSOrderTestsDS, ByVal pAddedOrderTests As SavedWSOrderTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO  'Do not remove New declaration, is needed

            Try
                Dim duplicateID As Boolean
                Dim linqRes As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)

                For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pAddedOrderTests.tparSavedWSOrderTests.Rows
                    linqRes = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pFinal.tparSavedWSOrderTests _
                              Where a.ESPatientID = row.ESPatientID AndAlso a.SampleClass = row.SampleClass _
                            AndAlso a.TestType = row.TestType AndAlso a.TestID = row.TestID _
                            AndAlso a.SampleType = row.SampleType Select a).ToList

                    If (linqRes.Count > 0) Then
                        'If record found, edit fields CalctestIDs and CalcTestNames
                        duplicateID = False

                        linqRes(0).BeginEdit()
                        If (Not linqRes(0).IsCalcTestIDsNull) Then
                            Dim myCalcTestList() As String = linqRes(0).CalcTestIDs.Split(CChar(","))
                            For Each id As String In myCalcTestList
                                If (id.Trim = row.CalcTestIDs) Then
                                    duplicateID = True
                                    Exit For
                                End If
                            Next

                            If (Not duplicateID) Then
                                linqRes(0).CalcTestIDs &= ","
                                linqRes(0).CalcTestNames &= ","
                            End If
                        End If

                        If (Not duplicateID) Then
                            linqRes(0).CalcTestIDs &= row.CalcTestIDs
                            linqRes(0).CalcTestNames &= row.CalcTestNames
                        End If
                        linqRes(0).EndEdit()
                    Else
                        'If no record found add it into structure to return
                        pFinal.tparSavedWSOrderTests.ImportRow(row)
                    End If
                    pFinal.tparSavedWSOrderTests.AcceptChanges()
                Next
                linqRes = Nothing

                resultData.SetDatos = pFinal
                resultData.HasError = False
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ValidateDuplicateRequest", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Decode and validate an XML Message sent by LIS. All AwosID in the message that cannot be accepted, are returned in an OrderTestsLISInfoDS 
        ''' </summary>
        ''' <param name="pXMLDoc">LIS XML Message</param>
        ''' <param name="pRejectedOrderTestsLISInfoDS">Typed DataSet OrderTestsLISInfoDS used to return all rejected Service Nodes</param>
        ''' <param name="pChannel">ES Channel Identifier</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <param name="pAnalyzerSN">Analyzer Serial Number</param>
        ''' <param name="pLISTestsMappingsDS">Typed DataSet AllTestsByTypeDS containing all defined LIS Test Mappings</param>
        ''' <param name="pLISMappings">Typed DataSet LISMappingsDS containing all defined LIS Mappings for configuration values</param>
        ''' <param name="pDefaultPatientTube">Type of tube defined by default for Patients</param>
        ''' <param name="pDefaultControlTube">Type of tube defined by default for Controls</param>
        ''' <param name="pWorkSessionID">Current WorkSession ID</param>
        ''' <param name="pMsgDateTime">Date and Time the Message was received from LIS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SGM 21/03/2013
        ''' Modified by: SGM 25/03/2013 - Passed pRejectedOrderTestsLISInfoDS by reference when calling function ProcessLISOrderTests
        '''              SA  04/04/2013 - Removed parameter for a DB Connection
        '''              SGM 02/05/2013 - Include XML schema for catching HL7 errors
        '''              AG  08/05/2013 - Added parameter pWorkSessionID, needed in call to function ProcessLISOrderTests
        '''              SGM 16/05/2013 - The tag "error" is treated by means of a unique way for both protocols (HL7 and ASTM)
        '''              JCM 23/07/2013 - BT #1228/1230 ==> When LIS sent a not well-built message, change LIS Status of the Sample to PENDING 
        '''              SA  01/04/2014 - BT #1564 ==> Added new parameter for the Message Date and Time, and inform them when calling function 
        '''                                            ProcessLISOrderTests
        ''' </remarks>
        Private Function ValidateXmlMessage(ByVal pXMLDoc As XmlDocument, ByRef pRejectedOrderTestsLISInfoDS As OrderTestsLISInfoDS, ByVal pChannel As String, _
                                            ByVal pAnalyzerModel As String, ByVal pAnalyzerSN As String, ByVal pLISTestsMappingsDS As AllTestsByTypeDS, _
                                            ByVal pLISMappings As LISMappingsDS, ByVal pDefaultPatientTube As String, ByVal pDefaultControlTube As String, _
                                            ByVal pWorkSessionID As String, ByVal pMsgDateTime As Date) As GlobalDataTO
            Dim resultData As New GlobalDataTO  'Do not remove New declaration, is needed

            Try
                'Instantiate if needed
                If (pRejectedOrderTestsLISInfoDS Is Nothing) Then
                    pRejectedOrderTestsLISInfoDS = New OrderTestsLISInfoDS
                End If

                'Verify if the XML Message can be processed
                If (pXMLDoc IsNot Nothing) Then
                    'SGM 02/05/2013
                    Dim myProtocol As String = String.Empty
                    Dim myUserSettingsDelegate As New UserSettingsDelegate
                    resultData = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_PROTOCOL_NAME.ToString())
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then myProtocol = CStr(resultData.SetDatos)

                    'SGM 16/05/2013 - The tag "error" is treated by means of a unique way for both protocols (HL7 & ASTM)
                    Dim myXmlHelper As xmlHelper
                    Dim ErrorsList As XmlNodeList

                    'Pending to solve STL errors with new driver 16/05/2013
                    myXmlHelper = New xmlHelper("udc", UDCSchema, "ci", ClinicalInfoSchema)
                    ErrorsList = myXmlHelper.QueryXmlNodeList(pXMLDoc, "udc:command/udc:body/udc:error")

                    'SGM 16/05/2013 - In case of no Errors found, Process message and treat it
                    If (ErrorsList IsNot Nothing AndAlso ErrorsList.Count = 0) Then
                        'Process Patient Nodes in the XML
                        resultData = MyClass.ProcessLISPatients(pXMLDoc, pChannel, pAnalyzerModel, pAnalyzerSN)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myPatientsDS As PatientsDS = TryCast(resultData.SetDatos, PatientsDS)

                            'Process Order Nodes in the XML
                            resultData = MyClass.ProcessLISOrders(pXMLDoc, myPatientsDS, pChannel, pAnalyzerModel, pAnalyzerSN)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myOrdersDS As OrdersDS = TryCast(resultData.SetDatos, OrdersDS)

                                'Process Service Nodes in the XML 
                                'BT #1564 - Inform also the Message Date and Time as function parameter
                                resultData = MyClass.ProcessLISOrderTests(pXMLDoc, myPatientsDS, myOrdersDS, pRejectedOrderTestsLISInfoDS, _
                                                                          pLISTestsMappingsDS, pLISMappings, pDefaultPatientTube, pDefaultControlTube, _
                                                                          pChannel, pAnalyzerModel, pAnalyzerSN, pWorkSessionID, pMsgDateTime)
                            End If
                        End If
                    Else
                        'JCM 23/07/2013 - There was any error and if message tag inform SpecimenId, we must change Specimen's LIS Status from ASKING to PENDING
                        Dim mySpecimentIds As New List(Of String)
                        For Each mySampleId As XmlNode In myXmlHelper.QueryXmlNodeList(pXMLDoc, "udc:command/udc:body/udc:message/ci:service/ci:specimen/ci:id")
                            If (mySampleId.HasChildNodes AndAlso Not String.IsNullOrEmpty(mySampleId.InnerText)) Then
                                mySpecimentIds.Add(mySampleId.InnerText)
                            End If
                        Next

                        If (mySpecimentIds.Count > 0) Then
                            resultData = (New BarcodePositionsWithNoRequestsDelegate).UpdateLISStatus(Nothing, mySpecimentIds, "PENDING")
                        End If

                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "xmlMessagesDelegate.ValidateXmlMessage", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
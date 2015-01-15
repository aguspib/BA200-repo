'Class creation 21/03/2013 AG
'Implements the internal business rules of the Embedded Synapse (ES) library such as
'   - Upload results requires 1 message for each patient (for control 1 message for all)
'   - Reject awos later than the order reception requires 1 message for each patient (for control 1 message for all)
'   - Host Query different rules for HL7 or ASTM
'   - ...
'
'Also specific business for BAx00 application when notifications are received

Imports System
Imports System.Xml
Imports System.Diagnostics

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.LISCommunications

    Public Class ESBusiness

#Region "LIS --> Presentation"

        ''' <summary>
        ''' Decodes and apply the business rules for each ES notification (control information or query response) received
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pChannelID"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pXMLNotification"></param>
        ''' <param name="pISUploadResultsNotificationFlag"></param>
        ''' <returns>GlobalDataTo with decoded notification info ( Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String)))</returns>
        ''' <remarks>
        ''' Created by: AG 21/03/2013
        ''' Modified by: SG 10/04/2013 - update ExportStatus of results
        ''' Modified by AG 07/03/2014 - #1533 new byref pISUploadResultsNotificationFlag parameter that is set to TRUE when notification belongs to a upload results message
        '''</remarks>
        Public Function TreatXMLNotification(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pChannelID As String, ByVal pAnalyzerModel As String, ByVal pAnalyzerID As String, _
                                     ByVal pXMLNotification As XmlDocument, ByRef pISUploadResultsNotificationFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing


            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTranslator As New ESxmlTranslator(pChannelID, pAnalyzerModel, pAnalyzerID)
                        resultData = myTranslator.DecodeXMLNotification(dbConnection, pXMLNotification)
                        If Not resultData.HasError Then
                            Dim notificationLis As New Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String))
                            notificationLis = DirectCast(resultData.SetDatos, Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String)))

                            resultData = New GlobalDataTO

                            Dim notificationValues As New List(Of String)
                            Dim myLogAcciones As New ApplicationLogManager()

                            '1) CONTROL INFORMATION
                            '======================
                            'STATUS
                            If notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.STATUS) AndAlso notificationLis(LISNotificationSensors.STATUS).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.STATUS) '1- Get Data
                                'No business

                                'STORAGE
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.STORAGE) AndAlso notificationLis(LISNotificationSensors.STORAGE).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.STORAGE) '1- Get Data
                                'No business

                                'DELIVERED
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.DELIVERED) AndAlso notificationLis(LISNotificationSensors.DELIVERED).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.DELIVERED) '1- Get Data
                                '2- Treat the DELIVERED information
                                'Update the export status to SENT into the affected results
                                'SGM 10/04/2013
                                For Each N As String In notificationValues
                                    Dim ID As String = MyClass.ExtractMessageIDFromNotification(N)
                                    If ID.Length > 0 Then
                                        'Debug.Print("Delivered Notification: " & ID & " - " & Now.ToString("HH:mm:ss:fff")) 'SG 15/04/2012 - LIS Message notification
                                        resultData = MyClass.UpdateExportStatus(Nothing, ID, "SENT")
                                        If resultData.HasError Then
                                            Throw New Exception(resultData.ErrorMessage)
                                        ElseIf resultData.AffectedRecords = 0 Then
                                            'NOTHING TO DO FOR HOST QUERY!!!!!!

                                            'AG 07/03/2014 - #1533
                                        ElseIf resultData.AffectedRecords > 0 Then
                                            pISUploadResultsNotificationFlag = True 'Inform notification belongs to a previous upload results message
                                            myLogAcciones.CreateLogActivity("Delivered notification - message uploading results: " & ID, "ESBusiness.TreatXMLNotification", EventLogEntryType.Information, False) 'AG 25/03/2014
                                            'AG 07/03/2014 - #1533
                                        End If
                                    End If
                                Next


                                'UNDELIVERED
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.UNDELIVERED) AndAlso notificationLis(LISNotificationSensors.UNDELIVERED).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.UNDELIVERED) '1- Get Data
                                '2- Treat the UNDELIVERED information
                                'Update the export status to NOTSENT into the affected results
                                'SGM 10/04/2013
                                Dim barcodeDelegate As BarcodePositionsWithNoRequestsDelegate = New BarcodePositionsWithNoRequestsDelegate()
                                For Each N As String In notificationValues
                                    Dim ID As String = MyClass.ExtractMessageIDFromNotification(N)
                                    If ID.Length > 0 Then
                                        ' Debug.Print("Undelivered Notification: " & ID & " - " & Now.ToString("HH:mm:ss:fff")) 'SG 15/04/2012 - LIS Message notification
                                        resultData = MyClass.UpdateExportStatus(Nothing, ID, "NOTSENT")
                                        If resultData.HasError Then
                                            Throw New Exception(resultData.ErrorMessage)
                                        ElseIf resultData.AffectedRecords = 0 Then
                                            'Call method to update hostquery status -> UpdateHQStatus
                                            barcodeDelegate.UpdateHQStatus(Nothing, pAnalyzerID, "SAMPLES", ID, "UNDELIVERED")
                                            Debug.Print(Now.ToString & ". HostQuery UNDELIVERED (undelivered)!!!")

                                            'AG 07/03/2014 - #1533
                                        ElseIf resultData.AffectedRecords > 0 Then
                                            pISUploadResultsNotificationFlag = True 'Inform notification belongs to a previous upload results message
                                            myLogAcciones.CreateLogActivity("Undelivered notification - message uploading results: " & ID, "ESBusiness.TreatXMLNotification", EventLogEntryType.Information, False) 'AG 25/03/2014
                                            'AG 07/03/2014 - #1533
                                        End If
                                    End If
                                Next

                                'UNRESPONDED
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.UNRESPONDED) AndAlso notificationLis(LISNotificationSensors.UNRESPONDED).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.UNRESPONDED) '1- Get Data
                                '2- Treat the UNRESPONDED information
                                'Update the export status to NOTSENT into the affected results
                                'SGM 10/04/2013
                                Dim barcodeDelegate As BarcodePositionsWithNoRequestsDelegate = New BarcodePositionsWithNoRequestsDelegate()
                                For Each N As String In notificationValues
                                    Dim ID As String = MyClass.ExtractMessageIDFromNotification(N)
                                    If ID.Length > 0 Then
                                        ' Debug.Print("Unresponded Notification: " & ID & " - " & Now.ToString("HH:mm:ss:fff")) 'SG 15/04/2012 - LIS Message notification
                                        resultData = MyClass.UpdateExportStatus(Nothing, ID, "NOTSENT")
                                        If resultData.HasError Then
                                            Throw New Exception(resultData.ErrorMessage)
                                        ElseIf resultData.AffectedRecords = 0 Then
                                            'Call method to update hostquery status -> UpdateHQStatus
                                            barcodeDelegate.UpdateHQStatus(Nothing, pAnalyzerID, "SAMPLES", ID, "UNDELIVERED")
                                            Debug.Print(Now.ToString & ". HostQuery UNDELIVERED (unresponded)!!!")

                                            'AG 07/03/2014 - #1533
                                        ElseIf resultData.AffectedRecords > 0 Then
                                            pISUploadResultsNotificationFlag = True 'Inform notification belongs to a previous upload results message
                                            myLogAcciones.CreateLogActivity("Unresponded notification - message uploading results: " & ID, "ESBusiness.TreatXMLNotification", EventLogEntryType.Information, False) 'AG 25/03/2014
                                            'AG 07/03/2014 - #1533
                                        End If
                                    End If
                                Next


                                'PENDINGMESSAGES
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES) AndAlso notificationLis(LISNotificationSensors.PENDINGMESSAGES).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.PENDINGMESSAGES) '1- Get Data
                                '2- Treat the PENDINGMESSAGES information
                                '??? - It will be used from LIS utilities screen but it is not defined yet
                                'TODO (use Nothing as connection)

                                'DELETED
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.DELETED) AndAlso notificationLis(LISNotificationSensors.DELETED).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.DELETED) '1- Get Data
                                '2- Treat the DELETED information
                                '??? - It will be used from LIS utilities screen but it is not defined yet
                                'TODO (use Nothing as connection)


                                '2) QUERY RESPONSE
                                '=================
                                'INVALID
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.INVALID) AndAlso notificationLis(LISNotificationSensors.INVALID).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.INVALID) '1- Get Data
                                '2- Treat the INVALID information
                                'Call method to update hostquery status -> UpdateHQStatus
                                ' No business



                                'QUERYALL
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.QUERYALL) AndAlso notificationLis(LISNotificationSensors.QUERYALL).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.QUERYALL) '1- Get Data
                                '2- Treat the QUERYALL information
                                'No business

                                'HOSTQUERY
                            ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.HOSTQUERY) AndAlso notificationLis(LISNotificationSensors.HOSTQUERY).Count > 0 Then
                                notificationValues = notificationLis(LISNotificationSensors.HOSTQUERY) '1- Get Data
                                '2- Treat the HOSTQUERY information
                                'No business
                                Dim barcodeDelegate As BarcodePositionsWithNoRequestsDelegate = New BarcodePositionsWithNoRequestsDelegate()
                                For Each N As String In notificationValues
                                    ' Has Error ?
                                    If MyClass.ExtractMessageFiledFromNotification(N, 3) <> "" Then
                                        Dim ID As String = MyClass.ExtractMessageIDFromNotification(N)
                                        If ID.Length > 0 Then
                                            'Call method to update hostquery status -> UpdateHQStatus
                                            barcodeDelegate.UpdateHQStatus(Nothing, pAnalyzerID, "SAMPLES", ID, "REJECTED")
                                            Debug.Print(Now.ToString & ". HostQuery REJECTED!!!")
                                        End If
                                    End If
                                Next

                            End If

                            resultData.SetDatos = notificationLis

                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.TreatXMLNotification", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Extract from a notification string the affected message id
        ''' </summary>
        ''' <param name="pNotification"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 10/04/2013</remarks>
        Private Function ExtractMessageIDFromNotification(ByVal pNotification As String) As String
            Dim myID As String = ""
            Try
                Dim mySep As String = GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR.ToString
                Dim myValues As String() = pNotification.Split(mySep.ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                If myValues.Length > 0 Then
                    myID = myValues(0)
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.ExtractMessageIDFromNotification", EventLogEntryType.Error, False)
            End Try
            Return myID
        End Function

        ''' <summary>
        ''' Extract from a notification string the affected message field
        ''' </summary>
        ''' <param name="pNotification"></param>
        ''' <param name="position">Value of Position of Field Requested, first field is position 0</param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 10/04/2013</remarks>
        Private Function ExtractMessageFiledFromNotification(ByVal pNotification As String, ByVal position As Integer) As String
            Dim myFiled As String = ""
            Try
                Dim mySep As String = GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR.ToString
                Dim myValues As String() = pNotification.Split(mySep.ToCharArray, StringSplitOptions.None)
                If myValues.Length > position Then
                    myFiled = myValues(position)
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.ExtractMessageFiledFromNotification, on Position " + position, EventLogEntryType.Error, False)
            End Try
            Return myFiled
        End Function

        ''' <summary>
        ''' Updates all the results related to the informed messageID with the informed ExportStatus
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pLISMessageID"></param>
        ''' <param name="pNewExportStatus"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 10/04/2013
        ''' AG 17/04/2013 complete the historical part
        ''' AG 18/04/2013 use the proer template
        ''' Modified by: DL 25/04/2013 Clear fields used for upload results to LIS (free DB space)
        ''' AG 14/03/2014 - #1533 for current WS results, get the list of affected OrderTestID - RerunNumber (add parameter ByVal pCurrentWSResultsAffected As ExecutionsDS)
        ''' AG 02/04/2014 - #1564 inform the ExportDateTime is required (previous correction #1533 was wrong developped and left this field as NULL always)</remarks>
        Public Function UpdateExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pLISMessageID As String, _
                                           ByVal pNewExportStatus As String) As GlobalDataTO

            'Dim resultData As GlobalDataTO = Nothing
            'Dim dbConnection As SqlClient.SqlConnection = Nothing

            'Try
            '    resultData = DAOBase.GetOpenDBConnection(pDBConnection)

            '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '        dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
            '        If (Not dbConnection Is Nothing) Then

            '            Dim myResultsDelegate As New ResultsDelegate
            '            resultData = myResultsDelegate.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus)

            '            'AG 17/04/2013
            '            If Not resultData.HasError AndAlso resultData.AffectedRecords = 0 Then
            '                Dim myHistDlg As New HisWSResultsDelegate
            '                resultData = myHistDlg.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus)
            '            End If
            '            'AG 17/04/2013
            '        End If
            '    End If

            'Catch ex As Exception
            '    resultData = New GlobalDataTO()
            '    resultData.HasError = True
            '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            '    resultData.ErrorMessage = ex.Message

            '    Dim myLogAcciones As New ApplicationLogManager()
            '    myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.UpdateExportStatus", EventLogEntryType.Error, False)

            'Finally
            '    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            'End Try

            'Return resultData


            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultsDelegate As New ResultsDelegate

                        'AG 02/04/2014 - #1564 inform the export datetime when the new status is SENT!!
                        'resultData = myResultsDelegate.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus)
                        Dim setExportDateTime As Boolean = False
                        If pNewExportStatus = "SENT" Then setExportDateTime = True
                        resultData = myResultsDelegate.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus, setExportDateTime)
                        'AG 02/04/2014

                        If Not resultData.HasError AndAlso resultData.AffectedRecords = 0 Then
                            Dim myHistDlg As New HisWSResultsDelegate
                            'AG 02/04/2014 - #1565 NOTE: Not inform the flag setExportDateTime because in historical tables only the status is saved!!!
                            resultData = myHistDlg.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus)

                            If Not resultData.HasError AndAlso resultData.AffectedRecords > 0 Then
                                Dim myAffected As Integer = resultData.AffectedRecords
                                If pNewExportStatus = "SENT" Then
                                    'DL 25/04/2013
                                    'Clear fields used for upload results to LIS (free DB space)
                                    resultData = myHistDlg.ClearIdentifiersForLIS(dbConnection, pLISMessageID)
                                    'DL 25/04/2013
                                End If

                                'Recover and return the affected records value
                                resultData.AffectedRecords = myAffected

                            End If

                            'AG 14/03/2014  - #1533 - uncomment this code
                            'Else 'Current WS results uploaded to pNewExportStatus - get the list of affected OrderTest-RerunNumber
                            '    'Use another GlobalDataTo for not to lose AffectedRecords!!
                            '    Dim auxGlobal As GlobalDataTO
                            '    auxGlobal = myResultsDelegate.GetResultsByMessageID(dbConnection, pLISMessageID)
                            '    If Not auxGlobal Is Nothing AndAlso Not auxGlobal.HasError AndAlso Not auxGlobal.SetDatos Is Nothing Then
                            '        pCurrentWSResultsAffected = DirectCast(auxGlobal.SetDatos, ExecutionsDS)
                            '    End If
                            'AG 14/03/2014  - #1533

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.UpdateExportStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function


#End Region

#Region "Presentation --> LIS"

        ''' <summary>
        ''' Receives the complete set of results to export
        ''' This set must be evaluate and separate into several xml messages to LIS (1 xml for each patient, 1 xml for all controls)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionsToUpload"></param>
        ''' <returns>GlobalDataTo (List of (ExecutionsDS) where each list item contains the results to upload in a unique xml message)</returns>
        ''' <remarks>
        ''' AG 21/03/2013
        ''' Modified by SG 25/04/2013 - include sampleclass = N or U as patient
        ''' AG 19/06/2013 - divide by sampleId and then by patientID
        ''' SGM 01/07/2013 - add executions with patientID without sampleID - Bug #1200
        ''' </remarks>
        Public Function DivideResultsToUploadIntoSeveralMessages(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsToUpload As ExecutionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Search different sample classes in results to upload
                        Dim linqSampleClasses As List(Of String)
                        linqSampleClasses = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions Select a.SampleClass Distinct).ToList

                        'Search different sample types in results to upload
                        Dim linqSampleTypes As List(Of String)
                        linqSampleTypes = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions Select a.SampleType Distinct).ToList

                        'Search different patients in results to upload
                        Dim linqSampleID As List(Of String)
                        'linqSampleID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions Select a.SampleID Distinct).ToList
                        linqSampleID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions Where Not a.IsSampleIDNull AndAlso a.SampleID <> "" Select a.SampleID Distinct).ToList

                        'AG 19/06/2013
                        Dim linqPatientID As List(Of String)
                        linqPatientID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions Where Not a.IsPatientIDNull AndAlso a.PatientID <> "" Select a.PatientID Distinct).ToList

                        'Temporal for create xml
                        Dim linqResForXml As List(Of ExecutionsDS.twksWSExecutionsRow)

                        Dim toReturn As New List(Of ExecutionsDS)

                        Dim myLogAcciones As New ApplicationLogManager() 'AG 02/01/2014 - BT #1433 (v211 patch2)
                        Dim logTrace As String = String.Empty 'AG 02/01/2014 - BT #1433 (v211 patch2)
                        Dim testInfo As String = String.Empty 'AG 02/01/2014 - BT #1433 (v211 patch2)

                        For Each sClass As String In linqSampleClasses
                            'If sClass = "PATIENT" Then
                            If sClass.ToUpperBS = "PATIENT" Or sClass.ToUpperBS = "N" Or sClass.ToUpperBS = "U" Then 'SGM 25/04/2013
                                'AG 19/03/2013
                                'STL integration 19/03/2013 - Is not required separate by sampletype. Divide by patient not by specimen (tube) 
                                'Double loop search results for each patient and sample type

                                ''Add the distinct sampleID
                                'For Each patient As String In linqSampleID
                                '    linqResForXml = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions _
                                '                   Where a.SampleClass = sClass AndAlso Not a.IsSampleIDNull AndAlso a.SampleID = patient Select a Distinct).ToList

                                '    Dim tempDS As New ExecutionsDS
                                '    For Each row As ExecutionsDS.twksWSExecutionsRow In linqResForXml
                                '        tempDS.twksWSExecutions.ImportRow(row)
                                '    Next
                                '    tempDS.twksWSExecutions.AcceptChanges()
                                '    If tempDS.twksWSExecutions.Rows.Count > 0 Then
                                '        toReturn.Add(tempDS)
                                '    End If
                                'Next
                                ''AG 19/03/2013

                                ''AG 19/06/2013 - Add the distinct PatientID
                                'For Each patient As String In linqPatientID
                                '    linqResForXml = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions _
                                '                   Where a.SampleClass = sClass AndAlso Not a.IsPatientIDNull AndAlso a.PatientID = patient Select a Distinct).ToList

                                '    Dim tempDS As New ExecutionsDS
                                '    For Each row As ExecutionsDS.twksWSExecutionsRow In linqResForXml
                                '        tempDS.twksWSExecutions.ImportRow(row)
                                '    Next
                                '    tempDS.twksWSExecutions.AcceptChanges()
                                '    If tempDS.twksWSExecutions.Rows.Count > 0 Then
                                '        toReturn.Add(tempDS)
                                '    End If
                                'Next
                                ''AG 19/06/2013



                                'SGM 01/07/2013 - add executions with patientID without sampleID - Bug #1200
                                For Each patient As String In linqPatientID
                                    linqResForXml = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions _
                                                   Where a.SampleClass = sClass AndAlso Not a.IsPatientIDNull AndAlso a.PatientID = patient AndAlso a.IsSampleIDNull Select a Distinct).ToList

                                    Dim tempDS As New ExecutionsDS
                                    For Each row As ExecutionsDS.twksWSExecutionsRow In linqResForXml
                                        tempDS.twksWSExecutions.ImportRow(row)
                                        'AG 02/01/2014 - BT #1433 - Add new traces to upload results (v211 patch2)
                                        testInfo = ""
                                        If Not row.IsTestNameNull Then testInfo = row.TestName
                                        If Not row.IsSampleTypeNull Then testInfo &= "-" & row.SampleType
                                        If Not row.IsTestTypeNull Then testInfo &= " (" & row.TestType & ") "

                                        logTrace = ""
                                        logTrace = "Prepare results to upload: PATIENT, PatientID = " & patient & " , " & testInfo
                                        myLogAcciones.CreateLogActivity(logTrace, "ESBusiness.DivideResultsToUploadIntoSeveralMessages", EventLogEntryType.Information, False)
                                        'AG 16/12/2013
                                    Next
                                    tempDS.twksWSExecutions.AcceptChanges()
                                    If tempDS.twksWSExecutions.Rows.Count > 0 Then
                                        toReturn.Add(tempDS)
                                    End If
                                Next

                                'Add the distinct sampleID
                                For Each sample As String In linqSampleID
                                    linqResForXml = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions _
                                                   Where a.SampleClass = sClass AndAlso Not a.IsSampleIDNull AndAlso a.SampleID = sample Select a Distinct).ToList

                                    Dim tempDS As New ExecutionsDS
                                    For Each row As ExecutionsDS.twksWSExecutionsRow In linqResForXml
                                        tempDS.twksWSExecutions.ImportRow(row)
                                        'AG 02/01/2014 - BT #1433 - Add new traces to upload results (v211 patch2)
                                        testInfo = ""
                                        If Not row.IsTestNameNull Then testInfo = row.TestName
                                        If Not row.IsSampleTypeNull Then testInfo &= "-" & row.SampleType
                                        If Not row.IsTestTypeNull Then testInfo &= " (" & row.TestType & ") "

                                        logTrace = ""
                                        logTrace = "Prepare results to upload: PATIENT, SampleID = " & sample & " , " & testInfo
                                        myLogAcciones.CreateLogActivity(logTrace, "ESBusiness.DivideResultsToUploadIntoSeveralMessages", EventLogEntryType.Information, False)
                                        'AG 16/12/2013
                                    Next
                                    tempDS.twksWSExecutions.AcceptChanges()
                                    If tempDS.twksWSExecutions.Rows.Count > 0 Then
                                        toReturn.Add(tempDS)
                                    End If
                                Next
                                'SGM 01/07/2013

                            Else 'Ctrls
                                linqResForXml = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutionsToUpload.twksWSExecutions _
                                           Where a.SampleClass = sClass Select a Distinct).ToList

                                Dim tempDS As New ExecutionsDS
                                For Each row As ExecutionsDS.twksWSExecutionsRow In linqResForXml
                                    tempDS.twksWSExecutions.ImportRow(row)
                                    'AG 02/01/2014 - BT #1433 - Add new traces to upload results (v211 patch2)
                                    testInfo = ""
                                    If Not row.IsTestNameNull Then testInfo = row.TestName
                                    If Not row.IsSampleTypeNull Then testInfo &= "-" & row.SampleType
                                    If Not row.IsTestTypeNull Then testInfo &= " (" & row.TestType & ") "

                                    logTrace = ""
                                    logTrace = "Prepare results to upload: CONTROL, " & testInfo
                                    myLogAcciones.CreateLogActivity(logTrace, "ESBusiness.DivideResultsToUploadIntoSeveralMessages", EventLogEntryType.Information, False)
                                    'AG 16/12/2013
                                Next

                                tempDS.twksWSExecutions.AcceptChanges()
                                If tempDS.twksWSExecutions.Rows.Count > 0 Then
                                    toReturn.Add(tempDS)
                                End If
                            End If
                        Next

                        'Release lists
                        linqResForXml = Nothing
                        linqPatientID = Nothing
                        linqSampleTypes = Nothing
                        linqSampleClasses = Nothing

                        resultData.SetDatos = toReturn
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.DivideResultsToUploadIntoSeveralMessages", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Receives the complete set of awos to reject
        ''' This set must be evaluate and separate into several xml messages to LIS (1 xml for each patient, 1 xml for all controls)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pToRejectDS"></param>
        ''' <returns>GlobalDataTo (List(Of OrderTestsLISInfoDS) where each list item contains the awos information to reject in a unique xml message)</returns>
        ''' <remarks>AG 21/03/2013</remarks>
        Public Function DivideAwosToRejectIntoSeveralMessages(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pToRejectDS As OrderTestsLISInfoDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Search different sample classes in results to upload
                        Dim linqSampleClasses As List(Of String)
                        linqSampleClasses = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pToRejectDS.twksOrderTestsLISInfo Select a.SampleClass Distinct).ToList

                        'Search different sample types in results to upload
                        Dim linqSampleTypes As List(Of String)
                        linqSampleTypes = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pToRejectDS.twksOrderTestsLISInfo Select a.SampleType Distinct).ToList

                        'Search different patients in results to upload
                        Dim linqPatientID As List(Of String)
                        'AG 30/04/2013 - distinct by LISPatientID instead of ESPatientID (return to original code)
                        linqPatientID = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pToRejectDS.twksOrderTestsLISInfo Select a.ESPatientID Distinct).ToList
                        'linqPatientID = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pToRejectDS.twksOrderTestsLISInfo Select a.LISPatientID Distinct).ToList

                        'Temporal for create xml
                        Dim linqResForXml As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)

                        Dim myLogAcciones As New ApplicationLogManager() 'AG 02/01/2014 - BT #1433 (v211 patch2)
                        Dim logTrace As String = String.Empty 'AG 02/01/2014 - BT #1433 (v211 patch2)
                        Dim testInfo As String = String.Empty 'AG 02/01/2014 - BT #1433 (v211 patch2)

                        Dim toReturn As New List(Of OrderTestsLISInfoDS)
                        For Each sClass As String In linqSampleClasses
                            If sClass.ToUpperBS = "PATIENT" Then 'AG 22/03/2013 - The sample class arrives at this point already mapped for LIS ??? (use both values)
                                'AG 19/03/2013
                                'STL integration 19/03/2013 - Is not required separate by sampletype. Divide by patient not by specimen (tube) 
                                'Double loop search awos to reject for each patient and sample type

                                For Each patient As String In linqPatientID
                                    'AG 30/04/2013 - use LISPatientID instead of ESPatientID (return to original code)
                                    linqResForXml = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pToRejectDS.twksOrderTestsLISInfo _
                                                   Where a.SampleClass = sClass AndAlso a.ESPatientID = patient Select a Distinct).ToList
                                    'linqResForXml = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pToRejectDS.twksOrderTestsLISInfo _
                                    '                 Where a.SampleClass = sClass AndAlso a.LISPatientID = patient Select a Distinct).ToList

                                    Dim tempDS As New OrderTestsLISInfoDS
                                    For Each row As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In linqResForXml
                                        tempDS.twksOrderTestsLISInfo.ImportRow(row)
                                        'AG 02/01/2014 - BT #1433 - Add new traces to upload results (v211 patch2)
                                        testInfo = ""
                                        If Not row.IsTestIDStringNull Then testInfo = row.TestIDString
                                        If Not row.IsSampleTypeNull Then testInfo &= "-" & row.SampleType
                                        If Not row.IsTestTypeNull Then testInfo &= " (" & row.TestType & ") "

                                        logTrace = ""
                                        logTrace = "Prepare results to reject: PATIENT, ESPatientID = " & patient & " , " & testInfo
                                        myLogAcciones.CreateLogActivity(logTrace, "ESBusiness.DivideAwosToRejectIntoSeveralMessages", EventLogEntryType.Information, False)
                                        'AG 16/12/2013
                                    Next
                                    tempDS.twksOrderTestsLISInfo.AcceptChanges()
                                    If tempDS.twksOrderTestsLISInfo.Rows.Count > 0 Then
                                        toReturn.Add(tempDS)
                                    End If
                                Next
                                'AG 19/03/2013

                            Else 'Ctrls
                                linqResForXml = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pToRejectDS.twksOrderTestsLISInfo _
                                           Where a.SampleClass = sClass Select a Distinct).ToList

                                Dim tempDS As New OrderTestsLISInfoDS
                                For Each row As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In linqResForXml
                                    tempDS.twksOrderTestsLISInfo.ImportRow(row)
                                    'AG 02/01/2014 - BT #1433 - Add new traces to upload results (v211 patch2)
                                    testInfo = ""
                                    If Not row.IsTestIDStringNull Then testInfo = row.TestIDString
                                    If Not row.IsSampleTypeNull Then testInfo &= "-" & row.SampleType
                                    If Not row.IsTestTypeNull Then testInfo &= " (" & row.TestType & ") "

                                    logTrace = ""
                                    logTrace = "Prepare results to reject: CONTROL, " & testInfo
                                    myLogAcciones.CreateLogActivity(logTrace, "ESBusiness.DivideAwosToRejectIntoSeveralMessages", EventLogEntryType.Information, False)
                                    'AG 16/12/2013
                                Next
                                tempDS.twksOrderTestsLISInfo.AcceptChanges()
                                If tempDS.twksOrderTestsLISInfo.Rows.Count > 0 Then
                                    toReturn.Add(tempDS)
                                End If
                            End If
                        Next

                        'Release lists
                        linqResForXml = Nothing
                        linqPatientID = Nothing
                        linqSampleTypes = Nothing
                        linqSampleClasses = Nothing

                        resultData.SetDatos = toReturn
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.DivideAwosToRejectIntoSeveralMessages", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' The HostQuery message depends on the protocol
        ''' Initial specifications 
        ''' - 1 xml message can contain a maximum number of specimens (LIS_HOST_QUERY_PACKAGE)
        ''' 
        ''' STL specification change (March 2013) - PENDING APPROVAL!!!
        ''' - HL7: 1 xml message for each specimen
        ''' - ASTM: 1 xml message can contain a maximum number of specimens (LIS_HOST_QUERY_PACKAGE)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pSpecimenList"></param>
        ''' <returns>GlobalDataTo (List(Of String) where each list item contains the specimens in a unique xml message)</returns>
        ''' <remarks>AG 21/03/2013
        ''' AG 18/04/2013 - For HL7 do not read host query package and force value to 1 - CONFIRMED!!!
        ''' AG 07/05/2013 - Change ToUpper for ToUpperBS (ToUpperInvariant)</remarks>
        Public Function DivideIntoSeveralHostQueryMessages(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSpecimenList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the settiong LIS_HOST_QUERY_PACKAGE
                        Dim maxSpecimensByMessage As Integer = 1
                        Dim settingsDlg As New UserSettingsDelegate
                        'AG 18/04/2013
                        Dim myProtocolName As String = ""
                        'Get the protocol name to validate the host query package
                        resultData = settingsDlg.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_PROTOCOL_NAME.ToString)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myDS As UserSettingDS = DirectCast(resultData.SetDatos, UserSettingDS)
                            If myDS.tcfgUserSettings.Rows.Count > 0 AndAlso Not myDS.tcfgUserSettings(0).IsCurrentValueNull Then
                                myProtocolName = CStr(myDS.tcfgUserSettings(0).CurrentValue)
                            End If
                        End If

                        If myProtocolName.ToUpperBS <> "HL7" Then
                            'AG 18/04/2013

                            resultData = settingsDlg.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_HOST_QUERY_PACKAGE.ToString)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim myDS As UserSettingDS = DirectCast(resultData.SetDatos, UserSettingDS)
                                If myDS.tcfgUserSettings.Rows.Count > 0 AndAlso Not myDS.tcfgUserSettings(0).IsCurrentValueNull Then
                                    maxSpecimensByMessage = CInt(myDS.tcfgUserSettings(0).CurrentValue)
                                    If maxSpecimensByMessage = 0 Then maxSpecimensByMessage = 1
                                    If maxSpecimensByMessage < 0 Then maxSpecimensByMessage *= -1
                                End If
                            End If
                        End If 'AG 18/04/2013

                        'Calculate the number of HostQuery messages required
                        Dim N As Integer = 1
                        N = CInt(pSpecimenList.Count / maxSpecimensByMessage)
                        If N * maxSpecimensByMessage < pSpecimenList.Count Then
                            N += 1
                        End If

                        Dim myLogAcciones As New ApplicationLogManager() 'AG 02/01/2014 - BT #1433 (v211 patch2)
                        Dim specimensInMessage As String = String.Empty 'AG 02/01/2014 - BT #1433 (v211 patch2)

                        'Divide the entry specimen list into several specimens list
                        Dim value As New List(Of List(Of String))
                        Dim counter As Integer = 1
                        Dim deleteOffset As Integer = maxSpecimensByMessage
                        For i As Integer = 1 To N
                            Dim subList As New List(Of String)
                            For j As Integer = 1 To pSpecimenList.Count
                                subList.Add(pSpecimenList(j - 1))
                                specimensInMessage &= pSpecimenList(j - 1) & ", " 'AG 02/01/2014 - BT #1433 (v211 patch2)
                                If j = maxSpecimensByMessage Then Exit For
                            Next
                            If pSpecimenList.Count > 0 Then
                                If pSpecimenList.Count < deleteOffset Then deleteOffset = pSpecimenList.Count
                                pSpecimenList.RemoveRange(0, deleteOffset)
                            End If

                            value.Add(subList)
                            'AG 02/01/2014 - BT #1433 (v211 patch2)
                            myLogAcciones.CreateLogActivity("HQ message with specimens: " & specimensInMessage, "ESBusiness.DivideIntoSeveralHostQueryMessages", EventLogEntryType.Information, False)
                            specimensInMessage = String.Empty
                            'AG 02/01/2014 - BT #1433
                        Next

                        resultData.SetDatos = value

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.DivideIntoSeveralHostQueryMessages", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Evaluate the configuration settings and the current LIS connection and analyzer status and decides if the required action is allowed or not
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAction"></param>
        ''' <param name="pRunningFlag"></param>
        ''' <param name="pConnectingFlag"></param>
        ''' <param name="pLISConnectionStatus"></param>
        ''' <param name="pStorageValues"></param>
        ''' <param name="pPendingOrdersCount"></param>
        ''' <returns>Boolean + parameter by reference</returns>
        ''' <remarks>AG 26/04/2013
        ''' AG 07/05/2013 - Change ToUpper for ToUpperBS (ToUpperInvariant)</remarks>
        Public Function AllowLISAction(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAction As GlobalEnumerates.LISActions, ByVal pRunningFlag As Boolean, ByVal pConnectingFlag As Boolean,
                                       ByVal pLISConnectionStatus As String, ByVal pStorageValues As String, Optional ByRef pPendingOrdersCount As Integer = 0) As Boolean
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim toReturn As Boolean = False

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        '0- Read the required settings:
                        Dim liscommsEnable As Boolean = False
                        Dim AcceptDownloadInRunning As Boolean = True

                        Dim userSettings As New UserSettingsDelegate
                        resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            liscommsEnable = CType(resultData.SetDatos, Boolean)
                        End If
                        resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_DOWNLOAD_ONRUNNING.ToString)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            AcceptDownloadInRunning = CType(resultData.SetDatos, Boolean)
                        End If

                        '1- Read LIS connection status and storage
                        Dim lisConnectionStatusOK As Boolean = False
                        If pLISConnectionStatus.ToUpperBS() = GlobalEnumerates.LISStatus.connectionAccepted.ToString.ToUpperBS() OrElse pLISConnectionStatus.ToUpperBS() = GlobalEnumerates.LISStatus.connectionEnabled.ToString.ToUpperBS() Then
                            lisConnectionStatusOK = True
                        End If
                        Dim lisStorageWarnings As Boolean = True
                        If pStorageValues = "0" Then lisStorageWarnings = False

                        Select Case pAction
                            Case LISActions.ConnectWitlhLIS
                                'Lis comms enabled
                                toReturn = liscommsEnable

                            Case LISActions.QueryAll_MDIButton
                                'Lis comms enabled, in running if accept download in running
                                toReturn = liscommsEnable
                                If toReturn AndAlso pRunningFlag Then
                                    toReturn = AcceptDownloadInRunning
                                End If

                                'DL 10/05/2013. LIS connected and not storage warnings
                                If toReturn AndAlso (Not lisConnectionStatusOK OrElse lisStorageWarnings) Then
                                    toReturn = False
                                End If
                                'DL 10/05/2013

                            Case LISActions.HostQuery_MDIButton
                                'Lis comms enabled, in running if accept download in running
                                toReturn = liscommsEnable
                                If toReturn AndAlso pRunningFlag Then
                                    toReturn = AcceptDownloadInRunning
                                    'TO CONFIRM: remove previous rule
                                End If

                                'DL 10/05/2013. LIS connected and not storage warnings
                                If toReturn AndAlso (Not lisConnectionStatusOK OrElse lisStorageWarnings) Then
                                    toReturn = False
                                End If
                                'DL 10/05/2013

                            Case LISActions.OrdersDownload_MDIButton
                                'Lis comms enabled and no lisStorageWarnings. In running if accept download in running
                                If liscommsEnable AndAlso Not lisStorageWarnings Then
                                    toReturn = True
                                End If

                                If toReturn AndAlso pRunningFlag Then
                                    toReturn = AcceptDownloadInRunning
                                End If

                                If toReturn Then
                                    'To enable validate if there is at least one pending message in table twksXmlMessages
                                    toReturn = False 'Set to FALSE and evaluate if there is something to download

                                    Dim myXmlDelegate As New xmlMessagesDelegate
                                    resultData = myXmlDelegate.ReadByStatus(dbConnection, "PENDING")
                                    Dim xmlPending As Integer = DirectCast(resultData.SetDatos, List(Of XMLMessagesTO)).Count
                                    If xmlPending > 0 Then toReturn = True

                                    'Another cause to enable validate if there is at least one Saved WS From LIS
                                    Dim mySavedWSDS As SavedWSDS
                                    Dim mySavedWSDelegate As New SavedWSDelegate
                                    Dim savedLISWorkOrders As Integer = 0
                                    resultData = mySavedWSDelegate.ReadLISSavedWS(dbConnection)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        mySavedWSDS = TryCast(resultData.SetDatos, SavedWSDS)
                                        If (mySavedWSDS.tparSavedWS.Rows.Count > 0) Then
                                            savedLISWorkOrders = mySavedWSDS.tparSavedWS.Rows.Count
                                            toReturn = True
                                        End If
                                    End If

                                    'Finally if button active inform about the pending LIS workorders to process
                                    If toReturn Then
                                        pPendingOrdersCount = xmlPending + savedLISWorkOrders
                                    End If

                                End If

                            Case LISActions.HostQuery
                                'Lis comms enabled, in running if accept download in running
                                toReturn = liscommsEnable
                                If toReturn AndAlso pRunningFlag Then
                                    toReturn = AcceptDownloadInRunning
                                End If
                                'DL 10/05/2013. LIS connected and not storage warnings
                                If toReturn AndAlso (Not lisConnectionStatusOK OrElse lisStorageWarnings) Then
                                    toReturn = False
                                End If

                            Case LISActions.LISUtilities_Menu
                                'Lis comms enabled
                                toReturn = liscommsEnable


                            Case LISActions.LISUtilities_DeleteLISSavedWS
                                'Connection process not in course, not running and connected with LIS
                                If Not pConnectingFlag AndAlso Not pRunningFlag AndAlso lisConnectionStatusOK Then
                                    toReturn = True
                                End If

                                'DL 10/05/2013
                                If toReturn AndAlso lisStorageWarnings Then
                                    toReturn = False
                                End If
                                'DL 10/05/2013


                            Case LISActions.LISUtilities_ClearQueue
                                'Connection process not in course, not running and connected with LIS
                                'AG 03/05/2013: remove previous lisConnectionStatusOK rule
                                If Not pConnectingFlag AndAlso Not pRunningFlag Then
                                    toReturn = True
                                End If

                                'Active although :
                                ' - LIS NOT connected
                                ' - LIS storage warnings


                            Case LISActions.LISUtilities_TracesEnabling
                                'Always active
                                toReturn = True


                            Case LISActions.UploadResults
                                'Lis comms enabled
                                toReturn = liscommsEnable

                                'DL 10/05/2013. LIS connected and not storage warnings
                                If toReturn AndAlso (Not lisConnectionStatusOK OrElse lisStorageWarnings) Then
                                    toReturn = False
                                End If

                        End Select

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.AllowLISAction", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return toReturn
        End Function


        ''' <summary>
        ''' Filter the results to upload removing those results with incomplete LIS mapping values
        ''' Test Name required
        ''' Sample Type required
        ''' Units optional
        ''' 
        ''' Business copied from ESxmlTranslator.CreateServiceNode (the code that gets and evaluates LIS mapping values)
        ''' </summary>
        ''' <param name="pIsHistorical"></param>
        ''' <param name="pToUpload"></param>
        ''' <param name="pTestsMappingDS"></param>
        ''' <param name="pConfigMappingDS"></param>
        ''' <returns>GlobalDataTo with data as ExecutionsDS (all mapping values are valid)</returns>
        ''' <remarks>AG 29/09/2014 - Creation - Method incomplete, the current WS results is finished but not the historical results part</remarks>
        Public Function ExcludeNotMappedResults(ByVal pIsHistorical As Boolean, ByVal pToUpload As ExecutionsDS, ByVal pTestsMappingDS As AllTestsByTypeDS, ByVal pConfigMappingDS As LISMappingsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                'No connection is required

                If Not pIsHistorical Then
                    'Current WS results

                    Dim removeFlag As Boolean = False

                    'Loop in reverser order because is easier remove rows
                    For index As Integer = pToUpload.twksWSExecutions.Rows.Count - 1 To 0 Step -1
                        'Evaluate if the required mapping values are OK else remove row
                        removeFlag = False

                        resultData = ValidateLISMapping(pToUpload.twksWSExecutions(index), pTestsMappingDS, pConfigMappingDS)
                        If resultData.HasError Then
                            removeFlag = True
                        End If

                        'Remove incomplete LIS mapping results
                        If removeFlag Then
                            pToUpload.twksWSExecutions(index).Delete()
                            pToUpload.twksWSExecutions.AcceptChanges()
                        End If

                    Next

                Else
                    'Historical
                    'If not manual order --> See historical results dataset fields LISSampleType and LISTestName cannot be neither NULL neither ""
                    'If manual order --> Same business as current WS results

                    'PENDING FINISH THE BUSINESS ... READ CODE!!!

                    'Dim lnqResults As List(Of ExecutionsDS.twksWSExecutionsRow)

                    ''Historic results requested by LIS
                    'lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUpload.twksWSExecutions _
                    '              Where Not a.IsLISRequestNull AndAlso a.LISRequest Select a).ToList

                    'If lnqResults.Count > 0 Then
                    '    'Search the lnqResults contains into Historical results and discard results with invalid LISSampleType and LISTestName values
                    'End If

                    ''Historic results not requested by LIS
                    'lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUpload.twksWSExecutions _
                    '              Where a.IsLISRequestNull OrElse Not a.LISRequest Select a).ToList

                    'If lnqResults.Count > 0 Then
                    '    'Use method ValidateLISMapping and remove all results to upload without valid LIS values
                    'End If

                End If

                resultData.SetDatos = pToUpload

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.ExcludeNotMappedResults", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Evaluate if the result to upload has informed all the required LIS mapping with valid values
        ''' Test Name required
        ''' Sample Type required
        ''' Units optional
        ''' 
        ''' Business copied from ESxmlTranslator.CreateServiceNode (the code that gets and evaluates LIS mapping values)
        ''' </summary>
        ''' <param name="pToUploadRow"></param>
        ''' <param name="pTestsMappingDS"></param>
        ''' <param name="pConfigMappingDS"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ValidateLISMapping(ByVal pToUploadRow As ExecutionsDS.twksWSExecutionsRow, ByVal pTestsMappingDS As AllTestsByTypeDS, ByVal pConfigMappingDS As LISMappingsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Try
                Dim removeFlag As Boolean = False
                Dim myLISMappingDelegate As New LISMappingsDelegate
                Dim myAllTestMappingDelegate As New AllTestByTypeDelegate

                'Test Name
                resultData = myAllTestMappingDelegate.GetLISTestID(pTestsMappingDS, pToUploadRow.TestID, pToUploadRow.TestType)
                If resultData.HasError Then
                    removeFlag = True
                End If


                'Sample type
                If Not removeFlag Then
                    resultData = myLISMappingDelegate.GetLISSampleType(pConfigMappingDS, pToUploadRow.SampleType)
                    If resultData.HasError Then
                        removeFlag = True
                    End If
                End If

                'Units (not!! this field is optional)

                resultData.SetDatos = removeFlag


            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESBusiness.ValidateLISMapping", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

    End Class

End Namespace

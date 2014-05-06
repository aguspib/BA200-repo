Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL
    Partial Public Class BarcodePositionsWithNoRequestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Insert data of an incomplete Patient Sample when there is not information about the Tests to execute to it
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <param name="pBarCode">Typed Datatset BarCodeDS containing the decided Barcode information</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 29/08/2011
        ''' </remarks>
        Public Function AddPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                    ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pBarCode As BarCodesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pBarCode.DecodedSamplesFields.Rows.Count > 0) Then
                            'Read all positions in currently saved as incomplete Patient Samples
                            Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                            resultData = myDAO.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)

                            If (Not resultData.HasError) Then '(1)
                                Dim positionsWithNoTestRequests As BarcodePositionsWithNoRequestsDS = DirectCast(resultData.SetDatos, BarcodePositionsWithNoRequestsDS)

                                Dim currentExternalPID As String = ""
                                Dim currentSampleType As String = ""
                                Dim currentBarCodeInfo As String = ""

                                If (Not pBarCode.DecodedSamplesFields(0).IsExternalPIDNull) Then currentExternalPID = pBarCode.DecodedSamplesFields(0).ExternalPID
                                If (Not pBarCode.DecodedSamplesFields(0).IsSampleTypeNull) Then currentSampleType = pBarCode.DecodedSamplesFields(0).SampleType

                                '1st: If the ExternalPID and SampleType already exist in a different position ... delete OLD position
                                'AG 16/09/2011 - No!! Several tubes the same patient (sample type) is allowed using barcode
                                Dim linqRes As List(Of BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)
                                'If (currentSampleType = "") Then
                                '    linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                '              Where a.ExternalPID = currentExternalPID _
                                '            AndAlso a.CellNumber <> pCellNumber _
                                '             Select a).ToList
                                'Else
                                '    linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                '              Where a.ExternalPID = currentExternalPID _
                                '            AndAlso a.SampleType = currentSampleType _
                                '            AndAlso a.CellNumber <> pCellNumber _
                                '             Select a).ToList
                                'End If

                                'If (linqRes.Count > 0) Then
                                '    For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In linqRes
                                '        resultData = myDAO.DeletePosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, row.CellNumber)
                                '        If (resultData.HasError) Then Exit For
                                '    Next
                                'End If

                                '2nd: If current position is used for other ExternalPID ... delete position
                                If (Not resultData.HasError) Then
                                    If (currentSampleType = "") Then
                                        linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                                  Where a.ExternalPID <> currentExternalPID _
                                                AndAlso a.CellNumber = pCellNumber _
                                                 Select a).ToList
                                    Else
                                        linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                                  Where (a.ExternalPID <> currentExternalPID OrElse (a.ExternalPID = currentExternalPID AndAlso a.SampleType <> currentSampleType)) _
                                                AndAlso a.CellNumber = pCellNumber _
                                                 Select a).ToList
                                    End If

                                    If (linqRes.Count > 0) Then
                                        For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In linqRes
                                            resultData = myDAO.DeletePosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, row.CellNumber)
                                            If (resultData.HasError) Then Exit For
                                        Next
                                    End If
                                End If

                                '3rd: Finally add information into current cell position (only if previous information has different ExternalPID than current in parameters)
                                If (Not resultData.HasError) Then
                                    If (currentSampleType = "") Then
                                        linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                                  Where a.ExternalPID = currentExternalPID _
                                                AndAlso a.CellNumber = pCellNumber _
                                                 Select a).ToList
                                    Else
                                        linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                                  Where a.ExternalPID = currentExternalPID _
                                                AndAlso a.SampleType = currentSampleType _
                                                AndAlso a.CellNumber = pCellNumber _
                                                 Select a).ToList
                                    End If

                                    If (linqRes.Count = 0) Then
                                        '30/04/2013 JCM
                                        Dim lisStatus As String = ""
                                        Dim messageId As String = ""

                                        '4th: Check if on current twksWSBarcodePositionsWithNoRequests there are Patient Samples with the same BarCode and LISStatus=ASKING
                                        resultData = myDAO.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)
                                        If (Not resultData.HasError) Then
                                            If (String.IsNullOrEmpty(currentSampleType)) Then
                                                linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                                          Where a.AnalyzerID = pAnalyzerID _
                                                        AndAlso a.WorkSessionID = pWorkSessionID _
                                                        AndAlso a.BarCodeInfo = pBarCode.DecodedSamplesFields(0).BarcodeInfo _
                                                        AndAlso a.LISStatus = "ASKING" _
                                                        AndAlso (a.IsSampleTypeNull OrElse String.IsNullOrEmpty(a.SampleType)) _
                                                         Select a).ToList

                                            Else
                                                linqRes = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In positionsWithNoTestRequests.twksWSBarcodePositionsWithNoRequests _
                                                          Where a.AnalyzerID = pAnalyzerID _
                                                        AndAlso a.WorkSessionID = pWorkSessionID _
                                                        AndAlso a.BarCodeInfo = pBarCode.DecodedSamplesFields(0).BarcodeInfo _
                                                        AndAlso a.LISStatus = "ASKING" _
                                                        AndAlso (Not a.IsSampleTypeNull AndAlso Not String.IsNullOrEmpty(a.SampleType)) _
                                                        AndAlso a.SampleType = currentSampleType
                                                         Select a).ToList
                                            End If

                                            If (linqRes.Count > 0) Then
                                                lisStatus = linqRes(0).LISStatus
                                                messageId = linqRes(0).MessageId
                                            End If
                                        End If

                                        Dim newRecordDS As New BarcodePositionsWithNoRequestsDS
                                        Dim newRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow

                                        newRow = newRecordDS.twksWSBarcodePositionsWithNoRequests.NewtwksWSBarcodePositionsWithNoRequestsRow
                                        With newRow
                                            .AnalyzerID = pAnalyzerID
                                            .WorkSessionID = pWorkSessionID
                                            .RotorType = pRotorType
                                            .CellNumber = pCellNumber
                                            .ExternalPID = currentExternalPID
                                            .SampleType = currentSampleType
                                            If (Not pBarCode.DecodedSamplesFields(0).IsPatientIDNull) Then .PatientID = pBarCode.DecodedSamplesFields(0).PatientID
                                            If (Not pBarCode.DecodedSamplesFields(0).IsBarcodeInfoNull) Then .BarCodeInfo = pBarCode.DecodedSamplesFields(0).BarcodeInfo
                                            If (Not String.IsNullOrEmpty(lisStatus)) Then .LISStatus = lisStatus
                                            If (Not String.IsNullOrEmpty(messageId)) Then .MessageId = messageId
                                        End With

                                        newRecordDS.twksWSBarcodePositionsWithNoRequests.AddtwksWSBarcodePositionsWithNoRequestsRow(newRow)
                                        newRecordDS.AcceptChanges()

                                        resultData = myDAO.Create(dbConnection, newRecordDS)
                                    End If
                                End If
                            End If 'If Not resultData.HasError Then '(1)
                        End If 'If pBarCode.DecodedSamplesFields.Rows.Count > 0 Then

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.AddPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For a Barcode saved as incomplete Patient Sample, search if it can be linked to a Patient Sample that exists as required Element in the active WS. 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBCPosWithNoRequest">List of rows of a typed DataSet BarcodePositionsWithNoRequestDS containing the information of one or several tubes
        '''                                    of a scanned Patient Sample marked as incomplete</param>
        ''' <param name="pUpdatePosition">When TRUE, it indicates the Samples Rotor Position has to be updated with the information if a required Element linked
        '''                               to the Barcode has been found. When false, the position is not updated</param>
        ''' <param name="pNumSampleTypes">Number of different Sample Types that exists for the informed Barcode. When it is not informed (its value is 0), the
        '''                               value is searched in this function by calling function CountSampleTypesByBC in DAO Class</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS containing all data of the required Element linked to the Barcode</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2013
        ''' Modified by: SA 30/05/2013 - Added parameter pNumSampleTypes to indicate the number of different Sample Types that exists for the same Barcode 
        '''              SA 06/06/2013 - When parameter pNumSampleTypes is zero and function CountSampleTypesByBC is called but returns also zero, pNumSampleTypes 
        '''                              is set to one, due to, although still not saved, there is at least the SampleType in process
        '''              SA 11/06/2013 - When calling function AddWorkSession in WorkSessionsDelegate, inform optional parameter for the status of the active WS with
        '''                              value of field WSStatus from the first row of parameter pBCPosWithNoRequest
        '''              SA 25/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions. When value of global flag 
        '''                                           NEWAddWorkSession is TRUE, call new version of function AddWorkSession
        ''' </remarks>
        Public Function CheckIncompletedPatientSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBCPosWithNoRequest As List(Of BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow), _
                                                      ByVal pUpdatePosition As Boolean, Optional ByVal pNumSampleTypes As Integer = 0) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myElementID As Integer = -1
                        Dim inUsePos As Boolean = False

                        'Get the first row in the entry DS
                        Dim myBCPosWithNoRequestRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow = pBCPosWithNoRequest.First

                        'Verify if there are Order Tests requested by LIS in the active WS for the Barcode (search SpecimenID = Barcode)
                        Dim myPatientID As String = String.Empty
                        Dim searchElement As Boolean = False
                        Dim searchSampleType As String = String.Empty
                        Dim myOTLISInfoDelegate As New OrderTestsLISInfoDelegate

                        myGlobalDataTO = myOTLISInfoDelegate.GetOrderTestBySpecimenID(dbConnection, myBCPosWithNoRequestRow.BarCodeInfo)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myOrderTestsDS As OrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                            If (myOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                'Get the PatientID/SampleID to which the Order Tests belongs (use values in the first row, 
                                'a Barcode cannot be linked to more than one Patient)
                                If (Not myOrderTestsDS.twksOrderTests.First.IsPatientIDNull) Then
                                    myPatientID = myOrderTestsDS.twksOrderTests.First.PatientID
                                Else
                                    myPatientID = myOrderTestsDS.twksOrderTests.First.SampleID
                                End If

                                If (Not String.IsNullOrEmpty(myBCPosWithNoRequestRow.SampleType)) Then
                                    'THE SAMPLE TYPE IS INFORMED FOR THE BARCODE
                                    'Verify if the SampleType exists in the list of Order Tests requested by LIS
                                    Dim myLinq1 As List(Of OrderTestsDS.twksOrderTestsRow) = (From a As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests _
                                                                                             Where a.SampleType = myBCPosWithNoRequestRow.SampleType _
                                                                                            Select a).ToList()
                                    If (myLinq1.Count > 0) Then
                                        searchElement = True
                                        searchSampleType = myBCPosWithNoRequestRow.SampleType
                                    Else
                                        'If there are not Order Tests for the specific SampleType, get all different SampleTypes in the group of Order Tests
                                        Dim myLinq2 As List(Of String) = (From b In myOrderTestsDS.twksOrderTests _
                                                                        Select b.SampleType Distinct).ToList()

                                        If (myLinq2.Count = 1) Then
                                            If (pNumSampleTypes = 0) Then
                                                'If the parameter has not been informed, count the number of different SampleTypes for the BC here...
                                                Dim twksWSBCPosWithNoRequestsDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                                                myGlobalDataTO = twksWSBCPosWithNoRequestsDAO.CountSampleTypesByBC(dbConnection, myBCPosWithNoRequestRow)

                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    pNumSampleTypes = CType(myGlobalDataTO.SetDatos, Integer)

                                                    'There is not Sample Type saved, but there is at least the one being processed
                                                    If (pNumSampleTypes = 0) Then pNumSampleTypes = 1
                                                End If
                                            End If

                                            'If there is only a SampleType between all OrderTests requested by LIS, the SampleType in the Barcode is ignored 
                                            'and replaced by the SampleType requested by LIS, but only when there is an unique Sample Type for the Barcode 
                                            If (pNumSampleTypes = 1) Then
                                                searchElement = True
                                                searchSampleType = myLinq2.First
                                            End If
                                        End If

                                        myLinq2 = Nothing
                                        myLinq1 = Nothing
                                    End If
                                Else
                                    'THE SAMPLE TYPE IS NOT INFORMED FOR THE BARCODE
                                    'Get all different SampleTypes in the group of Order Tests requested by LIS
                                    Dim myLinq1 As List(Of String) = (From b In myOrderTestsDS.twksOrderTests _
                                                                    Select b.SampleType Distinct).ToList()

                                    If (myLinq1.Count = 1) Then
                                        If (pNumSampleTypes = 0) Then
                                            'If the parameter has not been informed, count the number of different SampleTypes for the BC here...
                                            Dim twksWSBCPosWithNoRequestsDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                                            myGlobalDataTO = twksWSBCPosWithNoRequestsDAO.CountSampleTypesByBC(dbConnection, myBCPosWithNoRequestRow)

                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                pNumSampleTypes = CType(myGlobalDataTO.SetDatos, Integer)

                                                'There is not Sample Type saved, but there is at least the one being processed
                                                If (pNumSampleTypes = 0) Then pNumSampleTypes = 1
                                            End If
                                        End If

                                        'If there is only a SampleType between all OrderTests requested by LIS, the SampleType in the Barcode is replaced 
                                        'by the SampleType requested by LIS, but only when there is an unique Sample Type for the Barcode 
                                        If (pNumSampleTypes = 1) Then
                                            searchElement = True
                                            searchSampleType = myLinq1.First
                                        End If
                                    End If
                                    myLinq1 = Nothing
                                End If
                            Else
                                'If there are not Order Tests requested by LIS for the full Barcode, search if there are Order Tests for the PatientID/SampleID 
                                'specified in the Barcode - Search all different SampleTypes requested for the PatientID/SampleID in the active WS
                                myPatientID = myBCPosWithNoRequestRow.ExternalPID
                                'myPatientID = myBCPosWithNoRequestRow.BarCodeInfo

                                Dim myOrderTestsDelegate As New OrderTestsDelegate
                                myGlobalDataTO = myOrderTestsDelegate.GetSampleTypesByPatient(dbConnection, myBCPosWithNoRequestRow.WorkSessionID, myPatientID, False)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                    If (Not String.IsNullOrEmpty(myBCPosWithNoRequestRow.SampleType)) Then
                                        'THE SAMPLE TYPE IS INFORMED FOR THE BARCODE
                                        'Verify if the SampleType exists in the list of Sample Types
                                        Dim myLinq1 As List(Of OrderTestsDS.twksOrderTestsRow) = (From a As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests _
                                                                                                 Where a.SampleType = myBCPosWithNoRequestRow.SampleType _
                                                                                                Select a).ToList()
                                        If (myLinq1.Count > 0) Then
                                            searchElement = True
                                            searchSampleType = myBCPosWithNoRequestRow.SampleType
                                        Else
                                            'If there is only a SampleType for the PatientID/SampleID in the active WS...
                                            If (myOrderTestsDS.twksOrderTests.Rows.Count = 1) Then
                                                If (pNumSampleTypes = 0) Then
                                                    'If the parameter has not been informed, count the number of different SampleTypes for the BC here...
                                                    Dim twksWSBCPosWithNoRequestsDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                                                    myGlobalDataTO = twksWSBCPosWithNoRequestsDAO.CountSampleTypesByBC(dbConnection, myBCPosWithNoRequestRow)

                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        pNumSampleTypes = CType(myGlobalDataTO.SetDatos, Integer)

                                                        'There is not Sample Type saved, but there is at least the one being processed
                                                        If (pNumSampleTypes = 0) Then pNumSampleTypes = 1
                                                    End If
                                                End If

                                                'If there is only a SampleType between all OrderTests, the SampleType in the Barcode is replaced by that Sample 
                                                'Type, but only when there is an unique Sample Type for the Barcode 
                                                If (pNumSampleTypes = 1) Then
                                                    searchElement = True
                                                    searchSampleType = myOrderTestsDS.twksOrderTests.First.SampleType
                                                End If
                                            End If
                                        End If
                                        myLinq1 = Nothing
                                    Else
                                        'THE SAMPLE TYPE IS NOT INFORMED FOR THE BARCODE
                                        If (myOrderTestsDS.twksOrderTests.Rows.Count = 1) Then
                                            If (pNumSampleTypes = 0) Then
                                                'If the parameter has not been informed, count the number of different SampleTypes for the BC here...
                                                Dim twksWSBCPosWithNoRequestsDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                                                myGlobalDataTO = twksWSBCPosWithNoRequestsDAO.CountSampleTypesByBC(dbConnection, myBCPosWithNoRequestRow)

                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    pNumSampleTypes = CType(myGlobalDataTO.SetDatos, Integer)

                                                    'There is not Sample Type saved, but there is at least the one being processed
                                                    If (pNumSampleTypes = 0) Then pNumSampleTypes = 1
                                                End If
                                            End If

                                            'If there is only a SampleType between all OrderTests, the SampleType in the Barcode is replaced by that Sample 
                                            'Type, but only when there is an unique Sample Type for the Barcode 
                                            If (pNumSampleTypes = 1) Then
                                                searchElement = True
                                                searchSampleType = myOrderTestsDS.twksOrderTests.First.SampleType
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            Dim myWSRequiredElemDS As New WSRequiredElementsDS
                            Dim myWSRequiredElemDelegate As New WSRequiredElementsDelegate
                            If (searchElement) Then
                                'Verify if there is a required Element in the WS for the PatientID/SampleID and the SampleType
                                myGlobalDataTO = myWSRequiredElemDelegate.GetByPatientAndSampleType(dbConnection, myBCPosWithNoRequestRow.WorkSessionID, myPatientID, _
                                                                                                    searchSampleType)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myWSRequiredElemDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                                    If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count = 1) Then
                                        myElementID = myWSRequiredElemDS.twksWSRequiredElements.First.ElementID
                                        inUsePos = True
                                    Else
                                        'Get all Order Tests requested in the WS for the PatientID/SampleID and the SampleType that have not been sent to positioning yet
                                        'NOTE: this part of the code can be executed only in two cases:
                                        '      (1) When the function has been called from function BarcodeWSDelegate.SaveOKReadSamplesRotorPosition; i.e. when a position 
                                        '          in Samples Rotor has been scanned, if there are Order Tests for it but the required Element has not been created yet
                                        '          (the Status of the Order Tests is OPEN), the active WorkSession is updated by sending the Order Tests to positioning
                                        '          and creating the required Element for the Patient Sample and also the Elements for the needed Reagents, Additional 
                                        '          Solutions, and Calibrators
                                        '      (2) When the function has been called from function WorkSessionsDelegate.PrepareOrderTestsForWS (in an special flow in which
                                        '          the AddWorkSession function is not called)
                                        Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                                        myGlobalDataTO = myWSOrderTestsDelegate.GetByPatientAndSampleType(dbConnection, myBCPosWithNoRequestRow.WorkSessionID, myPatientID, _
                                                                                                          searchSampleType)

                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim myWSOrderTestsDS As WSOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, WSOrderTestsDS)

                                            If (myWSOrderTestsDS.twksWSOrderTests.Rows.Count > 0) Then
                                                Dim myWSDelegate As New WorkSessionsDelegate
                                                If (GlobalConstants.NEWAddWorkSession) Then
                                                    'BT #1545
                                                    myGlobalDataTO = myWSDelegate.AddWorkSession_NEW(dbConnection, myWSOrderTestsDS, False, myBCPosWithNoRequestRow.AnalyzerID, myBCPosWithNoRequestRow.WSStatus, False)
                                                Else
                                                    myGlobalDataTO = myWSDelegate.AddWorkSession(dbConnection, myWSOrderTestsDS, False, myBCPosWithNoRequestRow.AnalyzerID, myBCPosWithNoRequestRow.WSStatus, False)
                                                End If

                                                If (Not myGlobalDataTO.HasError) Then
                                                    'Search the added required Element for the PatientID/SampleID and SampleType
                                                    myGlobalDataTO = myWSRequiredElemDelegate.GetByPatientAndSampleType(dbConnection, myBCPosWithNoRequestRow.WorkSessionID, myPatientID, _
                                                                                                                        searchSampleType)
                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        myWSRequiredElemDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                                                        If (myWSRequiredElemDS.twksWSRequiredElements.Rows.Count = 1) Then
                                                            myElementID = myWSRequiredElemDS.twksWSRequiredElements.First.ElementID
                                                            inUsePos = True
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            If (pUpdatePosition AndAlso inUsePos) Then
                                Dim myWSRotorPosDS As New WSRotorContentByPositionDS
                                Dim myWSRotorPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                                For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In pBCPosWithNoRequest
                                    'Prepare information to update information of the Rotor Cell
                                    myWSRotorPosRow = myWSRotorPosDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                    myWSRotorPosRow.AnalyzerID = row.AnalyzerID
                                    myWSRotorPosRow.WorkSessionID = row.WorkSessionID
                                    myWSRotorPosRow.RotorType = row.RotorType
                                    myWSRotorPosRow.RingNumber = row.RingNumber
                                    myWSRotorPosRow.CellNumber = row.CellNumber
                                    myWSRotorPosRow.ElementID = myElementID
                                    myWSRotorPosRow.Status = "PENDING"
                                    myWSRotorPosDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myWSRotorPosRow)
                                Next

                                'Update the information of the Cell in Samples Rotor, and delete the Cell from the list of NOT IN USE positions
                                Dim myWSRotorPosDelegate As New WSRotorContentByPositionDelegate

                                myGlobalDataTO = myWSRotorPosDelegate.UpdateNotInUseRotorPosition(dbConnection, myWSRotorPosDS)
                                If (Not myGlobalDataTO.HasError) Then
                                    'Update the Status of the required Element to POSITIONED
                                    myGlobalDataTO = myWSRequiredElemDelegate.UpdateStatus(dbConnection, myElementID, "POS")
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'Finally, delete all tubes with the same Barcode and Sample Type from the list of incomplete Patient Samples
                                    For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In pBCPosWithNoRequest
                                        myGlobalDataTO = DeletePosition(dbConnection, row.AnalyzerID, row.WorkSessionID, row.RotorType, row.CellNumber)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Next
                                End If
                            End If

                            'Finally, return a WSRequiredElementsDS with all data of the required Element that was linked to the Barcode,
                            'or an empty WSRequiredElementsDS if a required Element was not linked to the Barcode
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO.SetDatos = myWSRequiredElemDS
                                myGlobalDataTO.HasError = False
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.CheckIncompletePatientSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When the list of Tests to execute is received from LIS, mark as complete the Patient Samples, updating also field
        ''' PatientID with the Patient Identifier from the application DB. The update is executed first by ExternalPID and Sample
        ''' Type, but if no record were updated then it is executed by ExternalPID and SampleType NULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientData">Row of typed DataSet SavedWSOrderTestsDS containing data of the Patient sent from LIS</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 26/09/2011
        ''' </remarks>
        Public Function CompletePatientSamplesFromLIS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                      ByVal pPatientData As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBCPosWithNoRequestDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myBCPosWithNoRequestDAO.CompleteByExternalPIDAndSampleType(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", pPatientData.ExternalPID, _
                                                                                                pPatientData.SampleType, pPatientData.SampleID, True)
                        If (Not resultData.HasError) Then
                            If (resultData.AffectedRecords = 0) Then
                                resultData = myBCPosWithNoRequestDAO.CompleteByExternalPIDAndSampleType(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", pPatientData.ExternalPID, _
                                                                                                        pPatientData.SampleType, pPatientData.SampleID, False)
                            End If
                        End If
                    End If
                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateRelatedIncompletedSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deletes a cell number position from the table twksWSBarcodePositionsWithNoRequest due:
        ''' 1) Scanning of position returns NO READ and no there is not previous information for it added manually 
        ''' 2) ExternalSampleID read exists in WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pCellNumber">Cell Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 29/08/2011
        ''' </remarks>
        Public Function DeletePosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                       ByVal pRotorType As String, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myDAO.DeletePosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCellNumber)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.DeletePosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all elements with Barcode informed and not in use on the WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with all information of the 
        '''          incomplete Patient Samples returned</returns>
        ''' <remarks>
        ''' Created by: TR 07/05/2013
        ''' </remarks>
        Public Function GetScannedAndNotInUseElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
                        myGlobalDataTO = myBarcodePositionsWithNoRequests.GetScannedAndNotInUseElements(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ReadByAnalizerAndWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update Info of Sample requested to HQ
        ''' </summary>
        ''' <param name="pDBConnection ">Open DB Connection</param>
        ''' <param name="pBarcodePositionsWithNoRequestsRow">DataRow</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionsDS containing status of the active Work Session after updating
        '''          it by adding new Order Tests 
        ''' </returns>
        ''' <remarks>
        ''' Created by:  JC 08/04/2013
        ''' Modified by: SA 28/05/2013 - When field SampleType is informed, update it also for the cell in table of Not In Use Rotor Positions
        ''' </remarks>
        Public Function HQUpdatePatientRequestedSamples(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                        ByVal pBarcodePositionsWithNoRequestsRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myDAO.UpdateSamples(dbConnection, pBarcodePositionsWithNoRequestsRow)

                        If (Not resultData.HasError) Then
                            Dim mySampleType As String = String.Empty
                            If (Not pBarcodePositionsWithNoRequestsRow.IsSampleTypeNull) Then mySampleType = pBarcodePositionsWithNoRequestsRow.SampleType

                            Dim myWSNotInUseRotorPositionsDelegate As New WSNotInUseRotorPositionsDelegate
                            resultData = myWSNotInUseRotorPositionsDelegate.UpdateSampleType(dbConnection, pBarcodePositionsWithNoRequestsRow.AnalyzerID, _
                                                                                             pBarcodePositionsWithNoRequestsRow.WorkSessionID, _
                                                                                             pBarcodePositionsWithNoRequestsRow.RotorType, _
                                                                                             pBarcodePositionsWithNoRequestsRow.CellNumber, _
                                                                                             mySampleType)
                        End If
                    End If
                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.HQUpdatePatientRequestedSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add the group of completed Patient Samples (plus all the required elements) to the active WorkSession and 
        ''' deleted them from the table of Incomplete Patient Samples
        ''' </summary>
        ''' <param name="pWorkSessionID">Identifier of the WorkSession</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer</param>
        ''' <param name="pWorkSessionStatus">Current WorkSession Status</param>
        ''' <param name="pWorkSessionResultDS">Typed DataSet WorkSessionResultDS containing all OrderTests that have
        '''                                    to be added to the active WorkSession</param>
        ''' <param name="pBarcodePositionsWithNoRequestsDS">Typed DataSet BarcodePositionsWithNoRequestsDS containing the list of 
        '''                                                 Incomplete Patient Samples that have been completed</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionsDS containing status of the active Work Session after updating
        '''          it by adding new Order Tests 
        ''' </returns>
        ''' <remarks>
        ''' Created by:  DL 01/09/2011 
        ''' Modified by: SA 13/09/2011 - Removed returning of typed DS WSRequiredElementsDS; it is not needed. Delete completed Patient
        '''                              Samples filtering by ExternalPID and SampleType instead of by CellNumber (due to the same 
        '''                              ExternalPID/SampleType can be placed in more than one Rotor Position
        '''              SA 19/09/2011 - Added new parameter to indicate if the Incompleted Patient Samples have to be deleted or marked
        '''                              as completed but not deleted.  Additionally, delete/complete Samples by ExternalID/SampleType or
        '''                              by Position depending on if the CellNumber is informed or not
        '''              SA 21/09/2011 - Removed parameter pDeleteSamples and the calls to function for deleting Incomplete Samples;
        '''                              first update values in table of Incomplete Samples and after that call PrepareOrderTestsForWS
        '''              SA 07/10/2011 - For incomplete Patient Samples with not SampleType informed, after update the CompletedFlag, update the 
        '''                              selected SampleType in the rotor position saved as NOT IN USE.  Delete all incomplete Patient Samples
        '''                              marked as Completed. Finally, if the active WS was not updated (due to the Completed Patient Samples were
        '''                              already included in it), call function FindElementsInNotInUsePosition.
        '''              SA 10/05/2012 - Delete of all incomplete Patient Samples that have been marked as completed has to be executed only
        '''                              when the auxiliary screen was opened for START WS Button or from the WS Rotor Positioning Screen, but 
        '''                              not when it is opened from WS Samples Request Screen (that is, when parameter pWorkSessionResultDS 
        '''                              is nothing)  
        ''' </remarks>
        Public Function ProcessCompletedPatientSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                       ByVal pWorkSessionStatus As String, ByVal pWorkSessionResultDS As WorkSessionResultDS, _
                                                       ByVal pBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        Dim myWSNotInUseRotorPositionsDelegate As New WSNotInUseRotorPositionsDelegate

                        For Each myRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In pBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests
                            If (myRow.CellNumber = 0) Then
                                'Mark as completed all positions that correspond to the same ExternalPID/SampleType in the table of Incomplete Patient Samples
                                resultData = myDAO.CompleteByExternalPIDAndSampleType(dbConnection, pAnalyzerID, pWorkSessionID, myRow.RotorType, myRow.ExternalPID, myRow.SampleType)
                            Else
                                'Mark as completed the specified position from the table of Incomplete Patient Samples
                                resultData = myDAO.UpdateCompletedFlagByPosition(dbConnection, pAnalyzerID, pWorkSessionID, myRow.RotorType, myRow.CellNumber, True, myRow.SampleType)

                                If (Not resultData.HasError) Then
                                    If (Not myRow.IsSampleTypeNull AndAlso myRow.NotSampleType) Then
                                        'For those incomplete Patient Samples that had not informed the SampleType originally, search the position in table  
                                        'of Not InUse Rotor Positions and update it with the informed SampleType
                                        resultData = myWSNotInUseRotorPositionsDelegate.UpdateSampleType(dbConnection, pAnalyzerID, pWorkSessionID, myRow.RotorType, _
                                                                                                         myRow.CellNumber, myRow.SampleType)
                                    End If
                                End If
                            End If

                            If (resultData.HasError) Then Exit For
                        Next myRow

                        If (Not resultData.HasError) Then
                            'Only when the process is executed from START Button or from the screen of Rotor Positioning, delete all incomplete 
                            'Patient Samples that have been marked as completed
                            If (Not pWorkSessionResultDS Is Nothing) Then
                                resultData = myDAO.DeleteCompletedSamples(dbConnection, pAnalyzerID, pWorkSessionID)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'Only when the process is executed from START Button or from the screen of Rotor Positioning, save the WS
                            Dim wsDS As New WorkSessionsDS
                            If (Not pWorkSessionResultDS Is Nothing) Then
                                Dim myWSDelegate As New WorkSessionsDelegate
                                resultData = myWSDelegate.PrepareOrderTestsForWS(dbConnection, pWorkSessionResultDS, False, pWorkSessionID, pAnalyzerID, pWorkSessionStatus, True, True)

                                If (Not resultData.HasError) Then
                                    'If the WS was not updated, then search if there are NOT INUSE positions that are now INUSE
                                    If (DirectCast(resultData.SetDatos, WorkSessionsDS).twksWorkSessions.Rows.Count = 0) Then
                                        resultData = myWSDelegate.FindElementsInNotInUsePosition(dbConnection, pWorkSessionID, pAnalyzerID, True)
                                        If (Not resultData.HasError) Then resultData.SetDatos = wsDS
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ProcessCompletedPatientSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Samples Barcodes with LISStatus = ASKING (a Host Query was sent to LIS for them) for which LIS has not sent information or it has 
        ''' not sent any accepted Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with the list of Samples Barcodes with LISStatus = ASKING
        '''          (a Host Query was sent to LIS for them) for which LIS has not sent information or it has not sent any accepted Order Test</returns>
        ''' <remarks>
        ''' Created by:  SA 04/07/2013
        ''' </remarks>
        Public Function ReadAskingBCNotSentByLIS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myBarcodePositionsWithNoRequests.ReadAskingBCNotSentByLIS(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ReadAskingBCNotSentByLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all positions with incomplete data in the specified Analyzer, WorkSession and, optionally RotorType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Type of Rotor. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with "incomplete" cells</returns>
        ''' <remarks>
        ''' Created by:  DL  03/08/2011
        ''' Modified by: SA  05/08/2011 - Changed the function template (it has the one for Insert/Update/Delete, not the one for Selects)
        '''              JCI 25/03/2013 - Get Icons for Stat/Routine and load them in the DS to return according value of field StatFlag
        '''              SA  09/04/2013 - Added new optional parameter pForLoadScreen to indicate the function is used to load data in an screen
        '''                               (when True), and in that case the icons for Stat/Routine are obtained and loaded in the DS to return
        ''' </remarks>
        Public Function ReadByAnalyzerAndWorkSession(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                     Optional ByVal pRotorType As String = "", Optional ByVal pForLoadScreen As Boolean = True) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
                        myGlobalDataTO = myBarcodePositionsWithNoRequests.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)

                        If (Not myGlobalDataTO.HasError) Then
                            If (pForLoadScreen) Then
                                Dim myBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)

                                'Get icons used for STAT and ROUTINE Patient Samples  
                                Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                                Dim STATIcon As Byte() = preloadedDataConfig.GetIconImage("STATS")
                                Dim NORMALIcon As Byte() = preloadedDataConfig.GetIconImage("ROUTINES")

                                'Load the corresponding ICON in the DS to return according value of field StatFlag
                                For Each bcpRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows
                                    If (bcpRow.StatFlag) Then
                                        bcpRow.SampleClassIcon = STATIcon
                                    Else
                                        bcpRow.SampleClassIcon = NORMALIcon
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ReadByAnalizerAndWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all positions with incomplete data in the specified Analyzer, WorkSession and optionally, RotorType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Type of Rotor. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarcodePositionsWithNoRequestsDS with "incomplete" cells</returns>
        ''' <remarks>
        ''' Created by:  SA 13/09/2011
        ''' </remarks>
        Public Function ReadDistinctPatientSamples(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   Optional ByVal pRotorType As String = "") As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
                        myGlobalDataTO = myBarcodePositionsWithNoRequests.ReadDistinctPatientSamples(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ReadDistinctPatientSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all records from the table twksWSBarcodePositionsWithNoRequest by Analyzer, WorkSession and Rotor Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 15/09/2011 
        ''' Modified by: SA 10/04/2013 - Open a DB Transaction instead of a DB Connection: this is not a query function 
        ''' </remarks>
        Public Function ResetRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO()
                        resultData = myDAO.ResetRotor(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ResetRotor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deletes a session from the table twksWSBarcodePositionsWithNoRequest due:
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 05/09/2011
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myDAO.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For all samples in table of Incompleted Patient Samples that have been reviewed and value of field CompletedFlag has 
        ''' been changed for them, update the field in the table. Additionally, for those Samples marked again as incompleted that
        ''' have not informed the SampleType originally, set also SampleType to NULL. 
        ''' Before delete the "completed" Samples, for those that have not informed the SampleType originally, search the position
        ''' in table of Not InUse Rotor Positions and update it with the informed SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pIncompleteSamplesDS">Typed DataSet BarcodePositionsWithNoRequestsDS containing a group of samples from table of Incompleted 
        '''                                    Patient Samples that have been reviewed and value of field CompletedFlag has been changed for them</param>
        ''' <param name="pDeleteCompleted">When TRUE, it indicates all samples marked as Completed, will be deleted</param>
        ''' <param name="pSearchByBarcodeInfo"></param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2011
        ''' </remarks>
        Public Function UpdateCompletedFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                            ByVal pIncompleteSamplesDS As BarcodePositionsWithNoRequestsDS, ByVal pDeleteCompleted As Boolean, _
                                            Optional ByVal pSearchByBarcodeInfo As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Incompleted Samples which CompletedFlag has been changed
                        Dim lstDiffCompletesFlag As List(Of BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)
                        lstDiffCompletesFlag = (From b In pIncompleteSamplesDS.twksWSBarcodePositionsWithNoRequests _
                                               Where (Not b.IsCompletedFlagNull) _
                                              Select b).ToList()

                        'Update the flag in the DB
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        Dim myWSNotInUseRotorPositionsDelegate As New WSNotInUseRotorPositionsDelegate

                        For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In lstDiffCompletesFlag
                            'TR 21/09/2011 -Validate if Sample type is informed or not 
                            If (Not row.IsSampleTypeNull) Then
                                'Inform the sample type.
                                resultData = myDAO.UpdateCompletedFlagByPosition(dbConnection, pAnalyzerID, pWorkSessionID, row.RotorType, _
                                                                                 row.CellNumber, row.CompletedFlag, row.SampleType)
                                If (resultData.HasError) Then Exit For
                            Else
                                'Do not inform the sample type.
                                resultData = myDAO.UpdateCompletedFlagByPosition(dbConnection, pAnalyzerID, pWorkSessionID, row.RotorType, _
                                                                                 row.CellNumber, row.CompletedFlag)
                                If (resultData.HasError) Then Exit For
                            End If
                        Next

                        'If the Completed Samples have been positioned, them delete of all them from the table
                        If (Not resultData.HasError AndAlso pDeleteCompleted) Then
                            'Search those that do not have the SampleType informed originally to set the selected value in the table
                            'of Not InUse Rotor Positions
                            lstDiffCompletesFlag = (From b In pIncompleteSamplesDS.twksWSBarcodePositionsWithNoRequests _
                                               Where Not b.IsSampleTypeNull _
                                                 AndAlso b.NotSampleType Select b).ToList()
                            'TR 21/09/2011 -If delete complete is true then update the sampletype by the cell number.
                            For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In lstDiffCompletesFlag
                                resultData = myWSNotInUseRotorPositionsDelegate.UpdateSampleType(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                                 row.RotorType, row.CellNumber, row.SampleType)
                            Next

                            resultData = myDAO.DeleteCompletedSamples(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If
                    End If
                End If
                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateCompletedFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the status of Sample by MessageID. Sets MessageID to NULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">AnalyzerID</param>
        ''' <param name="pRotorType">RotorType</param>
        ''' <param name="pMessageId">Value of MessageID</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  JC 24/04/2013
        ''' </remarks>
        Public Function UpdateHQStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                       ByVal pMessageID As String, ByVal pLISStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myDAO.UpdateHQStatus(dbConnection, pAnalyzerID, pRotorType, pMessageID, pLISStatus)

                    End If
                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateHQStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update to the informed value, the LISStatus of a list of BarcodeInfo values saved in the table of Incomplete Patient Samples
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatusBarCodeInfoList">LIS of BarcodeInfo values which LIS Status have to be updated to the specified value</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 15/03/2013
        ''' </remarks>
        Public Function UpdateLISStatus(ByVal pDBConnection As SqlClient.SqlConnection, pStatusBarCodeInfoList As List(Of String), ByVal pNewStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBCPosWithNoRequestDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        myGlobalDataTO = myBCPosWithNoRequestDAO.UpdateLISStatus(dbConnection, pStatusBarCodeInfoList, pNewStatus)
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateLISStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update Info of Sample requested to HQ
        ''' </summary>
        ''' <param name="pDBConnection ">Sqlconnection to perform DB Access</param>
        ''' <param name="pAnalyzerID">AnalyerId</param>
        ''' <param name="pRotorType">RotorType</param>
        ''' <param name="pSpecimenID">SpecimenID, internally is BarcodeInfo</param>
        ''' <param name="pMessageID">Value of MessageID</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionsDS containing status of the active Work Session after updating
        '''          it by adding new Order Tests</returns>
        ''' <remarks>
        ''' Created by:  JC 08/04/2013
        ''' </remarks>
        Public Function UpdateMessageIDBySpecimenID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                                    ByVal pSpecimenID As String, ByVal pMessageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myDAO.UpdateMessageIDBySpecimenID(dbConnection, pAnalyzerID, pRotorType, pSpecimenID, pMessageID)
                    End If
                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateMessageIdBySpecimenID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When some Patient Samples are deleted, search if there are Incomplete Patient Samples marked as completed to mark them as incompleted
        ''' When some Patient Samples are added manually, search if there are Incomplete Patient Samples marked as incomplete to mark them as completed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWorkSessionResultDS">Typed DataSet WorkSessionResultDS containing all OrderTests that have to be added to the active WorkSession</param>
        ''' <param name="pWithPositioning">When TRUE, it indicates all Incomplete Patient Samples marked as completed have to be deleted from the correspondent table</param>
        ''' <param name="pSearchByBarcodeInfo">When TRUE, it indicates the searching of requested Order Tests for each incomplete Sample have to be done by 
        '''                                    BarcodeInfo and SampleType. When FALSE, the searching is executed by SampleID and SampleType. Optional 
        '''                                    parameter with False as default value</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2011
        ''' Modified by: TR 21/09/2011 - When CompletedFlag is set to False, if the Sample originally does not have a Sample Type informed,
        '''                              then set NULL value to field SampleType
        '''              SA 05/04/2013 - Added new optional parameter to indicate if the searching of requested Order Tests for each incomplete Sample 
        '''                              have to be done by BarcodeInfo and SampleType (when informed) instead of by SampleID and SampleType
        ''' </remarks>
        Public Function UpdateRelatedIncompletedSamples(ByVal pDBConnection As SqlConnection, ByVal pAnalyerID As String, ByVal pWorkSessionID As String, _
                                                        ByVal pWorkSessionResultDS As WorkSessionResultDS, ByVal pWithPositioning As Boolean, _
                                                        Optional ByVal pSearchByBarcodeInfo As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBCPosWithNoRequestDAO As New twksWSBarcodePositionsWithNoRequestsDAO

                        resultData = myBCPosWithNoRequestDAO.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyerID, pWorkSessionID, "SAMPLES")
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim bcPosWithNoRequestDS As BarcodePositionsWithNoRequestsDS = DirectCast(resultData.SetDatos, BarcodePositionsWithNoRequestsDS)

                            Dim myPID As String = String.Empty
                            Dim lstRequestedPatients As List(Of WorkSessionResultDS.PatientsRow)

                            For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In bcPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests
                                If (pSearchByBarcodeInfo) Then
                                    'Search by BarcodeInfo and, if it has been informed, by SampleType. For Order Tests requested by LIS, the searching is by
                                    'LIS field SpecimenID, while for manual Order Tests, the searching if by field SampleID
                                    If (Not row.IsSampleTypeNull) Then
                                        lstRequestedPatients = (From a As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients _
                                                               Where a.SampleClass = "PATIENT" _
                                                             AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                                                             AndAlso a.SampleType = row.SampleType _
                                                             AndAlso ((a.LISRequest = True AndAlso a.SpecimenID = row.BarCodeInfo) _
                                                              OrElse (a.LISRequest = False AndAlso a.SampleID = row.BarCodeInfo)) _
                                                              Select a).ToList()
                                    Else
                                        lstRequestedPatients = (From a As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients _
                                                               Where a.SampleClass = "PATIENT" _
                                                             AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                                                             AndAlso ((a.LISRequest = True AndAlso a.SpecimenID = row.BarCodeInfo) _
                                                              OrElse (a.LISRequest = False AndAlso a.SampleID = row.BarCodeInfo)) _
                                                              Select a).ToList()
                                    End If
                                Else
                                    'Search by SampleID and SampleType
                                    myPID = row.ExternalPID
                                    If (Not row.IsPatientIDNull) Then myPID = row.PatientID

                                    'Search if there are requested Tests for the Incompleted Sample (value of StatFlag is ignored due to the same Sample tube is used)
                                    lstRequestedPatients = (From a In pWorkSessionResultDS.Patients _
                                                           Where a.SampleClass = "PATIENT" _
                                                         AndAlso a.SampleID = myPID _
                                                         AndAlso a.SampleType = row.SampleType _
                                                         AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                                                          Select a).ToList()
                                End If


                                If (row.CompletedFlag AndAlso lstRequestedPatients.Count = 0) Then
                                    'Tests have been removed, mark the Incomplete Sample as not Completed
                                    row.CompletedFlag = False

                                    'TR 21/09/2011 -If the sample originally does not have a sample type informed then set it to null value.
                                    If (row.NotSampleType) Then row.SetSampleTypeNull()

                                ElseIf (Not row.CompletedFlag AndAlso lstRequestedPatients.Count > 0) Then
                                    'Tests have been added, mark the Incomplete Sample as Completed
                                    row.CompletedFlag = True

                                    If (pSearchByBarcodeInfo) Then
                                        'Update also fields ExternalID, StatFlag and, if not informed, the SampleType 
                                        row.ExternalPID = lstRequestedPatients.First.SampleID
                                        row.StatFlag = lstRequestedPatients.First.StatFlag
                                        If (row.IsSampleTypeNull) Then row.SampleType = lstRequestedPatients.First.SampleType
                                    End If
                                Else
                                    'No update is needed; set Null in the DS to exclude that incomplete samples from the updation process
                                    row.SetCompletedFlagNull()
                                End If
                                row.AcceptChanges()
                            Next row

                            'Update the affected records in table of Incompleted Patient Samples
                            resultData = Me.UpdateCompletedFlag(dbConnection, pAnalyerID, pWorkSessionID, bcPosWithNoRequestDS, pWithPositioning, pSearchByBarcodeInfo)
                        End If
                    End If
                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateRelatedIncompletedSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the StatFlag of the specified list of Incomplete Patient Samples
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarcodePositionsWithNoRequestsDS">Typed DataSet BarcodePositionsWithNoRequestsDS containing the information
        '''                                                 of the incomplete cells to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 03/08/2011
        ''' Modified by: SA 13/09/2011 - Changed to update only the StatFlag of the informed Incomplete Patient Samples
        ''' </remarks>
        Public Function UpdateStatFlag(ByVal pDBConnection As SqlConnection, ByVal pBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
                        myGlobalDataTO = myBarcodePositionsWithNoRequests.UpdateStatFlag(dbConnection, pBarcodePositionsWithNoRequestsDS)
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateStatFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the Specimen is positioned in Samples Rotor (the Tube has been already scanned).
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSpecimenID">Specimen Identifier</param>
        ''' <param name="pPatientID">Sample/Patient Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <returns>GlobalDataTO containing an Integer value with the number of Tubes placed in Samples Rotor and marked as incompleted Samples</returns>
        ''' <remarks>
        ''' Created by:  TR 11/04/2013
        ''' </remarks>
        Public Function VerifyScannedSpecimen(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                              ByVal pSpecimenID As String, ByVal pPatientID As String, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
                        myGlobalDataTO = myBarcodePositionsWithNoRequests.VerifyScannedSpecimen(dbConnection, pAnalyzerID, pWorkSessionID, pSpecimenID, pPatientID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.VerifyScannedSpecimen", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the CellNumber of the specified Patient Sample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCellNumber">New Position in the specified Rotor</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <param name="pBarcodeInfo">Sample Barcode</param>
        ''' <param name="pSampleType">Sample Type Code. optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XB 27/08/2013
        ''' </remarks>
        Public Function UpdateCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCellNumber As Integer, ByVal pAnalyzerID As String, _
                                         ByVal pWorkSessionID As String, ByVal pRotorType As String, ByVal pBarcodeInfo As String, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
                        resultData = myDAO.UpdateCellNumber(dbConnection, pCellNumber, pAnalyzerID, pWorkSessionID, pRotorType, pBarcodeInfo, pSampleType)
                    End If
                End If

                If (Not resultData.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.UpdateCellNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Commented methods"
        '''' <summary>
        '''' Read bar code positions with no request by primary key.
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID"></param>
        '''' <param name="pWorkSessionID"></param>
        '''' <param name="pRotorType"></param>
        '''' <param name="pCellNumber"></param>
        '''' <returns></returns>
        '''' <remarks>CREATED BY: DL 03/08/2011</remarks>
        'Public Function Read(ByVal pDBConnection As SqlConnection, _
        '                     ByVal pAnalyzerID As String, _
        '                     ByVal pWorkSessionID As String, _
        '                     ByVal pRotorType As String, _
        '                     ByVal pCellNumber As Integer) As GlobalDataTO

        '    Dim result As Boolean = False
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
        '                myGlobalDataTO = myBarcodePositionsWithNoRequests.Read(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCellNumber)
        '            End If
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.Read", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return myGlobalDataTO

        'End Function

        '''' <summary>
        '''' Gets information about an specific position in table twksWSBarcodePositionsWithNoRequests
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID"></param>
        '''' <param name="pWorkSessionID"></param>
        '''' <param name="pRotorType"></param>
        '''' <param name="pCellNumber"></param>
        '''' <returns></returns>
        '''' <remarks>AG 29/08/2011</remarks>
        'Public Function ReadPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                             ByVal pWorkSessionID As String, ByVal pRotorType As String, ByVal pCellNumber As Integer) As GlobalDataTO

        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDAO As New twksWSBarcodePositionsWithNoRequestsDAO
        '                resultData = myDAO.ReadPosition(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, pCellNumber)
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.ReadPosition", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return resultData
        'End Function

        '''' <summary>
        '''' After BarCode scanning, insert cells with incomplete data in table twksWSBarcodePositionsWithNoRequests
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pBarcodePositionsWithNoRequestsDS">Typed DataSet BarcodePositionsWithNoRequestsDS containing the information
        ''''                                                 of the incomplete cells to add</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created by: DL 03/08/2011
        '''' </remarks>
        'Public Function Create(ByVal pDBConnection As SqlConnection, ByVal pBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myBarcodePositionsWithNoRequests As New twksWSBarcodePositionsWithNoRequestsDAO
        '                myGlobalDataTO = myBarcodePositionsWithNoRequests.Create(dbConnection, pBarcodePositionsWithNoRequestsDS)
        '            End If
        '        End If

        '        If (Not myGlobalDataTO.HasError) Then
        '            'When the Database Connection was opened locally, then the Commit is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '        Else
        '            'When the Connection was opened locally, then the Rollback is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO = New GlobalDataTO
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BarcodePositionsWithNoRequestsDelegate.Create", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace

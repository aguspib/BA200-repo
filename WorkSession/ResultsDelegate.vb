Option Strict On
Option Explicit On

Imports System.Text
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL
    Public Class ResultsDelegate

#Region "Declarations"
        Private ReadOnly myAnalyzerModel As String 'SGM 04/03/11

        'RH 12/01/2012
        Private ReadOnly DatePattern As String = SystemInfoManager.OSDateFormat
        Private ReadOnly TimePattern As String = SystemInfoManager.OSShortTimeFormat

        Private lockThis As New Object()

#End Region

#Region "Constructor"
        'SGM 04/03/11
        Public Sub New(Optional ByVal pAnalyzerModel As String = "")
            Try
                myAnalyzerModel = pAnalyzerModel
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrdersDelegate.New", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Attributes"
        'TR 28/06/2013 -Information to export to LIS (using ES) that will be executed from presentation layer
        Private lastExportedResultsDSAttribute As New ExecutionsDS

#End Region

#Region "Properties"


        ''' <summary>
        ''' Information to export to LIS (using ES) that will be executed from presentation layer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 28/06/2013
        ''' </remarks>
        Public ReadOnly Property LastExportedResults() As ExecutionsDS
            Get
                SyncLock lockThis
                    Return lastExportedResultsDSAttribute
                End SyncLock
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Remove the DS with information with the results automatically exported.
        ''' </summary>
        ''' <remarks>TR 26/06/2013</remarks>
        Public Sub ClearLastExportedResults()
            SyncLock lockThis
                lastExportedResultsDSAttribute.Clear()
            End SyncLock
        End Sub

        ''' <summary>
        ''' Clears (set to NULL) the CurveResultsID field by OrderTest, RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 09/06/2010 (Tested pending)
        ''' </remarks>
        Public Function ClearCurveResultsID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        myGlobalDataTO = mytwksResultDAO.ClearCurveResultsID(dbConnection, pOrderTestID, pRerunNumber)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.ClearCurveResultsID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Results that exist for the specified Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestId">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 09/03/2010 
        ''' </remarks>
        Public Function DeleteByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete Curve Results if exists
                        Dim curve_del As New CurveResultsDelegate
                        resultData = curve_del.DeleteByOrderTestID(dbConnection, pOrderTestID)

                        If (Not resultData.HasError) Then
                            'Delete results from table twksResults
                            Dim myDAO As New twksResultsDAO
                            resultData = myDAO.DeleteResultsByOrderTestID(dbConnection, pOrderTestID)
                        End If

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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.DeleteByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete old calibrator results (parameters TestId, TestVersion, SampleType
        ''' (TO DO: When calibrates with curve delete the curve results too)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestId"></param>
        ''' <param name="pTestVersion"></param>
        ''' <param name="pSampleType"></param>
        ''' <returns>GlobalDataTo indicating if results succeed or fails</returns>
        ''' <remarks>
        ''' Created by:  AG 18/03/2010 (Tested Pending)
        ''' </remarks>
        Public Function DeleteCalibrationResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestId As Integer, _
                                                 ByVal pTestVersion As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        If (Not resultData.HasError) Then
                            'TO DO: Delete curve results from twksCurveResults table

                            'Delete results from table twksResults
                            Dim myDAO As New twksResultsDAO
                            resultData = myDAO.DeleteCalibrationResultsByTestIDSampleType(dbConnection, pTestId, pTestVersion, pSampleType)

                        End If

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.DeleteCalibrationResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Results that exist for all Order Tests belonging to the specified Order
        ''' Used to delete OFF-SYSTEM Order Tests with Results from the screen of WS Preparation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 25/01/2011
        ''' </remarks>
        Public Function DeleteResultsByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultsDAO
                        resultData = myDAO.DeleteResultsByOrderID(dbConnection, pOrderID)

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.DeleteResultsByOrderID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Specific method for deleting results related to the informed Order Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function DeleteOFFSResultsByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myResultAlarmsDelegate As New ResultAlarmsDelegate
                        resultData = myResultAlarmsDelegate.DeleteResultAlarmsByOrderTestID(dbConnection, pOrderTestID)

                        If Not resultData.HasError Then
                            Dim myDAO As New twksResultsDAO
                            resultData = myDAO.DeleteResultsByOrderTestID(dbConnection, pOrderTestID)
                        End If


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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.DeleteOFFSResultsByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete old blank and calibrator results (parameters TestId, TestVersion)
        ''' (TO DO: When calibrates with curve delete the curve results too)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestId"></param>
        ''' <param name="pTestVersion"></param>        
        ''' <returns>GlobalDataTo indicating if results succeed or fails</returns>
        ''' <remarks>
        ''' Created by:  AG 18/03/2010 (Tested Pending)
        ''' </remarks>
        Public Function DeleteTestResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestId As Integer, _
                                                 ByVal pTestVersion As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        If (Not resultData.HasError) Then
                            'TO DO: Delete curve results from twksCurveResults table

                            'Delete results from table twksResults
                            Dim myDAO As New twksResultsDAO
                            resultData = myDAO.DeleteResultsByTestID(dbConnection, pTestId, pTestVersion)

                        End If

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.DeleteTestResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete Old blank and calibrator results 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo indicating if results succeed or fails</returns>
        ''' <remarks>
        ''' Created by:  JB 10/10/2012
        ''' Modified by: SA 19/10/2012 - When calling function GetLastExecutedCalibrator, third parameter should by
        '''                              SampleType instead of SampleClass
        ''' </remarks>
        Public Function DeleteOldBlankCalib(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultsDAO As New twksResultsDAO
                        resultData = myResultsDAO.GetOldBlankCalibToDelete(dbConnection)

                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim myResultsDS As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                            Dim myCurveResultsDel As New CurveResultsDelegate
                            Dim myAdditionalElementDS As WSAdditionalElementsDS

                            For Each row As ResultsDS.twksResultsRow In myResultsDS.twksResults
                                'For each Blank: Delete all blank results
                                If (row.SampleClass = "BLANK") Then
                                    resultData = myResultsDAO.GetLastExecutedBlank(dbConnection, row.TestID, row.TestVersion, row.AnalyzerID, , False, True)
                                    If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                        myAdditionalElementDS = DirectCast(resultData.SetDatos, WSAdditionalElementsDS)

                                        Dim first As Boolean = True
                                        For Each rowElem As WSAdditionalElementsDS.WSAdditionalElementsTableRow In myAdditionalElementDS.WSAdditionalElementsTable
                                            If (Not first) Then
                                                resultData = myResultsDAO.DeleteResultsByOrderTestID(dbConnection, rowElem.PreviousOrderTestID)
                                                If (resultData.HasError) Then Exit For
                                            End If
                                            first = False
                                        Next
                                    End If

                                ElseIf (row.SampleClass = "CALIB") Then
                                    'For each Calib: Delete all calib results and curves
                                    resultData = myResultsDAO.GetLastExecutedCalibrator(dbConnection, row.TestID, row.SampleType, row.TestVersion, row.AnalyzerID, , False)
                                    If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                        myAdditionalElementDS = DirectCast(resultData.SetDatos, WSAdditionalElementsDS)

                                        If (myAdditionalElementDS.WSAdditionalElementsTable.Count > 1) Then
                                            Dim firstOT As Integer = myAdditionalElementDS.WSAdditionalElementsTable(0).PreviousOrderTestID
                                            Dim lstOT As List(Of Integer) = (From elem In myAdditionalElementDS.WSAdditionalElementsTable _
                                                                           Select elem.PreviousOrderTestID Distinct).ToList

                                            For Each otID As Integer In lstOT
                                                If (otID <> firstOT) Then
                                                    resultData = myCurveResultsDel.DeleteByOrderTestID(dbConnection, otID)
                                                    If (resultData.HasError) Then Exit For

                                                    resultData = myResultsDAO.DeleteResultsByOrderTestID(dbConnection, otID)
                                                    If (resultData.HasError) Then Exit For
                                                End If
                                            Next
                                        End If
                                    End If
                                End If
                                If (resultData.HasError) Then Exit For
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
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.DeleteOldBlankCalib", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the accepted Results for the specified OrderTestID (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pIgnoreValidationStatus"></param>
        ''' <param name="pApplyCONVERT"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all accepted Results found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010 
        ''' Modified by: AG 12/03/2010 - Added optional parameter pIgnoreValidationStatus. This parameter is set to TRUE 
        '''                              to get the last calculated results without taking care of validation status
        ''' </remarks>
        Public Function GetAcceptedResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                           Optional ByVal pIgnoreValidationStatus As Boolean = False, Optional ByVal pApplyCONVERT As Boolean = True, _
                                           Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetAcceptedResults(dbConnection, pOrderTestID, pIgnoreValidationStatus, pApplyCONVERT, pTestType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetAcceptedResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests with Calculated Test Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderForReportFlag"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (subtable vwksCalcResults)</returns>
        ''' <remarks>
        ''' Created by: RH 25/08/2010
        ''' Modified by AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results (new parameter)
        ''' </remarks>
        Public Function GetCalculatedTestResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                 ByVal pWorkSessionID As String, Optional ByVal pOrderForReportFlag As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetCalculatedTestResults(dbConnection, pAnalyzerID, pWorkSessionID, pOrderForReportFlag)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetCalculatedTestResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Results for the specified AnalyzerID and WorkSessionID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pUploadResultsFlag">TRUE only for current WS automatic online export. Otherwise FALSE</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by:  AG 25/06/2012
        ''' Modified by: AG 28/06/2013 - Call FillSpecimenIDList after GetCompleteResults (CANCELLED because function GetCompleteResults use a view that does 
        '''                              not return neither PatientID nor SampleID)
        '''              AG 02/01/2014 - BT #1444 ==> Automatic upload for Manual Orders of STD Tests does not upload the SpecimenID. Changes:
        '''                                           ** Added new optional parameter pUploadResulsFlags.
        '''                                           ** Fill column PatientID for Patients when it is NULL (code was obtained from IResults.LoadExecutionsResults and adapted)
        '''              SA 13/01/2014 - BT #1407 ==> Removed code changes made for BT #1444 due to field PatientID is still missing for Calculated Tests.  To solve all cases 
        '''                                           views vwksCalcResults and vwksCompleteResults were modified to return the field for all Test Types
        '''              AG 17/02/2014 - #1505 filter query by list of orders / ordertests depending which is informed (used in automatic upload to LIS)
        '''              AG 04/04/2014 - #1884 Fill column SpecimenID for the Summary Results Report 
        ''' </remarks>
        Public Function GetCompleteResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, Optional ByVal pUploadResultsFlag As Boolean = False, _
                                           Optional orderIDList As List(Of String) = Nothing, Optional orderTestIDList As List(Of Integer) = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then


                        'AG 17/02/2014 - #1505
                        'Dim mytwksResultDAO As New twksResultsDAO()
                        'resultData = mytwksResultDAO.GetCompleteResults(dbConnection, pAnalyzerID, pWorkSessionID)

                        Dim filterClause As String = ""
                        If Not orderIDList Is Nothing AndAlso orderIDList.Count > 0 Then
                            If orderIDList.Count > 1 Then
                                For i As Integer = 0 To orderIDList.Count - 1
                                    Select Case i
                                        Case 0 'First item
                                            filterClause &= " (OrderID = '" & orderIDList(i) & "' "
                                        Case orderIDList.Count - 1 'Last item
                                            filterClause &= " OR OrderID = '" & orderIDList(i) & "' ) "
                                        Case Else 'Middle item
                                            filterClause &= " OR OrderID = '" & orderIDList(i) & "' "
                                    End Select
                                Next
                            Else 'Only 1 item
                                filterClause &= " (OrderID = '" & orderIDList(0) & "' ) "
                            End If

                        ElseIf Not orderTestIDList Is Nothing AndAlso orderTestIDList.Count > 0 Then
                            If orderTestIDList.Count > 1 Then
                                For i As Integer = 0 To orderTestIDList.Count - 1
                                    Select Case i
                                        Case 0 'First item
                                            filterClause &= " (OrderTestID = " & orderTestIDList(i)
                                        Case orderTestIDList.Count - 1 'Last item
                                            filterClause &= " OR OrderTestID = " & orderTestIDList(i) & " ) "
                                        Case Else 'Middle item
                                            filterClause &= " OR OrderTestID = " & orderTestIDList(i)
                                    End Select
                                Next
                            Else 'Only 1 item
                                filterClause &= " (OrderTestID = " & orderTestIDList(0) & " ) "
                            End If
                        End If

                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetCompleteResults(dbConnection, pAnalyzerID, pWorkSessionID, filterClause)

                        'IT 04/09/2014 - #1884
                        Dim AverageResultsDS As New ResultsDS
                        If (Not resultData.HasError) Then
                            AverageResultsDS = CType(resultData.SetDatos, ResultsDS)
                        End If
                        resultData = FillSpecimenIDListForReport(dbConnection, pWorkSessionID, AverageResultsDS)
                        'IT 04/09/2014 - #1884

                        'AG 17/02/2014 - #1505

                        'AG 28/06/2013 - CANCELLED because the GetCompleteResults view does not return neither PatientID neither SampleID
                        'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        '    Dim myResults As New ResultsDS
                        '    myResults = DirectCast(resultData.SetDatos, ResultsDS)
                        '    resultData = FillSpecimenIDList(dbConnection, pWorkSessionID, myResults)
                        'End If
                        'AG 28/06/2013

                        ''AG 02/01/2014 - BT #1444 This code has to be executed only just before to upload results to LIS (v211 patch2)
                        ''This code is got and adapted from IResults.LoadExecutionsResults
                        ''Automatic upload for manual orders of std tests do not upload specimenID
                        'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        '    If pUploadResultsFlag Then
                        '        Dim auxiliaryData As New GlobalDataTO
                        '        Dim myCompletedResultsDS As New ResultsDS
                        '        myCompletedResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                        '        Dim myLinqRes As List(Of ResultsDS.vwksResultsRow)
                        '        myLinqRes = (From a As ResultsDS.vwksResultsRow In myCompletedResultsDS.vwksResults _
                        '                     Where a.SampleClass = "PATIENT" AndAlso a.IsPatientIDNull Select a).ToList

                        '        If myLinqRes.Count > 0 Then
                        '            Dim myExecutionDelegate As New ExecutionsDelegate
                        '            auxiliaryData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)

                        '            If (Not auxiliaryData.HasError) Then
                        '                Dim ExecutionsResultsDS As New ExecutionsDS
                        '                ExecutionsResultsDS = CType(auxiliaryData.SetDatos, ExecutionsDS)

                        '                Dim myLinqEx As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)

                        '                For Each row As ResultsDS.vwksResultsRow In myLinqRes
                        '                    If Not row.IsOrderTestIDNull AndAlso Not row.IsRerunNumberNull Then
                        '                        myLinqEx = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                        '                                    Where a.OrderTestID = row.OrderTestID AndAlso a.RerunNumber = row.RerunNumber Select a).ToList
                        '                        If myLinqEx.Count > 0 AndAlso Not myLinqEx(0).IsPatientIDNull Then
                        '                            row.BeginEdit()
                        '                            row.PatientID = myLinqEx(0).PatientID
                        '                            row.EndEdit()
                        '                            row.AcceptChanges()
                        '                        End If
                        '                    End If
                        '                Next
                        '                myLinqEx = Nothing
                        '            End If

                        '            myCompletedResultsDS.vwksResults.AcceptChanges()
                        '            resultData.SetDatos = myCompletedResultsDS 'Inform the data to return!!
                        '        End If

                        '        myLinqRes = Nothing
                        '        auxiliaryData = Nothing
                        '    End If
                        'End If
                        ''AG 02/01/2014 
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetCompleteResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests with ISE or OFFSYSTEM Test Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderForReportFlag"></param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not. If succeed, returns an ResultsDS 
        '''          dataset with the results (view vwksCalcResults)
        ''' </returns>
        ''' <remarks>
        ''' Created by: AG 01/12/2010 (Copied and adapted from GetCalculatedTestResults)
        ''' Modified by AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results (new parameter)
        ''' </remarks>
        Public Function GetISEOFFSTestResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                              ByVal pWorkSessionID As String, Optional ByVal pOrderForReportFlag As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetISEOFFSTestResults(dbConnection, pAnalyzerID, pWorkSessionID, pOrderForReportFlag)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetISEOFFSTestResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' If exists an accepted valid result for a Blank executed in the informed Analyzer with the same version that the current 
        ''' Test Version and optionally, inside the allowed period of time, this function returns the ABS Value of the Blank
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pCurrentTestVersion">Current Version of the informed Test</param>
        ''' <param name="pMaxDaysLastBlank">Maximum number of days that can have passed from the last Blank execution for the informed Test. 
        '''                                It is an optional parameter</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <param name="pApplyCONVERT" ></param>
        ''' <param name="pIgnoreOrderTestStatus"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAdditionalElementsDS with the result of the last executed Blank 
        '''          for the informed Test</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SA 08/01/2010 - Changes to return a GlobalDataTO instead a typed DataSet WSAdditionalElementsDS. Changes
        '''                              to implement the open of a DB Connection according the new template. 
        '''              AG 09/03/2010 - AnalyzerID is an optional parameter (change parameter list ordenation)
        '''              AG 19/11/2012 - add optional parameter pIgnoreOrderTestStatus (it will be informed as TRUE when called from calculations class)
        ''' </remarks>
        Public Function GetLastExecutedBlank(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pCurrentTestVersion As Integer, _
                                             Optional ByVal pAnalyzerID As String = "", Optional ByVal pMaxDaysLastBlank As String = "", _
                                             Optional ByVal pApplyCONVERT As Boolean = True, _
                                             Optional ByVal pIgnoreOrderTestStatus As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetLastExecutedBlank(dbConnection, pTestID, pCurrentTestVersion, pAnalyzerID, pMaxDaysLastBlank, pApplyCONVERT, False, pIgnoreOrderTestStatus)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetLastExecutedBlank", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' If exists an accepted valid result for an Experimental Calibrator executed in the informed Analyzer with the same version than 
        ''' the current Test Version and optionally, inside the allowed period of time, this function returns the ABS Value and Calibrator  
        ''' Factor of each Calibrator point 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pCurrentTestVersion">Current Version of the informed Test</param>
        ''' ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <param name="pMaxDaysLastCalib">Maximum number of days that can have passed from the last Calibrator execution for the informed TestID/SampleType.  
        '''                                 It is an optional parameter</param>
        ''' <param name="pApplyCONVERT" ></param>
        ''' <param name="pIgnoreOrderTestStatus"></param>
        ''' <returns>DataSet with the result of the last executed Calibrator for the informed 
        '''          TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by: TR
        ''' Modified by: SA 08/01/2010 - Changes to return a GlobalDataTO instead a typed DataSet WSAdditionalElementsDS. Changes
        '''                              to implement the open of a DB Connection according the new template. 
        '''              AG 09/03/2010 - AnalyzerID is an optional parameter (change parameter list ordenation)
        '''              AG 19/11/2012 - add optional parameter pIgnoreOrderTestStatus (it will be informed as TRUE when called from calculations class)
        ''' </remarks>
        Public Function GetLastExecutedCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
                                                  ByVal pCurrentTestVersion As Integer, Optional ByVal pAnalyzerID As String = "", _
                                                  Optional ByVal pMaxDaysLastCalib As String = "", _
                                                  Optional ByVal pApplyCONVERT As Boolean = True, _
                                                  Optional ByVal pIgnoreOrderTestStatus As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetLastExecutedCalibrator(dbConnection, pTestID, pSampleType, pCurrentTestVersion, pAnalyzerID, pMaxDaysLastCalib, pApplyCONVERT, pIgnoreOrderTestStatus)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetLastExecutedCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Results for the specified OrderTestID (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by: RH 19/07/2010 
        ''' </remarks>
        Public Function GetResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetResults(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the result filtering by sample class and fill reference range interval
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerId"></param>
        ''' <param name="pWorkSessionId"></param>
        ''' <param name="pClassList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetResultsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerId As String, ByVal pWorkSessionId As String, ByVal ParamArray pClassList() As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim filterClause As String = String.Empty
                        Dim InFilter As String = String.Empty

                        For Each className As String In pClassList
                            InFilter += String.Format("'{0}', ", className)
                        Next

                        If (InFilter <> String.Empty) Then
                            InFilter = InFilter.Substring(0, InFilter.Length - 2)
                            filterClause = String.Format("SampleClass IN ({0})", InFilter)
                        End If

                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetCompleteResults(dbConnection, pAnalyzerId, pWorkSessionId, filterClause)

                        Dim resultsDS As New ResultsDS
                        If (Not resultData.HasError) Then
                            resultsDS = CType(resultData.SetDatos, ResultsDS)
                        End If

                        FillReferenceRangeInterval(dbConnection, resultsDS)

                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsByClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Result Alarms (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Result Alarms (vwksResultsAlarms)</returns>
        ''' <remarks>
        ''' Created by: RH 19/07/2010
        ''' AG 17/02/2014 - #1505 filter query by list of orders / ordertests depending which is informed (used in automatic upload to LIS)
        ''' </remarks>
        Public Function GetResultAlarms(ByVal pDBConnection As SqlClient.SqlConnection, Optional orderTestIDList As List(Of Integer) = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'AG 17/02/2014 - #1505
                        'Dim mytwksResultDAO As New twksResultsDAO()
                        'resultData = mytwksResultDAO.GetResultAlarms(dbConnection)

                        Dim filterClause As String = ""
                        If Not orderTestIDList Is Nothing AndAlso orderTestIDList.Count > 0 Then
                            If orderTestIDList.Count > 1 Then
                                For i As Integer = 0 To orderTestIDList.Count - 1
                                    Select Case i
                                        Case 0 'First item
                                            filterClause &= " (OrderTestID = " & orderTestIDList(i)
                                        Case orderTestIDList.Count - 1 'Last item
                                            filterClause &= " OR OrderTestID = " & orderTestIDList(i) & " ) "
                                        Case Else 'Middle item
                                            filterClause &= " OR OrderTestID = " & orderTestIDList(i)
                                    End Select
                                Next
                            Else 'Only 1 item
                                filterClause &= " (OrderTestID = " & orderTestIDList(0) & " ) "
                            End If
                        End If

                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetResultAlarms(dbConnection, filterClause)
                        'AG 17/02/2014 - #1505

                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the accepted Results for the specified OrderTestID and RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber" >Rerun Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all accepted Results found 
        '''          for the informed OrderTestID and rerun number</returns>
        ''' <remarks>
        ''' Created by:  DL 06/05/2010 
        ''' </remarks>
        Public Function ReadByOrderTestIDandRerunNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                        ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.ReadByOrderTestIDandRerunNumber(dbConnection, pOrderTestID, pRerunNumber)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.ReadByOrderTestIDandRerunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Recalculate the ExportStatus in recalculations mode (this method only has to be called in PATIENT results recalculations)
        ''' 1. Read the current ExportStatus value
        ''' 2. Current value isnt NOTSENT ... new value is UPDATED
        ''' 3. Else (no result exist or current value is NOTSENT) ... new value is NOTSENT
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing an String value with the new Export to LIMS status for the specified OrderTestID/RerunNumber</returns>
        ''' <remarks>
        ''' Created by:  AG 10/09/2010
        ''' Modified by: AG 09/05/2012
        ''' </remarks>
        Public Function RecalculateExportStatusValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                     ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()

                        resultData = mytwksResultDAO.ReadByOrderTestIDandRerunNumber(dbConnection, pOrderTestID, pRerunNumber)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myResultsDS As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                            resultData.SetDatos = "NOTSENT" 'Default return value
                            If (myResultsDS.twksResults.Rows.Count > 0 AndAlso Not myResultsDS.twksResults(0).IsExportStatusNull) Then
                                Dim currentValue As String = myResultsDS.twksResults(0).ExportStatus

                                If (Not String.Equals(currentValue, "NOTSENT")) Then
                                    resultData.SetDatos = "UPDATED"
                                Else
                                    resultData.SetDatos = "NOTSENT"
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.RecalculateExportStatusValue", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Reset (set to False) the accepted result flag for the result with the informed OrderTestID and RerunNumber different of the informed one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a </returns>
        ''' <remarks>
        ''' Created by:  AG 03/03/2010 
        ''' Modified by: SA 16/07/2012 - Added optional parameters for the Analyzer and WorkSession
        ''' </remarks>
        Public Function ResetAcceptedResultFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                                Optional ByVal pAnalyzerID As String = "", Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()

                        resultData = mytwksResultDAO.ResetAcceptedResultFlag(dbConnection, pOrderTestID, pRerunNumber, pAnalyzerID, pWorkSessionID)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.ResetAcceptedResultFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all results belonging to Order Tests that were included in the WorkSession.
        ''' Validated and accepted results of Blank and Calibrators are not deleted 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 04/06/2010
        ''' Modified by: SA 20/07/2010 - Previous call to ResetWS changed to calls to new functions
        '''                              ResetWSPatientCtrlResults and ResetWSBlankCalibResults
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (Not resultData.HasError) Then
                            'Delete results from table twksResults - All Patient and Control Results
                            Dim myDAO As New twksResultsDAO
                            resultData = myDAO.ResetWSPatientCtrlResults(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete results from table twksResults - All Blank and Calibrator Results
                            'excepting those that have been validated and accepted
                            Dim myDAO As New twksResultsDAO
                            resultData = myDAO.ResetWSBlankCalibResults(dbConnection)
                        End If

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add/update results for the specified group of OffSystem Tests 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestResultsDS">Typed DataSet OffSystemTestsResultsDS containing the list of results for all 
        '''                                       OffSystem Tests requested in a WorkSession</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 18/01/2011
        ''' Modified by: AG 21/05/2012 - Function for export Results to LIMS is called when each processed OrderTest of an OffSystem Test is CLOSED
        '''                            - Added parameters for the IDs of the active Analyzer and WorkSession
        '''              SA 22/05/2012 - Inform the SampleID when calling the function for export Results to LIMS
        '''              SA 25/06/2012 - Inform AnalyzerID and WorkSessionID when the result is added to the ResultsDS dataset. Call function Create
        '''                              instead of InsertResult (both in twksResultsDAO) 
        '''              SA 31/10/2012 - When a result for an OffSystem Test is created, set field ExportStatus to NOTSENT instead of to NULL
        '''              AG 16/01/2014 - BA-2011 ==> Inform the proper value for the ExportStatus
        '''              SA 14/01/2015 - BA-2153 ==> For each saved OFFS Test, verify if there are affected Calculated Tests that have to be recalculated 
        ''' </remarks>
        Public Function SaveOffSystemResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestResultsDS As OffSystemTestsResultsDS, _
                                             ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim ignoreOffSystemTest As Boolean = False
                        Dim newOrderTestStatus As String = String.Empty
                        Dim updatedExportStatusValue As String = String.Empty

                        Dim myResultsDS As New ResultsDS
                        Dim myResultRow As ResultsDS.twksResultsRow

                        Dim myResultAlarmsDS As New ResultAlarmsDS
                        Dim myResultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow

                        Dim myTempResultsDS As ResultsDS
                        Dim myTestRefRangesDS As TestRefRangesDS

                        Dim myExport As New ExportDelegate
                        Dim mytwksResultDAO As New twksResultsDAO()
                        Dim myOrderTestsDelegate As New OrderTestsDelegate
                        Dim myResultAlarmsDelegate As New ResultAlarmsDelegate

                        'BA-2153: Declare OperateCalculatedTestDelegate needed to verify if there are Calculated Tests that have to be recalculated due to 
                        '         the result of OFFS Tests included in their Formulas have been changed. Inform the Delegate properties: AnalyzerID and WorkSessionID
                        Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                        myCalcTestsDelegate.AnalyzerID = pAnalyzerID
                        myCalcTestsDelegate.WorkSessionID = pWorkSessionID

                        For Each offSystemTestResult As OffSystemTestsResultsDS.OffSystemTestsResultsRow In pOffSystemTestResultsDS.OffSystemTestsResults
                            'Verify if the Result for the OffSystem Test already exists...
                            myResultsDS.Clear()
                            myResultRow = myResultsDS.twksResults.NewtwksResultsRow
                            myResultRow.OrderTestID = offSystemTestResult.OrderTestID
                            myResultRow.RerunNumber = 1
                            myResultRow.MultiPointNumber = 1
                            myResultRow.AnalyzerID = pAnalyzerID
                            myResultRow.WorkSessionID = pWorkSessionID
                            myResultRow.AcceptedResultFlag = True
                            myResultRow.ManualResultFlag = True
                            myResultRow.ValidationStatus = "OK"
                            myResultRow.ExportStatus = "NOTSENT"

                            If (Not String.IsNullOrEmpty(offSystemTestResult.ResultValue)) Then
                                If (offSystemTestResult.ResultType = "QUANTIVE") Then
                                    myResultRow.ManualResult = Convert.ToDecimal(offSystemTestResult.ResultValue)
                                ElseIf (offSystemTestResult.ResultType = "QUALTIVE") Then
                                    myResultRow.ManualResultText = offSystemTestResult.ResultValue
                                End If
                            Else
                                myResultRow.SetManualResultNull()
                                myResultRow.SetManualResultTextNull()
                            End If
                            myResultsDS.twksResults.AddtwksResultsRow(myResultRow)

                            ignoreOffSystemTest = True
                            newOrderTestStatus = "CLOSED"

                            resultData = mytwksResultDAO.ExistsResult(dbConnection, myResultsDS)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myTempResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                                If (myTempResultsDS.twksResults.Rows.Count = 0) Then
                                    If (Not offSystemTestResult.IsResultValueNull) Then
                                        'New OffSystem Test result; add it
                                        ignoreOffSystemTest = False
                                        resultData = mytwksResultDAO.Create(dbConnection, myResultsDS)
                                    End If
                                Else
                                    'AG 16/10/2014 BA-2011- Assign new value for ExportStatus
                                    updatedExportStatusValue = "NOTSENT"  'Defaul value
                                    If (Not myTempResultsDS.twksResults(0).IsExportStatusNull AndAlso myTempResultsDS.twksResults(0).ExportStatus = "SENT") Then
                                        updatedExportStatusValue = "UPDATED"
                                    End If
                                    'AG 16/10/2014 BA-2011

                                    If (Not String.IsNullOrEmpty(offSystemTestResult.ResultValue)) Then
                                        'There is a result for the OffSystem Test; update the value if it has been changed
                                        If (offSystemTestResult.ResultType = "QUANTIVE") Then
                                            If (myResultsDS.twksResults(0).ManualResult <> myTempResultsDS.twksResults(0).ManualResult) Then
                                                ignoreOffSystemTest = False
                                                resultData = mytwksResultDAO.UpdateManualResult(dbConnection, myResultsDS.twksResults(0).ManualResultFlag, _
                                                                                                offSystemTestResult.ResultType, myResultsDS.twksResults(0).ManualResult, _
                                                                                                Nothing, myResultsDS.twksResults(0).OrderTestID, 1, 1, updatedExportStatusValue)
                                            End If

                                        ElseIf (offSystemTestResult.ResultType = "QUALTIVE") Then
                                            If (myResultsDS.twksResults(0).ManualResultText <> myTempResultsDS.twksResults(0).ManualResultText) Then
                                                ignoreOffSystemTest = False
                                                resultData = mytwksResultDAO.UpdateManualResult(dbConnection, myResultsDS.twksResults(0).ManualResultFlag, _
                                                                                                offSystemTestResult.ResultType, Nothing, _
                                                                                                myResultsDS.twksResults(0).ManualResultText, _
                                                                                                myResultsDS.twksResults(0).OrderTestID, 1, 1, updatedExportStatusValue)
                                            End If
                                        End If
                                    Else
                                        'There is a result for the OffSystem Test but the ResultValue has been deleted; remove the previous result
                                        ignoreOffSystemTest = False

                                        'Delete all Alarms that exists currently for the Order Test results (if any)
                                        resultData = myResultAlarmsDelegate.DeleteAll(dbConnection, offSystemTestResult.OrderTestID, 1, 1)
                                        If (resultData.HasError) Then Exit For

                                        'Delete all Results that exists currently for the Order Test
                                        resultData = mytwksResultDAO.DeleteResultsByOrderTestID(dbConnection, offSystemTestResult.OrderTestID)
                                        offSystemTestResult.SetActiveRangeTypeNull() 'To avoid validation of ReferenceRanges if for deleted Results...
                                        newOrderTestStatus = "OPEN"
                                    End If
                                End If

                                If (Not ignoreOffSystemTest) Then
                                    If (Not resultData.HasError) Then
                                        'If Reference Ranges have been defined for the Test/SampleType, validate if the informed result is inside the limits
                                        If (Not offSystemTestResult.IsActiveRangeTypeNull) Then
                                            'Delete all Alarms that exists currently for the Order Test result (if any)
                                            resultData = myResultAlarmsDelegate.DeleteAll(dbConnection, offSystemTestResult.OrderTestID, 1, 1)
                                            If (resultData.HasError) Then Exit For

                                            'Get the Reference Range Interval defined for the Test
                                            resultData = myOrderTestsDelegate.GetReferenceRangeInterval(dbConnection, offSystemTestResult.OrderTestID, "OFFS", _
                                                                                                        offSystemTestResult.TestID, offSystemTestResult.SampleType, _
                                                                                                        offSystemTestResult.ActiveRangeType)

                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myTestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                                                If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
                                                    If (myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit <> -1) AndAlso _
                                                       (myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit <> -1) Then
                                                        If (Convert.ToSingle(offSystemTestResult.ResultValue) < myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit) OrElse _
                                                           (Convert.ToSingle(offSystemTestResult.ResultValue) > myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit) Then
                                                            'Add the Alarm of result value out of the Normality Range
                                                            myResultAlarmsDS.Clear()
                                                            myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
                                                            myResultAlarmRow.OrderTestID = offSystemTestResult.OrderTestID
                                                            myResultAlarmRow.RerunNumber = 1
                                                            myResultAlarmRow.MultiPointNumber = 1

                                                            'TR 19/07/2012 -Validate the result value to set the corresponding alarm
                                                            If (Convert.ToSingle(offSystemTestResult.ResultValue) < myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit) Then
                                                                'Set lower alarm value
                                                                myResultAlarmRow.AlarmID = GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString
                                                            ElseIf (Convert.ToSingle(offSystemTestResult.ResultValue) > myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit) Then
                                                                'Set hight alarm value
                                                                myResultAlarmRow.AlarmID = GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString
                                                            End If

                                                            myResultAlarmRow.AlarmDateTime = Now
                                                            myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)

                                                            resultData = myResultAlarmsDelegate.Add(dbConnection, myResultAlarmsDS)
                                                            If (resultData.HasError) Then Exit For
                                                        End If
                                                    End If
                                                End If
                                            Else
                                                'Error getting the Reference Ranges interval
                                                Exit For
                                            End If
                                        End If

                                        'Close the Order Test due to it has a result
                                        resultData = myOrderTestsDelegate.UpdateStatusByOrderTestID(dbConnection, offSystemTestResult.OrderTestID, newOrderTestStatus, True)
                                        If (resultData.HasError) Then Exit For

                                        'BA-2153: Verify if there are Calculated Tests that have to be recalculated due to the result of OFFS Tests included in their
                                        '         Formulas have been changed
                                        resultData = myCalcTestsDelegate.ExecuteCalculatedTest(dbConnection, offSystemTestResult.OrderTestID, False)
                                        If (resultData.HasError) Then Exit For

                                        'OnLine Export to LIMS
                                        If (newOrderTestStatus = "CLOSED" AndAlso pAnalyzerID.Trim <> String.Empty AndAlso pWorkSessionID.Trim <> String.Empty) Then
                                            resultData = myExport.ManageLISExportation(dbConnection, pAnalyzerID, pWorkSessionID, offSystemTestResult.OrderTestID, True)
                                            If (resultData.HasError) Then Exit For

                                            'TR 28/06/2013
                                            AddIntoLastExportedResults(DirectCast(resultData.SetDatos, ExecutionsDS))
                                        End If
                                    Else
                                        'Error adding/updating/deleting the result for the OffSystem Test 
                                        Exit For
                                    End If
                                End If
                            Else
                                'Error verifying if the result for the OffSystem Test already exists
                                Exit For
                            End If
                        Next

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.SaveOffSystemResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add/update a Result for an OrderTest/RerunNumber/MultipointNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing all data for the Result to update</param>
        ''' <param name="pRecalculusFlag">Optional parameter with default value FALSE. It is used as parameter when 
        '''                               calling function UpdateResult</param> 
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 02/03/2010
        ''' Modified by: AG 25/06/2012 - Call function Create instead of InsertResult (Create also inserts fields AnalyzerID and WorkSessionID)
        '''              SA 04/07/2012 - Changed implementation to allow saving several Results (the entry DS can contain results for all points
        '''                              or a multipoint Calibrator)
        '''              TR 19/07/2012 - For Control Results, get the CtrlSendingGroup for the corresponding OrderTest and inform it in the ResultsDS
        '''              SA 03/12/2014 - BA-1616 ==> Added new optional parameter pRecalculusFlag. It is used as parameter when calling function UpdateResult
        ''' </remarks>
        Public Function SaveResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, _
                                    Optional ByVal pRecalculusFlag As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultDS As New ResultsDS
                        Dim mytwksResultDAO As New twksResultsDAO()
                        Dim myWSOrderTestDelegate As New WSOrderTestsDelegate()

                        For Each resultRow As ResultsDS.twksResultsRow In pResultsDS.twksResults
                            'Move the current result to the ResultsDS that will be used to save (insert/update) it
                            myResultDS.Clear()
                            myResultDS.twksResults.ImportRow(resultRow)

                            resultData = mytwksResultDAO.ExistsResult(dbConnection, myResultDS)
                            If (resultData.HasError) Then Exit For

                            If (DirectCast(resultData.SetDatos, ResultsDS).twksResults.Rows.Count > 0) Then
                                'BA-1616: Inform parameter pRecalculusFlag when call function UpdateResult
                                resultData = mytwksResultDAO.UpdateResult(dbConnection, myResultDS, pRecalculusFlag)
                                If (resultData.HasError) Then Exit For
                            Else
                                'Search the Control Sending Group for the CTRL OrderTestID
                                If (Not resultRow.IsSampleClassNull AndAlso resultRow.SampleClass = "CTRL") Then
                                    resultData = myWSOrderTestDelegate.GetControlSendingGroup(dbConnection, resultRow.WorkSessionID, resultRow.OrderTestID)

                                    If (Not resultData.HasError AndAlso Not CInt(resultData.SetDatos) = 0) Then
                                        'Inform the value for the current CTRL Result
                                        myResultDS.twksResults(0).CtrlsSendingGroup = CInt(resultData.SetDatos)
                                    End If
                                End If

                                'AG 25/06/2012 - Call Create method to inserts also AnalyzerID and WorkSessionID
                                resultData = mytwksResultDAO.Create(dbConnection, myResultDS)
                                If (resultData.HasError) Then Exit For
                            End If
                        Next

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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.SaveResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set to the informed value the AcceptedResultFlag of the Result with the informed OrderTestID and RerunNumber 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pNewAcceptedValue">Value TRUE/FALSE to assign to the result</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 03/03/2010 
        ''' Modified by: SA 16/07/2012 - Added optional parameters for the Analyzer and WorkSession
        ''' </remarks>
        Public Function UpdateAcceptedResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                             ByVal pNewAcceptedValue As Boolean, Optional ByVal pAnalyzerID As String = "", _
                                             Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()

                        resultData = mytwksResultDAO.UpdateAcceptedResult(dbConnection, pOrderTestID, pRerunNumber, pNewAcceptedValue, pAnalyzerID, pWorkSessionID)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateAcceptedResult", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Collapse field into the twksResults table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  AG 02/08/2010
        ''' </remarks>
        Public Function UpdateCollapse(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultsDAO

                        For Each row As ResultsDS.twksResultsRow In pResultsDS.twksResults
                            resultData = myDAO.UpdateCollapse(dbConnection, row.Collapsed, row.OrderTestID, row.RerunNumber, row.MultiPointNumber)
                            If resultData.HasError Then Exit For
                        Next

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
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateCollapse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields ExportStatus and ExportDataTime for the group of results sent to and external LIMS system
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the exported results</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        '''Created by:  TR 14/05/2010
        '''Modified by: SG 10/04/2013 - new parameter "pAlternativeStatus"
        ''' AG 30/07/2014 - #1887 OrderToExport management - NOT REQUIRED in current scenarios
        ''' </remarks>
        Public Function UpdateExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, ByVal pAlternativeStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        myGlobalDataTO = mytwksResultDAO.UpdateExportStatus(dbConnection, pResultsDS, pAlternativeStatus)

                        'AG 30/07/2014 #1887 NOT required because this method is used:
                        '     isLISWithFilesMode (ExportStatus = TRUE) - out of date!!
                        '     not isLISWithFilesMode (ExportStatus = FALSE because not msg to LIS can be sent, so OrderToExport is already TRUE)
                        'If Not myGlobalDataTO.HasError Then
                        '    myGlobalDataTO = Me.UpdateOrderToExportAfterChangesInExportStatus(Nothing, pResultsDS, pAlternativeStatus)
                        'End If

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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateExportStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update fields ExportStatus and ExportDateTime for a group of Patient Results that have been selected to be exported 
        ''' manually to LIS. Update also timestamp fields (TS_User and TS_DateTime). This function does not fulfill the template 
        ''' for DELEGATE functions due to the quantity of data the DAO function has to update can be huge
        ''' </summary>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing all Patient Results that have been exported</param>
        ''' <param name="pAlternativeStatus">When informed, the status of all Results will be updated with this value</param>
        ''' <returns>GlobalDataTO containing success/errorinformation</returns>
        ''' <remarks>
        ''' Created by:  SA 12/02/2014 - BT #1497
        ''' AG 30/07/2014 - #1887 OrderToExport management - NOT REQUIRED in current scenarios
        ''' </remarks>
        Public Function UpdateExportStatusMASIVE(ByVal pResultsDS As ResultsDS, ByVal pAlternativeStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing

            Try
                Dim mytwksResultDAO As New twksResultsDAO()
                myGlobalDataTO = mytwksResultDAO.UpdateExportStatusMASIVE(pResultsDS, pAlternativeStatus)

                'AG 30/07/2014 #1887 NOT required because this method is NEVER used:
                'If Not myGlobalDataTO.HasError Then
                '    myGlobalDataTO = Me.UpdateOrderToExportAfterChangesInExportStatus(Nothing, pResultsDS, pAlternativeStatus)
                'End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateExportStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Updates fields ManualResultFlag, ManualResult and ManualResultText for the specified OrderTestID/MultiItemNumber/RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pManualResultFlag">When TRUE, parameter pManualResult or pManualResultText has to be informed (according value
        '''                                 of parameter pResultType)</param>
        ''' <param name="pResultType">QUANTITATIVE or QUALITATIVE</param>
        ''' <param name="pManualResult">Informed when ManualResultFlag is TRUE, and ResultType is QUANTITATIVE</param>
        ''' <param name="pManualResultText">Informed when ManualResultFlag is TRUE, and ResultType is QUALITATIVE</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pMultiItemNumber">MultiItem Number (only for multipoint Calibrators; otherwise, its value is always one</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pExportStatus"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: SA 26/11/2010 - Changed parameter pRerunNumber to optional
        '''              AG 16/1/2014 BA-2011 do not use optional parameters + add new parameter ExportStatus and also update ExportStatus and ExportDateTime
        ''' </remarks>
        Public Function UpdateManualResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pManualResultFlag As Boolean, ByVal pResultType As String, _
                                           ByVal pManualResult As Single, ByVal pManualResultText As String, ByVal pOrderTestID As Integer, _
                                           ByVal pMultiItemNumber As Integer, ByVal pRerunNumber As Integer, ByVal pExportStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultsDAO
                        resultData = myDAO.UpdateManualResult(dbConnection, pManualResultFlag, pResultType, pManualResult, pManualResultText, _
                                                              pOrderTestID, pMultiItemNumber, pRerunNumber, pExportStatus)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateManualResult", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Printed field into the twksResults table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  RH 22/09/2010
        ''' </remarks>
        Public Function UpdatePrinted(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultsDAO

                        For Each row As ResultsDS.twksResultsRow In pResultsDS.twksResults
                            resultData = myDAO.UpdatePrinted(dbConnection, row.Printed, row.OrderTestID)
                            If resultData.HasError Then Exit For
                        Next

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

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdatePrinted", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Results for the specified OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by: XB 21/03/2013 
        ''' </remarks>
        Public Function GetResultsByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetResultsByOrderTest(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsByOrderTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Fill the column SpecimenIDList in the results DS using the required elements information
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pEntryRow"></param>
        ''' <returns>GlobalDataTo (ResultsDS.XtraSamplesRow)</returns>
        ''' <remarks>AG 28/06/2013</remarks>
        Public Function FillSpecimenIDList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByRef pEntryRow As ResultsDS.XtraSamplesRow) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reqElementsDlg As New WSRequiredElementsDelegate
                        resultData = reqElementsDlg.GetLISPatientElements(dbConnection, pWorkSessionID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim reqElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            Dim linqRes As List(Of ResultsDS.XtraSamplesRow)
                            Dim patsampleID As String = String.Empty
                            For Each row As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElementsDS.twksWSRequiredElements
                                'Apply next business only for the patient / sample ID with barcode informed
                                If Not row.IsSpecimenIDListNull AndAlso row.SpecimenIDList <> String.Empty Then
                                    If Not row.IsPatientIDNull Then
                                        patsampleID = row.PatientID
                                    ElseIf Not row.IsSampleIDNull Then
                                        patsampleID = row.SampleID
                                    Else
                                        patsampleID = String.Empty
                                    End If

                                    If patsampleID <> String.Empty Then
                                        If pEntryRow.PatientID = patsampleID AndAlso pEntryRow.SampleType = row.SampleType Then
                                            pEntryRow.BeginEdit()
                                            If pEntryRow.IsSpecimenIDListNull Then
                                                pEntryRow.SpecimenIDList = row.SpecimenIDList.Replace(CChar(vbCrLf), ", ")
                                            Else
                                                pEntryRow.SpecimenIDList &= ", " & row.SpecimenIDList
                                            End If
                                            pEntryRow.EndEdit()
                                        End If

                                    End If
                                End If
                            Next
                            linqRes = Nothing
                        End If
                        resultData.SetDatos = pEntryRow

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.FillSpecimenIDList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Fill the column SpecimenIDList in the results DS using the required elements information
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pResultsDS"></param>
        ''' <returns>GlobalDataTo (ResultsDS)</returns>
        ''' <remarks>AG 28/06/2013</remarks>
        Public Function FillSpecimenIDListForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByRef pResultsDS As ResultsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reqElementsDlg As New WSRequiredElementsDelegate
                        resultData = reqElementsDlg.GetLISPatientElements(dbConnection, pWorkSessionID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim reqElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            Dim linqRes As List(Of ResultsDS.vwksResultsRow)
                            Dim patsampleID As String = String.Empty
                            For Each row As WSRequiredElementsDS.twksWSRequiredElementsRow In reqElementsDS.twksWSRequiredElements
                                'Apply next business only for the patient / sample ID with barcode informed
                                If Not row.IsSpecimenIDListNull AndAlso row.SpecimenIDList <> String.Empty Then
                                    If Not row.IsPatientIDNull Then
                                        patsampleID = row.PatientID
                                    ElseIf Not row.IsSampleIDNull Then
                                        patsampleID = row.SampleID
                                    Else
                                        patsampleID = String.Empty
                                    End If

                                    If patsampleID <> String.Empty Then
                                        linqRes = (From a As ResultsDS.vwksResultsRow In pResultsDS.vwksResults _
                                                   Where a.SampleType = row.SampleType _
                                                   AndAlso (Not a.IsPatientIDNull AndAlso a.PatientID = patsampleID) _
                                                   Select a).ToList

                                        If linqRes.Count > 0 Then
                                            For Each resRow As ResultsDS.vwksResultsRow In linqRes
                                                resRow.BeginEdit()
                                                If resRow.IsSpecimenIDListNull Then
                                                    'In case the field contains more than one SpecimenID, replace the Carriage Return by a comma
                                                    resRow.SpecimenIDList = row.SpecimenIDList.Replace(CChar(vbCrLf), ", ")
                                                Else
                                                    resRow.SpecimenIDList &= ", " & row.SpecimenIDList
                                                End If
                                                resRow.EndEdit()
                                            Next
                                            pResultsDS.AcceptChanges()
                                        End If
                                    End If
                                End If
                            Next
                            linqRes = Nothing
                        End If
                        resultData.SetDatos = pResultsDS

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.FillSpecimenIDListForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Fill the reference range interval values of the results
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pResultsDS"></param>
        ''' <remarks></remarks>
        Public Sub FillReferenceRangeInterval(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pResultsDS As ResultsDS)

            Dim resultData As GlobalDataTO = Nothing

            'Read Reference Range Limits
            Dim MinimunValue As Nullable(Of Single) = Nothing
            Dim MaximunValue As Nullable(Of Single) = Nothing

            Dim myOrderTestsDelegate As New OrderTestsDelegate
            For Each resultRow As ResultsDS.vwksResultsRow In pResultsDS.vwksResults.Rows
                If (Not resultRow.IsActiveRangeTypeNull) Then
                    Dim mySampleType As String = String.Empty
                    If (resultRow.TestType <> "CALC") Then mySampleType = resultRow.SampleType

                    'Get the Reference Range for the Test/SampleType according the TestType and the Type of Range
                    resultData = myOrderTestsDelegate.GetReferenceRangeInterval(pDBConnection, resultRow.OrderTestID, resultRow.TestType, _
                                                                                resultRow.TestID, mySampleType, resultRow.ActiveRangeType)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myTestRefRangesDS As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                        If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
                            MinimunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
                            MaximunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit
                        End If
                    End If

                    If (MinimunValue.HasValue AndAlso MaximunValue.HasValue) Then
                        If (MinimunValue <> -1 AndAlso MaximunValue <> -1) Then
                            resultRow.NormalLowerLimit = MinimunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                            resultRow.NormalUpperLimit = MaximunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                        End If
                    End If
                End If
            Next resultRow

        End Sub
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Add the las exported results to class level dataset.
        ''' </summary>
        ''' <param name="pExecutionsDS"></param>
        ''' <remarks>CREATED BY: TR 28/06/2013</remarks>
        Private Sub AddIntoLastExportedResults(ByVal pExecutionsDS As ExecutionsDS)
            Try
                'Move row from parameter to lastExportedResultsDSAttribute
                SyncLock lockThis
                    For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions.Rows
                        lastExportedResultsDSAttribute.twksWSExecutions.ImportRow(row)
                    Next
                    lastExportedResultsDSAttribute.twksWSExecutions.AcceptChanges()
                End SyncLock

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.AddIntoLastExportedResults", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' When the export frequency is on finish every test we must search for all calculated tests results whose results needs the pOrderTestID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pSampleClass"></param>
        ''' <param name="pOrderID"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pIncludedOTList">List of ordertests already included into results to export DS, so is not necessary to evaluate it again</param>
        ''' <param name="pNewAddedFlag">Returned as TRUE when a new item is added into pIncludedOTList</param>
        ''' <returns>GlobalDataTo (ResultsDS.vwksResults)</returns>
        ''' <remarks>AG 24/07/2013</remarks>
        Private Function GetCalcTestsResultsToExport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                     ByVal pSampleClass As String, ByVal pOrderID As String, ByVal pOrderTestID As Integer, _
                                                     ByRef pIncludedOTList As List(Of Integer), ByRef pNewAddedFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim dataToReturn As New ResultsDS
                        Dim otCalcDlg As New OrderCalculatedTestsDelegate

                        resultData = otCalcDlg.GetByOrderTestID(dbConnection, pOrderTestID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myOTCalcDS As New OrderCalculatedTestsDS
                            myOTCalcDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)
                            Dim auxResultsDS As New ResultsDS
                            Dim mytwksResultDAO As New twksResultsDAO

                            For Each row As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myOTCalcDS.twksOrderCalculatedTests.Rows
                                If Not row.IsCalcOrderTestIDNull Then
                                    'Search calculated test results and add into dataset of data to export (only if not included yet)
                                    If Not pIncludedOTList.Contains(row.CalcOrderTestID) Then
                                        pIncludedOTList.Add(row.CalcOrderTestID)
                                        pNewAddedFlag = True
                                        resultData = mytwksResultDAO.GetResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID, pSampleClass, pOrderID, row.CalcOrderTestID)

                                        'Add results into dataToReturn
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            auxResultsDS.Clear()
                                            auxResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                                            For Each resRow As ResultsDS.vwksResultsRow In auxResultsDS.vwksResults
                                                dataToReturn.vwksResults.ImportRow(resRow)
                                            Next
                                            dataToReturn.vwksResults.AcceptChanges()
                                        End If
                                    End If
                                End If

                            Next
                        End If
                        resultData.SetDatos = dataToReturn

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetCalcTestsResultsToExport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "METHODS FOR EXPORT CONTROL RESULTS TO QC MODULE"

        ''' <summary>
        ''' Create a new TestControl information in the history table in QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code: STD or ISE</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/Sample in QC Module</param>
        ''' <param name="pQCControlID">Identifier of the Control/Lot in QC Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011 
        ''' Modified by: SA 21/05/2012 - Added parameter for the TestType
        ''' </remarks>
        Private Function CreateNewHistoryTestControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                        ByVal pSampleType As String, ByVal pControlID As Integer, ByVal pQCTestSampleID As Integer, _
                                                        ByVal pQCControlID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Get the values for Min/Max Concentration values defined for the TestType/TestID/SampleType and ControlID
                Dim myTestControlDelegate As New TestControlsDelegate
                myGlobalDataTO = myTestControlDelegate.GetControlsNEW(pDBConnection, pTestType, pTestID, pSampleType, pControlID)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myTestControlDS As TestControlsDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)

                    If (myTestControlDS.tparTestControls.Count > 0) Then
                        Dim myHistoryTestControlLotsDS As New HistoryTestControlLotsDS
                        Dim myHistoryTestControlLotsRow As HistoryTestControlLotsDS.tqcHistoryTestControlLotsRow

                        'Add values to the needed DS
                        myHistoryTestControlLotsRow = myHistoryTestControlLotsDS.tqcHistoryTestControlLots.NewtqcHistoryTestControlLotsRow()
                        myHistoryTestControlLotsRow.QCTestSampleID = pQCTestSampleID
                        myHistoryTestControlLotsRow.QCControlLotID = pQCControlID
                        myHistoryTestControlLotsRow.MinConcentration = myTestControlDS.tparTestControls.First.MinConcentration
                        myHistoryTestControlLotsRow.MaxConcentration = myTestControlDS.tparTestControls.First.MaxConcentration
                        myHistoryTestControlLotsDS.tqcHistoryTestControlLots.AddtqcHistoryTestControlLotsRow(myHistoryTestControlLotsRow)

                        'Create the TestControl information in the history table in QC Module
                        Dim myHistoryTestControlLotsDelegate As New HistoryTestControlLotsDelegate
                        myGlobalDataTO = myHistoryTestControlLotsDelegate.Create(pDBConnection, myHistoryTestControlLotsDS)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.CreateNewHistoryTestControl", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a new Runs Group for the informed QCTestSampleID/QCControlLotID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/Sample in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Number of the new Runs Group to create</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RunsGroupsDS with the information of the created Runs Group</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' Modified by: SA 06/06/2012 - Added parameter for AnalyzerID and inform it in the typed Dataset RunsGroupsDS used to 
        '''                              save the new Runs Group
        ''' </remarks>
        Private Function CreateNewRunGroupsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                               ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myRunGroupsDS As New RunsGroupsDS
                Dim myRunGroupsRow As RunsGroupsDS.tqcRunsGroupsRow

                'Add all information to the needed DS
                myRunGroupsRow = myRunGroupsDS.tqcRunsGroups.NewtqcRunsGroupsRow
                myRunGroupsRow.QCTestSampleID = pQCTestSampleID
                myRunGroupsRow.QCControlLotID = pQCControlLotID
                myRunGroupsRow.AnalyzerID = pAnalyzerID
                myRunGroupsRow.RunsGroupNumber = pRunsGroupNumber
                myRunGroupsRow.ClosedRunsGroup = False
                myRunGroupsDS.tqcRunsGroups.AddtqcRunsGroupsRow(myRunGroupsRow)

                'Create the Run Group in table tqcRunsGroups
                Dim myRunGroupsDelegate As New RunsGroupsDelegate
                myGlobalDataTO = myRunGroupsDelegate.CreateNEW(pDBConnection, myRunGroupsDS)

                'If the RunsGroup is created, return the RunsGroupsDS
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO.SetDatos = myRunGroupsDS
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.CreateNewRunGroups", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the accepted and validated QC Results for all Controls requested in the specified WS Analyzer 
        ''' and export them to QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pQCResultsDS">Optional parameter. When informed, it is a typed DS QCResultsDS containing the QC Results
        '''                            entered in the QC Results Simulator screen</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/07/2011 - New implementation
        ''' </remarks>
        Public Function ExportControlResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                Optional ByVal pQCResultsDS As QCResultsDS = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDS As QCResultsDS = Nothing

                        If (pQCResultsDS Is Nothing) Then
                            'Get the QC Results accepted and validated in the informed WorkSession
                            Dim myWSResultsDAO As New twksResultsDAO
                            myGlobalDataTO = myWSResultsDAO.ReadWSControlResultsNEW(dbConnection, pWorkSessionID, pAnalyzerID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)
                            End If
                        Else
                            'Get QC Results entered in the QC Results Simulator screen
                            myQCResultsDS = pQCResultsDS
                        End If

                        If (Not myGlobalDataTO.HasError AndAlso myQCResultsDS.tqcResults.Rows.Count > 0) Then
                            myGlobalDataTO = MoveResultsToQCModuleNEW(dbConnection, myQCResultsDS)
                        End If

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.ExportControlResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For all different pairs of TestTypes/TestIDs/SampleTypes and ControlIDs/LotNumbers:
        ''' ** Verify if the link already exists in QC Module; if it does not exist, get Min/Max Concentration values and created it
        ''' ** When needed, create the RunsGroupNumber in which all the results have to be added
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing a group of results from the active WorkSession that has 
        '''                            to be exported to QC Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' </remarks>
        Private Function ManageQCResultsRunsGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerID As String = pQCResultsDS.tqcResults.First.AnalyzerID

                        Dim qcTestSampleID As Integer
                        Dim qcControlLotID As Integer
                        Dim myRunsGroupNumber As Integer
                        Dim mydiffElemParts() As String
                        Dim myLastRunsGroupStatus As Boolean
                        Dim myCalculationMode As String = String.Empty

                        Dim myHistTestControlsDS As New HistoryTestControlLotsDS
                        Dim myHistTestControlsDelegate As New HistoryTestControlLotsDelegate

                        Dim myRunGroupsDS As RunsGroupsDS
                        Dim myRunGroupsDelegate As New RunsGroupsDelegate

                        Dim myQCResultsDelegate As New QCResultsDelegate
                        Dim lstResults As List(Of QCResultsDS.tqcResultsRow)

                        'Get all different pairs of QCTestSampleID/QCControlLotID in the entry DS 
                        Dim lstQCIDs As List(Of String) = (From a In pQCResultsDS.tqcResults _
                                                         Select a.QCTestSampleID & "|" & a.QCControlLotID).Distinct.ToList
                        For Each row As String In lstQCIDs
                            mydiffElemParts = row.Split(CChar("|"))
                            qcTestSampleID = Convert.ToInt32(mydiffElemParts(0))
                            qcControlLotID = Convert.ToInt32(mydiffElemParts(1))

                            'Get all Results for the QCTestSampleID and QCControlLotID
                            lstResults = (From b As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults _
                                             Where b.QCTestSampleID = qcTestSampleID _
                                           AndAlso b.QCControlLotID = qcControlLotID _
                                            Select b).ToList

                            'Verify if the link between the TestType/TestID/SampleType and the ControlID/LotNumber already exist in QC Module
                            myGlobalDataTO = myHistTestControlsDelegate.ReadByTestSampleIDAndControlID(dbConnection, qcTestSampleID, qcControlLotID)
                            If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For

                            myHistTestControlsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestControlLotsDS)
                            If (myHistTestControlsDS.tqcHistoryTestControlLots.Rows.Count = 0) Then
                                'The link does not exist; create it
                                myGlobalDataTO = CreateNewHistoryTestControlNEW(dbConnection, lstResults.First.TestType, lstResults.First.TestID, _
                                                                                lstResults.First.SampleType, lstResults.First.ControlID, qcTestSampleID, qcControlLotID)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'Create the first RunsGroup for the informed QCTestSampleID/QCControlLotID
                                myRunsGroupNumber = 1
                                myGlobalDataTO = CreateNewRunGroupsNEW(dbConnection, qcTestSampleID, qcControlLotID, myAnalyzerID, myRunsGroupNumber)
                                If (myGlobalDataTO.HasError) Then Exit For
                            Else
                                'The link exists; verify if there are RunsGroups created for the informed QCTestSampleID/QCControlLotID
                                myGlobalDataTO = myRunGroupsDelegate.ReadByQCTestSampleIDAndQCControlLotIDNEW(dbConnection, qcTestSampleID, qcControlLotID, myAnalyzerID)
                                If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For

                                myRunGroupsDS = DirectCast(myGlobalDataTO.SetDatos, RunsGroupsDS)
                                If (myRunGroupsDS.tqcRunsGroups.Count = 0) Then
                                    'There are not RunsGroups for the QCTestSampleID/QCControlLotID: insert the first RunsGroup by creating RunsGroupNumber=1 
                                    'in table tqcRunsGroups
                                    myRunsGroupNumber = 1

                                    myGlobalDataTO = CreateNewRunGroupsNEW(dbConnection, qcTestSampleID, qcControlLotID, myAnalyzerID, myRunsGroupNumber)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Else
                                    'There are RunsGroups for the QCTestSampleID/QCControlLotID, get the last one: MAX(RunsGroupNumber)
                                    myRunsGroupNumber = (From a In myRunGroupsDS.tqcRunsGroups _
                                                       Select a.RunsGroupNumber).Max

                                    'Verify if the last RunsGroup is closed (due to results in it have been cumulated)
                                    myLastRunsGroupStatus = (From a In myRunGroupsDS.tqcRunsGroups _
                                                            Where a.RunsGroupNumber = myRunsGroupNumber _
                                                           Select a).First().ClosedRunsGroup

                                    'Get value of CalculationMode for the informed QCTestSampleID/QCControlLotID
                                    myCalculationMode = lstResults.First.CalculationMode
                                    If (myCalculationMode = "MANUAL") Then
                                        If (myLastRunsGroupStatus) Then
                                            'Delete all open QC Results marked with IncludedInMean = TRUE for the TestType/TestID/SampleType and 
                                            'ControlID/LotNumber before create the new RunsGroup
                                            myGlobalDataTO = myQCResultsDelegate.DeleteStatisticResultsNEW(dbConnection, qcTestSampleID, qcControlLotID, myAnalyzerID)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            'The last RunsGroup is closed; a new one has to be created with RunsGroupNumber + 1
                                            myRunsGroupNumber += 1

                                            myGlobalDataTO = CreateNewRunGroupsNEW(dbConnection, qcTestSampleID, qcControlLotID, myAnalyzerID, myRunsGroupNumber)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    Else
                                        If (myLastRunsGroupStatus) Then
                                            'The last RunsGroup is closed; a new one has to be created with RunsGroupNumber + 1, but the created RunsGroup will 
                                            'not be considered as a new one
                                            myRunsGroupNumber += 1

                                            myGlobalDataTO = CreateNewRunGroupsNEW(dbConnection, qcTestSampleID, qcControlLotID, myAnalyzerID, myRunsGroupNumber)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            'Move all QC Results marked as included in Mean and belonging to the closed RunsGroup to the new created one
                                            myGlobalDataTO = myQCResultsDelegate.MoveStatisticResultsNEW(dbConnection, qcTestSampleID, qcControlLotID, myAnalyzerID, myRunsGroupNumber)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    End If
                                End If
                            End If

                            'Get all results in the entry DataSet for the QCTestSampleID/QCControlLotID and update value of field  
                            'RunsGroupNumber for each one of them
                            For Each result As QCResultsDS.tqcResultsRow In lstResults
                                result.BeginEdit()
                                result.RunsGroupNumber = myRunsGroupNumber
                                result.EndEdit()
                            Next result
                        Next row

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.ManageQCResultsRunsGroup", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of QC Results, get all different ControlID/LotNumber and for each one of them, verify if it already exists
        ''' in QC Module and create it in table tqcHistoryControlLots when not; finally, field QCControlLotID is updated in the entry DataSet
        ''' with the value read/generated for all QC Results for the ControlID/LotNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing a group of results from the active WorkSession that has 
        '''                            to be exported to QC Module. When this function finishes, it is returned with field QCControlLotID
        '''                            informed for all QC Results</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' </remarks>
        Private Function MoveControlLotToQC(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mydiffElemParts() As String
                        Dim myQCControlLotID As Integer

                        Dim myControlsDS As New ControlsDS
                        Dim myControlLotsDelegate As New ControlsDelegate

                        Dim myHistControlLotsDS As HistoryControlLotsDS
                        Dim myHistControlLotsToAddDS As New HistoryControlLotsDS
                        Dim myHistControlLotsDelegate As New HistoryControlLotsDelegate
                        Dim myHistoryControlLotRow As HistoryControlLotsDS.tqcHistoryControlLotsRow

                        Dim lstResultsByControlLot As List(Of QCResultsDS.tqcResultsRow)

                        'Get the list of different ControlID/LotNumber in the list of QC Results 
                        Dim lstControlsList As List(Of String) = (From a In pQCResultsDS.tqcResults _
                                                                Select a.ControlID & "|" & a.LotNumber).Distinct.ToList

                        For Each row As String In lstControlsList
                            mydiffElemParts = row.Split(CChar("|"))

                            'Search if the ControlID/LotNumber already exists in the history table of Controls/LotNumbers in QC Module
                            myGlobalDataTO = myHistControlLotsDelegate.GetQCControlLotIDByControlIDAndLotNumber(dbConnection, Convert.ToInt32(mydiffElemParts(0)), mydiffElemParts(1))
                            If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For

                            myHistControlLotsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryControlLotsDS)
                            If (myHistControlLotsDS.tqcHistoryControlLots.Count > 0) Then
                                'The ControlID/LotNumber already exist; get the ID in QC Module
                                myQCControlLotID = myHistControlLotsDS.tqcHistoryControlLots(0).QCControlLotID
                            Else
                                'The ControlID/LotNumber does not exist in QCModule; get all data needed to add it
                                myGlobalDataTO = myControlLotsDelegate.GetControlData(dbConnection, Convert.ToInt32(mydiffElemParts(0)))
                                If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For

                                myControlsDS = DirectCast(myGlobalDataTO.SetDatos, ControlsDS)
                                If (myControlsDS.tparControls.Rows.Count = 1) Then
                                    myHistControlLotsToAddDS.Clear()

                                    myHistoryControlLotRow = myHistControlLotsToAddDS.tqcHistoryControlLots.NewtqcHistoryControlLotsRow()
                                    myHistoryControlLotRow.ControlID = Convert.ToInt32(mydiffElemParts(0))
                                    myHistoryControlLotRow.LotNumber = mydiffElemParts(1)
                                    myHistoryControlLotRow.CreationDate = DateTime.Now
                                    myHistoryControlLotRow.ControlName = myControlsDS.tparControls.First.ControlName
                                    myHistoryControlLotRow.SampleType = myControlsDS.tparControls.First.SampleType
                                    myHistoryControlLotRow.ClosedLot = False
                                    myHistoryControlLotRow.DeletedControl = False
                                    myHistControlLotsToAddDS.tqcHistoryControlLots.AddtqcHistoryControlLotsRow(myHistoryControlLotRow)
                                    myHistControlLotsToAddDS.AcceptChanges()

                                    'Create the Control/Lot in tqcHistoryControlLots
                                    myGlobalDataTO = myHistControlLotsDelegate.Create(dbConnection, myHistControlLotsToAddDS)
                                    If (myGlobalDataTO.HasError) Then Exit For

                                    'Get the ID generated automatically by the DB
                                    If (myHistControlLotsToAddDS.tqcHistoryControlLots.Count > 0) Then
                                        myQCControlLotID = myHistControlLotsToAddDS.tqcHistoryControlLots(0).QCControlLotID
                                    Else
                                        'This case should not be possible...
                                        Exit For
                                    End If
                                Else
                                    'This case should not be possible...
                                    Exit For
                                End If
                            End If

                            'Get all results in the entry DataSet for the ControlID/LotNumber and update value of field  
                            'QCControlLotID for each one of them
                            lstResultsByControlLot = (From b As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults _
                                                     Where b.ControlID = Convert.ToInt32(mydiffElemParts(0)) _
                                                   AndAlso b.LotNumber = mydiffElemParts(1) _
                                                    Select b).ToList

                            For Each result As QCResultsDS.tqcResultsRow In lstResultsByControlLot
                                result.BeginEdit()
                                result.QCControlLotID = myQCControlLotID
                                result.EndEdit()
                            Next result
                        Next row

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                        'Finally, return the same DS received as entry parameter (updated or not)
                        myGlobalDataTO.SetDatos = pQCResultsDS
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveControlLotToQC", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Export to QC Module all accepted and validated QC Results in the active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DS QCResultsDS containing the group of QC Results to be exported to QC Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 21/05/2012
        ''' </remarks>
        Public Function MoveResultsToQCModuleNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDS As QCResultsDS = pQCResultsDS
                        Dim myAnalyzerID As String = pQCResultsDS.tqcResults.First.AnalyzerID

                        'Search if the different TestTypes/TestIDs/SampleTypes already exists in the history table of Tests/SampleTypes in QC Module
                        myGlobalDataTO = MoveTestSampleTypeToQC(dbConnection, myQCResultsDS)
                        If (Not myGlobalDataTO.HasError) Then
                            'Search if the different ControlIDs/LotNumbers already exists in the history table of Controls/Lots in QC Module
                            myGlobalDataTO = MoveControlLotToQC(dbConnection, myQCResultsDS)
                            If (Not myGlobalDataTO.HasError) Then
                                'Search if the different links between TestTypes/TestIDs/SampleTypes and ControlIDs/LotNumbers already exists in QC Module,
                                'and verify if a new RunsGroup has to be created for each one of them
                                myGlobalDataTO = ManageQCResultsRunsGroup(dbConnection, myQCResultsDS)
                                If (Not myGlobalDataTO.HasError) Then
                                    'Get the list of different QCTestSampleIDs in the QC Results DataSet
                                    Dim lstQCTestSampleIDs As List(Of Integer) = (From a In myQCResultsDS.tqcResults _
                                                                                  Select a.QCTestSampleID).Distinct.ToList()

                                    Dim nextRunNumber As Integer = 0
                                    Dim myQCResultsDelegate As New QCResultsDelegate
                                    Dim myDistCtrlSendGroup As New List(Of Integer)

                                    'Dim lstDates As List(Of Date)
                                    Dim lstResults As List(Of QCResultsDS.tqcResultsRow)

                                    For Each qcTestSampleID As Integer In lstQCTestSampleIDs
                                        'Get the maximum RunNumber that exists currently in QC Module for the QCTestSampleID in process
                                        myGlobalDataTO = myQCResultsDelegate.GetMaxRunNumberByTestSample(dbConnection, qcTestSampleID, myAnalyzerID)
                                        If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For

                                        nextRunNumber = CType(myGlobalDataTO.SetDatos, Integer) + 1

                                        'TR 19/07/2012 -New Implementation.
                                        'Search distinct ctrlSendingGroup by QCTestSampleID 
                                        myDistCtrlSendGroup = (From b As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                                               Where b.QCTestSampleID = qcTestSampleID _
                                                               AndAlso Not b.IsCtrlsSendingGroupNull _
                                                               Select b.CtrlsSendingGroup Distinct).ToList()


                                        For Each CtrlSendingGroupDistinct As Integer In myDistCtrlSendGroup
                                            'For each distintc group get all the results order by ResultDateTime
                                            lstResults = (From a As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                                         Where a.QCTestSampleID = qcTestSampleID _
                                                       AndAlso a.CtrlsSendingGroup = CtrlSendingGroupDistinct _
                                                      Order By a.ResultDateTime).ToList()

                                            For Each result As QCResultsDS.tqcResultsRow In lstResults
                                                result.BeginEdit()
                                                result.RunNumber = nextRunNumber
                                                result.ValidationStatus = "PENDING"
                                                result.Excluded = False
                                                result.ClosedResult = False
                                                result.TS_DateTime = Now
                                                result.EndEdit()
                                            Next result

                                            'Increase the value
                                            nextRunNumber += 1
                                        Next

                                    Next qcTestSampleID

                                    'Finally, insert all QC Results in table tqcResults
                                    If (Not myGlobalDataTO.HasError) Then
                                        myGlobalDataTO = myQCResultsDelegate.CreateNEW(dbConnection, myQCResultsDS)
                                    End If

                                    'For all Test/SampleTypes with CalculationMode STATISTICS, for all linked Control/Lots, mark as included
                                    'in statistics the first Number of Series open QC Results
                                    If (Not myGlobalDataTO.HasError) Then
                                        myGlobalDataTO = myQCResultsDelegate.SetResultsForStatisticsNEW(dbConnection, myAnalyzerID)
                                    End If
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
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveResultsToQCModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of QC Results, get all different TestType/TestID/SampleType and for each one of them, verify if it already exists
        ''' in QC Module and create it in table tqcHistoryTestSamples when not; finally, field QCTestSampleID is updated in the entry DataSet
        ''' with the value read/generated for all QC Results for the TestType/TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing a group of results from the active WorkSession that has 
        '''                            to be exported to QC Module. When this function finishes, it is returned with field QCTestSampleID
        '''                            informed for all QC Results</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' </remarks>
        Private Function MoveTestSampleTypeToQC(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim isNEW As Boolean
                        Dim myCalcMode As String
                        Dim mydiffElemParts() As String
                        Dim myQCTestSampleID As Integer
                        Dim myTestSamplesDelegate As New TestSamplesDelegate
                        Dim myISETestSamplesDelegate As New ISETestSamplesDelegate
                        Dim myHistTestSamplesDS As HistoryTestSamplesDS
                        Dim myHistTestSamplesToAddDS As New HistoryTestSamplesDS
                        Dim myHistTestSamplesDelegate As New HistoryTestSamplesDelegate
                        Dim myHistTestSamplesRulesDelegate As New HistoryTestSamplesRulesDelegate
                        Dim myHistoryTestSampleRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow
                        Dim lstResultsByTestSample As List(Of QCResultsDS.tqcResultsRow)

                        'Get the list of different TestType/TestID/SampleType in the list of QC Results 
                        Dim lstTestsList As List(Of String) = (From a In pQCResultsDS.tqcResults _
                                                               Select a.TestType & "|" & a.TestID & "|" & a.SampleType).Distinct.ToList

                        For Each row As String In lstTestsList
                            mydiffElemParts = row.Split(CChar("|"))

                            'Verify if the TestType/TestID/SampleType already exists in QC Module
                            myGlobalDataTO = myHistTestSamplesDelegate.ReadByTestIDAndSampleTypeNEW(dbConnection, mydiffElemParts(0), Convert.ToInt32(mydiffElemParts(1)), _
                                                                                                    mydiffElemParts(2))
                            If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For

                            myHistTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                            If (myHistTestSamplesDS.tqcHistoryTestSamples.Rows.Count > 0) Then
                                'The TestType/TestID/SampleType already exist; get the ID in QC Module and the CalculationMode
                                myQCTestSampleID = myHistTestSamplesDS.tqcHistoryTestSamples.First.QCTestSampleID
                                myCalcMode = myHistTestSamplesDS.tqcHistoryTestSamples.First.CalculationMode
                                isNEW = False
                            Else
                                'The TestType/TestID/SampleType does not exist in QCModule; get all data needed to add it, depending on the TestType
                                If (mydiffElemParts(0) = "STD") Then
                                    myGlobalDataTO = myTestSamplesDelegate.GetDefinitionForQCModule(dbConnection, Convert.ToInt32(mydiffElemParts(1)), mydiffElemParts(2))
                                    If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For

                                ElseIf (mydiffElemParts(0) = "ISE") Then
                                    myGlobalDataTO = myISETestSamplesDelegate.GetDefinitionForQCModule(dbConnection, Convert.ToInt32(mydiffElemParts(1)), mydiffElemParts(2))
                                    If (myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos Is Nothing) Then Exit For
                                End If

                                myHistTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                                If (myHistTestSamplesDS.tqcHistoryTestSamples.Rows.Count = 1) Then
                                    'Test/SampleType does not exist in QC Module, load its values in a local DS and create it
                                    myHistTestSamplesToAddDS.Clear()

                                    myHistoryTestSampleRow = myHistTestSamplesToAddDS.tqcHistoryTestSamples.NewtqcHistoryTestSamplesRow
                                    myHistoryTestSampleRow.TestType = mydiffElemParts(0)
                                    myHistoryTestSampleRow.TestID = Convert.ToInt32(mydiffElemParts(1))
                                    myHistoryTestSampleRow.SampleType = mydiffElemParts(2)
                                    myHistoryTestSampleRow.CreationDate = DateTime.Now
                                    myHistoryTestSampleRow.TestName = myHistTestSamplesDS.tqcHistoryTestSamples.First.TestName
                                    myHistoryTestSampleRow.TestShortName = myHistTestSamplesDS.tqcHistoryTestSamples.First.TestShortName
                                    myHistoryTestSampleRow.PreloadedTest = myHistTestSamplesDS.tqcHistoryTestSamples.First.PreloadedTest
                                    myHistoryTestSampleRow.MeasureUnit = myHistTestSamplesDS.tqcHistoryTestSamples.First.MeasureUnit
                                    myHistoryTestSampleRow.DecimalsAllowed = myHistTestSamplesDS.tqcHistoryTestSamples.First.DecimalsAllowed
                                    myHistoryTestSampleRow.RejectionCriteria = myHistTestSamplesDS.tqcHistoryTestSamples.First.RejectionCriteria
                                    myHistoryTestSampleRow.CalculationMode = myHistTestSamplesDS.tqcHistoryTestSamples.First.CalculationMode
                                    myHistoryTestSampleRow.NumberOfSeries = myHistTestSamplesDS.tqcHistoryTestSamples.First.NumberOfSeries
                                    myHistoryTestSampleRow.DeletedTest = False
                                    myHistoryTestSampleRow.DeletedSampleType = False
                                    myHistoryTestSampleRow.TestLongName = myHistTestSamplesDS.tqcHistoryTestSamples.First.TestLongName  ' WE 31/07/2014 - #1865
                                    myHistTestSamplesToAddDS.tqcHistoryTestSamples.AddtqcHistoryTestSamplesRow(myHistoryTestSampleRow)
                                    myHistTestSamplesToAddDS.AcceptChanges()

                                    myGlobalDataTO = myHistTestSamplesDelegate.CreateNEW(dbConnection, myHistTestSamplesToAddDS)
                                    If (myGlobalDataTO.HasError) Then Exit For

                                    'Get the ID generated automatically by the DB
                                    myHistTestSamplesToAddDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                                    If (myHistTestSamplesToAddDS.tqcHistoryTestSamples.Count > 0) Then
                                        myQCTestSampleID = myHistTestSamplesToAddDS.tqcHistoryTestSamples.First.QCTestSampleID
                                        myCalcMode = myHistTestSamplesToAddDS.tqcHistoryTestSamples.First.CalculationMode
                                        isNEW = True
                                    Else
                                        'This case should not be possible...
                                        Exit For
                                    End If
                                Else
                                    'This case should not be possible...
                                    Exit For
                                End If
                            End If

                            'Get all results in the entry DataSet for the TestType/TestID/SampleType and update value of fields  
                            'QCTestSampleID and CalculationMode for each one of them
                            lstResultsByTestSample = (From b As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults _
                                                     Where b.TestType = mydiffElemParts(0) _
                                                   AndAlso b.TestID = Convert.ToInt32(mydiffElemParts(1)) _
                                                   AndAlso b.SampleType = mydiffElemParts(2) _
                                                    Select b).ToList

                            For Each result As QCResultsDS.tqcResultsRow In lstResultsByTestSample
                                result.BeginEdit()
                                result.QCTestSampleID = myQCTestSampleID
                                result.CalculationMode = myCalcMode
                                result.EndEdit()
                            Next result

                            If (isNEW) Then
                                'INSERT MULTIRULES: If the QCTestSampleID was created then search the Multirules that are active for the  
                                'TestType/TestID/SampleType and insert them in table tqcHistoryTestSamplesRules in QC Module
                                myGlobalDataTO = myHistTestSamplesRulesDelegate.InsertFromTestSampleMultiRulesNEW(dbConnection, mydiffElemParts(0), Convert.ToInt32(mydiffElemParts(1)), _
                                                                                                                  mydiffElemParts(2), myQCTestSampleID)
                                If (myGlobalDataTO.HasError) Then Exit For
                            End If
                        Next row

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveTestSampleTypeToQC", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "METHODS FOR EXPORT RESULTS TO HISTORIC MODULE"

        ''' <summary>
        ''' Create relations between each Calibrator result executed in the WorkSession and the Blank used to calculate it, and between
        ''' each Patient result (STD Tests) executed in the WorkSession and the Blank and Calibrator used to calculate it
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pHistOrderTestsDS">Typed DataSet HisWSOrderTestsDS with all data of the OrderTests that have been moved
        '''                                 to Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 23/10/2012 
        ''' </remarks>
        Public Function CreateBlkCalibRelations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                        ByVal pWorkSessionID As String, ByVal pHistOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistWSID As String = pHistOrderTestsDS.thisWSOrderTests.First.WorkSessionID

                        Dim myHisWSOrderTestsDelegate As New HisWSOrderTestsDelegate

                        'Get all Blanks executed in the WorkSession and update Blank fields for all Calibrator and Patient Results calculated with them
                        Dim lstBlanks As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow) = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In pHistOrderTestsDS.thisWSOrderTests _
                                                                                          Where a.SampleClass = "BLANK" _
                                                                                         Select a).ToList()

                        For Each blankOTRow As HisWSOrderTestsDS.thisWSOrderTestsRow In lstBlanks
                            myGlobalDataTO = myHisWSOrderTestsDelegate.UpdateBLANKFields(dbConnection, pAnalyzerID, myHistWSID, blankOTRow, False)
                            If (myGlobalDataTO.HasError) Then Exit For
                        Next
                        lstBlanks = Nothing

                        'Get all Calibrators executed in the WorkSession and update Calibrator fields for all Patient Results calculated with them
                        If (Not myGlobalDataTO.HasError) Then
                            Dim lstCalibs As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow) = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In pHistOrderTestsDS.thisWSOrderTests _
                                                                                              Where a.SampleClass = "CALIB" _
                                                                                             Select a).ToList()

                            For Each calibOTRow As HisWSOrderTestsDS.thisWSOrderTestsRow In lstCalibs
                                myGlobalDataTO = myHisWSOrderTestsDelegate.UpdateCALIBFields(dbConnection, pAnalyzerID, myHistWSID, calibOTRow)
                                If (myGlobalDataTO.HasError) Then Exit For
                            Next
                            lstCalibs = Nothing
                        End If

                        'Get all Blank and Calibrators not executed in the WorkSession due to values from a previous WorkSession were used
                        Dim myWSOTsDelegate As New WSOrderTestsDelegate
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myWSOTsDelegate.GetPreviousBlkCalibUsed(dbConnection, pAnalyzerID, pWorkSessionID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myHistWSOrderTestsDS As HisWSOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSOrderTestsDS)

                                Dim myHistWSOrderTestsDelegate As New HisWSOrderTestsDelegate
                                For Each previousOT As HisWSOrderTestsDS.thisWSOrderTestsRow In myHistWSOrderTestsDS.thisWSOrderTests
                                    If (previousOT.SampleClass = "BLANK") Then
                                        'Update Blank fields for all Calibrator and Patient Results calculated with the reused Blank
                                        myGlobalDataTO = myHisWSOrderTestsDelegate.UpdateBLANKFields(dbConnection, pAnalyzerID, myHistWSID, previousOT, True)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                    ElseIf (previousOT.SampleClass = "CALIB") Then
                                        'Update Calibrator fields for all Patient Results calculated with the reused Calibrator
                                        myGlobalDataTO = myHisWSOrderTestsDelegate.UpdateCALIBFields(dbConnection, pAnalyzerID, myHistWSID, previousOT)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    End If
                                Next

                                'Get all Test/SampleTypes requested in the WorkSession but using the Calibrator of an alternative SampleType
                                If (Not myGlobalDataTO.HasError) Then
                                    myGlobalDataTO = myWSOTsDelegate.GetCalibratorsWithAlternative(dbConnection, pAnalyzerID, pWorkSessionID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim myAlternativeOTsDS As TestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                                        'Verify if a Blank for the Test and a Calibrator for the Test/Alternative SampleType was executed in the WorkSession 
                                        Dim lstAlternativeBlanks As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow)
                                        Dim lstAlternativeCalibs As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow)

                                        For Each alternativeOTRow As TestSamplesDS.tparTestSamplesRow In myAlternativeOTsDS.tparTestSamples
                                            'Search a Blank executed in the WorkSession
                                            lstAlternativeBlanks = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In pHistOrderTestsDS.thisWSOrderTests _
                                                                   Where a.SampleClass = "BLANK" _
                                                                 AndAlso a.TestID = alternativeOTRow.TestID _
                                                                  Select a).ToList()

                                            If (lstAlternativeBlanks.Count = 0) Then
                                                'If the Blank for the Test was not executed in the WorkSession, search it in the DS containing the used Blank 
                                                'result from previous WorkSessions
                                                lstAlternativeBlanks = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In myHistWSOrderTestsDS.thisWSOrderTests _
                                                                       Where a.SampleClass = "BLANK" _
                                                                     AndAlso a.TestID = alternativeOTRow.TestID _
                                                                      Select a).ToList()
                                            End If

                                            If (lstAlternativeBlanks.Count = 1) Then
                                                'Update Blank fields for all Patient Results for the Test
                                                lstAlternativeBlanks.First.BeginEdit()
                                                lstAlternativeBlanks.First.HistTestID = alternativeOTRow.HistTestID
                                                lstAlternativeBlanks.First.TestVersionNumber = alternativeOTRow.TestVersionNumber
                                                lstAlternativeBlanks.First.EndEdit()

                                                myGlobalDataTO = myHisWSOrderTestsDelegate.UpdateBLANKFields(dbConnection, pAnalyzerID, myHistWSID, lstAlternativeBlanks.First(), False)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            End If

                                            'Search a Calibrator executed in the WorkSession
                                            lstAlternativeCalibs = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In pHistOrderTestsDS.thisWSOrderTests _
                                                                   Where a.SampleClass = "CALIB" _
                                                                 AndAlso a.TestID = alternativeOTRow.TestID _
                                                                 AndAlso a.SampleType = alternativeOTRow.SampleTypeAlternative _
                                                                  Select a).ToList()

                                            If (lstAlternativeCalibs.Count = 0) Then
                                                'If the Calibrator for the Test/Alternative SampleType was not execute in the WorkSession, search it in the DS containing 
                                                'the used Blank and Calibrator results from previous WorkSessions
                                                lstAlternativeCalibs = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In myHistWSOrderTestsDS.thisWSOrderTests _
                                                                       Where a.SampleClass = "CALIB" _
                                                                     AndAlso a.TestID = alternativeOTRow.TestID _
                                                                     AndAlso a.SampleType = alternativeOTRow.SampleTypeAlternative _
                                                                      Select a).ToList()
                                            End If

                                            If (lstAlternativeCalibs.Count = 1) Then
                                                'Update Calibrator fields for all Patient Results for the Test/SampleType
                                                lstAlternativeCalibs.First.BeginEdit()
                                                lstAlternativeCalibs.First.HistTestID = alternativeOTRow.HistTestID
                                                lstAlternativeCalibs.First.SampleType = alternativeOTRow.SampleType
                                                lstAlternativeCalibs.First.TestVersionNumber = alternativeOTRow.TestVersionNumber
                                                lstAlternativeCalibs.First.EndEdit()

                                                myGlobalDataTO = myHisWSOrderTestsDelegate.UpdateCALIBFields(dbConnection, pAnalyzerID, myHistWSID, lstAlternativeCalibs.First())
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            End If
                                        Next
                                        lstAlternativeBlanks = Nothing
                                        lstAlternativeCalibs = Nothing
                                    End If
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
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.CreateBlkCalibRelations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For all exported results of Calculated Tests, search the exported results of the Standard and/or Calculated Tests used to get the calculated result 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Identifier of the Work Session being exported</param>
        ''' <param name="pHistOrderTestsDS">Typed DataSet HisWSOrderTestsDS with all data of the OrderTests that have been moved to Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/09/2012 
        ''' Modified by: SA 04/07/2013 - When preparing the DS to save all Calculated Tests relations, field WorkSessionID should have the value of field WorkSessionID 
        '''                              for the Historic Order Test for the Calculated Test (historyOT.WorkSessionID, which is the ID created for the Work Session in 
        '''                              Historic Module) instead of the value of parameter pWorkSessionID (which is the ID of the current WS, the one being Reset) 
        ''' </remarks>
        Public Function CreateWSCalcTestsRelations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                   ByVal pWorkSessionID As String, ByVal pHistOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all results of requested Calculated Tests that have been moved to Historic Module
                        Dim lstHistOrderTestsList As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow) = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In pHistOrderTestsDS.thisWSOrderTests _
                                                                                                      Where a.SampleClass = "PATIENT" _
                                                                                                    AndAlso a.TestType = "CALC" _
                                                                                                     Select a).ToList()

                        If (lstHistOrderTestsList.Count > 0) Then
                            'Get Standard and/or Calculated Tests used to get results of all Calculated Tests requested in the WorkSession
                            Dim myCalcOTsDelegate As New OrderCalculatedTestsDelegate
                            myGlobalDataTO = myCalcOTsDelegate.ReadByWorkSession(dbConnection, pWorkSessionID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myCalcOTsDs As OrderCalculatedTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderCalculatedTestsDS)

                                Dim lstHistAuxiliaryList As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow)
                                Dim myCalculatedOTList As List(Of OrderCalculatedTestsDS.twksOrderCalculatedTestsRow)

                                Dim myCalcTestsRelationsDS As New HisWSCalcTestRelations
                                Dim myCalcTestsRelationsRow As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow

                                'For each calculated result exported, search it by CalcOrderTestID = OrderTestID in the returned OrderCalculatedTestsDS
                                For Each historyOT As HisWSOrderTestsDS.thisWSOrderTestsRow In lstHistOrderTestsList
                                    myCalculatedOTList = (From b As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myCalcOTsDs.twksOrderCalculatedTests _
                                                         Where b.CalcOrderTestID = historyOT.OrderTestID _
                                                        Select b).ToList

                                    'For each standard and/or calculated result needed to get the calculated result, get its HistOrderTestID
                                    For Each calcOT As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myCalculatedOTList
                                        lstHistAuxiliaryList = (From c As HisWSOrderTestsDS.thisWSOrderTestsRow In pHistOrderTestsDS.thisWSOrderTests _
                                                               Where c.SampleClass = "PATIENT" _
                                                             AndAlso c.OrderTestID = calcOT.OrderTestID _
                                                              Select c).ToList()

                                        If (lstHistAuxiliaryList.Count = 1) Then
                                            'Add the relation to the DS that will be used to create all Calculated Tests relations in Historic Module
                                            myCalcTestsRelationsRow = myCalcTestsRelationsDS.thisWSCalcTestsRelations.NewthisWSCalcTestsRelationsRow
                                            myCalcTestsRelationsRow.AnalyzerID = pAnalyzerID
                                            myCalcTestsRelationsRow.WorkSessionID = historyOT.WorkSessionID
                                            myCalcTestsRelationsRow.HistOrderTestIDCALC = historyOT.HistOrderTestID
                                            myCalcTestsRelationsRow.HistOrderTestID = lstHistAuxiliaryList.First.HistOrderTestID
                                            myCalcTestsRelationsDS.thisWSCalcTestsRelations.AddthisWSCalcTestsRelationsRow(myCalcTestsRelationsRow)
                                        End If
                                    Next
                                Next

                                If (myCalcTestsRelationsDS.thisWSCalcTestsRelations.Rows.Count > 0) Then
                                    'Create all Calculated Tests relations in Historic Module
                                    Dim myWSCalcTestsRelationsDelegate As New HisWSCalcTestsRelationsDelegate
                                    myGlobalDataTO = myWSCalcTestsRelationsDelegate.Create(dbConnection, myCalcTestsRelationsDS)
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
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.CreateWSCalcTestsRelations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the accepted and validated Results for all Blanks, Calibrators and Patient Samples requested in the specified WS Analyzer 
        ''' and export them to HISTORICS Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/02/2012
        ''' Modified by: SA 19/06/2012 - After getting all Results to move to Historics Module, get all requested Calibrators for 
        '''                              Standard TestIDs/SampleTypes and for each one, verify if the TestID/SampleType uses and 
        '''                              Experimental Calibrator and in this case, inform the CalibratorID in the DS
        '''              TR 26/06/2012 - When an Experimental Calibrator is used for a TestID/SampleType, besides the CalibratorID, inform
        '''                              also fields Name, Lot Number and Number of Calibration Points
        '''              SA 27/08/2012 - After calling function GetTestCalibratorData, verify if CalibratorType=EXPERIMENTAL instead of the
        '''                              number of returned records 
        '''              JB 19/09/2012 - Change function name (from ExportValidatedResults to ExportValidatedResultsAndAlarms)
        '''              TR 26/09/2012 - Before export Alarms, validate there are not errors in the previous process of export accepted and 
        '''                              validated results 
        '''              SA 02/10/2012 - Removed the getting of requested Calibrators for Standard TestIDs/SampleTypes
        '''              SA 10/10/2012 - Get from function MoveResultsToHISTModule the ID of the WorkSession created in Historic Module
        '''                              and link the Alarms to export to it (instead of to pWorkSessionID)
        '''              SA 10/02/2014 - BT #1496 ==> Inform optional parameter pWorkSessionID when calling function GetAlarmsMonitor in WSAnalyzerAlarmsDelegate
        ''' </remarks>
        Public Function ExportValidatedResultsAndAlarms(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                        ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        Dim StartTime As DateTime = Now
                        Dim myLogAcciones As New ApplicationLogManager()
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                        'Get Results accepted and validated in the informed WorkSession for all requested Blanks, Calibrators and Patient Samples
                        Dim myResultsDAO As New twksResultsDAO
                        myGlobalDataTO = myResultsDAO.ReadWSAcceptedResults(dbConnection, pWorkSessionID, pAnalyzerID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myResultsDS As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                            Dim myHistWSID As String = String.Empty
                            If (myResultsDS.vwksResults.Rows.Count > 0) Then
                                'Export all results to HISTORICS Module
                                myGlobalDataTO = MoveResultsToHISTModule(dbConnection, pAnalyzerID, pWorkSessionID, myResultsDS)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myHistWSID = CType(myGlobalDataTO.SetDatos, String)
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'JB 25/09/2012 - BEGIN - Export Alarms to HISTORICS Module 
                                Dim myWSAnalyzerAlarms As New WSAnalyzerAlarmsDelegate
                                myGlobalDataTO = myWSAnalyzerAlarms.GetAlarmsMonitor(dbConnection, pAnalyzerID, pWorkSessionID)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myWSAnalyzerAlamrsDS As WSAnalyzerAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzerAlarmsDS)

                                    If (myWSAnalyzerAlamrsDS.vwksAlarmsMonitor.Rows.Count > 0) Then
                                        If (myResultsDS.vwksResults.Rows.Count > 0) Then
                                            'There are alarms in the WS: move them to Historic Module - Use the WorkSessionID created in Historic Module
                                            myGlobalDataTO = MoveAlarmsToHISTModule(dbConnection, pAnalyzerID, myHistWSID, myWSAnalyzerAlamrsDS)
                                        Else
                                            'Get the Identifier of the Generic Analyzer 
                                            Dim myAnalyzerDelegate As New AnalyzersDelegate
                                            myGlobalDataTO = myAnalyzerDelegate.GetAnalyzerGeneric(dbConnection)

                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                Dim myAnalyzerDS As AnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)

                                                If (myAnalyzerDS.tcfgAnalyzers.Rows.Count > 0) Then
                                                    If (pAnalyzerID <> myAnalyzerDS.tcfgAnalyzers(0).AnalyzerID) Then
                                                        'The Analyzer is not the Generic Analyzer: move Alarms to Historic Module
                                                        myGlobalDataTO = MoveAlarmsToHISTModule(dbConnection, pAnalyzerID, "", myWSAnalyzerAlamrsDS)
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                                'JB 25/09/2012 - END
                            End If
                        End If

                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        GlobalBase.CreateLogActivity("Export Blank, Calibrator and Patient Results to Historic Module " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                        "ResultsDelegate.ExportValidatedResultsAndAlarms", EventLogEntryType.Information, False)
                        StartTime = Now
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.ExportValidatedResultsAndAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Move a group of Alarms to Historics Module before reset the active WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWSAnalyzerAlarmsDS">Typed DataSet WSAnalyzerAlarmsDS containing the group of alarms to export</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  JB 21/09/2012 
        ''' Modified by: TR 26/09/2012 - Validate not NULL value before inform fields AdditionalInfo, AlarmStatus and OKDateTime
        '''              JV 27/01/2014 - #1463
        ''' </remarks>
        Public Function MoveAlarmsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pWorkSessionID As String, ByVal pWSAnalyzerAlarmsDS As WSAnalyzerAlarmsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSAnalyzerAlarmsDS As New HisWSAnalyzerAlarmsDS

                        For Each alarmRow As WSAnalyzerAlarmsDS.vwksAlarmsMonitorRow In pWSAnalyzerAlarmsDS.vwksAlarmsMonitor
                            Dim myHisWSAnalyzerAlarmsRow As HisWSAnalyzerAlarmsDS.thisWSAnalyzerAlarmsRow
                            myHisWSAnalyzerAlarmsRow = myHisWSAnalyzerAlarmsDS.thisWSAnalyzerAlarms.NewthisWSAnalyzerAlarmsRow()
                            With myHisWSAnalyzerAlarmsRow
                                .AnalyzerID = pAnalyzerID
                                .AlarmID = alarmRow.AlarmID
                                .AlarmDateTime = alarmRow.AlarmDateTime
                                .AlarmItem = alarmRow.AlarmItem
                                .AlarmType = alarmRow.AlarmType
                                .WorkSessionID = pWorkSessionID

                                'JV 27/01/2014 - #1463
                                If (alarmRow.AlarmID = GlobalEnumerates.Alarms.WS_PAUSE_MODE_WARN.ToString() AndAlso (Not alarmRow.IsAlarmPeriodSECNull)) Then
                                    If alarmRow.AlarmPeriodSEC > 0 Then
                                        Dim mySeconds As Integer = alarmRow.AlarmPeriodSEC
                                        Dim myMinutes As Integer = 0
                                        Dim myHours As Integer = 0

                                        'Verify if Pause Lapse can be expressed in HOURS
                                        If (mySeconds >= 3600) Then
                                            'Get the number of Hours and also the remaining seconds... 
                                            myHours = Math.DivRem(alarmRow.AlarmPeriodSEC, 3600, mySeconds)
                                        End If

                                        'Verify if Pause Lapse can be expressed in MINUTES
                                        If (mySeconds >= 60) Then
                                            'Get the number of minutes and also the remaining seconds...
                                            Dim mySecondsToCalc As Integer = mySeconds
                                            myMinutes = Math.DivRem(mySecondsToCalc, 60, mySeconds)
                                        End If

                                        'Finally, format the Pause lapse as [Hours:Minutes:Seconds] and concatenate it to the Alarm Description field
                                        .AdditionalInfo = String.Format("[{0}:{1}:{2}]", myHours.ToString("#00"), myMinutes.ToString("00"), mySeconds.ToString("00"))
                                    End If
                                ElseIf (Not alarmRow.IsAdditionalInfoNull) Then
                                    .AdditionalInfo = alarmRow.AdditionalInfo
                                End If
                                'If (Not alarmRow.IsAdditionalInfoNull) Then .AdditionalInfo = alarmRow.AdditionalInfo
                                'JV 27/01/2014 - END #1463
                                If (Not alarmRow.IsAlarmStatusNull) Then .AlarmStatus = alarmRow.AlarmStatus
                                If (Not alarmRow.IsOKDateTimeNull) Then .OKDateTime = alarmRow.OKDateTime
                            End With
                            myHisWSAnalyzerAlarmsDS.thisWSAnalyzerAlarms.AddthisWSAnalyzerAlarmsRow(myHisWSAnalyzerAlarmsRow)
                        Next

                        Dim myHisWSAnalyzerAlarmsDelegate As New HisWSAnalyzerAlarmsDelegate
                        myGlobalDataTO = myHisWSAnalyzerAlarmsDelegate.Create(dbConnection, myHisWSAnalyzerAlarmsDS)

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveAlarmsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of Results to export to Historics Module, get all different Calculated Tests, verify which of them have been already exported
        ''' and add the rest to Calculated Tests table in Historics
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <returns>GlobalDataTO containing a typed DS HisCalculatedTestsDS with all data saved for all different Calculated Tests having Results to export</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' </remarks>
        Private Function MoveCalcTestsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data of all different Calculated Tests saved in DB having Results in the WS
                        Dim lstCalcTestsList As List(Of Integer) = (From a In pResultsDS.vwksResults Where a.TestType = "CALC" _
                                                                  Select a.TestID Distinct).ToList()

                        'Move all Calculated Tests to a HisCalculatedTestsDS
                        Dim myHistCalcTestsDS As New HisCalculatedTestsDS
                        Dim myHistCalcTestsRow As HisCalculatedTestsDS.thisCalculatedTestsRow = Nothing
                        For Each calcTestID As Integer In lstCalcTestsList
                            myHistCalcTestsRow = myHistCalcTestsDS.thisCalculatedTests.NewthisCalculatedTestsRow
                            myHistCalcTestsRow.CalcTestID = calcTestID
                            myHistCalcTestsDS.thisCalculatedTests.AddthisCalculatedTestsRow(myHistCalcTestsRow)
                        Next
                        lstCalcTestsList = Nothing

                        Dim myHistCalcTestsDelegate As New HisCalculatedTestsDelegate
                        myGlobalDataTO = myHistCalcTestsDelegate.CheckCalculatedTestsInHistorics(dbConnection, myHistCalcTestsDS)

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveCalcTestsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of Results to export to Historics Module, get all different ISE Test/SampleTypes, verify which of them have been already exported
        ''' and add the rest to ISE Test Samples table in Historics
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <returns>GlobalDataTO containing a typed DS HisISETestSamplesDS with all data saved for all different ISE Test/Sample Types having Results to export</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' </remarks>
        Private Function MoveISETestsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data of all different ISE Test/SampleTypes saved in DB having Results in the WS
                        Dim lstISETestsList As List(Of String) = (From a In pResultsDS.vwksResults Where a.TestType = "ISE" _
                                                                Select a.TestID.ToString + "|" & a.SampleType Distinct).ToList()

                        'Move all ISE Test/SampleTypes to a HisISETestSamplesDS
                        Dim myHistISETestsDS As New HisISETestSamplesDS
                        Dim myHistISETestsRow As HisISETestSamplesDS.thisISETestSamplesRow = Nothing
                        For Each iseTestSample As String In lstISETestsList
                            myHistISETestsRow = myHistISETestsDS.thisISETestSamples.NewthisISETestSamplesRow()
                            myHistISETestsRow.ISETestID = Convert.ToInt32(iseTestSample.Split(CChar("|"))(0))
                            myHistISETestsRow.SampleType = iseTestSample.Split(CChar("|"))(1).ToString
                            myHistISETestsDS.thisISETestSamples.AddthisISETestSamplesRow(myHistISETestsRow)
                        Next
                        lstISETestsList = Nothing

                        Dim myHistISETestsDelegate As New HisISETestSamplesDelegate
                        myGlobalDataTO = myHistISETestsDelegate.CheckISETestSamplesInHistorics(dbConnection, myHistISETestsDS)

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveISETestsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of Results to export to Historics Module, get all different OFF-SYSTEM Test/SampleTypes, verify which of them have been already exported
        ''' and add the rest to OFF-SYSTEMS Test Samples table in Historics
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <returns>GlobalDataTO containing a typed DS HisOffSystemTestSamplesDS with all data saved for all different OFF-SYSTEM Test/Sample Types 
        '''          having Results to export</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' </remarks>
        Private Function MoveOFFSTestsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data of all different OFF-SYSTEM Test/SampleTypes saved in DB having Results in the WS
                        Dim lstOFFSTestsList As List(Of String) = (From a In pResultsDS.vwksResults Where a.TestType = "OFFS" _
                                                                 Select a.TestID.ToString + "|" & a.SampleType Distinct).ToList()

                        'Move all OFF-SYSTEM Test/SampleTypes to a HisOffSystemTestSamplesDS
                        Dim myHistOFFSTestsDS As New HisOFFSTestSamplesDS
                        Dim myHistOFFSTestsRow As HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow = Nothing
                        For Each offTestSample As String In lstOFFSTestsList
                            myHistOFFSTestsRow = myHistOFFSTestsDS.thisOffSystemTestSamples.NewthisOffSystemTestSamplesRow
                            myHistOFFSTestsRow.OffSystemTestID = Convert.ToInt32(offTestSample.Split(CChar("|"))(0))
                            myHistOFFSTestsRow.SampleType = offTestSample.Split(CChar("|"))(1).ToString
                            myHistOFFSTestsDS.thisOffSystemTestSamples.AddthisOffSystemTestSamplesRow(myHistOFFSTestsRow)
                        Next
                        lstOFFSTestsList = Nothing

                        Dim myHistOFFSTestsDelegate As New HisOFFSTestSamplesDelegate
                        myGlobalDataTO = myHistOFFSTestsDelegate.CheckOFFSTestSamplesInHistorics(dbConnection, myHistOFFSTestsDS)

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveOFFSTestsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of Results to export to Historics Module, get all different Patients, verify which of them have been already exported
        ''' and add the rest to Patients table in Historics
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <returns>GlobalDataTO containing a typed DS HisPatientDS with all data saved for all different Patients having Results to export</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' </remarks>
        Private Function MovePatientsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data of all different Patients saved in DB having Results in the WS
                        Dim lstPatientsList As List(Of String) = (From a In pResultsDS.vwksResults Select a.PatientID Distinct).ToList()

                        'Move all Patients to a HisPatientDS
                        Dim myHistPatientsDS As New HisPatientDS
                        Dim myHistPatientsRow As HisPatientDS.thisPatientsRow = myHistPatientsDS.thisPatients.NewthisPatientsRow()
                        For Each patientID As String In lstPatientsList
                            myHistPatientsRow = myHistPatientsDS.thisPatients.NewthisPatientsRow()
                            If (patientID <> String.Empty) Then
                                myHistPatientsRow.PatientID = patientID
                                myHistPatientsDS.thisPatients.AddthisPatientsRow(myHistPatientsRow)
                            End If
                        Next
                        lstPatientsList = Nothing

                        If (myHistPatientsDS.thisPatients.Rows.Count > 0) Then
                            Dim myHistPatientsDelegate As New HisPatientsDelegate
                            myGlobalDataTO = myHistPatientsDelegate.CheckPatientsInHistorics(dbConnection, myHistPatientsDS)
                        Else
                            'Return an empty DS
                            myGlobalDataTO.SetDatos = myHistPatientsDS
                        End If

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MovePatientsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of Results to export to Historics Module, get all different Reagents, verify which of them have been already exported
        ''' and add the rest to Reagents table in Historics
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <returns>GlobalDataTO containing a typed DS HisReagentsDS with all data saved for all different Reagent with Results to export</returns>
        ''' <remarks>
        ''' Created by:  TR 29/02/2012
        ''' Modified by: SA 31/10/2012 - When there are not Reagents to move to Historic Module, return an empty HisReagentsDS
        ''' </remarks>
        Private Function MoveReagentsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data of all different Standard Tests saved in DB having Results in the WS
                        Dim lstReagentsList As List(Of Integer) = (From a In pResultsDS.vwksResults Where a.TestType = "STD" _
                                                                 Select a.TestID Distinct).ToList()

                        Dim myTestReagentsDS As New TestReagentsDS
                        Dim myTestReagentsDelegate As New TestReagentsDelegate

                        Dim reagentsToAddDS As New HisReagentsDS
                        Dim myReagentToAddRow As HisReagentsDS.thisReagentsRow

                        'Get data of all Reagents needed for the Test
                        For Each myTestID As Integer In lstReagentsList
                            myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(dbConnection, myTestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                                For Each myTestReagentRow As TestReagentsDS.tparTestReagentsRow In myTestReagentsDS.tparTestReagents.Rows
                                    myReagentToAddRow = reagentsToAddDS.thisReagents.NewthisReagentsRow
                                    myReagentToAddRow.ReagentID = myTestReagentRow.ReagentID
                                    myReagentToAddRow.ReagentName = myTestReagentRow.ReagentName
                                    myReagentToAddRow.PreloadedReagent = myTestReagentRow.PreloadedReagent
                                    myReagentToAddRow.ClosedReagent = False

                                    'Add Reagent to DataSet
                                    reagentsToAddDS.thisReagents.AddthisReagentsRow(myReagentToAddRow)
                                Next
                            Else
                                'Error getting Reagents used for the Test
                                Exit For
                            End If
                        Next
                        lstReagentsList = Nothing

                        If (Not myGlobalDataTO.HasError AndAlso reagentsToAddDS.thisReagents.Count > 0) Then
                            Dim myHisReagentsDelegate As New HisReagentsDelegate
                            myGlobalDataTO = myHisReagentsDelegate.CheckReagentsInHistorics(dbConnection, reagentsToAddDS)
                        Else
                            'Return an empty DS
                            myGlobalDataTO.SetDatos = reagentsToAddDS
                        End If

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveReagentsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Move a group of accepted and validated Results for Blanks, Calibrators and Patient Samples to Historics Module before reset the
        ''' active WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <returns>GlobalDataTO containing an string value with the Work Session Identifier in Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SA 21/02/2012 
        ''' Modified by: SA 01/07/2013 - Call to function MoveWSExecutionsToHISTModule in ResultsDelegate has been commented due to
        '''                              from version 2.1.0 saving of Readings, Executions and Adjust Base Lines are disabled 
        ''' </remarks>
        Public Function MoveResultsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                ByVal pWorkSessionID As String, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Export basic data of the Analyzer WS to Historics Module
                        Dim myWorkSessionDS As New WorkSessionsDS

                        myGlobalDataTO = MoveWSToHISTModule(dbConnection, pWorkSessionID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myWorkSessionDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                        End If

                        'Get data of all Patients having Results to export
                        Dim allPatientsDS As New HisPatientDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MovePatientsToHISTModule(dbConnection, pResultsDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                allPatientsDS = DirectCast(myGlobalDataTO.SetDatos, HisPatientDS)
                            End If
                        End If

                        'Get data of all Calculated Tests having Results to export
                        Dim allCalcTestsDS As New HisCalculatedTestsDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveCalcTestsToHISTModule(dbConnection, pResultsDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                allCalcTestsDS = DirectCast(myGlobalDataTO.SetDatos, HisCalculatedTestsDS)
                            End If
                        End If

                        'Get data of all ISE Tests having Results to export
                        Dim allISETestsDS As New HisISETestSamplesDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveISETestsToHISTModule(dbConnection, pResultsDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                allISETestsDS = DirectCast(myGlobalDataTO.SetDatos, HisISETestSamplesDS)
                            End If
                        End If

                        'Get data of all OFF SYSTEM Tests having Results to export
                        Dim allOFFSTestsDS As New HisOFFSTestSamplesDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveOFFSTestsToHISTModule(dbConnection, pResultsDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                allOFFSTestsDS = DirectCast(myGlobalDataTO.SetDatos, HisOFFSTestSamplesDS)
                            End If
                        End If

                        'Get data of all Reagents needed for all Standard Tests with Results to export
                        Dim allReagentsDS As New HisReagentsDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveReagentsToHISTModule(dbConnection, pResultsDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                allReagentsDS = DirectCast(myGlobalDataTO.SetDatos, HisReagentsDS)
                            End If
                        End If

                        'Get data of all STANDARD Tests having Results to export
                        Dim allSTDTestsDS As New HisTestSamplesDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveCalibratorsAndSTDTestsToHISTModule(dbConnection, pResultsDS, allReagentsDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                allSTDTestsDS = DirectCast(myGlobalDataTO.SetDatos, HisTestSamplesDS)
                            End If
                        End If

                        'Get data of all Order Tests having Results to export
                        Dim allOrderTestsDS As New HisWSOrderTestsDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveWSOrderTestsToHISTModule(dbConnection, pResultsDS, allSTDTestsDS, allISETestsDS, _
                                                                          allCalcTestsDS, allOFFSTestsDS, allPatientsDS, _
                                                                          myWorkSessionDS.twksWorkSessions.First.HistWorkSessionID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                allOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSOrderTestsDS)
                            End If
                        End If

                        'Move all WS Results 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveWSResultsToHISTModule(dbConnection, pResultsDS, allOrderTestsDS)
                        End If

                        'Move all WS Curve Results
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = MoveWSCurveResultsToHISTModule(dbConnection, pResultsDS)
                        End If

                        ''Move all BaseLines and also all WS Executions and Readings corresponding for the exported Results
                        'If (Not myGlobalDataTO.HasError) Then
                        '    myGlobalDataTO = MoveWSExecutionsToHISTModule(dbConnection, pAnalyzerID, pWorkSessionID, allOrderTestsDS)
                        'End If

                        'For WS Results corresponding to Calculated Tests, link the result with the results used to calculate it
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = CreateWSCalcTestsRelations(dbConnection, pAnalyzerID, pWorkSessionID, allOrderTestsDS)
                        End If

                        'Finally, create relations between each Calibrator result executed in the WorkSession and the Blank used to calculate it, and between
                        'each Patient result (STD Tests) executed in the WorkSession and the Blank and Calibrator used to calculate it
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = CreateBlkCalibRelations(dbConnection, pAnalyzerID, pWorkSessionID, allOrderTestsDS)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Return the identifier of the WorkSessionID in Historic Module
                            myGlobalDataTO.SetDatos = myWorkSessionDS.twksWorkSessions.First.HistWorkSessionID
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveResultsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of Results to export to Historics Module, get all different STANDARD Test/SampleTypes and verify which of them 
        ''' have been already exported and add the rest to STANDARD Test Samples table in Historics. Besides get all different Experimental 
        ''' Calibrators needed for the STANDARD Test/SampleTypes, verify which of them have been already exported and add the rest to the 
        ''' Calibrators table in Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <param name="pHisReagentsDS">Typed Dataset HisReagentsDS containing data of all needed Reagents for the group of Standard Test/SampleType</param>
        ''' <returns>GlobalDataTO containing a typed DS HisTestSamplesDS with all data saved for all different STANDARD Test/Sample Types 
        '''          having Results to export</returns>
        ''' <remarks>
        ''' Created by:  SA 29/02/2012 
        ''' Modified by: SA 28/09/2012 - Get the list of relations between WaveLength and LedPosition for all filters used in the Analyzer, and inform the 
        '''                              obtained DS when calling function CheckSTDTestSamplesInHistorics in HisTestSamplesDelegate
        '''              SA 02/10/2012 - Name changed from MoveSTDTestsToHISTModule to MoveCalibratorsAndSTDTestsToHISTModule. Implementation changed to
        '''                              include moving of Experimental Calibrators to Historic Module in this function
        ''' </remarks>
        Private Function MoveCalibratorsAndSTDTestsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, _
                                                                ByVal pHisReagentsDS As HisReagentsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the list of WaveLengths available for the Analyzer
                        Dim myAnalyzerLedPosDS As New AnalyzerLedPositionsDS
                        Dim myAnalyzerLedPosDelegate As New AnalyzerLedPositionsDelegate

                        myGlobalDataTO = myAnalyzerLedPosDelegate.GetAllWaveLengths(dbConnection, pResultsDS.vwksResults.First.AnalyzerID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myAnalyzerLedPosDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzerLedPositionsDS)

                            'Get different STANDARD Test/SampleTypes saved in DB having Results in the WS
                            Dim lstSTDTestsList As List(Of String) = (From a In pResultsDS.vwksResults Where a.TestType = "STD" _
                                                                    Select a.TestID.ToString + "|" & a.SampleType Distinct).ToList()

                            'Move all STANDARD Test/SampleTypes to a HisTestSamplesDS
                            Dim myTestCalibratorDS As TestSampleCalibratorDS
                            Dim myTestCalibratorsDelegate As New TestCalibratorsDelegate

                            Dim myCalibID As Integer = 0
                            Dim myHisCalibratorsDS As New HisCalibratorsDS
                            Dim myHisCalibratorRow As HisCalibratorsDS.thisCalibratorsRow

                            Dim myHistTestsDS As New HisTestSamplesDS
                            Dim myHistTestsRow As HisTestSamplesDS.thisTestSamplesRow = Nothing

                            For Each stdTestSample As String In lstSTDTestsList
                                myHistTestsRow = myHistTestsDS.thisTestSamples.NewthisTestSamplesRow
                                myHistTestsRow.TestID = Convert.ToInt32(stdTestSample.Split(CChar("|"))(0))
                                myHistTestsRow.SampleType = stdTestSample.Split(CChar("|"))(1).ToString

                                'Verify if the TestID/SampleType uses and Experimental Calibrator and in this case, inform data of the Calibrator 
                                'in the DS needed to move all the needed Experimental Calibrator to Historic Module
                                myGlobalDataTO = myTestCalibratorsDelegate.GetTestCalibratorData(dbConnection, myHistTestsRow.TestID, myHistTestsRow.SampleType)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS)

                                    If (myTestCalibratorDS.tparTestCalibrators.Rows.Count > 0) Then
                                        myHistTestsRow.CalibratorType = myTestCalibratorDS.tparTestCalibrators.First.CalibratorType

                                        If (myHistTestsRow.CalibratorType = "FACTOR") Then
                                            myHistTestsRow.CalibratorFactor = myTestCalibratorDS.tparTestCalibrators.First.CalibratorFactor

                                        ElseIf (myHistTestsRow.CalibratorType = "EXPERIMENT") Then
                                            myCalibID = myTestCalibratorDS.tparTestCalibrators.First.CalibratorID

                                            'If the Calibrator has not been already added to the HisCalibratorsDS, add it
                                            If (myHisCalibratorsDS.thisCalibrators.ToList.Where(Function(a) a.CalibratorID = myCalibID).Count = 0) Then
                                                myHisCalibratorRow = myHisCalibratorsDS.thisCalibrators.NewthisCalibratorsRow()
                                                myHisCalibratorRow.CalibratorID = myCalibID
                                                myHisCalibratorRow.CalibratorName = myTestCalibratorDS.tparTestCalibrators.First.CalibratorName
                                                myHisCalibratorRow.LotNumber = myTestCalibratorDS.tparTestCalibrators.First.LotNumber
                                                myHisCalibratorRow.NumberOfCalibrators = myTestCalibratorDS.tparTestCalibrators.First.RealNumOfCalibrators
                                                myHisCalibratorsDS.thisCalibrators.AddthisCalibratorsRow(myHisCalibratorRow)
                                            End If

                                            'Additionally, inform Calibrator fields for the Standard Test/SampleType
                                            myHistTestsRow.SetCalibPointUsedNull()
                                            If (myTestCalibratorDS.tparTestCalibrators.First.RealNumOfCalibrators <> myTestCalibratorDS.tparTestCalibrators.First.NumberOfCalibrators) Then
                                                myHistTestsRow.CalibPointUsed = myTestCalibratorDS.tparTestCalibrators.First.RealNumOfCalibrators
                                            End If

                                            myHistTestsRow.SetCurveTypeNull()
                                            If (Not myTestCalibratorDS.tparTestCalibrators.First.IsCurveTypeNull) Then myHistTestsRow.CurveType = myTestCalibratorDS.tparTestCalibrators.First.CurveType

                                            myHistTestsRow.SetCurveGrowthTypeNull()
                                            If (Not myTestCalibratorDS.tparTestCalibrators.First.IsCurveGrowthTypeNull) Then myHistTestsRow.CurveGrowthType = myTestCalibratorDS.tparTestCalibrators.First.CurveGrowthType

                                            myHistTestsRow.SetCurveAxisXTypeNull()
                                            If (Not myTestCalibratorDS.tparTestCalibrators.First.IsCurveAxisXTypeNull) Then myHistTestsRow.CurveAxisXType = myTestCalibratorDS.tparTestCalibrators.First.CurveAxisXType

                                            myHistTestsRow.SetCurveAxisYTypeNull()
                                            If (Not myTestCalibratorDS.tparTestCalibrators.First.IsCurveAxisYTypeNull) Then myHistTestsRow.CurveAxisYType = myTestCalibratorDS.tparTestCalibrators.First.CurveAxisYType

                                            'Provisionally, inform value of CalibratorID in Parameters Programming in the DS field for the ID in Historic Module
                                            myHistTestsRow.HistCalibratorID = myCalibID

                                            'If the Test/SampleType uses the Experimental Calibrator of an alternative SampleType, this value is informed provisionally
                                            'to allow get later the Theoretical Concentration Values 
                                            myHistTestsRow.SetAlternativeSampleTypeNull()
                                            If (myTestCalibratorDS.tparTestCalibrators.Rows.Count = 2) Then
                                                myHistTestsRow.AlternativeSampleType = myTestCalibratorDS.tparTestCalibrators.Last.AlternativeSampleType
                                            End If
                                        End If
                                    End If
                                Else
                                    'Error getting the needed Experimental Calibrator for the Standard Test/SampleType
                                    Exit For
                                End If
                                myHistTestsDS.thisTestSamples.AddthisTestSamplesRow(myHistTestsRow)
                            Next
                            lstSTDTestsList = Nothing

                            'Move all needed Calibrators to Historic Module
                            If (Not myGlobalDataTO.HasError AndAlso myHisCalibratorsDS.thisCalibrators.Rows.Count > 0) Then
                                Dim myHisCalibDelegate As New HisCalibratorsDelegate
                                myGlobalDataTO = myHisCalibDelegate.CheckCalibratorsInHistorics(dbConnection, myHisCalibratorsDS)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myHisCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, HisCalibratorsDS)
                                End If
                            End If

                            'Move all Standard Tests/SampleTypes to Historic Module
                            If (Not myGlobalDataTO.HasError) Then
                                Dim myHistTestsDelegate As New HisTestSamplesDelegate
                                myGlobalDataTO = myHistTestsDelegate.CheckSTDTestSamplesInHistorics(dbConnection, myHistTestsDS, myHisCalibratorsDS, pHisReagentsDS, _
                                                                                                    pResultsDS, myAnalyzerLedPosDS)
                            End If
                        End If

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveCalibratorsAndSTDTestsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' From the group of Results to export to Historic Module, get all Results for Multipoint Calibrators and for each
        ''' one of them, get the Curve Results and move them to Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 22/06/2012
        ''' Modified by: SA 10/10/2012 - Do not move to Historic Module curve result values of NOT CALC Calibrators
        '''              JB 15/10/2012 - Modified the Insert all curve results method
        ''' </remarks>
        Private Function MoveWSCurveResultsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCurveResultsDS As New CurveResultsDS
                        Dim myCurveResultsDelegate As New CurveResultsDelegate

                        Dim myHisWSCurveResultsDS As New HisWSCurveResultsDS
                        Dim myHisWSCurveResultRow As HisWSCurveResultsDS.thisWSCurveResultsRow = Nothing
                        Dim myResultWithCurveResultID As ResultsDS.vwksResultsRow

                        'Get the different CurveResultID in the group of results
                        Dim lstCurveResultIDs As List(Of Integer) = (From a In pResultsDS.vwksResults _
                                                                    Where a.ValidationStatus = "OK" _
                                                                  AndAlso Not a.IsCurveResultsIDNull _
                                                                   Select a.CurveResultsID Distinct).ToList()

                        'JB 15/10/2012 - Modified Create method
                        Dim myHisWSCurveResultsDelegate As New HisWSCurveResultsDelegate
                        For Each curve As Integer In lstCurveResultIDs
                            'Get the first Result with the Curve Result ID in process to get value of fields HistOrderTestID, HistWorkSessionID and AnalyzerID
                            myResultWithCurveResultID = (From a As ResultsDS.vwksResultsRow In pResultsDS.vwksResults _
                                                        Where a.ValidationStatus = "OK" _
                                                  AndAlso Not a.IsCurveResultsIDNull AndAlso a.CurveResultsID = curve _
                                                       Select a).First()
                            With myResultWithCurveResultID
                                myGlobalDataTO = myHisWSCurveResultsDelegate.Create(dbConnection, .AnalyzerID, .CurveResultsID, .HistWorkSessionID, .HistOrderTestID)
                            End With

                            If (myGlobalDataTO.HasError) Then Exit For
                        Next

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveWSOrderTestsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Move to Historic Module all Executions, adjustment Base Lines and Base Lines by Well corresponding
        ''' to all accepted and validated Blank, Calibrator and Patient Results for the specified Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAllOrderTestsDS">Typed DataSet HisWSOrderTestsDS containing all OrderTests of the specified
        '''                                Analyzer WorkSession that have been moved to Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 15/06/2012
        ''' Modified by: SA 30/08/2012 - Implementation changed
        '''              SA 01/10/2012 - Changed the way of get and save values of BaseLine by Well for each Execution
        ''' </remarks>
        Public Function MoveWSExecutionsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     ByVal pWorkSessionID As String, ByVal pAllOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Executions that have to be moved to Historic Module
                        Dim myExecutionsDelegate As New ExecutionsDelegate

                        myGlobalDataTO = myExecutionsDelegate.GetExecutionsForHistoricTable(dbConnection, pWorkSessionID, pAnalyzerID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myHistWSExecutionsDS As HisWSExecutionsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSExecutionsDS)

                            'Select the ID of all different adjustment Base Lines in the group of returned Executions
                            Dim diffAdjustBaseLineList As New List(Of Integer)
                            diffAdjustBaseLineList = (From a As HisWSExecutionsDS.thisWSExecutionsRow In myHistWSExecutionsDS.thisWSExecutions _
                                                 Where Not a.IsAdjustBaseLineIDNull _
                                                    Select a.AdjustBaseLineID Distinct).ToList()

                            Dim adjustBLInHIST As Integer = 0
                            Dim myHistAdjustBaseLineID As Integer = 0
                            Dim myAdjustBLDelegate As New WSBLinesDelegate
                            Dim myHistAdjustBLDelegate As New HisAdjustBaseLinesDelegate
                            Dim myWSExecutionsToUpdate As New List(Of HisWSExecutionsDS.thisWSExecutionsRow)

                            For Each myAdjustBaseLineID As Integer In diffAdjustBaseLineList
                                myHistAdjustBaseLineID = 0

                                'Verify if the Adjustment BaseLine has been already moved to Historic Module 
                                myGlobalDataTO = myAdjustBLDelegate.VerifyBLMovedToHistoric(dbConnection, pAnalyzerID, myAdjustBaseLineID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    adjustBLInHIST = DirectCast(myGlobalDataTO.SetDatos, Integer)

                                    If (adjustBLInHIST = 1) Then
                                        'Get the ID of the last Adjustment BaseLine moved to Historic Module for the specified Analyzer
                                        myGlobalDataTO = myHistAdjustBLDelegate.GetLastHistAdjustBaseLine(dbConnection, pAnalyzerID)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myHistAdjustBaseLineID = DirectCast(myGlobalDataTO.SetDatos, Integer)
                                        Else
                                            'Error getting the last generated ID for an Adjustment BaseLine for the Analyzer in Historic Module
                                            Exit For
                                        End If
                                    End If
                                Else
                                    'Error verifying if the Adjustment BaseLine has been already moved to Historic Module
                                    Exit For
                                End If

                                If (myHistAdjustBaseLineID = 0) Then
                                    'Generate the next ID for an Adjustment BaseLine in Historic Module
                                    myGlobalDataTO = myHistAdjustBLDelegate.GetNextAdjustBaseLineID(dbConnection, pAnalyzerID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myHistAdjustBaseLineID = DirectCast(myGlobalDataTO.SetDatos, Integer)

                                        'Insert the Adjustment BL with all its Wave Lengths in Historic Module 
                                        myGlobalDataTO = myHistAdjustBLDelegate.InsertNewBaseLines(dbConnection, pAnalyzerID, myAdjustBaseLineID, _
                                                                                                   myHistAdjustBaseLineID)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'Set flag MovedToHistoric = TRUE to indicate the Adjustment BaseLine (with all its Wave Lengths) has been exported to Historic Module
                                        myGlobalDataTO = myAdjustBLDelegate.UpdateMovedToHistoric(dbConnection, pAnalyzerID, myAdjustBaseLineID)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Else
                                        'Error generating the next HistAdjustBaseLineID
                                        Exit For
                                    End If
                                End If

                                'Get all Executions with the AdjustBaseLineID in process and update value of field HistAdjustBaseLineID
                                'in the HistWSExecutionsDS
                                myWSExecutionsToUpdate = (From a As HisWSExecutionsDS.thisWSExecutionsRow In myHistWSExecutionsDS.thisWSExecutions _
                                                         Where a.AnalyzerID = pAnalyzerID.Trim _
                                                       AndAlso a.WorkSessionID = pWorkSessionID.Trim _
                                                   AndAlso Not a.IsAdjustBaseLineIDNull _
                                                       AndAlso a.AdjustBaseLineID = myAdjustBaseLineID _
                                                        Select a).ToList()

                                For Each wsExecution As HisWSExecutionsDS.thisWSExecutionsRow In myWSExecutionsToUpdate
                                    wsExecution.BeginEdit()
                                    wsExecution.HistAdjustBaseLineID = myHistAdjustBaseLineID
                                    wsExecution.EndEdit()
                                Next
                            Next

                            If (Not myGlobalDataTO.HasError) Then
                                Dim myWSBLinesByWellDS As New BaseLinesDS
                                Dim baseLineWL As List(Of BaseLinesDS.twksWSBaseLinesRow)
                                Dim myWSBLinesByWellDelegate As New WSBLinesByWellDelegate

                                'Get all OrderTests for STD and ISE Tests
                                Dim orderTestsList As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow) = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In pAllOrderTestsDS.thisWSOrderTests _
                                                                                                       Where a.TestType = "STD" OrElse a.TestType = "ISE" _
                                                                                                      Select a).ToList()

                                Dim myExecutionsAlarmsDS As New WSExecutionAlarmsDS
                                Dim myExecutionsAlarmsDelegate As New WSExecutionAlarmsDelegate
                                Dim myHistWSReadingDelegate As New HisWSReadingsDelegate

                                For Each wsOrderTest As HisWSOrderTestsDS.thisWSOrderTestsRow In orderTestsList
                                    'Get the list of Executions for the OrderTest in process to update value of field HistOrderTestID and also 
                                    'the BaseLine by Well fields in the DS
                                    myWSExecutionsToUpdate = (From a As HisWSExecutionsDS.thisWSExecutionsRow In myHistWSExecutionsDS.thisWSExecutions _
                                                             Where a.AnalyzerID = pAnalyzerID.Trim _
                                                           AndAlso a.WorkSessionID = pWorkSessionID.Trim _
                                                           AndAlso a.OrderTestID = wsOrderTest.OrderTestID _
                                                            Select a).ToList()

                                    For Each wsExecution As HisWSExecutionsDS.thisWSExecutionsRow In myWSExecutionsToUpdate
                                        wsExecution.BeginEdit()
                                        wsExecution.HistOrderTestID = wsOrderTest.HistOrderTestID
                                        wsExecution.WorkSessionID = wsOrderTest.WorkSessionID

                                        If (wsOrderTest.TestType = "STD") Then
                                            'Get values for all WaveLengths for the BaseLineID/WellUsed
                                            myGlobalDataTO = myWSBLinesByWellDelegate.Read(dbConnection, pAnalyzerID, pWorkSessionID, wsExecution.BaseLineID, _
                                                                                           wsExecution.WellUsed)

                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                myWSBLinesByWellDS = DirectCast(myGlobalDataTO.SetDatos, BaseLinesDS)

                                                If (myWSBLinesByWellDS.twksWSBaseLines.Rows.Count > 0) Then
                                                    'Get WaveLength, MainLight and RefLight for the Main WaveLength
                                                    baseLineWL = (From a As BaseLinesDS.twksWSBaseLinesRow In myWSBLinesByWellDS.twksWSBaseLines _
                                                                 Where a.Wavelength = wsOrderTest.MainLedPosition _
                                                                Select a).ToList

                                                    If (baseLineWL.Count = 1) Then
                                                        wsExecution.BLMainLedPos = baseLineWL.First.Wavelength
                                                        wsExecution.BLMainWL = wsOrderTest.MainWaveLength

                                                        wsExecution.BLMainWL_MainLight = baseLineWL.First.MainLight
                                                        wsExecution.BLMainWL_RefLight = baseLineWL.First.RefLight
                                                    End If

                                                    'If the Test is Bichromatic, get WaveLength, MainLight and RefLight for the Reference WaveLength
                                                    If (Not wsOrderTest.IsReferenceLedPositionNull) Then
                                                        baseLineWL = (From a As BaseLinesDS.twksWSBaseLinesRow In myWSBLinesByWellDS.twksWSBaseLines _
                                                                     Where a.Wavelength = wsOrderTest.ReferenceLedPosition _
                                                                    Select a).ToList

                                                        If (baseLineWL.Count = 1) Then
                                                            wsExecution.BLRefLedPos = baseLineWL.First.Wavelength
                                                            wsExecution.BLRefWL = wsOrderTest.ReferenceWaveLength
                                                            wsExecution.BLRefWL_MainLight = baseLineWL.First.MainLight
                                                            wsExecution.BLRefWL_RefLight = baseLineWL.First.RefLight
                                                        End If
                                                    End If
                                                End If
                                            Else
                                                'Error getting the values of the BaseLine by Well 
                                                Exit For
                                            End If
                                        End If

                                        'Get all Alarms for the Execution and build an String list with all Alarm Codes divided by pipe characters
                                        myGlobalDataTO = myExecutionsAlarmsDelegate.Read(dbConnection, wsExecution.ExecutionID)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myExecutionsAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, WSExecutionAlarmsDS)

                                            'Build the string list of Alarm codes
                                            For Each myExecAlarmRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In myExecutionsAlarmsDS.twksWSExecutionAlarms
                                                wsExecution.AlarmList &= myExecAlarmRow.AlarmID & "|"
                                            Next

                                            If (wsExecution.AlarmList.Count > 0) Then
                                                'Remove the last pipe character in the built Alarms list
                                                wsExecution.AlarmList = wsExecution.AlarmList.Remove(wsExecution.AlarmList.Length - 1, 1)
                                            End If
                                        Else
                                            'Error getting the list of Execution Alarms
                                            Exit For
                                        End If
                                        wsExecution.EndEdit()

                                        'Finally, move all Execution Readings to Historic Module
                                        myGlobalDataTO = myHistWSReadingDelegate.InsertNewReadings(dbConnection, pAnalyzerID, pWorkSessionID, wsExecution.ExecutionID, _
                                                                                                   wsExecution.MultiPointNumber, wsExecution.ReplicateNumber, _
                                                                                                   wsExecution.HistOrderTestID, wsExecution.WorkSessionID)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Next
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next
                            End If

                            'Finally, move all the Executions to Historic Module
                            If (Not myGlobalDataTO.HasError) Then
                                Dim myHisWSExecutionsDelegate As New HisWSExecutionsDelegate
                                myGlobalDataTO = myHisWSExecutionsDelegate.Create(dbConnection, myHistWSExecutionsDS)
                            End If
                        End If

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveWSExecutionsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Order Tests having Results to export and move them to Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <param name="pHistSTDTestsDS">Typed DataSet HisSTDTestsDS containing the historic information for all Standard Tests having results to export</param>
        ''' <param name="pHistISETestsDS">Typed DataSet HisISETestsDS containing the historic information for all ISE Tests having results to export</param>
        ''' <param name="pHistCALCTestsDS">Typed DataSet HisCALCTestsDS containing the historic information for all Calculated Tests having results to export</param>
        ''' <param name="pHistOFFTestsDS">Typed DataSet HisOFFTestsDS containing the historic information for all OffSystem Tests having results to export</param>
        ''' <param name="pHistPatientsDS">Typed DataSet HisPatientsDS containing the historic information for all Patients having results to export</param>
        ''' <param name="pHistWorkSessionID">Identifier of the WS to create in Historic Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSOrderTestsDS with the list of Order Tests added to Historic Module</returns>
        ''' <remarks>
        ''' Created by:  TR 19/06/2012
        ''' Modified by: SA 28/09/2012 - For STD Tests, inform also fields MainLedPosition and ReferenceLedPosition in the DS to return 
        '''              SA 02/10/2012 - Removed parameter pHistCalibratorsDS 
        '''              SA 17/10/2012 - For STD Tests, inform also fields MainWaveLength and ReferenceWaveLength in the DS to return 
        '''              SA 18/10/2012 - For STD Tests, inform also fields BlankAbsorbanceLimit, KineticBlankLimit, FactorLowerLimit and 
        '''                              FactorUpperLimit in the DS to return
        '''              SA 19/10/2012 - Removed movement of previous Blank results to the new created TestVersionNumber
        '''              SA 08/11/2012 - For Patient Samples, if the SampleID is an autonumeric one, before save it in Historic Module, 
        '''                              modify the SampleID by adding to it a suffix with the sequence number of the generated WorkSessionID
        '''                              (when in the same day several WS containing autonumeric Patients are executed and exported to Historic
        '''                              Module, this is needed to avoid grouping different Patients as the same in the Historic Results Screen)
        '''              SA 28/11/2012 - For Patient Samples, field HistPatientID has to be set to NULL only when the SampleID is informed. Fixed a
        '''                              codification error in which that field was set to NULL off of the main IF and as a result of it, both fields
        '''                              HistPatientID and SampleID were saved as NULL
        '''              AG 24/04/2013 - Fill new fields to be added in new history records: LISRequest, ExternalQC, SpecimenID, AwosID, ESOrderID, 
        '''                              ESPatientID, LISOrderID, LISPatientID, LISTestName, LISSampleType, LISUnits
        '''              AG 24/07/2014 - BT #1886 (RQ00086 v3.1.0) ==> Historic patient results can be re-sent (all fields required has to be save although 
        '''                                                            the result is already sent)
        '''              SA 26/08/2014 - BA-1861 ==> Added changes to export field SpecimenID (Barcode) to the Historic Module also when the corresponding result has 
        '''                                          been already exported and/or when the Order Test has been manually requested for a Patient Sample having Tests 
        '''                                          requested by LIS.
        '''              SA 12/11/2014 - BA-2095 ==> For Order Tests of Patient Samples that do not have Tests requested by LIS, leave field SpecimenID empty instead
        '''                                          of inform it with the same value than PatientID/SampleID.
        ''' </remarks>
        Private Function MoveWSOrderTestsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, _
                                                      ByVal pHistSTDTestsDS As HisTestSamplesDS, ByVal pHistISETestsDS As HisISETestSamplesDS, _
                                                      ByVal pHistCALCTestsDS As HisCalculatedTestsDS, ByVal pHistOFFTestsDS As HisOFFSTestSamplesDS, _
                                                      ByVal pHistPatientsDS As HisPatientDS, ByVal pHistWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySTDTestList As New List(Of HisTestSamplesDS.thisTestSamplesRow)
                        Dim myCALTestList As New List(Of HisCalculatedTestsDS.thisCalculatedTestsRow)
                        Dim myOFFTestList As New List(Of HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow)
                        Dim myISETestList As New List(Of HisISETestSamplesDS.thisISETestSamplesRow)
                        Dim myCALIBList As New List(Of HisCalibratorsDS.thisCalibratorsRow)
                        Dim myPATIENTList As New List(Of HisPatientDS.thisPatientsRow)

                        'Get the SequenceNumber of the WorkSessionID in Historic Module (last two characters)
                        Dim myWSSequenceNum As String = pHistWorkSessionID.Substring(8)

                        'Get the WorkSessionID from the first record in the entry parameter pResultsDS
                        Dim myWorkSessionID As String = pResultsDS.vwksResults.First.WorkSessionID

                        'Read required data needed to fill new fields needed to upload results to LIS
                        Dim lisInfoDS As New OrderTestsLISInfoDS
                        Dim lisInfoDlg As New OrderTestsLISInfoDelegate

                        myGlobalDataTO = lisInfoDlg.ReadAll(dbConnection)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            lisInfoDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsLISInfoDS)
                        End If

                        Dim testMappDS As New AllTestsByTypeDS
                        Dim testMappDlg As New AllTestByTypeDelegate

                        myGlobalDataTO = testMappDlg.ReadAll(dbConnection)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            testMappDS = DirectCast(myGlobalDataTO.SetDatos, AllTestsByTypeDS)
                        End If

                        Dim confMappDS As New LISMappingsDS
                        Dim confMappDlg As New LISMappingsDelegate

                        myGlobalDataTO = confMappDlg.ReadAll(dbConnection)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            confMappDS = DirectCast(myGlobalDataTO.SetDatos, LISMappingsDS)
                        End If

                        'BA-1861 - Get the list of all required Patient Samples that have been sent by an external LIS system
                        '          (those having field SpecimenIDList informed)
                        Dim myDataSet As New WSRequiredElementsDS
                        Dim myWSReqElemDelegate As New WSRequiredElementsDelegate

                        myGlobalDataTO = myWSReqElemDelegate.GetLISPatientElements(dbConnection, myWorkSessionID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myDataSet = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)
                        End If

                        'Load each Result in a HisWSOrderTestsDS
                        Dim mySampleID As String = String.Empty
                        Dim myHistOrderTestsDS As New HisWSOrderTestsDS
                        Dim myHistOrderTestsRow As HisWSOrderTestsDS.thisWSOrderTestsRow = Nothing
                        Dim lisInfoLinqRes As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                        Dim lstSpecimenID As List(Of WSRequiredElementsDS.twksWSRequiredElementsRow)

                        For Each myResultsRow As ResultsDS.vwksResultsRow In pResultsDS.vwksResults.Rows
                            'For Multipoint Calibrators, process only data of the first point, the rest are ignored
                            If (myResultsRow.MultiPointNumber = 1) Then
                                myHistOrderTestsRow = myHistOrderTestsDS.thisWSOrderTests.NewthisWSOrderTestsRow()
                                myHistOrderTestsRow.AnalyzerID = myResultsRow.AnalyzerID
                                myHistOrderTestsRow.WorkSessionID = pHistWorkSessionID.Trim
                                myHistOrderTestsRow.OrderTestID = myResultsRow.OrderTestID
                                myHistOrderTestsRow.OrderDateTime = myResultsRow.OrderDateTime
                                myHistOrderTestsRow.SampleClass = myResultsRow.SampleClass
                                myHistOrderTestsRow.StatFlag = myResultsRow.StatFlag
                                myHistOrderTestsRow.TestType = myResultsRow.TestType
                                myHistOrderTestsRow.SampleType = myResultsRow.SampleType

                                'Get value of fields that depends on value of field TestType...
                                Select Case (myHistOrderTestsRow.TestType)
                                    Case "STD"
                                        'Set value of fields HistTestID, TestVersionNumber, MeasureUnit, ReplicatesNumber, MainLedPosition, ReferenceLedPosition,
                                        'BlankAbsorbanceLimit, KineticBlankLimit, FactorLowerLimit and FactorUpperLimit
                                        mySTDTestList = (From a As HisTestSamplesDS.thisTestSamplesRow In pHistSTDTestsDS.thisTestSamples _
                                                        Where a.TestID = myResultsRow.TestID _
                                                      AndAlso a.SampleType = myResultsRow.SampleType _
                                                       Select a).ToList()

                                        If (mySTDTestList.Count > 0) Then
                                            myHistOrderTestsRow.HistTestID = mySTDTestList.First.HistTestID
                                            myHistOrderTestsRow.TestVersionNumber = mySTDTestList.First.TestVersionNumber
                                            myHistOrderTestsRow.MeasureUnit = mySTDTestList.First.MeasureUnit
                                            myHistOrderTestsRow.MainLedPosition = mySTDTestList.First.MainLedPosition
                                            myHistOrderTestsRow.MainWaveLength = mySTDTestList.First.MainWavelength

                                            myHistOrderTestsRow.SetReferenceLedPositionNull()
                                            If (Not mySTDTestList.First().IsReferenceLedPositionNull) Then
                                                myHistOrderTestsRow.ReferenceLedPosition = mySTDTestList.First.ReferenceLedPosition
                                                myHistOrderTestsRow.ReferenceWaveLength = mySTDTestList.First.ReferenceWavelength
                                            End If

                                            'Limits defined for the Test/Sample Type that are needed to save the Results
                                            myHistOrderTestsRow.SetBlankAbsorbanceLimitNull()
                                            If (Not mySTDTestList.First.IsBlankAbsorbanceLimitNull) Then myHistOrderTestsRow.BlankAbsorbanceLimit = mySTDTestList.First.BlankAbsorbanceLimit

                                            myHistOrderTestsRow.SetKineticBlankLimitNull()
                                            If (Not mySTDTestList.First.IsKineticBlankLimitNull) Then myHistOrderTestsRow.KineticBlankLimit = mySTDTestList.First.KineticBlankLimit

                                            myHistOrderTestsRow.SetFactorLowerLimitNull()
                                            If (Not mySTDTestList.First.IsFactorLowerLimitNull) Then myHistOrderTestsRow.FactorLowerLimit = mySTDTestList.First.FactorLowerLimit

                                            myHistOrderTestsRow.SetFactorUpperLimitNull()
                                            If (Not mySTDTestList.First.IsFactorUpperLimitNull) Then myHistOrderTestsRow.FactorUpperLimit = mySTDTestList.First.FactorUpperLimit


                                            'If the Test/SampleType uses an Experimental Calibrator, inform the HistCalibratorID
                                            If (myResultsRow.SampleClass = "CALIB") Then
                                                If (Not mySTDTestList.First.IsHistCalibratorIDNull) Then myHistOrderTestsRow.HistCalibratorID = mySTDTestList.First.HistCalibratorID
                                            End If
                                        End If
                                        myHistOrderTestsRow.ReplicatesNumber = myResultsRow.ReplicatesNumber

                                        'For STD Tests, the TestID in Parameters Programming is also added to the DS (function CreatePreviousBlkCalib needs it)
                                        myHistOrderTestsRow.TestID = myResultsRow.TestID
                                        Exit Select

                                    Case "ISE"
                                        'Set value of fields HistISETestID, MeasureUnit and ReplicatesNumber
                                        myISETestList = (From a As HisISETestSamplesDS.thisISETestSamplesRow In pHistISETestsDS.thisISETestSamples _
                                                        Where a.ISETestID = myResultsRow.TestID _
                                                      AndAlso a.SampleType = myResultsRow.SampleType _
                                                       Select a).ToList()

                                        If (myISETestList.Count > 0) Then
                                            myHistOrderTestsRow.HistTestID = myISETestList.First().HistISETestID
                                            myHistOrderTestsRow.MeasureUnit = myISETestList.First().MeasureUnit
                                        End If

                                        myHistOrderTestsRow.ReplicatesNumber = myResultsRow.ReplicatesNumber
                                        Exit Select

                                    Case "CALC"
                                        'Set value of fields HistCalcTestID, MeasureUnit and ReplicatesNumber
                                        myCALTestList = (From a As HisCalculatedTestsDS.thisCalculatedTestsRow In pHistCALCTestsDS.thisCalculatedTests _
                                                        Where a.CalcTestID = myResultsRow.TestID _
                                                       Select a).ToList()

                                        If (myCALTestList.Count > 0) Then
                                            myHistOrderTestsRow.HistTestID = myCALTestList.First().HistCalcTestID
                                            myHistOrderTestsRow.MeasureUnit = myCALTestList.First().MeasureUnit
                                        End If

                                        myHistOrderTestsRow.ReplicatesNumber = 1
                                        Exit Select

                                    Case "OFFS"
                                        'Set value of fields HistOffSystemTestID, MeasureUnit and ReplicatesNumber
                                        myOFFTestList = (From a As HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow In pHistOFFTestsDS.thisOffSystemTestSamples _
                                                        Where a.OffSystemTestID = myResultsRow.TestID _
                                                      AndAlso a.SampleType = myResultsRow.SampleType _
                                                       Select a).ToList()

                                        If (myOFFTestList.Count > 0) Then
                                            myHistOrderTestsRow.HistTestID = myOFFTestList.First().HistOffSystemTestID
                                            myHistOrderTestsRow.MeasureUnit = myOFFTestList.First().MeasureUnit
                                        End If

                                        myHistOrderTestsRow.ReplicatesNumber = 1
                                        Exit Select
                                End Select

                                'For Patient results, if field PatientID is informed, set value of field HistPatientID
                                If (myResultsRow.SampleClass = "PATIENT") Then
                                    If (Not myResultsRow.IsPatientIDNull AndAlso myResultsRow.PatientID <> String.Empty) Then
                                        myPATIENTList = (From a As HisPatientDS.thisPatientsRow In pHistPatientsDS.thisPatients _
                                                        Where a.PatientID = myResultsRow.PatientID _
                                                       Select a).ToList()

                                        If (myPATIENTList.Count > 0) Then
                                            myHistOrderTestsRow.HistPatientID = myPATIENTList.First().HistPatientID
                                            myHistOrderTestsRow.SetSampleIDNull()
                                        Else
                                            myHistOrderTestsRow.SampleID = myResultsRow.PatientID
                                            myHistOrderTestsRow.SetHistPatientIDNull()
                                        End If
                                        mySampleID = myResultsRow.PatientID

                                    ElseIf (myResultsRow.SampleID.Substring(0, 1) = "#") Then
                                        'For autonumeric Patients, add the WS Sequence Number as suffix...
                                        myHistOrderTestsRow.SampleID = myResultsRow.SampleID & "-" & myWSSequenceNum
                                        myHistOrderTestsRow.SetHistPatientIDNull()

                                        mySampleID = myHistOrderTestsRow.SampleID
                                    Else
                                        myHistOrderTestsRow.SampleID = myResultsRow.SampleID
                                        myHistOrderTestsRow.SetHistPatientIDNull()

                                        mySampleID = myResultsRow.SampleID
                                    End If
                                Else
                                    myHistOrderTestsRow.SetSampleIDNull()
                                    myHistOrderTestsRow.SetHistPatientIDNull()
                                End If

                                'BT #1886 (RQ00086 v3.1.0) - Historic Patient Results can be always send to LIS, although they have been already exported. 
                                'If (myResultsRow.SampleClass = "PATIENT") AndAlso ((Not myResultsRow.IsExportStatusNull AndAlso myResultsRow.ExportStatus <> "SENT") OrElse (myResultsRow.IsExportStatusNull)) Then
                                If (myResultsRow.SampleClass = "PATIENT") Then
                                    'Fields obtained from OrderTests
                                    If (Not myResultsRow.IsLISRequestNull) Then myHistOrderTestsRow.LISRequest = myResultsRow.LISRequest Else myHistOrderTestsRow.SetLISRequestNull()
                                    If (Not myResultsRow.IsExternalQCNull) Then myHistOrderTestsRow.ExternalQC = myResultsRow.ExternalQC Else myHistOrderTestsRow.SetExternalQCNull()

                                    'Fields obtained from OrderTestsLISInfo
                                    lisInfoLinqRes = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In lisInfoDS.twksOrderTestsLISInfo _
                                                     Where a.OrderTestID = myResultsRow.OrderTestID _
                                                   AndAlso a.RerunNumber = myResultsRow.RerunNumber _
                                                    Select a).ToList

                                    If (lisInfoLinqRes.Count > 0) Then
                                        If (Not lisInfoLinqRes(0).IsSpecimenIDNull) Then myHistOrderTestsRow.SpecimenID = lisInfoLinqRes(0).SpecimenID Else myHistOrderTestsRow.SetSpecimenIDNull()
                                        If (Not lisInfoLinqRes(0).IsAwosIDNull) Then myHistOrderTestsRow.AwosID = lisInfoLinqRes(0).AwosID Else myHistOrderTestsRow.SetAwosIDNull()
                                        If (Not lisInfoLinqRes(0).IsESOrderIDNull) Then myHistOrderTestsRow.ESOrderID = lisInfoLinqRes(0).ESOrderID Else myHistOrderTestsRow.SetESOrderIDNull()
                                        If (Not lisInfoLinqRes(0).IsESPatientIDNull) Then myHistOrderTestsRow.ESPatientID = lisInfoLinqRes(0).ESPatientID Else myHistOrderTestsRow.SetESPatientIDNull()
                                        If (Not lisInfoLinqRes(0).IsLISOrderIDNull) Then myHistOrderTestsRow.LISOrderID = lisInfoLinqRes(0).LISOrderID Else myHistOrderTestsRow.SetLISOrderIDNull()
                                        If (Not lisInfoLinqRes(0).IsLISPatientIDNull) Then myHistOrderTestsRow.LISPatientID = lisInfoLinqRes(0).LISPatientID Else myHistOrderTestsRow.SetLISPatientIDNull()
                                    Else
                                        'BA-1861 - It is a manual Patient Order Test; get the SpecimenID sent by LIS for the same PatientID and SampleType (if any), 
                                        '          or set SpecimenID = SampleID if there is not LIS information for the PatientID and SampleType.
                                        lstSpecimenID = (From b As WSRequiredElementsDS.twksWSRequiredElementsRow In myDataSet.twksWSRequiredElements _
                                                        Where b.PatientID = mySampleID _
                                                      AndAlso b.SampleType = myHistOrderTestsRow.SampleType _
                                                       Select b).ToList

                                        If (lstSpecimenID.Count > 0) Then
                                            myHistOrderTestsRow.SpecimenID = lstSpecimenID.First.SpecimenIDList.Trim.Split(CChar(vbCrLf))(0)
                                        Else
                                            'BA-2095 ==> For Order Tests of Patient Samples that do not have Tests requested by LIS, leave field 
                                            '            SpecimenID empty instead of inform it with the same value than PatientID or SampleID
                                            myHistOrderTestsRow.SetSpecimenIDNull()
                                        End If
                                    End If

                                    'Fields get from LIS mapping values
                                    myGlobalDataTO = testMappDlg.GetLISTestID(testMappDS, myResultsRow.TestID, myResultsRow.TestType)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myHistOrderTestsRow.LISTestName = DirectCast(myGlobalDataTO.SetDatos, String)
                                    Else
                                        myHistOrderTestsRow.SetLISTestNameNull()
                                    End If

                                    myGlobalDataTO = confMappDlg.GetLISSampleType(confMappDS, myResultsRow.SampleType)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myHistOrderTestsRow.LISSampleType = DirectCast(myGlobalDataTO.SetDatos, String)
                                    Else
                                        myHistOrderTestsRow.SetLISSampleTypeNull()
                                    End If

                                    myGlobalDataTO = confMappDlg.GetLISUnits(confMappDS, myHistOrderTestsRow.MeasureUnit) 'AG 24/04/2013 - Do not use myResultsRow.MeasureUnit because is NULL
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myHistOrderTestsRow.LISUnits = DirectCast(myGlobalDataTO.SetDatos, String)
                                    Else
                                        myHistOrderTestsRow.SetLISUnitsNull()
                                    End If
                                End If
                                myHistOrderTestsDS.thisWSOrderTests.AddthisWSOrderTestsRow(myHistOrderTestsRow)
                            End If
                        Next

                        'Create all Order Tests added to HistWSOrderTestsDS
                        Dim myHistOrderTestsDelegate As New HisWSOrderTestsDelegate
                        myGlobalDataTO = myHistOrderTestsDelegate.Create(dbConnection, myHistOrderTestsDS)

                        'Set all created lists to Nothing to release memory
                        mySTDTestList = Nothing
                        myCALTestList = Nothing
                        myOFFTestList = Nothing
                        myISETestList = Nothing
                        myCALIBList = Nothing
                        myPATIENTList = Nothing
                        lstSpecimenID = Nothing
                        lisInfoLinqRes = Nothing

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveWSOrderTestsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Move all accepted and validated Results in the current WorkSession to Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <param name="pHisWSOrderTestsDS">Typed DataSet HisWSOrderTestsDS containing all Order Tests that have 
        '''                                  been added to Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 21/06/2012
        ''' Modified by: JB 04/10/2012 - Add NotCalcCalib field
        '''              JB 08/10/2012 - Update ClosedResult, HistWSID and HistOTID
        '''              JB 10/10/2012 - Close old Blanks and Calibrators
        '''              SA 17/10/2012 - When informed, copy values of fields RelativeErrorCurve and ExportStatus to the HisWSResultsDS
        '''              SA 18/10/2012 - When informed, copy values of fields BlankAbsorbanceLimit, KineticBlankLimit, FactorLowerLimit and
        '''                              FactorUpperLimit to the HisWSResultsDS
        '''              SA 22/10/2012 - When informed, copy values of fields ABS_Initial, ABS_WorkReagent and ABS_MainFilter to the HisWSResultsDS
        '''              SA 25/10/2012 - Added evaluation of Result out of the Panic Range when it is informed
        '''              AG 24/04/2013 - Added column to hisResults LISMessageID informed only for those current results with ExportStatus = 'SENDING' when the RESET is performed
        '''</remarks>
        Private Function MoveWSResultsToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, _
                                                   ByVal pHisWSOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCONCValue As Single
                        Dim mySampleType As String
                        Dim myResultAlarmsDS As New ResultAlarmsDS
                        Dim myThisWSResultsDS As New HisWSResultsDS
                        Dim myTestRefRangesDS As New TestRefRangesDS

                        Dim myOrderTestDelegate As New OrderTestsDelegate
                        Dim myResultsAlarmsDelegate As New ResultAlarmsDelegate
                        Dim myHisWSResultsRow As HisWSResultsDS.thisWSResultsRow
                        Dim myHisOrderTestList As New List(Of HisWSOrderTestsDS.thisWSOrderTestsRow)
                        Dim myHisWSResultsDelegate As New HisWSResultsDelegate

                        For Each myResultsRow As ResultsDS.vwksResultsRow In pResultsDS.vwksResults.Rows
                            myHisWSResultsRow = myThisWSResultsDS.thisWSResults.NewthisWSResultsRow

                            'For the OrderTestID of the Result, search its ID in Historic Module
                            myHisOrderTestList = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In pHisWSOrderTestsDS.thisWSOrderTests _
                                                 Where a.OrderTestID = myResultsRow.OrderTestID _
                                                Select a).ToList()

                            If (myHisOrderTestList.Count > 0) Then
                                myHisWSResultsRow.HistOrderTestID = myHisOrderTestList.First.HistOrderTestID
                                myHisWSResultsRow.WorkSessionID = myHisOrderTestList.First.WorkSessionID
                                myHisWSResultsRow.AnalyzerID = myResultsRow.AnalyzerID
                                myHisWSResultsRow.MultiPointNumber = myResultsRow.MultiPointNumber
                                myHisWSResultsRow.ResultDateTime = myResultsRow.ResultDateTime

                                If (Not myResultsRow.IsABSValueNull) Then myHisWSResultsRow.ABSValue = myResultsRow.ABSValue
                                If (Not myResultsRow.IsCONC_ValueNull) Then myHisWSResultsRow.CONCValue = myResultsRow.CONC_Value

                                myHisWSResultsRow.ManualResultFlag = myResultsRow.ManualResultFlag
                                If (Not myResultsRow.IsManualResultNull) Then myHisWSResultsRow.ManualResult = myResultsRow.ManualResult
                                If (Not myResultsRow.IsManualResultTextNull AndAlso myResultsRow.ManualResultText <> String.Empty) Then
                                    myHisWSResultsRow.ManualResultText = myResultsRow.ManualResultText
                                End If

                                If (Not myResultsRow.IsUserCommentNull AndAlso myResultsRow.UserComment <> String.Empty) Then
                                    myHisWSResultsRow.UserComment = myResultsRow.UserComment
                                End If

                                If (Not myResultsRow.IsCalibratorFactorNull) Then myHisWSResultsRow.CalibratorFactor = myResultsRow.CalibratorFactor
                                If (Not myResultsRow.IsCalibratorBlankAbsUsedNull) Then myHisWSResultsRow.CalibratorBlankAbsUsed = myResultsRow.CalibratorBlankAbsUsed
                                If (Not myResultsRow.IsCurveSlopeNull) Then myHisWSResultsRow.CurveSlope = myResultsRow.CurveSlope
                                If (Not myResultsRow.IsCurveOffsetNull) Then myHisWSResultsRow.CurveOffSet = myResultsRow.CurveOffset
                                If (Not myResultsRow.IsCurveCorrelationNull) Then myHisWSResultsRow.CurveCorrelation = myResultsRow.CurveCorrelation
                                If (Not myResultsRow.IsRelativeErrorCurveNull) Then myHisWSResultsRow.RelativeErrorCurve = myResultsRow.RelativeErrorCurve
                                If (Not myResultsRow.IsExportStatusNull) Then myHisWSResultsRow.ExportStatus = myResultsRow.ExportStatus
                                If (Not myResultsRow.IsABS_InitialNull) Then myHisWSResultsRow.ABSInitial = myResultsRow.ABS_Initial
                                If (Not myResultsRow.IsAbs_WorkReagentNull) Then myHisWSResultsRow.ABSWorkReagent = myResultsRow.Abs_WorkReagent
                                If (Not myResultsRow.IsABS_MainFilterNull) Then myHisWSResultsRow.ABSMainFilter = myResultsRow.ABS_MainFilter

                                If (Not myHisOrderTestList.First.IsBlankAbsorbanceLimitNull) Then myHisWSResultsRow.BlankAbsorbanceLimit = myHisOrderTestList.First.BlankAbsorbanceLimit
                                If (Not myHisOrderTestList.First.IsKineticBlankLimitNull) Then myHisWSResultsRow.KineticBlankLimit = myHisOrderTestList.First.KineticBlankLimit
                                If (Not myHisOrderTestList.First.IsFactorLowerLimitNull) Then myHisWSResultsRow.FactorLowerLimit = myHisOrderTestList.First.FactorLowerLimit
                                If (Not myHisOrderTestList.First.IsFactorUpperLimitNull) Then myHisWSResultsRow.FactorUpperLimit = myHisOrderTestList.First.FactorUpperLimit

                                'JB 08/10/2012 - All Patient results and all results of not calculated Calibrators are marked as CLOSED. Rest of Blank and Calibrator
                                'Results remain OPEN, which mean they can be reused in other Work Sessions
                                myHisWSResultsRow.ClosedResult = Not ((myResultsRow.SampleClass = "BLANK") OrElse _
                                                                      (myResultsRow.SampleClass = "CALIB" AndAlso myResultsRow.ValidationStatus = "OK"))

                                'JB 04/10/2012 - Set NotCalcCalib = TRUE for results of not calculated Calibrators; for any other result this field is NULL
                                If (myResultsRow.SampleClass = "CALIB") Then myHisWSResultsRow.NotCalcCalib = (myResultsRow.ValidationStatus = "NOTCALC")

                                'For Patient Results, if there are Reference Ranges defined for the TestID/SampleType to which the Result 
                                'belongs, get the Min/Max values applied to validate the Result
                                If (myResultsRow.SampleClass = "PATIENT" AndAlso Not myResultsRow.IsActiveRangeTypeNull) Then
                                    mySampleType = String.Empty
                                    If (myResultsRow.TestType <> "CALC") Then mySampleType = myResultsRow.SampleType

                                    myGlobalDataTO = myOrderTestDelegate.GetReferenceRangeInterval(dbConnection, myResultsRow.OrderTestID, _
                                                                                                   myResultsRow.TestType, myResultsRow.TestID, _
                                                                                                   mySampleType, myResultsRow.ActiveRangeType)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myTestRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)

                                        If (myTestRefRangesDS.tparTestRefRanges.Count > 0) Then
                                            If (myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit <> -1 AndAlso _
                                                myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit <> -1) Then
                                                myHisWSResultsRow.MinRefRange = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
                                                myHisWSResultsRow.MaxRefRange = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit

                                                If (myResultsRow.TestType <> "OFFS") Then
                                                    myCONCValue = myHisWSResultsRow.CONCValue
                                                Else
                                                    myCONCValue = myHisWSResultsRow.ManualResult
                                                End If

                                                'Verify if the result is out of the limits of the NORMALITY REFERENCE RANGE
                                                If (myCONCValue < myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit) Then
                                                    myHisWSResultsRow.RemarkAlert = GlobalConstants.LOW
                                                ElseIf (myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit < myCONCValue) Then
                                                    myHisWSResultsRow.RemarkAlert = GlobalConstants.HIGH
                                                End If

                                                If (myTestRefRangesDS.tparTestRefRanges(0).BorderLineLowerLimit <> -1 AndAlso _
                                                    myTestRefRangesDS.tparTestRefRanges(0).BorderLineUpperLimit <> -1) Then
                                                    'If there are Panic Ranges informed, then verify if the result is out of the limits of the PANIC RANGE
                                                    If (myCONCValue < myTestRefRangesDS.tparTestRefRanges(0).BorderLineLowerLimit) Then
                                                        myHisWSResultsRow.RemarkAlert = GlobalConstants.PANIC_LOW

                                                    ElseIf (myTestRefRangesDS.tparTestRefRanges(0).BorderLineUpperLimit < myCONCValue) Then
                                                        myHisWSResultsRow.RemarkAlert = GlobalConstants.PANIC_HIGH
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Else
                                        'Error getting the applied Reference Ranges...
                                        Exit For
                                    End If
                                End If

                                'JB 10/10/2012 - When a new Blank and/or Calibrator result is moved to Historic Module, all previous Blank and/or Calibrator results
                                'for the same TestID/SampleType are marked as CLOSED (only the last moved remains OPEN)
                                If (Not myGlobalDataTO.HasError) Then
                                    If ((myResultsRow.SampleClass = "BLANK") OrElse _
                                        (myResultsRow.SampleClass = "CALIB" AndAlso myResultsRow.ValidationStatus = "OK")) Then
                                        myGlobalDataTO = myHisWSResultsDelegate.CloseOLDBlankCalibResults(dbConnection, myResultsRow.AnalyzerID, myHisOrderTestList.First.HistTestID, _
                                                                                                          myHisOrderTestList.First.SampleType)
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'Get all Result alarms (if any)
                                    myGlobalDataTO = myResultsAlarmsDelegate.Read(dbConnection, myResultsRow.OrderTestID, myResultsRow.RerunNumber, _
                                                                                  myResultsRow.MultiPointNumber)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myResultAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, ResultAlarmsDS)

                                        'Build a string list containing all alarm codes divided by pipe character
                                        For Each myResultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow In myResultAlarmsDS.twksResultAlarms.Rows
                                            myHisWSResultsRow.AlarmList &= myResultAlarmRow.AlarmID & "|"
                                        Next

                                        If (myHisWSResultsRow.AlarmList.Count > 0) Then
                                            'Remove the last pipe character on the built list alarm and inform the * in field RemarkAlert if it has not informed before
                                            myHisWSResultsRow.AlarmList = myHisWSResultsRow.AlarmList.Remove(myHisWSResultsRow.AlarmList.Length - 1, 1)
                                            If (myHisWSResultsRow.IsRemarkAlertNull) Then myHisWSResultsRow.RemarkAlert = GlobalConstants.NOT_PANIC_REMARK
                                        End If
                                    End If
                                Else
                                    'Error getting the list of alarms raised for the Result
                                    Exit For
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'Finally, inform value of fields HistOrderTestID and HistWorkSessionID in the ResultsDS DataSet to be used 
                                    'later when moving the WS Executions
                                    myResultsRow.HistOrderTestID = myHisOrderTestList.First().HistOrderTestID
                                    myResultsRow.HistWorkSessionID = myHisOrderTestList.First().WorkSessionID
                                End If
                            End If

                            'AG 24/04/2013
                            If (Not myResultsRow.IsLISMessageIDNull AndAlso Not myResultsRow.IsExportStatusNull AndAlso myResultsRow.ExportStatus = "SENDING") Then myHisWSResultsRow.LISMessageID = myResultsRow.LISMessageID Else myHisWSResultsRow.SetLISMessageIDNull()
                            'AG 24/04/2013

                            myThisWSResultsDS.thisWSResults.AddthisWSResultsRow(myHisWSResultsRow)
                        Next

                        'Create all Results in Historic Module
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myHisWSResultsDelegate.Create(dbConnection, myThisWSResultsDS)
                        End If

                        'JB 08/10/2012 - For Blanks and accepted and validated Calibrators, inform the identifiers of WorkSession and OrderTest
                        'in Historic Module in the corresponding result in table twksResults
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myResultDAO As New twksResultsDAO
                            Dim lst As List(Of ResultsDS.vwksResultsRow) = (From resRow As ResultsDS.vwksResultsRow In pResultsDS.vwksResults _
                                                                           Where resRow.SampleClass = "BLANK" _
                                                                         OrElse (resRow.SampleClass = "CALIB" AndAlso resRow.ValidationStatus = "OK")).ToList
                            For Each row In lst
                                myGlobalDataTO = myResultDAO.UpdateHistoryData(pDBConnection, row)
                                If (myGlobalDataTO.HasError) Then Exit For
                            Next
                        End If
                        'JB 08/10/2012 - End

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

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveWSResultsToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Export the basic data of the active WorkSession to Historics Module 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionsDS with the information of the WorkSession created
        '''          in Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012 
        ''' Modified by: SA 31/10/2012 - If the StartDateTime is NULL it means there is a PENDING WorkSession containing results 
        '''                              for OffSystem Tests; in this case, the current date and time is assigned as WS StartDateTime
        ''' </remarks>
        Private Function MoveWSToHISTModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get Analyzer and StartDateTime of the active WorkSession
                        Dim myWS As New WorkSessionsDelegate
                        myGlobalDataTO = myWS.GetByWorkSession(dbConnection, pWorkSessionID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myWorkSessionDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)

                            If (myWorkSessionDS.twksWorkSessions.Rows.Count > 0) Then
                                Dim myHistAnalyzerWSDelegate As New HisAnalyzerWorkSessionsDelegate

                                'Verify if there are WorkSessions in Historic Module for the current date
                                myGlobalDataTO = myHistAnalyzerWSDelegate.GenerateNextSequenceNumber(dbConnection, myWorkSessionDS.twksWorkSessions.First.AnalyzerID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myWorkSessionDS.twksWorkSessions.First.BeginEdit()
                                    myWorkSessionDS.twksWorkSessions.First.HistWorkSessionID = myGlobalDataTO.SetDatos.ToString

                                    'If the StartDateTime is NULL it means there is a PENDING WorkSession containing results for OffSystem Tests
                                    If (myWorkSessionDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                                        myWorkSessionDS.twksWorkSessions.First.StartDateTime = Now()
                                    End If
                                    myWorkSessionDS.twksWorkSessions.First.EndEdit()

                                    'Finally, create the new WS in Historic Module
                                    myGlobalDataTO = myHistAnalyzerWSDelegate.Create(dbConnection, myWorkSessionDS)
                                End If
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
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.MoveWSToHISTModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "METHODS FOR RESULTS REPORTS"
        ''' <summary>
        ''' Get Results info by Patient Sample for Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="GetReplicates"></param>
        ''' <param name="pCompact"></param>
        ''' <param name="pOrderList">Informed for autoprint</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (ReportMaster and ReportDetails tables)</returns>
        ''' <remarks>
        ''' Created by:  RH 03/01/2012
        ''' Modified by: RH 19/03/2012 Label PatientID in Multilanguage
        '''              AG 27/05/2013 - Add new samples types LIQ and SER
        '''              AG 03/10/2013 - new parameter compact that will fill the field FullID without multilanguage resources
        '''              AG 29/07/2014 - #1894 (tests that form part of a calculated test must be excluded from final report depends on the CALC test programming)
        '''              Modified by AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results (when results of different test type are mixed)
        '''              AG 22/09/2014 - BA-1940 in report header show all patient's different barcodes
        '''              AG 23/09/2014 - BA-1940 Merge manual and auto print business in one method not in 2 duplicates methods with few changes
        '''                              (methods GetResultsByPatientSampleForReport and GetResultsByPatientSampleForReportByOrderList)
        '''                              In order to perfom this merge we use KDiff3 utility
        ''' </remarks>
        Public Function GetResultsByPatientSampleForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                           ByVal pWorkSessionID As String, Optional ByVal GetReplicates As Boolean = True, _
                                                           Optional ByVal pCompact As Boolean = False, _
                                                           Optional ByVal pOrderList As List(Of String) = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myExecutionDelegate As New ExecutionsDelegate

                        'AG 23/09/2014 - BA-1940 Merge manual and auto print business in one method not in 2 duplicates methods with few changes
                        'resultData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)
                        Dim autoPrintMode As Boolean = False
                        If Not pOrderList Is Nothing Then autoPrintMode = True

                        If Not autoPrintMode Then
                            resultData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)
                        Else
                            resultData = myExecutionDelegate.GetWSExecutionsResultsByOrderIDList(dbConnection, pAnalyzerID, pWorkSessionID, pOrderList)
                        End If
                        'AG 23/09/2014 - BA-1940

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim ExecutionsResultsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            'Get the list of different OrderTestIDs in the group of Executions
                            Dim OrderTestList As List(Of Integer) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                    Where row.OrderToPrint = True AndAlso row.SampleClass = "PATIENT" _
                                                                 Order By row.TestPosition _
                                                                   Select row.OrderTestID Distinct).ToList()

                            'AG 29/07/2014 - #1894 Get all orderTests that form part of a calculated test that has to be excluded from patients final report
                            Dim orderCalcDelg As New OrderCalculatedTestsDelegate
                            Dim toExcludeFromReport As New List(Of Integer) 'Order tests that form part of a calculated test programmed to not print the partial tests
                            resultData = orderCalcDelg.GetOrderTestsToExcludeInPatientsReport(dbConnection)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                toExcludeFromReport = DirectCast(resultData.SetDatos, List(Of Integer))

                                'Remove these orderTest from OrderTestList of STD tests
                                If toExcludeFromReport.Count > 0 Then
                                    Dim positionToDelete As Integer = 0
                                    For Each item As Integer In toExcludeFromReport
                                        If OrderTestList.Contains(item) Then
                                            positionToDelete = OrderTestList.IndexOf(item)
                                            OrderTestList.RemoveAt(positionToDelete)
                                        End If
                                    Next
                                End If
                            End If
                            'AG 29/07/2014

                            'Get the Patient Results for the list of OrderTestIDs
                            Dim AverageResultsDS As New ResultsDS
                            Dim myResultsDelegate As New ResultsDelegate

                            resultData = GetResultsForReports(dbConnection, OrderTestList)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                            End If

                            'Get Calculated Results
                            'AG 01/08/2014 #1897 add new optional parameter to TRUE
                            resultData = myResultsDelegate.GetCalculatedTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                For Each resultRow As ResultsDS.vwksResultsRow In DirectCast(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                    'AG 29/07/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
                                    'AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
                                        AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    End If
                                    'AG 29/07/2014
                                Next
                            End If

                            'Get ISE & OffSystem Tests Results
                            'AG 01/08/2014 #1897 add new optional parameter to TRUE
                            resultData = myResultsDelegate.GetISEOFFSTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                    'AG 29/07/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
                                    'AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
                                        AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    End If
                                    'AG 29/07/2014
                                Next resultRow
                            End If

                            'AG 03/10/2013 - final report compact has to show barcode, we need inform the avg results field
                            If pCompact Then
                                resultData = FillSpecimenIDListForReport(dbConnection, pWorkSessionID, AverageResultsDS)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                                End If
                            End If
                            'AG 03/10/2013

                            'AG 01/08/2014 #1897 Finally sort the dataset using the TestPosition column
                            'Protection: If no results (for example all tests OUT) exit function returning the espected DS but empty
                            If AverageResultsDS.vwksResults.Rows.Count = 0 Then
                                'Returned data empty
                                resultData.SetDatos = New ResultsDS
                            Else

                                'AverageResultsDS.vwksResults.DefaultView.Sort = "TestPosition" 'This code does not work!!
                                Dim sortedReportList As New List(Of ResultsDS.vwksResultsRow)
                                Dim sortedResultsToPrintDS As New ResultsDS
                                sortedReportList = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
                                              Select a Order By a.TestPosition Ascending).ToList

                                For Each reportRow As ResultsDS.vwksResultsRow In sortedReportList
                                    sortedResultsToPrintDS.vwksResults.ImportRow(reportRow)
                                Next
                                sortedResultsToPrintDS.vwksResults.AcceptChanges()
                                AverageResultsDS = sortedResultsToPrintDS
                                sortedReportList = Nothing
                                'AG 01/08/2014 #1897

                                'Read Reference Range Limits
                                Dim MinimunValue As Nullable(Of Single) = Nothing
                                Dim MaximunValue As Nullable(Of Single) = Nothing

                                Dim myOrderTestsDelegate As New OrderTestsDelegate
                                For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                                    If (Not resultRow.IsActiveRangeTypeNull) Then
                                        Dim mySampleType As String = String.Empty
                                        If (resultRow.TestType <> "CALC") Then mySampleType = resultRow.SampleType

                                        'Get the Reference Range for the Test/SampleType according the TestType and the Type of Range
                                        resultData = myOrderTestsDelegate.GetReferenceRangeInterval(dbConnection, resultRow.OrderTestID, resultRow.TestType, _
                                                                                                    resultRow.TestID, mySampleType, resultRow.ActiveRangeType)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myTestRefRangesDS As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                                            If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
                                                MinimunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
                                                MaximunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit
                                            End If
                                        End If

                                        If (MinimunValue.HasValue AndAlso MaximunValue.HasValue) Then
                                            If (MinimunValue <> -1 AndAlso MaximunValue <> -1) Then
                                                resultRow.NormalLowerLimit = MinimunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                                resultRow.NormalUpperLimit = MaximunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                            End If
                                        End If
                                    End If
                                Next resultRow

                                'Fill Average PatientID and Name fields
                                Dim IsOrderProcessed As New Dictionary(Of String, Boolean)
                                For Each executionRow As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults.Rows
                                    If (executionRow.SampleClass = "PATIENT") Then
                                        If (Not IsOrderProcessed.ContainsKey(executionRow.OrderID)) Then
                                            For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                                                If (resultRow.OrderID = executionRow.OrderID) Then
                                                    resultRow.PatientName = executionRow.PatientName
                                                    resultRow.PatientID = executionRow.PatientID
                                                End If
                                            Next resultRow

                                            IsOrderProcessed(executionRow.OrderID) = True
                                        End If
                                    End If
                                Next executionRow

                                Dim currentLanguageGlobal As New GlobalBase
                                Dim CurrentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage
                                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

                                Dim literalPatientID As String
                                literalPatientID = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_PatientID", CurrentLanguage)

                                'DL 17/06/2013
                                Dim literalPatientName As String
                                literalPatientName = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Summary_PatientName", CurrentLanguage)
                                'DL 17/06/2013

                                Dim literalGender As String
                                literalGender = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Gender", CurrentLanguage)

                                Dim literalBirthDate As String
                                literalBirthDate = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_DateOfBirth", CurrentLanguage)

                                Dim literalAge As String
                                literalAge = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Age", CurrentLanguage)

                                Dim literalPerformedBy As String
                                literalPerformedBy = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Patients_PerformedBy", CurrentLanguage)

                                Dim literalComments As String
                                literalComments = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Flags", CurrentLanguage)  'EF 03/06/2014 (cargar texto LBL_FLags)

                                'RH 15/05/2012 Get Patient data
                                Dim myPatientsDelegate As New PatientDelegate
                                Dim SelectedPatients As List(Of String) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                          Where row.SampleClass = "PATIENT" _
                                                                        AndAlso row.OrderToPrint = True _
                                                                        AndAlso row.RerunNumber = 1 _
                                                                        Select row.PatientID Distinct).ToList()


                                resultData = myPatientsDelegate.GetPatientsForReport(dbConnection, CurrentLanguage, SelectedPatients)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim PatientsData As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)

                                    'AG 23/09/2014 - BA-1940 Merge manual and auto print business in one method not in 2 duplicates methods with few changes
                                    '   NOTE: Next code #1226 was only in the method for manual print. But it seems it has to exists also in auto print
                                    '   That's why next code aply for always (if some problem found we ought to execute it only If not autoPrintMode Then ... End If

                                    'JV 31/10/13 #1226 Obtain the OK orders selected by the user
                                    Dim myDel As New OrderTestsDelegate
                                    Dim myGDT As GlobalDataTO
                                    myGDT = myDel.GetOrdersOKByUser(Nothing, pAnalyzerID, pWorkSessionID)
                                    If (Not myGDT.HasError AndAlso Not myGDT.SetDatos Is Nothing) Then
                                        Dim OT As OrdersDS = DirectCast(myGDT.SetDatos, OrdersDS)
                                        Dim SelectedOK As List(Of String) = Enumerable.Cast(Of String)(From row In OT.twksOrders Select row.PatientID Distinct).ToList()
                                        SelectedPatients = (From p In SelectedPatients Where SelectedOK.Contains(p) Select p).ToList()
                                        SelectedOK = Nothing 'AG 23/09/2014 - BA-1940
                                    End If
                                    myDel = Nothing
                                    myGDT = Nothing
                                    'JV 31/10/13 #1226

                                    'RH 24/05/2012 Create info for not registered patients
                                    For Each PatientID As String In SelectedPatients
                                        If (From row In PatientsData.tparPatients Where String.Compare(row.PatientID, PatientID, False) = 0 Select row).Count = 0 Then
                                            Dim newRow As PatientsDS.tparPatientsRow

                                            newRow = PatientsData.tparPatients.NewtparPatientsRow()
                                            newRow.PatientID = PatientID
                                            PatientsData.tparPatients.AddtparPatientsRow(newRow)
                                        End If
                                    Next

                                    Dim AgeUnitsListDS As New PreloadedMasterDataDS
                                    Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

                                    resultData = preloadedMasterConfig.GetList(dbConnection, PreloadedMasterDataEnum.AGE_UNITS)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        AgeUnitsListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                        Dim ResultsForReportDS As New ResultsDS
                                        Dim PatientIDList As New List(Of String)
                                        Dim SamplesList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
                                        Dim BarcodesByPatient As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) 'AG 23/09/2014 - BA-1940
                                        Dim StatFlag() As Boolean = {True, False}
                                        Dim existsRow As Boolean = False
                                        Dim FullID As String
                                        Dim FullName As String
                                        Dim FullGender As String
                                        Dim FullBirthDate As String
                                        Dim FullAge As String
                                        Dim FullPerformedBy As String
                                        Dim FullComments As String
                                        Dim Pat As PatientsDS.tparPatientsRow

                                        Dim linqSpecimenFromResults As List(Of ResultsDS.vwksResultsRow) 'AG 28/06/2013
                                        'Fill ReportMaster table
                                        For i As Integer = 0 To 1
                                            Dim aux_i = i
                                            SamplesList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                          Where row.SampleClass = "PATIENT" _
                                                        AndAlso row.OrderToPrint = True _
                                                        AndAlso row.StatFlag = StatFlag(aux_i) _
                                                        AndAlso row.RerunNumber = 1 _
                                                         Select row).ToList()

                                            For Each sampleRow As ExecutionsDS.vwksWSExecutionsResultsRow In SamplesList
                                                'AG 23/09/2014 - BA-1940 Merge manual and auto print business in one method not in 2 duplicates methods with few changes
                                                '   NOTE: Next If condition #1226 was only in the method for manual print. But it seems it has to exists also in auto print
                                                '   That's why next code aply for always (if some problem found we ought to execute different conditions depending the variable autoPrintMode value

                                                If (SelectedPatients.Contains(sampleRow.PatientID) AndAlso Not PatientIDList.Contains(sampleRow.PatientID)) Then 'If (Not PatientIDList.Contains(sampleRow.PatientID)) Then 'JV 31/10/13 #1226
                                                    'AG 23/09/2014 - BA-1940 

                                                    PatientIDList.Add(sampleRow.PatientID)

                                                    Pat = (From row As PatientsDS.tparPatientsRow In PatientsData.tparPatients _
                                                          Where row.PatientID = sampleRow.PatientID _
                                                         Select row).First()

                                                    If (Not Pat.IsDateOfBirthNull) Then
                                                        Pat.AgeWithUnit = Utilities.GetAgeUnits(Pat.DateOfBirth, AgeUnitsListDS)
                                                        Pat.FormatedDateOfBirth = Pat.DateOfBirth.ToString(DatePattern)
                                                    End If

                                                    'AG 23/09/2014 - BA-1940 'DL 17/06/2013
                                                    Dim patIDforReport As String = sampleRow.PatientID
                                                    'If Not sampleRow.IsSpecimenIDListNull Then
                                                    '    patIDforReport &= " (" & sampleRow.SpecimenIDList & ")"
                                                    'End If
                                                    BarcodesByPatient = (From item In SamplesList Where Not item.IsSpecimenIDListNull And item.PatientID = sampleRow.PatientID Select item).ToList
                                                    If BarcodesByPatient.Count > 0 Then
                                                        Dim patBarCodeTO As New PatientSpecimenTO
                                                        For Each item As ExecutionsDS.vwksWSExecutionsResultsRow In BarcodesByPatient
                                                            patBarCodeTO.UpdateSpecimenList(item.SpecimenIDList)
                                                        Next
                                                        patIDforReport &= patBarCodeTO.GetSpecimenIdListForReports
                                                    End If
                                                    'AG 23/09/2014 - BA-1940

                                                    'EF 30/05/2014 (Separar el título del valor, Cambiar orden de campos Nombre Paciente y mostrar solo si está asignado) + XB 10/07/2014 - kill repeated lables - #1673
                                                    'FullID = String.Format("{0}: {1}", literalPatientID, patIDforReport
                                                    ''FullName = String.Format("{0} {1}", Pat.FirstName, Pat.LastName)
                                                    'FullName = String.Format("{0}: {1} {2}", literalPatientName, Pat.FirstName, Pat.LastName)
                                                    ''DL 17/06/2013
                                                    'FullGender = String.Format("{0}: {1}", literalGender, Pat.Gender)
                                                    'FullBirthDate = String.Format("{0}: {1}", literalBirthDate, Pat.FormatedDateOfBirth)
                                                    'FullAge = String.Format("{0}: {1}", literalAge, Pat.AgeWithUnit)
                                                    'FullPerformedBy = String.Format("{0}: {1}", literalPerformedBy, Pat.PerformedBy)
                                                    'FullComments = String.Format("{0}: {1}", literalComments, Pat.Comments)

                                                    FullID = String.Format("{0}", patIDforReport)
                                                    If (Pat.LastName <> "-" And Pat.LastName <> "") Or (Pat.FirstName <> "-" And Pat.FirstName <> "") Then FullName = String.Format("{0}, {1}", Pat.LastName, Pat.FirstName) Else FullName = ""
                                                    FullGender = String.Format("{0}", Pat.Gender)
                                                    FullBirthDate = String.Format("{0}", Pat.FormatedDateOfBirth)
                                                    FullAge = String.Format("{0}", Pat.AgeWithUnit)
                                                    FullPerformedBy = String.Format("{0}", Pat.PerformedBy)
                                                    FullComments = String.Format("{0}", Pat.Comments)
                                                    'EF 30/05/2014 End

                                                    ResultsForReportDS.ReportSampleMaster.AddReportSampleMasterRow(sampleRow.PatientID, FullID, _
                                                                                                                   FullName, FullGender, _
                                                                                                                   FullBirthDate, FullAge, _
                                                                                                                   FullPerformedBy, FullComments, DateTime.Now) 'IT 30/07/2014 #BA-1893
                                                End If
                                            Next sampleRow

                                        Next
                                        linqSpecimenFromResults = Nothing 'AG 28/06/2013
                                        SamplesList = Nothing 'AG 23/09/2014 - BA-1940
                                        BarcodesByPatient = Nothing 'AG 23/09/2014 - BA-1940

                                        'Fill ReportDetails table
                                        Dim DetailPatientID As String
                                        Dim TestName As String
                                        Dim SampleType As String
                                        Dim ReplicateNumber As String
                                        Dim ABSValue As String
                                        Dim CONC_Value As String
                                        Dim ReferenceRanges As String
                                        Dim Unit As String
                                        Dim ResultDate As String
                                        Dim Flags As String  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)

                                        Dim tmpOrderTestId As Integer
                                        Dim IsAverageDone As Dictionary(Of String, Boolean)
                                        Dim AverageList As List(Of ResultsDS.vwksResultsRow)

                                        Dim myOrderTestID As Integer
                                        Dim maxTheoreticalConc As Single
                                        Dim Filter As String

                                        For Each SampleID As String In PatientIDList
                                            tmpOrderTestId = -1
                                            IsAverageDone = New Dictionary(Of String, Boolean)
                                            AverageList = New List(Of ResultsDS.vwksResultsRow)
                                            'Filter = String.Empty

                                            For Each row As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                                                If (row.PatientID = SampleID) Then
                                                    If (String.Compare(row.TestType, "STD", False) = 0) Then
                                                        myOrderTestID = row.OrderTestID
                                                        maxTheoreticalConc = (From resultRow In AverageResultsDS.vwksResults _
                                                                             Where resultRow.OrderTestID = myOrderTestID _
                                                                            Select resultRow.TheoricalConcentration).Max

                                                        If (row.TheoricalConcentration = maxTheoreticalConc) Then
                                                            AverageList.Add(row)
                                                        End If

                                                        'AG 01/12/2010
                                                    ElseIf (row.TestType = "ISE" OrElse row.TestType = "OFFS") Then
                                                        AverageList.Add(row)
                                                        'End AG 01/12/2010

                                                    Else 'CALC
                                                        AverageList.Add(row)
                                                    End If
                                                End If
                                            Next row

                                            'TR 27/05/2013 -Get a list of sample types separated by commas
                                            Dim SampleTypes() As String = Nothing
                                            Dim myMasterDataDelegate As New MasterDataDelegate

                                            resultData = myMasterDataDelegate.GetSampleTypes(dbConnection)
                                            If Not resultData.HasError Then
                                                SampleTypes = resultData.SetDatos.ToString.Split(CChar(","))
                                            End If

                                            'RH 18/05/2012 Sort by SampleType
                                            'Dim SampleTypes() As String = {"SER", "URI", "PLM", "WBL", "CSF", "LIQ", "SEM"}
                                            For Each sortedSampleType In SampleTypes
                                                Dim NewAverageList As List(Of ResultsDS.vwksResultsRow) = (From row In AverageList _
                                                                                                          Where String.Compare(row.SampleType, sortedSampleType, False) = 0 _
                                                                                                         Select row).ToList()

                                                For Each resultRow As ResultsDS.vwksResultsRow In NewAverageList
                                                    Filter = String.Format("{0}{1}", resultRow.OrderTestID, resultRow.RerunNumber)

                                                    'Insert only Accepted Results
                                                    If (resultRow.AcceptedResultFlag AndAlso Not IsAverageDone.ContainsKey(Filter)) Then
                                                        IsAverageDone(Filter) = True
                                                        DetailPatientID = resultRow.PatientID

                                                        'AG 29/04/2014 - #1608
                                                        'TestName = resultRow.TestName
                                                        If Not resultRow.IsTestLongNameNull Then TestName = resultRow.TestLongName Else TestName = resultRow.TestName

                                                        SampleType = resultRow.SampleType
                                                        ReplicateNumber = String.Empty

                                                        If (Not resultRow.IsABSValueNull) Then
                                                            ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                                        Else
                                                            ABSValue = String.Empty
                                                        End If

                                                        If (Not resultRow.IsCONC_ValueNull) Then
                                                            Dim hasConcentrationError As Boolean = False

                                                            If (Not resultRow.IsCONC_ErrorNull) Then
                                                                hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
                                                            End If

                                                            If (Not hasConcentrationError) Then
                                                                CONC_Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                                            Else
                                                                CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                                            End If
                                                        Else
                                                            If (Not resultRow.IsManualResultTextNull) Then
                                                                CONC_Value = resultRow.ManualResultText
                                                            Else
                                                                CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                                            End If
                                                        End If

                                                        Unit = resultRow.MeasureUnit

                                                        If (Not String.IsNullOrEmpty(resultRow.NormalLowerLimit) AndAlso _
                                                            Not String.IsNullOrEmpty(resultRow.NormalUpperLimit)) Then
                                                            ReferenceRanges = String.Format("{0} - {1}", resultRow.NormalLowerLimit, resultRow.NormalUpperLimit)
                                                        Else
                                                            ReferenceRanges = String.Empty
                                                        End If

                                                        'AG 15/09/2010 - Special case when Absorbance has error
                                                        If (Not resultRow.IsABS_ErrorNull) Then
                                                            If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
                                                                ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                                                CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                                            End If
                                                        End If
                                                        'END AG 15/09/2010

                                                        'EF 03/06/2014 - Provisional v3.0.1 (PENDIENTE hacer un metodo único que devuelva el valor del FLAG a todos los reports, pantallas, etc y reutilizar!!)
                                                        'RH 16/05/2012
                                                        Flags = String.Empty
                                                        '    'Verify if the result is out of the limits of the NORMALITY REFERENCE RANGE
                                                        If ((Not resultRow.IsActiveRangeTypeNull AndAlso Not String.Compare(resultRow.ActiveRangeType, String.Empty, False) = 0) AndAlso _
                                                            (IsNumeric(CONC_Value))) Then
                                                            If (Not resultRow.IsNormalLowerLimitNull AndAlso Not resultRow.IsNormalUpperLimitNull) Then
                                                                If (CSng(CONC_Value) < CSng(resultRow.NormalLowerLimit)) Then
                                                                    Flags = GlobalConstants.LOW '"L"
                                                                ElseIf (CSng(CONC_Value) > CSng(resultRow.NormalUpperLimit)) Then
                                                                    Flags = GlobalConstants.HIGH '"H"
                                                                End If
                                                            End If
                                                        End If
                                                        'RH 16/05/2012 - END
                                                        'If there are Panic Ranges informed, then verify if the result is out of the limits of the PANIC RANGE
                                                        If IsNumeric(CONC_Value) And (Not resultRow.IsPanicLowerLimitNull AndAlso Not resultRow.IsPanicUpperLimitNull) Then
                                                            If (CSng(CONC_Value) < CSng(resultRow.PanicLowerLimit)) Then
                                                                Flags = GlobalConstants.PANIC_LOW  'PL
                                                            ElseIf (CSng(resultRow.PanicUpperLimit) < CSng(CONC_Value)) Then
                                                                Flags = GlobalConstants.PANIC_HIGH 'PH 
                                                            End If
                                                        End If
                                                        'EF 03/06/2014 END

                                                        ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & _
                                                                     resultRow.ResultDateTime.ToString(TimePattern)

                                                        'AG 03/10/2013 - compact report shows the barcode here because there is not header
                                                        If pCompact Then
                                                            If Not resultRow.IsSpecimenIDListNull Then
                                                                DetailPatientID &= " (" & resultRow.SpecimenIDList & ")"
                                                            End If
                                                        End If
                                                        'AG 03/10/2013

                                                        'Insert Details row
                                                        ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow(resultRow.PatientID, DetailPatientID, TestName, SampleType, _
                                                                                                                         ReplicateNumber, ABSValue, CONC_Value, _
                                                                                                                         ReferenceRanges, Unit, ResultDate, _
                                                                                                                         Flags)  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)

                                                        tmpOrderTestId = resultRow.OrderTestID
                                                        Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010

                                                        Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                                                                             Where row.OrderTestID = tmpOrderTestId _
                                                                                                                           AndAlso row.RerunNumber = myRerunNumber _
                                                                                                                            Select row).ToList()

                                                        If (GetReplicates AndAlso (SampleList.Count > 0)) Then
                                                            Flags = String.Empty  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)
                                                            TestName = String.Empty
                                                            SampleType = String.Empty
                                                            ReferenceRanges = String.Empty

                                                            For j As Integer = 0 To SampleList.Count - 1
                                                                DetailPatientID = SampleList(j).PatientID
                                                                ReplicateNumber = SampleList(j).ReplicateNumber.ToString()
                                                                Unit = resultRow.MeasureUnit

                                                                'AG 02/08/2010
                                                                Dim hasConcentrationError As Boolean = False
                                                                If (Not SampleList(j).IsCONC_ErrorNull) Then 'RH 14/09/2010
                                                                    hasConcentrationError = Not String.IsNullOrEmpty(SampleList(j).CONC_Error)
                                                                End If

                                                                If (Not hasConcentrationError) Then
                                                                    If (Not SampleList(j).IsCONC_ValueNull) Then
                                                                        CONC_Value = SampleList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                                                    ElseIf (Not resultRow.IsManualResultTextNull) Then
                                                                        'Take the Manual Result text from the average result
                                                                        CONC_Value = resultRow.ManualResultText
                                                                    Else
                                                                        CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                                                    End If
                                                                Else
                                                                    CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                                                End If
                                                                'END AG 02/08/2010

                                                                'JV 17/12/2013 #1184 - INI
                                                                If (Not SampleList(j).IsABS_ValueNull) Then
                                                                    ABSValue = SampleList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                                                Else
                                                                    ABSValue = String.Empty
                                                                End If
                                                                'JV 17/12/2013 #1184 - END

                                                                'AG 15/09/2010 - Special case when Absorbance has error
                                                                If (Not SampleList(j).IsABS_ErrorNull) Then
                                                                    If (Not String.IsNullOrEmpty(SampleList(j).ABS_Error)) Then
                                                                        ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                                                        CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                                                    End If
                                                                End If
                                                                'END AG 15/09/2010

                                                                ResultDate = SampleList(j).ResultDate.ToString(DatePattern) & " " & _
                                                                             SampleList(j).ResultDate.ToString(TimePattern)

                                                                'AG 03/10/2013 - compact report shows the barcode here because there is not header
                                                                If pCompact Then
                                                                    If Not SampleList(j).IsSpecimenIDListNull Then
                                                                        DetailPatientID &= " (" & SampleList(j).SpecimenIDList & ")"
                                                                    End If
                                                                End If
                                                                'AG 03/10/2013

                                                                'Insert Details row
                                                                ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow(resultRow.PatientID, DetailPatientID, TestName, _
                                                                                                                                 SampleType, ReplicateNumber, _
                                                                                                                                 ABSValue, CONC_Value, _
                                                                                                                                 ReferenceRanges, Unit, _
                                                                                                                                 ResultDate, Flags)  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)
                                                            Next
                                                        End If
                                                    End If
                                                Next
                                            Next
                                        Next

                                        resultData.SetDatos = ResultsForReportDS
                                    End If
                                End If
                                SelectedPatients = Nothing 'AG 23/09/2014 - BA-1940 

                            End If 'AG 10/09/2014 -BA-1894 , BA-1897 - Protection End If

                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsByPatientSampleForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get Results info by Test for Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultDS (ReportMaster and ReportDetails tables)</returns>
        ''' <remarks>
        ''' Created by: RH 10/01/2012
        '''             AG 28/06/2013 - Patch for the test types without executions
        '''             SA 30/04/2014 - BT #1608 ==> When the header for Test is filled, if field TestLongName is informed, use it as Test Name
        '''                                          instead of field TestName
        '''             AG 10/09/2014 - BA-1894 and BA-1897 integrate also in reports by TEST the same functionality existing by PATIENT (v3.0.2.2)
        ''' </remarks>
        Public Function GetResultsByTestForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myExecutionDelegate As New ExecutionsDelegate

                        resultData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim ExecutionsResultsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                            Dim OrderTestList As List(Of Integer) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                 Order By row.TestPosition _
                                                                   Select row.OrderTestID Distinct).ToList()

                            'AG 10/09/2014 - #1894 Get all orderTests that form part of a calculated test that has to be excluded from patients final report
                            Dim orderCalcDelg As New OrderCalculatedTestsDelegate
                            Dim toExcludeFromReport As New List(Of Integer) 'Order tests that form part of a calculated test programmed to not print the partial tests
                            resultData = orderCalcDelg.GetOrderTestsToExcludeInPatientsReport(dbConnection)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                toExcludeFromReport = DirectCast(resultData.SetDatos, List(Of Integer))

                                'Remove these orderTest from OrderTestList of STD tests
                                If toExcludeFromReport.Count > 0 Then
                                    Dim positionToDelete As Integer = 0
                                    For Each item As Integer In toExcludeFromReport
                                        If OrderTestList.Contains(item) Then
                                            positionToDelete = OrderTestList.IndexOf(item)
                                            OrderTestList.RemoveAt(positionToDelete)
                                        End If
                                    Next
                                End If
                            End If
                            'AG 10/09/2014

                            'TR 09/07/2012
                            Dim AverageResultsDS As New ResultsDS
                            resultData = GetResultsForReports(dbConnection, OrderTestList)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                                'Get Calculated Results
                                'AG 10/09/2014 #1897 add new optional parameter to TRUE / add only order tests not existing in to exclude list
                                resultData = GetCalculatedTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    For Each resultRow As ResultsDS.vwksResultsRow In DirectCast(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                        'AG 10/09/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
                                        If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
                                            AverageResultsDS.vwksResults.ImportRow(resultRow)
                                        End If
                                    Next
                                End If

                            End If

                            If (Not resultData.HasError) Then
                                'Get ISE & OffSystem Tests Results
                                'AG 10/09/2014 #1897 add new optional parameter to TRUE / add only order tests not existing in to exclude list
                                resultData = GetISEOFFSTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                        'AG 10/09/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
                                        If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
                                            AverageResultsDS.vwksResults.ImportRow(resultRow)
                                        End If
                                    Next resultRow
                                End If

                                'AG 28/06/2013 - Patch for the test types without executions
                                resultData = FillSpecimenIDListForReport(dbConnection, pWorkSessionID, AverageResultsDS)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                                End If
                                'AG 28/06/2013

                                'AG 10/09/2014 #1897 Finally sort the dataset using the TestPosition column
                                'Protection: If no results (for example all tests OUT) exit function returning the espected DS but empty
                                If AverageResultsDS.vwksResults.Rows.Count = 0 Then
                                    'Returned data empty
                                    resultData.SetDatos = New ResultsDS
                                Else

                                    Dim sortedReportList As New List(Of ResultsDS.vwksResultsRow)
                                    Dim sortedResultsToPrintDS As New ResultsDS
                                    sortedReportList = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
                                                  Select a Order By a.TestPosition Ascending).ToList

                                    For Each reportRow As ResultsDS.vwksResultsRow In sortedReportList
                                        sortedResultsToPrintDS.vwksResults.ImportRow(reportRow)
                                    Next
                                    sortedResultsToPrintDS.vwksResults.AcceptChanges()
                                    AverageResultsDS = sortedResultsToPrintDS
                                    sortedReportList = Nothing
                                    'AG 10/09/2014 #1897

                                    Dim IsOrderProcessed As New Dictionary(Of String, Boolean)

                                    'Fill Average PatientID and Name fields
                                    For Each executionRow As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults.Rows
                                        If (String.Equals(executionRow.SampleClass, "PATIENT")) Then
                                            If (Not IsOrderProcessed.ContainsKey(executionRow.OrderID)) Then
                                                For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                                                    If (String.Compare(resultRow.OrderID, executionRow.OrderID, False) = 0) Then
                                                        resultRow.PatientName = executionRow.PatientName
                                                        resultRow.PatientID = executionRow.PatientID
                                                    End If
                                                Next resultRow

                                                IsOrderProcessed(executionRow.OrderID) = True
                                            End If
                                        End If
                                    Next executionRow

                                    'Fill ReportMaster table
                                    Dim TestsList As New List(Of String)
                                    Dim NamePlusUnit As String
                                    Dim TestTypeTestID As String
                                    Dim ResultsForReportDS As New ResultsDS

                                    'Note that here TestID is in fact TestPosition
                                    Dim myTestName As String = String.Empty
                                    For Each testRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults
                                        If (Not TestsList.Contains(testRow.TestName)) Then
                                            TestsList.Add(testRow.TestName)
                                            TestTypeTestID = testRow.TestType & testRow.TestID

                                            'BT #1608 - When field TestLongName is informed for the Test, it is used as TestName in the Report
                                            If (testRow.IsTestLongNameNull) Then
                                                myTestName = testRow.TestName
                                            Else
                                                myTestName = testRow.TestLongName
                                            End If

                                            'RH 23/03/2012
                                            If (String.IsNullOrEmpty(testRow.MeasureUnit)) Then
                                                NamePlusUnit = myTestName 'testRow.TestName

                                            Else
                                                'NamePlusUnit = String.Format("{0} ({1})", testRow.TestName, testRow.MeasureUnit)
                                                NamePlusUnit = String.Format("{0} ({1})", myTestName, testRow.MeasureUnit)
                                            End If

                                            ResultsForReportDS.ReportTestMaster.AddReportTestMasterRow(TestTypeTestID, NamePlusUnit)
                                        End If
                                    Next testRow

                                    'Multilanguage
                                    Dim currentLanguageGlobal As New GlobalBase
                                    Dim LanguageID As String = GlobalBase.GetSessionInfo().ApplicationLanguage
                                    Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                                    Dim BLANKLiteral As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_BLANK", LanguageID)
                                    Dim CALIBLiteral As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CALIB", LanguageID)
                                    Dim CTRLLiteral As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CTRL", LanguageID)
                                    Dim PATIENTLiteral As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_PATIENT", LanguageID)
                                    'END Multilanguage

                                    'Fill ReportDetails table
                                    For Each Test As String In TestsList
                                        InsertResultsBLANK(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForReportDS, BLANKLiteral)
                                        InsertResultsCALIB(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForReportDS, CALIBLiteral)
                                        InsertResultsCTRL(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForReportDS, CTRLLiteral)
                                        InsertResultsPATIENT(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForReportDS, PATIENTLiteral)
                                    Next

                                    resultData.SetDatos = ResultsForReportDS

                                End If 'AG 10/09/2014 -BA-1894 , BA-1897 - Protection End If
                            End If
                    End If
                End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsByTestForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Results Calibrator Curve multipoint info for Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns a ResultDS (ReportCalibCurve table)
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH 23/01/2012
        ''' </remarks>
        Public Function GetResultsCalibCurveForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                   ByVal pWorkSessionID As String, ByVal TestName As String, _
                                                   ByVal AcceptedRerunNumber As Integer) As GlobalDataTO

            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myExecutionDelegate As New ExecutionsDelegate

                        resultData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)

                        If (Not resultData.HasError) Then
                            Dim AverageResultsDS As New ResultsDS
                            Dim ExecutionsResultsDS As ExecutionsDS

                            ExecutionsResultsDS = CType(resultData.SetDatos, ExecutionsDS)

                            Dim OrderTestList As List(Of Integer) = _
                                    (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                     Where String.Compare(row.SampleClass, "CALIB", False) = 0 AndAlso String.Compare(row.TestName, TestName, False) = 0 _
                                     Order By row.TestPosition _
                                     Select row.OrderTestID Distinct).ToList()

                            Dim myResultsDelegate As New ResultsDelegate

                            For Each OrderTestID As Integer In OrderTestList
                                'Get all results for current OrderTestID.
                                resultData = myResultsDelegate.GetResults(dbConnection, OrderTestID)
                                If (Not resultData.HasError) Then
                                    For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                        AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    Next resultRow
                                Else
                                    'ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                                    Exit For
                                End If
                            Next OrderTestID

                            Dim ResultsForReportDS As New ResultsDS

                            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                                    (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                     Where String.Compare(row.TestName, TestName, False) = 0 AndAlso String.Compare(row.SampleClass, "CALIB", False) = 0 _
                                     AndAlso row.RerunNumber = AcceptedRerunNumber _
                                     Select row).ToList()

                            If TestList.Count = 0 Then
                                resultData.SetDatos = ResultsForReportDS

                                Return resultData
                            End If

                            Dim TheoreticalConcList As List(Of Single) = _
                                            (From row In AverageResultsDS.vwksResults _
                                             Where row.OrderTestID = TestList(0).OrderTestID _
                                             Select row.TheoricalConcentration Distinct).ToList()

                            If TheoreticalConcList.Count = 0 Then
                                resultData.SetDatos = ResultsForReportDS

                                Return resultData
                            End If

                            'Const i As Integer = 0
                            Dim IsAverageDone As New Dictionary(Of Integer, Boolean)

                            Dim CurveResultsID As Integer = 0
                            Dim SampleType As String = String.Empty
                            Dim CurveGrowthType As String = String.Empty
                            Dim CurveType As String = String.Empty
                            Dim CurveAxisXType As String = String.Empty
                            Dim CurveAxisYType As String = String.Empty
                            Dim CalibrationError As String = String.Empty
                            Dim MeasureUnit As String = String.Empty
                            Dim MultiPointNumber As String = String.Empty
                            Dim ABSValue As String = String.Empty
                            Dim CONC_Value As String = String.Empty
                            Dim TheoreticalConc As String = String.Empty
                            Dim RelativeErrorCurve As String = String.Empty
                            Dim CurveSlope As Single = 0
                            Dim CurveOffset As Single = 0
                            Dim CurveCorrelation As Single = 0
                            Dim CalibratorBlankAbsUsed As Single = 0

                            Dim itempoint As Integer = 0

                            For Each myTheoreticalConc As Single In TheoreticalConcList
                                itempoint += 1

                                Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                                                (From row In AverageResultsDS.vwksResults _
                                                 Where row.OrderTestID = TestList(0).OrderTestID _
                                                 AndAlso row.TheoricalConcentration = myTheoreticalConc _
                                                 AndAlso row.MultiPointNumber = itempoint _
                                                 AndAlso row.RerunNumber = AcceptedRerunNumber _
                                                 Select row).ToList()

                                For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                                    If Not IsAverageDone.ContainsKey(resultRow.MultiPointNumber) Then
                                        IsAverageDone(resultRow.MultiPointNumber) = True

                                        CurveResultsID = resultRow.CurveResultsID
                                        SampleType = resultRow.SampleType
                                        CurveGrowthType = resultRow.CurveGrowthType
                                        CurveType = resultRow.CurveType
                                        CurveAxisXType = resultRow.CurveAxisXType
                                        CurveAxisYType = resultRow.CurveAxisYType
                                        CalibrationError = resultRow.CalibrationError
                                        MeasureUnit = resultRow.MeasureUnit
                                        MultiPointNumber = resultRow.MultiPointNumber.ToString()
                                        ABSValue = resultRow.ABSValue.ToStringWithDecimals(4)
                                        CONC_Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                        TheoreticalConc = resultRow.TheoricalConcentration.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                        RelativeErrorCurve = resultRow.RelativeErrorCurve.ToStringWithDecimals(4)

                                        If resultRow.IsCurveSlopeNull Then
                                            CurveSlope = Single.MinValue
                                        Else
                                            CurveSlope = resultRow.CurveSlope
                                        End If

                                        If resultRow.IsCurveOffsetNull Then
                                            CurveOffset = Single.MinValue
                                        Else
                                            CurveOffset = resultRow.CurveOffset
                                        End If

                                        If resultRow.IsCurveCorrelationNull Then
                                            CurveCorrelation = Single.MinValue
                                        Else
                                            CurveCorrelation = resultRow.CurveCorrelation
                                        End If

                                        CalibratorBlankAbsUsed = resultRow.CalibratorBlankAbsUsed

                                        ResultsForReportDS.ReportCalibCurve.AddReportCalibCurveRow( _
                                                CurveResultsID, SampleType, CurveGrowthType, CurveType, _
                                                CurveAxisXType, CurveAxisYType, CalibrationError, MeasureUnit, _
                                                MultiPointNumber, ABSValue, CONC_Value, TheoreticalConc, _
                                                RelativeErrorCurve, CurveSlope, CurveOffset, CurveCorrelation, _
                                                CalibratorBlankAbsUsed)
                                    End If
                                Next

                            Next myTheoreticalConc

                            resultData.SetDatos = ResultsForReportDS
                            'Else
                            'ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsCalibCurveForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Results for the specified OrderTestID (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestList">Order Test List</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by:  RH
        ''' </remarks>
        Public Function GetResultsForReports(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestList As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim i As Integer = 0
                        Dim myResultsDS As New ResultsDS
                        Dim myOrderList As New StringBuilder
                        Dim AverageResultsDS As New ResultsDS
                        Dim mytwksResultDAO As New twksResultsDAO

                        For Each orderTestID As Integer In pOrderTestList
                            If (i = 200) Then
                                i = 0
                                myOrderList.Append(orderTestID & ",")

                                'Get all results for current OrderTestID
                                resultData = mytwksResultDAO.GetResultsForReport(dbConnection, myOrderList.ToString().Remove(myOrderList.ToString().Length - 1, 1))
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                                    For Each resultRow As ResultsDS.vwksResultsRow In myResultsDS.vwksResults.Rows
                                        AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    Next resultRow
                                Else
                                    Exit For
                                End If

                                myOrderList.Length = 0
                            Else
                                myOrderList.Append(orderTestID & ",")
                            End If

                            If (pOrderTestList.Last() = orderTestID) Then
                                If (myOrderList.Length >= 0) Then
                                    'Get all results for current OrderTestID.
                                    resultData = mytwksResultDAO.GetResultsForReport(dbConnection, myOrderList.ToString().Remove(myOrderList.ToString().Length - 1, 1))
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                                        For Each resultRow As ResultsDS.vwksResultsRow In myResultsDS.vwksResults.Rows
                                            AverageResultsDS.vwksResults.ImportRow(resultRow)
                                        Next resultRow
                                    Else
                                        Exit For
                                    End If

                                    myOrderList.Length = 0
                                End If
                            End If

                            i += 1
                        Next orderTestID

                        If (Not resultData.HasError) Then
                            resultData.SetDatos = AverageResultsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsForReports", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Summary Results info by Patient Sample for Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns a DataTable
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH 18/01/2012
        ''' </remarks>
        Public Function GetSummaryResultsByPatientSampleForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                   ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim AverageResultsDS As New ResultsDS

                        Dim orderCalcDelg As New OrderCalculatedTestsDelegate
                        Dim toExcludeFromReport As New List(Of Integer) 'Order tests that form part of a calculated test programmed to not print the partial tests
                        resultData = orderCalcDelg.GetOrderTestsToExcludeInPatientsReport(dbConnection)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            toExcludeFromReport = DirectCast(resultData.SetDatos, List(Of Integer))
                        End If

                        Dim myResultsDelegate As New ResultsDelegate
                        'Get all results for current Analyzer & WorkSession
                        resultData = myResultsDelegate.GetCompleteResults(Nothing, pAnalyzerID, pWorkSessionID)
                        If (Not resultData.HasError) Then
                            AverageResultsDS = CType(resultData.SetDatos, ResultsDS)

                            'IT 04/04/2014 (INI) - #1884 Exclude CALC tests that form part of another calculated test programmed to not print the partial tests
                            For Each row As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                                If ((row.TestType = "CALC") And (toExcludeFromReport.Contains(row.OrderTestID))) Then
                                    row.Delete()
                                End If
                            Next

                            'Confirm changes done in the entry DataSet 
                            AverageResultsDS.vwksResults.AcceptChanges()
                            'IT 04/04/2014 (FIN) - #1884
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetSummaryResultsByPatientSampleForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Fills a ResultsDS with BLANK results for Test Report
        ''' </summary>
        ''' <param name="AverageResultsDS">Average info</param>
        ''' <param name="ExecutionsResultsDS">Executions info</param>
        ''' <param name="ResultsForReportDS">The DataSet to fill in</param>
        ''' <remarks>
        ''' Created by: RH 11/01/2012
        ''' Modified by: JV #1502 20/02/2014 - optional param for not including replicates for new report
        ''' </remarks>
        Private Sub InsertResultsBLANK(ByVal TestName As String, ByVal AverageResultsDS As ResultsDS, ByVal ExecutionsResultsDS As ExecutionsDS, _
                                       ByVal ResultsForReportDS As ResultsDS, ByVal SampleClassLiteral As String, Optional ByVal pReplicates As Boolean = True)
            Dim SampleClass As String
            Dim Name As String
            Dim SampleType As String = String.Empty
            Dim ReplicateNumber As String
            Dim ABSValue As String
            Dim CONC_Value As String = String.Empty
            Dim MeasureUnit As String = String.Empty
            Dim Factor As String = String.Empty
            Dim ResultDate As String
            Dim TestTypeTestID As String

            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                               Where String.Compare(row.TestName, TestName, False) = 0 AndAlso String.Compare(row.SampleClass, "BLANK", False) = 0 _
                                                                              Select row).ToList()
            If (TestList.Count = 0) Then
                Return
            End If

            Dim maxTheoreticalConc As Single = (From row In AverageResultsDS.vwksResults _
                                               Where row.OrderTestID = TestList(0).OrderTestID _
                                              Select row.TheoricalConcentration).Max

            Dim AverageList As List(Of ResultsDS.vwksResultsRow) = (From row In AverageResultsDS.vwksResults _
                                                                   Where row.OrderTestID = TestList(0).OrderTestID _
                                                                 AndAlso row.TheoricalConcentration = maxTheoreticalConc _
                                                                  Select row).ToList()

            For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                'Insert only Accepted Results
                If (resultRow.AcceptedResultFlag) Then
                    SampleClass = SampleClassLiteral
                    Name = TestName
                    ReplicateNumber = String.Empty

                    'AG 15/09/2010 - Special case when Absorbance has error
                    If (Not resultRow.IsABS_ErrorNull) Then
                        If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
                            ABSValue = GlobalConstants.ABSORBANCE_ERROR
                        Else
                            ABSValue = String.Empty
                        End If
                    Else
                        'There is no error, so update ABSValue
                        If (Not resultRow.IsABSValueNull) Then
                            ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        Else
                            ABSValue = String.Empty
                        End If
                    End If
                    'END AG 15/09/2010

                    ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & _
                                 resultRow.ResultDateTime.ToString(TimePattern)

                    TestTypeTestID = resultRow.TestType & resultRow.TestID
                    MeasureUnit = resultRow.MeasureUnit

                    'Insert new row
                    ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow(TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, _
                                                                                 ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)

                    Dim OrderTestID As Integer = resultRow.OrderTestID
                    Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010

                    'AG 04/08/2010 - add rerunnumber condition
                    TestList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                               Where row.OrderTestID = OrderTestID _
                             AndAlso row.RerunNumber = myRerunNumber _
                              Select row).ToList()

                    If (TestList.Count > 0 And pReplicates) Then 'If (TestList.Count > 0) Then
                        For j As Integer = 0 To TestList.Count - 1
                            SampleClass = String.Empty
                            Name = String.Empty
                            ReplicateNumber = TestList(j).ReplicateNumber.ToString()

                            'AG 15/09/2010 - Special case when Absorbance has error
                            If (Not TestList(j).IsABS_ErrorNull) Then
                                If (Not String.IsNullOrEmpty(TestList(j).ABS_Error)) Then
                                    ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                Else
                                    ABSValue = String.Empty
                                    'Also change value for WorkReagent, AbsInitial,AbsMainFilter? By now no!
                                End If
                            Else
                                'RH 18/10/2010
                                'There is no error, so update dgv("ABSValue", k).Value
                                If (Not TestList(j).IsABS_ValueNull) Then
                                    ABSValue = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                Else
                                    ABSValue = String.Empty
                                End If
                            End If
                            'END AG 15/09/2010

                            ResultDate = TestList(j).ResultDate.ToString(DatePattern) & " " & _
                                         TestList(j).ResultDate.ToString(TimePattern)

                            'Insert new row
                            ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow(TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, _
                                                                                         ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)
                        Next
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Fills a ResultsDS with CALIB results for Test Report
        ''' </summary>
        ''' <param name="AverageResultsDS">Average info</param>
        ''' <param name="ExecutionsResultsDS">Executions info</param>
        ''' <param name="ResultsForReportDS">The DataSet to fill in</param>
        ''' <remarks>
        ''' Created by: RH 11/01/2012
        ''' Modified by: JV #1502 20/02/2014 - optional param for not including replicates for new report
        ''' </remarks>
        Private Sub InsertResultsCALIB(ByVal TestName As String, ByVal AverageResultsDS As ResultsDS, ByVal ExecutionsResultsDS As ExecutionsDS, _
                                       ByVal ResultsForReportDS As ResultsDS, ByVal SampleClassLiteral As String, Optional ByVal pReplicates As Boolean = True)
            Dim SampleClass As String
            Dim Name As String
            Dim SampleType As String
            Dim ReplicateNumber As String
            Dim ABSValue As String
            Dim CONC_Value As String
            Dim MeasureUnit As String = String.Empty
            Dim Factor As String
            Dim ResultDate As String
            Dim TestTypeTestID As String

            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                               Where String.Compare(row.TestName, TestName, False) = 0 AndAlso String.Compare(row.SampleClass, "CALIB", False) = 0 _
                                                                              Select row).ToList()

            If (TestList.Count = 0) Then
                Return
            End If

            Dim AverageList As List(Of ResultsDS.vwksResultsRow) = (From row In AverageResultsDS.vwksResults _
                                                                   Where String.Compare(row.TestName, TestName, False) = 0 AndAlso String.Compare(row.OrderID, TestList(0).OrderID, False) = 0 _
                                                                  Select row).ToList()

            Dim calibratorMaxItemNumber As Integer = (From row In AverageList Select row.MultiPointNumber).Max

            If (calibratorMaxItemNumber = 1) Then 'Show the calibrator 1 item grid
                For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                    'Insert only Accepted Results
                    If (resultRow.AcceptedResultFlag) Then
                        SampleClass = SampleClassLiteral
                        Name = resultRow.CalibratorName
                        SampleType = resultRow.SampleType
                        ReplicateNumber = String.Empty
                        ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        CONC_Value = resultRow.TheoricalConcentration.ToStringWithDecimals(resultRow.DecimalsAllowed)

                        If (Not resultRow.IsCalibratorFactorNull) Then
                            If (String.Compare(resultRow.CalibrationError.Trim, "", False) = 0) Then
                                If (Not resultRow.ManualResultFlag) Then
                                    Factor = resultRow.CalibratorFactor.ToString("F4")
                                Else
                                    Factor = resultRow.ManualResult.ToString("F4")
                                End If
                            Else
                                Factor = GlobalConstants.FACTOR_NOT_CALCULATED
                            End If
                        Else
                            Factor = String.Empty
                        End If

                        'AG 15/09/2010 - Special case when Absorbance has error
                        If (Not resultRow.IsABS_ErrorNull) Then
                            If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
                                ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                'Also change value for Factor? By now no!
                            End If
                        End If

                        ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & _
                                     resultRow.ResultDateTime.ToString(TimePattern)

                        TestTypeTestID = resultRow.TestType & resultRow.TestID
                        MeasureUnit = resultRow.MeasureUnit

                        'Insert new row
                        ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow(TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, _
                                                                                     ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)

                        Dim OrderTestID As Integer = resultRow.OrderTestID
                        Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010

                        'AG 04/08/2010 - add rerunnumber condition
                        TestList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                   Where row.OrderTestID = OrderTestID _
                                 AndAlso row.RerunNumber = myRerunNumber _
                                  Select row).ToList()

                        If (TestList.Count > 0 And pReplicates) Then 'If (TestList.Count > 0) Then
                            For j As Integer = 0 To TestList.Count - 1
                                SampleClass = String.Empty
                                Name = String.Empty
                                SampleType = String.Empty
                                ReplicateNumber = TestList(j).ReplicateNumber.ToString()
                                ABSValue = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                CONC_Value = resultRow.TheoricalConcentration.ToStringWithDecimals(resultRow.DecimalsAllowed)

                                'AG 15/09/2010 - Special case when Absorbance has error
                                If (Not TestList(j).IsABS_ErrorNull) Then
                                    If (Not String.IsNullOrEmpty(TestList(j).ABS_Error)) Then
                                        ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                    End If
                                End If
                                'END AG 15/09/2010

                                Factor = String.Empty

                                ResultDate = TestList(j).ResultDate.ToString(DatePattern) & " " & _
                                             TestList(j).ResultDate.ToString(TimePattern)

                                'Insert new row
                                ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow(TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, _
                                                                                             ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)
                            Next
                        End If
                    End If
                Next
            Else
                'MultiItem Calibrator
                Dim TheoreticalConcList As List(Of Single) = (From row In AverageResultsDS.vwksResults _
                                                             Where row.OrderTestID = TestList(0).OrderTestID _
                                                            Select row.TheoricalConcentration Distinct _
                                                          Order By TheoricalConcentration Descending).ToList()
                If (TheoreticalConcList.Count = 0) Then Return

                Dim IsAverageDone As New Dictionary(Of Integer, Boolean)
                Dim itempoint As Integer = 0

                For Each myTheoreticalConc As Single In TheoreticalConcList
                    itempoint += 1

                    AverageList = (From row In AverageResultsDS.vwksResults _
                                  Where row.OrderTestID = TestList(0).OrderTestID _
                                AndAlso row.TheoricalConcentration = myTheoreticalConc _
                                AndAlso row.MultiPointNumber = itempoint _
                                 Select row).ToList()

                    For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                        'Insert only Accepted Results
                        If (resultRow.AcceptedResultFlag AndAlso Not IsAverageDone.ContainsKey(resultRow.MultiPointNumber)) Then
                            IsAverageDone(resultRow.MultiPointNumber) = True

                            SampleClass = String.Format("{0} {1}", SampleClassLiteral, resultRow.MultiPointNumber)
                            Name = resultRow.CalibratorName
                            SampleType = resultRow.SampleType
                            ReplicateNumber = String.Empty
                            ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            Factor = String.Empty

                            If (Not resultRow.IsCONC_ValueNull) Then
                                Dim hasConcentrationError As Boolean = False

                                If (Not resultRow.IsCONC_ErrorNull) Then
                                    hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error) 'AG 15/09/2010
                                End If

                                If (Not hasConcentrationError) Then
                                    CONC_Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                Else
                                    CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                End If
                            Else
                                CONC_Value = String.Empty
                            End If

                            'AG 15/09/2010 - Special case when Absorbance has error
                            If (Not resultRow.IsABS_ErrorNull) Then
                                If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
                                    ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                    CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                End If
                            End If
                            'END AG 15/09/2010

                            ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & _
                                         resultRow.ResultDateTime.ToString(TimePattern)

                            TestTypeTestID = resultRow.TestType & resultRow.TestID
                            MeasureUnit = resultRow.MeasureUnit

                            'Insert new row
                            ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow(TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, _
                                                                                         ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)

                            Dim OrderTestID As Integer = resultRow.OrderTestID
                            Dim myRerunNumber As Integer = resultRow.RerunNumber

                            'AG 04/08/2010 - add rerunnumber condition
                            TestList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                       Where row.OrderTestID = OrderTestID _
                                     AndAlso row.RerunNumber = myRerunNumber _
                                     AndAlso row.MultiItemNumber = itempoint _
                                      Select row Distinct).ToList()

                            If pReplicates Then
                                For j As Integer = 0 To TestList.Count - 1
                                    SampleClass = String.Empty
                                    Name = String.Empty
                                    SampleType = String.Empty
                                    ReplicateNumber = TestList(j).ReplicateNumber.ToString()
                                    ABSValue = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                    CONC_Value = resultRow.TheoricalConcentration.ToStringWithDecimals(resultRow.DecimalsAllowed)

                                    'AG 15/09/2010 - Special case when Absorbance has error
                                    If (Not TestList(j).IsABS_ErrorNull) Then
                                        If (Not String.IsNullOrEmpty(TestList(j).ABS_Error)) Then
                                            ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                        End If
                                    End If
                                    'END AG 15/09/2010

                                    If (Not TestList(j).IsCONC_ValueNull) Then
                                        Dim hasConcentrationError As Boolean = False

                                        If (Not TestList(j).IsCONC_ErrorNull) Then
                                            hasConcentrationError = Not String.IsNullOrEmpty(TestList(j).CONC_Error) 'AG 15/09/2010
                                        End If

                                        If (Not hasConcentrationError) Then
                                            CONC_Value = TestList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                        Else
                                            CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                        End If
                                    Else
                                        CONC_Value = String.Empty
                                    End If

                                    'AG 15/09/2010 - Special case when Absorbance has error
                                    If (Not TestList(j).IsABS_ErrorNull) Then
                                        If (Not String.IsNullOrEmpty(TestList(j).ABS_Error)) Then
                                            ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                            CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                        End If
                                    End If
                                    'END AG 15/09/2010

                                    ResultDate = TestList(j).ResultDate.ToString(DatePattern) & " " & _
                                                 TestList(j).ResultDate.ToString(TimePattern)

                                    'Insert new row
                                    ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow(TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, _
                                                                                                 ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)
                                Next
                            End If
                        End If
                    Next
                Next myTheoreticalConc
            End If
        End Sub

        ''' <summary>
        ''' Fills a ResultsDS with CTRL results for Test Report
        ''' </summary>
        ''' <param name="AverageResultsDS">Average info</param>
        ''' <param name="ExecutionsResultsDS">Executions info</param>
        ''' <param name="ResultsForReportDS">The DataSet to fill in</param>
        ''' <remarks>
        ''' Created by: RH 11/01/2012
        ''' Modified by: JV #1502 20/02/2014 - optional param for not including replicates for new report
        ''' </remarks>
        Private Sub InsertResultsCTRL(ByVal TestName As String, _
                                      ByVal AverageResultsDS As ResultsDS, _
                                      ByVal ExecutionsResultsDS As ExecutionsDS, _
                                      ByVal ResultsForReportDS As ResultsDS, _
                                      ByVal SampleClassLiteral As String, Optional ByVal pReplicates As Boolean = True)

            Dim SampleClass As String
            Dim Name As String = Nothing
            Dim SampleType As String
            Dim ReplicateNumber As String
            Dim ABSValue As String
            Dim CONC_Value As String
            Dim MeasureUnit As String = String.Empty
            Dim Factor As String = String.Empty
            Dim ResultDate As String
            Dim TestTypeTestID As String

            Dim orderTestIDs As List(Of Integer) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                    Where String.Compare(row.TestName, TestName, False) = 0 _
                                                    AndAlso String.Compare(row.SampleClass, "CTRL", False) = 0 _
                                                    Select row.OrderTestID Distinct).ToList()

            If (orderTestIDs.Count = 0) Then
                Return
            End If

            'Dim maxRows As Integer = 0

            For Each orderTestID As Integer In orderTestIDs
                'Get all results for the currently processed OrderTestID 
                Dim AverageList As List(Of ResultsDS.vwksResultsRow) = (From row In AverageResultsDS.vwksResults _
                                                                       Where String.Compare(row.TestName, TestName, False) = 0 AndAlso row.OrderTestID = orderTestID _
                                                                      Select row).ToList()

                For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                    'Insert only Accepted Results
                    If resultRow.AcceptedResultFlag Then
                        SampleClass = SampleClassLiteral

                        'DL 28/06/2012. Begin
                        If String.Equals(resultRow.ControlName.Trim, String.Empty) AndAlso _
                           String.Equals(resultRow.SampleClass, "CTRL") AndAlso _
                           String.Equals(resultRow.TestType, "ISE") Then

                            Dim myTestControlDelegate As New TestControlsDelegate
                            Dim myGlobalDataTO As New GlobalDataTO

                            myGlobalDataTO = myTestControlDelegate.GetControlsNEW(Nothing, resultRow.TestType, resultRow.TestID, resultRow.SampleType, 0)

                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO Is Nothing Then
                                Dim myTestControlsData As New TestControlsDS()
                                myTestControlsData = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)
                                If myTestControlsData.tparTestControls.Rows.Count > 0 Then
                                    Name = myTestControlsData.tparTestControls.First.ControlName
                                End If
                            End If
                        Else
                            Name = resultRow.ControlName
                        End If
                        'DL 28/06/2012. End

                        SampleType = resultRow.SampleType
                        ReplicateNumber = String.Empty

                        'DL 28/06/2012. BEGIN
                        'ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)

                        If Not resultRow.IsABSValueNull Then
                            ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        Else
                            ABSValue = String.Empty
                        End If
                        'DL 28/06/2012. END 

                        If (Not resultRow.IsCONC_ValueNull) Then
                            Dim hasConcentrationError As Boolean = False

                            If (Not resultRow.IsCONC_ErrorNull) Then
                                hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
                            End If

                            If (Not hasConcentrationError) Then
                                CONC_Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                            Else
                                CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            End If
                        Else
                            CONC_Value = String.Empty
                        End If

                        If (Not resultRow.IsABS_ErrorNull) Then
                            If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
                                ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                            End If
                        End If

                        ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & resultRow.ResultDateTime.ToString(TimePattern)
                        TestTypeTestID = resultRow.TestType & resultRow.TestID
                        MeasureUnit = resultRow.MeasureUnit

                        'Insert new row
                        ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow( _
                                TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)

                        Dim myRerunNumber As Integer = resultRow.RerunNumber
                        Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                                (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                 Where row.OrderTestID = orderTestID _
                                 AndAlso row.RerunNumber = myRerunNumber _
                                 Select row).ToList()

                        If (TestList.Count > 0 And pReplicates) Then 'If (TestList.Count > 0) Then
                            For j As Integer = 0 To TestList.Count - 1
                                SampleClass = String.Empty
                                Name = String.Empty
                                SampleType = String.Empty
                                ReplicateNumber = TestList(j).ReplicateNumber.ToString()

                                'DL 28/06/2012. BEGIN
                                'ABSValue = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)

                                If Not TestList(j).IsABS_ValueNull Then
                                    ABSValue = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                Else
                                    ABSValue = String.Empty
                                End If
                                'DL 28/06/2012. END 

                                Dim hasConcentrationError As Boolean = False
                                If (Not TestList(j).IsCONC_ErrorNull) Then
                                    hasConcentrationError = Not String.IsNullOrEmpty(TestList(j).CONC_Error)
                                End If
                                If (Not hasConcentrationError) Then
                                    CONC_Value = TestList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                Else
                                    CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                End If

                                If (Not TestList(j).IsABS_ErrorNull) Then
                                    If (Not String.IsNullOrEmpty(TestList(j).ABS_Error)) Then
                                        ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                        CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                    End If
                                End If

                                ResultDate = TestList(j).ResultDate.ToString(DatePattern) & " " & _
                                                           TestList(j).ResultDate.ToString(TimePattern)

                                'TestTypeTestID = resultRow.TestType & resultRow.TestID

                                'Insert new row
                                ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow( _
                                        TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)

                            Next
                        End If
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Fills a ResultsDS with PATIENT results for Test Report
        ''' </summary>
        ''' <param name="AverageResultsDS">Average info</param>
        ''' <param name="ExecutionsResultsDS">Executions info</param>
        ''' <param name="ResultsForReportDS">The DataSet to fill in</param>
        ''' <remarks>
        ''' Created by: RH 11/01/2012
        ''' Modified by AG 28/06/2013 - Patch for the test types without executions
        '''             XB 10/10/2014 - Correction - BA-1993
        ''' </remarks>
        Private Sub InsertResultsPATIENT(ByVal TestName As String, _
                                         ByVal AverageResultsDS As ResultsDS, _
                                         ByVal ExecutionsResultsDS As ExecutionsDS, _
                                         ByVal ResultsForReportDS As ResultsDS, _
                                         ByVal SampleClassLiteral As String)

            Dim SampleClass As String
            Dim Name As String
            Dim SampleType As String
            Dim ReplicateNumber As String
            Dim ABSValue As String
            Dim CONC_Value As String
            Dim MeasureUnit As String = String.Empty
            Dim Factor As String = String.Empty
            Dim ResultDate As String
            Dim TestTypeTestID As String

            Dim CalcTestInserted As New Dictionary(Of String, Boolean)
            Dim ISEOFFSTestInserted As New Dictionary(Of String, Boolean)
            Dim PatientInserted As New Dictionary(Of String, Boolean)
            Dim SamplesAverageList As New List(Of ResultsDS.vwksResultsRow)

            For Each row As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                If String.Compare(row.TestName, TestName, False) = 0 AndAlso String.Compare(row.PatientID, String.Empty, False) <> 0 AndAlso _
                   (Not row.IsCONC_ValueNull OrElse Not row.IsManualResultTextNull) Then
                    If String.Compare(row.TestType, "STD", False) = 0 Then
                        Dim myOrderTestID As Integer = row.OrderTestID
                        Dim maxTheoreticalConc As Single = _
                            (From resultRow In AverageResultsDS.vwksResults _
                             Where resultRow.OrderTestID = myOrderTestID _
                             Select resultRow.TheoricalConcentration).Max

                        If row.TheoricalConcentration = maxTheoreticalConc Then
                            SamplesAverageList.Add(row)
                        End If
                        'AG 01/12/2010
                    ElseIf String.Compare(row.TestType, "ISE", False) = 0 OrElse String.Compare(row.TestType, "OFFS", False) = 0 Then
                        'AG 14/01/2011 - original code fails when several reruns ... <AG 22/12/2010>
                        ''If Not ISEOFFSTestInserted.ContainsKey(row.TestName) Then
                        'If ISEOFFSTestInserted.ContainsKey(row.TestName) And PatientInserted.ContainsKey(row.PatientName) Then
                        'Dim pat_RerunID As String = ""
                        'pat_RerunID = row.PatientName & "<RerunNumber: " & row.RerunNumber & ">"
                        'Dim pat_RerunID As String = String.Format("{0}<RerunNumber: {1}>", row.PatientID, row.RerunNumber)

                        'RH 13/12/2011
                        Dim pat_RerunID As String = String.Format("{0}<RerunNumber: {1}><SampleType: {2}>", _
                                                                  row.PatientID, row.RerunNumber, row.SampleType)

                        If ISEOFFSTestInserted.ContainsKey(row.TestName) AndAlso PatientInserted.ContainsKey(pat_RerunID) Then
                            'Nothing: Test-Patient already added
                        Else
                            ISEOFFSTestInserted(row.TestName) = True
                            'PatientInserted(row.PatientName) = True 'AG 22/12/2010
                            PatientInserted(pat_RerunID) = True 'AG 14/01/2011
                            SamplesAverageList.Add(row)
                        End If
                        'End AG 01/12/2010

                    Else 'It is CALC
                        'AG 22/12/2010
                        'If Not CalcTestInserted.ContainsKey(row.TestName) Then
                        If CalcTestInserted.ContainsKey(row.TestName) AndAlso PatientInserted.ContainsKey(row.PatientID) Then
                            ''Nothing: Test-Patient already added
                        Else
                            CalcTestInserted(row.TestName) = True
                            PatientInserted(row.PatientID) = True 'AG 22/12/2010
                            SamplesAverageList.Add(row)
                        End If
                    End If
                End If
            Next

            If SamplesAverageList.Count = 0 Then
                Return
            End If

            Dim IsAverageDone As New Dictionary(Of String, Boolean)
            Dim linqSpecimenFromResults As List(Of ResultsDS.vwksResultsRow) 'AG 28/06/2013
            For Each resultRow As ResultsDS.vwksResultsRow In SamplesAverageList
                Dim Filter As String = String.Format("{0}{1}", resultRow.OrderTestID, resultRow.RerunNumber)

                'If Not IsAverageDone.ContainsKey(Filter) Then

                'Insert only Accepted Results
                If resultRow.AcceptedResultFlag AndAlso Not IsAverageDone.ContainsKey(Filter) Then
                    IsAverageDone(Filter) = True

                    Dim OrderTestID As Integer = resultRow.OrderTestID

                    SampleClass = SampleClassLiteral
                    SampleType = resultRow.SampleType
                    ReplicateNumber = String.Empty

                    If Not resultRow.IsABSValueNull Then
                        ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                    Else
                        ABSValue = String.Empty
                    End If

                    If Not resultRow.IsCONC_ValueNull Then
                        'AG 02/08/2010
                        Dim hasConcentrationError As Boolean = False

                        If Not resultRow.IsCONC_ErrorNull Then 'RH 14/09/2010
                            hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
                        End If

                        If Not hasConcentrationError Then
                            CONC_Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                        Else
                            CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                        End If
                        'END AG 02/08/2010
                    Else
                        If (Not resultRow.IsManualResultTextNull) Then
                            CONC_Value = resultRow.ManualResultText
                        Else
                            CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                        End If
                    End If

                    'AG 15/09/2010 - Special case when Absorbance has error
                    If Not resultRow.IsABS_ErrorNull Then
                        If Not String.IsNullOrEmpty(resultRow.ABS_Error) Then
                            ABSValue = GlobalConstants.ABSORBANCE_ERROR
                            CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                        End If
                    End If
                    'END AG 15/09/2010

                    'Name = IIf(resultRow.RerunNumber = 1, resultRow.PatientName, String.Format("{0} ({1})", resultRow.PatientName, resultRow.RerunNumber)).ToString()
                    'Name = resultRow.PatientName

                    'DL 17/06/2013
                    Dim mySpecimenIDList As String = String.Empty
                    Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010
                    Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                            (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                             Where row.OrderTestID = OrderTestID _
                             AndAlso row.RerunNumber = myRerunNumber _
                             Select row).ToList()

                    If SampleList.Count > 0 Then
                        If Not SampleList.First.IsSpecimenIDListNull Then
                            mySpecimenIDList = "(" & SampleList.First.SpecimenIDList & ")"
                        End If

                        'AG 28/06/2013 - Patch for the test types without executions
                    ElseIf resultRow.TestType = "CALC" OrElse resultRow.TestType = "OFFS" Then
                        linqSpecimenFromResults = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
                                          Where Not a.IsSpecimenIDListNull AndAlso a.OrderID = resultRow.OrderID Select a).ToList
                        If linqSpecimenFromResults.Count > 0 Then
                            mySpecimenIDList &= " (" & linqSpecimenFromResults.First.SpecimenIDList & ")"
                        End If
                        'AG 28/06/2013

                    End If

                    If mySpecimenIDList <> String.Empty Then
                        Name = resultRow.PatientID & vbCrLf & mySpecimenIDList
                    Else
                        Name = resultRow.PatientID
                    End If

                    'Name = resultRow.PatientID
                    'DL 17/06/2013

                    ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & _
                                    resultRow.ResultDateTime.ToString(TimePattern)

                    TestTypeTestID = resultRow.TestType & resultRow.TestID

                    MeasureUnit = resultRow.MeasureUnit

                    'Insert new row
                    ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow( _
                            TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)

                    'DL 17/06/2013
                    'Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010
                    'Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                    '        (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                    '         Where row.OrderTestID = OrderTestID _
                    '         AndAlso row.RerunNumber = myRerunNumber _
                    '         Select row).ToList()
                    'DL 17/06/2013

                    If SampleList.Count > 0 Then
                        For j As Integer = 0 To SampleList.Count - 1
                            SampleClass = String.Empty
                            Name = String.Empty
                            SampleType = String.Empty

                            Dim hasConcentrationError As Boolean = False

                            If Not SampleList(j).IsCONC_ErrorNull Then
                                hasConcentrationError = Not String.IsNullOrEmpty(SampleList(j).CONC_Error)
                            End If

                            If Not hasConcentrationError Then
                                If (Not SampleList(j).IsCONC_ValueNull) Then
                                    CONC_Value = SampleList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                ElseIf Not resultRow.IsManualResultTextNull Then
                                    CONC_Value = resultRow.ManualResultText
                                Else
                                    CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                End If
                            Else
                                CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            End If

                            ' XB 10/10/2014 - BA-1993
                            If (Not SampleList(j).IsABS_ValueNull) Then
                                ABSValue = SampleList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            Else
                                ABSValue = String.Empty
                            End If
                            ' XB 10/10/2014 - BA-1993

                            'AG 15/09/2010 - Special case when Absorbance has error
                            If Not SampleList(j).IsABS_ErrorNull Then
                                If Not String.IsNullOrEmpty(SampleList(j).ABS_Error) Then
                                    ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                    CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                End If
                            End If
                            'END AG 15/09/2010


                            ReplicateNumber = SampleList(j).ReplicateNumber.ToString()

                            ResultDate = SampleList(j).ResultDate.ToString(DatePattern) & " " & _
                                    SampleList(j).ResultDate.ToString(TimePattern)

                            'TestTypeTestID = resultRow.TestType & resultRow.TestID

                            'Insert new row
                            ResultsForReportDS.ReportTestDetails.AddReportTestDetailsRow( _
                                    TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, ABSValue, CONC_Value, MeasureUnit, Factor, ResultDate)

                        Next
                    End If
                End If
            Next
            linqSpecimenFromResults = Nothing
        End Sub
#End Region

#Region "OLD METHODS FOR RESULTS REPORTS - COMMENTED"

        ' ''' <summary>
        ' ''' Get Results info by Patient Sample for Report
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ' ''' <param name="GetReplicates"></param>
        ' ''' <param name="pCompact"></param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (ReportMaster and ReportDetails tables)</returns>
        ' ''' <remarks>
        ' ''' Created by:  RH 03/01/2012
        ' ''' Modified by: RH 19/03/2012 Label PatientID in Multilanguage
        ' '''              AG 27/05/2013 - Add new samples types LIQ and SER
        ' '''              AG 03/10/2013 - new parameter compact that will fill the field FullID without multilanguage resources
        ' '''              AG 29/07/2014 - #1894 (tests that form part of a calculated test must be excluded from final report depends on the CALC test programming)
        ' '''              Modified by AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results (when results of different test type are mixed)
        ' '''              AG 22/09/2014 - BA-1940 in report header show all patient's different barcodes
        ' '''              AG 23/09/2014 - BA-1940 comment old code and implement a unique method compatible manual and auto firal report (GetResultsByPatientSampleForReportByOrderList)
        ' ''' </remarks>
        'Public Function GetResultsByPatientSampleForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                                   ByVal pWorkSessionID As String, Optional ByVal GetReplicates As Boolean = True, _
        '                                                   Optional ByVal pCompact As Boolean = False) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myExecutionDelegate As New ExecutionsDelegate

        '                resultData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)
        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    Dim ExecutionsResultsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

        '                    'Get the list of different OrderTestIDs in the group of Executions
        '                    Dim OrderTestList As List(Of Integer) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                            Where row.OrderToPrint = True AndAlso row.SampleClass = "PATIENT" _
        '                                                         Order By row.TestPosition _
        '                                                           Select row.OrderTestID Distinct).ToList()

        '                    'AG 29/07/2014 - #1894 Get all orderTests that form part of a calculated test that has to be excluded from patients final report
        '                    Dim orderCalcDelg As New OrderCalculatedTestsDelegate
        '                    Dim toExcludeFromReport As New List(Of Integer) 'Order tests that form part of a calculated test programmed to not print the partial tests
        '                    resultData = orderCalcDelg.GetOrderTestsToExcludeInPatientsReport(dbConnection)
        '                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                        toExcludeFromReport = DirectCast(resultData.SetDatos, List(Of Integer))

        '                        'Remove these orderTest from OrderTestList of STD tests
        '                        If toExcludeFromReport.Count > 0 Then
        '                            Dim positionToDelete As Integer = 0
        '                            For Each item As Integer In toExcludeFromReport
        '                                If OrderTestList.Contains(item) Then
        '                                    positionToDelete = OrderTestList.IndexOf(item)
        '                                    OrderTestList.RemoveAt(positionToDelete)
        '                                End If
        '                            Next
        '                        End If
        '                    End If
        '                    'AG 29/07/2014

        '                    'Get the Patient Results for the list of OrderTestIDs
        '                    Dim AverageResultsDS As New ResultsDS
        '                    Dim myResultsDelegate As New ResultsDelegate

        '                    resultData = GetResultsForReports(dbConnection, OrderTestList)
        '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                        AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
        '                    End If

        '                    'Get Calculated Results
        '                    'AG 01/08/2014 #1897 add new optional parameter to TRUE
        '                    resultData = myResultsDelegate.GetCalculatedTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
        '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                        For Each resultRow As ResultsDS.vwksResultsRow In DirectCast(resultData.SetDatos, ResultsDS).vwksResults.Rows
        '                            'AG 29/07/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
        '                            'AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
        '                                AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            End If
        '                            'AG 29/07/2014
        '                        Next
        '                    End If

        '                    'Get ISE & OffSystem Tests Results
        '                    'AG 01/08/2014 #1897 add new optional parameter to TRUE
        '                    resultData = myResultsDelegate.GetISEOFFSTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
        '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                        For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
        '                            'AG 29/07/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
        '                            'AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
        '                                AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            End If
        '                            'AG 29/07/2014
        '                        Next resultRow
        '                    End If

        '                    'AG 03/10/2013 - final report compact has to show barcode, we need inform the avg results field
        '                    If pCompact Then
        '                        resultData = FillSpecimenIDListForReport(dbConnection, pWorkSessionID, AverageResultsDS)
        '                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                            AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
        '                        End If
        '                    End If
        '                    'AG 03/10/2013

        '                    'AG 01/08/2014 #1897 Finally sort the dataset using the TestPosition column
        '                    'Protection: If no results (for example all tests OUT) exit function returning the espected DS but empty
        '                    If AverageResultsDS.vwksResults.Rows.Count = 0 Then
        '                        'Returned data empty
        '                        resultData.SetDatos = New ResultsDS
        '                    Else

        '                        'AverageResultsDS.vwksResults.DefaultView.Sort = "TestPosition" 'This code does not work!!
        '                        Dim sortedReportList As New List(Of ResultsDS.vwksResultsRow)
        '                        Dim sortedResultsToPrintDS As New ResultsDS
        '                        sortedReportList = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
        '                                      Select a Order By a.TestPosition Ascending).ToList

        '                        For Each reportRow As ResultsDS.vwksResultsRow In sortedReportList
        '                            sortedResultsToPrintDS.vwksResults.ImportRow(reportRow)
        '                        Next
        '                        sortedResultsToPrintDS.vwksResults.AcceptChanges()
        '                        AverageResultsDS = sortedResultsToPrintDS
        '                        sortedReportList = Nothing
        '                        'AG 01/08/2014 #1897

        '                        'Read Reference Range Limits
        '                        Dim MinimunValue As Nullable(Of Single) = Nothing
        '                        Dim MaximunValue As Nullable(Of Single) = Nothing

        '                        Dim myOrderTestsDelegate As New OrderTestsDelegate
        '                        For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
        '                            If (Not resultRow.IsActiveRangeTypeNull) Then
        '                                Dim mySampleType As String = String.Empty
        '                                If (resultRow.TestType <> "CALC") Then mySampleType = resultRow.SampleType

        '                                'Get the Reference Range for the Test/SampleType according the TestType and the Type of Range
        '                                resultData = myOrderTestsDelegate.GetReferenceRangeInterval(dbConnection, resultRow.OrderTestID, resultRow.TestType, _
        '                                                                                            resultRow.TestID, mySampleType, resultRow.ActiveRangeType)

        '                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                    Dim myTestRefRangesDS As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

        '                                    If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
        '                                        MinimunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
        '                                        MaximunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit
        '                                    End If
        '                                End If

        '                                If (MinimunValue.HasValue AndAlso MaximunValue.HasValue) Then
        '                                    If (MinimunValue <> -1 AndAlso MaximunValue <> -1) Then
        '                                        resultRow.NormalLowerLimit = MinimunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                        resultRow.NormalUpperLimit = MaximunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                    End If
        '                                End If
        '                            End If
        '                        Next resultRow

        '                        'Fill Average PatientID and Name fields
        '                        Dim IsOrderProcessed As New Dictionary(Of String, Boolean)
        '                        For Each executionRow As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults.Rows
        '                            If (executionRow.SampleClass = "PATIENT") Then
        '                                If (Not IsOrderProcessed.ContainsKey(executionRow.OrderID)) Then
        '                                    For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
        '                                        If (resultRow.OrderID = executionRow.OrderID) Then
        '                                            resultRow.PatientName = executionRow.PatientName
        '                                            resultRow.PatientID = executionRow.PatientID
        '                                        End If
        '                                    Next resultRow

        '                                    IsOrderProcessed(executionRow.OrderID) = True
        '                                End If
        '                            End If
        '                        Next executionRow

        '                        Dim currentLanguageGlobal As New GlobalBase
        '                        Dim CurrentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage
        '                        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        '                        Dim literalPatientID As String
        '                        literalPatientID = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_PatientID", CurrentLanguage)

        '                        'DL 17/06/2013
        '                        Dim literalPatientName As String
        '                        literalPatientName = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Summary_PatientName", CurrentLanguage)
        '                        'DL 17/06/2013

        '                        Dim literalGender As String
        '                        literalGender = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Gender", CurrentLanguage)

        '                        Dim literalBirthDate As String
        '                        literalBirthDate = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_DateOfBirth", CurrentLanguage)

        '                        Dim literalAge As String
        '                        literalAge = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Age", CurrentLanguage)

        '                        Dim literalPerformedBy As String
        '                        literalPerformedBy = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Patients_PerformedBy", CurrentLanguage)

        '                        Dim literalComments As String
        '                        literalComments = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Flags", CurrentLanguage)  'EF 03/06/2014 (cargar texto LBL_FLags)

        '                        'RH 15/05/2012 Get Patient data
        '                        Dim myPatientsDelegate As New PatientDelegate
        '                        Dim SelectedPatients As List(Of String) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                                  Where row.SampleClass = "PATIENT" _
        '                                                                AndAlso row.OrderToPrint = True _
        '                                                                AndAlso row.RerunNumber = 1 _
        '                                                                Select row.PatientID Distinct).ToList()


        '                        resultData = myPatientsDelegate.GetPatientsForReport(dbConnection, CurrentLanguage, SelectedPatients)
        '                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                            Dim PatientsData As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)

        '                            'JV 31/10/13 #1226 Obtain the OK orders selected by the user
        '                            Dim myDel As New OrderTestsDelegate
        '                            Dim myGDT As GlobalDataTO
        '                            myGDT = myDel.GetOrdersOKByUser(Nothing, pAnalyzerID, pWorkSessionID)
        '                            If (Not myGDT.HasError AndAlso Not myGDT.SetDatos Is Nothing) Then
        '                                Dim OT As OrdersDS = DirectCast(myGDT.SetDatos, OrdersDS)
        '                                Dim SelectedOK As List(Of String) = Enumerable.Cast(Of String)(From row In OT.twksOrders Select row.PatientID Distinct).ToList()
        '                                SelectedPatients = (From p In SelectedPatients Where SelectedOK.Contains(p) Select p).ToList()
        '                            End If
        '                            myDel = Nothing
        '                            myGDT = Nothing
        '                            'JV 31/10/13 #1226

        '                            'RH 24/05/2012 Create info for not registered patients
        '                            For Each PatientID As String In SelectedPatients
        '                                If (From row In PatientsData.tparPatients Where String.Compare(row.PatientID, PatientID, False) = 0 Select row).Count = 0 Then
        '                                    Dim newRow As PatientsDS.tparPatientsRow

        '                                    newRow = PatientsData.tparPatients.NewtparPatientsRow()
        '                                    newRow.PatientID = PatientID
        '                                    PatientsData.tparPatients.AddtparPatientsRow(newRow)
        '                                End If
        '                            Next

        '                            Dim AgeUnitsListDS As New PreloadedMasterDataDS
        '                            Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

        '                            resultData = preloadedMasterConfig.GetList(dbConnection, PreloadedMasterDataEnum.AGE_UNITS)
        '                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                AgeUnitsListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

        '                                Dim ResultsForReportDS As New ResultsDS
        '                                Dim PatientIDList As New List(Of String)
        '                                Dim SamplesList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
        '                                Dim BarcodesByPatient As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) 'AG 22/09/2014 - BA-1940
        '                                Dim StatFlag() As Boolean = {True, False}
        '                                Dim existsRow As Boolean = False
        '                                Dim FullID As String
        '                                Dim FullName As String
        '                                Dim FullGender As String
        '                                Dim FullBirthDate As String
        '                                Dim FullAge As String
        '                                Dim FullPerformedBy As String
        '                                Dim FullComments As String
        '                                Dim Pat As PatientsDS.tparPatientsRow

        '                                Dim linqSpecimenFromResults As List(Of ResultsDS.vwksResultsRow) 'AG 28/06/2013
        '                                'Fill ReportMaster table
        '                                For i As Integer = 0 To 1
        '                                    SamplesList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                  Where row.SampleClass = "PATIENT" _
        '                                                AndAlso row.OrderToPrint = True _
        '                                                AndAlso row.StatFlag = StatFlag(i) _
        '                                                AndAlso row.RerunNumber = 1 _
        '                                                 Select row).ToList()

        '                                    For Each sampleRow As ExecutionsDS.vwksWSExecutionsResultsRow In SamplesList
        '                                        If (SelectedPatients.Contains(sampleRow.PatientID) AndAlso Not PatientIDList.Contains(sampleRow.PatientID)) Then 'If (Not PatientIDList.Contains(sampleRow.PatientID)) Then 'JV 31/10/13 #1226
        '                                            PatientIDList.Add(sampleRow.PatientID)

        '                                            Pat = (From row As PatientsDS.tparPatientsRow In PatientsData.tparPatients _
        '                                                  Where row.PatientID = sampleRow.PatientID _
        '                                                 Select row).First()

        '                                            If (Not Pat.IsDateOfBirthNull) Then
        '                                                Pat.AgeWithUnit = Utilities.GetAgeUnits(Pat.DateOfBirth, AgeUnitsListDS)
        '                                                Pat.FormatedDateOfBirth = Pat.DateOfBirth.ToString(DatePattern)
        '                                            End If

        '                                            'AG 22/09/2014 - BA-1940 'DL 17/06/2013
        '                                            Dim patIDforReport As String = sampleRow.PatientID
        '                                            'If Not sampleRow.IsSpecimenIDListNull Then
        '                                            '    patIDforReport &= " (" & sampleRow.SpecimenIDList & ")"
        '                                            'End If
        '                                            BarcodesByPatient = (From item In SamplesList Where Not item.IsSpecimenIDListNull And item.PatientID = sampleRow.PatientID Select item).ToList
        '                                            If BarcodesByPatient.Count > 0 Then
        '                                                Dim patBarCodeTO As New PatientSpecimenTO
        '                                                For Each item As ExecutionsDS.vwksWSExecutionsResultsRow In BarcodesByPatient
        '                                                    patBarCodeTO.UpdateSpecimenList(item.SpecimenIDList)
        '                                                Next
        '                                                patIDforReport &= patBarCodeTO.GetSpecimenIdListForReports
        '                                            End If
        '                                            'AG 22/09/2014 - BA-1940

        '                                            'EF 30/05/2014 (Separar el título del valor, Cambiar orden de campos Nombre Paciente y mostrar solo si está asignado)
        '                                            'FullID = String.Format("{0}: {1}", literalPatientID, patIDforReport
        '                                            ''FullName = String.Format("{0} {1}", Pat.FirstName, Pat.LastName)
        '                                            'FullName = String.Format("{0}: {1} {2}", literalPatientName, Pat.FirstName, Pat.LastName)
        '                                            ''DL 17/06/2013
        '                                            'FullGender = String.Format("{0}: {1}", literalGender, Pat.Gender)
        '                                            'FullBirthDate = String.Format("{0}: {1}", literalBirthDate, Pat.FormatedDateOfBirth)
        '                                            'FullAge = String.Format("{0}: {1}", literalAge, Pat.AgeWithUnit)
        '                                            'FullPerformedBy = String.Format("{0}: {1}", literalPerformedBy, Pat.PerformedBy)
        '                                            'FullComments = String.Format("{0}: {1}", literalComments, Pat.Comments)

        '                                            FullID = String.Format("{0}", patIDforReport)
        '                                            If (Pat.LastName <> "-" And Pat.LastName <> "") Or (Pat.FirstName <> "-" And Pat.FirstName <> "") Then FullName = String.Format("{0}, {1}", Pat.LastName, Pat.FirstName) Else FullName = ""
        '                                            FullGender = String.Format("{0}", Pat.Gender)
        '                                            FullBirthDate = String.Format("{0}", Pat.FormatedDateOfBirth)
        '                                            FullAge = String.Format("{0}", Pat.AgeWithUnit)
        '                                            FullPerformedBy = String.Format("{0}", Pat.PerformedBy)
        '                                            FullComments = String.Format("{0}", Pat.Comments)
        '                                            'EF 30/05/2014 End

        '                                            ResultsForReportDS.ReportSampleMaster.AddReportSampleMasterRow(sampleRow.PatientID, FullID, _
        '                                                                                                           FullName, FullGender, _
        '                                                                                                           FullBirthDate, FullAge, _
        '                                                                                                           FullPerformedBy, FullComments, DateTime.Now) 'IT 30/07/2014 #BA-1893
        '                                        End If
        '                                    Next sampleRow

        '                                Next i
        '                                linqSpecimenFromResults = Nothing 'AG 28/06/2013
        '                                SamplesList = Nothing 'AG 22/09/2014 - BA-1940
        '                                BarcodesByPatient = Nothing 'AG 23/09/2014 - BA-1940

        '                                'Fill ReportDetails table
        '                                Dim DetailPatientID As String
        '                                Dim TestName As String
        '                                Dim SampleType As String
        '                                Dim ReplicateNumber As String
        '                                Dim ABSValue As String
        '                                Dim CONC_Value As String
        '                                Dim ReferenceRanges As String
        '                                Dim Unit As String
        '                                Dim ResultDate As String
        '                                Dim Flags As String  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)

        '                                Dim tmpOrderTestId As Integer
        '                                Dim IsAverageDone As Dictionary(Of String, Boolean)
        '                                Dim AverageList As List(Of ResultsDS.vwksResultsRow)

        '                                Dim myOrderTestID As Integer
        '                                Dim maxTheoreticalConc As Single
        '                                Dim Filter As String

        '                                For Each SampleID As String In PatientIDList
        '                                    tmpOrderTestId = -1
        '                                    IsAverageDone = New Dictionary(Of String, Boolean)
        '                                    AverageList = New List(Of ResultsDS.vwksResultsRow)
        '                                    'Filter = String.Empty

        '                                    For Each row As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
        '                                        If (row.PatientID = SampleID) Then
        '                                            If (String.Compare(row.TestType, "STD", False) = 0) Then
        '                                                myOrderTestID = row.OrderTestID
        '                                                maxTheoreticalConc = (From resultRow In AverageResultsDS.vwksResults _
        '                                                                     Where resultRow.OrderTestID = myOrderTestID _
        '                                                                    Select resultRow.TheoricalConcentration).Max

        '                                                If (row.TheoricalConcentration = maxTheoreticalConc) Then
        '                                                    AverageList.Add(row)
        '                                                End If

        '                                                'AG 01/12/2010
        '                                            ElseIf (row.TestType = "ISE" OrElse row.TestType = "OFFS") Then
        '                                                AverageList.Add(row)
        '                                                'End AG 01/12/2010

        '                                            Else 'CALC
        '                                                AverageList.Add(row)
        '                                            End If
        '                                        End If
        '                                    Next row

        '                                    'TR 27/05/2013 -Get a list of sample types separated by commas
        '                                    Dim SampleTypes() As String
        '                                    Dim myMasterDataDelegate As New MasterDataDelegate

        '                                    resultData = myMasterDataDelegate.GetSampleTypes(dbConnection)
        '                                    If Not resultData.HasError Then
        '                                        SampleTypes = resultData.SetDatos.ToString.Split(CChar(","))
        '                                    End If

        '                                    'RH 18/05/2012 Sort by SampleType
        '                                    'Dim SampleTypes() As String = {"SER", "URI", "PLM", "WBL", "CSF", "LIQ", "SEM"}
        '                                    For Each sortedSampleType In SampleTypes
        '                                        Dim NewAverageList As List(Of ResultsDS.vwksResultsRow) = (From row In AverageList _
        '                                                                                                  Where String.Compare(row.SampleType, sortedSampleType, False) = 0 _
        '                                                                                                 Select row).ToList()

        '                                        For Each resultRow As ResultsDS.vwksResultsRow In NewAverageList
        '                                            Filter = String.Format("{0}{1}", resultRow.OrderTestID, resultRow.RerunNumber)

        '                                            'Insert only Accepted Results
        '                                            If (resultRow.AcceptedResultFlag AndAlso Not IsAverageDone.ContainsKey(Filter)) Then
        '                                                IsAverageDone(Filter) = True
        '                                                DetailPatientID = resultRow.PatientID

        '                                                'AG 29/04/2014 - #1608
        '                                                'TestName = resultRow.TestName
        '                                                If Not resultRow.IsTestLongNameNull Then TestName = resultRow.TestLongName Else TestName = resultRow.TestName

        '                                                SampleType = resultRow.SampleType
        '                                                ReplicateNumber = String.Empty

        '                                                If (Not resultRow.IsABSValueNull) Then
        '                                                    ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
        '                                                Else
        '                                                    ABSValue = String.Empty
        '                                                End If

        '                                                If (Not resultRow.IsCONC_ValueNull) Then
        '                                                    Dim hasConcentrationError As Boolean = False

        '                                                    If (Not resultRow.IsCONC_ErrorNull) Then
        '                                                        hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
        '                                                    End If

        '                                                    If (Not hasConcentrationError) Then
        '                                                        CONC_Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                                    Else
        '                                                        CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                    End If
        '                                                Else
        '                                                    If (Not resultRow.IsManualResultTextNull) Then
        '                                                        CONC_Value = resultRow.ManualResultText
        '                                                    Else
        '                                                        CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                    End If
        '                                                End If

        '                                                Unit = resultRow.MeasureUnit

        '                                                If (Not String.IsNullOrEmpty(resultRow.NormalLowerLimit) AndAlso _
        '                                                    Not String.IsNullOrEmpty(resultRow.NormalUpperLimit)) Then
        '                                                    ReferenceRanges = String.Format("{0} - {1}", resultRow.NormalLowerLimit, resultRow.NormalUpperLimit)
        '                                                Else
        '                                                    ReferenceRanges = String.Empty
        '                                                End If

        '                                                'AG 15/09/2010 - Special case when Absorbance has error
        '                                                If (Not resultRow.IsABS_ErrorNull) Then
        '                                                    If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
        '                                                        ABSValue = GlobalConstants.ABSORBANCE_ERROR
        '                                                        CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
        '                                                    End If
        '                                                End If
        '                                                'END AG 15/09/2010

        '                                                'EF 03/06/2014 - Provisional v3.0.1 (PENDIENTE hacer un metodo único que devuelva el valor del FLAG a todos los reports, pantallas, etc y reutilizar!!)
        '                                                'RH 16/05/2012
        '                                                Flags = String.Empty
        '                                                '    'Verify if the result is out of the limits of the NORMALITY REFERENCE RANGE
        '                                                If ((Not resultRow.IsActiveRangeTypeNull AndAlso Not String.Compare(resultRow.ActiveRangeType, String.Empty, False) = 0) AndAlso _
        '                                                    (IsNumeric(CONC_Value))) Then
        '                                                    If (Not resultRow.IsNormalLowerLimitNull AndAlso Not resultRow.IsNormalUpperLimitNull) Then
        '                                                        If (CSng(CONC_Value) < CSng(resultRow.NormalLowerLimit)) Then
        '                                                            Flags = GlobalConstants.LOW '"L"
        '                                                        ElseIf (CSng(CONC_Value) > CSng(resultRow.NormalUpperLimit)) Then
        '                                                            Flags = GlobalConstants.HIGH '"H"
        '                                                        End If
        '                                                    End If
        '                                                End If
        '                                                'RH 16/05/2012 - END
        '                                                'If there are Panic Ranges informed, then verify if the result is out of the limits of the PANIC RANGE
        '                                                If IsNumeric(CONC_Value) And (Not resultRow.IsPanicLowerLimitNull AndAlso Not resultRow.IsPanicUpperLimitNull) Then
        '                                                    If (CSng(CONC_Value) < CSng(resultRow.PanicLowerLimit)) Then
        '                                                        Flags = GlobalConstants.PANIC_LOW  'PL
        '                                                    ElseIf (CSng(resultRow.PanicUpperLimit) < CSng(CONC_Value)) Then
        '                                                        Flags = GlobalConstants.PANIC_HIGH 'PH 
        '                                                    End If
        '                                                End If
        '                                                'EF 03/06/2014 END

        '                                                ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & _
        '                                                             resultRow.ResultDateTime.ToString(TimePattern)

        '                                                'AG 03/10/2013 - compact report shows the barcode here because there is not header
        '                                                If pCompact Then
        '                                                    If Not resultRow.IsSpecimenIDListNull Then
        '                                                        DetailPatientID &= " (" & resultRow.SpecimenIDList & ")"
        '                                                    End If
        '                                                End If
        '                                                'AG 03/10/2013

        '                                                'Insert Details row
        '                                                ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow(resultRow.PatientID, DetailPatientID, TestName, SampleType, _
        '                                                                                                                 ReplicateNumber, ABSValue, CONC_Value, _
        '                                                                                                                 ReferenceRanges, Unit, ResultDate, _
        '                                                                                                                 Flags)  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)

        '                                                tmpOrderTestId = resultRow.OrderTestID
        '                                                Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010

        '                                                Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                                                                                     Where row.OrderTestID = tmpOrderTestId _
        '                                                                                                                   AndAlso row.RerunNumber = myRerunNumber _
        '                                                                                                                    Select row).ToList()

        '                                                If (GetReplicates AndAlso (SampleList.Count > 0)) Then
        '                                                    Flags = String.Empty  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)
        '                                                    TestName = String.Empty
        '                                                    SampleType = String.Empty
        '                                                    ReferenceRanges = String.Empty

        '                                                    For j As Integer = 0 To SampleList.Count - 1
        '                                                        DetailPatientID = SampleList(j).PatientID
        '                                                        ReplicateNumber = SampleList(j).ReplicateNumber.ToString()
        '                                                        Unit = resultRow.MeasureUnit

        '                                                        'AG 02/08/2010
        '                                                        Dim hasConcentrationError As Boolean = False
        '                                                        If (Not SampleList(j).IsCONC_ErrorNull) Then 'RH 14/09/2010
        '                                                            hasConcentrationError = Not String.IsNullOrEmpty(SampleList(j).CONC_Error)
        '                                                        End If

        '                                                        If (Not hasConcentrationError) Then
        '                                                            If (Not SampleList(j).IsCONC_ValueNull) Then
        '                                                                CONC_Value = SampleList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                                            ElseIf (Not resultRow.IsManualResultTextNull) Then
        '                                                                'Take the Manual Result text from the average result
        '                                                                CONC_Value = resultRow.ManualResultText
        '                                                            Else
        '                                                                CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                            End If
        '                                                        Else
        '                                                            CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                        End If
        '                                                        'END AG 02/08/2010

        '                                                        'JV 17/12/2013 #1184 - INI
        '                                                        If (Not SampleList(j).IsABS_ValueNull) Then
        '                                                            ABSValue = SampleList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
        '                                                        Else
        '                                                            ABSValue = String.Empty
        '                                                        End If
        '                                                        'JV 17/12/2013 #1184 - END

        '                                                        'AG 15/09/2010 - Special case when Absorbance has error
        '                                                        If (Not SampleList(j).IsABS_ErrorNull) Then
        '                                                            If (Not String.IsNullOrEmpty(SampleList(j).ABS_Error)) Then
        '                                                                ABSValue = GlobalConstants.ABSORBANCE_ERROR
        '                                                                CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
        '                                                            End If
        '                                                        End If
        '                                                        'END AG 15/09/2010

        '                                                        ResultDate = SampleList(j).ResultDate.ToString(DatePattern) & " " & _
        '                                                                     SampleList(j).ResultDate.ToString(TimePattern)

        '                                                        'AG 03/10/2013 - compact report shows the barcode here because there is not header
        '                                                        If pCompact Then
        '                                                            If Not SampleList(j).IsSpecimenIDListNull Then
        '                                                                DetailPatientID &= " (" & SampleList(j).SpecimenIDList & ")"
        '                                                            End If
        '                                                        End If
        '                                                        'AG 03/10/2013

        '                                                        'Insert Details row
        '                                                        ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow(resultRow.PatientID, DetailPatientID, TestName, _
        '                                                                                                                         SampleType, ReplicateNumber, _
        '                                                                                                                         ABSValue, CONC_Value, _
        '                                                                                                                         ReferenceRanges, Unit, _
        '                                                                                                                         ResultDate, Flags)  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)
        '                                                    Next
        '                                                End If
        '                                            End If
        '                                        Next
        '                                    Next
        '                                Next

        '                                resultData.SetDatos = ResultsForReportDS
        '                            End If
        '                        End If

        '                    End If 'AG 10/09/2014 -BA-1894 , BA-1897 - Protection End If

        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsByPatientSampleForReport", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' 25/09/2013 - CF v2.1.1 - Original code from GetResultsByPatientSampleForReport function. 
        ' ''' Get Results info by Patient Sample for Report using a List of orderIds as a filter. 
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ' ''' <param name="pOrderList">List of orders to filter the results by</param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (ReportMaster and ReportDetails tables)</returns>
        ' ''' <remarks>
        ' ''' Created by:  RH 03/01/2012
        ' ''' Modified by: RH 19/03/2012 Label PatientID in Multilanguage
        ' '''              AG 27/05/2013 - Add new samples types LIQ and SER
        ' '''              CF 25/09/2013 - Copied the GetResultsByPatientSampleForReport function and added the orderList param. 
        ' '''              AG 03/10/2013 - new parameter compact that will fill the field FullID without multilanguage resources
        ' '''              AG 29/07/2014 - #1894 (tests that form part of a calculated test must be excluded from final report depends on the CALC test programming)
        ' '''              AG 01/08/2014 #1897 fix issue Test order for patient report (final and compact) from current WS results (when results of different test type are mixed)
        ' '''              AG 23/09/2014 - BA-1940 in report header show all patient's different barcodes
        ' '''              AG 23/09/2014 - BA-1940 comment old code and implement a unique method compatible manual and auto firal report (GetResultsByPatientSampleForReportByOrderList)
        ' ''' </remarks>
        'Public Function GetResultsByPatientSampleForReportByOrderList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                                   ByVal pWorkSessionID As String, ByVal pOrderList As List(Of String), _
        '                                                   Optional ByVal GetReplicates As Boolean = True, Optional ByVal pCompact As Boolean = False) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myExecutionDelegate As New ExecutionsDelegate

        '                resultData = myExecutionDelegate.GetWSExecutionsResultsByOrderIDList(dbConnection, pAnalyzerID, pWorkSessionID, pOrderList)
        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    Dim ExecutionsResultsDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

        '                    'Get the list of different OrderTestIDs in the group of Executions
        '                    Dim OrderTestList As List(Of Integer) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                            Where row.OrderToPrint = True AndAlso row.SampleClass = "PATIENT" _
        '                                                         Order By row.TestPosition _
        '                                                           Select row.OrderTestID Distinct).ToList()

        '                    'AG 29/07/2014 - #1894 Get all orderTests that form part of a calculated test that has to be excluded from patients final report
        '                    Dim orderCalcDelg As New OrderCalculatedTestsDelegate
        '                    Dim toExcludeFromReport As New List(Of Integer) 'Order tests that form part of a calculated test programmed to not print the partial tests
        '                    resultData = orderCalcDelg.GetOrderTestsToExcludeInPatientsReport(dbConnection)
        '                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                        toExcludeFromReport = DirectCast(resultData.SetDatos, List(Of Integer))

        '                        'Remove these orderTest from OrderTestList of STD tests
        '                        If toExcludeFromReport.Count > 0 Then
        '                            Dim positionToDelete As Integer = 0
        '                            For Each item As Integer In toExcludeFromReport
        '                                If OrderTestList.Contains(item) Then
        '                                    positionToDelete = OrderTestList.IndexOf(item)
        '                                    OrderTestList.RemoveAt(positionToDelete)
        '                                End If
        '                            Next
        '                        End If
        '                    End If
        '                    'AG 29/07/2014

        '                    'Get the Patient Results for the list of OrderTestIDs
        '                    Dim AverageResultsDS As New ResultsDS
        '                    Dim myResultsDelegate As New ResultsDelegate

        '                    resultData = GetResultsForReports(dbConnection, OrderTestList)
        '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                        AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
        '                    End If

        '                    'Get Calculated Results
        '                    'AG 01/08/2014 #1897 add new optional parameter to TRUE
        '                    resultData = myResultsDelegate.GetCalculatedTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
        '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                        For Each resultRow As ResultsDS.vwksResultsRow In DirectCast(resultData.SetDatos, ResultsDS).vwksResults.Rows
        '                            'AG 29/07/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
        '                            'AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
        '                                AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            End If
        '                            'AG 29/07/2014
        '                        Next
        '                    End If

        '                    'Get ISE & OffSystem Tests Results
        '                    'AG 01/08/2014 #1897 add new optional parameter to TRUE
        '                    resultData = myResultsDelegate.GetISEOFFSTestResults(dbConnection, pAnalyzerID, pWorkSessionID, True)
        '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                        For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
        '                            'AG 29/07/2014 - #1894 exclude CALC tests that form part of another calculated test programmed to not print the partial tests
        '                            'AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            If Not toExcludeFromReport.Contains(resultRow.OrderTestID) Then
        '                                AverageResultsDS.vwksResults.ImportRow(resultRow)
        '                            End If
        '                            'AG 29/07/2014
        '                        Next resultRow
        '                    End If

        '                    'AG 03/10/2013 - final report compact has to show barcode, we need inform the avg results field
        '                    If pCompact Then
        '                        resultData = FillSpecimenIDListForReport(dbConnection, pWorkSessionID, AverageResultsDS)
        '                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                            AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
        '                        End If
        '                    End If
        '                    'AG 03/10/2013

        '                    'AG 01/08/2014 #1897 Finally sort the dataset using the TestPosition column
        '                    'Protection: If no results (for example all tests OUT) exit function returning the espected DS but empty
        '                    If AverageResultsDS.vwksResults.Rows.Count = 0 Then
        '                        'Returned data empty
        '                        resultData.SetDatos = New ResultsDS
        '                    Else

        '                        'AverageResultsDS.vwksResults.DefaultView.Sort = "TestPosition" 'This code does not work!!
        '                        Dim sortedReportList As New List(Of ResultsDS.vwksResultsRow)
        '                        Dim sortedResultsToPrintDS As New ResultsDS
        '                        sortedReportList = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
        '                                      Select a Order By a.TestPosition Ascending).ToList

        '                        For Each reportRow As ResultsDS.vwksResultsRow In sortedReportList
        '                            sortedResultsToPrintDS.vwksResults.ImportRow(reportRow)
        '                        Next
        '                        sortedResultsToPrintDS.vwksResults.AcceptChanges()
        '                        AverageResultsDS = sortedResultsToPrintDS
        '                        sortedReportList = Nothing
        '                        'AG 01/08/2014 #1897

        '                        'Read Reference Range Limits
        '                        Dim MinimunValue As Nullable(Of Single) = Nothing
        '                        Dim MaximunValue As Nullable(Of Single) = Nothing

        '                        Dim myOrderTestsDelegate As New OrderTestsDelegate
        '                        For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
        '                            If (Not resultRow.IsActiveRangeTypeNull) Then
        '                                Dim mySampleType As String = String.Empty
        '                                If (resultRow.TestType <> "CALC") Then mySampleType = resultRow.SampleType

        '                                'Get the Reference Range for the Test/SampleType according the TestType and the Type of Range
        '                                resultData = myOrderTestsDelegate.GetReferenceRangeInterval(dbConnection, resultRow.OrderTestID, resultRow.TestType, _
        '                                                                                            resultRow.TestID, mySampleType, resultRow.ActiveRangeType)

        '                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                    Dim myTestRefRangesDS As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

        '                                    If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
        '                                        MinimunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
        '                                        MaximunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit
        '                                    End If
        '                                End If

        '                                If (MinimunValue.HasValue AndAlso MaximunValue.HasValue) Then
        '                                    If (MinimunValue <> -1 AndAlso MaximunValue <> -1) Then
        '                                        resultRow.NormalLowerLimit = MinimunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                        resultRow.NormalUpperLimit = MaximunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                    End If
        '                                End If
        '                            End If
        '                        Next resultRow

        '                        'Fill Average PatientID and Name fields
        '                        Dim IsOrderProcessed As New Dictionary(Of String, Boolean)
        '                        For Each executionRow As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults.Rows
        '                            If (executionRow.SampleClass = "PATIENT") Then
        '                                If (Not IsOrderProcessed.ContainsKey(executionRow.OrderID)) Then
        '                                    For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
        '                                        If (resultRow.OrderID = executionRow.OrderID) Then
        '                                            resultRow.PatientName = executionRow.PatientName
        '                                            resultRow.PatientID = executionRow.PatientID
        '                                        End If
        '                                    Next resultRow

        '                                    IsOrderProcessed(executionRow.OrderID) = True
        '                                End If
        '                            End If
        '                        Next executionRow

        '                        Dim currentLanguageGlobal As New GlobalBase
        '                        Dim CurrentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage
        '                        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        '                        Dim literalPatientID As String
        '                        literalPatientID = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_PatientID", CurrentLanguage)

        '                        'DL 17/06/2013
        '                        Dim literalPatientName As String
        '                        literalPatientName = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Summary_PatientName", CurrentLanguage)
        '                        'DL 17/06/2013

        '                        Dim literalGender As String
        '                        literalGender = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Gender", CurrentLanguage)

        '                        Dim literalBirthDate As String
        '                        literalBirthDate = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_DateOfBirth", CurrentLanguage)

        '                        Dim literalAge As String
        '                        literalAge = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Age", CurrentLanguage)

        '                        Dim literalPerformedBy As String
        '                        literalPerformedBy = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Patients_PerformedBy", CurrentLanguage)

        '                        Dim literalComments As String
        '                        literalComments = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Flags", CurrentLanguage)  'EF 03/06/2014 (cargar texto LBL_FLags)

        '                        'RH 15/05/2012 Get Patient data
        '                        Dim myPatientsDelegate As New PatientDelegate
        '                        Dim SelectedPatients As List(Of String) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                                  Where row.SampleClass = "PATIENT" _
        '                                                                AndAlso row.OrderToPrint = True _
        '                                                                AndAlso row.RerunNumber = 1 _
        '                                                                 Select row.PatientID Distinct).ToList()

        '                        resultData = myPatientsDelegate.GetPatientsForReport(dbConnection, CurrentLanguage, SelectedPatients)
        '                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                            Dim PatientsData As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)

        '                            'RH 24/05/2012 Create info for not registered patients
        '                            For Each PatientID As String In SelectedPatients
        '                                If (From row In PatientsData.tparPatients Where String.Compare(row.PatientID, PatientID, False) = 0 Select row).Count = 0 Then
        '                                    Dim newRow As PatientsDS.tparPatientsRow

        '                                    newRow = PatientsData.tparPatients.NewtparPatientsRow()
        '                                    newRow.PatientID = PatientID
        '                                    PatientsData.tparPatients.AddtparPatientsRow(newRow)
        '                                End If
        '                            Next

        '                            Dim AgeUnitsListDS As New PreloadedMasterDataDS
        '                            Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

        '                            resultData = preloadedMasterConfig.GetList(dbConnection, PreloadedMasterDataEnum.AGE_UNITS)
        '                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                AgeUnitsListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

        '                                Dim ResultsForReportDS As New ResultsDS
        '                                Dim PatientIDList As New List(Of String)
        '                                Dim SamplesList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
        '                                Dim BarcodesByPatient As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) 'AG 23/09/2014 - BA-1940
        '                                Dim StatFlag() As Boolean = {True, False}
        '                                Dim existsRow As Boolean = False
        '                                Dim FullID As String
        '                                Dim FullName As String
        '                                Dim FullGender As String
        '                                Dim FullBirthDate As String
        '                                Dim FullAge As String
        '                                Dim FullPerformedBy As String
        '                                Dim FullComments As String
        '                                Dim Pat As PatientsDS.tparPatientsRow

        '                                Dim linqSpecimenFromResults As List(Of ResultsDS.vwksResultsRow) 'AG 28/06/2013
        '                                'Fill ReportMaster table
        '                                For i As Integer = 0 To 1
        '                                    SamplesList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                  Where row.SampleClass = "PATIENT" _
        '                                                AndAlso row.OrderToPrint = True _
        '                                                AndAlso row.StatFlag = StatFlag(i) _
        '                                                AndAlso row.RerunNumber = 1 _
        '                                                 Select row).ToList()

        '                                    For Each sampleRow As ExecutionsDS.vwksWSExecutionsResultsRow In SamplesList
        '                                        If (Not PatientIDList.Contains(sampleRow.PatientID)) Then
        '                                            PatientIDList.Add(sampleRow.PatientID)

        '                                            Pat = (From row As PatientsDS.tparPatientsRow In PatientsData.tparPatients _
        '                                                  Where row.PatientID = sampleRow.PatientID _
        '                                                 Select row).First()

        '                                            If (Not Pat.IsDateOfBirthNull) Then
        '                                                Pat.AgeWithUnit = Utilities.GetAgeUnits(Pat.DateOfBirth, AgeUnitsListDS)
        '                                                Pat.FormatedDateOfBirth = Pat.DateOfBirth.ToString(DatePattern)
        '                                            End If

        '                                            'AG 23/09/2014 - BA-1940 'DL 17/06/2013
        '                                            Dim patIDforReport As String = sampleRow.PatientID
        '                                            'If Not sampleRow.IsSpecimenIDListNull Then
        '                                            '    patIDforReport &= " (" & sampleRow.SpecimenIDList & ")"
        '                                            'End If
        '                                            BarcodesByPatient = (From item In SamplesList Where Not item.IsSpecimenIDListNull And item.PatientID = sampleRow.PatientID Select item).ToList
        '                                            If BarcodesByPatient.Count > 0 Then
        '                                                Dim patBarCodeTO As New PatientSpecimenTO
        '                                                For Each item As ExecutionsDS.vwksWSExecutionsResultsRow In BarcodesByPatient
        '                                                    patBarCodeTO.UpdateSpecimenList(item.SpecimenIDList)
        '                                                Next
        '                                                patIDforReport &= patBarCodeTO.GetSpecimenIdListForReports
        '                                            End If
        '                                            'AG 23/09/2014 - BA-1940

        '                                            ' XB 10/07/2014 - kill repeated lables - #1673
        '                                            'FullID = String.Format("{0}: {1}", literalPatientID, patIDforReport)

        '                                            ''FullName = String.Format("{0} {1}", Pat.FirstName, Pat.LastName)
        '                                            'FullName = String.Format("{0}: {1} {2}", literalPatientName, Pat.FirstName, Pat.LastName)
        '                                            ''DL 17/06/2013
        '                                            'FullGender = String.Format("{0}: {1}", literalGender, Pat.Gender)
        '                                            'FullBirthDate = String.Format("{0}: {1}", literalBirthDate, Pat.FormatedDateOfBirth)
        '                                            'FullAge = String.Format("{0}: {1}", literalAge, Pat.AgeWithUnit)
        '                                            'FullPerformedBy = String.Format("{0}: {1}", literalPerformedBy, Pat.PerformedBy)
        '                                            'FullComments = String.Format("{0}: {1}", literalComments, Pat.Comments)

        '                                            FullID = String.Format("{0}", patIDforReport)
        '                                            If (Pat.LastName <> "-" And Pat.LastName <> "") Or (Pat.FirstName <> "-" And Pat.FirstName <> "") Then FullName = String.Format("{0}, {1}", Pat.LastName, Pat.FirstName) Else FullName = ""
        '                                            FullGender = String.Format("{0}", Pat.Gender)
        '                                            FullBirthDate = String.Format("{0}", Pat.FormatedDateOfBirth)
        '                                            FullAge = String.Format("{0}", Pat.AgeWithUnit)
        '                                            FullPerformedBy = String.Format("{0}", Pat.PerformedBy)
        '                                            FullComments = String.Format("{0}", Pat.Comments)
        '                                            ' XB 10/07/2014 - #1673

        '                                            ResultsForReportDS.ReportSampleMaster.AddReportSampleMasterRow(sampleRow.PatientID, FullID, _
        '                                                                                                           FullName, FullGender, _
        '                                                                                                           FullBirthDate, FullAge, _
        '                                                                                                           FullPerformedBy, FullComments, DateTime.Now) 'IT 30/07/2014 #BA-1893
        '                                        End If
        '                                    Next sampleRow

        '                                Next i
        '                                linqSpecimenFromResults = Nothing 'AG 28/06/2013
        '                                SamplesList = Nothing 'AG 23/09/2014 - BA-1940
        '                                BarcodesByPatient = Nothing 'AG 23/09/2014 - BA-1940

        '                                'Fill ReportDetails table
        '                                Dim DetailPatientID As String
        '                                Dim TestName As String
        '                                Dim SampleType As String
        '                                Dim ReplicateNumber As String
        '                                Dim ABSValue As String
        '                                Dim CONC_Value As String
        '                                Dim ReferenceRanges As String
        '                                Dim Unit As String
        '                                Dim ResultDate As String
        '                                Dim Flags As String  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)

        '                                Dim tmpOrderTestId As Integer
        '                                Dim IsAverageDone As Dictionary(Of String, Boolean)
        '                                Dim AverageList As List(Of ResultsDS.vwksResultsRow)

        '                                Dim myOrderTestID As Integer
        '                                Dim maxTheoreticalConc As Single
        '                                Dim Filter As String

        '                                For Each SampleID As String In PatientIDList
        '                                    tmpOrderTestId = -1
        '                                    IsAverageDone = New Dictionary(Of String, Boolean)
        '                                    AverageList = New List(Of ResultsDS.vwksResultsRow)
        '                                    'Filter = String.Empty

        '                                    For Each row As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
        '                                        If (row.PatientID = SampleID) Then
        '                                            If (String.Compare(row.TestType, "STD", False) = 0) Then
        '                                                myOrderTestID = row.OrderTestID
        '                                                maxTheoreticalConc = (From resultRow In AverageResultsDS.vwksResults _
        '                                                                     Where resultRow.OrderTestID = myOrderTestID _
        '                                                                    Select resultRow.TheoricalConcentration).Max

        '                                                If (row.TheoricalConcentration = maxTheoreticalConc) Then
        '                                                    AverageList.Add(row)
        '                                                End If

        '                                                'AG 01/12/2010
        '                                            ElseIf (row.TestType = "ISE" OrElse row.TestType = "OFFS") Then
        '                                                AverageList.Add(row)
        '                                                'End AG 01/12/2010

        '                                            Else 'CALC
        '                                                AverageList.Add(row)
        '                                            End If
        '                                        End If
        '                                    Next row

        '                                    'TR 27/05/2013 -Get a list of sample types separated by commas
        '                                    Dim SampleTypes() As String
        '                                    Dim myMasterDataDelegate As New MasterDataDelegate

        '                                    resultData = myMasterDataDelegate.GetSampleTypes(dbConnection)
        '                                    If Not resultData.HasError Then
        '                                        SampleTypes = resultData.SetDatos.ToString.Split(CChar(","))
        '                                    End If

        '                                    'RH 18/05/2012 Sort by SampleType
        '                                    'Dim SampleTypes() As String = {"SER", "URI", "PLM", "WBL", "CSF", "LIQ", "SEM"}
        '                                    For Each sortedSampleType In SampleTypes
        '                                        Dim NewAverageList As List(Of ResultsDS.vwksResultsRow) = (From row In AverageList _
        '                                                                                                  Where String.Compare(row.SampleType, sortedSampleType, False) = 0 _
        '                                                                                                 Select row).ToList()

        '                                        For Each resultRow As ResultsDS.vwksResultsRow In NewAverageList
        '                                            Filter = String.Format("{0}{1}", resultRow.OrderTestID, resultRow.RerunNumber)

        '                                            'Insert only Accepted Results
        '                                            If (resultRow.AcceptedResultFlag AndAlso Not IsAverageDone.ContainsKey(Filter)) Then
        '                                                IsAverageDone(Filter) = True
        '                                                DetailPatientID = resultRow.PatientID

        '                                                'AG 29/04/2014 - #1608
        '                                                'TestName = resultRow.TestName
        '                                                If Not resultRow.IsTestLongNameNull Then TestName = resultRow.TestLongName Else TestName = resultRow.TestName

        '                                                SampleType = resultRow.SampleType
        '                                                ReplicateNumber = String.Empty

        '                                                If (Not resultRow.IsABSValueNull) Then
        '                                                    ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
        '                                                Else
        '                                                    ABSValue = String.Empty
        '                                                End If

        '                                                If (Not resultRow.IsCONC_ValueNull) Then
        '                                                    Dim hasConcentrationError As Boolean = False

        '                                                    If (Not resultRow.IsCONC_ErrorNull) Then
        '                                                        hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
        '                                                    End If

        '                                                    If (Not hasConcentrationError) Then
        '                                                        CONC_Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                                    Else
        '                                                        CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                    End If
        '                                                Else
        '                                                    If (Not resultRow.IsManualResultTextNull) Then
        '                                                        CONC_Value = resultRow.ManualResultText
        '                                                    Else
        '                                                        CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                    End If
        '                                                End If

        '                                                Unit = resultRow.MeasureUnit

        '                                                If (Not String.IsNullOrEmpty(resultRow.NormalLowerLimit) AndAlso _
        '                                                    Not String.IsNullOrEmpty(resultRow.NormalUpperLimit)) Then
        '                                                    ReferenceRanges = String.Format("{0} - {1}", resultRow.NormalLowerLimit, resultRow.NormalUpperLimit)
        '                                                Else
        '                                                    ReferenceRanges = String.Empty
        '                                                End If

        '                                                'AG 15/09/2010 - Special case when Absorbance has error
        '                                                If (Not resultRow.IsABS_ErrorNull) Then
        '                                                    If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
        '                                                        ABSValue = GlobalConstants.ABSORBANCE_ERROR
        '                                                        CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
        '                                                    End If
        '                                                End If
        '                                                'END AG 15/09/2010

        '                                                'EF 03/06/2014 - Provisional v3.0.1 (PENDIENTE hacer un metodo único que devuelva el valor del FLAG a todos los reports, pantallas, etc y reutilizar!!)
        '                                                'RH 16/05/2012
        '                                                Flags = String.Empty
        '                                                '    'Verify if the result is out of the limits of the NORMALITY REFERENCE RANGE
        '                                                If ((Not resultRow.IsActiveRangeTypeNull AndAlso Not String.Compare(resultRow.ActiveRangeType, String.Empty, False) = 0) AndAlso _
        '                                                    (IsNumeric(CONC_Value))) Then
        '                                                    If (Not resultRow.IsNormalLowerLimitNull AndAlso Not resultRow.IsNormalUpperLimitNull) Then
        '                                                        If (CSng(CONC_Value) < CSng(resultRow.NormalLowerLimit)) Then
        '                                                            Flags = GlobalConstants.LOW '"L"
        '                                                        ElseIf (CSng(CONC_Value) > CSng(resultRow.NormalUpperLimit)) Then
        '                                                            Flags = GlobalConstants.HIGH '"H"
        '                                                        End If
        '                                                    End If
        '                                                End If
        '                                                'RH 16/05/2012 - END
        '                                                'If there are Panic Ranges informed, then verify if the result is out of the limits of the PANIC RANGE
        '                                                If IsNumeric(CONC_Value) And (Not resultRow.IsPanicLowerLimitNull AndAlso Not resultRow.IsPanicUpperLimitNull) Then
        '                                                    If (CSng(CONC_Value) < CSng(resultRow.PanicLowerLimit)) Then
        '                                                        Flags = GlobalConstants.PANIC_LOW  'PL
        '                                                    ElseIf (CSng(resultRow.PanicUpperLimit) < CSng(CONC_Value)) Then
        '                                                        Flags = GlobalConstants.PANIC_HIGH 'PH 
        '                                                    End If
        '                                                End If
        '                                                'EF 03/06/2014 END

        '                                                ResultDate = resultRow.ResultDateTime.ToString(DatePattern) & " " & _
        '                                                             resultRow.ResultDateTime.ToString(TimePattern)

        '                                                'AG 03/10/2013 - compact report shows the barcode here because there is not header
        '                                                If pCompact Then
        '                                                    If Not resultRow.IsSpecimenIDListNull Then
        '                                                        DetailPatientID &= " (" & resultRow.SpecimenIDList & ")"
        '                                                    End If
        '                                                End If
        '                                                'AG 03/10/2013

        '                                                'Insert Details row
        '                                                ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow(resultRow.PatientID, DetailPatientID, TestName, SampleType, _
        '                                                                                                                 ReplicateNumber, ABSValue, CONC_Value, _
        '                                                                                                                 ReferenceRanges, Unit, ResultDate, _
        '                                                                                                                 Flags)  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)

        '                                                tmpOrderTestId = resultRow.OrderTestID
        '                                                Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010

        '                                                Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
        '                                                                                                                     Where row.OrderTestID = tmpOrderTestId _
        '                                                                                                                   AndAlso row.RerunNumber = myRerunNumber _
        '                                                                                                                    Select row).ToList()

        '                                                If (GetReplicates AndAlso (SampleList.Count > 0)) Then
        '                                                    Flags = String.Empty  'EF 03/06/2014 (cambio nombre variable 'remarks' a FLAGS)
        '                                                    TestName = String.Empty
        '                                                    SampleType = String.Empty
        '                                                    ReferenceRanges = String.Empty

        '                                                    For j As Integer = 0 To SampleList.Count - 1
        '                                                        DetailPatientID = SampleList(j).PatientID
        '                                                        ReplicateNumber = SampleList(j).ReplicateNumber.ToString()
        '                                                        Unit = resultRow.MeasureUnit

        '                                                        'AG 02/08/2010
        '                                                        Dim hasConcentrationError As Boolean = False
        '                                                        If (Not SampleList(j).IsCONC_ErrorNull) Then 'RH 14/09/2010
        '                                                            hasConcentrationError = Not String.IsNullOrEmpty(SampleList(j).CONC_Error)
        '                                                        End If

        '                                                        If (Not hasConcentrationError) Then
        '                                                            If (Not SampleList(j).IsCONC_ValueNull) Then
        '                                                                CONC_Value = SampleList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
        '                                                            ElseIf (Not resultRow.IsManualResultTextNull) Then
        '                                                                'Take the Manual Result text from the average result
        '                                                                CONC_Value = resultRow.ManualResultText
        '                                                            Else
        '                                                                CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                            End If
        '                                                        Else
        '                                                            CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
        '                                                        End If
        '                                                        'END AG 02/08/2010

        '                                                        'AG 15/09/2010 - Special case when Absorbance has error
        '                                                        If (Not SampleList(j).IsABS_ErrorNull) Then
        '                                                            If (Not String.IsNullOrEmpty(SampleList(j).ABS_Error)) Then
        '                                                                ABSValue = GlobalConstants.ABSORBANCE_ERROR
        '                                                                CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
        '                                                            End If
        '                                                        End If
        '                                                        'END AG 15/09/2010

        '                                                        ResultDate = SampleList(j).ResultDate.ToString(DatePattern) & " " & _
        '                                                                     SampleList(j).ResultDate.ToString(TimePattern)

        '                                                        'AG 03/10/2013 - compact report shows the barcode here because there is not header
        '                                                        If pCompact Then
        '                                                            If Not SampleList(j).IsSpecimenIDListNull Then
        '                                                                DetailPatientID &= " (" & SampleList(j).SpecimenIDList & ")"
        '                                                            End If
        '                                                        End If
        '                                                        'AG 03/10/2013

        '                                                        'Insert Details row
        '                                                        ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow(resultRow.PatientID, DetailPatientID, TestName, _
        '                                                                                                                         SampleType, ReplicateNumber, _
        '                                                                                                                         ABSValue, CONC_Value, _
        '                                                                                                                         ReferenceRanges, Unit, _
        '                                                                                                                         ResultDate, Flags) 'EF 03/06/2014 (cambio nombre variable 'remarks' a Flags)
        '                                                    Next
        '                                                End If
        '                                            End If
        '                                        Next
        '                                    Next
        '                                Next

        '                                resultData.SetDatos = ResultsForReportDS
        '                            End If
        '                        End If

        '                    End If 'AG 10/09/2014 -BA-1894 , BA-1897 - Protection End If

        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsByPatientSampleForReportByOrderList", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

#End Region

#Region "METHODS FOR EXPORT RESULTS TO LIS"
        ''' <summary>
        ''' Count the total number of results selected to be Exported to LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pIncludeExportedResults">When TRUE, it indicates that results that have been already sent to LIS (ExportStatus = SENT) will be sent to LIS again
        '''                                       (apply only for Patient results). When FALSE, it means that all Patient and/or Control results that have not been still 
        '''                                       sent to LIS will be sent to LIS</param>
        ''' <returns>GlobalDataTO containing an integer value with the total number of results selected to be sent to LIS</returns>
        ''' <remarks>
        ''' Created by: SA 18/09/2014 - BA-1927
        ''' </remarks>
        Public Function CountTotalResultsToExportToLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                       ByVal pIncludeExportedResults As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultsDAO As New twksResultsDAO
                        resultData = myResultsDAO.CountTotalResultsToExportToLIS(dbConnection, pAnalyzerID, pWorkSessionID, pIncludeExportedResults)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.CountTotalResultsToExportToLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Results info by Export to LIS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>
        ''' GlobalDataTo indicating if an error has occurred or not.
        ''' If succeed, returns a ResultDS (ReportMaster and ReportDetails tables)
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL 27/06/2012
        ''' </remarks>
        Public Function GetResultsByLIS(ByVal pDBConnection As SqlClient.SqlConnection, _
                                        ByVal pAnalyzerID As String, _
                                        ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myExecutionDelegate As New ExecutionsDelegate

                        resultData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, pAnalyzerID, pWorkSessionID)

                        If (Not resultData.HasError) Then
                            Dim AverageResultsDS As New ResultsDS
                            Dim ExecutionsResultsDS As ExecutionsDS

                            ExecutionsResultsDS = CType(resultData.SetDatos, ExecutionsDS)

                            Dim OrderTestList As List(Of Integer) = _
                                    (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                     Order By row.TestPosition _
                                     Select row.OrderTestID Distinct).ToList()

                            Dim myResultsDelegate As New ResultsDelegate

                            For Each OrderTestID As Integer In OrderTestList
                                'Get all results for current OrderTestID.
                                resultData = myResultsDelegate.GetResults(dbConnection, OrderTestID)
                                If (Not resultData.HasError) Then
                                    For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                        If Not String.Equals(resultRow.ExportStatus, "SENT") And _
                                          (String.Equals(resultRow.SampleClass, "PATIENT") Or String.Equals(resultRow.SampleClass, "CTRL")) Then
                                            AverageResultsDS.vwksResults.ImportRow(resultRow)
                                        End If

                                    Next resultRow
                                Else
                                    Exit For
                                End If
                            Next OrderTestID

                            'Get Calculated Results
                            resultData = myResultsDelegate.GetCalculatedTestResults(dbConnection, pAnalyzerID, pWorkSessionID)
                            If (Not resultData.HasError) Then
                                For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                    If Not String.Equals(resultRow.ExportStatus, "SENT") And _
                                          (String.Equals(resultRow.SampleClass, "PATIENT") Or String.Equals(resultRow.SampleClass, "CTRL")) Then
                                        AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    End If
                                Next
                            End If

                            'Get ISE & OffSystem tests Results
                            resultData = myResultsDelegate.GetISEOFFSTestResults(dbConnection, pAnalyzerID, pWorkSessionID)
                            If (Not resultData.HasError) Then
                                For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                    If Not String.Equals(resultRow.ExportStatus, "SENT") And _
                                          (String.Equals(resultRow.SampleClass, "PATIENT") Or String.Equals(resultRow.SampleClass, "CTRL")) Then
                                        AverageResultsDS.vwksResults.ImportRow(resultRow)
                                    End If
                                Next resultRow

                            End If

                            Dim IsOrderProcessed As New Dictionary(Of String, Boolean)
                            'Fill Average PatientID and Name fields
                            For Each executionRow As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults.Rows
                                If String.Equals(executionRow.SampleClass, "PATIENT") Then
                                    If Not IsOrderProcessed.ContainsKey(executionRow.OrderID) Then
                                        For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                                            If String.Equals(resultRow.OrderID, executionRow.OrderID) Then
                                                resultRow.PatientName = executionRow.PatientName
                                                resultRow.PatientID = executionRow.PatientID
                                            End If
                                        Next resultRow

                                        IsOrderProcessed(executionRow.OrderID) = True
                                    End If

                                ElseIf String.Equals(executionRow.SampleClass, "CTRL") Then

                                    If Not IsOrderProcessed.ContainsKey(executionRow.OrderID) Then
                                        For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                                            If String.Equals(resultRow.SampleClass, executionRow.SampleClass) AndAlso _
                                               String.Equals(executionRow.TestType, "ISE") Then

                                                If String.Equals(resultRow.ControlName, String.Empty) Then
                                                    Dim myTestControlDelegate As New TestControlsDelegate
                                                    Dim myGlobalDataTO As New GlobalDataTO

                                                    myGlobalDataTO = myTestControlDelegate.GetControlsNEW(Nothing, resultRow.TestType, resultRow.TestID, resultRow.SampleType, 0)

                                                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO Is Nothing Then
                                                        Dim myTestControlsData As New TestControlsDS()
                                                        myTestControlsData = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)
                                                        If myTestControlsData.tparTestControls.Rows.Count > 0 Then
                                                            resultRow.ControlName = myTestControlsData.tparTestControls.First.ControlName
                                                        End If
                                                    End If

                                                End If

                                            End If
                                        Next resultRow

                                        IsOrderProcessed(executionRow.OrderID) = True
                                    End If


                                    'If String.Equals(resultRow.ControlName.Trim, String.Empty) AndAlso _
                                    '   String.Equals(resultRow.SampleClass, "CTRL") AndAlso _
                                    '   String.Equals(resultRow.TestType, "ISE") Then

                                    '    Dim myTestControlDelegate As New TestControlsDelegate
                                    '    Dim myGlobalDataTO As New GlobalDataTO

                                    '    myGlobalDataTO = myTestControlDelegate.GetControlsNEW(Nothing, resultRow.TestType, resultRow.TestID, resultRow.SampleType, 0)

                                    '    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO Is Nothing Then
                                    '        Dim myTestControlsData As New TestControlsDS()
                                    '        myTestControlsData = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)
                                    '        If myTestControlsData.tparTestControls.Rows.Count > 0 Then
                                    '            Name = myTestControlsData.tparTestControls.First.ControlName
                                    '        End If
                                    '    End If
                                    'Else
                                    '    Name = resultRow.ControlName
                                    'End If

                                End If
                            Next executionRow

                            'Dim ResultsForLISDS As New ResultsDS
                            'Dim TestsList As New List(Of String)
                            'Dim NamePlusUnit As String
                            'Dim TestTypeTestID As String

                            'Note that here TestID is in fact TestPosition.
                            'For Each testRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults
                            '    If Not TestsList.Contains(testRow.TestName) Then
                            '        TestsList.Add(testRow.TestName)
                            '        TestTypeTestID = testRow.TestType & testRow.TestID

                            '        If String.IsNullOrEmpty(testRow.MeasureUnit) Then
                            '            'NamePlusUnit = testRow.TestName
                            '            NamePlusUnit = String.Format("SampleClass: {0} SampleType: {1} TestName: {2} MeasureUnit: {3}", _
                            '                                         testRow.SampleClass, _
                            '                                         testRow.SampleType, _
                            '                                         testRow.TestName, _
                            '                                         "")
                            '        Else
                            '            NamePlusUnit = String.Format("SampleClass: {0} SampleType: {1} TestName: {2} MeasureUnit: {3}", _
                            '                                         testRow.SampleClass, _
                            '                                         testRow.SampleType, _
                            '                                         testRow.TestName, _
                            '                                         testRow.MeasureUnit)
                            '        End If

                            '        ResultsForLISDS.ReportTestMaster.AddReportTestMasterRow(TestTypeTestID, NamePlusUnit)
                            '    End If
                            'Next testRow

                            'Fill Details table
                            'For Each Test As String In TestsList
                            '    'InsertResultsBLANK(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForReportDS, "")
                            '    'InsertResultsCALIB(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForReportDS, "")
                            'InsertResultsCTRL(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForLISDS, "")
                            '    InsertResultsPATIENT(Test, AverageResultsDS, ExecutionsResultsDS, ResultsForLISDS, "")
                            'Next

                            resultData.SetDatos = AverageResultsDS

                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsByTestForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get Patient and Control Results to export (according the configured Export Frequency)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSampleClass">SampleClass code. Optional parameter; when informed, value is CTRL or PATIENT</param>
        ''' <param name="pOrderID">Order Identifier. Optional parameter; when informed, only results of the OrderTests included in the Order
        '''                        are returned</param>
        ''' <param name="pOrderTestID">Order Test Identifier. Optional parameter; when informed, only results of the specified OrderTest
        '''                            are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (subtable vwksResults) with the group of Results to export</returns>
        '''  <remarks>
        ''' Created by:  TR 13/07/2012
        ''' Modified by: SG 10/04/2012 - add parameter "pIncludeSentResults"
        ''' AG 23/07/2013 - If the ordertest is informed search all calculated tests that need it, send this list to the GetResultsToExport method
        ''' </remarks>
        Public Function GetResultsToExport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                           Optional ByVal pSampleClass As String = "", Optional ByVal pOrderID As String = "", _
                                           Optional ByVal pOrderTestID As Integer = -1, _
                                           Optional ByVal pIncludeSentResults As Boolean = False) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID, pSampleClass, pOrderID, pOrderTestID, pIncludeSentResults)

                        'AG 23/07/2013 - When informed search all calculated test ordertests that depends on the pOrderTests received in parameter (export freq ON finish patient test)
                        If pOrderTestID <> -1 Then
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim dataToReturn As New ResultsDS
                                dataToReturn = DirectCast(resultData.SetDatos, ResultsDS)

                                Dim affectedCalcTestOT As New List(Of Integer) 'Internal queue with the affected calctests ordertests - do not allow duplicates
                                Dim auxDS As New ResultsDS
                                Dim repeatLoopFlag As Boolean = False

                                'Lopp #1
                                'Look for all calculated tests (orderTests) that depends of pOrderTestID
                                resultData = GetCalcTestsResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID, pSampleClass, pOrderID, pOrderTestID, affectedCalcTestOT, repeatLoopFlag)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    'Include the results into Dataset to return
                                    auxDS = DirectCast(resultData.SetDatos, ResultsDS)
                                    For Each resRow As ResultsDS.vwksResultsRow In auxDS.vwksResults
                                        dataToReturn.vwksResults.ImportRow(resRow)
                                    Next
                                    dataToReturn.vwksResults.AcceptChanges()
                                End If

                                'Loop #2
                                'Look for all calculated tests (ordertests) that depends of Row.CalcTestOT (for example BUN also belongs to BUN / CREATININE)
                                Dim Temp_affectedCalcTestOT As List(Of Integer) 'SGM 24/07/2013
                                While repeatLoopFlag
                                    repeatLoopFlag = False
                                    Temp_affectedCalcTestOT = New List(Of Integer)(affectedCalcTestOT) 'SGM 24/07/2013
                                    For Each item As Integer In affectedCalcTestOT
                                        'resultData = GetCalcTestsResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID, pSampleClass, pOrderID, item, affectedCalcTestOT, repeatLoopFlag)
                                        resultData = GetCalcTestsResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID, pSampleClass, pOrderID, item, Temp_affectedCalcTestOT, repeatLoopFlag)
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            'Include the results into Dataset to return
                                            auxDS = DirectCast(resultData.SetDatos, ResultsDS)
                                            For Each resRow As ResultsDS.vwksResultsRow In auxDS.vwksResults
                                                dataToReturn.vwksResults.ImportRow(resRow)
                                            Next
                                            dataToReturn.vwksResults.AcceptChanges()
                                        End If
                                    Next
                                    affectedCalcTestOT = New List(Of Integer)(Temp_affectedCalcTestOT) 'SGM 24/07/2013

                                End While
                                affectedCalcTestOT = Nothing
                                Temp_affectedCalcTestOT = Nothing
                                resultData.SetDatos = dataToReturn
                            End If
                        End If
                        'AG 23/07/2013

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsToExport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Patient and Control Results to export (those related with a new Blank or Calibrator result)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestType">Test Type code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter, not informed when the method is searching Patient 
        '''                           or Control Results related with a new Blank Result</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (subtable vwksResults) with the group of Results to export</returns>
        ''' <remarks>
        ''' Created by:  TR 13/07/212
        ''' Modified by: SA 01/08/2012 - Function name changed to remove overload of function GetResultsToExport
        ''' </remarks>
        Public Function GetResultsToExportForBLANKAndCALIB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                           ByVal pTestType As String, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksResultDAO As New twksResultsDAO()
                        resultData = mytwksResultDAO.GetResultsToExportForBLANKAndCALIB(dbConnection, pWorkSessionID, pAnalyzerID, pTestType, pTestID, pSampleType)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetResultsToExportForBLANKAndCALIB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the LISMessageID (requires ExportStatus to "SENDING")
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' </remarks>
        Public Function UpdateLISMessageID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultsDAO

                        resultData = myDAO.UpdateLISMessageID(dbConnection, pResultsDS)

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

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateLISMessageID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Updates the LISMessageID depending on ExportStatus
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID"></param>
        ''' <param name="pNewExportStatus"></param>
        ''' <param name="pSetDateTime"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' AG 02/04/2014 - #1564 add parameter pSetDateTime (when TRUE the DAO has to inform ExportDateTime, else leave as NULL)
        ''' AG 30/07/2014 - #1887 OrderToExport management
        ''' AG 21/10/2014 - BA-2011 remember the affected results
        ''' AG 22/10/2014 - BA-2011 inform new parameter required pOnlyPatientsFlag = False (it can apply for both patients or controls)
        ''' </remarks>
        Public Function UpdateExportStatusByMessageID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pLISMessageID As String, _
                                           ByVal pNewExportStatus As String, ByVal pSetDateTime As Boolean) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultsDAO

                        'AG 02/04/2014 - #1564
                        'resultData = myDAO.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus)
                        resultData = myDAO.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus, pSetDateTime)

                        If Not resultData.HasError Then
                            Dim myAffected As Integer = resultData.AffectedRecords 'AG 21/10/2014 BA-2011

                            'AG 30/07/2014 #1887 - OrderToExport management
                            Dim myOrder As New OrdersDelegate
                            If pNewExportStatus = "SENT" Then
                                resultData = myOrder.SetNewOrderToExportValue(dbConnection, , , pLISMessageID)
                            Else
                                'Set OrderToExport = TRUE because some result sent to LIS has not been accepted!!!
                                resultData = myOrder.UpdateOrderToExport(dbConnection, True, False, , , pLISMessageID)
                            End If
                            'AG 30/07/2014

                            resultData.AffectedRecords = myAffected 'AG 21/10/2014 BA-2011
                        End If

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

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateExportStatusByMessageID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Get all validated and accepted results ExportStatus by OrderID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderID"></param>
        ''' <param name="pOnlyMappedWithLIS"></param>
        ''' <returns>Returns dataset containing only all Accepted results that have a valid mapping with LIS. </returns>
        ''' <remarks>
        ''' Created:  AG 30/07/2014 #1887   OrderToExport management
        ''' Modified: AG 17/10/2014 BA-2011 parameter for return only the results mapped with LIS + pOrderID parameters changes is 'List (Of String)' instead of 'String'
        '''           WE 17/10/2014 BA-2018 Req.6: Tests without a LIS mapping can´t be sent to LIS, as a consequence if a patient only has Tests without LIS mapping or
        '''                                 the rest have already been sent (without modifications of its related results afterwards), the checkbox 'To be sent to LIS'
        '''                                 must be unchecked on screen Actual Results.
        ''' </remarks>
        Public Function GetAcceptedResultsByOrder(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As List(Of String), ByVal pOnlyMappedWithLIS As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksResultsDAO
                        resultData = myDAO.GetAcceptedResultsByOrder(dbConnection, pOrderID)

                        If pOnlyMappedWithLIS AndAlso Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myResDS As New ResultsDS
                            myResDS = DirectCast(resultData.SetDatos, ResultsDS)

                            Dim myAllTestsByType As New AllTestByTypeDelegate
                            resultData = Nothing
                            resultData = myAllTestsByType.ReadAll(dbConnection)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim dsAllTestsByType As New AllTestsByTypeDS
                                dsAllTestsByType = DirectCast(resultData.SetDatos, AllTestsByTypeDS)

                                ' From myResDS.vwksResults remove all rows with tests without LIS mapped value
                                ' Easier coding approach: fill a separate dataset only with those rows that have a LIS mapping.

                                ' Create separate dataset to collect only rows with tests that have a LIS mapping.
                                Dim myNewDataSet As New ResultsDS
                                myNewDataSet.Clear()

                                For Each myDataRow As ResultsDS.vwksResultsRow In myResDS.vwksResults
                                    resultData = Nothing
                                    resultData = myAllTestsByType.GetLISTestID(dsAllTestsByType, myDataRow.TestID, myDataRow.TestType)
                                    ' If Row(i) has LIS mapping => add Row(i) to separate dataset.
                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                        myNewDataSet.vwksResults.ImportRow(myDataRow)
                                    End If
                                Next

                                ' Return dataset with all accepted results that have mapping with LIS.
                                resultData.HasError = False
                                resultData.SetDatos = myNewDataSet

                                'AG 21/10/2014 BA-2018 remove the error flag!!!
                                resultData.ErrorCode = ""
                            End If

                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.GetAcceptedResultsByOrder", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' After call methods UpdateExportStatus / UpdateExportStatusMASSIVE the field OrderToExport must be re-evaluated
        '''
        ''' DESIGN NOTES:
        ''' Method UpdateExportStatus is used:
        '''     isLISWithFilesMode (ExportStatus = TRUE) - out of date!!
        '''     not isLISWithFilesMode (ExportStatus = FALSE because not msg to LIS can be sent, so OrderToExport is already TRUE)
        '''
        ''' Method UpdateExportStatusMASSIVE is never used
        ''' 
        ''' CONCLUSION: OrderToExport management NOT REQUIRED after current use of methods UpdateExportStatus / UpdateExportStatusMASSIVE
        ''' 
        ''' Define the method but I dont develop the code! It is not required by now
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pResultsDS"></param>
        ''' <param name="pAlternativeStatus"></param>
        ''' <returns></returns>
        ''' <remarks>AG 30/07/2014 creation #1887 - OrderToExport management
        ''' AG 22/10/2014 - BA-2011 inform new parameter required pOnlyPatientsFlag = False (it can apply for both patients or controls)
        ''' </remarks>
        Private Function UpdateOrderToExportAfterChangesInExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, ByVal pAlternativeStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim OrderToExportToEvaluate_OrderList As New List(Of String)
                        Dim OrderToExportFALSE_OrderList As New List(Of String)

                        Dim myOrder As New OrdersDelegate
                        If pAlternativeStatus = "SENT" Then '-> See notes in summary! Only possible working with files. Out of date!!!
                            'Get all different orderID in DS as fill OrderToExportToEvaluate_OrderList

                        ElseIf pAlternativeStatus.Length > 0 Then
                            'Get all different orderID in DS as fill OrderToExportFALSE_OrderList

                        Else '-> See notes in summary! Case not possible
                            'Get by linq OrderTests with ExportStatus <> SENT, get their orderID and fill OrderToExportFALSE_OrderList

                            'Get by linq OrderTests with ExportStatus = SENT, get their orderID and if not exists in OrderToExportFALSE_OrderList add to OrderToExportToEvaluate_OrderList

                        End If

                        'Finally update the OrderToExport value
                        'Orders to evaluate OrderToExport new value
                        For Each tmpOrder As String In OrderToExportToEvaluate_OrderList
                            myGlobalDataTO = myOrder.SetNewOrderToExportValue(dbConnection, tmpOrder)
                        Next

                        'Orders to set OrderToExport = FALSE
                        For Each tmpOrder As String In OrderToExportFALSE_OrderList
                            'Infomr the SampleClass of the Order
                            myGlobalDataTO = myOrder.UpdateOrderToExport(dbConnection, False, False, tmpOrder)
                        Next

                        'Release memory
                        OrderToExportToEvaluate_OrderList.Clear()
                        OrderToExportToEvaluate_OrderList = Nothing
                        OrderToExportFALSE_OrderList.Clear()
                        OrderToExportFALSE_OrderList = Nothing

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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.UpdateOrderToExportAfterChangesInExportStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Check wether all results belonging to 1 Patient´s Order(s) are valid and accepted OR all related Tests are NOT mapped to LIS.
        ''' </summary>
        ''' <param name="pDBConnection">Database Connection to be used</param>
        ''' <param name="pOrderID">List of 1 or more OrderID(s)</param>
        ''' <param name="pSampleClass">Sample class of the order</param>
        ''' <returns>Returns TRUE if Patient´s results are all NOT Accepted OR all Tests NOT mapped to LIS. Else returns FALSE.</returns>
        ''' <remarks>
        ''' Created:  WE 20/10/2014 BA-2018 Req.7: Tests without a LIS mapping can´t be sent to LIS, as a consequence if a patient only has Tests without LIS mapping or
        '''                         the rest have already been sent (without modifications of its related results afterwards), the checkbox 'To be sent to LIS'
        '''                         must be unchecked on screen Actual Results.
        ''' Modified: AG 22/10/2014 BA-2011 validation new parameter pSampleClass because the control also can have 2 or more orders.
        ''' </remarks>
        Public Function AllResultsNotAcceptedOrAllTestsNotMapped(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As List(Of String), ByVal pSampleClass As String) As Boolean
            ' IN:  List of Patients/OrderID (1 or more patients)
            '
            Dim result As Boolean = False
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        ' Look for other orders for the same sample.
                        If pOrderID.Count = 1 Then
                            Dim ordersDlg As New OrdersDelegate
                            resultData = ordersDlg.ReadRelatedOrdersByOrderID(dbConnection, pOrderID(0), pSampleClass)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then

                                For Each row As OrdersDS.twksOrdersRow In DirectCast(resultData.SetDatos, OrdersDS).twksOrders
                                    If Not pOrderID.Contains(row.OrderID) Then
                                        pOrderID.Add(row.OrderID)
                                    End If
                                Next
                            End If
                        End If

                        If Not resultData.HasError Then
                            resultData = GetAcceptedResultsByOrder(dbConnection, pOrderID, True)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim myResDS As New ResultsDS
                                myResDS = DirectCast(resultData.SetDatos, ResultsDS)

                                If myResDS.vwksResults.Rows.Count > 0 Then
                                    result = False
                                Else
                                    result = True
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ResultsDelegate.AllResultsNotAcceptedOrAllTestsNotMapped", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return result
        End Function




#End Region

    End Class
End Namespace



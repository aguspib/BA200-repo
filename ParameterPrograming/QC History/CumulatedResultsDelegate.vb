Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Windows.Forms

Namespace Biosystems.Ax00.BL

    Public Class CumulatedResultsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create a new Cumulated Serie for a QCTestSampleID/QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 20/05/2011
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCumResultNumDAO As New tqcCumulatedResultsDAO
                        myGlobalDataTO = myCumResultNumDAO.CreateNEW(dbConnection, pCumulatedResultDS)

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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete a group of Cumulated Series for a Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCumSeriesToDelete">Typed DataSet containing all the Cumulated Series to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: SA 27/06/2011 - Delete the XML File 
        '''              SA 05/06/2012 - Inform parameter AnalyzerID when calling functions in classes tqcCumulatedResultsDAO
        '''                              and LastCumulatedValuesDelegate
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumSeriesToDelete As CumulatedResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCumResultNumDAO As New tqcCumulatedResultsDAO
                        Dim myRunsGroupDelegate As New RunsGroupsDelegate
                        Dim myCumSeriesByControlLotDS As New CumulatedResultsDS
                        Dim myLastCumulatedDelegate As New LastCumulatedValuesDelegate

                        Dim lstXMLNames As List(Of String) = (From a In pCumSeriesToDelete.tqcCumulatedResults _
                                                         Where Not a.IsXMLFileNameNull _
                                                            Select a.XMLFileName).ToList

                        For Each cumSerie As CumulatedResultsDS.tqcCumulatedResultsRow In pCumSeriesToDelete.tqcCumulatedResults.Rows
                            'Delete the RunsGroup of the results included in the Cumulated Serie
                            myGlobalDataTO = myRunsGroupDelegate.DeleteNEW(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID, cumSerie.AnalyzerID, cumSerie.CumResultsNum)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Delete the Cumulated Serie
                            myGlobalDataTO = myCumResultNumDAO.DeleteNEW(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID, cumSerie.AnalyzerID, cumSerie.CumResultsNum)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Delete the Last Cumulated Values currently saved for the Test/SampleType and Control/Lot
                            myGlobalDataTO = myLastCumulatedDelegate.DeleteNEW(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID, cumSerie.AnalyzerID)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Get the remaining Cumulated Series for the Test/SampleType and Control/Lot
                            myGlobalDataTO = myCumResultNumDAO.ReadByQcTestSampleIDQCControlLotIDNEW(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID, cumSerie.AnalyzerID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                                If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count > 0) Then
                                    'Recalculate the Last Cumulated Values based in the existing Cumulated Series for the Test/SampleType and Control/Lot 
                                    myGlobalDataTO = RecalculateLastCumulatedValuesNEW(myCumSeriesByControlLotDS)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                                        If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count = 1) Then
                                            'Insert the new calculated Last Cumulated values
                                            myGlobalDataTO = myLastCumulatedDelegate.AddLastCumValuesNEW(dbConnection, myCumSeriesByControlLotDS)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    Else
                                        'Error recalculating the last cumulated values
                                        Exit For
                                    End If
                                End If
                            Else
                                'Error getting the Cumulated Series for the Test/SampleType and Control/Lot
                                Exit For
                            End If
                        Next

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Delete all XML Files containing the Results of the deleted Cumulated Series
                            For Each xmlFile As String In lstXMLNames
                                IO.File.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & xmlFile)
                            Next
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
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
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Controls/Lots that have Cumulated Series for the specified Test/SampleType in the informed period of time
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''  <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        ''' <param name="pDateTo">Date To to search Cumulated Series</param>
        ''' <param name="pCumulatedSeriesDS">Typed DataSet CumulatedResultsDS containing all Cumulated Series for all Controls/Lots linked 
        '''                                  to the informed Test/SampleType in the specified range of dates</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCCumulatedSummaryDS containing summary data of all Controls/Lots 
        '''          that have Cumulated Series for the specified Test/SampleType in the informed period of time</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: SA 07/07/2011 - For Controls/Lots having only one Cumulated Serie, set to Null fields Mean, SD, CV, and Min/Max Range
        '''              SA 05/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetControlsLotsWithCumulatedSeriesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                              ByVal pAnalyzerID As String, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime, _
                                                              ByVal pCumulatedSeriesDS As CumulatedResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCumResultsDAO As New tqcCumulatedResultsDAO
                        myGlobalDataTO = myCumResultsDAO.GetControlsLotsWithCumulatedSeriesNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pDateFrom, pDateTo)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myQCCumSummaryDS As QCCumulatedSummaryDS = DirectCast(myGlobalDataTO.SetDatos, QCCumulatedSummaryDS)

                            Dim myMean As Single = 0
                            Dim mySD As Single = 0

                            Dim myCumSeriesByControlLotDS As New CumulatedResultsDS
                            Dim lstCumSeriesByControlLot As List(Of CumulatedResultsDS.tqcCumulatedResultsRow)
                            For Each controlLot As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In myQCCumSummaryDS.QCCumulatedSummaryTable.Rows
                                'Get the Cumulated Series for the Control/Lot in process
                                lstCumSeriesByControlLot = (From a As CumulatedResultsDS.tqcCumulatedResultsRow In pCumulatedSeriesDS.tqcCumulatedResults _
                                                           Where a.QCControlLotID = controlLot.QCControlLotID _
                                                        Order By a.CumDateTime _
                                                          Select a).ToList

                                If (lstCumSeriesByControlLot.Count = 1) Then
                                    controlLot.BeginEdit()
                                    controlLot.SetMeanNull()
                                    controlLot.SetSDNull()
                                    controlLot.SetCVNull()
                                    controlLot.SetMinRangeNull()
                                    controlLot.SetMaxRangeNull()
                                    controlLot.EndEdit()

                                ElseIf (lstCumSeriesByControlLot.Count > 1) Then
                                    myCumSeriesByControlLotDS.Clear()
                                    For Each cumSerie As CumulatedResultsDS.tqcCumulatedResultsRow In lstCumSeriesByControlLot
                                        myCumSeriesByControlLotDS.tqcCumulatedResults.ImportRow(cumSerie)
                                    Next

                                    myGlobalDataTO = RecalculateLastCumulatedValuesNEW(myCumSeriesByControlLotDS)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                                        If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count = 1) Then
                                            'Update calculated fields for the Control/Lot
                                            myMean = myCumSeriesByControlLotDS.tqcCumulatedResults.First.Mean
                                            mySD = myCumSeriesByControlLotDS.tqcCumulatedResults.First.SD

                                            controlLot.BeginEdit()
                                            controlLot.Mean = myMean
                                            controlLot.SD = mySD
                                            controlLot.CV = (mySD / myMean) * 100
                                            controlLot.MinRange = myMean - (controlLot.RejectionCriteria * mySD)
                                            controlLot.MaxRange = myMean + (controlLot.RejectionCriteria * mySD)
                                            controlLot.EndEdit()
                                        End If
                                    Else
                                        'Error recalculating last cumulated values
                                        Exit For
                                    End If
                                End If
                            Next

                            If (Not myGlobalDataTO.HasError) Then
                                If (myQCCumSummaryDS.QCCumulatedSummaryTable.Count > 0) Then
                                    'Mark the first Control/Lot as selected and return the DS
                                    myQCCumSummaryDS.QCCumulatedSummaryTable.First.Selected = True
                                    myQCCumSummaryDS.AcceptChanges()
                                End If

                                myGlobalDataTO.SetDatos = myQCCumSummaryDS
                                myGlobalDataTO.HasError = False
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.GetControlsLotsWithCumulatedSeries", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get details of all Cumulated Series of the the specified Test/SampleType (all Controls/Lots) in the informed period of time
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        ''' <param name="pDateTo">Date To to search Cumulated Series</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS containing data of all Cumulated Series saved for 
        '''          the specified Test/SampleType (all Controls/Lots) in the informed period of time</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: TR 29/06/2011 - Added the calculation for VisibleCumResultNum
        '''              SA 05/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetCumulatedSeriesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                              ByVal pAnalyzerID As String, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCumResultsDAO As New tqcCumulatedResultsDAO
                        myGlobalDataTO = myCumResultsDAO.GetCumulatedSeriesNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pDateFrom, pDateTo)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myCumulatedResultsDS As CumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)
                            Dim myControlsLotsIDList As List(Of Integer) = (From a In myCumulatedResultsDS.tqcCumulatedResults _
                                                                          Select a.QCControlLotID Distinct).ToList()

                            Dim mySecuence As Integer
                            Dim myCumulateResultList As New List(Of CumulatedResultsDS.tqcCumulatedResultsRow)
                            For Each controlLotID As Integer In myControlsLotsIDList
                                mySecuence = 0
                                myCumulateResultList = (From a In myCumulatedResultsDS.tqcCumulatedResults _
                                                       Where a.QCControlLotID = controlLotID _
                                                      Select a).ToList()
                                'Set the visible cumResultNum value.
                                For Each cumResultRow As CumulatedResultsDS.tqcCumulatedResultsRow In myCumulateResultList
                                    mySecuence += 1
                                    cumResultRow.VisibleCumResultNumber = mySecuence
                                Next
                            Next
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.GetCumulatedSeries", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the oldest date of a cumulated serie for the specified QCTestSampleID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        ''' <remarks>
        ''' Created by:  SA 15/06/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetMinCumDateTimeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                             ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCumResultsDAO As New tqcCumulatedResultsDAO
                        myGlobalDataTO = myCumResultsDAO.GetMinCumDateTimeNEW(dbConnection, pQCTestSampleID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.GetMinCumDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the newest date of a cumulated serie for the specified QCTestSampleID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pQCTestSampleID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 02/07/2012</remarks>
        Public Function GetMaxCumDateTime(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                          ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCumResultsDAO As New tqcCumulatedResultsDAO
                        myGlobalDataTO = myCumResultsDAO.GetMaxCumDateTime(dbConnection, pQCTestSampleID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.GetMaxCumDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/QCControlLotID, create a new Cumulated Serie with non excluded QC Results in the Runs Group that is open.
        ''' After create the Cumulated Serie, the Runs Group is marked as closed and all QC Results included in it are also marked as closed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">QC TestSample ID</param>
        ''' <param name="pQCControlLotID">QC ControlLot ID</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <param name="pSaveXMLFileToTEMPPath">Optional parameter. When True, it indicates the generated XML files have to be created in a
        '''                                      temporary path</param>
        ''' <returns>GlobalDataTO containing success/error information. If the first Cumulated Serie of an Analyzer was deleted, and the DB Transaction
        '''          was not opened locally, the GlobalDataTO also contains the name of the XML files with the results of the deleted series</returns>
        ''' <remarks>
        ''' Created by:  TR 23/05/2011
        ''' Modified by: SA 01/06/2011 - Added code to save also last cumulated values for the specified QCTestSampleID/QCControlLotID
        '''              SA 21/06/2011 - Verify if the Maximum Number of Cumulated Serie by QCTestSampleID/QCControlLotID has been reached,
        '''                              and in this case, delete the oldest Cumulated Serie and resorting the rest before insert the new 
        '''                              Cumulated Serie
        '''              DL 27/06/2011 - Added export of Cumulated QC Results to an XML
        '''              SA 27/06/2011 - Added the name of the XML file in the DS used to create the Cumulated Serie. The XML file has to be wrote just 
        '''                              before execute the Commit Transaction. QC Results included in the Cumulated Runs Group are physically deleted.
        '''                              Added optional parameter to indicate if the XML files have to be created in a TEMP path due to the control of
        '''                              the DB Transaction is done in another function (and in this case, after executing the Commit in that method,
        '''                              the generated XML files are moved from the TEMP path to the final one)
        '''              SA 05/07/2011 - Changed the way of calculate the SD
        '''              SA 08/07/2011 - If the first Cumulated Serie was deleted, delete the XML File after Commit the DB Transaction or, if the
        '''                              DB Transaction was not opened locally, return the name of the XML File in the GlobalDataTO
        '''              SA 05/06/2012 - Added optional parameter for AnalyzerID
        '''              SA 14/06/2012 - Implementation changed to manage the case of cumulate QC Results of several Analyzers
        ''' </remarks>
        Public Function SaveCumulateResultNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                              Optional ByVal pAnalyzerID As String = "", Optional ByVal pSaveXMLFileToTEMPPath As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myXMLFileName As String = String.Empty
                        Dim myFileNameToDelete As String = String.Empty
                        Dim myQCResultsToExportDS As New QCResultsDS
                        Dim allQCResultsToExportDS As New QCResultsDS

                        Dim myQCResultDS As New QCResultsDS
                        Dim myQCResultCalculationDS As New QCResultsCalculationDS
                        Dim myQCResultDelegate As New QCResultsDelegate
                        Dim myCumResultNumDAO As New tqcCumulatedResultsDAO

                        'Get value of the General Setting for the maximum number of Cumulated Series 
                        Dim myGeneralSettingsDelegate As New GeneralSettingsDelegate
                        myGlobalDataTO = myGeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_CUMULATED_QCSERIES.ToString)

                        Dim maxNumber As Integer = 0
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            maxNumber = Convert.ToInt32(myGlobalDataTO.SetDatos)

                            'Get calculated data needed to create the new Cumulated Serie
                            myGlobalDataTO = myQCResultDelegate.GetDataToCreateCumulateNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myQCResultCalculationDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsCalculationDS)

                                Dim myMinCumResultNum As Integer = 0
                                Dim myMaxCumResultNum As Integer = 0
                                Dim myRunsGroupNumber As Integer = 0

                                Dim myCumulatedResultDS As New CumulatedResultsDS
                                Dim myCumulatedResultRow As CumulatedResultsDS.tqcCumulatedResultsRow

                                For Each cumDataRow As QCResultsCalculationDS.tQCResultCalculationRow In myQCResultCalculationDS.tQCResultCalculation.Rows
                                    myRunsGroupNumber = cumDataRow.RunsGroupNumber

                                    'Search on tqcCumulatedResults all Cumulated Series saved for the QCTestSampleID/QCControlID
                                    myGlobalDataTO = myCumResultNumDAO.ReadByQcTestSampleIDQCControlLotIDNEW(dbConnection, pQCTestSampleID, pQCControlLotID, cumDataRow.AnalyzerID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myCumulatedResultDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                                        'If there is at least a Cumulated Serie, then get the last one and set CumResultsNum = CumResultsNum + 1
                                        If (myCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
                                            'Get the MIN(CumResultsNum) and set the value to local variable
                                            myMinCumResultNum = (From a In myCumulatedResultDS.tqcCumulatedResults _
                                                               Select a.CumResultsNum).Min

                                            'Get the MAX(CumResultsNum) and set the value to local variable
                                            myMaxCumResultNum = (From a In myCumulatedResultDS.tqcCumulatedResults _
                                                               Select a.CumResultsNum).Max

                                            If (myCumulatedResultDS.tqcCumulatedResults.Count = maxNumber) Then
                                                'Get the name of the XML File containing the QC Results for the first Cumulate
                                                Dim xmlName As List(Of String) = (From a In myCumulatedResultDS.tqcCumulatedResults _
                                                                                 Where a.CumResultsNum = myMinCumResultNum _
                                                                               And Not a.IsXMLFileNameNull _
                                                                                Select a.XMLFileName).ToList()
                                                If (xmlName.Count = 1) Then cumDataRow.XMLFileToDelete = xmlName.First.ToString 'myFileNameToDelete = xmlName.First.ToString

                                                'Delete the first Cumulated Serie
                                                myGlobalDataTO = DeleteFirstCumulatedNEW(dbConnection, pQCTestSampleID, pQCControlLotID, cumDataRow.AnalyzerID, myMinCumResultNum, "")
                                                If (Not myGlobalDataTO.HasError) Then
                                                    Dim mytqcCumulatedResults As New tqcCumulatedResultsDAO
                                                    myGlobalDataTO = mytqcCumulatedResults.DecrementCumResultsNumNEW(dbConnection, pQCTestSampleID, pQCControlLotID, cumDataRow.AnalyzerID)
                                                End If
                                            Else
                                                'Increment the value by 1
                                                myMaxCumResultNum += 1
                                            End If
                                        Else
                                            'It is the first Cumulated Serie for the QCTestSampleID/QCControlLotID
                                            myMaxCumResultNum = 1
                                        End If
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'Generate the name of the XML File in which the QC Results included in the Cumulate will be exported
                                        cumDataRow.XMLFileToCreate = cumDataRow.AnalyzerID.Trim & "-" & _
                                                                     pQCTestSampleID.ToString & "-" & _
                                                                     pQCControlLotID.ToString & "-" & _
                                                                     myRunsGroupNumber.ToString & "-" & _
                                                                     DateTime.Now.ToString("yyyyMMdd") & " " & _
                                                                     DateTime.Now.ToString("HH") & _
                                                                     DateTime.Now.ToString("mm") & _
                                                                     DateTime.Now.ToString("ss") & ".xml"

                                        'Clear previous values to reuse the DS
                                        myCumulatedResultDS.tqcCumulatedResults.Clear()

                                        'Add a row with all values of the new Cumulated Serie and insert it
                                        myCumulatedResultRow = myCumulatedResultDS.tqcCumulatedResults.NewtqcCumulatedResultsRow()
                                        myCumulatedResultRow.AnalyzerID = cumDataRow.AnalyzerID
                                        myCumulatedResultRow.QCTestSampleID = pQCTestSampleID
                                        myCumulatedResultRow.QCControlLotID = pQCControlLotID
                                        myCumulatedResultRow.CumResultsNum = myMaxCumResultNum
                                        myCumulatedResultRow.CumDateTime = DateTime.Now
                                        myCumulatedResultRow.TotalRuns = cumDataRow.N
                                        myCumulatedResultRow.FirstRunDateTime = cumDataRow.FirstRunDateTime
                                        myCumulatedResultRow.LastRunDateTime = cumDataRow.LastRunDateTime
                                        myCumulatedResultRow.SumResults = cumDataRow.SumXi
                                        myCumulatedResultRow.SumSQRDResults = cumDataRow.SumXi2
                                        myCumulatedResultRow.Mean = cumDataRow.Mean

                                        'Validate if there is more than one Result to accumulate; in that case, SD cannot be calculated and it is set to zero 
                                        If (cumDataRow.N > 1) Then
                                            'Calculate the SD
                                            myCumulatedResultRow.SD = CalculateSDNEW(cumDataRow)
                                        Else
                                            'Set SD = 0 
                                            myCumulatedResultRow.SD = 0
                                        End If

                                        'Inform the name of the XML File containing the QC Results included in the Cumulated 
                                        myCumulatedResultRow.XMLFileName = cumDataRow.XMLFileToCreate

                                        'Get the current logged User and set the current DateTime in the audit fields
                                        Dim myGlobalBase As New GlobalBase
                                        myCumulatedResultRow.TS_User = GlobalBase.GetSessionInfo.UserName
                                        myCumulatedResultRow.TS_DateTime = DateTime.Now

                                        myCumulatedResultDS.tqcCumulatedResults.AddtqcCumulatedResultsRow(myCumulatedResultRow)

                                        'Insert the new Cumulated Serie for the QCTestSampleID/QCControlID.
                                        myGlobalDataTO = CreateNEW(dbConnection, myCumulatedResultDS)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        If (myMaxCumResultNum = 1) Then
                                            'Insert values of the Cumulated Serie as Last Cumulated Values
                                            Dim myLastCumValuesDelegate As New LastCumulatedValuesDelegate
                                            myGlobalDataTO = myLastCumValuesDelegate.AddLastCumValuesNEW(dbConnection, myCumulatedResultDS)
                                        Else
                                            'Update Last Cumulated Values by adding data of the Cumulated Serie and then calculating the new Mean and SD values
                                            Dim myLastCumValuesDelegate As New LastCumulatedValuesDelegate
                                            myGlobalDataTO = myLastCumValuesDelegate.ModifyLastCumValuesNEW(dbConnection, myCumulatedResultDS)
                                        End If
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'Close the RunsGroupNumber informing the CumResultsNum
                                        Dim myRunsGroupDelegate As New RunsGroupsDelegate
                                        myGlobalDataTO = myRunsGroupDelegate.CloseRunsGroupNEW(dbConnection, pQCTestSampleID, pQCControlLotID, cumDataRow.AnalyzerID, _
                                                                                               myMaxCumResultNum, myRunsGroupNumber)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'Get all the QC Results included on the RunsGroupNumber and that will be exported to an XML file
                                        Dim myQCResultsDelegate As New QCResultsDelegate
                                        myGlobalDataTO = myQCResultDelegate.GetQCResultsToExportNEW(dbConnection, pQCTestSampleID, pQCControlLotID, _
                                                                                                    cumDataRow.AnalyzerID, myRunsGroupNumber)

                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myQCResultsToExportDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

                                            'Finally, delete the QC Results
                                            myGlobalDataTO = myQCResultDelegate.DeleteByRunsGroupNumNEW(dbConnection, pQCTestSampleID, pQCControlLotID, _
                                                                                                        cumDataRow.AnalyzerID, myRunsGroupNumber)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            'Copy all deleted QC Results to a final DS than will be used to export them to an XML file
                                            If (allQCResultsToExportDS.tqcResults.Count = 0) Then
                                                allQCResultsToExportDS = myQCResultsToExportDS
                                            Else
                                                For Each result As QCResultsDS.tqcResultsRow In myQCResultsToExportDS.tqcResults.Rows
                                                    allQCResultsToExportDS.tqcResults.ImportRow(result)
                                                Next
                                            End If
                                        Else
                                            'Error getting the group of QCResults to export to an XML File
                                            Exit For
                                        End If
                                    Else
                                        'Error searching all Cumulated Series saved for the QCTestSampleID/QCControlID
                                        Exit For
                                    End If
                                Next
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'If there were not errors in the process, then export the QC Results to an XML file
                            Dim myDirName As String = Application.StartupPath & GlobalBase.QCResultsFilesPath
                            If (allQCResultsToExportDS.tqcResults.Rows.Count > 0) Then
                                If (pSaveXMLFileToTEMPPath) Then myDirName &= "TEMP\"

                                'Mark all results as Closed 
                                For Each qcResultRow As QCResultsDS.tqcResultsRow In allQCResultsToExportDS.tqcResults.Rows
                                    qcResultRow.ClosedResult = True
                                Next
                                allQCResultsToExportDS.tqcResults.AcceptChanges()

                                'If needed, create the final directory... 
                                If (Not IO.Directory.Exists(myDirName)) Then IO.Directory.CreateDirectory(myDirName)

                                Dim allDeletedFiles As String = String.Empty
                                Dim lstAnalyzerResults As List(Of QCResultsDS.tqcResultsRow)
                                Dim myNewQCResultsToExportDS As New QCResultsDS 'TR 29/06/2012 -Data set use to create the xml file.
                                For Each cumDataRow As QCResultsCalculationDS.tQCResultCalculationRow In myQCResultCalculationDS.tQCResultCalculation.Rows
                                    'Get all QC Results for the QCTestSampleID/CQControlLotID/AnalyzerID/RunsGroupNumber
                                    lstAnalyzerResults = (From a As QCResultsDS.tqcResultsRow In allQCResultsToExportDS.tqcResults _
                                                         Where a.AnalyzerID = cumDataRow.AnalyzerID _
                                                        Select a).ToList()

                                    myNewQCResultsToExportDS.Clear()
                                    myNewQCResultsToExportDS.AcceptChanges()

                                    For Each result As QCResultsDS.tqcResultsRow In lstAnalyzerResults
                                        myNewQCResultsToExportDS.tqcResults.ImportRow(result)
                                    Next
                                    myQCResultsToExportDS.AcceptChanges()
                                    myNewQCResultsToExportDS.WriteXml(myDirName & cumDataRow.XMLFileToCreate)

                                    'If the first Cumulated Serie was deleted, then delete also the XML file containing all its results
                                    If (cumDataRow.XMLFileToDelete <> String.Empty) Then
                                        If (Not pSaveXMLFileToTEMPPath) Then
                                            IO.File.Delete(myDirName & cumDataRow.XMLFileToDelete)
                                        Else
                                            If (allDeletedFiles.Length > 0) Then allDeletedFiles &= ", "
                                            allDeletedFiles &= cumDataRow.XMLFileToDelete
                                        End If
                                    End If

                                Next

                                If (pSaveXMLFileToTEMPPath) Then
                                    myGlobalDataTO.SetDatos = allDeletedFiles
                                End If

                            End If
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If


                'myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then
                '        Dim myXMLFileName As String = String.Empty
                '        Dim myFileNameToDelete As String = String.Empty
                '        Dim myQCResultsToExportDS As New QCResultsDS

                '        Dim myQCResultDS As New QCResultsDS
                '        Dim myQCResultCalculationDS As New QCResultsCalculationDS
                '        Dim myQCResultDelegate As New QCResultsDelegate
                '        Dim myCumResultNumDAO As New tqcCumulatedResultsDAO

                '        myGlobalDataTO = myQCResultDelegate.GetDataToCreateCumulateNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '            myQCResultCalculationDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsCalculationDS)

                '            If (myQCResultCalculationDS.tQCResultCalculation.Count > 0) Then
                '                Dim myMinCumResultNum As Integer = 0
                '                Dim myMaxCumResultNum As Integer = 0
                '                Dim myRunsGroupNumber As Integer = myQCResultCalculationDS.tQCResultCalculation(0).RunsGroupNumber

                '                Dim myCumulatedResultDS As New CumulatedResultsDS
                '                Dim myCumulatedResultRow As CumulatedResultsDS.tqcCumulatedResultsRow

                '                'Search on tqcCumulatedResults all Cumulated Series saved for the QCTestSampleID/QCControlID
                '                myGlobalDataTO = myCumResultNumDAO.ReadByQcTestSampleIDQCControlLotIDNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '                    myCumulatedResultDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                '                    'If there is at least a Cumulated Serie, then get the last one and set CumResultsNum = CumResultsNum + 1
                '                    If (myCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
                '                        'Get the MIN(CumResultsNum) and set the value to local variable
                '                        myMinCumResultNum = (From a In myCumulatedResultDS.tqcCumulatedResults _
                '                                           Select a.CumResultsNum).Min

                '                        'Get the MAX(CumResultsNum) and set the value to local variable
                '                        myMaxCumResultNum = (From a In myCumulatedResultDS.tqcCumulatedResults _
                '                                           Select a.CumResultsNum).Max

                '                        'Get value of the General Setting for the maximum number of Cumulated Series 
                '                        Dim myGeneralSettingsDelegate As New GeneralSettingsDelegate
                '                        myGlobalDataTO = myGeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_CUMULATED_QCSERIES.ToString)

                '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '                            Dim maxNumber As Integer = Convert.ToInt32(myGlobalDataTO.SetDatos)
                '                            If (myCumulatedResultDS.tqcCumulatedResults.Count = maxNumber) Then
                '                                'Get the name of the XML File containing the QC Results for the first Cumulate
                '                                Dim xmlName As List(Of String) = (From a In myCumulatedResultDS.tqcCumulatedResults _
                '                                                                 Where a.CumResultsNum = myMinCumResultNum _
                '                                                               And Not a.IsXMLFileNameNull _
                '                                                                Select a.XMLFileName).ToList()
                '                                If (xmlName.Count = 1) Then myFileNameToDelete = xmlName.First.ToString

                '                                'Delete the first Cumulated Serie
                '                                myGlobalDataTO = DeleteFirstCumulatedNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, myMinCumResultNum, "")
                '                                If (Not myGlobalDataTO.HasError) Then
                '                                    Dim mytqcCumulatedResults As New tqcCumulatedResultsDAO
                '                                    myGlobalDataTO = mytqcCumulatedResults.DecrementCumResultsNumNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                '                                End If
                '                            Else
                '                                'Increment the value by 1
                '                                myMaxCumResultNum += 1
                '                            End If
                '                        End If
                '                    Else
                '                        'It is the first Cumulated Serie for the QCTestSampleID/QCControlLotID
                '                        myMaxCumResultNum = 1
                '                    End If

                '                    If (Not myGlobalDataTO.HasError) Then
                '                        'Generate the name of the XML File in which the QC Results included in the Cumulate will be exported
                '                        myXMLFileName = pQCTestSampleID.ToString & "-" & _
                '                                        pQCControlLotID.ToString & "-" & _
                '                                        myRunsGroupNumber.ToString & "-" & _
                '                                        DateTime.Now.ToString("yyyyMMdd") & " " & _
                '                                        DateTime.Now.ToString("HH") & _
                '                                        DateTime.Now.ToString("mm") & _
                '                                        DateTime.Now.ToString("ss") & ".xml"

                '                        'Clear previous values to reuse the DS
                '                        myCumulatedResultDS.tqcCumulatedResults.Clear()

                '                        'Add a row with all values of the new Cumulated Serie and insert it
                '                        myCumulatedResultRow = myCumulatedResultDS.tqcCumulatedResults.NewtqcCumulatedResultsRow()
                '                        myCumulatedResultRow.AnalyzerID = pAnalyzerID
                '                        myCumulatedResultRow.QCTestSampleID = pQCTestSampleID
                '                        myCumulatedResultRow.QCControlLotID = pQCControlLotID
                '                        myCumulatedResultRow.CumResultsNum = myMaxCumResultNum
                '                        myCumulatedResultRow.CumDateTime = DateTime.Now
                '                        myCumulatedResultRow.TotalRuns = myQCResultCalculationDS.tQCResultCalculation(0).N
                '                        myCumulatedResultRow.FirstRunDateTime = myQCResultCalculationDS.tQCResultCalculation(0).FirstRunDateTime
                '                        myCumulatedResultRow.LastRunDateTime = myQCResultCalculationDS.tQCResultCalculation(0).LastRunDateTime
                '                        myCumulatedResultRow.SumResults = myQCResultCalculationDS.tQCResultCalculation(0).SumXi
                '                        myCumulatedResultRow.SumSQRDResults = myQCResultCalculationDS.tQCResultCalculation(0).SumXi2
                '                        myCumulatedResultRow.Mean = myQCResultCalculationDS.tQCResultCalculation(0).Mean

                '                        'Validate if there is more than one Result to accumulate; in that case, SD cannot be calculated and it is set to zero 
                '                        If (myQCResultCalculationDS.tQCResultCalculation(0).N > 1) Then
                '                            'Calculate the SD
                '                            myCumulatedResultRow.SD = CalculateSD(myQCResultCalculationDS)
                '                        Else
                '                            'Set SD = 0 
                '                            myCumulatedResultRow.SD = 0
                '                        End If

                '                        'Inform the name of the XML File containing the QC Results included in the Cumulated 
                '                        myCumulatedResultRow.XMLFileName = myXMLFileName

                '                        'Get the current logged User and set the current DateTime in the audit fields
                '                        Dim myGlobalBase As New GlobalBase
                '                        myCumulatedResultRow.TS_User = GlobalBase.GetSessionInfo.UserName
                '                        myCumulatedResultRow.TS_DateTime = DateTime.Now

                '                        myCumulatedResultDS.tqcCumulatedResults.AddtqcCumulatedResultsRow(myCumulatedResultRow)

                '                        'Insert the new Cumulated Serie for the QCTestSampleID/QCControlID.
                '                        myGlobalDataTO = CreateNEW(dbConnection, myCumulatedResultDS)
                '                        If (Not myGlobalDataTO.HasError) Then
                '                            If (myMaxCumResultNum = 1) Then
                '                                'Insert values of the Cumulated Serie as Last Cumulated Values
                '                                Dim myLastCumValuesDelegate As New LastCumulatedValuesDelegate
                '                                myGlobalDataTO = myLastCumValuesDelegate.AddLastCumValuesNEW(dbConnection, myCumulatedResultDS)
                '                            Else
                '                                'Update Last Cumulated Values by adding data of the Cumulated Serie and then calculating the new Mean and SD values
                '                                Dim myLastCumValuesDelegate As New LastCumulatedValuesDelegate
                '                                myGlobalDataTO = myLastCumValuesDelegate.ModifyLastCumValuesNEW(dbConnection, myCumulatedResultDS)
                '                            End If

                '                            If (Not myGlobalDataTO.HasError) Then
                '                                'Close the RunsGroupNumber informing the CumResultsNum
                '                                Dim myRunsGroupDelegate As New RunsGroupsDelegate
                '                                myGlobalDataTO = myRunsGroupDelegate.CloseRunsGroupNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, _
                '                                                                                       myMaxCumResultNum, myRunsGroupNumber)

                '                                If (Not myGlobalDataTO.HasError) Then
                '                                    'Get all the QC Results included on the RunsGroupNumber and that will be exported to an XML file
                '                                    Dim myQCResultsDelegate As New QCResultsDelegate
                '                                    myGlobalDataTO = myQCResultDelegate.GetQCResultsToExportNEW(dbConnection, pQCTestSampleID, pQCControlLotID, _
                '                                                                                                pAnalyzerID, myRunsGroupNumber)

                '                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '                                        myQCResultsToExportDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

                '                                        'Finally, delete the QC Results
                '                                        myGlobalDataTO = myQCResultDelegate.DeleteByRunsGroupNumNEW(dbConnection, pQCTestSampleID, pQCControlLotID, _
                '                                                                                                    pAnalyzerID, myRunsGroupNumber)
                '                                    End If
                '                                End If
                '                            End If
                '                        End If
                '                    End If
                '                End If
                '            End If
                '        End If

                '        If (Not myGlobalDataTO.HasError) Then
                '            'When the Database Connection was opened locally, then the Commit is executed
                '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                '            'If there were not errors in the process, then export the QC Results to an XML file
                '            Dim myDirName As String = Application.StartupPath & GlobalBase.QCResultsFilesPath
                '            If (myQCResultsToExportDS.tqcResults.Rows.Count > 0) Then
                '                If (pSaveXMLFileToTEMPPath) Then myDirName &= "TEMP\"

                '                'Mark all results as Closed 
                '                For Each qcResultRow As QCResultsDS.tqcResultsRow In myQCResultsToExportDS.tqcResults.Rows
                '                    qcResultRow.ClosedResult = True
                '                Next
                '                myQCResultsToExportDS.tqcResults.AcceptChanges()

                '                'If needed, create the final directory... Write the XML file
                '                If (Not IO.Directory.Exists(myDirName)) Then IO.Directory.CreateDirectory(myDirName)
                '                myQCResultsToExportDS.WriteXml(myDirName & myXMLFileName)
                '            End If

                '            'If the first Cumulated Serie was deleted, then delete also the XML file containing all its results
                '            If (myFileNameToDelete <> String.Empty) Then
                '                If (Not pSaveXMLFileToTEMPPath) Then
                '                    IO.File.Delete(myDirName & myFileNameToDelete)
                '                Else
                '                    myGlobalDataTO.SetDatos = myFileNameToDelete
                '                End If
                '            End If
                '        Else
                '            'When the Database Connection was opened locally, then the Rollback is executed
                '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                '        End If
                '    End If
                'End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.SaveCumulateResult", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the oldest Cumulated Serie for the specified Test/SampleType and Control/Lot
        ''' Used when the maximum number of Cumulated Series for a Test/SampleType and Control/Lot has been reached
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFirstCumSerieNumber">Number of the oldest Cumulated Serie for the informed QCTestSampleID/QCControlLotID</param>
        ''' <param name="pXMLFileName">Name of the XML file containing the individual QC Results included in the first Cumulate</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/06/2011
        ''' Modified by: SA 28/06/2011 - Also delete the XML file containing the individual QC Results included in the Cumulate Serie 
        '''              SA 05/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Private Function DeleteFirstCumulatedNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                 ByVal pAnalyzerID As String, ByVal pFirstCumSerieNumber As Integer, ByVal pXMLFileName As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete the first Cumulated Serie for the specified Test/SampleType and Control/Lot
                        Dim myCumResultNumDAO As New tqcCumulatedResultsDAO
                        myGlobalDataTO = myCumResultNumDAO.DeleteNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pFirstCumSerieNumber)

                        If (Not myGlobalDataTO.HasError) Then
                            'Re-enum the rest of Cumulated Series for the Test/SampleType and Control/Lot
                            Dim myRunsGroupDelegate As New RunsGroupsDelegate
                            myGlobalDataTO = myRunsGroupDelegate.DecrementCumResultsNumNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                        End If

                        Dim myLastCumulatedDelegate As New LastCumulatedValuesDelegate
                        If (Not myGlobalDataTO.HasError) Then
                            'Delete the Last Cumulated Values currently saved for the Test/SampleType and Control/Lot
                            myGlobalDataTO = myLastCumulatedDelegate.DeleteNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Get the remaining Cumulated Series for the Test/SampleType and Control/Lot
                            myGlobalDataTO = myCumResultNumDAO.ReadByQcTestSampleIDQCControlLotIDNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myCumSeriesByControlLotDS As CumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                                If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count > 0) Then
                                    'Recalculate the Last Cumulated Values based in the existing Cumulated Series for the Test/SampleType and Control/Lot 
                                    myGlobalDataTO = RecalculateLastCumulatedValuesNEW(myCumSeriesByControlLotDS)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                                        If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count = 1) Then
                                            'Insert the new calculated Last Cumulated values
                                            myGlobalDataTO = myLastCumulatedDelegate.AddLastCumValuesNEW(dbConnection, myCumSeriesByControlLotDS)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            If (pXMLFileName <> String.Empty) Then
                                'Delete the XML File from the QC XML Files path
                                IO.File.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & pXMLFileName)
                            End If
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
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
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.DeleteFirstCumulated", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Recalculate the last cumulated values for a QCTestSampleID/QCControlLotID according the specified group
        ''' of Cumulated Series
        ''' </summary>
        ''' <param name="pCumResultValuesDS">Typed DataSet CumulatedResultsDS containing all Cumulated Series for an
        '''                                  specific QCTestSampleID and QCControlLotID</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS containing the calculated last cumulated values</returns>
        ''' <remarks>
        ''' Created by:  SA 16/06/2011
        ''' Modified by: SA 28/06/2011 - Changed calculation of SD (mySumResultsSQRD)
        '''              SA 05/06/2012 - Field AnalyzerID informed in the typed Dataset CumulatedResultsDS to return
        ''' </remarks>
        Private Function RecalculateLastCumulatedValuesNEW(ByVal pCumResultValuesDS As CumulatedResultsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                Dim myN As Integer = 0
                Dim mySumResults As Double = 0
                Dim mySumSQRDResults As Double = 0
                Dim mySumResultsSQRD As Double = 0
                Dim myMean As Single = 0
                Dim mySD As Single = 0

                myN = pCumResultValuesDS.tqcCumulatedResults(0).TotalRuns
                mySumResults = pCumResultValuesDS.tqcCumulatedResults(0).SumResults
                mySumSQRDResults = pCumResultValuesDS.tqcCumulatedResults(0).SumSQRDResults
                mySumResultsSQRD = Math.Pow(mySumResults, 2)
                myMean = pCumResultValuesDS.tqcCumulatedResults(0).Mean
                mySD = pCumResultValuesDS.tqcCumulatedResults(0).SD

                If (pCumResultValuesDS.tqcCumulatedResults.Rows.Count > 1) Then
                    For i As Integer = 1 To pCumResultValuesDS.tqcCumulatedResults.Rows.Count - 1
                        myN += pCumResultValuesDS.tqcCumulatedResults(i).TotalRuns
                        mySumResults += pCumResultValuesDS.tqcCumulatedResults(i).SumResults
                        mySumSQRDResults += pCumResultValuesDS.tqcCumulatedResults(i).SumSQRDResults
                    Next

                    mySumResultsSQRD = Math.Pow(mySumResults, 2)
                    myMean = Convert.ToSingle(mySumResults / myN)
                    mySD = Convert.ToSingle(Math.Sqrt(((myN * mySumSQRDResults) - mySumResultsSQRD) / (myN * (myN - 1))))
                End If

                'Add the calculated values to the DS to return
                Dim myLastCumulatedDS As New CumulatedResultsDS
                Dim myLastCumulatedRow As CumulatedResultsDS.tqcCumulatedResultsRow

                myLastCumulatedRow = myLastCumulatedDS.tqcCumulatedResults.NewtqcCumulatedResultsRow
                myLastCumulatedRow.AnalyzerID = pCumResultValuesDS.tqcCumulatedResults(0).AnalyzerID
                myLastCumulatedRow.QCTestSampleID = pCumResultValuesDS.tqcCumulatedResults(0).QCTestSampleID
                myLastCumulatedRow.QCControlLotID = pCumResultValuesDS.tqcCumulatedResults(0).QCControlLotID
                myLastCumulatedRow.TotalRuns = myN
                myLastCumulatedRow.SumResults = mySumResults
                myLastCumulatedRow.SumSQRDResults = mySumSQRDResults
                myLastCumulatedRow.Mean = myMean
                myLastCumulatedRow.SD = mySD

                myLastCumulatedDS.tqcCumulatedResults.AddtqcCumulatedResultsRow(myLastCumulatedRow)

                resultData.SetDatos = myLastCumulatedDS
                resultData.HasError = False
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.RecalculateLastCumulatedValues", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Calculate the Standard Deviation for a new Cumulated Serie 
        ''' </summary>
        ''' <param name="pQCResultCalculationRow">Row of a typed DataSet QCResultsCalculationDS containing all data needed
        '''                                       to calculate the SD of the Cumulated Serie</param>
        ''' <returns>A Double value containing the calculated SD</returns>
        ''' <remarks>
        ''' Created by:  TR 23/05/2011
        ''' Modified by: SA 02/06/2011 - Changed the returned value from Double to Single
        '''              JV 06/11/2013 - issue #1185 Absolut value in sqrt to avoid bad result
        ''' </remarks>
        Private Function CalculateSDNEW(ByVal pQCResultCalculationRow As QCResultsCalculationDS.tQCResultCalculationRow) As Single
            Dim myCalculateSD As Single = 0

            Try
                'Assign values to local variables to do the calculation
                Dim myN As Integer = pQCResultCalculationRow.N
                Dim mySumXi As Double = pQCResultCalculationRow.SumXi
                Dim mySumXi2 As Double = pQCResultCalculationRow.SumXi2

                myCalculateSD = Convert.ToSingle(Math.Sqrt(Math.Abs(((myN * mySumXi2) - (mySumXi * mySumXi)) / (myN * (myN - 1)))))

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.CalculateSD", EventLogEntryType.Error, False)
                Throw ex
            End Try
            Return myCalculateSD
        End Function
#End Region

#Region "TO DELETE - OLD METHODS"
        '''' <summary>
        '''' Create a new Cumulated Serie for a QCTestSampleID/QCControlLotID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 20/05/2011
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumulatedResultDS As CumulatedResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myCumResultNumDAO As New tqcCumulatedResultsDAO
        '                myGlobalDataTO = myCumResultNumDAO.Create(dbConnection, pCumulatedResultDS)

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.Create", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete a group of Cumulated Series for a Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pCumSeriesToDelete">Typed DataSet containing all the Cumulated Series to delete</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' Modified by: SA 27/06/2011 - Delete the XML File 
        '''' </remarks>
        'Public Function DeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCumSeriesToDelete As CumulatedResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myCumResultNumDAO As New tqcCumulatedResultsDAO
        '                Dim myRunsGroupDelegate As New RunsGroupsDelegate
        '                Dim myCumSeriesByControlLotDS As New CumulatedResultsDS
        '                Dim myLastCumulatedDelegate As New LastCumulatedValuesDelegate

        '                Dim lstXMLNames As List(Of String) = (From a In pCumSeriesToDelete.tqcCumulatedResults _
        '                                                 Where Not a.IsXMLFileNameNull _
        '                                                    Select a.XMLFileName).ToList

        '                For Each cumSerie As CumulatedResultsDS.tqcCumulatedResultsRow In pCumSeriesToDelete.tqcCumulatedResults.Rows
        '                    'Delete the RunsGroup of the results included in the Cumulated Serie
        '                    myGlobalDataTO = myRunsGroupDelegate.Delete(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID, cumSerie.CumResultsNum)
        '                    If (myGlobalDataTO.HasError) Then Exit For

        '                    'Delete the Cumulated Serie
        '                    myGlobalDataTO = myCumResultNumDAO.Delete(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID, cumSerie.CumResultsNum)
        '                    If (myGlobalDataTO.HasError) Then Exit For

        '                    'Delete the Last Cumulated Values currently saved for the Test/SampleType and Control/Lot
        '                    myGlobalDataTO = myLastCumulatedDelegate.Delete(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID)
        '                    If (myGlobalDataTO.HasError) Then Exit For

        '                    'Get the remaining Cumulated Series for the Test/SampleType and Control/Lot
        '                    myGlobalDataTO = myCumResultNumDAO.ReadByQcTestSampleIDQCControlLotID(dbConnection, cumSerie.QCTestSampleID, cumSerie.QCControlLotID)
        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

        '                        If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count > 0) Then
        '                            'Recalculate the Last Cumulated Values based in the existing Cumulated Series for the Test/SampleType and Control/Lot 
        '                            myGlobalDataTO = RecalculateLastCumulatedValuesOLD(myCumSeriesByControlLotDS)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

        '                                If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count = 1) Then
        '                                    'Insert the new calculated Last Cumulated values
        '                                    myGlobalDataTO = myLastCumulatedDelegate.AddLastCumValues(dbConnection, myCumSeriesByControlLotDS)
        '                                    If (myGlobalDataTO.HasError) Then Exit For
        '                                End If
        '                            Else
        '                                'Error recalculating the last cumulated values
        '                                Exit For
        '                            End If
        '                        End If
        '                    Else
        '                        'Error getting the Cumulated Series for the Test/SampleType and Control/Lot
        '                        Exit For
        '                    End If
        '                Next

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

        '                    'Delete all XML Files containing the Results of the deleted Cumulated Series
        '                    For Each xmlFile As String In lstXMLNames
        '                        IO.File.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & xmlFile)
        '                    Next
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.Delete", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all Controls/Lots that have Cumulated Series for the specified Test/SampleType in the informed period of time
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        '''' <param name="pDateTo">Date To to search Cumulated Series</param>
        '''' <param name="pCumulatedSeriesDS">Typed DataSet CumulatedResultsDS containing all Cumulated Series for all Controls/Lots linked 
        ''''                                  to the informed Test/SampleType in the specified range of dates</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCCumulatedSummaryDS containing summary data of all Controls/Lots 
        ''''          that have Cumulated Series for the specified Test/SampleType in the informed period of time</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' Modified by: SA 07/07/2011 - For Controls/Lots having only one Cumulated Serie, set to Null fields Mean, SD, CV, and Min/Max Range
        '''' </remarks>
        'Public Function GetControlsLotsWithCumulatedSeriesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pDateFrom As DateTime, _
        '                                                   ByVal pDateTo As DateTime, ByVal pCumulatedSeriesDS As CumulatedResultsDS) As GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myCumResultsDAO As New tqcCumulatedResultsDAO
        '                myGlobalDataTO = myCumResultsDAO.GetControlsLotsWithCumulatedSeriesOLD(dbConnection, pQCTestSampleID, pDateFrom, pDateTo)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myQCCumSummaryDS As QCCumulatedSummaryDS = DirectCast(myGlobalDataTO.SetDatos, QCCumulatedSummaryDS)

        '                    Dim myMean As Single = 0
        '                    Dim mySD As Single = 0

        '                    Dim myCumSeriesByControlLotDS As New CumulatedResultsDS
        '                    Dim lstCumSeriesByControlLot As List(Of CumulatedResultsDS.tqcCumulatedResultsRow)
        '                    For Each controlLot As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In myQCCumSummaryDS.QCCumulatedSummaryTable.Rows
        '                        'Get the Cumulated Series for the Control/Lot in process
        '                        lstCumSeriesByControlLot = (From a As CumulatedResultsDS.tqcCumulatedResultsRow In pCumulatedSeriesDS.tqcCumulatedResults _
        '                                                   Where a.QCControlLotID = controlLot.QCControlLotID _
        '                                                Order By a.CumDateTime _
        '                                                  Select a).ToList

        '                        If (lstCumSeriesByControlLot.Count = 1) Then
        '                            controlLot.BeginEdit()
        '                            controlLot.SetMeanNull()
        '                            controlLot.SetSDNull()
        '                            controlLot.SetCVNull()
        '                            controlLot.SetMinRangeNull()
        '                            controlLot.SetMaxRangeNull()
        '                            controlLot.EndEdit()

        '                        ElseIf (lstCumSeriesByControlLot.Count > 1) Then
        '                            myCumSeriesByControlLotDS.Clear()
        '                            For Each cumSerie As CumulatedResultsDS.tqcCumulatedResultsRow In lstCumSeriesByControlLot
        '                                myCumSeriesByControlLotDS.tqcCumulatedResults.ImportRow(cumSerie)
        '                            Next

        '                            myGlobalDataTO = RecalculateLastCumulatedValuesOLD(myCumSeriesByControlLotDS)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

        '                                If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count = 1) Then
        '                                    'Update calculated fields for the Control/Lot
        '                                    myMean = myCumSeriesByControlLotDS.tqcCumulatedResults.First.Mean
        '                                    mySD = myCumSeriesByControlLotDS.tqcCumulatedResults.First.SD

        '                                    controlLot.BeginEdit()
        '                                    controlLot.Mean = myMean
        '                                    controlLot.SD = mySD
        '                                    controlLot.CV = (mySD / myMean) * 100
        '                                    controlLot.MinRange = myMean - (controlLot.RejectionCriteria * mySD)
        '                                    controlLot.MaxRange = myMean + (controlLot.RejectionCriteria * mySD)
        '                                    controlLot.EndEdit()
        '                                End If
        '                            Else
        '                                'Error recalculating last cumulated values
        '                                Exit For
        '                            End If
        '                        End If
        '                    Next

        '                    If (Not myGlobalDataTO.HasError) Then
        '                        If (myQCCumSummaryDS.QCCumulatedSummaryTable.Count > 0) Then
        '                            'Mark the first Control/Lot as selected and return the DS
        '                            myQCCumSummaryDS.QCCumulatedSummaryTable.First.Selected = True
        '                            myQCCumSummaryDS.AcceptChanges()
        '                        End If

        '                        myGlobalDataTO.SetDatos = myQCCumSummaryDS
        '                        myGlobalDataTO.HasError = False
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.GetControlsLotsWithCumulatedSeries", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get details of all Cumulated Series of the the specified Test/SampleType (all Controls/Lots) in the informed period of time
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pDateFrom">Date From to search Cumulated Series</param>
        '''' <param name="pDateTo">Date To to search Cumulated Series</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS containing data of all Cumulated Series saved for 
        ''''          the specified Test/SampleType (all Controls/Lots) in the informed period of time</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' Modified by: TR 29/06/2011 - Added the calculation for VisibleCumResultNum.
        '''' </remarks>
        'Public Function GetCumulatedSeriesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pDateFrom As DateTime, _
        '                                   ByVal pDateTo As DateTime) As GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myCumResultsDAO As New tqcCumulatedResultsDAO
        '                myGlobalDataTO = myCumResultsDAO.GetCumulatedSeries(dbConnection, pQCTestSampleID, pDateFrom, pDateTo)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myCumulatedResultsDS As CumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)
        '                    Dim myControlsLotsIDList As List(Of Integer) = (From a In myCumulatedResultsDS.tqcCumulatedResults _
        '                                                                  Select a.QCControlLotID Distinct).ToList()

        '                    Dim mySecuence As Integer
        '                    Dim myCumulateResultList As New List(Of CumulatedResultsDS.tqcCumulatedResultsRow)
        '                    For Each controlLotID As Integer In myControlsLotsIDList
        '                        mySecuence = 0
        '                        myCumulateResultList = (From a In myCumulatedResultsDS.tqcCumulatedResults _
        '                                               Where a.QCControlLotID = controlLotID _
        '                                              Select a).ToList()
        '                        'Set the visible cumResultNum value.
        '                        For Each cumResultRow As CumulatedResultsDS.tqcCumulatedResultsRow In myCumulateResultList
        '                            mySecuence += 1
        '                            cumResultRow.VisibleCumResultNumber = mySecuence
        '                        Next
        '                    Next
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.GetCumulatedSeries", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the oldest date of a cumulated serie for the specified QCTestSampleID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        '''' <remarks>
        '''' Created by:  SA 15/06/2011
        '''' </remarks>
        'Public Function GetMinCumDateTimeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myCumResultsDAO As New tqcCumulatedResultsDAO
        '                myGlobalDataTO = myCumResultsDAO.GetMinCumDateTime(dbConnection, pQCTestSampleID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.GetMinCumDateTime", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCTestSampleID/QCControlLotID, create a new Cumulated Serie with non excluded QC Results in the Runs Group that is open.
        '''' After create the Cumulated Serie, the Runs Group is marked as closed and all QC Results included in it are also marked as closed
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">QC TestSample ID</param>
        '''' <param name="pQCControlLotID">QC ControlLot ID</param>
        '''' <param name="pSaveXMLFileToTEMPPath">Optional parameter. When True, it indicates the generated XML files have to be created in a
        ''''                                      temporary path</param>
        '''' <returns>GlobalDataTO containing success/error information. If the first Cumulated Serie was deleted, and the DB Transaction
        ''''          was not opened locally, the GlobalDataTO also contains the name of the XML file with the results of the deleted serie</returns>
        '''' <remarks>
        '''' Created by:  TR 23/05/2011
        '''' Modified by: SA 01/06/2011 - Added code to save also last cumulated values for the specified QCTestSampleID/QCControlLotID
        ''''              SA 21/06/2011 - Verify if the Maximum Number of Cumulated Serie by QCTestSampleID/QCControlLotID has been reached,
        ''''                              and in this case, delete the oldest Cumulated Serie and resorting the rest before insert the new 
        ''''                              Cumulated Serie
        ''''              DL 27/06/2011 - Added export of Cumulated QC Results to an XML
        ''''              SA 27/06/2011 - Added the name of the XML file in the DS used to create the Cumulated Serie. The XML file has to be wrote just 
        ''''                              before execute the Commit Transaction. QC Results included in the Cumulated Runs Group are physically deleted.
        ''''                              Added optional parameter to indicate if the XML files have to be created in a TEMP path due to the control of
        ''''                              the DB Transaction is done in another function (and in this case, after executing the Commit in that method,
        ''''                              the generated XML files are moved from the TEMP path to the final one)
        ''''              SA 05/07/2011 - Changed the way of calculate the SD
        ''''              SA 08/07/2011 - If the first Cumulated Serie was deleted, delete the XML File after Commit the DB Transaction or, if the
        ''''                              DB Transaction was not opened locally, return the name of the XML File in the GlobalDataTO
        '''' </remarks>
        'Public Function SaveCumulateResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                   Optional ByVal pSaveXMLFileToTEMPPath As Boolean = False) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myXMLFileName As String = String.Empty
        '                Dim myFileNameToDelete As String = String.Empty
        '                Dim myQCResultsToExportDS As New QCResultsDS

        '                Dim myQCResultDS As New QCResultsDS
        '                Dim myQCResultCalculationDS As New QCResultsCalculationDS
        '                Dim myQCResultDelegate As New QCResultsDelegate
        '                Dim myCumResultNumDAO As New tqcCumulatedResultsDAO

        '                'Get calculated data needed to create the new Cumulated Serie
        '                myGlobalDataTO = myQCResultDelegate.GetDataToCreateCumulate(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    myQCResultCalculationDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsCalculationDS)
        '                    If (myQCResultCalculationDS.tQCResultCalculation.Count > 0) Then
        '                        Dim myMinCumResultNum As Integer = 0
        '                        Dim myMaxCumResultNum As Integer = 0
        '                        Dim myRunsGroupNumber As Integer = myQCResultCalculationDS.tQCResultCalculation(0).RunsGroupNumber

        '                        Dim myCumulatedResultDS As New CumulatedResultsDS
        '                        Dim myCumulatedResultRow As CumulatedResultsDS.tqcCumulatedResultsRow

        '                        'Search on tqcCumulatedResults all Cumulated Series saved for the QCTestSampleID/QCControlID
        '                        myGlobalDataTO = myCumResultNumDAO.ReadByQcTestSampleIDQCControlLotID(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            myCumulatedResultDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

        '                            'If there is at least a Cumulated Serie, then get the last one and set CumResultsNum = CumResultsNum + 1
        '                            If (myCumulatedResultDS.tqcCumulatedResults.Count > 0) Then
        '                                'Get the MIN(CumResultsNum) and set the value to local variable
        '                                myMinCumResultNum = (From a In myCumulatedResultDS.tqcCumulatedResults _
        '                                                   Select a.CumResultsNum).Min

        '                                'Get the MAX(CumResultsNum) and set the value to local variable
        '                                myMaxCumResultNum = (From a In myCumulatedResultDS.tqcCumulatedResults _
        '                                                   Select a.CumResultsNum).Max

        '                                'Get value of the General Setting for the maximum number of Cumulated Series 
        '                                Dim myGeneralSettingsDelegate As New GeneralSettingsDelegate
        '                                myGlobalDataTO = myGeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_CUMULATED_QCSERIES.ToString)

        '                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                    Dim maxNumber As Integer = Convert.ToInt32(myGlobalDataTO.SetDatos)
        '                                    If (myCumulatedResultDS.tqcCumulatedResults.Count = maxNumber) Then
        '                                        'Get the name of the XML File containing the QC Results for the first Cumulate
        '                                        Dim xmlName As List(Of String) = (From a In myCumulatedResultDS.tqcCumulatedResults _
        '                                                                         Where a.CumResultsNum = myMinCumResultNum _
        '                                                                       And Not a.IsXMLFileNameNull _
        '                                                                        Select a.XMLFileName).ToList()
        '                                        If (xmlName.Count = 1) Then myFileNameToDelete = xmlName.First.ToString

        '                                        'Delete the first Cumulated Serie
        '                                        myGlobalDataTO = DeleteFirstCumulated(dbConnection, pQCTestSampleID, pQCControlLotID, myMinCumResultNum, "")
        '                                        If (Not myGlobalDataTO.HasError) Then
        '                                            Dim mytqcCumulatedResults As New tqcCumulatedResultsDAO
        '                                            myGlobalDataTO = mytqcCumulatedResults.DecrementCumResultsNum(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                                        End If
        '                                    Else
        '                                        'Increment the value by 1
        '                                        myMaxCumResultNum += 1
        '                                    End If
        '                                End If
        '                            Else
        '                                'It is the first Cumulated Serie for the QCTestSampleID/QCControlLotID
        '                                myMaxCumResultNum = 1
        '                            End If

        '                            If (Not myGlobalDataTO.HasError) Then
        '                                'Generate the name of the XML File in which the QC Results included in the Cumulate will be exported
        '                                myXMLFileName = pQCTestSampleID.ToString & "-" & _
        '                                                pQCControlLotID.ToString & "-" & _
        '                                                myRunsGroupNumber.ToString & "-" & _
        '                                                DateTime.Now.ToString("yyyyMMdd") & " " & _
        '                                                DateTime.Now.ToString("HH") & _
        '                                                DateTime.Now.ToString("mm") & _
        '                                                DateTime.Now.ToString("ss") & ".xml"

        '                                'Clear previous values to reuse the DS
        '                                myCumulatedResultDS.tqcCumulatedResults.Clear()

        '                                'Add a row with all values of the new Cumulated Serie and insert it
        '                                myCumulatedResultRow = myCumulatedResultDS.tqcCumulatedResults.NewtqcCumulatedResultsRow()
        '                                myCumulatedResultRow.QCTestSampleID = pQCTestSampleID
        '                                myCumulatedResultRow.QCControlLotID = pQCControlLotID
        '                                myCumulatedResultRow.CumResultsNum = myMaxCumResultNum
        '                                myCumulatedResultRow.CumDateTime = DateTime.Now
        '                                myCumulatedResultRow.TotalRuns = myQCResultCalculationDS.tQCResultCalculation(0).N
        '                                myCumulatedResultRow.FirstRunDateTime = myQCResultCalculationDS.tQCResultCalculation(0).FirstRunDateTime
        '                                myCumulatedResultRow.LastRunDateTime = myQCResultCalculationDS.tQCResultCalculation(0).LastRunDateTime
        '                                myCumulatedResultRow.SumResults = myQCResultCalculationDS.tQCResultCalculation(0).SumXi
        '                                myCumulatedResultRow.SumSQRDResults = myQCResultCalculationDS.tQCResultCalculation(0).SumXi2
        '                                myCumulatedResultRow.Mean = myQCResultCalculationDS.tQCResultCalculation(0).Mean

        '                                'Validate if there is more than one Result to accumulate; in that case, SD cannot be calculated and it is set to zero 
        '                                If (myQCResultCalculationDS.tQCResultCalculation(0).N > 1) Then
        '                                    'Calculate the SD
        '                                    myCumulatedResultRow.SD = CalculateSD(myQCResultCalculationDS)
        '                                Else
        '                                    'Set SD = 0 
        '                                    myCumulatedResultRow.SD = 0
        '                                End If

        '                                'Inform the name of the XML File containing the QC Results included in the Cumulated 
        '                                myCumulatedResultRow.XMLFileName = myXMLFileName

        '                                'Get the current logged User and set the current DateTime in the audit fields
        '                                Dim myGlobalBase As New GlobalBase
        '                                myCumulatedResultRow.TS_User = GlobalBase.GetSessionInfo.UserName
        '                                myCumulatedResultRow.TS_DateTime = DateTime.Now

        '                                myCumulatedResultDS.tqcCumulatedResults.AddtqcCumulatedResultsRow(myCumulatedResultRow)

        '                                'Insert the new Cumulated Serie for the QCTestSampleID/QCControlID.
        '                                myGlobalDataTO = CreateOLD(dbConnection, myCumulatedResultDS)
        '                                If (Not myGlobalDataTO.HasError) Then
        '                                    If (myMaxCumResultNum = 1) Then
        '                                        'Insert values of the Cumulated Serie as Last Cumulated Values
        '                                        Dim myLastCumValuesDelegate As New LastCumulatedValuesDelegate
        '                                        myGlobalDataTO = myLastCumValuesDelegate.AddLastCumValues(dbConnection, myCumulatedResultDS)
        '                                    Else
        '                                        'Update Last Cumulated Values by adding data of the Cumulated Serie and then calculating the new Mean and SD values
        '                                        Dim myLastCumValuesDelegate As New LastCumulatedValuesDelegate
        '                                        myGlobalDataTO = myLastCumValuesDelegate.ModifyLastCumValues(dbConnection, myCumulatedResultDS)
        '                                    End If

        '                                    If (Not myGlobalDataTO.HasError) Then
        '                                        'Close the RunsGroupNumber informing the CumResultsNum
        '                                        Dim myRunsGroupDelegate As New RunsGroupsDelegate
        '                                        myGlobalDataTO = myRunsGroupDelegate.CloseRunsGroup(dbConnection, pQCTestSampleID, pQCControlLotID, myMaxCumResultNum, myRunsGroupNumber)

        '                                        If (Not myGlobalDataTO.HasError) Then
        '                                            'Get all the QC Results included on the RunsGroupNumber and that will be exported to an XML file
        '                                            Dim myQCResultsDelegate As New QCResultsDelegate
        '                                            myGlobalDataTO = myQCResultDelegate.GetQCResultsToExport(dbConnection, pQCTestSampleID, pQCControlLotID, myRunsGroupNumber)

        '                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                                myQCResultsToExportDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

        '                                                'Finally, delete the QC Results
        '                                                myGlobalDataTO = myQCResultDelegate.DeleteByRunsGroupNum(dbConnection, pQCTestSampleID, pQCControlLotID, myRunsGroupNumber)
        '                                            End If
        '                                        End If
        '                                    End If
        '                                End If
        '                            End If
        '                        End If
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

        '                    'If there were not errors in the process, then export the QC Results to an XML file
        '                    Dim myDirName As String = Application.StartupPath & GlobalBase.QCResultsFilesPath
        '                    If (myQCResultsToExportDS.tqcResults.Rows.Count > 0) Then
        '                        If (pSaveXMLFileToTEMPPath) Then myDirName &= "TEMP\"

        '                        'Mark all results as Closed 
        '                        For Each qcResultRow As QCResultsDS.tqcResultsRow In myQCResultsToExportDS.tqcResults.Rows
        '                            qcResultRow.ClosedResult = True
        '                        Next
        '                        myQCResultsToExportDS.tqcResults.AcceptChanges()

        '                        'If needed, create the final directory... Write the XML file
        '                        If (Not IO.Directory.Exists(myDirName)) Then IO.Directory.CreateDirectory(myDirName)
        '                        myQCResultsToExportDS.WriteXml(myDirName & myXMLFileName)
        '                    End If

        '                    'If the first Cumulated Serie was deleted, then delete also the XML file containing all its results
        '                    If (myFileNameToDelete <> String.Empty) Then
        '                        If (Not pSaveXMLFileToTEMPPath) Then
        '                            IO.File.Delete(myDirName & myFileNameToDelete)
        '                        Else
        '                            myGlobalDataTO.SetDatos = myFileNameToDelete
        '                        End If
        '                    End If
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.SaveCumulateResult", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Calculate the Standard Deviation for a new Cumulated Serie 
        '''' </summary>
        '''' <param name="pQCResultCalculationDS">Typed DataSet QCResultsCalculationDS containing all data needed
        ''''                                      to calculate the SD of the Cumulated Serie</param>
        '''' <returns>A Double value containing the calculated SD</returns>
        '''' <remarks>
        '''' Created by:  TR 23/05/2011
        '''' Modified by: SA 02/06/2011 - Changed the returned value from Double to Single
        '''' </remarks>
        'Private Function CalculateSDOLD(ByVal pQCResultCalculationDS As QCResultsCalculationDS) As Single
        '    Dim myCalculateSD As Single = 0

        '    Try
        '        Dim myN As Integer = 0
        '        Dim mySumXi As Double = 0
        '        Dim mySumXi2 As Double = 0

        '        If (pQCResultCalculationDS.tQCResultCalculation.Count > 0) Then
        '            'Assign values to local variables to do the calculation
        '            myN = pQCResultCalculationDS.tQCResultCalculation(0).N
        '            mySumXi = pQCResultCalculationDS.tQCResultCalculation(0).SumXi
        '            mySumXi2 = pQCResultCalculationDS.tQCResultCalculation(0).SumXi2

        '            myCalculateSD = Convert.ToSingle(Math.Sqrt(((myN * mySumXi2) - (mySumXi * mySumXi)) / (myN * (myN - 1))))
        '        End If
        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.CalculateSD", EventLogEntryType.Error, False)
        '        Throw ex
        '    End Try
        '    Return myCalculateSD
        'End Function

        '''' <summary>
        '''' Delete the oldest Cumulated Serie for the specified Test/SampleType and Control/Lot
        '''' Used when the maximum number of Cumulated Series for a Test/SampleType and Control/Lot has been reached
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pFirstCumSerieNumber">Number of the oldest Cumulated Serie for the informed QCTestSampleID/QCControlLotID</param>
        '''' <param name="pXMLFileName">Name of the XML file containing the individual QC Results included in the first Cumulate</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 22/06/2011
        '''' Modified by: SA 28/06/2011 - Also delete the XML file containing the individual QC Results included in the Cumulate Serie 
        '''' </remarks>
        'Private Function DeleteFirstCumulatedOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                      ByVal pFirstCumSerieNumber As Integer, ByVal pXMLFileName As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Delete the first Cumulated Serie for the specified Test/SampleType and Control/Lot
        '                Dim myCumResultNumDAO As New tqcCumulatedResultsDAO
        '                myGlobalDataTO = myCumResultNumDAO.DeleteOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pFirstCumSerieNumber)

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Re-enum the rest of Cumulated Series for the Test/SampleType and Control/Lot
        '                    Dim myRunsGroupDelegate As New RunsGroupsDelegate
        '                    myGlobalDataTO = myRunsGroupDelegate.DecrementCumResultsNumOLD(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                End If

        '                Dim myLastCumulatedDelegate As New LastCumulatedValuesDelegate
        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Delete the Last Cumulated Values currently saved for the Test/SampleType and Control/Lot
        '                    myGlobalDataTO = myLastCumulatedDelegate.DeleteOLD(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Get the remaining Cumulated Series for the Test/SampleType and Control/Lot
        '                    myGlobalDataTO = myCumResultNumDAO.ReadByQcTestSampleIDQCControlLotIDOLD(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim myCumSeriesByControlLotDS As CumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

        '                        If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count > 0) Then
        '                            'Recalculate the Last Cumulated Values based in the existing Cumulated Series for the Test/SampleType and Control/Lot 
        '                            myGlobalDataTO = RecalculateLastCumulatedValuesOLD(myCumSeriesByControlLotDS)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                myCumSeriesByControlLotDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

        '                                If (myCumSeriesByControlLotDS.tqcCumulatedResults.Rows.Count = 1) Then
        '                                    'Insert the new calculated Last Cumulated values
        '                                    myGlobalDataTO = myLastCumulatedDelegate.AddLastCumValues(dbConnection, myCumSeriesByControlLotDS)
        '                                End If
        '                            End If
        '                        End If
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

        '                    If (pXMLFileName <> String.Empty) Then
        '                        'Delete the XML File from the QC XML Files path
        '                        IO.File.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & pXMLFileName)
        '                    End If
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.DeleteFirstCumulated", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Recalculate the last cumulated values for a QCTestSampleID/QCControlLotID according the specified group
        '''' of Cumulated Series
        '''' </summary>
        '''' <param name="pCumResultValuesDS">Typed DataSet CumulatedResultsDS containing all Cumulated Series for an
        ''''                                  specific QCTestSampleID and QCControlLotID</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CumulatedResultsDS containing the calculated last cumulated values</returns>
        '''' <remarks>
        '''' Created by:  SA 16/06/2011
        '''' Modified by: SA 28/06/2011 - Changed calculation of SD (mySumResultsSQRD)
        '''' </remarks>
        'Private Function RecalculateLastCumulatedValuesOLD(ByVal pCumResultValuesDS As CumulatedResultsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Try
        '        Dim myN As Integer = 0
        '        Dim mySumResults As Double = 0
        '        Dim mySumSQRDResults As Double = 0
        '        Dim mySumResultsSQRD As Double = 0
        '        Dim myMean As Single = 0
        '        Dim mySD As Single = 0

        '        myN = pCumResultValuesDS.tqcCumulatedResults(0).TotalRuns
        '        mySumResults = pCumResultValuesDS.tqcCumulatedResults(0).SumResults
        '        mySumSQRDResults = pCumResultValuesDS.tqcCumulatedResults(0).SumSQRDResults
        '        mySumResultsSQRD = Math.Pow(mySumResults, 2)
        '        myMean = pCumResultValuesDS.tqcCumulatedResults(0).Mean
        '        mySD = pCumResultValuesDS.tqcCumulatedResults(0).SD

        '        If (pCumResultValuesDS.tqcCumulatedResults.Rows.Count > 1) Then
        '            For i As Integer = 1 To pCumResultValuesDS.tqcCumulatedResults.Rows.Count - 1
        '                myN += pCumResultValuesDS.tqcCumulatedResults(i).TotalRuns
        '                mySumResults += pCumResultValuesDS.tqcCumulatedResults(i).SumResults
        '                mySumSQRDResults += pCumResultValuesDS.tqcCumulatedResults(i).SumSQRDResults
        '            Next

        '            mySumResultsSQRD = Math.Pow(mySumResults, 2)
        '            myMean = Convert.ToSingle(mySumResults / myN)
        '            mySD = Convert.ToSingle(Math.Sqrt(((myN * mySumSQRDResults) - mySumResultsSQRD) / (myN * (myN - 1))))
        '        End If

        '        'Add the calculated values to the DS to return
        '        Dim myLastCumulatedDS As New CumulatedResultsDS
        '        Dim myLastCumulatedRow As CumulatedResultsDS.tqcCumulatedResultsRow

        '        myLastCumulatedRow = myLastCumulatedDS.tqcCumulatedResults.NewtqcCumulatedResultsRow
        '        myLastCumulatedRow.QCTestSampleID = pCumResultValuesDS.tqcCumulatedResults(0).QCTestSampleID
        '        myLastCumulatedRow.QCControlLotID = pCumResultValuesDS.tqcCumulatedResults(0).QCControlLotID
        '        myLastCumulatedRow.TotalRuns = myN
        '        myLastCumulatedRow.SumResults = mySumResults
        '        myLastCumulatedRow.SumSQRDResults = mySumSQRDResults
        '        myLastCumulatedRow.Mean = myMean
        '        myLastCumulatedRow.SD = mySD

        '        myLastCumulatedDS.tqcCumulatedResults.AddtqcCumulatedResultsRow(myLastCumulatedRow)

        '        resultData.SetDatos = myLastCumulatedDS
        '        resultData.HasError = False
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CumulatedResultsDelegate.RecalculateLastCumulatedValues", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class
End Namespace



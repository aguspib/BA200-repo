Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.Windows.Forms

Namespace Biosystems.Ax00.BL

    Public Class QCResultsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Insert new QC Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.CreateNEW(dbConnection, pQCResultsDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the number of non cumulated and not excluded QC Results for the specified QCTestSampleID/QCControlLotID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of non cumulated and not excluded QC Results
        '''          for the specified QCTestSampleID/QCControlLotID/AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  SA 20/06/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function CountNonCumulatedResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                    ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.CountNonCumulatedResultsNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.CountNonCumulatedResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Return the number of results for and specific runsgroup where the included mean is true.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pQCTestSampleID"></param>
        ''' <param name="pQCControlLotID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pCumSerieNum"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR/SA</remarks>
        Public Function CountStatisticsSResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                   ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String, ByVal pCumSerieNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.CountStatisticsSResults(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pCumSerieNum)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.CountStatisticsSResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified Control/Lot, create a new cumulated serie for each one of the selected Tests/SampleTypes 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCSummaryByTestDS">Typed DataSet QCSummaryByTestSampleDS containing all Tests/SampleTypes linked to the
        '''                                  informed Control/Lot with a flag that indicates for each one if its results have to
        '''                                  be cumulated</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/06/2011
        ''' Modified by: SA 27/06/2011 - Added management of the created XML files
        '''              SA 19/10/2011 - Management of deletion of XML Files when the first Cumulated Serie was deleted was bad done due to 
        '''                              the name of the file was not saved inside the For/Next, then it works only for the last processed 
        '''                              Test/SampleType, but not for the previous ones 
        '''              SA 06/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function CumulateControlResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCSummaryByTestDS As QCSummaryByTestSampleDS, _
                                                  ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim xmlName As String = String.Empty

                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim selectedTests As List(Of QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow)
                        selectedTests = (From a As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow In pQCSummaryByTestDS.QCSummaryByTestSampleTable _
                                        Where a.Selected = True Select a).ToList

                        For Each testSample As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow In selectedTests
                            Dim myCumulateDelegate As New CumulatedResultsDelegate
                            myGlobalDataTO = myCumulateDelegate.SaveCumulateResultNEW(dbConnection, testSample.QCTestSampleID, pQCControlLotID, pAnalyzerID, True)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'If the first Cumulated Serie was deleted, add the name of the XML File containing all its results
                            'to the list of XML Files to delete
                            If (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                If (DirectCast(myGlobalDataTO.SetDatos, String) <> String.Empty) Then
                                    If (xmlName <> String.Empty) Then xmlName &= ","
                                    xmlName &= DirectCast(myGlobalDataTO.SetDatos, String)
                                End If
                            End If
                        Next
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                    'Get names of all XML Files created in the TEMPORARY path
                    Dim myCreatedFiles As String() = IO.Directory.GetFiles(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\")

                    'Move all created XMLFiles from TEMP Dir to the final one
                    For Each myFileName As String In myCreatedFiles
                        IO.File.Move(myFileName, Application.StartupPath & GlobalBase.QCResultsFilesPath & myFileName.Substring(myFileName.LastIndexOf(CChar("\")) + 1))
                    Next

                    'Delete the TEMP Dir
                    IO.Directory.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\", True)

                    'If the first Cumulated Serie was deleted, delete also the XML File containing all its results
                    If (xmlName <> String.Empty) Then
                        Dim filesToDelete As String() = Split(xmlName, ",")
                        For Each xmlFile As String In filesToDelete
                            IO.File.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & xmlFile)
                        Next xmlFile
                    End If
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                    'It the TEMP Dir was created, them delete it
                    If (IO.Directory.Exists(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\")) Then
                        IO.Directory.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\", True)
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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.CumulateControlResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete a group of QC Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/06/2011
        ''' Modified by: SA 06/07/2011 - Informed parameter RunNumber when calling the function that delete the Alarms
        '''              SA 04/06/2012 - Informed parameter AnalyzerID when calling the function that delete the Alarms
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Before deleting the Results remove the Alarms
                        Dim myResultsAlarmDS As New ResultAlarmsDS
                        Dim myResultsAlarmDelegate As New QCResultAlarmsDelegate

                        For Each qcResultRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults.Rows
                            myGlobalDataTO = myResultsAlarmDelegate.DeleteNEW(dbConnection, qcResultRow.QCTestSampleID, qcResultRow.QCControlLotID, _
                                                                              qcResultRow.AnalyzerID, qcResultRow.RunsGroupNumber, qcResultRow.RunNumber)
                            If (myGlobalDataTO.HasError) Then Exit For
                        Next

                        If (Not myGlobalDataTO.HasError) Then
                            Dim myQCResultsDAO As New tqcResultsDAO
                            myGlobalDataTO = myQCResultsDAO.DeleteNEW(dbConnection, pQCResultsDS)
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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/QCControlLotID/AnalyzerID, delete the QC Results included in the RunsGroup for 
        ''' the informed Cumulated Serie
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCumResultsNum">Number of the Cumulated Serie to be deleted for the QCTestSampleID and QCControlLotID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/06/2011 
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function DeleteByCumResultsNumNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                 ByVal pAnalyzerID As String, ByVal pCumResultsNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.DeleteByCumResultsNumNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pCumResultsNum)

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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.DeleteByCumResultsNum", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/QCControlLotID/AnalyzerID, delete all the QC Results included in the informed RunsGroup,
        ''' including the Results Alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNum">Number of the Runs Group in which the QC Results to delete are included</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 27/06/2011 
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function DeleteByRunsGroupNumNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                ByVal pAnalyzerID As String, ByVal pRunsGroupNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete the Results Alarms
                        Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate
                        myGlobalDataTO = myQCResultAlarmsDelegate.DeleteNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pRunsGroupNum)

                        If (Not myGlobalDataTO.HasError) Then
                            'Delete the Results
                            Dim myQCResultsDAO As New tqcResultsDAO
                            myGlobalDataTO = myQCResultsDAO.DeleteByRunsGroupNumNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pRunsGroupNum)

                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.DeleteByRunsGroupNum", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all QC Results marked as included in calculation of statistical values for the informed 
        ''' QCTestSampleID/QCControlLotID/AnalyzerID and optionally, Runs Group Number
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Optional parameter. Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Optional parameter. Number of the open Runs Group for the specified
        '''                                Test/SampleType and Control/Lot</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 21/12/2011 - Parameter for the RunsGroupNumber changed to optional
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID
        ''' </remarks>
        Public Function DeleteStatisticResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                  ByVal pQCControlLotID As Integer, Optional ByVal pAnalyzerID As String = "", _
                                                  Optional ByVal pRunsGroupNumber As Integer = 0) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete all Alarms for the group of Results that will be deleted
                        Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate
                        myGlobalDataTO = myQCResultAlarmsDelegate.DeleteNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pRunsGroupNumber)

                        If (Not myGlobalDataTO.HasError) Then
                            'Delete the group of Results
                            Dim myQCResultsDAO As New tqcResultsDAO
                            myGlobalDataTO = myQCResultsDAO.DeleteStatisticResultsNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)
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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.DeleteStatisticResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For each Control/Lot having non cumulated QC Results with Alarms for the specified Test/SampleType, build for each Result 
        ''' a String list containing the description of all alarms (in the current application Language) divided by commas
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOpenQCResultsDS">Typed DataSet OpenQCResultsDS containing all Controls/Lots having non cumulated
        '''                                QC Results for the informed Test/SampleType</param>
        ''' <param name="pLanguageID">Code of the current Application Language</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultAlarms having for each QCTestSampleID/QCControlLotID and 
        '''          RunNumber the list of Alarm descriptions divided by commas</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SA 16/06/2011 - Pass the active RunsGroupNumber as parameter for the function used to get the Alarms
        '''              SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Private Function GetAllAlarmsDescriptionsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String, _
                                                     ByVal pOpenQCResultsDS As OpenQCResultsDS, ByVal pLanguageID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myQCResultAlarmsDS As New QCResultAlarms
                Dim myReceivedQCResultAlarmsDS As New QCResultAlarms
                Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate

                For Each openQCResultsROW As OpenQCResultsDS.tOpenResultsRow In pOpenQCResultsDS.tOpenResults.Rows
                    myGlobalDataTO = myQCResultAlarmsDelegate.GetAlarmsAndDescriptionsNEW(pDBConnection, pQCTestSampleID, openQCResultsROW.QCControlLotID, _
                                                                                          pAnalyzerID, openQCResultsROW.RunsGroupNumber, pLanguageID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myReceivedQCResultAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

                        For Each receivedRow As QCResultAlarms.tqcResultAlarmsRow In myReceivedQCResultAlarmsDS.tqcResultAlarms.Rows
                            myQCResultAlarmsDS.tqcResultAlarms.ImportRow(receivedRow)
                        Next
                    Else
                        'Error getting the multilanguage description of the Alarms...
                        Exit For
                    End If
                Next
                If (Not myGlobalDataTO.HasError) Then myGlobalDataTO.SetDatos = myQCResultAlarmsDS
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorMessage = ex.Message
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetAllAlarmsDescriptions", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Select all Tests/SampleTypes with not cumulated QC Results for the specified QCControlLotID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCSummaryByTestSampleDS with statistical QC data for all 
        '''          Tests/SampleTypes linked to the informed QCControlLotID/AnalyzerID that have non cumulated QC Results</returns>
        ''' <remarks>
        ''' Created by:  SA 09/06/2011
        ''' Modified by: SA 27/06/2011 - For the selected Control/Lot, if there are QC Results pending to review for a linked Test/SampleType,
        '''                              the results validation of all the non Cumulated QC Results is executed before verify if the icon of 
        '''                              Results with Alarms have to be shown
        '''              SA 05/05/2011 - Before process data of the returned Tests/SampleTypes to calculate values of Mean,SD, CV and Range, 
        '''                              group data of all rows returned for each Test/SampleType in an unique row
        '''              SA 01/12/2011 - Changed the function logic due to the change in the way the Statistical values are calculated  
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; get also icon for ISE Tests and inform fields TestTypeIcon and TestType  
        '''                              in the DS according value of TestType field     
        ''' </remarks>
        Public Function GetByQCControlLotIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer, _
                                               ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.ReadByQCControlLotIDNEW(dbConnection, pQCControlLotID, pAnalyzerID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myFinalOpenResultsDS As OpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

                            'Get Icons for Preloaded and User-defined Standard Tests, ISE Tests, and also the Icon for Alarm
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                            Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
                            Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")
                            Dim imageISETest As Byte() = preloadedDataConfig.GetIconImage("TISE_SYS")
                            Dim imageAlarms As Byte() = preloadedDataConfig.GetIconImage("STUS_WITHERRS")

                            Dim myQCSummaryDS As New QCSummaryByTestSampleDS
                            Dim myQCSummaryRow As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow

                            Dim myLastCumulateValueDS As New CumulatedResultsDS
                            Dim myLastCumulateValueDelegate As New LastCumulatedValuesDelegate

                            Dim numByStatus As Integer = 0
                            Dim myNonCumulatedResultsDS As OpenQCResultsDS

                            For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myFinalOpenResultsDS.tOpenResults.Rows
                                If (openQCResultRow.CalculationMode = "STATISTIC") Then
                                    'Calculate Mean, SD, CV and Ranges for the Control/Lot
                                    myGlobalDataTO = GetResultsByControlLotForStatisticsModeNEW(dbConnection, openQCResultRow.QCTestSampleID, pAnalyzerID, openQCResultRow.MinResultDateTime, _
                                                                                                openQCResultRow.MaxResultDateTime, pQCControlLotID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myNonCumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)
                                    Else
                                        'Error getting data for the Control/Lot and Test/SampleType
                                        Exit For
                                    End If
                                Else
                                    'Calculate Mean, SD, CV and Ranges for the Control/Lot
                                    myGlobalDataTO = GetResultsByControlLotForManualModeNEW(dbConnection, openQCResultRow.QCTestSampleID, pAnalyzerID, openQCResultRow.MinResultDateTime, _
                                                                                            openQCResultRow.MaxResultDateTime, pQCControlLotID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myNonCumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)
                                    Else
                                        'Error getting data for the Control/Lot and Test/SampleType
                                        Exit For
                                    End If
                                End If

                                If (myNonCumulatedResultsDS.tOpenResults.Count > 0) Then
                                    'Create the row for the Test/Sample Type in the DS to return
                                    myQCSummaryRow = myQCSummaryDS.QCSummaryByTestSampleTable.NewQCSummaryByTestSampleTableRow
                                    myQCSummaryRow.QCTestSampleID = openQCResultRow.QCTestSampleID
                                    myQCSummaryRow.RunsGroupNumber = openQCResultRow.RunsGroupNumber
                                    myQCSummaryRow.PreloadedTest = openQCResultRow.PreloadedTest

                                    myQCSummaryRow.Selected = True
                                    If (openQCResultRow.TestType = "STD") Then
                                        If (openQCResultRow.PreloadedTest) Then
                                            myQCSummaryRow.TestTypeIcon = imageTest
                                        Else
                                            myQCSummaryRow.TestTypeIcon = imageUserTest
                                        End If
                                    Else
                                        myQCSummaryRow.TestTypeIcon = imageISETest
                                    End If

                                    myQCSummaryRow.TestType = openQCResultRow.TestType
                                    myQCSummaryRow.TestName = openQCResultRow.TestName
                                    myQCSummaryRow.SampleType = openQCResultRow.SampleType
                                    myQCSummaryRow.MeasureUnit = openQCResultRow.MeasureUnit
                                    myQCSummaryRow.RejectionCriteria = openQCResultRow.RejectionCriteria
                                    myQCSummaryRow.DecimalsAllowed = openQCResultRow.DecimalsAllowed

                                    myQCSummaryRow.n = myNonCumulatedResultsDS.tOpenResults(0).n
                                    myQCSummaryRow.CalcMean = myNonCumulatedResultsDS.tOpenResults(0).CalcMean

                                    If (myNonCumulatedResultsDS.tOpenResults(0).n > 1) Then
                                        myQCSummaryRow.CalcSD = myNonCumulatedResultsDS.tOpenResults(0).CalcSD
                                        myQCSummaryRow.CalcCV = myNonCumulatedResultsDS.tOpenResults(0).CalcCV

                                        myQCSummaryRow.MinRange = myNonCumulatedResultsDS.tOpenResults(0).CalcMean - (openQCResultRow.RejectionCriteria * myNonCumulatedResultsDS.tOpenResults(0).CalcSD)
                                        If (myQCSummaryRow.MinRange < 0) Then myQCSummaryRow.MinRange = 0

                                        myQCSummaryRow.MaxRange = myNonCumulatedResultsDS.tOpenResults(0).CalcMean + (openQCResultRow.RejectionCriteria * myNonCumulatedResultsDS.tOpenResults(0).CalcSD)
                                    Else
                                        myQCSummaryRow.SetCalcSDNull()
                                        myQCSummaryRow.SetCalcCVNull()

                                        myQCSummaryRow.SetMinRangeNull()
                                        myQCSummaryRow.SetMaxRangeNull()
                                    End If

                                    'Validate if there are Cumulated values for the QCTestSampleID/QCControlLotID
                                    myGlobalDataTO = myLastCumulateValueDelegate.ReadNEW(dbConnection, openQCResultRow.QCTestSampleID, pQCControlLotID, pAnalyzerID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myLastCumulateValueDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                                        If (myLastCumulateValueDS.tqcCumulatedResults.Count > 0) Then
                                            'Set values of Mean and SD for the last cumulated serie
                                            myQCSummaryRow.CumulatedMean = myLastCumulateValueDS.tqcCumulatedResults(0).Mean
                                            myQCSummaryRow.CumulatedSD = myLastCumulateValueDS.tqcCumulatedResults(0).SD

                                            'If there are Cumulated values for the QCTestSampleID/QCControlLotID... 
                                            '** Calculate the Minimum allowed value according the RejectionCriteria currently defined - MIN RANGE
                                            myQCSummaryRow.CumulatedMinRange = myLastCumulateValueDS.tqcCumulatedResults(0).Mean - _
                                                                               (openQCResultRow.RejectionCriteria * myLastCumulateValueDS.tqcCumulatedResults(0).SD)
                                            If (myQCSummaryRow.CumulatedMinRange < 0) Then myQCSummaryRow.CumulatedMinRange = 0

                                            '** Calculate the Maximum allowed value according the RejectionCriteria currently defined - MAX RANGE
                                            myQCSummaryRow.CumulatedMaxRange = myLastCumulateValueDS.tqcCumulatedResults(0).Mean + _
                                                                               (openQCResultRow.RejectionCriteria * myLastCumulateValueDS.tqcCumulatedResults(0).SD)
                                        End If
                                    Else
                                        'Error getting the last cumulated values for the QCTestSampleID/QCControlLotID
                                        Exit For
                                    End If

                                    'Verify if the Test/SampleType has results pending of validation for the selected Control/Lot
                                    myGlobalDataTO = myQCResultsDAO.CountNotReviewedResultsNEW(dbConnection, pQCControlLotID, openQCResultRow.QCTestSampleID, _
                                                                                               pAnalyzerID, openQCResultRow.RunsGroupNumber)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        numByStatus = DirectCast(myGlobalDataTO.SetDatos, Int32)
                                        If (numByStatus > 0) Then
                                            'Validate the QC Results
                                            myGlobalDataTO = GetNonCumulateResultForAllControlLotsNEW(dbConnection, openQCResultRow.QCTestSampleID, pAnalyzerID, myNonCumulatedResultsDS, _
                                                                                                      openQCResultRow.MinResultDateTime, openQCResultRow.MaxResultDateTime)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    Else
                                        'Error verifying if there are QC Results pending to validate
                                        Exit For
                                    End If

                                    'Finally verify if there are alarms for the QCControlLotID/QCTestSampleID/RunsGroupNumber to show the Icon
                                    myGlobalDataTO = myQCResultsDAO.CountResultsWithAlarmsNEW(dbConnection, pQCControlLotID, openQCResultRow.QCTestSampleID, pAnalyzerID, _
                                                                                              openQCResultRow.RunsGroupNumber)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        numByStatus = DirectCast(myGlobalDataTO.SetDatos, Int32)

                                        myQCSummaryRow.SetAlarmsIconPathNull()
                                        If (numByStatus > 0) Then myQCSummaryRow.AlarmsIconPath = imageAlarms
                                    Else
                                        'Error verifying if there are QC Results with alarms
                                        Exit For
                                    End If

                                    myQCSummaryDS.QCSummaryByTestSampleTable.AddQCSummaryByTestSampleTableRow(myQCSummaryRow)
                                    myQCSummaryDS.AcceptChanges()
                                End If
                            Next

                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO.SetDatos = myQCSummaryDS
                                myGlobalDataTO.HasError = False
                            End If

                            imageTest = Nothing
                            imageUserTest = Nothing
                            imageAlarms = Nothing
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetByQCControlLotID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search in history table of QC Module all Controls/Lots having ClosedLot = FALSE and DeleteControl=FALSE and having for at least 
        ''' a Test/SampleType, enough QC Results pending to accumulate
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of all Controls/Lots in 
        '''          the history table in QC Module that have QC Results thar can be cumulated </returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetControlsToCumulateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetControlsToCumulateNEW(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetControlsToCumulate", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all not excluded QC Results for the specified Test/SampleType and Control/Lot and calculate all data needed to 
        ''' create a new cumulated serie
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultsCalculationDS with the needed data</returns>
        ''' <remarks>
        ''' Created by:  TR 19/05/2011
        ''' Modified by: SA 05/07/2011 - If more than one row was returned for the Runs Group, it means there are automatic and manual results, 
        '''                              and in this case, values of both rows have to be summarized
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID
        '''              SA 14/06/2012 - Changed implementation due to when AnalyzerID is not informed it is possible to get data to cumulate
        '''                              for more than one Analyzer (when cumulation is done from Programming screens)
        ''' </remarks>
        Public Function GetDataToCreateCumulateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                   ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetDataToCreateCumulateNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myQCResultCalculationDS As QCResultsCalculationDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsCalculationDS)

                            Dim myFinalQCResultCalculationDS As New QCResultsCalculationDS
                            If (myQCResultCalculationDS.tQCResultCalculation.Rows.Count > 0) Then
                                Dim lstQCResultCalculations As List(Of QCResultsCalculationDS.tQCResultCalculationRow)
                                Dim lstDiffAnalyzers As List(Of String) = (From a As QCResultsCalculationDS.tQCResultCalculationRow In myQCResultCalculationDS.tQCResultCalculation _
                                                                         Select a.AnalyzerID Distinct).ToList
                                For Each analyzerID As String In lstDiffAnalyzers
                                    'Get values for the Analyzer
                                    lstQCResultCalculations = (From b As QCResultsCalculationDS.tQCResultCalculationRow In myQCResultCalculationDS.tQCResultCalculation _
                                                              Where b.AnalyzerID = analyzerID _
                                                             Select b).ToList()

                                    If (lstQCResultCalculations.Count > 1) Then
                                        'If there are two rows it means there are automatic and manual results in the RunsGroup, values have to be summarized
                                        lstQCResultCalculations.First.N += lstQCResultCalculations.Last.N
                                        lstQCResultCalculations.First.SumXi += lstQCResultCalculations.Last.SumXi
                                        lstQCResultCalculations.First.SumXi2 += lstQCResultCalculations.Last.SumXi2
                                        lstQCResultCalculations.First.Sum2Xi += lstQCResultCalculations.Last.Sum2Xi

                                        'Summarize also the range of Dates of the Results
                                        If (lstQCResultCalculations.First.FirstRunDateTime < lstQCResultCalculations.First.FirstRunDateTime) Then
                                            lstQCResultCalculations.First.FirstRunDateTime = lstQCResultCalculations.Last.FirstRunDateTime
                                        End If
                                        If (lstQCResultCalculations.Last.LastRunDateTime > lstQCResultCalculations.First.LastRunDateTime) Then
                                            lstQCResultCalculations.First.LastRunDateTime = lstQCResultCalculations.Last.LastRunDateTime
                                        End If
                                    End If

                                    'Calculate Cumulated Mean
                                    lstQCResultCalculations.First.Mean = lstQCResultCalculations.First.SumXi / lstQCResultCalculations.First.N


                                    'Finally, copy the first row to the DS to return
                                    myFinalQCResultCalculationDS.tQCResultCalculation.ImportRow(lstQCResultCalculations.First)
                                Next

                                ''Move the first row to the DS to return
                                'myFinalQCResultCalculationDS.tQCResultCalculation.ImportRow(myQCResultCalculationDS.tQCResultCalculation.First)

                                'myFinalQCResultCalculationDS.tQCResultCalculation.First.BeginEdit()
                                'If (myQCResultCalculationDS.tQCResultCalculation.Rows.Count > 1) Then
                                '    'If there are two rows it means there are automatic and manual results in the RunsGroup, values have to be summarized
                                '    myFinalQCResultCalculationDS.tQCResultCalculation.First.N += myQCResultCalculationDS.tQCResultCalculation.Last.N
                                '    myFinalQCResultCalculationDS.tQCResultCalculation.First.SumXi += myQCResultCalculationDS.tQCResultCalculation.Last.SumXi
                                '    myFinalQCResultCalculationDS.tQCResultCalculation.First.SumXi2 += myQCResultCalculationDS.tQCResultCalculation.Last.SumXi2
                                '    myFinalQCResultCalculationDS.tQCResultCalculation.First.Sum2Xi += myQCResultCalculationDS.tQCResultCalculation.Last.Sum2Xi

                                '    'Summarize also the range of Dates of the Results
                                '    If (myQCResultCalculationDS.tQCResultCalculation.Last.FirstRunDateTime < myQCResultCalculationDS.tQCResultCalculation.First.FirstRunDateTime) Then
                                '        myFinalQCResultCalculationDS.tQCResultCalculation.First.FirstRunDateTime = myQCResultCalculationDS.tQCResultCalculation.Last.FirstRunDateTime
                                '    End If
                                '    If (myQCResultCalculationDS.tQCResultCalculation.Last.LastRunDateTime > myQCResultCalculationDS.tQCResultCalculation.First.LastRunDateTime) Then
                                '        myFinalQCResultCalculationDS.tQCResultCalculation.First.LastRunDateTime = myQCResultCalculationDS.tQCResultCalculation.Last.LastRunDateTime
                                '    End If
                                'End If

                                'myFinalQCResultCalculationDS.tQCResultCalculation.First.Mean = myFinalQCResultCalculationDS.tQCResultCalculation.First.SumXi / _
                                '                                                               myFinalQCResultCalculationDS.tQCResultCalculation.First.N
                                'myFinalQCResultCalculationDS.tQCResultCalculation.First.EndEdit()
                            End If

                            myGlobalDataTO.SetDatos = myFinalQCResultCalculationDS
                            myGlobalDataTO.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetDataToCreateCumulate", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the more recent date of a non cumulated QC Result for the specified QCTestSampleID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a datetime value with the maximum date</returns>
        ''' <remarks>
        ''' Created by:  SA 13/07/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetMaxResultDateTimeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetMaxResultDateTimeNEW(dbConnection, pQCTestSampleID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMaxResultDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Max RunNumber for the QCTestSampleID, QCControlID and RunsGroupNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNum">Number of the currently opened Runs Group in which the Results are included</param>
        ''' <returns>GlobalDataTO containing an integer value with the last created RunNumber for the
        '''          specified QCTestSampleID, QCControlID and RunsGroupNumber</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' Modified by: SA 15/06/2012 - Added parameter for the AnalyzerID
        ''' </remarks>
        Public Function GetMaxRunNumberNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                           ByVal pAnalyzerID As String, ByVal pRunsGroupNum As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetMaxRunNumberNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pRunsGroupNum)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMaxRunNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/AnalyzerID, get the maximum Run Number of all its non cumulated QC Results (between all its 
        ''' linked Control/Lots)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the TestType/TestID/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing an integer value with the maximum Run Number between all non cumulated QC Results for
        '''          the specified QCTestSampleID/AnalyzerID</returns>
        ''' <remarks>
        ''' Created by:  SA 17/01/2012
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetMaxRunNumberByTestSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                    ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetMaxRunNumberByTestSample(dbConnection, pQCTestSampleID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMaxRunNumberByTestSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the oldest date of a non cumulated QC Result for the specified QCTestSampleID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        ''' <remarks>
        ''' Created by:  TR 27/05/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetMinResultDateTimeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetMinResultDateTimeNEW(dbConnection, pQCTestSampleID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMinResultDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get and validate all non cumulated QC Results for the specified QCTestSampleID and each one of the QCControlLotID 
        ''' contained in the typed DataSet OpenQCResultsDS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <param name="pOpenQCResultsDS">Typed DataSet OpenQCResultsDS containing all Controls/Lots for which the informed Test/Sample
        '''                                have non cumulated QC Results in the specified range of dates </param>
        ''' <param name="pDateFrom">Date From to filter results</param>
        ''' <param name="pDateTo">Date To to filter results</param>
        ''' <returns>GlobalDataTO containing a typed DataSet</returns>
        ''' <remarks>
        ''' Created by:  TR 31/05/2011
        ''' Modified by: SA 06/06/2011 - Change the template: function has to be inside a DB Transaction. Added updation of fields 
        '''                              ValidationStatus. When ManualResultFlag is True the validation of Result inside Min/Max
        '''                              range was wrong.
        '''              TR 08/06/2011 - Validate the result value to load on VisibleResultValue depending if value of field ManualResultFlag is
        '''                              True (then VisibleResultValue = ManualResultValue) or False (then VisibleResultValue = ResultValue)
        '''              SA 06/07/2011 - Validation of result inside Range and calculation of Absolute and Relative Errors should be done only for not
        '''                              excluded Results
        '''              SA 17/10/2011 - If the DB Transaction was opened locally, execute the Rollback and close the opened DB Connection before 
        '''                              executing the two Exit Try included in the code   
        '''              SA 29/11/2011 - Calculate also the Relative Error expressed as %: (Absolute Error / Mean) * 100 
        '''              SA 25/01/2012 - When the result is out of range of valid values, set alarm QC_OUT_OF_RANGE instead of CONC_REMARK7.
        '''                              Apply Multirules also when there is only a Control/Lot with non cumulated QC Results
        '''              SA 30/05/2012 - Implementation changed due to changes in ApplyMultirules function (a new DS containing all QC Results for the
        '''                              TestType/TestID/SampleType in pairs according the RunNumber has to be built)
        '''              SA 04/06/2012 - Added parameter for AnalyzerID; informed this parameter when calling other functions related with QC Results
        '''                              and QC Result Alarms 
        ''' </remarks>
        Public Function GetNonCumulateResultForAllControlLotsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                                 ByVal pAnalyzerID As String, ByVal pOpenQCResultsDS As OpenQCResultsDS, ByVal pDateFrom As DateTime, _
                                                                 ByVal pDateTo As DateTime) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCControlIDList As String = ""
                        Dim myQCResultsDS As New QCResultsDS

                        Dim myResultsControl1DS As New QCResultsDS
                        Dim myResultsControl2DS As New QCResultsDS
                        Dim myQCResultAlarmsDS As New QCResultAlarms

                        Dim myQCResultsDAO As New tqcResultsDAO
                        Dim myQCResultsList As New List(Of QCResultsDS.tqcResultsRow)
                        Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate

                        'Build the list of QCControlLotIDs separated by commas
                        myGlobalDataTO = GetQCControlList(pOpenQCResultsDS)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myQCControlIDList = CStr(myGlobalDataTO.SetDatos)
                        Else
                            'If there are not Controls/Lots with results for the Test/SampleType, nothing to do...
                            If (pDBConnection Is Nothing) Then
                                DAOBase.RollbackTransaction(dbConnection)
                                If (Not dbConnection Is Nothing) Then dbConnection.Close()
                            End If
                            Exit Try
                        End If

                        'Search all non Cumulated Results the Test/SampleType has for the list of QCControlLotIDs in the specified range of dates
                        myGlobalDataTO = myQCResultsDAO.GetNonCumulateResultsNEW(dbConnection, pQCTestSampleID, myQCControlIDList, pAnalyzerID, pDateFrom, pDateTo)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)
                        Else
                            'If the Test/SampleType does not have non Cumulated Results for any of the Controls/Lots in the list, nothing to do...
                            If (pDBConnection Is Nothing) Then
                                DAOBase.RollbackTransaction(dbConnection)
                                If (Not dbConnection Is Nothing) Then dbConnection.Close()
                            End If
                            Exit Try
                        End If

                        Dim ctrlWestgard1 As Integer = 0
                        Dim ctrlWestgard2 As Integer = 0

                        'For each Control/Lot with non Cumulated Results for the Test/Sample Type
                        For Each openQCResultsROW As OpenQCResultsDS.tOpenResultsRow In pOpenQCResultsDS.tOpenResults.Rows
                            'Delete all Alarms saved for all the Results in the RunsGroup
                            myGlobalDataTO = myQCResultAlarmsDelegate.DeleteNEW(dbConnection, pQCTestSampleID, openQCResultsROW.QCControlLotID, _
                                                                                pAnalyzerID, openQCResultsROW.RunsGroupNumber)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Update ValidationStatus = OK for all Results included in the RunsGroup that are inside of the informed range of dates
                            myGlobalDataTO = myQCResultsDAO.UpdateValStatusByDateRangeNEW(dbConnection, openQCResultsROW.QCControlLotID, pQCTestSampleID, _
                                                                                          pAnalyzerID, openQCResultsROW.RunsGroupNumber, True, pDateFrom, pDateTo)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Update ValidationStatus = PENDING for all Results included in the RunsGroup that are outside of the informed range of dates
                            myGlobalDataTO = myQCResultsDAO.UpdateValStatusByDateRangeNEW(dbConnection, openQCResultsROW.QCControlLotID, pQCTestSampleID, _
                                                                                          pAnalyzerID, openQCResultsROW.RunsGroupNumber, False, pDateFrom, pDateTo)
                            If (myGlobalDataTO.HasError) Then Exit For

                            'Get from DataSet myQCResultsDS all Results corresponding to the QCControlLotID being processed
                            myQCResultsList = (From a As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                              Where a.QCTestSampleID = pQCTestSampleID _
                                            AndAlso a.QCControlLotID = openQCResultsROW.QCControlLotID _
                                             Select a).ToList()

                            If (myQCResultsList.Count > 0) Then
                                'Dim myRunsNumber As Integer = 1

                                For Each qcResultRow As QCResultsDS.tqcResultsRow In myQCResultsList
                                    'Calculate the RunNumber to shown in screen
                                    'qcResultRow.CalcRunNumber = myRunsNumber
                                    'myRunsNumber += 1

                                    'If there is an User Comment for the Result, or if it is a Result entered manually, 
                                    'get the path for the Icon of modified Results
                                    If (Not qcResultRow.IsResultCommentNull AndAlso Not qcResultRow.ResultComment = "") OrElse qcResultRow.ManualResultFlag Then
                                        qcResultRow.IconPath = My.Application.Info.DirectoryPath & GlobalBase.ImagesPath & GetIconName("CHG_PWD")
                                    End If

                                    If (qcResultRow.ManualResultFlag) Then
                                        'If manual result flag is checked, then the value shown is the manual result value
                                        qcResultRow.VisibleResultValue = qcResultRow.ManualResultValue
                                    Else
                                        qcResultRow.VisibleResultValue = qcResultRow.ResultValue
                                    End If

                                    'If the Result is not Excluded, verify if it is inside range and calculate ABS and REL Error
                                    If (Not qcResultRow.Excluded) Then
                                        'If ranges (Min/Max) are informed, validate if the Result is inside the range of allowed values
                                        If (Not openQCResultsROW.IsMinRangeNull AndAlso Not openQCResultsROW.IsMaxRangeNull) Then
                                            If (qcResultRow.VisibleResultValue < openQCResultsROW.MinRange OrElse _
                                                qcResultRow.VisibleResultValue > openQCResultsROW.MaxRange) Then
                                                'Insert Alarm QC_OUT_OF_RANGE in tqcResult Alarms for the result
                                                InsertNewQcResultAlarmNEW(myQCResultAlarmsDS, qcResultRow.QCControlLotID, qcResultRow.QCTestSampleID, _
                                                                          pAnalyzerID, qcResultRow.RunsGroupNumber, qcResultRow.RunNumber, GlobalEnumerates.CalculationRemarks.QC_OUT_OF_RANGE.ToString)
                                            End If
                                        End If

                                        'If Mean and SD have a value, then calculate Absolute and Relative Errors
                                        If (Not openQCResultsROW.IsMeanNull AndAlso Not openQCResultsROW.IsSDNull) Then
                                            'Calculate Absolute Error...
                                            qcResultRow.ABSError = qcResultRow.VisibleResultValue - openQCResultsROW.Mean

                                            'Calculate Relative Errors (SDI and %)
                                            qcResultRow.RELError = 0
                                            If (openQCResultsROW.SD > 0) Then qcResultRow.RELError = (qcResultRow.ABSError / openQCResultsROW.SD)

                                            qcResultRow.RELErrorPercent = 0
                                            If (openQCResultsROW.Mean > 0) Then qcResultRow.RELErrorPercent = (qcResultRow.ABSError / openQCResultsROW.Mean) * 100

                                            'If the Control/Lot has been included in the pair of Controls to apply Westgard Rules,
                                            'add the Result to the proper DS (depending if it is marked as first or second Control)
                                            If (openQCResultsROW.WestgardControlNum = 1) Then
                                                qcResultRow.ValidationStatus = "OK"
                                                ctrlWestgard1 = openQCResultsROW.QCControlLotID

                                            ElseIf (openQCResultsROW.WestgardControlNum = 2) Then
                                                qcResultRow.ValidationStatus = "OK"
                                                ctrlWestgard2 = openQCResultsROW.QCControlLotID
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        Next

                        'Calculate the Number of Serie to show in the screen of QC Results Review and load the DS needed for function ApplyWestgardRules
                        Dim myResultsForWestgardDS As New QCResultsForWestgardDS
                        Dim myResultsForWestgardRow As QCResultsForWestgardDS.tQCResultsForWestgardRow

                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myQCResultsDAO.GetRunNumberListByTestSample(dbConnection, pQCTestSampleID, pAnalyzerID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myRunNumbersListDS As QCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

                                Dim pairCreated As Boolean
                                Dim calcRunNumber As Integer = 1
                                Dim lstResults As New List(Of QCResultsDS.tqcResultsRow)

                                For Each row As QCResultsDS.tqcResultsRow In myRunNumbersListDS.tqcResults.Rows
                                    lstResults = (From b As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                                 Where b.QCTestSampleID = pQCTestSampleID _
                                               AndAlso b.RunNumber = row.RunNumber _
                                                Select b).ToList()

                                    pairCreated = False
                                    For Each resultRow As QCResultsDS.tqcResultsRow In lstResults
                                        'Set value of RunNumber to be shown in the screen of QCResulsReview
                                        resultRow.BeginEdit()
                                        resultRow.CalcRunNumber = calcRunNumber
                                        resultRow.EndEdit()

                                        If (resultRow.QCControlLotID = ctrlWestgard1 OrElse resultRow.QCControlLotID = ctrlWestgard2) Then
                                            If (Not pairCreated) Then
                                                myResultsForWestgardRow = myResultsForWestgardDS.tQCResultsForWestgard.NewtQCResultsForWestgardRow()
                                                myResultsForWestgardRow.RowIndex = calcRunNumber - 1
                                                myResultsForWestgardRow.QCTestSampleID = pQCTestSampleID
                                                myResultsForWestgardRow.RunNumber = row.RunNumber

                                                pairCreated = True
                                            End If

                                            If (resultRow.QCControlLotID = ctrlWestgard1) Then
                                                myResultsForWestgardRow.QCControlLotID1 = resultRow.QCControlLotID
                                                myResultsForWestgardRow.RunsGroupNumber1 = resultRow.RunsGroupNumber
                                                myResultsForWestgardRow.RelError1 = resultRow.RELError
                                                myResultsForWestgardRow.ValidationStatus1 = resultRow.ValidationStatus

                                            ElseIf (resultRow.QCControlLotID = ctrlWestgard2) Then
                                                myResultsForWestgardRow.QCControlLotID2 = resultRow.QCControlLotID
                                                myResultsForWestgardRow.RunsGroupNumber2 = resultRow.RunsGroupNumber
                                                myResultsForWestgardRow.RelError2 = resultRow.RELError
                                                myResultsForWestgardRow.ValidationStatus2 = resultRow.ValidationStatus
                                            End If
                                        End If
                                    Next resultRow

                                    If (pairCreated) Then
                                        myResultsForWestgardRow.PairOK = (Not myResultsForWestgardRow.IsQCControlLotID1Null AndAlso _
                                                                          Not myResultsForWestgardRow.IsQCControlLotID2Null)
                                        myResultsForWestgardDS.tQCResultsForWestgard.AddtQCResultsForWestgardRow(myResultsForWestgardRow)
                                    End If

                                    calcRunNumber += 1
                                Next row
                            End If
                        End If

                        'Apply selected Westgard Rules for the Test/SampleType 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = ApplyMultiRulesNEW(dbConnection, pQCTestSampleID, pAnalyzerID, myResultsForWestgardDS, myQCResultAlarmsDS)
                        End If

                        'Get the DS containing the list of alarms for the different Results. For each Alarm, insert it in tqcResultAlarm
                        If (Not myGlobalDataTO.HasError) Then
                            If (myQCResultAlarmsDS.tqcResultAlarms.Count > 0) Then
                                myGlobalDataTO = myQCResultAlarmsDelegate.CreateNEW(dbConnection, myQCResultAlarmsDS)

                                If (Not myGlobalDataTO.HasError) Then
                                    'Get description of all Alarms in the current Language
                                    Dim currentSession As New GlobalBase
                                    myGlobalDataTO = GetAllAlarmsDescriptionsNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pOpenQCResultsDS, _
                                                                                 currentSession.GetSessionInfo().ApplicationLanguage)

                                    If (Not myGlobalDataTO.HasError) Then
                                        myQCResultAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

                                        'For each returned record, search the result in the DS of Results and update the field AlarmsList
                                        Dim myQCResultList As New List(Of QCResultsDS.tqcResultsRow)
                                        For Each receivedAlarmRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsDS.tqcResultAlarms.Rows
                                            myQCResultList = (From a In myQCResultsDS.tqcResults _
                                                             Where a.QCTestSampleID = receivedAlarmRow.QCTestSampleID _
                                                               And a.QCControlLotID = receivedAlarmRow.QCControlLotID _
                                                               And a.RunNumber = receivedAlarmRow.RunNumber _
                                                            Select a).ToList()

                                            For Each resulToUpdate As QCResultsDS.tqcResultsRow In myQCResultList
                                                resulToUpdate.AlarmsList = receivedAlarmRow.AlarmDesc

                                                resulToUpdate.ValidationStatus = "ERROR"
                                                If (receivedAlarmRow.AlarmIDList.Trim = "WESTGARD_1-2s") Then
                                                    'If there is only an Alarm for the Result and it is the Multirule 1-2s, then ValidationStatus = WARNING 
                                                    resulToUpdate.ValidationStatus = "WARNING"
                                                End If

                                                'Update the ValidationStatus of the specific Result 
                                                myGlobalDataTO = myQCResultsDAO.UpdateValStatusByResultNEW(dbConnection, resulToUpdate.QCControlLotID, resulToUpdate.QCTestSampleID, _
                                                                                                           pAnalyzerID, resulToUpdate.RunsGroupNumber, resulToUpdate.ValidationStatus, _
                                                                                                           resulToUpdate.RunNumber)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            Next
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        Next
                                    End If
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Return the list of Results...
                            myGlobalDataTO.SetDatos = myQCResultsDS

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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetNonCumulateResultForAllControlLots", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified QCTestSampleID/QCControlLotID/AnalyzerID/RunsGroupNumber, get all QC Results to export to an
        ''' XML File (when a group of series are cumulated)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pGroupNumber">Identifier of the Group Number in QC Module</param> 
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS containing all QC Results to export to an
        '''          XML File (when a group of series are cumulated)</returns>
        ''' <remarks>
        ''' Created by:  SA 27/06/2011
        ''' Modified by: SA 28/06/2011 - Get also the Alarms for each QC Result
        '''              SA 06/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetQCResultsToExportNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                ByVal pAnalyzerID As String, ByVal pGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetQCResultsToExportNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pGroupNumber)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myQCResultsDS As QCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

                            If (myQCResultsDS.tqcResults.Count > 0) Then
                                'Get all Alarms linked to QC Results in the Runs Group
                                Dim myQCResultsAlarmsDelegate As New QCResultAlarmsDelegate
                                myGlobalDataTO = myQCResultsAlarmsDelegate.GetAlarmsNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pGroupNumber)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myQCResultAlarmsDS As QCResultAlarms = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

                                    Dim lstQCResult As List(Of QCResultsDS.tqcResultsRow)
                                    For Each resultAlarmRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsDS.tqcResultAlarms.Rows
                                        lstQCResult = (From a As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                                      Where a.QCTestSampleID = pQCTestSampleID _
                                                    AndAlso a.QCControlLotID = pQCControlLotID _
                                                    AndAlso a.RunsGroupNumber = pGroupNumber _
                                                    AndAlso a.RunNumber = resultAlarmRow.RunNumber _
                                                     Select a).ToList
                                        If (lstQCResult.Count = 1) Then
                                            lstQCResult.First.BeginEdit()
                                            lstQCResult.First.AlarmsList = resultAlarmRow.AlarmDesc
                                            lstQCResult.First.EndEdit()
                                        End If
                                    Next
                                    myQCResultsDS.AcceptChanges()
                                End If
                            End If

                            myGlobalDataTO.SetDatos = myQCResultsDS
                            myGlobalDataTO.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetQCResultsToExport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified Test/SampleType, get all non cumulated QC Results in the informed range of dates for 
        ''' each one of the linked Control/Lots. Used for Test/Sample Type with CalculationMode = MANUAL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <param name="pDateFrom">Date From to filter results</param>
        ''' <param name="pDateTo">Date To to filter results</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a Typed DataSet OpenQCResultsDS with the information for all Controls/Lots
        '''          linked to the Test/SampleType and having non cumulated QC Results in the specified range of dates</returns>
        ''' <remarks>
        ''' Created by:  TR 31/05/2011
        ''' Modified by: SA 09/06/2011 - Open a DB Connection instead of a DB Transaction
        '''              TR 17/06/2011 - Added searching of Mean and SD from the last cumulated values saved for the Test/SampleType and 
        '''                              each Control/Lot
        '''              SA 05/05/2011 - Before process data of the returned Controls to calculate values of Mean,SD, CV and Range, 
        '''                              group data of all rows returned for each Control in an unique row
        '''              SA 13/07/2011 - Removed searching of last cumulated values and getting the Range of valid values according them
        '''              SA 01/12/2011 - Changed function name from GetByQCTestSampleIDResultDateTime to GetResultsByControlLotForManualMode, 
        '''                              due to this function is used only when CalculationMode = MANUAL
        '''                              Added optional parameter to get data only for the specified Control/Lot
        '''              SA 02/12/2011 - Get Mean, SD and CV of returned QC Results
        '''              SA 19/01/2012 - After searching if Controls with not Excluded open QC Results, verify also if there are Controls with
        '''                              all their open QC Results marked as Excluded
        '''              SA 04/06/2012 - Added parameter for AnalyzerID  
        '''              JV 06/11/2013 - issue #1185 Math.Abs and avoid division by zero with CalcCV
        ''' </remarks>
        Public Function GetResultsByControlLotForManualModeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                               ByVal pAnalyzerID As String, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime, _
                                                               Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        Dim myOpenQCResultsDS As New OpenQCResultsDS

                        'Select All Controls With not excluded Open Results for the Test/Sample between dates
                        myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pDateFrom, pDateTo, False, pQCControlLotID, False)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

                            Dim myFinalOpenResultsDS As New OpenQCResultsDS
                            If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
                                'Move data to a different DS, grouping data of all rows returned for each Control in an unique row (a maximum of two rows can be returned
                                'for a Control: one with values for non manual Results and another with values for manual Results)
                                Dim myQCControlID As Integer = 0

                                For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
                                    If (openQCResultRow.QCControlLotID <> myQCControlID) Then
                                        myQCControlID = openQCResultRow.QCControlLotID
                                        myFinalOpenResultsDS.tOpenResults.ImportRow(openQCResultRow)
                                    Else
                                        'Add values to the previous ones if it is the same Control/Lot
                                        myFinalOpenResultsDS.tOpenResults.Last.BeginEdit()
                                        myFinalOpenResultsDS.tOpenResults.Last.n += openQCResultRow.n
                                        myFinalOpenResultsDS.tOpenResults.Last.Sumxi += openQCResultRow.Sumxi
                                        myFinalOpenResultsDS.tOpenResults.Last.Sumxi2 += openQCResultRow.Sumxi2
                                        myFinalOpenResultsDS.tOpenResults.Last.EndEdit()
                                    End If
                                Next

                                Dim numberOfControl As Integer = 1
                                For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myFinalOpenResultsDS.tOpenResults.Rows
                                    openQCResultRow.Selected = False

                                    'Get Mean and SD from Theorical Concentration Values (Min/Max); CV is set to NULL
                                    'Minimum and Maximum allowed values are defined for the theorical Min/Max Concentration values (MinRange/MaxRange)
                                    openQCResultRow.Mean = (openQCResultRow.MinRange + openQCResultRow.MaxRange) / 2
                                    openQCResultRow.SD = (openQCResultRow.MaxRange - openQCResultRow.MinRange) / (2 * openQCResultRow.RejectionCriteria)
                                    openQCResultRow.SetCVNull()

                                    'Get Mean, SD and CV of all returned results 
                                    openQCResultRow.CalcMean = openQCResultRow.Sumxi / openQCResultRow.n
                                    If (openQCResultRow.n > 1) Then
                                        openQCResultRow.CalcSD = Math.Sqrt(Math.Abs(((openQCResultRow.n * openQCResultRow.Sumxi2) - (openQCResultRow.Sumxi * openQCResultRow.Sumxi)) / _
                                                                           (openQCResultRow.n * (openQCResultRow.n - 1))))
                                        If openQCResultRow.CalcMean <> 0 Then
                                            openQCResultRow.CalcCV = (openQCResultRow.CalcSD / openQCResultRow.CalcMean) * 100
                                        Else
                                            openQCResultRow.CalcCV = 0
                                        End If
                                    Else
                                        'Values cannot be calculated
                                        openQCResultRow.SetCalcSDNull()
                                        openQCResultRow.SetCalcCVNull()
                                    End If
                                Next
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Special Case: search if there are Controls with open QC Results in the interval of dates but all of them are excluded
                                myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pDateFrom, pDateTo, False, pQCControlLotID, True)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

                                    If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
                                        'Move data to a different DS, ignoring the second row of a duplicated Control (a maximum of two rows can be returned
                                        'for a Control: one with values for non manual Results and another with values for manual Results). If the Control have
                                        'been already included in the DS to return, it is also ignored
                                        Dim myQCControlID As Integer = 0

                                        For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
                                            If (openQCResultRow.QCControlLotID <> myQCControlID) Then
                                                'The Control is added to the final DS only if it was not already included in it
                                                myQCControlID = openQCResultRow.QCControlLotID

                                                If (myFinalOpenResultsDS.tOpenResults.Where(Function(a) a.QCControlLotID = myQCControlID).Count() = 0) Then
                                                    'Get Mean and SD from Theorical Concentration Values (Min/Max); CV is set to NULL
                                                    'Minimum and Maximum allowed values are defined for the theorical Min/Max Concentration values (MinRange/MaxRange)
                                                    openQCResultRow.Mean = (openQCResultRow.MinRange + openQCResultRow.MaxRange) / 2
                                                    openQCResultRow.SD = (openQCResultRow.MaxRange - openQCResultRow.MinRange) / (2 * openQCResultRow.RejectionCriteria)
                                                    openQCResultRow.SetCVNull()

                                                    'Set n=0 and also set to NULL fields Mean, SD and CV of all returned results 
                                                    openQCResultRow.n = 0
                                                    openQCResultRow.SetCalcMeanNull()
                                                    openQCResultRow.SetCalcSDNull()
                                                    openQCResultRow.SetCalcCVNull()

                                                    'Finally, move the row to the final DS
                                                    myFinalOpenResultsDS.tOpenResults.ImportRow(openQCResultRow)
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            End If

                            'Return the DS with all the information
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO.SetDatos = myFinalOpenResultsDS
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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetResultsByControlLotForManualMode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the specified Test/SampleType, get all non cumulated QC Results for each one of the linked Control/Lots (or for 
        ''' the specified one, when the optional parameter for the Control/Lot is informed) and calculate:
        '''   ** Statistical values of Mean, SD, CV and Range using the first n (pMinNumSeries) results, without filtering them 
        '''      by the informed range of dates
        '''   ** Mean, SD and CV of the results included in the informed range of dates and not included in the statistics ones
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pDateFrom">Date From to filter results</param>
        ''' <param name="pDateTo">Date To to filter results</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a Typed DataSet OpenQCResultsDS with the information for all Controls/Lots
        '''          linked to the Test/SampleType and having non cumulated QC Results</returns>
        ''' <remarks>
        ''' Created by:  SA 30/11/2011
        ''' Modified by: SA 14/12/2011 - Open DBTransaction instead of DBConnection due to values in table of Last Statistic Values
        '''                              are updated when they can be calculated
        '''              SA 04/06/2012 - Added parameter for AnalyzerID 
        '''              JV 06/11/2013 - issue #1185 Math.Abs and avoid division by zero with CalcCV
        ''' </remarks>
        Public Function GetResultsByControlLotForStatisticsModeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                                   ByVal pAnalyzerID As String, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime, _
                                                                   Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        Dim myFinalOpenResultsDS As New OpenQCResultsDS

                        'Get values needed for calculation of Statistic
                        myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotForStatisticsNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pQCControlLotID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myStatisticsResultsDS As QCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

                            Dim i As Integer = 0
                            Dim mySumXi As Double
                            Dim mySumXi2 As Double

                            Dim myQCControlLotID As Integer = 0
                            Dim myControlRow As OpenQCResultsDS.tOpenResultsRow

                            For Each qcControlResult As QCResultsDS.tqcResultsRow In myStatisticsResultsDS.tqcResults.Rows
                                If (qcControlResult.QCControlLotID <> myQCControlLotID) Then
                                    'Initialization of all variables used to count records and sumarize values
                                    i = 0
                                    mySumXi = 0
                                    mySumXi2 = 0

                                    'A new linked Control/Lot will be processed
                                    myQCControlLotID = qcControlResult.QCControlLotID

                                    'Add the row for the Control/Lot in myFinalOpenResultsDS
                                    myControlRow = myFinalOpenResultsDS.tOpenResults.NewtOpenResultsRow

                                    myControlRow.Selected = False
                                    myControlRow.WestgardControlNum = qcControlResult.WestgardControlNum
                                    myControlRow.QCControlLotID = qcControlResult.QCControlLotID
                                    myControlRow.ControlName = qcControlResult.ControlName
                                    myControlRow.LotNumber = qcControlResult.LotNumber
                                    myControlRow.ControlNameLotNum = qcControlResult.ControlNameLotNum
                                    myControlRow.RunsGroupNumber = qcControlResult.RunsGroupNumber
                                    myControlRow.MeasureUnit = qcControlResult.MeasureUnit
                                    myControlRow.SetMeanNull()
                                    myControlRow.SetSDNull()
                                    myControlRow.SetCVNull()
                                    myControlRow.SetMinRangeNull()
                                    myControlRow.SetMaxRangeNull()
                                    myControlRow.n = 0
                                    myControlRow.Sumxi = 0
                                    myControlRow.Sumxi2 = 0
                                    myControlRow.SetCalcMeanNull()
                                    myControlRow.SetCalcSDNull()
                                    myControlRow.SetCalcCVNull()

                                    myFinalOpenResultsDS.tOpenResults.AddtOpenResultsRow(myControlRow)
                                End If

                                i += 1
                                mySumXi += qcControlResult.ResultValue
                                mySumXi2 += (qcControlResult.ResultValue * qcControlResult.ResultValue)

                                myControlRow.BeginEdit()
                                If (i = qcControlResult.NumberOfSeries) Then
                                    'Calculate the statistical values of Mean, SD, CV and Ranges
                                    myControlRow.Mean = mySumXi / i
                                    myControlRow.SD = Math.Sqrt(Math.Abs(((i * mySumXi2) - (mySumXi * mySumXi)) / (i * (i - 1))))
                                    myControlRow.CV = CDbl(IIf(myControlRow.Mean <> 0, (myControlRow.SD / myControlRow.Mean) * 100, 0))

                                    myControlRow.MinRange = myControlRow.Mean - (qcControlResult.RejectionCriteria * myControlRow.SD)
                                    If (myControlRow.MinRange < 0) Then myControlRow.MinRange = 0

                                    myControlRow.MaxRange = myControlRow.Mean + (qcControlResult.RejectionCriteria * myControlRow.SD)
                                    If (myControlRow.MaxRange < 0) Then myControlRow.MaxRange = 0
                                End If
                                myControlRow.EndEdit()
                            Next

                            Dim myOpenQCResultsDS As New OpenQCResultsDS
                            For Each qcControlResult As OpenQCResultsDS.tOpenResultsRow In myFinalOpenResultsDS.tOpenResults.Rows
                                'Select all Open Results for the Test/SampleType and Control/Lot between dates
                                myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pDateFrom, pDateTo, True, _
                                                                                          qcControlResult.QCControlLotID, False)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

                                    If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
                                        qcControlResult.BeginEdit()
                                        For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
                                            qcControlResult.n += openQCResultRow.n
                                            qcControlResult.Sumxi += openQCResultRow.Sumxi
                                            qcControlResult.Sumxi2 += openQCResultRow.Sumxi2
                                        Next

                                        qcControlResult.CalcMean = qcControlResult.Sumxi / qcControlResult.n
                                        If (qcControlResult.n > 1) Then
                                            qcControlResult.CalcSD = Math.Sqrt(Math.Abs(((qcControlResult.n * qcControlResult.Sumxi2) - (qcControlResult.Sumxi * qcControlResult.Sumxi)) / _
                                                                               (qcControlResult.n * (qcControlResult.n - 1))))
                                            If qcControlResult.CalcMean <> 0 Then
                                                qcControlResult.CalcCV = (qcControlResult.CalcSD / qcControlResult.CalcMean) * 100
                                            Else
                                                qcControlResult.CalcCV = 0
                                            End If
                                        End If
                                        qcControlResult.EndEdit()
                                    End If
                                Else
                                    'Error getting the not excluded and not included in Mean open QC Results 
                                    Exit For
                                End If
                            Next

                            If (Not myGlobalDataTO.HasError) Then
                                'Special Case: search if there are Controls with open QC Results in the interval of dates but all of them are excluded
                                myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pDateFrom, pDateTo, True, -1, True)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

                                    If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
                                        'Move data to a different DS, ignoring the second row of a duplicated Control (a maximum of two rows can be returned
                                        'for a Control: one with values for non manual Results and another with values for manual Results). If the Control have
                                        'been already included in the DS to return, it is also ignored
                                        Dim myQCControlID As Integer = 0

                                        For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
                                            If (openQCResultRow.QCControlLotID <> myQCControlID) Then
                                                'The Control is added to the final DS only if it was not already included in it
                                                myQCControlID = openQCResultRow.QCControlLotID

                                                If (myFinalOpenResultsDS.tOpenResults.Where(Function(a) a.QCControlLotID = myQCControlID).Count() = 0) Then
                                                    'Set to NULL fields MinRange, MaxRange, Mean, SD, and CV
                                                    openQCResultRow.SetMinRangeNull()
                                                    openQCResultRow.SetMaxRangeNull()
                                                    openQCResultRow.SetMeanNull()
                                                    openQCResultRow.SetSDNull()
                                                    openQCResultRow.SetCVNull()

                                                    'Set n=0 and also set to NULL fields Mean, SD and CV of all returned results 
                                                    openQCResultRow.n = 0
                                                    openQCResultRow.SetCalcMeanNull()
                                                    openQCResultRow.SetCalcSDNull()
                                                    openQCResultRow.SetCalcCVNull()

                                                    'Finally, move the row to the final DS
                                                    myFinalOpenResultsDS.tOpenResults.ImportRow(openQCResultRow)
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If

                        'Return the DS with all the information
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO.SetDatos = myFinalOpenResultsDS
                            myGlobalDataTO.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetResultsByControlLotForStatisticsMode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        ''' informed Test/Sample Type, Control/Lot and Runs Group Number
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pNewRunsGroupNumber">Number of the open Runs Group  for the Test/SampleType and Control/Lot to which 
        '''                                   the QC Results have to be assigned</param>
        ''' <returns>GlobalDataTO containing success / error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID 
        ''' </remarks>
        Public Function MoveStatisticResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                ByVal pAnalyzerID As String, ByVal pNewRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete all Alarms for the group of Results that will be moved to a new Runs Group
                        Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate
                        myGlobalDataTO = myQCResultAlarmsDelegate.DeleteNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pNewRunsGroupNumber - 1)

                        If (Not myGlobalDataTO.HasError) Then
                            'Move the Result to the new group
                            Dim myQCResultsDAO As New tqcResultsDAO
                            myGlobalDataTO = myQCResultsDAO.MoveStatisticResultsNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pNewRunsGroupNumber)
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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.MoveStatisticResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there are QC Results pending to cumulate for the specified Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pTestType">Test Type Code. Optional parameter</param>
        ''' <param name="pTestID">Test Identifier. Optional parameter</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Tests/SampleTypes
        '''          for which the informed Control has QC Results pending to accumulate</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2011
        ''' Modified by: SA 24/05/2011 - Added optional parameters to filter results also by TestID and SampleType
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID; added optional parameter for TestType
        ''' </remarks>
        Public Function SearchPendingResultsByControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                                         Optional ByVal pTestType As String = "", Optional ByVal pTestID As Integer = 0, _
                                                         Optional ByVal pSampleType As String = "", Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.SearchPendingResultsByControlNEW(dbConnection, pControlID, pTestType, pTestID, pSampleType, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.SearchPendingResultsByControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there are QC Results pending to cumulate for the specified TestID and Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Controls/Lots
        '''          for which the informed Test/SampleType has QC Results pending to accumulate</returns>
        ''' <remarks>
        ''' Created by:  TR 24/05/2011
        ''' Modified by: SA 04/06/2012 - Added optional parameter for AnalyzerID; added parameter for TestType
        ''' </remarks>
        Public Function SearchPendingResultsByTestIDSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, _
                                                                  ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                                                  Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.SearchPendingResultsByTestIDSampleTypeNEW(dbConnection, pTestType, pTestID, pSampleType, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.SearchPendingResultsByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all open QC Results for all Control/Lots linked to Test/SampleTypes with Calculation Mode defined as STATISTICS
        ''' (the first NumberOfSeries results will be used to calculate the statistic values)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed Dataset QCResultsDS with the list of all QC Results that will
        '''          be used to calculate statistics for all Control/Lots linked to Test/SampleTypes with 
        '''          CalculationMode = STATISTIC</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function SetResultsForStatisticsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                   Optional ByVal pQCTestSampleID As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetResultsForStatisticsNEW(dbConnection, pAnalyzerID, pQCTestSampleID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myQCResultsDS As QCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

                            'Get the list of different QCTestSampleIDs
                            Dim lstTestSamplesList As List(Of Integer) = (From a As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                                                         Select a.QCTestSampleID Distinct).ToList()

                            Dim i As Integer
                            Dim numOfSeries As Integer
                            Dim lstCtrlLotList As List(Of Integer)
                            Dim lstQCResultsList As List(Of QCResultsDS.tqcResultsRow)

                            Dim myQCResultsToUpdateDS As New QCResultsDS

                            For Each qcTestSampleID As Integer In lstTestSamplesList
                                'Set IncludedInMean = FALSE for all open QC Results for the Test/SampleType
                                myGlobalDataTO = myQCResultsDAO.UnmarkStatisticResultsNEW(dbConnection, qcTestSampleID, pAnalyzerID)

                                'Get the Number of Series defined for the Test/SampleType
                                numOfSeries = (From b As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                              Where b.QCTestSampleID = qcTestSampleID _
                                             Select b.NumberOfSeries).First()

                                'Get the list of different Control/Lots linked to the Test/SampleType
                                lstCtrlLotList = (From c As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                                 Where c.QCTestSampleID = qcTestSampleID _
                                                Select c.QCControlLotID Distinct).ToList()

                                For Each qcControlLotID As Integer In lstCtrlLotList
                                    'Get the list of QC Results for the Test/SampleType and Control/Lot
                                    lstQCResultsList = (From d As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
                                                       Where d.QCTestSampleID = qcTestSampleID _
                                                     AndAlso d.QCControlLotID = qcControlLotID _
                                                      Select d Order By d.ResultDateTime).ToList()

                                    i = 0
                                    For Each qcResult As QCResultsDS.tqcResultsRow In lstQCResultsList
                                        If (i < numOfSeries) Then
                                            myQCResultsToUpdateDS.tqcResults.ImportRow(qcResult)
                                            i += 1
                                        Else
                                            Exit For
                                        End If
                                    Next
                                Next
                            Next
                            lstTestSamplesList = Nothing
                            lstCtrlLotList = Nothing
                            lstQCResultsList = Nothing

                            'Set IncludedInMean = TRUE for the first numOfSeries Results for each Test/SampleType and Control/Lot
                            myGlobalDataTO = myQCResultsDAO.MarkStatisticResultsNEW(dbConnection, myQCResultsToUpdateDS)
                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.SetResultsForStatistics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        ''' informed Test/Sample Type and optionally, the Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        ''' <returns>GlobalDataTO containing the total number of QC Results marked as included in Mean
        '''          for the specified Test/SampleType and optionally, the Control/Lot</returns>
        ''' <remarks>
        ''' Created by:  SA 15/12/2011
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function UnmarkStatisticResultsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String, _
                                                  Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.UnmarkStatisticResultsNEW(dbConnection, pQCTestSampleID, pAnalyzerID, pQCControlLotID)

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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.UnmarkStatisticResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update values used for QC Calculation Criteria in history tables of QC Module (tqcHistoryTestSamples,  
        ''' tqcHistoryTestSamplesRules and tqcHistoryTestControlLots) and also in tables tparTestSamples and tparTestSamplesRules
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <param name="pCalculationMode">Calculation Mode: MANUAL/STATISTICS</param>
        ''' <param name="pNumberOfSeries">Minimum number or series required to calculate statistics</param>
        ''' <param name="pRejectionNumeric">Rejection Criteria</param>
        ''' <param name="pQCControlLotIDForWESG1">Identifier (in QC Module) of the first Control to apply Multirules</param>
        ''' <param name="pQCControlLotIDForWESG2">Identifier (in QC Module) of the second Control to apply Multirules</param>
        ''' <param name="pTestSampleMultirulesDS">Typed DataSet TestSamplesMultirulesDS containing the list of available Multirules 
        '''                                       and indicating for each one if it has been selected to validate the results for 
        '''                                       the Test/SampleType</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS containing all data updated</returns>
        ''' <remarks>
        ''' Created by:  TR 30/05/2011
        ''' Modified by: SA 16/12/2011 - Set value of IncludedInMean flag for all open Results depending on the CalculationMode
        '''              SA 03/01/2012 - Update field RejectionCriteria only when it is informed. Return a typed DataSet HistoryTestSamplesDS with all
        '''                              data of the informed QCTestSampleID (needed when the function is called from function SaveLastCumulatedAsTarget
        '''                              in HistoryTestControlLotsDelegate)  
        '''              SA 25/01/2012 - When only one Control/Lot has been selected for Multirules application, update WestgardControlNum=1 for it
        '''              SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function UpdateChangedValuesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String, _
                                               ByVal pCalculationMode As String, ByVal pNumberOfSeries As Integer, ByVal pRejectionNumeric As Single, _
                                               ByVal pQCControlLotIDForWESG1 As String, ByVal pQCControlLotIDForWESG2 As String, _
                                               ByVal pTestSampleMultirulesDS As TestSamplesMultirulesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestID As Integer = 0
                        Dim myTestType As String = String.Empty
                        Dim mySampleType As String = String.Empty
                        Dim myQCHistoryTestSampleDS As New HistoryTestSamplesDS

                        '1.1- Update values on table tqcHistoryTestSamples 
                        '     ** Get the current values for the informed QCTestSampleID 
                        Dim myQCHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
                        myGlobalDataTO = myQCHistoryTestSamplesDelegate.Read(dbConnection, pQCTestSampleID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myQCHistoryTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)

                            If (myQCHistoryTestSampleDS.tqcHistoryTestSamples.Count > 0) Then
                                'Get TestType, TestID and SampleType 
                                myTestType = myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).TestType
                                myTestID = myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).TestID
                                mySampleType = myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).SampleType

                                'Update DS with the new values 
                                myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).CalculationMode = pCalculationMode
                                myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).NumberOfSeries = pNumberOfSeries
                                If (pRejectionNumeric > 0) Then myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).RejectionCriteria = pRejectionNumeric
                                myQCHistoryTestSampleDS.AcceptChanges()

                                'Update on tqcHistoryTestSamples
                                myGlobalDataTO = myQCHistoryTestSamplesDelegate.UpdateByQCTestIdAndSampleTypeNEW(dbConnection, myQCHistoryTestSampleDS)
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            If (myTestType = "STD") Then
                                '1.2A Update values on table tparTestSamples 
                                '     ** Get the current values for the TestID and SampleType identified by the informed QCTestSampleID in QC Module
                                Dim myTestSampleDelegate As New TestSamplesDelegate
                                myGlobalDataTO = myTestSampleDelegate.GetDefinition(dbConnection, myTestID, mySampleType)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myTestSampleDS As TestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                                    If (myTestSampleDS.tparTestSamples.Count > 0) Then
                                        'Update DS with the new values
                                        myTestSampleDS.tparTestSamples(0).CalculationMode = pCalculationMode
                                        myTestSampleDS.tparTestSamples(0).NumberOfSeries = pNumberOfSeries
                                        If (pRejectionNumeric > 0) Then myTestSampleDS.tparTestSamples(0).RejectionCriteria = pRejectionNumeric
                                        myTestSampleDS.AcceptChanges()

                                        'Update on tparTestSamples
                                        myGlobalDataTO = myTestSampleDelegate.Update(dbConnection, myTestSampleDS)
                                    End If
                                End If

                            ElseIf (myTestType = "ISE") Then
                                '1.2B Update values on table tparISETestSamples 
                                '     ** Get the current values for the TestID and SampleType identified by the informed QCTestSampleID in QC Module
                                Dim myISETestSampleDelegate As New ISETestSamplesDelegate
                                myGlobalDataTO = myISETestSampleDelegate.GetDefinitionForQCModule(dbConnection, myTestID, mySampleType)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myISETestSamplesDS As HistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                                    If (myISETestSamplesDS.tqcHistoryTestSamples.Count > 0) Then
                                        'Update DS with the new values
                                        myISETestSamplesDS.tqcHistoryTestSamples(0).CalculationMode = pCalculationMode
                                        myISETestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries = pNumberOfSeries
                                        If (pRejectionNumeric > 0) Then myISETestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria = pRejectionNumeric
                                        myISETestSamplesDS.AcceptChanges()

                                        'Update on tparTestSamples
                                        myGlobalDataTO = myISETestSampleDelegate.UpdateQCValues(dbConnection, myISETestSamplesDS)
                                    End If
                                End If
                            End If
                        End If

                        '1.3 If a least a Control has been selected for multirules application...
                        If (pQCControlLotIDForWESG1 <> "") Then
                            '** Update field WestgardControlNum for each QCControlLotID in table tqcHistoryTestControlLots
                            '   Values: 1 for pQCControlLotIDForWESG1 / 
                            '           2 for pQCControlLotIDForWESG2 (when informed) / 
                            '           0 for any other QCControlLotID linked to the QCTestSampleID
                            If (Not myGlobalDataTO.HasError) Then
                                Dim myHistoryTestControlsLotsDelegate As New HistoryTestControlLotsDelegate
                                myGlobalDataTO = myHistoryTestControlsLotsDelegate.UpdateWestgardControlNum(dbConnection, pQCTestSampleID, pQCControlLotIDForWESG1, _
                                                                                                            pQCControlLotIDForWESG2)
                            End If

                            '*** Update Multirules to apply for the TestType/TestID/SampleType
                            If (Not myGlobalDataTO.HasError) Then
                                'Update in table tparTestSamplesMultirules 
                                Dim myTestSampleMultiDelegate As New TestSamplesMultirulesDelegate
                                myGlobalDataTO = myTestSampleMultiDelegate.DeleMultiRulesByTestIDNEW(dbConnection, myTestType, myTestID, mySampleType)

                                If (Not myGlobalDataTO.HasError) Then
                                    myGlobalDataTO = myTestSampleMultiDelegate.AddMultiRulesNEW(dbConnection, pTestSampleMultirulesDS)
                                End If

                                'Update in table tqcHistoryTestSamplesMultirules 
                                If (Not myGlobalDataTO.HasError) Then
                                    Dim myHistoryTestSampleRulesDelegate As New HistoryTestSamplesRulesDelegate
                                    myGlobalDataTO = myHistoryTestSampleRulesDelegate.Delete(dbConnection, pQCTestSampleID)

                                    If (Not myGlobalDataTO.HasError) Then
                                        myGlobalDataTO = myHistoryTestSampleRulesDelegate.InsertFromTestSampleMultiRulesNEW(dbConnection, myTestType, myTestID, mySampleType, pQCTestSampleID)
                                    End If
                                End If
                            End If
                        End If

                        '1.4 Set value of IncludedInMean flag for all open Results depending on the CalculationMode
                        If (Not myGlobalDataTO.HasError) Then
                            If (pCalculationMode = "MANUAL") Then
                                Dim mytqcResultsDAO As New tqcResultsDAO
                                myGlobalDataTO = mytqcResultsDAO.UnmarkStatisticResultsNEW(dbConnection, pQCTestSampleID, pAnalyzerID)
                            Else
                                myGlobalDataTO = SetResultsForStatisticsNEW(dbConnection, pAnalyzerID, pQCTestSampleID)
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = myQCHistoryTestSampleDS
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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.UpdateChangedValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update fields ManualResultValue, ResultComment and/or Excluded flag for an specific QC Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing data of the QC Result to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 08/06/2011
        ''' </remarks>
        Public Function UpdateManualResultNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.UpdateManualResultNEW(dbConnection, pQCResultsDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.UpdateManualResult", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Apply the Multirules selected for the specified QCTestSampleID. 
        ''' In general, selected Multirules are applied for a pair of Controls, first Across Controls and if the Alarm is not raised, 
        ''' for each individual Control, but each Multirule has its particular characteristics:
        '''    1-2s: Warning Rule. Verify if the Result for each Control is out of +/- 2SD.
        '''          It is applied Within Control and Within Run.
        '''    1-3s: Verify if the Result for each Control is out of +/- 3SD. 
        '''          It is applied Within Control and Within Run.
        '''    2-2s: When applied Across Controls and Within Run, verify if the Results for both Controls are out of +/- 2SD in the same direction
        '''          When applied Within Control and Across Runs, verify if the last two Runs of each Control are out of +/- 2SD in the same direction
        '''    R-4s: When applied Across Controls and Within Run, verify if the last result for one Control is upper the Mean and the last result for 
        '''          the other Control is below the Mean and, additionally, there are more than  4SD between the two results
        '''          When applied Within Control and Across Runs, for each Control, verify if for the last two runs, one result is upper the Mean and  
        '''          the other is below the Mean and, additionally, there are more than 4SD between the two results
        '''    4-1s: When applied Across Controls and Across Runs, verify if the last two Results of both Controls are out of +/- 1SD 
        '''          When applied Within Control and Across Runs, verify if the last four Results of  each Control are out of +/- 1SD
        '''    10Xm: When applied Across Controls and Across Runs, verify if the last five Results of both Controls are all below or upper the Mean
        '''          When applied Within Control and Across Runs, verify if the last ten Results of  each Control are all below or upper the Mean
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAllResultsDS">Typed Dataset QCResultsForWestgardDS containing results for the pair of controls to which the active
        '''                             multirules have to be applied, grouped by Serie</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all Results having non Westgard Alarms (Out of Range Alarm)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultAlarms containing the list of results with Alarms</returns>
        ''' <remarks>
        ''' Created by:  TR 01/06/2011
        ''' Modified by: SA 08/06/2011 - Apply rule R-4s before 4-1s; changed application of rule 2-2s (results have to exceed +/-2s but in the same
        '''                              direction); changed application of rule 10x (last five results of one Control can be below the Mean and the last 
        '''                              five results of the other can be upper the Mean, or vice versa); apply rest of rules only if the 1-2s alarm was 
        '''                              violated for the runs being processed or when it was not applied; solved some code errors
        '''              SA 05/01/2012 - Removed application of R-4s Within Control and Across Runs
        '''                              Changed order of application of Rules 2-2s and R-4s: first apply 2-2s Across Controls/Within Runs; next R-4s
        '''                              Across Controls/Within Run and finally, 2-2s Within Control/Across Runs
        '''              SA 25/04/2012 - Applied following changes in the way the selected Multirules are applied:
        '''                              ** If rule 1-2s was not violated for the runs being processed, the rest of the rules are also applied (the opposite  
        '''                                 change described in 08/06/2011 is cancelled)
        '''                              ** Rule R-4s is applied Within Control and Across Runs (the opposite change described in 05/01/2012 is cancelled)
        '''                              ** Changed application of rule R-4s when applied Across Controls and Within Run:
        '''                                 (a) Current implementation: verify if the last Result for one Control is out of +2SD and the last Result for the 
        '''                                     other Control is out of –2SD
        '''                                 (b) New implementation: verify if the last result for one Control is upper the Mean and the last result for the other
        '''                                     Control is below the Mean and, additionally, there are more than  4SD between the two results 
        '''                              ** Changed application of rule R-4s when applied Within Control and Across Runs:
        '''                                 (a) Current implementation: verify if for the last two Results of each Control, one is out of +2SD and the other 
        '''                                     is out of –2SD 
        '''                                 (b) New implementation: for each Control, verify if for the last two runs, one result is upper the Mean and the 
        '''                                     other is below the Mean and, additionally, there are more than 4SD between the two results.
        '''              TR 23/05/2012 - Implementation changed due to the function receives the results for both controls in an unique dataset instead on in
        '''                              two (one for each control). In this way it is possible to manage rule application when for the same Serie (RunNumber) 
        '''                              there are not results for both controls
        '''              SA 30/05/2012 - Code for each Rule was moved to individual functions (one for Rule). Changed this function to call these new individual
        '''                              functions by Rule
        ''' </remarks>
        Private Function ApplyMultiRulesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pAnalyzerID As String, _
                                            ByVal pAllResultsDS As QCResultsForWestgardDS, ByVal pQCResultAlarmsDS As QCResultAlarms) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing

            Try
                Dim conditionOK As Boolean = False
                Dim belowMeanCtrl1 As Boolean = False
                Dim belowMeanCtrl2 As Boolean = False

                'Get the list of multirules to apply to the specified QCTestSampleID
                Dim myHistoryTestSamplesRulesDelegate As New HistoryTestSamplesRulesDelegate
                myGlobalDataTO = myHistoryTestSamplesRulesDelegate.ReadByQCTestSampleID(pDBConnection, pQCTestSampleID)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myHistoryTestSamplesRulesDS As HistoryTestSamplesRulesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesRulesDS)

                    If (myHistoryTestSamplesRulesDS.tqcHistoryTestSamplesRules.Count > 0) Then
                        'Move the selected Rules to a List
                        Dim myHistTestSampRulesList As List(Of HistoryTestSamplesRulesDS.tqcHistoryTestSamplesRulesRow)
                        myHistTestSampRulesList = (From a As HistoryTestSamplesRulesDS.tqcHistoryTestSamplesRulesRow In myHistoryTestSamplesRulesDS.tqcHistoryTestSamplesRules _
                                                 Select a).ToList()

                        'Set the value to the TotalRuns
                        Dim myTotalRuns As Integer = pAllResultsDS.tQCResultsForWestgard.Count
                        Dim myAlarmCtrl As Boolean

                        For i As Integer = 0 To myTotalRuns - 1
                            myAlarmCtrl = False

                            'WESTGARD_1-2s - Warning Rule, applied always
                            If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").Count = 1) Then
                                ApplyRule_1x2s(pAnalyzerID, pAllResultsDS, i, pQCResultAlarmsDS)
                            End If

                            'Continue applying the rest of selected rules..
                            'WESTGARD_1-3s
                            If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").Count = 1) Then
                                myAlarmCtrl = ApplyRule_1x3s(pAnalyzerID, pAllResultsDS, i, pQCResultAlarmsDS)
                            End If

                            'WESTGARD_2-2s - Across Controls/Within Run
                            If (Not myAlarmCtrl) Then
                                If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").Count = 1) Then
                                    myAlarmCtrl = ApplyRule_2x2s(pAnalyzerID, pAllResultsDS, i, pQCResultAlarmsDS, True)
                                End If
                            End If

                            'WESTGARD_R-4s 
                            If (Not myAlarmCtrl) Then
                                If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").Count = 1) Then
                                    myAlarmCtrl = ApplyRule_Rx4s(pAnalyzerID, pAllResultsDS, i, pQCResultAlarmsDS)
                                End If
                            End If

                            'WESTGARD_2-2s Within Control/Across Runs
                            If (Not myAlarmCtrl) Then
                                If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").Count = 1) Then
                                    myAlarmCtrl = ApplyRule_2x2s(pAnalyzerID, pAllResultsDS, i, pQCResultAlarmsDS, False)
                                End If
                            End If

                            'WESTGARD_4-1s 
                            If (Not myAlarmCtrl) Then
                                If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").Count = 1) Then
                                    myAlarmCtrl = ApplyRule_4x1s(pAnalyzerID, pAllResultsDS, i, pQCResultAlarmsDS)
                                End If
                            End If

                            'WESTGARD_10X  
                            If (Not myAlarmCtrl) Then
                                If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_10X").Count = 1) Then
                                    myAlarmCtrl = ApplyRule_10X(pAnalyzerID, pAllResultsDS, i, pQCResultAlarmsDS)
                                End If
                            End If
                        Next i

                        myGlobalDataTO.SetDatos = pQCResultAlarmsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyMultiRules", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Apply rule 1-2s for results of the specified QCTestSampleID in the informed Serie
        ''' ** 1-2s is a Warning Rule. Verify if the Result for each Control is out of +/- 2SD.
        ''' ** It is applied Within Control and Within Run. 
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAllResultsDS">Typed Dataset QCResultsForWestgardDS containing results for the pair of controls to which the rule
        '''                             1-2s has to be applied, grouped by Serie</param>
        ''' <param name="pSerie">Number of Serie to which the rule has to be applied</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all results with alarms</param>
        ''' <remarks>
        ''' Created by: SA 30/05/2012 - Code moved from function ApplyMultiRules
        ''' </remarks>
        Private Sub ApplyRule_1x2s(ByVal pAnalyzerID As String, ByVal pAllResultsDS As QCResultsForWestgardDS, ByVal pSerie As Integer, _
                                   ByVal pQCResultAlarmsDS As QCResultAlarms)
            Try
                Dim i As Integer = pSerie

                'Within Control/Within Run --> Verify if the Result for the first Control is out of +/- 2SD
                If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError1Null) Then
                    If (Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError1) > 2) Then
                        'Insert Alarm and set ValidationStatus = WARNING for the first Control
                        InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID1, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                                  pAnalyzerID, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber1, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                                  "WESTGARD_1-2s")

                        pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                        pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus1 = "WARNING"
                        pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                    End If
                End If

                'Within Control/Within Run --> Verify if the Result for the second Control is out of +/- 2SD
                If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError2Null) Then
                    If (Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError2) > 2) Then
                        'Insert Alarm and set ValidationStatus = WARNING for the second Control
                        InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID2, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                                  pAnalyzerID, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber2, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                                  "WESTGARD_1-2s")

                        pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                        pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus2 = "WARNING"
                        pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                    End If
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyRule_1x2s", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Apply rule 1-3s for results of the specified QCTestSampleID in the informed Serie
        ''' ** Verify if the result for each control is out of +/-3SD
        ''' ** It is applied Within Control and Within Run
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAllResultsDS">Typed Dataset QCResultsForWestgardDS containing results for the pair of controls to which the rule
        '''                             1-3s has to be applied, grouped by Serie</param>
        ''' <param name="pSerie">Number of Serie to which the rule has to be applied</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all results with alarms</param>
        ''' <returns>TRUE when the alarm for rule 1-3s was raised for at least one of the results
        '''          Otherwise, it returns FALSE</returns>
        ''' <remarks>
        ''' Created by: SA 30/05/2012 - Code moved from function ApplyMultiRules
        ''' </remarks>
        Private Function ApplyRule_1x3s(ByVal pAnalyzerID As String, ByVal pAllResultsDS As QCResultsForWestgardDS, ByVal pSerie As Integer, _
                                        ByVal pQCResultAlarmsDS As QCResultAlarms) As Boolean
            Try
                Dim i As Integer = pSerie
                Dim myAlarmCtrl1 As Boolean = False
                Dim myAlarmCtrl2 As Boolean = False

                'Within Control/Within Run --> Verify if the Result for the first Control is out of +/- 3SD
                If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError1Null) Then
                    If (Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError1) > 3) Then
                        'Insert Alarm and set ValidationStatus = ERROR for the first Control
                        InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID1, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                                  pAnalyzerID, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber1, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                                  "WESTGARD_1-3s")

                        pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                        pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus1 = "ERROR"
                        pAllResultsDS.tQCResultsForWestgard(i).EndEdit()

                        myAlarmCtrl1 = True
                    End If
                End If

                'Within Control/Within Run --> Verify if the Result for the second Control is out of +/- 3SD
                If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError2Null) Then
                    If (Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError2) > 3) Then
                        'Insert Alarm and set ValidationStatus = ERROR for the second Control
                        InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID2, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                                  pAnalyzerID, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber2, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                                  "WESTGARD_1-3s")

                        pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                        pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus2 = "ERROR"
                        pAllResultsDS.tQCResultsForWestgard(i).EndEdit()

                        myAlarmCtrl2 = True
                    End If
                End If

                'Return TRUE if alarm for rule 1-3s was raised for at least one of the results
                Return (myAlarmCtrl1 Or myAlarmCtrl2)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyRule_1x3s", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Apply rule 2-2s for results of the specified QCTestSampleID in the informed Serie
        ''' ** When applied Across Controls and Within Run, verify if the Results for both Controls are out of +/- 2SD in the same direction
        ''' ** When applied Within Control and Across Runs, verify if the last two Runs of each Control are out of +/- 2SD in the same direction
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAllResultsDS">Typed Dataset QCResultsForWestgardDS containing results for the pair of controls to which the rule
        '''                             2-2s has to be applied, grouped by Serie</param>
        ''' <param name="pSerie">Number of Serie to which the rule has to be applied</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all results with alarms</param>
        ''' <param name="pAcross">When TRUE, it indicates the rule has to be applied Across Controls/Within Run
        '''                       When FALSE, it indicates the rule has to be applied Within Control/Across Runs</param>
        ''' <returns>True when the alarm for rule 2-2s was raised for at least one of the results</returns>
        ''' <remarks>
        ''' Created by: SA 30/05/2012 - Code moved from function ApplyMultiRules
        ''' </remarks>
        Private Function ApplyRule_2x2s(ByVal pAnalyzerID As String, ByVal pAllResultsDS As QCResultsForWestgardDS, ByVal pSerie As Integer, _
                                        ByVal pQCResultAlarmsDS As QCResultAlarms, ByVal pAcross As Boolean) As Boolean
            Try
                Dim i As Integer = pSerie
                Dim myAlarmCtrl1 As Boolean = False
                Dim myAlarmCtrl2 As Boolean = False

                If (pAcross) Then
                    'Across Controls/Within Run --> Verify if the Results for both Controls are out of +/- 2SD in the same direction
                    If (pAllResultsDS.tQCResultsForWestgard(i).PairOK) Then
                        If (pAllResultsDS.tQCResultsForWestgard(i).RelError1 > 2 AndAlso pAllResultsDS.tQCResultsForWestgard(i).RelError2 > 2) OrElse _
                           (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < -2 AndAlso pAllResultsDS.tQCResultsForWestgard(i).RelError2 < -2) Then
                            myAlarmCtrl1 = True
                            myAlarmCtrl2 = True
                        End If
                    End If
                Else
                    'Within Control/Across Runs --> Verify if the last two Results for each Control are out of +/- 2SD in the same direction
                    '                               This verification can be done only when the previous Result has not been rejected
                    Dim myRowIndex As Integer = i
                    Dim myPreviousIndex As Integer
                    Dim myFilterResults As List(Of QCResultsForWestgardDS.tQCResultsForWestgardRow)

                    'Apply the rule for the first Control
                    If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError1Null) Then
                        'Search the previous result for the same Control
                        myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where Not a.IsRelError1Null _
                                        AndAlso a.RowIndex < myRowIndex _
                                       Order By a.RowIndex Descending _
                                         Select a).ToList()

                        If (myFilterResults.Count > 0) Then
                            myPreviousIndex = myFilterResults.First().RowIndex

                            If (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR") Then
                                If (pAllResultsDS.tQCResultsForWestgard(i).RelError1 > 2 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 > 2) OrElse _
                                   (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < -2 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 < -2) Then
                                    myAlarmCtrl1 = True
                                End If
                            End If
                        End If
                    End If

                    'Apply the rule for the second Control
                    If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError2Null) Then
                        'Search the previous result for the same Control
                        myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where Not a.IsRelError2Null _
                                        AndAlso a.RowIndex < myRowIndex _
                                       Order By a.RowIndex Descending _
                                         Select a).ToList()

                        If (myFilterResults.Count > 0) Then
                            myPreviousIndex = myFilterResults.First().RowIndex

                            If (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR") Then
                                If (pAllResultsDS.tQCResultsForWestgard(i).RelError2 > 2 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 > 2) OrElse _
                                   (pAllResultsDS.tQCResultsForWestgard(i).RelError2 < -2 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 < -2) Then
                                    myAlarmCtrl2 = True
                                End If
                            End If
                        End If
                    End If
                End If

                If (myAlarmCtrl1) Then
                    'Insert Alarm and set ValidationStatus = ERROR for the first Control
                    InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID1, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                              pAnalyzerID, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber1, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                              "WESTGARD_2-2s")

                    pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                    pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus1 = "ERROR"
                    pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                End If

                If (myAlarmCtrl2) Then
                    'Insert Alarm and set ValidationStatus = ERROR for the second Control
                    InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID2, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                              pAnalyzerID, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber2, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                              "WESTGARD_2-2s")

                    pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                    pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus2 = "ERROR"
                    pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                End If

                'Return TRUE if alarm for rule 2-2s was raised for at least one of the results
                Return (myAlarmCtrl1 Or myAlarmCtrl2)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyRule_2x2s", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Apply rule R-4s for results of the specified QCTestSampleID in the informed Serie
        ''' ** When applied Across Controls and Within Run, verify if the last result for one Control is upper the Mean and the last result for 
        '''    the other Control is below the Mean and, additionally, there are more than  4SD between the two results
        ''' ** When applied Within Control and Across Runs, for each Control, verify if for the last two runs, one result is upper the Mean and  
        '''    the other is below the Mean and, additionally, there are more than 4SD between the two results
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAllResultsDS">Typed Dataset QCResultsForWestgardDS containing results for the pair of controls to which the rule
        '''                             R-4s has to be applied, grouped by Serie</param>
        ''' <param name="pSerie">Number of Serie to which the rule has to be applied</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all results with alarms</param>
        ''' <returns>True when the alarm for rule R-4s was raised for at least one of the results</returns>
        ''' <remarks>
        ''' Created by: SA 30/05/2012 - Code moved from function ApplyMultiRules
        ''' </remarks>
        Private Function ApplyRule_Rx4s(ByVal pAnalyzerID As String, ByVal pAllResultsDS As QCResultsForWestgardDS, ByVal pSerie As Integer, _
                                        ByVal pQCResultAlarmsDS As QCResultAlarms) As Boolean
            Try
                Dim i As Integer = pSerie
                Dim myAlarmCtrl1 As Boolean = False
                Dim myAlarmCtrl2 As Boolean = False

                'Across Controls/Within Run --> Verify if the last Result for one Control is upper the Mean and the last Result for the other  
                '                               Control is below the Mean and in this case, if there are more than 4SD between them
                If (pAllResultsDS.tQCResultsForWestgard(i).PairOK) Then
                    If (pAllResultsDS.tQCResultsForWestgard(i).RelError1 > 0 AndAlso pAllResultsDS.tQCResultsForWestgard(i).RelError2 < 0) OrElse _
                       (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < 0 AndAlso pAllResultsDS.tQCResultsForWestgard(i).RelError2 > 0) Then
                        If ((Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError1) + Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError2)) > 4) Then
                            myAlarmCtrl1 = True
                            myAlarmCtrl2 = True
                        End If
                    End If
                End If

                'Within Control/Across Runs --> Verify if for the last two Results of each Control, one is upper the Mean and the other is below 
                '                               the Mean, and in this case, if there are more than 4SD between them. This verification can be 
                '                               done only when the previous Result has not been rejected.
                If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2) Then
                    Dim myRowIndex As Integer = i
                    Dim myPreviousIndex As Integer
                    Dim myFilterResults As List(Of QCResultsForWestgardDS.tQCResultsForWestgardRow)

                    'Apply the rule for the first Control
                    If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError1Null) Then
                        'Search the previous result for the same Control
                        myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where Not a.IsRelError1Null _
                                        AndAlso a.RowIndex < myRowIndex _
                                       Order By a.RowIndex Descending _
                                         Select a).ToList()

                        If (myFilterResults.Count > 0) Then
                            myPreviousIndex = myFilterResults.First().RowIndex

                            If (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR") Then
                                If (pAllResultsDS.tQCResultsForWestgard(i).RelError1 > 0 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 < 0) OrElse _
                                   (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < 0 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 > 0) Then
                                    If ((Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError1) + Math.Abs(pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1)) > 4) Then
                                        myAlarmCtrl1 = True
                                    End If
                                End If
                            End If
                        End If
                    End If

                    'Apply the rule for the second Control
                    If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError2Null) Then
                        'Search the previous result for the same Control
                        myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where Not a.IsRelError2Null _
                                        AndAlso a.RowIndex < myRowIndex _
                                       Order By a.RowIndex Descending _
                                         Select a).ToList()

                        If (myFilterResults.Count > 0) Then
                            myPreviousIndex = myFilterResults.First().RowIndex

                            If (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR") Then
                                If (pAllResultsDS.tQCResultsForWestgard(i).RelError2 > 0 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 < 0) OrElse _
                                   (pAllResultsDS.tQCResultsForWestgard(i).RelError2 < 0 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 > 0) Then
                                    If ((Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError2) + Math.Abs(pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2)) > 4) Then
                                        myAlarmCtrl2 = True
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                If (myAlarmCtrl1) Then
                    'Insert Alarm and set ValidationStatus = ERROR for the first Control
                    InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID1, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                              pAnalyzerID, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber1, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                              "WESTGARD_R-4s")

                    pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                    pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus1 = "ERROR"
                    pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                End If

                If (myAlarmCtrl2) Then
                    'Insert Alarm and set ValidationStatus = ERROR for the second Control
                    InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID2, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                              pAnalyzerID, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber2, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                              "WESTGARD_R-4s")

                    pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                    pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus2 = "ERROR"
                    pAllResultsDS.tQCResultsForWestgard(i).EndEdit()

                End If

                'Return TRUE if alarm for rule R-4s was raised for at least one of the results
                Return (myAlarmCtrl1 Or myAlarmCtrl2)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyRule_Rx4s", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Apply rule 4-1s for results of the specified QCTestSampleID in the informed Serie
        ''' ** When applied Across Controls and Across Runs, verify if the last two Results of both Controls are out of +/- 1SD in the same direction
        ''' ** When applied Within Control and Across Runs, verify if the last four Results of  each Control are out of +/- 1SD in the same direction
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAllResultsDS">Typed Dataset QCResultsForWestgardDS containing results for the pair of controls to which the rule 4-1s has to
        '''                             be applied, grouped by Serie</param>
        ''' <param name="pSerie">Number of Serie to which the rule has to be applied</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all results with alarms</param>
        ''' <returns>True when the alarm for rule 4-1s was raised for at least one of the results</returns>
        ''' <remarks>
        ''' Created by:  SA 30/05/2012 - Code moved from function ApplyMultiRules
        ''' Modified by: SA 03/07/2012 - When the rule is applied, all results have to be out of +/-1SD but IN THE SAME DIRECTION
        '''              SA 19/09/2012 - When verifying if results are out of -1SD, use -1 instead of 1 to compare values 
        ''' </remarks>
        Private Function ApplyRule_4x1s(ByVal pAnalyzerID As String, ByVal pAllResultsDS As QCResultsForWestgardDS, ByVal pSerie As Integer, _
                                        ByVal pQCResultAlarmsDS As QCResultAlarms) As Boolean
            Try
                Dim i As Integer = pSerie
                Dim myAlarmCtrl1 As Boolean = False
                Dim myAlarmCtrl2 As Boolean = False

                Dim myRowIndex As Integer = i
                Dim myPreviousIndex As Integer
                Dim myFilterResults As List(Of QCResultsForWestgardDS.tQCResultsForWestgardRow)

                'Across Controls/Across Runs --> Verify if the last two Results of both Controls are out of +/- 1SD in the same direction 
                '                                This verification can be done only when the previous Results have not been rejected
                If (pAllResultsDS.tQCResultsForWestgard(i).PairOK) Then
                    'Search a previous Serie having results for both Controls
                    myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where a.RowIndex < myRowIndex _
                                    AndAlso a.PairOK _
                                   Order By a.RowIndex Descending _
                                     Select a).ToList()

                    If (myFilterResults.Count > 0) Then
                        myPreviousIndex = myFilterResults.First().RowIndex

                        If (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR" AndAlso _
                            pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR") Then
                            If (pAllResultsDS.tQCResultsForWestgard(i).RelError1 > 1 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 > 1) And _
                               (pAllResultsDS.tQCResultsForWestgard(i).RelError2 > 1 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 > 1) Then
                                'The last two Results of both Controls are out of +1SD 
                                myAlarmCtrl1 = True
                                myAlarmCtrl2 = True

                            ElseIf (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < -1 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 < -1) And _
                                   (pAllResultsDS.tQCResultsForWestgard(i).RelError2 < -1 AndAlso pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 < -1) Then
                                'The last two Results of both Controls are out of -1SD
                                myAlarmCtrl1 = True
                                myAlarmCtrl2 = True
                            End If
                        End If
                    End If
                End If

                'Within Control/Across Runs --> Verify if the last four Results of each Control are out of +/- 1SD in the same direction
                '                               This verification can be done only when none of the previous Results used have been rejected.
                If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2) Then
                    Dim belowMean As Boolean
                    Dim conditionOK As Boolean

                    'Apply the rule for the first Control
                    If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError1Null AndAlso Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError1) > 1) Then
                        'Search the previous three results for the same Control
                        myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where Not a.IsRelError1Null _
                                        AndAlso a.RowIndex < myRowIndex _
                                       Order By a.RowIndex Descending _
                                         Select a).ToList()

                        If (myFilterResults.Count >= 3) Then
                            conditionOK = True
                            belowMean = (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < 0)

                            For j As Integer = 0 To 2
                                myPreviousIndex = myFilterResults(j).RowIndex
                                If (belowMean) Then
                                    conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR") AndAlso _
                                                  (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 < -1)
                                Else
                                    conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR") AndAlso _
                                                  (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 > 1)
                                End If
                                If (Not conditionOK) Then Exit For
                            Next

                            If (conditionOK) Then myAlarmCtrl1 = True
                        End If
                    End If

                    'Apply the rule for the second Control
                    If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError2Null AndAlso Math.Abs(pAllResultsDS.tQCResultsForWestgard(i).RelError2) > 1) Then
                        'Search the previous three results for the same Control
                        myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where Not a.IsRelError2Null _
                                        AndAlso a.RowIndex < myRowIndex _
                                       Order By a.RowIndex Descending _
                                         Select a).ToList()

                        If (myFilterResults.Count >= 3) Then
                            conditionOK = True
                            belowMean = (pAllResultsDS.tQCResultsForWestgard(i).RelError2 < 0)

                            For j As Integer = 0 To 2
                                myPreviousIndex = myFilterResults(j).RowIndex
                                If (belowMean) Then
                                    conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR") AndAlso _
                                                  (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 < -1)
                                Else
                                    conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR") AndAlso _
                                                  (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 > 1)
                                End If
                                If (Not conditionOK) Then Exit For
                            Next
                            If (conditionOK) Then myAlarmCtrl2 = True
                        End If
                    End If
                End If

                If (myAlarmCtrl1) Then
                    'Insert Alarm and set ValidationStatus = ERROR for the first Control
                    InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID1, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                              pAnalyzerID, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber1, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                              "WESTGARD_4-1s")

                    pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                    pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus1 = "ERROR"
                    pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                End If

                If (myAlarmCtrl2) Then
                    'Insert Alarm and set ValidationStatus = ERROR for the second Control
                    InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID2, _
                                              pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                              pAnalyzerID, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber2, _
                                              pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                              "WESTGARD_4-1s")

                    pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                    pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus2 = "ERROR"
                    pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                End If

                'Return TRUE if alarm for rule R-4s was raised for at least one of the results
                Return (myAlarmCtrl1 Or myAlarmCtrl2)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyRule_4x1s", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Apply rule 10X for results of the specified QCTestSampleID in the informed Serie
        ''' ** When applied Across Controls and Across Runs, verify if the last five Results of both Controls are all below or upper the Mean
        ''' ** When applied Within Control and Across Runs, verify if the last ten Results of  each Control are all below or upper the Mean
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pAllResultsDS">Typed Dataset QCResultsForWestgardDS containing results for the pair of controls to which the rule
        '''                             10X has to be applied, grouped by Serie</param>
        ''' <param name="pSerie">Number of Serie to which the rule has to be applied</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all results with alarms</param>
        ''' <returns>True when the alarm for rule 10X was raised for at least one of the results</returns>
        ''' <remarks>
        ''' Created by:  SA 30/05/2012 - Code moved from function ApplyMultiRules
        ''' Modified by: SA 23/07/2013 - Fixed error when rule is applied Across Controls/Across Runs: last five results of both Controls 
        '''                              have to be below or upper the Mean (currently, the 10X is raised when last five results of Control 1 are
        '''                              below the Mean and last five results of Control 2 are upper the Mean, or viceversa). BugTracking #1096
        '''                              NOTE: Code is commented...
        ''' </remarks>
        Private Function ApplyRule_10X(ByVal pAnalyzerID As String, ByVal pAllResultsDS As QCResultsForWestgardDS, ByVal pSerie As Integer, _
                                       ByVal pQCResultAlarmsDS As QCResultAlarms) As Boolean
            Try
                Dim i As Integer = pSerie
                Dim myRowIndex As Integer = i

                Dim myAlarmCtrl1 As Boolean = False
                Dim myAlarmCtrl2 As Boolean = False

                Dim conditionOK As Boolean
                Dim belowMeanCtrl1 As Boolean
                Dim belowMeanCtrl2 As Boolean
                Dim myPreviousIndex As Integer
                Dim myFilterResults As List(Of QCResultsForWestgardDS.tQCResultsForWestgardRow)

                'Across Controls/Across Runs --> Verify if the last five Results of both Controls are all below the Mean, or all upper the Mean
                '                                This verification can be done only when none of the previous Results used have been rejected
                If (pAllResultsDS.tQCResultsForWestgard(i).PairOK) Then
                    'Search four previous Series having results for both Controls
                    myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                      Where a.RowIndex < myRowIndex _
                                    AndAlso a.PairOK _
                                   Order By a.RowIndex Descending _
                                     Select a).ToList()

                    If (myFilterResults.Count >= 4) Then
                        belowMeanCtrl1 = (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < 0)
                        For j As Integer = 0 To 3
                            myPreviousIndex = myFilterResults(j).RowIndex

                            If (belowMeanCtrl1) Then
                                conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR" AndAlso _
                                               pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 < 0)
                            Else
                                conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR" AndAlso _
                                               pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 > 0)
                            End If
                            If (Not conditionOK) Then Exit For
                        Next

                        If (conditionOK) Then
                            belowMeanCtrl2 = (pAllResultsDS.tQCResultsForWestgard(i).RelError2 < 0)
                            If (belowMeanCtrl1 = belowMeanCtrl2) Then 'If (belowMeanCtrl1 = belowMeanCtrl2) Then 'JV 18/12/2013 #1096
                                For j As Integer = 0 To 3
                                    myPreviousIndex = myFilterResults(j).RowIndex

                                    If (belowMeanCtrl2) Then
                                        conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR" AndAlso _
                                                       pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 < 0)
                                    Else
                                        conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR" AndAlso _
                                                       pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 > 0)
                                    End If
                                    If (Not conditionOK) Then Exit For
                                Next
                            Else 'Else 'JV 18/12/2013 #1096
                                conditionOK = False 'conditionOK = False 'JV 18/12/2013 #1096
                            End If 'End If 'JV 18/12/2013 #1096
                        End If

                            If (conditionOK) Then
                                myAlarmCtrl1 = True
                                myAlarmCtrl2 = True
                            End If
                        End If
                    End If

                    'Within Control/Across Runs --> Verify if the last ten Results of each Control are all below the Mean, or all upper the Mean
                    '                               This verification can be done only when none of the previous Results used have been rejected
                    If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2) Then
                        'Apply the rule for the first Control
                        If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError1Null) Then
                            'Search the previous nine results for the same Control
                            myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                          Where Not a.IsRelError1Null _
                                            AndAlso a.RowIndex < myRowIndex _
                                           Order By a.RowIndex Descending _
                                             Select a).ToList()

                            If (myFilterResults.Count >= 9) Then
                                belowMeanCtrl1 = (pAllResultsDS.tQCResultsForWestgard(i).RelError1 < 0)
                                For j As Integer = 0 To 8
                                    myPreviousIndex = myFilterResults(j).RowIndex

                                    If (belowMeanCtrl1) Then
                                        conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR" AndAlso _
                                                       pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 < 0)
                                    Else
                                        conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus1 <> "ERROR" AndAlso _
                                                       pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError1 > 0)
                                    End If
                                    If (Not conditionOK) Then Exit For
                                Next

                                If (conditionOK) Then myAlarmCtrl1 = True
                            End If
                        End If

                        'Apply the rule for the second Control
                        If (Not pAllResultsDS.tQCResultsForWestgard(i).IsRelError2Null) Then
                            'Search the previous nine results for the same Control
                            myFilterResults = (From a In pAllResultsDS.tQCResultsForWestgard _
                                           Where Not a.IsRelError2Null _
                                             AndAlso a.RowIndex < myRowIndex _
                                            Order By a.RowIndex Descending _
                                              Select a).ToList()

                            If (myFilterResults.Count >= 9) Then
                                belowMeanCtrl2 = (pAllResultsDS.tQCResultsForWestgard(i).RelError2 < 0)
                                For j As Integer = 0 To 8
                                    myPreviousIndex = myFilterResults(j).RowIndex

                                    If (belowMeanCtrl2) Then
                                        conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR" AndAlso _
                                                       pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 < 0)
                                    Else
                                        conditionOK = (pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).ValidationStatus2 <> "ERROR" AndAlso _
                                                       pAllResultsDS.tQCResultsForWestgard(myPreviousIndex).RelError2 > 0)
                                    End If
                                    If (Not conditionOK) Then Exit For
                                Next

                                If (conditionOK) Then myAlarmCtrl2 = True
                            End If
                        End If
                    End If

                    If (myAlarmCtrl1) Then
                        'Insert Alarm and set ValidationStatus = ERROR for the first Control
                        InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID1, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                                  pAnalyzerID, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber1, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                                  "WESTGARD_10X")

                        pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                        pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus1 = "ERROR"
                        pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                    End If

                    If (myAlarmCtrl2) Then
                        'Insert Alarm and set ValidationStatus = ERROR for the second Control
                        InsertNewQcResultAlarmNEW(pQCResultAlarmsDS, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCControlLotID2, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).QCTestSampleID, _
                                                  pAnalyzerID, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunsGroupNumber2, _
                                                  pAllResultsDS.tQCResultsForWestgard(i).RunNumber, _
                                                  "WESTGARD_10X")

                        pAllResultsDS.tQCResultsForWestgard(i).BeginEdit()
                        pAllResultsDS.tQCResultsForWestgard(i).ValidationStatus2 = "ERROR"
                        pAllResultsDS.tQCResultsForWestgard(i).EndEdit()
                    End If

                    'Return TRUE if alarm for rule R-10X was raised for at least one of the Controls
                    Return (myAlarmCtrl1 Or myAlarmCtrl2)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyRule_10X", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Get the physical name of the Icon identified for the specified key
        ''' (from table tfmwPreloadedMasterData, SubTable ICON_PATHS in the DB)
        ''' </summary>
        ''' <param name="pIconKey">Icon Identifier</param>
        ''' <returns>The physical name of the Icon identified for the specified key</returns>
        ''' <remarks>
        ''' Created by:  TR 09/04/2010
        ''' </remarks>
        Private Function GetIconName(ByVal pIconKey As String) As String
            Dim myIconName As String = ""
            Try
                Dim myGlobalDataTO As New GlobalDataTO
                Dim getPreloadedMasterData As New PreloadedMasterDataDelegate

                myGlobalDataTO = getPreloadedMasterData.GetSubTableItem(Nothing, PreloadedMasterDataEnum.ICON_PATHS, pIconKey)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMasterDataDS As New PreloadedMasterDataDS
                    myMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                    If (myMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                        myIconName = myMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc.Trim
                    End If
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetControlList", EventLogEntryType.Error, False)
            End Try
            Return myIconName
        End Function

        ''' <summary>
        ''' Create a list of QCControlLotIDs separated by commas 
        ''' </summary>
        ''' <param name="pOpenQCResultsDS">Typed DataSet OpenQCResultsDS containing all Controls/Lots</param>
        ''' <returns>GlobalDataTO containing a string with the list of QCControlLotIDs separated by commas</returns>
        ''' <remarks>
        ''' Created by: TR 01/06/2011
        ''' </remarks>
        Private Function GetQCControlList(ByVal pOpenQCResultsDS As OpenQCResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Build a string list with all QCControlLotIDs
                Dim mycontrolIDList As String = ""
                Dim qControlID As New List(Of Integer)

                qControlID = (From a In pOpenQCResultsDS.tOpenResults _
                            Select a.QCControlLotID).ToList
                If (qControlID.Count > 0) Then
                    If (qControlID.Count > 1) Then
                        For Each controlID As String In qControlID
                            mycontrolIDList &= controlID.ToString() & ","
                        Next
                        'Remove the last comma
                        mycontrolIDList = mycontrolIDList.Remove(mycontrolIDList.Count - 1, 1)
                    Else
                        mycontrolIDList = qControlID.First.ToString()
                    End If
                End If
                myGlobalDataTO.SetDatos = mycontrolIDList

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetControlList", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Insert a row in a QCResultAlarmsDS for a QC Result and the specified Alarm Code
        ''' </summary>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarmsDS containing all alarms raised for QC Results</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Runs Group Number</param>
        ''' <param name="pRunsNumber">Run Number for the Result</param>
        ''' <param name="pRuleID">Code of the Westgard Ruleto insert as alarm for the result</param>
        ''' <remarks>
        ''' Created by:  TR 23/05/2012
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID 
        ''' </remarks>
        Private Sub InsertNewQcResultAlarmNEW(ByRef pQCResultAlarmsDS As QCResultAlarms, ByVal pQCControlLotID As Integer, ByVal pQCTestSampleID As Integer, _
                                              ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer, ByVal pRunsNumber As Integer, ByVal pRuleID As String)
            Try
                'Add the Alarm to the QC Result in the QCResultAlarmsDS  
                Dim myQCResultAlarmsRow As QCResultAlarms.tqcResultAlarmsRow

                myQCResultAlarmsRow = pQCResultAlarmsDS.tqcResultAlarms.NewtqcResultAlarmsRow
                myQCResultAlarmsRow.QCControlLotID = pQCControlLotID
                myQCResultAlarmsRow.QCTestSampleID = pQCTestSampleID
                myQCResultAlarmsRow.AnalyzerID = pAnalyzerID
                myQCResultAlarmsRow.RunsGroupNumber = pRunsGroupNumber
                myQCResultAlarmsRow.RunNumber = pRunsNumber
                myQCResultAlarmsRow.AlarmID = pRuleID

                pQCResultAlarmsDS.tqcResultAlarms.AddtqcResultAlarmsRow(myQCResultAlarmsRow)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.InsertNewQcResultAlarm", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "TEMPORARY - ONLY FOR QC RESULTS SIMULATOR"
        Public Function GetFirstDateTimeForResultsCreationNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pTestType As String, _
                                                              ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetFirstDateTimeForResultsCreationNEW(dbConnection, pAnalyzerID, pTestType, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetFirstDateTimeForResultsCreation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' THIS METHOD IS FOR QC SIMULATOR ONLY (DO NOT IMPLEMENT IN OTHER APPLICATION AREAS).
        ''' For the specified Analyzer WorkSession get the maximum CtrlsSendingGroup created. If there is not any CtrlsSendingGroup
        ''' then it returns zero.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analizer ID.</param>
        ''' <returns>
        ''' GlobalDataTO containing an integer value with the maximum CtrlsSendingGroup in the specified
        ''' Analyzer WorkSession. If there is not any CtrlsSendingGroup, then it contains zero
        ''' </returns>
        ''' <remarks>
        ''' CREATED BY: TR 24/07/2012
        ''' </remarks>
        Public Function GetMaxCtrlsSendingGroup(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultsDAO As New tqcResultsDAO
                        myGlobalDataTO = myQCResultsDAO.GetMaxCtrlsSendingGroup(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMaxCtrlsSendingGroup", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Get the number of non cumulated and not excluded QC Results for the specified QCTestSampleID and QCControlLotID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing an integer value with the number of non cumulated and not excluded QC Results
        ''''          for the specified QCTestSampleID and QCControlLotID</returns>
        '''' <remarks>
        '''' Created by:  SA 20/06/2011
        '''' </remarks>
        'Public Function CountNonCumulatedResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                         ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.CountNonCumulatedResultsOLD(dbConnection, pQCTestSampleID, pQCControlLotID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.CountNonCumulatedResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Insert new QC Results
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to add</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 17/05/2011
        '''' </remarks>
        'Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.Create(dbConnection, pQCResultsDS)

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.Create", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified Control/Lot, create a new cumulated serie for each one of the selected Tests/SampleTypes 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCSummaryByTestDS">Typed DataSet QCSummaryByTestSampleDS containing all Tests/SampleTypes linked to the
        ''''                                  informed Control/Lot with a flag that indicates for each one if its results have to
        ''''                                  be cumulated</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 10/06/2011
        '''' Modified by: SA 27/06/2011 - Added management of the created XML files
        ''''              SA 19/10/2011 - Management of deletion of XML Files when the first Cumulated Serie was deleted was bad done due to 
        ''''                              the name of the file was not saved inside the For/Next, then it works only for the last processed 
        ''''                              Test/SampleType, but not for the previous ones 
        '''' </remarks>
        'Public Function CumulateControlResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCSummaryByTestDS As QCSummaryByTestSampleDS, _
        '                                          ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        Dim xmlName As String = String.Empty

        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim selectedTests As List(Of QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow)
        '                selectedTests = (From a As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow In pQCSummaryByTestDS.QCSummaryByTestSampleTable _
        '                                Where a.Selected = True Select a).ToList

        '                For Each testSample As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow In selectedTests
        '                    Dim myCumulateDelegate As New CumulatedResultsDelegate
        '                    myGlobalDataTO = myCumulateDelegate.SaveCumulateResultOLD(dbConnection, testSample.QCTestSampleID, pQCControlLotID, True)
        '                    If (myGlobalDataTO.HasError) Then Exit For

        '                    'If the first Cumulated Serie was deleted, add the name of the XML File containing all its results
        '                    'to the list of XML Files to delete
        '                    If (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        If (DirectCast(myGlobalDataTO.SetDatos, String) <> String.Empty) Then
        '                            If (xmlName <> String.Empty) Then xmlName &= ","
        '                            xmlName &= DirectCast(myGlobalDataTO.SetDatos, String)
        '                        End If
        '                    End If
        '                Next
        '            End If
        '        End If

        '        If (Not myGlobalDataTO.HasError) Then
        '            'When the Database Connection was opened locally, then the Commit is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

        '            'Get names of all XML Files created in the TEMPORARY path
        '            Dim myCreatedFiles As String() = IO.Directory.GetFiles(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\")

        '            'Move all created XMLFiles from TEMP Dir to the final one
        '            For Each myFileName As String In myCreatedFiles
        '                IO.File.Move(myFileName, Application.StartupPath & GlobalBase.QCResultsFilesPath & myFileName.Substring(myFileName.LastIndexOf(CChar("\")) + 1))
        '            Next

        '            'Delete the TEMP Dir
        '            IO.Directory.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\", True)

        '            'If the first Cumulated Serie was deleted, delete also the XML File containing all its results
        '            If (xmlName <> String.Empty) Then
        '                Dim filesToDelete As String() = Split(xmlName, ",")
        '                For Each xmlFile As String In filesToDelete
        '                    IO.File.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & xmlFile)
        '                Next xmlFile
        '            End If
        '        Else
        '            'When the Database Connection was opened locally, then the Rollback is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '            'It the TEMP Dir was created, them delete it
        '            If (IO.Directory.Exists(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\")) Then
        '                IO.Directory.Delete(Application.StartupPath & GlobalBase.QCResultsFilesPath & "TEMP\", True)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.CumulateControlResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete a group of QC Results
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing the list of QC Results to delete</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 10/06/2011
        '''' Modified by: SA 06/07/2011 - Add parameter RunNumber when calling the function that delete the Alarms
        '''' </remarks>
        'Public Function DeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Before deleting the Results remove the Alarms
        '                Dim myResultsAlarmDS As New ResultAlarmsDS
        '                Dim myResultsAlarmDelegate As New QCResultAlarmsDelegate

        '                For Each qcResultRow As QCResultsDS.tqcResultsRow In pQCResultsDS.tqcResults.Rows
        '                    myGlobalDataTO = myResultsAlarmDelegate.DeleteOLD(dbConnection, qcResultRow.QCTestSampleID, qcResultRow.QCControlLotID, _
        '                                                                      qcResultRow.RunsGroupNumber, qcResultRow.RunNumber)
        '                    If (myGlobalDataTO.HasError) Then Exit For
        '                Next

        '                If (Not myGlobalDataTO.HasError) Then
        '                    Dim myQCResultsDAO As New tqcResultsDAO
        '                    myGlobalDataTO = myQCResultsDAO.DeleteOLD(dbConnection, pQCResultsDS)
        '                End If

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.Delete", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCTestSampleID and QCControlLotID, delete the QC Results included in the RunsGroup for 
        '''' the informed Cumulated Serie
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pCumResultsNum">Number of the Cumulated Serie to be deleted for the QCTestSampleID and QCControlLotID</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 21/06/2011 
        '''' </remarks>
        'Public Function DeleteByCumResultsNumOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                         ByVal pCumResultsNum As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.DeleteByCumResultsNumOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pCumResultsNum)

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.DeleteByCumResultsNum", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCTestSampleID and QCControlLotID, delete all the QC Results included in the informed RunsGroup,
        '''' including the Results Alarms
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNum">Number of the Runs Group in which the QC Results to delete are included</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 27/06/2011 
        '''' </remarks>
        'Public Function DeleteByRunsGroupNumOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                        ByVal pRunsGroupNum As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Delete the Results Alarms
        '                Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate
        '                myGlobalDataTO = myQCResultAlarmsDelegate.DeleteOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pRunsGroupNum)

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Delete the Results
        '                    Dim myQCResultsDAO As New tqcResultsDAO
        '                    myGlobalDataTO = myQCResultsDAO.DeleteByRunsGroupNumOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pRunsGroupNum)

        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'When the Database Connection was opened locally, then the Commit is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    Else
        '                        'When the Database Connection was opened locally, then the Rollback is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.DeleteByRunsGroupNum", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete all QC Results marked as included in calculation of statistical values for the 
        '''' informed Test/Sample Type, Control/Lot and Runs Group Number
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNumber">Optional parameter. Number of the open Runs Group for the specified
        ''''                                Test/SampleType and Control/Lot</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 15/12/2011
        '''' Modified by: SA 21/12/2011 - Parameter for the RunsGroupNumber changed to optional
        '''' </remarks>
        'Public Function DeleteStatisticResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                          ByVal pQCControlLotID As Integer, Optional ByVal pRunsGroupNumber As Integer = 0) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Delete all Alarms for the group of Results that will be deleted
        '                Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate
        '                myGlobalDataTO = myQCResultAlarmsDelegate.DeleteOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pRunsGroupNumber)

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Delete the group of Results
        '                    Dim myQCResultsDAO As New tqcResultsDAO
        '                    myGlobalDataTO = myQCResultsDAO.DeleteStatisticResultsOLD(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                End If

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.DeleteStatisticResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Select all Tests/SampleTypes with not cumulated QC Results for the specified Control/Lot
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCSummaryByTestSampleDS with statistical QC data for all 
        ''''          Tests/SampleTypes linked to the informed Control/Lot that have non cumulated QC Results</returns>
        '''' <remarks>
        '''' Created by:  SA 09/06/2011
        '''' Modified by: SA 27/06/2011 - For the selected Control/Lot, if there are QC Results pending to review for a linked Test/SampleType,
        ''''                              the results validation of all the non Cumulated QC Results is executed before verify if the icon of 
        ''''                              Results with Alarms have to be shown
        ''''              SA 05/05/2011 - Before process data of the returned Tests/SampleTypes to calculate values of Mean,SD, CV and Range, 
        ''''                              group data of all rows returned for each Test/SampleType in an unique row
        ''''              SA 01/12/2011 - Changed the function logic due to the change in the way the Statistical values are calculated                      
        '''' </remarks>
        'Public Function GetByQCControlLotIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.ReadByQCControlLotIDOLD(dbConnection, pQCControlLotID)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myFinalOpenResultsDS As OpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

        '                    'Get Icons for Preloaded and User-defined Standard Tests and also the Icon for Alarm
        '                    Dim preloadedDataConfig As New PreloadedMasterDataDelegate
        '                    Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
        '                    Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")
        '                    Dim imageAlarms As Byte() = preloadedDataConfig.GetIconImage("STUS_WITHERRS")

        '                    Dim myQCSummaryDS As New QCSummaryByTestSampleDS
        '                    Dim myQCSummaryRow As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow

        '                    Dim myLastCumulateValueDS As New CumulatedResultsDS
        '                    Dim myLastCumulateValueDelegate As New LastCumulatedValuesDelegate

        '                    Dim numByStatus As Integer = 0
        '                    Dim myNonCumulatedResultsDS As OpenQCResultsDS

        '                    For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myFinalOpenResultsDS.tOpenResults.Rows
        '                        If (openQCResultRow.CalculationMode = "STATISTIC") Then
        '                            'Calculate Mean, SD, CV and Ranges for the Control/Lot
        '                            myGlobalDataTO = GetResultsByControlLotForStatisticsModeOLD(dbConnection, openQCResultRow.QCTestSampleID, openQCResultRow.MinResultDateTime, _
        '                                                                                        openQCResultRow.MaxResultDateTime, pQCControlLotID)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                myNonCumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)
        '                            Else
        '                                'Error getting data for the Control/Lot and Test/SampleType
        '                                Exit For
        '                            End If
        '                        Else
        '                            'Calculate Mean, SD, CV and Ranges for the Control/Lot
        '                            myGlobalDataTO = GetResultsByControlLotForManualModeOLD(dbConnection, openQCResultRow.QCTestSampleID, openQCResultRow.MinResultDateTime, _
        '                                                                                    openQCResultRow.MaxResultDateTime, pQCControlLotID)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                myNonCumulatedResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)
        '                            Else
        '                                'Error getting data for the Control/Lot and Test/SampleType
        '                                Exit For
        '                            End If
        '                        End If

        '                        If (myNonCumulatedResultsDS.tOpenResults.Count > 0) Then
        '                            'Create the row for the Test/Sample Type in the DS to return
        '                            myQCSummaryRow = myQCSummaryDS.QCSummaryByTestSampleTable.NewQCSummaryByTestSampleTableRow
        '                            myQCSummaryRow.QCTestSampleID = openQCResultRow.QCTestSampleID
        '                            myQCSummaryRow.RunsGroupNumber = openQCResultRow.RunsGroupNumber
        '                            myQCSummaryRow.PreloadedTest = openQCResultRow.PreloadedTest

        '                            myQCSummaryRow.Selected = True
        '                            If (openQCResultRow.PreloadedTest) Then
        '                                myQCSummaryRow.TestTypeIcon = imageTest
        '                            Else
        '                                myQCSummaryRow.TestTypeIcon = imageUserTest
        '                            End If

        '                            myQCSummaryRow.TestName = openQCResultRow.TestName
        '                            myQCSummaryRow.SampleType = openQCResultRow.SampleType
        '                            myQCSummaryRow.MeasureUnit = openQCResultRow.MeasureUnit
        '                            myQCSummaryRow.RejectionCriteria = openQCResultRow.RejectionCriteria
        '                            myQCSummaryRow.DecimalsAllowed = openQCResultRow.DecimalsAllowed

        '                            myQCSummaryRow.n = myNonCumulatedResultsDS.tOpenResults(0).n
        '                            myQCSummaryRow.CalcMean = myNonCumulatedResultsDS.tOpenResults(0).CalcMean

        '                            If (myNonCumulatedResultsDS.tOpenResults(0).n > 1) Then
        '                                myQCSummaryRow.CalcSD = myNonCumulatedResultsDS.tOpenResults(0).CalcSD
        '                                myQCSummaryRow.CalcCV = myNonCumulatedResultsDS.tOpenResults(0).CalcCV

        '                                myQCSummaryRow.MinRange = myNonCumulatedResultsDS.tOpenResults(0).CalcMean - (openQCResultRow.RejectionCriteria * myNonCumulatedResultsDS.tOpenResults(0).CalcSD)
        '                                If (myQCSummaryRow.MinRange < 0) Then myQCSummaryRow.MinRange = 0

        '                                myQCSummaryRow.MaxRange = myNonCumulatedResultsDS.tOpenResults(0).CalcMean + (openQCResultRow.RejectionCriteria * myNonCumulatedResultsDS.tOpenResults(0).CalcSD)
        '                            Else
        '                                myQCSummaryRow.SetCalcSDNull()
        '                                myQCSummaryRow.SetCalcCVNull()

        '                                myQCSummaryRow.SetMinRangeNull()
        '                                myQCSummaryRow.SetMaxRangeNull()
        '                            End If

        '                            'Validate if there are Cumulated values for the QCTestSampleID/QCControlLotID
        '                            myGlobalDataTO = myLastCumulateValueDelegate.ReadOLD(dbConnection, openQCResultRow.QCTestSampleID, pQCControlLotID)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                myLastCumulateValueDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

        '                                If (myLastCumulateValueDS.tqcCumulatedResults.Count > 0) Then
        '                                    'Set values of Mean and SD for the last cumulated serie
        '                                    myQCSummaryRow.CumulatedMean = myLastCumulateValueDS.tqcCumulatedResults(0).Mean
        '                                    myQCSummaryRow.CumulatedSD = myLastCumulateValueDS.tqcCumulatedResults(0).SD

        '                                    'If there are Cumulated values for the QCTestSampleID/QCControlLotID... 
        '                                    '** Calculate the Minimum allowed value according the RejectionCriteria currently defined - MIN RANGE
        '                                    myQCSummaryRow.CumulatedMinRange = myLastCumulateValueDS.tqcCumulatedResults(0).Mean - _
        '                                                                       (openQCResultRow.RejectionCriteria * myLastCumulateValueDS.tqcCumulatedResults(0).SD)
        '                                    If (myQCSummaryRow.CumulatedMinRange < 0) Then myQCSummaryRow.CumulatedMinRange = 0

        '                                    '** Calculate the Maximum allowed value according the RejectionCriteria currently defined - MAX RANGE
        '                                    myQCSummaryRow.CumulatedMaxRange = myLastCumulateValueDS.tqcCumulatedResults(0).Mean + _
        '                                                                       (openQCResultRow.RejectionCriteria * myLastCumulateValueDS.tqcCumulatedResults(0).SD)
        '                                End If
        '                            Else
        '                                'Error getting the last cumulated values for the QCTestSampleID/QCControlLotID
        '                                Exit For
        '                            End If

        '                            'Verify if the Test/SampleType has results pending of validation for the selected Control/Lot
        '                            myGlobalDataTO = myQCResultsDAO.CountNotReviewedResultsOLD(dbConnection, pQCControlLotID, openQCResultRow.QCTestSampleID, _
        '                                                                                       openQCResultRow.RunsGroupNumber)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                numByStatus = DirectCast(myGlobalDataTO.SetDatos, Int32)
        '                                If (numByStatus > 0) Then
        '                                    'Validate the QC Results
        '                                    myGlobalDataTO = GetNonCumulateResultForAllControlLotsOLD(dbConnection, openQCResultRow.QCTestSampleID, myNonCumulatedResultsDS, _
        '                                                                                              openQCResultRow.MinResultDateTime, openQCResultRow.MaxResultDateTime)
        '                                    If (myGlobalDataTO.HasError) Then Exit For
        '                                End If
        '                            Else
        '                                'Error verifying if there are QC Results pending to validate
        '                                Exit For
        '                            End If

        '                            'Finally verify if there are alarms for the QCControlLotID/QCTestSampleID/RunsGroupNumber to show the Icon
        '                            myGlobalDataTO = myQCResultsDAO.CountResultsWithAlarmsOLD(dbConnection, pQCControlLotID, openQCResultRow.QCTestSampleID, openQCResultRow.RunsGroupNumber)
        '                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                numByStatus = DirectCast(myGlobalDataTO.SetDatos, Int32)

        '                                myQCSummaryRow.SetAlarmsIconPathNull()
        '                                If (numByStatus > 0) Then myQCSummaryRow.AlarmsIconPath = imageAlarms
        '                            Else
        '                                'Error verifying if there are QC Results with alarms
        '                                Exit For
        '                            End If

        '                            myQCSummaryDS.QCSummaryByTestSampleTable.AddQCSummaryByTestSampleTableRow(myQCSummaryRow)
        '                            myQCSummaryDS.AcceptChanges()
        '                        End If
        '                    Next

        '                    If (Not myGlobalDataTO.HasError) Then
        '                        myGlobalDataTO.SetDatos = myQCSummaryDS
        '                        myGlobalDataTO.HasError = False
        '                    End If

        '                    imageTest = Nothing
        '                    imageUserTest = Nothing
        '                    imageAlarms = Nothing
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetByQCControlLotID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Search in history table of QC Module all Controls/Lots having ClosedLot = FALSE and DeleteControl=FALSE and having for at least 
        '''' a Test/SampleType, enough QC Results pending to accumulate
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of all Controls/Lots in 
        ''''          the history table in QC Module that have QC Results thar can be cumulated </returns>
        '''' <remarks>
        '''' Created by: SA 01/07/2011
        '''' </remarks>
        'Public Function GetControlsToCumulateOLD(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetControlsToCumulateOLD(dbConnection)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetControlsToCumulate", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all not excluded QC Results for the specified Test/SampleType and Control/Lot and calculate all data needed to 
        '''' create a new cumulated serie
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultsCalculationDS with the needed data</returns>
        '''' <remarks>
        '''' Created by:  TR 19/05/2011
        '''' Modified by: SA 05/07/2011 - If more than one row was returned for the Runs Group, it means there are automatic and manual results, 
        ''''                              and in this case, values of both rows have to be summarized
        '''' </remarks>
        'Public Function GetDataToCreateCumulateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                           ByVal pQCControlLotID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetDataToCreateCumulateOLD(dbConnection, pQCTestSampleID, pQCControlLotID)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myQCResultCalculationDS As QCResultsCalculationDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsCalculationDS)

        '                    Dim myFinalQCResultCalculationDS As New QCResultsCalculationDS
        '                    If (myQCResultCalculationDS.tQCResultCalculation.Rows.Count > 0) Then
        '                        'Move the first row to the DS to return
        '                        myFinalQCResultCalculationDS.tQCResultCalculation.ImportRow(myQCResultCalculationDS.tQCResultCalculation.First)

        '                        myFinalQCResultCalculationDS.tQCResultCalculation.First.BeginEdit()
        '                        If (myQCResultCalculationDS.tQCResultCalculation.Rows.Count > 1) Then
        '                            'If there are two rows it means there are automatic and manual results in the RunsGroup, values have to be summarized
        '                            myFinalQCResultCalculationDS.tQCResultCalculation.First.N += myQCResultCalculationDS.tQCResultCalculation.Last.N
        '                            myFinalQCResultCalculationDS.tQCResultCalculation.First.SumXi += myQCResultCalculationDS.tQCResultCalculation.Last.SumXi
        '                            myFinalQCResultCalculationDS.tQCResultCalculation.First.SumXi2 += myQCResultCalculationDS.tQCResultCalculation.Last.SumXi2
        '                            myFinalQCResultCalculationDS.tQCResultCalculation.First.Sum2Xi += myQCResultCalculationDS.tQCResultCalculation.Last.Sum2Xi

        '                            'Summarize also the range of Dates of the Results
        '                            If (myQCResultCalculationDS.tQCResultCalculation.Last.FirstRunDateTime < myQCResultCalculationDS.tQCResultCalculation.First.FirstRunDateTime) Then
        '                                myFinalQCResultCalculationDS.tQCResultCalculation.First.FirstRunDateTime = myQCResultCalculationDS.tQCResultCalculation.Last.FirstRunDateTime
        '                            End If
        '                            If (myQCResultCalculationDS.tQCResultCalculation.Last.LastRunDateTime > myQCResultCalculationDS.tQCResultCalculation.First.LastRunDateTime) Then
        '                                myFinalQCResultCalculationDS.tQCResultCalculation.First.LastRunDateTime = myQCResultCalculationDS.tQCResultCalculation.Last.LastRunDateTime
        '                            End If
        '                        End If

        '                        myFinalQCResultCalculationDS.tQCResultCalculation.First.Mean = myFinalQCResultCalculationDS.tQCResultCalculation.First.SumXi / _
        '                                                                                       myFinalQCResultCalculationDS.tQCResultCalculation.First.N
        '                        myFinalQCResultCalculationDS.tQCResultCalculation.First.EndEdit()
        '                    End If

        '                    myGlobalDataTO.SetDatos = myFinalQCResultCalculationDS
        '                    myGlobalDataTO.HasError = False
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetDataToCreateCumulate", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the more recent date of a non cumulated QC Result for the specified QCTestSampleID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <returns>GlobalDataTO containing a datetime value with the maximum date</returns>
        '''' <remarks>
        '''' Created by:  SA 13/07/2011
        '''' </remarks>
        'Public Function GetMaxResultDateTimeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetMaxResultDateTimeOLD(dbConnection, pQCTestSampleID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMaxResultDateTime", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the Max RunNumber for the QCTestSampleID, QCControlID and RunsGroupNumber
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNum">Number of the currently opened Runs Group in which the Results are included</param>
        '''' <returns>GlobalDataTO containing an integer value with the last created RunNumber for the
        ''''          specified QCTestSampleID, QCControlID and RunsGroupNumber</returns>
        '''' <remarks>
        '''' Created by: TR 17/05/2011
        '''' </remarks>
        'Public Function GetMaxRunNumberOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                   ByVal pRunsGroupNum As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetMaxRunNumberOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pRunsGroupNum)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMaxRunNumber", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get the oldest date of a non cumulated QC Result for the specified QCTestSampleID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <returns>GlobalDataTO containing a datetime value with the minimum date</returns>
        '''' <remarks>
        '''' Created by:  TR 27/05/2011
        '''' </remarks>
        'Public Function GetMinResultDateTimeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetMinResultDateTimeOLD(dbConnection, pQCTestSampleID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetMinResultDateTime", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get and validate all non cumulated QC Results for the specified QCTestSampleID and each one of the QCControlLotID 
        '''' contained in the typed DataSet OpenQCResultsDS
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pOpenQCResultsDS">Typed DataSet OpenQCResultsDS containing all Controls/Lots for which the informed Test/Sample
        ''''                                have non cumulated QC Results in the specified range of dates </param>
        '''' <param name="pDateFrom">Date From to filter results</param>
        '''' <param name="pDateTo">Date To to filter results</param>
        '''' <returns>GlobalDataTO containing a typed DataSet</returns>
        '''' <remarks>
        '''' Created by:  TR 31/05/2011
        '''' Modified by: SA 06/06/2011 - Change the template: function has to be inside a DB Transaction. Added updation of fields 
        ''''                              ValidationStatus. When ManualResultFlag is True the validation of Result inside Min/Max
        ''''                              range was wrong.
        ''''              TR 08/06/2011 - Validate the result value to load on VisibleResultValue depending if value of field ManualResultFlag is
        ''''                              True (then VisibleResultValue = ManualResultValue) or False (then VisibleResultValue = ResultValue)
        ''''              SA 06/07/2011 - Validation of result inside Range and calculation of Absolute and Relative Errors should be done only for not
        ''''                              excluded Results
        ''''              SA 17/10/2011 - If the DB Transaction was opened locally, execute the Rollback and close the opened DB Connection before 
        ''''                              executing the two Exit Try included in the code   
        ''''              SA 29/11/2011 - Calculate also the Relative Error expressed as %: (Absolute Error / Mean) * 100 
        ''''              SA 25/01/2012 - When the result is out of range of valid values, set alarm QC_OUT_OF_RANGE instead of CONC_REMARK7.
        ''''                              Apply Multirules also when there is only a Control/Lot with non cumulated QC Results
        '''' </remarks>
        'Public Function GetNonCumulateResultForAllControlLotsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                                         ByVal pOpenQCResultsDS As OpenQCResultsDS, ByVal pDateFrom As DateTime, _
        '                                                         ByVal pDateTo As DateTime) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCControlIDList As String = ""
        '                Dim myQCResultsDS As New QCResultsDS

        '                Dim myResultsControl1DS As New QCResultsDS
        '                Dim myResultsControl2DS As New QCResultsDS
        '                Dim myQCResultAlarmsDS As New QCResultAlarms

        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                Dim myQCResultsList As New List(Of QCResultsDS.tqcResultsRow)
        '                Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate

        '                'Build the list of QCControlLotIDs separated by commas
        '                myGlobalDataTO = GetQCControlList(pOpenQCResultsDS)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    myQCControlIDList = CStr(myGlobalDataTO.SetDatos)
        '                Else
        '                    'If there are not Controls/Lots with results for the Test/SampleType, nothing to do...
        '                    If (pDBConnection Is Nothing) Then
        '                        DAOBase.RollbackTransaction(dbConnection)
        '                        If (Not dbConnection Is Nothing) Then dbConnection.Close()
        '                    End If
        '                    Exit Try
        '                End If

        '                'Search all non Cumulated Results the Test/SampleType has for the list of QCControlLotIDs in the specified range of dates
        '                myGlobalDataTO = myQCResultsDAO.GetNonCumulateResultsOLD(dbConnection, pQCTestSampleID, myQCControlIDList, pDateFrom, pDateTo)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    myQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)
        '                Else
        '                    'If the Test/SampleType does not have non Cumulated Results for any of the Controls/Lots in the list, nothing to do...
        '                    If (pDBConnection Is Nothing) Then
        '                        DAOBase.RollbackTransaction(dbConnection)
        '                        If (Not dbConnection Is Nothing) Then dbConnection.Close()
        '                    End If
        '                    Exit Try
        '                End If

        '                'For each Control/Lot with non Cumulated Results for the Test/Sample Type
        '                For Each openQCResultsROW As OpenQCResultsDS.tOpenResultsRow In pOpenQCResultsDS.tOpenResults.Rows
        '                    'Delete all Alarms saved for all the Results in the RunsGroup
        '                    myGlobalDataTO = myQCResultAlarmsDelegate.DeleteOLD(dbConnection, pQCTestSampleID, openQCResultsROW.QCControlLotID, _
        '                                                                        openQCResultsROW.RunsGroupNumber)
        '                    If (myGlobalDataTO.HasError) Then Exit For

        '                    'Update ValidationStatus = OK for all Results included in the RunsGroup that are inside of the informed range of dates
        '                    myGlobalDataTO = myQCResultsDAO.UpdateValStatusByDateRangeOLD(dbConnection, openQCResultsROW.QCControlLotID, pQCTestSampleID, _
        '                                                                                  openQCResultsROW.RunsGroupNumber, True, pDateFrom, pDateTo)
        '                    If (myGlobalDataTO.HasError) Then Exit For

        '                    'Update ValidationStatus = PENDING for all Results included in the RunsGroup that are outside of the informed range of dates
        '                    myGlobalDataTO = myQCResultsDAO.UpdateValStatusByDateRangeOLD(dbConnection, openQCResultsROW.QCControlLotID, pQCTestSampleID, _
        '                                                                                  openQCResultsROW.RunsGroupNumber, False, pDateFrom, pDateTo)
        '                    If (myGlobalDataTO.HasError) Then Exit For

        '                    'Get from DataSet myQCResultsDS all Results corresponding to the QCControlLotID being processed
        '                    myQCResultsList = (From a In myQCResultsDS.tqcResults _
        '                                      Where a.QCTestSampleID = pQCTestSampleID _
        '                                        And a.QCControlLotID = openQCResultsROW.QCControlLotID _
        '                                     Select a).ToList()

        '                    If (myQCResultsList.Count > 0) Then
        '                        Dim myRunsNumber As Integer = 1

        '                        For Each qcResultRow As QCResultsDS.tqcResultsRow In myQCResultsList
        '                            'Calculate the RunNumber to shown in screen
        '                            qcResultRow.CalcRunNumber = myRunsNumber
        '                            myRunsNumber += 1

        '                            'If there is an User Comment for the Result, or if it is a Result entered manually, 
        '                            'get the path for the Icon of modified Results
        '                            If (Not qcResultRow.IsResultCommentNull AndAlso Not qcResultRow.ResultComment = "") OrElse qcResultRow.ManualResultFlag Then
        '                                qcResultRow.IconPath = My.Application.Info.DirectoryPath & GlobalBase.ImagesPath & GetIconName("CHG_PWD")
        '                            End If

        '                            If (qcResultRow.ManualResultFlag) Then
        '                                'If manual result flag is checked, then the value shown is the manual result value
        '                                qcResultRow.VisibleResultValue = qcResultRow.ManualResultValue
        '                            Else
        '                                qcResultRow.VisibleResultValue = qcResultRow.ResultValue
        '                            End If

        '                            'If the Result is not Excluded, verify if it is inside range and calculate ABS and REL Error
        '                            If (Not qcResultRow.Excluded) Then
        '                                'If ranges (Min/Max) are informed, validate if the Result is inside the range of allowed values
        '                                If (Not openQCResultsROW.IsMinRangeNull AndAlso Not openQCResultsROW.IsMaxRangeNull) Then
        '                                    If (qcResultRow.VisibleResultValue < openQCResultsROW.MinRange OrElse _
        '                                        qcResultRow.VisibleResultValue > openQCResultsROW.MaxRange) Then
        '                                        'Insert Alarm QC_OUT_OF_RANGE in tqcResult Alarms for the result.
        '                                        InsertNewQcResultAlarmOLD(myQCResultAlarmsDS, qcResultRow, GlobalEnumerates.CalculationRemarks.QC_OUT_OF_RANGE.ToString)
        '                                    End If
        '                                End If

        '                                'If Mean and SD have a value, then calculate Absolute and Relative Errors
        '                                If (Not openQCResultsROW.IsMeanNull AndAlso Not openQCResultsROW.IsSDNull) Then
        '                                    'Calculate Absolute Error...
        '                                    qcResultRow.ABSError = qcResultRow.VisibleResultValue - openQCResultsROW.Mean

        '                                    'Calculate Relative Errors (SDI and %)
        '                                    qcResultRow.RELError = 0
        '                                    If (openQCResultsROW.SD > 0) Then qcResultRow.RELError = (qcResultRow.ABSError / openQCResultsROW.SD)

        '                                    qcResultRow.RELErrorPercent = 0
        '                                    If (openQCResultsROW.Mean > 0) Then qcResultRow.RELErrorPercent = (qcResultRow.ABSError / openQCResultsROW.Mean) * 100

        '                                    'If the Control/Lot has been included in the pair of Controls to apply Westgard Rules,
        '                                    'add the Result to the proper DS (depending if it is marked as first or second Control)
        '                                    If (openQCResultsROW.WestgardControlNum = 1) Then
        '                                        qcResultRow.ValidationStatus = "OK"
        '                                        myResultsControl1DS.tqcResults.ImportRow(qcResultRow)
        '                                    ElseIf (openQCResultsROW.WestgardControlNum = 2) Then
        '                                        qcResultRow.ValidationStatus = "OK"
        '                                        myResultsControl2DS.tqcResults.ImportRow(qcResultRow)
        '                                    End If
        '                                End If
        '                            End If
        '                        Next
        '                    End If
        '                Next

        '                'Apply Westgard Rules selected for the Test/SampleType if there are Results for at least one of its linked Control/Lots
        '                If (myResultsControl1DS.tqcResults.Count > 0) Then
        '                    myGlobalDataTO = ApplyMultiRulesOLD(dbConnection, pQCTestSampleID, myResultsControl1DS, myResultsControl2DS, myQCResultAlarmsDS)
        '                End If

        '                'Get the DS containing the list of alarms for the different Results. For each Alarm, insert it in tqcResultAlarm
        '                If (Not myGlobalDataTO.HasError) Then
        '                    myGlobalDataTO = myQCResultAlarmsDelegate.CreateOLD(dbConnection, myQCResultAlarmsDS)

        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'Get description of all Alarms in the current Language
        '                        Dim currentSession As New GlobalBase
        '                        myGlobalDataTO = GetAllAlarmsDescriptionsOLD(dbConnection, pQCTestSampleID, pOpenQCResultsDS, currentSession.GetSessionInfo().ApplicationLanguage)

        '                        If (Not myGlobalDataTO.HasError) Then
        '                            myQCResultAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

        '                            'For each returned record, search the result in the DS of Results and update the field AlarmsList
        '                            Dim myQCResultList As New List(Of QCResultsDS.tqcResultsRow)
        '                            For Each receivedAlarmRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsDS.tqcResultAlarms.Rows
        '                                myQCResultList = (From a In myQCResultsDS.tqcResults _
        '                                                 Where a.QCTestSampleID = receivedAlarmRow.QCTestSampleID _
        '                                                   And a.QCControlLotID = receivedAlarmRow.QCControlLotID _
        '                                                   And a.RunNumber = receivedAlarmRow.RunNumber _
        '                                                Select a).ToList()

        '                                For Each resulToUpdate As QCResultsDS.tqcResultsRow In myQCResultList
        '                                    resulToUpdate.AlarmsList = receivedAlarmRow.AlarmDesc

        '                                    resulToUpdate.ValidationStatus = "ERROR"
        '                                    If (receivedAlarmRow.AlarmIDList.Trim = "WESTGARD_1-2s") Then
        '                                        'If there is only an Alarm for the Result and it is the Multirule 1-2s, then ValidationStatus = WARNING 
        '                                        resulToUpdate.ValidationStatus = "WARNING"
        '                                    End If

        '                                    'Update the ValidationStatus of the specific Result 
        '                                    myGlobalDataTO = myQCResultsDAO.UpdateValStatusByResultOLD(dbConnection, resulToUpdate.QCControlLotID, resulToUpdate.QCTestSampleID, _
        '                                                                                               resulToUpdate.RunsGroupNumber, resulToUpdate.ValidationStatus, _
        '                                                                                               resulToUpdate.RunNumber)
        '                                    If (myGlobalDataTO.HasError) Then Exit For
        '                                Next
        '                                If (myGlobalDataTO.HasError) Then Exit For
        '                            Next
        '                        End If
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Return the list of Results...
        '                    myGlobalDataTO.SetDatos = myQCResultsDS

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetNonCumulateResultForAllControlLots", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified QCTestSampleID/QCControlLotID/RunsGroupNumber, get all QC Results to export to an
        '''' XML File (when a group of series are cumulated)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pGroupNumber">Identifier of the Group Number in QC Module</param> 
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultsDS containing all QC Results to export to an
        ''''          XML File (when a group of series are cumulated)</returns>
        '''' <remarks>
        '''' Created by:  SA 27/06/2011
        '''' Modified by: SA 28/06/2011 - Get also the Alarms for each QC Result
        '''' </remarks>
        'Public Function GetQCResultsToExportOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                     ByVal pGroupNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetQCResultsToExportOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pGroupNumber)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myQCResultsDS As QCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

        '                    If (myQCResultsDS.tqcResults.Count > 0) Then
        '                        'Get all Alarms linked to QC Results in the Runs Group
        '                        Dim myQCResultsAlarmsDelegate As New QCResultAlarmsDelegate
        '                        myGlobalDataTO = myQCResultsAlarmsDelegate.GetAlarmsOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pGroupNumber)

        '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            Dim myQCResultAlarmsDS As QCResultAlarms = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

        '                            Dim lstQCResult As List(Of QCResultsDS.tqcResultsRow)
        '                            For Each resultAlarmRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsDS.tqcResultAlarms.Rows
        '                                lstQCResult = (From a As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
        '                                              Where a.QCTestSampleID = pQCTestSampleID _
        '                                            AndAlso a.QCControlLotID = pQCControlLotID _
        '                                            AndAlso a.RunsGroupNumber = pGroupNumber _
        '                                            AndAlso a.RunNumber = resultAlarmRow.RunNumber _
        '                                             Select a).ToList
        '                                If (lstQCResult.Count = 1) Then
        '                                    lstQCResult.First.BeginEdit()
        '                                    lstQCResult.First.AlarmsList = resultAlarmRow.AlarmDesc
        '                                    lstQCResult.First.EndEdit()
        '                                End If
        '                            Next
        '                            myQCResultsDS.AcceptChanges()
        '                        End If
        '                    End If

        '                    myGlobalDataTO.SetDatos = myQCResultsDS
        '                    myGlobalDataTO.HasError = False
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetQCResultsToExport", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified Test/SampleType, get all non cumulated QC Results in the informed range of dates for 
        '''' each one of the linked Control/Lots. Used for Test/Sample Type with CalculationMode = MANUAL
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pDateFrom">Date From to filter results</param>
        '''' <param name="pDateTo">Date To to filter results</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a Typed DataSet OpenQCResultsDS with the information for all Controls/Lots
        ''''          linked to the Test/SampleType and having non cumulated QC Results in the specified range of dates</returns>
        '''' <remarks>
        '''' Created by:  TR 31/05/2011
        '''' Modified by: SA 09/06/2011 - Open a DB Connection instead of a DB Transaction
        ''''              TR 17/06/2011 - Added searching of Mean and SD from the last cumulated values saved for the Test/SampleType and 
        ''''                              each Control/Lot
        ''''              SA 05/05/2011 - Before process data of the returned Controls to calculate values of Mean,SD, CV and Range, 
        ''''                              group data of all rows returned for each Control in an unique row
        ''''              SA 13/07/2011 - Removed searching of last cumulated values and getting the Range of valid values according them
        ''''              SA 01/12/2011 - Changed function name from GetByQCTestSampleIDResultDateTime to GetResultsByControlLotForManualMode, 
        ''''                              due to this function is used only when CalculationMode = MANUAL
        ''''                              Added optional parameter to get data only for the specified Control/Lot
        ''''              SA 02/12/2011 - Get Mean, SD and CV of returned QC Results
        ''''              SA 19/01/2012 - After searching if Controls with not Excluded open QC Results, verify also if there are Controls with
        ''''                              all their open QC Results marked as Excluded
        '''' </remarks>
        'Public Function GetResultsByControlLotForManualModeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                                    ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime, _
        '                                                    Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                Dim myOpenQCResultsDS As New OpenQCResultsDS

        '                'Select All Controls With not excluded Open Results for the Test/Sample between dates
        '                myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotOLD(dbConnection, pQCTestSampleID, pDateFrom, pDateTo, False, pQCControlLotID, False)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

        '                    Dim myFinalOpenResultsDS As New OpenQCResultsDS
        '                    If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
        '                        'Move data to a different DS, grouping data of all rows returned for each Control in an unique row (a maximum of two rows can be returned
        '                        'for a Control: one with values for non manual Results and another with values for manual Results)
        '                        Dim myQCControlID As Integer = 0

        '                        For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
        '                            If (openQCResultRow.QCControlLotID <> myQCControlID) Then
        '                                myQCControlID = openQCResultRow.QCControlLotID
        '                                myFinalOpenResultsDS.tOpenResults.ImportRow(openQCResultRow)
        '                            Else
        '                                'Add values to the previous ones if it is the same Control/Lot
        '                                myFinalOpenResultsDS.tOpenResults.Last.BeginEdit()
        '                                myFinalOpenResultsDS.tOpenResults.Last.n += openQCResultRow.n
        '                                myFinalOpenResultsDS.tOpenResults.Last.Sumxi += openQCResultRow.Sumxi
        '                                myFinalOpenResultsDS.tOpenResults.Last.Sumxi2 += openQCResultRow.Sumxi2
        '                                myFinalOpenResultsDS.tOpenResults.Last.EndEdit()
        '                            End If
        '                        Next

        '                        Dim numberOfControl As Integer = 1
        '                        For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myFinalOpenResultsDS.tOpenResults.Rows
        '                            openQCResultRow.Selected = False

        '                            'Get Mean and SD from Theorical Concentration Values (Min/Max); CV is set to NULL
        '                            'Minimum and Maximum allowed values are defined for the theorical Min/Max Concentration values (MinRange/MaxRange)
        '                            openQCResultRow.Mean = (openQCResultRow.MinRange + openQCResultRow.MaxRange) / 2
        '                            openQCResultRow.SD = (openQCResultRow.MaxRange - openQCResultRow.MinRange) / (2 * openQCResultRow.RejectionCriteria)
        '                            openQCResultRow.SetCVNull()

        '                            'Get Mean, SD and CV of all returned results 
        '                            openQCResultRow.CalcMean = openQCResultRow.Sumxi / openQCResultRow.n
        '                            If (openQCResultRow.n > 1) Then
        '                                openQCResultRow.CalcSD = Math.Sqrt(((openQCResultRow.n * openQCResultRow.Sumxi2) - (openQCResultRow.Sumxi * openQCResultRow.Sumxi)) / _
        '                                                                   (openQCResultRow.n * (openQCResultRow.n - 1)))
        '                                openQCResultRow.CalcCV = (openQCResultRow.CalcSD / openQCResultRow.CalcMean) * 100
        '                            Else
        '                                'Values cannot be calculated
        '                                openQCResultRow.SetCalcSDNull()
        '                                openQCResultRow.SetCalcCVNull()
        '                            End If
        '                        Next
        '                    End If

        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'Special Case: search if there are Controls with open QC Results in the interval of dates but all of them are excluded
        '                        myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotOLD(dbConnection, pQCTestSampleID, pDateFrom, pDateTo, False, pQCControlLotID, True)
        '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

        '                            If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
        '                                'Move data to a different DS, ignoring the second row of a duplicated Control (a maximum of two rows can be returned
        '                                'for a Control: one with values for non manual Results and another with values for manual Results). If the Control have
        '                                'been already included in the DS to return, it is also ignored
        '                                Dim myQCControlID As Integer = 0

        '                                For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
        '                                    If (openQCResultRow.QCControlLotID <> myQCControlID) Then
        '                                        'The Control is added to the final DS only if it was not already included in it
        '                                        myQCControlID = openQCResultRow.QCControlLotID

        '                                        If (myFinalOpenResultsDS.tOpenResults.Where(Function(a) a.QCControlLotID = myQCControlID).Count() = 0) Then
        '                                            'Get Mean and SD from Theorical Concentration Values (Min/Max); CV is set to NULL
        '                                            'Minimum and Maximum allowed values are defined for the theorical Min/Max Concentration values (MinRange/MaxRange)
        '                                            openQCResultRow.Mean = (openQCResultRow.MinRange + openQCResultRow.MaxRange) / 2
        '                                            openQCResultRow.SD = (openQCResultRow.MaxRange - openQCResultRow.MinRange) / (2 * openQCResultRow.RejectionCriteria)
        '                                            openQCResultRow.SetCVNull()

        '                                            'Set n=0 and also set to NULL fields Mean, SD and CV of all returned results 
        '                                            openQCResultRow.n = 0
        '                                            openQCResultRow.SetCalcMeanNull()
        '                                            openQCResultRow.SetCalcSDNull()
        '                                            openQCResultRow.SetCalcCVNull()

        '                                            'Finally, move the row to the final DS
        '                                            myFinalOpenResultsDS.tOpenResults.ImportRow(openQCResultRow)
        '                                        End If
        '                                    End If
        '                                Next
        '                            End If
        '                        End If
        '                    End If

        '                    'Return the DS with all the information
        '                    If (Not myGlobalDataTO.HasError) Then
        '                        myGlobalDataTO.SetDatos = myFinalOpenResultsDS
        '                        myGlobalDataTO.HasError = False
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetResultsByControlLotForManualMode", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the specified Test/SampleType, get all non cumulated QC Results for each one of the linked Control/Lots (or for 
        '''' the specified one, when the optional parameter for the Control/Lot is informed) and calculate:
        ''''   ** Statistical values of Mean, SD, CV and Range using the first n (pMinNumSeries) results, without filtering them 
        ''''      by the informed range of dates
        ''''   ** Mean, SD and CV of the results included in the informed range of dates and not included in the statistics ones
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pDateFrom">Date From to filter results</param>
        '''' <param name="pDateTo">Date To to filter results</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a Typed DataSet OpenQCResultsDS with the information for all Controls/Lots
        ''''          linked to the Test/SampleType and having non cumulated QC Results</returns>
        '''' <remarks>
        '''' Created by:  SA 30/11/2011
        '''' Modified by: SA 14/12/2011 - Open DBTransaction instead of DBConnection due to values in table of Last Statistic Values
        ''''                              are updated when they can be calculated
        '''' </remarks>
        'Public Function GetResultsByControlLotForStatisticsModeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                                           ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime, _
        '                                                           Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                Dim myFinalOpenResultsDS As New OpenQCResultsDS

        '                'Get values needed for calculation of Statistic
        '                myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotForStatisticsOLD(dbConnection, pQCTestSampleID, pQCControlLotID)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myStatisticsResultsDS As QCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

        '                    Dim i As Integer = 0
        '                    Dim mySumXi As Double
        '                    Dim mySumXi2 As Double

        '                    Dim myQCControlLotID As Integer = 0
        '                    Dim myControlRow As OpenQCResultsDS.tOpenResultsRow

        '                    For Each qcControlResult As QCResultsDS.tqcResultsRow In myStatisticsResultsDS.tqcResults.Rows
        '                        If (qcControlResult.QCControlLotID <> myQCControlLotID) Then
        '                            'Initialization of all variables used to count records and sumarize values
        '                            i = 0
        '                            mySumXi = 0
        '                            mySumXi2 = 0

        '                            'A new linked Control/Lot will be processed
        '                            myQCControlLotID = qcControlResult.QCControlLotID

        '                            'Add the row for the Control/Lot in myFinalOpenResultsDS
        '                            myControlRow = myFinalOpenResultsDS.tOpenResults.NewtOpenResultsRow

        '                            myControlRow.Selected = False
        '                            myControlRow.WestgardControlNum = qcControlResult.WestgardControlNum
        '                            myControlRow.QCControlLotID = qcControlResult.QCControlLotID
        '                            myControlRow.ControlName = qcControlResult.ControlName
        '                            myControlRow.LotNumber = qcControlResult.LotNumber
        '                            myControlRow.ControlNameLotNum = qcControlResult.ControlNameLotNum
        '                            myControlRow.RunsGroupNumber = qcControlResult.RunsGroupNumber
        '                            myControlRow.MeasureUnit = qcControlResult.MeasureUnit
        '                            myControlRow.SetMeanNull()
        '                            myControlRow.SetSDNull()
        '                            myControlRow.SetCVNull()
        '                            myControlRow.SetMinRangeNull()
        '                            myControlRow.SetMaxRangeNull()
        '                            myControlRow.n = 0
        '                            myControlRow.Sumxi = 0
        '                            myControlRow.Sumxi2 = 0
        '                            myControlRow.SetCalcMeanNull()
        '                            myControlRow.SetCalcSDNull()
        '                            myControlRow.SetCalcCVNull()

        '                            myFinalOpenResultsDS.tOpenResults.AddtOpenResultsRow(myControlRow)
        '                        End If

        '                        i += 1
        '                        mySumXi += qcControlResult.ResultValue
        '                        mySumXi2 += (qcControlResult.ResultValue * qcControlResult.ResultValue)

        '                        myControlRow.BeginEdit()
        '                        If (i = qcControlResult.NumberOfSeries) Then
        '                            'Calculate the statistical values of Mean, SD, CV and Ranges
        '                            myControlRow.Mean = mySumXi / i
        '                            myControlRow.SD = Math.Sqrt(((i * mySumXi2) - (mySumXi * mySumXi)) / (i * (i - 1)))
        '                            myControlRow.CV = (myControlRow.SD / myControlRow.Mean) * 100

        '                            myControlRow.MinRange = myControlRow.Mean - (qcControlResult.RejectionCriteria * myControlRow.SD)
        '                            If (myControlRow.MinRange < 0) Then myControlRow.MinRange = 0

        '                            myControlRow.MaxRange = myControlRow.Mean + (qcControlResult.RejectionCriteria * myControlRow.SD)
        '                            If (myControlRow.MaxRange < 0) Then myControlRow.MaxRange = 0
        '                        End If
        '                        myControlRow.EndEdit()
        '                    Next

        '                    Dim myOpenQCResultsDS As New OpenQCResultsDS
        '                    For Each qcControlResult As OpenQCResultsDS.tOpenResultsRow In myFinalOpenResultsDS.tOpenResults.Rows
        '                        'Select all Open Results for the Test/SampleType and Control/Lot between dates
        '                        myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotOLD(dbConnection, pQCTestSampleID, pDateFrom, pDateTo, True, qcControlResult.QCControlLotID, False)
        '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

        '                            If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
        '                                qcControlResult.BeginEdit()
        '                                For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
        '                                    qcControlResult.n += openQCResultRow.n
        '                                    qcControlResult.Sumxi += openQCResultRow.Sumxi
        '                                    qcControlResult.Sumxi2 += openQCResultRow.Sumxi2
        '                                Next

        '                                qcControlResult.CalcMean = qcControlResult.Sumxi / qcControlResult.n
        '                                If (qcControlResult.n > 1) Then
        '                                    qcControlResult.CalcSD = Math.Sqrt(((qcControlResult.n * qcControlResult.Sumxi2) - (qcControlResult.Sumxi * qcControlResult.Sumxi)) / _
        '                                                                       (qcControlResult.n * (qcControlResult.n - 1)))
        '                                    qcControlResult.CalcCV = (qcControlResult.CalcSD / qcControlResult.CalcMean) * 100
        '                                End If
        '                                qcControlResult.EndEdit()
        '                            End If
        '                        Else
        '                            'Error getting the not excluded and not included in Mean open QC Results 
        '                            Exit For
        '                        End If
        '                    Next

        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'Special Case: search if there are Controls with open QC Results in the interval of dates but all of them are excluded
        '                        myGlobalDataTO = myQCResultsDAO.GetResultsByControlLotOLD(dbConnection, pQCTestSampleID, pDateFrom, pDateTo, True, -1, True)
        '                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            myOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

        '                            If (myOpenQCResultsDS.tOpenResults.Rows.Count > 0) Then
        '                                'Move data to a different DS, ignoring the second row of a duplicated Control (a maximum of two rows can be returned
        '                                'for a Control: one with values for non manual Results and another with values for manual Results). If the Control have
        '                                'been already included in the DS to return, it is also ignored
        '                                Dim myQCControlID As Integer = 0

        '                                For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In myOpenQCResultsDS.tOpenResults.Rows
        '                                    If (openQCResultRow.QCControlLotID <> myQCControlID) Then
        '                                        'The Control is added to the final DS only if it was not already included in it
        '                                        myQCControlID = openQCResultRow.QCControlLotID

        '                                        If (myFinalOpenResultsDS.tOpenResults.Where(Function(a) a.QCControlLotID = myQCControlID).Count() = 0) Then
        '                                            'Set to NULL fields MinRange, MaxRange, Mean, SD, and CV
        '                                            openQCResultRow.SetMinRangeNull()
        '                                            openQCResultRow.SetMaxRangeNull()
        '                                            openQCResultRow.SetMeanNull()
        '                                            openQCResultRow.SetSDNull()
        '                                            openQCResultRow.SetCVNull()

        '                                            'Set n=0 and also set to NULL fields Mean, SD and CV of all returned results 
        '                                            openQCResultRow.n = 0
        '                                            openQCResultRow.SetCalcMeanNull()
        '                                            openQCResultRow.SetCalcSDNull()
        '                                            openQCResultRow.SetCalcCVNull()

        '                                            'Finally, move the row to the final DS
        '                                            myFinalOpenResultsDS.tOpenResults.ImportRow(openQCResultRow)
        '                                        End If
        '                                    End If
        '                                Next
        '                            End If
        '                        End If
        '                    End If
        '                End If

        '                'Return the DS with all the information
        '                If (Not myGlobalDataTO.HasError) Then
        '                    myGlobalDataTO.SetDatos = myFinalOpenResultsDS
        '                    myGlobalDataTO.HasError = False
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetResultsByControlLotForStatisticsMode", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        '''' informed Test/Sample Type, Control/Lot and Runs Group Number
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pNewRunsGroupNumber">Number of the open Runs Group  for the Test/SampleType and Control/Lot to which 
        ''''                                   the QC Results have to be assigned</param>
        '''' <returns>GlobalDataTO containing success / error information</returns>
        '''' <remarks>
        '''' Created by: SA 15/12/2011
        '''' </remarks>
        'Public Function MoveStatisticResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                        ByVal pQCControlLotID As Integer, ByVal pNewRunsGroupNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Delete all Alarms for the group of Results that will be moved to a new Runs Group
        '                Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate
        '                myGlobalDataTO = myQCResultAlarmsDelegate.DeleteOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pNewRunsGroupNumber - 1)

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Move the Result to the new group
        '                    Dim myQCResultsDAO As New tqcResultsDAO
        '                    myGlobalDataTO = myQCResultsDAO.MoveStatisticResultsOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pNewRunsGroupNumber)
        '                End If

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.MoveStatisticResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Verify if there are QC Results pending to cumulate for the specified Control
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlID">Control Identifier</param>
        '''' <param name="pTestID">Test Identifier. Optional parameter</param>
        '''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Tests/SampleTypes
        ''''          for which the informed Control has QC Results pending to accumulate</returns>
        '''' <remarks>
        '''' Created by:  TR 17/05/2011
        '''' Modified by: SA 24/05/2011 - Added optional parameters to filter results also by TestID and SampleType
        '''' </remarks>
        'Public Function SearchPendingResultsByControlOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
        '                                                 Optional ByVal pTestID As Integer = 0, Optional ByVal pSampleType As String = "") As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.SearchPendingResultsByControlOLD(dbConnection, pControlID, pTestID, pSampleType)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.SearchPendingResultsByControl", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Verify if there are QC Results pending to cumulate for the specified TestID and Sample Type
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryQCInformationDS containing the information of the different Controls/Lots
        ''''          for which the informed Test/SampleType has QC Results pending to accumulate</returns>
        '''' <remarks>
        '''' Created by:  TR 24/05/2011
        '''' </remarks>
        'Public Function SearchPendingResultsByTestIDSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) _
        '                                                          As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.SearchPendingResultsByTestIDSampleTypeNEW(dbConnection, pTestID, pSampleType)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.SearchPendingResultsByTestIDSampleType", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all open QC Results for all Control/Lots linked to Test/SampleTypes with Calculation Mode defined as STATISTICS
        '''' (the first NumberOfSeries results will be used to calculate the statistic values)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed Dataset QCResultsDS with the list of all QC Results that will
        ''''          be used to calculate statistics for all Control/Lots linked to Test/SampleTypes with 
        ''''          CalculationMode = STATISTIC</returns>
        '''' <remarks>
        '''' Created by: SA 15/12/2011
        '''' </remarks>
        'Public Function SetResultsForStatisticsOLD(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pQCTestSampleID As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetResultsForStatisticsOLD(dbConnection, pQCTestSampleID)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myQCResultsDS As QCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)

        '                    'Get the list of different QCTestSampleIDs
        '                    Dim lstTestSamplesList As List(Of Integer) = (From a As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
        '                                                                 Select a.QCTestSampleID Distinct).ToList()

        '                    Dim i As Integer
        '                    Dim numOfSeries As Integer
        '                    Dim lstCtrlLotList As List(Of Integer)
        '                    Dim lstQCResultsList As List(Of QCResultsDS.tqcResultsRow)

        '                    Dim myQCResultsToUpdateDS As New QCResultsDS

        '                    For Each qcTestSampleID As Integer In lstTestSamplesList
        '                        'Set IncludedInMean = FALSE for all open QC Results for the Test/SampleType
        '                        myGlobalDataTO = myQCResultsDAO.UnmarkStatisticResults(dbConnection, qcTestSampleID)

        '                        'Get the Number of Series defined for the Test/SampleType
        '                        numOfSeries = (From b As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
        '                                      Where b.QCTestSampleID = qcTestSampleID _
        '                                     Select b.NumberOfSeries).First()

        '                        'Get the list of different Control/Lots linked to the Test/SampleType
        '                        lstCtrlLotList = (From c As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
        '                                         Where c.QCTestSampleID = qcTestSampleID _
        '                                        Select c.QCControlLotID Distinct).ToList()

        '                        For Each qcControlLotID As Integer In lstCtrlLotList
        '                            'Get the list of QC Results for the Test/SampleType and Control/Lot
        '                            lstQCResultsList = (From d As QCResultsDS.tqcResultsRow In myQCResultsDS.tqcResults _
        '                                               Where d.QCTestSampleID = qcTestSampleID _
        '                                             AndAlso d.QCControlLotID = qcControlLotID _
        '                                              Select d Order By d.ResultDateTime).ToList()

        '                            i = 0
        '                            For Each qcResult As QCResultsDS.tqcResultsRow In lstQCResultsList
        '                                If (i < numOfSeries) Then
        '                                    myQCResultsToUpdateDS.tqcResults.ImportRow(qcResult)
        '                                    i += 1
        '                                Else
        '                                    Exit For
        '                                End If
        '                            Next
        '                        Next
        '                    Next
        '                    lstTestSamplesList = Nothing
        '                    lstCtrlLotList = Nothing
        '                    lstQCResultsList = Nothing

        '                    'Set IncludedInMean = TRUE for the first numOfSeries Results for each Test/SampleType and Control/Lot
        '                    myGlobalDataTO = myQCResultsDAO.MarkStatisticResultsOLD(dbConnection, myQCResultsToUpdateDS)
        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'When the Database Connection was opened locally, then the Commit is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    Else
        '                        'When the Database Connection was opened locally, then the Rollback is executed
        '                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.SetResultsForStatistics", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Set flag IncludedInMean = FALSE for all QC Results marked as included in calculation of statistical values for the 
        '''' informed Test/Sample Type and optionally, the Control/Lot
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module. Optional parameter</param>
        '''' <returns>GlobalDataTO containing the total number of QC Results marked as included in Mean
        ''''          for the specified Test/SampleType and optionally, the Control/Lot</returns>
        '''' <remarks>
        '''' Created by: SA 15/12/2011
        '''' </remarks>
        'Public Function UnmarkStatisticResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                          Optional ByVal pQCControlLotID As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.UnmarkStatisticResultsOLD(dbConnection, pQCTestSampleID, pQCControlLotID)

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.UnmarkStatisticResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Update values used for QC Calculation Criteria in history tables of QC Module (tqcHistoryTestSamples,  
        '''' tqcHistoryTestSamplesRules and tqcHistoryTestControlLots) and also in tables tparTestSamples and tparTestSamplesRules
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pCalculationMode">Calculation Mode: MANUAL/STATISTICS</param>
        '''' <param name="pNumberOfSeries">Minimum number or series required to calculate statistics</param>
        '''' <param name="pRejectionNumeric">Rejection Criteria</param>
        '''' <param name="pQCControlLotIDForWESG1">Identifier (in QC Module) of the first Control to apply Multirules</param>
        '''' <param name="pQCControlLotIDForWESG2">Identifier (in QC Module) of the second Control to apply Multirules</param>
        '''' <param name="pTestSampleMultirulesDS">Typed DataSet TestSamplesMultirulesDS containing the list of available Multirules 
        ''''                                       and indicating for each one if it has been selected to validate the results for 
        ''''                                       the Test/SampleType</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 30/05/2011
        '''' Modified by: SA 16/12/2011 - Set value of IncludedInMean flag for all open Results depending on the CalculationMode
        ''''              SA 03/01/2012 - Update field RejectionCriteria only when it is informed. Return a typed DataSet HistoryTestSamplesDS with all
        ''''                              data of the informed QCTestSampleID (needed when the function is called from function SaveLastCumulatedAsTarget
        ''''                              in HistoryTestControlLotsDelegate)  
        ''''              SA 25/01/2012 - When only one Control/Lot has been selected for Multirules application, update WestgardControlNum=1 for it
        '''' </remarks>
        'Public Function UpdateChangedValuesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pCalculationMode As String, _
        '                                       ByVal pNumberOfSeries As Integer, ByVal pRejectionNumeric As Single, ByVal pQCControlLotIDForWESG1 As String, _
        '                                       ByVal pQCControlLotIDForWESG2 As String, ByVal pTestSampleMultirulesDS As TestSamplesMultirulesDS) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestID As Integer = 0
        '                Dim mySampleType As String = String.Empty
        '                Dim myQCHistoryTestSampleDS As New HistoryTestSamplesDS

        '                '1.1- Update values on table tqcHistoryTestSamples 
        '                '     ** Get the current values for the informed QCTestSampleID 
        '                Dim myQCHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
        '                myGlobalDataTO = myQCHistoryTestSamplesDelegate.Read(dbConnection, pQCTestSampleID)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    myQCHistoryTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)

        '                    If (myQCHistoryTestSampleDS.tqcHistoryTestSamples.Count > 0) Then
        '                        'Get TestID and SampleType 
        '                        myTestID = myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).TestID
        '                        mySampleType = myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).SampleType

        '                        'Update DS with the new values 
        '                        myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).CalculationMode = pCalculationMode
        '                        myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).NumberOfSeries = pNumberOfSeries
        '                        If (pRejectionNumeric > 0) Then myQCHistoryTestSampleDS.tqcHistoryTestSamples(0).RejectionCriteria = pRejectionNumeric
        '                        myQCHistoryTestSampleDS.AcceptChanges()

        '                        'Update on tqcHistoryTestSamples
        '                        myGlobalDataTO = myQCHistoryTestSamplesDelegate.UpdateByQCTestIdAndSampleIDOLD(dbConnection, myQCHistoryTestSampleDS)
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    '1.2 Update values on table tparTestSamples 
        '                    '    ** Get the current values for the TestID and SampleType identified by the informed QCTestSampleID in QC Module
        '                    Dim myTestSampleDelegate As New TestSamplesDelegate
        '                    myGlobalDataTO = myTestSampleDelegate.GetDefinition(dbConnection, myTestID, mySampleType)

        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim myTestSampleDS As TestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

        '                        If (myTestSampleDS.tparTestSamples.Count > 0) Then
        '                            'Update DS with the new values
        '                            myTestSampleDS.tparTestSamples(0).CalculationMode = pCalculationMode
        '                            myTestSampleDS.tparTestSamples(0).NumberOfSeries = pNumberOfSeries
        '                            If (pRejectionNumeric > 0) Then myTestSampleDS.tparTestSamples(0).RejectionCriteria = pRejectionNumeric
        '                            myTestSampleDS.AcceptChanges()

        '                            'Update on tparTestSamples
        '                            myGlobalDataTO = myTestSampleDelegate.Update(dbConnection, myTestSampleDS)
        '                        End If
        '                    End If
        '                End If

        '                '1.3 If a least a Control has been selected for multirules application...
        '                If (pQCControlLotIDForWESG1 <> "") Then
        '                    '** Update field WestgardControlNum for each QCControlLotID in table tqcHistoryTestControlLots
        '                    '   Values: 1 for pQCControlLotIDForWESG1 / 
        '                    '           2 for pQCControlLotIDForWESG2 (when informed) / 
        '                    '           0 for any other QCControlLotID linked to the QCTestSampleID
        '                    If (Not myGlobalDataTO.HasError) Then
        '                        Dim myHistoryTestControlsLotsDelegate As New HistoryTestControlLotsDelegate
        '                        myGlobalDataTO = myHistoryTestControlsLotsDelegate.UpdateWestgardControlNum(dbConnection, pQCTestSampleID, pQCControlLotIDForWESG1, _
        '                                                                                                    pQCControlLotIDForWESG2)
        '                    End If

        '                    '*** Update Multirules to apply for the Test/SampleType
        '                    If (Not myGlobalDataTO.HasError) Then
        '                        'Update in table tparTestSamplesMultirules 
        '                        Dim myTestSampleMultiDelegate As New TestSamplesMultirulesDelegate
        '                        myGlobalDataTO = myTestSampleMultiDelegate.DeleMultiRulesByTestID(dbConnection, myTestID, mySampleType)

        '                        If (Not myGlobalDataTO.HasError) Then
        '                            'TR 14/06/2012 -implement new functionality.
        '                            myGlobalDataTO = myTestSampleMultiDelegate.AddMultiRulesNEW(dbConnection, pTestSampleMultirulesDS)
        '                        End If

        '                        'Update in table tqcHistoryTestSamplesMultirules 
        '                        If (Not myGlobalDataTO.HasError) Then
        '                            Dim myHistoryTestSampleRulesDelegate As New HistoryTestSamplesRulesDelegate
        '                            myGlobalDataTO = myHistoryTestSampleRulesDelegate.Delete(dbConnection, pQCTestSampleID)

        '                            If (Not myGlobalDataTO.HasError) Then
        '                                myGlobalDataTO = myHistoryTestSampleRulesDelegate.InsertFromTestSampleMultiRules(dbConnection, myTestID, mySampleType, pQCTestSampleID)
        '                            End If
        '                        End If
        '                    End If
        '                End If

        '                '1.4 Set value of IncludedInMean flag for all open Results depending on the CalculationMode
        '                If (Not myGlobalDataTO.HasError) Then
        '                    If (pCalculationMode = "MANUAL") Then
        '                        Dim mytqcResultsDAO As New tqcResultsDAO
        '                        myGlobalDataTO = mytqcResultsDAO.UnmarkStatisticResults(dbConnection, pQCTestSampleID)
        '                    Else
        '                        myGlobalDataTO = SetResultsForStatistics(dbConnection, pQCTestSampleID)
        '                    End If
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    myGlobalDataTO.SetDatos = myQCHistoryTestSampleDS
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.UpdateChangedValues", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Update fields ManualResultValue, ResultComment and/or Excluded flag for an specific QC Result
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultsDS">Typed DataSet QCResultsDS containing data of the QC Result to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: TR 08/06/2011
        '''' </remarks>
        'Public Function UpdateManualResultOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultsDS As QCResultsDS) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.UpdateManualResultOLD(dbConnection, pQCResultsDS)

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

        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.UpdateManualResult", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Apply the Multirules selected for the specified QCTestSampleID. 
        '''' In general, selected Multirules are applied for a pair of Controls, first Across Controls and if the Alarm is not raised, 
        '''' for each individual Control, but each Multirule has its particular characteristics:
        ''''    1-2s: Warning Rule. Verify if the Result for each Control is out of +/- 2SD.
        ''''          It is applied Within Control and Within Run. When applied, the rest of selected Rules will be applied only if the
        ''''          1-2s Rule was violated for the result of whatever of the two Controls 
        ''''    1-3s: Verify if the Result for each Control is out of +/- 3SD. 
        ''''          It is applied Within Control and Within Run.
        ''''    2-2s: When applied Across Controls and Within Run, verify if the Results for both Controls are out of +/- 2SD in the same direction
        ''''          When applied Within Control and Across Runs, verify if the last two Runs of each Control are out of +/- 2SD in the same direction
        ''''    R-4s: When applied Across Controls and Within Run, verify if the last result for one Control is upper the Mean and the last result for 
        ''''          the other Control is below the Mean and, additionally, there are more than  4SD between the two results
        ''''          When applied Within Control and Across Runs, for each Control, verify if for the last two runs, one result is upper the Mean and  
        ''''          the other is below the Mean and, additionally, there are more than 4SD between the two results
        ''''    4-1s: When applied Across Controls and Across Runs, verify if the last two Results of both Controls are out of +/- 1SD 
        ''''          When applied Within Control and Across Runs, verify if the last four Results of  each Control are out of +/- 1SD
        ''''    10Xm: When applied Across Controls and Across Runs, verify if the last five Results of both Controls are all below or upper the Mean
        ''''          When applied Within Control and Across Runs, verify if the last ten Results of  each Control are all below or upper the Mean
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pResultsControl1DS">Typed DataSet QCResultsDS containing all Results for the first Control/Lot</param>
        '''' <param name="pResultsControl2DS">Typed DataSet QCResultsDS containing all Results for the second Control/Lot</param>
        '''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing all Results having non Westgard Alarms (Out of Range Alarm)</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultAlarms containing the list of results with Alarms</returns>
        '''' <remarks>
        '''' Created by:  TR 01/06/2011
        '''' Modified by: SA 08/06/2011 - Apply rule R-4s before 4-1s; changed application of rule 2-2s (results have to exceed +/-2s but in the same
        ''''                              direction); changed application of rule 10x (last five results of one Control can be below the Mean and the last 
        ''''                              five results of the other can be upper the Mean, or vice versa); apply rest of rules only if the 1-2s alarm was 
        ''''                              violated for the runs being processed or when it was not applied; solved some code errors
        ''''              SA 05/01/2012 - Removed application of R-4s Within Control and Across Runs
        ''''                              Changed order of application of Rules 2-2s and R-4s: first apply 2-2s Across Controls/Within Runs; next R-4s
        ''''                              Across Controls/Within Run and finally, 2-2s Within Control/Across Runs
        ''''              SA 25/04/2012 - Applied following changes in the way the selected Multirules are applied:
        ''''                              ** If rule 1-2s was not violated for the runs being processed, the rest of the rules are also applied (the opposite  
        ''''                                 change described in 08/06/2011 is cancelled)
        ''''                              ** Rule R-4s is applied Within Control and Across Runs (the opposite change described in 05/01/2012 is cancelled)
        ''''                              ** Changed application of rule R-4s when applied Across Controls and Within Run:
        ''''                                 (a) Current implementation: verify if the last Result for one Control is out of +2SD and the last Result for the 
        ''''                                     other Control is out of –2SD
        ''''                                 (b) New implementation: verify if the last result for one Control is upper the Mean and the last result for the other
        ''''                                     Control is below the Mean and, additionally, there are more than  4SD between the two results 
        ''''                              ** Changed application of rule R-4s when applied Within Control and Across Runs:
        ''''                                 (a) Current implementation: verify if for the last two Results of each Control, one is out of +2SD and the other 
        ''''                                     is out of –2SD 
        ''''                                 (b) New implementation: for each Control, verify if for the last two runs, one result is upper the Mean and the 
        ''''                                     other is below the Mean and, additionally, there are more than 4SD between the two results
        '''' </remarks>
        'Private Function ApplyMultiRulesOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pResultsControl1DS As QCResultsDS, _
        '                                    ByVal pResultsControl2DS As QCResultsDS, ByVal pQCResultAlarmsDS As QCResultAlarms) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing

        '    Try
        '        Dim conditionOK As Boolean = False
        '        Dim belowMeanCtrl1 As Boolean = False
        '        Dim belowMeanCtrl2 As Boolean = False

        '        'Get the list of multirules to apply to the specified QCTestSampleID
        '        Dim myHistoryTestSamplesRulesDelegate As New HistoryTestSamplesRulesDelegate
        '        myGlobalDataTO = myHistoryTestSamplesRulesDelegate.ReadByQCTestSampleIDOLD(pDBConnection, pQCTestSampleID)

        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            Dim myHistoryTestSamplesRulesDS As HistoryTestSamplesRulesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesRulesDS)

        '            If (myHistoryTestSamplesRulesDS.tqcHistoryTestSamplesRules.Count > 0) Then
        '                'Move the selected Rules to a List
        '                Dim myHistTestSampRulesList As List(Of HistoryTestSamplesRulesDS.tqcHistoryTestSamplesRulesRow)
        '                myHistTestSampRulesList = (From a In myHistoryTestSamplesRulesDS.tqcHistoryTestSamplesRules _
        '                                         Select a).ToList()

        '                'Get the numbers of results for each Control/Lot
        '                Dim myRunsFirstControl As Integer = pResultsControl1DS.tqcResults.Count
        '                Dim myRunsSecondControl As Integer = pResultsControl2DS.tqcResults.Count

        '                'Set the value to the TotalRuns
        '                Dim myTotalRuns As Integer = 0
        '                If (myRunsFirstControl >= myRunsSecondControl) Then
        '                    myTotalRuns = myRunsFirstControl
        '                Else
        '                    myTotalRuns = myRunsSecondControl
        '                End If

        '                Dim myAlarmCtrl1 As Boolean
        '                Dim myAlarmCtrl2 As Boolean

        '                For i As Integer = 0 To myTotalRuns - 1
        '                    myAlarmCtrl1 = False
        '                    myAlarmCtrl2 = False

        '                    'WESTGARD_1-2s
        '                    If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").Count = 1) Then
        '                        'Within Control/Within Run --> Verify if the Result for the first Control is out of +/- 2SD
        '                        If (i < myRunsFirstControl) Then
        '                            If (Math.Abs(pResultsControl1DS.tqcResults(i).RELError) > 2) Then
        '                                InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                          myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").First().RuleID)
        '                            End If
        '                        End If

        '                        'Within Control/Within Run --> Verify if the Result for the second Control is out of +/- 2SD
        '                        If (i < myRunsSecondControl) Then
        '                            If (Math.Abs(pResultsControl2DS.tqcResults(i).RELError) > 2) Then
        '                                InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                          myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").First().RuleID)
        '                            End If
        '                        End If
        '                    End If

        '                    'Continue applying rules the rest of selected rules..
        '                    'WESTGARD_1-3s
        '                    If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").Count = 1) Then
        '                        'Within Control/Within Run --> Verify if the Result for the first Control is out of +/- 3SD
        '                        If (i < myRunsFirstControl) Then
        '                            If (Math.Abs(pResultsControl1DS.tqcResults(i).RELError) > 3) Then
        '                                InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                          myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").First().RuleID)
        '                                myAlarmCtrl1 = True
        '                            End If
        '                        End If

        '                        'Within Control/Within Run --> Verify if the Result for the first Control is out of +/- 3SD
        '                        If (i < myRunsSecondControl) Then
        '                            If (Math.Abs(pResultsControl2DS.tqcResults(i).RELError) > 3) Then
        '                                InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                          myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").First().RuleID)
        '                                myAlarmCtrl2 = True
        '                            End If
        '                        End If
        '                    End If

        '                    'WESTGARD_2-2s Across Controls/Within Run
        '                    If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").Count = 1) Then
        '                        If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2) Then
        '                            'Across Controls/Within Run --> Verify if the Results for both Controls are out of +/- 2SD in the same direction
        '                            If (i < myRunsFirstControl AndAlso i < myRunsSecondControl) Then
        '                                If ((pResultsControl1DS.tqcResults(i).RELError > 2) AndAlso (pResultsControl2DS.tqcResults(i).RELError > 2)) OrElse _
        '                                   ((pResultsControl1DS.tqcResults(i).RELError < -2) AndAlso (pResultsControl2DS.tqcResults(i).RELError < -2)) Then
        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl1 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().RuleID)
        '                                    myAlarmCtrl1 = True

        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl2 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().RuleID)
        '                                    myAlarmCtrl2 = True
        '                                End If
        '                            End If
        '                        End If
        '                    End If

        '                    'WESTGARD_R-4s 
        '                    If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").Count = 1) Then
        '                        If (Not myAlarmCtrl1 And Not myAlarmCtrl2) Then
        '                            'Across Controls/Within Run --> Verify if the last Result for one Control is upper the Mean and the last Result for the other  
        '                            '                               Control is below the Mean and in this case, if there are more than 4SD between them
        '                            If (i < myRunsFirstControl AndAlso i < myRunsSecondControl) Then
        '                                If (pResultsControl1DS.tqcResults(i).RELError > 0 AndAlso pResultsControl2DS.tqcResults(i).RELError < 0) OrElse _
        '                                   (pResultsControl1DS.tqcResults(i).RELError < 0 AndAlso pResultsControl2DS.tqcResults(i).RELError > 0) Then
        '                                    If (Math.Abs(pResultsControl1DS.tqcResults(i).RELError) + Math.Abs(pResultsControl2DS.tqcResults(i).RELError) > 4) Then
        '                                        'Insert a row in QCAlarmsDS; set alarmCtrl1 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().RuleID)
        '                                        myAlarmCtrl1 = True

        '                                        'Insert a row in QCAlarmsDS; set alarmCtrl2 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().RuleID)
        '                                        myAlarmCtrl2 = True
        '                                    End If
        '                                End If
        '                            End If
        '                        End If

        '                        'Within Control/Across Runs --> Verify if for the last two Results of each Control, one is upper the Mean and the other is below 
        '                        '                               the Mean, and in this case, if there are more than 4SD between them. This verification can be 
        '                        '                               done only when the previous Result has not been rejected
        '                        If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2 AndAlso i > 0) Then
        '                            If (i < myRunsFirstControl AndAlso pResultsControl1DS.tqcResults(i - 1).ValidationStatus <> "ERROR") Then
        '                                If (pResultsControl1DS.tqcResults(i).RELError > 0 AndAlso pResultsControl1DS.tqcResults(i - 1).RELError < 0) OrElse _
        '                                   (pResultsControl1DS.tqcResults(i).RELError < 0 AndAlso pResultsControl1DS.tqcResults(i - 1).RELError > 0) Then
        '                                    If (Math.Abs(pResultsControl1DS.tqcResults(i).RELError) + Math.Abs(pResultsControl1DS.tqcResults(i - 1).RELError) > 4) Then
        '                                        'Insert a row in QCAlarmsDS; set alarmCtrl1 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().RuleID)
        '                                        myAlarmCtrl1 = True
        '                                    End If
        '                                End If
        '                            End If

        '                            If (i < myRunsSecondControl AndAlso pResultsControl2DS.tqcResults(i - 1).ValidationStatus <> "ERROR") Then
        '                                If (pResultsControl2DS.tqcResults(i).RELError > 0 AndAlso pResultsControl2DS.tqcResults(i - 1).RELError < 0) OrElse _
        '                                   (pResultsControl2DS.tqcResults(i).RELError < 0 AndAlso pResultsControl2DS.tqcResults(i - 1).RELError > 0) Then
        '                                    If (Math.Abs(pResultsControl2DS.tqcResults(i).RELError) + Math.Abs(pResultsControl2DS.tqcResults(i - 1).RELError) > 4) Then
        '                                        'Insert a row in QCAlarmsDS; set alarmCtrl2 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().RuleID)
        '                                        myAlarmCtrl2 = True
        '                                    End If
        '                                End If
        '                            End If
        '                        End If
        '                    End If

        '                    'WESTGARD_2-2s Within Control/Across Runs
        '                    If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").Count = 1) Then
        '                        'Within Control/Across Runs --> Verify if the last two Results for each Control are out of +/- 2SD in the same direction
        '                        '                               This verification can be done only when the previous Result has not been rejected
        '                        If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2 AndAlso i > 0) Then
        '                            If (i < myRunsFirstControl AndAlso pResultsControl1DS.tqcResults(i - 1).ValidationStatus <> "ERROR") Then
        '                                If ((pResultsControl1DS.tqcResults(i).RELError > 2) AndAlso (pResultsControl1DS.tqcResults(i - 1).RELError > 2)) OrElse _
        '                                   ((pResultsControl1DS.tqcResults(i).RELError < -2) AndAlso (pResultsControl1DS.tqcResults(i - 1).RELError < -2)) Then
        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl1 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().RuleID)
        '                                    myAlarmCtrl1 = True
        '                                End If
        '                            End If

        '                            If (i < myRunsSecondControl AndAlso pResultsControl2DS.tqcResults(i - 1).ValidationStatus <> "ERROR") Then
        '                                If ((pResultsControl2DS.tqcResults(i).RELError > 2) AndAlso (pResultsControl2DS.tqcResults(i - 1).RELError > 2)) OrElse _
        '                                   ((pResultsControl2DS.tqcResults(i).RELError < -2) AndAlso (pResultsControl2DS.tqcResults(i - 1).RELError < -2)) Then
        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl2 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().RuleID)
        '                                    myAlarmCtrl2 = True
        '                                End If
        '                            End If
        '                        End If
        '                    End If

        '                    'WESTGARD_4-1s 
        '                    If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").Count = 1) Then
        '                        'Across Controls/Across Runs --> Verify if the last two Results of both Controls are out of +/- 1SD 
        '                        '                                This verification can be done only when the previous Results have not been rejected
        '                        If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2 AndAlso i > 0) Then
        '                            If (i < myRunsFirstControl AndAlso i < myRunsSecondControl) AndAlso _
        '                               (pResultsControl1DS.tqcResults(i - 1).ValidationStatus <> "ERROR" AndAlso pResultsControl2DS.tqcResults(i - 1).ValidationStatus <> "ERROR") Then
        '                                If (Math.Abs(pResultsControl1DS.tqcResults(i).RELError) > 1) AndAlso (Math.Abs(pResultsControl1DS.tqcResults(i - 1).RELError) > 1) AndAlso _
        '                                   (Math.Abs(pResultsControl2DS.tqcResults(i).RELError) > 1) AndAlso (Math.Abs(pResultsControl2DS.tqcResults(i - 1).RELError) > 1) Then
        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl1 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().RuleID)
        '                                    myAlarmCtrl1 = True

        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl2 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().RuleID)
        '                                    myAlarmCtrl2 = True
        '                                End If
        '                            End If

        '                            'Within Control/Across Runs --> Verify if the last four Results of each Control are out of +/- 1SD
        '                            '                               This verification can be done only when none of the previous Results used have been rejected
        '                            If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2 AndAlso i > 2) Then
        '                                If (i < myRunsFirstControl) Then
        '                                    conditionOK = True
        '                                    For j As Integer = 0 To 3
        '                                        conditionOK = (pResultsControl1DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                      (Math.Abs(pResultsControl1DS.tqcResults(i - j).RELError) > 1)
        '                                        If (Not conditionOK) Then Exit For
        '                                    Next

        '                                    If (conditionOK) Then
        '                                        'Insert a row in QCAlarms1DS; set alarmCtrl1 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().RuleID)
        '                                        myAlarmCtrl1 = True
        '                                    End If
        '                                End If

        '                                If (i < myRunsSecondControl) Then
        '                                    conditionOK = True
        '                                    For j As Integer = 0 To 3
        '                                        conditionOK = (pResultsControl2DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                      (Math.Abs(pResultsControl2DS.tqcResults(i - j).RELError) > 1)
        '                                        If (Not conditionOK) Then Exit For
        '                                    Next

        '                                    If (conditionOK) Then
        '                                        'Insert a row in QCAlarms2DS; set alarmCtrl2 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().RuleID)
        '                                        myAlarmCtrl2 = True
        '                                    End If
        '                                End If
        '                            End If
        '                        End If
        '                    End If

        '                    'WESTGARD_10X  
        '                    If (myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_10X").Count = 1) Then
        '                        If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2 AndAlso i > 3) Then
        '                            'Across Controls/Across Runs --> Verify if the last five Results of both Controls are all below the Mean, or all upper the Mean
        '                            '                                This verification can be done only when none of the previous Results used have been rejected
        '                            If (i < myRunsFirstControl AndAlso i < myRunsSecondControl) Then
        '                                conditionOK = True

        '                                belowMeanCtrl1 = (pResultsControl1DS.tqcResults(i).RELError < 0)
        '                                For j As Integer = 0 To 4
        '                                    If (belowMeanCtrl1) Then
        '                                        conditionOK = (pResultsControl1DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                      (pResultsControl1DS.tqcResults(i - j).RELError < 0)
        '                                    Else
        '                                        conditionOK = (pResultsControl1DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                      (pResultsControl1DS.tqcResults(i - j).RELError > 0)
        '                                    End If
        '                                    If (Not conditionOK) Then Exit For
        '                                Next

        '                                If (conditionOK) Then
        '                                    belowMeanCtrl2 = (pResultsControl2DS.tqcResults(i).RELError < 0)
        '                                    For j As Integer = 0 To 4
        '                                        If (belowMeanCtrl2) Then
        '                                            conditionOK = (pResultsControl2DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                          (pResultsControl2DS.tqcResults(i - j).RELError < 0)
        '                                        Else
        '                                            conditionOK = (pResultsControl2DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                          (pResultsControl2DS.tqcResults(i - j).RELError > 0)
        '                                        End If
        '                                        If (Not conditionOK) Then Exit For
        '                                    Next
        '                                End If

        '                                If (conditionOK) Then
        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl1 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().RuleID)
        '                                    myAlarmCtrl1 = True

        '                                    'Insert a row in QCAlarmsDS; set alarmCtrl2 = TRUE
        '                                    InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                              myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().RuleID)
        '                                    myAlarmCtrl2 = True
        '                                End If
        '                            End If

        '                            'Within Control/Across Runs --> Verify if the last ten Results of each Control are all below the Mean, or all upper the Mean
        '                            '                               This verification can be done only when none of the previous Results used have been rejected
        '                            If (Not myAlarmCtrl1 AndAlso Not myAlarmCtrl2 AndAlso i > 8) Then
        '                                If (i < myRunsFirstControl) Then
        '                                    conditionOK = True
        '                                    belowMeanCtrl1 = (pResultsControl1DS.tqcResults(i).RELError < 0)
        '                                    For j As Integer = 0 To 9
        '                                        If (belowMeanCtrl1) Then
        '                                            conditionOK = (pResultsControl1DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                          (pResultsControl1DS.tqcResults(i - j).RELError < 0)
        '                                        Else
        '                                            conditionOK = (pResultsControl1DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                          (pResultsControl1DS.tqcResults(i - j).RELError > 0)
        '                                        End If
        '                                        If (Not conditionOK) Then Exit For
        '                                    Next

        '                                    If (conditionOK) Then
        '                                        'Insert a row in QCAlarmsDS; set alarmCtrl1 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl1DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().RuleID)
        '                                        myAlarmCtrl1 = True
        '                                    End If
        '                                End If

        '                                If (i < myRunsSecondControl) Then
        '                                    conditionOK = True
        '                                    belowMeanCtrl2 = (pResultsControl2DS.tqcResults(i).RELError < 0)
        '                                    For j As Integer = 0 To 9
        '                                        If (belowMeanCtrl2) Then
        '                                            conditionOK = (pResultsControl2DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                          (pResultsControl2DS.tqcResults(i - j).RELError < 0)
        '                                        Else
        '                                            conditionOK = (pResultsControl2DS.tqcResults(i - j).ValidationStatus <> "ERROR") AndAlso _
        '                                                          (pResultsControl2DS.tqcResults(i - j).RELError > 0)
        '                                        End If
        '                                        If (Not conditionOK) Then Exit For
        '                                    Next

        '                                    If (conditionOK) Then
        '                                        'Insert a row in QCAlarmsDS; set alarmCtrl2 = TRUE
        '                                        InsertNewQcResultAlarmOLD(pQCResultAlarmsDS, pResultsControl2DS.tqcResults(i), _
        '                                                                  myHistTestSampRulesList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().RuleID)
        '                                        myAlarmCtrl2 = True
        '                                    End If
        '                                End If
        '                            End If
        '                        End If
        '                    End If

        '                Next
        '                myGlobalDataTO.SetDatos = pQCResultAlarmsDS
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.ApplyMultiRules", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For each Control/Lot having non cumulated QC Results with Alarms for the specified Test/SampleType, build for each Result 
        '''' a String list containing the description of all alarms (in the current application Language) divided by commas
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pOpenQCResultsDS">Typed DataSet OpenQCResultsDS containing all Controls/Lots having non cumulated
        ''''                                QC Results for the informed Test/SampleType</param>
        '''' <param name="pLanguageID">Code of the current Application Language</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultAlarms having for each QCTestSampleID/QCControlLotID and 
        ''''          RunNumber the list of Alarm descriptions divided by commas</returns>
        '''' <remarks>
        '''' Created by:  TR
        '''' Modified by: SA 16/06/2011 - Pass the active RunsGroupNumber as parameter for the function used to get the Alarms
        '''' </remarks>
        'Private Function GetAllAlarmsDescriptionsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
        '                                          ByVal pOpenQCResultsDS As OpenQCResultsDS, ByVal pLanguageID As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        Dim myQCResultAlarmsDS As New QCResultAlarms
        '        Dim myReceivedQCResultAlarmsDS As New QCResultAlarms
        '        Dim myQCResultAlarmsDelegate As New QCResultAlarmsDelegate

        '        For Each openQCResultsROW As OpenQCResultsDS.tOpenResultsRow In pOpenQCResultsDS.tOpenResults.Rows
        '            myGlobalDataTO = myQCResultAlarmsDelegate.GetAlarmsAndDescriptionsOLD(pDBConnection, pQCTestSampleID, openQCResultsROW.QCControlLotID, _
        '                                                                                  openQCResultsROW.RunsGroupNumber, pLanguageID)
        '            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                myReceivedQCResultAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

        '                For Each receivedRow As QCResultAlarms.tqcResultAlarmsRow In myReceivedQCResultAlarmsDS.tqcResultAlarms.Rows
        '                    myQCResultAlarmsDS.tqcResultAlarms.ImportRow(receivedRow)
        '                Next
        '            Else
        '                'Error getting the multilanguage description of the Alarms...
        '                Exit For
        '            End If
        '        Next
        '        If (Not myGlobalDataTO.HasError) Then myGlobalDataTO.SetDatos = myQCResultAlarmsDS
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorMessage = ex.Message
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetAllAlarmsDescriptions", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Insert a row in a QCResultAlarmsDS for a QC Result and the specified Alarm Code
        '''' </summary>
        '''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarmsDS in which the Result Alarm is added</param>
        '''' <param name="pQCResultsRow">Result information</param>
        '''' <remarks>
        '''' Created by:  TR 01/06/2011
        '''' Modified by: SA 08/06/2011 - Update also fields AlarmsList and ValidationStatus for the Result in process
        '''' </remarks>
        'Private Sub InsertNewQcResultAlarmOLD(ByRef pQCResultAlarmsDS As QCResultAlarms, ByVal pQCResultsRow As QCResultsDS.tqcResultsRow, ByVal pRuleID As String)
        '    Try
        '        'Update the Validation Status
        '        pQCResultsRow.BeginEdit()
        '        If (pRuleID = "WESTGARD_1-2s") Then
        '            pQCResultsRow.ValidationStatus = "WARNING"
        '        Else
        '            pQCResultsRow.ValidationStatus = "ERROR"
        '        End If
        '        pQCResultsRow.EndEdit()
        '        pQCResultsRow.AcceptChanges()

        '        'Add the Alarm to the QC Result in the QCResultAlarmsDS  
        '        Dim myQCResultAlarmsRow As QCResultAlarms.tqcResultAlarmsRow

        '        myQCResultAlarmsRow = pQCResultAlarmsDS.tqcResultAlarms.NewtqcResultAlarmsRow
        '        myQCResultAlarmsRow.QCControlLotID = pQCResultsRow.QCControlLotID
        '        myQCResultAlarmsRow.QCTestSampleID = pQCResultsRow.QCTestSampleID
        '        myQCResultAlarmsRow.RunsGroupNumber = pQCResultsRow.RunsGroupNumber
        '        myQCResultAlarmsRow.RunNumber = pQCResultsRow.RunNumber
        '        myQCResultAlarmsRow.AlarmID = pRuleID

        '        pQCResultAlarmsDS.tqcResultAlarms.AddtqcResultAlarmsRow(myQCResultAlarmsRow)
        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.InsertNewQcResultAlarm", EventLogEntryType.Error, False)
        '    End Try
        'End Sub

        'Public Function GetFirstDateTimeForResultsCreation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                                   ByVal pControlID As Integer, ByVal pLotNumber As String) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultsDAO As New tqcResultsDAO
        '                myGlobalDataTO = myQCResultsDAO.GetFirstDateTimeForResultsCreation(dbConnection, pTestID, pSampleType, pControlID, pLotNumber)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "QCResultsDelegate.GetFirstDateTimeForResultsCreation", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region

    End Class
End Namespace

Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class QCResultAlarmsDelegate

#Region "Other Methods"
        ''' <summary>
        ''' Insert Alarms for QC Results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing the Alarms to insert</param>
        ''' <returns>GlobalDataSet containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 02/01/2011
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultAlarmsDS As QCResultAlarms) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
                        myGlobalDataTO = myQCResultAlarmsDAO.CreateNEW(dbConnection, pQCResultAlarmsDS)

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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "QCResultAlarmsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete Alarms for all results of the specified QCTestSampleID/QCControlLotID/AnalyzerID, and optionally, 
        ''' RunsGroupNumber and an specific result (RunNumber)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Optional parameter. Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Optional parameter. Number of the open Runs Group for the specified
        '''                                QCTestSampleID/QCControlLotID/AnalyzerID</param>
        ''' <param name="pRunNumber">Optional parameter. Run Number of an specific Result</param>
        ''' <returns>GlobalDataSet containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 01/06/2011
        ''' Modified by: SA 06/07/2011 - Added optional parameter to delete only the Alarms of an specific result
        '''              SA 21/12/2011 - Parameter for the RunsGroupNumber changed to optional
        '''              SA 04/06/2012 - Added optional parameter for AnalyzerID   
        ''' </remarks>
        Public Function DeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                  Optional ByVal pAnalyzerID As String = "", Optional ByVal pRunsGroupNumber As Integer = 0, Optional ByVal pRunNumber As Integer = 0) _
                                  As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
                        myGlobalDataTO = myQCResultAlarmsDAO.DeleteNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pRunsGroupNumber, pRunNumber)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "QCResultAlarmsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all Alarms for all QC Results included in the informed RunsGroup for the informed QCTestSampleID/QCControlLotID/AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRunsGroupNumber">Number of the Runs Group in which the results are included</param>
        ''' <returns>GlobalDataSet containing a typed DataSet QCResultAlarms with the list of Alarms for all QC Results
        '''          included in the informed RunsGroup for the Test/SampleType and Control/Lot</returns>
        ''' <remarks>
        ''' Created by:  SA 28/06/2011 
        ''' Modified by: SA 04/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetAlarmsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                     ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
                        myGlobalDataTO = myQCResultAlarmsDAO.ReadByRunsGroupNumberNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, pRunsGroupNumber)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myQCResultAlarmsDS As QCResultAlarms = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

                            Dim myAlarmsDS As New QCResultAlarms
                            If (myQCResultAlarmsDS.tqcResultAlarms.Count > 0) Then
                                'Get all different Runs Numbers returned to build a String list containing the ID of all its Alarms
                                Dim myDistinctRunsNumberList As List(Of Integer)
                                myDistinctRunsNumberList = (From a In myQCResultAlarmsDS.tqcResultAlarms _
                                                           Select a.RunNumber).Distinct.ToList()

                                Dim myAlarmIDList As String = ""
                                Dim myQCResultAlarmsList As New List(Of QCResultAlarms.tqcResultAlarmsRow)
                                For Each runNumber As Integer In myDistinctRunsNumberList
                                    myAlarmIDList = ""

                                    'Get all Alarms for the RunNumber and build the String List
                                    myQCResultAlarmsList = (From a In myQCResultAlarmsDS.tqcResultAlarms _
                                                           Where a.RunNumber = runNumber Select a).ToList()

                                    For Each resultAlarmsRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsList
                                        myAlarmIDList &= resultAlarmsRow.AlarmID.ToString & ", "
                                    Next

                                    If (myAlarmIDList.Count > 0) Then
                                        'Add the String List to the DS to return
                                        myQCResultAlarmsList.First().QCTestSampleID = pQCTestSampleID
                                        myQCResultAlarmsList.First().QCControlLotID = pQCControlLotID
                                        myQCResultAlarmsList.First().AnalyzerID = pAnalyzerID
                                        myQCResultAlarmsList.First().RunsGroupNumber = pRunsGroupNumber
                                        myQCResultAlarmsList.First().RunNumber = runNumber
                                        myQCResultAlarmsList.First().AlarmDesc = myAlarmIDList.Remove(myAlarmIDList.Count - 2, 1)

                                        myAlarmsDS.tqcResultAlarms.ImportRow(myQCResultAlarmsList.First())
                                    End If
                                Next
                            End If

                            'Return the DS with the String List of Alarms descriptions for each Result (RunNumber)
                            myGlobalDataTO.SetDatos = myAlarmsDS
                            myGlobalDataTO.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "QCResultAlarmsDelegate.GetAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the informed QCTestSampleID/QCControlLotID/AnalyzerID, get all Results having alarms and build for each Result a String list containing
        ''' the description of all Alarms (in the current application Language) divided by commas
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pCurrentLanguage">Code of the current Application Language</param>
        ''' <returns>GlobalDataTO containing a typed DataSet QCResultAlarms with the list of Alarm Descriptions by Result in a String list</returns>
        ''' <remarks>
        ''' Created by:  TR 02/06/2011
        ''' Modified by: SA 16/06/2011 - Added parameter for the RunsGroupNumber and pass it to the DAO function to get only alarms for Results in the
        '''                              active RunsGroup
        ''' </remarks>
        Public Function GetAlarmsAndDescriptionsNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                                                    ByVal pAnalyzerID As String, ByVal pRunsGroupNumber As Integer, ByVal pCurrentLanguage As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
                        myGlobalDataTO = myQCResultAlarmsDAO.GetAlarmsAndDescriptionsNEW(dbConnection, pQCTestSampleID, pQCControlLotID, pAnalyzerID, _
                                                                                         pRunsGroupNumber, pCurrentLanguage)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myQCResultAlarmsDS As QCResultAlarms = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

                            Dim myResultQCResultAlarmsDS As New QCResultAlarms
                            If (myQCResultAlarmsDS.tqcResultAlarms.Count > 0) Then
                                'Get all different Runs Numbers returned to build a String list containing the description of all its Alarms
                                'in the current application Language
                                Dim myDistinctRunsNumberList As List(Of Integer)
                                myDistinctRunsNumberList = (From a In myQCResultAlarmsDS.tqcResultAlarms _
                                                           Select a.RunNumber).Distinct.ToList()

                                Dim myAlarmIDs As String = ""
                                Dim myAlarmDesc As String = ""
                                Dim myQCResultAlarmsList As New List(Of QCResultAlarms.tqcResultAlarmsRow)
                                For Each runNumber As Integer In myDistinctRunsNumberList
                                    myAlarmIDs = ""
                                    myAlarmDesc = ""

                                    'Get all Alarms for the RunNumber and build the String List
                                    myQCResultAlarmsList = (From a In myQCResultAlarmsDS.tqcResultAlarms _
                                                           Where a.RunNumber = runNumber Select a).ToList()

                                    For Each resultAlarmsRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsList
                                        myAlarmIDs &= resultAlarmsRow.AlarmID.ToString & ", "
                                        myAlarmDesc &= resultAlarmsRow.AlarmDesc & ", "
                                    Next

                                    If (myAlarmDesc.Count > 0) Then
                                        'Add the String List to the DS to return
                                        myQCResultAlarmsList.First().QCTestSampleID = pQCTestSampleID
                                        myQCResultAlarmsList.First().QCControlLotID = pQCControlLotID
                                        myQCResultAlarmsList.First().AnalyzerID = pAnalyzerID
                                        myQCResultAlarmsList.First().AlarmDesc = myAlarmDesc.Remove(myAlarmDesc.Count - 2, 1)
                                        myQCResultAlarmsList.First().AlarmIDList = myAlarmIDs.Remove(myAlarmIDs.Count - 2, 1)

                                        myResultQCResultAlarmsDS.tqcResultAlarms.ImportRow(myQCResultAlarmsList.First())
                                    End If
                                Next
                            End If

                            'Return the DS with the String List of Alarms descriptions for each Result (RunNumber)
                            myGlobalDataTO.SetDatos = myResultQCResultAlarmsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "QCResultsDelegate.GetAlarmsAndDescriptions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Insert Alarms for QC Results
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCResultAlarmsDS">Typed DataSet QCResultAlarms containing the Alarms to insert</param>
        '''' <returns>GlobalDataSet containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 02/01/2011
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCResultAlarmsDS As QCResultAlarms) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
        '                myGlobalDataTO = myQCResultAlarmsDAO.CreateOLD(dbConnection, pQCResultAlarmsDS)

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

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "QCResultAlarmsDelegate.Create", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Delete Alarms for all results of the specified QCTestSampleID/QCControlLotID/RunsGroupNumber, 
        '''' and optionally, of an specific result (RunNumber)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNumber">Optional parameter. Number of the open Runs Group for the specified
        ''''                                Test/SampleType and Control/Lot</param>
        '''' <param name="pRunNumber">Optional parameter. Run Number of an specific Result</param>
        '''' <returns>GlobalDataSet containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 01/06/2011
        '''' Modified by: SA 06/07/2011 - Added optional parameter to delete only the Alarms of an specific result
        ''''              SA 21/12/2011 - Parameter for the RunsGroupNumber changed to optional
        '''' </remarks>
        'Public Function DeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                          Optional ByVal pRunsGroupNumber As Integer = 0, Optional ByVal pRunNumber As Integer = 0) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
        '                myGlobalDataTO = myQCResultAlarmsDAO.DeleteOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pRunsGroupNumber, pRunNumber)

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

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "QCResultAlarmsDelegate.Delete", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Read all Alarms for all QC Results included in the informed RunsGroup for the Test/SampleType and Control/Lot
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pRunsGroupNumber">Number of the Runs Group in which the results are included</param>
        '''' <returns>GlobalDataSet containing a typed DataSet QCResultAlarms with the list of Alarms for all QC Results
        ''''          included in the informed RunsGroup for the Test/SampleType and Control/Lot</returns>
        '''' <remarks>
        '''' Created by:  SA 28/06/2011 
        '''' </remarks>
        'Public Function GetAlarmsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                             ByVal pRunsGroupNumber As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
        '                myGlobalDataTO = myQCResultAlarmsDAO.ReadByRunsGroupNumberOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pRunsGroupNumber)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myQCResultAlarmsDS As QCResultAlarms = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

        '                    Dim myAlarmsDS As New QCResultAlarms
        '                    If (myQCResultAlarmsDS.tqcResultAlarms.Count > 0) Then
        '                        'Get all different Runs Numbers returned to build a String list containing the ID of all its Alarms
        '                        Dim myDistinctRunsNumberList As List(Of Integer)
        '                        myDistinctRunsNumberList = (From a In myQCResultAlarmsDS.tqcResultAlarms _
        '                                                   Select a.RunNumber).Distinct.ToList()

        '                        Dim myAlarmIDList As String = ""
        '                        Dim myQCResultAlarmsList As New List(Of QCResultAlarms.tqcResultAlarmsRow)
        '                        For Each runNumber As Integer In myDistinctRunsNumberList
        '                            myAlarmIDList = ""

        '                            'Get all Alarms for the RunNumber and build the String List
        '                            myQCResultAlarmsList = (From a In myQCResultAlarmsDS.tqcResultAlarms _
        '                                                   Where a.RunNumber = runNumber Select a).ToList()

        '                            For Each resultAlarmsRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsList
        '                                myAlarmIDList &= resultAlarmsRow.AlarmID.ToString & ", "
        '                            Next

        '                            If (myAlarmIDList.Count > 0) Then
        '                                'Add the String List to the DS to return
        '                                myQCResultAlarmsList.First().QCTestSampleID = pQCTestSampleID
        '                                myQCResultAlarmsList.First().QCControlLotID = pQCControlLotID
        '                                myQCResultAlarmsList.First().RunsGroupNumber = pRunsGroupNumber
        '                                myQCResultAlarmsList.First().RunNumber = runNumber
        '                                myQCResultAlarmsList.First().AlarmDesc = myAlarmIDList.Remove(myAlarmIDList.Count - 2, 1)

        '                                myAlarmsDS.tqcResultAlarms.ImportRow(myQCResultAlarmsList.First())
        '                            End If
        '                        Next
        '                    End If

        '                    'Return the DS with the String List of Alarms descriptions for each Result (RunNumber)
        '                    myGlobalDataTO.SetDatos = myAlarmsDS
        '                    myGlobalDataTO.HasError = False
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "QCResultAlarmsDelegate.GetAlarms", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' For the informed QCTestSampleID/QCControlLotID, get all Results having alarms and build for each Result a String list containing
        '''' the description of all Alarms (in the current application Language) divided by commas
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
        '''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        '''' <param name="pCurrentLanguage">Code of the current Application Language</param>
        '''' <returns>GlobalDataTO containing a typed DataSet QCResultAlarms with the list of Alarm Descriptions by Result in a String list</returns>
        '''' <remarks>
        '''' Created by:  TR 02/06/2011
        '''' Modified by: SA 16/06/2011 - Added parameter for the RunsGroupNumber and pass it to the DAO function to get only alarms for Results in the
        ''''                              active RunsGroup
        '''' </remarks>
        'Public Function GetAlarmsAndDescriptionsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
        '                                            ByVal pRunsGroupNumber As Integer, ByVal pCurrentLanguage As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myQCResultAlarmsDAO As New tqcResultAlarmsDAO
        '                myGlobalDataTO = myQCResultAlarmsDAO.GetAlarmsAndDescriptionsOLD(dbConnection, pQCTestSampleID, pQCControlLotID, pRunsGroupNumber, pCurrentLanguage)

        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim myQCResultAlamrsDS As QCResultAlarms = DirectCast(myGlobalDataTO.SetDatos, QCResultAlarms)

        '                    Dim myResultQCResultAlarmsDS As New QCResultAlarms
        '                    If (myQCResultAlamrsDS.tqcResultAlarms.Count > 0) Then
        '                        'Get all different Runs Numbers returned to build a String list containing the description of all its Alarms
        '                        'in the current application Language
        '                        Dim myDistinctRunsNumberList As List(Of Integer)
        '                        myDistinctRunsNumberList = (From a In myQCResultAlamrsDS.tqcResultAlarms _
        '                                                   Select a.RunNumber).Distinct.ToList()

        '                        Dim myAlarmIDs As String = ""
        '                        Dim myAlarmDesc As String = ""
        '                        Dim myQCResultAlarmsList As New List(Of QCResultAlarms.tqcResultAlarmsRow)
        '                        For Each runNumber As Integer In myDistinctRunsNumberList
        '                            myAlarmIDs = ""
        '                            myAlarmDesc = ""

        '                            'Get all Alarms for the RunNumber and build the String List
        '                            myQCResultAlarmsList = (From a In myQCResultAlamrsDS.tqcResultAlarms _
        '                                                   Where a.RunNumber = runNumber Select a).ToList()

        '                            For Each resultAlarmsRow As QCResultAlarms.tqcResultAlarmsRow In myQCResultAlarmsList
        '                                myAlarmIDs &= resultAlarmsRow.AlarmID.ToString & ", "
        '                                myAlarmDesc &= resultAlarmsRow.AlarmDesc & ", "
        '                            Next

        '                            If (myAlarmDesc.Count > 0) Then
        '                                'Add the String List to the DS to return
        '                                myQCResultAlarmsList.First().QCTestSampleID = pQCTestSampleID
        '                                myQCResultAlarmsList.First().QCControlLotID = pQCControlLotID
        '                                myQCResultAlarmsList.First().AlarmDesc = myAlarmDesc.Remove(myAlarmDesc.Count - 2, 1)
        '                                myQCResultAlarmsList.First().AlarmIDList = myAlarmIDs.Remove(myAlarmIDs.Count - 2, 1)

        '                                myResultQCResultAlarmsDS.tqcResultAlarms.ImportRow(myQCResultAlarmsList.First())
        '                            End If
        '                        Next
        '                    End If

        '                    'Return the DS with the String List of Alarms descriptions for each Result (RunNumber)
        '                    myGlobalDataTO.SetDatos = myResultQCResultAlarmsDS
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "QCResultsDelegate.GetAlarmsAndDescriptions", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region

   End Class
End Namespace

Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants

Namespace Biosystems.Ax00.BL

    Public Class HistoryControlLotsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' When a Lot is changed for a Control or a Control is deleted, the Lot is marked as Closed in the history table of QC Module.
        ''' Besides, if the Control was deleted, it is marked also as deleted in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Identifier of the Control that has been changed or deleted</param>
        ''' <param name="pLotNumber">Lot Number that has been changed for the Control. Optional parameter, informed only when
        '''                          the screen was called for a Lot change</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/05/2011
        ''' </remarks>
        Public Function CloseLotDeleteControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                              Optional ByVal pLotNumber As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryControlLotsDAO As New tqcHistoryControlLotsDAO
                        myGlobalDataTO = myHistoryControlLotsDAO.CloseLotDeleteControl(dbConnection, pControlID, pLotNumber)

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " HistoryControlLotsDelegate.CloseLotDeleteControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a new Control/Lot in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryControlLotsDS">Typed DataSet HistoryControlLotsDS containing all data needed to create the 
        '''                                     Control/Lot in the history table of QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS containing data of the created Control/Lot
        '''          including the automatically generated QCControlLotID</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryControlLotsDS As HistoryControlLotsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryControlLotsDAO As New tqcHistoryControlLotsDAO
                        myGlobalDataTO = myHistoryControlLotsDAO.Create(dbConnection, pHistoryControlLotsDS)

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryControlLotsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For the informed Control, search if it exists in QC Module and in this case, get the identifier QCControlLotID.
        ''' Besides, search all Tests/SampleTypes linked to the Control and having information in QC Module, and get the 
        ''' identifier QCTestSample for each one of them 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with the identifier of the informed Control in
        '''          the history table in QC Module (QCControlLotID) and the identifier of each one of the linked Tests/SampleTypes in 
        '''          the history table in QC Module (QCTestSampleID)</returns>
        ''' <remarks>
        ''' Created by:  SA 23/05/2011
        ''' </remarks>
        Public Function GetLinkedTestsSampleTypesByControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistControlLotsDAO As New tqcHistoryControlLotsDAO
                        resultData = myHistControlLotsDAO.GetLinkedTestsSampleTypesByControl(dbConnection, pControlID)
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " HistoryControlLotsDelegate.GetLinkedTestsSampleTypesByControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of the specified Control/Lot in the history table in QC Module (field QCControlLotID)
        ''' Flags of ClosedLot and DeletedControl have to be FALSE, which mean it is the active Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pLotNumber">Lot Number; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of the Control/Lot in 
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' </remarks>
        Public Function GetQCControlLotIDByControlIDAndLotNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer, _
                                                                 ByVal pLotNumber As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryControlLotsDAO As New tqcHistoryControlLotsDAO
                        myGlobalDataTO = myHistoryControlLotsDAO.GetQCControlLotIDByControlIDAndLotNumber(dbConnection, pControlID, pLotNumber)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryControlLotsDelegate.GetQCControlLotIDByControlIDAndLotNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all data of the specified QCControlLotID in the history Control/Lots table in QC Module. 
        ''' Flags of ClosedLot and DeletedControl have to be FALSE, which mean it is the active Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCControlLotID">Identifier of the Control/Lot in QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryControlLotsDS with data of the Control/Lot in
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by: SA 02/01/2012
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCControlLotID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryControlLotsDAO As New tqcHistoryControlLotsDAO
                        myGlobalDataTO = myHistoryControlLotsDAO.Read(dbConnection, pQCControlLotID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryControlLotsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When basic data of a Control (Name and/or Sample Type) is changed in Controls Programming Screen,
        ''' values are updated for all not delete records the Control has in the history table of QC Module
        ''' Besides, if Min/Max Concentration values have been changed for Tests/Sample Types linked to the Control/Lot
        ''' they are updated also in the correspondent history table in QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlDS">Typed DataSet ControlDS containing data to update the Control basic
        '''                          information in the history table of QC Module</param>
        ''' <param name="pTestControlsDS">Typed DataSet TestControlsDS with the list of Tests/SampleTypes unlinked from the Control</param>
        ''' <param name="pDeletedTestControlsDS">Typed DataSet SelectedTestsDS with the list of Tests/SampleTypes unlinked from the Control</param>
        ''' <param name="pLotChanged">Flag indicating if the Lot used for the Control has been changed. When True, it indicates that is not
        '''                           needed to execute the accumulation of QC results for unlinked Tests/SampleTypes</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/05/2011
        ''' Modified by: SA 21/12/2011 - Once the open QC Results for the updated Control/Lot and the deleted Test/Sample Types have been cumulated
        '''                              and deleted, if the CalculationMode is STATISTIC, QC Results used to calculate the statistical values are
        '''                              also deleted
        '''              SA 05/06/2012 - Added parameter for AnalyzerID, and informed when calling functions SaveCumulatedResult in CumulatedResultsDelegate
        '''                              and DeleteStatisticResults in QCResultsDelegate. Informed parameter TestType when calling function  
        '''                              ReadByTestIDAndSampleType in HistoryTestSamplesDelegate
        ''' </remarks>
        Public Function UpdateControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS, _
                                         ByVal pTestControlsDS As TestControlsDS, ByVal pDeletedTestControlsDS As SelectedTestsDS, _
                                         ByVal pLotChanged As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update basic Control Data
                        Dim myHistoryControlLotsDAO As New tqcHistoryControlLotsDAO
                        myGlobalDataTO = myHistoryControlLotsDAO.UpdateControlData(dbConnection, pControlDS)

                        Dim myQCControlLotID As Integer = 0
                        If (Not myGlobalDataTO.HasError) Then
                            'Get the identifier of the ControlLot in the history table of QC Module
                            myGlobalDataTO = myHistoryControlLotsDAO.GetQCControlLotIDByControlIDAndLotNumber(dbConnection, pControlDS.tparControls(0).ControlID, _
                                                                                                              pControlDS.tparControls(0).LotNumber)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myHistoryControlLotDS As HistoryControlLotsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryControlLotsDS)
                                If (myHistoryControlLotDS.tqcHistoryControlLots.Rows.Count = 1) Then
                                    myQCControlLotID = myHistoryControlLotDS.tqcHistoryControlLots(0).QCControlLotID

                                    Dim myHistoryTestSampleDS As HistoryTestSamplesDS
                                    Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
                                    Dim myHistoryTestControlLotsDelegate As New HistoryTestControlLotsDelegate

                                    'Update Min/Max Concentration values for the Tests/Sample Types linked to the Control
                                    For Each testControlRow As TestControlsDS.tparTestControlsRow In pTestControlsDS.tparTestControls.Rows
                                        'Get the identifier of the Test/SampleType in the history table of QC Module
                                        myGlobalDataTO = myHistoryTestSamplesDelegate.ReadByTestIDAndSampleTypeNEW(dbConnection, testControlRow.TestType, _
                                                                                                                   testControlRow.TestID, testControlRow.SampleType)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        myHistoryTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                                        If (myHistoryTestSampleDS.tqcHistoryTestSamples.Rows.Count = 1) Then
                                            'Once the two identifiers have been found, update Min/Max Concentration values 
                                            myGlobalDataTO = myHistoryTestControlLotsDelegate.Update(dbConnection, myHistoryTestSampleDS.tqcHistoryTestSamples(0).QCTestSampleID, _
                                                                                                     myQCControlLotID, testControlRow.MinConcentration, testControlRow.MaxConcentration)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    Next

                                    If (Not myGlobalDataTO.HasError) AndAlso (Not pLotChanged) Then
                                        'Search if there are QC Results pending to accumulate for the Control and each unlinked Tests/SampleTypes
                                        For Each deletedTC As SelectedTestsDS.SelectedTestTableRow In pDeletedTestControlsDS.SelectedTestTable.Rows
                                            'Get the identifier of the Test/SampleType in the history table of QC Module
                                            myGlobalDataTO = myHistoryTestSamplesDelegate.ReadByTestIDAndSampleTypeNEW(dbConnection, deletedTC.TestType, deletedTC.TestID, _
                                                                                                                       deletedTC.SampleType)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            myHistoryTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                                            If (myHistoryTestSampleDS.tqcHistoryTestSamples.Rows.Count = 1) Then
                                                'Cumulated all pending QC Results for the Control/Lot and Test/SampleType
                                                Dim myCumResultsDelegate As New CumulatedResultsDelegate
                                                myGlobalDataTO = myCumResultsDelegate.SaveCumulateResultNEW(dbConnection, myHistoryTestSampleDS.tqcHistoryTestSamples(0).QCTestSampleID, _
                                                                                                            myQCControlLotID)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                If (myHistoryTestSampleDS.tqcHistoryTestSamples(0).CalculationMode = "STATISTIC") Then
                                                    'Delete the group of QC Results used to calculate the statistic values for the Test/SampleType and Control/Lot
                                                    Dim myQCResultsDelegate As New QCResultsDelegate
                                                    myGlobalDataTO = myQCResultsDelegate.DeleteStatisticResultsNEW(dbConnection, myHistoryTestSampleDS.tqcHistoryTestSamples(0).QCTestSampleID, _
                                                                                                                   myQCControlLotID)
                                                    If (myGlobalDataTO.HasError) Then Exit For
                                                End If
                                            End If
                                        Next
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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " HistoryControlLotsDelegate.UpdateControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' When basic data of a Control (Name and/or Sample Type) is changed in Controls Programming Screen,
        '''' values are updated for all not delete records the Control has in the history table of QC Module
        '''' Besides, if Min/Max Concentration values have been changed for Tests/Sample Types linked to the Control/Lot
        '''' they are updated also in the correspondent history table in QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pControlDS">Typed DataSet ControlDS containing data to update the Control basic
        ''''                          information in the history table of QC Module</param>
        '''' <param name="pTestControlsDS">Typed DataSet TestControlsDS with the list of Tests/SampleTypes unlinked from the Control</param>
        '''' <param name="pDeletedTestControlsDS">Typed DataSet SelectedTestsDS with the list of Tests/SampleTypes unlinked from the Control</param>
        '''' <param name="pLotChanged">Flag indicating if the Lot used for the Control has been changed. When True, it indicates that is not
        ''''                           needed to execute the accumulation of QC results for unlinked Tests/SampleTypes</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 20/05/2011
        '''' Modified by: SA 21/12/2011 - Once the open QC Results for the updated Control/Lot and the deleted Test/Sample Types have been cumulated
        ''''                              and deleted, if the CalculationMode is STATISTIC, QC Results used to calculate the statistical values are
        ''''                              also deleted
        '''' </remarks>
        'Public Function UpdateControlOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlDS As ControlsDS, ByVal pTestControlsDS As TestControlsDS, _
        '                              ByVal pDeletedTestControlsDS As SelectedTestsDS, ByVal pLotChanged As Boolean) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Update basic Control Data
        '                Dim myHistoryControlLotsDAO As New tqcHistoryControlLotsDAO
        '                myGlobalDataTO = myHistoryControlLotsDAO.UpdateControlData(dbConnection, pControlDS)

        '                Dim myQCControlLotID As Integer = 0
        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Get the identifier of the ControlLot in the history table of QC Module
        '                    myGlobalDataTO = myHistoryControlLotsDAO.GetQCControlLotIDByControlIDAndLotNumber(dbConnection, pControlDS.tparControls(0).ControlID, _
        '                                                                                                      pControlDS.tparControls(0).LotNumber)

        '                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                        Dim myHistoryControlLotDS As HistoryControlLotsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryControlLotsDS)
        '                        If (myHistoryControlLotDS.tqcHistoryControlLots.Rows.Count = 1) Then
        '                            myQCControlLotID = myHistoryControlLotDS.tqcHistoryControlLots(0).QCControlLotID

        '                            Dim myHistoryTestSampleDS As HistoryTestSamplesDS
        '                            Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
        '                            Dim myHistoryTestControlLotsDelegate As New HistoryTestControlLotsDelegate

        '                            'Update Min/Max Concentration values for the Tests/Sample Types linked to the Control
        '                            For Each testControlRow As TestControlsDS.tparTestControlsRow In pTestControlsDS.tparTestControls.Rows
        '                                'Get the identifier of the Test/SampleType in the history table of QC Module
        '                                myGlobalDataTO = myHistoryTestSamplesDelegate.ReadByTestIDAndSampleTypeOLD(dbConnection, testControlRow.TestID, _
        '                                                                                                        testControlRow.SampleType)
        '                                If (myGlobalDataTO.HasError) Then Exit For

        '                                myHistoryTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
        '                                If (myHistoryTestSampleDS.tqcHistoryTestSamples.Rows.Count = 1) Then
        '                                    'Once the two identifiers have been found, update Min/Max Concentration values 
        '                                    myGlobalDataTO = myHistoryTestControlLotsDelegate.Update(dbConnection, myHistoryTestSampleDS.tqcHistoryTestSamples(0).QCTestSampleID, _
        '                                                                                             myQCControlLotID, testControlRow.MinConcentration, testControlRow.MaxConcentration)
        '                                    If (myGlobalDataTO.HasError) Then Exit For
        '                                End If
        '                            Next

        '                            If (Not myGlobalDataTO.HasError) AndAlso (Not pLotChanged) Then
        '                                'Search if there are QC Results pending to accumulate for the Control and each unlinked Tests/SampleTypes
        '                                For Each deletedTC As SelectedTestsDS.SelectedTestTableRow In pDeletedTestControlsDS.SelectedTestTable.Rows
        '                                    'Get the identifier of the Test/SampleType in the history table of QC Module
        '                                    myGlobalDataTO = myHistoryTestSamplesDelegate.ReadByTestIDAndSampleTypeOLD(dbConnection, deletedTC.TestID, deletedTC.SampleType)
        '                                    If (myGlobalDataTO.HasError) Then Exit For

        '                                    myHistoryTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
        '                                    If (myHistoryTestSampleDS.tqcHistoryTestSamples.Rows.Count = 1) Then
        '                                        'Cumulated all pending QC Results for the Control/Lot and Test/SampleType
        '                                        Dim myCumResultsDelegate As New CumulatedResultsDelegate
        '                                        myGlobalDataTO = myCumResultsDelegate.SaveCumulateResult(dbConnection, myHistoryTestSampleDS.tqcHistoryTestSamples(0).QCTestSampleID, _
        '                                                                                                 myQCControlLotID)
        '                                        If (myGlobalDataTO.HasError) Then Exit For

        '                                        If (myHistoryTestSampleDS.tqcHistoryTestSamples(0).CalculationMode = "STATISTIC") Then
        '                                            'Delete the group of QC Results used to calculate the statistic values for the Test/SampleType and Control/Lot
        '                                            Dim myQCResultsDelegate As New QCResultsDelegate
        '                                            myGlobalDataTO = myQCResultsDelegate.DeleteStatisticResults(dbConnection, myHistoryTestSampleDS.tqcHistoryTestSamples(0).QCTestSampleID, _
        '                                                                                                        myQCControlLotID)
        '                                            If (myGlobalDataTO.HasError) Then Exit For
        '                                        End If
        '                                    End If
        '                                Next
        '                            End If
        '                        End If
        '                    End If
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

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " HistoryControlLotsDelegate.UpdateControl", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function        
#End Region
    End Class
End Namespace


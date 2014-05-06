Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class HistoryTestSamplesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create a new TestType/TestID/SampleType in the history table of QC Module 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing all data needed to create the 
        '''                                     TestType/TestID/SampleType in the history table of QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS containing data of the created TestType/TestID/SampleType
        '''          including the automatically generated QCTestSampleID</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.CreateNEW(dbConnection, pHistoryTestSamplesDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When a SampleType is deleted for a Test or when a Test is deleted, the SampleType is marked as Deleted in 
        ''' the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 11/05/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function MarkSampleTypeValuesAsDeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                        ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.MarkSampleTypeValuesAsDeleteNEW(dbConnection, pTestType, pTestID, pSampleType)

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
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.MarkSampleTypeValuesAsDelete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When a Test is deleted, the Test and all its SampleTypes are marked as Deleted in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 11/05/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function MarkTestValuesAsDeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.MarkTestValuesAsDeleteNEW(dbConnection, pTestType, pTestID)

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
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.MarkTestValuesAsDelete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all data of the specified QCTestSampleID in the history Test/SampleTypes table in QC Module
        ''' Flags of DeletedSampleType and DeletedTest have to be FALSE, which mean it is the active Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with data of the Test/SampleType in
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by: TR 27/05/2011
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.Read(dbConnection, pQCTestSampleID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all TestTypes/TestIDs/SampleTypes in history table in QC Module or optionally, only those that are currently active
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOnlyActive">Optional parameter. When True, only TestTypes/TestIDs/SampleTypes not marked as deleted and having
        '''                           QC Results pending to cumulate are returned. When False, only TestTypes/TestIDs/SampleTypes having
        '''                           Cumulated QC Results are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with all the Tests/SampleTypes</returns>
        ''' <remarks>
        ''' Created by:  TR 26/05/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for AnalyzerID 
        ''' </remarks>
        Public Function ReadAllNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, Optional ByVal pOnlyActive As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.ReadAllNEW(dbConnection, pAnalyzerID, pOnlyActive)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all data of the specified Test/SampleType in the history table in QC Module (field QCTestSampleID)
        ''' Flags of DeletedSampleType and DeletedTest have to be FALSE, which mean it is the active Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code: STD or ISE</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with data of the Test/SampleType in
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 21/05/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function ReadByTestIDAndSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.ReadByTestIDAndSampleTypeNEW(dbConnection, pTestType, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSampleDelegate.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When basic data of a Test (Name, ShortName, MeasureUnit, and/or DecimalsAllowed) is changed in Tests Programming Screen,
        ''' values are updated for all not delete records the Test has in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test
        '''                                     to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 11/05/2011
        ''' </remarks>
        Public Function UpdateByTestIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.UpdateByTestIDNEW(dbConnection, pHistoryTestSamplesDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.UpdateByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When basic data of a Test/SampleType (RejectionCriteria, CalculationMode, and/or NumberOfSeries) is changed in Tests Programming Screen,
        ''' values are updated for all not delete records the Test/SampleType has in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test/SampleType to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 11/05/2011
        ''' </remarks>
        Public Function UpdateByQCTestIdAndSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
                        myGlobalDataTO = myHistoryTestSamplesDAO.UpdateByTestIDAndSampleTypeNEW(dbConnection, pHistoryTestSamplesDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.UpdateByQCTestIdAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Create a new Test/SampleType in the history table of QC Module 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing all data needed to create the 
        ''''                                     Test/SampleType in the history table of QC Module</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS containing data of the created Test/SampleType
        ''''          including the automatically generated QCTestSampleID</returns>
        '''' <remarks>
        '''' Created by:  TR 13/05/2011
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
        '                myGlobalDataTO = myHistoryTestSamplesDAO.CreateOLD(dbConnection, pHistoryTestSamplesDS)

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.Create", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' When a SampleType is deleted for a Test or when a Test is deleted, the SampleType is marked as Deleted in 
        '''' the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 11/05/2011
        '''' </remarks>
        'Public Function MarkSampleTypeValuesAsDeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
        '                myGlobalDataTO = myHistoryTestSamplesDAO.MarkSampleTypeValuesAsDeleteOLD(dbConnection, pTestID, pSampleType)

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.MarkSampleTypeValuesAsDelete", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' When a Test is deleted, the Test and all its SampleTypes are marked as Deleted in the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 11/05/2011
        '''' </remarks>
        'Public Function MarkTestValuesAsDeleteOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
        '                myGlobalDataTO = myHistoryTestSamplesDAO.MarkTestValuesAsDelete(dbConnection, pTestID)

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.MarkTestValuesAsDelete", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all Tests/SampleTypes in history table in QC Module or optionally, only those that are currently active
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pOnlyActive">Optional parameter. When True, only Tests/SampleTypes not marked as deleted are returned</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with all the Tests/SampleTypes</returns>
        '''' <remarks>
        '''' Created by:  TR 26/05/2011
        '''' </remarks>
        'Public Function ReadAllOLD(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pOnlyActive As Boolean = False) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
        '                myGlobalDataTO = myHistoryTestSamplesDAO.ReadAllOLD(dbConnection, pOnlyActive)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.ReadAll", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all data of the specified Test/SampleType in the history table in QC Module (field QCTestSampleID)
        '''' Flags of DeletedSampleType and DeletedTest have to be FALSE, which mean it is the active Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with data of the Test/SampleType in
        ''''          the history table in QC Module</returns>
        '''' <remarks>
        '''' Created by: TR 10/05/2011
        '''' </remarks>
        'Public Function ReadByTestIDAndSampleTypeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
        '                myGlobalDataTO = myHistoryTestSamplesDAO.ReadByTestIDAndSampleTypeOLD(dbConnection, pTestID, pSampleType)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSampleDelegate.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' When basic data of a Test (Name, ShortName, MeasureUnit, and/or DecimalsAllowed) is changed in Tests Programming Screen,
        '''' values are updated for all not delete records the Test has in the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test
        ''''                                     to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 11/05/2011
        '''' </remarks>
        'Public Function UpdateByTestIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
        '                myGlobalDataTO = myHistoryTestSamplesDAO.UpdateByTestIDOLD(dbConnection, pHistoryTestSamplesDS)

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.UpdateByTestID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' When basic data of a Test/SampleType (RejectionCriteria, CalculationMode, and/or NumberOfSeries) is changed in Tests Programming Screen,
        '''' values are updated for all not delete records the Test/SampleType has in the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test/SampleType to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 11/05/2011
        '''' </remarks>
        'Public Function UpdateByQCTestIdAndSampleIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHistoryTestSamplesDAO As New tqcHistoryTestSamplesDAO
        '                myGlobalDataTO = myHistoryTestSamplesDAO.UpdateByTestIDAndSampleTypeOLD(dbConnection, pHistoryTestSamplesDS)

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "HistoryTestSamplesDelegate.UpdateByQCTestIdAndSampleID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region

    End Class
End Namespace

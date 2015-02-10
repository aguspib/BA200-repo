Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL
    Public Class HistoryTestControlLotsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create the link between the specified QCTestSampleID and QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestControlLotDS">Typed DataSet HistoryTestControlLotsDS containing the information needed to 
        '''                                        create the link between the specified QCTestSampleID and QCControlLotID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' Modified by: SA 02/06/2011 - Use function ReplaceNumericString insteat of ToSQLString to format decimal values to avoid loss precision
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestControlLotDS As HistoryTestControlLotsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestControlLotsDAO As New tqcHistoryTestControlLotsDAO
                        myGlobalDataTO = myHistoryTestControlLotsDAO.Create(dbConnection, pHistoryTestControlLotDS)

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
                GlobalBase.CreateLogActivity(ex.Message, "HistoryTestControlLotsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get values of all Control/Lots linked to the specified Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with all values defined for 
        '''          all Control/Lots linked to the specified Test/SampleType</returns>
        ''' <remarks>
        ''' Created by:  TR 27/05/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function GetAllControlsLinkedToTestSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                                ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestControlLotsDAO As New tqcHistoryTestControlLotsDAO
                        myGlobalDataTO = myHistoryTestControlLotsDAO.GetAllControlsLinkedToTestSampleTypeNEW(dbConnection, pQCTestSampleID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryTestControlLotsDelegate.GetAllControlsLinkedToTestSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the identifiers in history tables of QC Module for the specified TestType/TestID/SampleType and Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <param name="pLotNumber">Lot Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with the identifiers of the
        '''          Test/SampleType and the Control/Lot in QC Module</returns>
        ''' <remarks>
        ''' Created by:  SA 17/06/2011
        ''' Modified by: SA 05/06/2012 - Added parameter for TestType
        ''' </remarks>
        Public Function GetQCIDsForTestAndControlNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String, ByVal pControlID As Integer, ByVal pLotNumber As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestControlLotsDAO As New tqcHistoryTestControlLotsDAO
                        resultData = myHistoryTestControlLotsDAO.GetQCIDsForTestAndControlNEW(dbConnection, pTestType, pTestID, pSampleType, pControlID, pLotNumber)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryTestControlLotsDelegate.GetQCIDsForTestAndControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all values of an existing link between an specific QCTestSampleID and QCControlLotID (Min/Max Concentration) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestControlLotsDS with all Test Control values</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' </remarks>
        Public Function ReadByTestSampleIDAndControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                       ByVal pQCControlLotID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestControlLotsDAO As New tqcHistoryTestControlLotsDAO
                        myGlobalDataTO = myHistoryTestControlLotsDAO.Read(dbConnection, pQCTestSampleID, pQCControlLotID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryTestControlLotsDelegate.ReadByTestSampleIDAndControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update Min/Max Concentration values for the specified QCTestSampleID and QCControlLotID in table tqcHistoryTestControlLots
        ''' in QC Module, and for the correspondent TestType/TestID/SampleType in table tparTestControls. Besides, update CalculationMode to MANUAL
        ''' for the specified QCTestSampleID in table tqcHistoryTestSamples in QC Module, and for the correspondent TestType/TestID/SampleType in
        ''' table tparTestSamples or tparISETestSamples
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pMinValue">Min concentration value</param>
        ''' <param name="pMaxValue">Max concentration value</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 02/01/2012
        ''' Modified by: SA 06/06/2012 - Added parameter for AnalyzerID
        ''' </remarks>
        Public Function SaveLastCumulatedAsTargetNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                     ByVal pQCControlLotID As Integer, ByVal pAnalyzerID As String, ByVal pMinValue As Single, _
                                                     ByVal pMaxValue As Single) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim dataOK As Boolean = False

                        'Update CalculationMode = MANUAL and NumberOfSeries = 0 for the Test/SampleType in tables tqcHistoryTestSamples and tparTestSamples
                        Dim myQCResultsDelegate As New QCResultsDelegate
                        myGlobalDataTO = myQCResultsDelegate.UpdateChangedValuesNEW(dbConnection, pQCTestSampleID, pAnalyzerID, "MANUAL", 0, 0, _
                                                                                    String.Empty, String.Empty, Nothing)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myHistoryTestSamplesDS As HistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)

                            If (myHistoryTestSamplesDS.tqcHistoryTestSamples.Count = 1) Then
                                Dim myTestControlsDS As New TestControlsDS

                                Dim myTestControlRow As TestControlsDS.tparTestControlsRow = myTestControlsDS.tparTestControls.NewtparTestControlsRow()
                                myTestControlRow.TestType = myHistoryTestSamplesDS.tqcHistoryTestSamples.First.TestType
                                myTestControlRow.TestID = myHistoryTestSamplesDS.tqcHistoryTestSamples.First.TestID
                                myTestControlRow.SampleType = myHistoryTestSamplesDS.tqcHistoryTestSamples.First.SampleType

                                myTestControlRow.TargetMean = (pMinValue + pMaxValue) / 2
                                myTestControlRow.TargetSD = (pMaxValue - pMinValue) / (2 * myHistoryTestSamplesDS.tqcHistoryTestSamples.First.RejectionCriteria)
                                myTestControlRow.MinConcentration = pMinValue
                                myTestControlRow.MaxConcentration = pMaxValue
                                myTestControlRow.SetActiveControlNull() 'TR SET Null Values To avoid override.

                                'Get the ControlLot and LotNumber for the specified QCControlLotID
                                Dim myHistoryControlLotDelegate As New HistoryControlLotsDelegate

                                myGlobalDataTO = myHistoryControlLotDelegate.Read(dbConnection, pQCControlLotID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myHistoryControlLotsDS As HistoryControlLotsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryControlLotsDS)

                                    If (myHistoryControlLotsDS.tqcHistoryControlLots.Count = 1) Then
                                        'Inform found values in the DS row 
                                        myTestControlRow.ControlID = myHistoryControlLotsDS.tqcHistoryControlLots.First.ControlID
                                        myTestControlRow.LotNumber = myHistoryControlLotsDS.tqcHistoryControlLots.First.LotNumber

                                        'Finally add the row to the local TestControlsDS and update values in table tparTestControls
                                        myTestControlsDS.tparTestControls.AddtparTestControlsRow(myTestControlRow)

                                        'Update Min/Max values in table tparTestControls 
                                        Dim myTestControlsDelegate As New TestControlsDelegate
                                        myGlobalDataTO = myTestControlsDelegate.UpdateTestControlsNEW(dbConnection, myTestControlsDS)

                                        If (Not myGlobalDataTO.HasError) Then
                                            'Update Min/Max values in table tqcHistoryTestControlLots in QC Module
                                            myGlobalDataTO = Update(dbConnection, pQCTestSampleID, pQCControlLotID, pMinValue, pMaxValue)
                                            If (Not myGlobalDataTO.HasError) Then dataOK = True
                                        End If
                                    End If
                                End If
                            End If
                            myGlobalDataTO.HasError = (Not dataOK)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryTestControlLotsDelegate.SaveLastCumulatedAsTarget", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update Min/Max Concentration values for an specific QCTestSampleID and QCControlLotID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC</param>
        ''' <param name="pMinValue">Min concentration value</param>
        ''' <param name="pMaxValue">Max concentration value</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 02/06/2011 - Use function ReplaceNumericString insteat of ToSQLString to format decimal values to avoid loss precision
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, _
                               ByVal pMinValue As Single, ByVal pMaxValue As Single) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestControlLotsDAO As New tqcHistoryTestControlLotsDAO
                        myGlobalDataTO = myHistoryTestControlLotsDAO.Update(dbConnection, pQCTestSampleID, pQCControlLotID, pMinValue, pMaxValue)

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
                GlobalBase.CreateLogActivity(ex.Message, "HistoryTestControlLotsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update field WestgardControNum for each Control/Lot linked to the specified Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
        ''' <param name="pQCControlLotIDForWESG1">QC Identifier for the first Control/Lot selected to apply Multirules</param>
        ''' <param name="pQCControlLotIDForWESG2">QC Identifier for the second Control/Lot selected to apply Multirules
        '''                                       Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 30/05/2011
        ''' Modified by: SA 25/01/2012 - Parameter for second Control/Lot changed to optional
        ''' </remarks>
        Public Function UpdateWestgardControlNum(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer, _
                                                 ByVal pQCControlLotIDForWESG1 As String, Optional ByVal pQCControlLotIDForWESG2 As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistoryTestControlLotsDAO As New tqcHistoryTestControlLotsDAO
                        myGlobalDataTO = myHistoryTestControlLotsDAO.UpdateWestgardControlNum(dbConnection, pQCTestSampleID, pQCControlLotIDForWESG1, pQCControlLotIDForWESG2)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HistoryTestControlLotsDelegate.UpdateWestgardControlNum", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

    End Class
End Namespace



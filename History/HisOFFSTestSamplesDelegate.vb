Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisOFFSTestSamplesDelegate

#Region "Public Functions"
        ''' <summary>
        ''' Receive a list of OFF_SYSTEM Test Identifier/Sample Type and for each one of them, verify if it already exists in Historics Module and when it does not
        ''' exists, get the needed data and create the new OFFSTestID/Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisOFFSTestsDS">Typed DataSet HisOFFSTestSamplesDS containing the list of OffSystem TestID/SampleTypes to verify if they already exist 
        '''                               in Historics Module and create them when not</param>
        ''' <returns>GlobalDataTO containing a typed Dataset HisOFFSTestSamplesDS with the identifier in Historics Module for each OFFSTestID/Sample Type in it</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' </remarks>
        Public Function CheckOFFSTestSamplesInHistorics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisOFFSTestsDS As HisOFFSTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOFFSTestDataDS As OffSystemTestsDS
                        Dim myOFFSTestsDelegate As New OffSystemTestsDelegate

                        Dim auxiliaryDS As HisOFFSTestSamplesDS
                        Dim offsTestsToAddDS As New HisOFFSTestSamplesDS
                        Dim myHisOFFSTestsDAO As New thisOffSystemTestSamplesDAO

                        For Each offsTestSampleRow As HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow In pHisOFFSTestsDS.thisOffSystemTestSamples
                            myGlobalDataTO = myHisOFFSTestsDAO.ReadByOFFSTestIDAndSampleType(dbConnection, offsTestSampleRow.OffSystemTestID, offsTestSampleRow.SampleType)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                auxiliaryDS = DirectCast(myGlobalDataTO.SetDatos, HisOFFSTestSamplesDS)

                                If (auxiliaryDS.thisOffSystemTestSamples.Rows.Count = 0) Then
                                    'New OFFSTest/SampleType; get basic data from tables tparOffSystemTests and tparOffSystemTestSamples
                                    myGlobalDataTO = myOFFSTestsDelegate.GetByTestIDAndSampleType(dbConnection, offsTestSampleRow.OffSystemTestID, offsTestSampleRow.SampleType)

                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myOFFSTestDataDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                                        If (myOFFSTestDataDS.tparOffSystemTests.Rows.Count > 0) Then
                                            offsTestSampleRow.BeginEdit()
                                            offsTestSampleRow.OffSystemTestName = myOFFSTestDataDS.tparOffSystemTests.First.Name
                                            offsTestSampleRow.ResultType = myOFFSTestDataDS.tparOffSystemTests.First.ResultType
                                            offsTestSampleRow.MeasureUnit = myOFFSTestDataDS.tparOffSystemTests.First.Units
                                            offsTestSampleRow.DecimalsAllowed = myOFFSTestDataDS.tparOffSystemTests.First.Decimals
                                            offsTestSampleRow.EndEdit()
                                        End If
                                    Else
                                        'Error getting data of the OFFS Test/SampleType
                                        Exit For
                                    End If

                                    'Copy all OFFS Test/SampleType data to the auxiliary DS of elements to add
                                    offsTestsToAddDS.thisOffSystemTestSamples.ImportRow(offsTestSampleRow)
                                Else
                                    'The OFFS Test/SampleType already exists in Historics Module; inform all fields in the DS
                                    offsTestSampleRow.BeginEdit()
                                    offsTestSampleRow.HistOffSystemTestID = auxiliaryDS.thisOffSystemTestSamples.First.HistOffSystemTestID
                                    offsTestSampleRow.OffSystemTestName = auxiliaryDS.thisOffSystemTestSamples.First.OffSystemTestName
                                    offsTestSampleRow.ResultType = auxiliaryDS.thisOffSystemTestSamples.First.ResultType
                                    offsTestSampleRow.MeasureUnit = auxiliaryDS.thisOffSystemTestSamples.First.MeasureUnit
                                    offsTestSampleRow.DecimalsAllowed = auxiliaryDS.thisOffSystemTestSamples.First.DecimalsAllowed
                                    offsTestSampleRow.EndEdit()
                                End If
                            Else
                                'Error verifying if OFFS Test/SampleType already exists in Historics Module
                                Exit For
                            End If
                        Next

                        'Add to Historics Module all new OFFS Test/SampleTypes
                        If (Not myGlobalDataTO.HasError AndAlso offsTestsToAddDS.thisOffSystemTestSamples.Rows.Count > 0) Then
                            myGlobalDataTO = myHisOFFSTestsDAO.Create(dbConnection, offsTestsToAddDS)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                offsTestsToAddDS = DirectCast(myGlobalDataTO.SetDatos, HisOFFSTestSamplesDS)

                                'Search the added OFFS Test/SampleType in the entry DS and inform the OFFTest/Sample Identifier in Historics Module
                                Dim lstOFFSTestToUpdate As List(Of HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow)
                                For Each offsTestSampleRow As HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow In offsTestsToAddDS.thisOffSystemTestSamples
                                    lstOFFSTestToUpdate = (From a As HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow In pHisOFFSTestsDS.thisOffSystemTestSamples _
                                                         Where a.OffSystemTestID = offsTestSampleRow.OffSystemTestID _
                                                       AndAlso a.SampleType = offsTestSampleRow.SampleType _
                                                       AndAlso a.IsHistOffSystemTestIDNull).ToList

                                    If (lstOFFSTestToUpdate.Count = 1) Then
                                        lstOFFSTestToUpdate.First.BeginEdit()
                                        lstOFFSTestToUpdate.First.HistOffSystemTestID = offsTestSampleRow.HistOffSystemTestID
                                        lstOFFSTestToUpdate.First.EndEdit()
                                    End If
                                Next
                                lstOFFSTestToUpdate = Nothing
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = pHisOFFSTestsDS
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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisOFFSTestSamplesDelegate.CheckOFFSTestSamplesInHistorics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all not in use closed Off System Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisOffSystemTestSamplesDAO
                        resultData = myDAO.DeleteClosedNotInUse(dbConnection)

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
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisOFFSTestSamplesDelegate.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class
End Namespace
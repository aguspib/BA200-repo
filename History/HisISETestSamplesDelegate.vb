Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisISETestSamplesDelegate

#Region "Public Functions"

        ''' <summary>
        ''' Receive a list of ISE Test Identifier/Sample Type and for each one of them, verify if it already exists in Historics Module and when it does not
        ''' exists, get the needed data and create the new ISETestID/Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisISETestsDS">Typed DataSet HisISETestSamplesDS containing the list of ISETestID/SampleTypes to verify if they already exist in Historics
        '''                              Module and create them when not</param>
        ''' <returns>GlobalDataTO containing a typed Dataset HisISETestSamplesDS with the identifier in Historics Module for each ISETestID/Sample Type in it</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' </remarks>
        Public Function CheckISETestSamplesInHistorics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisISETestsDS As HisISETestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISETestDataDS As ISETestsDS
                        Dim myISETestsDelegate As New ISETestsDelegate

                        Dim auxiliaryDS As HisISETestSamplesDS
                        Dim iseTestsToAddDS As New HisISETestSamplesDS
                        Dim myHisISETestsDAO As New thisISETestSamplesDAO

                        For Each iseTestSampleRow As HisISETestSamplesDS.thisISETestSamplesRow In pHisISETestsDS.thisISETestSamples
                            myGlobalDataTO = myHisISETestsDAO.ReadByISETestIDAndSampleType(dbConnection, iseTestSampleRow.ISETestID, iseTestSampleRow.SampleType)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                auxiliaryDS = DirectCast(myGlobalDataTO.SetDatos, HisISETestSamplesDS)

                                If (auxiliaryDS.thisISETestSamples.Rows.Count = 0) Then
                                    'New ISETest/SampleType; get basic data from tables tparISETests and tparISETestSamples
                                    myGlobalDataTO = myISETestsDelegate.GetByTestIDAndSampleType(dbConnection, iseTestSampleRow.ISETestID, iseTestSampleRow.SampleType)

                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myISETestDataDS = DirectCast(myGlobalDataTO.SetDatos, ISETestsDS)

                                        If (myISETestDataDS.tparISETests.Rows.Count > 0) Then
                                            iseTestSampleRow.BeginEdit()
                                            iseTestSampleRow.ISETestName = myISETestDataDS.tparISETests.First.Name
                                            iseTestSampleRow.MeasureUnit = myISETestDataDS.tparISETests.First.Units
                                            iseTestSampleRow.DecimalsAllowed = myISETestDataDS.tparISETests.First.Decimals
                                            iseTestSampleRow.TestLongName = myISETestDataDS.tparISETests.First.TestLongName   ' WE 30/07/2014 - #1865

                                            If (Not myISETestDataDS.tparISETests.First.IsSlopeFactorA2Null) Then    ' WE 25/08/2014 - #1865
                                                iseTestSampleRow.SlopeFactorA2 = myISETestDataDS.tparISETests.First.SlopeFactorA2
                                            End If
                                            If (Not myISETestDataDS.tparISETests.First.IsSlopeFactorB2Null) Then    ' WE 25/08/2014 - #1865
                                                iseTestSampleRow.SlopeFactorB2 = myISETestDataDS.tparISETests.First.SlopeFactorB2
                                            End If

                                            iseTestSampleRow.EndEdit()
                                        End If
                                    Else
                                        'Error getting data of the ISETest/SampleType
                                        Exit For
                                    End If

                                    'Copy all ISETest/SampleType data to the auxiliary DS of elements to add
                                    iseTestsToAddDS.thisISETestSamples.ImportRow(iseTestSampleRow)
                                Else
                                    'The ISETest/SampleType already exists in Historics Module; inform all fields in the DS
                                    iseTestSampleRow.BeginEdit()
                                    iseTestSampleRow.HistISETestID = auxiliaryDS.thisISETestSamples.First.HistISETestID()
                                    iseTestSampleRow.ISETestName = auxiliaryDS.thisISETestSamples.First.ISETestName
                                    iseTestSampleRow.MeasureUnit = auxiliaryDS.thisISETestSamples.First.MeasureUnit
                                    iseTestSampleRow.DecimalsAllowed = auxiliaryDS.thisISETestSamples.First.DecimalsAllowed
                                    iseTestSampleRow.TestLongName = auxiliaryDS.thisISETestSamples.First.TestLongName   ' WE 30/07/2014 - #1865

                                    If (Not auxiliaryDS.thisISETestSamples.First.IsSlopeFactorA2Null) Then  ' WE 25/08/2014 - #1865
                                        iseTestSampleRow.SlopeFactorA2 = auxiliaryDS.thisISETestSamples.First.SlopeFactorA2
                                    End If
                                    If (Not auxiliaryDS.thisISETestSamples.First.IsSlopeFactorB2Null) Then  ' WE 25/08/2014 - #1865
                                        iseTestSampleRow.SlopeFactorB2 = auxiliaryDS.thisISETestSamples.First.SlopeFactorB2
                                    End If

                                    iseTestSampleRow.EndEdit()
                                End If
                            Else
                                'Error verifying if ISETest/SampleType already exists in Historics Module
                                Exit For
                            End If
                        Next

                        'Add to Historics Module all new ISETest/SampleTypes
                        If (Not myGlobalDataTO.HasError AndAlso iseTestsToAddDS.thisISETestSamples.Rows.Count > 0) Then
                            myGlobalDataTO = myHisISETestsDAO.Create(dbConnection, iseTestsToAddDS)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                iseTestsToAddDS = DirectCast(myGlobalDataTO.SetDatos, HisISETestSamplesDS)

                                'Search the added ISETest/SampleType in the entry DS and inform the ISETest/Sample Identifier in Historics Module
                                Dim lstISETestToUpdate As List(Of HisISETestSamplesDS.thisISETestSamplesRow)
                                For Each iseTestSampleRow As HisISETestSamplesDS.thisISETestSamplesRow In iseTestsToAddDS.thisISETestSamples
                                    lstISETestToUpdate = (From a As HisISETestSamplesDS.thisISETestSamplesRow In pHisISETestsDS.thisISETestSamples _
                                                         Where a.ISETestID = iseTestSampleRow.ISETestID _
                                                       AndAlso a.SampleType = iseTestSampleRow.SampleType _
                                                       AndAlso a.IsHistISETestIDNull).ToList

                                    If (lstISETestToUpdate.Count = 1) Then
                                        lstISETestToUpdate.First.BeginEdit()
                                        lstISETestToUpdate.First.HistISETestID = iseTestSampleRow.HistISETestID
                                        lstISETestToUpdate.First.ISETestName = iseTestSampleRow.ISETestName
                                        lstISETestToUpdate.First.MeasureUnit = iseTestSampleRow.MeasureUnit
                                        lstISETestToUpdate.First.DecimalsAllowed = iseTestSampleRow.DecimalsAllowed
                                        lstISETestToUpdate.First.TestLongName = iseTestSampleRow.TestLongName   ' WE 30/07/2014 - #1865

                                        If (Not iseTestSampleRow.IsSlopeFactorA2Null) Then
                                            lstISETestToUpdate.First.SlopeFactorA2 = iseTestSampleRow.SlopeFactorA2 ' WE 25/08/2014 - #1865
                                        End If
                                        If (Not iseTestSampleRow.IsSlopeFactorB2Null) Then
                                            lstISETestToUpdate.First.SlopeFactorB2 = iseTestSampleRow.SlopeFactorB2 ' WE 25/08/2014 - #1865
                                        End If

                                        lstISETestToUpdate.First.EndEdit()
                                    End If
                                Next
                            End If
                        End If


                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            myGlobalDataTO.SetDatos = pHisISETestsDS
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisISETestSamplesDelegate.CheckISETestSamplesInHistorics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region
    End Class
End Namespace

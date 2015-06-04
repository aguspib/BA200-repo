Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL
    Partial Public Class WorkSessionsDelegate

#Region "Declaration"
        Public TotalSecElapsedTime As Integer = 0
        Private lockThis As New Object
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
        ''' <returns>ExecutionsDS</returns>
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
        ''' Add all Elements required for the different Patient Samples in a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        ''' <param name="pNewWorkSession">When True, it indicates a new WS is being created and all needed Patient Samples have to be added
        '''                               and it is not needed to validate if the Element is already in the WS</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. It is used to update the status of the Patient Sample tube when it is already 
        '''                           positioned and new tests are requested for the Patient</param>
        ''' <returns>Global object containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: VR 10/12/2009 - Get a GlobalDataTO when calling function GetSampleTypesTubes in OrderTestsDelegate
        '''              SA 05/01/2010 - Changed the way of open the DB Transaction to the new template 
        '''              SA 05/01/2010 - Implemented changes needed to use this function to add new Order Tests to an existing Work Session
        '''              SA 09/03/2010 - Implemented changes needed to shown a SampleID and also the produced by the change in the return
        '''                              type of function GenerateElementID 
        '''              SA 17/03/2010 - Changes to get the TubeType specified for each Patient Sample and inform it in table of Required Elements
        '''              SA 14/10/2010 - Changed the management of Automatic Predilutions: only the full Sample tube is added as Required Element
        '''              SA 22/10/2010 - Inform field OnlyForISE in the DS according value returned by function GetSampleTypesTubes
        '''              SA 17/02/2011 - New parameter to indicate when the function was called for a new WorkSession
        '''              AG 09/09/2011 - Update the status of the Calibrator Sample Tubes if they are already positioned
        '''              SA 13/01/2012 - Changed the function template. Use the same dataToReturn when calling function 
        '''                              UpdateRequiredElementPositionStatusByAddingOrderTests, and stop the process in case of error 
        '''              SG 29/04/2013 - Build the specimenID list and assing it to each PATIENT Element 
        ''' </remarks>
        Public Function AddWSElementsForPatientSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                       ByVal pOrderTestsList As String, ByVal pNewWorkSession As Boolean, ByVal pAnalyzerID As String) _
                                                       As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim noError As Boolean = True

                        'Get list of required Sample Types by Patient
                        Dim sampleTypeTubesData As New OrderTestsDelegate

                        dataToReturn = sampleTypeTubesData.GetSampleTypesTubes(dbConnection, pOrderTestsList, pWorkSessionID)
                        noError = (Not dataToReturn.HasError)

                        If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim sampleTypeTubesDataDS As TubesBySampleTypeDS = DirectCast(dataToReturn.SetDatos, TubesBySampleTypeDS)

                            Dim wsElementDataDS As New WSRequiredElementsDS
                            Dim wsElementFoundDS As New WSRequiredElementsDS
                            Dim wsElementData As New WSRequiredElementsDelegate

                            Dim i As Integer = 0
                            Dim newElement As Boolean = True
                            Dim visibleElement As Boolean = True
                            Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow = Nothing

                            'SGM 29/04/2013
                            'From table twksOrderTestsLISInfo,  get all different SpecimenID sent by LIS for each Patient/Sample Type
                            Dim myAllSpecimensDS As New OrderTestsLISInfoDS
                            Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate

                            dataToReturn = myOrderTestsLISInfoDelegate.GetSpecimensByPatientSampleType(dbConnection)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                myAllSpecimensDS = DirectCast(dataToReturn.SetDatos, OrderTestsLISInfoDS)
                            End If
                            'End SGM 29/04/2013

                            Do While (i < sampleTypeTubesDataDS.TubesBySampleTypeTable.Rows.Count) And (noError)
                                If (i = 0) Then wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow

                                If (noError) Then
                                    visibleElement = True

                                    'Prepare the DataSet Row needed to verify if there is already an Element in the Work Session 
                                    'for the Patient Sample in process
                                    wsElementsRow.WorkSessionID = pWorkSessionID.Trim
                                    wsElementsRow.TubeContent = "PATIENT"
                                    wsElementsRow.SampleType = sampleTypeTubesDataDS.TubesBySampleTypeTable(i).SampleType
                                    wsElementsRow.TubeType = sampleTypeTubesDataDS.TubesBySampleTypeTable(i).TubeType
                                    wsElementsRow.OnlyForISE = sampleTypeTubesDataDS.TubesBySampleTypeTable(i).OnlyForISE

                                    If (Not sampleTypeTubesDataDS.TubesBySampleTypeTable(i).IsPatientIDNull) Then
                                        wsElementsRow.PatientID = sampleTypeTubesDataDS.TubesBySampleTypeTable(i).PatientID
                                        wsElementsRow.SetOrderIDNull()
                                        wsElementsRow.SetSampleIDNull()
                                    ElseIf (Not sampleTypeTubesDataDS.TubesBySampleTypeTable(i).IsSampleIDNull) Then
                                        wsElementsRow.SampleID = sampleTypeTubesDataDS.TubesBySampleTypeTable(i).SampleID
                                        wsElementsRow.SetOrderIDNull()
                                        wsElementsRow.SetPatientIDNull()
                                    Else
                                        wsElementsRow.OrderID = sampleTypeTubesDataDS.TubesBySampleTypeTable(i).OrderID
                                        wsElementsRow.SetPatientIDNull()
                                        wsElementsRow.SetSampleIDNull()
                                    End If

                                    Dim toGetSpecimenIDList As Boolean = False 'SGM 29/04/2013
                                    If (Not sampleTypeTubesDataDS.TubesBySampleTypeTable(i).IsPredilutionFactorNull) Then
                                        If (String.Compare(sampleTypeTubesDataDS.TubesBySampleTypeTable(i).PredilutionMode, "INST", False) = 0) Then
                                            'Automatic dilutions are added as full Samples, the Predilution Factor is not informed
                                            wsElementsRow.SetPredilutionFactorNull()
                                            toGetSpecimenIDList = True 'SGM 29/04/2013
                                        Else
                                            'This is a Patient Sample with dilution
                                            wsElementsRow.PredilutionFactor = sampleTypeTubesDataDS.TubesBySampleTypeTable(i).PredilutionFactor
                                            toGetSpecimenIDList = True 'SGM 11/06/2013
                                        End If

                                        'Tubes for Automatic Predilution will not be visible in the TreeView of required Elements (they do not have
                                        'to be positioned due to the dilution is executed in a cell of the Analyzer Reactions Rotor)
                                        'If (sampleTypeTubesDataDS.TubesBySampleTypeTable(i).PredilutionMode = "INST") Then visibleElement = False
                                    Else
                                        wsElementsRow.SetPredilutionFactorNull()
                                        toGetSpecimenIDList = True 'SGM 29/04/2013
                                    End If

                                    'SGM 29/04/2013 - Build the SpecimenID list and assing it to each element
                                    If (toGetSpecimenIDList) Then
                                        Dim myPatientID As String = String.Empty
                                        If (Not wsElementsRow.IsPatientIDNull) Then
                                            myPatientID = wsElementsRow.PatientID
                                        ElseIf (Not wsElementsRow.IsSampleIDNull) Then
                                            myPatientID = wsElementsRow.SampleID
                                        End If

                                        Dim mySampleType As String = String.Empty
                                        If (Not wsElementsRow.IsSampleTypeNull) Then
                                            mySampleType = wsElementsRow.SampleType
                                        End If

                                        If (myPatientID.Length > 0 And mySampleType.Length > 0) Then
                                            Dim lqSpecimenIDs As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                                            lqSpecimenIDs = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In myAllSpecimensDS.twksOrderTestsLISInfo _
                                                            Where a.LISPatientID = myPatientID _
                                                          AndAlso a.SampleType = mySampleType _
                                                           Select a).ToList

                                            Dim mySpecimenIDList As String = String.Empty
                                            For Each row As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In lqSpecimenIDs
                                                mySpecimenIDList &= row.SpecimenID & GlobalConstants.SPECIMENID_SEPARATOR
                                            Next

                                            If (mySpecimenIDList.Length > 0) Then
                                                mySpecimenIDList = mySpecimenIDList.Substring(0, mySpecimenIDList.Length - GlobalConstants.SPECIMENID_SEPARATOR.Length)
                                                wsElementsRow.SpecimenIDList = mySpecimenIDList
                                            Else
                                                wsElementsRow.SetSpecimenIDListNull()
                                            End If
                                            lqSpecimenIDs = Nothing
                                        End If
                                    Else
                                        wsElementsRow.SetSpecimenIDListNull()
                                    End If
                                    'End SGM 29/04/2013

                                    'If the dataset row has not been created, it is added now
                                    If (i = 0) And (wsElementDataDS.twksWSRequiredElements.Rows.Count = 0) Then wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                                    newElement = True
                                    If (Not pNewWorkSession) Then
                                        'Verify if there is a Required Element for the Patient Sample in the Work Session
                                        dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)
                                        noError = (Not dataToReturn.HasError)

                                        If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                            wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)
                                            newElement = (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0)
                                        End If
                                    End If

                                    If (newElement) Then
                                        'An Element was not found, generate the next ElementID to insert it and set Status to 
                                        'Not Positioned; the PredilutionFactor is also informed
                                        dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                        noError = (Not dataToReturn.HasError)

                                        If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                            wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                            wsElementsRow.ElementStatus = "NOPOS"

                                            'Add the Patient Sample to the list of required Elements for the specified Work Session
                                            dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS, wsElementsRow.SpecimenIDList)
                                            noError = (Not dataToReturn.HasError)
                                        End If
                                    Else
                                        'An Element already exists in the WS for the Patient Sample
                                        'Update the ElementFinished flag and position Status for the Patient
                                        dataToReturn = UpdateRequiredElementStatusByAddingOrderTests(dbConnection, pAnalyzerID, pWorkSessionID, "PATIENT", _
                                                                                                     wsElementFoundDS.twksWSRequiredElements(0).ElementID, _
                                                                                                     wsElementsRow.SpecimenIDList)
                                        noError = (Not dataToReturn.HasError)
                                    End If

                                    i += 1
                                End If
                            Loop
                        End If

                        If (noError) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForPatientSamples", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add all Dilution Solutions needed to execute automatic predilutions as required Work Session Elements 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of identifiers of all Order Tests included in the Work Session that have to be executed in an 
        '''                               Analyzer, separated by commas</param>
        ''' <returns>Global object containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 27/05/2014 - BT #1519 ==> Code to add to the Work Session all needed Dilution Solutions was extracted from the previous used function
        '''                                           AddWSElementsForAdditionalSolutions (which was used for DilutionSolutions and also for WashingSolutions) although 
        '''                                           with following changes:
        '''                                           * Saline Solution has to be treated in the same way than the rest of Dilution Solutions (the If to exclude SALINESOL
        '''                                             from the process was removed)
        '''                                           * When the Dilution Solution in process already exist as required Element in the active WorkSession, the Update is not needed
        ''' </remarks>
        Public Function AddWSElementsForDilutionSolutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                          ByVal pOrderTestsList As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim isOK As Boolean = True

                        'Get the list of Dilution Solutions needed for automatic predilutions in the active WorkSession
                        Dim orderTestsDelegate As New OrderTestsDelegate
                        dataToReturn = orderTestsDelegate.VerifyAutomaticDilutions(dbConnection, pOrderTestsList, pWorkSessionID)
                        isOK = (Not dataToReturn.HasError)

                        If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim diluentSolutionsDS As TestSamplesDS = DirectCast(dataToReturn.SetDatos, TestSamplesDS)

                            Dim preloadedDataDS As PreloadedMasterDataDS
                            Dim preloadedData As New PreloadedMasterDataDelegate

                            If (diluentSolutionsDS.tparTestSamples.Rows.Count > 0) Then
                                'Get the list of available Dilution Solutions from table of Preloaded Master Data
                                dataToReturn = preloadedData.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS)
                                isOK = (Not dataToReturn.HasError)

                                If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                    preloadedDataDS = DirectCast(dataToReturn.SetDatos, PreloadedMasterDataDS)

                                    If (preloadedDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        'Get the minimum size for bottles of Reagents and Additional Solutions
                                        Dim reagentTubeSizesData As New ReagentTubeTypesDelegate
                                        dataToReturn = reagentTubeSizesData.GetMinimumBottleSize(dbConnection)
                                        isOK = (Not dataToReturn.HasError)

                                        If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                            Dim minimumBottleSize As Single = CType(dataToReturn.SetDatos, Single)

                                            'Prepare a local DataSet WSRequiredElementsDS with one row to be used to check for each Dilution Solution if
                                            'it already exists as a required Element in the active Work Session
                                            Dim wsElementDataDS As New WSRequiredElementsDS
                                            Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow
                                            wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                            wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                                            Dim wsElementFoundDS As New WSRequiredElementsDS
                                            Dim wsElementData As New WSRequiredElementsDelegate
                                            Dim dilutionSolutionsList As List(Of TestSamplesDS.tparTestSamplesRow)

                                            'For each available Dilution Solution, check if it is needed in the active WorkSession (if it exists in the TestSamplesDS
                                            'previouly obtained) and in this case, check if it already exists as required Element in the active Work Session:
                                            '** If the Dilution Solution exists as required Element, nothing to do
                                            '** Otherwise, the Dilution Solution is added as a new required Element
                                            For Each dilutionSol As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In preloadedDataDS.tfmwPreloadedMasterData
                                                'Check if the Dilution Solution is in the list of Dilution Solutions needed in the active WorkSession
                                                dilutionSolutionsList = (From a As TestSamplesDS.tparTestSamplesRow In diluentSolutionsDS.tparTestSamples _
                                                                        Where a.DiluentSolution = dilutionSol.ItemID _
                                                                       Select a).ToList

                                                If (dilutionSolutionsList.Count > 0) Then
                                                    'Fill a row in a local DataSet WSRequiredElements informing values for the Dilution Solution in process
                                                    wsElementsRow.WorkSessionID = pWorkSessionID
                                                    wsElementsRow.TubeContent = "SPEC_SOL"
                                                    wsElementsRow.SolutionCode = dilutionSol.ItemID

                                                    'Verify if the Dilution Solution already exists as Required Element in the WorkSession
                                                    dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)
                                                    isOK = (Not dataToReturn.HasError)

                                                    If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                        wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)

                                                        If (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0) Then
                                                            'The Dilution Solution does not exist as required Element in the active Work Session
                                                            'Generate the next ElementID and inform it in the correspondent field in the row; 
                                                            'inform also RequiredVolume = minBottleSize and ElementStatus = NOPOS
                                                            dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                                            isOK = (Not dataToReturn.HasError)

                                                            If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                                wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                                                wsElementsRow.RequiredVolume = minimumBottleSize
                                                                wsElementsRow.ElementStatus = "NOPOS"

                                                                'Add the Dilution Solution to the list of required Elements for the specified Work Session
                                                                dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                                            End If
                                                        End If
                                                    End If
                                                End If

                                                'If an error has happen during the process finish the adding process
                                                If (Not isOK) Then Exit For
                                            Next
                                            dilutionSolutionsList = Nothing
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If (isOK) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForDilutionSolutions", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add all Washing Solutions needed to avoid Contaminations (between Reagents and/or of Reactions Rotor Wells) as required Work Session Elements
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of identifiers of all Order Tests included in the Work Session that have to be executed in an Analyzer,
        '''                               separated by commas</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>Global object containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 27/05/2014 - BT #1519 ==> Code to add to the Work Session all needed Washing Solutions was extracted from the previous used function
        '''                                           AddWSElementsForAdditionalSolutions (which was used for DilutionSolutions and also for WashingSolutions) although 
        '''                                           with following changes:
        '''                                           * Instead of adding all available Washing Solutions to the active WorkSession, add only the Washing Solutions needed
        '''                                             to avoid Contaminations (between Reagents and/or of Reactions Rotor Wells) in the active WorkSession. 
        '''                                           * Added new parameter to inform the Analyzer Identifier (pAnalyzerID)
        ''' </remarks>
        Public Function AddWSElementsForWashingSolutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestsList As String, _
                                                         ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim isOK As Boolean = True

                        'Get the list of Washing Solutions needed to avoid Contaminations in the Work Session
                        Dim myContaminationsDelegate As New ContaminationsDelegate
                        dataToReturn = myContaminationsDelegate.GetWSContaminationsWithWASH(dbConnection, pWorkSessionID, pAnalyzerID)
                        isOK = (Not dataToReturn.HasError)

                        If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim contaminationsDS As ContaminationsDS = DirectCast(dataToReturn.SetDatos, ContaminationsDS)

                            Dim preloadedDataDS As PreloadedMasterDataDS
                            Dim preloadedData As New PreloadedMasterDataDelegate

                            If (contaminationsDS.tparContaminations.Rows.Count > 0) Then
                                'Get the list of available Washing Solutions from table of Preloaded Master Data
                                dataToReturn = preloadedData.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS)
                                isOK = (Not dataToReturn.HasError)

                                If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                    preloadedDataDS = DirectCast(dataToReturn.SetDatos, PreloadedMasterDataDS)

                                    If (preloadedDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        'Get the minimum size for bottles of Reagents and Additional Solutions
                                        Dim reagentTubeSizesData As New ReagentTubeTypesDelegate
                                        dataToReturn = reagentTubeSizesData.GetMinimumBottleSize(dbConnection)
                                        isOK = (Not dataToReturn.HasError)

                                        If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                            Dim minimumBottleSize As Single = CType(dataToReturn.SetDatos, Single)

                                            'Prepare a local DataSet WSRequiredElementsDS with one row to be used to check for each Washing Solution if
                                            'it already exists as a required Element in the active Work Session
                                            Dim wsElementDataDS As New WSRequiredElementsDS
                                            Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow
                                            wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                            wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                                            Dim wsElementFoundDS As New WSRequiredElementsDS
                                            Dim wsElementData As New WSRequiredElementsDelegate
                                            Dim contaminationWashSolutionsList As List(Of ContaminationsDS.tparContaminationsRow)

                                            'For each available Washing Solution, check if it is needed in the active WorkSession to avoid Contaminations (if it exists
                                            'in the ContaminationsDS previouly obtained) and in this case, check if it already exists as required Element in the active 
                                            'Work Session:
                                            '** If the Washing Solution exists as required Element, nothing to do
                                            '** Otherwise, the Washing Solution is added as a new required Element
                                            For Each washingSol As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In preloadedDataDS.tfmwPreloadedMasterData
                                                'WASHSOL3 is the ISE Washing Solution and it is ignored in this process
                                                If (washingSol.ItemID <> "WASHSOL3") Then
                                                    'Check if the Washing Solution is in the list of Washing Solutions needed to avoid Contaminations in the active WorkSession
                                                    contaminationWashSolutionsList = (From a As ContaminationsDS.tparContaminationsRow In contaminationsDS.tparContaminations _
                                                                                     Where (Not a.IsWashingSolutionR1Null AndAlso a.WashingSolutionR1 = washingSol.ItemID) _
                                                                                    OrElse (Not a.IsWashingSolutionR2Null AndAlso a.WashingSolutionR2 = washingSol.ItemID) _
                                                                                    Select a).ToList

                                                    If (contaminationWashSolutionsList.Count > 0) Then
                                                        'Fill a row in a local DataSet WSRequiredElements informing values for the Washing Solution in process
                                                        wsElementsRow.WorkSessionID = pWorkSessionID
                                                        wsElementsRow.TubeContent = "WASH_SOL"
                                                        wsElementsRow.SolutionCode = washingSol.ItemID

                                                        'Verify if the Washing Solution already exists as Required Element in the WorkSession
                                                        dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)
                                                        isOK = (Not dataToReturn.HasError)

                                                        If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                            wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)

                                                            If (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0) Then
                                                                'The Washing Solution does not exist as required Element in the active Work Session
                                                                'Generate the next ElementID and inform it in the correspondent field in the row; 
                                                                'inform also RequiredVolume = minBottleSize and ElementStatus = NOPOS
                                                                dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                                                isOK = (Not dataToReturn.HasError)

                                                                If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                                    wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                                                    wsElementsRow.RequiredVolume = minimumBottleSize
                                                                    wsElementsRow.ElementStatus = "NOPOS"

                                                                    'Add the Washing Solution to the list of required Elements for the specified Work Session
                                                                    dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If

                                                'If an error has happen during the process finish the adding process
                                                If (Not isOK) Then Exit For
                                            Next
                                            contaminationWashSolutionsList = Nothing
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If (isOK) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForWashingSolutions", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' When there are ISE Tests requested in the WorkSession, add the ISE Washing Solution as a required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of identifiers of all Order Tests included in the Work Session 
        '''                               that have to be executed in an Analyzer, separated by commas. </param>
        ''' <returns>Global object containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 08/06/2011 - Based on AddWSElementsForAdditonalSolutions
        ''' Modified by: SA 16/02/2012 - Removed parameter pConstantVolumes
        '''              SA 20/03/2012 - Inform parameter for WorkSessionID when calling function IsThereAnyISETest
        '''              SA 14/05/2012 - Call to function IsThereAnyISETest changed by call to new function IsThereAnyTestByType 
        '''                              informing TestType parameter as ISE
        ''' </remarks>
        Public Function AddWSElementsForISEWashing(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestsList As String) _
                                                   As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim isOK As Boolean = True

                        'Get data of the ISE Tests in the list of Order Tests to add to the Work Session
                        Dim ISETestData As New OrderTestsDelegate
                        dataToReturn = ISETestData.IsThereAnyTestByType(dbConnection, pWorkSessionID, "ISE")
                        isOK = (Not dataToReturn.HasError)

                        If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim thereAreISETests As Boolean = DirectCast(dataToReturn.SetDatos, Boolean)

                            If (thereAreISETests) Then
                                'Get the default tube used for Blanks. For now it is the same for ISE.
                                Dim myUserSettingsDelegate As New UserSettingsDelegate
                                dataToReturn = myUserSettingsDelegate.GetDefaultSampleTube(Nothing, "BLANK")

                                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                    Dim defaultTubeISE As String = DirectCast(dataToReturn.SetDatos, String)

                                    'Get the required total volume of the ISE Washing Solution 
                                    'TODO: Calculate the right value
                                    Dim requiredVolume As Single = 0
                                    Dim orderTestData As New OrderTestsDelegate

                                    dataToReturn = orderTestData.GetSpecialSolutionVolume(dbConnection, pOrderTestsList, "WASHSOL3", pWorkSessionID)
                                    isOK = (Not dataToReturn.HasError)

                                    If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        requiredVolume = DirectCast(dataToReturn.SetDatos, Single)

                                        Dim wsElementDataDS As New WSRequiredElementsDS
                                        Dim wsElementFoundDS As New WSRequiredElementsDS
                                        Dim wsElementData As New WSRequiredElementsDelegate

                                        Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow
                                        wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                        wsElementsRow.WorkSessionID = pWorkSessionID
                                        wsElementsRow.TubeContent = "TUBE_WASH_SOL"
                                        wsElementsRow.SolutionCode = "WASHSOL3"
                                        wsElementsRow.RequiredVolume = requiredVolume
                                        wsElementsRow.OnlyForISE = True
                                        wsElementsRow.TubeType = defaultTubeISE
                                        wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                                        'Verify if there is already a Required Element for the ISE Test in the Work Session
                                        dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)
                                        isOK = (Not dataToReturn.HasError)

                                        If (isOK AndAlso Not dataToReturn Is Nothing) Then
                                            wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)

                                            If (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0) Then
                                                'An Element was not found, generate the next ElementID to insert it and set Status to Not Positioned
                                                dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                                isOK = (Not dataToReturn.HasError)

                                                If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                    wsElementsRow.ElementID = CType(dataToReturn.SetDatos, Integer)
                                                    wsElementsRow.ElementStatus = "NOPOS"

                                                    'Add the ISE Washing Solution to the list of required Elements for the specified Work Session
                                                    'Create Elements for the ISE Washing Solution Tube
                                                    dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                                    isOK = (Not dataToReturn.HasError)
                                                End If
                                                'Else
                                                'An Element already exists in the WS for the ISE Washing Solution --> Nothing to do in this case,
                                                'go to process the next element in the list
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            If (isOK) Then
                                'When the Database Transaction was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForISEWashing", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add a new Work Session with all the required elements according the list of Order Tests 
        ''' that have to be added to it, or update an existing one by adding it all the required elements 
        ''' according the list of new Order Tests that have to be added to it 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSOrderTestsList">List of Order Tests that have to be added to the Work Session</param>
        ''' <param name="pCreateWS">When True, it indicates the WS has to be created, including positions of all Rotors of the Analyzer, 
        '''                         and it has to return a WorkSessionsDS. When False, it indicates that some Order Tests will be added 
        '''                         to an existing WorkSession</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter, when informed, it is the Analyzer in which the Work Session
        '''                           has to be executed</param>
        ''' <param name="pCurrentWSStatus">Current Status of the WS to update. Optional parameter; when informed, it allows to known if the 
        '''                                status have to be updated, which happen in following cases: from OPEN to PENDING and from
        '''                                CLOSED to INPROCESS; in the rest of cases a status change is not needed</param>
        ''' <param name="pSearchNotInUseElements">When True, it indicates the process has to verify if there are not In Use positions in the WorkSession 
        '''                                       that corresponds to the new added Patient elements</param>
        ''' <returns>GlobalDataTO containing error information and, when a new WorkSession has been created or updated, a typed DataSet WorkSessionDS, 
        '''          containing the identifier of the Work Session and its current Status, and also information about the Analyzer in which the 
        '''          Work Session is or will be executed</returns>
        ''' <remarks>
        ''' Created by:   SA
        ''' Modified by:  AG 24/11/2009 - Add call to CreateWSRotorPositions (TESTED: PENDING)
        '''               VR 10/12/2009 - Change the Return Dataset to GlobalDataTO (TESTED: PENDING)
        '''               VR 11/12/2009 - Add the New parameter pCreateWS (TESTED: PENDING)
        '''               VR 15/12/2009 - Change the AddAnalyzerToWS 3 string parameters to Dataset (TESTED: PENDING)
        '''               VR 29/12/2009 - Change the Constant Value to Enum Value
        '''               SA 04/01/2010 - Changes in code flow to fulfill the function design. Hard code values for testing removed
        '''               SA 04/01/2010 - Changed the way of open the DB Transaction to the new template 
        '''               DL 26/01/2010 - Verify if there are no_inuse positions in the worksession that corresponds to the new  added elements
        '''               SA 02/02/2010 - Inform new field WorkSessionDesc with the same value of WorkSessionID (provisional)
        '''               SA 03/03/2010 - Added optional parameter for the Analyzer Identifier
        '''              GDS 10/05/2010 - Added call to UpdateInUseFlag
        '''               SA 11/05/2010 - Changed WorkSession Status from EMPTY to CREATED (Empty Status has been removed)
        '''               SA 27/05/2010 - Inform also WS Status in the DS to return.  Return the DS also in case of update of 
        '''                               an existing Work Session
        '''               SA 09/06/2010 - Call method UpdateInUseFlag to set InUse=False for all elements that have been removed from the 
        '''                               WorkSession (only for WorkSession updation)
        '''               SA 11/06/2010 - Added new optional parameter for the current WS Status, needed in WS updation to change the 
        '''                               status when it is needed. Added code to manage the status changing from OPEN to PENDING and/or 
        '''                               from CLOSED to INPROCESS. Changed WorkSession Status from CREATED to OPEN (Created Status has 
        '''                               been removed)
        '''               SA 12/07/2010 - Status of Order Tests sent to positioning have to be changed from OPEN to PENDING also when
        '''                               the WorkSession is created
        '''               SA 26/01/2011 - Added call to function AddWSAdditionalSolutions Diluent Solutions
        '''               SA 16/02/2011 - Changed the way of adding records to relation table twksWSRequiredElemByOrderTest
        '''               AG 06/06/2011 - Added creation of the Reactions Rotor when a new WorkSession is created
        '''               SA 04/08/2011 - When modify an existing WS, if current WSStatus is EMPTY, it is changed to PENDING
        '''               SA 16/09/2011 - When preparing the WorkSessionsDS to return for updation of the active WS, inform also 
        '''                               the received WS Status
        '''               SA 07/10/2011 - All "Exit Try" changed for a normal GlobalDataTO.HasError verification. With Exit Try, the DB Transaction
        '''                               was not rollbacked properly
        '''               SA 16/02/2012 - Remove declaration and use of variable additionalVolumes
        '''               SA 10/04/2013 - After calling function FindElementsInNotInUsePosition, call the new function FindPatientsInNotInUsePosition, which
        '''                               verify if there are incomplete Patient Samples that can be completed due to the required Element has been added
        '''                               to the updated WorkSession; added new optional parameter to avoid executing the searching of not in use elements 
        '''                               when the function has been called during the scanning of Samples Rotor
        '''               SA 30/05/2013 - After calling function FindPatientsInNotInUsePositions, get the returned Boolean value and inform field BCLinkToElem
        '''                               in the WorkSessionsDS to return. This value indicates if at least one of the incomplete Patient Samples placed in 
        '''                               Samples Rotor was linked to one of the required WS Elements (that is to say, the tube is no more a NOT IN USE one)
        '''               SA 22/07/2013 - When parameter pCreateWS is TRUE, check there is not a Work Session in table twksWorkSessions before create a new one. 
        '''                               This change is to avoid creation of a new WorkSession when Reset of the previous one has not finished  (this error has 
        '''                               happened in several customers, and it is probably due to thread synchronization problems) 
        '''               SA 09/09/2013 - When function was called to create an EMPTY WS but there is already a WorkSession in table twksWorkSessions, set new field
        '''                               CreateEmptyWSStopped to TRUE and write a Warning message in the application Log
        '''               SA 27/05/2014 - BT #1519 ==> Changed call to function AddWSElementsForAdditionalSolutions to add required Elements for all the available 
        '''                                            Dilution Solutions for a call to the new specific function AddWSElementsForDilutionSolutions
        '''                                        ==> Changed call to function AddWSElementsForAdditionalSolutions to add required Elements for all the available 
        '''                                            Washing Solutions for a call to the new specific function AddWSElementsForWashingSolutions. Besides, call the
        '''                                            function after the call to function to add the required Reagents (AddWSElementsForReagents), because to check 
        '''                                            the possible Contaminations the Reagents have to be already created as required Elements
        ''' </remarks>
        Public Function AddWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSOrderTestsList As WSOrderTestsDS, _
                                       ByVal pCreateWS As Boolean, Optional ByVal pAnalyzerID As String = "", Optional ByVal pCurrentWSStatus As String = "", _
                                       Optional ByVal pSearchNotInUseElements As Boolean = True) As GlobalDataTO

            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the logged User
                        'Dim currentSession As New GlobalBase
                        Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                        Dim stopProcess As Boolean = False
                        Dim workSessionDataDS As New WorkSessionsDS
                        Dim workSessionDR As WorkSessionsDS.twksWorkSessionsRow

                        'Was method called to create a new Work Session?
                        If (pCreateWS) Then
                            'Check there is not a Work Session in table twksWorkSessions to avoid creation of a new WorkSession when Reset of the
                            'previous one has not finished (this error has happened in several customers, and it is probably due to thread synchronization problems) 
                            Dim wsAnalyzerDelegate As New WSAnalyzersDelegate

                            dataToReturn = wsAnalyzerDelegate.GetActiveWSByAnalyzer(dbConnection, pAnalyzerID)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                Dim activeWorkSessionDS As WSAnalyzersDS = DirectCast(dataToReturn.SetDatos, WSAnalyzersDS)

                                If (activeWorkSessionDS.twksWSAnalyzers.Rows.Count = 0) Then
                                    'Generate next Work Session ID and fill information required to create a new WorkSession
                                    'in the correspondent DataSet (WorkSessionsDS)
                                    Dim workSessionData As New TwksWorkSessionsDAO

                                    workSessionDR = workSessionDataDS.twksWorkSessions.NewtwksWorkSessionsRow()
                                    workSessionDR.WorkSessionID = workSessionData.GenerateWorkSessionID(dbConnection)
                                    workSessionDR.WorkSessionDesc = workSessionDR.WorkSessionID
                                    workSessionDR.WSDateTime = DateTime.Now
                                    workSessionDR.TS_User = loggedUser.Trim
                                    workSessionDR.TS_DateTime = DateTime.Now
                                    workSessionDataDS.twksWorkSessions.AddtwksWorkSessionsRow(workSessionDR)

                                    'Add the WorkSession
                                    dataToReturn = workSessionData.Create(dbConnection, workSessionDataDS)
                                Else
                                    'There is a WorkSession, it is not allowed to create a new one; prepare the DS to return (with ID and Status of the 
                                    'existing WS) and stop the creating process
                                    workSessionDR = workSessionDataDS.twksWorkSessions.NewtwksWorkSessionsRow()
                                    workSessionDR.WorkSessionID = activeWorkSessionDS.twksWSAnalyzers(0).WorkSessionID
                                    workSessionDR.WorkSessionStatus = activeWorkSessionDS.twksWSAnalyzers(0).WSStatus
                                    workSessionDR.CreateEmptyWSStopped = True   'To write in the application LOG the name of the function who call AddWorkSession 
                                    workSessionDataDS.twksWorkSessions.AddtwksWorkSessionsRow(workSessionDR)

                                    stopProcess = True

                                    'Write the Warning in the Application LOG
                                    'Dim myLogAcciones As New ApplicationLogManager()
                                    GlobalBase.CreateLogActivity("WARNING: Called to add EMPTY WS when the previous one still exists", "WorkSessionsDelegate.AddWorkSession", EventLogEntryType.Error, False)
                                End If
                            End If
                        Else
                            'When a WorkSession will be updated, create the DS to return informing the WorkSessionID and WSStatus received
                            workSessionDR = workSessionDataDS.twksWorkSessions.NewtwksWorkSessionsRow()
                            workSessionDR.WorkSessionID = pWSOrderTestsList.twksWSOrderTests(0).WorkSessionID
                            workSessionDR.WorkSessionStatus = pCurrentWSStatus
                            workSessionDataDS.twksWorkSessions.AddtwksWorkSessionsRow(workSessionDR)
                        End If

                        If (Not dataToReturn.HasError AndAlso Not stopProcess) Then
                            'Build a string list with all the Order Tests included in the WorkSession that have to be positioned in an Analyzer
                            Dim orderTestsList As String = ""
                            For i As Integer = 0 To pWSOrderTestsList.twksWSOrderTests.Rows.Count - 1
                                If (pCreateWS) Then
                                    'Add the generated WorkSessionID to the OrderTests structure
                                    pWSOrderTestsList.BeginInit()
                                    pWSOrderTestsList.twksWSOrderTests(i).WorkSessionID = workSessionDataDS.twksWorkSessions(0).WorkSessionID
                                    pWSOrderTestsList.EndInit()
                                End If

                                If (Not pWSOrderTestsList.twksWSOrderTests(i).OpenOTFlag) AndAlso _
                                   (pWSOrderTestsList.twksWSOrderTests(i).ToSendFlag) Then
                                    orderTestsList += pWSOrderTestsList.twksWSOrderTests(i).OrderTestID.ToString & ", "
                                End If
                            Next i
                            If (orderTestsList.Trim <> "") Then orderTestsList = Mid(orderTestsList, 1, orderTestsList.Length - 2)

                            'Save the Work Session ID in a local variable 
                            Dim workSessionID As String
                            If (pCreateWS) Then
                                'In this case, get the generated WorkSessionID
                                workSessionID = workSessionDataDS.twksWorkSessions(0).WorkSessionID
                            Else
                                'In this case it is the same for all OrderTest rows, then the one in the first row is obtained
                                workSessionID = pWSOrderTestsList.twksWSOrderTests(0).WorkSessionID
                            End If

                            'Add the list of Order Tests to the Work Session
                            If (pWSOrderTestsList.twksWSOrderTests.Rows.Count > 0) Then
                                Dim wsOrderTestData As New WSOrderTestsDelegate
                                dataToReturn = wsOrderTestData.AddOrderTestsToWS(dbConnection, pWSOrderTestsList, pCreateWS)
                            End If

                            If (Not dataToReturn.HasError) Then
                                If (orderTestsList.Trim <> "") Then
                                    'Add Required Elements for all the available Diluent Solutions
                                    dataToReturn = AddWSElementsForDilutionSolutions(dbConnection, workSessionID, orderTestsList)

                                    'Add Required Elements for all the available ISE Washing Solutions
                                    If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForISEWashing(dbConnection, workSessionID, orderTestsList)

                                    'Add required Elements for all the Reagents needed according the list of Order Tests
                                    If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForReagents(dbConnection, workSessionID, orderTestsList, pCreateWS, pAnalyzerID)

                                    'Add Required Elements for all the available Washing Solutions
                                    If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForWashingSolutions(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                                    'Add required Elements for all the Controls needed according the list of Order Tests
                                    If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForControls(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                                    'Add required Elements for all the Calibrators needed according the list of Order Tests
                                    If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForCalibrators(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                                    'Add required Elements for all the Blanks/Additional solutions needed
                                    If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForBlanks(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                                    'Add required Elements for all the Patient Samples needed according the list of Order Tests
                                    If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForPatientSamples(dbConnection, workSessionID, orderTestsList, pCreateWS, _
                                                                                                                      pAnalyzerID)

                                    'Create all Required Elements by Order Tests
                                    If (Not dataToReturn.HasError) Then
                                        Dim requiredPosByOrderTestsData As New WSRequiredElemByOrderTestDelegate
                                        dataToReturn = requiredPosByOrderTestsData.AddOrderTestElements(dbConnection, workSessionID)
                                    End If
                                End If

                                If (pWSOrderTestsList.twksWSOrderTests.Rows.Count > 0) Then
                                    'Mark all elements required for the WS as InUse
                                    If (Not dataToReturn.HasError) Then dataToReturn = UpdateInUseFlag(dbConnection, workSessionID, pAnalyzerID, True, False)

                                    'Update Status of all Order Tests included in the Work Session from OPEN to PENDING
                                    If (Not dataToReturn.HasError) Then
                                        Dim orderTestsData As New OrderTestsDelegate
                                        dataToReturn = orderTestsData.UpdateStatus(dbConnection, "PENDING", "OPEN", workSessionID, loggedUser)
                                    End If
                                End If

                                If (Not dataToReturn.HasError) Then
                                    'Was method called to create a new Work Session?
                                    If (pCreateWS) Then
                                        'Create Cells (positions) for all Rotors and Rings in the Analyzer to which the new 
                                        'Work Session has been assigned
                                        Dim rotorContentByPos As New WSRotorContentByPositionDelegate
                                        dataToReturn = rotorContentByPos.CreateWSRotorPositions(dbConnection, workSessionID, pAnalyzerID)

                                        If (Not dataToReturn.HasError) Then
                                            'Include the AnalyzerID in the WorkSession and set the WorkSession Status to the initial one 
                                            'Fill a local typed DataSet WSAnalyzersDS
                                            Dim analyzerModel As String = ""
                                            If (pAnalyzerID.Trim = "") Then
                                                'If the Analyzer has not been informed, then get the one returned by the function of creation or
                                                'Rotor Positions
                                                If (Not dataToReturn.SetDatos Is Nothing) Then
                                                    Dim analyzerRotorsConfig As AnalyzerModelRotorsConfigDS = DirectCast(dataToReturn.SetDatos, AnalyzerModelRotorsConfigDS)

                                                    pAnalyzerID = analyzerRotorsConfig.tfmwAnalyzerModelRotorsConfig(0).AnalyzerID
                                                    analyzerModel = analyzerRotorsConfig.tfmwAnalyzerModelRotorsConfig(0).AnalyzerModel
                                                End If
                                            Else
                                                'Get the model of the informed Analyzer
                                                Dim analyzerDataDS As New AnalyzersDS
                                                Dim myAnalyzersDelegate As New AnalyzersDelegate

                                                dataToReturn = myAnalyzersDelegate.GetAnalyzerModel(dbConnection, pAnalyzerID)
                                                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                    analyzerDataDS = DirectCast(dataToReturn.SetDatos, AnalyzersDS)
                                                    If (analyzerDataDS.tcfgAnalyzers.Rows.Count = 1) Then analyzerModel = analyzerDataDS.tcfgAnalyzers(0).AnalyzerModel
                                                End If
                                            End If

                                            If (Not dataToReturn.HasError) Then
                                                'Create the Reactions rotor
                                                Dim reactions As New ReactionsRotorDelegate
                                                dataToReturn = reactions.CreateWSReactionsRotor(dbConnection, pAnalyzerID, analyzerModel, False)
                                            End If

                                            If (Not dataToReturn.HasError) Then
                                                'Calculate the Status for the new WorkSession
                                                Dim myWSStatus As String = IIf(String.Compare(orderTestsList.Trim, "", False) <> 0, "PENDING", _
                                                                               IIf(pWSOrderTestsList.twksWSOrderTests.Rows.Count > 0, "OPEN", "EMPTY")).ToString

                                                Dim myWSAnalyzersDS As New WSAnalyzersDS
                                                Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                                                Dim myAnalyzersRow As WSAnalyzersDS.twksWSAnalyzersRow

                                                myAnalyzersRow = myWSAnalyzersDS.twksWSAnalyzers.NewtwksWSAnalyzersRow()
                                                myAnalyzersRow.WorkSessionID = workSessionID
                                                myAnalyzersRow.AnalyzerID = pAnalyzerID
                                                myAnalyzersRow.WSStatus = myWSStatus
                                                myWSAnalyzersDS.twksWSAnalyzers.Rows.Add(myAnalyzersRow)

                                                dataToReturn = myWSAnalyzersDelegate.AddAnalyzerToWS(dbConnection, myWSAnalyzersDS)
                                                If (Not dataToReturn.HasError) Then
                                                    'Add values of AnalyzerID, AnalyzerModel and WSStatus to the DataSet to return (WorkSessionDS)
                                                    workSessionDataDS.twksWorkSessions(0).BeginEdit()
                                                    workSessionDataDS.twksWorkSessions(0).AnalyzerID = pAnalyzerID
                                                    workSessionDataDS.twksWorkSessions(0).AnalyzerModel = analyzerModel
                                                    workSessionDataDS.twksWorkSessions(0).WorkSessionStatus = myWSStatus
                                                    workSessionDataDS.twksWorkSessions(0).EndEdit()
                                                End If
                                            End If

                                            'AG 24/11/2014 BA-2065
                                            If Not dataToReturn.HasError Then
                                                Dim blWellDlg As New WSBLinesByWellDelegate
                                                dataToReturn = blWellDlg.UpdateWorkSessionID(dbConnection, pAnalyzerID, workSessionID)
                                            End If
                                            'AG 24/11/2014

                                        End If
                                    Else
                                        'When the WorkSession is updated, the InUse flag of all elements removed (if any) have to be set to False
                                        dataToReturn = UpdateInUseFlag(dbConnection, workSessionID, pAnalyzerID, False, True)
                                        If (Not dataToReturn.HasError) Then
                                            'If the WS was CLOSED, it changes to PENDING when new Order Tests are added to it (positioned or not)
                                            'If the WS was EMPTY, it changes to PENDING when new Order Tests are added to it (positioned or not)
                                            'If the WS was OPEN and some OrderTests were selected to be positioned, it changes to PENDING
                                            Dim newStatus As String = "PENDING"
                                            If (pCurrentWSStatus = "EMPTY" AndAlso orderTestsList.Trim = "") Then newStatus = "OPEN"
                                            If (pCurrentWSStatus = "CLOSED") Then newStatus = "INPROCESS"

                                            Dim changeStatus As Boolean = (String.Compare(pCurrentWSStatus, "CLOSED", False) = 0) OrElse (String.Compare(pCurrentWSStatus, "EMPTY", False) = 0) OrElse _
                                                                          (pCurrentWSStatus = "OPEN" AndAlso orderTestsList.Trim <> "")

                                            If (changeStatus) Then
                                                Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                                                dataToReturn = myWSAnalyzersDelegate.UpdateWSStatus(dbConnection, pAnalyzerID, workSessionID, newStatus, pCurrentWSStatus)
                                                If (Not dataToReturn.HasError) Then
                                                    If (dataToReturn.AffectedRecords = 1) Then
                                                        'Previous status was Created, inform the updated Status in the DS to return
                                                        workSessionDataDS.twksWorkSessions(0).BeginEdit()
                                                        workSessionDataDS.twksWorkSessions(0).WorkSessionStatus = newStatus
                                                        workSessionDataDS.twksWorkSessions(0).EndEdit()
                                                    End If
                                                End If
                                            End If

                                            If (Not dataToReturn.HasError) Then
                                                'Verify if there are not In Use positions in the WorkSession that corresponds to the new added elements (excepting Patients)
                                                dataToReturn = FindElementsInNotInUsePosition(dbConnection, workSessionID, pAnalyzerID, Not pSearchNotInUseElements)
                                            End If

                                            If (pSearchNotInUseElements) Then
                                                If (Not dataToReturn.HasError) Then
                                                    'Verify if there are not In Use positions in the WorkSession that corresponds to the new added Patient elements
                                                    dataToReturn = FindPatientsInNotInUsePositions(dbConnection, pAnalyzerID, workSessionID, "SAMPLES")
                                                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                        workSessionDataDS.twksWorkSessions(0).BeginEdit()
                                                        workSessionDataDS.twksWorkSessions(0).BCLinkedToElem = CType(dataToReturn.SetDatos, Boolean)
                                                        workSessionDataDS.twksWorkSessions(0).EndEdit()
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        If (Not dataToReturn.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'The DataSet containing information of the created or updated WorkSession is returned
                            dataToReturn.SetDatos = workSessionDataDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add Elements for all Reagents required according the list of Order Tests included in a Work Session and that has 
        ''' to be sent to an Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        ''' <param name="pNewWorkSession">When True, it indicates a new WS is being created and all needed Reagents have to be added
        '''                               and it is not needed to validate if the Element is already in the WS</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>Global object containing error information</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: SA 05/01/2010 - Changed the way of open the DB Transaction to the new template 
        '''              SA 05/01/2010 - Implemented changes needed to use this function to add new Order Tests to an existing Work Session 
        '''              SA 03/03/2010 - Changes to fix errors due to changes in returned data type of several called functions
        '''              SA 09/03/2010 - Changes due the return type of function GenerateElementID has been modified
        '''              SA 17/02/2011 - Added new parameter to indicate when the function was called for a new WorkSession
        '''              SA 08/02/2012 - Added new parameter for the Analyzer Identifier. For Reagents already included in the WorkSessions, 
        '''                              recalculate the total needed volume using the volume of the new requested Order Tests, plus the 
        '''                              volume of the unfinished previous ones (those having Executions with status Pending, Locked or InProcess)
        '''              SA 15/02/2012 - For Reagents already included in the WorkSessions, call new function CalculateReagentStatus to calculate the
        '''                              ElementStatus and the Reagent Required Volume. Removed parameter pConstantVolumes
        '''              SA 02/03/2012 - Changed the calling to function GetReagentTubesByElement due it was modified by removing the parameter for 
        '''                              the ReagentVolume needed for a preparation
        ''' </remarks>
        Public Function AddWSElementsForReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestsList As String, _
                                                 ByVal pNewWorkSession As Boolean, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim noError As Boolean

                        'Get the list of required Reagents 
                        Dim testReagentsData As New OrderTestsDelegate
                        dataToReturn = testReagentsData.GetRequiredReagents(dbConnection, pOrderTestsList, pWorkSessionID)
                        noError = (Not dataToReturn.HasError)

                        If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim testReagentsDataDS As RequiredReagentsDS = DirectCast(dataToReturn.SetDatos, RequiredReagentsDS)

                            Dim wsElementDataDS As New WSRequiredElementsDS
                            Dim wsElementFoundDS As New WSRequiredElementsDS
                            Dim wsElementData As New WSRequiredElementsDelegate

                            Dim i As Integer = 0
                            Dim newElement As Boolean = True
                            Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow = Nothing
                            Do While (i < testReagentsDataDS.RequiredReagents.Rows.Count) AndAlso (noError)
                                'Prepare the DataSet Row needed to verify if there is already an Element for the Reagent in the WS
                                If (i = 0) Then wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                wsElementsRow.WorkSessionID = pWorkSessionID.Trim
                                wsElementsRow.TubeContent = "REAGENT"
                                wsElementsRow.ReagentID = testReagentsDataDS.RequiredReagents(i).ReagentID
                                wsElementsRow.ReagentNumber = testReagentsDataDS.RequiredReagents(i).ReagentNumber
                                If (i = 0) Then wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                                newElement = True
                                If (Not pNewWorkSession) Then
                                    'Verify if there is a Required Element for the Reagent in the Work Session
                                    dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)
                                    noError = (Not dataToReturn.HasError)

                                    If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)
                                        newElement = (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0)
                                    End If
                                End If

                                If (newElement) Then
                                    'If a Required Element was not found in the WS for the Reagent in process, then the next 
                                    'ElementID has to be generated and informed in the correspondent field in the row, the
                                    'total Reagent Volume will be the RequiredVolume and the ElementStatus is set to Not Positioned
                                    dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                    noError = (Not dataToReturn.HasError)

                                    If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                        wsElementsRow.RequiredVolume = testReagentsDataDS.RequiredReagents(i).TotalVolume
                                        wsElementsRow.ElementStatus = "NOPOS"

                                        'Add the Reagent to the list of required Elements for the specified Work Session
                                        dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                        noError = (Not dataToReturn.HasError)
                                    End If

                                    'Calculate the number of Bottles of different sizes required for the Reagent according the total required Reagent Volume
                                    If (noError) Then
                                        dataToReturn = GetReagentTubesByElement(dbConnection, wsElementsRow.ElementID, testReagentsDataDS.RequiredReagents(i).TotalVolume)
                                        noError = (Not dataToReturn.HasError)
                                    End If
                                Else
                                    'A Required Element was found in the WS for the Reagent in process
                                    wsElementsRow.ElementID = wsElementFoundDS.twksWSRequiredElements(0).ElementID

                                    'Calculate the Reagent Volume needed for all Tests pending to execute...
                                    wsElementsRow.RequiredVolume = testReagentsDataDS.RequiredReagents(i).TotalVolume

                                    'Calculate the Reagent Status and how many bottles of each available size are needed to positioning the required volume pending to positioning
                                    dataToReturn = wsElementData.CalculateNeededBottlesAndReagentStatus(dbConnection, pAnalyzerID, wsElementsRow, testReagentsDataDS.RequiredReagents(i).ReagentVolume, True)
                                    noError = (Not dataToReturn.HasError)

                                    If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        wsElementsRow = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS.twksWSRequiredElementsRow)

                                        'Update data of the existing Required Element 
                                        dataToReturn = wsElementData.Update(dbConnection, wsElementDataDS)
                                        noError = (Not dataToReturn.HasError)
                                    End If
                                End If

                                'Next Reagent
                                i += 1
                            Loop
                        End If

                        If (noError) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForReagents", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add Elements for all Calibrators required according the list of Order Tests included in a Work Session 
        ''' that has to be sent to an Analyzer and belongs to Orders with SampleClass = CALIB
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        ''' <param name="pAnalyzerID" >Analyzer Identifier. It is used to update the status of the Calibrator tubes when they are  
        '''                            already positioned and new tests using the Calibrator are requested</param>
        ''' <returns>Global object containing error information</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: VR 18/12/2009 - Tested: OK
        '''              SA 05/01/2010 - Changed the way of open the DB Transaction to the new template
        '''              SA 05/01/2010 - Errors fixed, change was bad implemented
        '''              SA 03/03/2010 - Changes to fix errors due to changes in returned data type of several called functions
        '''              SA 09/03/2010 - Changes due the return type of function GenerateElementID has been modified
        '''              SA 17/03/2010 - Changes to get the TubeType specified for each Control and inform it in table of Required Elements
        '''              AG 09/09/2011 - Update the status of the Calibrator Sample Tubes if they are already positioned
        '''              SA 13/01/2012 - Changed the function template. Use the same dataToReturn when calling function 
        '''                              UpdateRequiredElementPositionStatusByAddingOrderTests, and stop the process in case of error;
        '''                              for multipoint Calibrators, function UpdateRequiredElementPositionStatusByAddingOrderTests has to be
        '''                              called for the ElementID of each point 
        ''' </remarks>
        Public Function AddWSElementsForCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestsList As String, _
                                                    ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim noError As Boolean = True

                        'Get the list of required Calibrators according the list of Order Tests included in 
                        'a WorkSession and belonging to Calibrator Orders 
                        Dim testCalibrationData As New OrderTestsDelegate
                        dataToReturn = testCalibrationData.GetCalibrationData(dbConnection, pOrderTestsList, pWorkSessionID)
                        noError = (Not dataToReturn.HasError)

                        If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim testCalibrationDataDS As TestSampleCalibratorDS = DirectCast(dataToReturn.SetDatos, TestSampleCalibratorDS)

                            If (testCalibrationDataDS.tparTestCalibrators.Rows.Count > 0) Then
                                Dim wsElementDataDS As New WSRequiredElementsDS
                                Dim wsElementFoundDS As New WSRequiredElementsDS
                                Dim wsElementData As New WSRequiredElementsDelegate

                                Dim i As Integer = 0
                                Dim j As Integer = 1
                                Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow = Nothing
                                Do While (i < testCalibrationDataDS.tparTestCalibrators.Rows.Count) AndAlso (noError)
                                    'For each Calibrator, the number of required Elements will depend on its number of points
                                    j = 1
                                    wsElementDataDS.Clear()
                                    Do While (j <= testCalibrationDataDS.tparTestCalibrators(i).NumberOfCalibrators) AndAlso (noError)
                                        'Prepare the DataSet Row needed to verify if there is already an Element for the Calibrator in the WS
                                        '  ** CalibratorID is the same for every point added
                                        '  ** TubeType is the same for every point added
                                        If (j = 1) Then wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                        wsElementsRow.WorkSessionID = pWorkSessionID.Trim
                                        wsElementsRow.TubeContent = "CALIB"
                                        wsElementsRow.CalibratorID = testCalibrationDataDS.tparTestCalibrators(i).CalibratorID
                                        wsElementsRow.MultiItemNumber = j
                                        wsElementsRow.TubeType = testCalibrationDataDS.tparTestCalibrators(i).TubeType
                                        If (j = 1) Then wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                                        'Verify if there is already a Required Element for the Calibrator in the Work Session
                                        '(for MultiPoint Calibrators is enough verifying if there is a Required Element for the first point)
                                        If (j = 1) Then
                                            dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)
                                            noError = (Not dataToReturn.HasError)

                                            If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)
                                                If (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0) Then
                                                    'An Element was not found, generate the next ElementID to insert it and set Status to Not Positioned
                                                    dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                                    noError = (Not dataToReturn.HasError)

                                                    If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                        wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                                        wsElementsRow.ElementStatus = "NOPOS"

                                                        'Create the Element for the Calibrator Point
                                                        dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                                        noError = (Not dataToReturn.HasError)
                                                    End If
                                                Else
                                                    'An Element already exists in the WS for the Calibrator
                                                    'Update the ElementFinished flag and position Status for all Calibrator points/tubes
                                                    For Each calibPoint As WSRequiredElementsDS.twksWSRequiredElementsRow In wsElementFoundDS.twksWSRequiredElements
                                                        dataToReturn = UpdateRequiredElementStatusByAddingOrderTests(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                                                     "CALIB", calibPoint.ElementID)
                                                        noError = (Not dataToReturn.HasError)
                                                        If (Not noError) Then Exit For
                                                    Next

                                                    'Nothing more to do in this case, go to process the next Calibrator in the list; for MultiPoint 
                                                    'Calibrators is not needed to process the rest of the Points
                                                    j = testCalibrationDataDS.tparTestCalibrators(i).NumberOfCalibrators
                                                End If
                                            End If
                                        Else
                                            'Generate the next ElementID to insert the Calibrator Point and set Status to Not Positioned
                                            dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                            noError = (Not dataToReturn.HasError)

                                            If (noError And Not dataToReturn.SetDatos Is Nothing) Then
                                                wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                                wsElementsRow.ElementStatus = "NOPOS"

                                                'Create the Element for the Calibrator Point
                                                dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                                noError = (Not dataToReturn.HasError)
                                            End If
                                        End If

                                        'Next Calibrator Point
                                        j += 1
                                    Loop

                                    'Next Calibrator
                                    i += 1
                                Loop
                                'Else
                                'There are not Experimental Calibrators for the list of informed Order Tests, or the needed Calibrator has been
                                'already added to the Work Session: do nothing in this case
                            End If
                        End If

                        If (noError) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForCalibrators", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add Elements for all Controls required according the list of Order Tests included in a Work Session
        ''' that has to be sent to an Analyzer and belongs to Orders with SampleClass = CTRL
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. It is used to update the status of the Control tube when it is
        '''                           already positioned and new tests using the Control are requested</param>
        ''' <returns>Global object containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: VR 16/12/2009 - Changed as per modified Design Doc (Tested: PENDING)
        '''              VR 17/12/2009 - Tested: OK
        '''              SA 05/01/2010 - Changed the way of open the DB Transaction to the new template
        '''              SA 05/01/2010 - Errors fixed, change was bad implemented
        '''              SA 03/03/2010 - Changes to fix errors due to changes in returned data type of several called functions
        '''              SA 09/03/2010 - Changes due the return type of function GenerateElementID has been modified
        '''              SA 17/03/2010 - Changes to get the TubeType specified for each Control and inform it in table of Required Elements
        '''              AG 09/09/2011 - Update the status of the Control Sample Tube if it is already positioned
        '''              SA 13/01/2012 - Changed the function template. Use the same dataToReturn when calling function 
        '''                              UpdateRequiredElementPositionStatusByAddingOrderTests, and stop the process in case of error
        ''' </remarks>
        Public Function AddWSElementsForControls(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                 ByVal pOrderTestsList As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim noError As Boolean = True

                        'Get data of the Controls defined for the different pairs of TestType/Test/SampleType
                        'in the list of Order Tests to add to the Work Session
                        Dim testControlsData As New OrderTestsDelegate
                        dataToReturn = testControlsData.GetControlsData(dbConnection, pOrderTestsList, pWorkSessionID)
                        noError = (Not dataToReturn.HasError)

                        If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim testControlsDataDS As TestControlsDS = DirectCast(dataToReturn.SetDatos, TestControlsDS)

                            'One Element has to be added for each Control
                            Dim wsElementDataDS As New WSRequiredElementsDS
                            Dim wsElementFoundDS As New WSRequiredElementsDS
                            Dim wsElementData As New WSRequiredElementsDelegate

                            Dim i As Integer = 0
                            Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow = Nothing
                            Do While (i < testControlsDataDS.tparTestControls.Rows.Count) And (noError)
                                'Prepare the DataSet Row needed to verify if there is already an Element for the Control in the WS
                                If (i = 0) Then wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                wsElementsRow.WorkSessionID = pWorkSessionID.Trim
                                wsElementsRow.TubeContent = "CTRL"
                                wsElementsRow.ControlID = testControlsDataDS.tparTestControls(i).ControlID
                                wsElementsRow.TubeType = testControlsDataDS.tparTestControls(i).TubeType
                                If (i = 0) Then wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                                'Verify if there is already a Required Element for the Control in the Work Session
                                dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)
                                noError = (Not dataToReturn.HasError)

                                If (noError And Not dataToReturn Is Nothing) Then
                                    wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)
                                    If (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0) Then
                                        'An Element was not found, generate the next ElementID to insert it and set Status to Not Positioned
                                        dataToReturn = wsElementData.GenerateElementID(dbConnection)
                                        noError = (Not dataToReturn.HasError)

                                        If (noError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                            wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                            wsElementsRow.ElementStatus = "NOPOS"

                                            'Add the Control to the list of required Elements for the specified Work Session
                                            'Create Elements for the Control Tubes
                                            dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                            noError = (Not dataToReturn.HasError)
                                        End If
                                    Else
                                        'An Element already exists in the WS for the Control -->
                                        'Update the ElementFinished flag and position Status for the Control
                                        dataToReturn = UpdateRequiredElementStatusByAddingOrderTests(dbConnection, pAnalyzerID, pWorkSessionID, "CTRL", _
                                                                                                     wsElementFoundDS.twksWSRequiredElements(0).ElementID)
                                        noError = (Not dataToReturn.HasError)
                                    End If
                                End If
                                i += 1
                            Loop
                        End If

                        If (noError) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForControls", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add Elements for all Blanks required according the list of Order Tests included in a Work Session
        ''' that has to be sent to an Analyzer and belongs to Orders with SampleClass = BLANK
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. It is used to update the status of an Additional Solution tube when it is
        '''                           already positioned and new tests that need the Additional Solution for the Blank are requested</param>
        ''' <returns>Global object containing error information</returns>
        ''' <remarks>
        ''' Created by:  RH 08/06/2011
        ''' Modified by: AG 09/09/2011 - Update the status of the Additional Solution Sample Tube if it is already positioned
        '''              SA 13/01/2012 - Changed the function template. Use the same dataToReturn when calling function 
        '''                              UpdateRequiredElementPositionStatusByAddingOrderTests, and stop the process in case of error. When
        '''                              the Additional Solution already exists in the WorkSession, update the required volume for it 
        '''              SA 16/02/2012 - Removed parameter pConstantVolumes
        ''' </remarks>
        Public Function AddWSElementsForBlanks(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pOrderTestsList As String, _
                                               ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim isOK As Boolean = True

                        'Get data of the Blanks defined for the different pairs of Test/SampleType
                        'in the list of Order Tests to add to the Work Session
                        Dim testBlanksData As New OrderTestsDelegate
                        dataToReturn = testBlanksData.GetBlanksData(dbConnection, pOrderTestsList, pWorkSessionID)
                        isOK = (Not dataToReturn.HasError)

                        If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                            Dim testBlanksDataDS As TestControlsDS = DirectCast(dataToReturn.SetDatos, TestControlsDS)

                            'One Element has to be added for each Blank
                            Dim wsElementDataDS As New WSRequiredElementsDS
                            Dim wsElementFoundDS As WSRequiredElementsDS
                            Dim wsElementData As New WSRequiredElementsDelegate

                            Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow
                            wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                            wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

                            Dim i As Integer = 0
                            Dim requiredVolume As Single = 0
                            Dim orderTestData As New OrderTestsDelegate
                            Do While (i < testBlanksDataDS.tparTestControls.Rows.Count) AndAlso (isOK)
                                'Prepare the DataSet Row needed to verify if there is already an Element for the Blank in the WS
                                '** Blank Mode is returned in field ControlName in the DataSet
                                wsElementsRow.WorkSessionID = pWorkSessionID
                                wsElementsRow.TubeContent = "TUBE_SPEC_SOL"
                                wsElementsRow.SolutionCode = testBlanksDataDS.tparTestControls(i).ControlName
                                wsElementsRow.TubeType = testBlanksDataDS.tparTestControls(i).TubeType

                                'Verify if there is already a Required Element for the Blank in the Work Session
                                dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)

                                isOK = (Not dataToReturn.HasError)
                                If (isOK AndAlso Not dataToReturn Is Nothing) Then
                                    wsElementFoundDS = DirectCast(dataToReturn.SetDatos, WSRequiredElementsDS)

                                    'Get the required total volume of the Sample Additional Solution
                                    dataToReturn = orderTestData.GetSpecialSolutionVolume(dbConnection, pOrderTestsList, wsElementsRow.SolutionCode, pWorkSessionID)

                                    isOK = (Not dataToReturn.HasError)
                                    If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        requiredVolume = DirectCast(dataToReturn.SetDatos, Single)

                                        If (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0) Then
                                            'An Element was not found
                                            'Generate the next ElementID to insert it and set Status to Not Positioned
                                            dataToReturn = wsElementData.GenerateElementID(dbConnection)

                                            isOK = (Not dataToReturn.HasError)
                                            If (isOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
                                                wsElementsRow.ElementStatus = "NOPOS"
                                                wsElementsRow.RequiredVolume = requiredVolume

                                                'Add the Blank to the list of required Elements for the specified Work Session
                                                'Create Elements for the Blank Tubes
                                                dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)
                                                isOK = (Not dataToReturn.HasError)
                                            End If
                                        Else
                                            'An Element already exists in the WS for the Blank
                                            'Update the required volume; do not change the Element Status
                                            wsElementsRow.ElementID = wsElementFoundDS.twksWSRequiredElements(0).ElementID
                                            wsElementsRow.ElementStatus = wsElementFoundDS.twksWSRequiredElements(0).ElementStatus
                                            wsElementsRow.RequiredVolume = requiredVolume

                                            dataToReturn = wsElementData.Update(dbConnection, wsElementDataDS)
                                            isOK = (Not dataToReturn.HasError)

                                            ''Update the ElementFinished flag and position Status for the Sample Additional Solution
                                            dataToReturn = UpdateRequiredElementStatusByAddingOrderTests(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", _
                                                                                                         wsElementFoundDS.twksWSRequiredElements(0).ElementID)
                                            isOK = (Not dataToReturn.HasError)
                                        End If
                                    End If
                                End If
                                i += 1
                            Loop
                        End If

                        If (isOK) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForBlanks", EventLogEntryType.Error, False)
            Finally
                'When Database Connection was opened locally, it has to be closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get data of the specified WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DS WorkSessionsDS with all WorkSession definition data</returns>
        ''' <remarks>
        ''' Created by:  DL 27/05/2010
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWS As New TwksWorkSessionsDAO
                        resultData = myWS.GetByWorkSession(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.GetByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Calculate the number of Reagent Bottles of the different available sizes that are needed for the specified Required Element of 
        ''' a Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pTotalReagentVol">Total required Reagent volume</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Modified by: BK 23/12/2009 - Changes in Datatypes
        '''              SA 11/01/2010 - Changed the way of opening a DB Transaction to fulfill the new template
        '''              SA 08/02/2012 - When a Bottle Size is already linked to the Element; just update the number of needed bottles with the calculated in this 
        '''                              method, without adding the calculated value to the previous one
        '''              SA 16/02/2012 - Removed parameter pConstantVolume, it is needed for Ax5 but not for Ax00. 
        '''                              Read MinValue of LimitID REAGENT_BOTTLE_VOLUME_LIMIT, which is the % of bottle volume that cannot be reach (residual volume)
        '''              AG 22/02/2012 - Residual Volume is calculated based in value of the new SW Parameter REAGENT_DEATH_VOLUME instead of in MinValue of the 
        '''                              LimitID REAGENT_BOTTLE_VOLUME_LIMIT
        '''              SA 02/03/2012 - Implementation changed due to creation of new function to calculate the death volume for a Reagent Bottle. Removed parameter
        '''                              pMinReagentVol, it is not needed
        ''' </remarks>
        Public Function GetReagentTubesByElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, ByVal pTotalReagentVol As Single) _
                                                 As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the different sizes of Reagent bottles currently used
                        Dim reagentBottlesDS As New ReagentTubeTypesDS
                        Dim reagentBottles As New ReagentTubeTypesDelegate

                        resultData = reagentBottles.GetReagentBottles(dbConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            reagentBottlesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)
                            If (reagentBottlesDS.ReagentTubeTypes.Rows.Count > 0) Then
                                'Assign the total required Reagent volume to a local variable
                                Dim remainderVolume As Single = pTotalReagentVol

                                'Assign the total number of available Reagent bottle sizes to a local variable
                                Dim i As Integer = reagentBottlesDS.ReagentTubeTypes.Rows.Count

                                Dim realBottleVol As Single = 0
                                Do While (remainderVolume > 0)
                                    If (i = 1) Then
                                        'Calculate the real volume for the Bottle as TotalBottleVol - BottleDeathVol
                                        resultData = reagentBottles.CalculateDeathVolByBottleType(dbConnection, reagentBottlesDS.ReagentTubeTypes.First.TubeCode, _
                                                                                                  reagentBottlesDS.ReagentTubeTypes.First.Section)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            realBottleVol = reagentBottlesDS.ReagentTubeTypes.First.TubeVolume - CType(resultData.SetDatos, Single)

                                            'If only one Reagent bottle size is available, calculate the number of bottles needed according the real bottle volume
                                            reagentBottlesDS.BeginInit()
                                            reagentBottlesDS.ReagentTubeTypes(0).NumOfBottles = CInt(pTotalReagentVol / realBottleVol)
                                            reagentBottlesDS.EndInit()
                                        End If
                                        remainderVolume = 0
                                    Else
                                        'If several Reagent bottle sizes are available, the total volume is distributted between them using the minimum 
                                        'number of bottles. To do this, big bottles are first used
                                        For x As Integer = 0 To i - 1
                                            'Calculate the real volume for the Bottle as TotalBottleVol - BottleDeathVol
                                            resultData = reagentBottles.CalculateDeathVolByBottleType(dbConnection, reagentBottlesDS.ReagentTubeTypes(x).TubeCode, _
                                                                                                      reagentBottlesDS.ReagentTubeTypes(x).Section)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                realBottleVol = reagentBottlesDS.ReagentTubeTypes(x).TubeVolume - CType(resultData.SetDatos, Single)

                                                While (remainderVolume > realBottleVol)
                                                    '...an additional bottle of this size is added
                                                    reagentBottlesDS.BeginInit()
                                                    reagentBottlesDS.ReagentTubeTypes(x).NumOfBottles = reagentBottlesDS.ReagentTubeTypes(x).NumOfBottles + 1
                                                    reagentBottlesDS.EndInit()

                                                    'Calculate the new remainder volume by subtracting the real volume of one bottle of this size
                                                    remainderVolume = remainderVolume - realBottleVol
                                                End While
                                            Else
                                                'Error calculating the death volume for the Reagent Bottle...
                                                remainderVolume = 0
                                                Exit For
                                            End If
                                        Next

                                        'Verify if all the volume has been distributted between the available bottle sizes. If not, then the remainder volume 
                                        'fits in the smallest available bottle size
                                        If (remainderVolume > 0) Then
                                            'Add an additional bottle of the smallest available size
                                            reagentBottlesDS.BeginInit()
                                            reagentBottlesDS.ReagentTubeTypes(i - 1).NumOfBottles = reagentBottlesDS.ReagentTubeTypes(i - 1).NumOfBottles + 1
                                            reagentBottlesDS.EndInit()

                                            remainderVolume = 0
                                        End If
                                    End If
                                Loop

                                If (Not resultData.HasError) Then
                                    'Move through the different available bottle sizes to save, for the specified Required Element, the number of bottles of each
                                    'available size that are needed (all bottle sizes are saved, including those that are not needed for the Required Element, for
                                    'which field NumTubes is set to zero)...
                                    Dim requiredPosTubes As New WSRequiredElementsTubesDelegate
                                    Dim requiredPosTubesDS As New WSRequiredElementsTubesDS
                                    Dim tmpRequiredPosTubesDS As New WSRequiredElementsTubesDS
                                    Dim requiredPosTubesRow As WSRequiredElementsTubesDS.twksWSRequiredElementsTubesRow = Nothing

                                    Dim j As Integer = 0
                                    Dim noError As Boolean = True
                                    Do While (j < i) AndAlso (noError)
                                        'Add number of bottles of the processed size to DataSet WSRequiredElementsTubesDS
                                        If (j = 0) Then requiredPosTubesRow = requiredPosTubesDS.twksWSRequiredElementsTubes.NewtwksWSRequiredElementsTubesRow()
                                        requiredPosTubesRow.ElementID = pElementID
                                        requiredPosTubesRow.TubeCode = reagentBottlesDS.ReagentTubeTypes(j).TubeCode
                                        requiredPosTubesRow.NumTubes = CType(reagentBottlesDS.ReagentTubeTypes(j).NumOfBottles, Integer)
                                        If (j = 0) Then requiredPosTubesDS.twksWSRequiredElementsTubes.Rows.Add(requiredPosTubesRow)

                                        'Verify if a record for the Required Element and Bottle size already exists
                                        resultData = requiredPosTubes.ExistRequiredElementTube(dbConnection, pElementID, reagentBottlesDS.ReagentTubeTypes(j).TubeCode)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            tmpRequiredPosTubesDS = DirectCast(resultData.SetDatos, WSRequiredElementsTubesDS)

                                            If (tmpRequiredPosTubesDS.twksWSRequiredElementsTubes.Rows.Count = 0) Then
                                                'Add the needed number of bottles of the specified size to the informed Required Element
                                                resultData = requiredPosTubes.AddRequiredElementsTubes(dbConnection, requiredPosTubesDS)
                                            Else
                                                'Update the number of bottles of the specified size that are needed for the Required Element
                                                resultData = requiredPosTubes.UpdateNumTubes(dbConnection, requiredPosTubesDS)
                                            End If
                                        End If

                                        noError = (Not resultData.HasError)
                                        j += 1
                                    Loop
                                End If
                            End If
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.GetReagentTubesByElement", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Go through each table in the WorkSessionResultDS DataSet(Patients-Controls-BlankCalibrators) and processes
        ''' the Selected records according the Sample Class. Once all the selected records have been processed, they are
        ''' added to a new or existing WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, 
        '''                           Calibrators and Blanks that have to be included in the Work Session</param>
        ''' <param name="pCreateEmptyWS">Flag indicating if the status of the WS to create should be EMPTY, which means all
        '''                              Order Tests have to be linked to it but with status OPEN</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter; when it is not informed, it means 
        '''                              a new WorkSession has to be created, otherwise it means the informed WorkSession
        '''                              has to be updated by adding to it new Order Tests</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter needed to indicate in which Analyzer will be executed 
        '''                           the requested Order Tests</param>
        ''' <param name="pCurrentWSStatus">Current Status of the WS to update. Optional parameter needed for WS updation to known if the
        '''                                Work Session Status has to be changed</param>
        ''' <param name="pChangesMade">Flag indicating if some Order Tests have been added/deleted to the WS</param>
        ''' <param name="pLISWithFiles">Flag indicating if the LIS implementation is using the ES library (when FALSE) or plain files 
        '''                             (when TRUE). Optional parameter</param>
        ''' <returns>GlobalDataTO containing error information and, when a new WorkSession has been created, a typed DataSet WorkSessionDS, 
        '''          containing the identifier of the created Work Session and also information about the Analyzer in which the Work Session 
        '''          will be executed</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 16/03/2010 - Each Control required for Test/SampleType is managed as an individual OrderTestID
        '''                              (field ControlNumber was added to Order Tests table and DS to known which OrderTestID
        '''                              corresponds to each Control Number)
        '''              SA 14/04/2010 - Added calls to methods ReorganizeBlanks and ReorganizeCalibrators when there are Order Tests
        '''                              selected to be included in a WorkSession
        '''              SA 26/04/2010 - Pass parameter pWorkSessionID to all Prepare** functions; changed parameter pIncludeOTsInWS by
        '''                              pCreateEmptyWS to indicate when all Order Tests have to be saved as unselected 
        '''              SA 11/06/2010 - Added new optional parameter for the current WS Status, needed in WS updation to change the 
        '''                              status when it is needed. 
        '''              SA 20/10/2010 - Call to UpdateInUseFlag for elements removed from the WS when there is nothing to add to the WS
        '''              SA 22/10/2010 - For Patient Samples, set ToSendFlag = True also for ISE Tests, not only for Standard ones
        '''              SA 19/01/2011 - Once the WorkSession has been created/updated, get all Off-System OrderTests and save the result 
        '''                              values entered for them
        '''              TR 05/08/2011 - After using Lists, set value to nothing to release memory
        '''              SA 20/09/2011 - Added management of incomplete Patient Samples (call UpdateRelatedIncompletedSamples in 
        '''                              class BarCodePositionsWithNoRequestDelegate) before add/update the WorkSession
        '''              SA 12/04/2012 - Added management of Alternative Calibrators when they are INPROCESS and new Order Tests are requested
        '''                              for the same Test and one of the SampleTypes using the Calibrator and not requested in the WS creation 
        '''              SA 10/04/2013 - Added new optional parameter to indicate if the LIS mode in use is plain Files or ES 
        '''              SA 08/05/2013 - When there are not Order Tests with status OPEN to add to the active WS and the function has been called with LIS 
        '''                              implemented using the ES library (pLISWithFiles = FALSE), verify if there are not In Use positions in the WorkSession 
        '''                              that corresponds to the new added Patient elements
        '''              SA 09/09/2013 - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous one still exists
        ''' </remarks>
        Public Function PrepareOrderTestsForWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                               ByVal pCreateEmptyWS As Boolean, Optional ByVal pWorkSessionID As String = "", Optional ByVal pAnalyzerID As String = "", _
                                               Optional ByVal pCurrentWSStatus As String = "", Optional ByVal pChangesMade As Boolean = True, _
                                               Optional ByVal pLISWithFiles As Boolean = False) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDelegate As New OrderTestsDelegate

                        If (pChangesMade) Then
                            'Create/Update Patient Orders... if some Patients will be sent to an Analyzer then
                            'reorganize the Open Patient Order Tests to place selected ones first
                            If (Not pCreateEmptyWS) Then myOrderTestsDelegate.ReorganizePatients(pWSResultDS)

                            myGlobalDataTO = PreparePatientOrders(dbConnection, pWSResultDS, pWorkSessionID, pAnalyzerID)
                            If (Not myGlobalDataTO.HasError) Then
                                'Create/Update Control Orders... if some Controls will be sent to an Analyzer then
                                'reorganize the Open Control Order Tests to place selected ones first
                                If (Not pCreateEmptyWS) Then myOrderTestsDelegate.ReorganizeControls(pWSResultDS)

                                myGlobalDataTO = PrepareControlOrders(dbConnection, pWSResultDS, pWorkSessionID, pAnalyzerID)
                                If (Not myGlobalDataTO.HasError) Then
                                    'Create/Update Calibrator Orders... if some Calibrators will be sent to an Analyzer then
                                    'reorganize the Open Calibrator Order Tests to place selected ones first
                                    If (Not pCreateEmptyWS) Then myOrderTestsDelegate.ReorganizeCalibrators(pWSResultDS)

                                    myGlobalDataTO = PrepareCalibratorOrders(dbConnection, pWSResultDS, pWorkSessionID, pAnalyzerID)
                                    If (Not myGlobalDataTO.HasError) Then
                                        'Create/Update Blank Orders.... if some Blanks will be sent to an Analyzer then 
                                        'reorganize the Open Blank Order Tests to place selected ones first
                                        If (Not pCreateEmptyWS) Then myOrderTestsDelegate.ReorganizeBlanks(pWSResultDS)
                                        myGlobalDataTO = PrepareBlankOrders(dbConnection, pWSResultDS, pWorkSessionID, pAnalyzerID)
                                    End If
                                End If
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Get the logged User
                            'Dim currentSession As New GlobalBase
                            Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                            'Prepare the DataSet needed to add all selected Order Tests to a new or existing Work Session
                            'Add OPEN Blanks and Calibrators....
                            Dim lstWSBlankCalibsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                            lstWSBlankCalibsDS = (From a In pWSResultDS.BlankCalibrators _
                                                 Where String.Compare(a.OTStatus, "OPEN", False) = 0 _
                                                Select a).ToList()

                            Dim myOpenFlag As Boolean = False
                            Dim myOrderTestsDS As New OrderTestsDS
                            Dim myWSOrderTestsDS As New WSOrderTestsDS
                            Dim myWSOrderTestDR As WSOrderTestsDS.twksWSOrderTestsRow

                            For Each blankCalibOrderTest In lstWSBlankCalibsDS
                                myWSOrderTestDR = myWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                                myWSOrderTestDR.OrderTestID = blankCalibOrderTest.OrderTestID

                                'When the WS is created Empty, or when the Order Test has not been selected, it is marked as OPEN
                                If (pCreateEmptyWS) Then
                                    myWSOrderTestDR.OpenOTFlag = True
                                Else
                                    If (Not blankCalibOrderTest.Selected) Then
                                        'If the Order Test is not selected it is not considered OPEN
                                        myWSOrderTestDR.OpenOTFlag = True
                                    Else
                                        myWSOrderTestDR.OpenOTFlag = False
                                    End If
                                End If
                                myOpenFlag = myWSOrderTestDR.OpenOTFlag

                                'All selected Order Tests corresponding to STANDARD Tests are marked to be positioned
                                myWSOrderTestDR.ToSendFlag = (blankCalibOrderTest.TestType = "STD")
                                If (Not blankCalibOrderTest.Selected) And (Not blankCalibOrderTest.IsPreviousOrderTestIDNull) Then
                                    'Not selected Blanks and Calibrators having previous results are marked to not be positioned 
                                    myWSOrderTestDR.ToSendFlag = False
                                End If

                                myWSOrderTestDR.TS_User = loggedUser
                                myWSOrderTestDR.TS_DateTime = DateTime.Now
                                myWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)

                                If (blankCalibOrderTest.SampleClass = "CALIB" And String.Compare(blankCalibOrderTest.TestType, "STD", False) = 0) Then
                                    If (blankCalibOrderTest.RequestedSampleTypes.Trim <> blankCalibOrderTest.SampleType.Trim) Then
                                        'Get all OrderTests belonging to the same Order created for the original requested SampleTypes when
                                        'an Alternative Calibrator is used
                                        myGlobalDataTO = myOrderTestsDelegate.ReadByAlternativeOrderTestID(dbConnection, blankCalibOrderTest.OrderTestID)
                                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                            For Each orderTestsRow As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests
                                                myWSOrderTestDR = myWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                                                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                                                myWSOrderTestDR.OrderTestID = orderTestsRow.OrderTestID
                                                myWSOrderTestDR.ToSendFlag = False
                                                myWSOrderTestDR.OpenOTFlag = myOpenFlag
                                                myWSOrderTestDR.TS_User = loggedUser
                                                myWSOrderTestDR.TS_DateTime = DateTime.Now
                                                myWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
                                            Next
                                        End If
                                    End If
                                End If
                            Next

                            'Special case of Alternative Calibrators when they are INPROCESS
                            Dim lstWSCalibsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                            lstWSCalibsDS = (From a In pWSResultDS.BlankCalibrators _
                                            Where a.SampleClass = "CALIB" _
                                          AndAlso a.OTStatus <> "OPEN" _
                                          AndAlso String.Compare(a.SampleType, a.RequestedSampleTypes, False) <> 0 _
                                           Select a).ToList()

                            For Each alternativeCalib As WorkSessionResultDS.BlankCalibratorsRow In lstWSCalibsDS
                                'Get all OrderTests belonging to the same Order created for the original requested SampleTypes when
                                'an Alternative Calibrator is used - Add only those that have not been linked to the active WorkSession
                                myGlobalDataTO = myOrderTestsDelegate.ReadByAlternativeOrderTestID(dbConnection, alternativeCalib.OrderTestID, pWorkSessionID)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                    For Each orderTestsRow As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests
                                        myWSOrderTestDR = myWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                                        myWSOrderTestDR.WorkSessionID = pWorkSessionID
                                        myWSOrderTestDR.OrderTestID = orderTestsRow.OrderTestID
                                        myWSOrderTestDR.ToSendFlag = False
                                        myWSOrderTestDR.OpenOTFlag = False
                                        myWSOrderTestDR.TS_User = loggedUser
                                        myWSOrderTestDR.TS_DateTime = DateTime.Now
                                        myWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
                                    Next
                                End If
                            Next

                            'Add Controls....
                            Dim myWSOrderTesDelegate As New WSOrderTestsDelegate
                            Dim myCtrlSendingGroup As Integer = 0
                            Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)
                            lstWSControlsDS = (From a In pWSResultDS.Controls _
                                               Where a.OTStatus = "OPEN" _
                                               Select a).ToList()
                            If lstWSControlsDS.Count > 0 Then
                                'TR 19/07/2012 -
                                'Validate if there are CtrlsSendingGroup
                                myGlobalDataTO = myWSOrderTesDelegate.GetMaxCtrlsSendingGroup(dbConnection, pWorkSessionID)
                                If Not myGlobalDataTO.HasError Then
                                    myCtrlSendingGroup = CInt(myGlobalDataTO.SetDatos) + 1
                                End If
                                'TR 19/07/2012 - END
                            End If

                            For Each controlOrderTest In lstWSControlsDS
                                'For Tests having more than one Control there is only one OrderTestID 
                                myWSOrderTestDR = myWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                                myWSOrderTestDR.OrderTestID = controlOrderTest.OrderTestID

                                'When the WS is created Empty, or when the Order Test has not been selected, it is marked as OPEN
                                If (pCreateEmptyWS) Then
                                    myWSOrderTestDR.OpenOTFlag = True
                                Else
                                    myWSOrderTestDR.OpenOTFlag = (Not controlOrderTest.Selected)
                                End If

                                'All selected Order Tests corresponding to STANDARD and ISE Tests are marked to be positioned
                                myWSOrderTestDR.ToSendFlag = (controlOrderTest.TestType = "STD" Or controlOrderTest.TestType = "ISE")

                                'TR 19/07/2012 -Set the sending Group.
                                If myCtrlSendingGroup > 0 Then
                                    myWSOrderTestDR.CtrlsSendingGroup = myCtrlSendingGroup
                                End If
                                'TR 19/07/2012 -END.

                                myWSOrderTestDR.TS_User = loggedUser
                                myWSOrderTestDR.TS_DateTime = DateTime.Now
                                myWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
                            Next

                            'Add Patients...
                            Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                            lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                               Where a.OTStatus = "OPEN" _
                                               Select a).ToList()

                            For Each patientOrderTest In lstWSPatientsDS
                                myWSOrderTestDR = myWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                                myWSOrderTestDR.OrderTestID = patientOrderTest.OrderTestID

                                'When the WS is created Empty, or when the Order Test has not been selected, it is marked as OPEN
                                If (pCreateEmptyWS) Then
                                    myWSOrderTestDR.OpenOTFlag = True
                                Else
                                    myWSOrderTestDR.OpenOTFlag = (Not patientOrderTest.Selected)
                                End If

                                'All selected Order Tests corresponding to STANDARD or ISE Tests are marked to be positioned
                                myWSOrderTestDR.ToSendFlag = (patientOrderTest.TestType = "STD" Or _
                                                              patientOrderTest.TestType = "ISE")

                                myWSOrderTestDR.TS_User = loggedUser
                                myWSOrderTestDR.TS_DateTime = DateTime.Now
                                myWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
                            Next

                            If (myWSOrderTestsDS.twksWSOrderTests.Rows.Count = 0) Then
                                If (Not pLISWithFiles) Then
                                    'Verify if there are not In Use positions in the WorkSession that corresponds to the new added Patient elements
                                    myGlobalDataTO = FindPatientsInNotInUsePositions(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES")
                                End If

                                'There is nothing to add to the WorkSession, update value of InUseFlag to False for all elements 
                                'that were deleted from the Work Session
                                myGlobalDataTO = UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, False, True)

                                'If there is nothing to add to the WorkSession, this is needed to avoid an error when the function
                                'that use the value returned try to get a WorkSessionsDS (returned when AddWorkSession is called)
                                myGlobalDataTO.SetDatos = New WorkSessionsDS
                            Else
                                Dim newWS As Boolean = (pWorkSessionID.Trim = "")

                                If (pLISWithFiles) Then
                                    ''Verify if changes are needed in the table containing the Incomplete Samples
                                    If (Not newWS) Then
                                        Dim resultData As GlobalDataTO = Nothing
                                        Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate
                                        resultData = myBCPosWithNoRequestDelegate.UpdateRelatedIncompletedSamples(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                                                  pWSResultDS, (Not pCreateEmptyWS))
                                        'If an error has happened, pass the error information to the GlobalDataTO to be returned
                                        If (resultData.HasError) Then myGlobalDataTO = resultData
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'Create/update a new WorkSession by adding it all selected Order Tests
                                    myGlobalDataTO = AddWorkSession(dbConnection, myWSOrderTestsDS, newWS, pAnalyzerID, pCurrentWSStatus, Not pLISWithFiles)
                                    If (newWS) Then
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                                            Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                                            If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                                                If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                                                    'Dim myLogAcciones As New ApplicationLogManager()
                                                    GlobalBase.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", _
                                                                                    "WorkSessionsDelegate.PrepareOrderTestsForWS", EventLogEntryType.Error, False)
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            lstWSBlankCalibsDS = Nothing
                            lstWSCalibsDS = Nothing
                            lstWSControlsDS = Nothing
                            lstWSPatientsDS = Nothing
                        End If

                        'Finally, get all requested Off-System Tests and prepare a DS to save the informed results
                        If (Not myGlobalDataTO.HasError) Then
                            Dim lstOrderTestsDS As List(Of WorkSessionResultDS.PatientsRow)
                            lstOrderTestsDS = (From a In pWSResultDS.Patients _
                                              Where String.Compare(a.SampleClass, "PATIENT", False) = 0 _
                                            AndAlso a.TestType = "OFFS" _
                                           Order By a.TestID _
                                             Select a).ToList()

                            If (lstOrderTestsDS.Count > 0) Then
                                Dim currentTest As Integer = -1
                                Dim currentResultType As String = ""
                                Dim currentActiveRangeType As String = ""

                                Dim returnData As New GlobalDataTO
                                Dim myOffSTestsDS As New OffSystemTestsDS
                                Dim myOffSystemTestDelegate As New OffSystemTestsDelegate
                                Dim myOffSystemTestsResultsDS As New OffSystemTestsResultsDS
                                Dim myOFFSTestResultRow As OffSystemTestsResultsDS.OffSystemTestsResultsRow

                                For Each reqOFFS As WorkSessionResultDS.PatientsRow In lstOrderTestsDS
                                    myOFFSTestResultRow = myOffSystemTestsResultsDS.OffSystemTestsResults.NewOffSystemTestsResultsRow
                                    myOFFSTestResultRow.OrderTestID = reqOFFS.OrderTestID
                                    myOFFSTestResultRow.TestID = reqOFFS.TestID
                                    myOFFSTestResultRow.SampleType = reqOFFS.SampleType

                                    If (reqOFFS.TestID <> currentTest) Then
                                        currentTest = reqOFFS.TestID

                                        'Get ResultType and ActiveRangeType for the TestID/SampleType 
                                        returnData = myOffSystemTestDelegate.GetByTestIDAndSampleType(dbConnection, reqOFFS.TestID, reqOFFS.SampleType)
                                        If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                                            myOffSTestsDS = DirectCast(returnData.SetDatos, OffSystemTestsDS)

                                            currentResultType = "QUALTIVE"
                                            currentActiveRangeType = ""
                                            If (myOffSTestsDS.tparOffSystemTests.Rows.Count = 1) Then
                                                currentResultType = myOffSTestsDS.tparOffSystemTests(0).ResultType

                                                If (Not myOffSTestsDS.tparOffSystemTests(0).IsActiveRangeTypeNull) Then
                                                    currentActiveRangeType = myOffSTestsDS.tparOffSystemTests(0).ActiveRangeType
                                                End If
                                            End If
                                        Else
                                            'Error getting the Result Type and ActiveRangeType of the OffSystem Test
                                            Exit For
                                        End If
                                    End If

                                    myOFFSTestResultRow.ResultType = currentResultType
                                    If (String.Compare(currentActiveRangeType, "", False) = 0) Then
                                        myOFFSTestResultRow.SetActiveRangeTypeNull()
                                    Else
                                        myOFFSTestResultRow.ActiveRangeType = currentActiveRangeType
                                    End If

                                    If (reqOFFS.IsOffSystemResultNull) Then
                                        myOFFSTestResultRow.SetResultValueNull()
                                    Else
                                        myOFFSTestResultRow.ResultValue = reqOFFS.OffSystemResult
                                    End If

                                    myOFFSTestResultRow.SampleID = reqOFFS.SampleID
                                    myOFFSTestResultRow.ResultDateTime = Now
                                    myOffSystemTestsResultsDS.OffSystemTestsResults.AddOffSystemTestsResultsRow(myOFFSTestResultRow)
                                Next
                                myOffSystemTestsResultsDS.AcceptChanges()

                                If (Not returnData.HasError) Then
                                    'Add/update/delete results for the requested Off-SystemTests
                                    Dim myResultsDelegate As New ResultsDelegate
                                    returnData = myResultsDelegate.SaveOffSystemResults(dbConnection, myOffSystemTestsResultsDS, pAnalyzerID, pWorkSessionID)

                                    'TR 28/06/2013 - get the last ExportedResults to send to LIS
                                    If (Not returnData.HasError) Then
                                        Me.lastExportedResultsDSAttribute = myResultsDelegate.LastExportedResults
                                    End If
                                    'TR 28/06/2013 -END.

                                End If

                                'If an error has happened, pass the error information to the GlobalDataTO to be returned
                                If (returnData.HasError) Then myGlobalDataTO = returnData
                            End If
                            lstOrderTestsDS = Nothing
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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareOrderTestsForWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of all types (Blanks, Calibrators, Controls and Patient Samples) that have been not 
        ''' sent to positioning yet (those with Status OPEN) and optionally, also the list of Order Tests already positioned 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pReturnOpenAsSelected">Optional parameter to indicate if OPEN Patient Samples, Calibrators and Blanks have to 
        '''                                     be returned as SELECTED (when its value is TRUE) or UNSELECTED (when its value is FALSE)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Blanks or Calibrators
        '''          obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 26/04/2010 - Parameter pWorkSessionID changed from optional to fixed
        '''              SA 28/08/2012 - Added new optional parameter for the Analyzer Identifier
        ''' </remarks>
        Public Shared Function GetOrderTestsForWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                           Optional ByVal pAnalyzerID As String = "", Optional ByVal pReturnOpenAsSelected As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTests As New OrderTestsDelegate
                        Dim wsFinalResultDS As New WorkSessionResultDS

                        'Get Blanks and Calibrators
                        resultData = myOrderTests.GetBlankCalibOrderTests(dbConnection, pWorkSessionID, pAnalyzerID, pReturnOpenAsSelected)
                        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                            Dim wsResultDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                            For Each blankCalibRow As WorkSessionResultDS.BlankCalibratorsRow In wsResultDS.BlankCalibrators
                                wsFinalResultDS.BlankCalibrators.ImportRow(blankCalibRow)
                            Next
                        End If

                        If (Not resultData.HasError) Then
                            'Get Controls
                            resultData = myOrderTests.GetControlOrderTests(dbConnection, pWorkSessionID)
                            If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                                Dim wsResultDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                                For Each controlRow As WorkSessionResultDS.ControlsRow In wsResultDS.Controls
                                    If Not String.Equals(controlRow.OTStatus, "OPEN") Then controlRow.Selected = True
                                    wsFinalResultDS.Controls.ImportRow(controlRow)
                                Next
                            End If

                            If (Not resultData.HasError) Then
                                'Get Patient Samples 
                                resultData = myOrderTests.GetPatientOrderTests(dbConnection, pWorkSessionID, pReturnOpenAsSelected)
                                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                                    Dim wsResultDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                                    For Each patientRow As WorkSessionResultDS.PatientsRow In wsResultDS.Patients
                                        wsFinalResultDS.Patients.ImportRow(patientRow)
                                    Next
                                    wsResultDS = Nothing
                                End If
                            End If
                        End If

                        'Return all obtained Order Tests...
                        If (Not resultData.HasError) Then resultData.SetDatos = wsFinalResultDS
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.GetOrderTestsForWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all data of the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS
        ''' Modified by: AG 14/05/2010 - Reset the new table base lines by well
        '''              SA 18/05/2010 - Added deletion of records in table twksOrderCalculatedTests
        '''              DL 04/06/2010 - Added deletion of records in table twksResults
        '''              SA 07/06/2010 - Added deletion of records in table twksWSExecutionAlarms
        '''              SG 16/07/2010 - Added deletion of records in table twksWSRepetitionsToAdd 
        '''              SG 19/07/2010 - Added deletion of records in table twksResultAlarms
        '''              SA 07/09/2010 - Added code to restart Identity Column OrderTestID in twksOrderTests 
        '''                              once the Reset WS has finished
        '''              RH 15/04/2011 - Remove deletion of records in tables twksWSExecutionAlarms and twksAnalyzerAlarms
        '''                              Now Alarms are kept as historical info so, don't clear it
        '''              SA 19/04/2011 - Save all non free positions in the Reagents Rotor in an internal Virtual Rotor
        '''                              before reset all positions
        '''              SA 20/06/2011 - Added validation of maximum number of QC Results pending to accumulate for the 
        '''                              Tests/SampleTypes with Controls requested in the WorkSession to reset
        '''              SA 20/07/2011 - Added reading of all QC Results in the Analyzer WorkSession and export to QC Module 
        '''              DL 04/08/2011 - Depending of value of User Setting RESETWS_DOWNLOAD_ONLY_PATIENTS, save all non free 
        '''                              positions containing Calibrators, Controls and/or Additional Solutions in the Samples Rotor 
        '''                              in an internal Virtual Rotor before reset all positions
        '''              SA 04/10/2011 - Activated again the code for deleting WSAnalyzerAlarms
        '''             XBC 14/06/2012 - Add pPreserveRotorPositions parameter with the aim to delete Rotor Positions when change of Analyzer
        '''             AG 17/11/2014 BA-2065 new parameter pAnalyzerModel
        '''             AG 15/01/2015 BA-2212
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String, _
                                Optional ByVal pPreserveRotorPositions As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResults As New ResultsDelegate

                        'For TestTypes/Tests/SampleTypes with Controls requested in the WorkSession, verify if they exceed the maximum
                        'number of non cumulated QC Results and in this case, execute the cumulation process
                        resultData = ValidateDependenciesOnResetWS(dbConnection, pWorkSessionID, pAnalyzerID, False)

                        If (Not resultData.HasError) Then
                            'Read all QC Results in the WS and export them to QC Module...
                            resultData = myResults.ExportControlResultsNEW(dbConnection, pWorkSessionID, pAnalyzerID)
                        End If

                        'If (Not resultData.HasError) Then
                        '    'Read all Blank, Calibrator and Patient Results in the WS and export them to HISTORIC Module...
                        '    resultData = myResults.ExportValidatedResultsAndAlarms(dbConnection, pAnalyzerID, pWorkSessionID)
                        'End If

                        If (pPreserveRotorPositions) Then ' XBC 14/06/2012
                            If (Not resultData.HasError) Then
                                'Save content of Reagents Rotor positions in an internal VirtualRotor before reset the Rotor
                                resultData = SaveReagentsRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID)
                            End If

                            If (Not resultData.HasError) Then
                                'Get value of User Setting RESETWS_DOWNLOAD_ONLY_PATIENTS
                                Dim myUserSettingDelegate As New UserSettingsDelegate
                                resultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.RESETWS_DOWNLOAD_ONLY_PATIENTS.ToString())

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    If CType(resultData.SetDatos, Boolean) Then
                                        'Save content of Samples Rotor positions in an internal VirtualRotor before reset the Rotor
                                        resultData = SaveSamplesRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID)
                                    End If
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'Delete Not InUse Rotor Positions for the specified AnalyzerID/WorkSessionID
                            Dim myWSNotInUseRotorPositions As New WSNotInUseRotorPositionsDelegate
                            resultData = myWSNotInUseRotorPositions.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Rotor Positions for the specified AnalyzerID/WorkSessionID
                            Dim myWSRotorContentByPosition As New WSRotorContentByPositionDelegate
                            resultData = myWSRotorContentByPosition.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Tubes for Required WS Elements for the specified AnalyzerID/WorkSessionID
                            Dim myWSRequiredElementsTubes As New WSRequiredElementsTubesDelegate
                            resultData = myWSRequiredElementsTubes.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all relations between Required WS Elements and WS Order Tests for the specified AnalyzerID/WorkSessionID
                            Dim myWSRequiredElemByOrderTest As New WSRequiredElemByOrderTestDelegate
                            resultData = myWSRequiredElemByOrderTest.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then 'GDS - 10/05/2010
                            'Set InUse=False for all Tests included in all WS Order Tests of the specified AnalyzerID/WorkSessionID
                            resultData = Me.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, False)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Required WS Elements for the specified AnalyzerID/WorkSessionID
                            Dim myWSRequiredElements As New WSRequiredElementsDelegate
                            resultData = myWSRequiredElements.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Analyzer Alarms for the specified AnalyzerID/WorkSessionID
                            Dim myWSAnalyzersAlarms As New WSAnalyzerAlarmsDelegate
                            resultData = myWSAnalyzersAlarms.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'AG 29/04/2011 - BLines do not form part of the WorkSession ... so we must not delete them during Reset!
                        'If (Not resultData.HasError) Then
                        '    'Delete all Analyzer Base Lines for the specified AnalyzerID/WorkSessionID
                        '    Dim myWSBaseLines As New WSBLinesDelegate
                        '    resultData = myWSBaseLines.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        'End If

                        If (Not resultData.HasError) Then
                            'Delete all base lines by well for the specified AnalyzerID/WorkSessionID
                            Dim myWSBaseLinesByWell As New WSBLinesByWellDelegate
                            resultData = myWSBaseLinesByWell.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID, pAnalyzerModel) 'AG 17/11/2014 BA-2065 inform analyzerModel
                        End If

                        'AG 15/01/2015 BA-2212
                        If (Not resultData.HasError) Then
                            Dim myWSBaseLines As New WSBLinesDelegate
                            resultData = myWSBaseLines.ResetWSForDynamicBL(dbConnection, pAnalyzerID, pWorkSessionID, pAnalyzerModel)
                        End If
                        'AG 15/01/2015

                        If (Not resultData.HasError) Then
                            'Delete all WS Preparations for the specified AnalyzerID/WorkSessionID
                            Dim myWSPreparations As New WSPreparationsDelegate
                            resultData = myWSPreparations.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Analyzer Readings for the specified AnalyzerID/WorkSessionID
                            Dim myWSReadings As New WSReadingsDelegate
                            resultData = myWSReadings.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'DAO is called directly to avoid circular reference calling Delegate
                        If (Not resultData.HasError) Then
                            'Delete all partial Execution results for the specified AnalyzerID/WorkSessionID 
                            Dim myExecutionsPartialResults As New tcalcExecutionsPartialResultsDAO
                            resultData = myExecutionsPartialResults.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'DAO is called directly to avoid circular reference calling Delegate
                        If (Not resultData.HasError) Then
                            Dim myExecutionsR1Results As New tcalcExecutionsR1ResultsDAO
                            resultData = myExecutionsR1Results.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'AG - Activate this code. These are not alarms, are calculation remarks!!!
                        'RH 15/04/2011 Now Alarms are kept as historical info so, don't clear it
                        If (Not resultData.HasError) Then
                            'Delete all Execution Alarms for the specified AnalyzerID/WorkSessionID 
                            Dim myWSExecutionAlarms As New WSExecutionAlarmsDelegate
                            resultData = myWSExecutionAlarms.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Analyzer Readings for the specified AnalyzerID/WorkSessionID
                            Dim myReactions As New ReactionsRotorDelegate
                            resultData = myReactions.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Execution for the specified AnalyzerID/WorkSessionID 
                            Dim myWSExecutions As New ExecutionsDelegate
                            resultData = myWSExecutions.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete the relation between the Analyzer and the WorkSession
                            Dim myWSAnalyzers As New WSAnalyzersDelegate
                            resultData = myWSAnalyzers.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete relations between Order Tests in the WorkSession and the Calculated Tests included in
                            'the WorkSession in which Formulas are included
                            Dim myCalcOrderTests As New OrderCalculatedTestsDelegate
                            resultData = myCalcOrderTests.DeleteByWorkSession(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all the Repetitions to Add that belong to the WorkSession
                            Dim myWSRepetitionsToAdd As New WSRepetitionsToAddDelegate
                            resultData = myWSRepetitionsToAdd.DeleteAll(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete relation between OrderTests and the WorkSession
                            Dim myWSOrderTests As New WSOrderTestsDelegate
                            resultData = myWSOrderTests.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete the WorkSession
                            Dim myDAO As New TwksWorkSessionsDAO
                            resultData = myDAO.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all result alarms that were included in the WorkSession
                            Dim myResultAlarms As New ResultAlarmsDelegate
                            resultData = myResultAlarms.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Results for the Order Tests included in the WorkSession
                            resultData = myResults.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Order Tests that were included in the WorkSession
                            Dim myOrderTests As New OrderTestsDelegate
                            resultData = myOrderTests.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete Order Demographics for all Orders that were included in the WorkSession
                            Dim myOrdersDemographics As New OrdersDemographicsDelegate
                            resultData = myOrdersDemographics.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Orders that were included in the WorkSession
                            Dim myOrders As New OrdersDelegate
                            resultData = myOrders.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete session from the table twksWSBarcodePositionsWithNoRequest
                            Dim noInformedSampleID As New BarcodePositionsWithNoRequestsDelegate
                            resultData = noInformedSampleID.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Restart the Identity Column OrderTestID in table twksOrderTests
                            Dim myOrderTests As New OrderTestsDelegate
                            resultData = myOrderTests.RestartIdentity(dbConnection)
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change the status of the Analyzer WorkSession from OPEN to EMPTY after deleted all
        ''' Orders and OrderTests included in it
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/09/2011
        ''' Modified by: SA 04/10/2011 - Call UpdateInUseFlag before deleting the WorkSession tables
        ''' </remarks>
        Public Function ChangeWSStatusFromOpenToEmpty(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Set InUse=False for all Tests included in all WS Order Tests of the specified AnalyzerID/WorkSessionID
                        resultData = Me.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, False)

                        If (Not resultData.HasError) Then
                            'Delete relations between Order Tests in the WorkSession and the Calculated Tests included in
                            'the WorkSession in which Formulas are included
                            Dim myCalcOrderTests As New OrderCalculatedTestsDelegate
                            resultData = myCalcOrderTests.DeleteByWorkSession(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete relation between OrderTests and the WorkSession
                            Dim myWSOrderTests As New WSOrderTestsDelegate
                            resultData = myWSOrderTests.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all result alarms that were included in the WorkSession
                            Dim myResultAlarms As New ResultAlarmsDelegate
                            resultData = myResultAlarms.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Results for the Order Tests included in the WorkSession
                            Dim myResults As New ResultsDelegate
                            resultData = myResults.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Order Tests that were included in the WorkSession
                            Dim myOrderTests As New OrderTestsDelegate
                            resultData = myOrderTests.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete Order Demographics for all Orders that were included in the WorkSession
                            Dim myOrdersDemographics As New OrdersDemographicsDelegate
                            resultData = myOrdersDemographics.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Orders that were included in the WorkSession
                            Dim myOrders As New OrdersDelegate
                            resultData = myOrders.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Restart the Identity Column OrderTestID in table twksOrderTests
                            Dim myOrderTests As New OrderTestsDelegate
                            resultData = myOrderTests.RestartIdentity(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Change the WS Status to EMPTY
                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                            resultData = myWSAnalyzersDelegate.UpdateWSStatus(dbConnection, pAnalyzerID, pWorkSessionID, "EMPTY", "OPEN")
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.ChangeWSStatusFromOpenToEmpty", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set or reset the InUse flag for all Elements included in a WorkSession added or reseted
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSession">Identifier of Work Session to be updated</param>
        ''' <param name="pAnalyzerID">Analizer Identifier in which is executed the WS to be updated</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Calibrators that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for all elements 
        '''                               that have been excluded from the active WorkSession    
        '''              SA  22/10/2010 - Update value of InUse flag also for ISE Tests included in the WorkSession
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSession As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update InUse flag for all Standard Tests included in the WorkSession
                        Dim myTest As New TestsDelegate
                        myGlobalDataTO = myTest.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)

                        If (Not myGlobalDataTO.HasError) Then
                            'Update InUse flag for all ISE Tests included in the WorkSession
                            Dim myISETest As New ISETestsDelegate
                            myGlobalDataTO = myISETest.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Update InUse flag for all Calculated Tests included in the WorkSession
                            Dim myCalculatedTest As New CalculatedTestsDelegate
                            myGlobalDataTO = myCalculatedTest.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'AG 10/12/2010: Update InUse flag for all OffSystem Tests included in the WorkSession
                            Dim myOffSystemTest As New OffSystemTestsDelegate
                            myGlobalDataTO = myOffSystemTest.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Update InUse flag for all Test Profiles included in the WorkSession
                            Dim myTestProfile As New TestProfilesDelegate
                            myGlobalDataTO = myTestProfile.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Update InUse flag for all Calibrators included in the WorkSession
                            Dim myCalibrator As New CalibratorsDelegate
                            myGlobalDataTO = myCalibrator.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Update InUse flag for all Controls included in the WorkSession
                            Dim myControl As New ControlsDelegate
                            myGlobalDataTO = myControl.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Update InUse flag for all Reagents included in the WorkSession
                            Dim myReagent As New ReagentsDelegate
                            myGlobalDataTO = myReagent.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Update InUse flag for all Patients included in the WorkSession
                            Dim myPatient As New PatientDelegate
                            myGlobalDataTO = myPatient.UpdateInUseFlag(dbConnection, pWorkSession, pAnalyzerID, pFlag, pUpdateForExcluded)
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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For all cells (in both Analyzer Rotors) containing tubes/bottles marked as NOT IN USE, verify what of them correspond to Elements
        ''' added to the informed WorkSession, calculating and changing the Cell Status and also the Element Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <param name="pFindPatients">When FALSE, not in use rotor positions containing Patient Samples are excluded from the search</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 26/01/2010
        ''' Modified by: SA 16/09/2011 - For Patient Samples, if the Element removed from table of NOT IN USE Position also exists in table of 
        '''                              Incomplete Patient Samples, delete the position also from that table
        '''              SA 07/10/2011 - Function changed from Private to Public due to now it is needed also in function ProcessCompletedPatientSamples 
        '''                              in class BarcodePositionsWithNoRequestDelegate
        '''              AG 03/02/2012 - For positions having a not depleted Reagent, calculate the number of Tests that can be executed according the 
        '''                              current volume of the correspondent bottle
        '''              SA 10/02/2012 - For Additional Solutions, count the number of not depleted bottles to mark the required Element as POS or NOPOS
        '''              SA 15/02/2012 - Process for Reagents updated due implementation of function CalculateReagentStatus has been changed
        '''              SA 16/04/2013 - Added parameter to indicate if the function has to search also Patient Samples elements
        '''              SA 12/01/2015 - BA-1999 ==> For Reagents, remove the call to function CalculateRemainingTests due to the number of tests that can be
        '''                                          executed with the available bottle volume is now calculated and returned by function FindElementIDRelatedWithRotorPosition
        ''' </remarks>
        Public Function FindElementsInNotInUsePosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                       ByVal pFindPatients As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the available Rotor Types for the Analyzer according its model
                        Dim analyzerConfg As New AnalyzerModelRotorsConfigDelegate

                        myGlobalDataTO = analyzerConfg.GetAnalyzerRotorTypes(dbConnection, pAnalyzerID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myAnalyzerModelRotorsConfDS As AnalyzerModelRotorsConfigDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzerModelRotorsConfigDS)

                            Dim testLeft As Integer = 0
                            Dim myElementStatus As String = String.Empty

                            Dim returnDS As WSRotorContentByPositionDS
                            Dim myVirtualRotorPosDS As VirtualRotorPosititionsDS
                            Dim rotorContentByPositionDelegate As New WSRotorContentByPositionDelegate

                            Dim myHisReagentsBottlesDS As HisReagentsBottlesDS
                            Dim myhisReagentsBottlesDelegate As New HisReagentBottlesDelegate

                            Dim notInUseRotorPositionsDelegate As New WSNotInUseRotorPositionsDelegate
                            Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate

                            Dim myWSRequiredElementDS As New WSRequiredElementsDS
                            Dim myRequiredElementsDelegate As New WSRequiredElementsDelegate
                            Dim myElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow = myWSRequiredElementDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow()

                            Dim previousElementID As Integer
                            Dim lstReagentPositions As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                            For Each rotorTypeDS As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow In myAnalyzerModelRotorsConfDS.tfmwAnalyzerModelRotorsConfig.Rows
                                'Search all not IN_USE positions that exists in the Analyzer Rotors for the informed Work Session
                                myGlobalDataTO = notInUseRotorPositionsDelegate.GetRotorPositionsByWorkSession(dbConnection, pAnalyzerID, rotorTypeDS.RotorType, pWorkSessionID, pFindPatients)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myVirtualRotorPosDS = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

                                    If (myVirtualRotorPosDS.tparVirtualRotorPosititions.Count > 0) Then
                                        myGlobalDataTO = myRequiredElementsDelegate.FindElementIDRelatedWithRotorPosition(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                                                          rotorTypeDS.RotorType, myVirtualRotorPosDS, True)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            returnDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)

                                            myGlobalDataTO = rotorContentByPositionDelegate.UpdateNotInUseRotorPosition(dbConnection, returnDS)
                                            If (Not myGlobalDataTO.HasError) Then
                                                If (rotorTypeDS.RotorType = "SAMPLES") Then
                                                    For Each elementsDR As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In returnDS.twksWSRotorContentByPosition
                                                        'Update the element Status for all types of elements positioned in Samples Rotor
                                                        myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, elementsDR.ElementID, elementsDR.ElementStatus)
                                                        If (myGlobalDataTO.HasError) Then Exit For

                                                        'For Patient Samples, if they exist in table of Incomplete Patient Samples, delete the position
                                                        If (elementsDR.TubeContent = "PATIENT" AndAlso (Not elementsDR.IsScannedPositionNull) AndAlso elementsDR.ScannedPosition) Then
                                                            myGlobalDataTO = myBCPosWithNoRequestDelegate.DeletePosition(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                                                         elementsDR.RotorType, elementsDR.CellNumber)
                                                            If (myGlobalDataTO.HasError) Then Exit For
                                                        End If
                                                    Next

                                                ElseIf (rotorTypeDS.RotorType = "REAGENTS") Then
                                                    'PROCESS FOR REAGENTS...
                                                    lstReagentPositions = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In returnDS.twksWSRotorContentByPosition _
                                                                          Where a.TubeContent = "REAGENT" _
                                                                       Order By a.ElementID _
                                                                         Select a).ToList()

                                                    previousElementID = 0
                                                    For Each elementsDR As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstReagentPositions
                                                        'For not depleted scanned Reagents, validate if there is information for the Bottle in the historic of Reagents
                                                        If (elementsDR.Status <> "DEPLETED") Then
                                                            If (Not elementsDR.IsBarCodeInfoNull AndAlso elementsDR.BarcodeStatus = "OK") Then
                                                                myGlobalDataTO = myhisReagentsBottlesDelegate.ReadByBarCode(dbConnection, elementsDR.BarCodeInfo)

                                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                                    myHisReagentsBottlesDS = DirectCast(myGlobalDataTO.SetDatos, HisReagentsBottlesDS)
                                                                    If (myHisReagentsBottlesDS.thisReagentsBottles.Count > 0) Then
                                                                        elementsDR.RealVolume = myHisReagentsBottlesDS.thisReagentsBottles(0).BottleVolume
                                                                        If (myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus = "LOCKED") Then elementsDR.Status = "LOCKED"
                                                                    End If
                                                                End If
                                                            End If

                                                            'If (String.Compare(elementsDR.Status, "DEPLETED", False) <> 0) Then
                                                            '    'TR 01/10/2012 -Validate if barcode is not null to validate if bottle exist on histreagensBottles
                                                            '    If Not elementsDR.IsBarCodeInfoNull AndAlso elementsDR.BarcodeStatus = "OK" Then
                                                            '        'Search on histReagentBottles table 
                                                            '        'Dim myhisReagentsBottlesDelegate As New HisReagentBottlesDelegate
                                                            '        myGlobalDataTO = myhisReagentsBottlesDelegate.ReadByBarCode(dbConnection, elementsDR.BarCodeInfo)
                                                            '        If Not myGlobalDataTO.HasError Then
                                                            '            'Dim myHisReagentsBottlesDS As HisReagentsBottlesDS
                                                            '            myHisReagentsBottlesDS = DirectCast(myGlobalDataTO.SetDatos, HisReagentsBottlesDS)
                                                            '            'Validate if Bottle exist on table.
                                                            '            If myHisReagentsBottlesDS.thisReagentsBottles.Count > 0 Then
                                                            '                'Udate the volume found on historic ASK ?? is update the volume
                                                            '                elementsDR.RealVolume = myHisReagentsBottlesDS.thisReagentsBottles(0).BottleVolume
                                                            '                If myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus = "LOCKED" Then
                                                            '                    'Set status locked
                                                            '                    elementsDR.Status = "LOCKED"
                                                            '                End If
                                                            '            End If
                                                            '        End If
                                                            '    End If
                                                            '    'Get the Reagent status on historic if exist.
                                                            '    'TR 01/10/2012 -END

                                                            'Update information of the Rotor Position: Real Volume and Remaining Tests
                                                            testLeft = 0
                                                            If (Not elementsDR.IsRemainingTestsNumberNull) Then testLeft = elementsDR.RemainingTestsNumber

                                                            myGlobalDataTO = rotorContentByPositionDelegate.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, rotorTypeDS.RotorType, _
                                                                                                                                           elementsDR.CellNumber, elementsDR.Status, elementsDR.RealVolume, _
                                                                                                                                           testLeft, True, False)
                                                            If (myGlobalDataTO.HasError) Then Exit For

                                                            ''Calculate the remaining number of Tests 
                                                            'myGlobalDataTO = myRequiredElementsDelegate.CalculateRemainingTests(dbConnection, pWorkSessionID, elementsDR.ElementID, _
                                                            '                                                                    elementsDR.RealVolume, elementsDR.TubeType)
                                                            'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                            '    testLeft = CType(myGlobalDataTO.SetDatos, Integer)
                                                            '    'Update information of the Rotor Position: Real Volume and Remaining Tests
                                                            '    myGlobalDataTO = rotorContentByPositionDelegate.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, rotorTypeDS.RotorType, _
                                                            '                                                                                   elementsDR.CellNumber, elementsDR.Status, elementsDR.RealVolume, _
                                                            '                                                                                   testLeft, True, False)
                                                            '    If (myGlobalDataTO.HasError) Then Exit For
                                                            'Else
                                                            '    'Error calculating the number of remaining tests for the current bottle volume
                                                            '    Exit For
                                                            'End If
                                                        End If

                                                        If (elementsDR.ElementID <> previousElementID) Then
                                                            'Get the total volume of the Reagent required for the WorkSession
                                                            myGlobalDataTO = myRequiredElementsDelegate.GetRequiredElementData(dbConnection, elementsDR.ElementID)
                                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                                'Save data needed to calculate an update the Reagent Status
                                                                myElementRow = myWSRequiredElementDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
                                                                myElementRow.BeginEdit()
                                                                myElementRow.WorkSessionID = pWorkSessionID
                                                                myElementRow.ElementID = elementsDR.ElementID
                                                                myElementRow.RequiredVolume = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS).twksWSRequiredElements.First.RequiredVolume
                                                                myElementRow.EndEdit()
                                                                myWSRequiredElementDS.twksWSRequiredElements.AddtwksWSRequiredElementsRow(myElementRow)

                                                                previousElementID = elementsDR.ElementID
                                                            Else
                                                                'Error getting details of the required Reagent...
                                                                Exit For
                                                            End If
                                                        End If
                                                    Next

                                                    If (Not myGlobalDataTO.HasError) Then
                                                        ''Update the ElementStatus of all different required Reagents
                                                        For Each reqReagentRow As WSRequiredElementsDS.twksWSRequiredElementsRow In myWSRequiredElementDS.twksWSRequiredElements
                                                            myGlobalDataTO = myRequiredElementsDelegate.CalculateNeededBottlesAndReagentStatus(dbConnection, pAnalyzerID, reqReagentRow, 0)
                                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                                myElementRow = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS.twksWSRequiredElementsRow)

                                                                'Update the Reagent status
                                                                myElementStatus = myElementRow.ElementStatus
                                                                myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, reqReagentRow.ElementID, myElementRow.ElementStatus)
                                                            Else
                                                                'Error calculating the Reagent Status...
                                                                Exit For
                                                            End If
                                                        Next
                                                    End If

                                                    If (Not myGlobalDataTO.HasError) Then
                                                        'PROCESS FOR ADDITIONAL SOLUTIONS...
                                                        lstReagentPositions = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In returnDS.twksWSRotorContentByPosition _
                                                                              Where a.TubeContent <> "REAGENT" _
                                                                             Select a).ToList()

                                                        For Each elementsDR As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstReagentPositions
                                                            If (elementsDR.Status = "DEPLETED") Then
                                                                'The bottle found is depleted; it is needed to know if there are at least a not depleted bottle placed in the Rotor 
                                                                myGlobalDataTO = rotorContentByPositionDelegate.VerifyTubesByElement(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                                                                     elementsDR.RotorType, elementsDR.ElementID, True)
                                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                                    If (DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS).twksWSRotorContentByPosition.Rows.Count = 0) Then
                                                                        'No more bottles: the Additional Solution is positioned but there is not enough volume
                                                                        myElementStatus = "INCOMPLETE"
                                                                    Else
                                                                        'At least one not depleted bottle is positioned: the Additional Solution is positioned
                                                                        myElementStatus = "POS"
                                                                    End If
                                                                Else
                                                                    'Error getting how many not depleted bottles of the additional solutions are placed in the Rotor...
                                                                    Exit For
                                                                End If
                                                            Else
                                                                'There is at least a not depleted tube in the Rotor for the Additional Solution...
                                                                myElementStatus = "POS"
                                                            End If

                                                            'Update the Status of the Required Additional Solution
                                                            myGlobalDataTO = myRequiredElementsDelegate.UpdateStatus(dbConnection, elementsDR.ElementID, myElementStatus)
                                                            If (myGlobalDataTO.HasError) Then Exit For
                                                        Next
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                                If (myGlobalDataTO.HasError) Then Exit For
                            Next
                            lstReagentPositions = Nothing
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.FindElementsInNotInUsePosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search all Patient Samples marked as incomplete (tube scanned but not Order Tests linked to it in the WS) and for each one of them, check if it has
        ''' been completed (some Order Tests have been added to the active WS for the Patient Sample tube)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">>Work Session Identifier</param>
        ''' <param name="pRotorType">Rotor Type</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if at least one of the incomplete Patient Samples was linked to a WS Required 
        '''          Element (TRUE); otherwise, it returns FALSE</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2013
        ''' Modified by: SA 30/05/2013 - After calling function CheckIncompletedPatientSample, verify if the Barcode was linked to an existing WS Required Element 
        '''                              and in this case, update to TRUE the flag to return 
        ''' </remarks>
        Public Function FindPatientsInNotInUsePositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                        ByVal pRotorType As String) As GlobalDataTO
            Dim bcLinkedToElement As Boolean = False
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate
                        myGlobalDataTO = myBCPosWithNoRequestDelegate.ReadByAnalyzerAndWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, pRotorType, False)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myBCPosWithNoRequestDS As BarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)

                            'Get the list of different Barcodes in group of Incomplete Patient Samples
                            Dim additionalPos As Boolean = False
                            Dim myBCInfo As String = String.Empty
                            Dim myDiffSampleTypes As List(Of String)
                            Dim myWSReqElementsDS As New WSRequiredElementsDS
                            Dim mybcRow As List(Of BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)
                            Dim myDiffBarcodes As List(Of String) = (From a In myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests _
                                                                   Select a.BarCodeInfo Distinct).ToList()

                            For Each diffBC As String In myDiffBarcodes
                                myBCInfo = diffBC
                                additionalPos = False

                                'Get the list of different Sample Types for the Barcode
                                myDiffSampleTypes = (From a In myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests _
                                                    Where a.BarCodeInfo = myBCInfo _
                                                   Select a.SampleType Distinct).ToList()

                                If (myDiffSampleTypes.Count = 1) Then
                                    'Select all tubes of the Incomplete Patient Sample and check if they can be completed
                                    mybcRow = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests _
                                               Where a.BarCodeInfo = myBCInfo _
                                              Select a).ToList

                                    myGlobalDataTO = myBCPosWithNoRequestDelegate.CheckIncompletedPatientSample(dbConnection, mybcRow, True, 0)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        If (Not bcLinkedToElement) Then
                                            myWSReqElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)
                                            bcLinkedToElement = (myWSReqElementsDS.twksWSRequiredElements.Rows.Count > 0)
                                        End If
                                    End If
                                Else
                                    'If there are several Sample Types for the same Barcode, check if the Sample has been completed only for 
                                    'tubes with Sample Type informed
                                    For Each diffST As String In myDiffSampleTypes
                                        If (diffST <> String.Empty) Then
                                            'Get all tubes with the Barcode and this Sample Type
                                            mybcRow = (From a As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBCPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests _
                                                      Where a.BarCodeInfo = myBCInfo _
                                                    AndAlso a.SampleType = diffST _
                                                     Select a).ToList

                                            myGlobalDataTO = myBCPosWithNoRequestDelegate.CheckIncompletedPatientSample(dbConnection, mybcRow, True, myDiffSampleTypes.Count)
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                If (Not bcLinkedToElement) Then
                                                    myWSReqElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)
                                                    bcLinkedToElement = (myWSReqElementsDS.twksWSRequiredElements.Rows.Count > 0)
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            Next
                            mybcRow = Nothing
                            myDiffBarcodes = Nothing
                            myDiffSampleTypes = Nothing
                        End If

                        myGlobalDataTO.SetDatos = bcLinkedToElement
                        myGlobalDataTO.HasError = False

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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.FindPatientsInNotInUsePositions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For each Control/Lot and Test/SampleType requested in the specified WorkSession, verify how many not Excluded QC Results  
        ''' are pending to accumulate. If the obtained number is greater than the value specified for the General Setting 
        ''' MAX_QCRESULTS_TO_ACCUMULATE, then add the Control and Test/SampleType to the list of affected Elements 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pToShowWarningScreen">When TRUE, it indicates the auxiliary screen of affected elements has to be shown
        '''                                    When FALSE, the process is executed but not warning message is shown</param>
        ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected elements</returns>
        ''' <remarks>
        ''' Created by:  SA 17/06/2011
        ''' Modified by: SA 15/06/2012 - Added parameter for the AnalyzerID; called new implementation of QC functions 
        '''              TR 11/06/2013 - Passed the open DB connection to tables PreloadedMasterData and Multilanguage to avoid errors when
        '''                              this function is called from the Update Version process.
        '''              SA 18/11/2013 - Passed the open DB Connection to all called functions to avoid locked errors when this function is 
        '''                              called from the Update Version process.
        '''              SA 11/11/2014 - BA-1885 ==> Replaced call to function GetGeneralSettingValue in GeneralSettingsDelegate by call to function 
        '''                                          ReadNumValueByParameterName in SwParametersDelegate due to MAX_QCRESULTS_TO_ACCUMULATE has been
        '''                                          moved from General Settings table to Sw Parameters table
        ''' </remarks>
        Public Function ValidateDependenciesOnResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                      ByVal pAnalyzerID As String, ByVal pToShowWarningScreen As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            'TR Set the value = nothing and do not declare as new.
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myQCSummaryByTestDS As New QCSummaryByTestSampleDS
                Dim myDependeciesElementsDS As New DependenciesElementsDS

                'Only if the function was called from the ResetWS process, a DB Transaction is opened
                Dim executeProcess As Boolean = True
                If (Not pToShowWarningScreen) Then
                    executeProcess = False

                    resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                        executeProcess = (Not dbConnection Is Nothing)
                    End If
                End If

                If (Not executeProcess) Then
                    'Return an empty DS
                    resultData.SetDatos = myDependeciesElementsDS
                Else
                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                    resultData = myOrderTestsDelegate.GetControlOrderTests(dbConnection, pWorkSessionID)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myWSResultsDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                        If (myWSResultsDS.Controls.Rows.Count > 0) Then
                            'Get Icon used for Controls 
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                            Dim imageControl As Byte() = preloadedDataConfig.GetIconImage("CTRL", dbConnection)

                            'Get the label for Results in the Current Application Language
                            'Dim currentLanguageGlobal As New GlobalBase
                            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                            Dim myResultsLabel As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Results", _
                                                                                                        GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString)

                            'Get value of the SW Parameter for the allowed maximum number of non cumulated QC Results for the Control/Lot and the Test/SampleType
                            Dim myMaxQCResults As Integer = 0

                            resultData = SwParametersDelegate.ReadNumValueByParameterName(dbConnection, GlobalEnumerates.SwParameters.MAX_QCRESULTS_TO_ACCUMULATE.ToString, Nothing)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myMaxQCResults = Convert.ToInt32(DirectCast(resultData.SetDatos, Single))

                                'Get all different Controls requested in the WS
                                Dim lstDiffControls As List(Of Integer) = (From a As WorkSessionResultDS.ControlsRow In myWSResultsDS.Controls _
                                                                         Select a.ControlID Distinct).ToList

                                Dim openResults As Integer = 0
                                Dim additionalInfo As String = String.Empty
                                Dim myQCResultsDelegate As New QCResultsDelegate
                                Dim myQCHistTestControlsDS As New HistoryTestControlLotsDS
                                Dim myQCHistTestControlsDelegate As New HistoryTestControlLotsDelegate
                                Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow
                                Dim myQCSummaryByTestsRow As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow

                                Dim lstDiffTestSamples As New List(Of WorkSessionResultDS.ControlsRow)
                                For Each requestedControl As Integer In lstDiffControls
                                    additionalInfo = String.Empty
                                    myQCSummaryByTestDS.Clear()

                                    'Get all Tests/SampleTypes for which the Control was requested in the active WorkSession
                                    lstDiffTestSamples = (From b As WorkSessionResultDS.ControlsRow In myWSResultsDS.Controls _
                                                         Where b.ControlID = requestedControl _
                                                        Select b).ToList

                                    For Each requestedTestSample As WorkSessionResultDS.ControlsRow In lstDiffTestSamples
                                        'Get the identifiers in QC Module for the Control/Lot and the Test/SampleType
                                        resultData = myQCHistTestControlsDelegate.GetQCIDsForTestAndControlNEW(dbConnection, requestedTestSample.TestType, requestedTestSample.TestID, _
                                                                                                               requestedTestSample.SampleType, requestedTestSample.ControlID, _
                                                                                                               requestedTestSample.LotNumber)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myQCHistTestControlsDS = DirectCast(resultData.SetDatos, HistoryTestControlLotsDS)

                                            If (myQCHistTestControlsDS.tqcHistoryTestControlLots.Rows.Count > 0) Then
                                                'Count how many not excluded QC Results are pending to Cumulate for the Control/Lot and the Test/SampleType
                                                resultData = myQCResultsDelegate.CountNonCumulatedResultsNEW(dbConnection, myQCHistTestControlsDS.tqcHistoryTestControlLots(0).QCTestSampleID, _
                                                                                                             myQCHistTestControlsDS.tqcHistoryTestControlLots(0).QCControlLotID, _
                                                                                                             pAnalyzerID)

                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    openResults = DirectCast(resultData.SetDatos, Integer)
                                                    If (openResults > myMaxQCResults) Then
                                                        If (pToShowWarningScreen) Then
                                                            'Add the Control to the list of affected Elements
                                                            If (String.Compare(additionalInfo, String.Empty, False) <> 0) Then additionalInfo &= Environment.NewLine
                                                            additionalInfo &= requestedTestSample.TestName & " [" & requestedTestSample.SampleType & "]: " & openResults.ToString & " " & myResultsLabel
                                                        Else
                                                            'Add the QCTestSampleID to the DS needed to execute the Cumulation process
                                                            myQCSummaryByTestsRow = myQCSummaryByTestDS.QCSummaryByTestSampleTable.NewQCSummaryByTestSampleTableRow()
                                                            myQCSummaryByTestsRow.Selected = True
                                                            myQCSummaryByTestsRow.QCTestSampleID = myQCHistTestControlsDS.tqcHistoryTestControlLots(0).QCTestSampleID
                                                            myQCSummaryByTestDS.QCSummaryByTestSampleTable.AddQCSummaryByTestSampleTableRow(myQCSummaryByTestsRow)
                                                        End If
                                                    End If
                                                Else
                                                    'Error getting the number of non Cumulated (nor Excluded) for the Test/SampleType and Control/LotNumber
                                                    Exit For
                                                End If
                                            End If
                                        Else
                                            'Error getting the identifiers in QC Module for the Test/SampleType and Control/LotNumber
                                            Exit For
                                        End If
                                    Next

                                    If (Not resultData.HasError) Then
                                        If (pToShowWarningScreen) Then
                                            If (String.Compare(additionalInfo, String.Empty, False) <> 0) Then
                                                'Add data to DS of affected Elements
                                                myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                                                myDependenciesElementsRow.Type = imageControl
                                                myDependenciesElementsRow.Name = lstDiffTestSamples.First.ControlName
                                                myDependenciesElementsRow.FormProfileMember = additionalInfo
                                                myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                                            End If
                                        Else
                                            If (myQCHistTestControlsDS.tqcHistoryTestControlLots.Rows.Count > 0 AndAlso myQCSummaryByTestDS.QCSummaryByTestSampleTable.Rows.Count > 0) Then
                                                'Cumulate the QC Results for the Control/Lot and all Tests/Sample Types that exceed the max. number 
                                                resultData = myQCResultsDelegate.CumulateControlResultsNEW(dbConnection, myQCSummaryByTestDS, myQCHistTestControlsDS.tqcHistoryTestControlLots(0).QCControlLotID, _
                                                                                                           pAnalyzerID)
                                                If (resultData.HasError) Then Exit For
                                            End If
                                        End If
                                    Else
                                        'There was an error when process the Tests/Sample Types
                                        Exit For
                                    End If
                                Next

                                If (pToShowWarningScreen) Then
                                    'Return the DS with the list of affected Elements
                                    If (Not resultData.HasError) Then resultData.SetDatos = myDependeciesElementsDS
                                Else
                                    'Commit or Rollback the DB Connection
                                    If (Not resultData.HasError) Then
                                        'When the Database Connection was opened locally, then the Commit is executed
                                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                    Else
                                        'When the Database Connection was opened locally, then the Rollback is executed
                                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                    End If
                                End If
                                lstDiffControls = Nothing
                            End If
                        Else
                            'Return an empty DS
                            resultData.SetDatos = myDependeciesElementsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.ValidateDependenciesOnResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Calculate the WorkSession Remaining Time
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorInitialize">Flag indicating if the time for Rotor Initialization before RUNNING has to be 
        '''                                added to the obtained WS Remaining Time</param>
        ''' <returns>GlobalDataTO containing a Single value with the maximum remaining time for all Pending and InProcess 
        '''          Executions for the informed Analyzer and WorkSession</returns>
        ''' <remarks>
        ''' Created by:  AG 21/09/2010
        ''' Modified by: SA 25/10/2011 - Changed the implementation
        '''              TR 02/11/2011 - Validate if total time for Standard Tests and/or total time for ISE Tests have been calculated
        '''                              before calculate the total time
        ''' </remarks>
        Public Function CalculateTimeRemaining(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                               ByVal pRotorInitialize As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cycleMachineTime As Single = 0

                        Dim myParametersDS As New ParametersDS
                        Dim mySWParametersDelegate As New SwParametersDelegate

                        'Get time needed for each Analyzer Cycle
                        resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString, True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                            If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then cycleMachineTime = Convert.ToSingle(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                        End If

                        Dim initialTime As Single = 0
                        Dim stdTestsTime As Single = 0
                        Dim iseTestsTime As Single = 0
                        Dim totalTestsTime As Single = 0

                        If (Not resultData.HasError) Then
                            If (pRotorInitialize) Then
                                'Get number of Cycles needed for Rotor initialization
                                Dim initialRotorCycles As Integer = 0

                                resultData = mySWParametersDelegate.GetParameterByAnalyzer(dbConnection, pAnalyzerID, GlobalEnumerates.SwParameters.WAITING_CYCLES_BEFORE_RUNNING.ToString, True)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then initialRotorCycles = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)

                                    'Calculate the initial time
                                    initialTime = initialRotorCycles * cycleMachineTime
                                End If
                            End If
                        End If

                        Dim myExecutionsDelegate As New ExecutionsDelegate
                        If (Not resultData.HasError) Then
                            'Get the maximum remaining time between all Pending and In Process Standard Preparations
                            resultData = myExecutionsDelegate.GetSTDTestsTimeEstimation(dbConnection, pWorkSessionID, pAnalyzerID, cycleMachineTime)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                stdTestsTime = Convert.ToSingle(resultData.SetDatos)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'get the maximum remaining time between all pending and in process ise preparations
                            resultData = myExecutionsDelegate.GetISETestsTimeEstimation(dbConnection, pWorkSessionID, pAnalyzerID, cycleMachineTime)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                iseTestsTime = Convert.ToSingle(resultData.SetDatos)
                            End If
                        End If
                        'TR 04/06/2012
                        If (Not resultData.HasError) Then
                            'TR 27/07/2012 -Validate if the recived time is greather than 0 before
                            'Asigne the totalTesttime Value
                            If (stdTestsTime > 0 OrElse iseTestsTime > 0) Then
                                totalTestsTime = initialTime
                                If stdTestsTime = 0 AndAlso iseTestsTime > 0 Then
                                    totalTestsTime += iseTestsTime
                                ElseIf stdTestsTime > 0 AndAlso iseTestsTime <= 0 Then
                                    totalTestsTime += stdTestsTime
                                ElseIf stdTestsTime > 0 AndAlso iseTestsTime > 0 Then

                                    If (stdTestsTime > iseTestsTime) Then
                                        totalTestsTime += stdTestsTime
                                    Else
                                        totalTestsTime += iseTestsTime
                                    End If
                                    Debug.Print("[BIO: " & stdTestsTime & "] -- " & "[ISE:" & iseTestsTime.ToString() & "]")
                                    Debug.Print("Total Time: " & totalTestsTime.ToString())

                                End If
                            End If

                            resultData.SetDatos = totalTestsTime
                            resultData.HasError = False
                        End If

                        'TR 04/06/2012 -END.

                        'If (Not resultData.HasError) Then
                        '    'Validate if total time for Standard Tests and/or total time for ISE Tests have been calculated before calculate the total time
                        '    If (stdTestsTime <> 0 OrElse iseTestsTime <> 0) Then
                        '        totalTestsTime = initialTime
                        '        If (stdTestsTime > iseTestsTime) Then
                        '            totalTestsTime += stdTestsTime
                        '        Else
                        '            totalTestsTime += iseTestsTime
                        '        End If
                        '    End If

                        '    resultData.SetDatos = totalTestsTime
                        '    resultData.HasError = False
                        'End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.CalculateTimeRemaining", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns a boolean indicating if the WorkSession has been started or not
        ''' When the WS is started, parameter pPendingExecutions is updated (to True when there are pending executions)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWSExecutionsNumber" ></param>
        ''' <param name="pPendingExecutionsLeft" ></param>
        ''' <returns>GlobalDataTO containing a boolean value that indicates if the WS has been started (TRUE) or not (FALSE)</returns>
        ''' <remarks>
        ''' Created by:  AG 16/05/2011
        ''' Modified by: AG 17/06/2011 - Implementation changed
        ''' </remarks>
        Public Function StartedWorkSessionFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                               ByRef pWSExecutionsNumber As Integer, ByRef pPendingExecutionsLeft As Boolean) As GlobalDataTO
            Dim finalResult As Boolean = False 'WS not started! (default value)
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim execDS As New ExecutionsDS
                        Dim myExec As New ExecutionsDelegate

                        resultData = myExec.GetExecutionByWorkSession(dbConnection, pAnalyzerID, pWorkSessionID, False)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            execDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                            pWSExecutionsNumber = execDS.twksWSExecutions.Rows.Count
                        End If

                        If (pWSExecutionsNumber > 0) Then
                            Dim myDAO As New TwksWorkSessionsDAO

                            resultData = myDAO.GetByWorkSession(dbConnection, pWorkSessionID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim wsDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)

                                If (wsDS.twksWorkSessions.Rows.Count > 0) Then
                                    'If ws StartData is NULL update it
                                    If (wsDS.twksWorkSessions(0).IsStartDateTimeNull) Then
                                        finalResult = False 'WS not started
                                    Else
                                        finalResult = True
                                    End If

                                    'Update pPendingExecutionsLeft flag
                                    resultData = myExec.GetExecutionsByStatus(dbConnection, pAnalyzerID, pWorkSessionID, "PENDING", True)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        execDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                                        If (execDS.twksWSExecutions.Rows.Count > 0) Then
                                            pPendingExecutionsLeft = True
                                        End If
                                    End If
                                End If
                            End If
                            'AG 17/06/2011
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionDelegate.StartedWorkSessionFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            resultData.SetDatos = finalResult
            Return resultData
        End Function

        ''' <summary>
        ''' Set the start date and time when the WorkSession is started
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' </remarks>
        Public Function UpdateStartDateTime(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TwksWorkSessionsDAO
                        resultData = myDAO.UpdateStartDateTime(dbConnection, pWorkSessionID)

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

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionDelegate.UpdateStartDateTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get the list of Order Tests of Patients/Controls that have been not performed into the current WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS with all the LIS Order tests not processed</returns>
        ''' <remarks>
        ''' Created by:  XB 22/04/2013
        ''' Modified by: XB 16/05/2013 - Only take the max Rerun to Save it for the future WS
        '''              XB 09/09/2013 - Preserve sorting according to 'CreationOrder' on saved worksession (bugstracking #1283)
        ''' </remarks>
        Public Function GetOrderTestsForLISReset(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        Dim mySavedDS As New SavedWSOrderTestsDS
                        resultData = myOrderTestsDAO.GetOrderTestsForLISReset(dbConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            'myDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                            Dim wsResultDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)

                            ' XB 09/09/2013
                            Dim lstWSRow As New List(Of WorkSessionResultDS.PatientsRow)
                            lstWSRow = (From a As WorkSessionResultDS.PatientsRow In wsResultDS.Patients _
                                        Order By a.CreationOrder Ascending _
                                        Select a).ToList
                            If (lstWSRow.Count > 0) Then

                                ' PATIENTS
                                'For Each patientRow As WorkSessionResultDS.PatientsRow In wsResultDS.Patients
                                For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSRow
                                    ' XB 09/09/2013

                                    ' Mount special fields : CalcTestIDs & CalcTestNames
                                    'Get information of tests included in the formula of Calculated Tests in the WorkSession
                                    'to inform fields CalcTestID and CalcTestName for each one of them
                                    Dim myCalcOrderTestsDS As OrderCalculatedTestsDS
                                    Dim myCalcOrderTestsDelegate As New OrderCalculatedTestsDelegate

                                    resultData = myCalcOrderTestsDelegate.ReadByWorkSession(dbConnection, pWorkSessionID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myCalcOrderTestsDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)

                                        Dim calcIDList As String = ""
                                        Dim calcNameList As String = ""
                                        Dim currentOrderTest As Integer = -1

                                        Dim lstPatientOT As List(Of WorkSessionResultDS.PatientsRow)
                                        For Each otInCalcTest As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myCalcOrderTestsDS.twksOrderCalculatedTests
                                            If (otInCalcTest.OrderTestID <> currentOrderTest) Then
                                                If (currentOrderTest <> -1) Then
                                                    lstPatientOT = (From a In wsResultDS.Patients _
                                                                    Where a.OrderTestID = currentOrderTest _
                                                                    Select a).ToList

                                                    If (lstPatientOT.Count = 1) Then
                                                        lstPatientOT.First.CalcTestID = calcIDList
                                                        lstPatientOT.First.CalcTestName = calcNameList
                                                    End If

                                                    calcIDList = ""
                                                    calcNameList = ""
                                                End If
                                                currentOrderTest = otInCalcTest.OrderTestID
                                            End If

                                            If Not String.Equals(calcIDList.Trim, String.Empty) Then
                                                calcIDList &= ", "
                                                calcNameList &= ", "
                                            End If
                                            calcIDList &= otInCalcTest.CalcTestID
                                            calcNameList &= otInCalcTest.CalcTestLongName
                                        Next

                                        '...process the last Order Test
                                        If Not String.Equals(calcIDList.Trim, String.Empty) Then
                                            lstPatientOT = (From a In wsResultDS.Patients _
                                                            Where a.OrderTestID = currentOrderTest _
                                                            Select a).ToList
                                            If (lstPatientOT.Count = 1) Then
                                                lstPatientOT.First.CalcTestID = calcIDList
                                                lstPatientOT.First.CalcTestName = calcNameList
                                            End If
                                        End If
                                        lstPatientOT = Nothing
                                    End If


                                    ' Create a new tSaveRow To Add
                                    Dim insertNewItem As Boolean = True
                                    If mySavedDS Is Nothing Then
                                        mySavedDS = New SavedWSOrderTestsDS

                                        ' XB 16/05/2013 - Take only the Max Rerun in case existing several reruns
                                    Else
                                        Dim lstSavedRowExists As New List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
                                        lstSavedRowExists = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedDS.tparSavedWSOrderTests _
                                                             Where a.SampleClass = patientRow.SampleClass _
                                                             And a.SampleID = patientRow.SampleID _
                                                             And a.StatFlag = patientRow.StatFlag _
                                                             And a.TestType = patientRow.TestType _
                                                             And a.TestID = patientRow.TestID _
                                                             And a.SampleType = patientRow.SampleType _
                                                             Select a).ToList

                                        If (lstSavedRowExists.Count > 0) Then
                                            insertNewItem = False
                                            If Not patientRow.IsRerunNumberNull AndAlso patientRow.RerunNumber > lstSavedRowExists(0).RerunNumber Then
                                                lstSavedRowExists(0).RerunNumber = patientRow.RerunNumber
                                                lstSavedRowExists(0).AwosID = patientRow.AwosID
                                                lstSavedRowExists(0).ESOrderID = patientRow.ESOrderID
                                                lstSavedRowExists(0).ESPatientID = patientRow.ESPatientID
                                            End If
                                        End If
                                        ' XB 16/05/2013
                                    End If

                                    If insertNewItem Then
                                        Dim mySavedRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow = mySavedDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow
                                        With mySavedRow
                                            .BeginEdit()
                                            .SampleClass = patientRow.SampleClass
                                            .SampleID = patientRow.SampleID
                                            .StatFlag = patientRow.StatFlag
                                            .TestType = patientRow.TestType
                                            .TestID = patientRow.TestID
                                            .SampleType = patientRow.SampleType
                                            .TubeType = patientRow.TubeType
                                            If patientRow.IsNumReplicatesNull Then
                                                .ReplicatesNumber = 1
                                            Else
                                                .ReplicatesNumber = patientRow.NumReplicates
                                            End If
                                            .CreationOrder = patientRow.CreationOrder
                                            .TestName = patientRow.TestName
                                            .FormulaText = patientRow.CalcTestFormula
                                            .PatientIDType = patientRow.SampleIDType
                                            If Not patientRow.IsAwosIDNull Then .AwosID = patientRow.AwosID
                                            If Not patientRow.IsSpecimenIDNull Then .SpecimenID = patientRow.SpecimenID
                                            If Not patientRow.IsESOrderIDNull Then .ESOrderID = patientRow.ESOrderID
                                            If Not patientRow.IsLISOrderIDNull Then .LISOrderID = patientRow.LISOrderID
                                            If Not patientRow.IsESPatientIDNull Then .ESPatientID = patientRow.ESPatientID
                                            If Not patientRow.IsLISPatientIDNull Then .LISPatientID = patientRow.LISPatientID
                                            .CalcTestIDs = patientRow.CalcTestID
                                            .CalcTestNames = patientRow.CalcTestName
                                            .ExternalQC = patientRow.ExternalQC
                                            If Not patientRow.IsRerunNumberNull Then .RerunNumber = patientRow.RerunNumber
                                            .EndEdit()
                                        End With
                                        mySavedDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(mySavedRow)
                                        mySavedDS.AcceptChanges()
                                    End If

                                Next

                            End If


                            ' CONTROLS
                            For Each controlRow As WorkSessionResultDS.ControlsRow In wsResultDS.Controls

                                ' Create a new tSaveRow To Add
                                If mySavedDS Is Nothing Then
                                    mySavedDS = New SavedWSOrderTestsDS
                                End If
                                Dim mySavedRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow = mySavedDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow
                                With mySavedRow
                                    .BeginEdit()
                                    .SampleClass = controlRow.SampleClass
                                    .StatFlag = False
                                    .TestType = controlRow.TestType
                                    .TestID = controlRow.TestID
                                    .SampleType = controlRow.SampleType
                                    .TubeType = controlRow.TubeType
                                    If controlRow.IsNumReplicatesNull Then
                                        .ReplicatesNumber = 1
                                    Else
                                        .ReplicatesNumber = controlRow.NumReplicates
                                    End If
                                    .TestName = controlRow.TestName
                                    '.ControlID = controlRow.ControlID  MUST BE NULL to be loaded from LIS OrderDownload successfully !!!
                                    If Not controlRow.IsAwosIDNull Then .AwosID = controlRow.AwosID
                                    If Not controlRow.IsSpecimenIDNull Then .SpecimenID = controlRow.SpecimenID
                                    If Not controlRow.IsESOrderIDNull Then .ESOrderID = controlRow.ESOrderID
                                    If Not controlRow.IsLISOrderIDNull Then .LISOrderID = controlRow.LISOrderID
                                    If Not controlRow.IsESPatientIDNull Then .ESPatientID = controlRow.ESPatientID
                                    If Not controlRow.IsLISPatientIDNull Then .LISPatientID = controlRow.LISPatientID
                                    .EndEdit()
                                End With
                                mySavedDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(mySavedRow)
                                mySavedDS.AcceptChanges()

                            Next

                        End If

                        resultData.SetDatos = mySavedDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.GetOrderTestsForLISReset", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update to FALSE the INUSE flag of an specific TestType / TestID if, after an accepted LIS Cancel, there are not other Order Tests 
        ''' for the TestType / TestID in the active Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 08/05/2013
        ''' Modified by: SA 09/05/2013 - Changed the function implementation to simplify it: TestType and TestID are received as parameters and the update
        '''                              is executed according the TestType. Validations are executed previously, then when this function is called is just
        '''                              to execute the update of the INUSE field
        ''' </remarks>
        Public Function UpdateInUseFlagOnLISCancellation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Select Case pTestType
                            Case "STD"
                                Dim myTestDlg As New TestsDelegate
                                resultData = myTestDlg.UpdateInUseByTestID(dbConnection, pTestID, False)

                            Case "ISE"
                                Dim myIseDlg As New ISETestsDelegate
                                resultData = myIseDlg.UpdateInUseByTestID(dbConnection, pTestID, False)
                            Case "CALC"
                                Dim myCalcDlg As New CalculatedTestsDelegate
                                resultData = myCalcDlg.UpdateInUseByTestID(dbConnection, pTestID, False)

                            Case "OFFS"
                                Dim myOffsDlg As New OffSystemTestsDelegate
                                resultData = myOffsDlg.UpdateInUseByTestID(dbConnection, pTestID, False)
                        End Select

                        'If Not pOTinCurrentWS Is Nothing Then
                        '    Dim myTestDlg As New TestsDelegate
                        '    Dim myIseDlg As New ISETestsDelegate
                        '    Dim myCalcDlg As New CalculatedTestsDelegate
                        '    Dim myOffsDlg As New OffSystemTestsDelegate

                        '    Dim linqRes As New List(Of OrderTestsDS.twksOrderTestsRow)
                        '    For Each myLISInfoRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pOTLISInfoDS.twksOrderTestsLISInfo.Rows
                        '        If Not myLISInfoRow.IsTestIDNull AndAlso Not myLISInfoRow.IsTestTypeNull Then
                        '            'Search if the TestID, TestType exists in current WS, else update the InUse field to FALSE
                        '            linqRes = (From a As OrderTestsDS.twksOrderTestsRow In pOTinCurrentWS.twksOrderTests _
                        '                       Where a.TestID = myLISInfoRow.TestID AndAlso a.TestType = myLISInfoRow.TestType Select a).ToList

                        '            If linqRes.Count = 0 Then
                        '                Select Case myLISInfoRow.TestType
                        '                    Case "STD"
                        '                        resultData = myTestDlg.UpdateInUseByTestID(dbConnection, myLISInfoRow.TestID, False)

                        '                    Case "ISE"
                        '                        resultData = myIseDlg.UpdateInUseByTestID(dbConnection, myLISInfoRow.TestID, False)

                        '                    Case "OFFS"
                        '                        resultData = myOffsDlg.UpdateInUseByTestID(dbConnection, myLISInfoRow.TestID, False)

                        '                    Case "CALC"
                        '                        resultData = myCalcDlg.UpdateInUseByTestID(dbConnection, myLISInfoRow.TestID, False)

                        '                    Case Else
                        '                        'Do nothing
                        '                End Select
                        '            End If
                        '        End If
                        '    Next
                        '    linqRes = Nothing
                        'End If

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.UpdateInUseFlagOnLISCancellation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Private Functions"
       ''' <summary>
        ''' Get all non free positions in the Reagents Rotor and save them in the internal Virtual Rotor. If this special
        ''' Virtual Rotor does not exist, then it is created 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA    19/04/2011
        ''' Modified by: AG/SA 06/02/2012 - Before save the non free positions in the Internal Reagents Virtual Rotor, for cells having Status 
        '''                                 DEPLETED or FEW, save also the status; for the rest of positions, save Status as NULL
        ''' </remarks>
        Private Function SaveReagentsRotorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                    ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if the Internal Virtual Rotor for Reagents has been created, and in this case, get the ID
                        Dim myVirtualRotorsDelegate As New VirtualRotorsDelegate

                        resultData = myVirtualRotorsDelegate.GetVRotorsByRotorType(dbConnection, "REAGENTS", True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myVirtualRotorsDS As VirtualRotorsDS = DirectCast(resultData.SetDatos, VirtualRotorsDS)

                            Dim myInternalVRotorID As Integer = -1
                            If (myVirtualRotorsDS.tparVirtualRotors.Count = 1) Then
                                myInternalVRotorID = myVirtualRotorsDS.tparVirtualRotors.First.VirtualRotorID
                            End If

                            'Get the content of all positions in the Reagents Rotor (InUse and Not InUse positions) to save them
                            'as an special Internal Virtual Rotor(Reagents Rotor is normally reused, without downloading it)
                            Dim myWSRotorContentByPosition As New WSRotorContentByPositionDelegate

                            resultData = myWSRotorContentByPosition.GetReagentsRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID, myInternalVRotorID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myReagentsPositions As VirtualRotorPosititionsDS = DirectCast(resultData.SetDatos, VirtualRotorPosititionsDS)
                                If (myReagentsPositions.tparVirtualRotorPosititions.Count > 0) Then
                                    'Get all positions with Status different of DEPLETED and FEW and set Status = NULL for all of them
                                    Dim linqRes As List(Of VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow) = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow _
                                                                                                                               In myReagentsPositions.tparVirtualRotorPosititions _
                                                                                                                   Where Not a.IsStatusNull() _
                                                                                                                     AndAlso a.Status <> "DEPLETED" _
                                                                                                                     AndAlso a.Status <> "FEW" _
                                                                                                                      Select a).ToList

                                    For Each element As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                        element.BeginEdit()
                                        element.SetStatusNull()
                                        element.EndEdit()
                                    Next
                                    myReagentsPositions.AcceptChanges()
                                    linqRes = Nothing

                                    'Save the Internal Virtual Rotor for Reagents with the content of all positions 
                                    resultData = myVirtualRotorsDelegate.Save(dbConnection, "REAGENTS", myReagentsPositions, "INTERNAL_REAGENTS_ROTOR", True)
                                Else
                                    If (myInternalVRotorID <> -1) Then
                                        'All positions are free in the Reagents Rotor, delete Internal Virtual Rotor if it existed
                                        resultData = myVirtualRotorsDelegate.Delete(dbConnection, myVirtualRotorsDS)
                                    End If
                                End If
                            End If
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.SaveReagentsRotorPositions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all non free positions containing calibrators, controls and/or sample additional solutions in the samples
        ''' rotors and save them in the internal Virtual Rotor. If this special Virtual Rotor does not exist, then it is
        ''' created if there is at least a non-free position containing the specified types of elements in the samples rotor.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 04/08/2011
        ''' Modified by: AG/SA 06/02/2012 - Before save the non free positions in the Internal Reagents Virtual Rotor, for cells having Status 
        '''                                 DEPLETED or FEW, save also the status; for the rest of positions, save Status as NULL
        '''              SA    08/01/2015 - BA-1999 ==> Remove from the loop to set Status NULL all cells having already Status NULL or having 
        '''                                             Status DEPLETED or FEW
        ''' </remarks>
        Private Function SaveSamplesRotorPositions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if the Internal Virtual Rotor for Samples has been created, and in this case, get the ID
                        Dim myVirtualRotorsDelegate As New VirtualRotorsDelegate

                        resultData = myVirtualRotorsDelegate.GetVRotorsByRotorType(dbConnection, "SAMPLES", True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myVirtualRotorsDS As VirtualRotorsDS = DirectCast(resultData.SetDatos, VirtualRotorsDS)

                            Dim myInternalVRotorID As Integer = -1
                            If (myVirtualRotorsDS.tparVirtualRotors.Count = 1) Then
                                myInternalVRotorID = myVirtualRotorsDS.tparVirtualRotors.First.VirtualRotorID
                            End If

                            'Get the content of all positions in the Samples Rotor (InUse and Not InUse positions) to save them as an special Internal Virtual Rotor
                            '(Calibrators, Controls and Additional Solutions loaded in Samples Rotor are normally reused, without downloading them)
                            Dim myWSRotorContentByPosition As New WSRotorContentByPositionDelegate
                            resultData = myWSRotorContentByPosition.GetSamplesRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID, myInternalVRotorID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim mySamplesPositions As VirtualRotorPosititionsDS = DirectCast(resultData.SetDatos, VirtualRotorPosititionsDS)

                                If (mySamplesPositions.tparVirtualRotorPosititions.Count > 0) Then
                                    'Get all positions with Status different of DEPLETED and FEW and set Status = NULL for all of them
                                    Dim linqRes As List(Of VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow) = (From a As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow _
                                                                                                                               In mySamplesPositions.tparVirtualRotorPosititions _
                                                                                                                   Where Not a.IsStatusNull() _
                                                                                                                     AndAlso a.Status <> "DEPLETED" _
                                                                                                                     AndAlso a.Status <> "FEW" _
                                                                                                                      Select a).ToList

                                    For Each element As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In linqRes
                                        element.BeginEdit()
                                        element.SetStatusNull()
                                        element.EndEdit()
                                    Next
                                    mySamplesPositions.AcceptChanges()
                                    linqRes = Nothing

                                    'Save the Internal Virtual Rotor for Samples with the content of all positions 
                                    resultData = myVirtualRotorsDelegate.Save(dbConnection, "SAMPLES", mySamplesPositions, "INTERNAL_SAMPLES_ROTOR", True)
                                Else
                                    If (myInternalVRotorID <> -1) Then
                                        'All positions are free in the Samples Rotor, delete Internal Virtual Rotor if it existed
                                        resultData = myVirtualRotorsDelegate.Delete(dbConnection, myVirtualRotorsDS)
                                    End If
                                End If
                            End If
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.SaveSamplesRotorPositions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new Blank Order or update an existing one 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, 
        '''                           Calibrators and Blanks that have to be included in the Work Session</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter needed to indicate the WorkSession
        '''                              to which the Open Blank Order Tests have been linked</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter needed to indicate in which Analyzer
        '''                           will be executed the requested Blank Order Tests</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table BlankCalibrators
        '''          updated with the generated OrderID / OrderTestID for all new Blanks</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 26/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value
        '''                              in table of Order Tests
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Blanks and Calibrators there are not 
        '''                              Blank Order Tests previously created, then it means that all Blank Order Tests 
        '''                              were deleted; remove them also from the DB
        '''              SA 01/09/2010 - If there is a previous result for the Blank being processed, value of PreviousOrderTestID
        '''                              is saved although New check has not been selected
        '''              SA 01/02/2012 - Added code to delete from DB all Blank Orders that have been removed from the active WorkSession
        '''              SA 29/08/2012 - When the result of a previous Blank is used inform also the field for the previous WorkSession Identifier
        '''              XB 27/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Private Function PrepareBlankOrders(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                            Optional ByVal pWorkSessionID As String = "", Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there are Blanks with Status OPEN in the table of Blanks and Calibrators
                        Dim lstWSBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                        lstWSBlanksDS = (From a In pWSResultDS.BlankCalibrators _
                                        Where String.Compare(a.SampleClass, "BLANK", False) = 0 _
                                      AndAlso a.OTStatus = "OPEN" _
                                       Select a).ToList()

                        Dim myOrdersDelegate As New OrdersDelegate()

                        If (lstWSBlanksDS.Count = 0) Then
                            'All Open Blank Order Tests have been deleted, remove them also from the DB 
                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "BLANK")
                        Else
                            'Get the logged User
                            'Dim currentSession As New GlobalBase
                            Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                            'Go through BlankCalibrators Table to process Blanks requested previously that have been not sent to an 
                            'Analyzer Rotor (fields OrderID and OrderTestID are informed)
                            lstWSBlanksDS = (From a In pWSResultDS.BlankCalibrators _
                                            Where String.Compare(a.SampleClass, "BLANK", False) = 0 _
                                      AndAlso Not a.IsOrderIDNull _
                                      AndAlso Not a.IsOrderTestIDNull _
                                         Order By a.OrderID _
                                           Select a).ToList()

                            Dim myOrderTestDS As New OrderTestsDS
                            Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow

                            If (lstWSBlanksDS.Count = 0) Then
                                'If there is an active WorkSession but in the grid of Blanks and Calibrators there are not 
                                'Blank Order Tests previously created, then it means that all Blank Order Tests were deleted;
                                'remove them also from the DB
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "BLANK")
                            Else
                                Dim listOfOrderID As String = String.Empty
                                Dim currentOrderID As String = String.Empty

                                For Each blankOrderTest In lstWSBlanksDS
                                    If (String.Compare(blankOrderTest.OrderID, currentOrderID, False) <> 0) Then
                                        'Add value of the next OrderID to process to the list of OrderIDs that remain in the WorkSession
                                        If (listOfOrderID <> "") Then listOfOrderID &= ", "
                                        listOfOrderID &= "'" & blankOrderTest.OrderID & "'"

                                        currentOrderID = blankOrderTest.OrderID
                                    End If

                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.OrderID = blankOrderTest.OrderID
                                    myOrderTestDR.OrderTestID = blankOrderTest.OrderTestID
                                    myOrderTestDR.TestType = blankOrderTest.TestType
                                    myOrderTestDR.TestID = blankOrderTest.TestID
                                    myOrderTestDR.SampleType = blankOrderTest.SampleType
                                    myOrderTestDR.TubeType = blankOrderTest.TubeType
                                    myOrderTestDR.OrderTestStatus = blankOrderTest.OTStatus
                                    myOrderTestDR.ReplicatesNumber = blankOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = blankOrderTest.CreationOrder

                                    'If there is a previous result for the Blank, the OrderTestID of the previous result is 
                                    'saved (value of New check is not relevant)
                                    If (Not blankOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = blankOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    ' XB 27/08/2014 - BT #1868
                                    myOrderTestDR.Selected = blankOrderTest.Selected

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Blank Order Tests to update...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    myGlobalDataTO = myOrdersDelegate.UpdateOrders(dbConnection, pWorkSessionID, myOrderTestDS)
                                End If

                                'All Blank Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                If (Not myGlobalDataTO.HasError) Then
                                    If (pWorkSessionID <> String.Empty AndAlso String.Compare(listOfOrderID, String.Empty, False) <> 0) Then
                                        myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "BLANK")
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Go through BlankCalibrators Table to process new requested Blanks
                                'If field OrderTestID is NULL it means it is a new Blank Order Tests that have to be included in a new Order with SampleClass BLANK 
                                'or, if there is already an Order with SampleClass BLANK in the WS, the new Blank Order Tests have to be added to it
                                lstWSBlanksDS = (From a In pWSResultDS.BlankCalibrators _
                                                Where a.SampleClass = "BLANK" _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.IsOrderTestIDNull _
                                               Select a).ToList()

                                myOrderTestDS.Clear()
                                For Each blankOrderTest In lstWSBlanksDS
                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.TestType = blankOrderTest.TestType
                                    myOrderTestDR.TestID = blankOrderTest.TestID
                                    myOrderTestDR.SampleType = blankOrderTest.SampleType
                                    myOrderTestDR.TubeType = blankOrderTest.TubeType
                                    myOrderTestDR.OrderTestStatus = blankOrderTest.OTStatus
                                    myOrderTestDR.ReplicatesNumber = blankOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = blankOrderTest.CreationOrder

                                    'If there is a previous result for the Blank and the New check is not selected, then
                                    'the OrderTestID of the previous result is saved
                                    If (Not blankOrderTest.NewCheck And Not blankOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = blankOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    ' XB 27/08/2014 - BT #1868
                                    myOrderTestDR.Selected = blankOrderTest.Selected

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Blank Order Tests to create, all of them are included in a new Order with SampleClass=BLANK...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    'A new BLANK Order has to be created: Generate the next OrderID
                                    myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)

                                    If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim orderID As String = CType(myGlobalDataTO.SetDatos, String)

                                        'Prepare the DataSet containing the Order data
                                        Dim myOrdersDS As New OrdersDS
                                        Dim myOrderDR As OrdersDS.twksOrdersRow

                                        myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                        myOrderDR.OrderID = orderID
                                        myOrderDR.SampleClass = "BLANK"
                                        myOrderDR.StatFlag = False
                                        myOrderDR.OrderDateTime = DateTime.Now
                                        myOrderDR.OrderStatus = "OPEN"
                                        myOrderDR.TS_User = loggedUser
                                        myOrderDR.TS_DateTime = DateTime.Now
                                        myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                        'Create the new Blank Order with all the new Order Tests
                                        myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                            Dim testType As String = String.Empty
                                            Dim testID As Integer = -1
                                            Dim sampleType As String = String.Empty
                                            For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                'Search the correspondent row in table BlankCalibrators to update fields OrderID and OrderTestID 
                                                testType = orderTestRow.TestType
                                                testID = orderTestRow.TestID
                                                sampleType = orderTestRow.SampleType

                                                lstWSBlanksDS = (From a In pWSResultDS.BlankCalibrators _
                                                                Where a.SampleClass = "BLANK" _
                                                              AndAlso a.OTStatus = "OPEN" _
                                                              AndAlso a.TestType = testType _
                                                              AndAlso a.TestID = testID _
                                                              AndAlso a.SampleType = sampleType _
                                                               Select a).ToList()

                                                For Each blankOrderTest In lstWSBlanksDS
                                                    blankOrderTest.OrderID = orderTestRow.OrderID
                                                    blankOrderTest.OrderTestID = orderTestRow.OrderTestID
                                                Next
                                            Next

                                            'Confirm changes in the DataSet
                                            pWSResultDS.BlankCalibrators.AcceptChanges()
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        lstWSBlanksDS = Nothing

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Return the same entry DataSet updated...
                            myGlobalDataTO.SetDatos = pWSResultDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareBlankOrders", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a new Calibrator Order containing all the new requested Calibrator Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, 
        '''                           Calibrators and Blanks that have to be included in the Work Session</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter needed to indicate the WorkSession
        '''                              to which the Open Calibrator Order Tests have been linked</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter needed to indicate in which Analyzer
        '''                           will be executed the requested Calibrator Order Tests</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table BlankCalibrators
        '''          updated with the generated OrderID / OrderTestID for all new Calibrators</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 26/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value
        '''                              in table of Order Tests
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Blanks and Calibrators there are not 
        '''                              Calibrator Order Tests previously created, then it means that all Calibrator Order Tests 
        '''                              were deleted; remove them also from the DB
        '''              SA 01/09/2010 - If there are previous results for the Calibrator being processed, value of PreviousOrderTestID
        '''                              is saved although the New check has not been selected
        '''              SA 01/02/2012 - Added code to delete from DB all Calibrator Orders that have been removed from the active WorkSession
        '''              SA 12/04/2012 - Besides OPEN Calibrators, those used as Alternative for other SampleTypes (fields SampleType and 
        '''                              RequestedSampleTypes are different) have to be also obtained and processed to manage the special case 
        '''                              of adding to an InProcess WS some requests for the same Test with a different SampleType using the
        '''                              same Calibrator
        '''              XB 27/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Private Function PrepareCalibratorOrders(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                                 Optional ByVal pWorkSessionID As String = "", _
                                                 Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there are Calibrators with Status OPEN in the table of Blanks and Calibrators OR 
                        'verify if there Calibrators used as Alternative for the same Test but other Sample Types (whatever Status)
                        Dim lstWSCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                        lstWSCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                             Where a.SampleClass = "CALIB" _
                                           AndAlso (String.Compare(a.OTStatus, "OPEN", False) = 0 OrElse a.SampleType <> a.RequestedSampleTypes) _
                                            Select a).ToList()

                        Dim myOrdersDelegate As New OrdersDelegate()
                        If (lstWSCalibratorsDS.Count = 0) Then
                            'All Calibrators Order Tests have been deleted, remove them also from the DB 
                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CALIB")
                        Else
                            'Get the logged User
                            'Dim currentSession As New GlobalBase
                            Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                            'Go through BlankCalibrators Table to process Calibrators requested previously (having been sent or not to 
                            'the Analyzer; fields OrderID and OrderTestID are informed) - For positioned Calibrator Order Tests, only value
                            'of field CreationOrder could be changed when more Tests using the same Calibrator are added
                            lstWSCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                                 Where String.Compare(a.SampleClass, "CALIB", False) = 0 _
                                           AndAlso Not a.IsOrderIDNull _
                                           AndAlso Not a.IsOrderTestIDNull _
                                              Order By a.OrderID _
                                                Select a).ToList()

                            Dim myOrderTestDS As New OrderTestsDS
                            Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow

                            If (lstWSCalibratorsDS.Count = 0) Then
                                'If there is an active WorkSession but in the grid of Blanks&Calibrators there are not Calibrator 
                                'Order Tests previously created, then it means that all Calibrator Order Tests were deleted; 
                                'remove them also from the DB
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CALIB")
                            Else
                                Dim orderTestID As Integer = -1
                                Dim listOfOrderID As String = ""
                                Dim currentOrderID As String = ""

                                Dim additionalSampleTypes() As String
                                Dim myOrderTestsDelegate As New OrderTestsDelegate()

                                For Each calibratorOrderTest In lstWSCalibratorsDS
                                    If (String.Compare(calibratorOrderTest.OrderID, currentOrderID, False) <> 0) Then
                                        'Add value of the next OrderID to process to the list of OrderIDs that remain in the WorkSession
                                        If (listOfOrderID <> "") Then listOfOrderID &= ", "
                                        listOfOrderID &= "'" & calibratorOrderTest.OrderID & "'"

                                        currentOrderID = calibratorOrderTest.OrderID
                                    End If

                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.OrderID = calibratorOrderTest.OrderID
                                    myOrderTestDR.OrderTestID = calibratorOrderTest.OrderTestID
                                    myOrderTestDR.TestType = calibratorOrderTest.TestType
                                    myOrderTestDR.TestID = calibratorOrderTest.TestID
                                    myOrderTestDR.SampleType = calibratorOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                    myOrderTestDR.TubeType = calibratorOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = calibratorOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = calibratorOrderTest.CreationOrder

                                    'If there are previous results for the Calibrator, the OrderTestID of the previous results is 
                                    'saved (value of New check is not relevant)
                                    If (Not calibratorOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = calibratorOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    ' XB 27/08/2014 - BT #1868
                                    myOrderTestDR.Selected = calibratorOrderTest.Selected

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)

                                    'Verify if the Calibrator corresponds to an Alternative used by another Sample Types
                                    If (String.Compare(calibratorOrderTest.SampleType.Trim, calibratorOrderTest.RequestedSampleTypes.Trim, False) <> 0) Then
                                        additionalSampleTypes = Split(calibratorOrderTest.RequestedSampleTypes.Trim)

                                        For Each reqSampleType As String In additionalSampleTypes
                                            If (String.Compare(reqSampleType.Trim, "", False) <> 0) Then
                                                'Verify if the Order Test for the requested Sample Type exists
                                                myGlobalDataTO = myOrderTestsDelegate.GetOrderTestByTestAndSampleType(dbConnection, calibratorOrderTest.OrderID, _
                                                                                                                      calibratorOrderTest.TestType, calibratorOrderTest.TestID, _
                                                                                                                      reqSampleType)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                orderTestID = -1
                                                If (Not myGlobalDataTO.SetDatos Is Nothing) Then orderTestID = CType(myGlobalDataTO.SetDatos, Integer)

                                                'A new row has to be added for each requested Sample Type
                                                myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                                myOrderTestDR.OrderID = calibratorOrderTest.OrderID
                                                If (orderTestID <> -1) Then myOrderTestDR.OrderTestID = orderTestID
                                                myOrderTestDR.TestType = calibratorOrderTest.TestType
                                                myOrderTestDR.TestID = calibratorOrderTest.TestID
                                                myOrderTestDR.SampleType = reqSampleType
                                                If (orderTestID <> -1) Then
                                                    myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                                Else
                                                    myOrderTestDR.OrderTestStatus = "OPEN"
                                                End If
                                                myOrderTestDR.TubeType = calibratorOrderTest.TubeType
                                                myOrderTestDR.ReplicatesNumber = calibratorOrderTest.NumReplicates
                                                myOrderTestDR.AlternativeOrderTestID = calibratorOrderTest.OrderTestID
                                                myOrderTestDR.AnalyzerID = pAnalyzerID
                                                myOrderTestDR.TS_User = loggedUser
                                                myOrderTestDR.TS_DateTime = DateTime.Now
                                                ' XB 27/08/2014 - BT #1868
                                                myOrderTestDR.Selected = False

                                                myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                            End If
                                        Next
                                    End If
                                Next

                                'If there are Calibrators Order Tests to update...
                                If (Not myGlobalDataTO.HasError) Then
                                    If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                        myGlobalDataTO = myOrdersDelegate.UpdateOrders(dbConnection, pWorkSessionID, myOrderTestDS)
                                    End If
                                End If

                                'All Calibrator Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                If (Not myGlobalDataTO.HasError) Then
                                    If (pWorkSessionID <> String.Empty AndAlso listOfOrderID <> String.Empty) Then
                                        myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "CALIB")
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Go through BlankCalibrators Table to process new requested Calibrators. If field OrderTestID 
                                'is NULL it means it is a new Blank Order Tests that have to be included in a new Order with SampleClass CALIB
                                lstWSCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                                     Where a.SampleClass = "CALIB" _
                                                   AndAlso a.OTStatus = "OPEN" _
                                                   AndAlso a.IsOrderTestIDNull _
                                                    Select a).ToList()

                                myOrderTestDS.Clear()
                                For Each calibratorOrderTest In lstWSCalibratorsDS
                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.TestType = calibratorOrderTest.TestType
                                    myOrderTestDR.TestID = calibratorOrderTest.TestID
                                    myOrderTestDR.SampleType = calibratorOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                    myOrderTestDR.TubeType = calibratorOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = calibratorOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = calibratorOrderTest.CreationOrder

                                    'If there are previous result for the Calibrator and the New check is not selected, then
                                    'the OrderTestID of the previous results is saved
                                    If (Not calibratorOrderTest.NewCheck And Not calibratorOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = calibratorOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    ' XB 27/08/2014 - BT #1868
                                    myOrderTestDR.Selected = calibratorOrderTest.Selected

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Calibrator Order Tests to create, all of them are included in a new Order with SampleClass=CALIB...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    'Generate the next OrderID
                                    myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim orderID As String = CType(myGlobalDataTO.SetDatos, String)

                                        'Prepare the DataSet containing the Order data
                                        Dim myOrdersDS As New OrdersDS
                                        Dim myOrderDR As OrdersDS.twksOrdersRow

                                        myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                        myOrderDR.OrderID = orderID
                                        myOrderDR.SampleClass = "CALIB"
                                        myOrderDR.StatFlag = False
                                        myOrderDR.OrderDateTime = DateTime.Now
                                        myOrderDR.OrderStatus = "OPEN"
                                        myOrderDR.TS_User = loggedUser
                                        myOrderDR.TS_DateTime = DateTime.Now
                                        myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                        'Create the new Calibrator Order with all the new Order Tests
                                        myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                            Dim testType As String = String.Empty
                                            Dim testID As Integer = -1
                                            Dim sampleType As String = String.Empty
                                            Dim additionalSampleTypes() As String

                                            Dim myOrderTestAlternativDS As New OrderTestsDS
                                            Dim myOrderTestsDelegate As New OrderTestsDelegate()

                                            For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                'Search the correspondent row in table BlankCalibrators to update fields OrderID and OrderTestID 
                                                testType = orderTestRow.TestType
                                                testID = orderTestRow.TestID
                                                sampleType = orderTestRow.SampleType

                                                lstWSCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                                                     Where a.SampleClass = "CALIB" _
                                                                   AndAlso a.OTStatus = "OPEN" _
                                                                   AndAlso a.TestType = testType _
                                                                   AndAlso a.TestID = testID _
                                                                   AndAlso a.SampleType = sampleType _
                                                                    Select a).ToList()

                                                For Each calibratorOrderTest In lstWSCalibratorsDS
                                                    calibratorOrderTest.OrderID = orderTestRow.OrderID
                                                    calibratorOrderTest.OrderTestID = orderTestRow.OrderTestID

                                                    'Insert an Order Test for each requested Sample Type when they use and Alternative Calibrator
                                                    If (calibratorOrderTest.SampleType.Trim <> calibratorOrderTest.RequestedSampleTypes.Trim) Then
                                                        additionalSampleTypes = Split(calibratorOrderTest.RequestedSampleTypes.Trim)

                                                        For Each reqSampleType As String In additionalSampleTypes
                                                            'A new row has to be added for each requested Sample Type
                                                            myOrderTestDR = myOrderTestAlternativDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                                            myOrderTestDR.OrderID = calibratorOrderTest.OrderID
                                                            myOrderTestDR.TestType = calibratorOrderTest.TestType
                                                            myOrderTestDR.TestID = calibratorOrderTest.TestID
                                                            myOrderTestDR.SampleType = reqSampleType
                                                            myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                                            myOrderTestDR.AlternativeOrderTestID = calibratorOrderTest.OrderTestID
                                                            myOrderTestDR.AnalyzerID = pAnalyzerID
                                                            myOrderTestDR.TS_User = loggedUser
                                                            myOrderTestDR.TS_DateTime = DateTime.Now
                                                            ' XB 27/08/2014 - BT #1868
                                                            myOrderTestDR.Selected = calibratorOrderTest.Selected

                                                            myOrderTestAlternativDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                                        Next

                                                        If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                                            'Add the Order Test for the Requested Sample Type
                                                            myGlobalDataTO = myOrderTestsDelegate.Create(dbConnection, myOrderTestAlternativDS)
                                                            If (myGlobalDataTO.HasError) Then Exit For
                                                        End If
                                                    End If
                                                Next
                                            Next
                                            pWSResultDS.BlankCalibrators.AcceptChanges()
                                        End If
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'When the Database Connection was opened locally, then the Commit is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                                    'Return the same entry DataSet updated...
                                    myGlobalDataTO.SetDatos = pWSResultDS
                                Else
                                    'When the Database Connection was opened locally, then the Rollback is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If
                            End If
                        End If
                        lstWSCalibratorsDS = Nothing
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareCalibratorOrders", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a new Control Order containing all the new requested Control Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, 
        '''                           Calibrators and Blanks that have to be included in the Work Session</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter needed to indicate the WorkSession
        '''                              to which the Open Blank Order Tests have been linked</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter needed to indicate in which Analyzer
        '''                           will be executed the requested Control Order Tests</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table Controls
        '''          updated with the generated OrderID / OrderTestID for all new Controls</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 16/03/2010 - Each Control required for Test/SampleType is managed as an individual OrderTestID
        '''                              (field ControlNumber was added to Order Tests table and DS to known which OrderTestID
        '''                              corresponds to each Control Number)
        '''              SA 26/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value
        '''                              in table of Order Tests
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Controls there are not Order Tests previously
        '''                              created, then it means that all Control Order Tests were deleted; remove them also from the DB
        '''              DL 15/04/2011 - Replace use of ControlNum field by use of ControlID field
        '''              SA 01/02/2012 - Added code to delete from DB all Control Orders that have been removed from the active WorkSession
        '''              SA 26/03/2013 - Added changes needed for new LIS implementation: for new Control Order Tests, when filling the OrderTestsDS, 
        '''                              include also LIS fields when they are informed. For Control Order Tests to update, when filling the 
        '''                              OrderTestsDS, include only LISRequest field when it is informed 
        '''              XB 27/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Private Function PrepareControlOrders(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                              Optional ByVal pWorkSessionID As String = "", Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there are Controls with Status OPEN in the table of Controls
                        Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)
                        lstWSControlsDS = (From a In pWSResultDS.Controls _
                                          Where a.SampleClass = "CTRL" _
                                        AndAlso a.OTStatus = "OPEN" _
                                         Select a).ToList()

                        Dim myOrdersDelegate As New OrdersDelegate()
                        If (lstWSControlsDS.Count = 0) Then
                            'All Control Order Tests have been deleted, remove them also from the DB 
                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CTRL")
                        Else
                            'Get the logged User
                            'Dim currentSession As New GlobalBase
                            Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                            'Go through Controls Table to process Controls requested previously (having been sent or not to 
                            'the Analyzer; fields OrderID and OrderTestID are informed) - For positioned Control Order Tests, only value
                            'of field CreationOrder could be changed, when more Tests using the same Control are added
                            lstWSControlsDS = (From a In pWSResultDS.Controls _
                                              Where a.SampleClass = "CTRL" _
                                        AndAlso Not a.IsOrderIDNull _
                                        AndAlso Not a.IsOrderTestIDNull _
                                           Order By a.OrderID, a.OrderTestID _
                                             Select a).ToList()

                            Dim myOrderTestDS As New OrderTestsDS
                            Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow

                            If (lstWSControlsDS.Count = 0) Then
                                'If there is an active WorkSession but in the grid of Controls there are not Order Tests previously
                                'created, then it means that all Control Order Tests were deleted; remove them also from the DB
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CTRL")
                            Else
                                Dim listOfOrderID As String = String.Empty
                                Dim currentOrderID As String = String.Empty
                                For Each controlOrderTest In lstWSControlsDS
                                    If (controlOrderTest.OrderID <> currentOrderID) Then
                                        'Add value of the next OrderID to process to the list of OrderIDs that remain in the WorkSession
                                        If (listOfOrderID <> String.Empty) Then listOfOrderID &= ", "
                                        listOfOrderID &= "'" & controlOrderTest.OrderID & "'"

                                        currentOrderID = controlOrderTest.OrderID
                                    End If

                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.OrderID = controlOrderTest.OrderID
                                    myOrderTestDR.OrderTestID = controlOrderTest.OrderTestID
                                    myOrderTestDR.TestType = controlOrderTest.TestType
                                    myOrderTestDR.TestID = controlOrderTest.TestID
                                    myOrderTestDR.SampleType = controlOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = controlOrderTest.OTStatus
                                    myOrderTestDR.TubeType = controlOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = controlOrderTest.NumReplicates
                                    myOrderTestDR.ControlID = controlOrderTest.ControlID
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = controlOrderTest.CreationOrder
                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    ' XB 27/08/2014 - BT #1868
                                    myOrderTestDR.Selected = controlOrderTest.Selected

                                    'SA 26/03/2013 - Set value of field LISRequest when it is informed
                                    If (Not controlOrderTest.IsLISRequestNull) Then myOrderTestDR.LISRequest = controlOrderTest.LISRequest

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Control Order Tests to update...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    myGlobalDataTO = myOrdersDelegate.UpdateOrders(dbConnection, pWorkSessionID, myOrderTestDS)
                                End If

                                'All Control Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                If (Not myGlobalDataTO.HasError) Then
                                    If (pWorkSessionID <> String.Empty AndAlso listOfOrderID <> String.Empty) Then
                                        myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "CTRL")
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Go through Controls Table to process new requested Controls. If field OrderTestID 
                                'is NULL it means it is a new Control Order Test that has to be included in a new Order with SampleClass CTRL
                                lstWSControlsDS = (From a In pWSResultDS.Controls _
                                                  Where a.SampleClass = "CTRL" _
                                                AndAlso a.OTStatus = "OPEN" _
                                                AndAlso a.IsOrderTestIDNull _
                                               Order By a.TestType, a.TestID, a.SampleType _
                                                 Select a).ToList()

                                myOrderTestDS.Clear()
                                For Each controlOrderTest In lstWSControlsDS
                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.TestType = controlOrderTest.TestType
                                    myOrderTestDR.TestID = controlOrderTest.TestID
                                    myOrderTestDR.SampleType = controlOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = controlOrderTest.OTStatus
                                    myOrderTestDR.TubeType = controlOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = controlOrderTest.NumReplicates
                                    myOrderTestDR.ControlID = controlOrderTest.ControlID
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.SampleClass = "CTRL"
                                    myOrderTestDR.CreationOrder = controlOrderTest.CreationOrder
                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    ' XB 27/08/2014 - BT #1868
                                    myOrderTestDR.Selected = controlOrderTest.Selected

                                    'SA 26/03/2013 - Set value of all LIS fields when they are informed
                                    If (Not controlOrderTest.IsLISRequestNull) Then myOrderTestDR.LISRequest = controlOrderTest.LISRequest
                                    If (Not controlOrderTest.IsAwosIDNull) Then myOrderTestDR.AwosID = controlOrderTest.AwosID
                                    If (Not controlOrderTest.IsSpecimenIDNull) Then myOrderTestDR.SpecimenID = controlOrderTest.SpecimenID
                                    If (Not controlOrderTest.IsESPatientIDNull) Then myOrderTestDR.ESPatientID = controlOrderTest.ESPatientID
                                    If (Not controlOrderTest.IsLISPatientIDNull) Then myOrderTestDR.LISPatientID = controlOrderTest.LISPatientID
                                    If (Not controlOrderTest.IsESOrderIDNull) Then myOrderTestDR.ESOrderID = controlOrderTest.ESOrderID
                                    If (Not controlOrderTest.IsLISOrderIDNull) Then myOrderTestDR.LISOrderID = controlOrderTest.LISOrderID

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Control Order Tests to create, all of them are included in a new Order with SampleClass=CTRL...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    Dim myOrdersDS As New OrdersDS

                                    'Generate the next OrderID
                                    myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)
                                    If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim orderID As String = CType(myGlobalDataTO.SetDatos, String)

                                        'Prepare the DataSet containing the Order data
                                        Dim myOrderDR As OrdersDS.twksOrdersRow

                                        myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                        myOrderDR.OrderID = orderID
                                        myOrderDR.SampleClass = "CTRL"
                                        myOrderDR.StatFlag = False
                                        myOrderDR.OrderDateTime = DateTime.Now
                                        myOrderDR.OrderStatus = "OPEN"
                                        myOrderDR.TS_User = loggedUser
                                        myOrderDR.TS_DateTime = DateTime.Now
                                        myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                        'Create the new Control Order with all the new Order Tests
                                        myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                            Dim testType As String = String.Empty
                                            Dim sampleType As String = String.Empty
                                            Dim testID As Integer = -1
                                            Dim controlID As Integer = -1
                                            For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                'Search the correspondent row in table Controls to update fields OrderID and OrderTestID 
                                                testType = orderTestRow.TestType
                                                testID = orderTestRow.TestID
                                                sampleType = orderTestRow.SampleType
                                                controlID = orderTestRow.ControlID

                                                lstWSControlsDS = (From a In pWSResultDS.Controls _
                                                                  Where a.SampleClass = "CTRL" _
                                                                AndAlso a.OTStatus = "OPEN" _
                                                                AndAlso a.TestType = testType _
                                                                AndAlso a.TestID = testID _
                                                                AndAlso a.SampleType = sampleType _
                                                                AndAlso a.ControlID = controlID _
                                                                 Select a).ToList()

                                                For Each controlOrderTest In lstWSControlsDS
                                                    controlOrderTest.OrderID = orderTestRow.OrderID
                                                    controlOrderTest.OrderTestID = orderTestRow.OrderTestID

                                                    'Confirm changes in the DataSet
                                                    pWSResultDS.Controls.AcceptChanges()
                                                Next
                                            Next
                                        End If
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'When the Database Connection was opened locally, then the Commit is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                                    'Return the same entry DataSet updated...
                                    myGlobalDataTO.SetDatos = pWSResultDS
                                Else
                                    'When the Database Connection was opened locally, then the Rollback is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If
                            End If
                        End If
                        lstWSControlsDS = Nothing
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareControlOrders", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a group of Patient Orders containing all the new requested Patient Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, 
        '''                           Calibrators and Blanks that have to be included in the Work Session</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter needed to indicate the WorkSession
        '''                              to which the Open Blank Order Tests have been linked</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter needed to indicate in which Analyzer
        '''                           will be executed the requested Patient Order Tests</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table Patients
        '''          updated with the generated OrderID / OrderTestID for all new Patient Samples</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 29/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value
        '''                              in table of Order Tests
        '''              SA 27/04/2010 - Changed LINQ to get Patient Order Tests previously created (field OrderID is informed) to 
        '''                              obtain also the ones already positioned, due to for them it is needed to update field
        '''                              CreationOrder
        '''              SA 18/05/2010 - Added code needed to manage Patient Order Tests with Calculated Tests in the WorkSession
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Patients there are not Order Tests previously
        '''                              created, then it means that all Patient Order Tests were deleted; remove them also from the DB. 
        '''                              Also, when all Order Tests of an Order previously created have been deleted, they are also deleted
        '''                              from the DB by calling function DeleteOrdersNotInWS in OrdersDelegate
        '''              SA 30/08/2010 - Rebuilding of relations between Order Tests of Standard and Calculated Tests should be done although
        '''                              there are not new Order Tests to add to the WorkSession 
        '''              SA 13/10/2010 - Do not inform OrderStatus = "OPEN" for existing Orders, to avoid re-opening of Closed Orders        
        '''              SA 21/10/2010 - Inform fields TubeType and NumReplicates not only for Standard Tests, but also for ISE Tests
        '''              SA 01/02/2012 - Inform the SampleClass (PATIENT) when calling function DeleteOrdersNotInWS in OrdersDelegate    
        '''              SA 26/03/2013 - Added changes needed for new LIS implementation: when filling the DS toAddOrderTestsDS, include also 
        '''                              LIS fields when they are informed. For both cases, add and mod Order Tests, inform field LISRequest if
        '''                              its value is not null in pWSResultsDS.Patients. For add Order Tests, inform value of field SampleClass
        '''              SA 03/05/2013 - When get from the WorkSessionResultDS DataSet the list of Patient Orders to modify (those having field OrderID
        '''                              informed), besides for the OrderID, data has to be sorted by SampleIDType. This is due to with the new LIS 
        '''                              implementation, if LIS sends new Order Tests and demographic data for a Patient initially added manually to the WS,
        '''                              the Order should be updated (informing field PatientID instead SampleID), but in current implementation the Order Test
        '''                              having SampleIDType=MAN is obtained before the one having SampleIDType=DB and the update is not executed (because the
        '''                              Order is updated with data of the first Order Test processed; from screen all Order Tests carried the same information
        '''                              and it was enough to take the first)
        '''              SA 04/06/2013 - When a SampleID is moved to field PatientID in an Order, its content has to be changed to Uppercase characters, due
        '''                              to this is the way PatientIDs are saved in tparPatients table
        '''              SA 05/02/2014 - BT #1491 ==> Exclude OPEN Order Tests requested by LIS from the process for update existing Orders, due to LIS Order Tests
        '''                                           can not be modified nor deleted. Done to improve the performance of this function when there are lot of OPEN 
        '''                                           Order Tests requested by LIS and avoid DB time out errors when other process try to access one of the tables
        '''                                           locked by this transaction.
        '''              XB 27/08/2014 - Add new field Selected - BT #1868
        ''' </remarks>
        Private Function PreparePatientOrders(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                              Optional ByVal pWorkSessionID As String = "", Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'For all Patient Order Tests in the WorkSession having relation with a Calculated Tests, delete 
                        'all relations defined in table twksOrderCalculatedTests
                        If (pWorkSessionID.Trim <> String.Empty) Then
                            Dim myCalcOrderTestsDelegate As New OrderCalculatedTestsDelegate
                            myGlobalDataTO = myCalcOrderTestsDelegate.DeleteByWorkSession(dbConnection, pWorkSessionID)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'Verify if there are Patient Order Tests with Status OPEN in the table of Patients
                            Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                            lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                              Where a.SampleClass = "PATIENT" _
                                            AndAlso a.OTStatus = "OPEN" _
                                             Select a).ToList()

                            Dim myOrdersDelegate As New OrdersDelegate()
                            Dim myOrderTestsDelegate As New OrderTestsDelegate

                            If (lstWSPatientsDS.Count = 0) Then
                                'All Patient Order Tests have been deleted, remove them also from the DB 
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "PATIENT")
                            Else
                                'Get the logged User
                                'Dim currentSession As New GlobalBase
                                Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                                'Go through Patients Table to process Patient Orders requested previously (field OrderID is informed). These Orders can have
                                'existing Order Tests (field OrderTestID informed) and non existing Order Tests (they have to be added to the Order) query 
                                'BT #1491 ==> ... Existing Order Tests requested by LIS are excluded from the query due to they cannot be modified (excepting
                                '             Order Tests with LISRequest=True but AwosID not informed, due to this is the case of Order Tests manually added  
                                '             that are also included in the formula of one or more Calculated Tests requested by LIS) 
                                lstWSPatientsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                  Where a.SampleClass = "PATIENT" _
                                                AndAlso a.OrderID <> String.Empty _
                                                AndAlso (a.IsLISRequestNull _
                                                 OrElse a.LISRequest = False _
                                                 OrElse (a.LISRequest = True And a.IsOrderTestIDNull) _
                                                 OrElse (a.LISRequest = True And Not a.IsOrderTestIDNull And a.IsAwosIDNull)) _
                                               Order By a.OrderID, a.SampleIDType _
                                                 Select a).ToList()

                                'lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                '                  Where a.SampleClass = "PATIENT" _
                                '                AndAlso a.OrderID <> String.Empty _
                                '               Order By a.OrderID, a.SampleIDType _
                                '                 Select a).ToList()

                                If (lstWSPatientsDS.Count = 0) Then
                                    'If there is an active WorkSession but in the grid of Patients there are not Order Tests previously
                                    'created, then it means that all Patient Order Tests were deleted; remove them also from the DB
                                    myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "PATIENT")
                                Else
                                    Dim myOrdersDS As New OrdersDS
                                    Dim myOrderDR As OrdersDS.twksOrdersRow

                                    Dim toModOrderTestsDS As New OrderTestsDS
                                    Dim toAddOrderTestsDS As New OrderTestsDS
                                    Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow

                                    Dim listOfOrderID As String = String.Empty
                                    Dim currentOrderID As String = String.Empty

                                    Dim testType As String = String.Empty
                                    Dim testID As Integer = -1
                                    Dim sampleType As String = String.Empty
                                    Dim lstOrderTestsDS As List(Of WorkSessionResultDS.PatientsRow)

                                    For i As Integer = 0 To lstWSPatientsDS.Count - 1
                                        If (lstWSPatientsDS(i).OrderID <> currentOrderID) Then
                                            If (currentOrderID <> String.Empty) Then
                                                'Update values of the Current Order
                                                myGlobalDataTO = myOrdersDelegate.ModifyOrder(dbConnection, myOrdersDS)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                'If there are Patient Order Tests to update for the current Order...
                                                If (toModOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                    myGlobalDataTO = myOrdersDelegate.UpdateOrders(dbConnection, pWorkSessionID, toModOrderTestsDS)
                                                    If (myGlobalDataTO.HasError) Then Exit For
                                                End If

                                                'If there are Patient Order Tests to add to the current Order
                                                If (toAddOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                    myGlobalDataTO = myOrderTestsDelegate.Create(dbConnection, toAddOrderTestsDS)

                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        toAddOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                        'Search the correspondent row in table Patients to update field OrderTestID 
                                                        For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In toAddOrderTestsDS.twksOrderTests.Rows
                                                            testType = orderTestRow.TestType
                                                            testID = orderTestRow.TestID
                                                            sampleType = orderTestRow.SampleType

                                                            lstOrderTestsDS = (From a In pWSResultDS.Patients _
                                                                              Where a.SampleClass = "PATIENT" _
                                                                            AndAlso a.OrderID = currentOrderID _
                                                                            AndAlso a.OTStatus = "OPEN" _
                                                                            AndAlso a.TestType = testType _
                                                                            AndAlso a.TestID = testID _
                                                                            AndAlso a.SampleType = sampleType _
                                                                             Select a).ToList()

                                                            If (lstOrderTestsDS.Count = 1) Then
                                                                lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID
                                                            End If
                                                        Next
                                                    End If
                                                End If
                                            End If

                                            'Prepare the DataSet to update Patient Order values
                                            myOrdersDS.twksOrders.Clear()

                                            myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                            myOrderDR.OrderID = lstWSPatientsDS(i).OrderID
                                            myOrderDR.SampleClass = "PATIENT"
                                            myOrderDR.StatFlag = lstWSPatientsDS(i).StatFlag

                                            '** Field PatientID will be SampleID (in Uppercase characters) when SampleIDType=DB, otherwise it will be Null
                                            '** Field SampleID will be SampleID when SampleIDType=MANUAL or AUTO, otherwise it will be Null
                                            If (lstWSPatientsDS(i).SampleIDType = "DB") Then
                                                myOrderDR.PatientID = lstWSPatientsDS(i).SampleID.ToUpperBS
                                                myOrderDR.SampleID = ""
                                            ElseIf (lstWSPatientsDS(i).SampleIDType = "MAN" Or lstWSPatientsDS(i).SampleIDType = "AUTO") Then
                                                myOrderDR.SampleID = lstWSPatientsDS(i).SampleID
                                                myOrderDR.PatientID = ""
                                            End If

                                            myOrderDR.OrderDateTime = DateTime.Now
                                            'myOrderDR.OrderStatus = "OPEN"        'SA - 13/10/2010
                                            myOrderDR.TS_User = loggedUser
                                            myOrderDR.TS_DateTime = DateTime.Now
                                            myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                            'Set value of the next OrderID to process
                                            currentOrderID = lstWSPatientsDS(i).OrderID

                                            If (pWorkSessionID.Trim <> String.Empty) Then
                                                'Add value of the next OrderID to process to the list of OrderID that remain in the WorkSession
                                                If (listOfOrderID <> String.Empty) Then listOfOrderID &= ", "
                                                listOfOrderID &= "'" & currentOrderID & "'"
                                            End If

                                            toModOrderTestsDS.Clear()
                                            toAddOrderTestsDS.Clear()
                                        End If

                                        If (Not lstWSPatientsDS(i).IsOrderTestIDNull) Then
                                            'Create a row in the DataSet containing the Order Tests to update; inform the OrderTestID
                                            myOrderTestDR = toModOrderTestsDS.twksOrderTests.NewtwksOrderTestsRow()
                                            myOrderTestDR.OrderTestID = lstWSPatientsDS(i).OrderTestID
                                        Else
                                            'Create a row in the DataSet containing the Order Tests to add
                                            myOrderTestDR = toAddOrderTestsDS.twksOrderTests.NewtwksOrderTestsRow()
                                        End If

                                        myOrderTestDR.OrderID = lstWSPatientsDS(i).OrderID
                                        myOrderTestDR.SampleClass = "PATIENT"
                                        myOrderTestDR.TestType = lstWSPatientsDS(i).TestType
                                        myOrderTestDR.TestID = lstWSPatientsDS(i).TestID
                                        myOrderTestDR.SampleType = lstWSPatientsDS(i).SampleType
                                        myOrderTestDR.OrderTestStatus = lstWSPatientsDS(i).OTStatus

                                        If (lstWSPatientsDS(i).TestType = "STD" OrElse lstWSPatientsDS(i).TestType = "ISE") Then
                                            'TubeType and NumReplicates is informed only for Standard and ISE Tests 
                                            myOrderTestDR.ReplicatesNumber = lstWSPatientsDS(i).NumReplicates
                                            myOrderTestDR.TubeType = lstWSPatientsDS(i).TubeType
                                        End If

                                        If (Not lstWSPatientsDS(i).IsTestProfileIDNull AndAlso lstWSPatientsDS(i).TestProfileID <> 0) Then
                                            myOrderTestDR.TestProfileID = lstWSPatientsDS(i).TestProfileID
                                        End If

                                        myOrderTestDR.CreationOrder = lstWSPatientsDS(i).CreationOrder
                                        myOrderTestDR.AnalyzerID = pAnalyzerID
                                        myOrderTestDR.TS_User = loggedUser
                                        myOrderTestDR.TS_DateTime = DateTime.Now
                                        ' XB 27/08/2014 - BT #1868
                                        myOrderTestDR.Selected = lstWSPatientsDS(i).Selected

                                        'SA 26/03/2013 - Inform value of LIS fields LISRequest and ExternalQC when they are informed
                                        If (Not lstWSPatientsDS(i).IsLISRequestNull) Then myOrderTestDR.LISRequest = lstWSPatientsDS(i).LISRequest
                                        If (Not lstWSPatientsDS(i).IsExternalQCNull) Then myOrderTestDR.ExternalQC = lstWSPatientsDS(i).ExternalQC

                                        If (Not lstWSPatientsDS(i).IsOrderTestIDNull) Then
                                            'Add row to the DataSet containing the Order Tests to update
                                            toModOrderTestsDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                        Else
                                            'SA 26/03/2013 - Fill also the rest of LIS fields if they are informed... 
                                            If (Not lstWSPatientsDS(i).IsAwosIDNull) Then myOrderTestDR.AwosID = lstWSPatientsDS(i).AwosID
                                            If (Not lstWSPatientsDS(i).IsSpecimenIDNull) Then myOrderTestDR.SpecimenID = lstWSPatientsDS(i).SpecimenID
                                            If (Not lstWSPatientsDS(i).IsESPatientIDNull) Then myOrderTestDR.ESPatientID = lstWSPatientsDS(i).ESPatientID
                                            If (Not lstWSPatientsDS(i).IsLISPatientIDNull) Then myOrderTestDR.LISPatientID = lstWSPatientsDS(i).LISPatientID
                                            If (Not lstWSPatientsDS(i).IsESOrderIDNull) Then myOrderTestDR.ESOrderID = lstWSPatientsDS(i).ESOrderID
                                            If (Not lstWSPatientsDS(i).IsLISOrderIDNull) Then myOrderTestDR.LISOrderID = lstWSPatientsDS(i).LISOrderID

                                            'Add row to the DataSet containing Order Tests to create
                                            toAddOrderTestsDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                        End If

                                        If (i = lstWSPatientsDS.Count - 1) Then
                                            'Update values of the Current Order
                                            myGlobalDataTO = myOrdersDelegate.ModifyOrder(dbConnection, myOrdersDS)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            'If there are Patient Order Tests to update for the current Order...
                                            If (toModOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                myGlobalDataTO = myOrdersDelegate.UpdateOrders(dbConnection, pWorkSessionID, toModOrderTestsDS)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            End If

                                            'If there are Patient Order Tests to add to the current Order
                                            If (toAddOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                myGlobalDataTO = myOrderTestsDelegate.Create(dbConnection, toAddOrderTestsDS)

                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    toAddOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                    'Search the correspondent row in table Patients to update field OrderTestID 
                                                    For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In toAddOrderTestsDS.twksOrderTests.Rows
                                                        testType = orderTestRow.TestType
                                                        testID = orderTestRow.TestID
                                                        sampleType = orderTestRow.SampleType

                                                        lstOrderTestsDS = (From a In pWSResultDS.Patients _
                                                                          Where a.SampleClass = "PATIENT" _
                                                                        AndAlso a.OrderID = currentOrderID _
                                                                        AndAlso a.OTStatus = "OPEN" _
                                                                        AndAlso a.TestType = testType _
                                                                        AndAlso a.TestID = testID _
                                                                        AndAlso a.SampleType = sampleType _
                                                                         Select a).ToList()

                                                        If (lstOrderTestsDS.Count = 1) Then
                                                            lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        End If
                                    Next

                                    If (Not myGlobalDataTO.HasError) Then
                                        If (pWorkSessionID.Trim <> String.Empty AndAlso listOfOrderID <> String.Empty) Then
                                            'All Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "PATIENT")
                                        End If
                                    End If
                                End If

                                'Special code to update field CreationOrder for all Patient Order Tests requested by LIS and also to create the needed
                                'row in the WSOrderTestsDS that will be used to add/update data in table twksWSOrderTests (to solve an aesthetic issue 
                                'derived from BT #1491)
                                If (Not myGlobalDataTO.HasError) Then
                                    lstWSPatientsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                      Where a.SampleClass = "PATIENT" _
                                                    AndAlso a.OrderID <> String.Empty _
                                                    AndAlso Not a.IsOrderTestIDNull _
                                                    AndAlso (Not a.IsLISRequestNull AndAlso a.LISRequest = True) _
                                                     Select a).ToList()

                                    If (lstWSPatientsDS.Count > 0) Then
                                        myGlobalDataTO = myOrderTestsDelegate.UpdateCreationOrderForOpenLISOTs(dbConnection, lstWSPatientsDS)
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'Go through Patients Table to process new Patient Order Tests - Get those with field OrderID not informed
                                    'lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                    '                  Where a.SampleClass = "PATIENT" _
                                    '                AndAlso a.OTStatus = "OPEN" _
                                    '                AndAlso a.OrderID = "" _
                                    '               Order By a.SampleIDType, a.SampleID, a.StatFlag _
                                    '                 Select a).ToList()

                                    'RH 12/07/2011 Substitute SampleID by CreationOrder, because the creation order must be kept
                                    lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                                      Where a.SampleClass = "PATIENT" _
                                                    AndAlso a.OTStatus = "OPEN" _
                                                    AndAlso a.OrderID = String.Empty _
                                                   Order By a.SampleIDType, a.CreationOrder, a.StatFlag _
                                                     Select a).ToList()

                                    If (lstWSPatientsDS.Count > 0) Then
                                        Dim myOrdersDS As New OrdersDS
                                        Dim myOrderDR As OrdersDS.twksOrdersRow
                                        Dim myOrderTestDS As New OrderTestsDS
                                        Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow
                                        Dim orderID As String = String.Empty
                                        Dim currentSampleID As String = String.Empty
                                        Dim currentStatFlag As Integer = -1
                                        Dim testType As String = String.Empty
                                        Dim testID As Integer = -1
                                        Dim sampleType As String = String.Empty
                                        Dim lstOrderTestsDS As List(Of WorkSessionResultDS.PatientsRow)

                                        For i As Integer = 0 To lstWSPatientsDS.Count - 1
                                            If ((lstWSPatientsDS(i).SampleID <> currentSampleID) OrElse CType(lstWSPatientsDS(i).StatFlag, Integer) <> currentStatFlag) Then
                                                If (currentSampleID <> String.Empty) Then
                                                    'It is not the first Patient Order, the previous is created 
                                                    myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)

                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                        'Search the correspondent row in table Patients to update field OrderTestID 
                                                        For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                            testType = orderTestRow.TestType
                                                            testID = orderTestRow.TestID
                                                            sampleType = orderTestRow.SampleType

                                                            lstOrderTestsDS = (From a In pWSResultDS.Patients _
                                                                              Where a.SampleClass = "PATIENT" _
                                                                            AndAlso a.OrderID = orderID _
                                                                            AndAlso a.OTStatus = "OPEN" _
                                                                            AndAlso a.TestType = testType _
                                                                            AndAlso a.TestID = testID _
                                                                            AndAlso a.SampleType = sampleType _
                                                                             Select a).ToList()

                                                            If (lstOrderTestsDS.Count = 1) Then
                                                                lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID
                                                            End If
                                                        Next
                                                    Else
                                                        'Error creating the new Patient Order
                                                        Exit For
                                                    End If
                                                End If

                                                'Update values of variables used to control the adding
                                                currentSampleID = lstWSPatientsDS(i).SampleID
                                                currentStatFlag = CType(lstWSPatientsDS(i).StatFlag, Integer)

                                                'Generate the next OrderID
                                                orderID = ""
                                                myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    orderID = CType(myGlobalDataTO.SetDatos, String)
                                                Else
                                                    'Error generating the next OrderID 
                                                    Exit For
                                                End If

                                                'Prepare the DataSet for the Order
                                                myOrdersDS.twksOrders.Clear()
                                                myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                                myOrderDR.OrderID = orderID
                                                myOrderDR.SampleClass = "PATIENT"
                                                myOrderDR.StatFlag = lstWSPatientsDS(i).StatFlag

                                                '** Field PatientID will be SampleID (in Uppercase characters) when SampleIDType=DB, otherwise it will be Null
                                                '** Field SampleID will be SampleID when SampleIDType=MANUAL, otherwise it will be Null
                                                If (lstWSPatientsDS(i).SampleIDType = "DB") Then myOrderDR.PatientID = lstWSPatientsDS(i).SampleID.ToUpperBS
                                                If (lstWSPatientsDS(i).SampleIDType = "MAN" OrElse lstWSPatientsDS(i).SampleIDType = "AUTO") Then myOrderDR.SampleID = lstWSPatientsDS(i).SampleID

                                                myOrderDR.OrderDateTime = DateTime.Now
                                                myOrderDR.OrderStatus = "OPEN"
                                                myOrderDR.TS_User = loggedUser
                                                myOrderDR.TS_DateTime = DateTime.Now
                                                myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                                myOrderTestDS.twksOrderTests.Clear()
                                            End If

                                            'Set value of the OrderID in the row DataSet in process
                                            lstWSPatientsDS(i).OrderID = orderID

                                            'Prepare the DataSet for the Order Tests
                                            myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow()
                                            myOrderTestDR.TestType = lstWSPatientsDS(i).TestType
                                            myOrderTestDR.TestID = lstWSPatientsDS(i).TestID
                                            myOrderTestDR.SampleType = lstWSPatientsDS(i).SampleType
                                            myOrderTestDR.OrderTestStatus = lstWSPatientsDS(i).OTStatus

                                            If (lstWSPatientsDS(i).TestType = "STD" OrElse lstWSPatientsDS(i).TestType = "ISE") Then
                                                'TubeType and NumReplicates is informed only for Standard and ISE Tests 
                                                myOrderTestDR.TubeType = lstWSPatientsDS(i).TubeType
                                                myOrderTestDR.ReplicatesNumber = lstWSPatientsDS(i).NumReplicates
                                            End If

                                            myOrderTestDR.CreationOrder = lstWSPatientsDS(i).CreationOrder
                                            myOrderTestDR.AnalyzerID = pAnalyzerID
                                            myOrderTestDR.SampleClass = "PATIENT"

                                            If (Not lstWSPatientsDS(i).IsTestProfileIDNull AndAlso lstWSPatientsDS(i).TestProfileID <> 0) Then
                                                myOrderTestDR.TestProfileID = lstWSPatientsDS(i).TestProfileID
                                            End If

                                            'SA 26/03/2013 - Inform LIS fields when they have a value in the entry DS
                                            If (Not lstWSPatientsDS(i).IsLISRequestNull) Then myOrderTestDR.LISRequest = lstWSPatientsDS(i).LISRequest
                                            If (Not lstWSPatientsDS(i).IsExternalQCNull) Then myOrderTestDR.ExternalQC = lstWSPatientsDS(i).ExternalQC
                                            If (Not lstWSPatientsDS(i).IsAwosIDNull) Then myOrderTestDR.AwosID = lstWSPatientsDS(i).AwosID
                                            If (Not lstWSPatientsDS(i).IsSpecimenIDNull) Then myOrderTestDR.SpecimenID = lstWSPatientsDS(i).SpecimenID
                                            If (Not lstWSPatientsDS(i).IsESPatientIDNull) Then myOrderTestDR.ESPatientID = lstWSPatientsDS(i).ESPatientID
                                            If (Not lstWSPatientsDS(i).IsLISPatientIDNull) Then myOrderTestDR.LISPatientID = lstWSPatientsDS(i).LISPatientID
                                            If (Not lstWSPatientsDS(i).IsESOrderIDNull) Then myOrderTestDR.ESOrderID = lstWSPatientsDS(i).ESOrderID
                                            If (Not lstWSPatientsDS(i).IsLISOrderIDNull) Then myOrderTestDR.LISOrderID = lstWSPatientsDS(i).LISOrderID

                                            ' XB 27/08/2014 - BT #1868
                                            myOrderTestDR.Selected = lstWSPatientsDS(i).Selected

                                            myOrderTestDR.TS_User = loggedUser
                                            myOrderTestDR.TS_DateTime = DateTime.Now
                                            myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)

                                            If (i = lstWSPatientsDS.Count - 1) Then
                                                'The Patient Order is created
                                                myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                    'Search the correspondent row in table Patients to update field OrderTestID 
                                                    For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                        testType = orderTestRow.TestType
                                                        testID = orderTestRow.TestID
                                                        sampleType = orderTestRow.SampleType

                                                        lstOrderTestsDS = (From a In pWSResultDS.Patients _
                                                                          Where a.SampleClass = "PATIENT" _
                                                                        AndAlso a.OrderID = orderID _
                                                                        AndAlso a.OTStatus = "OPEN" _
                                                                        AndAlso a.TestType = testType _
                                                                        AndAlso a.TestID = testID _
                                                                        AndAlso a.SampleType = sampleType _
                                                                         Select a).ToList()

                                                        If (lstOrderTestsDS.Count = 1) Then
                                                            lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Get all Patient Order Tests having a Test (whatever type) included in at least a Calculated Test
                                Dim lstCalcOrderTests As List(Of WorkSessionResultDS.PatientsRow)
                                lstCalcOrderTests = (From a In pWSResultDS.Patients _
                                                    Where a.SampleClass = "PATIENT" _
                                             AndAlso (Not a.IsCalcTestIDNull AndAlso a.CalcTestID <> String.Empty) _
                                                 Order By a.SampleID, a.StatFlag _
                                                   Select a).ToList()

                                Dim myCalcOrderTestsDS As New OrderCalculatedTestsDS
                                Dim myOrderTestDR As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow

                                Dim myIndex As Integer
                                Dim currentSampleID As String = String.Empty
                                Dim currentStatFlag As Boolean = False
                                Dim calcTestsList() As String
                                Dim lstOrderTestID As List(Of Integer)

                                For Each calcTests As WorkSessionResultDS.PatientsRow In lstCalcOrderTests
                                    currentSampleID = calcTests.SampleID
                                    currentStatFlag = calcTests.StatFlag
                                    calcTestsList = calcTests.CalcTestID.Split(CChar(", "))

                                    For i As Integer = 0 To calcTestsList.Count - 1
                                        myIndex = i

                                        'Search the ID of the Order Test for the Calculated Test
                                        lstOrderTestID = (From b In pWSResultDS.Patients _
                                                          Where b.SampleClass = "PATIENT" _
                                                        AndAlso b.SampleID = currentSampleID _
                                                        AndAlso b.StatFlag = currentStatFlag _
                                                        AndAlso b.TestType = "CALC" _
                                                        AndAlso b.TestID = Convert.ToInt32(calcTestsList(myIndex)) _
                                                         Select b.OrderTestID).ToList()

                                        If (lstOrderTestID.Count = 1) Then
                                            'Add the pair to the typed DataSet OrderCalculatedTestsDS 
                                            myOrderTestDR = myCalcOrderTestsDS.twksOrderCalculatedTests.NewtwksOrderCalculatedTestsRow()
                                            myOrderTestDR.OrderTestID = calcTests.OrderTestID
                                            myOrderTestDR.CalcOrderTestID = Convert.ToInt32(lstOrderTestID.First)
                                            myCalcOrderTestsDS.twksOrderCalculatedTests.AddtwksOrderCalculatedTestsRow(myOrderTestDR)
                                        End If
                                    Next
                                Next

                                'Save data in table twksOrderCalculatedTests....
                                If (myCalcOrderTestsDS.twksOrderCalculatedTests.Rows.Count > 0) Then
                                    Dim myCalcOrderTestsDelegate As New OrderCalculatedTestsDelegate
                                    myGlobalDataTO = myCalcOrderTestsDelegate.Create(dbConnection, myCalcOrderTestsDS)
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                                'Return the same entry DataSet updated...
                                pWSResultDS.Patients.AcceptChanges()
                                myGlobalDataTO.SetDatos = pWSResultDS
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PreparePatientOrders", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When new OrderTests using existing Required Elements are added, change the ElementFinished flag and, if the Element is already
        ''' positioned, the status of the Sample Tube position. This function is applied only for BLANKS, CALIBS, CTRLS and PATIENT Elements
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pSpecimenIDList">Optional parameter. Used only for PATIENT Elements when LIS has sent one or more Specimen IDs 
        '''                               (Barcode labels) for it</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 09/09/2011
        ''' Modified by: SA 19/01/2012 - Changed the function logic; just set ElementFinished=FALSE and, if there are positioned
        '''                              tubes for the Element, set position Status to PENDING if it was FINISHED
        '''              SG 29/04/2013 - Added optional pSpecimenIDList to update also field SpecimenIDList for the required Patient
        '''                              Sample element (the list of Specimen IDs for the Patient Sample is shown as branch ToolTip in the 
        '''                              TreeView of required WS Elements in Rotor Positioning Screen
        ''' </remarks>
        Private Function UpdateRequiredElementStatusByAddingOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                                       ByVal pWorkSessionID As String, ByVal pSampleClass As String, _
                                                                       ByVal pElementID As Integer, Optional pSpecimenIDList As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update flag ElementFinished=FALSE for the informed ElementID 
                        Dim myWSReqElementsDelegate As New WSRequiredElementsDelegate
                        resultData = myWSReqElementsDelegate.UpdateElementFinished(dbConnection, pElementID, False, pSpecimenIDList)

                        If (Not resultData.HasError) Then
                            'Verify if there are tubes of the informed ElementID positioned in Samples Rotor to change 
                            'also the Position Status to PENDING if they were FINISHED
                            Dim rcp_del As New WSRotorContentByPositionDelegate

                            resultData = rcp_del.GetPositionedElements(dbConnection, pAnalyzerID, "SAMPLES", pElementID.ToString)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim samplesRotorDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                'Dim myStatus As String = String.Empty
                                For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In samplesRotorDS.twksWSRotorContentByPosition
                                    If (String.Compare(row.Status, "FINISHED", False) = 0) Then
                                        resultData = rcp_del.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", row.CellNumber, _
                                                                                            "PENDING", 0, 0, True, True)
                                    End If
                                Next
                            End If
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.UpdateRequiredElementPositionStatusByAddingOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NEW FUNCTIONS FOR HISTORIC MODULE"
        ''' <summary>
        ''' Export all accepted and validated Quality Control Results and all the needed data to QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/10/2012
        ''' </remarks>
        Private Function ExportQCData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        Dim StartTime As DateTime = Now
                        'Dim myLogAcciones As New ApplicationLogManager()
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                        'For TestTypes/Tests/SampleTypes with Controls requested in the WorkSession, verify if they exceed the maximum
                        'number of non cumulated QC Results and in this case, execute the cumulation process
                        resultData = ValidateDependenciesOnResetWS(dbConnection, pWorkSessionID, pAnalyzerID, False)

                        If (Not resultData.HasError) Then
                            'Read all QC Results in the WS and export them to QC Module...
                            Dim myResults As New ResultsDelegate
                            resultData = myResults.ExportControlResultsNEW(dbConnection, pWorkSessionID, pAnalyzerID)
                        End If

                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        GlobalBase.CreateLogActivity("Export QC Results to QC Historic Module " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                        "WorkSessionsDelegate.ExportQCData", EventLogEntryType.Information, False)
                        StartTime = Now
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.ExportQCData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer and WorkSession, move all accepted and validated QC Results to QC Module, move all accepted
        ''' and validated Blank, Calibrator and Patient Results to HISTORIC Module, delete all temporary data (excepting Blank and
        ''' Calibrator results) and delete previous results of Blanks and Calibrators when new ones have been saved 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pPreserveRotorPositions">Flag indicating if the content of Reagents and Samples Rotors have to be saved 
        '''                                       to be used in following WorkSessions: default value is TRUE. It is set to FALSE
        '''                                       when the function is called due to the connected Analyzer is different of the one
        '''                                       in which the specified WorkSession was executed</param>
        ''' <param name="pSaveLISPendingOrders"></param>
        ''' <param name="pIsUpdateProcess"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS
        ''' Modified by: AG 14/05/2010 - Reset the new table base lines by well
        '''              SA 18/05/2010 - Added deletion of records in table twksOrderCalculatedTests
        '''              DL 04/06/2010 - Added deletion of records in table twksResults
        '''              SA 07/06/2010 - Added deletion of records in table twksWSExecutionAlarms
        '''              SG 16/07/2010 - Added deletion of records in table twksWSRepetitionsToAdd 
        '''              SG 19/07/2010 - Added deletion of records in table twksResultAlarms
        '''              SA 07/09/2010 - Added code to restart Identity Column OrderTestID in twksOrderTests 
        '''                              once the Reset WS has finished
        '''              RH 15/04/2011 - Remove deletion of records in tables twksWSExecutionAlarms and twksAnalyzerAlarms
        '''                              Now Alarms are kept as historical info so, don't clear it
        '''              SA 19/04/2011 - Save all non free positions in the Reagents Rotor in an internal Virtual Rotor
        '''                              before reset all positions
        '''              SA 20/06/2011 - Added validation of maximum number of QC Results pending to accumulate for the 
        '''                              Tests/SampleTypes with Controls requested in the WorkSession to reset
        '''              SA 20/07/2011 - Added reading of all QC Results in the Analyzer WorkSession and export to QC Module 
        '''              DL 04/08/2011 - Depending of value of User Setting RESETWS_DOWNLOAD_ONLY_PATIENTS, save all non free 
        '''                              positions containing Calibrators, Controls and/or Additional Solutions in the Samples Rotor 
        '''                              in an internal Virtual Rotor before reset all positions
        '''              SA 04/10/2011 - Activated again the code for deleting WSAnalyzerAlarms
        '''             XBC 14/06/2012 - Added pPreserveRotorPositions parameter with the aim to delete Rotor Positions when change of Analyzer
        '''              SA 04/09/2012 - Added call to function ExportValidatedResultsAndAlarms in ResultsDelegate to allow move to Historic Module
        '''                              all accepted and validated Blank, Calibrator and Patient Results in the specified Analyzer WorkSession
        '''              JB 10/10/2012 - Added call to function CurveResultsDelegate.DeleteForNOTCALCCalibrators
        '''                              Added call to function ResultsDelegate.DeleteOldBlankCalib
        '''              SA 28/11/2012 - Fixed error when calling function ExportQCData: order of parameters AnalyzerID and WorkSessionID were  
        '''                              interchanged
        '''              XB 23/04/2013 - ResetWSNEW is replaced by ResetWS_5DB_TRANS
        '''              XB 25/04/2013 - Add new optional pSaveLISPendingOrders parameter to indicate if the process to Save the LIS orders not processed Is required or not 
        '''              AG + TR 02/05/2013 - open the transaction only when pIsUpdateProcess is TRUE (also for close it)
        '''              TR 07/01/2013 -Clear commented code to make the function readable.
        '''              AG 17/11/2014 BA-2065 new parameter pAnalyzerModel
        ''' </remarks>
        Public Function ResetWSNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String, _
                                   Optional ByVal pPreserveRotorPositions As Boolean = True, Optional ByVal pSaveLISPendingOrders As Boolean = True, _
                                   Optional pIsUpdateProcess As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'AG + TR 02/05/2013
                If pIsUpdateProcess Then
                    resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    End If
                End If

                ' XB 23/04/2013 - ResetWSNEW is replaced by ResetWS_5DB_TRANS
                resultData = ResetWS_5DB_TRANS(pDBConnection, pAnalyzerID, pWorkSessionID, pAnalyzerModel, pPreserveRotorPositions, pSaveLISPendingOrders, pIsUpdateProcess) 'AG 17/11/2014 BA-2065 inform analyzerModel

                'AG + TR 02/05/2013
                If pIsUpdateProcess Then
                    If (Not resultData.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Analyzer and WorkSession, move all accepted and validated QC Results to QC Module, move all accepted
        ''' and validated Blank, Calibrator and Patient Results to HISTORIC Module, delete all temporary data (excepting Blank and
        ''' Calibrator results) and delete previous results of Blanks and Calibrators when new ones have been saved 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPreserveRotorPositions">Flag indicating if the content of Reagents and Samples Rotors have to be saved 
        '''                                       to be used in following WorkSessions: default value is TRUE. It is set to FALSE
        '''                                       when the function is called due to the connected Analyzer is different of the one
        '''                                       in which the specified WorkSession was executed</param>
        ''' <param name="pSaveLISPendingOrders">Flag indicating if the Block to save LIS orders not processed must be executed or not. Optional 
        '''                                     parameter; default value is True</param>
        ''' <param name="pIsUpdateProcess">Flag indicating if the function has been called during the Update Version process. When True, it means the 
        '''                                open DB Connection received as parameter has to used to guarantee the Update Version process is executed in
        '''                                an unique DB Transaction. Optional parameter; default value is False</param>
        ''' <returns>GlobalDataTO containing an string value with the list of errors found (if any; if more than one, then they are
        '''          divided by pipe charaters). Possible errors can be the following ones: QC_EXPORT_FAILED, HIST_EXPORT_FAILED and/or
        '''          RESET_WS_FAILED</returns>
        ''' <remarks>
        ''' Created by:  GDS
        ''' Modified by: AG 14/05/2010 - Reset the new table base lines by well
        '''              SA 18/05/2010 - Added deletion of records in table twksOrderCalculatedTests
        '''              DL 04/06/2010 - Added deletion of records in table twksResults
        '''              SA 07/06/2010 - Added deletion of records in table twksWSExecutionAlarms
        '''              SG 16/07/2010 - Added deletion of records in table twksWSRepetitionsToAdd 
        '''              SG 19/07/2010 - Added deletion of records in table twksResultAlarms
        '''              SA 07/09/2010 - Added code to restart Identity Column OrderTestID in twksOrderTests 
        '''                              once the Reset WS has finished
        '''              RH 15/04/2011 - Remove deletion of records in tables twksWSExecutionAlarms and twksAnalyzerAlarms
        '''                              Now Alarms are kept as historical info so, don't clear it
        '''              SA 19/04/2011 - Save all non free positions in the Reagents Rotor in an internal Virtual Rotor
        '''                              before reset all positions
        '''              SA 20/06/2011 - Added validation of maximum number of QC Results pending to accumulate for the 
        '''                              Tests/SampleTypes with Controls requested in the WorkSession to reset
        '''              SA 20/07/2011 - Added reading of all QC Results in the Analyzer WorkSession and export to QC Module 
        '''              DL 04/08/2011 - Depending of value of User Setting RESETWS_DOWNLOAD_ONLY_PATIENTS, save all non free 
        '''                              positions containing Calibrators, Controls and/or Additional Solutions in the Samples Rotor 
        '''                              in an internal Virtual Rotor before reset all positions
        '''              SA 04/10/2011 - Activated again the code for deleting WSAnalyzerAlarms
        '''             XBC 14/06/2012 - Added pPreserveRotorPositions parameter with the aim to delete Rotor Positions when change of Analyzer
        '''              SA 04/09/2012 - Added call to function ExportValidatedResultsAndAlarms in ResultsDelegate to allow move to Historic Module
        '''                              all accepted and validated Blank, Calibrator and Patient Results in the specified Analyzer WorkSession
        '''              JB 10/10/2012 - Added call to function CurveResultsDelegate.DeleteForNOTCALCCalibrators
        '''                              Added call to function ResultsDelegate.DeleteOldBlankCalib
        '''              SA 16/10/2012 - Implemented FOUR DB Transactions blocks: export QC results to QC Module, export Blank, Calibrator and Patient 
        '''                              Results to Historic Module, delete temporary WS data, and delete old Blank and Calibrator results when new
        '''                              ones were calculated in the active Work Session
        '''                            - Called new function ExportQCData that unifies function ValidateDependenciesOnResetWS and ExportControlResults
        '''                              in an unique and independent DB Transaction
        '''                            - Function DeleteOldBlankCalib is now called in an independent DB Transaction when the deletion of temporary
        '''                              WS data finishes succesfully
        '''              XB 22/04/2013 - Rescue this code to implement the Reset functionality divided on 5 independent transactions 
        '''                            - Add methods to save LIS pending order tests on current WS
        '''                            - Particular Reset for twksOrderTestsLISInfo table
        '''              XB 25/04/2013 - Added new optional pSaveLISPendingOrders parameter to indicate if the process to Save the LIS orders not processed Is required or not 
        '''              AG/TR 02/05/2013 - When flag pIsUpdateProcess is TRUE, function executes the standard steps to Open/Close the needed DB Transaction, using the
        '''                                 open DB Connection received as parameter
        '''              AG 18/11/2013 - (#1385) Delete all positions in process for analyzer
        '''              XB 05/02/2014 - Do not save previous Orders from LIS (Patch 2.1.1c) - Task #1491
        '''              AG 17/11/2014 BA-2065 inform analyzerModel
        '''              AG 15/01/2015 BA-2212
        ''' </remarks>
        Public Function ResetWS_5DB_TRANS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pAnalyzerModel As String, _
                                   Optional ByVal pPreserveRotorPositions As Boolean = True, Optional ByVal pSaveLISPendingOrders As Boolean = True, _
                                   Optional pIsUpdateProcess As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                Dim myResults As New ResultsDelegate
                Dim myErrorCodes As List(Of String) = Nothing
                Dim myErrorCode As String

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'TR 30/04/2013 - The open DB Connection has to be used if the function has been called during Update Version process
                If (pIsUpdateProcess) Then
                    resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    End If
                End If

                'FIRST BLOCK: EXPORT QC RESULTS TO QC HISTORIC MODULE
                resultData = ExportQCData(dbConnection, pAnalyzerID, pWorkSessionID)
                If (resultData.HasError) Then
                    If (myErrorCodes Is Nothing) Then myErrorCodes = New List(Of String)
                    myErrorCode = GlobalEnumerates.Messages.RESET_WS_FAILED.ToString
                    myErrorCodes.Add(myErrorCode)
                    GlobalBase.CreateLogActivity("Reset WS Failed - FIRST BLOCK: EXPORT QC RESULTS TO QC HISTORIC MODULE", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Error, False)
                End If

                'SECOND BLOCK: EXPORT BLANK; CALIBRATOR AND PATIENT RESULTS TO HISTORIC MODULE
                resultData = myResults.ExportValidatedResultsAndAlarms(dbConnection, pAnalyzerID, pWorkSessionID)
                If (resultData.HasError) Then
                    If (myErrorCodes Is Nothing) Then myErrorCodes = New List(Of String)
                    myErrorCode = GlobalEnumerates.Messages.RESET_WS_FAILED.ToString
                    myErrorCodes.Add(myErrorCode)
                    GlobalBase.CreateLogActivity("Reset WS Failed - SECOND BLOCK: EXPORT BLANK; CALIBRATOR AND PATIENT RESULTS TO HISTORIC MODULE ", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Error, False)
                End If

                'THIRD BLOCK: SAVE ROTORS
                If (pPreserveRotorPositions) Then ' XBC 14/06/2012
                    'Save content of Reagents Rotor positions in an internal VirtualRotor before reset the Rotor
                    resultData = SaveReagentsRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID)

                    If (Not resultData.HasError) Then
                        'Get value of User Setting RESETWS_DOWNLOAD_ONLY_PATIENTS
                        Dim myUserSettingDelegate As New UserSettingsDelegate
                        resultData = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.RESETWS_DOWNLOAD_ONLY_PATIENTS.ToString())

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            If (CType(resultData.SetDatos, Boolean)) Then
                                'Save content of Samples Rotor positions in an internal VirtualRotor before reset the Rotor
                                resultData = SaveSamplesRotorPositions(dbConnection, pAnalyzerID, pWorkSessionID)
                            End If
                        End If
                    End If

                    If (resultData.HasError) Then
                        If (myErrorCodes Is Nothing) Then myErrorCodes = New List(Of String)
                        myErrorCode = GlobalEnumerates.Messages.RESET_WS_FAILED.ToString
                        myErrorCodes.Add(myErrorCode)
                        GlobalBase.CreateLogActivity("Reset WS Failed - THIRD BLOCK: SAVE ROTORS ", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Error, False)
                    End If
                End If

                'FOURTH BLOCK: SAVE LIS NOT PROCESSED
                If pSaveLISPendingOrders Then
                    resultData = GetOrderTestsForLISReset(dbConnection, pWorkSessionID)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        ' Save found LIS Orders not processed as a new Saved WS
                        Dim myDS As New SavedWSOrderTestsDS
                        Dim mySavedWSDelegate As New SavedWSDelegate
                        myDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                        If myDS.tparSavedWSOrderTests.Rows.Count > 0 Then

                            ' XB 05/02/2014 - Do not save previous Orders from LIS (Patch 2.1.1c) - Task #1491
                            'resultData = mySavedWSDelegate.SaveFromLIS(dbConnection, "LIS " & Now.ToString("yyyyMMdd HH:mm:ss"), myDS)

                            ' and register them into the Log ...
                            Dim myDiffSampleIDs As List(Of String)
                            ' Get the list of different Sample IDs for the Barcode
                            myDiffSampleIDs = (From a In myDS.tparSavedWSOrderTests _
                                               Select a.SampleID Distinct).ToList()

                            Dim myDiffTests As List(Of String)
                            Dim myTests As String

                            If (myDiffSampleIDs.Count > 0) Then
                                GlobalBase.CreateLogActivity("Deleted LIS Orders on RESET: ", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Information, False)
                                For Each myrow As String In myDiffSampleIDs

                                    myDiffTests = (From a In myDS.tparSavedWSOrderTests _
                                                   Where a.SampleID = myrow
                                                   Select a.TestName Distinct).ToList

                                    myTests = ""
                                    For Each myRow2 As String In myDiffTests
                                        myTests = myTests + myRow2 + "; "
                                    Next

                                    GlobalBase.CreateLogActivity("Specimen: [" & myrow & "] for tests : " & myTests, "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Information, False)
                                Next
                            End If
                            ' XB 05/02/2014 - Do not save previous Orders from LIS (Patch 2.1.1c)

                        End If
                    End If
                    If (resultData.HasError) Then
                        If myErrorCodes Is Nothing Then myErrorCodes = New List(Of String)
                        myErrorCode = GlobalEnumerates.Messages.RESET_WS_FAILED.ToString
                        myErrorCodes.Add(myErrorCode)
                        GlobalBase.CreateLogActivity("Reset WS Failed - FOURTH BLOCK: SAVE LIS NOT PROCESSED ", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Error, False)
                    End If
                End If

                'FITH BLOCK: DELETE TEMPORARY DATA
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        If (Not resultData.HasError) Then
                            'Delete Not InUse Rotor Positions for the specified AnalyzerID/WorkSessionID
                            Dim myWSNotInUseRotorPositions As New WSNotInUseRotorPositionsDelegate
                            resultData = myWSNotInUseRotorPositions.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'AG 18/11/2013 - (#1385)
                        If Not resultData.HasError Then
                            'Delete all positions in process for analyzer
                            Dim rotorInProcess As New WSRotorPositionsInProcessDelegate
                            resultData = rotorInProcess.ResetWS(dbConnection, pAnalyzerID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Rotor Positions for the specified AnalyzerID/WorkSessionID
                            Dim myWSRotorContentByPosition As New WSRotorContentByPositionDelegate
                            resultData = myWSRotorContentByPosition.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Tubes for Required WS Elements for the specified AnalyzerID/WorkSessionID
                            Dim myWSRequiredElementsTubes As New WSRequiredElementsTubesDelegate
                            resultData = myWSRequiredElementsTubes.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all relations between Required WS Elements and WS Order Tests for the specified AnalyzerID/WorkSessionID
                            Dim myWSRequiredElemByOrderTest As New WSRequiredElemByOrderTestDelegate
                            resultData = myWSRequiredElemByOrderTest.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then 'GDS - 10/05/2010
                            'Set InUse=False for all Tests included in all WS Order Tests of the specified AnalyzerID/WorkSessionID
                            resultData = Me.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, False)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Required WS Elements for the specified AnalyzerID/WorkSessionID
                            Dim myWSRequiredElements As New WSRequiredElementsDelegate
                            resultData = myWSRequiredElements.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Analyzer Alarms for the specified AnalyzerID/WorkSessionID
                            Dim myWSAnalyzersAlarms As New WSAnalyzerAlarmsDelegate
                            resultData = myWSAnalyzersAlarms.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all base lines by well for the specified AnalyzerID/WorkSessionID
                            Dim myWSBaseLinesByWell As New WSBLinesByWellDelegate
                            resultData = myWSBaseLinesByWell.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID, pAnalyzerModel) 'AG 17/11/2014 BA-2065 inform analyzerModel
                        End If

                        'AG 15/01/2015 BA-2212
                        If (Not resultData.HasError) Then
                            Dim myWSBaseLines As New WSBLinesDelegate
                            resultData = myWSBaseLines.ResetWSForDynamicBL(dbConnection, pAnalyzerID, pWorkSessionID, pAnalyzerModel)
                        End If
                        'AG 15/01/2015

                        If (Not resultData.HasError) Then
                            'Delete all WS Preparations for the specified AnalyzerID/WorkSessionID
                            Dim myWSPreparations As New WSPreparationsDelegate
                            resultData = myWSPreparations.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Analyzer Readings for the specified AnalyzerID/WorkSessionID
                            Dim myWSReadings As New WSReadingsDelegate
                            resultData = myWSReadings.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'DAO is called directly to avoid circular reference calling Delegate
                        If (Not resultData.HasError) Then
                            'Delete all partial Execution results for the specified AnalyzerID/WorkSessionID 
                            Dim myExecutionsPartialResults As New tcalcExecutionsPartialResultsDAO
                            resultData = myExecutionsPartialResults.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        'DAO is called directly to avoid circular reference calling Delegate
                        If (Not resultData.HasError) Then
                            Dim myExecutionsR1Results As New tcalcExecutionsR1ResultsDAO
                            resultData = myExecutionsR1Results.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Execution Alarms (Calculations Remarks) for the specified AnalyzerID/WorkSessionID 
                            Dim myWSExecutionAlarms As New WSExecutionAlarmsDelegate
                            resultData = myWSExecutionAlarms.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all information of the used Reactions Rotor
                            Dim myReactions As New ReactionsRotorDelegate
                            resultData = myReactions.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Execution for the specified AnalyzerID/WorkSessionID 
                            Dim myWSExecutions As New ExecutionsDelegate
                            resultData = myWSExecutions.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete the relation between the Analyzer and the WorkSession
                            Dim myWSAnalyzers As New WSAnalyzersDelegate
                            resultData = myWSAnalyzers.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete relations between Order Tests in the WorkSession and the Calculated Tests included in
                            'the WorkSession in which Formulas are included
                            Dim myCalcOrderTests As New OrderCalculatedTestsDelegate
                            resultData = myCalcOrderTests.DeleteByWorkSession(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all the Repetitions to Add that belong to the WorkSession
                            Dim myWSRepetitionsToAdd As New WSRepetitionsToAddDelegate
                            resultData = myWSRepetitionsToAdd.DeleteAll(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete Order Tests LIS Information
                            Dim myOrderTestsLISInfo As New OrderTestsLISInfoDelegate
                            resultData = myOrderTestsLISInfo.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete relation between OrderTests and the WorkSession
                            Dim myWSOrderTests As New WSOrderTestsDelegate
                            resultData = myWSOrderTests.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete the WorkSession
                            Dim myDAO As New TwksWorkSessionsDAO
                            resultData = myDAO.ResetWS(dbConnection, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all result alarms that were included in the WorkSession
                            Dim myResultAlarms As New ResultAlarmsDelegate
                            resultData = myResultAlarms.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Curves for NOTCALC Calibrators
                            Dim myCurveResults As New CurveResultsDelegate
                            resultData = myCurveResults.DeleteForNOTCALCCalibrators(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Results for the Order Tests included in the WorkSession
                            resultData = myResults.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all old Blank and Calibrator results (only the last executed is maintaining)
                            resultData = myResults.DeleteOldBlankCalib(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Order Tests that were included in the WorkSession
                            Dim myOrderTests As New OrderTestsDelegate
                            resultData = myOrderTests.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all Orders that were included in the WorkSession
                            Dim myOrders As New OrdersDelegate
                            resultData = myOrders.ResetWS(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete session from the table twksWSBarcodePositionsWithNoRequest
                            Dim noInformedSampleID As New BarcodePositionsWithNoRequestsDelegate
                            resultData = noInformedSampleID.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)
                        End If

                        If (Not resultData.HasError) Then
                            'Restart the Identity Column OrderTestID in table twksOrderTests
                            Dim myOrderTests As New OrderTestsDelegate
                            resultData = myOrderTests.RestartIdentity(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                            'If the process had failed, add the Error Code to the list of errors to return
                            If (myErrorCodes Is Nothing) Then myErrorCodes = New List(Of String)
                            myErrorCode = GlobalEnumerates.Messages.RESET_WS_FAILED.ToString
                            myErrorCodes.Add(myErrorCode)
                            GlobalBase.CreateLogActivity("Reset WS Failed - FITH BLOCK: DELETE TEMPORARY DATA ", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Error, False)
                        End If
                    End If
                End If

                'AG + TR 02/05/2013
                If (pIsUpdateProcess) Then
                    If (Not resultData.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("Total time WorkSession reset + export " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Information, False)
                StartTime = Now
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'Return the list of errors (if any)...
                resultData.SetDatos = myErrorCodes
                If Not myErrorCodes Is Nothing AndAlso myErrorCodes.Count > 0 Then
                    resultData.HasError = True
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.ResetWSNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE-REVIEW"

        '''' <summary>
        '''' Get definition of Controls for the informed TestID/SampleType (ID, Control Number, Name and Lot Number) and return 
        '''' the values.  When the process for Controls is finished, the function that processes the Calibrator and Blank for the 
        '''' informed TestID/SampleType is called
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pTestVersionNumber">Current Test Version Number</param>
        '''' <param name="pMaxDaysLastCalibBlk">Maximum number of days that can have passed from the last execution of Calibrators 
        ''''                                    and/or Blank. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSAdditionalElementsDS with Controls, Calibrators and Blanks 
        ''''          needed for the informed TestID/SampleType</returns>
        '''' <remarks>
        '''' Created by:  TR
        '''' Modified by: SA 08/01/2010 - Changes to return a GlobalDataTO instead a typed DataSet WSAdditionalElementsDS. Changes
        ''''                              to implement the open of a DB Connection according the new template. 
        '''' </remarks>
        'Public Function AddControlsForWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
        '                                          ByVal pSampleType As String, ByVal pTestVersionNumber As Integer, _
        '                                          Optional ByVal pMaxDaysLastCalibBlk As String = "") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestControlsDS As New TestControlsDS
        '                Dim myTestControlsDelegate As New TestControlsDelegate

        '                'Get Controls defined for the specified Test and Sample Type
        '                ' modified by : DL 02/02/2010
        '                Dim resultdt As New GlobalDataTO

        '                resultdt = myTestControlsDelegate.GetControls(Nothing, pTestID, pSampleType)
        '                myTestControlsDS = CType(resultdt.SetDatos, TestControlsDS)
        '                'myTestControlsDS = myTestControlsDelegate.GetControls(pTestID, pSampleType)

        '                'If there are defined Controls for the Test and Sample Type...
        '                Dim result As New WSAdditionalElementsDS
        '                If (myTestControlsDS.tparTestControls.Rows.Count > 0) Then
        '                    For Each mytesControlDR As TestControlsDS.tparTestControlsRow In myTestControlsDS.tparTestControls.Rows
        '                        'Replace possible null values
        '                        If (mytesControlDR.IsLotNumberNull) Then mytesControlDR.LotNumber = ""

        '                        'Add Controls to the DataSet to return
        '                        Dim additionalElementRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow
        '                        additionalElementRow = result.WSAdditionalElementsTable.NewWSAdditionalElementsTableRow

        '                        additionalElementRow.SampleClass = "CTRL"
        '                        additionalElementRow.TestID = pTestID
        '                        additionalElementRow.SampleType = pSampleType
        '                        additionalElementRow.ControlID = mytesControlDR.ControlID
        '                        additionalElementRow.ControlNumber = mytesControlDR.ControlNum
        '                        additionalElementRow.ControlName = mytesControlDR.ControlName
        '                        additionalElementRow.LotNumber = mytesControlDR.LotNumber
        '                        additionalElementRow.AlternativeFlag = False

        '                        result.WSAdditionalElementsTable.Rows.Add(additionalElementRow)
        '                    Next
        '                End If

        '                'Get the required Calibrator and Blank for the informed Test and SampleType 
        '                resultData = AddCalibratorForWorkSession(dbConnection, pTestID, pSampleType, pTestVersionNumber, pMaxDaysLastCalibBlk)
        '                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '                    Dim myWSAdditionalElementsTempDS As New WSAdditionalElementsDS
        '                    myWSAdditionalElementsTempDS = CType(resultData.SetDatos, WSAdditionalElementsDS)

        '                    'If there are Calibrator and Blank records...
        '                    If (myWSAdditionalElementsTempDS.WSAdditionalElementsTable.Rows.Count > 0) Then
        '                        'Insert the obtained records in the DataSet to return 
        '                        For Each myBlankCalibDR As WSAdditionalElementsDS.WSAdditionalElementsTableRow In _
        '                                                   myWSAdditionalElementsTempDS.WSAdditionalElementsTable.Rows
        '                            result.WSAdditionalElementsTable.ImportRow(myBlankCalibDR)
        '                        Next
        '                    End If
        '                End If

        '                'Returns the DataSet containing Controls, Calibrator and Blank for the informed TestID/SampleType
        '                If (Not resultData.HasError) Then resultData.SetDatos = result
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddControlsForWorkSession", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the Calibrator definition for the informed TestID/SampleType and return its values. Additionally, 
        '''' if there are results from a previous Calibrator execution, they are also returned. When the process for 
        '''' Calibrator is finished, the function that processes the Blank for the informed TestID is called 
        '''' </summary>
        '''' <param name="pTestId">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type</param>
        '''' <param name="pTestVersionNumber">Current Test Version</param>
        '''' <param name="pMaxDaysLastCalibBlk">Maximum number of days that can have passed from the last
        ''''                                     execution of Calibrators and/or Blank. Optional parameter</param>
        '''' <returns>A DataSet containing Calibrator and Blank needed for the informed TestID/SampleType</returns>
        '''' <remarks>
        '''' Created by: TR
        '''' Modified by: SA 08/01/2010 - Changes to return a GlobalDataTO instead a typed DataSet WSAdditionalElementsDS. Changes
        ''''                              to implement the open of a DB Connection according the new template. 
        ''''              SA 08/01/2010 - New optional parameter pWorkSessionID. When this parameter is informed, verify if the 
        ''''                              Calibrator already exists as a Required Element in the informed WorkSession, and in that case
        ''''                              finish the function without add the Calibrator.        '''                 
        '''' </remarks>
        'Public Function AddCalibratorForWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestId As Integer, _
        '                                            ByVal pSampleType As String, ByVal pTestVersionNumber As Integer, _
        '                                            Optional ByVal pMaxDaysLastCalibBlk As String = "", _
        '                                            Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestCalibratorDelegate As New TestCalibratorsDelegate
        '                Dim myTestSampleCalibratorDS As New TestSampleCalibratorDS

        '                'Get Calibrator definition for the informed TestID/SampleType
        '                resultData = myTestCalibratorDelegate.GetTestCalibratorData(dbConnection, pTestId, pSampleType)

        '                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '                    'Get the DataSet contained in the GlobalDataTO
        '                    myTestSampleCalibratorDS = CType(resultData.SetDatos, TestSampleCalibratorDS)

        '                    'The DataSet containing the Calibrator definition for the TestID/SampleType has usually just one row, but 
        '                    'when the CalibratorType is defined as Alternative, then it has two rows, the first one containing the 
        '                    'Calibrator definition for the same TestID but the SampleType defined as the alternative of the original one, 
        '                    'and the second one for the original TestID/SampleType indicating it is using an alternative and which one
        '                    'is the alternative SampleType
        '                    Dim result As New WSAdditionalElementsDS
        '                    For Each myTestSampleCalDR As TestSampleCalibratorDS.tparTestCalibratorsRow _
        '                                               In myTestSampleCalibratorDS.tparTestCalibrators.Rows
        '                        'The Calibrator Type is Factor or Experimental 
        '                        If (Not myTestSampleCalDR.AlternativeFlag) Then
        '                            'Search if there is a recent result for the Calibrator executed for the same Test and SampleType
        '                            'Previous result can exist only for Experimental Calibrators
        '                            Dim myResultsDelegate As New ResultsDelegate
        '                            'resultData = myResultsDelegate.GetLastExecutedCalibrator(dbConnection, myTestSampleCalDR.TestID, myTestSampleCalDR.SampleType, _
        '                            '                                                         pTestVersionNumber, pMaxDaysLastCalibBlk)
        '                            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '                                result = CType(resultData.SetDatos, WSAdditionalElementsDS)

        '                                'Previous results were found...
        '                                If (result.WSAdditionalElementsTable.Rows.Count > 0) Then
        '                                    'For MultiPoint Calibrators, there will be an Absorbance result by each Point
        '                                    For Each myLastCalibratorROW As WSAdditionalElementsDS.WSAdditionalElementsTableRow _
        '                                                                 In result.WSAdditionalElementsTable.Rows
        '                                        'Replace values in case of Null fields
        '                                        If (myTestSampleCalDR.IsLotNumberNull) Then myTestSampleCalDR.LotNumber = ""

        '                                        'Complete fields containing Calibrator definition 
        '                                        myLastCalibratorROW.BeginEdit()
        '                                        myLastCalibratorROW.CalibratorID = myTestSampleCalDR.CalibratorID
        '                                        myLastCalibratorROW.CalibratorName = myTestSampleCalDR.CalibratorName
        '                                        myLastCalibratorROW.LotNumber = myTestSampleCalDR.LotNumber
        '                                        myLastCalibratorROW.AlternativeFlag = myTestSampleCalDR.AlternativeFlag
        '                                        myLastCalibratorROW.CalibratorType = myTestSampleCalDR.CalibratorType

        '                                        'For MultiPoint Calibrators, value of Calibrator Factor is set to zero
        '                                        If (result.WSAdditionalElementsTable.Rows.Count > 1) Then
        '                                            myLastCalibratorROW.CalibratorFactor = 0
        '                                        Else
        '                                            myLastCalibratorROW.CalibratorFactor = myTestSampleCalDR.CalibratorFactor
        '                                        End If
        '                                        myLastCalibratorROW.EndEdit()
        '                                    Next
        '                                Else
        '                                    'Previous results were not found...

        '                                    'Replace possible null values
        '                                    If myTestSampleCalDR.IsCalibratorIDNull Then myTestSampleCalDR.CalibratorID = 0
        '                                    If myTestSampleCalDR.IsCalibratorNameNull Then myTestSampleCalDR.CalibratorName = ""
        '                                    If myTestSampleCalDR.IsLotNumberNull Then myTestSampleCalDR.LotNumber = ""
        '                                    If myTestSampleCalDR.IsCalibratorFactorNull Then myTestSampleCalDR.CalibratorFactor = 0

        '                                    'Add the Calibrator information
        '                                    Dim additionalElementRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow
        '                                    additionalElementRow = result.WSAdditionalElementsTable.NewWSAdditionalElementsTableRow

        '                                    additionalElementRow.SampleClass = "CALIB"
        '                                    additionalElementRow.TestID = pTestId
        '                                    additionalElementRow.SampleType = myTestSampleCalDR.SampleType
        '                                    additionalElementRow.CalibratorID = myTestSampleCalDR.CalibratorID
        '                                    additionalElementRow.CalibratorName = myTestSampleCalDR.CalibratorName
        '                                    additionalElementRow.CalibratorFactor = myTestSampleCalDR.CalibratorFactor
        '                                    additionalElementRow.CalibratorType = myTestSampleCalDR.CalibratorType
        '                                    additionalElementRow.LotNumber = myTestSampleCalDR.LotNumber
        '                                    additionalElementRow.AlternativeFlag = False
        '                                    additionalElementRow.MultiPointNumber = 1

        '                                    result.WSAdditionalElementsTable.Rows.Add(additionalElementRow)
        '                                End If
        '                            End If
        '                        Else
        '                            'Then it corresponds to the row added when the Calibrator uses an Alternative one
        '                            'This additional row is also added to the DataSet to return
        '                            Dim additionalElementRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow
        '                            additionalElementRow = result.WSAdditionalElementsTable.NewWSAdditionalElementsTableRow

        '                            additionalElementRow.SampleClass = "CALIB"
        '                            additionalElementRow.TestID = pTestId
        '                            additionalElementRow.SampleType = myTestSampleCalDR.SampleType
        '                            additionalElementRow.CalibratorType = myTestSampleCalDR.CalibratorType
        '                            additionalElementRow.AlternativeFlag = True
        '                            additionalElementRow.AlternativeSampleType = myTestSampleCalDR.AlternativeSampleType

        '                            result.WSAdditionalElementsTable.Rows.Add(additionalElementRow)
        '                        End If
        '                    Next

        '                    'Get the required Blank for the informed Test 
        '                    Dim myWSAdditionalElementsTempDS As New WSAdditionalElementsDS
        '                    resultData = AddBlankForWorkSession(dbConnection, pTestId, pTestVersionNumber, pMaxDaysLastCalibBlk)
        '                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '                        myWSAdditionalElementsTempDS = CType(resultData.SetDatos, WSAdditionalElementsDS)

        '                        'If there are Blank records...
        '                        If (myWSAdditionalElementsTempDS.WSAdditionalElementsTable.Rows.Count > 0) Then
        '                            'Add the obtained records to the DataSet to return
        '                            result.WSAdditionalElementsTable.ImportRow(myWSAdditionalElementsTempDS.WSAdditionalElementsTable.Rows(0))
        '                        End If
        '                    End If

        '                    'Returns the DataSet containing Calibrator and Blank for the informed TestID/SampleType
        '                    If (Not resultData.HasError) Then resultData.SetDatos = result
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddCalibratorForWorkSession", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Search if there are results from a previous Blank execution for the informed TestID and return
        '''' the value. If there is not a previous result, add a Blank row in the DataSet to return 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestId">Test Identifier</param>
        '''' <param name="pTestVersionNumber">Current Test Version</param>
        '''' <param name="pMaxDaysLastCalibBlk">Maximum number of days that can have passed from the last
        ''''                                     execution of Calibrators and/or Blank. Optional parameter</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSAdditionalElementsDS with the Blank required for the informed TestID</returns>
        '''' <remarks>
        '''' Create by:  TR
        '''' Modified by: SA 08/01/2010 - Changes to return a GlobalDataTO instead a typed DataSet WSAdditionalElementsDS. Changes
        ''''                              to implement the open of a DB Connection according the new template. 
        '''' </remarks>
        'Public Function AddBlankForWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
        '                                       ByVal pTestVersionNumber As Integer, Optional ByVal pMaxDaysLastCalibBlk As String = "") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim mytwksResultDelegate As New ResultsDelegate()

        '                'Search if there is a recent result for the Blank executed for the informed Test
        '                'resultData = mytwksResultDelegate.GetLastExecutedBlank(dbConnection, pTestID, pTestVersionNumber, pMaxDaysLastCalibBlk)
        '                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '                    Dim result As New WSAdditionalElementsDS
        '                    result = CType(resultData.SetDatos, WSAdditionalElementsDS)

        '                    'A previous result was not found; add a new row informing SampleClass and TestID
        '                    If (result.WSAdditionalElementsTable.Rows.Count = 0) Then
        '                        Dim myLastBlankRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow

        '                        myLastBlankRow = result.WSAdditionalElementsTable.NewWSAdditionalElementsTableRow
        '                        myLastBlankRow.TestID = pTestID
        '                        myLastBlankRow.SampleClass = "BLANK"
        '                        myLastBlankRow.AlternativeFlag = False

        '                        result.WSAdditionalElementsTable.AddWSAdditionalElementsTableRow(myLastBlankRow)
        '                    Else
        '                        'A previous result was found, field AlternativeFlag is set to null
        '                        result.WSAdditionalElementsTable.Rows(0).BeginEdit()
        '                        result.WSAdditionalElementsTable.Rows(0).Item("AlternativeFlag") = False
        '                        result.WSAdditionalElementsTable.Rows(0).EndEdit()
        '                    End If

        '                    'Return the typed DataSet 
        '                    resultData.SetDatos = result
        '                    resultData.HasError = False
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddBlankForWorkSession", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the list of Required Elements needed for each one of the informed Order Tests. The SampleClass of 
        '''' the Order to which each Order Test belongs will determine the needed Required Elements in the following 
        '''' way:
        '''' - For Order Tests belonging to Blank Orders, Elements for Reagents of the corresponding Test
        '''' - For Order Tests belonging to Calibrator Orders, Elements for the Calibrator defined for the corresponding
        ''''   Test and Sample Type plus Elements for Reagents of the corresponding Test
        '''' - For Order Tests belonging to Control Orders, Elements for the Controls defined for the corresponding
        ''''   Test and Sample Type plus Elements for the Calibrator defined for the corresponding Test and Sample Type 
        ''''   plus Elements for Reagents of the corresponding Test
        '''' - For Order Tests belonging to Patient Orders, Elements for the Sample Type of the Patient plus Elements for 
        ''''   the Controls defined for the corresponding Test and Sample Type plus Elements for the Calibrator defined 
        ''''   for the corresponding Test and Sample Type plus Elements for Reagents of the corresponding Test
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        '''' <returns>List of required Elements needed for each one of the Order Tests in the list</returns>
        '''' <remarks>
        '''' Created by:  SA
        '''' Modified by: VR 10/12/2009 - Changed the Return dataset WSElementsByOrderTestDS to GlobalDataTO - Tested: PENDING
        ''''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template 
        ''''              SA 05/01/2010 - Changes due to all the called functions have been modified to return a GlobalDataTO 
        ''''                              instead a specific typed DataSet
        ''''              SA 09/03/2010 - Changes due to new field SampleID for Patient Order Tests
        '''' </remarks> 
        'Public Function GetOrderTestElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                     ByVal pOrderTestsList As String) As GlobalDataTO

        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim allRequiredElementsDS As New WSElementsByOrderTestDS
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Get details of each Order Test in the list - Modified by: DL 02/02/2010
        '                Dim otDetailsData As New OrderTestsDelegate
        '                myGlobalDataTO = otDetailsData.GetOrderTestsDetails(dbConnection, pOrderTestsList, pWorkSessionID)
        '                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    Dim otDetailsDS As New OrderTestsDetailsDS
        '                    otDetailsDS = CType(myGlobalDataTO.SetDatos, OrderTestsDetailsDS)

        '                    Dim noError As Boolean = True
        '                    Dim requiredElementsDS As New WSElementsByOrderTestDS
        '                    Dim requiredElementsData As New WSRequiredElementsDelegate
        '                    For i As Integer = 0 To otDetailsDS.OrderTestsDetails.Rows.Count - 1
        '                        'Get Required Elements needed for each Order Test according the SampleClass of the
        '                        'Order to which the Order Test belongs
        '                        If (otDetailsDS.OrderTestsDetails(i).SampleClass.Trim = "PATIENT") Then
        '                            If (otDetailsDS.OrderTestsDetails(i).IsPatientIDNull) Then otDetailsDS.OrderTestsDetails(i).PatientID = ""
        '                            If (otDetailsDS.OrderTestsDetails(i).IsSampleIDNull) Then otDetailsDS.OrderTestsDetails(i).SampleID = ""
        '                            myGlobalDataTO = requiredElementsData.GetPatientElements(dbConnection, pWorkSessionID, _
        '                                                                                     otDetailsDS.OrderTestsDetails(i))

        '                        ElseIf (otDetailsDS.OrderTestsDetails(i).SampleClass.Trim = "CTRL") Then
        '                            myGlobalDataTO = requiredElementsData.GetControlElements(dbConnection, pWorkSessionID, _
        '                                                                                     otDetailsDS.OrderTestsDetails(i))

        '                        ElseIf (otDetailsDS.OrderTestsDetails(i).SampleClass.Trim = "CALIB") Then
        '                            myGlobalDataTO = requiredElementsData.GetCalibratorElements(dbConnection, pWorkSessionID, _
        '                                                                                        otDetailsDS.OrderTestsDetails(i))

        '                        ElseIf (otDetailsDS.OrderTestsDetails(i).SampleClass.Trim = "BLANK") Then
        '                            myGlobalDataTO = requiredElementsData.GetBlankElements(dbConnection, pWorkSessionID, _
        '                                                                                   otDetailsDS.OrderTestsDetails(i))
        '                        End If

        '                        noError = (Not myGlobalDataTO.HasError)
        '                        If (Not noError) Then Exit For

        '                        If (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                            requiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSElementsByOrderTestDS)

        '                            'Add all the found Elements to the DataSet to return
        '                            For Each requiredElementsDR As WSElementsByOrderTestDS.twksWSRequiredElemByOrderTestRow _
        '                                                        In requiredElementsDS.twksWSRequiredElemByOrderTest.Rows
        '                                allRequiredElementsDS.twksWSRequiredElemByOrderTest.ImportRow(requiredElementsDR)
        '                            Next
        '                        End If
        '                    Next

        '                    If (noError) Then
        '                        myGlobalDataTO.HasError = False
        '                        myGlobalDataTO.SetDatos = allRequiredElementsDS
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.GetOrderTestElements", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Add Elements for all ISE Test required according the list of Order Tests included in a Work Session
        '''' that has to be sent to an Analyzer and belongs to Orders with SampleClass = BLANK
        '''' </summary>
        '''' <param name="pDBConnection">Open Database Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        '''' <returns>Global object containing error information</returns>
        '''' <remarks>
        '''' Created by: RH 08/06/2011
        '''' </remarks>
        'Public Function AddWSElementsForISEWashing(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                         ByVal pOrderTestsList As String, ByVal pConstantVolumes As Single) As GlobalDataTO
        '    Dim dataToReturn As GlobalDataTO
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)

        '        If (Not dataToReturn.HasError AndAlso Not dataToReturn Is Nothing) Then
        '            dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim IsOK As Boolean = True
        '                'Get data of the Blanks defined for the different pairs of Test/SampleType
        '                'in the list of Order Tests to add to the Work Session
        '                Dim testBlanksData As New OrderTestsDelegate
        '                dataToReturn = testBlanksData.GetBlanksData(dbConnection, pOrderTestsList, pWorkSessionID)
        '                IsOK = (Not dataToReturn.HasError)

        '                If (IsOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
        '                    Dim testBlanksDataDS As TestControlsDS
        '                    testBlanksDataDS = DirectCast(dataToReturn.SetDatos, TestControlsDS)

        '                    'One Element has to be added for each Blank
        '                    Dim wsElementDataDS As New WSRequiredElementsDS
        '                    Dim wsElementFoundDS As WSRequiredElementsDS
        '                    Dim wsElementData As New WSRequiredElementsDelegate

        '                    Dim i As Integer = 0
        '                    Dim wsElementsRow As WSRequiredElementsDS.twksWSRequiredElementsRow = Nothing

        '                    Do While (i < testBlanksDataDS.tparTestControls.Rows.Count) AndAlso (IsOK)
        '                        'Prepare the DataSet Row needed to verify if there is already an Element for the Blank in the WS
        '                        If (i = 0) Then wsElementsRow = wsElementDataDS.twksWSRequiredElements.NewtwksWSRequiredElementsRow
        '                        wsElementsRow.WorkSessionID = pWorkSessionID.Trim
        '                        wsElementsRow.TubeContent = "TUBE_WASH_SOL"
        '                        wsElementsRow.SolutionCode = testBlanksDataDS.tparTestControls(i).ControlName
        '                        wsElementsRow.TubeType = testBlanksDataDS.tparTestControls(i).TubeType
        '                        If (i = 0) Then wsElementDataDS.twksWSRequiredElements.Rows.Add(wsElementsRow)

        '                        Dim addSolutionElement As Boolean = True

        '                        'Get the required total volume of the Additional Solution
        '                        Dim requiredVolume As Single = 0
        '                        Dim orderTestData As New OrderTestsDelegate

        '                        dataToReturn = orderTestData.GetSpecialSolutionVolume( _
        '                                        dbConnection, pOrderTestsList, pConstantVolumes, _
        '                                        wsElementsRow.SolutionCode, pWorkSessionID)

        '                        IsOK = (Not dataToReturn.HasError)

        '                        If (IsOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
        '                            requiredVolume = DirectCast(dataToReturn.SetDatos, Single)

        '                            'Verify if there is already a Required Element for the Blank in the Work Session
        '                            dataToReturn = wsElementData.ExistRequiredElement(dbConnection, wsElementDataDS)

        '                            IsOK = (Not dataToReturn.HasError)

        '                            If (IsOK AndAlso Not dataToReturn Is Nothing) Then
        '                                wsElementFoundDS = CType(dataToReturn.SetDatos, WSRequiredElementsDS)

        '                                If (wsElementFoundDS.twksWSRequiredElements.Rows.Count = 0) Then
        '                                    'An Element was not found, generate the next ElementID to insert it and set Status to Not Positioned
        '                                    dataToReturn = wsElementData.GenerateElementID(dbConnection)

        '                                    IsOK = (Not dataToReturn.HasError)

        '                                    If (IsOK AndAlso Not dataToReturn.SetDatos Is Nothing) Then
        '                                        wsElementsRow.ElementID = DirectCast(dataToReturn.SetDatos, Integer)
        '                                        wsElementsRow.ElementStatus = "NOPOS"
        '                                        wsElementsRow.RequiredVolume = requiredVolume

        '                                        'Add the Blank to the list of required Elements for the specified Work Session
        '                                        'Create Elements for the Blank Tubes
        '                                        dataToReturn = wsElementData.AddRequiredElement(dbConnection, wsElementDataDS)

        '                                        IsOK = (Not dataToReturn.HasError)
        '                                    End If
        '                                Else
        '                                    'An Element already exists in the WS for the Blank --> Nothing to do in this case,
        '                                    'go to process the next Blank in the list
        '                                End If
        '                            End If
        '                        End If

        '                        i += 1
        '                    Loop
        '                End If

        '                If (IsOK) Then
        '                    'When the Database Transaction was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If

        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Transacction was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        dataToReturn = New GlobalDataTO()
        '        dataToReturn.HasError = True
        '        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWSElementsForISEWashing", EventLogEntryType.Error, False)

        '    Finally
        '        'When Database Connection was opened locally, it has to be closed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return dataToReturn

        'End Function

#End Region

#Region "TO TEST - NEW METHODS TO DIVIDE ADD WS IN SEVERAL DB TRANSACTIONS"

        ''' <summary>
        ''' Add a new empty Work Session or update the content of an existing one by adding to it all the required elements according the 
        ''' list of Order Tests that have to be added to it 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSOrderTestsList">Typed DataSet WSOrderTestsDS containing the list of Order Tests that have to be added to the 
        '''                                 Work Session</param>
        ''' <param name="pCreateWS">When True, it indicates the WS has to be created, including positions of all Rotors of the Analyzer, 
        '''                         and it has to return a WorkSessionsDS. When False, it indicates that some Order Tests will be added 
        '''                         to an existing WorkSession</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier in which the Work Session has to be executed</param>
        ''' <param name="pCurrentWSStatus">Current Status of the WS to update. Optional parameter; when informed, it allows to known if the 
        '''                                status have to be updated, which happen in following cases: from OPEN to PENDING and from
        '''                                CLOSED to INPROCESS; in the rest of cases a status change is not needed</param>
        ''' <param name="pSearchNotInUseElements">When True, it indicates the process has to verify if there are not In Use positions in the WorkSession 
        '''                                       that corresponds to the new added Patient elements</param>
        ''' <returns>GlobalDataTO containing error information and, when a WorkSession has been created or updated, a typed DataSet WorkSessionDS, 
        '''          containing the identifier of the Work Session and its current Status, and also information about the Analyzer in which the 
        '''          Work Session is or will be executed</returns>
        ''' <remarks>
        ''' Created by:   SA
        ''' Modified by:  AG 24/11/2009 - Added call to CreateWSRotorPositions 
        '''               VR 10/12/2009 - Changed the Return Dataset to GlobalDataTO
        '''               VR 11/12/2009 - Added the New parameter pCreateWS 
        '''               VR 29/12/2009 - Changed the Constant Value to Enum Value
        '''               SA 04/01/2010 - Changes in code flow to fulfill the function design. Hard code values for testing removed
        '''               SA 04/01/2010 - Changed the way of open the DB Transaction to the new template 
        '''               DL 26/01/2010 - Verify if in the WorkSession there are NOT_INUSE Elements that corresponds to the new added Elements
        '''               SA 02/02/2010 - Informed new field WorkSessionDesc with the same value of WorkSessionID (provisional)
        '''               SA 03/03/2010 - Added optional parameter for the Analyzer Identifier
        '''              GDS 10/05/2010 - Added call to UpdateInUseFlag
        '''               SA 11/05/2010 - Changed WorkSession Status from EMPTY to CREATED (Empty Status has been removed)
        '''               SA 27/05/2010 - Informed also field WSStatus in the DS to return.  Return the DS also in case of update of 
        '''                               an existing Work Session
        '''               SA 09/06/2010 - Called method UpdateInUseFlag to set InUse=False for all elements that have been removed from the 
        '''                               WorkSession (only for WorkSession updation)
        '''               SA 11/06/2010 - Added new optional parameter for the current WS Status, needed in WS update to change the 
        '''                               status when it is needed. Added code to manage the status changing from OPEN to PENDING and/or 
        '''                               from CLOSED to INPROCESS. Changed WorkSession Status from CREATED to OPEN (Created Status has 
        '''                               been removed)
        '''               SA 12/07/2010 - Status of Order Tests sent to positioning have to be changed from OPEN to PENDING also when
        '''                               the WorkSession is created
        '''               SA 26/01/2011 - Added call to function AddWSAdditionalSolutions, to add Diluent Solutions 
        '''               SA 16/02/2011 - Changed the way of adding records to relation table twksWSRequiredElemByOrderTest
        '''               AG 06/06/2011 - Added creation of the Reactions Rotor when a new WorkSession is created
        '''               SA 04/08/2011 - When modify an existing WS, if current WSStatus is EMPTY, it is changed to PENDING
        '''               SA 16/09/2011 - When preparing the WorkSessionsDS to return for update of the active WS, inform also the received WS Status
        '''               SA 07/10/2011 - All "Exit Try" changed for a normal GlobalDataTO.HasError verification. With Exit Try, the DB Transaction
        '''                               was not RollBacked properly
        '''               SA 16/02/2012 - Removed declaration and use of variable additionalVolumes
        '''               SA 10/04/2013 - After calling function FindElementsInNotInUsePosition, call the new function FindPatientsInNotInUsePosition, which
        '''                               verify if there are incomplete Patient Samples that can be completed due to the required Element has been added
        '''                               to the updated WorkSession; added new optional parameter to avoid executing the searching of NOT IN USE elements 
        '''                               when the function has been called during the scanning of Samples Rotor
        '''               SA 30/05/2013 - After calling function FindPatientsInNotInUsePositions, get the returned Boolean value and inform field BCLinkToElem
        '''                               in the WorkSessionsDS to return. This value indicates if at least one of the incomplete Patient Samples placed in 
        '''                               Samples Rotor was linked to one of the required WS Elements (that is to say, the tube is no more a NOT IN USE one)
        '''               SA 22/07/2013 - When parameter pCreateWS is TRUE, check there is not a Work Session in table twksWorkSessions before create a new one. 
        '''                               This change is to avoid creation of a new WorkSession when Reset of the previous one has not finished  (this error has 
        '''                               happened in several customers, and it is probably due to thread synchronization problems) 
        '''               SA 09/09/2013 - When function was called to create an EMPTY WS but there is already a WorkSession in table twksWorkSessions, set new field
        '''                               CreateEmptyWSStopped to TRUE and write a Warning message in the application Log
        '''               SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        '''               SA 27/05/2014 - BT #1519 ==> Changed call to function AddWSElementsForAdditionalSolutions to add required Elements for all the available 
        '''                                            Dilution Solutions for a call to the new specific function AddWSElementsForDilutionSolutions
        '''                                            Changed call to function AddWSElementsForAdditionalSolutions to add required Elements for all the available 
        '''                                            Washing Solutions for a call to the new specific function AddWSElementsForWashingSolutions.  Besides, call the
        '''                                            function after the call to function to add the required Reagents (AddWSElementsForReagents), because to check 
        '''                                            the possible Contaminations the Reagents have to be already created as required Elements
        ''' </remarks>
        Public Function AddWorkSession_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSOrderTestsList As WSOrderTestsDS, _
                                           ByVal pCreateWS As Boolean, ByVal pAnalyzerID As String, Optional ByVal pCurrentWSStatus As String = "", _
                                           Optional ByVal pSearchNotInUseElements As Boolean = True) As GlobalDataTO

            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'An OPEN DB Transaction is received only when this function is called from function OrderTestsDelegate.LoadFromSavedWSToChangeAnalyzer,
                'and in this case, all functions are executed in that DB Transaction. In the rest of the cases, an OPEN DB Transaction is not received 
                'and the new model of several DB Transactions is used
                Dim existOpenDBConn As Boolean = False
                If (Not pDBConnection Is Nothing) Then
                    dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                        existOpenDBConn = (Not dbConnection Is Nothing)
                    End If
                End If

                'Get the logged User
                'Dim currentSession As New GlobalBase
                Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                Dim stopProcess As Boolean = False
                Dim workSessionDataDS As New WorkSessionsDS
                Dim workSessionDR As WorkSessionsDS.twksWorkSessionsRow

                'Was method called to create a new Work Session?
                If (pCreateWS) Then
                    'Check there is not a Work Session in table twksWorkSessions to avoid creation of a new WorkSession when Reset of the
                    'previous one has not finished (this error has happened in several customers, and it is probably due to thread synchronization problems) 
                    Dim wsAnalyzerDelegate As New WSAnalyzersDelegate

                    dataToReturn = wsAnalyzerDelegate.GetActiveWSByAnalyzer(dbConnection, pAnalyzerID)
                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                        Dim activeWorkSessionDS As WSAnalyzersDS = DirectCast(dataToReturn.SetDatos, WSAnalyzersDS)

                        If (activeWorkSessionDS.twksWSAnalyzers.Rows.Count = 0) Then
                            'Create the EMPTY WorkSession 
                            dataToReturn = CreateEmptyWS(dbConnection, pAnalyzerID, loggedUser)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                'Get data of the created WorkSession 
                                workSessionDataDS = DirectCast(dataToReturn.SetDatos, WorkSessionsDS)

                                'AG 24/11/2014 BA-2065
                                If Not workSessionDataDS.twksWorkSessions(0).IsWorkSessionIDNull Then
                                    Dim blWellDlg As New WSBLinesByWellDelegate
                                    dataToReturn = blWellDlg.UpdateWorkSessionID(dbConnection, pAnalyzerID, workSessionDataDS.twksWorkSessions(0).WorkSessionID)
                                End If
                                'AG 24/11/2014

                            Else
                                stopProcess = True
                            End If
                        Else
                            'There is a WorkSession, it is not allowed to create a new one; prepare the DS to return (with ID and Status of the 
                            'existing WS) and stop the creating process
                            workSessionDR = workSessionDataDS.twksWorkSessions.NewtwksWorkSessionsRow()
                            workSessionDR.WorkSessionID = activeWorkSessionDS.twksWSAnalyzers(0).WorkSessionID
                            workSessionDR.WorkSessionStatus = activeWorkSessionDS.twksWSAnalyzers(0).WSStatus
                            workSessionDR.CreateEmptyWSStopped = True   'To write in the application LOG the name of the function who call AddWorkSession 
                            workSessionDataDS.twksWorkSessions.AddtwksWorkSessionsRow(workSessionDR)

                            stopProcess = True

                            'Write the Warning in the Application LOG
                            'Dim myLogAcciones As New ApplicationLogManager()
                            GlobalBase.CreateLogActivity("WARNING: Called to add EMPTY WS when the previous one still exists", "WorkSessionsDelegate.AddWorkSession_NEW", _
                                                            EventLogEntryType.Error, False)
                        End If
                    Else
                        stopProcess = True
                    End If
                Else
                    'When a WorkSession will be updated, create the DS to return informing the WorkSessionID and WSStatus received
                    workSessionDR = workSessionDataDS.twksWorkSessions.NewtwksWorkSessionsRow()
                    workSessionDR.WorkSessionID = pWSOrderTestsList.twksWSOrderTests(0).WorkSessionID
                    workSessionDR.WorkSessionStatus = pCurrentWSStatus
                    workSessionDataDS.twksWorkSessions.AddtwksWorkSessionsRow(workSessionDR)
                End If

                'The WS has to be updated if there are Order Tests to add to it
                If (Not pCreateWS AndAlso pWSOrderTestsList.twksWSOrderTests.Rows.Count > 0) Then
                    'Get the WorkSession Identifier; it is the same for all OrderTest rows, then the one in the first row is obtained
                    Dim workSessionID As String = pWSOrderTestsList.twksWSOrderTests(0).WorkSessionID

                    'Build a string list with all the Order Tests included in the WorkSession that have to be positioned in an Analyzer
                    Dim orderTestsList As String = String.Empty
                    Dim lstNotOpenAndToSend As List(Of String) = (From a As WSOrderTestsDS.twksWSOrderTestsRow In pWSOrderTestsList.twksWSOrderTests _
                                                                 Where Not a.OpenOTFlag AndAlso a.ToSendFlag _
                                                                Select CStr(a.OrderTestID)).ToList
                    orderTestsList = String.Join(",", lstNotOpenAndToSend.ToArray)
                    lstNotOpenAndToSend = Nothing

                    If (orderTestsList.Trim <> String.Empty) Then
                        If (Not existOpenDBConn) Then
                            'DB TRANSACTION #1 - Add the list of Required Elements for all Order Tests that have been sent to positioning
                            dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                            End If
                        End If

                        'DB TRANSACTION #1 - Add the list of Required Elements for all Order Tests that have been sent to positioning
                        If (Not dbConnection Is Nothing) Then
                            'Add Required Elements for all the available Diluent Solutions
                            dataToReturn = AddWSElementsForDilutionSolutions(dbConnection, workSessionID, orderTestsList)

                            'Add Required Elements for all the available ISE Washing Solutions
                            If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForISEWashing(dbConnection, workSessionID, orderTestsList)

                            'Add required Elements for all the Reagents needed according the list of Order Tests
                            If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForReagents(dbConnection, workSessionID, orderTestsList, pCreateWS, pAnalyzerID)

                            'Add Required Elements for all the available Washing Solutions
                            If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForWashingSolutions(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                            'Add required Elements for all the Controls needed according the list of Order Tests
                            If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForControls(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                            'Add required Elements for all the Calibrators needed according the list of Order Tests
                            If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForCalibrators(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                            'Add required Elements for all the Blanks/Additional solutions needed
                            If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForBlanks(dbConnection, workSessionID, orderTestsList, pAnalyzerID)

                            'Add required Elements for all the Patient Samples needed according the list of Order Tests
                            If (Not dataToReturn.HasError) Then dataToReturn = AddWSElementsForPatientSamples(dbConnection, workSessionID, orderTestsList, pCreateWS, _
                                                                                                              pAnalyzerID)

                            'Create all Required Elements by Order Tests
                            If (Not dataToReturn.HasError) Then
                                Dim requiredPosByOrderTestsData As New WSRequiredElemByOrderTestDelegate
                                dataToReturn = requiredPosByOrderTestsData.AddOrderTestElements(dbConnection, workSessionID)
                            End If

                            If (Not existOpenDBConn) Then
                                If (Not dataToReturn.HasError) Then
                                    'When the Database Connection was opened locally, then the Commit is executed
                                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                Else
                                    'When the Database Connection was opened locally, then the Rollback is executed
                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If
                                dbConnection.Close()
                                dbConnection = Nothing
                            End If
                        End If

                        'DB TRANSACTION #2 - Update Status of all Order Tests included in the Work Session from OPEN to PENDING
                        If (Not dataToReturn.HasError) Then
                            Dim orderTestsData As New OrderTestsDelegate
                            dataToReturn = orderTestsData.UpdateStatus(dbConnection, "PENDING", "OPEN", workSessionID, loggedUser)
                        End If
                    Else
                        'Initialize the local GlobalDataTO variable to avoid an error in the next block due to its current value is Nothing... 
                        dataToReturn = New GlobalDataTO
                    End If


                    'DB TRANSACTION #3 - Update InUse flags
                    If (Not dataToReturn.HasError) Then
                        If (Not existOpenDBConn) Then
                            dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                            End If
                        End If

                        If (Not dbConnection Is Nothing) Then
                            'Mark all elements required for the WS as InUse
                            dataToReturn = UpdateInUseFlag(dbConnection, workSessionID, pAnalyzerID, True, False)

                            'When the WorkSession is updated, the InUse flag of all elements removed (if any) have to be set to False
                            If (Not dataToReturn.HasError) Then dataToReturn = UpdateInUseFlag(dbConnection, workSessionID, pAnalyzerID, False, True)
                        End If

                        If (Not existOpenDBConn) Then
                            If (Not dataToReturn.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                            dbConnection.Close()
                            dbConnection = Nothing
                        End If
                    End If

                    'DB TRANSACTION #4 - Update the Work Session Status when needed
                    If (Not dataToReturn.HasError) Then
                        'If the WS was CLOSED, it changes to PENDING when new Order Tests are added to it (positioned or not)
                        'If the WS was EMPTY, it changes to PENDING when new Order Tests are added to it (positioned or not)
                        'If the WS was OPEN and some OrderTests were selected to be positioned, it changes to PENDING
                        Dim newStatus As String = "PENDING"
                        If (pCurrentWSStatus = "EMPTY" AndAlso orderTestsList.Trim = String.Empty) Then newStatus = "OPEN"
                        If (pCurrentWSStatus = "CLOSED") Then newStatus = "INPROCESS"

                        Dim changeStatus As Boolean = (pCurrentWSStatus = "CLOSED") OrElse (pCurrentWSStatus = "EMPTY") OrElse _
                                                      (pCurrentWSStatus = "OPEN" AndAlso orderTestsList.Trim <> String.Empty)

                        If (changeStatus) Then
                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                            dataToReturn = myWSAnalyzersDelegate.UpdateWSStatus(dbConnection, pAnalyzerID, workSessionID, newStatus, pCurrentWSStatus)
                            If (Not dataToReturn.HasError) Then
                                If (dataToReturn.AffectedRecords = 1) Then
                                    'Inform the updated Status in the DS to return
                                    workSessionDataDS.twksWorkSessions(0).BeginEdit()
                                    workSessionDataDS.twksWorkSessions(0).WorkSessionStatus = newStatus
                                    workSessionDataDS.twksWorkSessions(0).EndEdit()
                                End If
                            End If
                        End If
                    End If

                    'DB TRANSACTION #5 - Verify if there are not In Use positions in the WorkSession that corresponds to the new added elements (excepting Patients)
                    If (Not dataToReturn.HasError) Then
                        dataToReturn = FindElementsInNotInUsePosition(dbConnection, workSessionID, pAnalyzerID, Not pSearchNotInUseElements)
                    End If

                    If (pSearchNotInUseElements) Then
                        'DB TRANSACTION #6 - Verify if there are not In Use positions in the WorkSession that corresponds to the new added Patient elements
                        If (Not dataToReturn.HasError) Then
                            dataToReturn = FindPatientsInNotInUsePositions(dbConnection, pAnalyzerID, workSessionID, "SAMPLES")

                            If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                workSessionDataDS.twksWorkSessions(0).BeginEdit()
                                workSessionDataDS.twksWorkSessions(0).BCLinkedToElem = CType(dataToReturn.SetDatos, Boolean)
                                workSessionDataDS.twksWorkSessions(0).EndEdit()
                            End If
                        End If
                    End If
                End If

                'The DataSet containing information of the created or updated WorkSession is returned
                dataToReturn.SetDatos = workSessionDataDS
            Catch ex As Exception
                'If there is a Database Transacction opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.AddWorkSession_NEW", EventLogEntryType.Error, False)
            Finally
                'If there is an opened Database Transaction, then it is closed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Create an EMPTY WorkSession. The WorkSessionID is generated and data is added to following tables:
        ''' ** twksWorkSessions, twksWSAnalyzers, twksWSRotorContentByPosition and twksWSReactionsRotor **
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pLoggedUser">UserName of the logged User</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionsDS with data of the Empty WorkSession created</returns>
        ''' <remarks>
        ''' Created by:  SA 19/03/2014 - BT #1545 ==> Divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Public Function CreateEmptyWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pLoggedUser As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Generate next Work Session ID and fill information required to create a new WorkSession
                        'in the correspondent DataSet (WorkSessionsDS)
                        Dim workSessionDataDS As New WorkSessionsDS
                        Dim workSessionDR As WorkSessionsDS.twksWorkSessionsRow
                        Dim workSessionData As New TwksWorkSessionsDAO

                        workSessionDR = workSessionDataDS.twksWorkSessions.NewtwksWorkSessionsRow()
                        workSessionDR.WorkSessionID = workSessionData.GenerateWorkSessionID(dbConnection)
                        workSessionDR.WorkSessionDesc = workSessionDR.WorkSessionID
                        workSessionDR.WSDateTime = DateTime.Now
                        workSessionDR.TS_User = pLoggedUser.Trim
                        workSessionDR.TS_DateTime = DateTime.Now
                        workSessionDataDS.twksWorkSessions.AddtwksWorkSessionsRow(workSessionDR)

                        'Add the WorkSession
                        dataToReturn = workSessionData.Create(dbConnection, workSessionDataDS)

                        'Create Cells (positions) for all Rotors and Rings in the Analyzer to which the new Work Session has been assigned
                        If (Not dataToReturn.HasError) Then
                            Dim rotorContentByPos As New WSRotorContentByPositionDelegate
                            dataToReturn = rotorContentByPos.CreateWSRotorPositions(dbConnection, workSessionDR.WorkSessionID, pAnalyzerID)
                        End If

                        If (Not dataToReturn.HasError) Then
                            'Include the AnalyzerID in the WorkSession and set the WorkSession Status to the initial one 
                            'Fill a local typed DataSet WSAnalyzersDS
                            Dim analyzerModel As String = String.Empty
                            If (pAnalyzerID.Trim = String.Empty) Then
                                'If the Analyzer has not been informed, then get the one returned by the function of creation or Rotor Positions
                                If (Not dataToReturn.SetDatos Is Nothing) Then
                                    Dim analyzerRotorsConfig As AnalyzerModelRotorsConfigDS = DirectCast(dataToReturn.SetDatos, AnalyzerModelRotorsConfigDS)

                                    pAnalyzerID = analyzerRotorsConfig.tfmwAnalyzerModelRotorsConfig(0).AnalyzerID
                                    analyzerModel = analyzerRotorsConfig.tfmwAnalyzerModelRotorsConfig(0).AnalyzerModel
                                End If
                            Else
                                'Get the model of the informed Analyzer
                                Dim myAnalyzersDelegate As New AnalyzersDelegate

                                dataToReturn = myAnalyzersDelegate.GetAnalyzerModel(Nothing, pAnalyzerID)
                                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                    Dim analyzerDataDS As AnalyzersDS = DirectCast(dataToReturn.SetDatos, AnalyzersDS)
                                    If (analyzerDataDS.tcfgAnalyzers.Rows.Count = 1) Then analyzerModel = analyzerDataDS.tcfgAnalyzers(0).AnalyzerModel
                                End If
                            End If

                            If (Not dataToReturn.HasError) Then
                                'Create the Reactions rotor
                                Dim reactions As New ReactionsRotorDelegate
                                dataToReturn = reactions.CreateWSReactionsRotor(dbConnection, pAnalyzerID, analyzerModel, False)
                            End If

                            If (Not dataToReturn.HasError) Then
                                'Link the EMPTY WorkSession to the Analyzer
                                Dim myWSStatus As String = "EMPTY"
                                Dim myWSAnalyzersDS As New WSAnalyzersDS
                                Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                                Dim myAnalyzersRow As WSAnalyzersDS.twksWSAnalyzersRow

                                myAnalyzersRow = myWSAnalyzersDS.twksWSAnalyzers.NewtwksWSAnalyzersRow()
                                myAnalyzersRow.WorkSessionID = workSessionDR.WorkSessionID
                                myAnalyzersRow.AnalyzerID = pAnalyzerID
                                myAnalyzersRow.WSStatus = myWSStatus
                                myWSAnalyzersDS.twksWSAnalyzers.Rows.Add(myAnalyzersRow)

                                dataToReturn = myWSAnalyzersDelegate.AddAnalyzerToWS(dbConnection, myWSAnalyzersDS)
                                If (Not dataToReturn.HasError) Then
                                    'Add values of WorkSessionID, AnalyzerID, AnalyzerModel and WSStatus to the DataSet to return (WorkSessionDS)
                                    workSessionDataDS.twksWorkSessions(0).BeginEdit()
                                    workSessionDataDS.twksWorkSessions(0).WorkSessionID = workSessionDR.WorkSessionID
                                    workSessionDataDS.twksWorkSessions(0).AnalyzerID = pAnalyzerID
                                    workSessionDataDS.twksWorkSessions(0).AnalyzerModel = analyzerModel
                                    workSessionDataDS.twksWorkSessions(0).WorkSessionStatus = myWSStatus
                                    workSessionDataDS.twksWorkSessions(0).EndEdit()
                                End If
                            End If

                            If (Not dataToReturn.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                dataToReturn.SetDatos = workSessionDataDS
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.CreateEmptyWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Add a Blank or Calibrator Order Test to the WSOrderTestsDS that will be be used to add/update table twksWSOrderTests. This table links all requested Order 
        ''' Tests to the active WorkSession and indicates which Order Tests have to be send to positioning (OpenOTFlag) and which of these have been selected to be 
        ''' positioned (ToSendFlag). The rules to update these flags for Blanks and Calibrators are the following ones:
        ''' ** If button SaveWithoutPositioning has been clicked in IWSSamplesRequest screen, OpenOTFlag is TRUE for all Blank and Calibrator Order Tests.
        '''    Otherwise, value of OpenOTFlag will depend on if the Blank or Calibrator Order Test has been selected for positioning (FALSE) or not (TRUE)
        ''' ** ToSendFlag is FALSE for all Blank or Calibrator Order Tests having a valid previous result that have not been selected for positioning, which means the 
        '''    previous result will be used in the WorkSession. Otherwise, ToSendFlag is TRUE for Blank and Calibrator Order Tests
        ''' </summary>
        ''' <param name="pCreateOpenWS">When TRUE, OpenOTFlag is set to TRUE for the Blank or Calibrator Order Test</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pLoggedUser">Logged User</param>
        ''' <param name="pWSBlankCalibRow">Row of typed DataSet WorkSessionResultDS (subtable BlankCalibrators) containing data of the Blank or Calibrator Order 
        '''                                Test to add</param>
        ''' <param name="pWSOrderTestsDS">Typed DataSet WSOrderTestsDS in which the Blank or Calibrator Order Test has to be added, indicating the value of flags 
        '''                               ToSendFlag and OpenOTFlag</param>
        ''' <remarks>
        ''' Created by:  SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Private Sub BlkCalibWSOrderTestsDSRow(ByVal pCreateOpenWS As Boolean, ByVal pWorkSessionID As String, ByVal pLoggedUser As String, _
                                              ByVal pWSBlankCalibRow As WorkSessionResultDS.BlankCalibratorsRow, ByRef pWSOrderTestsDS As WSOrderTestsDS)
            Try
                Dim myOpenFlag As Boolean = False
                Dim myWSOrderTestDR As WSOrderTestsDS.twksWSOrderTestsRow

                myWSOrderTestDR = pWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a WSOrderTestsRow to load values
                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                myWSOrderTestDR.OrderTestID = pWSBlankCalibRow.OrderTestID

                'When the WS is created OPEN, or when the Order Test has not been selected, it is marked as OPEN
                If (pCreateOpenWS) Then
                    myWSOrderTestDR.OpenOTFlag = True
                Else
                    If (Not pWSBlankCalibRow.Selected) Then
                        'If the Blank Order Test is not selected it is not considered OPEN
                        myWSOrderTestDR.OpenOTFlag = True
                    Else
                        myWSOrderTestDR.OpenOTFlag = False
                    End If
                End If
                myOpenFlag = myWSOrderTestDR.OpenOTFlag

                'All selected Blank Order Tests corresponding to STANDARD Tests are marked to be positioned
                myWSOrderTestDR.ToSendFlag = True
                If (Not pWSBlankCalibRow.Selected) And (Not pWSBlankCalibRow.IsPreviousOrderTestIDNull) Then
                    'Not selected Blanks and Calibrators having previous results are marked to not be positioned 
                    myWSOrderTestDR.ToSendFlag = False
                End If

                myWSOrderTestDR.TS_User = pLoggedUser
                myWSOrderTestDR.TS_DateTime = DateTime.Now
                pWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.BlkCalibWSOrderTestsDSRow", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Add a Control Order Test to the WSOrderTestsDS that will be be used to add/update table twksWSOrderTests. This table links all requested Order 
        ''' Tests to the active WorkSession and indicates which Order Tests have to be send to positioning (OpenOTFlag) and which of these have been selected to be 
        ''' positioned (ToSendFlag). The rules to update these flags for Controls are the following ones:
        ''' ** If button SaveWithoutPositioning has been clicked in IWSSamplesRequest screen, OpenOTFlag is TRUE for all Control Order Tests.
        '''    Otherwise, value of OpenOTFlag will depend on if the Control Order Test has been selected for positioning (FALSE) or not (TRUE)
        ''' ** ToSendFlag is TRUE for all Control Order Tests 
        ''' </summary>
        ''' <param name="pCreateOpenWS">When TRUE, OpenOTFlag is set to TRUE for the Control Order Test</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pLoggedUser">Logged User</param>
        ''' <param name="pCtrlSendingGroup">Number of the Control Sending Group that have to be assigned to the Control</param>
        ''' <param name="pWSCtrlRow">Row of typed DataSet WorkSessionResultDS (subtable Controls) containing data of the Control Order Test to add</param>
        ''' <param name="pWSOrderTestsDS">Typed DataSet WSOrderTestsDS in which the Control Order Test has to be added, indicating the value of flags ToSendFlag 
        '''                               and OpenOTFlag</param>
        ''' <remarks>
        ''' Created by:  SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Private Sub CtrlWSOrderTestsDSRow(ByVal pCreateOpenWS As Boolean, ByVal pWorkSessionID As String, ByVal pLoggedUser As String, ByVal pCtrlSendingGroup As Integer, _
                                          ByVal pWSCtrlRow As WorkSessionResultDS.ControlsRow, ByRef pWSOrderTestsDS As WSOrderTestsDS)
            Try
                Dim myWSOrderTestDR As WSOrderTestsDS.twksWSOrderTestsRow

                'For Tests having more than one Control there is only one OrderTestID 
                myWSOrderTestDR = pWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                myWSOrderTestDR.OrderTestID = pWSCtrlRow.OrderTestID

                'When the WS is created Empty, or when the Order Test has not been selected, it is marked as OPEN
                If (pCreateOpenWS) Then
                    myWSOrderTestDR.OpenOTFlag = True
                Else
                    myWSOrderTestDR.OpenOTFlag = (Not pWSCtrlRow.Selected)
                End If

                'All selected Order Tests corresponding to STANDARD and ISE Tests are marked to be positioned
                myWSOrderTestDR.ToSendFlag = (pWSCtrlRow.TestType = "STD" Or pWSCtrlRow.TestType = "ISE")

                'Set the Ctrl Sending Group.
                If (pCtrlSendingGroup > 0) Then myWSOrderTestDR.CtrlsSendingGroup = pCtrlSendingGroup

                myWSOrderTestDR.TS_User = pLoggedUser
                myWSOrderTestDR.TS_DateTime = DateTime.Now
                pWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.CtrlWSOrderTestsDSRow", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Add a Patient Order Test to the WSOrderTestsDS that will be be used to add/update table twksWSOrderTests. This table links all requested Order 
        ''' Tests to the active WorkSession and indicates which Order Tests have to be send to positioning (OpenOTFlag) and which of these have been selected to be 
        ''' positioned (ToSendFlag). The rules to update these flags for Patients are the following ones:
        ''' ** If button SaveWithoutPositioning has been clicked in IWSSamplesRequest screen, OpenOTFlag is TRUE for all Patient Order Tests.
        '''    Otherwise, value of OpenOTFlag will depend on if the Patient Order Test has been selected for positioning (FALSE) or not (TRUE)
        ''' ** ToSendFlag is TRUE for all Patient Order Tests for Standard and ISE Tests, and FALSE for Calculated and OffSystem ones
        ''' </summary>
        ''' <param name="pCreateOpenWS">When TRUE, OpenOTFlag is set to TRUE for the Patient Order Test</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pLoggedUser">Logged User</param>
        ''' <param name="pWSPatientRow">Row of typed DataSet WorkSessionResultDS (subtable Patient) containing data of the Patient Order Test to add</param>
        ''' <param name="pWSOrderTestsDS">Typed DataSet WSOrderTestsDS in which the Patient Order Test has to be added, indicating the value of flags ToSendFlag 
        '''                               and OpenOTFlag</param>
        ''' <remarks>
        ''' Created by:  SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Private Sub PatientWSOrderTestsDSRow(ByVal pCreateOpenWS As Boolean, ByVal pWorkSessionID As String, ByVal pLoggedUser As String, _
                                             ByVal pWSPatientRow As WorkSessionResultDS.PatientsRow, ByRef pWSOrderTestsDS As WSOrderTestsDS)
            Try
                Dim myWSOrderTestDR As WSOrderTestsDS.twksWSOrderTestsRow

                myWSOrderTestDR = pWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                myWSOrderTestDR.OrderTestID = pWSPatientRow.OrderTestID

                'When the WS is created Empty, or when the Order Test has not been selected, it is marked as OPEN
                If (pCreateOpenWS) Then
                    myWSOrderTestDR.OpenOTFlag = True
                Else
                    myWSOrderTestDR.OpenOTFlag = (Not pWSPatientRow.Selected)
                End If

                'All selected Order Tests corresponding to STANDARD or ISE Tests are marked to be positioned
                myWSOrderTestDR.ToSendFlag = (pWSPatientRow.TestType = "STD" Or pWSPatientRow.TestType = "ISE")

                myWSOrderTestDR.TS_User = pLoggedUser
                myWSOrderTestDR.TS_DateTime = DateTime.Now
                pWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PatientWSOrderTestsDSRow", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Create a new Blank Order containing all the new requested Blank Order Tests, or update an existing one by addin to it all new 
        ''' requested Blank Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, Calibrators and Blanks that have to be 
        '''                           included in the Work Session</param>
        ''' <param name="pCreateOpenWS">When TRUE, it indicates all Order Tests have to be linked to the WorkSession with OpenOTFlag = TRUE</param>
        ''' <param name="pWorkSessionID">Identifier of the Work Session to which the Open Blank Order Tests have to be linked</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer in which the requested Blank Order Tests will be executed</param>
        ''' <param name="pWSOrderTestsDS">Typed DataSet WSOrderTestsDS in which all the OPEN Order Tests have to be added, indicating the value of flags  
        '''                               ToSendFlag and OpenOTFlag</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table BlankCalibrators updated with the generated 
        '''          OrderID / OrderTestID for all new Blanks</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 26/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value in table of Order Tests
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Blanks and Calibrators there are not Blank Order Tests 
        '''                              previously created, then it means that all Blank Order Tests were deleted; remove them also from the DB
        '''              SA 01/09/2010 - If there is a previous result for the Blank being processed, value of PreviousOrderTestID is saved although 
        '''                              New check has not been selected
        '''              SA 01/02/2012 - Added code to delete from DB all Blank Orders that have been removed from the active WorkSession
        '''              SA 29/08/2012 - When the result of a previous Blank is used inform also the field for the previous WorkSession Identifier
        '''              SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Private Function PrepareBlankOrders_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                                ByVal pCreateOpenWS As Boolean, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                ByRef pWSOrderTestsDS As WSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there are Blanks with Status OPEN in the table of Blanks and Calibrators
                        Dim lstWSBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                        lstWSBlanksDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                        Where a.SampleClass = "BLANK" _
                                      AndAlso a.OTStatus = "OPEN" _
                                       Select a).ToList()

                        Dim myOrdersDelegate As New OrdersDelegate()

                        If (lstWSBlanksDS.Count = 0) Then
                            'All Open Blank Order Tests have been deleted, remove them also from the DB 
                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "BLANK")
                        Else
                            'Get the logged User
                            'Dim currentSession As New GlobalBase
                            Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                            'Go through BlankCalibrators Table to process Blanks requested previously (fields OrderID and OrderTestID are informed)
                            'Go through BlankCalibrators Table to process Blanks requested previously (having been sent or not to the Analyzer; fields 
                            'OrderID and OrderTestID are informed) - For positioned Blank Order Tests, only value of field CreationOrder could be changed
                            lstWSBlanksDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                            Where a.SampleClass = "BLANK" _
                                      AndAlso Not a.IsOrderIDNull _
                                      AndAlso Not a.IsOrderTestIDNull _
                                         Order By a.OrderID _
                                           Select a).ToList()

                            Dim myOrderTestDS As New OrderTestsDS
                            Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow

                            If (lstWSBlanksDS.Count = 0) Then
                                'If there is an active WorkSession but in the grid of Blanks and Calibrators there are not 
                                'Blank Order Tests previously created, then it means that all Blank Order Tests were deleted;
                                'remove them also from the DB
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "BLANK")
                            Else
                                Dim listOfOrderID As String = String.Empty
                                Dim currentOrderID As String = String.Empty

                                For Each blankOrderTest In lstWSBlanksDS
                                    If (blankOrderTest.OrderID <> currentOrderID) Then
                                        'Add value of the next OrderID to process to the list of OrderIDs that remain in the WorkSession
                                        If (listOfOrderID <> "") Then listOfOrderID &= ", "
                                        listOfOrderID &= "'" & blankOrderTest.OrderID & "'"

                                        currentOrderID = blankOrderTest.OrderID
                                    End If

                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.OrderID = blankOrderTest.OrderID
                                    myOrderTestDR.OrderTestID = blankOrderTest.OrderTestID
                                    myOrderTestDR.TestType = blankOrderTest.TestType
                                    myOrderTestDR.TestID = blankOrderTest.TestID
                                    myOrderTestDR.SampleType = blankOrderTest.SampleType
                                    myOrderTestDR.TubeType = blankOrderTest.TubeType
                                    myOrderTestDR.OrderTestStatus = blankOrderTest.OTStatus
                                    myOrderTestDR.ReplicatesNumber = blankOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = blankOrderTest.CreationOrder

                                    'If there is a previous result for the Blank, the OrderTestID of the previous result is 
                                    'saved (value of New check is not relevant)
                                    If (Not blankOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = blankOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)

                                    'Prepare the corresponding WSOrderTestsDS row --> The OrderTest is added only if its Status = OPEN
                                    If (blankOrderTest.OTStatus = "OPEN") Then
                                        BlkCalibWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, blankOrderTest, pWSOrderTestsDS)
                                    End If
                                Next

                                'If there are Blank Order Tests to update...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    myGlobalDataTO = myOrdersDelegate.UpdateOrders_NEW(dbConnection, pWorkSessionID, myOrderTestDS)
                                End If

                                'All Blank Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                If (Not myGlobalDataTO.HasError) Then
                                    If (pWorkSessionID <> String.Empty AndAlso listOfOrderID <> String.Empty) Then
                                        myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "BLANK")
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Go through BlankCalibrators Table to process new requested Blanks
                                'If field OrderTestID is NULL it means it is a new Blank Order Tests that have to be included in a new Order with SampleClass BLANK 
                                'or, if there is already an Order with SampleClass BLANK in the WS, the new Blank Order Tests have to be added to it
                                lstWSBlanksDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                                Where a.SampleClass = "BLANK" _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.IsOrderTestIDNull _
                                               Select a).ToList()

                                myOrderTestDS.Clear()
                                For Each blankOrderTest In lstWSBlanksDS
                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.TestType = blankOrderTest.TestType
                                    myOrderTestDR.TestID = blankOrderTest.TestID
                                    myOrderTestDR.SampleType = blankOrderTest.SampleType
                                    myOrderTestDR.TubeType = blankOrderTest.TubeType
                                    myOrderTestDR.OrderTestStatus = blankOrderTest.OTStatus
                                    myOrderTestDR.ReplicatesNumber = blankOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = blankOrderTest.CreationOrder

                                    'If there is a previous result for the Blank and the New check is not selected, then
                                    'the OrderTestID of the previous result is saved
                                    If (Not blankOrderTest.NewCheck And Not blankOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = blankOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Blank Order Tests to create, all of them are included in a new Order with SampleClass=BLANK...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    'A new BLANK Order has to be created: Generate the next OrderID
                                    myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)

                                    If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim orderID As String = CType(myGlobalDataTO.SetDatos, String)

                                        'Prepare the DataSet containing the Order data
                                        Dim myOrdersDS As New OrdersDS
                                        Dim myOrderDR As OrdersDS.twksOrdersRow

                                        myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                        myOrderDR.OrderID = orderID
                                        myOrderDR.SampleClass = "BLANK"
                                        myOrderDR.StatFlag = False
                                        myOrderDR.OrderDateTime = DateTime.Now
                                        myOrderDR.OrderStatus = "OPEN"
                                        myOrderDR.TS_User = loggedUser
                                        myOrderDR.TS_DateTime = DateTime.Now
                                        myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                        'Create the new Blank Order with all the new Order Tests
                                        myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                        If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                            Dim testType As String = String.Empty
                                            Dim testID As Integer = -1
                                            Dim sampleType As String = String.Empty


                                            For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                'Search the correspondent row in table BlankCalibrators to update fields OrderID and OrderTestID 
                                                testType = orderTestRow.TestType
                                                testID = orderTestRow.TestID
                                                sampleType = orderTestRow.SampleType

                                                lstWSBlanksDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                                                Where a.SampleClass = "BLANK" _
                                                              AndAlso a.OTStatus = "OPEN" _
                                                              AndAlso a.TestType = testType _
                                                              AndAlso a.TestID = testID _
                                                              AndAlso a.SampleType = sampleType _
                                                               Select a).ToList()

                                                For Each blankOrderTest In lstWSBlanksDS
                                                    'Update value of fields OrderID and OrderTestID
                                                    blankOrderTest.OrderID = orderTestRow.OrderID
                                                    blankOrderTest.OrderTestID = orderTestRow.OrderTestID

                                                    'Prepare the corresponding WSOrderTestsDS row
                                                    BlkCalibWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, blankOrderTest, pWSOrderTestsDS)
                                                Next
                                            Next

                                            'Confirm changes in the DataSet
                                            pWSResultDS.BlankCalibrators.AcceptChanges()
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        lstWSBlanksDS = Nothing

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Return the same entry DataSet updated...
                            myGlobalDataTO.SetDatos = pWSResultDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareBlankOrders_NEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a new Calibrator Order containing all the new requested Calibrator Order Tests, or update an existing one by addin to it all new 
        ''' requested Calibrator Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, Calibrators and Blanks that have to be 
        '''                           included in the Work Session</param>
        ''' <param name="pCreateOpenWS">When TRUE, it indicates all Order Tests have to be linked to the WorkSession with OpenOTFlag = TRUE</param>
        ''' <param name="pWorkSessionID">Identifier of the Work Session to which the Open Calibrator Order Tests have to be linked</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer in which the requested Calibrator Order Tests will be executed</param>
        ''' <param name="pWSOrderTestsDS">Typed DataSet WSOrderTestsDS in which all the OPEN Order Tests have to be added, indicating the value of flags  
        '''                               ToSendFlag and OpenOTFlag</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table BlankCalibrators updated with the generated 
        '''          OrderID / OrderTestID for all new Calibrators</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 26/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value in table of Order Tests
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Blanks and Calibrators there are not Calibrator Order Tests 
        '''                              previously created, then it means that all Calibrator Order Tests were deleted; remove them also from the DB
        '''              SA 01/09/2010 - If there are previous results for the Calibrator being processed, value of PreviousOrderTestID is saved although 
        '''                              the New check has not been selected
        '''              SA 01/02/2012 - Added code to delete from DB all Calibrator Orders that have been removed from the active WorkSession
        '''              SA 12/04/2012 - Besides OPEN Calibrators, those used as Alternative for other SampleTypes (fields SampleType and RequestedSampleTypes 
        '''                              are different) have to be also obtained and processed to manage the special case of adding to an InProcess WS some 
        '''                              requests for the same Test with a different SampleType using the same Calibrator
        '''              SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Private Function PrepareCalibratorOrders_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                                     ByVal pCreateOpenWS As Boolean, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                     ByRef pWSOrderTestsDS As WSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there are Calibrators with Status OPEN in the table of Blanks and Calibrators OR 
                        'verify if there are Calibrators used as Alternative for the same Test but other Sample Types (whatever Status)
                        Dim lstWSCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                        lstWSCalibratorsDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                             Where a.SampleClass = "CALIB" _
                                           AndAlso (a.OTStatus = "OPEN" OrElse a.SampleType <> a.RequestedSampleTypes) _
                                            Select a).ToList()

                        Dim myOrdersDelegate As New OrdersDelegate()
                        If (lstWSCalibratorsDS.Count = 0) Then
                            'All Calibrators Order Tests have been deleted, remove them also from the DB 
                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CALIB")
                        Else
                            'Get the logged User
                            'Dim currentSession As New GlobalBase
                            Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                            'Go through BlankCalibrators Table to process Calibrators requested previously (having been sent or not to 
                            'the Analyzer; fields OrderID and OrderTestID are informed) - For positioned Calibrator Order Tests, only value
                            'of field CreationOrder could be changed when more Tests using the same Calibrator are added
                            lstWSCalibratorsDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                                 Where a.SampleClass = "CALIB" _
                                           AndAlso Not a.IsOrderIDNull _
                                           AndAlso Not a.IsOrderTestIDNull _
                                              Order By a.OrderID _
                                                Select a).ToList()

                            Dim myOrderTestDS As New OrderTestsDS
                            Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow
                            Dim myWSOrderTestDR As WSOrderTestsDS.twksWSOrderTestsRow

                            If (lstWSCalibratorsDS.Count = 0) Then
                                'If there is an active WorkSession but in the grid of Blanks&Calibrators there are not Calibrator 
                                'Order Tests previously created, then it means that all Calibrator Order Tests were deleted; 
                                'remove them also from the DB
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CALIB")
                            Else
                                Dim orderTestID As Integer = -1
                                Dim listOfOrderID As String = ""
                                Dim currentOrderID As String = ""

                                Dim additionalSampleTypes() As String
                                Dim myOrderTestsDelegate As New OrderTestsDelegate()

                                For Each calibratorOrderTest In lstWSCalibratorsDS
                                    If (calibratorOrderTest.OrderID <> currentOrderID) Then
                                        'Add value of the next OrderID to process to the list of OrderIDs that remain in the WorkSession
                                        If (listOfOrderID <> "") Then listOfOrderID &= ", "
                                        listOfOrderID &= "'" & calibratorOrderTest.OrderID & "'"

                                        currentOrderID = calibratorOrderTest.OrderID
                                    End If

                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.OrderID = calibratorOrderTest.OrderID
                                    myOrderTestDR.OrderTestID = calibratorOrderTest.OrderTestID
                                    myOrderTestDR.TestType = calibratorOrderTest.TestType
                                    myOrderTestDR.TestID = calibratorOrderTest.TestID
                                    myOrderTestDR.SampleType = calibratorOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                    myOrderTestDR.TubeType = calibratorOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = calibratorOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = calibratorOrderTest.CreationOrder

                                    'If there are previous results for the Calibrator, the OrderTestID of the previous results is 
                                    'saved (value of New check is not relevant)
                                    If (Not calibratorOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = calibratorOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)

                                    'Prepare the corresponding WSOrderTestsDS row --> The OrderTest is added only if its Status = OPEN
                                    If (calibratorOrderTest.OTStatus = "OPEN") Then
                                        BlkCalibWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, calibratorOrderTest, pWSOrderTestsDS)
                                    End If

                                    'Verify if the Calibrator corresponds to an Alternative used by another Sample Types
                                    If (calibratorOrderTest.SampleType.Trim <> calibratorOrderTest.RequestedSampleTypes.Trim) Then
                                        additionalSampleTypes = Split(calibratorOrderTest.RequestedSampleTypes.Trim)

                                        For Each reqSampleType As String In additionalSampleTypes
                                            If (reqSampleType.Trim <> String.Empty) Then
                                                'Verify if the Order Test for the requested Sample Type exists
                                                myGlobalDataTO = myOrderTestsDelegate.GetOrderTestByTestAndSampleType(dbConnection, calibratorOrderTest.OrderID, _
                                                                                                                      calibratorOrderTest.TestType, calibratorOrderTest.TestID, _
                                                                                                                      reqSampleType)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                orderTestID = -1
                                                If (Not myGlobalDataTO.SetDatos Is Nothing) Then orderTestID = CType(myGlobalDataTO.SetDatos, Integer)

                                                'A new row has to be added for each requested Sample Type
                                                myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                                myOrderTestDR.OrderID = calibratorOrderTest.OrderID
                                                If (orderTestID <> -1) Then myOrderTestDR.OrderTestID = orderTestID
                                                myOrderTestDR.TestType = calibratorOrderTest.TestType
                                                myOrderTestDR.TestID = calibratorOrderTest.TestID
                                                myOrderTestDR.SampleType = reqSampleType
                                                If (orderTestID <> -1) Then
                                                    myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                                Else
                                                    myOrderTestDR.OrderTestStatus = "OPEN"
                                                End If
                                                myOrderTestDR.TubeType = calibratorOrderTest.TubeType
                                                myOrderTestDR.ReplicatesNumber = calibratorOrderTest.NumReplicates
                                                myOrderTestDR.AlternativeOrderTestID = calibratorOrderTest.OrderTestID
                                                myOrderTestDR.AnalyzerID = pAnalyzerID
                                                myOrderTestDR.TS_User = loggedUser
                                                myOrderTestDR.TS_DateTime = DateTime.Now
                                                myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                            End If
                                        Next
                                    End If
                                Next

                                'If there are Calibrators Order Tests to update...
                                If (Not myGlobalDataTO.HasError) Then
                                    If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                        myGlobalDataTO = myOrdersDelegate.UpdateOrders_NEW(dbConnection, pWorkSessionID, myOrderTestDS)

                                        If (Not myGlobalDataTO.HasError) Then
                                            'Get all OrderTests using an alternative Calibrator
                                            Dim lstAlternativeCalibsDS As List(Of OrderTestsDS.twksOrderTestsRow) = (From a As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests _
                                                                                                                Where Not a.IsAlternativeOrderTestIDNull _
                                                                                                                 Order By a.OrderID, a.OrderTestID _
                                                                                                                   Select a).ToList()

                                            'Prepare the corresponding WSOrderTestsDS rows
                                            For Each alternativeOT As OrderTestsDS.twksOrderTestsRow In lstAlternativeCalibsDS
                                                myWSOrderTestDR = pWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                                                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                                                myWSOrderTestDR.OrderTestID = alternativeOT.OrderTestID
                                                myWSOrderTestDR.ToSendFlag = False
                                                myWSOrderTestDR.OpenOTFlag = False
                                                myWSOrderTestDR.TS_User = loggedUser
                                                myWSOrderTestDR.TS_DateTime = DateTime.Now
                                                pWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
                                            Next
                                            lstAlternativeCalibsDS = Nothing
                                        End If
                                    End If
                                End If

                                'All Calibrator Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                If (Not myGlobalDataTO.HasError) Then
                                    If (pWorkSessionID <> String.Empty AndAlso listOfOrderID <> String.Empty) Then
                                        myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "CALIB")
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Go through BlankCalibrators Table to process new requested Calibrators. If field OrderTestID 
                                'is NULL it means it is a new Blank Order Tests that have to be included in a new Order with SampleClass CALIB
                                lstWSCalibratorsDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                                     Where a.SampleClass = "CALIB" _
                                                   AndAlso a.OTStatus = "OPEN" _
                                                   AndAlso a.IsOrderTestIDNull _
                                                    Select a).ToList()

                                myOrderTestDS.Clear()
                                For Each calibratorOrderTest In lstWSCalibratorsDS
                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.TestType = calibratorOrderTest.TestType
                                    myOrderTestDR.TestID = calibratorOrderTest.TestID
                                    myOrderTestDR.SampleType = calibratorOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                    myOrderTestDR.TubeType = calibratorOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = calibratorOrderTest.NumReplicates
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = calibratorOrderTest.CreationOrder

                                    'If there are previous result for the Calibrator and the New check is not selected, then
                                    'the OrderTestID of the previous results is saved
                                    If (Not calibratorOrderTest.NewCheck And Not calibratorOrderTest.IsPreviousOrderTestIDNull) Then
                                        myOrderTestDR.PreviousOrderTestID = calibratorOrderTest.PreviousOrderTestID
                                    End If

                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now
                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Calibrator Order Tests to create, all of them are included in a new Order with SampleClass=CALIB...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    'Generate the next OrderID
                                    myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim orderID As String = CType(myGlobalDataTO.SetDatos, String)

                                        'Prepare the DataSet containing the Order data
                                        Dim myOrdersDS As New OrdersDS
                                        Dim myOrderDR As OrdersDS.twksOrdersRow

                                        myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                        myOrderDR.OrderID = orderID
                                        myOrderDR.SampleClass = "CALIB"
                                        myOrderDR.StatFlag = False
                                        myOrderDR.OrderDateTime = DateTime.Now
                                        myOrderDR.OrderStatus = "OPEN"
                                        myOrderDR.TS_User = loggedUser
                                        myOrderDR.TS_DateTime = DateTime.Now
                                        myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                        'Create the new Calibrator Order with all the new Order Tests
                                        myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                            Dim testType As String = String.Empty
                                            Dim testID As Integer = -1
                                            Dim sampleType As String = String.Empty
                                            Dim additionalSampleTypes() As String

                                            Dim myOrderTestAlternativDS As New OrderTestsDS
                                            Dim myOrderTestsDelegate As New OrderTestsDelegate()

                                            For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                'Search the correspondent row in table BlankCalibrators to update fields OrderID and OrderTestID 
                                                testType = orderTestRow.TestType
                                                testID = orderTestRow.TestID
                                                sampleType = orderTestRow.SampleType

                                                lstWSCalibratorsDS = (From a As WorkSessionResultDS.BlankCalibratorsRow In pWSResultDS.BlankCalibrators _
                                                                     Where a.SampleClass = "CALIB" _
                                                                   AndAlso a.OTStatus = "OPEN" _
                                                                   AndAlso a.TestType = testType _
                                                                   AndAlso a.TestID = testID _
                                                                   AndAlso a.SampleType = sampleType _
                                                                    Select a).ToList()

                                                For Each calibratorOrderTest In lstWSCalibratorsDS
                                                    calibratorOrderTest.OrderID = orderTestRow.OrderID
                                                    calibratorOrderTest.OrderTestID = orderTestRow.OrderTestID

                                                    'Prepare the corresponding WSOrderTestsDS row
                                                    BlkCalibWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, calibratorOrderTest, pWSOrderTestsDS)

                                                    'Insert an Order Test for each requested Sample Type when they use and Alternative Calibrator
                                                    If (calibratorOrderTest.SampleType.Trim <> calibratorOrderTest.RequestedSampleTypes.Trim) Then
                                                        additionalSampleTypes = Split(calibratorOrderTest.RequestedSampleTypes.Trim)

                                                        For Each reqSampleType As String In additionalSampleTypes
                                                            'A new row has to be added for each requested Sample Type
                                                            myOrderTestDR = myOrderTestAlternativDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                                            myOrderTestDR.OrderID = calibratorOrderTest.OrderID
                                                            myOrderTestDR.TestType = calibratorOrderTest.TestType
                                                            myOrderTestDR.TestID = calibratorOrderTest.TestID
                                                            myOrderTestDR.SampleType = reqSampleType
                                                            myOrderTestDR.OrderTestStatus = calibratorOrderTest.OTStatus
                                                            myOrderTestDR.AlternativeOrderTestID = calibratorOrderTest.OrderTestID
                                                            myOrderTestDR.AnalyzerID = pAnalyzerID
                                                            myOrderTestDR.TS_User = loggedUser
                                                            myOrderTestDR.TS_DateTime = DateTime.Now
                                                            myOrderTestAlternativDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                                        Next

                                                        If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                                            'Add the Order Test for the Requested Sample Type
                                                            myGlobalDataTO = myOrderTestsDelegate.Create(dbConnection, myOrderTestAlternativDS)
                                                            If (myGlobalDataTO.HasError) Then Exit For

                                                            'Prepare the WSOrderTestDS rows
                                                            For Each alternativeOT As OrderTestsDS.twksOrderTestsRow In myOrderTestAlternativDS.twksOrderTests
                                                                myWSOrderTestDR = pWSOrderTestsDS.twksWSOrderTests.NewtwksWSOrderTestsRow() 'Create a OrderTestsRow to load values
                                                                myWSOrderTestDR.WorkSessionID = pWorkSessionID
                                                                myWSOrderTestDR.OrderTestID = alternativeOT.OrderTestID
                                                                myWSOrderTestDR.ToSendFlag = False
                                                                myWSOrderTestDR.OpenOTFlag = False
                                                                myWSOrderTestDR.TS_User = loggedUser
                                                                myWSOrderTestDR.TS_DateTime = DateTime.Now
                                                                pWSOrderTestsDS.twksWSOrderTests.AddtwksWSOrderTestsRow(myWSOrderTestDR)
                                                            Next
                                                            myOrderTestAlternativDS.Clear()
                                                        End If
                                                    End If
                                                Next
                                            Next
                                            pWSResultDS.BlankCalibrators.AcceptChanges()
                                        End If
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'When the Database Connection was opened locally, then the Commit is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                                    'Return the same entry DataSet updated...
                                    myGlobalDataTO.SetDatos = pWSResultDS
                                Else
                                    'When the Database Connection was opened locally, then the Rollback is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If
                            End If
                        End If
                        lstWSCalibratorsDS = Nothing
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareCalibratorOrders_NEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a new Control Order containing all the new requested Control Order Tests, or update an existing one by addin to it all new 
        ''' requested Control Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, Calibrators and Blanks that have to be 
        '''                           included in the Work Session</param>
        ''' <param name="pCreateOpenWS">When TRUE, it indicates all Order Tests have to be linked to the WorkSession with OpenOTFlag = TRUE</param>
        ''' <param name="pWorkSessionID">Identifier of the Work Session to which the Open Control Order Tests have to be linked</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer in which the requested Control Order Tests will be executed</param>
        ''' <param name="pWSOrderTestsDS">Typed DataSet WSOrderTestsDS in which all the OPEN Order Tests have to be added, indicating the value of flags  
        '''                               ToSendFlag and OpenOTFlag</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table Controls updated with the generated 
        '''          OrderID / OrderTestID for all new Controls</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 16/03/2010 - Each Control required for Test/SampleType is managed as an individual OrderTestID (field ControlNumber was added to 
        '''                              Order Tests table and DS to known which OrderTestID corresponds to each Control Number)
        '''              SA 26/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value in table of Order Tests
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Controls there are not Order Tests previously created, then it 
        '''                              means that all Control Order Tests were deleted; remove them also from the DB
        '''              DL 15/04/2011 - Replace use of ControlNum field by use of ControlID field
        '''              SA 01/02/2012 - Added code to delete from DB all Control Orders that have been removed from the active WorkSession
        '''              SA 26/03/2013 - Added changes needed for new LIS implementation: for new Control Order Tests, when filling the OrderTestsDS, include 
        '''                              also LIS fields when they are informed. For Control Order Tests to update, when filling the OrderTestsDS, include 
        '''                              only LISRequest field when it is informed 
        '''              SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Private Function PrepareControlOrders_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                                  ByVal pCreateOpenWS As Boolean, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                  ByRef pWSOrderTestsDS As WSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there are Controls with Status OPEN in the table of Controls
                        Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)
                        lstWSControlsDS = (From a As WorkSessionResultDS.ControlsRow In pWSResultDS.Controls _
                                          Where a.SampleClass = "CTRL" _
                                        AndAlso a.OTStatus = "OPEN" _
                                         Select a).ToList()

                        Dim myOrdersDelegate As New OrdersDelegate()
                        If (lstWSControlsDS.Count = 0) Then
                            'All Control Order Tests have been deleted, remove them also from the DB 
                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CTRL")
                        Else
                            'Get the logged User
                            'Dim currentSession As New GlobalBase
                            Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                            'Validate if there are CtrlsSendingGroup
                            Dim myCtrlSendingGroup As Integer = 0
                            Dim myWSOrderTesDelegate As New WSOrderTestsDelegate

                            myGlobalDataTO = myWSOrderTesDelegate.GetMaxCtrlsSendingGroup(dbConnection, pWorkSessionID)
                            If (Not myGlobalDataTO.HasError) Then myCtrlSendingGroup = CInt(myGlobalDataTO.SetDatos) + 1

                            'Go through Controls Table to process Controls requested previously (having been sent or not to the Analyzer; 
                            'fields OrderID and OrderTestID are informed) - For positioned Control Order Tests, only value of field 
                            'CreationOrder could be changed, when more Tests using the same Control are added
                            lstWSControlsDS = (From a As WorkSessionResultDS.ControlsRow In pWSResultDS.Controls _
                                              Where a.SampleClass = "CTRL" _
                                        AndAlso Not a.IsOrderIDNull _
                                        AndAlso Not a.IsOrderTestIDNull _
                                           Order By a.OrderID, a.OrderTestID _
                                             Select a).ToList()

                            Dim myOrderTestDS As New OrderTestsDS
                            Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow

                            If (lstWSControlsDS.Count = 0) Then
                                'If there is an active WorkSession but in the grid of Controls there are not Order Tests previously
                                'created, then it means that all Control Order Tests were deleted; remove them also from the DB
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "CTRL")
                            Else
                                Dim listOfOrderID As String = String.Empty
                                Dim currentOrderID As String = String.Empty
                                For Each controlOrderTest In lstWSControlsDS
                                    If (controlOrderTest.OrderID <> currentOrderID) Then
                                        'Add value of the next OrderID to process to the list of OrderIDs that remain in the WorkSession
                                        If (listOfOrderID <> String.Empty) Then listOfOrderID &= ", "
                                        listOfOrderID &= "'" & controlOrderTest.OrderID & "'"

                                        currentOrderID = controlOrderTest.OrderID
                                    End If

                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.OrderID = controlOrderTest.OrderID
                                    myOrderTestDR.OrderTestID = controlOrderTest.OrderTestID
                                    myOrderTestDR.TestType = controlOrderTest.TestType
                                    myOrderTestDR.TestID = controlOrderTest.TestID
                                    myOrderTestDR.SampleType = controlOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = controlOrderTest.OTStatus
                                    myOrderTestDR.TubeType = controlOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = controlOrderTest.NumReplicates
                                    myOrderTestDR.ControlID = controlOrderTest.ControlID
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.CreationOrder = controlOrderTest.CreationOrder
                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now

                                    'Set value of field LISRequest when it is informed
                                    If (Not controlOrderTest.IsLISRequestNull) Then myOrderTestDR.LISRequest = controlOrderTest.LISRequest
                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)

                                    'Prepare the corresponding WSOrderTestsDS row --> The OrderTest is added only if its Status = OPEN
                                    If (controlOrderTest.OTStatus = "OPEN") Then CtrlWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, myCtrlSendingGroup, controlOrderTest, pWSOrderTestsDS)
                                Next

                                'If there are Control Order Tests to update...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    myGlobalDataTO = myOrdersDelegate.UpdateOrders_NEW(dbConnection, pWorkSessionID, myOrderTestDS)
                                End If

                                'All Control Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                If (Not myGlobalDataTO.HasError) Then
                                    If (pWorkSessionID <> String.Empty AndAlso listOfOrderID <> String.Empty) Then
                                        myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "CTRL")
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Go through Controls Table to process new requested Controls. If field OrderTestID 
                                'is NULL it means it is a new Control Order Test that has to be included in a new Order with SampleClass CTRL
                                lstWSControlsDS = (From a As WorkSessionResultDS.ControlsRow In pWSResultDS.Controls _
                                                  Where a.SampleClass = "CTRL" _
                                                AndAlso a.OTStatus = "OPEN" _
                                                AndAlso a.IsOrderTestIDNull _
                                               Order By a.TestType, a.TestID, a.SampleType _
                                                 Select a).ToList()

                                myOrderTestDS.Clear()
                                For Each controlOrderTest In lstWSControlsDS
                                    myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow() 'Create a OrderTestsRow to load values
                                    myOrderTestDR.TestType = controlOrderTest.TestType
                                    myOrderTestDR.TestID = controlOrderTest.TestID
                                    myOrderTestDR.SampleType = controlOrderTest.SampleType
                                    myOrderTestDR.OrderTestStatus = controlOrderTest.OTStatus
                                    myOrderTestDR.TubeType = controlOrderTest.TubeType
                                    myOrderTestDR.ReplicatesNumber = controlOrderTest.NumReplicates
                                    myOrderTestDR.ControlID = controlOrderTest.ControlID
                                    myOrderTestDR.AnalyzerID = pAnalyzerID
                                    myOrderTestDR.SampleClass = "CTRL"
                                    myOrderTestDR.CreationOrder = controlOrderTest.CreationOrder
                                    myOrderTestDR.TS_User = loggedUser
                                    myOrderTestDR.TS_DateTime = DateTime.Now

                                    'SA 26/03/2013 - Set value of all LIS fields when they are informed
                                    If (Not controlOrderTest.IsLISRequestNull) Then myOrderTestDR.LISRequest = controlOrderTest.LISRequest
                                    If (Not controlOrderTest.IsAwosIDNull) Then myOrderTestDR.AwosID = controlOrderTest.AwosID
                                    If (Not controlOrderTest.IsSpecimenIDNull) Then myOrderTestDR.SpecimenID = controlOrderTest.SpecimenID
                                    If (Not controlOrderTest.IsESPatientIDNull) Then myOrderTestDR.ESPatientID = controlOrderTest.ESPatientID
                                    If (Not controlOrderTest.IsLISPatientIDNull) Then myOrderTestDR.LISPatientID = controlOrderTest.LISPatientID
                                    If (Not controlOrderTest.IsESOrderIDNull) Then myOrderTestDR.ESOrderID = controlOrderTest.ESOrderID
                                    If (Not controlOrderTest.IsLISOrderIDNull) Then myOrderTestDR.LISOrderID = controlOrderTest.LISOrderID

                                    myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                Next

                                'If there are Control Order Tests to create, all of them are included in a new Order with SampleClass=CTRL...
                                If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                    Dim myOrdersDS As New OrdersDS

                                    'Generate the next OrderID
                                    myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)
                                    If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim orderID As String = CType(myGlobalDataTO.SetDatos, String)

                                        'Prepare the DataSet containing the Order data
                                        Dim myOrderDR As OrdersDS.twksOrdersRow

                                        myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                        myOrderDR.OrderID = orderID
                                        myOrderDR.SampleClass = "CTRL"
                                        myOrderDR.StatFlag = False
                                        myOrderDR.OrderDateTime = DateTime.Now
                                        myOrderDR.OrderStatus = "OPEN"
                                        myOrderDR.TS_User = loggedUser
                                        myOrderDR.TS_DateTime = DateTime.Now
                                        myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                        'Create the new Control Order with all the new Order Tests
                                        myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                            Dim testType As String = String.Empty
                                            Dim sampleType As String = String.Empty
                                            Dim testID As Integer = -1
                                            Dim controlID As Integer = -1
                                            For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                'Search the correspondent row in table Controls to update fields OrderID and OrderTestID 
                                                testType = orderTestRow.TestType
                                                testID = orderTestRow.TestID
                                                sampleType = orderTestRow.SampleType
                                                controlID = orderTestRow.ControlID

                                                lstWSControlsDS = (From a As WorkSessionResultDS.ControlsRow In pWSResultDS.Controls _
                                                                  Where a.SampleClass = "CTRL" _
                                                                AndAlso a.OTStatus = "OPEN" _
                                                                AndAlso a.TestType = testType _
                                                                AndAlso a.TestID = testID _
                                                                AndAlso a.SampleType = sampleType _
                                                                AndAlso a.ControlID = controlID _
                                                                 Select a).ToList()

                                                For Each controlOrderTest In lstWSControlsDS
                                                    controlOrderTest.OrderID = orderTestRow.OrderID
                                                    controlOrderTest.OrderTestID = orderTestRow.OrderTestID

                                                    'Prepare the corresponding WSOrderTestsDS row
                                                    CtrlWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, myCtrlSendingGroup, controlOrderTest, pWSOrderTestsDS)

                                                    'Confirm changes in the DataSet
                                                    pWSResultDS.Controls.AcceptChanges()
                                                Next
                                            Next
                                        End If
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'When the Database Connection was opened locally, then the Commit is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                                    'Return the same entry DataSet updated...
                                    myGlobalDataTO.SetDatos = pWSResultDS
                                Else
                                    'When the Database Connection was opened locally, then the Rollback is executed
                                    If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If
                            End If
                        End If
                        lstWSControlsDS = Nothing
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareControlOrders_NEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a group of Patient Orders containing all the new requested Patient Order Tests, or update existing Patient Orders if new Order Tests 
        ''' have been added to them
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, Calibrators and Blanks that have to be 
        '''                           included in the Work Session</param>
        ''' <param name="pCreateOpenWS">When TRUE, it indicates all Order Tests have to be linked to the WorkSession with OpenOTFlag = TRUE</param>
        ''' <param name="pWorkSessionID">Identifier of the Work Session to which the Open Patient Order Tests have to be linked</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer in which the requested Patient Order Tests will be executed</param>
        ''' <param name="pWSOrderTestsDS">Typed DataSet WSOrderTestsDS in which all the OPEN Order Tests have to be added, indicating the value of flags  
        '''                               ToSendFlag and OpenOTFlag</param>
        ''' <returns>GlobalDataTO containing the same entry typed DataSet WorkSessionResultDS with table Patients updated with the generated 
        '''          OrderID / OrderTestID for all new Patient Samples</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 29/03/2010 - Added also field CreationOrder in the typed DataSet OrderTestsDS to save the value in table of Order Tests
        '''              SA 27/04/2010 - Changed LINQ to get Patient Order Tests previously created (field OrderID is informed) to obtain also the 
        '''                              ones already positioned, due to for them it is needed to update field CreationOrder
        '''              SA 18/05/2010 - Added code needed to manage Patient Order Tests with Calculated Tests in the WorkSession
        '''              SA 08/06/2010 - If there is an active WorkSession but in the grid of Patients there are not Order Tests previously created, 
        '''                              then it means that all Patient Order Tests were deleted; remove them also from the DB. Also, when all Order Tests 
        '''                              of an Order previously created have been deleted, they are also deleted from the DB by calling function 
        '''                              DeleteOrdersNotInWS in OrdersDelegate
        '''              SA 30/08/2010 - Rebuilding of relations between Order Tests of Standard and Calculated Tests should be done although there are not 
        '''                              new Order Tests to add to the WorkSession 
        '''              SA 13/10/2010 - Do not inform OrderStatus = "OPEN" for existing Orders, to avoid re-opening of Closed Orders        
        '''              SA 21/10/2010 - Inform fields TubeType and NumReplicates not only for Standard Tests, but also for ISE Tests
        '''              SA 01/02/2012 - Inform the SampleClass (PATIENT) when calling function DeleteOrdersNotInWS in OrdersDelegate 
        '''              SA 26/03/2013 - Added changes needed for new LIS implementation: when filling the DS toAddOrderTestsDS, include also LIS fields when 
        '''                              they are informed. For both cases, add and mod Order Tests, inform field LISRequest if its value is not null in 
        '''                              pWSResultsDS.Patients. For add Order Tests, inform value of field SampleClass
        '''              SA 03/05/2013 - When get from the WorkSessionResultDS DataSet the list of Patient Orders to modify (those having field OrderID
        '''                              informed), besides for the OrderID, data has to be sorted by SampleIDType. This is due to with the new LIS 
        '''                              implementation, if LIS sends new Order Tests and demographic data for a Patient initially added manually to the WS,
        '''                              the Order should be updated (informing field PatientID instead SampleID), but in current implementation the Order Test
        '''                              having SampleIDType=MAN is obtained before the one having SampleIDType=DB and the update is not executed (because the
        '''                              Order is updated with data of the first Order Test processed; from screen all Order Tests carried the same information
        '''                              and it was enough to take the first)
        '''              SA 04/06/2013 - When a SampleID is moved to field PatientID in an Order, its content has to be changed to Uppercase characters, due
        '''                              to this is the way PatientIDs are saved in tparPatients table
        '''              SA 05/02/2014 - BT #1491 ==> Exclude OPEN Order Tests requested by LIS from the process for update existing Orders, due to LIS Order Tests
        '''                                           can not be modified nor deleted. Done to improve the performance of this function when there are lot of OPEN 
        '''                                           Order Tests requested by LIS and avoid DB time out errors when other process try to access one of the tables
        '''                                           locked by this transaction.
        '''              SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Private Function PreparePatientOrders_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSResultDS As WorkSessionResultDS, _
                                                  ByVal pCreateOpenWS As Boolean, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                  ByRef pWSOrderTestsDS As WSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'For all Patient Order Tests in the WorkSession having relation with a Calculated Tests, delete 
                        'all relations defined in table twksOrderCalculatedTests
                        Dim myCalcOrderTestsDelegate As New OrderCalculatedTestsDelegate
                        myGlobalDataTO = myCalcOrderTestsDelegate.DeleteByWorkSession(dbConnection, pWorkSessionID)

                        If (Not myGlobalDataTO.HasError) Then
                            'Verify if there are Patient Order Tests with Status OPEN in the table of Patients 
                            Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                            lstWSPatientsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                              Where a.SampleClass = "PATIENT" _
                                            AndAlso a.OTStatus = "OPEN" _
                                             Select a).ToList()

                            Dim myOrdersDelegate As New OrdersDelegate
                            Dim myOrderTestsDelegate As New OrderTestsDelegate

                            If (lstWSPatientsDS.Count = 0) Then
                                'All Patient Order Tests have been deleted, remove them also from the DB 
                                myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "PATIENT")
                            Else
                                'Get the logged User
                                'Dim currentSession As New GlobalBase
                                Dim loggedUser As String = GlobalBase.GetSessionInfo.UserName

                                'Go through Patients Table to process Patient Orders requested previously (field OrderID is informed). These Orders can have
                                'existing Order Tests (field OrderTestID informed) and non existing Order Tests (they have to be added to the Order) query 
                                'BT #1491 ==> ... Existing Order Tests requested by LIS are excluded from the query due to they cannot be modified (excepting
                                '             Order Tests with LISRequest=True but AwosID not informed, due to this is the case of Order Tests manually added  
                                '             that are also included in the formula of one or more Calculated Tests requested by LIS) 
                                lstWSPatientsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                  Where a.SampleClass = "PATIENT" _
                                                AndAlso a.OrderID <> String.Empty _
                                                AndAlso (a.IsLISRequestNull _
                                                 OrElse a.LISRequest = False _
                                                 OrElse (a.LISRequest = True And a.IsOrderTestIDNull) _
                                                 OrElse (a.LISRequest = True And Not a.IsOrderTestIDNull And a.IsAwosIDNull)) _
                                               Order By a.OrderID, a.SampleIDType _
                                                 Select a).ToList()

                                If (lstWSPatientsDS.Count = 0) Then
                                    'If there is an active WorkSession but in the grid of Patients there are not Order Tests previously
                                    'created, then it means that all Patient Order Tests were deleted; remove them also from the DB
                                    myGlobalDataTO = myOrdersDelegate.DeleteOrdersBySampleClass(dbConnection, pWorkSessionID, "PATIENT")
                                Else
                                    Dim myOrdersDS As New OrdersDS
                                    Dim myOrderDR As OrdersDS.twksOrdersRow

                                    Dim toModOrderTestsDS As New OrderTestsDS
                                    Dim toAddOrderTestsDS As New OrderTestsDS
                                    Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow

                                    Dim listOfOrderID As String = String.Empty
                                    Dim currentOrderID As String = String.Empty

                                    Dim testType As String = String.Empty
                                    Dim testID As Integer = -1
                                    Dim sampleType As String = String.Empty
                                    Dim lstOrderTestsDS As List(Of WorkSessionResultDS.PatientsRow)

                                    For i As Integer = 0 To lstWSPatientsDS.Count - 1
                                        If (lstWSPatientsDS(i).OrderID <> currentOrderID) Then
                                            If (currentOrderID <> String.Empty) Then
                                                'Update values of the Current Order
                                                myGlobalDataTO = myOrdersDelegate.ModifyOrder(dbConnection, myOrdersDS)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                'If there are Patient Order Tests to update for the current Order...
                                                If (toModOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                    myGlobalDataTO = myOrdersDelegate.UpdateOrders_NEW(dbConnection, pWorkSessionID, toModOrderTestsDS)
                                                    If (myGlobalDataTO.HasError) Then Exit For
                                                End If

                                                'If there are Patient Order Tests to add to the current Order
                                                If (toAddOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                    myGlobalDataTO = myOrderTestsDelegate.Create(dbConnection, toAddOrderTestsDS)

                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        toAddOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                        'Search the correspondent row in table Patients to update field OrderTestID 
                                                        For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In toAddOrderTestsDS.twksOrderTests.Rows
                                                            testType = orderTestRow.TestType
                                                            testID = orderTestRow.TestID
                                                            sampleType = orderTestRow.SampleType

                                                            lstOrderTestsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                                              Where a.SampleClass = "PATIENT" _
                                                                            AndAlso a.OrderID = currentOrderID _
                                                                            AndAlso a.OTStatus = "OPEN" _
                                                                            AndAlso a.TestType = testType _
                                                                            AndAlso a.TestID = testID _
                                                                            AndAlso a.SampleType = sampleType _
                                                                             Select a).ToList()

                                                            If (lstOrderTestsDS.Count = 1) Then
                                                                lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID

                                                                'Prepare the corresponding WSOrderTestsDS row
                                                                PatientWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, lstOrderTestsDS.First, pWSOrderTestsDS)
                                                            End If
                                                        Next
                                                    End If
                                                End If
                                            End If

                                            'Prepare the DataSet to update Patient Order values
                                            myOrdersDS.twksOrders.Clear()

                                            myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                            myOrderDR.OrderID = lstWSPatientsDS(i).OrderID
                                            myOrderDR.SampleClass = "PATIENT"
                                            myOrderDR.StatFlag = lstWSPatientsDS(i).StatFlag

                                            '** Field PatientID will be SampleID (in Uppercase characters) when SampleIDType=DB, otherwise it will be Null
                                            '** Field SampleID will be SampleID when SampleIDType=MANUAL or AUTO, otherwise it will be Null
                                            If (lstWSPatientsDS(i).SampleIDType = "DB") Then
                                                myOrderDR.PatientID = lstWSPatientsDS(i).SampleID.ToUpperBS
                                                myOrderDR.SampleID = ""
                                            ElseIf (lstWSPatientsDS(i).SampleIDType = "MAN" Or lstWSPatientsDS(i).SampleIDType = "AUTO") Then
                                                myOrderDR.SampleID = lstWSPatientsDS(i).SampleID
                                                myOrderDR.PatientID = ""
                                            End If

                                            myOrderDR.OrderDateTime = DateTime.Now
                                            'myOrderDR.OrderStatus = "OPEN"        'SA - 13/10/2010
                                            myOrderDR.TS_User = loggedUser
                                            myOrderDR.TS_DateTime = DateTime.Now
                                            myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                            'Set value of the next OrderID to process
                                            currentOrderID = lstWSPatientsDS(i).OrderID

                                            'Add value of the next OrderID to process to the list of OrderID that remain in the WorkSession
                                            If (listOfOrderID <> String.Empty) Then listOfOrderID &= ", "
                                            listOfOrderID &= "'" & currentOrderID & "'"

                                            toModOrderTestsDS.Clear()
                                            toAddOrderTestsDS.Clear()
                                        End If

                                        If (Not lstWSPatientsDS(i).IsOrderTestIDNull) Then
                                            'Create a row in the DataSet containing the Order Tests to update; inform the OrderTestID
                                            myOrderTestDR = toModOrderTestsDS.twksOrderTests.NewtwksOrderTestsRow()
                                            myOrderTestDR.OrderTestID = lstWSPatientsDS(i).OrderTestID
                                        Else
                                            'Create a row in the DataSet containing the Order Tests to add
                                            myOrderTestDR = toAddOrderTestsDS.twksOrderTests.NewtwksOrderTestsRow()
                                        End If

                                        myOrderTestDR.OrderID = lstWSPatientsDS(i).OrderID
                                        myOrderTestDR.SampleClass = "PATIENT"
                                        myOrderTestDR.TestType = lstWSPatientsDS(i).TestType
                                        myOrderTestDR.TestID = lstWSPatientsDS(i).TestID
                                        myOrderTestDR.SampleType = lstWSPatientsDS(i).SampleType
                                        myOrderTestDR.OrderTestStatus = lstWSPatientsDS(i).OTStatus

                                        If (lstWSPatientsDS(i).TestType = "STD" OrElse lstWSPatientsDS(i).TestType = "ISE") Then
                                            'TubeType and NumReplicates is informed only for Standard and ISE Tests 
                                            myOrderTestDR.ReplicatesNumber = lstWSPatientsDS(i).NumReplicates
                                            myOrderTestDR.TubeType = lstWSPatientsDS(i).TubeType
                                        End If

                                        If (Not lstWSPatientsDS(i).IsTestProfileIDNull AndAlso lstWSPatientsDS(i).TestProfileID <> 0) Then
                                            myOrderTestDR.TestProfileID = lstWSPatientsDS(i).TestProfileID
                                        End If

                                        myOrderTestDR.CreationOrder = lstWSPatientsDS(i).CreationOrder
                                        myOrderTestDR.AnalyzerID = pAnalyzerID
                                        myOrderTestDR.TS_User = loggedUser
                                        myOrderTestDR.TS_DateTime = DateTime.Now

                                        'SA 26/03/2013 - Inform value of LIS fields LISRequest and ExternalQC when they are informed
                                        If (Not lstWSPatientsDS(i).IsLISRequestNull) Then myOrderTestDR.LISRequest = lstWSPatientsDS(i).LISRequest
                                        If (Not lstWSPatientsDS(i).IsExternalQCNull) Then myOrderTestDR.ExternalQC = lstWSPatientsDS(i).ExternalQC

                                        If (Not lstWSPatientsDS(i).IsOrderTestIDNull) Then
                                            'Add row to the DataSet containing the Order Tests to update
                                            toModOrderTestsDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)

                                            'Prepare the corresponding WSOrderTestsDS row
                                            PatientWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, lstWSPatientsDS(i), pWSOrderTestsDS)
                                        Else
                                            'SA 26/03/2013 - Fill also the rest of LIS fields if they are informed... 
                                            If (Not lstWSPatientsDS(i).IsAwosIDNull) Then myOrderTestDR.AwosID = lstWSPatientsDS(i).AwosID
                                            If (Not lstWSPatientsDS(i).IsSpecimenIDNull) Then myOrderTestDR.SpecimenID = lstWSPatientsDS(i).SpecimenID
                                            If (Not lstWSPatientsDS(i).IsESPatientIDNull) Then myOrderTestDR.ESPatientID = lstWSPatientsDS(i).ESPatientID
                                            If (Not lstWSPatientsDS(i).IsLISPatientIDNull) Then myOrderTestDR.LISPatientID = lstWSPatientsDS(i).LISPatientID
                                            If (Not lstWSPatientsDS(i).IsESOrderIDNull) Then myOrderTestDR.ESOrderID = lstWSPatientsDS(i).ESOrderID
                                            If (Not lstWSPatientsDS(i).IsLISOrderIDNull) Then myOrderTestDR.LISOrderID = lstWSPatientsDS(i).LISOrderID

                                            'Add row to the DataSet containing Order Tests to create
                                            toAddOrderTestsDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)
                                        End If

                                        If (i = lstWSPatientsDS.Count - 1) Then
                                            'Update values of the Current Order
                                            myGlobalDataTO = myOrdersDelegate.ModifyOrder(dbConnection, myOrdersDS)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            'If there are Patient Order Tests to update for the current Order...
                                            If (toModOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                myGlobalDataTO = myOrdersDelegate.UpdateOrders_NEW(dbConnection, pWorkSessionID, toModOrderTestsDS)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            End If

                                            'If there are Patient Order Tests to add to the current Order
                                            If (toAddOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                                myGlobalDataTO = myOrderTestsDelegate.Create(dbConnection, toAddOrderTestsDS)

                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    toAddOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                    'Search the correspondent row in table Patients to update field OrderTestID 
                                                    For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In toAddOrderTestsDS.twksOrderTests.Rows
                                                        testType = orderTestRow.TestType
                                                        testID = orderTestRow.TestID
                                                        sampleType = orderTestRow.SampleType

                                                        lstOrderTestsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                                          Where a.SampleClass = "PATIENT" _
                                                                        AndAlso a.OrderID = currentOrderID _
                                                                        AndAlso a.OTStatus = "OPEN" _
                                                                        AndAlso a.TestType = testType _
                                                                        AndAlso a.TestID = testID _
                                                                        AndAlso a.SampleType = sampleType _
                                                                         Select a).ToList()

                                                        If (lstOrderTestsDS.Count = 1) Then
                                                            lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID

                                                            'Prepare the corresponding WSOrderTestsDS row
                                                            PatientWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, lstOrderTestsDS.First, pWSOrderTestsDS)
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        End If
                                    Next

                                    If (Not myGlobalDataTO.HasError) Then
                                        If (pWorkSessionID.Trim <> String.Empty AndAlso listOfOrderID <> String.Empty) Then
                                            'All Orders that are not in the list of Orders that remain in the WorkSession have to be deleted from the DB 
                                            myGlobalDataTO = myOrdersDelegate.DeleteOrdersNotInWS(dbConnection, pWorkSessionID, listOfOrderID, "PATIENT")
                                        End If
                                    End If
                                End If

                                'Special code to update field CreationOrder for all Patient Order Tests requested by LIS and also to create the needed
                                'row in the WSOrderTestsDS that will be used to add/update data in table twksWSOrderTests (to solve an aesthetic issue 
                                'derived from BT #1491)
                                If (Not myGlobalDataTO.HasError) Then
                                    lstWSPatientsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                      Where a.SampleClass = "PATIENT" _
                                                    AndAlso a.OrderID <> String.Empty _
                                                    AndAlso Not a.IsOrderTestIDNull _
                                                    AndAlso (Not a.IsLISRequestNull AndAlso a.LISRequest = True) _
                                                     Select a).ToList()

                                    If (lstWSPatientsDS.Count > 0) Then
                                        myGlobalDataTO = myOrderTestsDelegate.UpdateCreationOrderForOpenLISOTs(dbConnection, lstWSPatientsDS)
                                    End If
                                End If

                                If (Not myGlobalDataTO.HasError) Then
                                    'Go through Patients Table to process new Patient Order Tests - Get those with field OrderID not informed
                                    'RH 12/07/2011 ==> Replace SampleID by CreationOrder, because the creation order must be kept
                                    lstWSPatientsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                      Where a.SampleClass = "PATIENT" _
                                                    AndAlso a.OTStatus = "OPEN" _
                                                    AndAlso a.OrderID = String.Empty _
                                                   Order By a.SampleIDType, a.CreationOrder, a.StatFlag _
                                                     Select a).ToList()

                                    If (lstWSPatientsDS.Count > 0) Then
                                        Dim myOrdersDS As New OrdersDS
                                        Dim myOrderDR As OrdersDS.twksOrdersRow
                                        Dim myOrderTestDS As New OrderTestsDS
                                        Dim myOrderTestDR As OrderTestsDS.twksOrderTestsRow
                                        Dim orderID As String = String.Empty
                                        Dim currentSampleID As String = String.Empty
                                        Dim currentStatFlag As Integer = -1
                                        Dim testType As String = String.Empty
                                        Dim testID As Integer = -1
                                        Dim sampleType As String = String.Empty
                                        Dim lstOrderTestsDS As List(Of WorkSessionResultDS.PatientsRow)

                                        For i As Integer = 0 To lstWSPatientsDS.Count - 1
                                            If ((lstWSPatientsDS(i).SampleID <> currentSampleID) OrElse CType(lstWSPatientsDS(i).StatFlag, Integer) <> currentStatFlag) Then
                                                If (currentSampleID <> String.Empty) Then
                                                    'It is not the first Patient Order, the previous is created 
                                                    myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)

                                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                        myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                        'Search the correspondent row in table Patients to update field OrderTestID 
                                                        For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                            testType = orderTestRow.TestType
                                                            testID = orderTestRow.TestID
                                                            sampleType = orderTestRow.SampleType

                                                            lstOrderTestsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                                                              Where a.SampleClass = "PATIENT" _
                                                                            AndAlso a.OrderID = orderID _
                                                                            AndAlso a.OTStatus = "OPEN" _
                                                                            AndAlso a.TestType = testType _
                                                                            AndAlso a.TestID = testID _
                                                                            AndAlso a.SampleType = sampleType _
                                                                             Select a).ToList()

                                                            If (lstOrderTestsDS.Count = 1) Then
                                                                lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID

                                                                'Prepare the corresponding WSOrderTestsDS row
                                                                PatientWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, lstOrderTestsDS.First, pWSOrderTestsDS)
                                                            End If
                                                        Next
                                                    Else
                                                        'Error creating the new Patient Order
                                                        Exit For
                                                    End If
                                                End If

                                                'Update values of variables used to control the adding
                                                currentSampleID = lstWSPatientsDS(i).SampleID
                                                currentStatFlag = CType(lstWSPatientsDS(i).StatFlag, Integer)

                                                'Generate the next OrderID
                                                orderID = ""
                                                myGlobalDataTO = myOrdersDelegate.GenerateOrderID(dbConnection)
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    orderID = CType(myGlobalDataTO.SetDatos, String)
                                                Else
                                                    'Error generating the next OrderID 
                                                    Exit For
                                                End If

                                                'Prepare the DataSet for the Order
                                                myOrdersDS.twksOrders.Clear()
                                                myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                                myOrderDR.OrderID = orderID
                                                myOrderDR.SampleClass = "PATIENT"
                                                myOrderDR.StatFlag = lstWSPatientsDS(i).StatFlag

                                                '** Field PatientID will be SampleID (in Uppercase characters) when SampleIDType=DB, otherwise it will be Null
                                                '** Field SampleID will be SampleID when SampleIDType=MANUAL, otherwise it will be Null
                                                If (lstWSPatientsDS(i).SampleIDType = "DB") Then myOrderDR.PatientID = lstWSPatientsDS(i).SampleID.ToUpperBS
                                                If (lstWSPatientsDS(i).SampleIDType = "MAN" OrElse lstWSPatientsDS(i).SampleIDType = "AUTO") Then myOrderDR.SampleID = lstWSPatientsDS(i).SampleID

                                                myOrderDR.OrderDateTime = DateTime.Now
                                                myOrderDR.OrderStatus = "OPEN"
                                                myOrderDR.TS_User = loggedUser
                                                myOrderDR.TS_DateTime = DateTime.Now
                                                myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                                myOrderTestDS.twksOrderTests.Clear()
                                            End If

                                            'Set value of the OrderID in the row DataSet in process
                                            lstWSPatientsDS(i).OrderID = orderID

                                            'Prepare the DataSet for the Order Tests
                                            myOrderTestDR = myOrderTestDS.twksOrderTests.NewtwksOrderTestsRow()
                                            myOrderTestDR.TestType = lstWSPatientsDS(i).TestType
                                            myOrderTestDR.TestID = lstWSPatientsDS(i).TestID
                                            myOrderTestDR.SampleType = lstWSPatientsDS(i).SampleType
                                            myOrderTestDR.OrderTestStatus = lstWSPatientsDS(i).OTStatus

                                            If (lstWSPatientsDS(i).TestType = "STD" OrElse lstWSPatientsDS(i).TestType = "ISE") Then
                                                'TubeType and NumReplicates is informed only for Standard and ISE Tests 
                                                myOrderTestDR.TubeType = lstWSPatientsDS(i).TubeType
                                                myOrderTestDR.ReplicatesNumber = lstWSPatientsDS(i).NumReplicates
                                            End If

                                            myOrderTestDR.CreationOrder = lstWSPatientsDS(i).CreationOrder
                                            myOrderTestDR.AnalyzerID = pAnalyzerID
                                            myOrderTestDR.SampleClass = "PATIENT"

                                            If (Not lstWSPatientsDS(i).IsTestProfileIDNull AndAlso lstWSPatientsDS(i).TestProfileID <> 0) Then
                                                myOrderTestDR.TestProfileID = lstWSPatientsDS(i).TestProfileID
                                            End If

                                            'SA 26/03/2013 - Inform LIS fields when they have a value in the entry DS
                                            If (Not lstWSPatientsDS(i).IsLISRequestNull) Then myOrderTestDR.LISRequest = lstWSPatientsDS(i).LISRequest
                                            If (Not lstWSPatientsDS(i).IsExternalQCNull) Then myOrderTestDR.ExternalQC = lstWSPatientsDS(i).ExternalQC
                                            If (Not lstWSPatientsDS(i).IsAwosIDNull) Then myOrderTestDR.AwosID = lstWSPatientsDS(i).AwosID
                                            If (Not lstWSPatientsDS(i).IsSpecimenIDNull) Then myOrderTestDR.SpecimenID = lstWSPatientsDS(i).SpecimenID
                                            If (Not lstWSPatientsDS(i).IsESPatientIDNull) Then myOrderTestDR.ESPatientID = lstWSPatientsDS(i).ESPatientID
                                            If (Not lstWSPatientsDS(i).IsLISPatientIDNull) Then myOrderTestDR.LISPatientID = lstWSPatientsDS(i).LISPatientID
                                            If (Not lstWSPatientsDS(i).IsESOrderIDNull) Then myOrderTestDR.ESOrderID = lstWSPatientsDS(i).ESOrderID
                                            If (Not lstWSPatientsDS(i).IsLISOrderIDNull) Then myOrderTestDR.LISOrderID = lstWSPatientsDS(i).LISOrderID

                                            myOrderTestDR.TS_User = loggedUser
                                            myOrderTestDR.TS_DateTime = DateTime.Now
                                            myOrderTestDS.twksOrderTests.AddtwksOrderTestsRow(myOrderTestDR)

                                            If (i = lstWSPatientsDS.Count - 1) Then
                                                'The Patient Order is created
                                                myGlobalDataTO = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, myOrderTestDS, Nothing)
                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                    myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                                    'Search the correspondent row in table Patients to update field OrderTestID 
                                                    For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests.Rows
                                                        testType = orderTestRow.TestType
                                                        testID = orderTestRow.TestID
                                                        sampleType = orderTestRow.SampleType

                                                        lstOrderTestsDS = (From a In pWSResultDS.Patients _
                                                                          Where a.SampleClass = "PATIENT" _
                                                                        AndAlso a.OrderID = orderID _
                                                                        AndAlso a.OTStatus = "OPEN" _
                                                                        AndAlso a.TestType = testType _
                                                                        AndAlso a.TestID = testID _
                                                                        AndAlso a.SampleType = sampleType _
                                                                         Select a).ToList()

                                                        If (lstOrderTestsDS.Count = 1) Then
                                                            lstOrderTestsDS(0).OrderTestID = orderTestRow.OrderTestID

                                                            'Prepare the corresponding WSOrderTestsDS row
                                                            PatientWSOrderTestsDSRow(pCreateOpenWS, pWorkSessionID, loggedUser, lstOrderTestsDS.First, pWSOrderTestsDS)
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'Get all Patient Order Tests having a Test (whatever type) included in at least a Calculated Test
                                Dim lstCalcOrderTests As List(Of WorkSessionResultDS.PatientsRow)
                                lstCalcOrderTests = (From a In pWSResultDS.Patients _
                                                    Where a.SampleClass = "PATIENT" _
                                             AndAlso (Not a.IsCalcTestIDNull AndAlso a.CalcTestID <> String.Empty) _
                                                 Order By a.SampleID, a.StatFlag _
                                                   Select a).ToList()

                                Dim myCalcOrderTestsDS As New OrderCalculatedTestsDS
                                Dim myOrderTestDR As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow

                                Dim myIndex As Integer
                                Dim currentSampleID As String = String.Empty
                                Dim currentStatFlag As Boolean = False
                                Dim calcTestsList() As String
                                Dim lstOrderTestID As List(Of Integer)

                                For Each calcTests As WorkSessionResultDS.PatientsRow In lstCalcOrderTests
                                    currentSampleID = calcTests.SampleID
                                    currentStatFlag = calcTests.StatFlag
                                    calcTestsList = calcTests.CalcTestID.Split(CChar(", "))

                                    For i As Integer = 0 To calcTestsList.Count - 1
                                        myIndex = i

                                        'Search the ID of the Order Test for the Calculated Test
                                        lstOrderTestID = (From b In pWSResultDS.Patients _
                                                          Where b.SampleClass = "PATIENT" _
                                                        AndAlso b.SampleID = currentSampleID _
                                                        AndAlso b.StatFlag = currentStatFlag _
                                                        AndAlso b.TestType = "CALC" _
                                                        AndAlso b.TestID = Convert.ToInt32(calcTestsList(myIndex)) _
                                                         Select b.OrderTestID).ToList()

                                        If (lstOrderTestID.Count = 1) Then
                                            'Add the pair to the typed DataSet OrderCalculatedTestsDS 
                                            myOrderTestDR = myCalcOrderTestsDS.twksOrderCalculatedTests.NewtwksOrderCalculatedTestsRow()
                                            myOrderTestDR.OrderTestID = calcTests.OrderTestID
                                            myOrderTestDR.CalcOrderTestID = Convert.ToInt32(lstOrderTestID.First)
                                            myCalcOrderTestsDS.twksOrderCalculatedTests.AddtwksOrderCalculatedTestsRow(myOrderTestDR)
                                        End If
                                    Next
                                Next
                                lstCalcOrderTests = Nothing

                                'Save data in table twksOrderCalculatedTests....
                                If (myCalcOrderTestsDS.twksOrderCalculatedTests.Rows.Count > 0) Then
                                    myGlobalDataTO = myCalcOrderTestsDelegate.Create(dbConnection, myCalcOrderTestsDS)
                                End If
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                                'Return the same entry DataSet updated...
                                pWSResultDS.Patients.AcceptChanges()
                                myGlobalDataTO.SetDatos = pWSResultDS
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PreparePatientOrders_NEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Patient Order Tests for OffSystem Tests and add/update/delete results for each one of them
        ''' </summary>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, Calibrators and Blanks that have to be 
        '''                           included in the Work Session</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param> 
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 19/03/2014 - BT #1545 ==> Divide AddWorkSession process in several DB Transactions
        '''</remarks>
        Private Function SaveOffSystemOTsResults(ByVal pWSResultDS As WorkSessionResultDS, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim returnData As GlobalDataTO = Nothing

            Try
                'Get all Patient Order Tests for OffSystem Tests
                Dim lstOrderTestsDS As List(Of WorkSessionResultDS.PatientsRow)
                lstOrderTestsDS = (From a As WorkSessionResultDS.PatientsRow In pWSResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.TestType = "OFFS" _
                               Order By a.TestID _
                                 Select a).ToList()

                If (lstOrderTestsDS.Count > 0) Then
                    Dim currentTest As Integer = -1
                    Dim currentResultType As String = String.Empty
                    Dim currentActiveRangeType As String = String.Empty

                    Dim myOffSTestsDS As New OffSystemTestsDS
                    Dim myOffSystemTestDelegate As New OffSystemTestsDelegate
                    Dim myOffSystemTestsResultsDS As New OffSystemTestsResultsDS
                    Dim myOFFSTestResultRow As OffSystemTestsResultsDS.OffSystemTestsResultsRow

                    For Each reqOFFS As WorkSessionResultDS.PatientsRow In lstOrderTestsDS
                        myOFFSTestResultRow = myOffSystemTestsResultsDS.OffSystemTestsResults.NewOffSystemTestsResultsRow
                        myOFFSTestResultRow.OrderTestID = reqOFFS.OrderTestID
                        myOFFSTestResultRow.TestID = reqOFFS.TestID
                        myOFFSTestResultRow.SampleType = reqOFFS.SampleType

                        If (reqOFFS.TestID <> currentTest) Then
                            currentTest = reqOFFS.TestID

                            'Get ResultType and ActiveRangeType for the OffSystem TestID/SampleType 
                            returnData = myOffSystemTestDelegate.GetByTestIDAndSampleType(Nothing, reqOFFS.TestID, reqOFFS.SampleType)
                            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                                myOffSTestsDS = DirectCast(returnData.SetDatos, OffSystemTestsDS)

                                currentResultType = "QUALTIVE"
                                currentActiveRangeType = String.Empty
                                If (myOffSTestsDS.tparOffSystemTests.Rows.Count = 1) Then
                                    currentResultType = myOffSTestsDS.tparOffSystemTests(0).ResultType

                                    If (Not myOffSTestsDS.tparOffSystemTests(0).IsActiveRangeTypeNull) Then
                                        currentActiveRangeType = myOffSTestsDS.tparOffSystemTests(0).ActiveRangeType
                                    End If
                                End If
                            Else
                                'Error getting the Result Type and ActiveRangeType of the OffSystem Test
                                Exit For
                            End If
                        End If

                        myOFFSTestResultRow.ResultType = currentResultType
                        If (currentActiveRangeType = String.Empty) Then
                            myOFFSTestResultRow.SetActiveRangeTypeNull()
                        Else
                            myOFFSTestResultRow.ActiveRangeType = currentActiveRangeType
                        End If

                        If (reqOFFS.IsOffSystemResultNull) Then
                            myOFFSTestResultRow.SetResultValueNull()
                        Else
                            myOFFSTestResultRow.ResultValue = reqOFFS.OffSystemResult
                        End If

                        myOFFSTestResultRow.SampleID = reqOFFS.SampleID
                        myOFFSTestResultRow.ResultDateTime = Now
                        myOffSystemTestsResultsDS.OffSystemTestsResults.AddOffSystemTestsResultsRow(myOFFSTestResultRow)
                    Next
                    myOffSystemTestsResultsDS.AcceptChanges()

                    If (Not returnData.HasError) Then
                        'Add/update/delete results for all requested Off-SystemTests
                        Dim myResultsDelegate As New ResultsDelegate
                        returnData = myResultsDelegate.SaveOffSystemResults(Nothing, myOffSystemTestsResultsDS, pAnalyzerID, pWorkSessionID)

                        'Get the last ExportedResults to send to LIS
                        If (Not returnData.HasError) Then Me.lastExportedResultsDSAttribute = myResultsDelegate.LastExportedResults
                    End If
                Else
                    'If there are not OffSystem Tests in the WorkSession, returns an empty GlobalDataTO
                    returnData = New GlobalDataTO()
                End If
                lstOrderTestsDS = Nothing
            Catch ex As Exception
                returnData = New GlobalDataTO()
                returnData.HasError = True
                returnData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.SaveOffSystemOTsResults", EventLogEntryType.Error, False)
            End Try
            Return returnData
        End Function

        ''' <summary>
        ''' Go through each table in the WorkSessionResultDS DataSet(Patients-Controls-BlankCalibrators) and process the Selected records according 
        ''' the Sample Class. Once all the selected records have been processed, they are added to a new or existing WorkSession
        ''' </summary>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples, Controls, Calibrators and Blanks that have 
        '''                           to be included in the Work Session</param>
        ''' <param name="pWorkSessionID">Identifier of the Work Session to which the Open Order Tests have to be linked</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer in which the requested Order Tests will be executed</param>
        ''' <param name="pCreateOpenWS">When TRUE, it indicates all Order Tests have to be linked to the WorkSession with OpenOTFlag = TRUE</param>
        ''' <param name="pCurrentWSStatus">Current Status of the WS to update. Optional parameter; when informed, it allows to known if the status
        '''                                have to be updated, which happen in following cases: from OPEN to PENDING and from CLOSED to INPROCESS; 
        '''                                in the rest of cases a status change is not needed</param>
        ''' <param name="pChangesMade">Flag indicating if some Order Tests have been added/deleted to the WS</param>
        ''' <param name="pLISWithFiles">Flag indicating if the LIS implementation is using the ES library (when FALSE) or plain files (when TRUE). 
        '''                             Optional parameter</param>
        ''' <returns>GlobalDataTO containing error information and, when a new WorkSession has been created, a typed DataSet WorkSessionDS, 
        '''          containing the identifier of the created Work Session and also information about the Analyzer in which the Work Session 
        '''          will be executed</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 16/03/2010 - Each Control required for Test/SampleType is managed as an individual OrderTestID (field ControlNumber was 
        '''                              added to Order Tests table and DS to known which OrderTestID corresponds to each Control Number)
        '''              SA 14/04/2010 - Added calls to methods ReorganizeBlanks and ReorganizeCalibrators when there are Order Tests selected to be 
        '''                              included in a WorkSession
        '''              SA 26/04/2010 - Pass parameter pWorkSessionID to all Prepare** functions; changed parameter pIncludeOTsInWS by pCreateEmptyWS 
        '''                              to indicate when all Order Tests have to be saved as unselected 
        '''              SA 11/06/2010 - Added new optional parameter for the current WS Status, needed in WS updation to change the status when it is 
        '''                              needed. 
        '''              SA 20/10/2010 - Call to UpdateInUseFlag for elements removed from the WS when there is nothing to add to the WS
        '''              SA 22/10/2010 - For Patient Samples, set ToSendFlag = True also for ISE Tests, not only for Standard ones
        '''              SA 19/01/2011 - Once the WorkSession has been created/updated, get all Off-System OrderTests and save the result values entered for them
        '''              TR 05/08/2011 - After using Lists, set value to nothing to release memory
        '''              SA 20/09/2011 - Added management of incomplete Patient Samples (call UpdateRelatedIncompletedSamples in class 
        '''                              BarCodePositionsWithNoRequestDelegate) before add/update the WorkSession
        '''              SA 12/04/2012 - Added management of Alternative Calibrators when they are INPROCESS and new Order Tests are requested for the same 
        '''                              Test and one of the SampleTypes using the Calibrator and not requested in the WS creation 
        '''              SA 10/04/2013 - Added new optional parameter to indicate if the LIS mode in use is plain Files or ES 
        '''              SA 08/05/2013 - When there are not Order Tests with status OPEN to add to the active WS and the function has been called with LIS 
        '''                              implemented using the ES Library (pLISWithFiles = FALSE), verify if there are NOT IN USE positions in the WorkSession 
        '''                              that corresponds to the new Patient Elements to add (if any). NOTE: there are new Patient Elements to add when some
        '''                              Patient Order Tests with Status OPEN have been selected for positioning 
        '''              SA 09/09/2013 - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous one still exists
        '''              SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Public Function PrepareOrderTestsForWS_NEW(ByVal pWSResultDS As WorkSessionResultDS, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                   ByVal pCreateOpenWS As Boolean, Optional ByVal pCurrentWSStatus As String = "", _
                                                   Optional ByVal pChangesMade As Boolean = True, Optional ByVal pLISWithFiles As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myWSOrderTestsDS As New WSOrderTestsDS
                Dim myOrderTestsDelegate As New OrderTestsDelegate

                If (pChangesMade) Then
                    'If some Order Tests will be sent to the Analyzer then reorganize the Open Order Tests to place selected ones first
                    If (Not pCreateOpenWS) Then
                        myOrderTestsDelegate.ReorganizePatients(pWSResultDS)
                        myOrderTestsDelegate.ReorganizeControls(pWSResultDS)
                        myOrderTestsDelegate.ReorganizeCalibrators(pWSResultDS)
                        myOrderTestsDelegate.ReorganizeBlanks(pWSResultDS)
                    End If

                    'DB TRANSACTION #1 - Create/Update a Blank Order.... 
                    myGlobalDataTO = PrepareBlankOrders_NEW(Nothing, pWSResultDS, pCreateOpenWS, pWorkSessionID, pAnalyzerID, myWSOrderTestsDS)

                    'DB TRANSACTION #2 - Create/Update a Calibrator Order... 
                    If (Not myGlobalDataTO.HasError) Then myGlobalDataTO = PrepareCalibratorOrders_NEW(Nothing, pWSResultDS, pCreateOpenWS, pWorkSessionID, pAnalyzerID, myWSOrderTestsDS)

                    'DB TRANSACTION #3 - Create/Update a Control Order... 
                    If (Not myGlobalDataTO.HasError) Then myGlobalDataTO = PrepareControlOrders_NEW(Nothing, pWSResultDS, pCreateOpenWS, pWorkSessionID, pAnalyzerID, myWSOrderTestsDS)

                    'DB TRANSACTION #4 - Create/Update Patient Orders... 
                    If (Not myGlobalDataTO.HasError) Then myGlobalDataTO = PreparePatientOrders_NEW(Nothing, pWSResultDS, pCreateOpenWS, pWorkSessionID, pAnalyzerID, myWSOrderTestsDS)
                End If

                Dim newWS As Boolean = (pWorkSessionID.Trim = String.Empty)
                If (Not myGlobalDataTO.HasError) Then
                    If (Not pLISWithFiles) Then
                        If (myWSOrderTestsDS.twksWSOrderTests.Rows.Count > 0) Then
                            '******************************************************************************************************'
                            '** THE ACTIVE WORKSESSION IS UPDATED BY ADDING TO IT ALL NEW ORDER TESTS (THOSE HAVING STATUS OPEN) **' 
                            '******************************************************************************************************'

                            'DB TRANSACTION #5 - Link all added Order Tests to the active WorkSession
                            Dim wsOrderTestData As New WSOrderTestsDelegate
                            myGlobalDataTO = wsOrderTestData.AddOrderTestsToWS(Nothing, myWSOrderTestsDS, (pCurrentWSStatus = "EMPTY"))

                            If (Not myGlobalDataTO.HasError) Then
                                'DB TRANSACTION #6 - Create/update a new WorkSession by adding it all selected Order Tests
                                myGlobalDataTO = AddWorkSession_NEW(Nothing, myWSOrderTestsDS, newWS, pAnalyzerID, pCurrentWSStatus, Not pLISWithFiles)

                                If (newWS) Then
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                                        Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                                        If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                                            If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                                                'Dim myLogAcciones As New ApplicationLogManager()
                                                GlobalBase.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", _
                                                                                "WorkSessionsDelegate.PrepareOrderTestsForWS", EventLogEntryType.Error, False)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            '*********************************************************************************'
                            '** THERE ARE NOT ORDER TESTS WITH STATUS OPEN TO ADD TO THE ACTIVE WORKSESSION **' 
                            '*********************************************************************************'

                            'DB TRANSACTION #5 - Verify if there are not In Use positions in the WorkSession that corresponds to the new added Patient elements
                            myGlobalDataTO = FindPatientsInNotInUsePositions(Nothing, pAnalyzerID, pWorkSessionID, "SAMPLES")

                            'DB TRANSACTION #6 - There is nothing to add to the WorkSession, update value of InUseFlag to False for all elements that were 
                            'deleted from the Work Session
                            myGlobalDataTO = UpdateInUseFlag(Nothing, pWorkSessionID, pAnalyzerID, False, True)

                            'If there is nothing to add to the WorkSession, this is needed to avoid an error when the function
                            'that use the value returned try to get a WorkSessionsDS (returned when AddWorkSession is called)
                            myGlobalDataTO.SetDatos = New WorkSessionsDS
                        End If
                    Else
                        '*************************************'
                        '** ¡¡OLD CODE FOR LIS WITH FILES!! **' 
                        '*************************************'
                        If (myWSOrderTestsDS.twksWSOrderTests.Rows.Count > 0) Then
                            'Verify if changes are needed in the table containing the Incomplete Samples
                            If (Not newWS) Then
                                Dim resultData As GlobalDataTO = Nothing
                                Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate
                                resultData = myBCPosWithNoRequestDelegate.UpdateRelatedIncompletedSamples(Nothing, pAnalyzerID, pWorkSessionID, _
                                                                                                          pWSResultDS, (Not pCreateOpenWS))
                                'If an error has happened, pass the error information to the GlobalDataTO to be returned
                                If (resultData.HasError) Then myGlobalDataTO = resultData
                            End If
                        Else
                            'There is nothing to add to the WorkSession, update value of InUseFlag to False for all elements 
                            'that were deleted from the Work Session
                            myGlobalDataTO = UpdateInUseFlag(Nothing, pWorkSessionID, pAnalyzerID, False, True)

                            'If there is nothing to add to the WorkSession, this is needed to avoid an error when the function
                            'that use the value returned try to get a WorkSessionsDS (returned when AddWorkSession is called)
                            myGlobalDataTO.SetDatos = New WorkSessionsDS
                        End If
                    End If
                End If

                'Finally, get all requested Off-System Tests and prepare a DS to save the informed results
                If (Not myGlobalDataTO.HasError) Then
                    Dim returnData As GlobalDataTO = Nothing
                    returnData = SaveOffSystemOTsResults(pWSResultDS, pAnalyzerID, pWorkSessionID)

                    'If an error has happened, pass the error information to the GlobalDataTO to be returned
                    If (returnData.HasError) Then myGlobalDataTO = returnData
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WorkSessionsDelegate.PrepareOrderTestsForWS_NEW", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region
    End Class
End Namespace





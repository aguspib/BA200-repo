Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.GlobalConstants

Namespace Biosystems.Ax00.BL
    Partial Public Class OrderTestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Add all needed elements (Controls, Calibrators and Blanks) for a Patient OrderTests requested by LIS
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionResultDS">Typed DS WorkSessionResultsDS containing all Order Tests (all Sample Classes) added and to add 
        '''                                    to the active WorkSession</param>
        ''' <returns>GlobalDataTO containing the WorkSessionResultDS received as entry parameter after updating the subtables of Controls and 
        '''          BlankCalibrators with the added elements</returns>
        ''' <remarks>
        ''' Created by:  TR 26/09/2013
        ''' Modified by: TR 17/05/2013 - Inform optional parameter for the SampleClass when calling function AddControlOrderTests
        ''' </remarks>
        Public Function AddAllNeededOrderTestsForLIS(pAnalyzerID As String, ByRef pWorkSessionResultDS As WorkSessionResultDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO  'DO NOT REMOVE THE NEW!!

            Try
                'Sort Patients table in pWSResultsDS. 
                SortPatients(pWorkSessionResultDS, 1)

                '(0) Process for adding all needed(Controls, Calibrators) and Blanks for STD and ISE Tests requested for Patients.
                Dim filteredWorkSessionResultList As New List(Of String)
                filteredWorkSessionResultList = (From a As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients _
                                                Where a.SampleClass = "PATIENT" _
                                              AndAlso (a.TestType = "STD" Or a.TestType = "ISE") _
                                              AndAlso a.ExternalQC = False _
                                               Select a.TestType & "|" & a.TestID & "|" & a.SampleType).Distinct.ToList()

                Dim mySelectedTestDS As New SelectedTestsDS
                Dim mySelectedTestRow As SelectedTestsDS.SelectedTestTableRow

                'Process for each TestType / Test / SampleType
                For Each myWSResult As String In filteredWorkSessionResultList
                    mySelectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow

                    mySelectedTestRow.TestType = myWSResult.Split(CChar("|"))(0).ToString()
                    mySelectedTestRow.TestID = CInt(myWSResult.Split(CChar("|"))(1).ToString())
                    mySelectedTestRow.SampleType = myWSResult.Split(CChar("|"))(2).ToString()
                    mySelectedTestRow.Selected = True

                    mySelectedTestDS.SelectedTestTable.AddSelectedTestTableRow(mySelectedTestRow)
                Next

                If (mySelectedTestDS.SelectedTestTable.Count > 0) Then
                    'Add Controls, Calibrators and Blanks for the group of Tests / Sample Types in the DS
                    myGlobalDataTO = AddControlOrderTests(mySelectedTestDS, pWorkSessionResultDS, pAnalyzerID, False, True, "PATIENT")
                End If

                '(1) Process for adding all needed Calibrators and Blanks for STD Tests requested for External Controls.
                If (Not myGlobalDataTO.HasError) Then
                    mySelectedTestDS.SelectedTestTable.Clear()

                    filteredWorkSessionResultList = (From a As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients _
                                                    Where a.SampleClass = "PATIENT" _
                                                  AndAlso a.TestType = "STD" _
                                                  AndAlso a.ExternalQC = True _
                                                   Select a.TestID & "|" & a.SampleType).Distinct.ToList()

                    'Process for each Test / SampleType
                    If (filteredWorkSessionResultList.Count > 0) Then
                        For Each myWSResult As String In filteredWorkSessionResultList
                            mySelectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow

                            mySelectedTestRow.TestID = CInt(myWSResult.Split(CChar("|"))(0).ToString())
                            mySelectedTestRow.SampleType = myWSResult.Split(CChar("|"))(1).ToString()
                            mySelectedTestRow.TestType = "STD"

                            mySelectedTestDS.SelectedTestTable.AddSelectedTestTableRow(mySelectedTestRow)
                        Next

                        'Add Calibrators and Blanks for the group of Tests / Sample Types in the DS
                        myGlobalDataTO = AddCalibratorOrderTests(mySelectedTestDS, pWorkSessionResultDS, pAnalyzerID, False, True)
                    End If
                End If

                '(2) Check Calibrators and Blanks that have to be marked as Selected.
                If (Not myGlobalDataTO.HasError) Then
                    'Search in the active WS (pWsResultsDS.Patients) all different STD Tests/SampleTypes marked as selected
                    filteredWorkSessionResultList = (From a As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients _
                                                    Where a.SampleClass = "PATIENT" _
                                                  AndAlso a.TestType = "STD" _
                                                  AndAlso a.Selected _
                                                   Select a.TestID & "|" & a.SampleType).Distinct.ToList()

                    'Process for each Test / SampleType
                    For Each myWSResult As String In filteredWorkSessionResultList
                        'Search the needed Blank and Calibrator for the Test / SampleType and select them if they are unselected
                        SelectAllNeededOrderTests("PATIENT", "STD", CInt(myWSResult.Split(CChar("|"))(0).ToString()), _
                                                   myWSResult.Split(CChar("|"))(1).ToString(), pWorkSessionResultDS)
                    Next
                End If
                filteredWorkSessionResultList = Nothing

                If (Not myGlobalDataTO.HasError) Then myGlobalDataTO.SetDatos = pWorkSessionResultDS
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.AddAllNeededOrderTestsForLIS", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Function used in the screen of WSPreparation to manage the adding/deleting of Blank Order Tests
        ''' (only in the screen DataSet, this function does not changes in DataBase)
        ''' </summary>
        ''' <param name="pSelectedTestsDS">Typed DataSet containing the list of Tests for which a Blank Order
        '''                                Test has been requested</param>
        ''' <param name="pWSResultDS">Typed DataSet containing in the table of Blanks and Calibrators the list of 
        '''                           currently requested Blank Order Tests</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of Blanks and Calibrators
        '''          updated with the added Blank Order Tests</returns>
        ''' <remarks>
        ''' Created by:  GDS
        ''' Modified by: SA  03/03/2010 - Added parameter for the Analyzer Identifier (needed to get Last Blank results)
        '''              SA  26/03/2010 - Changes to set value for field CreationOrder: Blank Order Tests are sorted in the same 
        '''                               order they are requested
        '''              SA  18/02/2011 - Changes to get Test data (name, blank replicates and version number) and the Sample Type
        '''                               defined as default for the Test by calling one function instead of two.
        '''              RH  07/06/2011 - Added Default Tube Type and insert in the DS to return the BlankMode defined for the Test
        '''              SA  26/03/2013 - Added changes needed for new LIS implementation: new optional parameter pFromLIS Boolean with default value False. 
        '''                               When pFromLIS = TRUE: Do not search the icon for Blanks
        ''' </remarks>
        Public Function AddBlankOrderTests(ByVal pSelectedTestsDS As SelectedTestsDS, ByVal pWSResultDS As WorkSessionResultDS, _
                                           ByVal pAnalyzerID As String, Optional ByVal pFromLIS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO

            Try
                'Get the default tube used for Blanks
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                resultData = myUserSettingsDelegate.GetDefaultSampleTube(Nothing, "BLANK")

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim defaultTubeBlank As String = DirectCast(resultData.SetDatos, String)

                    'Get maximum number of days that can have passed from the last Blank or Calibrator execution
                    Dim maxDaysPreviousBlkCalib As String = ""
                    Dim myGeneralSettingsDelegate As New GeneralSettingsDelegate

                    resultData = myGeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_DAYS_PREVIOUS_BLK_CALIB.ToString)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        maxDaysPreviousBlkCalib = DirectCast(resultData.SetDatos, String)
                    End If

                    'SA 26/03/2013 - When function is used for processing LIS Order Tests, the icon is not obtained
                    Dim bArrImage As Byte() = Nothing
                    If (Not pFromLIS) Then
                        'Get the Blank Icon image byte array
                        Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                        bArrImage = preloadedDataConfig.GetIconImage("BLANK")
                    End If

                    'Get the max value of CreationOrder for Blanks in the list of Blanks and Calibrators
                    Dim lstBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstBlanksDS = (From a In pWSResultDS.BlankCalibrators _
                                  Where a.SampleClass = "BLANK" _
                                 Select a).ToList()

                    Dim nextCreationOrder As Integer = 1
                    If (lstBlanksDS.Count > 0) Then
                        Dim q = (From p In pWSResultDS.BlankCalibrators _
                                Where p.SampleClass = "BLANK" _
                               Select p.CreationOrder).Max

                        nextCreationOrder = q + 1
                    End If

                    'Declare all variables needed inside the FOR loop
                    Dim testID As Integer = -1
                    Dim testType As String = String.Empty
                    Dim lstWSResultDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)

                    Dim myTestsDS As New TestsDS
                    Dim myTestSamplesDelegate As New TestSamplesDelegate

                    Dim testNumRepl As Integer = -1
                    Dim testVersionNumber As Integer = -1
                    Dim blankMode As String = String.Empty
                    Dim testName As String = String.Empty
                    Dim defaultSampleType As String = String.Empty

                    Dim myResultsDelegate As New ResultsDelegate
                    Dim myHisResultsDelegate As New HisWSResultsDelegate
                    Dim myBlankCalibratorsRow As WorkSessionResultDS.BlankCalibratorsRow
                    Dim myBlankCalibratorsDataTable As WorkSessionResultDS.BlankCalibratorsDataTable
                    Dim myAdditionalElementsTable As WSAdditionalElementsDS.WSAdditionalElementsTableDataTable

                    'Test if all Blank Order Tests in pSelectedTestsDS have been processed
                    For Each selectedTestRow As SelectedTestsDS.SelectedTestTableRow In pSelectedTestsDS.SelectedTestTable
                        'Use temporary variables for avoid unexpected results in LINQ
                        testType = selectedTestRow.TestType
                        testID = selectedTestRow.TestID

                        'Verify if there is already a Blank Order Test for the selected Test 
                        lstWSResultDS = (From a In pWSResultDS.BlankCalibrators _
                                        Where a.SampleClass = "BLANK" _
                                      AndAlso a.TestType = testType _
                                      AndAlso a.TestID = testID _
                                       Select a).ToList()

                        'If there is not a Blank Order Test requested for the informed Test
                        If (lstWSResultDS.Count = 0) Then
                            If (testType = "STD") Then
                                'Get Test Name, Num of Blank Replicates and Test Version Number from the Test definition, 
                                'and get also the default SampleType used for the Test
                                resultData = myTestSamplesDelegate.GetDefaultSampleType(Nothing, testID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myTestsDS = DirectCast(resultData.SetDatos, TestsDS)

                                    If (myTestsDS.tparTests.Rows.Count = 1) Then
                                        testName = myTestsDS.tparTests(0).TestName
                                        testNumRepl = myTestsDS.tparTests(0).BlankReplicates
                                        testVersionNumber = myTestsDS.tparTests(0).TestVersionNumber
                                        defaultSampleType = myTestsDS.tparTests(0).SampleType
                                        blankMode = myTestsDS.tparTests(0).BlankMode

                                        'Insert the new Blank Order Tests
                                        myBlankCalibratorsDataTable = DirectCast(pWSResultDS.BlankCalibrators, WorkSessionResultDS.BlankCalibratorsDataTable)
                                        myBlankCalibratorsRow = myBlankCalibratorsDataTable.NewBlankCalibratorsRow()
                                        myBlankCalibratorsRow.Selected = Convert.ToBoolean(IIf(pFromLIS, False, True))
                                        myBlankCalibratorsRow.SampleClass = "BLANK"

                                        'SA 26/03/2013 - When function is used for processing LIS Order Tests, the icon is not obtained
                                        If (Not pFromLIS) Then myBlankCalibratorsRow.SampleClassIcon = bArrImage

                                        myBlankCalibratorsRow.TestType = testType
                                        myBlankCalibratorsRow.TestID = testID
                                        myBlankCalibratorsRow.TestName = testName
                                        myBlankCalibratorsRow.SampleType = defaultSampleType
                                        myBlankCalibratorsRow.NumReplicates = testNumRepl
                                        myBlankCalibratorsRow.OTStatus = "OPEN"

                                        'RH 07/06/2011
                                        myBlankCalibratorsRow.BlankMode = blankMode
                                        If (blankMode <> "REAGENT") Then myBlankCalibratorsRow.TubeType = defaultTubeBlank

                                        'Search if there is a recent result for the Blank executed for the same test
                                        resultData = myResultsDelegate.GetLastExecutedBlank(Nothing, testID, testVersionNumber, pAnalyzerID, maxDaysPreviousBlkCalib)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myAdditionalElementsTable = DirectCast(resultData.SetDatos, WSAdditionalElementsDS).WSAdditionalElementsTable

                                            If (myAdditionalElementsTable.Rows.Count > 0) Then
                                                myBlankCalibratorsRow.PreviousOrderTestID = myAdditionalElementsTable(0).PreviousOrderTestID
                                                myBlankCalibratorsRow.ABSValue = myAdditionalElementsTable(0).ABSValue.ToString
                                                myBlankCalibratorsRow.ABSDateTime = myAdditionalElementsTable(0).ResultDateTime

                                                myBlankCalibratorsRow.NewCheck = False
                                                myBlankCalibratorsRow.Selected = False
                                            Else
                                                myBlankCalibratorsRow.NewCheck = True
                                                myBlankCalibratorsRow.Selected = Convert.ToBoolean(IIf(pFromLIS, False, True))
                                            End If

                                            myBlankCalibratorsRow.CreationOrder = nextCreationOrder
                                            myBlankCalibratorsDataTable.Rows.Add(myBlankCalibratorsRow)

                                            'Increment value of the last nextCreationOrder
                                            nextCreationOrder += 1
                                        Else
                                            'Error searching if there is a previous result calculated for the Blank of the Test
                                            Exit For
                                        End If
                                    Else
                                        'Test data not found (this case is not possible)
                                        Exit For
                                    End If
                                Else
                                    'Error getting additional Test data (name, num of Blank replicates and current test version)
                                    Exit For
                                End If
                            End If
                        End If
                    Next

                    'TR 04/08/2011 -Set value to nothing 
                    lstBlanksDS = Nothing
                    lstWSResultDS = Nothing
                    bArrImage = Nothing

                    'Return the same entry DataSet with table of Blanks and Calibrators updated
                    If (Not resultData.HasError) Then
                        'Confirm the changes in Blanks and Calibrators Table and return the DataSet
                        pWSResultDS.AcceptChanges()
                        resultData.SetDatos = pWSResultDS
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.AddBlankOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used in the screen of WSPreparation to manage the adding/deleting of Control Order Tests
        ''' (only in the screen DataSet, this function does not change the DataBase)
        ''' </summary>
        ''' <param name="pSelectedTestsDS">Typed DataSet containing the list of Test/SampleType for which a Control 
        '''                                Order Test has been requested</param>
        ''' <param name="pWSResultDS">Typed DataSet containing in the table of Controls the list of currently requested
        '''                           Control Order Tests</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pForLoadSavedWS">Flag to avoid calling functions to get additional data for Tests (according the TestType) and the needed  
        '''                               Calibrators and Blanks when this function is used for loading a manual Saved WS (for LIS Saved WS, the 
        '''                               needed Calibrators and Blanks have to be obtained). Optional parameter</param>
        ''' <param name="pFromLIS">Flag to indicate when the function has been called in the process of Orders Download from LIS (implemented with ES); in
        '''                        this case its value is TRUE. Optional parameter with default value FALSE. When its value is TRUE, the icon for Controls is 
        '''                        not searched and the Sort Controls is not executed</param>
        ''' <param name="pSampleClass">Value of this field is PATIENT when the function is called after adding Patient Samples to the WS, or CTRL (defaul value)
        '''                            when it is called to add Control Samples to the WS. In the first case, needed Calibrators and Blanks will be also added 
        '''                            although no Controls were found for the TestType/TestID/SampleType</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of Controls updated with the
        '''          added Control Order Tests</returns>
        ''' <remarks>
        ''' Created by:  DL 02/03/2010
        ''' Modified by: SA 26/03/2010 - Changes to set value for field CreationOrder: Control Order Tests are sorted in the same order 
        '''                              they are requested, but grouped by the Control they need
        '''              SA 18/02/2011 - Some changes to improve the code (use AndAlso instead of And; remove unneeded "New")
        '''              SA 22/02/2011 - Added optional parameter pForLoadSavedWS to avoid calling function to get the needed Calibrators  
        '''                              and Blanks when this function is used for loading a Saved WS
        '''              DL 15/04/2011 - Replaced use of field ControlNumber for field ControlID
        '''              SA 18/06/2012 - Process Controls also for ISE Tests; get the Icon according the TestType and load in the DS to return
        '''              SA 26/03/2013 - Added changes needed for new LIS implementation: when load the Control in Controls table in pWSResultsDS, inform 
        '''                              all LIS fields contained in pSelectedTestsDS when they are informed. Besides, add new optional parameter pFromLIS 
        '''                              Boolean with default value False. When pFromLIS = TRUE: 
        '''                              ** Do not search the icon for Controls
        '''                              ** Do not call function SortControls
        '''                              ** Inform the parameter when calling function AddCalibratorOrderTests
        '''              SA 17/05/2013 - Added optional parameter pSampleClass to manage the adding of needed Calibrators and Blanks according the functionality 
        '''                              for which the function was called:
        '''                              ** When the function has been called after adding Patient Samples, add the needed Calibrators and Blanks always, no matter if 
        '''                                 Controls were not added for none of the TestType/TestID/SampleType
        '''                              ** When the function has been called for adding Control Samples, add the needed Calibrators and Blanks only for those 
        '''                                 TestType/TestID/SampleType for which Controls were added 
        ''' </remarks>
        Public Function AddControlOrderTests(ByVal pSelectedTestsDS As SelectedTestsDS, ByVal pWSResultDS As WorkSessionResultDS, ByVal pAnalyzerID As String, _
                                             Optional ByVal pForLoadSavedWS As Boolean = False, Optional ByVal pFromLIS As Boolean = False, _
                                             Optional pSampleClass As String = "CTRL") As GlobalDataTO
            Dim resultData As GlobalDataTO
            Try
                'Get the default tube used for Controls
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                resultData = myUserSettingsDelegate.GetDefaultSampleTube(Nothing, "CTRL")

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim defaultTubeControl As String = CType(resultData.SetDatos, String)

                    'Verify if there are Controls 
                    Dim nextCreationOrder As Integer = pWSResultDS.Controls.Rows.Count + 1

                    'SA 26/03/2013 - When function is used for processing LIS Order Tests, the icons are not obtained
                    Dim bTestTypeIconSTD As Byte() = Nothing
                    Dim bTestTypeIconISE As Byte() = Nothing

                    If (Not pFromLIS) Then
                        'Get Icon Image byte array for Test Type
                        Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                        bTestTypeIconSTD = preloadedDataConfig.GetIconImage("FREECELL")
                        bTestTypeIconISE = preloadedDataConfig.GetIconImage("TISE_SYS")
                    End If

                    'All Control Order Tests in pSelectedTestsDS have been processed?
                    Dim controlID As Integer
                    Dim tubeTypeToAssign As String
                    Dim selectedTubeType As String
                    Dim maxCreationOrder As Integer
                    Dim myTestControlsDS As New TestControlsDS
                    Dim myTestControlsDelegate As New TestControlsDelegate
                    Dim lstWSCtrlDS As List(Of WorkSessionResultDS.ControlsRow)
                    Dim myControlsRow As WorkSessionResultDS.ControlsRow
                    Dim myControlsDataTable As WorkSessionResultDS.ControlsDataTable
                    Dim mySelectedTestTableRow As SelectedTestsDS.SelectedTestTableRow

                    For Each selectedTestsRow As SelectedTestsDS.SelectedTestTableRow In pSelectedTestsDS.SelectedTestTable.Rows
                        'Only selected Controls will be added; partially selected Controls are not added
                        If (selectedTestsRow.Selected) Then
                            mySelectedTestTableRow = selectedTestsRow

                            If (mySelectedTestTableRow.TestType = "STD") OrElse (mySelectedTestTableRow.TestType = "ISE") Then
                                'Get data of the Controls needed for the selected Test/SampleType
                                resultData = myTestControlsDelegate.GetControlsNEW(Nothing, selectedTestsRow.TestType, selectedTestsRow.TestID, _
                                                                                   selectedTestsRow.SampleType, 0, True)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myTestControlsDS = DirectCast(resultData.SetDatos, TestControlsDS)

                                    'JCM 16-05-2013 If there are active Controls for the TestType/TestID/SampleType, it is marked as Added
                                    selectedTestsRow.Added = (myTestControlsDS.tparTestControls.Rows.Count > 0)
                                    For Each myTestControlsRow As TestControlsDS.tparTestControlsRow In myTestControlsDS.tparTestControls.Rows
                                        controlID = myTestControlsRow.ControlID
                                        lstWSCtrlDS = (From a As WorkSessionResultDS.ControlsRow In pWSResultDS.Controls _
                                                      Where a.SampleClass = "CTRL" _
                                                    AndAlso a.TestType = mySelectedTestTableRow.TestType _
                                                    AndAlso a.TestID = mySelectedTestTableRow.TestID _
                                                    AndAlso a.SampleType = mySelectedTestTableRow.SampleType _
                                                    AndAlso a.ControlID = controlID _
                                                     Select a).ToList()

                                        If (lstWSCtrlDS.Count = 0) Then
                                            'Verify if the needed Control is used for Tests already added to get the TubeType currently set for it
                                            tubeTypeToAssign = defaultTubeControl
                                            selectedTubeType = (From q In pWSResultDS.Controls _
                                                               Where q.SampleClass = "CTRL" _
                                                             AndAlso q.ControlID = controlID _
                                                              Select q.TubeType).Distinct().ToString

                                            If (selectedTubeType.Count = 1) Then
                                                tubeTypeToAssign = selectedTubeType(0).ToString

                                                'Get the max value of CreationOrder for Order Tests using the same Control 
                                                maxCreationOrder = Convert.ToInt32((From p In pWSResultDS.Controls _
                                                                                   Where p.SampleClass = "CTRL" _
                                                                                 AndAlso p.ControlID = controlID _
                                                                                  Select p.CreationOrder).Max())
                                                nextCreationOrder = maxCreationOrder + 1
                                            End If

                                            myControlsDataTable = DirectCast(pWSResultDS.Controls, WorkSessionResultDS.ControlsDataTable)
                                            myControlsRow = myControlsDataTable.NewControlsRow() 'RH 22/06/2012
                                            myControlsRow.Selected = False
                                            myControlsRow.SampleClass = "CTRL"

                                            'SA 26/03/2013 - When function is used for processing LIS Order Tests, the icons are not obtained
                                            myControlsRow.TestType = selectedTestsRow.TestType
                                            If (Not pFromLIS) Then
                                                If (selectedTestsRow.TestType = "STD") Then
                                                    myControlsRow.TestTypeIcon = bTestTypeIconSTD
                                                ElseIf (selectedTestsRow.TestType = "ISE") Then
                                                    myControlsRow.TestTypeIcon = bTestTypeIconISE
                                                End If
                                            End If

                                            'TR 12/03/2013 Set ListRequestIcon as FREE CELL
                                            'SA 26/03/2013 - When function is used for processing LIS Order Tests, the icons are not obtained
                                            If (Not pFromLIS) Then myControlsRow.LISRequestIcon = bTestTypeIconSTD

                                            myControlsRow.TestID = myTestControlsRow.TestID
                                            myControlsRow.TestName = myTestControlsRow.TestName
                                            myControlsRow.SampleType = selectedTestsRow.SampleType
                                            myControlsRow.NumReplicates = myTestControlsRow.ControlReplicates
                                            myControlsRow.TubeType = tubeTypeToAssign
                                            myControlsRow.ControlID = myTestControlsRow.ControlID
                                            myControlsRow.ControlName = myTestControlsRow.ControlName
                                            myControlsRow.LotNumber = myTestControlsRow.LotNumber
                                            myControlsRow.ExpirationDate = CDate(myTestControlsRow.ExpirationDate)
                                            myControlsRow.OTStatus = "OPEN"
                                            myControlsRow.CreationOrder = nextCreationOrder

                                            'SA 26/03/2013 - Inform field LISRequest when it is informed; otherwise, set False as value
                                            myControlsRow.LISRequest = False
                                            If (Not selectedTestsRow.IsLISRequestNull) Then myControlsRow.LISRequest = selectedTestsRow.LISRequest

                                            'SA 26/03/2013 - Inform also the rest of LIS fields when they are informed
                                            If (Not selectedTestsRow.IsAwosIDNull) Then myControlsRow.AwosID = selectedTestsRow.AwosID
                                            If (Not selectedTestsRow.IsSpecimenIDNull) Then myControlsRow.SpecimenID = selectedTestsRow.SpecimenID
                                            If (Not selectedTestsRow.IsESPatientIDNull) Then myControlsRow.ESPatientID = selectedTestsRow.ESPatientID
                                            If (Not selectedTestsRow.IsLISPatientIDNull) Then myControlsRow.LISPatientID = selectedTestsRow.LISPatientID
                                            If (Not selectedTestsRow.IsESOrderIDNull) Then myControlsRow.ESOrderID = selectedTestsRow.ESOrderID
                                            If (Not selectedTestsRow.IsLISOrderIDNull) Then myControlsRow.LISOrderID = selectedTestsRow.LISOrderID

                                            ' XB 01/09/2014 - BA #1868
                                            myControlsRow.ControlLevel = myTestControlsRow.ControlLevel

                                            myControlsDataTable.Rows.Add(myControlsRow)
                                        End If
                                    Next myTestControlsRow

                                    'Confirm changes done in the entry DataSet
                                    pWSResultDS.AcceptChanges()
                                End If
                            End If
                        End If
                    Next selectedTestsRow
                    lstWSCtrlDS = Nothing
                    bTestTypeIconSTD = Nothing
                    bTestTypeIconISE = Nothing
                End If

                If (Not resultData.HasError) Then
                    'SA 26/03/2013 - Rewrite value of CreationOrder but only when function is NOT used for processing LIS Order Tests
                    If (Not pFromLIS) Then SortControls(pWSResultDS)

                    If (Not pForLoadSavedWS) Then
                        If (pSampleClass = "CTRL") Then
                            'When the function has been called for adding Control Samples, add the needed Calibrators and Blanks only for those 
                            'TestType/TestID/SampleType for which Controls were added

                            'JCM 16-05-2013 Read all TestType/TestID/SampleType marked as Added
                            Dim dumyList As List(Of SelectedTestsDS.SelectedTestTableRow) = (From a As SelectedTestsDS.SelectedTestTableRow In pSelectedTestsDS.SelectedTestTable _
                                                                                            Where a.Added = True _
                                                                                            AndAlso a.TestType = "STD" _
                                                                                           Select a).ToList()
                            Dim pSelectedTestsWithControlsAddedDS As New SelectedTestsDS
                            For Each dr As SelectedTestsDS.SelectedTestTableRow In dumyList
                                pSelectedTestsWithControlsAddedDS.SelectedTestTable.ImportRow(dr)
                            Next

                            'JCM 16-05-2013 Add Calibrators and Blanks only for Standard Test/SampleTypes marked as Added
                            If (pSelectedTestsWithControlsAddedDS.SelectedTestTable.Rows.Count > 0) Then
                                'Add the needed Calibrators and Blanks
                                resultData = AddCalibratorOrderTests(pSelectedTestsWithControlsAddedDS, pWSResultDS, pAnalyzerID, pForLoadSavedWS, pFromLIS)
                            End If
                        Else
                            'When the function has been called after adding Patient Samples, add the needed Calibrators and Blanks always, no matter if 
                            'Controls were not added for none of the TestType/TestID/SampleType
                            resultData = AddCalibratorOrderTests(pSelectedTestsDS, pWSResultDS, pAnalyzerID, pForLoadSavedWS, pFromLIS)
                        End If
                    Else
                        'Return the updated DS
                        resultData.SetDatos = pWSResultDS
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.AddControlOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used in the screen of WSPreparation to manage the adding/deleting of Calibrator Order Tests
        ''' (only in the screen DataSet, this function does not changes in DataBase)
        ''' </summary>
        ''' <param name="pSelectedTestsDS">Typed DataSet containing the list of Test/SampleType for which a Calibrator 
        '''                                Order Test has been requested</param>
        ''' <param name="pWSResultDS">Typed DataSet containing in the table of Blanks and Calibrators the list of 
        '''                           currently requested Calibrator Order Tests</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pForLoadSavedWS">Flag to avoid calling functions to get additional data for Tests (according the TestType) and the needed Blanks 
        '''                               when this function is used for loading a Saved WS. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of Blanks and Calibrators
        '''          updated with the added Calibrator Order Tests</returns>
        ''' <remarks>
        ''' Created by:  GDS
        ''' Modified by: SA  03/03/2010 - Added parameter for the Analyzer Identifier (needed to get Last Blank results)
        '''              SA  26/03/2010 - Changes to set value for field CreationOrder: Calibrator Order Tests are sorted in the same order 
        '''                               they are requested, but grouped by the Calibrator they need
        '''              SA  24/11/2010 - For Calibrator Order Tests having previous results, inform fields OriginalFactorValue
        '''                               and FactorValue in the following way: if ManualResultFlag is False, get value of 
        '''                               CalibratorFactor; otherwise, get value of ManualResult
        '''              SA  18/02/2011 - Some changes to improve the code (use AndAlso instead of And; remove unneeded "New")
        '''              SA  22/02/2011 - Added optional parameter pForLoadSavedWS to avoid calling function to get the needed Blanks 
        '''                               when this function is used for loading a Saved WS
        '''              SA  23/05/2011 - Changed the building of the string with the last result of each point of a Calibrator: it is needed
        '''                               to control that all results belong to the same Calibrator (they have the same OrderTestID)
        '''              SA  26/03/2013 - Added changes needed for new LIS implementation: new optional parameter pFromLIS Boolean with default value False. 
        '''                               When pFromLIS = TRUE: 
        '''                               ** Do not search the icon for Calibrators
        '''                               ** Set field Selected = False
        '''                               ** Do not call function SortCalibrators
        '''                               ** Inform the parameter when calling function AddBlankOrderTests
        ''' </remarks>
        Public Function AddCalibratorOrderTests(ByVal pSelectedTestsDS As SelectedTestsDS, ByVal pWSResultDS As WorkSessionResultDS, ByVal pAnalyzerID As String, _
                                                Optional ByVal pForLoadSavedWS As Boolean = False, Optional ByVal pFromLIS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO

            Try
                'Get the default tube used for Calibrators
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                resultData = myUserSettingsDelegate.GetDefaultSampleTube(Nothing, "CALIB")

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim defaultTubeCalib As String = DirectCast(resultData.SetDatos, String)

                    'Get maximum number of days that can have passed from the last Blank or Calibrator execution
                    Dim maxDaysPreviousBlkCalib As String = ""
                    Dim myGeneralSettingsDelegate As New GeneralSettingsDelegate
                    resultData = myGeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_DAYS_PREVIOUS_BLK_CALIB.ToString)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        maxDaysPreviousBlkCalib = DirectCast(resultData.SetDatos, String)
                    End If

                    'SA 26/03/2013 - When function is used for processing LIS Order Tests, the icon is not obtained
                    Dim bArrImage As Byte() = Nothing
                    If (Not pFromLIS) Then
                        'Get the Calibrator Icon image byte array
                        Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                        bArrImage = preloadedDataConfig.GetIconImage("CALIB")
                    End If

                    'Verify if there are Calibrators in the list of Blanks and Calibrators to get the next creation order
                    Dim lstCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                       Where a.SampleClass = "CALIB" _
                                      Select a).ToList()

                    Dim nextCreationOrder As Integer = lstCalibratorsDS.Count + 1

                    'Declare all variables needed in the FOR loop
                    Dim testID As Integer = -1
                    Dim calibratorID As Integer = -1
                    Dim myLastCalibOTID As Integer = -1
                    Dim testType As String = String.Empty
                    Dim sampleType As String = String.Empty
                    Dim tubeTypeToAssign As String = String.Empty

                    Dim numPoints As Integer = 0
                    Dim pointResults As String = ""

                    Dim myTestSampleCalibratorDS As TestSampleCalibratorDS
                    Dim myTestCalibratorsDelegate As New TestCalibratorsDelegate

                    Dim myResultsDelegate As New ResultsDelegate
                    Dim myHisResultsDelegate As New HisWSResultsDelegate
                    Dim myAdditionalElementsTable As WSAdditionalElementsDS.WSAdditionalElementsTableDataTable

                    Dim lstWSResultDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    Dim myBlankCalibratorsRow As WorkSessionResultDS.BlankCalibratorsRow
                    Dim myBlankCalibratorsDataTable As WorkSessionResultDS.BlankCalibratorsDataTable

                    'Test if all Calibrator Order Tests in pSelectedTestsDS have been processed
                    For Each myTableRow As SelectedTestsDS.SelectedTestTableRow In pSelectedTestsDS.SelectedTestTable
                        testType = myTableRow.TestType
                        testID = myTableRow.TestID

                        If (testType = "STD") Then
                            'Get data of the Calibrator needed for the specified Test and Sample Type
                            resultData = myTestCalibratorsDelegate.GetTestCalibratorData(Nothing, myTableRow.TestID, myTableRow.SampleType)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myTestSampleCalibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                                If (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count > 0) Then
                                    'Theorical Factor Calibrators are ignored...
                                    If (myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorType <> "FACTOR") Then
                                        'Verify if there is already a Calibrator Order Test for the selected Test and the SampleType
                                        'using an Experimental Calibrator 
                                        sampleType = myTestSampleCalibratorDS.tparTestCalibrators(0).SampleType

                                        lstWSResultDS = (From a In pWSResultDS.BlankCalibrators _
                                                        Where a.SampleClass = "CALIB" _
                                                      AndAlso a.TestType = testType _
                                                      AndAlso a.TestID = testID _
                                                      AndAlso a.SampleType = sampleType _
                                                       Select a).ToList()

                                        'If there is not a Calibrator Order Test for the selected Test and Sample Type
                                        If (lstWSResultDS.Count = 0) Then
                                            'Verify if the needed Calibrator is used for Tests already added to get the TubeType currently set for it
                                            tubeTypeToAssign = defaultTubeCalib
                                            calibratorID = myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorID

                                            Dim selectedTubeType = (From q In pWSResultDS.BlankCalibrators _
                                                                   Where q.SampleClass = "CALIB" _
                                                                 AndAlso (Not q.IsCalibratorIDNull AndAlso q.CalibratorID = calibratorID) _
                                                                  Select q.TubeType).Distinct()
                                            If (selectedTubeType.Count = 1) Then
                                                tubeTypeToAssign = selectedTubeType(0).ToString

                                                'Get the max value of CreationOrder for Order Tests using the same Calibrator 
                                                Dim q = (From p In pWSResultDS.BlankCalibrators _
                                                        Where p.SampleClass = "CALIB" _
                                                      AndAlso (Not p.IsCalibratorIDNull AndAlso p.CalibratorID = calibratorID) _
                                                       Select p.CreationOrder).Max

                                                nextCreationOrder = q + 1
                                            End If

                                            myBlankCalibratorsDataTable = DirectCast(pWSResultDS.BlankCalibrators, WorkSessionResultDS.BlankCalibratorsDataTable)
                                            myBlankCalibratorsRow = DirectCast(myBlankCalibratorsDataTable.NewRow(), WorkSessionResultDS.BlankCalibratorsRow)

                                            'Insert the new Calibrator Order Tests
                                            myBlankCalibratorsRow.Selected = Convert.ToBoolean(IIf(pFromLIS, False, True))
                                            myBlankCalibratorsRow.SampleClass = "CALIB"

                                            'Get the Calibrator Icon image byte array
                                            If (Not pFromLIS) Then myBlankCalibratorsRow.SampleClassIcon = bArrImage

                                            myBlankCalibratorsRow.TestType = testType
                                            myBlankCalibratorsRow.TestID = testID
                                            myBlankCalibratorsRow.TestName = myTestSampleCalibratorDS.tparTestCalibrators(0).TestName
                                            myBlankCalibratorsRow.RequestedSampleTypes = sampleType
                                            myBlankCalibratorsRow.SampleType = myTestSampleCalibratorDS.tparTestCalibrators(0).SampleType
                                            myBlankCalibratorsRow.NumReplicates = myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorReplicates
                                            myBlankCalibratorsRow.TubeType = tubeTypeToAssign
                                            myBlankCalibratorsRow.CalibratorID = myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorID
                                            myBlankCalibratorsRow.CalibratorName = myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorName
                                            myBlankCalibratorsRow.LotNumber = myTestSampleCalibratorDS.tparTestCalibrators(0).LotNumber
                                            myBlankCalibratorsRow.NumberOfCalibrators = myTestSampleCalibratorDS.tparTestCalibrators(0).NumberOfCalibrators
                                            myBlankCalibratorsRow.OTStatus = "OPEN"


                                            'Search if there are recent results for the Calibrator executed for the same Test and Sample Type
                                            resultData = myResultsDelegate.GetLastExecutedCalibrator(Nothing, testID, myTestSampleCalibratorDS.tparTestCalibrators(0).SampleType, _
                                                                                                     myTestSampleCalibratorDS.tparTestCalibrators(0).TestVersionNumber, _
                                                                                                     pAnalyzerID, maxDaysPreviousBlkCalib)

                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myAdditionalElementsTable = DirectCast(resultData.SetDatos, WSAdditionalElementsDS).WSAdditionalElementsTable

                                                If (myAdditionalElementsTable.Rows.Count > 0) Then
                                                    'Get the OrderTestID of the last Calibrator
                                                    myLastCalibOTID = myAdditionalElementsTable(0).PreviousOrderTestID

                                                    'Create the string containing the result for each Calibrator point
                                                    pointResults = ""
                                                    numPoints = 0
                                                    For Each myAdditionalElementsTableRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow In myAdditionalElementsTable.Rows
                                                        If (myAdditionalElementsTableRow.PreviousOrderTestID = myLastCalibOTID) Then
                                                            pointResults += "|" + myAdditionalElementsTableRow.ABSValue.ToString
                                                            numPoints += 1
                                                        Else
                                                            Exit For
                                                        End If
                                                    Next
                                                    pointResults = pointResults.Substring(1).Replace("|", Environment.NewLine)

                                                    myBlankCalibratorsRow.PreviousOrderTestID = myAdditionalElementsTable(0).PreviousOrderTestID
                                                    myBlankCalibratorsRow.ABSValue = pointResults
                                                    myBlankCalibratorsRow.ABSDateTime = myAdditionalElementsTable(0).ResultDateTime

                                                    myBlankCalibratorsRow.NewCheck = False
                                                    myBlankCalibratorsRow.Selected = False

                                                    If (numPoints = 1) Then
                                                        'Only for Single Point Calibrators, the Factor is informed
                                                        If (Not myAdditionalElementsTable(0).IsCalibratorFactorNull) Then
                                                            myBlankCalibratorsRow.OriginalFactorValue = myAdditionalElementsTable(0).CalibratorFactor
                                                            myBlankCalibratorsRow.FactorValue = myAdditionalElementsTable(0).CalibratorFactor

                                                            If (myAdditionalElementsTable(0).ManualResultFlag) Then
                                                                'Factor Value has been manually changed
                                                                If (Not myAdditionalElementsTable(0).IsManualResultNull) Then
                                                                    myBlankCalibratorsRow.FactorValue = myAdditionalElementsTable(0).ManualResult
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                Else
                                                    myBlankCalibratorsRow.NewCheck = True
                                                    myBlankCalibratorsRow.Selected = Convert.ToBoolean(IIf(pFromLIS, False, True))
                                                End If
                                            Else
                                                'Error searching if there is a previous result calculated for the Calibrator of the Test/SampleType
                                                Exit For
                                            End If

                                            'Is there an additional row in TestSampleCalibratorDS ... Alternative Calibrator is used for the
                                            'selected Test and SampleType
                                            If (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count > 1) Then
                                                'Include the SampleType in field RequestedSampleType
                                                myBlankCalibratorsRow.RequestedSampleTypes = myTestSampleCalibratorDS.tparTestCalibrators(1).SampleType
                                            End If

                                            myBlankCalibratorsRow.CreationOrder = nextCreationOrder
                                            myBlankCalibratorsDataTable.Rows.Add(myBlankCalibratorsRow)

                                            'Increment value of the last nextCreationOrder
                                            nextCreationOrder += 1
                                        Else
                                            'Is there an additional row in TestSampleCalibratorDS ... Alternative Calibrator is used for the
                                            'selected Test and SampleType
                                            If (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count > 1) Then
                                                'The Calibrator was not used as Alternative for any other SampleType
                                                If (lstWSResultDS(0).RequestedSampleTypes = lstWSResultDS(0).SampleType) Then
                                                    lstWSResultDS(0).RequestedSampleTypes = ""
                                                End If

                                                'Verify if the SampleType is already included in the list of RequestedSampleTypes
                                                If (lstWSResultDS(0).RequestedSampleTypes.IndexOf(myTestSampleCalibratorDS.tparTestCalibrators(1).SampleType) = -1) Then
                                                    'Include the SampleType in field RequestedSampleType
                                                    lstWSResultDS(0).RequestedSampleTypes += " " + myTestSampleCalibratorDS.tparTestCalibrators(1).SampleType
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Else
                                'Error getting the definition of Calibration parameters for the selected Test/SampleType 
                                Exit For
                            End If
                        End If
                    Next

                    'TR 04/08/2011 -Set value to nothing 
                    lstCalibratorsDS = Nothing
                    lstWSResultDS = Nothing
                    bArrImage = Nothing

                    If (Not resultData.HasError) Then
                        'SA 26/03/2013 - Rewrite value of CreationOrder but only when function is NOT used for processing LIS Order Tests
                        If (Not pFromLIS) Then SortCalibrators(pWSResultDS)

                        'Confirm the changes in Blanks and Calibrators Table
                        pWSResultDS.BlankCalibrators.AcceptChanges()

                        If (Not pForLoadSavedWS) Then
                            'Add the needed Blanks
                            resultData = AddBlankOrderTests(pSelectedTestsDS, pWSResultDS, pAnalyzerID, pFromLIS)
                        Else
                            'Return the updated DS
                            resultData.SetDatos = pWSResultDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.AddCalibratorOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used in the screen of WSPreparation to manage the adding/deleting of Patient Order Tests
        ''' (only in the screen DataSet, this function does not changes in DataBase)
        ''' </summary>
        ''' <param name="pSelectedTestsDS">Typed DataSet containing the list of Test/SampleType for which a Patient Order Test has been requested</param>
        ''' <param name="pWSResultDS">Typed DataSet containing in the table of Patients the list of currently requested Patient Order Tests</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSampleID">Sample Identifier. When parameter pSampleIDType has value "AUTO", it has not value</param>
        ''' <param name="pStatFlag">Flag indicating if the Patient Order Tests are for Stat (when True) or for Routine (when False)</param>
        ''' <param name="pSampleIDType">Type of Sample Identifier: DB (existing PatientID); MANUAL (informed by User) or AUTO (automatically generated)</param>
        ''' <param name="pNumOrders">Number of Orders that has to be created (with the same Order Tests). Optional parameter informed only when 
        '''                          parameter pSampleIDType has value "AUTO"; in any other case, just one Order Test is created</param>
        ''' <param name="pLastSeq">Last Sequence Number that has been generated for automatic PatientIDs created. Optional parameter 
        '''                        informed only when parameter pSampleIDType has value "AUTO"</param>
        ''' <param name="pLastDate">Last Date for which automatic PatientIDs have been created. Optional parameter informed only when parameter
        '''                         pSampleIDType has value "AUTO"</param>
        ''' <param name="pForLoadSavedWS">Flag to avoid calling functions to get additional data for Tests (according the TestType) and the needed Controls, 
        '''                               Calibrators and Blanks when this function is used for loading a Saved WS. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of Patients updated with the added Patient Order Tests</returns>
        ''' <remarks>
        ''' Created by:  SA 03/03/2010
        ''' Modified by: SA 25/03/2010 - Changed the way of generate the first automatic SampleID; assign Sequence=0001 instead of read the last 
        '''                              generated OrderID from the DB. Add character # as prefix for automatic SampleIDs
        '''              SA 26/03/2010 - Changes to set value for field CreationOrder: Patient Order Tests are sorted in the same order 
        '''                              they are requested, but grouped by the PatientID/SampleID, StatFlag and SampleType
        '''              SA 11/05/2010 - Changes to manage adding of Calculated Tests
        '''              SA 21/10/2010 - Changes to manage adding of ISE Tests and Profiles with Tests of several Test Types
        '''              DL 29/11/2010 - Changes to manage adding of OFF-SYSTEM Tests
        '''              SA 02/12/2010 - Name of ICON for Off-System Tests was bad written. For Calculated and Off-System Tests, do not inform field
        '''                              NumReplicates
        '''              SA 18/01/2011 - For Off-System Tests, set field Selected as False
        '''              SA 18/02/2011 - Some changes to improve the code (use AndAlso instead of And; remove unneeded "New")
        '''              SA 22/02/2011 - Added optional parameter pForLoadSavedWS to avoid calling functions to get additional data for Tests (according
        '''                              the TestType) and the needed Controls, Calibrators and Blanks when this function is used for loading a Saved WS
        '''              SA 23/06/2011 - When search the TubeType to assign, LINQ has to be filtered by TestType STD or ISE (CALC and OFFS Tests do not have Tube)
        '''              TR 04/08/2011 - Set to nothing the value of variables used for Icons and LINQ sentences to release memory 
        '''              SA 17/10/2011 - SampleID comparations have to be done using UpperCase 
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        '''              SA 25/04/2013 - When pForLoadSavedWS=TRUE, inform all LIS fields informed in the SelectedTestsDS row in process when it is load to Patients 
        '''                              table in pWSResultsDS
        '''              TR 17/05/2013 - Inform optional parameter for the SampleClass when calling function AddControlOrderTests
        '''              SA 29/08/2013 - For each manual Order Tests to add, verify if there is at least an Order Test requested by an external LIS system for the same
        '''                              Patient and SampleType, and in this case, get value of the SpecimenID and assign it to the Order Test to add
        '''              SA 11/03/2014 - BT #1540 ==> For each manual Order Tests to add to an existing Order, get the PatientIDType of the Order PatientID/SampleID
        '''                                           (in the same way it is done for TubeType and SpecimenID)
        ''' </remarks>
        Public Function AddPatientOrderTests(ByVal pSelectedTestsDS As SelectedTestsDS, ByVal pWSResultDS As WorkSessionResultDS, ByVal pAnalyzerID As String, _
                                             ByVal pSampleID As String, ByVal pStatFlag As Boolean, ByVal pSampleIDType As String, _
                                             Optional ByVal pNumOrders As Integer = 1, Optional ByVal pLastSeq As Integer = 0, _
                                             Optional ByVal pLastDate As String = "", Optional ByVal pForLoadSavedWS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO

            Try
                Dim nextCreationOrder As Integer = 1

                'Get the tube used by default for Patient Samples
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                resultData = myUserSettingsDelegate.GetDefaultSampleTube(Nothing, "PATIENT")

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim defaultTubePatient As String = DirectCast(resultData.SetDatos, String)

                    'If a SampleID is not informed, it means it has to be generated automatically		
                    If String.Equals(pSampleIDType, "AUTO") AndAlso String.Equals(pSampleID.Trim, String.Empty) Then
                        If (pLastSeq > 0) Then
                            'Increment the Sequence Number for the OrderID and build the new OrderID using the current date and the Sequence
                            pLastSeq += 1
                        Else
                            pLastSeq = 1
                        End If
                        pSampleID = "#" & pLastDate & Format(pLastSeq, "0000")

                        'Set value of field CreationOrder...
                        nextCreationOrder = pWSResultDS.Patients.Rows.Count + 1
                    Else
                        If (pWSResultDS.Patients.Rows.Count > 0) Then
                            'Get the next CreationOrder for the specified PatientID/SampleID, StatFlag and SampleType
                            nextCreationOrder = pWSResultDS.Patients.Rows.Count + 1
                        End If
                    End If

                    If (Not resultData.HasError) Then
                        Dim myPatientsDataTable As WorkSessionResultDS.PatientsDataTable
                        myPatientsDataTable = DirectCast(pWSResultDS.Patients, WorkSessionResultDS.PatientsDataTable)

                        'Get Icon Image byte array for Patient's Priority and Test Type
                        Dim bPatientStat As Byte() = Nothing
                        Dim bPatientNotStat As Byte() = Nothing
                        Dim bTestTypeIconSTD As Byte() = Nothing
                        Dim bTestTypeIconCALC As Byte() = Nothing
                        Dim bTestTypeIconISE As Byte() = Nothing
                        Dim bTestTypeIconOFF As Byte() = Nothing
                        Dim preloadedDataConfig As New PreloadedMasterDataDelegate

                        bPatientStat = preloadedDataConfig.GetIconImage("STATS")
                        bPatientNotStat = preloadedDataConfig.GetIconImage("ROUTINES")
                        bTestTypeIconSTD = preloadedDataConfig.GetIconImage("FREECELL")
                        bTestTypeIconCALC = preloadedDataConfig.GetIconImage("TCALC")
                        bTestTypeIconISE = preloadedDataConfig.GetIconImage("TISE_SYS")
                        bTestTypeIconOFF = preloadedDataConfig.GetIconImage("TOFF_SYS")

                        For Each selectedOrderTestRow In pSelectedTestsDS.SelectedTestTable
                            'Get the SampleID, StatFlag, SampleType, TestType and TestID of the current row
                            Dim sampleID As String = pSampleID
                            Dim statFlag As Boolean = pStatFlag
                            Dim sampleType As String = selectedOrderTestRow.SampleType
                            Dim testType As String = selectedOrderTestRow.TestType
                            Dim testID As Integer = selectedOrderTestRow.TestID

                            'Verify if there is already a Patient Order for the SampleID/StatFlag
                            Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                            lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                              Where a.SampleClass = "PATIENT" _
                                            AndAlso a.SampleID = sampleID _
                                            AndAlso a.StatFlag = statFlag _
                                            AndAlso (Not a.IsOrderIDNull) _
                                             Select a).ToList()
                            'AndAlso a.SampleID.ToUpper = sampleID.ToUpper _

                            Dim currentOrderID As String = ""
                            If (lstWSPatientsDS.Count > 0) Then
                                'All new Order Tests have to be linked to the same existing Order 
                                currentOrderID = lstWSPatientsDS(0).OrderID

                                'Update the entry parameter pSampleID with value of the existing in DB to be sure all records have the same Case
                                pSampleID = lstWSPatientsDS(0).SampleID
                            End If

                            'Verify if there is already a Patient Order Test for the selected Test and SampleType for the SampleID/StatFlag
                            lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                              Where a.SampleClass = "PATIENT" _
                                            AndAlso a.SampleID = sampleID _
                                            AndAlso a.StatFlag = statFlag _
                                            AndAlso a.TestType = testType _
                                            AndAlso a.SampleType = sampleType _
                                            AndAlso a.TestID = testID _
                                             Select a).ToList()
                            'AndAlso a.SampleID.ToUpper = sampleID.ToUpper _

                            If (lstWSPatientsDS.Count = 0) Then
                                If (Not pForLoadSavedWS) Then
                                    'Get Test Name and Number of Replicates
                                    If String.Equals(testType, "STD") Then
                                        Dim myTestsDelegate As New TestsDelegate
                                        resultData = myTestsDelegate.Read(Nothing, testID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim testDataDS As TestsDS = DirectCast(resultData.SetDatos, TestsDS)

                                            'Update fields for TestName and Number of Replicates in the entry DataSet
                                            selectedOrderTestRow.TestName = testDataDS.tparTests(0).TestName
                                            selectedOrderTestRow.NumReplicates = testDataDS.tparTests(0).ReplicatesNumber
                                        End If

                                    ElseIf (testType = "CALC") Then
                                        Dim myCalcTestDelegate As New CalculatedTestsDelegate
                                        resultData = myCalcTestDelegate.GetCalcTest(Nothing, testID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim testDataDS As CalculatedTestsDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                                            'Update fields for TestName and Formula in the entry DataSet
                                            selectedOrderTestRow.TestName = testDataDS.tparCalculatedTests(0).CalcTestLongName
                                            selectedOrderTestRow.CalcTestFormula = "=" & testDataDS.tparCalculatedTests(0).FormulaText
                                        End If

                                    ElseIf (testType = "ISE") Then
                                        Dim myISETestsDelegate As New ISETestsDelegate
                                        resultData = myISETestsDelegate.Read(Nothing, testID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim iseTestDataDS As ISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)

                                            'Update fields for ISETestName and Number of Replicates in the entry DataSet
                                            selectedOrderTestRow.TestName = iseTestDataDS.tparISETests(0).Name
                                            selectedOrderTestRow.NumReplicates = 1
                                        End If

                                    ElseIf (testType = "OFFS") Then
                                        Dim myOffSystemTestsDelegate As New OffSystemTestsDelegate
                                        resultData = myOffSystemTestsDelegate.Read(Nothing, testID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myOffSystemTestsDS As OffSystemTestsDS = DirectCast(resultData.SetDatos, OffSystemTestsDS)

                                            'Update fields for OffSystem Test Name in the entry DataSet
                                            selectedOrderTestRow.TestName = myOffSystemTestsDS.tparOffSystemTests(0).Name
                                        End If
                                    End If
                                End If

                                If (Not resultData.HasError) Then
                                    'Verify if there are already Order Tests for the same Patient and SampleType to get the TubeType currently set for it
                                    Dim tubeTypeToAssign As String = defaultTubePatient
                                    Dim selectedTubeType = (From x In pWSResultDS.Patients _
                                                           Where x.SampleClass = "PATIENT" _
                                                         AndAlso (x.TestType = "STD" OrElse x.TestType = "ISE") _
                                                         AndAlso x.SampleID = sampleID _
                                                         AndAlso x.SampleType = sampleType _
                                                          Select x.TubeType).Distinct()
                                    If (selectedTubeType.Count = 1) Then tubeTypeToAssign = selectedTubeType(0).ToString

                                    'SA 29/08/2013 - See comments about this code in the function header
                                    'Verify if there are already Order Tests requested for LIS for the same Patient and SampleType to get the SpecimenID (Barcode) for it
                                    Dim sampleBCToAssign As String = String.Empty
                                    Dim selectedPatientSample As List(Of String) = (From x In pWSResultDS.Patients _
                                                                                   Where x.SampleClass = "PATIENT" _
                                                                                 AndAlso x.SampleID = sampleID _
                                                                                 AndAlso x.SampleType = sampleType _
                                                                                 AndAlso x.LISRequest _
                                                                             AndAlso Not x.IsSpecimenIDNull _
                                                                                  Select x.SpecimenID).Distinct.ToList
                                    If (selectedPatientSample.Count > 0) Then sampleBCToAssign = selectedPatientSample(0).ToString

                                    'BT #1540 - Get the type of Patient of the existing Order (DB/MAN)
                                    Dim patientIDTypeToAssign As String = pSampleIDType
                                    selectedPatientSample = (From x In pWSResultDS.Patients _
                                                            Where x.SampleClass = "PATIENT" _
                                                          AndAlso x.SampleID = sampleID _
                                                          AndAlso x.SampleType = sampleType _
                                                          AndAlso x.LISRequest _
                                                      AndAlso Not x.IsSampleIDTypeNull _
                                                           Select x.SampleIDType).Distinct.ToList
                                    If (selectedPatientSample.Count > 0) Then patientIDTypeToAssign = selectedPatientSample(0).ToString

                                    'Insert the new Patient Order Test in Patients table
                                    Dim myPatientsRow As WorkSessionResultDS.PatientsRow
                                    myPatientsRow = DirectCast(myPatientsDataTable.NewRow(), WorkSessionResultDS.PatientsRow)

                                    myPatientsRow.SampleClass = "PATIENT"
                                    myPatientsRow.OrderID = currentOrderID
                                    myPatientsRow.StatFlag = pStatFlag

                                    If (pStatFlag) Then
                                        myPatientsRow.SampleClassIcon = bPatientStat
                                    Else
                                        myPatientsRow.SampleClassIcon = bPatientNotStat
                                    End If

                                    myPatientsRow.TestType = testType
                                    myPatientsRow.Selected = (myPatientsRow.TestType <> "OFFS")

                                    If (myPatientsRow.TestType = "STD") Then
                                        myPatientsRow.TestTypeIcon = bTestTypeIconSTD

                                        myPatientsRow.NumReplicates = selectedOrderTestRow.NumReplicates
                                        myPatientsRow.TubeType = tubeTypeToAssign

                                    ElseIf (myPatientsRow.TestType = "CALC") Then
                                        myPatientsRow.TestTypeIcon = bTestTypeIconCALC

                                    ElseIf (myPatientsRow.TestType = "ISE") Then
                                        myPatientsRow.TestTypeIcon = bTestTypeIconISE

                                        myPatientsRow.NumReplicates = selectedOrderTestRow.NumReplicates
                                        myPatientsRow.TubeType = tubeTypeToAssign

                                    ElseIf (myPatientsRow.TestType = "OFFS") Then
                                        myPatientsRow.TestTypeIcon = bTestTypeIconOFF
                                    End If

                                    'SA 29/08/2013 - See comments about this code in the function header
                                    If (sampleBCToAssign.Trim <> String.Empty) Then
                                        myPatientsRow.SpecimenID = sampleBCToAssign.Trim
                                    Else
                                        myPatientsRow.SetSpecimenIDNull()
                                    End If

                                    'BT #1540 - Assign as SampleIDType the patientIDTypeToAssign instead of the entry parameter pSampleIDType 
                                    myPatientsRow.SampleIDType = patientIDTypeToAssign

                                    myPatientsRow.SampleID = pSampleID
                                    myPatientsRow.SampleType = sampleType
                                    myPatientsRow.TestID = testID
                                    myPatientsRow.TestName = selectedOrderTestRow.TestName
                                    myPatientsRow.CreationOrder = nextCreationOrder

                                    If (Not selectedOrderTestRow.IsTestProfileIDNull) Then
                                        myPatientsRow.TestProfileID = selectedOrderTestRow.TestProfileID
                                        myPatientsRow.TestProfileName = selectedOrderTestRow.TestProfileName
                                    End If

                                    If (selectedOrderTestRow.TestType = "CALC") Then
                                        'Inform the Test Formula...
                                        myPatientsRow.CalcTestFormula = selectedOrderTestRow.CalcTestFormula
                                    End If

                                    If (Not selectedOrderTestRow.IsCalcTestIDsNull AndAlso selectedOrderTestRow.CalcTestIDs.Trim <> "") Then
                                        'If the Selected Test is included in one or more Calculated Tests, the correspondent fields are informed
                                        myPatientsRow.CalcTestID = selectedOrderTestRow.CalcTestIDs
                                        myPatientsRow.CalcTestName = selectedOrderTestRow.CalcTestNames
                                    End If

                                    myPatientsRow.OTStatus = "OPEN"

                                    'Inform also LIS Values when they are informed...
                                    If (selectedOrderTestRow.IsLISRequestNull OrElse Not selectedOrderTestRow.LISRequest) Then
                                        myPatientsRow.LISRequest = False
                                        myPatientsRow.LISRequestIcon = bTestTypeIconSTD
                                    Else
                                        myPatientsRow.LISRequest = True
                                        myPatientsRow.LISRequestIcon = preloadedDataConfig.GetIconImage("LISREQUEST")
                                    End If
                                    myPatientsRow.LISRequest = selectedOrderTestRow.LISRequest

                                    myPatientsRow.ExternalQC = False
                                    If (Not selectedOrderTestRow.IsExternalQCNull) Then myPatientsRow.ExternalQC = selectedOrderTestRow.ExternalQC

                                    If (Not selectedOrderTestRow.IsAwosIDNull) Then myPatientsRow.AwosID = selectedOrderTestRow.AwosID
                                    If (Not selectedOrderTestRow.IsSpecimenIDNull) Then myPatientsRow.SpecimenID = selectedOrderTestRow.SpecimenID
                                    If (Not selectedOrderTestRow.IsESPatientIDNull) Then myPatientsRow.ESPatientID = selectedOrderTestRow.ESPatientID
                                    If (Not selectedOrderTestRow.IsLISPatientIDNull) Then myPatientsRow.LISPatientID = selectedOrderTestRow.LISPatientID
                                    If (Not selectedOrderTestRow.IsESOrderIDNull) Then myPatientsRow.ESOrderID = selectedOrderTestRow.ESOrderID
                                    If (Not selectedOrderTestRow.IsLISOrderIDNull) Then myPatientsRow.LISOrderID = selectedOrderTestRow.LISOrderID

                                    myPatientsDataTable.Rows.Add(myPatientsRow)
                                    nextCreationOrder += 1
                                Else
                                    'Error getting TestName and Number of Replicates
                                    Exit For
                                End If
                            Else
                                'Update value of Test Profile fields (ID and Name)
                                If (selectedOrderTestRow.IsTestProfileIDNull) Then
                                    lstWSPatientsDS(0).SetTestProfileIDNull()
                                    lstWSPatientsDS(0).SetTestProfileNameNull()
                                Else
                                    lstWSPatientsDS(0).TestProfileID = selectedOrderTestRow.TestProfileID
                                    lstWSPatientsDS(0).TestProfileName = selectedOrderTestRow.TestProfileName
                                End If

                                'Update values of fields used to link the selected Test with one or more Calculated Tests
                                If (selectedOrderTestRow.IsCalcTestIDsNull Or selectedOrderTestRow.CalcTestIDs.Trim = "") Then
                                    lstWSPatientsDS(0).SetCalcTestIDNull()
                                    lstWSPatientsDS(0).SetCalcTestNameNull()
                                Else
                                    lstWSPatientsDS(0).CalcTestID = selectedOrderTestRow.CalcTestIDs
                                    lstWSPatientsDS(0).CalcTestName = selectedOrderTestRow.CalcTestNames
                                End If
                            End If
                        Next

                        If (Not resultData.HasError) Then
                            If (pSampleIDType = "AUTO" AndAlso pNumOrders > 1) Then
                                For i As Integer = 1 To pNumOrders - 1
                                    'Increment the Sequence Number for the OrderID and build the new OrderID using the current date and the Sequence
                                    pLastSeq += 1
                                    pSampleID = "#" & pLastDate & Format(pLastSeq, "0000")

                                    For Each selectedOrderTestRow In pSelectedTestsDS.SelectedTestTable
                                        'Increment the CreationOrder
                                        nextCreationOrder += 1

                                        'Insert the new Patient Order Test in Patients table
                                        Dim myPatientsRow As WorkSessionResultDS.PatientsRow
                                        myPatientsRow = DirectCast(myPatientsDataTable.NewRow(), WorkSessionResultDS.PatientsRow)

                                        myPatientsRow.SampleClass = "PATIENT"
                                        myPatientsRow.StatFlag = pStatFlag

                                        If (pStatFlag) Then
                                            myPatientsRow.SampleClassIcon = bPatientStat
                                        Else
                                            myPatientsRow.SampleClassIcon = bPatientNotStat
                                        End If

                                        myPatientsRow.TestType = selectedOrderTestRow.TestType
                                        myPatientsRow.Selected = (myPatientsRow.TestType <> "OFFS")

                                        If (myPatientsRow.TestType = "STD") Then
                                            myPatientsRow.TestTypeIcon = bTestTypeIconSTD
                                            myPatientsRow.NumReplicates = selectedOrderTestRow.NumReplicates
                                            myPatientsRow.TubeType = defaultTubePatient

                                        ElseIf (myPatientsRow.TestType = "CALC") Then
                                            myPatientsRow.TestTypeIcon = bTestTypeIconCALC

                                        ElseIf (myPatientsRow.TestType = "ISE") Then
                                            myPatientsRow.TestTypeIcon = bTestTypeIconISE
                                            myPatientsRow.NumReplicates = 1
                                            myPatientsRow.TubeType = defaultTubePatient

                                        ElseIf (myPatientsRow.TestType = "OFFS") Then
                                            myPatientsRow.TestTypeIcon = bTestTypeIconOFF
                                        End If

                                        'TR 12/03/2013 Set the ListRequestIcon as clear.
                                        myPatientsRow.LISRequestIcon = bTestTypeIconSTD

                                        myPatientsRow.SampleIDType = pSampleIDType
                                        myPatientsRow.SampleID = pSampleID
                                        myPatientsRow.SampleType = selectedOrderTestRow.SampleType
                                        myPatientsRow.TestID = selectedOrderTestRow.TestID
                                        myPatientsRow.TestName = selectedOrderTestRow.TestName
                                        myPatientsRow.CreationOrder = nextCreationOrder

                                        If (Not selectedOrderTestRow.IsTestProfileIDNull) Then
                                            myPatientsRow.TestProfileID = selectedOrderTestRow.TestProfileID
                                            myPatientsRow.TestProfileName = selectedOrderTestRow.TestProfileName
                                        End If

                                        If (selectedOrderTestRow.TestType = "CALC") Then
                                            'Inform the Test Formula
                                            myPatientsRow.CalcTestFormula = selectedOrderTestRow.CalcTestFormula
                                        End If

                                        'If the selected Test is linked to one or more Calculated Test, inform the correspondent values
                                        If (Not selectedOrderTestRow.IsCalcTestIDsNull And selectedOrderTestRow.CalcTestIDs.Trim <> "") Then
                                            myPatientsRow.CalcTestID = selectedOrderTestRow.CalcTestIDs
                                            myPatientsRow.CalcTestName = selectedOrderTestRow.CalcTestNames
                                        End If

                                        myPatientsRow.OTStatus = "OPEN"
                                        myPatientsDataTable.Rows.Add(myPatientsRow)
                                    Next
                                Next
                            End If

                            'Rewrite value of CreationOrder when needed
                            SortPatients(pWSResultDS)

                            'Confirm changes done in Patients table
                            pWSResultDS.Patients.AcceptChanges()

                            If (Not pForLoadSavedWS) Then
                                'Add all needed Controls, Calibrators and Blanks
                                resultData = AddControlOrderTests(pSelectedTestsDS, pWSResultDS, pAnalyzerID, False, False, "PATIENT")
                            Else
                                'Return the WSResultDS 
                                resultData.SetDatos = pWSResultDS
                            End If
                        End If

                        bPatientStat = Nothing
                        bPatientNotStat = Nothing
                        bTestTypeIconSTD = Nothing
                        bTestTypeIconCALC = Nothing
                        bTestTypeIconISE = Nothing
                        bTestTypeIconOFF = Nothing
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.AddPatientOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used to manage the adding Patient Order Tests received from LIS
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSavedWSOrderTestsRow">Every saved WS row</param>
        ''' <param name="pWSResultDS">Typed DataSet containing in the table of Patients the list of currently requested Patient Order Tests</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if there is a tube positioned in Samples Rotor for the Patient Sample 
        '''          (when True) or if the Patient Sample already exists as a required Element in the WS</returns>
        ''' <remarks>
        ''' Created by:  XB 20/03/2013
        ''' Modified By: TR 11/04/2013 - Changed from Sub to Function; added new parameter pWorksessionID, who is needed for function VerifyScannedSpecimen
        '''              SA 15/04/2013 - Before verify is there is a not in use tube for the Patient Sample placed in Samples Rotor, verify if the Patient Sample 
        '''                              exists already as a required Element in the active WS
        '''              SA 22/05/2013 - Verification of Patient Sample already exists as WS Required Element should be done also for Calculated Tests
        '''              SA 01/08/2013 - When in the active WS there is an Order for the same SampleID but different StatFlag, the LIS Order Test is added to the
        '''                              same Order, and then the StatFlag sent by LIS is changed to the StatFlag of the existing Order
        ''' </remarks>
        Public Function AddPatientOrderTestsFromLIS(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pSavedWSOrderTestsRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, _
                                                    ByRef pWSResultDS As WorkSessionResultDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTube As String = ""
                Dim myOrderID As String = ""
                Dim nextCreationOrder As Integer = 1

                'Get the next creation order for Patients
                nextCreationOrder = pWSResultDS.Patients.Rows.Count + 1

                'Verify if in the active WS there is an Order with the same StatFlag for the same Patient
                Dim lstWSOrdersIDs As List(Of String) = (From a In pWSResultDS.Patients _
                                                        Where a.SampleClass = "PATIENT" _
                                                      AndAlso a.SampleID = pSavedWSOrderTestsRow.SampleID _
                                                      AndAlso a.StatFlag = pSavedWSOrderTestsRow.StatFlag _
                                                      AndAlso (Not a.IsOrderIDNull) _
                                                       Select a.OrderID Distinct).ToList
                If (lstWSOrdersIDs.Count > 0) Then
                    myOrderID = lstWSOrdersIDs(0)
                Else
                    'Verify if in the active WS there is an Order with the opposite StatFlag for the same Patient
                    lstWSOrdersIDs = (From a In pWSResultDS.Patients _
                                     Where a.SampleClass = "PATIENT" _
                                    AndAlso a.SampleID = pSavedWSOrderTestsRow.SampleID _
                                    AndAlso (Not a.IsOrderIDNull) _
                                     Select a.OrderID Distinct).ToList

                    If (lstWSOrdersIDs.Count > 0) Then
                        myOrderID = lstWSOrdersIDs(0)
                        pSavedWSOrderTestsRow.StatFlag = (Not pSavedWSOrderTestsRow.StatFlag) 'Change value of the StatFlag for the LIS Order Test to be added to the WS... 
                    End If
                End If

                'Verify if there are another Order Tests for the same Patient and Sample Type in the active WorkSession to get the tube type currently assgned
                Dim myPositionedSpecimen As Integer = 0
                If (pSavedWSOrderTestsRow.TestType = "STD" OrElse pSavedWSOrderTestsRow.TestType = "ISE") Then
                    Dim lstTubType As List(Of String) = (From a In pWSResultDS.Patients _
                                                        Where a.SampleClass = "PATIENT" _
                                                      AndAlso a.SampleID = pSavedWSOrderTestsRow.SampleID _
                                                      AndAlso a.SampleType = pSavedWSOrderTestsRow.SampleType _
                                                      AndAlso (a.TestType = "STD" Or a.TestType = "ISE") _
                                                       Select a.TubeType Distinct).ToList
                    If (lstTubType.Count > 0) Then myTube = lstTubType(0)
                End If

                If (pSavedWSOrderTestsRow.TestType <> "OFFS") Then
                    'Verify if in the active WS there are another Order Test for the same Patient and Sample Type that have been sent to positioning
                    '(the Patient Sample already exists as a required Element in the active Work Session)
                    myPositionedSpecimen = (From a In pWSResultDS.Patients _
                                           Where a.SampleClass = "PATIENT" _
                                         AndAlso a.SampleID = pSavedWSOrderTestsRow.SampleID _
                                         AndAlso a.SampleType = pSavedWSOrderTestsRow.SampleType _
                                         AndAlso a.TestType <> "OFFS" _
                                         AndAlso Not a.IsOrderTestIDNull _
                                         AndAlso a.OTStatus <> "OPEN" _
                                          Select a.OrderTestID).ToList.Count()

                    If (myPositionedSpecimen = 0) Then
                        'If the Patient Sample does not exist as a required Element in the WS, then verify if there is at least an scanned tube for it
                        Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate
                        myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.VerifyScannedSpecimen(Nothing, pAnalyzerID, pWorkSessionID, pSavedWSOrderTestsRow.SpecimenID, _
                                                                                                        pSavedWSOrderTestsRow.SampleID, pSavedWSOrderTestsRow.SampleType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myPositionedSpecimen = DirectCast(myGlobalDataTO.SetDatos, Integer)
                        End If
                    End If
                End If

                'Insert the new Patient Order Test in Patients table of pWSResultsDS with following information
                Dim myPatientsDataTable As WorkSessionResultDS.PatientsDataTable
                myPatientsDataTable = DirectCast(pWSResultDS.Patients, WorkSessionResultDS.PatientsDataTable)

                Dim myPatientsRow As WorkSessionResultDS.PatientsRow
                myPatientsRow = DirectCast(myPatientsDataTable.NewRow(), WorkSessionResultDS.PatientsRow)

                myPatientsRow.SampleClass = pSavedWSOrderTestsRow.SampleClass
                myPatientsRow.OrderID = myOrderID
                myPatientsRow.StatFlag = pSavedWSOrderTestsRow.StatFlag
                myPatientsRow.SampleID = pSavedWSOrderTestsRow.SampleID
                myPatientsRow.SampleIDType = pSavedWSOrderTestsRow.PatientIDType
                myPatientsRow.TestType = pSavedWSOrderTestsRow.TestType
                myPatientsRow.TestID = pSavedWSOrderTestsRow.TestID
                myPatientsRow.TestName = pSavedWSOrderTestsRow.TestName
                myPatientsRow.SampleType = pSavedWSOrderTestsRow.SampleType

                If (myPatientsRow.TestType = "STD" OrElse myPatientsRow.TestType = "ISE") Then
                    myPatientsRow.NumReplicates = pSavedWSOrderTestsRow.ReplicatesNumber
                    myPatientsRow.TubeType = CStr(IIf(myTube = "", pSavedWSOrderTestsRow.TubeType, myTube))
                Else
                    myPatientsRow.CalcTestFormula = pSavedWSOrderTestsRow.FormulaText
                End If

                If (Not pSavedWSOrderTestsRow.IsCalcTestIDsNull) Then
                    myPatientsRow.CalcTestID = pSavedWSOrderTestsRow.CalcTestIDs
                    myPatientsRow.CalcTestName = pSavedWSOrderTestsRow.CalcTestNames
                End If

                If (Not pSavedWSOrderTestsRow.IsAwosIDNull) Then myPatientsRow.AwosID = pSavedWSOrderTestsRow.AwosID
                If (Not pSavedWSOrderTestsRow.IsSpecimenIDNull) Then myPatientsRow.SpecimenID = pSavedWSOrderTestsRow.SpecimenID
                If (Not pSavedWSOrderTestsRow.IsESOrderIDNull) Then myPatientsRow.ESOrderID = pSavedWSOrderTestsRow.ESOrderID
                If (Not pSavedWSOrderTestsRow.IsLISOrderIDNull) Then myPatientsRow.LISOrderID = pSavedWSOrderTestsRow.LISOrderID
                If (Not pSavedWSOrderTestsRow.IsESPatientIDNull) Then myPatientsRow.ESPatientID = pSavedWSOrderTestsRow.ESPatientID
                If (Not pSavedWSOrderTestsRow.IsLISPatientIDNull) Then myPatientsRow.LISPatientID = pSavedWSOrderTestsRow.LISPatientID

                If (Not pSavedWSOrderTestsRow.IsExternalQCNull) Then
                    myPatientsRow.ExternalQC = pSavedWSOrderTestsRow.ExternalQC
                Else
                    myPatientsRow.ExternalQC = False
                End If

                myPatientsRow.Selected = (myPositionedSpecimen > 0)
                myPatientsRow.OTStatus = "OPEN"
                myPatientsRow.LISRequest = True
                myPatientsRow.CreationOrder = nextCreationOrder
                myPatientsDataTable.Rows.Add(myPatientsRow)

                'Confirm changes done in Patients table
                pWSResultDS.Patients.AcceptChanges()

                'Return a boolean value indicating if the Patient has to be added as a required WS Element 
                myGlobalDataTO.SetDatos = myPatientsRow.Selected
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.AddPatientOrderTestsFromLIS", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When a Cancel Awos message is received from LIS for a Calculated Test/SampleType, the Cancel is accepted if the Status of the OrderTest is 
        ''' OPEN, which means the Calculated Test is not in process. When the Cancel is accepted, if the Calculated Test/SampleType is not included in 
        ''' the formula of another requested Calculated Test, it is deleted and besides, it is verified if the Order Tests for the formula of the 
        ''' deleted Calculated Test can be also deleted. If the Formula member is also a Calculated Test, this same function is called recursively; if 
        ''' the formula member is an Standard Test, it should fulfill following conditions to be deleted:
        ''' ** The Status of the OrderTest is OPEN
        ''' ** It has not been requested by LIS individually
        ''' ** It is not included in the formula of another requested Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of the OrderTest to Cancel</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pTestID">Identifier of a Calculated Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/05/2013
        ''' </remarks>
        Public Function CancelAwosForOpenCalcTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pOrderID As String,
                                                  ByVal pSampleClass As String, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if the OrderTest is included in the formula of at least one requested Calculated Test
                        Dim myOrderCalculatedTestsDelegate As New OrderCalculatedTestsDelegate

                        resultData = myOrderCalculatedTestsDelegate.GetByOrderTestID(dbConnection, pOrderTestID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myOrderCalculatedTestsDS As OrderCalculatedTestsDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)

                            'If the OrderTest is included in the formula of at least one requested Calculated Test, it cannot be deleted. 
                            'Instead, the LIS information for the OrderTest is deleted
                            Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate
                            If (myOrderCalculatedTestsDS.twksOrderCalculatedTests.Rows.Count > 0) Then
                                'Get the last Rerun Number sent by LIS for the Order Test
                                resultData = myOrderTestsLISInfoDelegate.GetByOrderTestID(dbConnection, pOrderTestID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myOrderTestsLISInfoDS As OrderTestsLISInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                                    If (myOrderTestsLISInfoDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                        'Delete the LIS information for the last Rerun of the Order Test
                                        resultData = myOrderTestsLISInfoDelegate.Delete(dbConnection, pOrderTestID, myOrderTestsLISInfoDS.twksOrderTestsLISInfo.First.RerunNumber)
                                    End If
                                End If
                            Else
                                'Get OrderTest data for all Tests included in the formula of the Calculated Test 
                                resultData = myOrderCalculatedTestsDelegate.GetOTInfoByCalcOrderTestID(dbConnection, pOrderTestID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myOrderTestsInFormulaDS As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                                    'Delete links with all tests included in its formula
                                    resultData = myOrderCalculatedTestsDelegate.DeleteByCalOrderTestId(dbConnection, pOrderTestID)
                                    If (Not resultData.HasError) Then
                                        'Verify if every OrderTest of each Test included in the Formula can be also deleted
                                        For Each myRow As OrderTestsDS.twksOrderTestsRow In myOrderTestsInFormulaDS.twksOrderTests
                                            'If LIS does not sent an specific AwosID for the formula member OrderTest, then it is verified if it can be deleted according the TestType
                                            If (myRow.IsAwosIDNull) Then
                                                If (myRow.TestType = "CALC") Then
                                                    'If the formula member corresponds to a Calculated Test, call this function recursively to verify if it can be deleted
                                                    resultData = CancelAwosForOpenCalcTest(dbConnection, myRow.OrderTestID, myRow.OrderID, pSampleClass, myRow.TestID, myRow.SampleType)
                                                Else
                                                    'Validate if the OrderTest for the Standard Test/SampleType can be deleted from the active WS
                                                    If (myRow.OrderTestStatus = "OPEN") Then
                                                        'Verify if the OrderTest is included in the formula of at least one requested Calculated Test
                                                        resultData = myOrderCalculatedTestsDelegate.GetByOrderTestID(dbConnection, myRow.OrderTestID)
                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            myOrderCalculatedTestsDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)

                                                            If (myOrderCalculatedTestsDS.twksOrderCalculatedTests.Rows.Count = 0) Then
                                                                'The OrderTest for the Standard Test/SampleType can be deleted. Needed Calibrator and Blank are also deleted when it is possible
                                                                resultData = DeleteByOrderTestID(dbConnection, myRow.OrderTestID, myRow.OrderID, pSampleClass, myRow.TestType, _
                                                                                                 myRow.TestID, myRow.SampleType)
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Next

                                        If (Not resultData.HasError) Then
                                            'Delete the LIS information for the last Rerun of the Order Test
                                            resultData = myOrderTestsLISInfoDelegate.Delete(dbConnection, pOrderTestID, 1)

                                            If (Not resultData.HasError) Then
                                                'Finally, delete the OrderTest for the Calculated Test
                                                resultData = DeleteByOrderTestID(dbConnection, pOrderTestID, pOrderID, pSampleClass, "CALC", pTestID, pSampleType)
                                            End If
                                        End If
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.CancelAwosForOpenCalcTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Cancel Awos message is received from LIS for an Standard Test/SampleType, the Cancel is accepted and the corresponding Order Test is
        ''' deleted, it has to be verified if the needed Calibrator and Blank can be also deleted. A Calibrator can be deleted if following conditions 
        ''' are fulfilled:
        ''' ** The status of the Order Test for the Calibrator is OPEN, which mean the Calibrator is not in process
        ''' ** There are not Patient nor Control Order Tests for the same Standard Test/SampleType in the active WS, that is to say, the Calibrator is not needed;
        '''    when the for the Standard Test/SampleType the needed Calibrator is defined for an alternative SampleType, it is verified also that there are not 
        '''    Patient nor Control Order Tests in the active WS for the same Standard Test and the alternative SampleType.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if the Calibrator was deleted</returns>
        ''' <remarks>
        ''' Created by:  TR 15/05/2013
        ''' Modified by: SA 24/05/2013 - Open a DB Transaction instead a DB Connection
        ''' </remarks>
        Public Function CheckCalibratorCanBeDeleted(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myValueToReturn As Boolean = False
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO

                        'Verify is there is an OPEN CALIBRATOR for the Test/Sample Type
                        resultData = myOrderTestsDAO.ReadBlankOrCalibByTestID(dbConnection, "CALIB", pTestID, pSampleType)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myOrderTestsDS As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            If (myOrderTestsDS.twksOrderTests.Count > 0) Then
                                'Only if the status of the CALIBRATOR is OPEN it can be deleted
                                If (myOrderTestsDS.twksOrderTests.First().OrderTestStatus = "OPEN") Then
                                    Dim mySampleType As String = pSampleType
                                    Dim myOrderTestID As Integer = myOrderTestsDS.twksOrderTests.First().OrderTestID

                                    'Verify if the Experimental Calibrator is needed for other Order Tests in the active Work Session
                                    resultData = myOrderTestsDAO.CountBlankOrCalibDependencies(dbConnection, "CALIB", pTestID, pSampleType, myOrderTestID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim numOTs As Integer = DirectCast(resultData.SetDatos, Integer)

                                        If (numOTs = 0) Then
                                            'Delete the Order Test for the Calibrator and, if it is possible, the Blank for the Test
                                            resultData = DeleteByOrderTestID(dbConnection, myOrderTestID, myOrderTestsDS.twksOrderTests.First().OrderID, _
                                                                             "CALIB", "STD", pTestID, pSampleType)

                                            myValueToReturn = (Not resultData.HasError)
                                            If (myValueToReturn) Then
                                                'If field AlternativeOrderTestID is informed, it means the Calibrator is defined for an alternative Sample Type
                                                If (Not myOrderTestsDS.twksOrderTests.First().IsAlternativeOrderTestIDNull) Then
                                                    'Search the Sample Type for which the Calibrator is defined
                                                    resultData = myOrderTestsDAO.GetSampleTypeForAlternativeOT(dbConnection, myOrderTestsDS.twksOrderTests.First().AlternativeOrderTestID)
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        mySampleType = resultData.SetDatos.ToString()
                                                        myOrderTestID = myOrderTestsDS.twksOrderTests.First().AlternativeOrderTestID

                                                        'Verify if there are PATIENT or CTRL OrderTests for the TestID and the Alternative SampleType
                                                        resultData = myOrderTestsDAO.CountBlankOrCalibDependencies(dbConnection, "CALIB", pTestID, mySampleType, myOrderTestID)
                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            numOTs = DirectCast(resultData.SetDatos, Integer)

                                                            If (numOTs = 0) Then
                                                                'Delete the Order Test for the Calibrator used as Alternative and, if it is possible, the Blank for the Test
                                                                resultData = DeleteByOrderTestID(dbConnection, myOrderTestID, myOrderTestsDS.twksOrderTests.First().OrderID, _
                                                                                                 "CALIB", "STD", pTestID, mySampleType)
                                                                myValueToReturn = (Not resultData.HasError)
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Else
                                'The Test/SampleType does not use an Experimental Calibrator; verify if the Blank needed for the Test can be deleted
                                resultData = CheckBlankCanBeDeleted(dbConnection, pTestID)
                                myValueToReturn = (Not resultData.HasError)
                            End If

                            'Finally, return a boolean value indicating if the needed Calibrator and Blank were deleted
                            resultData.SetDatos = myValueToReturn
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.CheckCalibratorCanBeDeleted", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Cancel Awos message is received from LIS for an Standard Test/SampleType, the Cancel is accepted and the corresponding Order Test is
        ''' deleted, it has to be verified if the needed Calibrator and Blank can be also deleted. A Blank can be deleted if following conditions 
        ''' are fulfilled:
        ''' ** The status of the Order Test for the Blank is OPEN, which mean the Blank is not in process
        ''' ** There are not Patient nor Control nor Calibrator Order Tests for the same Standard Test (whatever SampleType) in the active WS; that is 
        '''    to say, the Blank is not needed.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if the Blank was deleted</returns>
        ''' <remarks>
        ''' Created by:  TR 15/05/2013
        ''' Modified by: SA 24/05/2013 - Open a DB Transaction instead a DB Connection
        ''' </remarks>
        Public Function CheckBlankCanBeDeleted(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myValueToReturn As Boolean = False
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO

                        'Verify if there is an OPEN BLANK for the Test in the active WS
                        resultData = myOrderTestsDAO.ReadBlankOrCalibByTestID(dbConnection, "BLANK", pTestID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myOrderTestsDS As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            If (myOrderTestsDS.twksOrderTests.Count > 0) Then
                                'Only if the status of the BLANK is OPEN it can be deleted
                                If (myOrderTestsDS.twksOrderTests.First().OrderTestStatus = "OPEN") Then
                                    Dim myOrderTestID As Integer = myOrderTestsDS.twksOrderTests.First().OrderTestID

                                    'Verify if there are PATIENT, CTRL or CALIB Order Tests for the specified Test (whatever Sample Type)
                                    resultData = myOrderTestsDAO.CountBlankOrCalibDependencies(dbConnection, "STD", pTestID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim NumOTs As Integer = DirectCast(resultData.SetDatos, Integer)

                                        If (NumOTs = 0) Then
                                            'There are not Order Tests depending on the BLANK, then it is deleted
                                            resultData = DeleteByOrderTestID(dbConnection, myOrderTestID, myOrderTestsDS.twksOrderTests.First().OrderID.ToString(), _
                                                                             "BLANK", "STD", pTestID)
                                            myValueToReturn = (Not resultData.HasError)
                                        End If
                                    End If
                                End If
                            Else
                                'There is not a BLANK for the Test in the active WS - This case is not possible!!
                                myValueToReturn = True
                            End If

                            'Finally, return a boolean value indicating if the needed Blank was deleted
                            resultData.SetDatos = myValueToReturn
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.CheckBlankCanBeDeleted", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count all OrderTests (or only the Calculated/OffSystem ones) in current WorkSession that have the specified OrderTestStatus
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pStatus">OrderTest Status to search</param>
        ''' <param name="pOnlyTestsWithNoExecutions">When TRUE, function count only CALC and OFFS Order Tests having the specified Status
        '''                                          When FALSE, function count all Order Tests having the specified Status</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of Order Tests with the specified Status</returns>
        ''' <remarks>
        ''' Created by: AG 03/03/2014 - BT #1524
        ''' </remarks>
        Public Function CountByOrderTestStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                            ByVal pStatus As String, ByVal pOnlyTestsWithNoExecutions As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TwksOrderTestsDAO
                        resultData = myDAO.CountByOrderTestStatus(dbConnection, pAnalyzerID, pWorkSessionID, pStatus, pOnlyTestsWithNoExecutions)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.CountByOrderTestStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a list of Order Tests to an existing Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsList">Typed DataSet OrderTestsDS containing the list of Order Tests to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of Order Tests added to the Order (including the 
        '''          generated OrderTestID for each one of them)</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: SA 27/01/2010 - Changed the way of opening a DB Transaction to fulfill the new template
        '''              SA 26/03/2013 - Added changes needed for new LIS implementation:  for each row in the OrderTestsDS received as parameter, 
        '''                              if LIS fields are informed, fill a row of a OrderTestsLISInfoDS (use RerunNumber = 1). Finally, call 
        '''                              function Create in OrderTestsLISInfoDelegate 
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As OrderTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pOrderTestsList.twksOrderTests.Rows.Count > 0) Then
                            'Add all Order Tests included in the entry Dataset
                            Dim myOrderTestDAO As New TwksOrderTestsDAO()
                            resultData = myOrderTestDAO.Create(dbConnection, pOrderTestsList)

                            If (Not resultData.HasError) Then
                                'If the Order to which the Order Tests have been added was Closed, it has to be re-opened
                                Dim myOrdersDelegate As New OrdersDelegate
                                resultData = myOrdersDelegate.ReOpenClosedOrder(dbConnection, pOrderTestsList.twksOrderTests(0).OrderID)
                            End If

                            'SA 26/03/2013 - For each Patient or Control LIS Order Test, inform LIS fields in the auxiliary table twksOrderTestsLISInfo
                            If (Not resultData.HasError) Then
                                'Get all LIS Order Tests with AWOSID informed from the entry DS
                                Dim lstLISOrderTests As List(Of OrderTestsDS.twksOrderTestsRow) = (From a As OrderTestsDS.twksOrderTestsRow In pOrderTestsList.twksOrderTests _
                                                                                                  Where (a.SampleClass = "PATIENT" OrElse a.SampleClass = "CTRL") _
                                                                                                AndAlso a.LISRequest = True _
                                                                                                AndAlso Not a.IsAwosIDNull() _
                                                                                                 Select a).ToList

                                If (lstLISOrderTests.Count > 0) Then
                                    'Fill an OrderTestsLISInfoDS with all LIS fields for every LIS Order Test
                                    Dim myOrderTestLISInfoDS As New OrderTestsLISInfoDS
                                    Dim newOTLISInfoRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow

                                    For Each otRow As OrderTestsDS.twksOrderTestsRow In lstLISOrderTests
                                        newOTLISInfoRow = myOrderTestLISInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow()

                                        newOTLISInfoRow.OrderTestID = otRow.OrderTestID
                                        newOTLISInfoRow.RerunNumber = 1
                                        If (Not otRow.IsAwosIDNull) Then newOTLISInfoRow.AwosID = otRow.AwosID
                                        If (Not otRow.IsSpecimenIDNull) Then newOTLISInfoRow.SpecimenID = otRow.SpecimenID
                                        If (Not otRow.IsESPatientIDNull) Then newOTLISInfoRow.ESPatientID = otRow.ESPatientID
                                        If (Not otRow.IsLISPatientIDNull) Then newOTLISInfoRow.LISPatientID = otRow.LISPatientID
                                        If (Not otRow.IsESOrderIDNull) Then newOTLISInfoRow.ESOrderID = otRow.ESOrderID
                                        If (Not otRow.IsLISOrderIDNull) Then newOTLISInfoRow.LISOrderID = otRow.LISOrderID

                                        myOrderTestLISInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(newOTLISInfoRow)
                                    Next

                                    'Finally, add the LIS information for the Order Tests
                                    Dim myOTsLISInfoDelegate As New OrderTestsLISInfoDelegate
                                    resultData = myOTsLISInfoDelegate.Create(dbConnection, myOrderTestLISInfoDS, False)
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            resultData.SetDatos = pOrderTestsList

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

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used in the screen of WS Preparation to manage the deletion of Blank Order Tests
        ''' </summary>
        ''' <param name="pDeletionType">Indicates how the list of Tests in the entry parameter pSelectedTestsDS have to be
        '''                             processed: when value is "IN_LIST" it means that all Blanks for the Tests included in 
        '''                             pSelectedTestsDS have to be deleted; when value is "NOT_IN_LIST" it means that all 
        '''                             Blanks for the Tests NOT included in pSelectedTestsDS have to be deleted</param>
        ''' <param name="pSelectedTestsDS">List of selected Tests</param>
        ''' <param name="pWSResultDS">WorkSessionResultDS containing the list of Patient Samples, Controls, Calibrators 
        '''                           and Blanks currently requested</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of BlankCalibrators updated
        '''          after delete all Blanks without any Calibrator, Control nor Patient Sample related</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2010
        ''' Modified by: SA 25/03/2010 - Added code to update value of field CreationOrder after delete Blanks
        '''              SA 31/01/2012 - Do not declare variables inside loops; set to nothing lists to save memory
        ''' </remarks>
        Public Function DeleteBlankOrderTests(ByVal pDeletionType As String, ByVal pSelectedTestsDS As SelectedTestsDS, _
                                              ByVal pWSResultDS As WorkSessionResultDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                'Get the list of non positioned Blanks in table of Blanks and Calibrators 
                Dim lstWSBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                lstWSBlanksDS = (From a In pWSResultDS.BlankCalibrators _
                                Where a.SampleClass = "BLANK" _
                              AndAlso a.OTStatus = "OPEN" _
                               Select a).ToList()

                Dim myTestType As String
                Dim myTestID As Integer
                Dim deleteOrderTest As Boolean

                Dim lstSelectedTestsDS As List(Of SelectedTestsDS.SelectedTestTableRow)
                Dim lstWSCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)
                Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)

                For Each blankOrderTest In lstWSBlanksDS
                    'Get TestType and TestID of the current row
                    myTestType = blankOrderTest.TestType
                    myTestID = blankOrderTest.TestID

                    'Verify if the Test Blank is in the list of Selected Tests
                    lstSelectedTestsDS = (From b In pSelectedTestsDS.SelectedTestTable _
                                         Where b.TestType = myTestType _
                                       AndAlso b.TestID = myTestID _
                                        Select b).ToList()

                    'Entry DataSet pSelectedTestsDS can contain the list of Tests for which the Blanks have to be deleted
                    '- when pDeletionType=IN_LIST - or the list of Tests for which the Blanks have not to be deleted 
                    '- when pDeletionType=NOT_IN_LIST -; in this last case, Blanks to delete are those with Tests that are 
                    'not in the list of selected Tests
                    deleteOrderTest = (pDeletionType = "IN_LIST" AndAlso lstSelectedTestsDS.Count = 1) OrElse _
                                      (pDeletionType = "NOT_IN_LIST" AndAlso lstSelectedTestsDS.Count = 0)
                    If (deleteOrderTest) Then
                        'The Blank can be deleted only when there is not a Calibrator, Control nor Patient Sample requested 
                        'for the same Test (and whatever Sample Type)

                        'Verify if there are Calibrators requested for the Test
                        lstWSCalibratorsDS = (From c In pWSResultDS.BlankCalibrators _
                                             Where c.SampleClass = "CALIB" _
                                           AndAlso c.TestType = myTestType _
                                           AndAlso c.TestID = myTestID _
                                            Select c).ToList()

                        If (lstWSCalibratorsDS.Count = 0) Then
                            'There are not Calibrators requested for the Test, then verify if there 
                            'are Controls requested for it
                            lstWSControlsDS = (From c In pWSResultDS.Controls _
                                              Where c.SampleClass = "CTRL" _
                                            AndAlso c.TestType = myTestType _
                                            AndAlso c.TestID = myTestID _
                                             Select c).ToList()

                            If (lstWSControlsDS.Count = 0) Then
                                'There are not Controls requested for the Test, then verify if there 
                                'are Patient Samples requested for it
                                lstWSPatientsDS = (From c In pWSResultDS.Patients _
                                                  Where c.SampleClass = "PATIENT" _
                                                AndAlso c.TestType = myTestType _
                                                AndAlso c.TestID = myTestID _
                                                 Select c).ToList()

                                If (lstWSPatientsDS.Count = 0) Then
                                    'There are not Patient Samples requested for the Test, then the Blank can be deleted
                                    blankOrderTest.Delete()

                                    'Confirm changes done in the entry DataSet 
                                    pWSResultDS.BlankCalibrators.AcceptChanges()
                                End If
                            End If
                        End If
                    End If
                Next
                lstSelectedTestsDS = Nothing
                lstWSCalibratorsDS = Nothing
                lstWSControlsDS = Nothing
                lstWSPatientsDS = Nothing

                'Rewrite value of CreationOrder for not deleted Blanks
                lstWSBlanksDS = (From a In pWSResultDS.BlankCalibrators _
                                Where a.SampleClass = "BLANK" _
                               Select a).ToList()

                Dim nextCreationOrder As Integer = 1
                For Each blankOrderTest In lstWSBlanksDS
                    blankOrderTest.CreationOrder = nextCreationOrder
                    nextCreationOrder += 1
                Next
                lstWSBlanksDS = Nothing

                'Confirm changes done in the entry DataSet 
                pWSResultDS.BlankCalibrators.AcceptChanges()

                'Return the updated pWSResultDS in the GlobalDataTO
                resultData.HasError = False
                resultData.SetDatos = pWSResultDS
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteBlankOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Order Tests included in the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 08/06/2010
        ''' Modified by: SA 25/01/2011 - Before deleting the Order Tests, delete their Results and Result Alarms
        '''              SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function DeleteByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete all Alarms for the Results of all Order Tests included in the OrderID
                        Dim myResultAlarmsDelegate As New ResultAlarmsDelegate
                        resultData = myResultAlarmsDelegate.DeleteResultAlarmsByOrderID(dbConnection, pOrderID)

                        If (Not resultData.HasError) Then
                            'Delete all Results of all Order Tests included in the OrderID
                            Dim myResultsDelegate As New ResultsDelegate
                            resultData = myResultsDelegate.DeleteResultsByOrderID(dbConnection, pOrderID)
                        End If

                        If (Not resultData.HasError) Then
                            Dim myOrderTestDAO As New TwksOrderTestsDAO()
                            resultData = myOrderTestDAO.DeleteByOrderID(dbConnection, pOrderID)
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteByOrderID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the informed Order Test when a Cancel is received from LIS. If the OrderTest corresponds to an Standard Test, 
        ''' check also if the needed Calibrator and Blank can be deleted.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of the OrderTest to delete</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 15/05/2013
        ''' </remarks>
        Public Function DeleteByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pOrderID As String,
                                            ByVal pSampleClass As String, ByVal pTestType As String, ByVal pTestID As Integer, _
                                            Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim deleteCalibOK As Boolean = False

                        'Delete relation between Order Test and the active WorkSession (WSOrderTest table)
                        Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                        resultData = myWSOrderTestsDelegate.DeleteByOrderTestID(dbConnection, pOrderTestID)

                        'Delete the Order Test
                        If (Not resultData.HasError) Then
                            Dim myOrderTestDAO As New TwksOrderTestsDAO()
                            resultData = myOrderTestDAO.DeleteByOrderTestID(dbConnection, pOrderTestID)
                        End If

                        'Delete the Order in which the Order Test was included if it does not contain other Order Tests
                        If (Not resultData.HasError) Then
                            Dim myOrdersDelegate As New OrdersDelegate
                            resultData = myOrdersDelegate.DeleteEmptyOrder(dbConnection, pOrderID)
                        End If

                        'Only when the Order Test was for an Standard Test, verify if the needed Calibrator and Blank can be also deleted
                        If (pTestType = "STD") Then
                            deleteCalibOK = True
                            If (Not resultData.HasError) Then
                                'If the deleted Order Test was a Patient or Control one, try to delete the needed Calibrator
                                If (pSampleClass = "PATIENT" OrElse pSampleClass = "CTRL") Then
                                    resultData = CheckCalibratorCanBeDeleted(dbConnection, pTestID, pSampleType)
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'If the deleted Order Test was a Calibrator one, try to delete the needed Blank
                                If (pSampleClass = "CALIB") Then
                                    resultData = CheckBlankCanBeDeleted(dbConnection, pTestID)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used in the screen of WS Preparation to manage the deletion of Calibrator Order Tests
        ''' </summary>
        ''' <param name="pDeletionType">Indicates how the list of Tests in the entry parameter pSelectedTestsDS have to be
        '''                             processed: when value is "IN_LIST" it means that all Calibrators for the Tests included in 
        '''                             pSelectedTestsDS have to be deleted; when value is "NOT_IN_LIST" it means that all 
        '''                             Calibrators for the Tests NOT included in pSelectedTestsDS have to be deleted</param>
        ''' <param name="pSelectedTestsDS">List of selected Tests</param>
        ''' <param name="pWSResultDS">WorkSessionResultDS containing the list of Patient Samples, Controls, Calibrators 
        '''                           and Blanks currently requested</param>
        ''' <param name="pSampleType">Code of the Sample Type selected to delete Calibrators. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of BlankCalibrators updated
        '''          after delete all specified Calibrators (and related Blanks when it was possible) without any Control nor 
        '''          Patient Sample related</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2010
        ''' Modified by: SA 25/03/2010 - Added code to update value of field CreationOrder after delete Calibrators
        '''              SA 29/09/2011 - Fixed the loop used to rebuilding of RequestedSampleTypes for deletion of Alternative Calibrators;
        '''                              it did not work when there was more than one Test using Alternative Calibrators due to only rebuild
        '''                              the first in the list
        '''              SA 31/01/2012 - Do not declare variables inside loops; set to nothing lists to save memory
        ''' </remarks>
        Public Function DeleteCalibratorOrderTests(ByVal pDeletionType As String, ByVal pSelectedTestsDS As SelectedTestsDS, _
                                                   ByVal pWSResultDS As WorkSessionResultDS, Optional ByVal pSampleType As String = "") _
                                                   As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                'Get the list of non positioned Calibrators in table of Blanks and Calibrators 
                Dim lstWSCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)

                If (pSampleType = "") Then
                    lstWSCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                         Where a.SampleClass = "CALIB" _
                                       AndAlso a.OTStatus = "OPEN" _
                                        Select a).ToList()
                Else
                    'Calibrator Order Tests are filtered by an specific SampleType
                    lstWSCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                         Where a.SampleClass = "CALIB" _
                                       AndAlso a.OTStatus = "OPEN" _
                                       AndAlso a.SampleType = pSampleType _
                                        Select a).ToList()
                End If

                Dim myTestType As String
                Dim myTestID As Integer
                Dim mySampleType As String

                Dim deleteOK As Boolean
                Dim deleteOrderTest As Boolean
                Dim lstSelectedTestsDS As List(Of SelectedTestsDS.SelectedTestTableRow)

                If (lstWSCalibratorsDS.Count > 0) Then
                    For Each calibratorOrderTest In lstWSCalibratorsDS
                        'Get TestType, TestID and SampleType of the current row
                        myTestType = calibratorOrderTest.TestType
                        myTestID = calibratorOrderTest.TestID
                        mySampleType = calibratorOrderTest.SampleType

                        'Verify if the Test/SampleType is in the list of Selected Tests
                        lstSelectedTestsDS = (From b In pSelectedTestsDS.SelectedTestTable _
                                             Where b.TestType = myTestType _
                                           AndAlso b.TestID = myTestID _
                                           AndAlso b.SampleType = mySampleType _
                                            Select b).ToList()

                        'Entry DataSet pSelectedTestsDS can contain the list of Test/SampleTypes for which the Calibrators have to be deleted
                        '- when pDeletionType=IN_LIST - or the list of Test/SampleTypes for which the Calibrators have not to be deleted 
                        '- when pDeletionType=NOT_IN_LIST -; in this last case, Calibrators to delete are those with Test/SampleTypes that are 
                        'not in the list of selected Tests
                        deleteOrderTest = (pDeletionType = "IN_LIST" AndAlso lstSelectedTestsDS.Count = 1) OrElse _
                                          (pDeletionType = "NOT_IN_LIST" AndAlso lstSelectedTestsDS.Count = 0)

                        If (deleteOrderTest) Then
                            'Verify if there are Control and/or Patient Order Tests for the Test and SampleType
                            deleteOK = True
                            If (VerifyCalibratorDeletion(calibratorOrderTest.TestType, calibratorOrderTest.TestID, _
                                                         calibratorOrderTest.SampleType, pWSResultDS)) Then
                                If (calibratorOrderTest.SampleType.Trim <> calibratorOrderTest.RequestedSampleTypes.Trim) Then
                                    'The Calibrator can be deleted only when there is not a Control nor Patient Sample requested 
                                    'for the same Test with whatever of the Sample Type specified in field RequestedSampleTypes 
                                    '(for Alternative Calibrators)
                                    Dim additionalSampleTypes() As String = Split(calibratorOrderTest.RequestedSampleTypes.Trim)
                                    For Each reqSampleType As String In additionalSampleTypes
                                        If (Not VerifyCalibratorDeletion(calibratorOrderTest.TestType, calibratorOrderTest.TestID, _
                                                                         reqSampleType, pWSResultDS)) Then
                                            deleteOK = False
                                            Exit For
                                        End If
                                    Next
                                End If
                            Else
                                deleteOK = False
                            End If

                            If (deleteOK) Then
                                'The Calibrator can be deleted
                                calibratorOrderTest.Delete()

                                'Confirm changes done in the entry DataSet
                                pWSResultDS.BlankCalibrators.AcceptChanges()

                                'Verify if it is possible to delete also the Blank requested for the Test
                                Dim mySelectedTestDS As New SelectedTestsDS
                                Dim selectedTestRow As SelectedTestsDS.SelectedTestTableRow

                                selectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow
                                selectedTestRow.TestType = myTestType
                                selectedTestRow.TestID = myTestID
                                selectedTestRow.SampleType = mySampleType
                                mySelectedTestDS.SelectedTestTable.Rows.Add(selectedTestRow)

                                resultData = DeleteBlankOrderTests("IN_LIST", mySelectedTestDS, pWSResultDS)
                            End If

                        ElseIf (pDeletionType = "IN_LIST") Then
                            'Verify if the Calibrator is used as Alternative of other SampleTypes for the same Test
                            If (calibratorOrderTest.SampleType.Trim <> calibratorOrderTest.RequestedSampleTypes.Trim) Then
                                'Get the list of Sample Types using the Calibrator as Alternative
                                Dim posInString As Integer
                                Dim addSampleType As String
                                Dim additionalSampleTypes() As String = Split(calibratorOrderTest.RequestedSampleTypes.Trim)

                                For Each reqSampleType As String In additionalSampleTypes
                                    addSampleType = reqSampleType

                                    'Verify if the Test/SampleType is in the list of Selected Tests
                                    lstSelectedTestsDS = (From b In pSelectedTestsDS.SelectedTestTable _
                                                         Where b.TestType = myTestType _
                                                           And b.TestID = myTestID _
                                                           And b.SampleType = addSampleType _
                                                        Select b).ToList()

                                    If (lstSelectedTestsDS.Count = 1) Then
                                        'Verify if there are Control or Patient Order Tests requested for the Test/SampleType 
                                        If (VerifyCalibratorDeletion(calibratorOrderTest.TestType, calibratorOrderTest.TestID, _
                                                                     addSampleType, pWSResultDS)) Then
                                            'Update value of field RequestedSampleTypes by removing the informed SampleType
                                            If (UBound(additionalSampleTypes) = 0) Then
                                                calibratorOrderTest.RequestedSampleTypes = calibratorOrderTest.SampleType
                                            Else
                                                posInString = InStr(calibratorOrderTest.RequestedSampleTypes, addSampleType)
                                                If (posInString = 1) Then
                                                    calibratorOrderTest.RequestedSampleTypes = Mid(calibratorOrderTest.RequestedSampleTypes, posInString + Len(addSampleType)).Trim
                                                Else
                                                    calibratorOrderTest.RequestedSampleTypes = Mid(calibratorOrderTest.RequestedSampleTypes, 1, posInString - 1).Trim + Mid(calibratorOrderTest.RequestedSampleTypes, posInString + Len(addSampleType))
                                                End If
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    Next
                    'End If
                Else
                    'Besides, verify if the SampleType appears in the list of Requested Sample Types for a Calibrator
                    lstWSCalibratorsDS = (From a In pWSResultDS.BlankCalibrators _
                                         Where a.SampleClass = "CALIB" _
                                           And a.OTStatus = "OPEN" _
                                           And (a.RequestedSampleTypes.Contains(pSampleType)) _
                                        Select a).ToList()

                    For Each calibratorOrderTest In lstWSCalibratorsDS
                        'Get TestType, TestID and SampleType of the current row
                        myTestType = calibratorOrderTest.TestType
                        myTestID = calibratorOrderTest.TestID

                        'Verify if the Test/SampleType is in the list of Selected Tests
                        lstSelectedTestsDS = (From b In pSelectedTestsDS.SelectedTestTable _
                                             Where b.TestType = myTestType _
                                           AndAlso b.TestID = myTestID _
                                           AndAlso b.SampleType = pSampleType _
                                            Select b).ToList()

                        'Entry DataSet pSelectedTestsDS can contain the list of Test/SampleTypes for which the Calibrators have to be deleted
                        '- when pDeletionType=IN_LIST - or the list of Test/SampleTypes for which the Calibrators have not to be deleted 
                        '- when pDeletionType=NOT_IN_LIST -; in this last case, Calibrators to delete are those with Test/SampleTypes that are 
                        'not in the list of selected Tests
                        deleteOrderTest = (pDeletionType = "IN_LIST" AndAlso lstSelectedTestsDS.Count = 1) Or _
                                          (pDeletionType = "NOT_IN_LIST" AndAlso lstSelectedTestsDS.Count = 0)

                        If (deleteOrderTest) Then
                            'Verify if there is a Control or Patient Order Tests for the Test and SampleType
                            If (VerifyCalibratorDeletion(myTestType, myTestID, pSampleType, pWSResultDS)) Then
                                'Get the list of Requested Sample Types
                                Dim additionalSampleTypes() As String = Split(calibratorOrderTest.RequestedSampleTypes.Trim)

                                'Update value of field RequestedSampleTypes by removing the informed SampleType
                                If (UBound(additionalSampleTypes) = 0) Then
                                    'There is not another SampleType using the Calibrator as Alternative...
                                    calibratorOrderTest.RequestedSampleTypes = calibratorOrderTest.SampleType
                                Else
                                    'There are more SampleTypes using the Calibrator as Alternative
                                    calibratorOrderTest.RequestedSampleTypes = ""
                                    For i As Integer = 0 To UBound(additionalSampleTypes)
                                        If (additionalSampleTypes(i).Trim <> pSampleType.Trim) Then calibratorOrderTest.RequestedSampleTypes += additionalSampleTypes(i) + " "
                                    Next
                                    calibratorOrderTest.RequestedSampleTypes = calibratorOrderTest.RequestedSampleTypes.Trim
                                End If

                                'Confirm changes done in the entry DataSet
                                pWSResultDS.BlankCalibrators.AcceptChanges()
                            End If
                        End If
                    Next
                End If
                lstSelectedTestsDS = Nothing
                lstWSCalibratorsDS = Nothing

                'Rewrite value of CreationOrder when needed
                SortCalibrators(pWSResultDS)

                'Confirm changes done in the entry DataSet
                pWSResultDS.BlankCalibrators.AcceptChanges()

                If (pDeletionType = "IN_LIST") Then
                    'If the requested Test/SampleType uses a Theorical Factor Calibrator, then the Blank for the Test
                    'remains in the grid and it should be also deleted
                    resultData = DeleteBlankOrderTests(pDeletionType, pSelectedTestsDS, pWSResultDS)
                End If

                'Return the updated pWSResultDS in the GlobalDataTO
                resultData.SetDatos = pWSResultDS
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteCalibratorOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used in the screen of WS Preparation to manage the deletion of Control Order Tests
        ''' </summary>
        ''' <param name="pDeletionType">Indicates how the list of Tests in the entry parameter pSelectedTestsDS have to be
        '''                             processed: when value is "IN_LIST" it means that all Controls for the Tests included in 
        '''                             pSelectedTestsDS have to be deleted; when value is "NOT_IN_LIST" it means that all 
        '''                             Controls for the Tests NOT included in pSelectedTestsDS have to be deleted</param>
        ''' <param name="pSelectedTestsDS">List of selected Tests</param>
        ''' <param name="pWSResultDS">WorkSessionResultDS containing the list of Patient Samples, Controls, Calibrators 
        '''                           and Blanks currently requested</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of Controls updated after delete
        '''          all specified Controls (and table BlankCalibrators updated after delete related Calibrators and Blanks 
        '''          when it was possible)</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2010
        ''' Modified by: SA 17/03/2010 - Changes to allow deleting only some of the Controls for a Test/SampleType when it uses
        '''                              a more than one Control
        '''              SA 26/03/2010 - Added code to update value of field CreationOrder after delete Controls
        '''              DL 15/04/2011 - Replaced use of field ControlNumber for field ControlID
        '''              SA 31/01/2012 - Do not declare variables inside loops; set to nothing lists to save memory
        ''' </remarks>
        Public Function DeleteControlOrderTests(ByVal pDeletionType As String, ByVal pSelectedTestsDS As SelectedTestsDS, _
                                                ByVal pWSResultDS As WorkSessionResultDS, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                'Get the list of non positioned Controls in table of Controls
                Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)

                If (pSampleType = "") Then
                    lstWSControlsDS = (From a In pWSResultDS.Controls _
                                      Where a.SampleClass = "CTRL" _
                                        AndAlso a.OTStatus = "OPEN" _
                                        AndAlso a.LISRequest = False _
                                     Select a).ToList()
                Else
                    lstWSControlsDS = (From a In pWSResultDS.Controls _
                                      Where a.SampleClass = "CTRL" _
                                    AndAlso a.OTStatus = "OPEN" _
                                    AndAlso a.SampleType = pSampleType _
                                     Select a).ToList()
                End If

                Dim myTestType As String
                Dim myTestID As Integer
                Dim mySampleType As String
                Dim myControlID As Integer
                Dim deleteOrderTest As Boolean
                Dim lstSelectedTestsDS As List(Of SelectedTestsDS.SelectedTestTableRow)

                For Each controlOrderTest In lstWSControlsDS
                    'Get TestType, TestID, SampleType and ControlID of the current row
                    myTestType = controlOrderTest.TestType
                    myTestID = controlOrderTest.TestID
                    mySampleType = controlOrderTest.SampleType
                    myControlID = controlOrderTest.ControlID

                    If (pDeletionType = "NOT_IN_LIST") Then
                        'Verify if the Test/SampleType is in the list of Selected Tests
                        lstSelectedTestsDS = (From b In pSelectedTestsDS.SelectedTestTable _
                                             Where b.TestType = myTestType _
                                           AndAlso b.TestID = myTestID _
                                           AndAlso b.SampleType = mySampleType _
                                            Select b).ToList()
                    Else
                        'Verify if the Test/SampleType/ControlID is in the list of Selected Tests
                        lstSelectedTestsDS = (From b In pSelectedTestsDS.SelectedTestTable _
                                             Where b.TestType = myTestType _
                                           AndAlso b.TestID = myTestID _
                                           AndAlso b.SampleType = mySampleType _
                                           AndAlso b.ControlID = myControlID _
                                            Select b).ToList()
                    End If

                    'Entry DataSet pSelectedTestsDS can contain the list of Test/SampleTypes for which the Controls have to be deleted
                    '- when pDeletionType=IN_LIST - or the list of TestType/Test/SampleTypes for which the Controls have not to be deleted 
                    '- when pDeletionType=NOT_IN_LIST -; in this last case, Controls to delete are those with TestType/Test/SampleTypes that are 
                    '  not in the list of selected Tests
                    deleteOrderTest = (pDeletionType = "IN_LIST" AndAlso lstSelectedTestsDS.Count > 0) OrElse _
                                      (pDeletionType = "NOT_IN_LIST" AndAlso lstSelectedTestsDS.Count = 0)

                    If (deleteOrderTest) Then
                        'Control Order Tests can be always deleted
                        controlOrderTest.Delete()

                        'Confirm changes done in the entry DataSet 
                        pWSResultDS.Controls.AcceptChanges()

                        'Verify if it is possible to delete also the Calibrator and Blank requested for the Test
                        Dim mySelectedTestDS As New SelectedTestsDS
                        Dim selectedTestRow As SelectedTestsDS.SelectedTestTableRow

                        selectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow
                        selectedTestRow.TestType = myTestType
                        selectedTestRow.TestID = myTestID
                        selectedTestRow.SampleType = mySampleType
                        mySelectedTestDS.SelectedTestTable.Rows.Add(selectedTestRow)

                        resultData = DeleteCalibratorOrderTests("IN_LIST", mySelectedTestDS, pWSResultDS, mySampleType)
                    End If
                Next
                lstWSControlsDS = Nothing
                lstSelectedTestsDS = Nothing

                'Rewrite value of CreationOrder when needed
                SortControls(pWSResultDS)

                'Confirm changes done in the entry DataSet 
                pWSResultDS.Controls.AcceptChanges()

                'Return the updated pWSResultDS in the GlobalDataTO
                resultData.HasError = False
                resultData.SetDatos = pWSResultDS

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteCalibratorOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Order, delete all Order Tests with Status OPEN that are not included in the 
        ''' list of informed Order Test IDs 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOrderTestsList">List of Order Test IDs that should remain in the Order</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 20/01/2011 - For the specified WorkSession and Order, get all OFF-SYSTEM Order Tests with status
        '''                              CLOSED (those having a result) that are not included in the informed list of OrderTestIDs, 
        '''                              and for each one of them, delete the results and change the OrderTestStatus to OPEN
        '''              SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function DeleteNotInList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pOrderID As String, ByVal pOrderTestsList As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pWorkSessionID.Trim <> "") Then
                            'Get all OFF-SYSTEM Order Tests with status CLOSED (those having a result) that are not included 
                            'in the informed list of OrderTestIDs
                            Dim myWSOrderTests As New WSOrderTestsDelegate
                            resultData = myWSOrderTests.GetClosedOffSystemOTs(dbConnection, pWorkSessionID, pOrderID, pOrderTestsList)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myWSOrderTestsDS As WSOrderTestsDS = DirectCast(resultData.SetDatos, WSOrderTestsDS)

                                Dim myOrderTestDAO As New TwksOrderTestsDAO()
                                Dim myResultsDelegate As New ResultsDelegate
                                For Each orderTest As WSOrderTestsDS.twksWSOrderTestsRow In myWSOrderTestsDS.twksWSOrderTests
                                    'Delete the Result for the OFF-SYSTEM Order Test
                                    resultData = myResultsDelegate.DeleteByOrderTestID(dbConnection, orderTest.OrderTestID)
                                    If (resultData.HasError) Then Exit For

                                    'Change the status of the OFF-SYSTEM OrderTest to OPEN to allow deleting it
                                    resultData = myOrderTestDAO.UpdateStatusByOrderTestID(dbConnection, orderTest.OrderTestID, "OPEN")
                                    If (resultData.HasError) Then Exit For
                                Next
                            End If

                            If (Not resultData.HasError) Then
                                'Remove the link between the specified Work Session and all deleted Order Tests
                                resultData = myWSOrderTests.DeleteNotInList(dbConnection, pWorkSessionID, pOrderID, pOrderTestsList)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all OPEN OrderTests that have been removed from the specified WorkSession
                            Dim myOrderTestDAO As New TwksOrderTestsDAO()
                            resultData = myOrderTestDAO.DeleteNotInList(dbConnection, pOrderID, pOrderTestsList)
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteNotInList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all existing Order Tests with status OPEN and belonging to Orders of the specified Sample Class
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <returns>Global object containing Succes/Error information </returns>
        ''' <remarks>
        ''' Created by:  SA 25/02/2010
        ''' Modified by: SA 26/04/2010 - Delete also the link between the Open Order Tests of the specified Sample Class and 
        '''                              the informed WorkSession
        '''              SA 18/01/2011 - Added call to function DeleteOffSystemResults in ResultsDelegate, to remove all
        '''                              results informed for deleted OFF-SYSTEM Order Tests  
        '''              SA 01/09/2011 - Changed the function template
        '''              XB 26/04/2013 - Remove OrderTestsLISInfo before to delete OrderTests
        ''' </remarks>
        Public Function DeleteOpenOrderTestsBySampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                          ByVal pSampleClass As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pWorkSessionID.Trim <> "") Then
                            If (pSampleClass = "PATIENT") Then
                                'Get all OFF-SYSTEM Order Tests with status CLOSED (those having a result) 
                                Dim myWSOrderTests As New WSOrderTestsDelegate
                                resultData = myWSOrderTests.GetClosedOffSystemOTs(dbConnection, pWorkSessionID)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myWSOrderTestsDS As WSOrderTestsDS = DirectCast(resultData.SetDatos, WSOrderTestsDS)

                                    Dim myOrderTestDAO As New TwksOrderTestsDAO()
                                    Dim myResultsDelegate As New ResultsDelegate
                                    For Each orderTest As WSOrderTestsDS.twksWSOrderTestsRow In myWSOrderTestsDS.twksWSOrderTests
                                        'Delete the Result for the OFF-SYSTEM Order Test
                                        resultData = myResultsDelegate.DeleteByOrderTestID(dbConnection, orderTest.OrderTestID)
                                        If (resultData.HasError) Then Exit For

                                        'Change the status of the OFF-SYSTEM OrderTest to OPEN to allow deleting it
                                        resultData = myOrderTestDAO.UpdateStatusByOrderTestID(dbConnection, orderTest.OrderTestID, "OPEN")
                                        If (resultData.HasError) Then Exit For
                                    Next
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'Remove the link between the specified Work Session and all Open Order Tests of the informed SampleClass
                                Dim myWSOrderTests As New WSOrderTestsDelegate
                                resultData = myWSOrderTests.DeleteWSOpenOTsBySampleClass(dbConnection, pWorkSessionID, pSampleClass)
                            End If

                            ' XB 26/04/2013 - Remove OrderTestsLISInfo before to delete OrderTests
                            If (Not resultData.HasError) Then
                                'Delete all Open Order Tests LIS Info of the specified Sample Class
                                If pSampleClass = "PATIENT" Or pSampleClass = "CTRL" Then
                                    Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate
                                    resultData = myOrderTestsLISInfoDelegate.DeleteOpenOrderTestsBySampleClass(dbConnection, pSampleClass)
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'Delete all Open Order Tests of the specified Sample Class
                                Dim myOrderTestDAO As New TwksOrderTestsDAO()
                                resultData = myOrderTestDAO.DeleteOpenOrderTestsBySampleClass(dbConnection, pSampleClass)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteOpenOrderTestsBySampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Function used in the screen of WS Preparation to manage the deletion of Patient Order Tests
        ''' </summary>
        ''' <param name="pDeletionType">Indicates how the list of Tests in the entry parameter pSelectedTestsDS have to be
        '''                             processed: when value is "IN_LIST" it means that all Patient Order Tests included in 
        '''                             pSelectedTestsDS have to be deleted; when value is "NOT_IN_LIST" it means that all 
        '''                             Patient Order Tests NOT included in pSelectedTestsDS have to be deleted</param>
        ''' <param name="pSelectedTestsDS">List of selected Tests</param>
        ''' <param name="pWSResultDS">WorkSessionResultDS containing the list of Patient Samples, Controls, Calibrators 
        '''                           and Blanks currently requested</param>
        ''' <param name="pSampleType">Code of SampleType selected. Optional parameter; informed only when pDeletionType=NOT_IN_LIST</param>
        ''' <param name="pSampleID">Selected PatientID/SampleID. Optional parameter; informed only when pDeletionType=NOT_IN_LIST</param>
        ''' <param name="pStatFlag">Priority of the selected Order.Optional parameter; informed only when pDeletionType=NOT_IN_LIST</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with table of Patient Samples updated after delete
        '''          all specified Patient Order Tests (and table BlankCalibrators updated after delete related Calibrators and Blanks 
        '''          when it was possible)</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2010
        ''' Modified by: SA 30/03/2010 - Added code to update value of field CreationOrder after delete Patient Samples
        '''              SA 30/04/2010 - Changed the way of verifying which Calibrators can be also deleted due to a performance problem
        '''                              when the number of patient order tests to delete is big
        '''              SA 14/05/2010 - Added management of deletion of Patient Order Tests with Calculated Tests
        '''              SA 21/10/2010 - Verify if the Order Test to delete is linked to a Profile to remove it from the rest of Order Tests 
        '''                              should be done for all Test Types, not only for Standard ones
        '''              SA 28/10/2010 - Delete also ISE and OFF-SYSTEM Tests. Check if Calibrators can be deleted only for Standard Tests
        '''              TR 11/03/2013 - When (pDeletionType=IN_LIST): add filter AndAlso LISRequest = FALSE in the first LINQ.
        '''              XB 02/12/2014 - Add functionality cases for ISE and OFFS tests included into a CALC test - BA-1867
        ''' </remarks>
        Public Function DeletePatientOrderTests(ByVal pDeletionType As String, ByVal pSelectedTestsDS As SelectedTestsDS, _
                                                ByVal pWSResultDS As WorkSessionResultDS, Optional ByVal pSampleType As String = "", _
                                                Optional ByVal pSampleID As String = "", Optional ByVal pStatFlag As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                'Dim deleteOrderTest As Boolean = False
                Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)

                If (pDeletionType = "NOT_IN_LIST") Then
                    'Get the SampleType, SampleID and StatFlag of the Patient Order to update
                    Dim sampleType As String = pSampleType
                    Dim sampleID As String = pSampleID
                    Dim statFlag As Boolean = pStatFlag

                    'Get the list of non positioned Order Tests for the specified SampleType / SampleID / StatFlag (in table Patients)
                    lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                      Where a.SampleClass = "PATIENT" _
                                    AndAlso a.SampleType = sampleType _
                                    AndAlso a.SampleID = sampleID _
                                    AndAlso a.StatFlag = statFlag _
                                    AndAlso a.OTStatus = "OPEN" _
                                     Select a).ToList()

                    Dim calcTestToVerify(-1) As String
                    For Each patientOrderTest In lstWSPatientsDS
                        'Get TestType and TestID of the current row
                        Dim testType As String = patientOrderTest.TestType
                        Dim testID As Integer = patientOrderTest.TestID

                        'Verify if the Test/SampleType is in the list of Selected Tests
                        Dim lstSelectedTestsDS As List(Of SelectedTestsDS.SelectedTestTableRow)
                        lstSelectedTestsDS = (From b In pSelectedTestsDS.SelectedTestTable _
                                             Where b.TestType = testType _
                                           AndAlso (Not b.IsTestIDNull AndAlso b.TestID = testID) _
                                           AndAlso b.SampleType = sampleType _
                                            Select b).ToList()

                        If (lstSelectedTestsDS.Count = 0) Then
                            If (testType = "CALC") Then
                                'The ID of the deleted Calculated Test is saved to verify also if for the same SampleID/StatFlag 
                                'but different SampleType, there are Tests linked to it 
                                ReDim Preserve calcTestToVerify(UBound(calcTestToVerify) + 1)
                                calcTestToVerify(UBound(calcTestToVerify)) = testID.ToString
                            End If

                            'Non positioned Patient Order Tests can always be deleted
                            patientOrderTest.Delete()

                            'Confirm changes done in the entry DataSet 
                            pWSResultDS.Patients.AcceptChanges()

                            'Only for Standard Tests...
                            If (testType = "STD") Then
                                'Verify if it is possible to delete also the Calibrator and Blank requested for the Test/SampleType
                                Dim mySelectedTestDS As New SelectedTestsDS
                                Dim selectedTestRow As SelectedTestsDS.SelectedTestTableRow

                                selectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow
                                selectedTestRow.TestType = testType
                                selectedTestRow.TestID = testID
                                selectedTestRow.SampleType = sampleType
                                mySelectedTestDS.SelectedTestTable.Rows.Add(selectedTestRow)

                                resultData = DeleteCalibratorOrderTests("IN_LIST", mySelectedTestDS, pWSResultDS)
                            End If
                        End If
                    Next

                    'Verify if there are Tests for which is needed to rebuild the list of Calculated Tests
                    If (UBound(calcTestToVerify) <> -1) Then
                        'Get the list of Order Tests linked to Calculated Tests for the same SampleID/StatFlag 
                        'but different SampleType
                        lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                          Where a.SampleClass = "PATIENT" _
                                        AndAlso a.SampleID = sampleID _
                                        AndAlso a.StatFlag = statFlag _
                                        AndAlso a.SampleType <> sampleType _
                                        AndAlso (Not a.IsCalcTestIDNull AndAlso a.CalcTestID <> "") _
                                         Select a).ToList()

                        For Each patientOrderTest In lstWSPatientsDS
                            'Get list of Calculated Tests in the current row
                            Dim finalIDList As String = ""
                            Dim finalNameList As String = ""

                            Dim calcIDs() As String = patientOrderTest.CalcTestID.Split(CChar(", "))
                            Dim calcNames() As String = patientOrderTest.CalcTestName.Split(CChar(", "))
                            For i As Integer = 0 To calcIDs.Count - 1
                                Dim toDelete As Boolean = False
                                For j As Integer = 0 To calcTestToVerify.Count - 1
                                    If (calcIDs(i) = calcTestToVerify(j)) Then
                                        toDelete = True
                                        Exit For
                                    End If
                                Next

                                If (Not toDelete) Then
                                    If (finalIDList <> "") Then
                                        finalIDList &= ", "
                                        finalNameList &= ", "
                                    End If
                                    finalIDList &= calcIDs(i)
                                    finalNameList &= calcNames(i)
                                End If
                            Next
                            patientOrderTest.CalcTestID = finalIDList
                            patientOrderTest.CalcTestName = finalNameList
                        Next
                    End If
                Else
                    If (pSelectedTestsDS.SelectedTestTable.Rows.Count = pWSResultDS.Patients.Rows.Count) Then
                        'All Patient rows have been selected to be deleted
                        pWSResultDS.Patients.Clear()
                        pWSResultDS.Patients.AcceptChanges()
                    Else
                        For Each selectedOrderTestRow In pSelectedTestsDS.SelectedTestTable
                            'Get the SampleID, StatFlag, SampleType, TestType and TestID of the current row
                            Dim sampleID As String = selectedOrderTestRow.SampleID
                            Dim statFlag As Boolean = selectedOrderTestRow.StatFlag
                            Dim sampleType As String = selectedOrderTestRow.SampleType
                            Dim testType As String = selectedOrderTestRow.TestType
                            Dim testID As Integer = selectedOrderTestRow.TestID

                            'Get the correspondent Order Test row in Patients table
                            lstWSPatientsDS = (From a In pWSResultDS.Patients _
                                              Where a.SampleClass = "PATIENT" _
                                                AndAlso a.SampleID = sampleID _
                                                AndAlso a.StatFlag = statFlag _
                                                AndAlso a.SampleType = sampleType _
                                                AndAlso a.TestType = testType _
                                                AndAlso a.TestID = testID _
                                                AndAlso a.OTStatus = "OPEN" _
                                                AndAlso a.LISRequest = False _
                                             Select a).ToList()

                            If (lstWSPatientsDS.Count = 1) Then
                                'Verify if the Patient Order Test to delete is linked to a Test Profile
                                If (Not lstWSPatientsDS(0).IsTestProfileIDNull) Then
                                    Dim testProfileID As Integer = lstWSPatientsDS(0).TestProfileID

                                    Dim lstSameTestProfileDS As List(Of WorkSessionResultDS.PatientsRow)
                                    lstSameTestProfileDS = (From b In pWSResultDS.Patients _
                                                           Where b.SampleClass = "PATIENT" _
                                                         AndAlso b.SampleID = sampleID _
                                                         AndAlso b.StatFlag = statFlag _
                                                         AndAlso (Not b.IsTestProfileIDNull AndAlso b.TestProfileID = testProfileID) _
                                                          Select b).ToList()

                                    'Remove the Test Profile information for the rest of the profile Tests
                                    For Each testInProfileRow In lstSameTestProfileDS
                                        testInProfileRow.SetTestProfileIDNull()
                                        testInProfileRow.SetTestProfileNameNull()
                                    Next
                                End If

                                ' XB 02/12/2014 - BA-1867
                                'If (testType = "STD") Then
                                If (testType = "STD") OrElse (testType = "ISE") OrElse (testType = "OFFS") Then
                                    ' XB 02/12/2014 - BA-1867
                                    'Verify if the Patient Order Test to delete is linked to a Calculated Test
                                    If (Not lstWSPatientsDS(0).IsCalcTestIDNull AndAlso lstWSPatientsDS(0).CalcTestID <> "") Then
                                        'Get the list of Calculated Tests in which the Test is included
                                        Dim calcIDs() As String = lstWSPatientsDS(0).CalcTestID.Split(CChar(", "))
                                        For i As Integer = 0 To calcIDs.Length - 1
                                            'Delete the Calculated Test
                                            DeleteCalculatedTest(sampleID, statFlag, Convert.ToInt32(calcIDs(i)), pWSResultDS)
                                        Next
                                    End If

                                    'Non positioned Patient Order Tests can always be deleted
                                    lstWSPatientsDS(0).Delete()

                                ElseIf (testType = "CALC") Then
                                    'Remove all links
                                    DeleteCalculatedTest(sampleID, statFlag, testID, pWSResultDS)

                                    ' XB 02/12/2014 - BA-1867
                                    'ElseIf (testType = "ISE") Then
                                    '    'Non positioned Patient Order Tests can always be deleted
                                    '    lstWSPatientsDS(0).Delete()

                                    'ElseIf (testType = "OFFS") Then
                                    '    'Non positioned Patient Order Tests can always be deleted
                                    '    lstWSPatientsDS(0).Delete()
                                    ' XB 02/12/2014 - BA-1867
                                End If

                                'Confirm changes done in the entry DataSet 
                                pWSResultDS.Patients.AcceptChanges()
                            End If
                        Next
                    End If

                    'Get the list of different Test/SampleType in the list of Patient Order Tests deleted
                    'to verify if the correspondent Calibrator can be also deleted 
                    Dim lstTestsList As List(Of String)
                    lstTestsList = (From a In pSelectedTestsDS.SelectedTestTable _
                                   Where a.TestType = "STD" _
                                   Select a.TestType & "|" & a.TestID & "|" & a.SampleType).Distinct.ToList

                    For Each differentElement As String In lstTestsList
                        Dim mydiffElemParts() As String = differentElement.Split(CChar("|"))
                        Dim mySelectedTestDS As New SelectedTestsDS
                        Dim selectedTestRow As SelectedTestsDS.SelectedTestTableRow

                        selectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow
                        selectedTestRow.TestType = mydiffElemParts(0)
                        selectedTestRow.TestID = Convert.ToInt32(mydiffElemParts(1))
                        selectedTestRow.SampleType = mydiffElemParts(2)
                        mySelectedTestDS.SelectedTestTable.Rows.Add(selectedTestRow)

                        resultData = DeleteCalibratorOrderTests("IN_LIST", mySelectedTestDS, pWSResultDS)
                    Next
                End If
                lstWSPatientsDS = Nothing

                'Rewrite value of CreationOrder when needed
                SortPatients(pWSResultDS)

                'Confirm changes done in the entry DataSet 
                pWSResultDS.Patients.AcceptChanges()

                'Return the updated pWSResultDS in the GlobalDataTO
                resultData.HasError = False
                resultData.SetDatos = pWSResultDS

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeletePatientOrderTests", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of Blanks and Calibrators that have been not included in a WorkSession (those
        ''' with Status OPEN) and optionally, also the list of Order Tests already linked to the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Blanks or Calibrators
        ''' <param name="pReturnOpenAsSelected">Optional parameter to indicate if OPEN Blanks and Calibrators have to be returned as SELECTED
        '''                                     (when its value is TRUE) or UNSELECTED (when its value is FALSE)</param>
        '''          obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 26/04/2010 - Parameter pWorkSessionID changed from optional to fixed
        '''              SA 01/09/2010 - For Blank and Calibrator Order Tests having previous results, the previous result
        '''                              is always shown, and Selected and New checks depends on value of field ToSendFlag
        '''              SA 24/11/2010 - For Calibrator Order Tests having previous results, inform fields OriginalFactorValue
        '''                              and FactorValue in the following way: if ManualResultFlag is False, get value of 
        '''                              CalibratorFactor; otherwise, get value of ManualResult
        '''              TR 05/08/2011 - Do not declare variables inside loops; set to nothing lists and Byte arrays to save memory
        '''              SA 29/08/2012 - Added new parameter for the Analyzer Identifier
        '''              SA 04/04/2013 - Added new optional parameter to indicate if OPEN Blanks and Calibrators have to be returned as SELECTED.
        '''                              Optional parameter with TRUE as default value
        ''' </remarks>
        Public Function GetBlankCalibOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                ByVal pAnalyzerID As String, Optional ByVal pReturnOpenAsSelected As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'Get Blanks and Calibrators
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO

                        resultData = myOrderTestsDAO.GetBlankCalibOrderTests(dbConnection, pWorkSessionID, pReturnOpenAsSelected)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim wsResultDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)

                            If (wsResultDS.BlankCalibrators.Rows.Count > 0) Then
                                'Process for Blanks... get the additional information 
                                Dim lstWSBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                                lstWSBlanksDS = (From a In wsResultDS.BlankCalibrators _
                                                Where String.Equals(a.SampleClass, "BLANK") _
                                               Select a).ToList()

                                If (lstWSBlanksDS.Count > 0) Then
                                    'Get full path for the Icon defined for Blanks
                                    Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                                    Dim imageBytes As Byte() = preloadedDataConfig.GetIconImage("BLANK")

                                    Dim blankResultDS As ResultsDS
                                    Dim lastResultsDelegate As New ResultsDelegate
                                    Dim lastHisResultsDelegate As New HisWSResultsDelegate

                                    For Each blankOrderTest In lstWSBlanksDS
                                        blankOrderTest.SampleClassIcon = imageBytes

                                        'A previous result will be used for the Blank...
                                        If (Not blankOrderTest.IsPreviousOrderTestIDNull) Then
                                            resultData = lastResultsDelegate.GetAcceptedResults(dbConnection, blankOrderTest.PreviousOrderTestID)

                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                blankResultDS = DirectCast(resultData.SetDatos, ResultsDS)

                                                If (blankResultDS.twksResults.Rows.Count = 1) Then
                                                    'If the Blank Order Test has ToSendFlag=1, it has to be shown as Selected and New
                                                    blankOrderTest.Selected = blankOrderTest.ToSendFlag
                                                    blankOrderTest.NewCheck = blankOrderTest.ToSendFlag

                                                    blankOrderTest.ABSValue = blankResultDS.twksResults(0).ABSValue.ToString
                                                    blankOrderTest.ABSDateTime = blankResultDS.twksResults(0).ResultDateTime
                                                End If
                                            End If
                                        End If
                                    Next
                                    imageBytes = Nothing
                                End If
                                lstWSBlanksDS = Nothing

                                'Process for Calibrators... get the additional information and manage the special case 
                                'of Alternative Calibrators
                                Dim lstWSCalibDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                                lstWSCalibDS = (From a In wsResultDS.BlankCalibrators _
                                               Where String.Equals(a.SampleClass, "CALIB") _
                                              Select a).ToList()

                                If (lstWSCalibDS.Count > 0) Then
                                    'Get full path for the Icon defined for Calibrators
                                    Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                                    Dim imageBytes As Byte() = preloadedDataConfig.GetIconImage("CALIB")

                                    Dim calibratorDS As TestSampleCalibratorDS
                                    Dim myTestCalibratorsDelegate As New TestCalibratorsDelegate

                                    Dim myOrderTestsDS As OrderTestsDS
                                    Dim myOrderTestsDelegate As New OrderTestsDelegate

                                    Dim calibResultDS As ResultsDS
                                    Dim lastResultsDelegate As New ResultsDelegate
                                    Dim lastHisResultsDelegate As New HisWSResultsDelegate

                                    Dim orderTestID As Integer = 0
                                    For Each calibOrderTest In lstWSCalibDS
                                        calibOrderTest.SampleClassIcon = imageBytes

                                        'Get data of the Experimental Calibrator defined for the Test and Sample Type
                                        resultData = myTestCalibratorsDelegate.GetTestCalibratorData(dbConnection, calibOrderTest.TestID, _
                                                                                                     calibOrderTest.SampleType)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            calibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                                            If (calibratorDS.tparTestCalibrators.Rows.Count = 1) Then
                                                calibOrderTest.CalibratorID = calibratorDS.tparTestCalibrators(0).CalibratorID
                                                calibOrderTest.CalibratorName = calibratorDS.tparTestCalibrators(0).CalibratorName
                                                calibOrderTest.LotNumber = calibratorDS.tparTestCalibrators(0).LotNumber
                                                calibOrderTest.NumberOfCalibrators = calibratorDS.tparTestCalibrators(0).NumberOfCalibrators
                                            End If
                                        End If

                                        'Verify if it is the Alternative Calibrator of a different SampleType for the same Test
                                        orderTestID = calibOrderTest.OrderTestID

                                        resultData = myOrderTestsDelegate.ReadByAlternativeOrderTestID(dbConnection, orderTestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                                            If (myOrderTestsDS.twksOrderTests.Rows.Count = 0) Then
                                                'Requested Sample Type and Sample Type have the same value 
                                                calibOrderTest.RequestedSampleTypes = calibOrderTest.SampleType
                                            Else
                                                'Requested Sample Type will be the list of Sample Types of all Order Tests having the Order Test 
                                                'in process as alternative Calibrator
                                                Dim previousSampleType As String = ""
                                                For Each alternativSampleType As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests
                                                    If (alternativSampleType.SampleType <> previousSampleType) Then
                                                        calibOrderTest.RequestedSampleTypes += alternativSampleType.SampleType + " "
                                                        previousSampleType = alternativSampleType.SampleType
                                                    End If
                                                Next

                                                'Remove last " " from the built list of Requested Sample Types
                                                calibOrderTest.RequestedSampleTypes = calibOrderTest.RequestedSampleTypes.Trim
                                            End If
                                        End If

                                        'A previous result will be used for the Calibrator...
                                        If (Not calibOrderTest.IsPreviousOrderTestIDNull) Then
                                            resultData = lastResultsDelegate.GetAcceptedResults(dbConnection, calibOrderTest.PreviousOrderTestID)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                calibResultDS = DirectCast(resultData.SetDatos, ResultsDS)

                                                If (calibResultDS.twksResults.Rows.Count > 0) Then
                                                    'If the Calibrator Order Test has ToSendFlag=1, it has to be shown as Selected and New
                                                    calibOrderTest.Selected = calibOrderTest.ToSendFlag
                                                    calibOrderTest.NewCheck = calibOrderTest.ToSendFlag

                                                    calibOrderTest.ABSValue = calibResultDS.twksResults(0).ABSValue.ToString
                                                    calibOrderTest.ABSDateTime = calibResultDS.twksResults(0).ResultDateTime

                                                    'Only for Single Points Calibrators, get FactorValue and inform it
                                                    If (calibResultDS.twksResults.Rows.Count = 1) Then
                                                        If (Not calibResultDS.twksResults(0).IsCalibratorFactorNull) Then
                                                            calibOrderTest.OriginalFactorValue = Convert.ToSingle(calibResultDS.twksResults(0).CalibratorFactor.ToString(GlobalConstants.CALIBRATOR_FACTOR_FORMAT))
                                                            calibOrderTest.FactorValue = Convert.ToSingle(calibResultDS.twksResults(0).CalibratorFactor.ToString(GlobalConstants.CALIBRATOR_FACTOR_FORMAT))

                                                            'FactorValue has been manually changed...
                                                            If (Not calibResultDS.twksResults(0).IsManualResultNull) Then
                                                                calibOrderTest.FactorValue = Convert.ToSingle(calibResultDS.twksResults(0).ManualResult.ToString(GlobalConstants.CALIBRATOR_FACTOR_FORMAT))
                                                            End If
                                                        End If
                                                    End If

                                                    'Multipoint Calibrator
                                                    For i As Integer = 1 To calibResultDS.twksResults.Rows.Count - 1
                                                        '... ABS Values are linked separating them by <Return>
                                                        calibOrderTest.ABSValue += Environment.NewLine + calibResultDS.twksResults(i).ABSValue.ToString
                                                    Next
                                                End If
                                            End If
                                        End If
                                    Next
                                    imageBytes = Nothing
                                End If
                                lstWSCalibDS = Nothing
                            End If

                            'Confirm changes done in the table
                            wsResultDS.BlankCalibrators.AcceptChanges()

                            'Return the updated DataSet
                            resultData.HasError = False
                            resultData.SetDatos = wsResultDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetBlankCalibOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the different Blank Modes (different of Only Reagent) for a group of Order Tests included in a WorkSession, 
        ''' having ToSendFlag=True and belonging to Orders with SampleClass BLANK. 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Transaction</param>
        ''' <param name="pOrderTestsList">List of Order Tests Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Blank Modes that are required in a 
        '''          WorkSession for the informed list of Order Tests</returns>
        ''' <remarks>
        ''' Created by:  RH 08/06/2011
        ''' </remarks>
        Public Function GetBlanksData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                        Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testControlsData As New TwksOrderTestsDAO
                        resultData = testControlsData.GetBlanksData(dbConnection, pOrderTestsList, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetBlanksData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all Experimental Calibrators required for a group of Order Tests included in a WorkSession having ToSendFlag=True 
        ''' and belonging to Orders with SampleClass Calibrator. If several Test/SampleType uses the same Calibrator it will be returned only once
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Test Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS with the list of Experimental 
        '''          Calibrators that are required in a WorkSession for the informed list of Order Tests. For each 
        '''          Calibrator, the number of Points is also returned</returns>
        ''' <remarks>
        ''' Modified by: SA 03/03/2010 - Changes to update function to the new template for DB Connection and data to return
        '''              SA 03/05/2010 - Added optional parameter WorkSessionID
        '''              SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function GetCalibrationData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                           Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testCalibrationData As New TwksOrderTestsDAO
                        resultData = testCalibrationData.GetCalibrationData(dbConnection, pOrderTestsList, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetCalibrationData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of Controls that have been not included in a WorkSession (those
        ''' with Status OPEN) and optionally, also the list of Order Tests already linked to the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Controls obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 03/03/2010
        ''' Modified by: SA 26/04/2010 - Parameter pWorkSessionID changed from optional to fixed
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 18/06/2012 - For each returned Control OrderTest, load the icon according the TestType in the DS to return
        '''              TR 18/02/2013 - Send the connection to the GetIconImage for the update process is needed.
        '''              TR 12/03/2013 - Set the icon image to LIS Request Control.
        ''' </remarks>
        Public Function GetControlOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        resultData = myOrderTestsDAO.GetControlOrderTests(dbConnection, pWorkSessionID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim wsResultDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)

                            'Get Icon Image byte array for Test Type
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate

                            Dim bTestTypeIconSTD As Byte() = Nothing
                            Dim bTestTypeIconISE As Byte() = Nothing
                            Dim LISRequestIcon As Byte() = Nothing
                            bTestTypeIconSTD = preloadedDataConfig.GetIconImage("FREECELL", dbConnection)
                            bTestTypeIconISE = preloadedDataConfig.GetIconImage("TISE_SYS", dbConnection)
                            LISRequestIcon = preloadedDataConfig.GetIconImage("LISREQUEST", dbConnection)
                            wsResultDS.Controls.BeginInit()
                            For Each controlOT As WorkSessionResultDS.ControlsRow In wsResultDS.Controls
                                'Set the proper Icon according the TestType
                                If String.Equals(controlOT.TestType, "STD") Then
                                    controlOT.TestTypeIcon = bTestTypeIconSTD
                                ElseIf String.Equals(controlOT.TestType, "ISE") Then
                                    controlOT.TestTypeIcon = bTestTypeIconISE
                                End If
                                'TR 12/03/2013
                                If controlOT.LISRequest Then
                                    controlOT.LISRequestIcon = LISRequestIcon
                                    If (controlOT.Selected AndAlso controlOT.OTStatus = "OPEN") Then controlOT.Selected = False
                                Else
                                    controlOT.LISRequestIcon = bTestTypeIconSTD
                                End If


                            Next
                            wsResultDS.Controls.AcceptChanges()

                            resultData.SetDatos = wsResultDS
                            resultData.HasError = False

                            bTestTypeIconSTD = Nothing
                            bTestTypeIconISE = Nothing
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetControlOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all Controls required for a group of Order Tests included in a WorkSession having ToSendFlag=True 
        ''' and belonging to Orders with SampleClass Control. If several Test/SampleType uses the same Control, it will be 
        ''' returned only once
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Transaction</param>
        ''' <param name="pOrderTestsList">List of Order Tests Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestControlsDS with the list of Controls that are required in a 
        '''          WorkSession for the informed list of Order Tests</returns>
        ''' <remarks>
        ''' Modified by: VR 16/12/2009 - Changed the Return Type TestControlsDS to GlobalDataTO
        '''              SA 03/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 03/05/2010 - Add parameter for WorkSession Identifier
        '''              SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function GetControlsData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                        Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data of the Controls defined for the different pairs of Test/SampleType
                        Dim testControlsData As New TwksOrderTestsDAO
                        resultData = testControlsData.GetControlsData(dbConnection, pOrderTestsList, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetControlData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified OrderTestID, get the identifier of its related required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with data of the related required Element</returns>
        ''' <remarks>
        ''' Created by:  SA 31/01/2012
        ''' </remarks>
        Public Function GetElementByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        myGlobalDataTO = myOrderTestsDAO.GetElementByOrderTestID(dbConnection, pWorkSessionID, pAnalyzerID, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetElementByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all not CLOSED Order Tests that need the specified required Element for their execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Identifier of a WS Required Element</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of not CLOSED Order Tests that need the specified required 
        '''          Element for their execution</returns>
        ''' <remarks>
        ''' Created by: TR 21/11/2103 - BT #1388
        ''' </remarks>
        Public Function GetNotClosedOrderTestByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testControlsData As New TwksOrderTestsDAO
                        resultData = testControlsData.GetNotClosedOrderTestByElementID(dbConnection, pElementID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetNotClosedOrderTestByElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of PatientID/SampleID in the active WS and having at least an OrderTest with an accepted Result. This function is used for Reports
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrdersDS with the list of PatientID/SampleID with at least an accepted Result in the active WS</returns>
        ''' <remarks>
        ''' Created by:  JV 31/10/2013 - BT #1226
        ''' </remarks>
        Public Function GetOrdersOKByUser(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TwksOrderTestsDAO
                        resultData = myDAO.GetOrdersOKByUser(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetOrdersOKByUser", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Order Test
        ''' </summary>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderDetailsDS with details of the specified Order Test</returns>
        ''' <remarks>
        ''' Modified by: DL 10/05/2010 - Added parameter for the DB Connection 
        '''              SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function GetOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim myGlobal As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobal.HasError) AndAlso (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestDAO As New TwksOrderTestsDAO()
                        myGlobal = myOrderTestDAO.Read(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                myGlobal = New GlobalDataTO
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetOrderTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Get all Order Tests included in the specified Order and having the specified Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pOrderTestStatus">Status of the OrderTests to get</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of Order Tests included in the specified Order
        '''          and having the specified Status</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        '''              AG 22/05/2012 - Added parameter for the Status of the OrderTests 
        ''' </remarks>
        Public Function GetOrderTestByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, ByVal pOrderTestStatus As String) As GlobalDataTO
            Dim myGlobal As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobal.HasError) AndAlso (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestDAO As New TwksOrderTestsDAO()
                        myGlobal = myOrderTestDAO.GetOrderTestByOrderID(dbConnection, pOrderID, pOrderTestStatus)
                    End If
                End If
            Catch ex As Exception
                myGlobal = New GlobalDataTO
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetOrderTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Verify if for an specific Calibrator Order, there is an Order Test for the informed Test and Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pTestType">Code of the Test Type</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Code of the Sample Type</param>
        ''' <returns>GlobalDataTO containing an integer value with the identifier of the Order Test for the
        '''          Test and requested Sample Type; if the Order Test does not exist then return Null</returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function GetOrderTestByTestAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, _
                                                        ByVal pTestType As String, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        resultData = myOrderTestsDAO.GetOrderTestByTestAndSampleType(dbConnection, pOrderID, pTestType, pTestID, pSampleType)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetOrderTestByTestAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if in the active WorkSession there are requested Order Tests (for whatever SampleClass) for the informed TestType and
        ''' TestID. Closed Blanks and Calibrators that remain in table twksOrderTests are automatically excluded from the searching by 
        ''' adding the INNER JOIN with table twksWSOrderTests  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with all the Order Tests for the informed TestType and TestID in
        '''          the active Work Session</returns>
        ''' <remarks>
        ''' Created by:  AG 08/05/2013
        ''' Modified by: SA 09/05/2013 - Added parameters to inform TestType and TestID and filter the query also by these values 
        ''' Modified by: AG 18/09/2014 - BA-1869 change pTestID to an optional parameter
        ''' </remarks>
        Public Function GetOrderTestsByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pTestType As String, _
                                                   Optional ByVal pTestID As Integer = -1) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TwksOrderTestsDAO
                        resultData = myDAO.GetOrderTestsByWorkSession(dbConnection, pWorkSessionID, pTestType, pTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetOrderTestsByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Order Tests of Patients that have been not included in a WorkSession (those
        ''' with Status OPEN) and optionally, also the list of Order Tests already linked to the specified Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pReturnOpenAsSelected">Optional parameter to indicate if OPEN Patient Samples have to be returned as SELECTED
        '''                                     (when its value is TRUE) or UNSELECTED (when its value is FALSE)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WorkSessionResultDS with all the Controls obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 03/03/2010
        ''' Modified by: SA 26/04/2010 - Parameter pWorkSessionID changed from optional to fixed
        '''              SA 21/10/2010 - Get and assign the ICON for ISE Tests
        '''              SA 02/12/2010 - Get and assign the ICON for Off-System Tests
        '''              SA 18/01/2011 - For each requested Off-System Test, verify if a result has been informed and in this
        '''                              case, add it to the DataSet to return
        '''              TR 05/08/2011 - Do not declare variables inside loops; set to nothing lists and Byte arrays to save memory
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 02/08/2012 - Inform optional parameter TestType when calling function GetAcceptedResults to verify if there is a
        '''                              result for a requested OffSystem Test
        '''              SA 04/04/2013 - Added new optional parameter to indicate if OPEN Patient Samples have to be returned as SELECTED.
        '''                              Optional parameter with TRUE as default value  
        '''              DL 13/06/2013 - For Tests included in the Formula of a Calculated Test, build the list of Specimen IDs and inform the 
        '''                              corresponding fields in the DS to return
        '''              SA 28/08/2013 - For Order Tests not requested by LIS, verify if the required Patient Sample is positioned in a tube 
        '''                              with a valid Barcode and in this case inform field SpecimenID in the DS to return. Fixed an error in 
        '''                              the process of informing the SpecimenID list for Calculated Tests
        ''' </remarks>
        Public Function GetPatientOrderTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                             Optional ByVal pReturnOpenAsSelected As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get Patient Samples
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        resultData = myOrderTestsDAO.GetPatientOrderTests(dbConnection, pWorkSessionID, pReturnOpenAsSelected)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim wsResultDS As WorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)

                            'Get Icon Image byte array for Patient's priority and Test Type
                            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                            Dim bPatientStat As Byte() = Nothing
                            Dim bPatientNotStat As Byte() = Nothing

                            bPatientStat = preloadedDataConfig.GetIconImage("STATS")
                            bPatientNotStat = preloadedDataConfig.GetIconImage("ROUTINES")

                            Dim bTestTypeIconSTD As Byte() = Nothing
                            Dim bTestTypeIconCALC As Byte() = Nothing
                            Dim bTestTypeIconISE As Byte() = Nothing
                            Dim bTestTypeIconOFF As Byte() = Nothing
                            Dim LISRequestIcon As Byte() = Nothing

                            bTestTypeIconSTD = preloadedDataConfig.GetIconImage("FREECELL")
                            bTestTypeIconCALC = preloadedDataConfig.GetIconImage("TCALC")
                            bTestTypeIconISE = preloadedDataConfig.GetIconImage("TISE_SYS")
                            bTestTypeIconOFF = preloadedDataConfig.GetIconImage("TOFF_SYS")
                            LISRequestIcon = preloadedDataConfig.GetIconImage("LISREQUEST", dbConnection)

                            Dim myOFFSResult As New ResultsDS
                            Dim myResultsDelegate As New ResultsDelegate

                            For Each patientRow As WorkSessionResultDS.PatientsRow In wsResultDS.Patients
                                'Set value of field SampleIDType...
                                If (Not IsDBNull(patientRow("PatientID"))) Then
                                    patientRow.SampleIDType = "DB"
                                    patientRow.SampleID = patientRow("PatientID").ToString

                                ElseIf (patientRow.SampleID <> "" AndAlso Left(patientRow.SampleID, 1) <> "#") Then
                                    patientRow.SampleIDType = "MAN"
                                Else
                                    patientRow.SampleIDType = "AUTO"
                                End If

                                'Set the proper Icon according value of the StatFlag
                                If patientRow.StatFlag Then
                                    patientRow.SampleClassIcon = bPatientStat
                                Else
                                    patientRow.SampleClassIcon = bPatientNotStat
                                End If

                                'Set the proper Icon according the TestType
                                If String.Equals(patientRow.TestType, "STD") Then
                                    patientRow.TestTypeIcon = bTestTypeIconSTD
                                ElseIf String.Equals(patientRow.TestType, "CALC") Then
                                    patientRow.TestTypeIcon = bTestTypeIconCALC
                                ElseIf String.Equals(patientRow.TestType, "ISE") Then
                                    patientRow.TestTypeIcon = bTestTypeIconISE
                                ElseIf String.Equals(patientRow.TestType, "OFFS") Then
                                    patientRow.TestTypeIcon = bTestTypeIconOFF
                                End If

                                'For Off-System Tests, search if there is a Result Value informed
                                If String.Equals(patientRow.TestType, "OFFS") Then
                                    resultData = myResultsDelegate.GetAcceptedResults(dbConnection, patientRow.OrderTestID, True, True, "OFFS")

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myOFFSResult = DirectCast(resultData.SetDatos, ResultsDS)

                                        If (myOFFSResult.twksResults.Rows.Count > 0) Then
                                            If (Not myOFFSResult.twksResults(0).IsManualResultNull) Then
                                                patientRow.OffSystemResult = myOFFSResult.twksResults(0).ManualResult.ToString
                                            ElseIf (Not myOFFSResult.twksResults(0).IsManualResultTextNull) Then
                                                patientRow.OffSystemResult = myOFFSResult.twksResults(0).ManualResultText.ToString
                                            End If
                                        End If
                                    Else
                                        'Error verifying if there is a result informed for the requested Off-System Tests
                                        Exit For
                                    End If
                                End If

                                If (patientRow.LISRequest) Then
                                    patientRow.LISRequestIcon = LISRequestIcon
                                    If (patientRow.Selected AndAlso patientRow.OTStatus = "OPEN") Then patientRow.Selected = False
                                Else
                                    patientRow.LISRequestIcon = bTestTypeIconSTD
                                End If
                            Next

                            Dim lstPatientOT As List(Of WorkSessionResultDS.PatientsRow)

                            'SA 28/08/2013 - See comment about this code in the function header
                            If (Not resultData.HasError) Then
                                'Verify if there are Patient Order Tests in the WorkSession added manually
                                lstPatientOT = (From a As WorkSessionResultDS.PatientsRow In wsResultDS.Patients _
                                               Where a.LISRequest = False _
                                            Order By a.SampleID, a.SampleType _
                                              Select a).ToList

                                If (lstPatientOT.Count > 0) Then
                                    'Get the list of required Patient Samples that have been sent by an external LIS system
                                    Dim myWSReqElemDelegate As New WSRequiredElementsDelegate
                                    resultData = myWSReqElemDelegate.GetLISPatientElements(dbConnection, pWorkSessionID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim myDataSet As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                        If (myDataSet.twksWSRequiredElements.Rows.Count > 0) Then
                                            'For each manual Patient Order Tests, get the SpecimenID sent by LIS for it (if any)
                                            Dim lstSpecimenID As List(Of WSRequiredElementsDS.twksWSRequiredElementsRow)
                                            For Each manualOT As WorkSessionResultDS.PatientsRow In lstPatientOT
                                                lstSpecimenID = (From b As WSRequiredElementsDS.twksWSRequiredElementsRow In myDataSet.twksWSRequiredElements _
                                                                Where b.PatientID = manualOT.SampleID _
                                                              AndAlso b.SampleType = manualOT.SampleType _
                                                               Select b).ToList
                                                If (lstSpecimenID.Count > 0) Then manualOT.SpecimenID = lstSpecimenID.First.SpecimenIDList.Trim.Split(CChar(vbCrLf))(0)
                                            Next
                                            lstSpecimenID = Nothing
                                        End If
                                    End If
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'Get information of tests included in the formula of Calculated Tests in the WorkSession to inform fields CalcTestID 
                                'and CalcTestName for each one of them. Inform also the SpecimenID list for each one of them.
                                Dim myCalcOrderTestsDS As OrderCalculatedTestsDS
                                Dim myCalcOrderTestsDelegate As New OrderCalculatedTestsDelegate

                                resultData = myCalcOrderTestsDelegate.ReadByWorkSession(dbConnection, pWorkSessionID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myCalcOrderTestsDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)

                                    Dim calcIDList As String = ""
                                    Dim calcNameList As String = ""
                                    Dim calcSpecimenList As String = ""
                                    Dim currentOrderTest As Integer = -1
                                    Dim lstOTCalc As List(Of OrderCalculatedTestsDS.twksOrderCalculatedTestsRow)

                                    For Each otInCalcTest As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myCalcOrderTestsDS.twksOrderCalculatedTests
                                        If (otInCalcTest.OrderTestID <> currentOrderTest) Then
                                            If (currentOrderTest <> -1) Then
                                                lstPatientOT = (From a As WorkSessionResultDS.PatientsRow In wsResultDS.Patients _
                                                                Where a.OrderTestID = currentOrderTest _
                                                                Select a).ToList

                                                If (lstPatientOT.Count = 1) Then
                                                    lstPatientOT.First.CalcTestID = calcIDList
                                                    lstPatientOT.First.CalcTestName = calcNameList

                                                    If (lstPatientOT.First.IsSpecimenIDNull OrElse lstPatientOT.First.SpecimenID = String.Empty) Then
                                                        lstPatientOT.First.SpecimenID = calcSpecimenList
                                                    End If
                                                End If

                                                calcIDList = String.Empty
                                                calcNameList = String.Empty
                                                calcSpecimenList = String.Empty
                                            End If
                                            currentOrderTest = otInCalcTest.OrderTestID
                                        End If

                                        If (calcIDList.Trim <> String.Empty) Then
                                            calcIDList &= ", "
                                            calcNameList &= ", "

                                            'Add new compare sentence, if calcSpecimenList is not empty
                                            If (otInCalcTest.SpecimenID <> String.Empty) Then
                                                If (Not calcSpecimenList.Contains(otInCalcTest.SpecimenID) AndAlso calcSpecimenList.Trim <> String.Empty) Then calcSpecimenList &= ", "
                                            End If
                                        End If
                                        calcIDList &= otInCalcTest.CalcTestID
                                        calcNameList &= otInCalcTest.CalcTestLongName

                                        If (otInCalcTest.SpecimenID <> String.Empty) Then
                                            If (Not calcSpecimenList.Contains(otInCalcTest.SpecimenID)) Then calcSpecimenList &= otInCalcTest.SpecimenID
                                        Else
                                            'If the SpecimenID is empty, verify if the Calculated Test is included in the formula of another Calculated Test requested by LIS
                                            lstOTCalc = (From b As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myCalcOrderTestsDS.twksOrderCalculatedTests _
                                                        Where b.OrderTestID = otInCalcTest.CalcOrderTestID _
                                                  AndAlso Not String.IsNullOrEmpty(b.SpecimenID) _
                                                       Select b).ToList()

                                            If (lstOTCalc.Count > 0) Then
                                                If (Not calcSpecimenList.Contains(lstOTCalc.First.SpecimenID)) Then calcSpecimenList &= lstOTCalc.First.SpecimenID
                                            End If
                                        End If
                                    Next

                                    '...process the last Order Test
                                    If (Not String.Equals(calcIDList.Trim, String.Empty)) Then
                                        lstPatientOT = (From a In wsResultDS.Patients _
                                                       Where a.OrderTestID = currentOrderTest _
                                                      Select a).ToList

                                        If (lstPatientOT.Count = 1) Then
                                            lstPatientOT.First.CalcTestID = calcIDList
                                            lstPatientOT.First.CalcTestName = calcNameList

                                            If (lstPatientOT.First.IsSpecimenIDNull OrElse lstPatientOT.First.SpecimenID = String.Empty) Then
                                                lstPatientOT.First.SpecimenID = calcSpecimenList
                                            End If
                                        End If
                                    End If
                                    lstPatientOT = Nothing
                                End If
                            End If

                            'Set value to nothing to avoid memory use
                            bPatientStat = Nothing
                            bPatientNotStat = Nothing
                            bTestTypeIconSTD = Nothing
                            bTestTypeIconCALC = Nothing
                            bTestTypeIconISE = Nothing
                            bTestTypeIconOFF = Nothing

                            If (Not resultData.HasError) Then
                                wsResultDS.Patients.AcceptChanges()
                                resultData.SetDatos = wsResultDS
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetPatientOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified OrderTestID, get the limits of the Reference Range that should be used to validate 
        ''' a result for the TestID/SampleType (depending on the Range Type and the data informed for the Patient 
        ''' for which the Order Test was requested)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test</param>
        ''' <param name="pTestType">Type of Test</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pRangeType">Type of ReferenceRange defined for the Test/SampleType</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with the lower and upper limits of the 
        '''          Reference Range; if a Reference Range is not found, then both values are set to -1</returns>
        ''' <remarks>
        ''' Created by:  SA 24/01/2011 - Moved here from CalculationsDelegate Class to allow call it from classes in 
        '''                              WorkSessions module. Some changes to improve the original function, fix
        '''                              uncontrolled exceptions and get minimum and maximum in the same function 
        '''                              (replace functions GetMinReferenceRange and GetMaxReferenceRange in 
        '''                              CalculationsDelegate)
        ''' Modified by: SA 01/09/2011 - Changed the function template
        '''              SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
        ''' </remarks>
        Public Function GetReferenceRangeInterval(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                  ByVal pTestType As String, ByVal pTestID As Integer, ByVal pSampleType As String, _
                                                  ByVal pRangeType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the Reference Ranges of the specified type defined for the TestType/TestID/SampleType
                        Dim myTestRefRangesDelegate As New TestRefRangesDelegate()
                        resultData = myTestRefRangesDelegate.ReadByTestID(dbConnection, pTestID, pSampleType, pRangeType, pTestType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myTestRefRanges As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                            If (myTestRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                                Dim minRangeValue As Single = -1
                                Dim maxRangeValue As Single = -1
                                Dim minBorderLineRangeValue As Single = -1
                                Dim maxBorderLineRangeValue As Single = -1

                                Select Case (pRangeType)
                                    Case "GENERIC"
                                        If (Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalLowerLimitNull) AndAlso _
                                           (Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalUpperLimitNull) Then
                                            minRangeValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalLowerLimit
                                            maxRangeValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalUpperLimit
                                        End If

                                        If (Not myTestRefRanges.tparTestRefRanges.Item(0).IsBorderLineLowerLimitNull) AndAlso _
                                           (Not myTestRefRanges.tparTestRefRanges.Item(0).IsBorderLineUpperLimitNull) Then
                                            minBorderLineRangeValue = myTestRefRanges.tparTestRefRanges.Item(0).BorderLineLowerLimit
                                            maxBorderLineRangeValue = myTestRefRanges.tparTestRefRanges.Item(0).BorderLineUpperLimit
                                        End If
                                    Case "DETAILED"
                                        'Get data of the PatientID for which the informed Order Test was requested
                                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                                        resultData = myOrderTestsDAO.GetOTPatientData(dbConnection, pOrderTestID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myPatientsDS As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)

                                            If (myPatientsDS.tparPatients.Rows.Count = 1) Then
                                                'Verify if there are Ranges defined by Gender 
                                                Dim qGenderRanges As List(Of String)
                                                qGenderRanges = (From a In myTestRefRanges.tparTestRefRanges _
                                                             Where Not a.IsGenderNull _
                                                                Select a.Gender).ToList()
                                                Dim rangeByGender As Boolean = (qGenderRanges.Count > 0)
                                                qGenderRanges = Nothing

                                                'Verify if there are Ranges defined by Age 
                                                Dim qAgeRanges As List(Of String)
                                                qAgeRanges = (From a In myTestRefRanges.tparTestRefRanges _
                                                         Where Not a.IsAgeUnitNull _
                                                            Select a.AgeUnit).ToList()
                                                Dim rangeByAge As Boolean = (qAgeRanges.Count > 0)
                                                qAgeRanges = Nothing

                                                If (rangeByGender AndAlso Not rangeByAge) Then
                                                    'Reference Ranges defined only By Gender; verify if field Gender is informed for the Patient
                                                    'and then search the detailed Range for the Patient's Gender
                                                    If (Not myPatientsDS.tparPatients(0).IsGenderNull) Then
                                                        Dim qRangeValues As List(Of TestRefRangesDS.tparTestRefRangesRow)
                                                        qRangeValues = (From b As TestRefRangesDS.tparTestRefRangesRow In myTestRefRanges.tparTestRefRanges _
                                                                       Where b.Gender = myPatientsDS.tparPatients(0).Gender _
                                                                      Select b).ToList()

                                                        If (qRangeValues.Count = 1) Then
                                                            minRangeValue = qRangeValues(0).NormalLowerLimit
                                                            maxRangeValue = qRangeValues(0).NormalUpperLimit
                                                        End If
                                                        qRangeValues = Nothing
                                                    End If

                                                ElseIf (rangeByAge) Then
                                                    'Reference Ranges defined By Age (with or without Gender); verify if field Age is informed for the Patient
                                                    If (Not myPatientsDS.tparPatients(0).IsAgeNull) Then
                                                        'Get the Patient Age expressed in Days, Months and Years 
                                                        Dim ageInDays As Integer = Convert.ToInt32(DateDiff(DateInterval.Day, myPatientsDS.tparPatients(0).DateOfBirth, Now))
                                                        Dim ageInMonths As Integer = Convert.ToInt32(DateDiff(DateInterval.Month, myPatientsDS.tparPatients(0).DateOfBirth, Now))
                                                        'Dim ageInYears As Integer = myPatientsDS.tparPatients(0).Age

                                                        'Get maximum limit defined for Age expressed in days and months
                                                        Dim myLimitsDS As FieldLimitsDS
                                                        Dim myLimitsDelegate As New FieldLimitsDelegate()

                                                        Dim maxAllowedDays As Integer = -1
                                                        resultData = myLimitsDelegate.GetList(dbConnection, GlobalEnumerates.FieldLimitsEnum.AGE_FROM_TO_DAYS)
                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            myLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)

                                                            If (myLimitsDS.tfmwFieldLimits.Rows.Count = 1) Then
                                                                maxAllowedDays = Convert.ToInt32(myLimitsDS.tfmwFieldLimits(0).MaxValue)
                                                            End If
                                                        End If

                                                        Dim maxAllowedMonths As Integer = -1
                                                        If (Not resultData.HasError) Then
                                                            resultData = myLimitsDelegate.GetList(dbConnection, GlobalEnumerates.FieldLimitsEnum.AGE_FROM_TO_MONTHS)
                                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                myLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)

                                                                If (myLimitsDS.tfmwFieldLimits.Rows.Count = 1) Then
                                                                    maxAllowedMonths = Convert.ToInt32(myLimitsDS.tfmwFieldLimits(0).MaxValue)
                                                                End If
                                                            End If
                                                        End If

                                                        'Finally, search the detailed Range...
                                                        If (Not resultData.HasError) Then
                                                            'Verify the AgeUnit in which the Range has to be searched
                                                            Dim ageUnit As String = "Y"
                                                            Dim ageValue As Integer = myPatientsDS.tparPatients(0).Age

                                                            If (ageInDays <= maxAllowedDays) Then
                                                                ageUnit = "D"
                                                                ageValue = ageInDays

                                                            ElseIf (ageInMonths <= maxAllowedMonths) Then
                                                                ageUnit = "M"
                                                                ageValue = ageInMonths
                                                            End If

                                                            If (Not rangeByGender) Then
                                                                'Reference Ranges defined only By Age
                                                                Dim rangeFound As Boolean = False
                                                                Dim qRangeValues As List(Of TestRefRangesDS.tparTestRefRangesRow)

                                                                Do While (Not rangeFound And ageUnit <> "")
                                                                    qRangeValues = (From b As TestRefRangesDS.tparTestRefRangesRow In myTestRefRanges.tparTestRefRanges _
                                                                                   Where b.AgeUnit = ageUnit _
                                                                                 AndAlso b.AgeRangeFrom <= ageValue _
                                                                                 AndAlso b.AgeRangeTo >= ageValue _
                                                                                  Select b).ToList()

                                                                    If (qRangeValues.Count = 1) Then
                                                                        rangeFound = True

                                                                        minRangeValue = qRangeValues(0).NormalLowerLimit
                                                                        maxRangeValue = qRangeValues(0).NormalUpperLimit
                                                                    Else
                                                                        If (ageUnit = "D") Then
                                                                            ageUnit = "M"
                                                                            ageValue = ageInMonths
                                                                        ElseIf (ageUnit = "M") Then
                                                                            ageUnit = "Y"
                                                                            ageValue = myPatientsDS.tparPatients(0).Age
                                                                        Else
                                                                            ageUnit = ""
                                                                        End If
                                                                    End If
                                                                Loop
                                                                qRangeValues = Nothing
                                                            Else
                                                                'Reference Ranges defined By Gender and Age; verify if fields Gender is informed 
                                                                'for the Patient
                                                                If (Not myPatientsDS.tparPatients(0).IsGenderNull) Then
                                                                    Dim rangeFound As Boolean = False
                                                                    Dim qRangeValues As List(Of TestRefRangesDS.tparTestRefRangesRow)

                                                                    Do While (Not rangeFound AndAlso ageUnit <> "")
                                                                        qRangeValues = (From b As TestRefRangesDS.tparTestRefRangesRow In myTestRefRanges.tparTestRefRanges _
                                                                                       Where b.Gender = myPatientsDS.tparPatients(0).Gender _
                                                                                     AndAlso b.AgeUnit = ageUnit _
                                                                                     AndAlso b.AgeRangeFrom <= ageValue _
                                                                                     AndAlso b.AgeRangeTo >= ageValue _
                                                                                      Select b).ToList()

                                                                        If (qRangeValues.Count = 1) Then
                                                                            rangeFound = True

                                                                            minRangeValue = qRangeValues(0).NormalLowerLimit
                                                                            maxRangeValue = qRangeValues(0).NormalUpperLimit
                                                                        Else
                                                                            If (ageUnit = "D") Then
                                                                                ageUnit = "M"
                                                                                ageValue = ageInMonths
                                                                            ElseIf (ageUnit = "M") Then
                                                                                ageUnit = "Y"
                                                                                ageValue = myPatientsDS.tparPatients(0).Age
                                                                            Else
                                                                                ageUnit = ""
                                                                            End If
                                                                        End If
                                                                    Loop
                                                                    qRangeValues = Nothing
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                End Select

                                If (Not resultData.HasError) Then
                                    'Return the Reference Range limits in a TestRefRangesDS
                                    Dim refRangeDs As New TestRefRangesDS

                                    'TR 17/07/2012 Validate the limits
                                    'If Not (minRangeValue = -1 AndAlso maxRangeValue = -1) OrElse _
                                    '   Not (minBorderLineRangeValue = -1 AndAlso maxBorderLineRangeValue = -1) Then
                                    Dim refRangeRow As TestRefRangesDS.tparTestRefRangesRow
                                    refRangeRow = refRangeDs.tparTestRefRanges.NewtparTestRefRangesRow
                                    refRangeRow.NormalLowerLimit = minRangeValue
                                    refRangeRow.NormalUpperLimit = maxRangeValue
                                    refRangeRow.BorderLineLowerLimit = minBorderLineRangeValue
                                    refRangeRow.BorderLineUpperLimit = maxBorderLineRangeValue
                                    refRangeDs.tparTestRefRanges.AddtparTestRefRangesRow(refRangeRow)

                                    resultData.SetDatos = refRangeDs
                                    resultData.HasError = False
                                    'End If

                                Else
                                    resultData.SetDatos = Nothing
                                    resultData.HasError = True
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetReferenceRangeInterval", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Reagents required for the group of Order Tests included in a WorkSession having ToSendFlag set
        ''' to True.  If several Order Tests have the same Test, the corresponding Reagents will be returned only once
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests included in a WorkSession</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RequiredReagentsDS with the list of Reagents that are required 
        '''          in a WorkSession according the list of Order Tests to execute included in it</returns>
        ''' <remarks>
        ''' Modified by: SA 03/03/2010 - Changed the Return Type TestReplicatesDS to GlobalDataTO; changed the way of 
        '''                              opening the DB Connection to fulfill the new template
        '''              SA 09/03/2010 - Error fixed: RequiredReagentsDS has to be returned in the GlobalDataTO
        '''              SA 30/04/2010 - Added optional parameter for Work Session ID; changes to receive a GlobalDataTO instead a 
        '''                              Typed DataSet SampleClassesByTestDS when calling GetSampleClassesByTest in OrdersDelegate
        '''              SA 20/07/2010 - Changed due return type of function GetNumberOfCalibrators is now a GlobalDataTO 
        '''              SA 01/09/2010 - When function GetReagentAndReplicatesByTest does not return data, it is not needed 
        '''                              return error Master Data Missing  
        '''              SA 01/09/2011 - Changed the function template; changed the calculation of total volume by each different SampleClass
        '''                              requested for each Test/SampleType: instead of using the number of replicates defined in the Test
        '''                              Programming, the calculation has to use the number of replicates specified for each different 
        '''                              SampleClass requested for the Test/SampleType when the WorkSession has been prepared
        '''              SA 16/02/2012 - Removed parameter pConstantVolumes; it is not needed for Ax00 Analyzers
        ''' </remarks>
        Public Function GetRequiredReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                            Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testReagentsDataDS As New RequiredReagentsDS

                        'Get the list of different pairs of TestID/SampleType in the Order Tests list, and for each one, the number of replicates for
                        'Blanks, Calibrators, Controls and Patient Samples, the required Reagents and the needed volume of each one of them
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        Dim testCalibratorValuesData As New TestCalibratorValuesDelegate
                        resultData = myOrderTestsDAO.GetReagentsByTest(dbConnection, pOrderTestsList, pWorkSessionID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim replicatesDS As TestReplicatesDS = DirectCast(resultData.SetDatos, TestReplicatesDS)

                            If (replicatesDS.TestReplicates.Rows.Count > 0) Then
                                'Every pair TestID/SampleType is processed
                                Dim totalRequestedReplicates As Single

                                Dim currentTestID As Integer = -1
                                Dim currentSampleType As String = ""
                                For Each replicatesRow As TestReplicatesDS.TestReplicatesRow In replicatesDS.TestReplicates.Rows
                                    If (replicatesRow.TestID <> currentTestID) OrElse (replicatesRow.SampleType <> currentSampleType) Then
                                        totalRequestedReplicates = 0

                                        'Get the number of points of the Calibrator used for the Test/SampleType
                                        resultData = testCalibratorValuesData.GetNumberOfCalibrators(dbConnection, replicatesRow.TestID, replicatesRow.SampleType)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            replicatesRow.NumberOfCalibrators = DirectCast(resultData.SetDatos, Integer)
                                        Else
                                            'Error getting the Number of Calibrators defined for the Test/SampleType
                                            Exit For
                                        End If

                                        'Get number of requested Blanks, Calibrators, Controls and Patient's Samples for the TestID/SampleType in the Work Session
                                        Dim myOrdersDelegate As New OrdersDelegate
                                        resultData = myOrdersDelegate.GetSampleClassesByTest(dbConnection, pOrderTestsList, replicatesRow.TestID, replicatesRow.SampleType, pWorkSessionID)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim sampleClassesDS As SampleClassesByTestDS = DirectCast(resultData.SetDatos, SampleClassesByTestDS)

                                            If (sampleClassesDS.SampleClassesByTest.Rows.Count > 0) Then
                                                'Every different SampleClass is processed
                                                For Each sampleClassesRow As SampleClassesByTestDS.SampleClassesByTestRow In sampleClassesDS.SampleClassesByTest.Rows
                                                    If (sampleClassesRow.SampleClass = "BLANK") Then
                                                        'Add the total Number of Replicates for the requested Blank
                                                        totalRequestedReplicates += sampleClassesRow.NumReplicates

                                                    ElseIf (sampleClassesRow.SampleClass = "CALIB") Then
                                                        'Add the total Number of Replicates for the requested Calibrator and multiply it by the Number of Calibrator Points
                                                        totalRequestedReplicates += replicatesRow.NumberOfCalibrators * sampleClassesRow.NumReplicates

                                                    ElseIf (sampleClassesRow.SampleClass = "CTRL") Then
                                                        'Add the total Number of Replicates for the requested Control
                                                        totalRequestedReplicates += sampleClassesRow.NumReplicates

                                                    Else
                                                        'Then SampleClass is PATIENT
                                                        'Add the total Number of Replicates for the requested Patient Samples
                                                        totalRequestedReplicates += sampleClassesRow.NumReplicates
                                                    End If
                                                Next
                                            End If
                                        Else
                                            'Error getting the number of requested Blanks, Calibrators, Controls and Patient's Samples for the
                                            'TestID/SampleType in the Work Session
                                            Exit For
                                        End If

                                        currentTestID = replicatesRow.TestID
                                        currentSampleType = replicatesRow.SampleType
                                    End If

                                    'Calculate the total required Reagent volume by multiplying the minimum Reagent Volume for the total number of
                                    'requested replicates for the Test/SampleType
                                    Dim totalReagentVol As Single = replicatesRow.ReagentVolume * totalRequestedReplicates

                                    'Verify if the Reagent data for the processed TestID/SampleType has been added to the DataSet to return
                                    Dim testReagentsRow As RequiredReagentsDS.RequiredReagentsRow

                                    'Verify if there is already a row for the correspondent Reagent in the DataSet
                                    Dim i As Integer = 0
                                    Dim reagentIndex As Integer = -1
                                    Do While (i <= testReagentsDataDS.RequiredReagents.Rows.Count - 1) AndAlso (reagentIndex = -1)
                                        If (testReagentsDataDS.RequiredReagents(i).ReagentID = replicatesRow.ReagentID) Then
                                            reagentIndex = i
                                        Else
                                            i += 1
                                        End If
                                    Loop

                                    If (reagentIndex = -1) Then
                                        'The Reagent is not in the DataSet, add it and its total required volume to the DataSet to return
                                        testReagentsRow = testReagentsDataDS.RequiredReagents.NewRequiredReagentsRow
                                        testReagentsRow.ReagentID = replicatesRow.ReagentID
                                        testReagentsRow.ReagentNumber = replicatesRow.ReagentNumber
                                        testReagentsRow.ReagentVolume = replicatesRow.ReagentVolume
                                        testReagentsRow.TotalVolume = totalReagentVol

                                        testReagentsDataDS.RequiredReagents.Rows.Add(testReagentsRow)
                                    Else
                                        'The Reagent is already included in the DataSet (which mean several different TestID/SampleType use the same Reagent): 
                                        'required total volume is acummulated
                                        testReagentsDataDS.RequiredReagents(reagentIndex).TotalVolume = testReagentsDataDS.RequiredReagents(reagentIndex).TotalVolume + totalReagentVol
                                    End If
                                Next

                                If (Not resultData.HasError) Then
                                    'Return the list of required Reagents
                                    resultData.SetDatos = testReagentsDataDS
                                End If
                            Else
                                'The returned DS is empty, but this is needed to avoid an error when return to function that make the call (if not, SetDatos contains a 
                                'TestReplicatesDS instead of the expected RequiredReagentsDS and an error is raised)
                                resultData.SetDatos = testReagentsDataDS
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetRequiredReagents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of different Sample Types needed for the specified PatientID/SampleID according the list of Tests that have been 
        ''' requested in the active WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">PatientID or SampleID</param>
        ''' <param name="pOnlySentToPos">When TRUE, it indicates only SampleTypes of Order Tests selected to be positioned (OpenOTFlag = FALSE) will be returned
        '''                              When FALSE, SampleTypes of all Order Tests will be returned. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of different Sample Types needed for the specified
        '''          Patient/Sample according the requested Tests</returns>
        ''' <remarks>
        ''' Created by:  SA 01/09/2011
        ''' Modified by: SA 10/04/2013 - Added optional parameter pOnlySentToPos to indicate if only OrderTests with OpenOTFlag=FALSE have to be returned 
        '''              SA 17/04/2013 - Deleted parameter pPatientExists
        ''' </remarks>
        Public Function GetSampleTypesByPatient(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                ByVal pPatientID As String, Optional ByVal pOnlySentToPos As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        resultData = myOrderTestsDAO.GetSampleTypesByPatient(dbConnection, pWorkSessionID, pPatientID, pOnlySentToPos)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetSampleTypesByPatient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of required positions for Patient's Sample Types for a group of Order Tests having ToSendFlag=True 
        ''' and belonging to Orders with SampleClass Patient. For each Patient, only one position will be created for each 
        ''' different SampleType (no matter how many Orders are defined for she/he).  When the Order Test belongs to an Order 
        ''' without an specified Patient, a position will be created for each different SampleType required in the Order         
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests Identifiers</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TubesBySampleTypeDS with the list of different Patient's Sample Types 
        '''          that are required in a WorkSession for the informed list of Order Tests</returns>
        ''' <remarks>
        ''' Modified by: VR 10/12/2009 - Changed the return type TubesBySampleTypeDS to GlobalDataTO
        '''              SA 03/03/2010 - Changed the way of opening the DB Connection to fulfill the new template
        '''              SA 03/05/2010 - Added parameter for Work Session Identifier
        '''              SA 21/10/2010 - Added changes for management of SampleType Tubes for ISE Tests
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 09/11/2011 - Added the sorting of Patient elements according the order in which Patients were requested
        '''                              (new function GetPatientSamplesCreationOrder in twksOrderTestsDAO is called to get the order)
        '''              XB 04/06/2013 - change URI sample type comparison in order to prepare the code in front of DB changes
        ''' </remarks>
        Public Function GetSampleTypesTubes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                            Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the list of required SampleType Elements by Patient for STD Tests
                        Dim sampleTypeTubesData As New TwksOrderTestsDAO
                        myGlobalDataTO = sampleTypeTubesData.GetSampleTypesTubesSTD(dbConnection, pOrderTestsList, pWorkSessionID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim sampleTubesDS As TubesBySampleTypeDS = DirectCast(myGlobalDataTO.SetDatos, TubesBySampleTypeDS)

                            'Get the list of required SampleType Elements by Patient for ISE Tests
                            myGlobalDataTO = sampleTypeTubesData.GetSampleTypesTubesISE(dbConnection, pOrderTestsList, pWorkSessionID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim iseSampleTubesDS As TubesBySampleTypeDS = DirectCast(myGlobalDataTO.SetDatos, TubesBySampleTypeDS)

                                Dim patientID As String = ""
                                Dim sampleType As String = ""
                                Dim predilutionFactor As Single = 0
                                Dim lstTubeExists As List(Of TubesBySampleTypeDS.TubesBySampleTypeTableRow)

                                For Each iseSampleTube As TubesBySampleTypeDS.TubesBySampleTypeTableRow In iseSampleTubesDS.TubesBySampleTypeTable
                                    'If (iseSampleTube.SampleType <> "URI") Then            ' XB 04/06/2013
                                    If Not iseSampleTube.SampleType.Contains("URI") Then    ' XB 04/06/2013
                                        If (Not iseSampleTube.IsPredilutionFactorNull) Then
                                            iseSampleTube.BeginEdit()
                                            iseSampleTube.PredilutionUseFlag = True
                                            iseSampleTube.PredilutionMode = "USER"
                                            iseSampleTube.EndEdit()
                                        End If

                                        patientID = ""
                                        If (Not iseSampleTube.IsPatientIDNull) Then
                                            patientID = iseSampleTube.PatientID
                                        ElseIf (Not iseSampleTube.IsSampleIDNull) Then
                                            patientID = iseSampleTube.SampleID
                                        End If
                                        sampleType = iseSampleTube.SampleType

                                        'Search if the required SampleType Tube has been already requested for an Standard Test
                                        If (iseSampleTube.IsPredilutionFactorNull) Then
                                            lstTubeExists = (From a As TubesBySampleTypeDS.TubesBySampleTypeTableRow In sampleTubesDS.TubesBySampleTypeTable _
                                                            Where ((Not a.IsPatientIDNull AndAlso a.PatientID = patientID) _
                                                           OrElse (Not a.IsSampleIDNull AndAlso a.SampleID = patientID)) _
                                                          AndAlso a.SampleType = sampleType _
                                                          AndAlso a.IsPredilutionFactorNull _
                                                           Select a).ToList()
                                        Else
                                            predilutionFactor = iseSampleTube.PredilutionFactor
                                            lstTubeExists = (From a As TubesBySampleTypeDS.TubesBySampleTypeTableRow In sampleTubesDS.TubesBySampleTypeTable _
                                                            Where ((Not a.IsPatientIDNull AndAlso a.PatientID = patientID) _
                                                           OrElse (Not a.IsSampleIDNull AndAlso a.SampleID = patientID)) _
                                                          AndAlso a.SampleType = sampleType _
                                                          AndAlso a.PredilutionMode = "USER" _
                                                          AndAlso a.PredilutionFactor = predilutionFactor _
                                                           Select a).ToList()
                                        End If

                                        If (lstTubeExists.Count = 0) Then
                                            'Add the SampleTypeTube to the final DS
                                            sampleTubesDS.TubesBySampleTypeTable.ImportRow(iseSampleTube)
                                        End If
                                    Else
                                        If (Not iseSampleTube.IsPredilutionFactorNull) Then
                                            'A Tube with diluted Urine is used only for ISE Tests
                                            iseSampleTube.BeginEdit()
                                            iseSampleTube.PredilutionUseFlag = True
                                            iseSampleTube.PredilutionMode = "USER"
                                            iseSampleTube.OnlyForISE = True
                                            iseSampleTube.EndEdit()

                                            'Add the Diluted SampleTypeTube to the final DS
                                            sampleTubesDS.TubesBySampleTypeTable.ImportRow(iseSampleTube)
                                        End If
                                    End If
                                Next
                                lstTubeExists = Nothing

                                'Get the final order for the Patient Sample Tubes
                                myGlobalDataTO = sampleTypeTubesData.GetPatientSamplesCreationOrder(dbConnection, pWorkSessionID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim patientSampleTubesDS As TubesBySampleTypeDS = DirectCast(myGlobalDataTO.SetDatos, TubesBySampleTypeDS)

                                    Dim sortedSampleTubesDS As New TubesBySampleTypeDS
                                    Dim myPatientTests As List(Of TubesBySampleTypeDS.TubesBySampleTypeTableRow)
                                    For Each patientRow As TubesBySampleTypeDS.TubesBySampleTypeTableRow In patientSampleTubesDS.TubesBySampleTypeTable
                                        patientID = ""
                                        If (Not patientRow.IsPatientIDNull) Then
                                            patientID = patientRow.PatientID
                                        ElseIf (Not patientRow.IsSampleIDNull) Then
                                            patientID = patientRow.SampleID
                                        End If
                                        sampleType = patientRow.SampleType

                                        myPatientTests = (From a As TubesBySampleTypeDS.TubesBySampleTypeTableRow In sampleTubesDS.TubesBySampleTypeTable _
                                                         Where ((Not a.IsPatientIDNull AndAlso a.PatientID = patientID) _
                                                         OrElse (Not a.IsSampleIDNull AndAlso a.SampleID = patientID)) _
                                                        AndAlso a.SampleType = sampleType _
                                                         Select a).ToList()

                                        For Each testRow As TubesBySampleTypeDS.TubesBySampleTypeTableRow In myPatientTests
                                            sortedSampleTubesDS.TubesBySampleTypeTable.ImportRow(testRow)
                                        Next
                                    Next
                                    myPatientTests = Nothing

                                    myGlobalDataTO.SetDatos = sortedSampleTubesDS 'sampleTubesDS
                                    myGlobalDataTO.HasError = False
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetSampleTypesTubes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the total volume needed for an specific Special Solution according the list of 
        ''' Order Tests included in a Work Session. 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests included in a WorkSession</param>
        ''' <param name="pSpecialSolutionCode">Code of the Special Solution</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing the needed total volume of the specified Solution Code</returns>
        ''' <remarks>
        ''' Modified by: SA 03/03/2010 - Changed the Return Type Single to GlobalDataTO; changed the way of 
        '''                              opening the DB Connection to fulfill the new template
        '''              SA 30/04/2010 - Added optional parameter WorkSessionID
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 16/02/2012 - Removed parameter pConstantVolumes; it is not needed for Ax00 Analyzers
        ''' </remarks>
        Public Function GetSpecialSolutionVolume(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                                 ByVal pSpecialSolutionCode As String, Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the total volume required for Blanks 
                        Dim orderTestsData As New TwksOrderTestsDAO
                        resultData = orderTestsData.GetSpecialSolutionVolumeForBlanks(dbConnection, pOrderTestsList, pSpecialSolutionCode, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetSpecialSolutionVolumeForBlanks", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is at least a Test of the specified TestType requested in the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pTestType">Test Type code: STD or ISE</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if there are Tests of the specified type requested in the WorkSession</returns>
        ''' <remarks>
        ''' Created by:  RH 15/06/2011
        ''' Modified by: SA 20/03/2012 - Added parameter for the WorkSessionID
        '''              SA 14/05/2012 - Function name changed from IsThereAnyISETest to IsThereAnyTestByType, and added parameter to inform 
        '''                              the TestType. These changes are to allow using the function for both STD and ISE Tests
        ''' </remarks>
        Public Function IsThereAnyTestByType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                             ByVal pTestType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TwksOrderTestsDAO
                        resultData = myDAO.IsThereAnyTestByType(dbConnection, pWorkSessionID, pTestType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.IsThereAnyTestByType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all LIS Saved WS and process each one of them, adding its Order Tests to the active Work Session when possible.
        ''' Order Tests that cannot be added to the active Work Session are returned in an OrderTestsLISInfoDS to be rejected  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWSStatus">Work Session Status</param>
        ''' <param name="pISEModuleReady">Flag indicating if ISE Module is ready or not</param>
        ''' <param name="pRejectedDS">Typed DS OrderTestsLISInfoDS to return the list of requested AWOS ID that have been rejected</param>
        ''' <param name="pAutoWSCreationInProcess">Flag indicating if the function has been called by the process for automatic WS creation</param>
        ''' <param name="pRunningMode">When TRUE, it indicates the ManageRepetitions function (in ProcessLISPatientOTs) the Analyzer is in RUNNING 
        '''                            mode; otherwise it indicates the Analyzer is in STANDBY</param>
        ''' <returns>GlobalDataTO containing an integer value: 0 = No Patient nor Control Sample was added; 
        '''                                                    1 = Some Patient and/or Control Samples were added, but there is not tube for any of them; 
        '''                                                    2 = Some Patient and/or Control Samples were added, and there is a tube for at least one of them
        ''' </returns>
        ''' <remarks>
        ''' Created by: SGM 22/03/2013
        ''' Modified by: SA 26/03/2013 - Added search of LIS Values for TestID and SampleType of all AwosID to reject.
        '''                              Changed the scope of the local DB Transaction (each LIS Saved WS is processed individually; the local transaction
        '''                              includes only the update of the active WS and the delete of those LIS Saved WS succesfully processed)  
        '''              SA 05/04/2013 - Added ByRef parameter to return the list of rejected AWOS ID 
        '''              AG 13/05/2013 - Send cancellation to LIS for those deleted tests that exist in the LIS saved worksession (in this case it is not necessary 
        '''                              to find the TEST mapping value for LIS)
        '''              SA 29/05/2013 - Informed optional parameter pFromLIS=TRUE when call function Delete in SavedWSDelegate
        '''              SA 30/05/2013 - After calling function PrepareOrderTestsForWS, get value of field BCLinkedToElem in the returned WorkSessionsDS and if it 
        '''                              is TRUE, set valueToReturn = 2 to indicate the screen of WS Rotor Positions has to be opened when the LIS Orders processing
        '''                              finished
        '''              SA 05/09/2013 - Added new parameter pAutoWSCreationInProcess to indicate when the function has been called by the process for automatic WS 
        '''                              creation. It is informed when call function ProcessLISPatientOTs to avoid count Selected or NotSelected Order Tests when the
        '''                              added Order Tests do not generate new executions
        '''              SA 11/03/2014 - BT #1536 ==> Changed call to function ProcessLISControlOTs due to its entry parameter for the DB Connection has been removed
        '''                                           due to it was not used
        '''              SA 21/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions. When value of global flag 
        '''                                           NEWAddWorkSession is TRUE, call new version of function PrepareOrderTestsForWS  
        '''              XB 24/03/2014 - BT #1536 ==> Transactions become smaller to avoid timeout malfunctions
        '''              AG 31/03/2014 - BT #1565 ==> Added new parameter pRunningMode (required for ManageRepetitions in ProcessLISPatientOTs)
        '''              SA 01/04/2014 - BT #1564 ==> When calling function ProcessLISPatientOTs, inform as parameter the sending Date and Time of the LIS Message 
        '''                                           from which all Patient Order Tests to process were extracted
        ''' </remarks>
        Public Function LoadLISSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                       ByVal pWSStatus As String, ByVal pISEModuleReady As Boolean, ByRef pRejectedDS As OrderTestsLISInfoDS, _
                                       ByVal pAutoWSCreationInProcess As Boolean, ByVal pRunningMode As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim TotalStartTime As DateTime = Now
                Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'Get all Saved WS sent by LIS
                Dim mySavedWSDS As SavedWSDS
                Dim mySavedWSDelegate As New SavedWSDelegate

                'TR 11/04/2013 - Added new variable to set the result
                Dim valueToReturn As Integer = 0

                resultData = mySavedWSDelegate.GetAll(Nothing, True)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    mySavedWSDS = TryCast(resultData.SetDatos, SavedWSDS)

                    If (mySavedWSDS.tparSavedWS.Rows.Count > 0) Then
                        'Get value for the parameter for the LIS Working Mode for Reruns
                        Dim myRerunLISMode As String = "BOTH"
                        Dim myUsersSettingsDelegate As New UserSettingsDelegate

                        resultData = myUsersSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WORKING_MODE_RERUNS.ToString)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myRerunLISMode = TryCast(resultData.SetDatos, String)
                        End If

                        'Get all Order Tests included in the active WS
                        Dim myWSResultsDS As New WorkSessionResultDS
                        Dim myWorkSessionDelegate As New WorkSessionsDelegate

                        resultData = myWorkSessionDelegate.GetOrderTestsForWS(Nothing, pWorkSessionID, pAnalyzerID, False)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myWSResultsDS = TryCast(resultData.SetDatos, WorkSessionResultDS)

                            'Process for each LIS SavedWS 
                            Dim toSavedWSToDelete As New SavedWSDS
                            Dim mySavedWSOTDS As SavedWSOrderTestsDS
                            Dim mySavedWSOTDelegate As New SavedWSOrderTestsDelegate

                            Dim SavedWSStartTime As DateTime = Now
                            For Each wsRow As SavedWSDS.tparSavedWSRow In mySavedWSDS.tparSavedWS
                                'Get all Order Tests included in the saved WS
                                resultData = mySavedWSOTDelegate.GetOrderTestsBySavedWSID(Nothing, wsRow.SavedWSID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    mySavedWSOTDS = TryCast(resultData.SetDatos, SavedWSOrderTestsDS)

                                    'Process to add to the active WS all Control Order Tests in the Saved WS
                                    resultData = ProcessLISControlOTs(pAnalyzerID, myWSResultsDS, mySavedWSOTDS, pRejectedDS)
                                    If (Not resultData.HasError) Then
                                        'Check if the value to return has to be updated... 
                                        If (CInt(resultData.SetDatos) > valueToReturn) Then valueToReturn = CInt(resultData.SetDatos)

                                        'Process to add to the active WS all Patient Order Tests in the Saved WS
                                        'XB 24/03/2014 - BT #1536
                                        'AG 31/03/2014 - BT #1565 - Inform parameter pRunningMode
                                        'SA 01/04/2014 - BT #1564 - Inform parameter for the LIS Message DateTime
                                        If (NEWAddWorkSession) Then
                                            resultData = ProcessLISPatientOTs_NEW(Nothing, pAnalyzerID, pWorkSessionID, myWSResultsDS, mySavedWSOTDS, myRerunLISMode, _
                                                                                  pISEModuleReady, pRejectedDS, pAutoWSCreationInProcess, pRunningMode, wsRow.TS_DateTime)
                                        Else
                                            resultData = ProcessLISPatientOTs(Nothing, pAnalyzerID, pWorkSessionID, myWSResultsDS, mySavedWSOTDS, myRerunLISMode, _
                                                                              pISEModuleReady, pRejectedDS, pAutoWSCreationInProcess, pRunningMode, wsRow.TS_DateTime)
                                        End If

                                        If (Not resultData.HasError) Then
                                            'Check if the value to return has to be updated... 
                                            If (CInt(resultData.SetDatos) > valueToReturn) Then valueToReturn = CInt(resultData.SetDatos)

                                            'Copy the Saved WS to the DS containing all LIS Saved WS to delete (all Saved WS already added to the active WS)
                                            toSavedWSToDelete.tparSavedWS.ImportRow(wsRow)
                                        End If
                                    End If
                                End If

                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                myLogAcciones.CreateLogActivity("Process of SavedWSID " & wsRow.SavedWSName & " = " & Now.Subtract(SavedWSStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                "OrderTestsDelegate.LoadLISSavedWS", EventLogEntryType.Information, False)
                                SavedWSStartTime = Now
                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            Next

                            'Verify if there is at least an AWOS ID to reject
                            If (pRejectedDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                'Get all defined LIS Test Mappings
                                Dim myAllTestMappingsDS As AllTestsByTypeDS
                                Dim myAllTestMappingsDelegate As New AllTestByTypeDelegate

                                resultData = myAllTestMappingsDelegate.ReadAll(Nothing)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myAllTestMappingsDS = DirectCast(resultData.SetDatos, AllTestsByTypeDS)

                                    'Get the rest of defined LIS Mappings
                                    Dim myLISMappingsDS As LISMappingsDS
                                    Dim myLISMappingsDelegate As New LISMappingsDelegate

                                    resultData = myLISMappingsDelegate.ReadAll(Nothing)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myLISMappingsDS = DirectCast(resultData.SetDatos, LISMappingsDS)

                                        'Search the LIS Value for the Test and SampleType of each rejected AWOS
                                        Dim lstMappedTest As List(Of AllTestsByTypeDS.vparAllTestsByTypeRow)
                                        Dim lstMappedSType As List(Of LISMappingsDS.vcfgLISMappingRow)

                                        For Each lisOT As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pRejectedDS.twksOrderTestsLISInfo
                                            'AG 13/05/2013 - Look for the LIS mapping value for testID only if not informed yet!
                                            If lisOT.IsTestIDStringNull Then
                                                'Search the LIS Test Identifier
                                                lstMappedTest = (From a As AllTestsByTypeDS.vparAllTestsByTypeRow In myAllTestMappingsDS.vparAllTestsByType _
                                                                Where a.TestType = lisOT.TestType _
                                                              AndAlso a.TestID = lisOT.TestID _
                                                               Select a).ToList
                                                'Inform fields TestIDText with the obtained values
                                                If (lstMappedTest.Count = 1) Then lisOT.TestIDString = lstMappedTest(0).LISValue
                                            End If

                                            'Search the LIS Sample Type Code
                                            lstMappedSType = (From a As LISMappingsDS.vcfgLISMappingRow In myLISMappingsDS.vcfgLISMapping _
                                                             Where a.ValueType = "SAMPLE_TYPES" _
                                                           AndAlso a.ValueId = lisOT.SampleType _
                                                            Select a).ToList
                                            'Inform fields SampleType with the obtained values
                                            'AG 13/05/2013 - Do not compare with = 1 because the linq returns N records (one for each language)
                                            'If (lstMappedSType.Count = 1) Then lisOT.SampleType = lstMappedSType(0).LISValue
                                            If (lstMappedSType.Count > 0) Then lisOT.SampleType = lstMappedSType(0).LISValue

                                        Next
                                    End If
                                End If
                            End If

                            'Open a DB Transaction to update the active WorkSession and delete all LIS Saved WS successfully processed
                            Dim UpdateWSStartTime As DateTime = Now

                            If (NEWAddWorkSession) Then
                                'BT #1545
                                If (valueToReturn > 0) Then
                                    'Add all accepted LIS Order Tests to the active WorkSession
                                    resultData = myWorkSessionDelegate.PrepareOrderTestsForWS_NEW(myWSResultsDS, pWorkSessionID, pAnalyzerID, False, _
                                                                                                  pWSStatus, True, False)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        'If at least an incomplete Patient Sample was linked to a WS Required Element, function should return the value
                                        'that allow opening the screen of WS Rotor Positions
                                        Dim myWorkSessionDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                                        If (myWorkSessionDS.twksWorkSessions.Rows.Count > 0) Then
                                            If (Not myWorkSessionDS.twksWorkSessions(0).IsBCLinkedToElemNull AndAlso myWorkSessionDS.twksWorkSessions(0).BCLinkedToElem) Then valueToReturn = 2
                                        End If
                                    End If
                                End If

                                'Delete all LIS Saved WS successfully processed
                                If (Not resultData.HasError) Then resultData = mySavedWSDelegate.Delete(Nothing, toSavedWSToDelete, True)
                            Else
                                'Open a DB Transaction to update the active WorkSession and delete all LIS Saved WS successfully processed
                                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                                    If (Not dbConnection Is Nothing) Then
                                        If (valueToReturn > 0) Then
                                            'Add all accepted LIS Order Tests to the active WorkSession
                                            resultData = myWorkSessionDelegate.PrepareOrderTestsForWS(dbConnection, myWSResultsDS, False, pWorkSessionID, pAnalyzerID, _
                                                                                                      pWSStatus, True, False)

                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                'If at least an incomplete Patient Sample was linked to a WS Required Element, function should return the value
                                                'that allow opening the screen of WS Rotor Positions
                                                Dim myWorkSessionDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                                                If (myWorkSessionDS.twksWorkSessions.Rows.Count > 0) Then
                                                    If (Not myWorkSessionDS.twksWorkSessions(0).IsBCLinkedToElemNull AndAlso myWorkSessionDS.twksWorkSessions(0).BCLinkedToElem) Then valueToReturn = 2
                                                End If
                                            End If
                                        End If

                                        If (Not resultData.HasError) Then
                                            'Delete all LIS Saved WS successfully processed
                                            resultData = mySavedWSDelegate.Delete(dbConnection, toSavedWSToDelete, True)
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
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                            myLogAcciones.CreateLogActivity("Total time update the active WS = " & Now.Subtract(UpdateWSStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "OrderTestsDelegate.LoadLISSavedWS", EventLogEntryType.Information, False)
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        End If
                    End If
                End If

                'TR 11/04/2013 - Set the value to return
                resultData.SetDatos = valueToReturn

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                myLogAcciones.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "OrderTestsDelegate.LoadLISSavedWS", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.LoadLISSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Load a Saved WS with a determinated ID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the Saved WS to load</param>
        ''' <param name="pAnalizerID">Analizer Identifier</param>
        ''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing the list of Samples currently requested;
        '''                           optional parameter, informed only when the SavedWS to load contains data of an Import
        '''                           from LIMS file</param>
        ''' <param name="pFromLIMS">Flag indicating if the Saved WS to be loaded contains data of an Import from LIMS
        '''                         file; optional parameter with default value False</param>
        ''' <returns>Global object containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 08/04/2010
        ''' Modified by: SG  08/07/2010 - When all elements in the SavedWS were deleted, returns an specific error
        '''              SA  12/07/2010 - Open a Transaction instead a Connection: if there are elements to delete, the
        '''                               deletion has to be executed in a DB Transaction. Code to delete an empty SavedWS
        '''                               was moved here from WSPreparation screen: it has to be executed in the same Transaction
        '''              SA  15/09/2010 - Changes to adapt the function to the Import from LIMS process
        '''              SA  24/02/2011 - Get and load first the Patient Order Tests. Then, get all different Standard Tests/Sample Type
        '''                               and load them in a SelectedTestsDS dataset; pass this DS as parameter to load Controls,
        '''                               Calibrators and Blanks
        '''              SA  01/09/2011 - Changed the function template
        '''              SA  25/04/2013 - Get all different TestType/TestID/SampleType for Standard and ISE Tests requested for Patients 
        '''                               that were loaded from the SavedWS. Only those with ExternalQC = FALSE are selected, due to for this
        '''                               type of Samples, linked Controls should not be added
        ''' </remarks>
        Public Function LoadFromSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, _
                                        ByVal pAnalizerID As String, Optional ByVal pWSResultDS As WorkSessionResultDS = Nothing, _
                                        Optional ByVal pFromLIMS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim wsPartiallyLoaded As Boolean = False
                        Dim myWorkSessionResultDS As New WorkSessionResultDS

                        'When this parameter is informed it means the Samples in the Saved WS will be
                        'added to Samples previously requested (used in Import from LIMS process)
                        If (Not pWSResultDS Is Nothing) Then myWorkSessionResultDS = pWSResultDS

                        'Delete all elements included in the Saved WS that have been deleted
                        Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate
                        resultData = mySavedWSOrderTests.ClearDeletedElements(dbConnection, pSavedWSID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            If (DirectCast(resultData.SetDatos, Integer) > 0) Then wsPartiallyLoaded = True
                        End If

                        'Get all Elements included in the Saved WS
                        resultData = mySavedWSOrderTests.GetOrderTestsBySavedWSID(dbConnection, pSavedWSID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)

                            Dim mySelectedSTDTestsDS As New SelectedTestsDS
                            If (mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Count > 0) Then
                                'Process the Patients
                                resultData = LoadAddPatientOrderTests(mySavedWSOrderTestsDS, myWorkSessionResultDS, pAnalizerID, pFromLIMS)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Add the Patient Samples to the DS to return
                                    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                                End If

                                'Get all different TestType/TestID/SampleType for Standard and ISE Tests requested for Patients 
                                'that were loaded from the SavedWS. Only those with ExternalQC = FALSE are selected, due to for this
                                'type of Samples, linked Controls should not be added
                                Dim lstTests As List(Of String)
                                lstTests = (From a In myWorkSessionResultDS.Patients _
                                           Where (a.TestType = "STD" Or a.TestType = "ISE") _
                                         AndAlso a.ExternalQC = False _
                                          Select a.TestType & "|" & a.TestID & "|" & a.SampleType Distinct).ToList

                                For Each diffTests As String In lstTests
                                    Dim myComponents() As String = diffTests.Split(CChar("|"))

                                    Dim mySelectedTestRow As SelectedTestsDS.SelectedTestTableRow
                                    mySelectedTestRow = mySelectedSTDTestsDS.SelectedTestTable.NewSelectedTestTableRow
                                    mySelectedTestRow.TestType = myComponents(0)
                                    mySelectedTestRow.SampleType = myComponents(2)
                                    mySelectedTestRow.TestID = Convert.ToInt32(myComponents(1))
                                    mySelectedTestRow.OTStatus = "OPEN"
                                    mySelectedTestRow.Selected = True
                                    mySelectedSTDTestsDS.SelectedTestTable.Rows.Add(mySelectedTestRow)
                                Next
                                lstTests = Nothing

                                'Process the Controls
                                resultData = LoadAddSampleClassOrderTests(mySavedWSOrderTestsDS, myWorkSessionResultDS, pAnalizerID, "CTRL", mySelectedSTDTestsDS)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Add the Controls to the DS to return
                                    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                                End If

                                'Get all different TestID/SampleType for Standard Tests requested for Patients 
                                'that were loaded from the SavedWS
                                lstTests = (From a In myWorkSessionResultDS.Patients _
                                           Where a.TestType = "STD" _
                                          Select a.TestID & "|" & a.SampleType Distinct).ToList

                                mySelectedSTDTestsDS.SelectedTestTable.Clear()
                                For Each diffTests As String In lstTests
                                    Dim myComponents() As String = diffTests.Split(CChar("|"))

                                    Dim mySelectedTestRow As SelectedTestsDS.SelectedTestTableRow
                                    mySelectedTestRow = mySelectedSTDTestsDS.SelectedTestTable.NewSelectedTestTableRow
                                    mySelectedTestRow.TestType = "STD"
                                    mySelectedTestRow.SampleType = myComponents(1)
                                    mySelectedTestRow.TestID = Convert.ToInt32(myComponents(0))
                                    mySelectedTestRow.OTStatus = "OPEN"
                                    mySelectedTestRow.Selected = True
                                    mySelectedSTDTestsDS.SelectedTestTable.Rows.Add(mySelectedTestRow)
                                Next
                                lstTests = Nothing

                                'Process the Calibrators
                                resultData = LoadAddSampleClassOrderTests(mySavedWSOrderTestsDS, myWorkSessionResultDS, pAnalizerID, "CALIB", mySelectedSTDTestsDS)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Add the Calibrators to the DS to return
                                    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                                End If

                                'Process the Blanks
                                resultData = LoadAddSampleClassOrderTests(mySavedWSOrderTestsDS, myWorkSessionResultDS, pAnalizerID, "BLANK", mySelectedSTDTestsDS)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Add the Blanks to the DS to return
                                    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
                                End If

                                'If some elements of the Saved WS were deleted, the message code of the warning that should be shown in the 
                                'screen is loaded in field ErrorCode of the GlobalDataTO object to return (although HasError is FALSE)
                                If (Not resultData.HasError AndAlso wsPartiallyLoaded) Then resultData.ErrorCode = GlobalEnumerates.Messages.WS_PARTIALLY_LOADED.ToString
                            Else
                                'All elements in the Saved WS were deleted, there is nothing to load, delete also the empty WS
                                Dim mySavedWSDS As New SavedWSDS
                                Dim mytparSavedWSRow As SavedWSDS.tparSavedWSRow = CType(mySavedWSDS.tparSavedWS.NewtparSavedWSRow, SavedWSDS.tparSavedWSRow)

                                mytparSavedWSRow.SavedWSID = pSavedWSID
                                mySavedWSDS.tparSavedWS.Rows.Add(mytparSavedWSRow)
                                mySavedWSDS.AcceptChanges()

                                Dim mySavedWSDelegate As New SavedWSDelegate
                                resultData = mySavedWSDelegate.Delete(dbConnection, mySavedWSDS)
                                If (Not resultData.HasError) Then
                                    resultData.ErrorCode = GlobalEnumerates.Messages.WS_NONE_LOADED.ToString
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.LoadFromSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a new Analyzer is connected and there is a WorkSession with status different of EMPTY and OPEN, all 
        ''' Order Tests in the WorkSession are copied in a saved WS. This function get all data in the saved WS, create 
        ''' all Orders and OrderTests from it and create the new WorkSession for the new connected Analyzer
        ''' new connected Analyzer 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the SavedWS to load</param>
        ''' <param name="pNewAnalyzerID">Identifier of the new connected Analyzer</param>
        ''' <returns>GlobalDataTO containing a a typed DataSet WorkSessionDS, with the identifier of the Work Session and its current 
        '''          Status, and also information about the Analyzer in which the Work Session is or will be executed</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' Modified by: SA 09/09/2013 - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous one still exists
        '''              SA 25/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions. When value of global flag 
        '''                                           NEWAddWorkSession is TRUE, call new version of function AddWorkSession  
        ''' </remarks>
        Public Function LoadFromSavedWSToChangeAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, _
                                                        ByVal pNewAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDS As OrderTestsDS
                        Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate

                        'Get maximum number of days that can have passed from the last Blank or Calibrator execution
                        Dim maxDaysPreviousBlkCalib As String = ""
                        Dim myGeneralSettingsDelegate As New GeneralSettingsDelegate

                        resultData = myGeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_DAYS_PREVIOUS_BLK_CALIB.ToString)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            maxDaysPreviousBlkCalib = DirectCast(resultData.SetDatos, String)
                        End If

                        'Get all BLANKS included in the Saved WS and create a new BLANK Order for them
                        resultData = mySavedWSOrderTests.GetOrderTestsToChangeAnalyzer(dbConnection, pSavedWSID, "BLANK", False, pNewAnalyzerID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            If (myOrderTestsDS.twksOrderTests.Rows.Count > 0) Then resultData = MoveBlanksFromSavedWS(dbConnection, myOrderTestsDS, maxDaysPreviousBlkCalib)
                        End If

                        'Get all CALIBRATORS included in the Saved WS and create a new CALIBRATOR Order for them
                        resultData = mySavedWSOrderTests.GetOrderTestsToChangeAnalyzer(dbConnection, pSavedWSID, "CALIB", False, pNewAnalyzerID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            If (myOrderTestsDS.twksOrderTests.Rows.Count > 0) Then resultData = MoveCalibratorsFromSavedWS(dbConnection, myOrderTestsDS, maxDaysPreviousBlkCalib)
                        End If

                        'Get all CONTROLS included in the Saved WS and create a new CTRL Order for them
                        resultData = mySavedWSOrderTests.GetOrderTestsToChangeAnalyzer(dbConnection, pSavedWSID, "CTRL", False, pNewAnalyzerID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            If (myOrderTestsDS.twksOrderTests.Rows.Count > 0) Then resultData = MoveControlsFromSavedWS(dbConnection, myOrderTestsDS)
                        End If

                        'Get all PATIENT SAMPLES included in the Saved WS and create a PATIENT Order for each one of them
                        resultData = MovePatientsFromSavedWS(dbConnection, pSavedWSID, pNewAnalyzerID)
                        If (Not resultData.HasError) Then
                            Dim myOTsDAO As New TwksOrderTestsDAO

                            'Get all OPEN Order Tests that have been created
                            resultData = myOTsDAO.GetOrderTestsToChangeAnalyzer(dbConnection, pNewAnalyzerID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myWSOrderTestsDS As WSOrderTestsDS = DirectCast(resultData.SetDatos, WSOrderTestsDS)

                                'Finally, create a new WorkSession for the new Analyzer containing all created Order Tests 
                                Dim myWSDelegate As New WorkSessionsDelegate
                                If (NEWAddWorkSession) Then
                                    'BT  #1545
                                    resultData = myWSDelegate.AddWorkSession_NEW(dbConnection, myWSOrderTestsDS, True, pNewAnalyzerID)
                                Else
                                    resultData = myWSDelegate.AddWorkSession(dbConnection, myWSOrderTestsDS, True, pNewAnalyzerID)
                                End If

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                                    Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                                    If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                                        If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                                            Dim myLogAcciones As New ApplicationLogManager()
                                            myLogAcciones.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", _
                                                                            "OrderTestsDelegate.LoadFromSavedWSToChangeAnalyzer", EventLogEntryType.Error, False)
                                        End If
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.LoadFromSavedWSToChangeAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new BLANK Order containing all informing Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsDS">Typed DataSet OrderTestsDS containing all BLANK OrderTests to add to a new Blank Order</param>
        ''' <param name="pMaxDaysLastBlank">Maximum number of days that can have passed from the last Blank execution for the informed Test. 
        '''                                 It is an optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed Dataset WSOrderTestsDS with all OrderTests  </returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' </remarks>
        Private Function MoveBlanksFromSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsDS As OrderTestsDS, _
                                               Optional ByVal pMaxDaysLastBlank As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myResultsDelegate As New ResultsDelegate
                        Dim myHistWSResults As New HisWSResultsDelegate
                        Dim myWSAddElementsDS As New WSAdditionalElementsDS

                        For Each row As OrderTestsDS.twksOrderTestsRow In pOrderTestsDS.twksOrderTests
                            'Search if there is a previous result for the Blank of each Test 
                            resultData = myResultsDelegate.GetLastExecutedBlank(dbConnection, row.TestID, row.TestVersionNumber, row.AnalyzerID, pMaxDaysLastBlank, False)


                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myWSAddElementsDS = DirectCast(resultData.SetDatos, WSAdditionalElementsDS)

                                If (myWSAddElementsDS.WSAdditionalElementsTable.Rows.Count > 0) Then
                                    row.PreviousOrderTestID = myWSAddElementsDS.WSAdditionalElementsTable.First.PreviousOrderTestID
                                End If
                            Else
                                'Error searching the result of the last executed Blank for the Test
                                Exit For
                            End If
                        Next

                        If (Not resultData.HasError) Then
                            'A new BLANK Order has to be created: Generate the next OrderID
                            Dim myOrdersDelegate As New OrdersDelegate
                            resultData = myOrdersDelegate.GenerateOrderID(dbConnection)

                            If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                                Dim orderID As String = CType(resultData.SetDatos, String)

                                'Prepare the DataSet containing the Order data
                                Dim myOrdersDS As New OrdersDS
                                Dim myOrderDR As OrdersDS.twksOrdersRow

                                myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                myOrderDR.OrderID = orderID
                                myOrderDR.SampleClass = "BLANK"
                                myOrderDR.StatFlag = False
                                myOrderDR.OrderDateTime = DateTime.Now
                                myOrderDR.OrderStatus = "OPEN"
                                myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                'Create the new Blank Order with all the new Order Tests
                                resultData = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, pOrderTestsDS, Nothing)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.MoveBlanksFromSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new CALIBRATOR Order containing all informing Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsDS">Typed DataSet OrderTestsDS containing all CALIBRATOR OrderTests to add to a new Calibrator Order</param>
        ''' <param name="pMaxDaysLastCalib">Maximum number of days that can have passed from the last Calibrator execution for the informed Test/SampleType
        '''                                 It is an optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' Modified by: SA 25/06/2012 - Added management of Alternative Calibrators
        '''              SA 02/08/2012 - Changes to set the proper CreationOrder number for each OrderTest obtained from the saved Work Session
        ''' </remarks>
        Private Function MoveCalibratorsFromSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsDS As OrderTestsDS, _
                                                    Optional ByVal pMaxDaysLastCalib As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestCalibDS As TestCalibratorsDS
                        Dim myTestCalibDelegate As New TestCalibratorsDelegate

                        Dim myResultsDelegate As New ResultsDelegate
                        Dim myHistWSResults As New HisWSResultsDelegate
                        Dim myWSAddElementsDS As New WSAdditionalElementsDS

                        Dim mySampleType As String
                        For Each row As OrderTestsDS.twksOrderTestsRow In pOrderTestsDS.twksOrderTests
                            mySampleType = row.SampleType
                            If (row.CalibratorType = "ALTERNATIV") Then mySampleType = row.SampleTypeAlternative

                            'Get the Identifier of the Calibrator used for the Test/SampleType
                            resultData = myTestCalibDelegate.GetTestCalibratorByTestID(dbConnection, row.TestID, mySampleType)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myTestCalibDS = DirectCast(resultData.SetDatos, TestCalibratorsDS)

                                If (myTestCalibDS.tparTestCalibrators.Rows.Count = 1) Then
                                    row.BeginEdit()
                                    row.CalibratorID = myTestCalibDS.tparTestCalibrators.First.CalibratorID
                                    row.EndEdit()
                                End If
                            Else
                                'Error getting the Calibrator Identifier
                                Exit For
                            End If

                            If (row.CalibratorType = "EXPERIMENT") Then
                                'Verify if there is a previous result for the Calibrator
                                resultData = myResultsDelegate.GetLastExecutedCalibrator(dbConnection, row.TestID, row.SampleType, row.TestVersionNumber, _
                                                                                         row.AnalyzerID, pMaxDaysLastCalib, False)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myWSAddElementsDS = DirectCast(resultData.SetDatos, WSAdditionalElementsDS)

                                    If (myWSAddElementsDS.WSAdditionalElementsTable.Rows.Count > 0) Then
                                        row.PreviousOrderTestID = myWSAddElementsDS.WSAdditionalElementsTable.First.PreviousOrderTestID
                                    End If
                                Else
                                    'Error searching the result of the last executed Calibrator for the Test
                                    Exit For
                                End If
                            End If
                        Next

                        If (Not resultData.HasError) Then
                            Dim lstDiffCalibs As List(Of Integer) = (From a In pOrderTestsDS.twksOrderTests _
                                                                 Order By a.CreationOrder _
                                                                   Select a.CalibratorID Distinct).ToList

                            Dim lastCreationOrder As Integer = 1
                            Dim lstOrderTests As List(Of OrderTestsDS.twksOrderTestsRow)
                            For Each diffCalibrator As Integer In lstDiffCalibs
                                'Get the list of requested Order Tests for the Calibrator
                                lstOrderTests = (From b As OrderTestsDS.twksOrderTestsRow In pOrderTestsDS.twksOrderTests _
                                                Where b.CalibratorID = diffCalibrator _
                                             Order By b.CreationOrder _
                                               Select b).ToList

                                'Update the CreationOrder field in the DS containing all Order Tests to add to the Order (the CreationOrder in the
                                'saved WS cannot be used due to unselected Tests are placed at the end of the whole WS Control list)
                                For Each otRow As OrderTestsDS.twksOrderTestsRow In lstOrderTests
                                    otRow.BeginEdit()
                                    otRow.CreationOrder = lastCreationOrder
                                    otRow.EndEdit()

                                    lastCreationOrder += 1
                                Next
                            Next
                            lstOrderTests = Nothing
                            lstDiffCalibs = Nothing

                            'A new CALIBRATOR Order has to be created: Generate the next OrderID
                            Dim myOrdersDelegate As New OrdersDelegate
                            resultData = myOrdersDelegate.GenerateOrderID(dbConnection)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim orderID As String = CType(resultData.SetDatos, String)

                                'Prepare the DataSet containing the Order data
                                Dim myOrdersDS As New OrdersDS
                                Dim myOrderDR As OrdersDS.twksOrdersRow

                                myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                                myOrderDR.OrderID = orderID
                                myOrderDR.SampleClass = "CALIB"
                                myOrderDR.StatFlag = False
                                myOrderDR.OrderDateTime = DateTime.Now
                                myOrderDR.OrderStatus = "OPEN"
                                myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                                'Create the new Calibrator Order with all the new Order Tests
                                resultData = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, pOrderTestsDS, Nothing)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                                    'Verify if there are requested Tests/SampleTypes using an Alternative Calibrator
                                    Dim lstAlternativeCalibs As List(Of OrderTestsDS.twksOrderTestsRow) = (From a As OrderTestsDS.twksOrderTestsRow In pOrderTestsDS.twksOrderTests _
                                                                                                          Where a.CalibratorType = "ALTERNATIV" _
                                                                                                         Select a).ToList()
                                    Dim alternativeOT As List(Of Integer)
                                    Dim myOrderTestDAO As New TwksOrderTestsDAO
                                    For Each row As OrderTestsDS.twksOrderTestsRow In lstAlternativeCalibs
                                        'Search the OrderTestID for the SampleType Alternative
                                        alternativeOT = (From b In pOrderTestsDS.twksOrderTests _
                                                        Where b.TestID = row.TestID _
                                                      AndAlso b.SampleType = row.SampleTypeAlternative _
                                                       Select b.OrderTestID).ToList

                                        If (alternativeOT.Count = 1) Then
                                            'Update field AlternativeOrderTestID in DB
                                            resultData = myOrderTestDAO.UpdateAlternativeOTByOrderTestID(dbConnection, row.OrderTestID, alternativeOT.First)
                                            If (resultData.HasError) Then Exit For

                                            'Update field AlternativeOrderTestID also in the DS; besides, if previous results had been found in Historics Module
                                            'for the Test/SampleType, then set the PreviousOrderTestID to NULL (due to the previous results for the 
                                            'Test/AlternativeSampleType will be shown as previous results for it)
                                            row.BeginEdit()
                                            row.AlternativeOrderTestID = alternativeOT.First
                                            row.SetPreviousOrderTestIDNull()
                                            row.SetPreviousWSIDNull()
                                            row.EndEdit()
                                        End If
                                    Next
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.MoveCalibratorsFromSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new CONTROL Order containing all informing Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsDS">Typed DataSet OrderTestsDS containing all CONTROL OrderTests to add to a new Control Order</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' Modified by: SA 02/08/2012 - Changes to set the proper CreationOrder number for each OrderTest obtained from
        '''                              the saved Work Session
        ''' </remarks>
        Private Function MoveControlsFromSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim lstDiffControls As List(Of Integer) = (From a In pOrderTestsDS.twksOrderTests _
                                                               Order By a.CreationOrder _
                                                                  Select a.ControlID Distinct).ToList

                        Dim lastCreationOrder As Integer = 1
                        Dim lstOrderTests As List(Of OrderTestsDS.twksOrderTestsRow)
                        For Each diffControl As Integer In lstDiffControls
                            'Get the list of requested Order Tests for the Control
                            lstOrderTests = (From b As OrderTestsDS.twksOrderTestsRow In pOrderTestsDS.twksOrderTests _
                                            Where b.ControlID = diffControl _
                                         Order By b.CreationOrder _
                                           Select b).ToList

                            'Update the CreationOrder field in the DS containing all Order Tests to add to the Order (the CreationOrder in the
                            'saved WS cannot be used due to unselected Tests are placed at the end of the whole WS Control list)
                            For Each otRow As OrderTestsDS.twksOrderTestsRow In lstOrderTests
                                otRow.BeginEdit()
                                otRow.CreationOrder = lastCreationOrder
                                otRow.EndEdit()

                                lastCreationOrder += 1
                            Next
                        Next
                        lstOrderTests = Nothing
                        lstDiffControls = Nothing

                        'A new CONTROL Order has to be created: Generate the next OrderID
                        Dim myOrdersDelegate As New OrdersDelegate
                        resultData = myOrdersDelegate.GenerateOrderID(dbConnection)

                        If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                            Dim orderID As String = CType(resultData.SetDatos, String)

                            'Prepare the DataSet containing the Order data
                            Dim myOrdersDS As New OrdersDS
                            Dim myOrderDR As OrdersDS.twksOrdersRow

                            myOrderDR = myOrdersDS.twksOrders.NewtwksOrdersRow
                            myOrderDR.OrderID = orderID
                            myOrderDR.SampleClass = "CTRL"
                            myOrderDR.StatFlag = False
                            myOrderDR.OrderDateTime = DateTime.Now
                            myOrderDR.OrderStatus = "OPEN"
                            myOrdersDS.twksOrders.AddtwksOrdersRow(myOrderDR)

                            'Create the new Control Order with all the new Order Tests
                            resultData = myOrdersDelegate.CreateOrder(dbConnection, myOrdersDS, pOrderTestsDS, Nothing)
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.MoveControlsFromSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create new PATIENT Orders containing all informing Order Tests for each different PatientID/SampleID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSavedWSID">Identifier of the SavedWS to load</param>
        ''' <param name="pNewAnalyzerID">Identifier of the new connected Analyzer</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' Modified by: SA 02/08/2012 - Changes to set the proper CreationOrder number for each OrderTest obtained from
        '''                              the saved Work Session
        '''              SA 04/06/2013 - When a SampleID is moved to field PatientID in an Order, its content has to be changed to Uppercase characters, due
        '''                              to this is the way PatientIDs are saved in tparPatients table
        ''' </remarks>
        Private Function MovePatientsFromSavedWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedWSID As Integer, _
                                                 ByVal pNewAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim orderDS As New OrdersDS
                        Dim orderTestsDS As New OrderTestsDS
                        Dim myOrdersDelegate As New OrdersDelegate
                        Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate

                        resultData = mySavedWSOrderTests.GetAllDifferentPatients(dbConnection, pSavedWSID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myOrdersDS As OrdersDS = DirectCast(resultData.SetDatos, OrdersDS)

                            Dim orderID As String = String.Empty
                            Dim lastCreationOrder As Integer = 1
                            For Each patientRow As OrdersDS.twksOrdersRow In myOrdersDS.twksOrders
                                'Move the Patient data to an empty Orders DataSet
                                orderDS.Clear()
                                orderDS.twksOrders.ImportRow(patientRow)

                                'A new PATIENT Order has to be created: Generate the next OrderID
                                resultData = myOrdersDelegate.GenerateOrderID(dbConnection)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    orderID = CType(resultData.SetDatos, String)

                                    orderDS.twksOrders.First.BeginEdit()
                                    orderDS.twksOrders.First.OrderID = orderID
                                    orderDS.twksOrders.First.SampleClass = "PATIENT"

                                    If (patientRow.SampleIDType = "DB") Then
                                        orderDS.twksOrders.First.PatientID = patientRow.SampleID.ToUpperBS
                                        orderDS.twksOrders.First.SetSampleIDNull()
                                    Else
                                        orderDS.twksOrders.First.SetPatientIDNull()
                                        orderDS.twksOrders.First.SampleID = patientRow.SampleID
                                    End If

                                    orderDS.twksOrders.First.OrderStatus = "OPEN"
                                    orderDS.twksOrders.First.OrderDateTime = DateTime.Now
                                    orderDS.twksOrders.First.EndEdit()
                                End If

                                'Get all OrderTests requested for the PatientID and StatFlag
                                resultData = mySavedWSOrderTests.GetOrderTestsToChangeAnalyzer(dbConnection, pSavedWSID, "PATIENT", patientRow.StatFlag, _
                                                                                               pNewAnalyzerID, patientRow.SampleID)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    orderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                                    'Update the CreationOrder field in the DS containing all Order Tests to add to the Order (the CreationOrder in the
                                    'saved WS cannot be used due to unselected Tests are placed at the end of the whole WS Patients list)
                                    For Each otRow As OrderTestsDS.twksOrderTestsRow In orderTestsDS.twksOrderTests
                                        otRow.BeginEdit()
                                        otRow.CreationOrder = lastCreationOrder
                                        otRow.EndEdit()

                                        lastCreationOrder += 1
                                    Next

                                    'Create the new Patient Order
                                    resultData = myOrdersDelegate.CreateOrder(dbConnection, orderDS, orderTestsDS, Nothing)
                                End If
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.MoveControlsFromSavedWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Process to add to the active WS all Control Order Tests in the Saved WS
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer identifier</param>
        ''' <param name="pWSResultDS">Current WS</param>
        ''' <param name="pSavedWSOrderTestsDS">Saved WS</param>
        ''' <param name="pRejectedOTLISInfo">LIS Rejected Data Information</param>
        ''' <returns>GlobalDataTO containing an integer value (0 = No Control was added; 1 = At least a Control was added)</returns>
        ''' <remarks>
        ''' Created by : XB 26/03/2013
        ''' Modified by: XB 05/04/2013 - April implementation: All Controls already existing on WS are rejected (all rerun requests for Internal 
        '''                              Controls are rejected)
        '''              SA 23/04/2013 - Changed the value returned by function: it has to be an integer value indicating if at least a Control 
        '''                              has been added to the WorkSession (0=None; 1=At least one)
        '''              AG 13/05/2013 - Added changes for rejecting all Order Tests in LIS Saved WS which Test has been deleted in BAx00 (field DeletedTestFlag is TRUE)
        '''             JCM 16/05/2013 - Management of rejected Awos has been moved to private function RejectControlTest.  After calling function AddControlOrderTests,
        '''                              sends a reject message for the Awos of all TestType/TestID/SampleType that were not added to the active Work Session due to 
        '''                              the QC is not active for them (flag Added = False in the SelectedTestsDS)
        '''              SA 11/06/2013 - Call function ImportErrorsLogDelegate.Add to save data loaded in the ImportErrorsLogDS in table twksImportErrorsLog (code is commented)
        '''              SA 17/02/2014 - BT #1510 ==> Code for adding rejected Order Tests to the entry DS pRejectedOTLISInfoDS has been commented to avoid the sending 
        '''                                           of rejecting messages to LIS and reduce message traffic. Exception: the rejection due to LIMS_TEST_WITH_NO_QC.
        '''              SA 11/03/2014 - BT #1536 ==> Removed entry parameter pDBConnection: it is not used
        ''' </remarks>
        Public Function ProcessLISControlOTs(ByVal pAnalyzerID As String, ByRef pWSResultDS As WorkSessionResultDS, ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS, _
                                             ByRef pRejectedOTLISInfo As OrderTestsLISInfoDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim mySelectedTestsDS As SelectedTestsDS = Nothing
                Dim myImportErrorsLogDS As ImportErrorsLogDS = Nothing

                'Get all CONTROL Order Tests in the Saved WS
                Dim lstControlOrderTests As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
                lstControlOrderTests = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
                                       Where a.SampleClass = "CTRL" _
                                      Select a).ToList

                For Each mySavedrow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstControlOrderTests
                    If (mySavedrow.DeletedTestFlag) Then
                        'If DeletedTestFlag is TRUE for the Order Test in the Saved WS, it means the Test or the Test/Sample Type 
                        'has been deleted; in this case, the AwosID is rejected
                        'BT #1510 ==> Sending of REJECTED Messages to LIS has been canceled
                        'RejectControlTest(mySavedrow, pRejectedOTLISInfo, myImportErrorsLogDS, GlobalEnumerates.Messages.LIMS_INVALID_TEST)
                    Else
                        'Search in the active WS (pWsResultsDS.Controls) if there is an Order Test for the same TestType, TestID, SampleType
                        Dim lstWSOrderTests As List(Of WorkSessionResultDS.ControlsRow)
                        lstWSOrderTests = (From a As WorkSessionResultDS.ControlsRow In pWSResultDS.Controls _
                                          Where a.SampleClass = "CTRL" _
                                        AndAlso a.TestType = mySavedrow.TestType _
                                        AndAlso a.TestID = mySavedrow.TestID _
                                        AndAlso a.SampleType = mySavedrow.SampleType _
                                         Select a).ToList

                        If (lstWSOrderTests.Count > 0) Then
                            'XB 05/04/2013 - BY NOW, ALL CONTROL LIS REQUESTS that already exists are REJECTED - Meeting SA+EF 05/04/2013
                            'BT #1510 ==> Sending of REJECTED Messages to LIS has been canceled
                            'RejectControlTest(mySavedrow, pRejectedOTLISInfo, myImportErrorsLogDS, GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN)

                            'XB 05/04/2013 - CANCELED BY NOW - meeting SA+EF 05/04/2013
                            'Dim finalOrderTestsLISInfoDS As OrderTestsLISInfoDS = Nothing
                            'Dim ThereAreExportedResults As Boolean = False
                            'Dim myExportedResults As Integer    
                            'Dim myRerunNumber As Integer     

                            'If lstWSOrderTests(0).LISRequest Then
                            '    ' If the Control was requested by LIS, the Rerun request is rejected 
                            '    ' Reruns of internal Controls are not allowed

                            '    ' Create the new row LIS data information
                            '    If pRejectedOTLISInfo Is Nothing Then
                            '        pRejectedOTLISInfo = New OrderTestsLISInfoDS
                            '    End If
                            '    Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = pRejectedOTLISInfo.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                            '    With myRow
                            '        .BeginEdit()
                            '        If Not mySavedrow.IsAwosIDNull Then
                            '            .AwosID = mySavedrow.AwosID
                            '        End If
                            '        If Not mySavedrow.IsSpecimenIDNull Then
                            '            .SpecimenID = mySavedrow.SpecimenID
                            '        End If
                            '        If Not mySavedrow.IsESOrderIDNull Then
                            '            .ESOrderID = mySavedrow.ESOrderID
                            '        End If
                            '        If Not mySavedrow.IsLISOrderIDNull Then
                            '            .LISOrderID = mySavedrow.LISOrderID
                            '        End If
                            '        If Not mySavedrow.IsESPatientIDNull Then
                            '            .ESPatientID = mySavedrow.ESPatientID
                            '        End If
                            '        If Not mySavedrow.IsLISPatientIDNull Then
                            '            .LISPatientID = mySavedrow.LISPatientID
                            '        End If
                            '        .TestType = mySavedrow.TestType
                            '        .TestID = mySavedrow.TestID
                            '        .SampleType = mySavedrow.SampleType
                            '        .SampleClass = "QC"
                            '        .StatFlagText = "normal"
                            '        .CheckLISValues = True
                            '        .EndEdit()
                            '    End With
                            '    pRejectedOTLISInfo.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                            '    pRejectedOTLISInfo.AcceptChanges()

                            '    ' Report Error details
                            '    If myImportErrorsLogDS Is Nothing Then
                            '        myImportErrorsLogDS = New ImportErrorsLogDS
                            '    End If
                            '    Dim myErrLogRow As ImportErrorsLogDS.twksImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                            '    With myErrLogRow
                            '        .BeginEdit()
                            '        .MessageID = mySavedrow.SavedWSName
                            '        If Not mySavedrow.IsAwosIDNull Then
                            '            .AwosID = mySavedrow.AwosID
                            '        End If
                            '        .ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                            '        .EndEdit()
                            '    End With
                            '    myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myErrLogRow)
                            '    myImportErrorsLogDS.AcceptChanges()

                            'Else
                            '    ' The same TestType/Test/SampleType/ControlID exists in the active WS, but was manually requested

                            '    ' Verify if there are already results for the Order Test (get them sorted by RerunNumber DESC)
                            '    Dim myResultsDelegate As New ResultsDelegate
                            '    Dim myResultsDS As New ResultsDS

                            '    'Get all results for the OrderTest sorted by RerunNumber DESC
                            '    resultData = myResultsDelegate.GetResultsByOrderTest(Nothing, lstWSOrderTests(0).OrderTestID)
                            '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            '        myResultsDS = CType(resultData.SetDatos, ResultsDS)
                            '        If myResultsDS.vwksResults.Rows.Count > 0 Then
                            '            ' Is there at least a Result for the Order Test

                            '            myExportedResults = (From a As ResultsDS.vwksResultsRow In myResultsDS.vwksResults _
                            '                                 Where a.ExportStatus = "SENT" _
                            '                                 Select a).Count

                            '            ThereAreExportedResults = (myExportedResults > 0)

                            '            If ThereAreExportedResults Then
                            '                ' If at least a result has been exported to LIS, the Rerun request is rejected; Reruns of internal Controls are not allowed
                            '                ' Fill a new row LIS data information
                            '                If pRejectedOTLISInfo Is Nothing Then
                            '                    pRejectedOTLISInfo = New OrderTestsLISInfoDS
                            '                End If
                            '                Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = pRejectedOTLISInfo.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                            '                With myRow
                            '                    .BeginEdit()
                            '                    If Not mySavedrow.IsAwosIDNull Then
                            '                        .AwosID = mySavedrow.AwosID
                            '                    End If
                            '                    If Not mySavedrow.IsSpecimenIDNull Then
                            '                        .SpecimenID = mySavedrow.SpecimenID
                            '                    End If
                            '                    If Not mySavedrow.IsESOrderIDNull Then
                            '                        .ESOrderID = mySavedrow.ESOrderID
                            '                    End If
                            '                    If Not mySavedrow.IsLISOrderIDNull Then
                            '                        .LISOrderID = mySavedrow.LISOrderID
                            '                    End If
                            '                    If Not mySavedrow.IsESPatientIDNull Then
                            '                        .ESPatientID = mySavedrow.ESPatientID
                            '                    End If
                            '                    If Not mySavedrow.IsLISPatientIDNull Then
                            '                        .LISPatientID = mySavedrow.LISPatientID
                            '                    End If
                            '                    .OrderTestID = lstWSOrderTests(0).OrderTestID
                            '                    .TestType = mySavedrow.TestType
                            '                    .TestID = mySavedrow.TestID
                            '                    .SampleType = mySavedrow.SampleType
                            '                    .SampleClass = "QC"
                            '                    .StatFlagText = "normal"
                            '                    .CheckLISValues = True
                            '                    .EndEdit()
                            '                End With
                            '                pRejectedOTLISInfo.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                            '                pRejectedOTLISInfo.AcceptChanges()

                            '                ' Report Error details
                            '                If myImportErrorsLogDS Is Nothing Then
                            '                    myImportErrorsLogDS = New ImportErrorsLogDS
                            '                End If
                            '                Dim myErrLogRow As ImportErrorsLogDS.twksImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                            '                With myErrLogRow
                            '                    .BeginEdit()
                            '                    .MessageID = mySavedrow.SavedWSName
                            '                    If Not mySavedrow.IsAwosIDNull Then
                            '                        .AwosID = mySavedrow.AwosID
                            '                    End If
                            '                    .ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                            '                    .EndEdit()
                            '                End With
                            '                myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myErrLogRow)
                            '                myImportErrorsLogDS.AcceptChanges()

                            '            Else
                            '                ' The new request is considered the same
                            '                ' LIS fields are linked to the last Rerun requested for the  existing OrderTest
                            '                myRerunNumber = myResultsDS.vwksResults(0).RerunNumber
                            '                lstWSOrderTests(0).LISRequest = True
                            '                ' Fill a new row LIS data information as Final order Test LIS
                            '                If finalOrderTestsLISInfoDS Is Nothing Then
                            '                    finalOrderTestsLISInfoDS = New OrderTestsLISInfoDS
                            '                End If
                            '                Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = finalOrderTestsLISInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                            '                With myRow
                            '                    .BeginEdit()
                            '                    If Not mySavedrow.IsAwosIDNull Then
                            '                        .AwosID = mySavedrow.AwosID
                            '                    End If
                            '                    If Not mySavedrow.IsSpecimenIDNull Then
                            '                        .SpecimenID = mySavedrow.SpecimenID
                            '                    End If
                            '                    If Not mySavedrow.IsESOrderIDNull Then
                            '                        .ESOrderID = mySavedrow.ESOrderID
                            '                    End If
                            '                    If Not mySavedrow.IsLISOrderIDNull Then
                            '                        .LISOrderID = mySavedrow.LISOrderID
                            '                    End If
                            '                    If Not mySavedrow.IsESPatientIDNull Then
                            '                        .ESPatientID = mySavedrow.ESPatientID
                            '                    End If
                            '                    If Not mySavedrow.IsLISPatientIDNull Then
                            '                        .LISPatientID = mySavedrow.LISPatientID
                            '                    End If
                            '                    .OrderTestID = lstWSOrderTests(0).OrderTestID
                            '                    .RerunNumber = myRerunNumber
                            '                    .EndEdit()
                            '                End With
                            '                finalOrderTestsLISInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                            '                finalOrderTestsLISInfoDS.AcceptChanges()
                            '            End If

                            '        Else
                            '            ' The new request is considered the same
                            '            ' LIS fields are linked to the existing OrderTest 
                            '            myRerunNumber = 1
                            '            lstWSOrderTests(0).LISRequest = True
                            '            ' Fill a new row LIS data information as Final order Test LIS
                            '            If finalOrderTestsLISInfoDS Is Nothing Then
                            '                finalOrderTestsLISInfoDS = New OrderTestsLISInfoDS
                            '            End If
                            '            Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = finalOrderTestsLISInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                            '            With myRow
                            '                .BeginEdit()
                            '                If Not mySavedrow.IsAwosIDNull Then
                            '                    .AwosID = mySavedrow.AwosID
                            '                End If
                            '                If Not mySavedrow.IsSpecimenIDNull Then
                            '                    .SpecimenID = mySavedrow.SpecimenID
                            '                End If
                            '                If Not mySavedrow.IsESOrderIDNull Then
                            '                    .ESOrderID = mySavedrow.ESOrderID
                            '                End If
                            '                If Not mySavedrow.IsLISOrderIDNull Then
                            '                    .LISOrderID = mySavedrow.LISOrderID
                            '                End If
                            '                If Not mySavedrow.IsESPatientIDNull Then
                            '                    .ESPatientID = mySavedrow.ESPatientID
                            '                End If
                            '                If Not mySavedrow.IsLISPatientIDNull Then
                            '                    .LISPatientID = mySavedrow.LISPatientID
                            '                End If
                            '                .OrderTestID = lstWSOrderTests(0).OrderTestID
                            '                .RerunNumber = myRerunNumber
                            '                .EndEdit()
                            '            End With
                            '            finalOrderTestsLISInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                            '            finalOrderTestsLISInfoDS.AcceptChanges()
                            '        End If
                            '    End If
                            'End If
                            ' XB 05/04/2013 - CANCELED BY NOW - meeting SA+EF 05/04/2013

                        Else
                            'Create a row of SelectedTestsDS and load the Test / Sample Type information in it. Fill fields TestType, TestID and SampleType 
                            If (mySelectedTestsDS Is Nothing) Then mySelectedTestsDS = New SelectedTestsDS

                            Dim mySelTestsRow As SelectedTestsDS.SelectedTestTableRow = mySelectedTestsDS.SelectedTestTable.NewSelectedTestTableRow
                            With mySelTestsRow
                                .BeginEdit()
                                .TestType = mySavedrow.TestType
                                .TestID = mySavedrow.TestID
                                .SampleType = mySavedrow.SampleType
                                .Selected = True
                                .LISRequest = True
                                If (Not mySavedrow.IsAwosIDNull) Then .AwosID = mySavedrow.AwosID
                                If (Not mySavedrow.IsSpecimenIDNull) Then .SpecimenID = mySavedrow.SpecimenID
                                If (Not mySavedrow.IsESOrderIDNull) Then .ESOrderID = mySavedrow.ESOrderID
                                If (Not mySavedrow.IsLISOrderIDNull) Then .LISOrderID = mySavedrow.LISOrderID
                                If (Not mySavedrow.IsESPatientIDNull) Then .ESPatientID = mySavedrow.ESPatientID
                                If (Not mySavedrow.IsLISPatientIDNull) Then .LISPatientID = mySavedrow.LISPatientID
                                .EndEdit()
                            End With
                            mySelectedTestsDS.SelectedTestTable.AddSelectedTestTableRow(mySelTestsRow)
                            mySelectedTestsDS.AcceptChanges()
                        End If
                    End If
                Next

                Dim addedControls As Integer = 0
                If (Not resultData.HasError AndAlso Not mySelectedTestsDS Is Nothing AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) Then
                    'Add Controls, Calibrators and Blanks for the group of Tests / Sample Types in the DS
                    resultData = AddControlOrderTests(mySelectedTestsDS, pWSResultDS, pAnalyzerID, False, True, "CTRL")

                    'JC 16-05-2013
                    If (Not resultData.HasError) Then
                        'Get all TestType/TestID/SampleType for which no Controls were added to the active Work Session (those having Added=False in the DS)
                        Dim lstSelectedNotAdded As List(Of SelectedTestsDS.SelectedTestTableRow) = (From a As SelectedTestsDS.SelectedTestTableRow In mySelectedTestsDS.SelectedTestTable _
                                                                                                   Where Not a.Added _
                                                                                                  Select a).ToList()

                        'Add all these AWOS to the DS containing the Rejected ones
                        For Each drRejected As SelectedTestsDS.SelectedTestTableRow In lstSelectedNotAdded
                            RejectControlTest(drRejected, pRejectedOTLISInfo, myImportErrorsLogDS, GlobalEnumerates.Messages.LIMS_TEST_WITH_NO_QC)
                        Next

                        'If there is at least a TestType/TestID/SampleType for which at least a Control was added to the active Work Session, set addedControls to 1
                        'to indicate the WS Samples Request Screen has to be opened when the Orders Download process finishes
                        If (mySelectedTestsDS.SelectedTestTable.Count > lstSelectedNotAdded.Count) Then
                            addedControls = 1
                        End If
                    End If
                End If

                If (Not resultData.HasError AndAlso Not myImportErrorsLogDS Is Nothing AndAlso myImportErrorsLogDS.twksImportErrorsLog.Rows.Count > 0) Then
                    'Write the content of DataSet LISImportErrorDS to table twksImportErrorsLog
                    Dim myImportErrorLog As New ImportErrorsLogDelegate
                    resultData = myImportErrorLog.Add(Nothing, myImportErrorsLogDS)
                End If

                resultData.SetDatos = addedControls
                resultData.HasError = False
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ProcessLISControlOTs", EventLogEntryType.Error, False)
                Throw
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Process to add to the active WS all Patient Order Tests in the Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pWorkSessionResultDS">Typed Dataset WorkSessionResultDS containing all Order Tests (of all Sample Classes) that are included
        '''                                    in the active WS</param>
        ''' <param name="pSavedWSOrderTestsDS">Typed Dataset SavedWSOrderTestsDS</param>
        ''' <param name="pRerunLISMode">Current value of setting for allowed Rerun LIS Mode: BOTH, ONLY ANALYZER or ONLY LIS</param>
        ''' <param name="pISEModuleReady">Flag indicating if the ISE Module is ready</param>
        ''' <param name="pRejectedOTLISInfo">Typed DataSet OrderTestsLISInfoDS to return information of all AwosID that have been rejected</param>
        ''' <param name="pAutoWSCreationInProcess">Flag indicating if the function has been called by the process for automatic WS creation</param>
        ''' <param name="pRunningMode">When TRUE, it indicates the ManageRepetitions function the Analyzer is in RUNNING mode; otherwise it indicates
        '''                            the Analyzer is in STANDBY</param>
        ''' <param name="pMsgDateTime">Sending Date and Time of the LIS Message from which the Patient Order Test to process was extracted</param>
        ''' <returns>GlobalDataTO containing a typed DataSet with an integer value: 
        '''          0 = No Patient Sample was added; 
        '''          1 = Some Patient Samples were added, but there is not tube for any of them; 
        '''          2 = Some Patient Samples were added, and there is a tube for at least one of them</returns>
        ''' <remarks>
        ''' Created by:  TR 25/03/2013
        ''' Modified by: TR 29/04/2013 - Search in the active WS (pWsResultsDS.Patients) if there is an Order Test requested by LIS with the same 
        '''                              Specimen but for different SampleID. If the SpecimenID is linked to a different Patient in the WS, the 
        '''                              LIS request is rejected
        '''              SA 30/04/2013 - When an Order Test has been found in the WS for the same Patient/TestType/TestID/SampleType and the LIS 
        '''                              request is not rejected, increment variables notSelectedPatients or selectedPatients according the Status 
        '''                              of the found Order Test (notSelectedPatients when OPEN or selectedPatients in any other case)
        '''              SA 03/05/2013 - Validation of duplicate Specimen has to be done before validation of duplicate Test/SampleType to manage Reruns
        '''              AG 13/05/2013 - Added changes for rejecting all Order Tests in LIS Saved WS which Test has been deleted in BAx00 (field 
        '''                              DeletedTestFlag is TRUE)
        '''              SA 11/06/2013 - When load data in an ImportErrorLogDS, inform field LineText as "MessageID | AwosID" instead of inform specific fields MessageID
        '''                              and AwosID in the DS (due to these fields have been deleted). Call function ImportErrorsLogDelegate.Add to save data loaded in the 
        '''                              ImportErrorsLogDS in table twksImportErrorsLog (code is commented)
        '''              SA 01/08/2013 - Before beginning process to validate if the LIS Patient Order Test can be added to the active Work Session, call new function
        '''                              SearchOrdersBySpecimenID to verify if there is already a manual Order with SampleID = SpecimenID sent by LIS, and in this case,
        '''                              update fields PatientID/SampleID of the existing Order with the SampleID of the LIS Order Test, and add it to the existing Order
        '''                              (the StatFlag sent by LIS is ignored in this case)
        '''              SA 05/09/2013 - Added new parameter pAutoWSCreationInProcess to indicate when the function has been called by the process for automatic WS creation.
        '''                              In this case, the count of Selected or NotSelected Order Tests is executed only when a new STD or ISE Test have been added to 
        '''                              the WS (due to only that type of Tests generates executions).
        '''              SA 17/02/2014 - BT #1510 ==> Code for adding rejected Order Tests to the entry DS pRejectedOTLISInfoDS has been commented to avoid the sending 
        '''                                           of rejecting messages to LIS and reduce message traffic. Exception: the rejection due to LIS_DUPLICATE_SPECIMEN. 
        '''              SA 17/03/2014 - BT #1536 ==> Changed the call to function ManageRepetitions in RepetitionsDelegate to inform its new optional parameter 
        '''                                           pLISDownloadProcess with value TRUE, to indicate to that function the open DB Connection informed has to be used
        '''              SA 28/03/2014 - BT #1556 ==> Changes to include error LIS_RERUN_WITH_DIF_SPECIMEN in the list of known errors that change the HasError flag 
        '''                                           of the GlobalDataTO to return to FALSE (to allow the deleting of the LIS SavedWS once it has been processed)
        '''              AG 31/03/2014 - BT #1565 ==> Added new parameter pRunningMode (required for ManageRepetitions)
        '''              SA 01/04/2014 - BT #1564 ==> Added new parameter for the Date and Time of the LIS Message from which the Patient Order Test to process was
        '''                                           extracted, and inform it when calling function VerifyRerunOfLISPatientOT
        ''' </remarks>
        Public Function ProcessLISPatientOTs(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, pWorkSessionID As String, _
                                             ByRef pWorkSessionResultDS As WorkSessionResultDS, ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS, _
                                             ByVal pRerunLISMode As String, pISEModuleReady As Boolean, ByRef pRejectedOTLISInfo As OrderTestsLISInfoDS, _
                                             ByVal pAutoWSCreationInProcess As Boolean, ByVal pRunningMode As Boolean, ByVal pMsgDateTime As Date) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim selectedPatients As Integer = 0
                        Dim notSelectedPatients As Integer = 0

                        Dim myImportErrorsLogDS As New ImportErrorsLogDS
                        Dim myRepetitionsDelegate As New RepetitionsDelegate
                        Dim myRepetitionsToAddDS As New WSRepetitionsToAddDS
                        Dim myFinalOrderTestsLISInfoDS As New OrderTestsLISInfoDS
                        Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate
                        Dim myImportErrorsLogRow As ImportErrorsLogDS.twksImportErrorsLogRow
                        Dim myPatientOrderTestList As New List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
                        Dim myOrderTestsLISInfoRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow
                        Dim myPatientOTOnActiveWSList As New List(Of WorkSessionResultDS.PatientsRow)

                        'On pSavedWSOrderTestsDS filter all patient order test (SampleClass = 'PATIENT')
                        myPatientOrderTestList = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
                                                 Where a.SampleClass = "PATIENT" Select a).ToList()

                        Dim countAdded As Boolean = True
                        For Each SavedWSOTRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In myPatientOrderTestList
                            'If DeletedTestFlag is TRUE for the Order Test in theSaved WS, it means the Test or the Test/Sample Type 
                            'has been deleted; in this case, the AwosID is rejected
                            If (SavedWSOTRow.DeletedTestFlag) Then
                                'Fill a row of OrderTestLISInfo
                                'BT #1510 ==> Sending of REJECTED Messages to LIS has been canceled
                                'myOrderTestsLISInfoRow = pRejectedOTLISInfo.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                                'myOrderTestsLISInfoRow.AwosID = SavedWSOTRow.AwosID
                                'myOrderTestsLISInfoRow.SpecimenID = SavedWSOTRow.SpecimenID
                                'myOrderTestsLISInfoRow.TestType = SavedWSOTRow.TestType
                                ''myOrderTestsLISInfoRow.TestID = SavedWSOTRow.TestID
                                'myOrderTestsLISInfoRow.TestIDString = SavedWSOTRow.TestName  'This field contains the LIS Test Name, not the Biosystem Test Name 
                                'myOrderTestsLISInfoRow.SampleType = SavedWSOTRow.SampleType
                                'myOrderTestsLISInfoRow.ESPatientID = SavedWSOTRow.ESPatientID
                                'myOrderTestsLISInfoRow.ESOrderID = SavedWSOTRow.ESOrderID
                                'myOrderTestsLISInfoRow.CheckLISValues = False

                                ''Inform the SampleClass
                                'If (SavedWSOTRow.SampleClass = "PATIENT" AndAlso Not SavedWSOTRow.ExternalQC) Then
                                '    myOrderTestsLISInfoRow.SampleClass = "patient"
                                'ElseIf (SavedWSOTRow.SampleClass = "PATIENT" AndAlso SavedWSOTRow.ExternalQC) Then
                                '    myOrderTestsLISInfoRow.SampleClass = "QC"
                                'End If

                                ''Inform StatFlagText
                                'If (SavedWSOTRow.StatFlag) Then
                                '    myOrderTestsLISInfoRow.StatFlagText = "stat"
                                'Else
                                '    myOrderTestsLISInfoRow.StatFlagText = "normal"
                                'End If
                                'pRejectedOTLISInfo.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myOrderTestsLISInfoRow)
                                'pRejectedOTLISInfo.twksOrderTestsLISInfo.AcceptChanges()

                                'Add a row to a LISImportErrorsDS informing Message Identifier, AwosID, and ErrorCode.
                                myImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                                myImportErrorsLogRow.ErrorCode = GlobalEnumerates.Messages.LIMS_INVALID_TEST.ToString
                                myImportErrorsLogRow.LineText = SavedWSOTRow.SavedWSName & " | " & SavedWSOTRow.AwosID & " | " & SavedWSOTRow.TestName
                                myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myImportErrorsLogRow)
                                myImportErrorsLogDS.twksImportErrorsLog.AcceptChanges()
                            Else
                                'Verify if there is already a manual Order with SampleID = SpecimenID sent by LIS, and in this case, update fields PatientID/SampleID 
                                'of the existing Order with the SampleID of the LIS Order Test
                                myGlobalDataTO = SearchOrdersBySpecimenID(dbConnection, SavedWSOTRow, pWorkSessionResultDS)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'Search in the active WS if there are Order Tests requested by LIS with the same Specimen but for different SampleID
                                '(Validate DUPLICATE SPECIMEN)
                                myPatientOTOnActiveWSList = (From a In pWorkSessionResultDS.Patients _
                                                             Where a.SampleClass = "PATIENT" _
                                                             AndAlso a.LISRequest _
                                                             AndAlso a.SampleID <> SavedWSOTRow.SampleID _
                                                             AndAlso Not a.IsSpecimenIDNull _
                                                             AndAlso a.SpecimenID = SavedWSOTRow.SpecimenID).ToList()

                                If (myPatientOTOnActiveWSList.Count = 0) Then
                                    'Search in the active WS, if there is an Order Test for the same SampleID, TestType, TestID and SampleType
                                    myPatientOTOnActiveWSList = (From a In pWorkSessionResultDS.Patients Where a.SampleClass = "PATIENT" _
                                                              AndAlso a.SampleID = SavedWSOTRow.SampleID _
                                                              AndAlso a.TestType = SavedWSOTRow.TestType _
                                                              AndAlso a.TestID = SavedWSOTRow.TestID _
                                                              AndAlso a.SampleType = SavedWSOTRow.SampleType _
                                                               Select a).ToList()

                                    If (myPatientOTOnActiveWSList.Count = 0) Then
                                        'Add the Patient to the active WorkSession.
                                        myGlobalDataTO = AddPatientOrderTestsFromLIS(pAnalyzerID, pWorkSessionID, SavedWSOTRow, pWorkSessionResultDS)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'According the value returned, increment the number of selected Patients (when TRUE) or the number of not selected Patients (when FALSE)
                                        If (Not pAutoWSCreationInProcess OrElse (SavedWSOTRow.TestType <> "OFFS" AndAlso SavedWSOTRow.TestType <> "CALC")) Then
                                            If (CBool(myGlobalDataTO.SetDatos)) Then
                                                'Increment Selected Patients
                                                selectedPatients += 1
                                            Else
                                                'Increment Not Selected Patients
                                                notSelectedPatients += 1
                                            End If
                                        End If
                                    Else
                                        'Found 
                                        If (SavedWSOTRow.IsAwosIDNull) Then
                                            'The Order Test was not requested by LIS, but added becaused it is included in the 
                                            'Formula of a Calculated Test requested by LIS
                                            If (myPatientOTOnActiveWSList.First().IsCalcTestIDNull OrElse myPatientOTOnActiveWSList.First().CalcTestID = String.Empty) Then
                                                myPatientOTOnActiveWSList.First().CalcTestID &= SavedWSOTRow.CalcTestIDs

                                            ElseIf (Not myPatientOTOnActiveWSList.First().CalcTestID.Contains(SavedWSOTRow.CalcTestIDs)) Then
                                                'Merge values in wsOT.CalcTestID with values in savedOT.CalcTestIDs and update value of wsOT.CalcTestID
                                                myPatientOTOnActiveWSList.First().CalcTestID = MergeValues(SavedWSOTRow.CalcTestIDs, _
                                                                                                           myPatientOTOnActiveWSList.First().CalcTestID)
                                            End If

                                            If (myPatientOTOnActiveWSList.First().IsCalcTestNameNull OrElse myPatientOTOnActiveWSList.First().CalcTestName = String.Empty) Then
                                                myPatientOTOnActiveWSList.First().CalcTestName &= SavedWSOTRow.CalcTestNames

                                            ElseIf (Not myPatientOrderTestList.First().CalcTestNames.Contains(SavedWSOTRow.CalcTestNames)) Then
                                                'Merge values in wsOT.CalcTestName with values in savedOT.CalcTestNames and update value of wsOT.CalcTestName.
                                                myPatientOTOnActiveWSList.First().CalcTestName = MergeValues(SavedWSOTRow.CalcTestNames, _
                                                                                                             myPatientOTOnActiveWSList.First().CalcTestName)
                                            End If
                                            myPatientOTOnActiveWSList.First().LISRequest = True

                                            If (Not pAutoWSCreationInProcess) Then
                                                If (myPatientOTOnActiveWSList.First().OTStatus <> "OPEN") Then
                                                    'Increment Selected Patients
                                                    selectedPatients += 1
                                                Else
                                                    'Increment Not Selected Patients
                                                    notSelectedPatients += 1
                                                End If
                                            End If
                                        Else
                                            If (myPatientOTOnActiveWSList.First().LISRequest) Then
                                                If (Not myPatientOTOnActiveWSList.First().IsAwosIDNull) Then
                                                    'The existing Order Test was requested by LIS: validate if the requested Rerun can be added
                                                    'BT #1564 - Inform parameter for the Message DateTime
                                                    myGlobalDataTO = VerifyRerunOfLISPatientOT(myPatientOTOnActiveWSList.First(), SavedWSOTRow, pRerunLISMode, _
                                                                                               myFinalOrderTestsLISInfoDS, myRepetitionsToAddDS, pMsgDateTime)
                                                Else
                                                    'If field LISRequest is TRUE but the AwosID is not informed, it means the Order Test was added due to 
                                                    'it is needed for an Order Test of a Calculated Test requested by LIS. 
                                                    If (Not myPatientOTOnActiveWSList.First().IsOrderTestIDNull) Then
                                                        'The OrderTest exists in the active WS and it has been already saved in DB; execute the process of 
                                                        'Verify Rerun of Manual Patient Order Test
                                                        myGlobalDataTO = VerifyRerunOfManualPatientOT(myPatientOTOnActiveWSList.First(), SavedWSOTRow, pRerunLISMode, _
                                                                                                      myFinalOrderTestsLISInfoDS, myRepetitionsToAddDS)
                                                    Else
                                                        'The LIS request for it is accepted, and LIS fields for the Order Test are updated with values received from LIS
                                                        myPatientOTOnActiveWSList.First().BeginEdit()
                                                        myPatientOTOnActiveWSList.First().AwosID = SavedWSOTRow.AwosID
                                                        myPatientOTOnActiveWSList.First().SpecimenID = SavedWSOTRow.SpecimenID
                                                        myPatientOTOnActiveWSList.First().ESPatientID = SavedWSOTRow.ESPatientID
                                                        myPatientOTOnActiveWSList.First().ESOrderID = SavedWSOTRow.ESOrderID
                                                        If (Not SavedWSOTRow.IsLISPatientIDNull) Then myPatientOTOnActiveWSList.First().LISPatientID = SavedWSOTRow.LISPatientID
                                                        If (Not SavedWSOTRow.IsLISOrderIDNull) Then myPatientOTOnActiveWSList.First().LISOrderID = SavedWSOTRow.LISOrderID
                                                        myPatientOTOnActiveWSList.First().EndEdit()
                                                    End If
                                                End If
                                            Else
                                                'The same TestType/Test/SampleType exists in the active WS for the Patient, but was manually requested.
                                                myGlobalDataTO = VerifyRerunOfManualPatientOT(myPatientOTOnActiveWSList.First(), SavedWSOTRow, pRerunLISMode, _
                                                                                              myFinalOrderTestsLISInfoDS, myRepetitionsToAddDS)
                                            End If

                                            If (myGlobalDataTO.HasError) Then
                                                'Fill a row of OrderTestsLISInfoDS with following information from pSavedWSOrderTestDS.Row: 
                                                'AwosID, SpecimenID, TestID, SampleType, ESPatientID, ESOrderID, SampleClass and StatFlagText..
                                                'BT #1510 ==> Sending of REJECTED Messages to LIS has been canceled
                                                'myOrderTestsLISInfoRow = pRejectedOTLISInfo.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                                                'myOrderTestsLISInfoRow.AwosID = SavedWSOTRow.AwosID
                                                'myOrderTestsLISInfoRow.SpecimenID = SavedWSOTRow.SpecimenID
                                                'myOrderTestsLISInfoRow.TestType = SavedWSOTRow.TestType
                                                'myOrderTestsLISInfoRow.TestID = SavedWSOTRow.TestID
                                                'myOrderTestsLISInfoRow.SampleType = SavedWSOTRow.SampleType
                                                'myOrderTestsLISInfoRow.ESPatientID = SavedWSOTRow.ESPatientID
                                                'myOrderTestsLISInfoRow.ESOrderID = SavedWSOTRow.ESOrderID
                                                'myOrderTestsLISInfoRow.CheckLISValues = True

                                                ''Inform the SampleClass
                                                'If (SavedWSOTRow.SampleClass = "PATIENT" AndAlso Not SavedWSOTRow.ExternalQC) Then
                                                '    myOrderTestsLISInfoRow.SampleClass = "patient"
                                                'ElseIf (SavedWSOTRow.SampleClass = "PATIENT" AndAlso SavedWSOTRow.ExternalQC) Then
                                                '    myOrderTestsLISInfoRow.SampleClass = "QC"
                                                'End If

                                                ''Inform StatFlagText
                                                'If (SavedWSOTRow.StatFlag) Then
                                                '    myOrderTestsLISInfoRow.StatFlagText = "stat"
                                                'Else
                                                '    myOrderTestsLISInfoRow.StatFlagText = "normal"
                                                'End If

                                                ''Add row to DS. 
                                                'pRejectedOTLISInfo.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myOrderTestsLISInfoRow)

                                                'Add a row to a LISImportErrorsDS informing Message Identifier, AwosID, and ErrorCode.
                                                myImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                                                myImportErrorsLogRow.ErrorCode = myGlobalDataTO.ErrorCode
                                                myImportErrorsLogRow.LineText = SavedWSOTRow.SavedWSName & " | " & SavedWSOTRow.AwosID

                                                'Add row to DS.
                                                myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myImportErrorsLogRow)
                                            Else
                                                If (Not pAutoWSCreationInProcess) Then
                                                    If (myPatientOTOnActiveWSList.First().OTStatus <> "OPEN") Then
                                                        'Increment Selected Patients
                                                        selectedPatients += 1
                                                    Else
                                                        'Increment Not Selected Patients
                                                        notSelectedPatients += 1
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                Else
                                    'If the SpecimenID is linked to a different Patient in the WS, the LIS request is rejected.
                                    'Fill a row of OrderTestsLISInfoDS with following information from pSavedWSOrderTestDS.Row: 
                                    'AwosID, SpecimenID, TestID, SampleType, ESPatientID, ESOrderID, SampleClass and StatFlagText..
                                    myOrderTestsLISInfoRow = pRejectedOTLISInfo.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                                    myOrderTestsLISInfoRow.AwosID = SavedWSOTRow.AwosID
                                    myOrderTestsLISInfoRow.SpecimenID = SavedWSOTRow.SpecimenID
                                    myOrderTestsLISInfoRow.TestType = SavedWSOTRow.TestType
                                    myOrderTestsLISInfoRow.TestID = SavedWSOTRow.TestID
                                    myOrderTestsLISInfoRow.SampleType = SavedWSOTRow.SampleType
                                    myOrderTestsLISInfoRow.ESPatientID = SavedWSOTRow.ESPatientID
                                    myOrderTestsLISInfoRow.ESOrderID = SavedWSOTRow.ESOrderID
                                    myOrderTestsLISInfoRow.CheckLISValues = True

                                    'Inform the SampleClass
                                    If (SavedWSOTRow.SampleClass = "PATIENT" AndAlso Not SavedWSOTRow.ExternalQC) Then
                                        myOrderTestsLISInfoRow.SampleClass = "patient"
                                    ElseIf (SavedWSOTRow.SampleClass = "PATIENT" AndAlso SavedWSOTRow.ExternalQC) Then
                                        myOrderTestsLISInfoRow.SampleClass = "QC"
                                    End If

                                    'Inform StatFlagText
                                    If (SavedWSOTRow.StatFlag) Then
                                        myOrderTestsLISInfoRow.StatFlagText = "stat"
                                    Else
                                        myOrderTestsLISInfoRow.StatFlagText = "normal"
                                    End If

                                    'Add row to DS. 
                                    pRejectedOTLISInfo.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myOrderTestsLISInfoRow)

                                    'Add a row to a LISImportErrorsDS informing Message Identifier, AwosID, and ErrorCode.
                                    myImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                                    myImportErrorsLogRow.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATE_SPECIMEN.ToString()
                                    myImportErrorsLogRow.LineText = SavedWSOTRow.SavedWSName & " | " & SavedWSOTRow.AwosID
                                    myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myImportErrorsLogRow)
                                End If
                            End If
                        Next

                        'Validate if an error has happened and if it is one of the known errors (LIS_DUPLICATE_REQUEST, LIS_NOT_ALLOWED_RERUN)
                        'before continuing with the process 
                        'BT #1556 - Error LIS_RERUN_WITH_DIF_SPECIMEN included in this list of controlled errors
                        If myGlobalDataTO.HasError AndAlso (myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATED_REQUEST.ToString() OrElse _
                                                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString() OrElse _
                                                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.LIS_RERUN_WITH_DIF_SPECIMEN.ToString()) Then
                            'Set the HasError to false because the error is a controlled one
                            myGlobalDataTO.HasError = False
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'If there are accepted Reruns, process them
                            If (myRepetitionsToAddDS.twksWSRepetitionsToAdd.Count > 0) Then
                                'Process for each Repetition to Add
                                For Each RepetitionRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow In myRepetitionsToAddDS.twksWSRepetitionsToAdd.Rows
                                    'AG 31/03/2014 - BT #1565 - Inform new parameter pRunningMode when calling the function
                                    Dim myLogAcciones As New ApplicationLogManager()
                                    myLogAcciones.CreateLogActivity("Launch Rerun !", "OrderTestsDelegate.ProcessLISPatientOTs", EventLogEntryType.Information, False)

                                    'Add the new Rerun to the active WorkSession
                                    'BT #1536 - Inform new optional parameter pLISDownloadProcess = TRUE when calling the ManageRepetitions function
                                    Dim rerunCreatedFlag As Boolean = False
                                    myGlobalDataTO = myRepetitionsDelegate.ManageRepetitions(dbConnection, pAnalyzerID, pWorkSessionID, RepetitionRow.OrderTestID, _
                                                                                             RepetitionRow.RerunNumber, rerunCreatedFlag, "", pRunningMode, True, "NONE", _
                                                                                             pISEModuleReady, True)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next
                            End If

                            If (myFinalOrderTestsLISInfoDS.twksOrderTestsLISInfo.Count > 0) Then
                                'Add LIS fields for all OrderTestID / RerunNumber in the DS
                                myGlobalDataTO = myOrderTestsLISInfoDelegate.Create(dbConnection, myFinalOrderTestsLISInfoDS, True)
                            End If

                            'Add to the active WorkSession all Controls, Calibrators and Blanks needed for the added STD Test/SampleTypes 
                            'requested for Patient Samples
                            If (Not myGlobalDataTO.HasError) Then
                                'AddAllOrderTestsNeededByLIS in OrderTestsDelegate
                                myGlobalDataTO = AddAllNeededOrderTestsForLIS(pAnalyzerID, pWorkSessionResultDS)
                            End If

                            If (Not myGlobalDataTO.HasError) Then
                                If (myImportErrorsLogDS.twksImportErrorsLog.Count > 0) Then
                                    'Write the content of DataSet LISImportErrorDS to table twksImportErrorsLog
                                    Dim myImportErrorLog As New ImportErrorsLogDelegate
                                    myGlobalDataTO = myImportErrorLog.Add(Nothing, myImportErrorsLogDS)
                                End If
                            End If
                        End If

                        'Set the function result value
                        If (Not myGlobalDataTO.HasError) Then
                            If (selectedPatients = 0 AndAlso notSelectedPatients = 0) Then
                                myGlobalDataTO.SetDatos = 0
                            ElseIf (selectedPatients = 0 AndAlso notSelectedPatients > 0) Then
                                myGlobalDataTO.SetDatos = 1
                            Else
                                myGlobalDataTO.SetDatos = 2
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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ProcessLISPatientOTs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search if the Order Test ID of a Calibrator is used as Alternative for the same Test and another
        ''' Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test used as Alternative</param>
        ''' <param name="pWorkSessionID">Work Session Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with the list of Order Tests
        '''          using the informed one as Alternative</returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        '''              SA 12/04/2012 - Added optional parameter pWorkSessionID to allow excluding OrderTests that are 
        '''                              already linked to the active WorkSession 
        ''' </remarks>
        Public Function ReadByAlternativeOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                     Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        resultData = myOrderTestsDAO.ReadByAlternativeOrderTestID(dbConnection, pOrderTestID, pWorkSessionID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ReadByAlternativeOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Order Tests with the specified Status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pStatus">Order Test Status to search</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderTestsDS with all Order Tests having the specified Status</returns>
        ''' <remarks>
        ''' Created by:  AG 18/07/2013
        ''' </remarks>
        Public Function ReadByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStatus As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TwksOrderTestsDAO
                        resultData = myDAO.ReadByStatus(dbConnection, pStatus)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ReadByStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Reorganizes OPEN Blank Order Tests, placing the selected ones first
        ''' </summary>
        ''' <param name="pWSResultDS">Type DataSet containing the list of Blank Order Tests to reorganize (re-sort)</param>
        ''' <remarks>
        ''' Created by:  SA 14/04/2010
        ''' Modified by: SA 01/09/2010 - In LINQ sentences, remove filter by field NewCheck, filter should be only by field Selected
        '''              SA 31/01/2012 - Set to Nothing all declared Lists
        ''' </remarks>
        Public Sub ReorganizeBlanks(ByVal pWSResultDS As WorkSessionResultDS)
            Try
                Dim minCreationOrder As Integer = 1

                'If there are rows in table of Blanks and Calibrators in the entry DataSet
                If (pWSResultDS.BlankCalibrators.Rows.Count > 0) Then
                    'Get the minimum value of Creation Order for OPEN Blank Order Tests
                    Dim lstWSOpenBlanks As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstWSOpenBlanks = (From a In pWSResultDS.BlankCalibrators _
                                      Where a.SampleClass = "BLANK" _
                                    AndAlso a.OTStatus = "OPEN" _
                                   Order By a.CreationOrder _
                                     Select a).ToList()

                    If (lstWSOpenBlanks.Count > 0) Then minCreationOrder = lstWSOpenBlanks(0).CreationOrder

                    'Get OPEN Blank Order Tests selected to be sent to the Analyzer (having or not a previous result)
                    lstWSOpenBlanks = (From a In pWSResultDS.BlankCalibrators _
                                      Where a.SampleClass = "BLANK" _
                                    AndAlso a.OTStatus = "OPEN" _
                                    AndAlso a.Selected = True _
                                   Order By a.CreationOrder Select a).ToList()

                    'Rewrite the CreationOrder... selected Blanks have to be first than unselected ones
                    For Each selectedBlank In lstWSOpenBlanks
                        selectedBlank.CreationOrder = minCreationOrder
                        minCreationOrder += 1
                    Next

                    'Finally, if there are unselected OPEN Order Tests they are also re-sorted
                    '(including Blank Order Tests having a previous result)
                    lstWSOpenBlanks = (From a In pWSResultDS.BlankCalibrators _
                                      Where a.SampleClass = "BLANK" _
                                    AndAlso a.OTStatus = "OPEN" _
                                    AndAlso a.Selected = False _
                                   Order By a.CreationOrder _
                                     Select a).ToList()

                    For Each selectedBlank In lstWSOpenBlanks
                        selectedBlank.CreationOrder = minCreationOrder
                        minCreationOrder += 1
                    Next
                    lstWSOpenBlanks = Nothing

                    'Confirm changes in the entry DataSet
                    pWSResultDS.BlankCalibrators.AcceptChanges()
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ReorganizeBlanks", EventLogEntryType.Error, False)
                'Throw ex  'Commented line RH 12/04/2012
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Reorganizes OPEN Calibrator Order Tests, placing the selected ones first, grouped by Calibrator
        ''' </summary>
        ''' <param name="pWSResultDS">Type DataSet containing the list of Calibrator Order Tests to reorganize (re-sort)</param>
        ''' <remarks>
        ''' Created by:  SA 14/04/2010
        ''' Modified by: SA 01/09/2010 - In LINQ sentences, remove filter by field NewCheck, filter should be only by field Selected
        '''              SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
        ''' </remarks>
        Public Sub ReorganizeCalibrators(ByVal pWSResultDS As WorkSessionResultDS)
            Try
                Dim nextCreationOrder As Integer = 1

                'Get Calibrator Order Tests sent and/or selected to be sent to the Analyzer
                Dim allCalibrators As List(Of Integer)
                allCalibrators = (From a In pWSResultDS.BlankCalibrators _
                                 Where a.SampleClass = "CALIB" _
                               AndAlso a.Selected = True _
                              Order By a.CreationOrder _
                                Select a.CalibratorID).Distinct().ToList

                'Get all selected Calibrator Order Tests using the same Calibrator to re-sorting them 
                '(including those selected but having previous results)
                Dim currentCalibID As Integer
                Dim lstSelectedCalib As List(Of WorkSessionResultDS.BlankCalibratorsRow)

                For Each calibratorID As Integer In allCalibrators
                    currentCalibID = calibratorID
                    lstSelectedCalib = (From a In pWSResultDS.BlankCalibrators _
                                       Where a.SampleClass = "CALIB" _
                                     AndAlso (Not a.IsCalibratorIDNull AndAlso a.CalibratorID = currentCalibID) _
                                     AndAlso a.Selected = True _
                                    Order By a.CreationOrder Select a).ToList

                    For Each selectedCalibRow As WorkSessionResultDS.BlankCalibratorsRow In lstSelectedCalib
                        selectedCalibRow.CreationOrder = nextCreationOrder
                        nextCreationOrder += 1
                    Next
                Next
                allCalibrators = Nothing
                lstSelectedCalib = Nothing

                'Finally, if there are unselected OPEN Order Tests they are also re-sorted (including those not selected
                'but having previous results)
                Dim lstWSOpenCalibs As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                lstWSOpenCalibs = (From a In pWSResultDS.BlankCalibrators _
                                  Where a.SampleClass = "CALIB" _
                                AndAlso a.OTStatus = "OPEN" _
                                AndAlso a.Selected = False _
                               Order By a.CreationOrder _
                                 Select a).ToList()

                For Each unselectedCalib In lstWSOpenCalibs
                    unselectedCalib.CreationOrder = nextCreationOrder
                    nextCreationOrder += 1
                Next
                lstWSOpenCalibs = Nothing

                'Confirm changes in the entry DataSet
                pWSResultDS.BlankCalibrators.AcceptChanges()
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ReorganizeCalibrators", EventLogEntryType.Error, False)
                'Throw ex  'Commented line RH 12/04/2012
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Reorganizes OPEN Control Order Tests, placing the selected ones first, grouped by Controls
        ''' </summary>
        ''' <param name="pWSResultDS">Type DataSet containing the list of Control Order Tests to reorganize (re-sort)</param>
        ''' <remarks>
        ''' Created by:  SA 14/04/2010
        ''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists
        ''' </remarks>
        Public Sub ReorganizeControls(ByVal pWSResultDS As WorkSessionResultDS)
            Try
                Dim nextCreationOrder As Integer = 1

                'Get Control Order Tests sent and/or selected to be sent to the Analyzer
                Dim allControls As List(Of Integer)
                allControls = (From a In pWSResultDS.Controls _
                               Where a.SampleClass = "CTRL" _
                             AndAlso a.Selected = True _
                            Order By a.CreationOrder _
                              Select a.ControlID).Distinct().ToList

                'Get all selected Control Order Tests using the same Control to re-sorting them 
                Dim lstSelectedCtrl As List(Of WorkSessionResultDS.ControlsRow)
                For Each controlID As Integer In allControls
                    lstSelectedCtrl = (From a In pWSResultDS.Controls _
                                      Where a.SampleClass = "CTRL" _
                                    AndAlso a.ControlID = controlID _
                                    AndAlso a.Selected = True _
                                   Order By a.CreationOrder _
                                     Select a).ToList

                    For Each selectedCtrlRow As WorkSessionResultDS.ControlsRow In lstSelectedCtrl
                        selectedCtrlRow.CreationOrder = nextCreationOrder
                        nextCreationOrder += 1
                    Next
                Next
                lstSelectedCtrl = Nothing
                allControls = Nothing

                'Finally, if there are unselected OPEN Order Tests they are also re-sorted
                Dim lstWSOpenCtrls As List(Of WorkSessionResultDS.ControlsRow)
                lstWSOpenCtrls = (From a In pWSResultDS.Controls _
                                 Where a.SampleClass = "CTRL" _
                               AndAlso a.OTStatus = "OPEN" _
                               AndAlso a.Selected = False _
                              Order By a.CreationOrder _
                                Select a).ToList()

                For Each unselectedCtrl In lstWSOpenCtrls
                    unselectedCtrl.CreationOrder = nextCreationOrder
                    nextCreationOrder += 1
                Next
                lstWSOpenCtrls = Nothing

                'Confirm changes in the entry DataSet
                pWSResultDS.Controls.AcceptChanges()
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ReorganizeControls", EventLogEntryType.Error, False)
                'Throw ex  'Commented line RH 12/04/2012
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Reorganizes OPEN Patient Order Tests, placing the selected ones first, grouped by Priority (Stat/Routine) and SampleID
        ''' </summary>
        ''' <param name="pWSResultDS">Type DataSet containing the list of Patient Order Tests to reorganize (re-sort)</param>
        ''' <remarks>
        ''' Created by:  SA 16/04/2010
        ''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
        ''' </remarks>
        Public Sub ReorganizePatients(ByVal pWSResultDS As WorkSessionResultDS)
            Try
                Dim i As Integer = 1
                Dim nextCreationOrder As Integer = 1

                Dim currentPriority As Boolean
                Dim allPatients As List(Of String)
                Dim lstSelectedPatients As List(Of WorkSessionResultDS.PatientsRow)

                Do While (i >= 0)
                    currentPriority = Convert.ToBoolean(IIf(i = 1, True, False))

                    'Get all Patient Order Tests with the current priority sent and/or selected to be sent to the Analyzer
                    allPatients = (From a In pWSResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.StatFlag = currentPriority _
                                AndAlso a.Selected = True _
                               Order By a.CreationOrder _
                                 Select a.SampleID).Distinct().ToList

                    'Get all selected Patient Order Tests with the current priority and having the same PatientID to re-sorting them 
                    For Each sampleID As String In allPatients
                        lstSelectedPatients = (From a In pWSResultDS.Patients _
                                              Where a.SampleClass = "PATIENT" _
                                            AndAlso a.SampleID = sampleID _
                                            AndAlso a.StatFlag = currentPriority _
                                            AndAlso a.Selected = True _
                                           Order By a.CreationOrder _
                                             Select a).ToList

                        For Each selectedPatientRow As WorkSessionResultDS.PatientsRow In lstSelectedPatients
                            selectedPatientRow.CreationOrder = nextCreationOrder
                            nextCreationOrder += 1
                        Next
                    Next
                    i -= 1
                Loop
                allPatients = Nothing
                lstSelectedPatients = Nothing

                'Finally, if there are unselected OPEN Order Tests they are also re-sorted 
                Dim lstOPENPatients As List(Of WorkSessionResultDS.PatientsRow)
                lstOPENPatients = (From a In pWSResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.OTStatus = "OPEN" _
                                AndAlso a.Selected = False _
                               Order By a.CreationOrder _
                                 Select a).ToList()

                For Each unselectedPatient In lstOPENPatients
                    unselectedPatient.CreationOrder = nextCreationOrder
                    nextCreationOrder += 1
                Next
                lstOPENPatients = Nothing

                'Confirm changes in the entry DataSet
                pWSResultDS.Patients.AcceptChanges()
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ReorganizePatients", EventLogEntryType.Error, False)
                'Throw ex  'Commented line RH 12/04/2012
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 21/04/2010
        ''' Modified by: SA  01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TwksOrderTestsDAO
                        resultData = myDAO.ResetWS(dbConnection)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the maximum OrderTestID currently stored in table twksOrderTests (if any) and restart the Identity
        ''' value for that column. New initial value for OrderTestID will be 1 if there are not records in the table.
        ''' or Max(OrderTestID)+1 if there are recoreds int it 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 07/09/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function RestartIdentity(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO
                        resultData = myOrderTestsDAO.RestartIdentity(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.RestartIdentity", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Patient or Control Order Test is selected to be positioned in an Analyzer Rotor, the needed Blank and Calibrator
        ''' are also selected (if they are unselected currently). When the selected Order Test is a Calibrator, then the needed Blank
        ''' is also selected (if it is unselected currently)
        ''' </summary>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pWSResultDS">WorkSessionResultDS containing the list of Patient Samples, Controls, Calibrators 
        '''                           and Blanks currently requested</param>
        ''' <remarks>
        ''' Created by:  SA 22/03/2010
        ''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists
        '''              SA 05/04/2013 - When searching the Blank, do not filter the LINQ by SampleType
        ''' </remarks>
        Public Sub SelectAllNeededOrderTests(ByVal pSampleClass As String, ByVal pTestType As String, ByVal pTestID As Integer, _
                                             ByVal pSampleType As String, ByVal pWSResultDS As WorkSessionResultDS)
            Try
                If (pTestType = "STD") Then
                    Dim lstWSResultDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)

                    If (pSampleClass <> "CALIB") Then
                        'For Patient Samples and Controls, check if the needed Calibrator is also selected
                        'For Calibrators having previous results, they are selected only if the New Check has been checked
                        lstWSResultDS = (From a In pWSResultDS.BlankCalibrators _
                                        Where a.SampleClass = "CALIB" _
                                      AndAlso a.TestType = pTestType _
                                      AndAlso a.TestID = pTestID _
                                      AndAlso (a.SampleType = pSampleType OrElse a.RequestedSampleTypes.Contains(pSampleType)) _
                                      AndAlso a.Selected = False _
                                      AndAlso a.NewCheck = True _
                                       Select a).ToList()

                        For Each blankCalib In lstWSResultDS
                            blankCalib.Selected = True
                        Next
                    End If

                    'For Patient Samples, Controls and Calibrators, check is the needed Blank is also selected 
                    'For Blanks having previous results, they are selected only if the New Check has been checked
                    lstWSResultDS = (From a In pWSResultDS.BlankCalibrators _
                                    Where a.SampleClass = "BLANK" _
                                  AndAlso a.TestType = pTestType _
                                  AndAlso a.TestID = pTestID _
                                  AndAlso (a.IsPreviousOrderTestIDNull OrElse a.PreviousOrderTestID.ToString = "") _
                                   Select a).ToList()

                    For Each blankCalib In lstWSResultDS
                        blankCalib.Selected = True
                    Next

                    lstWSResultDS = Nothing
                    pWSResultDS.BlankCalibrators.AcceptChanges()
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.SelectAllNeededOrderTests", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' When a Patient Order Test containing a Calculated Test is selected, all the component Tests (of whatever type)
        ''' are also selected. When a component Test of Standard type is selected, then the needed Blank and Calibrator are
        ''' also selected
        ''' </summary>
        ''' <param name="pSampleID">PatientID/SampleID of the selected Order Test</param>
        ''' <param name="pStatFlag">StatFlag of the selected Order Test</param>
        ''' <param name="pCalcTestID">Identifier of the selected Calculated Test</param>
        ''' <param name="pWSResultDS">WorkSessionResultDS containing the list of Patient Samples, Controls, Calibrators 
        '''                           and Blanks currently requested</param>
        ''' <remarks>
        ''' Created by:  SA 
        ''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
        ''' </remarks>
        Public Sub SelectCalcTestComponents(ByVal pSampleID As String, ByVal pStatFlag As Boolean, ByVal pCalcTestID As Integer, _
                                            ByVal pWSResultDS As WorkSessionResultDS)
            Try
                Dim lstWSPatients As List(Of WorkSessionResultDS.PatientsRow)
                lstWSPatients = (From b In pWSResultDS.Patients _
                                Where b.SampleID = pSampleID _
                              AndAlso b.StatFlag = pStatFlag _
                              AndAlso (Not b.IsCalcTestIDNull AndAlso b.CalcTestID <> "") _
                               Select b).ToList()

                Dim calcIDs() As String
                Dim isComponent As Boolean = False
                For Each testInFormula As WorkSessionResultDS.PatientsRow In lstWSPatients
                    calcIDs = testInFormula.CalcTestID.Split(CChar(", "))
                    For i As Integer = 0 To calcIDs.Length - 1
                        If (calcIDs(i) = pCalcTestID.ToString) Then
                            isComponent = True
                            Exit For
                        Else
                            isComponent = False
                        End If
                    Next

                    If (isComponent) Then
                        testInFormula.Selected = True
                        If (testInFormula.TestType = "STD") Then
                            SelectAllNeededOrderTests(testInFormula.SampleClass, testInFormula.TestType, testInFormula.TestID, testInFormula.SampleType, pWSResultDS)

                        ElseIf (testInFormula.TestType = "CALC") Then
                            SelectCalcTestComponents(testInFormula.SampleID, testInFormula.StatFlag, testInFormula.TestID, pWSResultDS)
                        End If
                    End If
                Next
                lstWSPatients = Nothing

                pWSResultDS.Patients.AcceptChanges()
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.SelectCalcTestComponents", EventLogEntryType.Error, False)
                'Throw ex  'Commented line RH 12/04/2012
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Rewrite value of CreationOrder for Patient Order Tests
        ''' </summary>
        ''' <param name="pWSResultDS">Type DataSet containing the list of Patient Order Tests to sort</param>
        ''' <param name="pSortingType">Optional parameter to indicate the type of sorting to be applied in the
        '''                            grid of Patients; possible values: 
        '''                            1 - Status / Priority / SampleID: sent Order Tests are placed first, sorted by priority (Stat Order Tests
        '''                                are placed first). Open Order Tests are also sorted by priority.  In both cases, Open Order Tests 
        '''                                inside each Priority are placed in the order they have been requested but grouped by SampleID and
        '''                                SampleType.  
        '''                            2 - Priority / Status: Stat Order Tests are placed in first place, sorted by Status (sent ones are placed
        '''                                first)
        '''                            3 - SampleID / Priority / Status: Order Tests are placed grouped by SampleID, and for each one sorted by
        '''                                Priority (Stat ones placed first) and Status (sent ones placed first)
        ''' </param>
        ''' <remarks>
        ''' Created by:  SA 26/03/2010
        ''' Modified by: SA 22/10/2010 - Added sorting of ISE Tests (following Standard Tests)
        '''              SA 02/12/2010 - Added sorting of OFF SYSTEM Tests (following Calculated Tests)
        '''              SA 31/01/2012 - Set to Nothing all declared Lists
        '''              XB 02/12/2014 - Adapt the sorting to ISE and OFFS included also inside CALC tests - BA-1867
        ''' </remarks>
        Public Sub SortPatients(ByVal pWSResultDS As WorkSessionResultDS, Optional ByVal pSortingType As Integer = 1)
            Try

                Dim nextCreationOrder As Integer = 1
                Dim lstSamplesDS As List(Of String)
                Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)

                Select Case (pSortingType)
                    Case 1  'Sort by Order Test Status and StatFlag 
                        'Get the maximum CreationOrder for Patient Order Tests that have been sent to the Analyzer
                        lstWSPatientsDS = (From b In pWSResultDS.Patients _
                                          Where b.SampleClass = "PATIENT" _
                                        AndAlso b.OTStatus <> "OPEN" _
                                       Order By b.CreationOrder Descending _
                                         Select b).ToList

                        If (lstWSPatientsDS.Count > 0) Then nextCreationOrder = lstWSPatientsDS.First.CreationOrder + 1

                        Dim statValue As Boolean
                        Dim mySampleID As String
                        Dim mySampleType As String
                        Dim lstSampleTypesDS As List(Of String)

                        Dim i As Integer = 1
                        Do While (i >= 0)
                            statValue = Convert.ToBoolean(IIf(i = 1, True, False))

                            'Get the list of different PatientID/SampleID for Open Patient Order Tests with the current priority
                            lstSamplesDS = (From a In pWSResultDS.Patients _
                                           Where a.SampleClass = "PATIENT" _
                                         AndAlso a.OTStatus = "OPEN" _
                                         AndAlso a.StatFlag = statValue _
                                        Order By a.CreationOrder _
                                          Select a.SampleID Distinct).ToList()

                            mySampleID = ""
                            mySampleType = ""
                            For Each sampleID As String In lstSamplesDS
                                mySampleID = sampleID

                                'Get the list of different SampleTypes for which the SampleID currently in process have requested Tests
                                lstSampleTypesDS = (From a In pWSResultDS.Patients _
                                                   Where a.SampleClass = "PATIENT" _
                                                 AndAlso a.OTStatus = "OPEN" _
                                                 AndAlso a.StatFlag = statValue _
                                                 AndAlso a.SampleID = mySampleID _
                                                Order By a.CreationOrder _
                                                  Select a.SampleType Distinct).ToList()

                                mySampleType = ""
                                For Each sampleType As String In lstSampleTypesDS
                                    'Get all Open Patient Order Tests with the current priority, corresponding to Standard Tests that are
                                    'not linked to Calculated Tests or to ISE Tests, and having the SampleID/SampleType 
                                    'currently in process
                                    mySampleType = sampleType
                                    lstWSPatientsDS = (From b In pWSResultDS.Patients _
                                                      Where b.SampleClass = "PATIENT" _
                                                    AndAlso b.OTStatus = "OPEN" _
                                                    AndAlso b.StatFlag = statValue _
                                                    AndAlso b.SampleID = mySampleID _
                                                    AndAlso b.SampleType = mySampleType _
                                                    AndAlso (b.TestType = "STD" OrElse b.TestType = "ISE") _
                                                    AndAlso b.IsCalcTestIDNull _
                                                   Order By b.CreationOrder _
                                                     Select b).ToList

                                    For Each patientOrderTest As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                                        'Update the CreationOrder
                                        patientOrderTest.CreationOrder = nextCreationOrder
                                        nextCreationOrder += 1
                                    Next

                                    'Get all Open Patient Order Tests with the current priority, corresponding to Partial Tests that are
                                    'linked to Calculated Tests and having the SampleID/SampleType currently in process
                                    lstWSPatientsDS = (From b In pWSResultDS.Patients _
                                                      Where b.SampleClass = "PATIENT" _
                                                    AndAlso b.OTStatus = "OPEN" _
                                                    AndAlso b.StatFlag = statValue _
                                                    AndAlso b.SampleID = mySampleID _
                                                    AndAlso b.SampleType = mySampleType _
                                                    AndAlso (b.TestType = "STD" OrElse b.TestType = "ISE" OrElse b.TestType = "OFFS") _
                                                    AndAlso Not b.IsCalcTestIDNull _
                                                   Order By b.CalcTestID, b.CreationOrder _
                                                     Select b).ToList

                                    ' XB 02/12/2014 - BA-1867
                                    'replace: AndAlso b.TestType = "STD"  by: AndAlso (b.TestType = "STD" OrElse b.TestType = "ISE" OrElse b.TestType = "OFFS")

                                    For Each patientOrderTest As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                                        'Update the CreationOrder
                                        patientOrderTest.CreationOrder = nextCreationOrder
                                        nextCreationOrder += 1
                                    Next

                                    '...get also all Open Patient Order Tests with the current priority, corresponding to Calculated Tests 
                                    'and having the SampleID/SampleType currently in process
                                    'mySampleType = sampleType
                                    lstWSPatientsDS = (From b In pWSResultDS.Patients _
                                                      Where b.SampleClass = "PATIENT" _
                                                    AndAlso b.OTStatus = "OPEN" _
                                                    AndAlso b.StatFlag = statValue _
                                                    AndAlso b.SampleID = mySampleID _
                                                    AndAlso b.SampleType = mySampleType _
                                                    AndAlso b.TestType = "CALC" _
                                                   Order By b.CreationOrder _
                                                     Select b).ToList

                                    For Each patientOrderTest As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                                        'Update the CreationOrder
                                        patientOrderTest.CreationOrder = nextCreationOrder
                                        nextCreationOrder += 1
                                    Next

                                    '...get also all Open Patient Order Tests with the current priority, corresponding to Off-System Tests not included inside any CALC test
                                    'and having the SampleID/SampleType currently in process
                                    lstWSPatientsDS = (From b In pWSResultDS.Patients _
                                                      Where b.SampleClass = "PATIENT" _
                                                    AndAlso b.OTStatus = "OPEN" _
                                                    AndAlso b.StatFlag = statValue _
                                                    AndAlso b.SampleID = mySampleID _
                                                    AndAlso b.SampleType = mySampleType _
                                                    AndAlso b.IsCalcTestIDNull _
                                                    AndAlso b.TestType = "OFFS" _
                                                   Order By b.CreationOrder _
                                                     Select b).ToList

                                    For Each patientOrderTest As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                                        'Update the CreationOrder
                                        patientOrderTest.CreationOrder = nextCreationOrder
                                        nextCreationOrder += 1
                                    Next
                                    ' XB 02/12/2014 - add condition "IsCalcTestIDNull" for the OFFS tests that are not included inside CALC test - BA-1867
                                Next
                            Next

                            'Decrement the loop variable to process Order Tests with low priority
                            i -= 1
                        Loop
                        lstSamplesDS = Nothing
                        lstWSPatientsDS = Nothing

                        pWSResultDS.Patients.AcceptChanges()

                    Case 2  'Sort by StatFlag and Order Test Status

                    Case 3 'Sort by PatientID/SampleID, StatFlag and Order Test Status 
                End Select
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.SortPatients", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Update information of the specified Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestsDS">Typed DataSet OrderTestsDS containing the list of Order Tests to update</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' Modified by: SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestDAO As New TwksOrderTestsDAO()
                        resultData = myOrderTestDAO.Update(dbConnection, pOrderTestsDS)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the LISRequest field by OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pLISStatusValue">Value to assign to LISStatus field</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 21/03/2013
        ''' </remarks>
        Public Function UpdateLISRequestByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                      ByVal pLISStatusValue As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestDAO As New TwksOrderTestsDAO()
                        resultData = myOrderTestDAO.UpdateLISRequestByOrderTestID(dbConnection, pOrderTestID, pLISStatusValue)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.UpdateLISRequestByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change the Status of the Order Tests included in the informed Work Session from the 
        ''' indicated current status to the specified new status. For instance:
        '''    ** When a Work Session is created, the status of all Order Tests included in it should
        '''       change from OPEN to PENDING
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pNewStatus">New Order Test Status</param>
        ''' <param name="pPreviousStatus">Current Status of the Order Tests to update</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pLoggedUser">Current logged User</param>
        ''' <returns>Global object containing sucess/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 27/01/2010 - Open a Transaction locally when the Connection is not informed
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewStatus As String, ByVal pPreviousStatus As String, _
                                     ByVal pWorkSessionID As String, ByVal pLoggedUser As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim orderTestData As New TwksOrderTestsDAO
                        dataToReturn = orderTestData.UpdateStatus(dbConnection, pNewStatus, pPreviousStatus, pWorkSessionID, pLoggedUser)

                        If (Not dataToReturn.HasError) Then
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

                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.UpdateStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update the Status of the specified Order Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test</param>
        ''' <param name="pNewStatusValue">Value of the new Order Test Status</param>
        ''' <param name="pOFFSOrderTest"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 03/03/2010 (OK)
        ''' Modified by: SA 14/06/2010 - Added optional parameters for WorkSession and Analyzer Identifiers.
        '''                              Added code to verify if the Order and the WorkSession in which the 
        '''                              specified Order Test is included can be closed
        '''              SG 15/06/2010 - Close Order and WorkSession when it is possible
        '''              AG 03/12/2010 - When the orderTestID new status is different than CLOSED then the OrderID new status is OPEN
        '''              SA 18/02/2011 - Some changes to reduce the number of queries sent to DB 
        '''              SA 01/09/2011 - Changed the function template
        '''              AG 22/05/2012 - when new executions are added, if the WSStatus is closed it becomes PENDING
        '''              TR 28/06/2013 - Add parameter POFFSOrderTest Validate if ordertest belong to and off system test to avoid the ToSendFlag and openOTFlag Filter.
        '''              AG 31/07/2014 - BA-1887 ==> Set OrderToExport = TRUE after 1 order test status is set to CLOSED
        '''              SA 19/09/2014 - BA-1927 ==> When calling function UpdateOrderToExport in OrdersDelegate, pass the local DB Connection instead 
        '''                                          of the received as parameter (to avoid timeouts)
        '''              AG 16/10/2014 BA-2011 - Update properly the OrderToExport field when the recalculated result is an accepted one
        ''' </remarks>
        Public Function UpdateStatusByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                                  ByVal pNewStatusValue As String, Optional pOFFSOrderTest As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim orderTestData As New TwksOrderTestsDAO

                        dataToReturn = orderTestData.UpdateStatusByOrderTestID(dbConnection, pOrderTestID, pNewStatusValue)
                        If (Not dataToReturn.HasError) Then
                            'Get the WorkSessionID, AnalyzerID and OrderID for the specified OrderTest Identifier
                            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

                            dataToReturn = myWSOrderTestsDelegate.GetWSAndAnalyzer(dbConnection, pOrderTestID)
                            If (Not dataToReturn.HasError And Not dataToReturn.SetDatos Is Nothing) Then
                                Dim myOrderTestWSAnalyzerDS As OrderTestWSAnalyzerDS = DirectCast(dataToReturn.SetDatos, OrderTestWSAnalyzerDS)

                                Dim myWorkSessionID As String = myOrderTestWSAnalyzerDS.twksOrderTestWSAnalyzer(0).WorkSessionID
                                Dim myAnalyzerID As String = myOrderTestWSAnalyzerDS.twksOrderTestWSAnalyzer(0).AnalyzerID
                                Dim myOrderID As String = myOrderTestWSAnalyzerDS.twksOrderTestWSAnalyzer(0).OrderID
                                Dim myOrderStatus As String = myOrderTestWSAnalyzerDS.twksOrderTestWSAnalyzer(0).OrderStatus
                                'AG 22/05/2012
                                Dim myWSStatus As String = ""
                                If Not myOrderTestWSAnalyzerDS.twksOrderTestWSAnalyzer(0).IsWSStatusNull Then myWSStatus = myOrderTestWSAnalyzerDS.twksOrderTestWSAnalyzer(0).WSStatus()
                                'AG 22/05/2012
                                If (pNewStatusValue = "CLOSED") Then
                                    'AG 16/10/2014 BA-2011 - Update properly the OrderToExport field after save new Calc test result (always accepted)
                                    Dim myOrdersDelegate As New OrdersDelegate
                                    dataToReturn = myOrdersDelegate.SetNewOrderToExportValue(dbConnection, , pOrderTestID)
                                    'AG 16/10/2014 BA-2011

                                    'Verify if all the Order Tests of the same Order are already CLOSED to set also to CLOSED the Status of the Order 

                                    dataToReturn = myWSOrderTestsDelegate.CountClosedOTsByOrder(dbConnection, myWorkSessionID, myAnalyzerID, myOrderID, pOFFSOrderTest)
                                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                        Dim numNotClosedByOrderID As Integer = CType(dataToReturn.SetDatos, Integer)
                                        If (numNotClosedByOrderID = 0) Then
                                            'Change Order Status to CLOSED
                                            dataToReturn = myOrdersDelegate.UpdateOrderStatus(dbConnection, myOrderID, "CLOSED")
                                            If (Not dataToReturn.HasError) Then
                                                'Verify if all the Order Tests included in the active WorkSession are already CLOSED  
                                                'to set also to CLOSED the Status of the WorkSession 
                                                dataToReturn = myWSOrderTestsDelegate.CountClosedOTsByWS(dbConnection, myWorkSessionID, myAnalyzerID, pOFFSOrderTest)
                                                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                                                    Dim numNotClosedByWS As Integer = CType(dataToReturn.SetDatos, Integer)
                                                    If (numNotClosedByWS = 0) Then
                                                        'Change the WorkSession Status to CLOSED
                                                        Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                                                        dataToReturn = myWSAnalyzersDelegate.UpdateWSStatus(dbConnection, myAnalyzerID, myWorkSessionID, "CLOSED")
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                Else
                                    'If the Order has a Status different of OPEN, then it is changed
                                    If (myOrderStatus <> "OPEN") Then
                                        Dim myOrdersDelegate As New OrdersDelegate
                                        dataToReturn = myOrdersDelegate.UpdateOrderStatus(dbConnection, myOrderID, "OPEN")
                                    End If

                                    'AG 22/05/2012 - If WS was CLOSED update to PENDING /else leave current value
                                    If myWSStatus = "CLOSED" Then
                                        Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                                        dataToReturn = myWSAnalyzersDelegate.UpdateWSStatus(dbConnection, myAnalyzerID, myWorkSessionID, "PENDING")
                                    End If
                                    'AG 22/05/2012
                                End If
                            End If

                        End If

                        If (Not dataToReturn.HasError) Then
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

                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.UpdateStatusByOrderTestId", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update the TubeType of all Order Tests of the specified SampleClass (Calibrators, Controls or
        ''' Patient Samples) that use the informed Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <param name="pNewTubeType">New Tube Type for the specified Element</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 19/03/2010
        ''' Modified by: SA 20/06/2011 - Implementation changed to include the new case of sample tubes for special solutions
        '''              SA 01/09/2011 - Changed the function template
        ''' </remarks>
        Public Function UpdateTubeTypeByRequiredElement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pElementID As Integer, _
                                                        ByVal pNewTubeType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReqElementDelegate As New WSRequiredElementsDelegate
                        resultData = myReqElementDelegate.GetRequiredElementData(dbConnection, pElementID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myReqElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            If (myReqElementsDS.twksWSRequiredElements.Rows.Count = 1) Then
                                Dim myOrderTestDAO As New TwksOrderTestsDAO()

                                Select Case (myReqElementsDS.twksWSRequiredElements(0).TubeContent)
                                    Case "CALIB"
                                        resultData = myOrderTestDAO.UpdateTubeTypeByCalibrator(dbConnection, pWorkSessionID, myReqElementsDS.twksWSRequiredElements(0).CalibratorID, _
                                                                                               pNewTubeType)
                                    Case "CTRL"
                                        resultData = myOrderTestDAO.UpdateTubeTypeByControl(dbConnection, pWorkSessionID, myReqElementsDS.twksWSRequiredElements(0).ControlID, _
                                                                                            pNewTubeType)
                                    Case "PATIENT"
                                        Dim myPatient As String = ""
                                        If (Not myReqElementsDS.twksWSRequiredElements(0).IsPatientIDNull) Then
                                            myPatient = myReqElementsDS.twksWSRequiredElements(0).PatientID
                                        ElseIf (Not myReqElementsDS.twksWSRequiredElements(0).IsSampleIDNull) Then
                                            myPatient = myReqElementsDS.twksWSRequiredElements(0).SampleID
                                        Else
                                            myPatient = myReqElementsDS.twksWSRequiredElements(0).OrderID
                                        End If
                                        resultData = myOrderTestDAO.UpdateTubeTypeByPatient(dbConnection, pWorkSessionID, myPatient, _
                                                                                            myReqElementsDS.twksWSRequiredElements(0).SampleType, pNewTubeType)
                                    Case "TUBE_SPEC_SOL"
                                        resultData = myOrderTestDAO.UpdateTubeTypeBySpecialSolution(dbConnection, pWorkSessionID, myReqElementsDS.twksWSRequiredElements(0).SolutionCode, _
                                                                                                    pNewTubeType)
                                End Select
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.UpdateTubeTypeByRequiredElement", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Change the Analyzer identifier of the informed WorkSession Order Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerIDNew">current connected Analyzer Identifier</param>
        ''' <param name="pAnalyzerIDOld">old connected Analyzer Identifier</param>
        ''' <param name="pOrderTestStatus">status of which is change</param>
        ''' <returns>GlobalDataTO containing Success/Error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 12/06/2012
        ''' </remarks>
        Public Function UpdateWSAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pAnalyzerIDNew As String, _
                                           ByVal pAnalyzerIDOld As String, _
                                           ByVal pOrderTestStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestsDAO As New TwksOrderTestsDAO

                        resultData = myOrderTestsDAO.UpdateWSAnalyzerID(dbConnection, pAnalyzerIDNew, pAnalyzerIDOld, pOrderTestStatus)

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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.UpdateWSAnalyzerID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Dilution Solutions needed for automatic dilutions programmed for Tests/SampleTypes requested for Patient Samples 
        ''' in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestsList">List of Order Tests included in a WorkSession</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with the list of Dilution Solutions needed for automatic dilutions 
        '''          programmed for Tests/SampleTypes requested for Patient Samples in the specified WorkSession</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2010
        ''' Modified by: SA 26/01/2011 - Add new parameter to specify the Diluent Solution, and pass it when calling
        '''                              the function in the DAO class
        '''              SA 01/09/2011 - Changed the function template
        '''              SA 28/05/2014 - BT #1519 ==> Changes to get all Dilution Solutions needed in the specified Work Session instead of 
        '''                                           verify for an specific Dilution Solution if it is needed for automatic predilutions of 
        '''                                           one or more Tests requested for Patient Samples in the Work Session:
        '''                                           * Parameter pDiluentSolution has been removed
        '''                                           * Return value has been changed from INTEGER to a typed DataSet TestSamplesDS containing the
        '''                                             list of Dilution Solutions needed in the Work Session
        ''' </remarks>
        Public Function VerifyAutomaticDilutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestsList As String, _
                                                 Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim orderTestsData As New TwksOrderTestsDAO
                        resultData = orderTestsData.VerifyAutomaticDilutions(dbConnection, pOrderTestsList, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.VerifyAutomaticDilutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Blank or Calibrator Order Test is unselected, verify if it is needed for a selected Calibrator Order Test
        ''' (only for Blanks), Control Order Test or Patient Order Test, case in which the unselection has to be cancelled.  
        ''' </summary>
        ''' <param name="pSampleClass">Sample Class of the unselected Order Test</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pWSResultDS">WorkSessionResultDS containing the list of Patient Samples, Controls, Calibrators 
        '''                           and Blanks currently requested</param>
        ''' <param name="pAlternativeSType">Optional parameter. It will be informed when the function is called to verify if 
        '''                                 a Calibrator can be unselected and the informed SampleType is not the one for which
        '''                                 the Calibrator is defined but one of the SampleType using it as alternative. And when 
        '''                                 it is informed, the validation of previous Calibrator results will be done for this
        '''                                 alternative SampleType</param>
        ''' <returns>True when the informed Order Test is needed for another selected Order Test (which means the unselection
        '''          has to be cancelled); otherwise it returns false</returns>
        ''' <remarks>
        ''' Created by:  SA 22/03/2010
        ''' Modified by: SA 08/06/2010 - For Blanks, when searching if it is needed for a requested Calibrator, Patient Sample or 
        '''                              Control, do not use the SampleType to compare
        '''              SA 23/05/2011 - For Blanks, if there is a previous result for it, then the unselect is allowed due to the 
        '''                              selected Patients, Controls and Calibrators can use the previous value
        '''                              For Calibrators, if there are previous results for it, then the unselect is allowed due to the 
        '''                              selected Patients and Controls can use the previous value
        '''              SA 31/01/2012 - Set to Nothing all declared Lists
        '''              SA 09/11/2012 - Added optional parameter pAlternativeSType. It is informed only when SampleClass is Calibrator
        '''                              and the informed SampleType is not the one for which the Calibrator is defined
        ''' </remarks>
        Public Function VerifyUnselectedOrderTest(ByVal pSampleClass As String, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                  ByVal pSampleType As String, ByVal pWSResultDS As WorkSessionResultDS, _
                                                  Optional ByVal pAlternativeSType As String = "") As Boolean
            Dim allowUnselect As Boolean = False

            Try
                If (pSampleClass = "BLANK") Then
                    'Verify if the Blank has a previous Result for it, then it can be unselected
                    Dim lstWSPreviousResultDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstWSPreviousResultDS = (From a In pWSResultDS.BlankCalibrators _
                                            Where a.SampleClass = pSampleClass _
                                          AndAlso a.TestType = pTestType _
                                          AndAlso a.TestID = pTestID _
                                          AndAlso Not a.IsPreviousOrderTestIDNull _
                                           Select a).ToList()

                    If (lstWSPreviousResultDS.Count = 1) Then
                        allowUnselect = True
                    Else
                        'Check if it is needed for a selected Calibrator Order Test
                        Dim lstWSResultDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                        lstWSResultDS = (From a In pWSResultDS.BlankCalibrators _
                                        Where a.SampleClass = "CALIB" _
                                      AndAlso a.TestType = pTestType _
                                      AndAlso a.TestID = pTestID _
                                      AndAlso a.Selected = True _
                                       Select a).ToList()

                        If (lstWSResultDS.Count = 0) Then
                            'There is not a selected Calibrator for the Test, check if there is 
                            '...a selected Patient Sample for the Test
                            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
                            lstWSPatientDS = (From a In pWSResultDS.Patients _
                                             Where a.SampleClass = "PATIENT" _
                                           AndAlso a.TestType = pTestType _
                                           AndAlso a.TestID = pTestID _
                                           AndAlso a.Selected = True _
                                            Select a).ToList()

                            If (lstWSPatientDS.Count = 0) Then
                                'There is not a selected Patient Sample for the Test, check if there is
                                '...a selected Control for the Test
                                Dim lstWSControlDS As List(Of WorkSessionResultDS.ControlsRow)
                                lstWSControlDS = (From a In pWSResultDS.Controls _
                                                 Where a.SampleClass = "CTRL" _
                                               AndAlso a.TestType = pTestType _
                                               AndAlso a.TestID = pTestID _
                                               AndAlso a.SampleType = pSampleType _
                                               AndAlso a.Selected = True _
                                                Select a).ToList()

                                allowUnselect = (lstWSControlDS.Count = 0)
                                lstWSControlDS = Nothing
                            End If
                            lstWSPatientDS = Nothing
                        End If
                        lstWSResultDS = Nothing
                    End If
                    lstWSPreviousResultDS = Nothing

                ElseIf (pSampleClass = "CALIB") Then
                    Dim noPreviousResults As Boolean = True
                    Dim realCalibSType As String = IIf(pAlternativeSType = String.Empty, pSampleType, pAlternativeSType).ToString

                    'Verify if the Calibrator has a previous Result for it, then it can be unselected
                    Dim lstWSPreviousResultDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstWSPreviousResultDS = (From a In pWSResultDS.BlankCalibrators _
                                            Where a.SampleClass = pSampleClass _
                                          AndAlso a.TestType = pTestType _
                                          AndAlso a.TestID = pTestID _
                                          AndAlso a.SampleType = realCalibSType _
                                          AndAlso Not a.IsPreviousOrderTestIDNull _
                                           Select a).ToList()
                    If (lstWSPreviousResultDS.Count = 1) Then
                        allowUnselect = True
                        noPreviousResults = False
                    End If

                    If (noPreviousResults) Then
                        'Check if it is needed for a selected Patient Order Test
                        Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
                        lstWSPatientDS = (From a In pWSResultDS.Patients _
                                         Where a.SampleClass = "PATIENT" _
                                       AndAlso a.TestType = pTestType _
                                       AndAlso a.TestID = pTestID _
                                       AndAlso a.SampleType = pSampleType _
                                       AndAlso a.Selected = True _
                                        Select a).ToList()

                        If (lstWSPatientDS.Count = 0) Then
                            'There is not a selected Patient Sample for the Test/SampleType, check if there is
                            '...a selected Control for the Test/SampleType
                            Dim lstWSControlDS As List(Of WorkSessionResultDS.ControlsRow)
                            lstWSControlDS = (From a In pWSResultDS.Controls _
                                             Where a.SampleClass = "CTRL" _
                                           AndAlso a.TestType = pTestType _
                                           AndAlso a.TestID = pTestID _
                                           AndAlso a.SampleType = pSampleType _
                                           AndAlso a.Selected = True _
                                            Select a).ToList()

                            allowUnselect = (lstWSControlDS.Count = 0)
                            lstWSControlDS = Nothing
                        End If
                        lstWSPatientDS = Nothing
                    End If
                    lstWSPreviousResultDS = Nothing
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.VerifyUnselectedOrderTest", EventLogEntryType.Error, False)
                Throw
            End Try
            Return allowUnselect
        End Function

        ''' <summary>
        ''' Verify the acceptance/rejection of reruns solicited by manual mode depending on current work session status 
        ''' </summary>
        ''' <param name="pWSPatientsRow">Row of the typed DataSet WorkSessionResultDS.Patients containing the Patient Order Test for which the Rerun acceptance 
        '''                              will be verified</param>
        ''' <param name="pSavedWSOrderTestsRow">Row of typed DataSet SavedWSOrderTestsDS containing data of the Patient Order Test for which LIS has requested a Rerun</param>
        ''' <param name="pRerunLISMode">Current value of setting for allowed Rerun LIS Mode: BOTH, ONLY ANALYZER or ONLY LIS</param>
        ''' <param name="pOrderTestsLISInfoDS">Typed DataSet OrderTestsLISInfoDS needed to add to table twksOrderTestLISInfo all Reruns requested by LIS that
        '''                                    have been accepted. If the Rerun requested by LIS is accepted, the OrderTestID/RerunNumber plus LIS fields are 
        '''                                    added to this DataSet and returned to be processed later</param>
        ''' <param name="pRepetitionsToAddDS">Typed DataSet WSRepetitionsToAddDS needed to manage all requested Rerun that are accepted. If the Rerun requested
        '''                                   by LIS is accepted, the OrderTestID is added to this DataSet and returned to be processed later</param>
        ''' <returns>GlobalDataTO containing information about if Patient Rerun is accepted ; otherwise it returns false (rerun is rejected)</returns>
        ''' <remarks>
        ''' Created by:  XB 21/03/2013
        ''' Modified by: SA 10/07/2013 - TestType verification has to be done when there are results for the OrderTestID and at least one of them has been
        '''                              already exported to LIS. If there are not results, or none of them has been exported, the LIS request is considered 
        '''                              the same, and LIS fields are updated for the last Rerun of the OrderTestID
        ''' </remarks>
        Public Function VerifyRerunOfManualPatientOT(ByRef pWSPatientsRow As WorkSessionResultDS.PatientsRow, 
                                                     ByVal pSavedWSOrderTestsRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, _
                                                     ByVal pRerunLISMode As String, ByRef pOrderTestsLISInfoDS As OrderTestsLISInfoDS, _
                                                     ByRef pRepetitionsToAddDS As WSRepetitionsToAddDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myRerunNumber As Integer
                Dim myResultsDelegate As New ResultsDelegate
                Dim myResultsDS As New ResultsDS
                Dim thereAreExportedResults As Boolean = False
                Dim myExportedResults As Integer
                Dim rerunAlreadyExists As Boolean = False
                Dim myReruns As Integer
                Dim myOrderTestIDToSearch As Integer
                Dim myExecutionsDelegate As New ExecutionsDelegate
                Dim myMaxRerunNumber As Integer
                Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate
                Dim myOrderTestLISInfoDS As OrderTestsLISInfoDS

                'Get all results for current OrderTestID and get them sorted by RerunNumber DESC
                resultData = myResultsDelegate.GetResultsByOrderTest(Nothing, pWSPatientsRow.OrderTestID)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                    If (myResultsDS.vwksResults.Rows.Count > 0) Then
                        'There at least a Result for the Order Test; verify if at least one of them have been exported to LIS
                        If (Not myResultsDS Is Nothing) Then
                            myExportedResults = (From a As ResultsDS.vwksResultsRow In myResultsDS.vwksResults _
                                                Where a.ExportStatus = "SENT" _
                                               Select a).Count
                        End If
                        thereAreExportedResults = (myExportedResults > 0)

                        If (thereAreExportedResults) Then
                            'The new request is considered a Rerun, but only if it is allowed
                            If (pWSPatientsRow.TestType = "CALC" OrElse pWSPatientsRow.TestType = "OFFS") Then
                                'Reruns for Calculated and OffSystem Tests are not allowed
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString

                            ElseIf (pRerunLISMode = "ANALYZER") Then
                                'Reruns requested by LIS are not allowed
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                            Else
                                'Verify that there is not a Rerun in process before accept a new one
                                resultData = myExecutionsDelegate.GetMaxRerunNumber(Nothing, pWSPatientsRow.OrderTestID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myMaxRerunNumber = CType(resultData.SetDatos, Integer)

                                    If (myMaxRerunNumber = myResultsDS.vwksResults.Item(0).RerunNumber) Then
                                        myRerunNumber = myResultsDS.vwksResults.Item(0).RerunNumber + 1

                                        myOrderTestIDToSearch = pWSPatientsRow.OrderTestID
                                        If (Not pRepetitionsToAddDS Is Nothing) Then
                                            myReruns = (From a As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow In pRepetitionsToAddDS.twksWSRepetitionsToAdd _
                                                       Where a.OrderTestID = myOrderTestIDToSearch _
                                                      Select a).Count
                                        End If
                                        rerunAlreadyExists = (myReruns > 0)

                                        If (rerunAlreadyExists) Then
                                            resultData.HasError = True
                                            resultData.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATED_REQUEST.ToString
                                        Else
                                            'Create a new Repetition To Add
                                            If (pRepetitionsToAddDS Is Nothing) Then pRepetitionsToAddDS = New WSRepetitionsToAddDS
                                            Dim myRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow = pRepetitionsToAddDS.twksWSRepetitionsToAdd.NewtwksWSRepetitionsToAddRow

                                            With myRow
                                                .BeginEdit()
                                                .OrderTestID = pWSPatientsRow.OrderTestID
                                                .PostDilutionType = "NONE"
                                                .RerunNumber = myRerunNumber
                                                .EndEdit()
                                            End With
                                            pRepetitionsToAddDS.twksWSRepetitionsToAdd.AddtwksWSRepetitionsToAddRow(myRow)
                                            pRepetitionsToAddDS.AcceptChanges()

                                            AcceptRerun(pWSPatientsRow, pSavedWSOrderTestsRow, pOrderTestsLISInfoDS, myRerunNumber)
                                        End If
                                    Else
                                        'Verify if the Rerun in process was requested by LIS
                                        resultData = myOrderTestsLISInfoDelegate.GetByOrderTestID(Nothing, pWSPatientsRow.OrderTestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myOrderTestLISInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                                            If (myOrderTestLISInfoDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                                If (myMaxRerunNumber = myOrderTestLISInfoDS.twksOrderTestsLISInfo.Item(0).RerunNumber) Then
                                                    resultData.HasError = True
                                                    resultData.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATED_REQUEST.ToString
                                                Else
                                                    'Create the new row LIS data information
                                                    If (pOrderTestsLISInfoDS Is Nothing) Then pOrderTestsLISInfoDS = New OrderTestsLISInfoDS
                                                    Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = pOrderTestsLISInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow

                                                    With myRow
                                                        .BeginEdit()
                                                        .OrderTestID = pWSPatientsRow.OrderTestID
                                                        .RerunNumber = myMaxRerunNumber

                                                        If (Not pSavedWSOrderTestsRow.IsAwosIDNull) Then .AwosID = pSavedWSOrderTestsRow.AwosID
                                                        If (Not pSavedWSOrderTestsRow.IsSpecimenIDNull) Then .SpecimenID = pSavedWSOrderTestsRow.SpecimenID
                                                        If (Not pSavedWSOrderTestsRow.IsESOrderIDNull) Then .ESOrderID = pSavedWSOrderTestsRow.ESOrderID
                                                        If (Not pSavedWSOrderTestsRow.IsLISOrderIDNull) Then .LISOrderID = pSavedWSOrderTestsRow.LISOrderID
                                                        If (Not pSavedWSOrderTestsRow.IsESPatientIDNull) Then .ESPatientID = pSavedWSOrderTestsRow.ESPatientID
                                                        If (Not pSavedWSOrderTestsRow.IsLISPatientIDNull) Then .LISPatientID = pSavedWSOrderTestsRow.LISPatientID
                                                        .EndEdit()
                                                    End With
                                                    pOrderTestsLISInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                                                    pOrderTestsLISInfoDS.AcceptChanges()
                                                End If
                                            End If
                                        End If
                                    End If
                                End If

                            End If
                        Else
                            'The new request is considered the same; 
                            'LIS fields are linked to the last Rerun requested for the  existing OrderTest
                            myRerunNumber = myResultsDS.vwksResults.Item(0).RerunNumber
                            AcceptRerun(pWSPatientsRow, pSavedWSOrderTestsRow, pOrderTestsLISInfoDS, myRerunNumber)
                        End If
                    Else
                        'There are not Results for the Order Test; the new request is considered the same and LIS fields are linked to the existing OrderTest
                        myRerunNumber = 1
                        AcceptRerun(pWSPatientsRow, pSavedWSOrderTestsRow, pOrderTestsLISInfoDS, myRerunNumber)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.VerifyRerunOfManualPatientOT", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify the acceptance/rejection of reruns solicited by LIS depending on current work session status 
        ''' </summary>
        ''' <param name="pWSPatientsRow">Row of the typed DataSet WorkSessionResultDS.Patients containing the Patient Order Test for which the Rerun acceptance 
        '''                              will be verified</param>
        ''' <param name="pSavedWSOrderTestsRow">Row of typed DataSet SavedWSOrderTestsDS containing data of the Patient Order Test for which LIS has requested a Rerun</param>
        ''' <param name="pRerunLISMode">Current value of setting for allowed Rerun LIS Mode: BOTH, ONLY ANALYZER or ONLY LIS</param>
        ''' <param name="pOrderTestsLISInfoDS">Typed DataSet OrderTestsLISInfoDS needed to add to table twksOrderTestLISInfo all Reruns requested by LIS that
        '''                                    have been accepted. If the Rerun requested by LIS is accepted, the OrderTestID/RerunNumber plus LIS fields are 
        '''                                    added to this DataSet and returned to be processed later</param>
        ''' <param name="pRepetitionsToAddDS">Typed DataSet WSRepetitionsToAddDS needed to manage all requested Rerun that are accepted. If the Rerun requested
        '''                                   by LIS is accepted, the OrderTestID is added to this DataSet and returned to be processed later</param>
        ''' <param name="pMsgDateTime">Sending Date and Time of the LIS Message from which the Patient Order Test to process was extracted</param>   
        ''' <returns>GlobalDataTO containing information about if Patient Rerun is accepted ; otherwise it returns false (rerun is rejected)</returns>
        ''' <remarks>
        ''' Created by:  XB 21/03/2013
        ''' Modified by: SA 14/05/2013 - If the Patient Order Test for which the Rerun request is verified has not been still added to the active WS
        '''                              (it has been added in a previously processed LIS Saved WS and its OrderTestID is NULL), the Rerun is not 
        '''                              allowed; it is treated as duplicated request (even when the SpecimenID is different)
        '''              SA 17/03/2014 - BT #1536 ==> Removed the Throw sentence from the Catch Exceptions block: if an error happens, it has to be returned
        '''                                           inside the GlobalDataTO 
        '''              SA 01/04/2014 - BT #1564 ==> Before accepting the Order Test as a Patient Rerun request, confirm the Message was sent after the 
        '''                                           export of the existing Result. The Rerun request is NOT accepted if MsgDateTime is previous than 
        '''                                           the ExportDateTime. Additionally, solved a previous error found while testing this change: when LIS 
        '''                                           requests de Test for the first time but automatic or manual rerun were requested, the accepted and exported
        '''                                           Result can be from one of the requested Reruns (remember that only one can be accepted)   
        ''' </remarks>
        Public Function VerifyRerunOfLISPatientOT(ByRef pWSPatientsRow As WorkSessionResultDS.PatientsRow, ByVal pSavedWSOrderTestsRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, _
                                                  ByVal pRerunLISMode As String, ByRef pOrderTestsLISInfoDS As OrderTestsLISInfoDS, ByRef pRepetitionsToAddDS As WSRepetitionsToAddDS, _
                                                  ByVal pMsgDateTime As Date) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                Dim myResultsDelegate As New ResultsDelegate
                Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate
                Dim myResultsDS As New ResultsDS
                Dim myOrderTestLISInfoDS As OrderTestsLISInfoDS
                Dim myExecutionsDelegate As New ExecutionsDelegate
                Dim myMaxRerunNumber As Integer
                Dim myRerunNumber As Integer

                If (pRerunLISMode = "ANALYZER") Then
                    ' Reruns requested by LIS are not allowed
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                Else
                    If (pWSPatientsRow.TestType = "CALC" OrElse pWSPatientsRow.TestType = "OFFS") Then
                        'Reruns for Calculated and OffSystem Tests are not allowed
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                    Else
                        If (pWSPatientsRow.IsOrderTestIDNull) Then
                            'A Rerun of an OrderTest still not added to the active WS is not allowed; it is a duplicated request
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATED_REQUEST.ToString
                        Else
                            'Get all results for the OrderTest sorted by RerunNumber DESC
                            resultData = myResultsDelegate.GetResultsByOrderTest(Nothing, pWSPatientsRow.OrderTestID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                                If (myResultsDS.vwksResults.Rows.Count = 0) Then
                                    'If there are not results for the Order Test, a rerun request is  not allowed
                                    resultData.HasError = True
                                    resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                                Else
                                    'Get the list of AwosID that has been received from LIS for the existing Order Test; sort them by RerunNumber DESC
                                    resultData = myOrderTestsLISInfoDelegate.GetByOrderTestID(Nothing, pWSPatientsRow.OrderTestID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myOrderTestLISInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                                        If (myOrderTestLISInfoDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                            If (myOrderTestLISInfoDS.twksOrderTestsLISInfo.Item(0).SpecimenID <> pSavedWSOrderTestsRow.SpecimenID) Then
                                                'If the Rerun is requested in another tube (the barcode is different than the barcode of the last Rerun requested 
                                                'by LIS), the rerun is not allowed
                                                resultData.HasError = True
                                                resultData.ErrorCode = GlobalEnumerates.Messages.LIS_RERUN_WITH_DIF_SPECIMEN.ToString
                                            Else
                                                Dim lstResults As New List(Of ResultsDS.vwksResultsRow)
                                                If (Not myResultsDS Is Nothing) Then
                                                    'BT #1564 - Verify if there is an accepted Result between all Reruns that are equal or bigger than the one requested by LIS
                                                    '           (if there is a Result for a bigger RerunNumber, it means an automatic or manual Rerun was requested) 
                                                    lstResults = (From a As ResultsDS.vwksResultsRow In myResultsDS.vwksResults _
                                                                 Where a.RerunNumber >= myOrderTestLISInfoDS.twksOrderTestsLISInfo.Item(0).RerunNumber _
                                                               AndAlso a.ValidationStatus = "OK" _
                                                               AndAlso a.AcceptedResultFlag = True _
                                                                  Select a).ToList
                                                End If

                                                If (lstResults.Count > 0) Then
                                                    If (lstResults(0).ExportStatus <> "SENT") Then
                                                        'If the result for the last Rerun requested by LIS has not been exported yet,a new rerun request is not allowed
                                                        resultData.HasError = True
                                                        resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                                                    Else
                                                        'BT #1564 - Before accepting the Order Test as a Patient Rerun request, confirm the Message was sent after the 
                                                        '           export of the existing Result. The Rerun request is NOT accepted if MsgDateTime is previous than 
                                                        '           the ExportDateTime - NOTE: field ExportDateTime should NOT be NULL when ExportStatus is SENT, but 
                                                        '           sometimes it is due to the Export to LIS process does not update the field correctly. If this 
                                                        '           field is NULL, changes for #1564 cannot be applied and "fake" repetitions will be generated 
                                                        If (Not lstResults(0).IsExportDateTimeNull AndAlso pMsgDateTime <= lstResults(0).ExportDateTime) Then
                                                            'If the Message DateTime is previous to the Export DateTime, it is not a LIS RERUN request, it is an old
                                                            'repeated Message pending to process and it has to be ignored
                                                            resultData.HasError = True
                                                            resultData.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString
                                                        Else
                                                            'LIS Rerun accepted...
                                                            resultData = myExecutionsDelegate.GetMaxRerunNumber(Nothing, pWSPatientsRow.OrderTestID)
                                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                myMaxRerunNumber = CType(resultData.SetDatos, Integer)

                                                                'BT #1564 - Compare the last executed (or in execution) Rerun Number against the last Rerun Number having a 
                                                                '           saved Result for the Order Test
                                                                If (myMaxRerunNumber > myResultsDS.vwksResults(0).RerunNumber) Then
                                                                    'The last Rerun was a manual or automatic Rerun
                                                                    'The new Rerun requested by LIS is considered the same as the existing one and LIS fields are linked to it
                                                                    myRerunNumber = myMaxRerunNumber
                                                                Else
                                                                    'The last Rerun was requested by LIS; a new Rerun requested by LIS Is added
                                                                    myRerunNumber = myMaxRerunNumber + 1

                                                                    'Create a new Repetition To Add
                                                                    If (pRepetitionsToAddDS Is Nothing) Then pRepetitionsToAddDS = New WSRepetitionsToAddDS
                                                                    Dim myRepetitionRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow = pRepetitionsToAddDS.twksWSRepetitionsToAdd.NewtwksWSRepetitionsToAddRow

                                                                    With (myRepetitionRow)
                                                                        .BeginEdit()
                                                                        .OrderTestID = pWSPatientsRow.OrderTestID
                                                                        .PostDilutionType = "NONE"
                                                                        .RerunNumber = myRerunNumber
                                                                        .EndEdit()
                                                                    End With
                                                                    pRepetitionsToAddDS.twksWSRepetitionsToAdd.AddtwksWSRepetitionsToAddRow(myRepetitionRow)
                                                                    pRepetitionsToAddDS.AcceptChanges()
                                                                End If

                                                                'Create the new row LIS data information
                                                                If (pOrderTestsLISInfoDS Is Nothing) Then pOrderTestsLISInfoDS = New OrderTestsLISInfoDS
                                                                Dim myLISInfoRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = pOrderTestsLISInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow

                                                                With myLISInfoRow
                                                                    .BeginEdit()
                                                                    .OrderTestID = pWSPatientsRow.OrderTestID
                                                                    .RerunNumber = myRerunNumber

                                                                    If (Not pSavedWSOrderTestsRow.IsAwosIDNull) Then .AwosID = pSavedWSOrderTestsRow.AwosID
                                                                    If (Not pSavedWSOrderTestsRow.IsSpecimenIDNull) Then .SpecimenID = pSavedWSOrderTestsRow.SpecimenID
                                                                    If (Not pSavedWSOrderTestsRow.IsESOrderIDNull) Then .ESOrderID = pSavedWSOrderTestsRow.ESOrderID
                                                                    If (Not pSavedWSOrderTestsRow.IsLISOrderIDNull) Then .LISOrderID = pSavedWSOrderTestsRow.LISOrderID
                                                                    If (Not pSavedWSOrderTestsRow.IsESPatientIDNull) Then .ESPatientID = pSavedWSOrderTestsRow.ESPatientID
                                                                    If (Not pSavedWSOrderTestsRow.IsLISPatientIDNull) Then .LISPatientID = pSavedWSOrderTestsRow.LISPatientID
                                                                    .EndEdit()
                                                                End With
                                                                pOrderTestsLISInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myLISInfoRow)
                                                                pOrderTestsLISInfoDS.AcceptChanges()
                                                            End If
                                                        End If
                                                    End If
                                                Else
                                                    'Verify if the previous Rerun had been cancelled by LIS
                                                    resultData = myExecutionsDelegate.VerifyLockedByLIS(Nothing, pWSPatientsRow.OrderTestID, myOrderTestLISInfoDS.twksOrderTestsLISInfo.Item(0).RerunNumber)
                                                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                                        Dim isLockedByLIS As Boolean = CType(resultData.SetDatos, Boolean)

                                                        If (isLockedByLIS) Then
                                                            'Unlock LIS Executions
                                                            resultData = myExecutionsDelegate.UnlockLISExecutions(Nothing, pWSPatientsRow.OrderTestID, myOrderTestLISInfoDS.twksOrderTestsLISInfo.Item(0).RerunNumber)
                                                            If (Not resultData.HasError) Then
                                                                'Update myOrderTestLISInfoDS with the corresponding information from LIS
                                                                With myOrderTestLISInfoDS.twksOrderTestsLISInfo.Item(0)
                                                                    .BeginEdit()
                                                                    If (Not pSavedWSOrderTestsRow.IsAwosIDNull) Then .AwosID = pSavedWSOrderTestsRow.AwosID
                                                                    If (Not pSavedWSOrderTestsRow.IsESOrderIDNull) Then .ESOrderID = pSavedWSOrderTestsRow.ESOrderID
                                                                    If (Not pSavedWSOrderTestsRow.IsLISOrderIDNull) Then .LISOrderID = pSavedWSOrderTestsRow.LISOrderID
                                                                    If (Not pSavedWSOrderTestsRow.IsESPatientIDNull) Then .ESPatientID = pSavedWSOrderTestsRow.ESPatientID
                                                                    If (Not pSavedWSOrderTestsRow.IsLISPatientIDNull) Then .LISPatientID = pSavedWSOrderTestsRow.LISPatientID
                                                                    .EndEdit()
                                                                End With
                                                                myOrderTestLISInfoDS.AcceptChanges()

                                                                'And update it into DB
                                                                resultData = myOrderTestsLISInfoDelegate.Update(Nothing, myOrderTestLISInfoDS)
                                                            End If
                                                        Else
                                                            'If there is not a result for the last Rerun requested by LIS and the last Rerun was not cancelled by LIS, a new rerun request is not allowed
                                                            resultData.HasError = True
                                                            resultData.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATED_REQUEST.ToString
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.VerifyRerunOfLISPatientOT", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Private Methods"
		''' <summary>
		''' Solicited Rerun are accepted
		''' </summary>
		''' <param name="pWSPatientsRow">each row of Patient</param>
		''' <param name="pSavedWSOrderTestsRow">Every saved WS row</param>
		''' <param name="pOrderTestsLISInfoDS">LIS Data Information</param>
		''' <param name="pRerunNumber">Rerun number</param>
		''' <remarks>
		''' Created by:  XB 21/03/2013
		''' Modified by: SA 03/04/2013 - When assign value to field SampleIDType, use MAN instead of MANUAL
		'''              SA 30/04/2013 - Field PatientIDType in SavedWSOrderTestsRow contains MAN or DB; assign its value to field SampleIDType in PatientsRow
		''' </remarks>
		Private Sub AcceptRerun(ByRef pWSPatientsRow As WorkSessionResultDS.PatientsRow, ByVal pSavedWSOrderTestsRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, _
								ByRef pOrderTestsLISInfoDS As OrderTestsLISInfoDS, ByVal pRerunNumber As Integer)
			Try
				pWSPatientsRow.SampleIDType = pSavedWSOrderTestsRow.PatientIDType
				If (Not pSavedWSOrderTestsRow.IsSampleIDNull) Then pWSPatientsRow.SampleID = pSavedWSOrderTestsRow.SampleID

				pWSPatientsRow.LISRequest = True
				If (Not pSavedWSOrderTestsRow.IsExternalQCNull) Then
					pWSPatientsRow.ExternalQC = pSavedWSOrderTestsRow.ExternalQC
				Else
					pWSPatientsRow.ExternalQC = False
				End If

				If (Not pSavedWSOrderTestsRow.IsCalcTestIDsNull) Then
					'Merge values for CalcTestID  and CalcTestName 
					Dim calcIDs() As String
					Dim calcAuxIDs() As String
					Dim calcNames() As String
					Dim calcAuxNames() As String

					calcIDs = pWSPatientsRow.CalcTestID.Split(CChar(","))
					calcNames = pWSPatientsRow.CalcTestName.Split(CChar(","))

					'Check if list is empty
					Dim emptyList As Boolean = True
					For i As Integer = 0 To calcIDs.Length - 1
						calcIDs(i) = Trim(calcIDs(i))
						If (calcIDs(i).Length > 0) Then
							emptyList = False
						End If
					Next
					For i As Integer = 0 To calcNames.Length - 1
						calcNames(i) = Trim(calcNames(i))
					Next

					calcAuxIDs = pSavedWSOrderTestsRow.CalcTestIDs.Split(CChar(","))
					For i As Integer = 0 To calcAuxIDs.Length - 1
						If (Not calcIDs.Contains(Trim(calcAuxIDs(i)))) Then
							If (emptyList AndAlso i = 0) Then
								pWSPatientsRow.CalcTestID &= Trim(calcAuxIDs(i))
							Else
								pWSPatientsRow.CalcTestID &= "," & Trim(calcAuxIDs(i))
							End If
						End If
					Next

					calcAuxNames = pSavedWSOrderTestsRow.CalcTestNames.Split(CChar(","))
					For i As Integer = 0 To calcAuxNames.Length - 1
						If (Not calcNames.Contains(Trim(calcAuxNames(i)))) Then
							If (emptyList AndAlso i = 0) Then
								pWSPatientsRow.CalcTestName &= Trim(calcAuxNames(i))
							Else
								pWSPatientsRow.CalcTestName &= "," & Trim(calcAuxNames(i))
							End If
						End If
					Next
				End If

				'Create the new row LIS data information
				If (pOrderTestsLISInfoDS Is Nothing) Then pOrderTestsLISInfoDS = New OrderTestsLISInfoDS

				Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = pOrderTestsLISInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
				With myRow
					.BeginEdit()
					.OrderTestID = pWSPatientsRow.OrderTestID
					.RerunNumber = pRerunNumber
					If (Not pSavedWSOrderTestsRow.IsAwosIDNull) Then .AwosID = pSavedWSOrderTestsRow.AwosID
					If (Not pSavedWSOrderTestsRow.IsSpecimenIDNull) Then .SpecimenID = pSavedWSOrderTestsRow.SpecimenID
					If (Not pSavedWSOrderTestsRow.IsESOrderIDNull) Then .ESOrderID = pSavedWSOrderTestsRow.ESOrderID
					If (Not pSavedWSOrderTestsRow.IsLISOrderIDNull) Then .LISOrderID = pSavedWSOrderTestsRow.LISOrderID
					If (Not pSavedWSOrderTestsRow.IsESPatientIDNull) Then .ESPatientID = pSavedWSOrderTestsRow.ESPatientID
					If (Not pSavedWSOrderTestsRow.IsLISPatientIDNull) Then .LISPatientID = pSavedWSOrderTestsRow.LISPatientID
					.EndEdit()
				End With
				pOrderTestsLISInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
				pOrderTestsLISInfoDS.AcceptChanges()

			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.AcceptRerun", EventLogEntryType.Error, False)
				Throw
			End Try
		End Sub

		''' <summary>
		''' When an Order Test that corresponds to a Calculated Test is deleted, remove the link that exists between other
		''' Tests and the Calculated one:
		'''   ** If the selected Calculated Test is included in another Calculated Test, it is also deleted
		'''   ** The selected Calculated Test is removed from the correspondent fields for all the Tests included in its formula
		''' and delete the correspondent row in the entry DataSet
		''' </summary>
		''' <param name="pSampleID">Patient / Sample Identifier</param>
		''' <param name="pStatFlag">Value of Stat</param>
		''' <param name="pCalcTestID">Identifier of the Calculated Test to delete</param>
		''' <param name="pWorkSessionResultDS">Typed DataSet WorkSessionResultDS containing all requested Patient Samples</param>
		''' <remarks>
		''' Created by:  SA 13/05/2010
		''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops
		'''              RH 12/04/2012 - If the collection is empty then do nothing. Update method name in CreateLogActivity()
		'''              SA 13/04/2012 - After get the list of Calculated Tests in which the Test in process is included, use TRIM to compare
		'''                              calcIDs(i) with parameter pCalcTestID (the split adds empty spaces on the rigth)
		''' </remarks>
		Private Sub DeleteCalculatedTest(ByVal pSampleID As String, ByVal pStatFlag As Boolean, ByVal pCalcTestID As Integer, _
										 ByVal pWorkSessionResultDS As WorkSessionResultDS)
			Try
				Dim calcIDs() As String
				Dim calcNames() As String

				'Verify if the specified Calculated Test is included in the formula of another requested Calculated Test
				Dim lstCalcParents As List(Of WorkSessionResultDS.PatientsRow)
				lstCalcParents = (From a In pWorkSessionResultDS.Patients _
								 Where a.SampleID = pSampleID _
								   And a.StatFlag = pStatFlag _
								   And a.TestType = "CALC" _
								   And a.TestID = pCalcTestID _
								Select a).ToList

				'RH 12/04/2012 If the collection is empty then do nothing
				'SA 13/04/2012 This case should not happen, it was due to the error when comparing "If (calcIDs(i).Trim <> pCalcTestID.ToString) Then"
				'              without using the TRIM to remove empty spaces
				'If (lstCalcParents.Count = 0) Then Return


				If (lstCalcParents.Count > 0) AndAlso (lstCalcParents.First.IsCalcTestIDNull) Then 'dl 23/05/2012
					'If (lstCalcParents.First.IsCalcTestIDNull) Then
					'The Calculated Test is not included in another requested Calculated Test...nothing to do                   
					'Else
				ElseIf (lstCalcParents.Count > 0) AndAlso (Not lstCalcParents.First.IsCalcTestIDNull) Then 'dl 23/05/2012
					'The Calculated Test is included in another requested Calculated Tests... execute the deletion of each one of them
					calcIDs = lstCalcParents.First.CalcTestID.Split(CChar(","))
					For i As Integer = 0 To calcIDs(i).Length - 1
						DeleteCalculatedTest(pSampleID, pStatFlag, Convert.ToInt32(calcIDs(i).Trim), pWorkSessionResultDS)
					Next
				End If
				lstCalcParents = Nothing

				'Search all Tests included in the Formula to remove the link with the Calculated Test
				Dim lstCalcChilds As List(Of WorkSessionResultDS.PatientsRow)
				lstCalcChilds = (From a In pWorkSessionResultDS.Patients _
								Where a.SampleID = pSampleID _
								  And a.StatFlag = pStatFlag _
								  And (Not a.IsCalcTestIDNull) _
							   Select a).ToList

				Dim finalListIDs As String
				Dim finalListNames As String
				For Each childTest As WorkSessionResultDS.PatientsRow In lstCalcChilds
					'Get the list of Calculated Tests in which the Test in process is included
					calcIDs = childTest.CalcTestID.Split(CChar(","))
					calcNames = childTest.CalcTestName.Split(CChar(","))

					'...and remove the deleted Calculated Test from the list
					finalListIDs = ""
					finalListNames = ""
					For i As Integer = 0 To calcIDs.Length - 1
						If (calcIDs(i).Trim <> pCalcTestID.ToString) Then
							If (finalListIDs <> "") Then finalListIDs &= ", "
							finalListIDs &= calcIDs(i).Trim

							If (finalListNames <> "") Then finalListNames &= ", "
							finalListNames &= calcNames(i).Trim
						End If
					Next

					'Rest of Calculated Tests remain in the list
					childTest.CalcTestID = finalListIDs
					childTest.CalcTestName = finalListNames
				Next
				lstCalcChilds = Nothing

				'Finally, the Calculated Test can be deleted
				Dim lstPatientRow As List(Of WorkSessionResultDS.PatientsRow)
				lstPatientRow = (From a In pWorkSessionResultDS.Patients _
								Where a.SampleID = pSampleID _
								  And a.StatFlag = pStatFlag _
								  And a.TestType = "CALC" _
								  And a.TestID = pCalcTestID _
							   Select a).ToList

				If (lstPatientRow.Count = 1) Then lstPatientRow(0).Delete()
				lstPatientRow = Nothing

				pWorkSessionResultDS.Patients.AcceptChanges()

			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.DeleteCalculatedTest", EventLogEntryType.Error, False)
				'Throw ex  'Commented line RH 12/04/2012
				'Do prefer using an empty throw when catching and re-throwing an exception.
				'This is the best way to preserve the exception call stack.
				'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
				'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
				Throw

			End Try
		End Sub

		''' <summary>
		''' Get additional information for all Patient Order Tests according the Test Type in the following way:
		'''   ** For STANDARD and ISE Tests: Name and Number of Replicates
		'''   ** For CALCULATED Tests: Name and Formula
		'''   ** For OFF-SYSTEM Tests: Name
		''' </summary>
		''' <param name="pSavedWSOrderTestsDS">Typed DataSet SavedWSOrderTestsDS containing the list of all Order Tests in a Saved WS</param>
		''' <returns>GlobalDataTO containing a typed DataSet SavedWSOrderTestsDS with the list of all Patient Order Tests in
		'''          the Saved Work Session with the additional information obtained for each different Test according its type</returns>
		''' <remarks>
		''' Created by:  SA 21/02/2011
		''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
		'''              SA 08/02/2012 - Do not inform Number of Replicate for ISE Tests
		'''              SA 18/04/2012 - Changes in management of Calculate Tests: inform fields CalcTestIDs and CalcTestNames for Tests
		'''                              included in the Formula only for those Patients for which the Calculated Test has been requested 
		''' </remarks>
		Private Function GetTestsData(ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS) As GlobalDataTO
			Dim resultData As New GlobalDataTO

			Try
				'Get all different Standard Tests requested for Patients 
				Dim lstSTDTestIDs As List(Of Integer)
				lstSTDTestIDs = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
								Where a.SampleClass = "PATIENT" _
							  AndAlso a.TestType = "STD" _
							   Select a.TestID Distinct).ToList

				Dim testDataDS As New TestsDS
				Dim myTestsDelegate As New TestsDelegate
				Dim lstSTDTests As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)

				For Each standardTestID As Integer In lstSTDTestIDs
					resultData = myTestsDelegate.Read(Nothing, standardTestID)

					If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
						testDataDS = DirectCast(resultData.SetDatos, TestsDS)

						'Update fields for TestName in the DataSet
						lstSTDTests = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
									  Where a.SampleClass = "PATIENT" _
									AndAlso a.TestType = "STD" _
									AndAlso a.TestID = standardTestID _
									 Select a).ToList
						For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstSTDTests
							row.BeginEdit()
							row.TestName = testDataDS.tparTests(0).TestName
							row.EndEdit()
						Next
					End If
				Next
				lstSTDTests = Nothing
				lstSTDTestIDs = Nothing

				'Get all different Calculated Tests requested for Patients 
				Dim lstCALCTestIDs As List(Of Integer)
				lstCALCTestIDs = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
								 Where a.SampleClass = "PATIENT" _
							   AndAlso a.TestType = "CALC" _
								Select a.TestID Distinct).ToList

				Dim myTestID As Integer
				Dim myTestType As String
				Dim mySampleType As String
				Dim incompleteCalcTest As Boolean

				Dim myFormulasDS As New FormulasDS
				Dim calcTestDataDS As New CalculatedTestsDS
				Dim myFormulasDelegate As New FormulasDelegate

				Dim lstTests As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
				Dim lstCALCTests As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)

				For Each calcTestID As Integer In lstCALCTestIDs
					'Get the list of Tests included in the Formula of the Calculated Test
					incompleteCalcTest = False

					resultData = myFormulasDelegate.GetTestsInFormula(Nothing, calcTestID, False)
					If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
						myFormulasDS = DirectCast(resultData.SetDatos, FormulasDS)

						'Check if all Tests included in the Formula also exist in the Work Session 
						For Each testInFormulas As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas
							myTestType = testInFormulas.TestType
							myTestID = Convert.ToInt32(testInFormulas.Value)
							mySampleType = testInFormulas.SampleType

							'Search if the component Test is also in the list of requested Tests
							lstTests = (From b As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
									   Where b.SampleClass = "PATIENT" _
									 AndAlso b.TestType = myTestType _
									 AndAlso b.TestID = myTestID _
									 AndAlso b.SampleType = mySampleType _
									  Select b).ToList()

							If (lstTests.Count = 0) Then
								incompleteCalcTest = True
								Exit For
							End If
						Next

						'Search of rows for the Calculated Test
						lstCALCTests = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
									  Where a.SampleClass = "PATIENT" _
									AndAlso a.TestType = "CALC" _
									AndAlso a.TestID = calcTestID _
									 Select a).ToList

						If (incompleteCalcTest) Then
							'One of the Tests in the Formula is not in the Work Session...the Calculated Test has to be deleted
							For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstCALCTests
								row.Delete()
								pSavedWSOrderTestsDS.AcceptChanges()
							Next
						Else
							'If all Tests included in the Formula exist in the Work Session, process them again to inform field CalcTestIDs
							For Each calcPatientOT As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstCALCTests
								For Each testInFormulas As FormulasDS.tparFormulasRow In myFormulasDS.tparFormulas
									myTestType = testInFormulas.TestType
									myTestID = Convert.ToInt32(testInFormulas.Value)
									mySampleType = testInFormulas.SampleType

									'Search the component Test in the list of requested Tests
									lstTests = (From c In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
											   Where c.SampleClass = "PATIENT" _
											 AndAlso c.TestType = myTestType _
											 AndAlso c.TestID = myTestID _
											 AndAlso c.SampleType = mySampleType _
											 AndAlso c.SampleID = calcPatientOT.SampleID _
											  Select c).ToList()

									For Each componentTests As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstTests
										If (componentTests.CalcTestIDs <> "") Then
											componentTests.CalcTestIDs &= ", "
											componentTests.CalcTestNames &= ", "
										End If

										componentTests.CalcTestIDs &= testInFormulas.CalcTestID
										componentTests.CalcTestNames &= testInFormulas.TestName
									Next
								Next
								pSavedWSOrderTestsDS.AcceptChanges()
							Next

							'Get TestName and Formula Text for the Calculated Test
							Dim myCalcTestsDelegate As New CalculatedTestsDelegate
							resultData = myCalcTestsDelegate.GetCalcTest(Nothing, calcTestID)

							If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
								calcTestDataDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

								'Update fields for TestName and FormulaText in the DataSet
								For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstCALCTests
									row.BeginEdit()
									row.TestName = calcTestDataDS.tparCalculatedTests(0).CalcTestLongName
									row.FormulaText = "=" & calcTestDataDS.tparCalculatedTests(0).FormulaText
									row.EndEdit()
								Next
							End If
						End If
					End If
				Next
				lstTests = Nothing
				lstCALCTests = Nothing
				lstCALCTestIDs = Nothing

				'Get all different ISE Tests requested for Patients 
				Dim lstISETestIDs As List(Of Integer)
				lstISETestIDs = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
								Where a.SampleClass = "PATIENT" _
							  AndAlso a.TestType = "ISE" _
							   Select a.TestID Distinct).ToList

				Dim iseTestDataDS As New ISETestsDS
				Dim myISETestsDelegate As New ISETestsDelegate
				Dim lstISETests As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)

				For Each iseTestID As Integer In lstISETestIDs
					resultData = myISETestsDelegate.Read(Nothing, iseTestID)

					If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
						iseTestDataDS = DirectCast(resultData.SetDatos, ISETestsDS)

						'Update field for TestName in the DataSet
						lstISETests = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
									  Where a.SampleClass = "PATIENT" _
									AndAlso a.TestType = "ISE" _
									AndAlso a.TestID = iseTestID _
									 Select a).ToList

						For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstISETests
							row.BeginEdit()
							row.TestName = iseTestDataDS.tparISETests(0).Name
							row.EndEdit()
						Next
					End If
				Next
				lstISETests = Nothing
				lstISETestIDs = Nothing

				'Get all different OFF-SYSTEM Tests requested for Patients 
				Dim lstOFFSTestIDs As List(Of Integer)
				lstOFFSTestIDs = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
								 Where a.SampleClass = "PATIENT" _
								   And a.TestType = "OFFS" _
								Select a.TestID Distinct).ToList

				Dim offTestDataDS As New OffSystemTestsDS
				Dim myOFFSTestsDelegate As New OffSystemTestsDelegate
				Dim lstOFFSTests As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)

				For Each offsTestID As Integer In lstOFFSTestIDs
					resultData = myOFFSTestsDelegate.Read(Nothing, offsTestID)

					If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
						offTestDataDS = DirectCast(resultData.SetDatos, OffSystemTestsDS)

						'Update field for TestName in the DataSet
						lstOFFSTests = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
									   Where a.SampleClass = "PATIENT" _
									 AndAlso a.TestType = "OFFS" _
									 AndAlso a.TestID = offsTestID _
									  Select a).ToList

						For Each row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In lstOFFSTests
							row.BeginEdit()
							row.TestName = offTestDataDS.tparOffSystemTests(0).Name
							row.EndEdit()
						Next
					End If
				Next
				lstOFFSTests = Nothing
				lstOFFSTestIDs = Nothing

				resultData.SetDatos = pSavedWSOrderTestsDS
				resultData.HasError = False
			Catch ex As Exception
				resultData.HasError = True
				resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
				resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.GetTestsData", EventLogEntryType.Error, False)
			End Try
			Return resultData
		End Function

		''' <summary>
		''' Load PATIENT Order Tests from the specified Saved WS
		''' </summary>
		''' <param name="pSavedWSOrderTestsDS">DataSet with all the Saved WS Order Tests</param>
		''' <param name="pWorkSessionResultDS">DataSet with the result of the process</param>
		''' <param name="pAnalizerID">Analizer Identifier</param>
		''' <param name="pFromLIMS">Flag indicating if the Saved WS to be loaded contains data of an Import from LIMS
		'''                         file; optional parameter with default value False</param>
		''' <returns>GlobalDataTO containing sucess/error information</returns>
		''' <remarks>
		''' Created by:  GDS 08/04/2010
		''' Modified by: SA  28/04/2010 - Changes to avoid duplication of the Patient Order Tests loaded
		'''              SA  25/05/2010 - Changes to load also WS having Calculated Tests
		'''              SA  28/05/2010 - Changes to change the date part of automatic SampleID using the current date
		'''              SA  15/09/2010 - Changes to adapt the function to the Import from LIMS process
		'''              SA  21/02/2011 - Call function GetTestsData to get additional information for the different 
		'''                               TestType/TestID requested for Patients in the Saved WS to load. Removed calls to function
		'''                               ProcessCalcTests
		'''              SA  31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
		'''              SA  25/04/2013 - For Patient Order Tests requested by an external LIS system, all informed LIS fields were  
        '''                               informed also in the SelectedTestsDS passed to function AddPatientOrderTests 
        '''              XB  28/08/2014 - Add behaviour for new field Selected - BT #1868
		''' </remarks>
		Private Function LoadAddPatientOrderTests(ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS, ByVal pWorkSessionResultDS As WorkSessionResultDS, _
												  ByVal pAnalizerID As String, Optional ByVal pFromLIMS As Boolean = False) As GlobalDataTO
			Dim resultData As New GlobalDataTO
			Try
				'Get additional data for each different TestType/Test requested for Patients
				resultData = GetTestsData(pSavedWSOrderTestsDS)
				If (Not resultData.HasError) Then
					'Get the list of different PatientID/SampleID in the list of Patient Order Tests
					Dim linqSampleIDs As List(Of String)
					linqSampleIDs = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
									Where a.SampleClass = "PATIENT" _
								 Order By a.CreationOrder _
								   Select a.SampleID Distinct).ToList

					Dim tempSampleID As String
					Dim sSampleIDType As String
					Dim linqSavedWSOrderTests As List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)

					For Each sSampleID As String In linqSampleIDs
						tempSampleID = sSampleID

						'Get all STAT Patient Order Tests for the SampleID
						linqSavedWSOrderTests = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
												Where a.SampleClass = "PATIENT" _
											  AndAlso a.SampleID = tempSampleID _
											  AndAlso a.StatFlag = True _
											 Order By a.CreationOrder _
											   Select a).ToList

						If (linqSavedWSOrderTests.Count > 0) Then
							sSampleIDType = linqSavedWSOrderTests(0).PatientIDType

							'Prepare the DataSet needed to load the Patient Order Tests
							Dim mySelectedTestDS As New SelectedTestsDS
							Dim mySelectedTestRow As SelectedTestsDS.SelectedTestTableRow
							For Each tempSavedWSOrderTests As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In linqSavedWSOrderTests
								mySelectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow

								mySelectedTestRow.TestType = tempSavedWSOrderTests.TestType
								mySelectedTestRow.SampleType = tempSavedWSOrderTests.SampleType
								mySelectedTestRow.TestID = tempSavedWSOrderTests.TestID
								mySelectedTestRow.TestName = tempSavedWSOrderTests.TestName
								mySelectedTestRow.NumReplicates = tempSavedWSOrderTests.ReplicatesNumber
								mySelectedTestRow.OTStatus = "OPEN"
								mySelectedTestRow.Selected = True

								'If the Test is included in the Formula of one or more Calculated Tests also requested, 
								'inform the correspondent fields
								If (Not tempSavedWSOrderTests.IsCalcTestIDsNull) Then
									mySelectedTestRow.CalcTestIDs = tempSavedWSOrderTests.CalcTestIDs
									mySelectedTestRow.CalcTestNames = tempSavedWSOrderTests.CalcTestNames
								End If
								If (tempSavedWSOrderTests.TestType = "CALC") Then mySelectedTestRow.CalcTestFormula = tempSavedWSOrderTests.FormulaText

								'For Patient Order Tests requested by an external LIS system, fill the informed LIS fields
								mySelectedTestRow.ExternalQC = False
								If (Not tempSavedWSOrderTests.IsExternalQCNull) Then mySelectedTestRow.ExternalQC = tempSavedWSOrderTests.ExternalQC
								mySelectedTestRow.LISRequest = (Not tempSavedWSOrderTests.IsAwosIDNull)

								If (Not tempSavedWSOrderTests.IsAwosIDNull) Then mySelectedTestRow.AwosID = tempSavedWSOrderTests.AwosID
								If (Not tempSavedWSOrderTests.IsSpecimenIDNull) Then mySelectedTestRow.SpecimenID = tempSavedWSOrderTests.SpecimenID
								If (Not tempSavedWSOrderTests.IsESPatientIDNull) Then mySelectedTestRow.ESPatientID = tempSavedWSOrderTests.ESPatientID
								If (Not tempSavedWSOrderTests.IsLISPatientIDNull) Then mySelectedTestRow.LISPatientID = tempSavedWSOrderTests.LISPatientID
								If (Not tempSavedWSOrderTests.IsESOrderIDNull) Then mySelectedTestRow.ESOrderID = tempSavedWSOrderTests.ESOrderID
                                If (Not tempSavedWSOrderTests.IsLISOrderIDNull) Then mySelectedTestRow.LISOrderID = tempSavedWSOrderTests.LISOrderID

								mySelectedTestDS.SelectedTestTable.Rows.Add(mySelectedTestRow)
							Next

							'Add the STAT Patient Order Tests
							resultData = AddPatientOrderTests(mySelectedTestDS, pWorkSessionResultDS, pAnalizerID, sSampleID, _
															  True, sSampleIDType, 1, 0, "", True)

							If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
								pWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
							Else
								'Error adding the STAT Patient Order Tests
								Exit For
							End If
						End If

						'Get all ROUTINE Patient Order Tests for the SampleID
						linqSavedWSOrderTests = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
												Where a.SampleClass = "PATIENT" _
											  AndAlso a.SampleID = tempSampleID _
											  AndAlso a.StatFlag = False _
											 Order By a.CreationOrder _
											   Select a).ToList

						If (linqSavedWSOrderTests.Count > 0) Then
							sSampleIDType = linqSavedWSOrderTests(0).PatientIDType

							'Prepare the DataSet needed to load the Patient Order Tests
							Dim mySelectedTestDS As New SelectedTestsDS
							Dim mySelectedTestRow As SelectedTestsDS.SelectedTestTableRow

							For Each tempSavedWSOrderTests As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In linqSavedWSOrderTests
								mySelectedTestRow = mySelectedTestDS.SelectedTestTable.NewSelectedTestTableRow

								mySelectedTestRow.TestType = tempSavedWSOrderTests.TestType
								mySelectedTestRow.SampleType = tempSavedWSOrderTests.SampleType
								mySelectedTestRow.TestID = tempSavedWSOrderTests.TestID
								mySelectedTestRow.TestName = tempSavedWSOrderTests.TestName
								mySelectedTestRow.NumReplicates = tempSavedWSOrderTests.ReplicatesNumber
								mySelectedTestRow.OTStatus = "OPEN"
								mySelectedTestRow.Selected = True

								'If the Test is included in the Formula of one or more Calculated Tests also requested, 
								'inform the corresponding fields
								If (Not tempSavedWSOrderTests.IsCalcTestIDsNull) Then
									mySelectedTestRow.CalcTestIDs = tempSavedWSOrderTests.CalcTestIDs
									mySelectedTestRow.CalcTestNames = tempSavedWSOrderTests.CalcTestNames
								End If
								If (tempSavedWSOrderTests.TestType = "CALC") Then mySelectedTestRow.CalcTestFormula = tempSavedWSOrderTests.FormulaText

								'For Patient Order Tests requested by an external LIS system, fill the informed LIS fields
								mySelectedTestRow.ExternalQC = False
								If (Not tempSavedWSOrderTests.IsExternalQCNull) Then mySelectedTestRow.ExternalQC = tempSavedWSOrderTests.ExternalQC
								mySelectedTestRow.LISRequest = (Not tempSavedWSOrderTests.IsAwosIDNull)

								If (Not tempSavedWSOrderTests.IsAwosIDNull) Then mySelectedTestRow.AwosID = tempSavedWSOrderTests.AwosID
								If (Not tempSavedWSOrderTests.IsSpecimenIDNull) Then mySelectedTestRow.SpecimenID = tempSavedWSOrderTests.SpecimenID
								If (Not tempSavedWSOrderTests.IsESPatientIDNull) Then mySelectedTestRow.ESPatientID = tempSavedWSOrderTests.ESPatientID
								If (Not tempSavedWSOrderTests.IsLISPatientIDNull) Then mySelectedTestRow.LISPatientID = tempSavedWSOrderTests.LISPatientID
								If (Not tempSavedWSOrderTests.IsESOrderIDNull) Then mySelectedTestRow.ESOrderID = tempSavedWSOrderTests.ESOrderID
								If (Not tempSavedWSOrderTests.IsLISOrderIDNull) Then mySelectedTestRow.LISOrderID = tempSavedWSOrderTests.LISOrderID

								mySelectedTestDS.SelectedTestTable.Rows.Add(mySelectedTestRow)
							Next

							'Add the ROUTINE Patient Order Tests
							resultData = AddPatientOrderTests(mySelectedTestDS, pWorkSessionResultDS, pAnalizerID, sSampleID, _
															  False, sSampleIDType, 1, 0, "", True)

							If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
								pWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
							Else
								'Error adding the ROUTINE Patient Order Tests
								Exit For
							End If
						End If
					Next
					linqSampleIDs = Nothing

					If (Not resultData.HasError) Then
						'For each added Patient Order Tests, update values of TubeType and Num Replicates with the ones stored in DB
						Dim currentSampleID As String
						Dim currentSampleType As String
						Dim currentTestID As Integer

						For Each rowPatient As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients.Rows
							'Search saved value of TubeType for the SampleID and SampleType of the current record
							'Search saved value of Number of Replicates for SampleID and TestID of the current record
							currentSampleID = rowPatient.SampleID
							currentSampleType = rowPatient.SampleType
							currentTestID = rowPatient.TestID

                            ' XB 28/08/2014 -- BT #1868
                            'If (rowPatient.TestType = "STD" OrElse rowPatient.TestType = "ISE") Then
                            '    linqSavedWSOrderTests = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
                            '                            Where a.SampleClass = "PATIENT" _
                            '                          AndAlso a.SampleID = currentSampleID _
                            '                          AndAlso a.SampleType = currentSampleType _
                            '                          AndAlso a.TestID = currentTestID _
                            '                           Select a).ToList

                            '    If (linqSavedWSOrderTests.Count > 0) Then
                            '        rowPatient.BeginEdit()
                            '        rowPatient.TubeType = linqSavedWSOrderTests(0).TubeType
                            '        rowPatient.NumReplicates = linqSavedWSOrderTests(0).ReplicatesNumber
                            '        rowPatient.EndEdit()
                            '        rowPatient.AcceptChanges()
                            '    End If
                            'End If

                            If (rowPatient.TestType = "STD" OrElse rowPatient.TestType = "ISE" OrElse rowPatient.TestType = "CALC") Then
                                linqSavedWSOrderTests = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
                                                        Where a.SampleClass = "PATIENT" _
                                                      AndAlso a.SampleID = currentSampleID _
                                                      AndAlso a.SampleType = currentSampleType _
                                                      AndAlso a.TestID = currentTestID _
                                                       Select a).ToList

                                If (linqSavedWSOrderTests.Count > 0) Then
                                    rowPatient.BeginEdit()

                                    If (rowPatient.TestType = "STD" OrElse rowPatient.TestType = "ISE") Then
                                        rowPatient.TubeType = linqSavedWSOrderTests(0).TubeType
                                        rowPatient.NumReplicates = linqSavedWSOrderTests(0).ReplicatesNumber
                                    End If

                                    If Not (linqSavedWSOrderTests(0).IsSelectedNull) Then
                                        rowPatient.Selected = linqSavedWSOrderTests(0).Selected
                                    End If

                                    rowPatient.EndEdit()
                                    rowPatient.AcceptChanges()
                                End If
                            End If
                            ' XB 28/08/2014 -- BT #1868

                            If (rowPatient.SampleIDType = "AUTO") Then
                                'Update the SampleID: date part have the current date
                                rowPatient.BeginEdit()
                                rowPatient.SampleID = "#" & DateTime.Now.ToString("yyyyMMdd") & rowPatient.SampleID.Substring(9)
                                rowPatient.EndEdit()
                                rowPatient.AcceptChanges()
                            End If
                        Next

						resultData.SetDatos = pWorkSessionResultDS
					End If
					linqSavedWSOrderTests = Nothing
				End If
			Catch ex As Exception
				resultData.HasError = True
				resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
				resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.LoadAddPatientOrderTests", EventLogEntryType.Error, False)
			End Try
			Return resultData
		End Function

		''' <summary>
		''' Load BLANK, CALIBRATOR, CONTROL and PATIENT Order Tests from the specified Saved WS
		''' </summary>
		''' <param name="pSavedWSOrderTestsDS">DataSet with all the Saved WS Order Tests</param>
		''' <param name="pWorkSessionResultDS">DataSet with the result of the process</param>
		''' <param name="pAnalizerID">Analizer Identifier</param>
		''' <returns>GlobalDataTO containing sucess/error information</returns>
		''' <remarks>
		''' Created by:  GDS 08/04/2010
		''' Modified by: SA  28/04/2010 - When searching stored values of TubeType and Number of Replicates, filter saved Order
		'''                               Tests for SampleClass, TestID and SampleType instead of by CreationOrder
		'''              SA  24/02/2011 - Added new parameter pSelectedTestsDS. For each requested Order Test of the informed
		'''                               SampleClass, verify if the Test/SampleType is already included in the list of Selected
		'''                               Tests and add it to the DS in case it is not included 
		'''              SA  23/05/2011 - Condition of (linqSavedWSOrderTests.Count > 0) is only for adding additional components 
		'''                               to the list of selected Tests/SampleTypes. The rest of the code has to be executed always
		'''              SA  31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
		'''              SA  25/04/2013 - For Control Order Tests requested by an external LIS system, all informed LIS fields were  
		'''                               informed also in the SelectedTestsDS passed to function AddControlOrderTests 
        '''              TR  17/05/2013 - Inform optional parameter for the SampleClass when calling function AddControlOrderTests
        '''              XB  28/08/2014 - Add behaviour for new field Selected - BT #1868
		''' </remarks>
		Private Function LoadAddSampleClassOrderTests(ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS, ByVal pWorkSessionResultDS As WorkSessionResultDS, _
													  ByVal pAnalizerID As String, ByVal pSampleClass As String, ByVal pSelectedTestDS As SelectedTestsDS) As GlobalDataTO
			Dim resultData As New GlobalDataTO

			Try
				'Search all Order Tests in the SavedWS for the informed SampleClass
				Dim linqSavedWSOrderTests As New List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
				linqSavedWSOrderTests = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
										Where a.SampleClass = pSampleClass _
									 Order By a.CreationOrder _
									   Select a).ToList()

				If (linqSavedWSOrderTests.Count > 0) Then
					For Each requestedOT In linqSavedWSOrderTests
						'Verify if the TestType/Test/SampleType is already included in the list of Tests for Patients
						Dim lstTestSample As List(Of SelectedTestsDS.SelectedTestTableRow)
						lstTestSample = (From b As SelectedTestsDS.SelectedTestTableRow In pSelectedTestDS.SelectedTestTable _
										Where b.TestType = requestedOT.TestType _
									  AndAlso b.TestID = requestedOT.TestID _
									  AndAlso b.SampleType = requestedOT.SampleType _
									   Select b).ToList()

						If (lstTestSample.Count = 0) Then
							'TestType/TestID/SampleType not in the list, add it
							Dim mySelectedTestRow As SelectedTestsDS.SelectedTestTableRow
							mySelectedTestRow = pSelectedTestDS.SelectedTestTable.NewSelectedTestTableRow

							mySelectedTestRow.TestType = requestedOT.TestType
							mySelectedTestRow.SampleType = requestedOT.SampleType
							mySelectedTestRow.TestID = requestedOT.TestID
                            mySelectedTestRow.OTStatus = "OPEN"
                            mySelectedTestRow.Selected = True

							If (pSampleClass = "CTRL") Then
								mySelectedTestRow.ExternalQC = False
								If (Not requestedOT.IsExternalQCNull) Then mySelectedTestRow.ExternalQC = requestedOT.ExternalQC
								mySelectedTestRow.LISRequest = (Not requestedOT.IsAwosIDNull)

								If (Not requestedOT.IsAwosIDNull) Then mySelectedTestRow.AwosID = requestedOT.AwosID
								If (Not requestedOT.IsSpecimenIDNull) Then mySelectedTestRow.SpecimenID = requestedOT.SpecimenID
								If (Not requestedOT.IsESPatientIDNull) Then mySelectedTestRow.ESPatientID = requestedOT.ESPatientID
								If (Not requestedOT.IsLISPatientIDNull) Then mySelectedTestRow.LISPatientID = requestedOT.LISPatientID
								If (Not requestedOT.IsESOrderIDNull) Then mySelectedTestRow.ESOrderID = requestedOT.ESOrderID
								If (Not requestedOT.IsLISOrderIDNull) Then mySelectedTestRow.LISOrderID = requestedOT.LISOrderID
							End If

							pSelectedTestDS.SelectedTestTable.Rows.Add(mySelectedTestRow)
						Else
							'Only for CONTROLS, if TestType/TestID/SampleType is in the list, update LIS fields if they are informed
							If (pSampleClass = "CTRL") Then
								lstTestSample.First.BeginEdit()
								lstTestSample.First.ExternalQC = False
								If (Not requestedOT.IsExternalQCNull) Then lstTestSample.First.ExternalQC = requestedOT.ExternalQC
								lstTestSample.First.LISRequest = (Not requestedOT.IsAwosIDNull)

								If (Not requestedOT.IsAwosIDNull) Then lstTestSample.First.AwosID = requestedOT.AwosID
								If (Not requestedOT.IsSpecimenIDNull) Then lstTestSample.First.SpecimenID = requestedOT.SpecimenID
								If (Not requestedOT.IsESPatientIDNull) Then lstTestSample.First.ESPatientID = requestedOT.ESPatientID
								If (Not requestedOT.IsLISPatientIDNull) Then lstTestSample.First.LISPatientID = requestedOT.LISPatientID
								If (Not requestedOT.IsESOrderIDNull) Then lstTestSample.First.ESOrderID = requestedOT.ESOrderID
								If (Not requestedOT.IsLISOrderIDNull) Then lstTestSample.First.LISOrderID = requestedOT.LISOrderID
								lstTestSample.First.EndEdit()
							End If
						End If
					Next
				End If

				Dim myOrderTests As New OrderTestsDelegate
				Select Case pSampleClass
					Case "CTRL"
						resultData = myOrderTests.AddControlOrderTests(pSelectedTestDS, pWorkSessionResultDS, pAnalizerID, True, False, "CTRL")
						If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
							'Update fields Number of Replicates and Tube Type with the values saved in DB - Get Control having the same Test and SampleType
							Dim tempControlRow As WorkSessionResultDS.ControlsRow
							For Each rowControl As WorkSessionResultDS.ControlsRow In DirectCast(resultData.SetDatos, WorkSessionResultDS).Controls.Rows
								tempControlRow = rowControl
								linqSavedWSOrderTests = (From a As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
														Where a.SampleClass = pSampleClass _
													  AndAlso a.TestType = tempControlRow.TestType _
													  AndAlso a.TestID = tempControlRow.TestID _
													  AndAlso a.SampleType = tempControlRow.SampleType _
													   Select a).ToList

								If (linqSavedWSOrderTests.Count > 0) Then
									rowControl.BeginEdit()
									rowControl.NumReplicates = linqSavedWSOrderTests(0).ReplicatesNumber
                                    rowControl.TubeType = linqSavedWSOrderTests(0).TubeType

                                    ' XB 29/08/2014 - BT #1868
                                    If Not linqSavedWSOrderTests(0).IsSelectedNull Then
                                        rowControl.Selected = linqSavedWSOrderTests(0).Selected
                                    End If

									rowControl.EndEdit()
									rowControl.AcceptChanges()
								End If
							Next
						End If

					Case "CALIB"
						resultData = myOrderTests.AddCalibratorOrderTests(pSelectedTestDS, pWorkSessionResultDS, pAnalizerID, True)
						If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
							'Update fields Number of Replicates and Tube Type with the values saved in DB - Get Calibrator having the same Test and SampleType
							Dim tempBlankRow As WorkSessionResultDS.BlankCalibratorsRow
							For Each rowBlank As WorkSessionResultDS.BlankCalibratorsRow In DirectCast(resultData.SetDatos, WorkSessionResultDS).BlankCalibrators.Rows
								If (rowBlank.SampleClass = pSampleClass) Then
									tempBlankRow = rowBlank
									linqSavedWSOrderTests = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
															Where a.SampleClass = pSampleClass _
														  AndAlso a.TestID = tempBlankRow.TestID _
														  AndAlso a.SampleType = tempBlankRow.SampleType _
														   Select a).ToList

									If (linqSavedWSOrderTests.Count > 0) Then
										rowBlank.BeginEdit()
										rowBlank.NumReplicates = linqSavedWSOrderTests(0).ReplicatesNumber
										rowBlank.TubeType = linqSavedWSOrderTests(0).TubeType

                                        ' XB 29/08/2014 - BT #1868
                                        If Not linqSavedWSOrderTests(0).IsSelectedNull Then
                                            rowBlank.Selected = linqSavedWSOrderTests(0).Selected
                                        End If

                                        rowBlank.EndEdit()
										rowBlank.AcceptChanges()
									End If
								End If
							Next
						End If

					Case "BLANK"
						resultData = myOrderTests.AddBlankOrderTests(pSelectedTestDS, pWorkSessionResultDS, pAnalizerID)
						If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
							'Update field Number of Replicates with the value saved in DB - Get Blank having the same Test
							Dim tempBlankRow As WorkSessionResultDS.BlankCalibratorsRow
							For Each rowBlank As WorkSessionResultDS.BlankCalibratorsRow In DirectCast(resultData.SetDatos, WorkSessionResultDS).BlankCalibrators.Rows
								If (rowBlank.SampleClass = pSampleClass) Then
									tempBlankRow = rowBlank
									linqSavedWSOrderTests = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
															Where a.SampleClass = pSampleClass _
														  AndAlso a.TestID = tempBlankRow.TestID _
														   Select a).ToList

									If (linqSavedWSOrderTests.Count > 0) Then
										rowBlank.BeginEdit()
										rowBlank.NumReplicates = linqSavedWSOrderTests(0).ReplicatesNumber

                                        ' XB 29/08/2014 - BT #1868
                                        If Not linqSavedWSOrderTests(0).IsSelectedNull Then
                                            rowBlank.Selected = linqSavedWSOrderTests(0).Selected
                                        End If

                                        rowBlank.EndEdit()
										rowBlank.AcceptChanges()
									End If
								End If
							Next
						End If
				End Select
				linqSavedWSOrderTests = Nothing
			Catch ex As Exception
				resultData.HasError = True
				resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
				resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.LoadAddSampleClassOrderTests", EventLogEntryType.Error, False)
			End Try
			Return resultData
		End Function

		''' <summary>
		''' Merge received values and validate if value do not exist on the origin to avoid 
		''' duplicate value.
		''' </summary>
		''' <param name="pValuesToAdd">Values To add on the result.</param>
		''' <param name="pPrevValues">Previous values.</param>
		''' <returns></returns>
		''' <remarks>
		''' Created by: TR 03/04/2013
		''' </remarks>
		Private Function MergeValues(pValuesToAdd As String, pPrevValues As String) As String
			Dim myResults As String = String.Empty
			Try
				myResults = pPrevValues
				For Each ValueToMerge As String In pValuesToAdd.Split(CChar(","))

					If myResults.Length > 0 Then
						If Not myResults.Contains(ValueToMerge) Then
							myResults &= "," & ValueToMerge
						End If
					Else
						myResults = ValueToMerge
					End If
				Next
			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.MergeValues", EventLogEntryType.Error, False)
			End Try
			Return myResults

		End Function

		''' <summary>
		''' Overload of function RejectControlTest, needed to load the content of a row of a typed DataSet SelectedTestsDS in a row of a typed
		''' DataSet SavedWSOrderTestsDS, which is the parameter the function needs 
		''' </summary>
		''' <param name="mySavedrow">Row of a typed DataSet SelectedTestsDS containing all data of the LIS Awos to reject</param>
		''' <param name="pRejectedOTLISInfo">Typed DataSet OrderTestsLISInfoDS in which data of the rejected LIS Awos has to be loaded</param>
		''' <param name="myImportErrorsLogDS">Typed DataSet ImportErrorsLogDS to load the reason of the rejection for the LIS MessageID and LIS Awos ID</param>
		''' <param name="msgLISError">Code of the error code that has to be informed as reason of the rejection in the ImportErrorsLogDS</param>
		''' <remarks>
		''' Created by: JCM 16/05/2013
		''' </remarks>
		Private Sub RejectControlTest(ByRef mySavedrow As SelectedTestsDS.SelectedTestTableRow, ByRef pRejectedOTLISInfo As OrderTestsLISInfoDS, ByRef myImportErrorsLogDS As ImportErrorsLogDS, _
									  ByVal msgLISerror As GlobalEnumerates.Messages)
			Try
				'Load data in SelectedTestsDS.SelectedTestTableRow to a SavedWSOrderTestsDS.tparSavedWSOrderTestsRow before calling
				'the function that add the information to the DS containing the Awos to reject
                Dim row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow = Nothing
				With row
					If (Not mySavedrow.IsAwosIDNull) Then .AwosID = mySavedrow.AwosID
					If (Not mySavedrow.IsSpecimenIDNull) Then .SpecimenID = mySavedrow.SpecimenID
					If (Not mySavedrow.IsESOrderIDNull) Then .ESOrderID = mySavedrow.ESOrderID
					If (Not mySavedrow.IsESPatientIDNull) Then .ESPatientID = mySavedrow.ESPatientID
					.TestType = mySavedrow.TestType
					.TestID = mySavedrow.TestID
					.SampleType = mySavedrow.SampleType
				End With

				RejectControlTest(row, pRejectedOTLISInfo, myImportErrorsLogDS, msgLISerror)
			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.RejectControlTest", EventLogEntryType.Error, False)
				Throw
			End Try
		End Sub

		''' <summary>
		''' When a LIS Awos ID is rejected, this function load the two needed typed DataSet: an OrderTestsLISInfoDS needed to built the rejection message
		''' to sent to the external LIS system, and an ImportErrorsLogDS needed to save the reason of the rejection in table twksImportErrorsLog 
		''' </summary>
		''' <param name="mySavedrow">Row of a typed DataSet SavedWSOrderTestsDS containing all data of the LIS Awos to reject</param>
		''' <param name="pRejectedOTLISInfo">Typed DataSet OrderTestsLISInfoDS in which data of the rejected LIS Awos has to be loaded</param>
		''' <param name="myImportErrorsLogDS">Typed DataSet ImportErrorsLogDS to load the reason of the rejection for the LIS MessageID and LIS Awos ID</param>
		''' <param name="msgLISError">Code of the error code that has to be informed as reason of the rejection in the ImportErrorsLogDS</param>
		''' <remarks>
		''' Created by:  JCM 16/05/2013
		''' Modified by: SA  11/06/2013 - When load data in an ImportErrorLogDS, inform field LineText as "MessageID | AwosID" instead of inform specific fields MessageID
		'''                               and AwosID in the DS (due to these fields have been deleted).
		''' </remarks>
		Private Sub RejectControlTest(ByRef mySavedrow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, ByRef pRejectedOTLISInfo As OrderTestsLISInfoDS, _
									  ByRef myImportErrorsLogDS As ImportErrorsLogDS, ByVal msgLISerror As GlobalEnumerates.Messages)

			Try
				If (pRejectedOTLISInfo Is Nothing) Then pRejectedOTLISInfo = New OrderTestsLISInfoDS
				Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = pRejectedOTLISInfo.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow

				With myRow
					.BeginEdit()
					If (Not mySavedrow.IsAwosIDNull) Then .AwosID = mySavedrow.AwosID
					If (Not mySavedrow.IsSpecimenIDNull) Then .SpecimenID = mySavedrow.SpecimenID
					If (Not mySavedrow.IsESOrderIDNull) Then .ESOrderID = mySavedrow.ESOrderID
					If (Not mySavedrow.IsESPatientIDNull) Then .ESPatientID = mySavedrow.ESPatientID

					.TestType = mySavedrow.TestType
					.SampleType = mySavedrow.SampleType
					.SampleClass = "QC"
					.StatFlagText = "normal"

					If (mySavedrow.DeletedTestFlag) Then
						'If DeletedTestFlag is TRUE for the Order Test in theSaved WS, it means the Test or the Test/Sample Type has been deleted
						'In this case, field TestName contains the LIS Test Name, not the Biosystem Test Name, and field TestID is not informed 
						.CheckLISValues = False
						If (Not mySavedrow.IsTestNameNull) Then .TestIDString = mySavedrow.TestName
					Else
						.TestID = mySavedrow.TestID
						.CheckLISValues = True
					End If
					.EndEdit()
				End With

				pRejectedOTLISInfo.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
				pRejectedOTLISInfo.AcceptChanges()

				'Report Error details
				If (myImportErrorsLogDS Is Nothing) Then myImportErrorsLogDS = New ImportErrorsLogDS
				Dim myErrLogRow As ImportErrorsLogDS.twksImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
				With myErrLogRow
					.BeginEdit()
					.ErrorCode = msgLISerror.ToString()
					.LineText = mySavedrow.SavedWSName
					If (Not mySavedrow.IsAwosIDNull) Then .LineText &= " | " & mySavedrow.AwosID
					.EndEdit()
				End With
				myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myErrLogRow)
				myImportErrorsLogDS.AcceptChanges()
			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.RejectControlTest", EventLogEntryType.Error, False)
				Throw
			End Try
		End Sub

		''' <summary>
		''' Search in the active WS if there are manual Orders having the SpecimenID sent by LIS as SampleID, and in this case, update field SampleID
		''' in the DS with the value of field SampleID in the LIS Saved WS  
		''' </summary>
		''' <param name="pSavedLISRow">Row of a typed DataSet SavedWSOrderTestsDS containing all data of the LIS Awos being processed</param>
		''' <param name="pWorkSessionResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples in the active WorkSession</param>
		''' <returns>GlobalDataTO containing an string list with the OrderIDs from the active WS for which field PatientID/SampleID has to be updated</returns>
		''' <remarks>
		''' Created by:  SA 01/08/2013
		''' </remarks>
		Private Function SearchOrdersBySpecimenID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSavedLISRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, _
												  ByRef pWorkSessionResultDS As WorkSessionResultDS) As GlobalDataTO
			Dim myOrders As New List(Of String)
			Dim myGlobalDataTO As New GlobalDataTO
			Dim dbConnection As SqlClient.SqlConnection = Nothing

			Try
				myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
				If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
					dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
					If (Not dbConnection Is Nothing) Then
						'Search if there are Orders in the active Work Session with SampleID = SpecimenID sent by LIS
						Dim mySpecimenID As String = pSavedLISRow.SpecimenID
						Dim myPatientOTOnActiveWSList As List(Of WorkSessionResultDS.PatientsRow) = (From a As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients _
																									Where a.SampleClass = "PATIENT" _
																								  AndAlso a.SampleID = mySpecimenID).ToList()

						If (myPatientOTOnActiveWSList.Count > 0) Then
							'For all Order Tests belonging to Orders with SampleID = SpecimenID, update fields PatientID/SampleID with the value sent by LIS
							For Each orderTest As WorkSessionResultDS.PatientsRow In myPatientOTOnActiveWSList
								orderTest.BeginEdit()
								orderTest.SampleID = pSavedLISRow.SampleID
								orderTest.SampleIDType = pSavedLISRow.PatientIDType
								orderTest.EndEdit()
							Next

							'Get the list of OrderIDs updated (normal case is one, but it can be two if there are two Orders with different Priority)
							myOrders = (From a As WorkSessionResultDS.PatientsRow In myPatientOTOnActiveWSList _
										Select a.OrderID Distinct).ToList()

							Dim myOrdersDelegate As New OrdersDelegate
							myGlobalDataTO = myOrdersDelegate.UpdatePatientSampleFields(dbConnection, myOrders, pSavedLISRow.SampleID, pSavedLISRow.PatientIDType, _
																						pSavedLISRow.SampleType)
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

				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.SearchOrderBySpecimenID", EventLogEntryType.Error, False)
			Finally
				If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
			End Try
			Return myGlobalDataTO
		End Function

		''' <summary>
		''' Rewrite value of CreationOrder for Calibrator Order Tests
		''' </summary>
		''' <param name="pWSResultDS">Type DataSet containing the list of Calibrator Order Tests to sort</param>
		''' <param name="pSortingType">Optional parameter to indicate the type of sorting to be applied in the
		'''                            grid of Calibrators; possible values: 
		'''                            1 - Status / Calibrator: sent Order Tests are placed first. Open Order Tests are 
		'''                                placed in the order they have been requested but grouped by Calibrator
		'''                            2 - Calibrator / Status: Order Tests are placed in the order they have been requested
		'''                                but grouped by Calibrator and for each Calibrator the sent ones are placed first</param>
		''' <remarks>
		''' Created by:  SA 26/03/2010
		''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
		''' </remarks>
		Private Sub SortCalibrators(ByVal pWSResultDS As WorkSessionResultDS, Optional ByVal pSortingType As Integer = 1)
			Try
				If (pSortingType = 1) Then
					'If there are Calibrator Order Tests sent to the Analyzer, the OPEN Order Tests will be placed after them
					'grouped by Calibrator
					Dim nextCreationOrder As Integer = 1
					Dim lstNonOpenCalib = (From x In pWSResultDS.BlankCalibrators _
											 Where x.SampleClass = "CALIB" _
											   And x.OTStatus <> "OPEN" _
											 Order By x.CreationOrder Descending _
											 Select x.CreationOrder).ToList()

					If (lstNonOpenCalib.Count > 0) Then nextCreationOrder = lstNonOpenCalib(0) + 1
					lstNonOpenCalib = Nothing

					'Get all different Open Calibrator Order Tests in the list
					Dim allCalibrators = (From a In pWSResultDS.BlankCalibrators _
											Where a.SampleClass = "CALIB" _
											  And a.OTStatus = "OPEN" _
											Order By a.CreationOrder _
											Select a.CalibratorID).Distinct()

					Dim myCalibratorID As Integer
					Dim lstCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
					For Each calibratorRow In allCalibrators
						'Get all Tests using the Calibrator
						myCalibratorID = calibratorRow
						lstCalibratorsDS = (From b In pWSResultDS.BlankCalibrators _
											  Where b.SampleClass = "CALIB" _
												And b.OTStatus = "OPEN" _
												And (Not b.IsCalibratorIDNull AndAlso _
														 b.CalibratorID = myCalibratorID) _
											  Order By b.CreationOrder _
											  Select b).ToList

						For Each calibOrderTest In lstCalibratorsDS
							'Update the CreationOrder
							calibOrderTest.CreationOrder = nextCreationOrder
							nextCreationOrder += 1
						Next
					Next
					lstCalibratorsDS = Nothing
					allCalibrators = Nothing

				ElseIf (pSortingType = 2) Then
					'Get all different Calibrators in the list
					Dim nextCreationOrder As Integer = 1
					Dim allCalibrators = (From a In pWSResultDS.BlankCalibrators _
											Where a.SampleClass = "CALIB" _
											Order By a.CreationOrder _
											Select a.CalibratorID).Distinct()

					Dim myCalibratorID As Integer
					Dim lstCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
					For Each calibratorRow In allCalibrators
						'Get all Tests using the Calibrator
						myCalibratorID = calibratorRow
						lstCalibratorsDS = (From b In pWSResultDS.BlankCalibrators _
											  Where b.SampleClass = "CALIB" _
												And (Not b.IsCalibratorIDNull AndAlso _
														 b.CalibratorID = myCalibratorID) _
											  Order By b.CreationOrder _
											  Select b).ToList

						For Each calibOrderTest In lstCalibratorsDS
							'Update the CreationOrder
							calibOrderTest.CreationOrder = nextCreationOrder
							nextCreationOrder += 1
						Next
					Next
					lstCalibratorsDS = Nothing
					allCalibrators = Nothing
				End If
			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.SortCalibrators", EventLogEntryType.Error, False)
				'Throw ex  'Commented line RH 12/04/2012
				'Do prefer using an empty throw when catching and re-throwing an exception.
				'This is the best way to preserve the exception call stack.
				'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
				'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
				Throw
			End Try
		End Sub

		''' <summary>
		''' Rewrite value of CreationOrder for Control Order Tests
		''' </summary>
		''' <param name="pWSResultDS">Type DataSet containing the list of Control Order Tests to sort</param>
		''' <param name="pSortingType">Optional parameter to indicate the type of sorting to be applied in the
		'''                            grid of Controls; possible values: 
		'''                            1 - Status / Control: sent Order Tests are placed first. Open Order Tests are 
		'''                                placed in the order they have been requested but grouped by Control
		'''                            2 - Control / Status: Order Tests are placed in the order they have been requested
		'''                                but grouped by Control and for each Control the sent ones are placed first</param>
		''' <remarks>
		''' Created by:  SA 26/03/2010
		''' Modified by: DL 15/04/2011 - Replaced use of field ControlNumber for field ControlID
		'''              SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
		''' </remarks>
		Private Sub SortControls(ByVal pWSResultDS As WorkSessionResultDS, Optional ByVal pSortingType As Integer = 1)
			Try
				If (pSortingType = 1) Then
					'If there are Control Order Tests sent to the Analyzer, the OPEN Order Tests will be placed after them
					'grouped by Control
					Dim nextCreationOrder As Integer = 1
					Dim lstNonOpenCtrls = (From x In pWSResultDS.Controls _
										  Where x.SampleClass = "CTRL" _
										AndAlso x.OTStatus <> "OPEN" _
									   Order By x.CreationOrder Descending _
										 Select x.CreationOrder).ToList()

					If (lstNonOpenCtrls.Count > 0) Then nextCreationOrder = lstNonOpenCtrls(0) + 1
					lstNonOpenCtrls = Nothing

					'Get all different OPEN Controls in the list
					Dim allControls = (From a In pWSResultDS.Controls _
									  Where a.SampleClass = "CTRL" _
									AndAlso a.OTStatus = "OPEN" _
								   Order By a.CreationOrder _
									 Select a.ControlID).Distinct()

					Dim myControlID As Integer
					Dim lstControlsDS As List(Of WorkSessionResultDS.ControlsRow)
					For Each controlRow In allControls
						'Get all Tests/SampleTypes using the Control
						myControlID = controlRow
						lstControlsDS = (From b In pWSResultDS.Controls _
										Where b.SampleClass = "CTRL" _
									  AndAlso b.OTStatus = "OPEN" _
									  AndAlso b.ControlID = myControlID _
									 Order By b.CreationOrder _
									   Select b).ToList

						For Each ctrlOrderTest In lstControlsDS
							'Update the CreationOrder
							ctrlOrderTest.CreationOrder = nextCreationOrder
							nextCreationOrder += 1
						Next
					Next
					lstControlsDS = Nothing
					allControls = Nothing

				ElseIf (pSortingType = 2) Then
					'Get all different Controls in the list
					Dim nextCreationOrder As Integer = 1
					Dim allControls = (From a In pWSResultDS.Controls _
									  Where a.SampleClass = "CTRL" _
								   Order By a.CreationOrder _
									 Select a.ControlID).Distinct()

					Dim myControlID As Integer
					Dim lstControlsDS As List(Of WorkSessionResultDS.ControlsRow)
					For Each controlRow In allControls
						'Get all Tests using the Control
						myControlID = controlRow
						lstControlsDS = (From b In pWSResultDS.Controls _
										Where b.SampleClass = "CTRL" _
									  AndAlso b.ControlID = myControlID _
									 Order By b.CreationOrder _
									   Select b).ToList

						For Each ctrlOrderTest In lstControlsDS
							'Update the CreationOrder
							ctrlOrderTest.CreationOrder = nextCreationOrder
							nextCreationOrder += 1
						Next
					Next
					lstControlsDS = Nothing
					allControls = Nothing
				End If
			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.SortControls", EventLogEntryType.Error, False)
				'Throw ex  'Commented line RH 12/04/2012
				'Do prefer using an empty throw when catching and re-throwing an exception.
				'This is the best way to preserve the exception call stack.
				'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
				'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
				Throw
			End Try
		End Sub

		''' <summary>
		''' Execute all validations needed to verify if a Calibrator Order Test can be deleted.  It is allowed to delete a Calibrator
		''' when there are not Controls nor Patient Samples requested for the same TestType/SampleType/Test
		''' </summary>
		''' <param name="pTestType">Code of the Test Type</param>
		''' <param name="pTestID">Test Identifier</param>
		''' <param name="pSampleType">Code of the Sample Type</param>
		''' <param name="pWSResultDS">Typed DataSet WorkSessionResultDS containing all requested Order Tests</param>
		''' <returns>True when the Calibrator Order Test can be deleted; otherwise it returns False</returns>
		''' <remarks>
		''' Created by:  SA 26/02/2010
		''' Modified by: SA 31/01/2012 - Set to Nothing all declared Lists; do not declare variables inside loops 
		''' </remarks>
		Private Function VerifyCalibratorDeletion(ByVal pTestType As String, ByVal pTestID As Integer, ByVal pSampleType As String, _
												  ByVal pWSResultDS As WorkSessionResultDS) As Boolean
			Dim deleteCalibrator As Boolean = False

			Try
				'Verify if there are Controls requested for the Test/SampleType
				Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)
				lstWSControlsDS = (From c In pWSResultDS.Controls _
									 Where c.SampleClass = "CTRL" _
									   And c.TestType = pTestType _
									   And c.TestID = pTestID _
									   And c.SampleType = pSampleType _
									 Select c).ToList()

				If (lstWSControlsDS.Count = 0) Then
					'There are not Controls requested for the Test/SampleType, then verify if there 
					'are Patient Samples requested for it
					Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
					lstWSPatientsDS = (From c In pWSResultDS.Patients _
										 Where c.SampleClass = "PATIENT" _
										   And c.TestType = pTestType _
										   And c.TestID = pTestID _
										   And c.SampleType = pSampleType _
										 Select c).ToList()

					If (lstWSPatientsDS.Count = 0) Then deleteCalibrator = True
					lstWSPatientsDS = Nothing
				End If
				lstWSControlsDS = Nothing
			Catch ex As Exception
				Dim myLogAcciones As New ApplicationLogManager()
				myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.VerifyCalibratorDeletion", EventLogEntryType.Error, False)
				'Throw ex  'Commented line RH 12/04/2012
				'Do prefer using an empty throw when catching and re-throwing an exception.
				'This is the best way to preserve the exception call stack.
				'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
				'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
				Throw
			End Try
			Return deleteCalibrator
		End Function

#End Region

#Region "TO TEST - NEW METHODS TO DIVIDE SAVING OF LIS PATIENT ORDER TESTS IN SEVERAL TRANSACTIONS"
        ''' <summary>
        ''' Process to add to the active WS all Patient Order Tests in the Saved WS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pWorkSessionResultDS">Typed Dataset WorkSessionResultDS containing all Order Tests (of all Sample Classes) that are included
        '''                                    in the active WS</param>
        ''' <param name="pSavedWSOrderTestsDS">Typed Dataset SavedWSOrderTestsDS </param>
        ''' <param name="pRerunLISMode">Current value of setting for allowed Rerun LIS Mode: BOTH, ONLY ANALYZER or ONLY LIS</param>
        ''' <param name="pISEModuleReady">Flag indicating if the ISE Module is ready</param>
        ''' <param name="pRejectedOTLISInfo">Typed DataSet OrderTestsLISInfoDS to return information of all AwosID that have been rejected</param>
        ''' <param name="pAutoWSCreationInProcess">Flag indicating if the function has been called by the process for automatic WS creation</param>
        ''' <param name="pRunningMode">When TRUE, it indicates the ManageRepetitions function the Analyzer is in RUNNING mode; otherwise it indicates
        '''                            the Analyzer is in STANDBY</param>
        ''' <param name="pMsgDateTime">Sending Date and Time of the LIS Message from which the Patient Order Test to process was extracted</param>
        ''' <returns>GlobalDataTO containing a typed DataSet with an integer value: 
        '''          0 = No Patient Sample was added; 
        '''          1 = Some Patient Samples were added, but there is not tube for any of them; 
        '''          2 = Some Patient Samples were added, and there is a tube for at least one of them</returns>
        ''' <remarks>
        ''' Created by:  TR 25/03/2013
        ''' Modified by: TR 29/04/2013 - Search in the active WS (pWsResultsDS.Patients) if there is an Order Test requested by LIS with the same 
        '''                              Specimen but for different SampleID. If the SpecimenID is linked to a different Patient in the WS, the 
        '''                              LIS request is rejected
        '''              SA 30/04/2013 - When an Order Test has been found in the WS for the same Patient/TestType/TestID/SampleType and the LIS 
        '''                              request is not rejected, increment variables notSelectedPatients or selectedPatients according the Status 
        '''                              of the found Order Test (notSelectedPatients when OPEN or selectedPatients in any other case)
        '''              SA 03/05/2013 - Validation of duplicate Specimen has to be done before validation of duplicate Test/SampleType to manage Reruns
        '''              AG 13/05/2013 - Added changes for rejecting all Order Tests in LIS Saved WS which Test has been deleted in BAx00 (field 
        '''                              DeletedTestFlag is TRUE)
        '''              SA 11/06/2013 - When load data in an ImportErrorLogDS, inform field LineText as "MessageID | AwosID" instead of inform specific fields MessageID
        '''                              and AwosID in the DS (due to these fields have been deleted). Call function ImportErrorsLogDelegate.Add to save data loaded in the 
        '''                              ImportErrorsLogDS in table twksImportErrorsLog (code is commented)
        '''              SA 01/08/2013 - Before beginning process to validate if the LIS Patient Order Test can be added to the active Work Session, call new function
        '''                              SearchOrdersBySpecimenID to verify if there is already a manual Order with SampleID = SpecimenID sent by LIS, and in this case,
        '''                              update fields PatientID/SampleID of the existing Order with the SampleID of the LIS Order Test, and add it to the existing Order
        '''                              (the StatFlag sent by LIS is ignored in this case)
        '''              SA 05/09/2013 - Added new parameter pAutoWSCreationInProcess to indicate when the function has been called by the process for automatic WS creation.
        '''                              In this case, the count of Selected or NotSelected Order Tests is executed only when a new STD or ISE Test have been added to 
        '''                              the WS (due to only that type of Tests generates executions).
        '''              SA 17/02/2014 - BT #1510 ==> Code for adding rejected Order Tests to the entry DS pRejectedOTLISInfoDS has been commented to avoid the sending 
        '''                                           of rejecting messages to LIS and reduce message traffic. Exception: the rejection due to LIS_DUPLICATE_SPECIMEN. 
        '''              XB 24/03/2014 - BT #1536 ==> Transactions become smaller to avoid timeout malfunctions
        '''              SA 28/03/2014 - BT #1556 ==> Changes to include error LIS_RERUN_WITH_DIF_SPECIMEN in the list of known errors that change the HasError flag 
        '''                                           of the GlobalDataTO to return to FALSE (to allow the deleting of the LIS SavedWS once it has been processed) 
        '''              AG 31/03/2014 - BT #1565 ==> Added new parameter pRunningMode (required for ManageRepetitions)
        '''              SA 01/04/2014 - BT #1564 ==> Added new parameter for the Date and Time of the LIS Message from which the Patient Order Test to process was
        '''                                           extracted, and inform it when calling function VerifyRerunOfLISPatientOT
        ''' </remarks>
        Public Function ProcessLISPatientOTs_NEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, pWorkSessionID As String, _
                                                 ByRef pWorkSessionResultDS As WorkSessionResultDS, ByVal pSavedWSOrderTestsDS As SavedWSOrderTestsDS, _
                                                 ByVal pRerunLISMode As String, pISEModuleReady As Boolean, ByRef pRejectedOTLISInfo As OrderTestsLISInfoDS, _
                                                 ByVal pAutoWSCreationInProcess As Boolean, ByVal pRunningMode As Boolean, ByVal pMsgDateTime As Date) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim selectedPatients As Integer = 0
                Dim notSelectedPatients As Integer = 0

                Dim myOrdersToUpdateDS As New OrderTestsDS
                Dim myImportErrorsLogDS As New ImportErrorsLogDS
                Dim myRepetitionsDelegate As New RepetitionsDelegate
                Dim myRepetitionsToAddDS As New WSRepetitionsToAddDS
                Dim myFinalOrderTestsLISInfoDS As New OrderTestsLISInfoDS
                Dim myOrderTestsLISInfoDelegate As New OrderTestsLISInfoDelegate
                Dim myImportErrorsLogRow As ImportErrorsLogDS.twksImportErrorsLogRow
                Dim myPatientOrderTestList As New List(Of SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
                Dim myOrderTestsLISInfoRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow
                Dim myPatientOTOnActiveWSList As New List(Of WorkSessionResultDS.PatientsRow)

                'On pSavedWSOrderTestsDS filter all patient order test (SampleClass = 'PATIENT')
                myPatientOrderTestList = (From a In pSavedWSOrderTestsDS.tparSavedWSOrderTests _
                                         Where a.SampleClass = "PATIENT" Select a).ToList()

                Dim countAdded As Boolean = True
                For Each SavedWSOTRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In myPatientOrderTestList
                    'If DeletedTestFlag is TRUE for the Order Test in theSaved WS, it means the Test or the Test/Sample Type 
                    'has been deleted; in this case, the AwosID is rejected
                    If (SavedWSOTRow.DeletedTestFlag) Then
                        'Add a row to a LISImportErrorsDS informing Message Identifier, AwosID, and ErrorCode.
                        myImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                        myImportErrorsLogRow.ErrorCode = GlobalEnumerates.Messages.LIMS_INVALID_TEST.ToString
                        myImportErrorsLogRow.LineText = SavedWSOTRow.SavedWSName & " | " & SavedWSOTRow.AwosID & " | " & SavedWSOTRow.TestName
                        myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myImportErrorsLogRow)
                        myImportErrorsLogDS.twksImportErrorsLog.AcceptChanges()
                    Else
                        'Verify if there is already a manual Order with SampleID = SpecimenID sent by LIS, and in this case, get the DS containing 
                        'the list of Orders for which fields PatientID / SampleID has to be updated with the SampleID of the LIS Order Test
                        myGlobalDataTO = SearchOrdersBySpecimenID_NEW(SavedWSOTRow, pWorkSessionResultDS, myOrdersToUpdateDS)
                        If (myGlobalDataTO.HasError) Then Exit For

                        'Search in the active WS if there are Order Tests requested by LIS with the same Specimen but for different SampleID
                        '(Validate DUPLICATE SPECIMEN)
                        myPatientOTOnActiveWSList = (From a In pWorkSessionResultDS.Patients _
                                                     Where a.SampleClass = "PATIENT" _
                                                     AndAlso a.LISRequest _
                                                     AndAlso a.SampleID <> SavedWSOTRow.SampleID _
                                                     AndAlso Not a.IsSpecimenIDNull _
                                                     AndAlso a.SpecimenID = SavedWSOTRow.SpecimenID).ToList()

                        If (myPatientOTOnActiveWSList.Count = 0) Then
                            'Search in the active WS, if there is an Order Test for the same SampleID, TestType, TestID and SampleType
                            myPatientOTOnActiveWSList = (From a In pWorkSessionResultDS.Patients Where a.SampleClass = "PATIENT" _
                                                      AndAlso a.SampleID = SavedWSOTRow.SampleID _
                                                      AndAlso a.TestType = SavedWSOTRow.TestType _
                                                      AndAlso a.TestID = SavedWSOTRow.TestID _
                                                      AndAlso a.SampleType = SavedWSOTRow.SampleType _
                                                       Select a).ToList()

                            If (myPatientOTOnActiveWSList.Count = 0) Then
                                'Add the Patient to the active WorkSession.
                                myGlobalDataTO = AddPatientOrderTestsFromLIS(pAnalyzerID, pWorkSessionID, SavedWSOTRow, pWorkSessionResultDS)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'According the value returned, increment the number of selected Patients (when TRUE) or the number of not selected Patients (when FALSE)
                                If (Not pAutoWSCreationInProcess OrElse (SavedWSOTRow.TestType <> "OFFS" AndAlso SavedWSOTRow.TestType <> "CALC")) Then
                                    If (CBool(myGlobalDataTO.SetDatos)) Then
                                        'Increment Selected Patients
                                        selectedPatients += 1
                                    Else
                                        'Increment Not Selected Patients
                                        notSelectedPatients += 1
                                    End If
                                End If
                            Else
                                'Found 
                                If (SavedWSOTRow.IsAwosIDNull) Then
                                    'The Order Test was not requested by LIS, but added becaused it is included in the 
                                    'Formula of a Calculated Test requested by LIS
                                    If (myPatientOTOnActiveWSList.First().IsCalcTestIDNull OrElse myPatientOTOnActiveWSList.First().CalcTestID = String.Empty) Then
                                        myPatientOTOnActiveWSList.First().CalcTestID &= SavedWSOTRow.CalcTestIDs

                                    ElseIf (Not myPatientOTOnActiveWSList.First().CalcTestID.Contains(SavedWSOTRow.CalcTestIDs)) Then
                                        'Merge values in wsOT.CalcTestID with values in savedOT.CalcTestIDs and update value of wsOT.CalcTestID
                                        myPatientOTOnActiveWSList.First().CalcTestID = MergeValues(SavedWSOTRow.CalcTestIDs, _
                                                                                                   myPatientOTOnActiveWSList.First().CalcTestID)
                                    End If

                                    If (myPatientOTOnActiveWSList.First().IsCalcTestNameNull OrElse myPatientOTOnActiveWSList.First().CalcTestName = String.Empty) Then
                                        myPatientOTOnActiveWSList.First().CalcTestName &= SavedWSOTRow.CalcTestNames

                                    ElseIf (Not myPatientOrderTestList.First().CalcTestNames.Contains(SavedWSOTRow.CalcTestNames)) Then
                                        'Merge values in wsOT.CalcTestName with values in savedOT.CalcTestNames and update value of wsOT.CalcTestName.
                                        myPatientOTOnActiveWSList.First().CalcTestName = MergeValues(SavedWSOTRow.CalcTestNames, _
                                                                                                     myPatientOTOnActiveWSList.First().CalcTestName)
                                    End If
                                    myPatientOTOnActiveWSList.First().LISRequest = True

                                    If (Not pAutoWSCreationInProcess) Then
                                        If (myPatientOTOnActiveWSList.First().OTStatus <> "OPEN") Then
                                            'Increment Selected Patients
                                            selectedPatients += 1
                                        Else
                                            'Increment Not Selected Patients
                                            notSelectedPatients += 1
                                        End If
                                    End If
                                Else
                                    If (myPatientOTOnActiveWSList.First().LISRequest) Then
                                        If (Not myPatientOTOnActiveWSList.First().IsAwosIDNull) Then
                                            'The existing Order Test was requested by LIS: validate if the requested Rerun can be added
                                            'BT #1564 - Inform parameter for the Message DateTime
                                            myGlobalDataTO = VerifyRerunOfLISPatientOT(myPatientOTOnActiveWSList.First(), SavedWSOTRow, pRerunLISMode, _
                                                                                       myFinalOrderTestsLISInfoDS, myRepetitionsToAddDS, pMsgDateTime)
                                        Else
                                            'If field LISRequest is TRUE but the AwosID is not informed, it means the Order Test was added due to 
                                            'it is needed for an Order Test of a Calculated Test requested by LIS. 
                                            If (Not myPatientOTOnActiveWSList.First().IsOrderTestIDNull) Then
                                                'The OrderTest exists in the active WS and it has been already saved in DB; execute the process of 
                                                'Verify Rerun of Manual Patient Order Test
                                                myGlobalDataTO = VerifyRerunOfManualPatientOT(myPatientOTOnActiveWSList.First(), SavedWSOTRow, pRerunLISMode, _
                                                                                              myFinalOrderTestsLISInfoDS, myRepetitionsToAddDS)
                                            Else
                                                'The LIS request for it is accepted, and LIS fields for the Order Test are updated with values received from LIS
                                                myPatientOTOnActiveWSList.First().BeginEdit()
                                                myPatientOTOnActiveWSList.First().AwosID = SavedWSOTRow.AwosID
                                                myPatientOTOnActiveWSList.First().SpecimenID = SavedWSOTRow.SpecimenID
                                                myPatientOTOnActiveWSList.First().ESPatientID = SavedWSOTRow.ESPatientID
                                                myPatientOTOnActiveWSList.First().ESOrderID = SavedWSOTRow.ESOrderID
                                                If (Not SavedWSOTRow.IsLISPatientIDNull) Then myPatientOTOnActiveWSList.First().LISPatientID = SavedWSOTRow.LISPatientID
                                                If (Not SavedWSOTRow.IsLISOrderIDNull) Then myPatientOTOnActiveWSList.First().LISOrderID = SavedWSOTRow.LISOrderID
                                                myPatientOTOnActiveWSList.First().EndEdit()
                                            End If
                                        End If
                                    Else
                                        'The same TestType/Test/SampleType exists in the active WS for the Patient, but was manually requested.
                                        myGlobalDataTO = VerifyRerunOfManualPatientOT(myPatientOTOnActiveWSList.First(), SavedWSOTRow, pRerunLISMode, _
                                                                                      myFinalOrderTestsLISInfoDS, myRepetitionsToAddDS)
                                    End If

                                    If (myGlobalDataTO.HasError) Then
                                        'Add a row to a LISImportErrorsDS informing Message Identifier, AwosID, and ErrorCode.
                                        myImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                                        myImportErrorsLogRow.ErrorCode = myGlobalDataTO.ErrorCode
                                        myImportErrorsLogRow.LineText = SavedWSOTRow.SavedWSName & " | " & SavedWSOTRow.AwosID

                                        'Add row to DS.
                                        myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myImportErrorsLogRow)
                                    Else
                                        If (Not pAutoWSCreationInProcess) Then
                                            If (myPatientOTOnActiveWSList.First().OTStatus <> "OPEN") Then
                                                'Increment Selected Patients
                                                selectedPatients += 1
                                            Else
                                                'Increment Not Selected Patients
                                                notSelectedPatients += 1
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            'If the SpecimenID is linked to a different Patient in the WS, the LIS request is rejected.
                            'Fill a row of OrderTestsLISInfoDS with following information from pSavedWSOrderTestDS.Row: 
                            'AwosID, SpecimenID, TestID, SampleType, ESPatientID, ESOrderID, SampleClass and StatFlagText..
                            myOrderTestsLISInfoRow = pRejectedOTLISInfo.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                            myOrderTestsLISInfoRow.AwosID = SavedWSOTRow.AwosID
                            myOrderTestsLISInfoRow.SpecimenID = SavedWSOTRow.SpecimenID
                            myOrderTestsLISInfoRow.TestType = SavedWSOTRow.TestType
                            myOrderTestsLISInfoRow.TestID = SavedWSOTRow.TestID
                            myOrderTestsLISInfoRow.SampleType = SavedWSOTRow.SampleType
                            myOrderTestsLISInfoRow.ESPatientID = SavedWSOTRow.ESPatientID
                            myOrderTestsLISInfoRow.ESOrderID = SavedWSOTRow.ESOrderID
                            myOrderTestsLISInfoRow.CheckLISValues = True

                            'Inform the SampleClass
                            If (SavedWSOTRow.SampleClass = "PATIENT" AndAlso Not SavedWSOTRow.ExternalQC) Then
                                myOrderTestsLISInfoRow.SampleClass = "patient"
                            ElseIf (SavedWSOTRow.SampleClass = "PATIENT" AndAlso SavedWSOTRow.ExternalQC) Then
                                myOrderTestsLISInfoRow.SampleClass = "QC"
                            End If

                            'Inform StatFlagText
                            If (SavedWSOTRow.StatFlag) Then
                                myOrderTestsLISInfoRow.StatFlagText = "stat"
                            Else
                                myOrderTestsLISInfoRow.StatFlagText = "normal"
                            End If

                            'Add row to DS. 
                            pRejectedOTLISInfo.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myOrderTestsLISInfoRow)

                            'Add a row to a LISImportErrorsDS informing Message Identifier, AwosID, and ErrorCode.
                            myImportErrorsLogRow = myImportErrorsLogDS.twksImportErrorsLog.NewtwksImportErrorsLogRow
                            myImportErrorsLogRow.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATE_SPECIMEN.ToString()
                            myImportErrorsLogRow.LineText = SavedWSOTRow.SavedWSName & " | " & SavedWSOTRow.AwosID
                            myImportErrorsLogDS.twksImportErrorsLog.AddtwksImportErrorsLogRow(myImportErrorsLogRow)
                        End If
                    End If
                Next

                'Validate if an error has happened and if it is one of the known errors (LIS_DUPLICATE_REQUEST, LIS_NOT_ALLOWED_RERUN)
                'before continuing with the process 
                'BT #1556 - Error LIS_RERUN_WITH_DIF_SPECIMEN included in this list of controlled errors
                If myGlobalDataTO.HasError AndAlso (myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.LIS_DUPLICATED_REQUEST.ToString() OrElse _
                                                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.LIS_NOT_ALLOWED_RERUN.ToString() OrElse _
                                                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.LIS_RERUN_WITH_DIF_SPECIMEN.ToString()) Then
                    'Set the HasError to false because the error is a controlled one
                    myGlobalDataTO.HasError = False
                End If

                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                        If (Not dbConnection Is Nothing) Then
                            'If there are accepted Reruns, process them
                            If (myRepetitionsToAddDS.twksWSRepetitionsToAdd.Count > 0) Then
                                'Process for each Repetition to Add
                                Dim rerunCreatedFlag As Boolean = False
                                For Each repetitionRow As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow In myRepetitionsToAddDS.twksWSRepetitionsToAdd.Rows
                                    'Add the new Rerun to the active WorkSession
                                    rerunCreatedFlag = False
                                    'AG 31/03/2014 - #1565 inform new parameter pRunningMode in the proper position
                                    Dim myLogAcciones As New ApplicationLogManager()
                                    myLogAcciones.CreateLogActivity("Launch Rerun !", "OrderTestsDelegate.ProcessLISPatientOTs_NEW", EventLogEntryType.Information, False)
                                    myGlobalDataTO = myRepetitionsDelegate.ManageRepetitions(dbConnection, pAnalyzerID, pWorkSessionID, _
                                                                                             repetitionRow.OrderTestID, repetitionRow.RerunNumber, _
                                                                                             rerunCreatedFlag, "", pRunningMode, True, "NONE", pISEModuleReady)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next
                            End If

                            If (Not myGlobalDataTO.HasError AndAlso myFinalOrderTestsLISInfoDS.twksOrderTestsLISInfo.Count > 0) Then
                                'Add LIS fields for all OrderTestID / RerunNumber in the DS
                                myGlobalDataTO = myOrderTestsLISInfoDelegate.Create(dbConnection, myFinalOrderTestsLISInfoDS, True)
                            End If

                            If (Not myGlobalDataTO.HasError AndAlso myOrdersToUpdateDS.twksOrderTests.Count > 0) Then
                                'Update all manual Orders having SampleID = SpecimenID sent by LIS...
                                Dim myOrdersDelegate As New OrdersDelegate
                                myGlobalDataTO = myOrdersDelegate.UpdatePatientSampleFields_NEW(dbConnection, myOrdersToUpdateDS)
                            End If

                            If (Not myGlobalDataTO.HasError AndAlso myImportErrorsLogDS.twksImportErrorsLog.Count > 0) Then
                                'Write the content of DataSet LISImportErrorDS to table twksImportErrorsLog
                                Dim myImportErrorLog As New ImportErrorsLogDelegate
                                myGlobalDataTO = myImportErrorLog.Add(Nothing, myImportErrorsLogDS)
                            End If
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                        'Add to the active WorkSession all Controls, Calibrators and Blanks needed for the added STD Test/SampleTypes 
                        'requested for Patient Samples
                        If (Not myGlobalDataTO.HasError) Then
                            'AddAllOrderTestsNeededByLIS in OrderTestsDelegate
                            myGlobalDataTO = AddAllNeededOrderTestsForLIS(pAnalyzerID, pWorkSessionResultDS)
                        End If

                        'Set the function result value
                        If (Not myGlobalDataTO.HasError) Then
                            If (selectedPatients = 0 AndAlso notSelectedPatients = 0) Then
                                myGlobalDataTO.SetDatos = 0
                            ElseIf (selectedPatients = 0 AndAlso notSelectedPatients > 0) Then
                                myGlobalDataTO.SetDatos = 1
                            Else
                                myGlobalDataTO.SetDatos = 2
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
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.ProcessLISPatientOTs_NEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search in the active WS if there are manual Orders having the SpecimenID sent by LIS as SampleID, and in this case, returns them to update later the 
        ''' field PatientID/SampleID with the value of field SampleID in the LIS Saved WS  
        ''' </summary>
        ''' <param name="pSavedLISRow">Row of a typed DataSet SavedWSOrderTestsDS containing all data of the LIS Awos being processed</param>
        ''' <param name="pWorkSessionResultDS">Typed DataSet WorkSessionResultDS containing all Patient Samples in the active WorkSession</param>
        ''' <param name="pOrderTestsToUpdateDS">Typed DataSet OrderTestsDS to load the list of Orders from the active WS for which field PatientID/SampleID has to be updated</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 11/03/2014 - BT #1536 ==> Removed the entry parameter for the DB Connection. Added new OrderTestsDS parameter to add the list of 
        '''                                           Orders for which the PatientID/SampleID has to be changed. Removed the call to function UpdatePatientSampleFields
        '''                                           in OrdersDelegate
        ''' </remarks>
        Private Function SearchOrdersBySpecimenID_NEW(ByVal pSavedLISRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow, ByRef pWorkSessionResultDS As WorkSessionResultDS, _
                                                      ByRef pOrderTestsToUpdateDS As OrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Search if there are MANUAL Orders in the active Work Session with SampleID = SpecimenID sent by LIS
                Dim mySpecimenID As String = pSavedLISRow.SpecimenID
                Dim myPatientOTOnActiveWSList As List(Of WorkSessionResultDS.PatientsRow) = (From a As WorkSessionResultDS.PatientsRow In pWorkSessionResultDS.Patients _
                                                                                            Where a.SampleClass = "PATIENT" _
                                                                                          AndAlso a.SampleID = mySpecimenID _
                                                                                          AndAlso a.SampleIDType = "MAN").ToList()

                If (myPatientOTOnActiveWSList.Count > 0) Then
                    Dim myOrderToUpdateRow As OrderTestsDS.twksOrderTestsRow

                    'For all Order Tests belonging to Orders with SampleID = SpecimenID, update fields PatientID/SampleID with the value sent by LIS
                    For Each orderTest As WorkSessionResultDS.PatientsRow In myPatientOTOnActiveWSList
                        orderTest.BeginEdit()
                        orderTest.SampleID = pSavedLISRow.SampleID
                        orderTest.SampleIDType = pSavedLISRow.PatientIDType
                        orderTest.EndEdit()

                        'Add a row in the OrderTestsDS to return
                        myOrderToUpdateRow = pOrderTestsToUpdateDS.twksOrderTests.NewtwksOrderTestsRow()
                        myOrderToUpdateRow.OrderID = orderTest.OrderID
                        myOrderToUpdateRow.SampleID = pSavedLISRow.SampleID
                        myOrderToUpdateRow.PatientIDType = pSavedLISRow.PatientIDType
                        myOrderToUpdateRow.SampleType = pSavedLISRow.SampleType
                        pOrderTestsToUpdateDS.twksOrderTests.AddtwksOrderTestsRow(myOrderToUpdateRow)
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestsDelegate.SearchOrderBySpecimenID_NEW", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO TEST - NEW METHODS TO DIVIDE ADD WS IN SEVERAL DB TRANSACTIONS"
        ''' <summary>
        ''' Update value of field CreationOrder for all Order Tests requested by an external LIS system but still not positioned (these with
        ''' OrderTestStatus = OPEN
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLstOpenLISOTs">List of WorkSessionResultDS.PatientsRow containing all LIS Order Tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 19/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions
        ''' </remarks>
        Public Function UpdateCreationOrderForOpenLISOTs(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLstOpenLISOTs As List(Of WorkSessionResultDS.PatientsRow)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksOrderTestsDAO As New TwksOrderTestsDAO
                        resultData = mytwksOrderTestsDAO.UpdateCreationOrderForOpenLISOTs(dbConnection, pLstOpenLISOTs)

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
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderTestsDelegate.UpdateCreationOrderForOpenLISOTs", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE- OLD FUNCTIONS"
		''' <summary>
		''' Get the Identifier of the Test of the informed Order Test
		''' </summary>
		''' <param name="pDBConnection">Open DB Connection</param>
		''' <param name="pOrderTestID">Identifier of the Order Test</param>
		''' <returns></returns>
		''' <remarks>
		''' PENDING: WHY THIS METHOD IS USED INSTEAD THE GetOrderTest??
		''' </remarks>
		Public Function GetTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
			Dim resultData As New GlobalDataTO
			Dim dbConnection As New SqlClient.SqlConnection

			Try
				resultData = DAOBase.GetOpenDBConnection(pDBConnection)
				If (Not resultData.HasError) Then
					dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
					If (Not dbConnection Is Nothing) Then
						Dim mytwksOrderTests As New TwksOrderTestsDAO
						resultData = mytwksOrderTests.GetTestID(pDBConnection, pOrderTestID)
					End If
				End If
			Catch ex As Exception
				resultData.HasError = True
				resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

				Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderTestDelegate.GetTestID", EventLogEntryType.Error, False)
			Finally
				If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
			End Try
			Return resultData
		End Function

		' ''' <summary>
		' ''' Delete the informed Order Test
		' ''' </summary>
		' ''' <param name="pDBConnection">Open DB Connection</param>
		' ''' <param name="pOrderTestID">Order Identifier</param>
		' ''' <returns>GlobalDataTO containing success/error information</returns>
		' ''' <remarks>
		' ''' Created by:  SG 15/03/2013
		' ''' Modified by: TR 20/03/2013 -Change the function name.
		' ''' </remarks>
		'Public Function DeleteByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
		'    Dim resultData As GlobalDataTO = Nothing
		'    Dim dbConnection As SqlClient.SqlConnection = Nothing

		'    Try
		'        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
		'        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
		'            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
		'            If (Not dbConnection Is Nothing) Then

		'                resultData = MyClass.GetOrderTest(dbConnection, pOrderTestID)
		'                If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
		'                    Dim myOrderTestsDS As OrderTestsDS = TryCast(resultData.SetDatos, OrderTestsDS)
		'                    If myOrderTestsDS.twksOrderTests.Rows.Count > 0 Then
		'                        'Delete relation between Order Test and the active WorkSession (WSOrderTest table)
		'                        Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
		'                        resultData = myWSOrderTestsDelegate.DeleteByOrderTestID(dbConnection, pOrderTestID)

		'                        If (Not resultData.HasError) Then
		'                            Dim myOrderTestDAO As New TwksOrderTestsDAO()
		'                            resultData = myOrderTestDAO.DeleteByOrderTestID(dbConnection, pOrderTestID)
		'                        End If

		'                        If (Not resultData.HasError) Then
		'                            'When the Database Connection was opened locally, then the Commit is executed
		'                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
		'                        Else
		'                            'When the Database Connection was opened locally, then the Rollback is executed
		'                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
		'                        End If
		'                    End If
		'                End If
		'            End If
		'        End If
		'    Catch ex As Exception
		'        'When the Database Connection was opened locally, then the Rollback is executed
		'        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

		'        resultData = New GlobalDataTO()
		'        resultData.HasError = True
		'        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

		'        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))" , "OrderTestsDelegate.DeleteByOrderTestID", EventLogEntryType.Error, False)
		'    Finally
		'        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
		'    End Try
		'    Return resultData
		'End Function

#End Region
    End Class
End Namespace

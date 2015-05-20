Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Namespace Biosystems.Ax00.Core.Entities.WorkSession
    ''' <summary>
    '''     This class handles the worksession sorting, by processing execution times and contaminations
    ''' </summary>
    ''' <remarks></remarks>
    Public Class WSExecutionsSorter

        Sub New(executions As ExecutionsDS, activeAnalyzer As String)
            Me.Executions = executions
            Me.activeAnalyzer = activeAnalyzer
        End Sub

#Region "Public properties"

        ''' <summary>
        '''     This property contains the WSexecutions dataset.
        ''' </summary>
        Public Property Executions As ExecutionsDS

        ''' <summary>
        ''' This property is used to identify the current analyzer in order to know how to perform internal calculations
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property activeAnalyzer As String = ""

#End Region

#Region "attributes"

        Dim highContaminationPersitance As Integer
        Dim allPossibleSampleClasses As String() = {"BLANK", "CALIB", "CTRL", "PATIENT"}
        Dim allPossibleSampleTypes As IEnumerable(Of String)
        Dim ExecutionsStatValues As IEnumerable(Of Boolean)
        Dim ExecutionsSampleClasses As IEnumerable(Of String)
        Dim ExecutionsSampleTypes As IEnumerable(Of String)
        Dim contaminationsDataDS As ContaminationsDS = Nothing

#End Region

#Region "Public methods"

        ''' <summary>
        '''     This method will sort the twksWSExecutions datatable in the ExecutionsDS dataset reducing contaminations as much as
        '''     possible. This sorting is done by order
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>True if the sort could be performed without errors</returns>
        ''' <remarks>After calling this method, the ExecutionsDS property will contain sorted results</remarks>
        Public Function SortByContamination(ByVal pDBConnection As SqlConnection) As Boolean
            Dim dbConnection As SqlConnection = Nothing
            Dim ReturnData As Boolean = True

            Try
                Dim connection = GetSafeOpenDBConnection(pDBConnection)
                If (Not connection.HasError AndAlso Not connection.SetDatos Is Nothing) Then
                    dbConnection = connection.SetDatos
                    If (Not dbConnection Is Nothing) Then

                        InitializeAttributes(dbConnection)

                        Dim returnDS As New ExecutionsDS
                        For Each StatFlag In ExecutionsStatValues
                            For Each SampleClass In allPossibleSampleClasses
                                Dim sampleClassString = SampleClass
                                'ElementID es el ID del tipo de cosa. ID del calibrador, ID del blanco (-1) y en caso de no ser blancos y calibradores, es el number de row en pacientes dentro del grid de tests... ??
                                Dim elements =
                                        (From wse In Executions.twksWSExecutions Where wse.SampleClass = sampleClassString
                                        Select wse.ElementID Distinct)
                                For Each elementID In elements
                                    returnDS = AppendExecutionsIntoResults(dbConnection, SampleClass, StatFlag, elementID, returnDS)
                                Next elementID
                            Next SampleClass
                        Next StatFlag
                        Executions = returnDS
                        ReturnData = True
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
                ReturnData = False

            Finally
                If pDBConnection Is Nothing And dbConnection IsNot Nothing Then
                    Try : dbConnection.Close()
                    Catch
                    End Try
                End If
            End Try

            Return ReturnData
        End Function

        ''' <summary>
        '''     This method will sort the twksWSExecutions datatable in the ExecutionsDS dataset reducing contaminations as much as
        '''     possible. This sorting is done by "group time"", in blanks and callibrators, controls, etc..
        ''' </summary>
        ''' <returns>True if the sort could be performed without errors</returns>
        ''' <remarks>After calling this method, the ExecutionsDS property will contain sorted results</remarks>
        Public Function SortByElementGroupTime() As Boolean 'ByVal Executions As ExecutionsDS) As GlobalDataTO
            Dim returnDataSet As New ExecutionsDS
            Dim success As Boolean = False

            Try
                'Dim qOrders As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
                Dim index = 0

                While index < Executions.twksWSExecutions.Rows.Count
                    Dim statFlag = Executions.twksWSExecutions(index).StatFlag
                    Dim sampleClass = Executions.twksWSExecutions(index).SampleClass

                    Dim qOrders = (From wse In Executions.twksWSExecutions _
                            Where wse.StatFlag = statFlag AndAlso wse.SampleClass = sampleClass _
                            Select wse)

                    index += qOrders.Count

                    If sampleClass <> "PATIENT" Then
                        OrderByExecutionTime(qOrders, returnDataSet)
                    Else
                        'When SampleClass = 'PATIENT' do not sort
                        For Each wse In qOrders
                            returnDataSet.twksWSExecutions.ImportRow(wse)
                        Next
                    End If
                End While

                Executions = returnDataSet

                success = True

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)

            End Try

            Return success
        End Function

        ''' <summary>
        '''     This method will sort the twksWSExecutions datatable in the ExecutionsDS dataset reducing contaminations as much as
        '''     possible. This sort takes into account contaminations between groups.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>True if the sort could be performed without errors</returns>
        ''' <remarks>After calling this method, the ExecutionsDS property will contain sorted results</remarks>
        Public Function SortByGroupContamination(ByVal pDBConnection As SqlClient.SqlConnection) As Boolean

            Dim success = False
            Dim pExecutions = Executions

            'Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim bestContaminationNumber As Integer = Integer.MaxValue
            'Dim currentContaminationNumber As Integer
            'Dim contaminationsDataDS As ContaminationsDS = Nothing

            Try
                Dim connection = DAOBase.GetSafeOpenDBConnection(pDBConnection)
                'If (connection.SetDatos IsNot Nothing) Then
                dbConnection = connection.SetDatos
                If (Not dbConnection Is Nothing) Then
                    'Get all R1 Contaminations 

                    InitializeAttributes(dbConnection)
                    Dim Stats() As Boolean = {True, False}

                    Dim stdOrderTestsCount As Integer = 0

                    Dim returnDS As New ExecutionsDS
                    Dim previousElementLastReagentID As Integer = -1
                    Dim PreviousReagentsIDList As New List(Of Integer) 'List of previous reagents sent before the current previousElementLastReagentID, 
                    '                                                   remember this information in order to check the high contamination persistance
                    '                                                   (One Item for each different OrderTest)

                    Dim previousElementLastMaxReplicates As Integer = 1
                    Dim previousOrderTestMaxReplicatesList As New List(Of Integer) 'AG 19/12/2011 - Same item number as previous list, indicates the replicate number for each item in previous list

                    Dim OrderContaminationNumber As Integer = 0

                    For Each StatFlag In Stats
                        If ExecutionsStatValues.Contains(StatFlag) Then

                            For Each SampleClass In allPossibleSampleClasses
                                If ExecutionsSampleClasses.Contains(SampleClass) Then

                                    'AG 27/04/2012 AG + RH - Search the elementid for each sampleClass (the elementid code can be repeated from different sampleclasses)
                                    'Dim Elements = (From wse In pExecutions.twksWSExecutions _
                                    '              Select wse.ElementID Distinct).ToList()
                                    Dim Elements = (From wse In pExecutions.twksWSExecutions _
                                                    Where wse.SampleClass = SampleClass _
                                                    Select wse.ElementID Distinct).ToList()
                                    'AG 27/04/2012

                                    For Each elementID In Elements
                                        Dim ID = elementID
                                        Dim Stat As Boolean = StatFlag
                                        Dim SClass As String = SampleClass

                                        For Each sortedSampleType In allPossibleSampleTypes
                                            'AG 30/08/2012 - add OrElse differentSampleTypeValues.Contains("") because when create a ws only with blanks there not exists sampletype!!
                                            If ExecutionsSampleTypes.Contains(sortedSampleType) OrElse _
                                               (ExecutionsSampleClasses.Count = 1 AndAlso ExecutionsSampleClasses.Contains("BLANK")) Then

                                                Dim SType As String = sortedSampleType

                                                Dim StandardAndIseOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) 'All test type order tests
                                                Dim StandardOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) = Nothing  'Only STD test order tests

                                                'NEW: When a patient or ctrl has Ise & std test executions the ISE executions are the first
                                                If SClass = "PATIENT" OrElse SClass = "CTRL" Then 'Apply OrderBy sample type order {"SER", "URI", "PLM", "WBL", "CSF"} (case one PATIENT with several sample types) + execution type
                                                    StandardAndIseOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                     Where wse.StatFlag = Stat AndAlso _
                                                                     wse.SampleClass = SClass AndAlso _
                                                                     wse.SampleType = SType AndAlso _
                                                                     wse.ElementID = ID _
                                                                     Select wse Order By wse.ExecutionType).ToList()

                                                    If StandardAndIseOrderTests.Count > 0 Then
                                                        'Look for the STD orderTests
                                                        StandardOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                 Where wse.StatFlag = Stat AndAlso _
                                                                 wse.SampleClass = SClass AndAlso _
                                                                 wse.SampleType = SType AndAlso _
                                                                 wse.ElementID = ID AndAlso _
                                                                 wse.ExecutionType = "PREP_STD" _
                                                                 Select wse).ToList()
                                                        stdOrderTestsCount = StandardOrderTests.Count
                                                    Else
                                                        stdOrderTestsCount = 0
                                                    End If

                                                Else 'Do not apply OrderBy & do not take care about sample type order inside the same SAMPLE
                                                    StandardAndIseOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                             Where wse.StatFlag = Stat AndAlso _
                                                                             wse.SampleClass = SClass AndAlso _
                                                                             wse.ElementID = ID _
                                                                             Select wse).ToList()

                                                    If StandardAndIseOrderTests.Count > 0 Then
                                                        'Look for the STD orderTests
                                                        StandardOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                      Where wse.StatFlag = Stat AndAlso _
                                                                      wse.SampleClass = SClass AndAlso _
                                                                      wse.ElementID = ID AndAlso _
                                                                      wse.ExecutionType = "PREP_STD" _
                                                                      Select wse).ToList()
                                                        stdOrderTestsCount = StandardOrderTests.Count
                                                    Else
                                                        stdOrderTestsCount = 0
                                                    End If
                                                End If


                                                If stdOrderTestsCount > 0 Then
                                                    OrderContaminationNumber = 0

                                                    'Only move the contaminated tests, so no changes into first element tests and also when the
                                                    'last reagent on previous element do not contaminates the first reagent in current element
                                                    If previousElementLastReagentID <> -1 Then
                                                        Dim pendingOrderTestInNewElement = (From wse In StandardOrderTests _
                                                                                                    Where wse.ExecutionStatus = "PENDING" _
                                                                                                    Select wse).ToList()
                                                        If pendingOrderTestInNewElement.Count > 0 Then
                                                            'Search contamination between Elements
                                                            Dim existContamination = (From wse In contaminationsDataDS.tparContaminations _
                                                                                        Where wse.ReagentContaminatorID = previousElementLastReagentID _
                                                                                        AndAlso wse.ReagentContaminatedID = pendingOrderTestInNewElement(0).ReagentID _
                                                                                        Select wse).ToList()

                                                            If existContamination.Count > 0 Then
                                                                'Calculate the contaminations inside the current Element + 1 (contamination between last and next elementID)
                                                                OrderContaminationNumber = 1 + ExecutionsDelegate.GetContaminationNumber(contaminationsDataDS, StandardOrderTests, highContaminationPersitance)

                                                            ElseIf highContaminationPersitance > 0 Then
                                                                'If no LOW contamination exists between consecutive executions take care about the previous due the high contamination
                                                                'has persistance > 1 (Evaluate only when last OrderTest has MaxReplicates < pHighContaminationPersistance)
                                                                If previousElementLastMaxReplicates < highContaminationPersitance Then

                                                                    'Evaluate if the last reagents sent contaminates (HIGH contamination) the first pending to be sent
                                                                    For highIndex As Integer = PreviousReagentsIDList.Count - highContaminationPersitance To PreviousReagentsIDList.Count - 2
                                                                        Dim auxHighIndex = highIndex
                                                                        If auxHighIndex >= 0 Then 'Avoid overflow
                                                                            existContamination = (From wse In contaminationsDataDS.tparContaminations _
                                                                                              Where wse.ReagentContaminatorID = PreviousReagentsIDList(auxHighIndex) _
                                                                                              AndAlso wse.ReagentContaminatedID = pendingOrderTestInNewElement(0).ReagentID _
                                                                                              AndAlso Not wse.IsWashingSolutionR1Null _
                                                                                              Select wse).ToList()
                                                                            If existContamination.Count > 0 Then
                                                                                Exit For
                                                                            End If
                                                                        End If
                                                                    Next

                                                                    'If previous step has no contamination then evaluate if the LAST reagent sent contaminates (HIGH contamination) the second, third,... reagent pending to be sent
                                                                    If existContamination.Count = 0 Then
                                                                        Dim newPendingOrderTestMaxReplicates As Integer = 1
                                                                        newPendingOrderTestMaxReplicates = (From wse In pendingOrderTestInNewElement _
                                                                                                            Select wse.ReplicateNumber).Max
                                                                        If newPendingOrderTestMaxReplicates < highContaminationPersitance Then
                                                                            For i = 1 To highContaminationPersitance - 1
                                                                                Dim aux_i = i
                                                                                If aux_i <= pendingOrderTestInNewElement.Count - 1 Then 'Avoid overflow
                                                                                    existContamination = (From wse In contaminationsDataDS.tparContaminations _
                                                                                                      Where wse.ReagentContaminatorID = PreviousReagentsIDList(PreviousReagentsIDList.Count - 1) _
                                                                                                      AndAlso wse.ReagentContaminatedID = pendingOrderTestInNewElement(aux_i).ReagentID _
                                                                                                      AndAlso Not wse.IsWashingSolutionR1Null _
                                                                                                      Select wse).ToList()
                                                                                    If existContamination.Count > 0 Then
                                                                                        Exit For
                                                                                    End If
                                                                                End If
                                                                            Next
                                                                        End If
                                                                    End If

                                                                    If existContamination.Count > 0 Then
                                                                        'Calculate the contaminations inside the current Element + 1 (contamination between last and next elementID)
                                                                        OrderContaminationNumber = 1 + ExecutionsDelegate.GetContaminationNumber(contaminationsDataDS, StandardOrderTests, highContaminationPersitance)
                                                                    End If

                                                                End If 'If previousElementLastMaxReplicates < highContaminationPersitance Then
                                                            End If ' If existContamination.Count > 0 Then

                                                        End If 'If pendingOrderTestInNewElement.Count > 0 Then
                                                    End If 'If previousElementLastReagentID <> -1 Then

                                                    ManageContaminations(dbConnection, returnDS, StandardOrderTests, StandardAndIseOrderTests, OrderContaminationNumber, PreviousReagentsIDList, previousOrderTestMaxReplicatesList)

                                                    'AG 07/11/2011 - search the last reagentID of the current Element before change the ElementID
                                                    StandardOrderTests = (From wse In returnDS.twksWSExecutions _
                                                                    Where wse.StatFlag = Stat AndAlso _
                                                                    wse.SampleClass = SClass AndAlso _
                                                                    wse.ElementID = ID AndAlso _
                                                                    wse.ExecutionStatus = "PENDING" _
                                                                    Select wse).ToList()

                                                    If StandardOrderTests.Count > 0 Then
                                                        'AG 19/12/2011 - Inform the list of reagents and replicates using the executions of the last element group
                                                        'The last reagentID used has the higher indexes
                                                        Dim maxReplicates As Integer
                                                        For item = 0 To StandardOrderTests.Count - 1
                                                            Dim itemIndex = item
                                                            maxReplicates = (From wse In returnDS.twksWSExecutions _
                                                                                                Where wse.OrderTestID = StandardOrderTests(itemIndex).OrderTestID _
                                                                                                Select wse.ReplicateNumber).Max

                                                            If PreviousReagentsIDList.Count = 0 Then
                                                                PreviousReagentsIDList.Add(StandardOrderTests(itemIndex).ReagentID)
                                                                previousOrderTestMaxReplicatesList.Add(maxReplicates)

                                                                'When reagent changes
                                                            ElseIf PreviousReagentsIDList(PreviousReagentsIDList.Count - 1) <> StandardOrderTests(itemIndex).ReagentID Then
                                                                PreviousReagentsIDList.Add(StandardOrderTests(itemIndex).ReagentID)
                                                                previousOrderTestMaxReplicatesList.Add(maxReplicates)
                                                            End If

                                                            If itemIndex = StandardOrderTests.Count - 1 Then
                                                                previousElementLastReagentID = StandardOrderTests(itemIndex).ReagentID
                                                                previousElementLastMaxReplicates = maxReplicates
                                                            End If
                                                            'AG 19/12/2011
                                                        Next
                                                    Else
                                                        'Do nothing, the sentence previousElementLastReagentID = -1 is not allowed due
                                                        'WS could contain a Element LOCKED completely
                                                    End If
                                                    'AG 07/11/2011
                                                Else
                                                    'AG 14/12/2011 - Different test types
                                                    For Each wse In StandardAndIseOrderTests
                                                        returnDS.twksWSExecutions.ImportRow(wse)
                                                    Next
                                                    'AG 14/12/2011
                                                End If

                                                If SClass <> "PATIENT" AndAlso SClass <> "CTRL" Then Exit For 'For blank, calib do not take care about the sample type inside the same SAMPLE

                                                'AG 20/02/2014 - #1514
                                                StandardAndIseOrderTests = Nothing
                                                StandardOrderTests = Nothing
                                                'AG 20/02/2014 - #1514

                                            End If
                                        Next 'For Each mySampleType

                                    Next 'For each elementID


                                End If
                            Next 'For each SampleClass

                        End If
                    Next 'For each StatFlag

                    'resultData.SetDatos = returnDS
                    Executions = returnDS
                    success = True
                    'AG 20/02/2014 - #1514
                    ExecutionsStatValues = Nothing
                    ExecutionsSampleClasses = Nothing
                    ExecutionsSampleTypes = Nothing
                    PreviousReagentsIDList = Nothing
                    previousOrderTestMaxReplicatesList = Nothing
                    'AG 20/02/2014 - #1514
                    'End If

                    'End If
                End If

            Catch ex As Exception
                'resultData = New GlobalDataTO()
                'resultData.HasError = True
                'resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                'resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex) '.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.SortWSExecutionsByElementGroupContaminationNew", EventLogEntryType.Error, False)

            End Try
            'AG 19/02/2014 - #1514
            bestResult = Nothing
            currentResult = Nothing
            'AG 19/02/2014 - #1514

            Return success
        End Function




#End Region

#Region "Private methods"

        Private Sub InitializeAttributes(dbconnection As SqlConnection)
            highContaminationPersitance =
                SwParametersDelegate.ReadIntValue(dbconnection, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS,
                                                  Nothing).SetDatos

            allPossibleSampleClasses = {"BLANK", "CALIB", "CTRL", "PATIENT"}
            allPossibleSampleTypes = GetSampleTypesStringsCollection(dbconnection)

            'Different Stat, SampleClasses and SampleTypes in WorkSession
            ExecutionsStatValues =
                (From wse In Executions.twksWSExecutions Select wse.StatFlag Distinct Order By StatFlag Ascending)
            ExecutionsSampleClasses = (From wse In Executions.twksWSExecutions Select wse.SampleClass Distinct)
            ExecutionsSampleTypes = (From wse In Executions.twksWSExecutions Select wse.SampleType Distinct)

            'Get contaminationsDataDS
            Dim contaminationsByType = ContaminationsDelegate.GetContaminationsByType(dbconnection, "R1")
            If (Not contaminationsByType.HasError AndAlso Not contaminationsByType.SetDatos Is Nothing) Then
                contaminationsDataDS = DirectCast(contaminationsByType.SetDatos, ContaminationsDS)
            End If
        End Sub

        Private Function AppendExecutionsIntoResults(ByVal dbConnection As SqlConnection, ByVal sampleClass As String,
                                                     ByVal Stat As Boolean, ByVal ID As Integer,
                                                     ByVal returnDS As ExecutionsDS) As ExecutionsDS
            'Dim stdExecutionTypeOrderTestsCount As Integer

            For Each sortedSampleType In allPossibleSampleTypes
                'AG 30/08/2012 - add OrElse differentSampleTypeValues.Contains("") because when create a ws only with blanks there not exists sampletype!!
                If ExecutionsSampleTypes.Contains(sortedSampleType) OrElse
                   (ExecutionsSampleClasses.Count = 1 AndAlso ExecutionsSampleClasses.Contains("BLANK")) Then

                    Dim sampleType As String = sortedSampleType

                    Dim allTestTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow) _
                    'All test type order tests executions
                    Dim standardExecutionTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow) = Nothing _
                    'Only STD test order tests executions

                    'NEW: When a patient OR CTRL has Ise & std test executions the ISE executions are the first
                    If sampleClass = "PATIENT" OrElse sampleClass = "CTRL" Then _
                        'Apply OrderBy sample type order {"SER", "URI", "PLM", "WBL", "CSF"} (case one PATIENT with several sample types) + execution type

                        allTestTypeOrderTests = GetOrderTestsByExecutionType(sampleClass, Stat, sampleType, ID)
                        standardExecutionTypeOrderTests = GetStandardExecutionTypeOrderTestsForPatients(sampleClass, Stat,
                                                                                                        sampleType, ID)

                    Else
                        allTestTypeOrderTests = GetOrderTestsByReadingCycle(sampleClass, Stat, ID)
                        standardExecutionTypeOrderTests =
                            GetStandardExecutionTypeOrderTestsForBlanksAndCallibrators(sampleClass, Stat, ID)

                    End If

                    Dim orderContaminationNumber = 0
                    If standardExecutionTypeOrderTests.Count > 0 Then
                        orderContaminationNumber = ExecutionsDelegate.GetContaminationNumber(contaminationsDataDS,
                                                                                             standardExecutionTypeOrderTests,
                                                                                             highContaminationPersitance)
                    End If

                    returnDS = ManageContaminations(dbConnection, returnDS, standardExecutionTypeOrderTests,
                                                    allTestTypeOrderTests, orderContaminationNumber)

                    If sampleClass <> "PATIENT" AndAlso sampleClass <> "CTRL" Then Exit For _
                    'For blank, calib do not take care about the sample type inside the same SAMPLE

                End If
            Next 'For each mySampleType
            Return returnDS
        End Function

        Private Function GetStandardExecutionTypeOrderTestsForBlanksAndCallibrators(ByVal sampleClass As String,
                                                                                    ByVal Stat As Boolean,
                                                                                    ByVal ID As Integer) _
            As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
            Dim standardExecutionTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

            standardExecutionTypeOrderTests = (From wse In Executions.twksWSExecutions _
                Where wse.StatFlag = Stat AndAlso
                      wse.SampleClass = sampleClass AndAlso
                      wse.ElementID = ID AndAlso
                      wse.ExecutionType = "PREP_STD" _
                Select wse)
            Return standardExecutionTypeOrderTests
        End Function

        Private Function GetStandardExecutionTypeOrderTestsForPatients(ByVal sampleClass As String, ByVal Stat As Boolean,
                                                                       ByVal sampleType As String, ByVal ID As Integer) _
            As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
            Dim standardExecutionTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

            standardExecutionTypeOrderTests = (From wse In Executions.twksWSExecutions _
                Where wse.StatFlag = Stat AndAlso
                      wse.SampleClass = sampleClass AndAlso
                      wse.SampleType = sampleType AndAlso
                      wse.ElementID = ID AndAlso
                      wse.ExecutionType = "PREP_STD" _
                Select wse)
            Return standardExecutionTypeOrderTests
        End Function

        Private Function GetOrderTestsByExecutionType(ByVal sampleClass As String, ByVal Stat As Boolean,
                                                      ByVal sampleType As String, ByVal ID As Integer) _
            As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
            Dim allTestTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

            allTestTypeOrderTests = (From wse In Executions.twksWSExecutions _
                Where wse.StatFlag = Stat AndAlso
                      wse.SampleClass = sampleClass AndAlso
                      wse.SampleType = sampleType AndAlso
                      wse.ElementID = ID _
                Select wse Order By wse.ExecutionType)
            Return allTestTypeOrderTests
        End Function

        Private Function ManageContaminations(ByVal pDbConnection As SqlConnection,
                                              ByVal executionsDS As ExecutionsDS,
                                              ByVal StandardOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow),
                                              ByVal AllTestTypeOrderTests As  _
                                                 IEnumerable(Of ExecutionsDS.twksWSExecutionsRow),
                                              ByVal OrderContaminationNumber As Integer,
                                              Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                                              Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing) _
            As ExecutionsDS

            Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
            'Dim bestContaminationNumber As Integer = Integer.MaxValue
            Dim currentContaminationNumber As Integer

            If (OrderContaminationNumber > 0) Then

                currentContaminationNumber = OrderContaminationNumber
                currentResult = StandardOrderTests.ToList()
                bestResult = ExecutionsDelegate.ManageContaminationsForRunningAndStatic(False, activeAnalyzer, pDbConnection, contaminationsDataDS,
                                                                                        currentResult, highContaminationPersitance, currentContaminationNumber,
                                                                                        pPreviousReagentID, pPreviousReagentIDMaxReplicates)

                Dim stdPrepFlag As Boolean = False
                For Each wse In AllTestTypeOrderTests
                    If wse.ExecutionType <> "PREP_STD" Then
                        executionsDS.twksWSExecutions.ImportRow(wse)
                    ElseIf Not stdPrepFlag Then
                        stdPrepFlag = True
                        'Add all std test executions using bestResult
                        For Each wseStdTest In bestResult
                            executionsDS.twksWSExecutions.ImportRow(wseStdTest)
                        Next
                    End If
                Next
            Else
                For Each wse In AllTestTypeOrderTests
                    executionsDS.twksWSExecutions.ImportRow(wse)
                Next
            End If
            Return executionsDS
        End Function

        Private Sub OrderByExecutionTime(ByVal executionsToOrder As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow),
                                         ByRef returnDS As ExecutionsDS)

            Dim executionsList = executionsToOrder.ToList()
            While executionsList.Count > 0

                Dim wseMaxReadingCycle = (From wse In executionsList
                        Order By wse.ExecutionStatus Descending, wse.ReadingCycle Descending
                        Select wse).First

                'We need to calculate all items at once! Do not use IEnumerable here or a "collection modified" exception will happen
                Dim wseSelected =
                        (From wse In executionsList Where wse.ElementID = wseMaxReadingCycle.ElementID Select wse).ToArray

                For Each wse In wseSelected
                    returnDS.twksWSExecutions.ImportRow(wse)
                    executionsList.Remove(wse)
                Next

            End While
        End Sub

        Private Function GetSampleTypesStringsCollection(ByVal dbConnection As SqlConnection) As IEnumerable(Of String)

            Dim sampleTypesTable = MasterDataDelegate.GetSampleTypesDataTable(dbConnection)
            Dim lista = (From sampleType In sampleTypesTable.SetDatos Select sampleType.ItemID)
            Return lista
        End Function



        Private Function GetOrderTestsByReadingCycle(ByVal sampleClass As String, ByVal Stat As Boolean, ByVal ID As Integer) _
        As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
            Dim AllTestTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

            AllTestTypeOrderTests = (From wse In Executions.twksWSExecutions _
                Where wse.StatFlag = Stat AndAlso
                      wse.SampleClass = sampleClass AndAlso
                      wse.ElementID = ID _
                Select wse Order By wse.ReadingCycle Descending)
            Return AllTestTypeOrderTests
        End Function

#End Region

    End Class
End Namespace

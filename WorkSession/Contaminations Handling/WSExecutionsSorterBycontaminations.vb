Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class WSSorter

    Sub New(executions As ExecutionsDS)
        Me.Executions = executions
    End Sub


    Public Property Executions As ExecutionsDS
    Public Property activeAnalyzer As String = ""

    Public Function SortWSExecutionsByContamination(ByVal pDBConnection As SqlConnection) As Boolean 'TypedGlobalDataTo(Of ExecutionsDS)

        'Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Dim contaminationsDataDS As ContaminationsDS = Nothing

        Dim ReturnData As Boolean = True

        Try
            Dim connection = DAOBase.GetSafeOpenDBConnection(pDBConnection)
            If (Not connection.HasError AndAlso Not connection.SetDatos Is Nothing) Then
                dbConnection = connection.SetDatos
                If (Not dbConnection Is Nothing) Then
                    'Get all R1 Contaminations 
                    'Dim myContaminationsDelegate As New ContaminationsDelegate

                    Dim contaminationsByType = ContaminationsDelegate.GetContaminationsByType(dbConnection, "R1")

                    If (Not contaminationsByType.HasError AndAlso Not contaminationsByType.SetDatos Is Nothing) Then
                        contaminationsDataDS = DirectCast(contaminationsByType.SetDatos, ContaminationsDS)

                        Dim highContaminationPersitance As Integer = SwParametersDelegate.ReadIntValue(pDBConnection, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

                        Dim stats() As Boolean = {True, False}
                        Dim sampleClasses() As String = {"BLANK", "CALIB", "CTRL", "PATIENT"}
                        Dim sampleTypes = GetSampleTypesStringsCollection(dbConnection)

                        'TR 27/05/2013 -Get a list of sample types separated by commas

                        Dim stdOrderTestsCount As Integer = 0

                        'Different Stat, SampleClasses and SampleTypes in WorkSession
                        Dim differentStatValues As List(Of Boolean) = (From wse In Executions.twksWSExecutions Select wse.StatFlag Distinct).ToList
                        Dim differentSampleClassValues As List(Of String) = (From wse In Executions.twksWSExecutions Select wse.SampleClass Distinct).ToList
                        Dim differentSampleTypeValues As List(Of String) = (From wse In Executions.twksWSExecutions Select wse.SampleType Distinct).ToList

                        Dim returnDS As New ExecutionsDS
                        For Each StatFlag In stats
                            If differentStatValues.Contains(StatFlag) Then 'Do not perform business if not necessary

                                For Each SampleClass In sampleClasses
                                    If differentSampleClassValues.Contains(SampleClass) Then 'Do not perform business if not necessary

                                        'AG 27/04/2012 AG + RH - Search the elementid for each sampleClass (the elementid code can be repeated from different sampleclasses)
                                        'Dim Elements = (From wse In pExecutions.twksWSExecutions _
                                        '              Select wse.ElementID Distinct).ToList()
                                        Dim Elements = (From wse In Executions.twksWSExecutions _
                                                        Where wse.SampleClass = SampleClass _
                                                        Select wse.ElementID Distinct).ToList()
                                        'AG 27/04/2012

                                        For Each elementID In Elements
                                            Dim ID = elementID
                                            Dim Stat As Boolean = StatFlag
                                            Dim SClass As String = SampleClass

                                            For Each sortedSampleType In sampleTypes
                                                'AG 30/08/2012 - add OrElse differentSampleTypeValues.Contains("") because when create a ws only with blanks there not exists sampletype!!
                                                If differentSampleTypeValues.Contains(sortedSampleType) OrElse _
                                                   (differentSampleClassValues.Count = 1 AndAlso differentSampleClassValues.Contains("BLANK")) Then

                                                    Dim SType As String = sortedSampleType

                                                    Dim AllTestTypeOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) 'All test type order tests executions
                                                    Dim OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) = Nothing  'Only STD test order tests executions

                                                    'NEW: When a patient OR CTRL has Ise & std test executions the ISE executions are the first
                                                    If SClass = "PATIENT" OrElse SClass = "CTRL" Then 'Apply OrderBy sample type order {"SER", "URI", "PLM", "WBL", "CSF"} (case one PATIENT with several sample types) + execution type
                                                        AllTestTypeOrderTests = (From wse In Executions.twksWSExecutions _
                                                                         Where wse.StatFlag = Stat AndAlso _
                                                                         wse.SampleClass = SClass AndAlso _
                                                                         wse.SampleType = SType AndAlso _
                                                                         wse.ElementID = ID _
                                                                         Select wse Order By wse.ExecutionType).ToList()

                                                        If AllTestTypeOrderTests.Count > 0 Then
                                                            'Look for the STD orderTests
                                                            OrderTests = (From wse In Executions.twksWSExecutions _
                                                                          Where wse.StatFlag = Stat AndAlso _
                                                                          wse.SampleClass = SClass AndAlso _
                                                                          wse.SampleType = SType AndAlso _
                                                                          wse.ElementID = ID AndAlso _
                                                                          wse.ExecutionType = "PREP_STD" _
                                                                          Select wse).ToList()
                                                            stdOrderTestsCount = OrderTests.Count
                                                        Else
                                                            stdOrderTestsCount = 0
                                                        End If

                                                    Else 'Do not apply OrderBy & do not take care about sample type order inside the same SAMPLE
                                                        'AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                        '                         Where wse.StatFlag = Stat AndAlso _
                                                        '                         wse.SampleClass = SClass AndAlso _
                                                        '                         wse.ElementID = ID _
                                                        '                         Select wse).ToList()

                                                        'RH 16/05/2012 Sort non PATIENT executions by time (ReadingCycle Descending)
                                                        AllTestTypeOrderTests = (From wse In Executions.twksWSExecutions _
                                                                                 Where wse.StatFlag = Stat AndAlso _
                                                                                 wse.SampleClass = SClass AndAlso _
                                                                                 wse.ElementID = ID _
                                                                                 Select wse Order By wse.ReadingCycle Descending).ToList()
                                                        If AllTestTypeOrderTests.Count > 0 Then
                                                            'Look for the STD orderTests
                                                            OrderTests = (From wse In Executions.twksWSExecutions _
                                                                          Where wse.StatFlag = Stat AndAlso _
                                                                          wse.SampleClass = SClass AndAlso _
                                                                          wse.ElementID = ID AndAlso _
                                                                          wse.ExecutionType = "PREP_STD" _
                                                                          Select wse).ToList()
                                                            stdOrderTestsCount = OrderTests.Count
                                                        Else
                                                            stdOrderTestsCount = 0
                                                        End If

                                                    End If

                                                    Dim OrderContaminationNumber As Integer = 0
                                                    If stdOrderTestsCount > 0 Then
                                                        OrderContaminationNumber = ExecutionsDelegate.GetContaminationNumber(contaminationsDataDS, OrderTests, highContaminationPersitance)
                                                    End If

                                                    ManageContaminations(activeAnalyzer, dbConnection, returnDS, contaminationsDataDS, highContaminationPersitance, OrderTests, AllTestTypeOrderTests, OrderContaminationNumber)

                                                    If SClass <> "PATIENT" AndAlso SClass <> "CTRL" Then Exit For 'For blank, calib do not take care about the sample type inside the same SAMPLE

                                                    'AG 19/02/2014 - #1514
                                                    AllTestTypeOrderTests = Nothing
                                                    OrderTests = Nothing
                                                    'AG 19/02/2014 - #1514

                                                End If
                                            Next 'For each mySampleType
                                        Next 'For each elementID

                                    End If
                                Next 'For each SampleClass

                            End If
                        Next 'For each StatFlag

                        Executions = returnDS
                        ReturnData = True
                    End If

                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ReturnData = False
        End Try

        'AG 19/02/2014 - #1514
        bestResult = Nothing
        currentResult = Nothing
        'AG 19/02/2014 - #1514

        Return ReturnData
    End Function

    Public Shared Function GetSampleTypesStringsCollection(ByVal dbConnection As SqlConnection) As IEnumerable(Of String)

        Dim sampleTypesTable = MasterDataDelegate.GetSampleTypesDataTable(dbConnection)
        Dim lista = (From sampleType In sampleTypesTable.SetDatos Select sampleType.ItemID)
        Return lista

    End Function

    Private Sub ManageContaminations(ByVal activeAnalyzer As String,
                                 ByVal pConn As SqlConnection,
                                 ByRef returnDS As ExecutionsDS,
                                 ByVal contaminationsDataDS As ContaminationsDS,
                                 ByVal highContaminationPersitance As Integer,
                                 ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                                 ByVal AllTestTypeOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                                 ByVal OrderContaminationNumber As Integer,
                                 Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                                 Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)

        Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        'Dim bestContaminationNumber As Integer = Integer.MaxValue
        Dim currentContaminationNumber As Integer

        If (OrderContaminationNumber > 0) Then

            currentContaminationNumber = OrderContaminationNumber
            currentResult = OrderTests.ToList()
            bestResult = ExecutionsDelegate.ManageContaminationsForRunningAndStatic(activeAnalyzer, pConn, contaminationsDataDS, currentResult, highContaminationPersitance, currentContaminationNumber, pPreviousReagentID, pPreviousReagentIDMaxReplicates)

            'A last try, if the order tests only have 2 tests that are contaminating between them, why not to interchange them?
            If currentContaminationNumber > 0 Then
                If OrderTests.Count = 2 Then
                    'Okay, if there are contaminations, why not to try interchange them?
                    currentResult.Clear()
                    For z = OrderTests.Count - 1 To 0 Step -1
                        currentResult.Add(OrderTests(z))
                    Next
                    currentContaminationNumber = ExecutionsDelegate.GetContaminationNumber(contaminationsDataDS, currentResult, highContaminationPersitance)
                    If currentContaminationNumber = 0 Then
                        bestResult = currentResult
                    End If
                End If
            End If

            Dim stdPrepFlag As Boolean = False
            For Each wse In AllTestTypeOrderTests
                If wse.ExecutionType <> "PREP_STD" Then
                    returnDS.twksWSExecutions.ImportRow(wse)
                ElseIf Not stdPrepFlag Then
                    stdPrepFlag = True
                    'Add all std test executions using bestResult
                    For Each wseStdTest In bestResult
                        returnDS.twksWSExecutions.ImportRow(wseStdTest)
                    Next
                End If
            Next
        Else
            For Each wse In AllTestTypeOrderTests
                returnDS.twksWSExecutions.ImportRow(wse)
            Next
        End If

    End Sub


End Class

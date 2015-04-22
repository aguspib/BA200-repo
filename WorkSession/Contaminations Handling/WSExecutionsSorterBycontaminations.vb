Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class WSSorter

    Sub New(executions As ExecutionsDS, activeAnalyzer As String)
        Me.Executions = executions
        Me.activeAnalyzer = activeAnalyzer
    End Sub

    Sub New(executions As ExecutionsDS)
        Me.New(executions, "")
    End Sub

    Public Property Executions As ExecutionsDS
    Public Property activeAnalyzer As String = ""

#Region "attributes"
    Dim highContaminationPersitance As Integer
    Dim allPossibleSampleClasses As String() = {"BLANK", "CALIB", "CTRL", "PATIENT"}
    Dim allPossibleSampleTypes As IEnumerable(Of String)
    Dim ExecutionsStatValues As IEnumerable(Of Boolean)
    Dim ExecutionsSampleClasses As IEnumerable(Of String)
    Dim ExecutionsSampleTypes As IEnumerable(Of String)
    Dim contaminationsDataDS As ContaminationsDS = Nothing
#End Region

    Private Sub InitializeAttributes(dbconnection As SqlConnection)
        highContaminationPersitance = SwParametersDelegate.ReadIntValue(dbconnection, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

        allPossibleSampleClasses = {"BLANK", "CALIB", "CTRL", "PATIENT"}
        allPossibleSampleTypes = GetSampleTypesStringsCollection(dbconnection)


        'Different Stat, SampleClasses and SampleTypes in WorkSession
        ExecutionsStatValues = (From wse In Executions.twksWSExecutions Select wse.StatFlag Distinct Order By StatFlag Ascending)
        ExecutionsSampleClasses = (From wse In Executions.twksWSExecutions Select wse.SampleClass Distinct)
        ExecutionsSampleTypes = (From wse In Executions.twksWSExecutions Select wse.SampleType Distinct)

        'Get contaminationsDataDS
        Dim contaminationsByType = ContaminationsDelegate.GetContaminationsByType(dbconnection, "R1")
        If (Not contaminationsByType.HasError AndAlso Not contaminationsByType.SetDatos Is Nothing) Then
            contaminationsDataDS = DirectCast(contaminationsByType.SetDatos, ContaminationsDS)
        End If

    End Sub


    Public Function SortWSExecutionsByContamination(ByVal pDBConnection As SqlConnection) As Boolean

        Dim dbConnection As SqlConnection = Nothing
        Dim ReturnData As Boolean = True

        Try
            Dim connection = DAOBase.GetSafeOpenDBConnection(pDBConnection)
            If (Not connection.HasError AndAlso Not connection.SetDatos Is Nothing) Then
                dbConnection = connection.SetDatos
                If (Not dbConnection Is Nothing) Then

                    InitializeAttributes(dbConnection)

                    Dim returnDS As New ExecutionsDS
                    For Each StatFlag In ExecutionsStatValues
                        For Each SampleClass In allPossibleSampleClasses
                            Dim sampleClassString = SampleClass
                            'ElementID es el ID del tipo de cosa. ID del calibrador, ID del blanco (-1) y en caso de no ser blancos y calibradores, es el number de row en pacientes dentro del grid de tests... ??
                            Dim elements = (From wse In Executions.twksWSExecutions Where wse.SampleClass = sampleClassString Select wse.ElementID Distinct)
                            For Each elementID In elements
                                returnDS = AppendExecutionsIntoResults(dbConnection, SampleClass, StatFlag, elementID, contaminationsDataDS, returnDS)
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
                Catch : End Try
            End If
        End Try

        Return ReturnData

    End Function

    Private Function AppendExecutionsIntoResults(ByVal dbConnection As SqlConnection, ByVal sampleClass As String, ByVal Stat As Boolean, ByVal ID As Integer, ByVal contaminationsDataDS As ContaminationsDS, ByVal returnDS As ExecutionsDS) As ExecutionsDS
        'Dim stdExecutionTypeOrderTestsCount As Integer

        For Each sortedSampleType In allPossibleSampleTypes
            'AG 30/08/2012 - add OrElse differentSampleTypeValues.Contains("") because when create a ws only with blanks there not exists sampletype!!
            If ExecutionsSampleTypes.Contains(sortedSampleType) OrElse _
               (ExecutionsSampleClasses.Count = 1 AndAlso ExecutionsSampleClasses.Contains("BLANK")) Then

                Dim sampleType As String = sortedSampleType

                Dim allTestTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow) 'All test type order tests executions
                Dim standardExecutionTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow) = Nothing  'Only STD test order tests executions

                'NEW: When a patient OR CTRL has Ise & std test executions the ISE executions are the first
                If sampleClass = "PATIENT" OrElse sampleClass = "CTRL" Then 'Apply OrderBy sample type order {"SER", "URI", "PLM", "WBL", "CSF"} (case one PATIENT with several sample types) + execution type

                    allTestTypeOrderTests = GetOrderTestsByExecutionType(sampleClass, Stat, sampleType, ID)
                    standardExecutionTypeOrderTests = GetStandardExecutionTypeOrderTests(sampleClass, Stat, sampleType, ID)

                Else
                    allTestTypeOrderTests = GetOrderTestsByReadingCycle(sampleClass, Stat, ID)
                    standardExecutionTypeOrderTests = GetStandardExecutionTypeOrderTests(sampleClass, Stat, ID)

                End If

                Dim orderContaminationNumber = 0
                If standardExecutionTypeOrderTests.Count > 0 Then
                    orderContaminationNumber = ExecutionsDelegate.GetContaminationNumber(contaminationsDataDS, standardExecutionTypeOrderTests, highContaminationPersitance)
                End If

                ManageContaminations(activeAnalyzer, dbConnection, returnDS, contaminationsDataDS, highContaminationPersitance, standardExecutionTypeOrderTests, allTestTypeOrderTests, orderContaminationNumber)

                If sampleClass <> "PATIENT" AndAlso sampleClass <> "CTRL" Then Exit For 'For blank, calib do not take care about the sample type inside the same SAMPLE

            End If
        Next 'For each mySampleType
        Return returnDS

    End Function

    Private Function GetStandardExecutionTypeOrderTests(ByVal sampleClass As String, ByVal Stat As Boolean, ByVal ID As Integer) As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
        Dim standardExecutionTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

        standardExecutionTypeOrderTests = (From wse In Executions.twksWSExecutions _
            Where wse.StatFlag = Stat AndAlso _
                  wse.SampleClass = sampleClass AndAlso _
                  wse.ElementID = ID AndAlso _
                  wse.ExecutionType = "PREP_STD" _
            Select wse)
        Return standardExecutionTypeOrderTests
    End Function

    Private Function GetStandardExecutionTypeOrderTests(ByVal sampleClass As String, ByVal Stat As Boolean, ByVal sampleType As String, ByVal ID As Integer) As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
        Dim standardExecutionTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

        standardExecutionTypeOrderTests = (From wse In Executions.twksWSExecutions _
            Where wse.StatFlag = Stat AndAlso _
                  wse.SampleClass = sampleClass AndAlso _
                  wse.SampleType = sampleType AndAlso _
                  wse.ElementID = ID AndAlso _
                  wse.ExecutionType = "PREP_STD" _
            Select wse)
        Return standardExecutionTypeOrderTests
    End Function

    Private Function GetOrderTestsByReadingCycle(ByVal sampleClass As String, ByVal Stat As Boolean, ByVal ID As Integer) As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
        Dim AllTestTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

        AllTestTypeOrderTests = (From wse In Executions.twksWSExecutions _
            Where wse.StatFlag = Stat AndAlso _
                  wse.SampleClass = sampleClass AndAlso _
                  wse.ElementID = ID _
            Select wse Order By wse.ReadingCycle Descending)
        Return AllTestTypeOrderTests
    End Function

    Private Function GetOrderTestsByExecutionType(ByVal sampleClass As String, ByVal Stat As Boolean, ByVal sampleType As String, ByVal ID As Integer) As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
        Dim allTestTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)

        allTestTypeOrderTests = (From wse In Executions.twksWSExecutions _
            Where wse.StatFlag = Stat AndAlso _
                  wse.SampleClass = sampleClass AndAlso _
                  wse.SampleType = sampleType AndAlso _
                  wse.ElementID = ID _
            Select wse Order By wse.ExecutionType)
        Return allTestTypeOrderTests
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
                                 ByVal OrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow),
                                 ByVal AllTestTypeOrderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow),
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


    Public Function SortWSExecutionsByElementGroupTime() As Boolean 'ByVal Executions As ExecutionsDS) As GlobalDataTO
        Dim returnDataSet As New ExecutionsDS
        Dim success As Boolean = False

        Try
            Dim qOrders As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim index = 0

            While index < Executions.twksWSExecutions.Rows.Count
                Dim statFlag = Executions.twksWSExecutions(index).StatFlag
                Dim sampleClass = Executions.twksWSExecutions(index).SampleClass

                qOrders = (From wse In Executions.twksWSExecutions _
                       Where wse.StatFlag = statFlag AndAlso wse.SampleClass = sampleClass _
                       Select wse).ToList()

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

    Private Sub OrderByExecutionTime(ByVal pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), ByRef returnDS As ExecutionsDS)
        While pExecutions.Count > 0

            'AG 16/09/2011 - add order criteria first by ExecutionStatus
            'Dim wseMaxReadingCycle = (From wse In pExecutions _
            '       Order By wse.ReadingCycle Descending _
            '       Select wse).ToList()(0)

            Dim wseMaxReadingCycle = (From wse In pExecutions _
                                      Order By wse.ExecutionStatus Descending, wse.ReadingCycle Descending _
                                      Select wse).ToList()(0)
            'AG 16/09/2011

            Dim wseSelected = (From wse In pExecutions _
                   Where wse.ElementID = wseMaxReadingCycle.ElementID _
                   Select wse).ToList()

            For Each wse In wseSelected
                returnDS.twksWSExecutions.ImportRow(wse)
            Next

            For Each wse In wseSelected
                pExecutions.Remove(wse)
            Next
        End While
    End Sub

End Class

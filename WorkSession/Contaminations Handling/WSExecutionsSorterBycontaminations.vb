﻿Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class WSSorter

    Public Function SortWSExecutionsByContamination(ByVal activeAnalyzer As String, ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutions As ExecutionsDS) As GlobalDataTO

        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Dim bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Dim currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        'Dim bestContaminationNumber As Integer = Integer.MaxValue
        'Dim currentContaminationNumber As Integer
        Dim contaminationsDataDS As ContaminationsDS = Nothing

        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Get all R1 Contaminations 
                    Dim myContaminationsDelegate As New ContaminationsDelegate
                    resultData = myContaminationsDelegate.GetContaminationsByType(dbConnection, "R1")

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        contaminationsDataDS = DirectCast(resultData.SetDatos, ContaminationsDS)

                        Dim highContaminationPersitance As Integer = 0

                        Dim swParametersDlg As New SwParametersDelegate
                        resultData = swParametersDlg.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS.ToString, Nothing)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            highContaminationPersitance = CInt(resultData.SetDatos)
                        End If

                        Dim Stats() As Boolean = {True, False}
                        Dim SampleClasses() As String = {"BLANK", "CALIB", "CTRL", "PATIENT"}

                        'TR 27/05/2013 -Get a list of sample types separated by commas
                        Dim SampleTypes() As String = Nothing
                        Dim myMasterDataDelegate As New MasterDataDelegate
                        resultData = myMasterDataDelegate.GetSampleTypes(dbConnection)
                        If Not resultData.HasError Then
                            SampleTypes = resultData.SetDatos.ToString.Split(CChar(","))
                        End If

                        'Dim SampleTypes() As String = {"SER", "URI", "PLM", "WBL", "CSF", "LIQ", "SEM"}
                        Dim stdOrderTestsCount As Integer = 0

                        'Different Stat, SampleClasses and SampleTypes in WorkSession
                        Dim differentStatValues As List(Of Boolean) = (From wse In pExecutions.twksWSExecutions Select wse.StatFlag Distinct).ToList
                        Dim differentSampleClassValues As List(Of String) = (From wse In pExecutions.twksWSExecutions Select wse.SampleClass Distinct).ToList
                        Dim differentSampleTypeValues As List(Of String) = (From wse In pExecutions.twksWSExecutions Select wse.SampleType Distinct).ToList

                        Dim returnDS As New ExecutionsDS
                        For Each StatFlag In Stats
                            If differentStatValues.Contains(StatFlag) Then 'Do not perform business if not necessary

                                For Each SampleClass In SampleClasses
                                    If differentSampleClassValues.Contains(SampleClass) Then 'Do not perform business if not necessary

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

                                            For Each sortedSampleType In SampleTypes
                                                'AG 30/08/2012 - add OrElse differentSampleTypeValues.Contains("") because when create a ws only with blanks there not exists sampletype!!
                                                If differentSampleTypeValues.Contains(sortedSampleType) OrElse _
                                                   (differentSampleClassValues.Count = 1 AndAlso differentSampleClassValues.Contains("BLANK")) Then

                                                    Dim SType As String = sortedSampleType

                                                    Dim AllTestTypeOrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) 'All test type order tests executions
                                                    Dim OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow) = Nothing  'Only STD test order tests executions

                                                    'NEW: When a patient OR CTRL has Ise & std test executions the ISE executions are the first
                                                    If SClass = "PATIENT" OrElse SClass = "CTRL" Then 'Apply OrderBy sample type order {"SER", "URI", "PLM", "WBL", "CSF"} (case one PATIENT with several sample types) + execution type
                                                        AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                         Where wse.StatFlag = Stat AndAlso _
                                                                         wse.SampleClass = SClass AndAlso _
                                                                         wse.SampleType = SType AndAlso _
                                                                         wse.ElementID = ID _
                                                                         Select wse Order By wse.ExecutionType).ToList()

                                                        If AllTestTypeOrderTests.Count > 0 Then
                                                            'Look for the STD orderTests
                                                            OrderTests = (From wse In pExecutions.twksWSExecutions _
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
                                                        AllTestTypeOrderTests = (From wse In pExecutions.twksWSExecutions _
                                                                                 Where wse.StatFlag = Stat AndAlso _
                                                                                 wse.SampleClass = SClass AndAlso _
                                                                                 wse.ElementID = ID _
                                                                                 Select wse Order By wse.ReadingCycle Descending).ToList()
                                                        If AllTestTypeOrderTests.Count > 0 Then
                                                            'Look for the STD orderTests
                                                            OrderTests = (From wse In pExecutions.twksWSExecutions _
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

                        'AG 19/02/2014 - #1514
                        differentStatValues = Nothing
                        differentSampleClassValues = Nothing
                        differentSampleTypeValues = Nothing
                        'AG 19/02/2014 - #1514

                        resultData.SetDatos = returnDS
                    End If

                End If
            End If

        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.SortWSExecutionsByContamination", EventLogEntryType.Error, False)

        End Try

        'AG 19/02/2014 - #1514
        bestResult = Nothing
        currentResult = Nothing
        'AG 19/02/2014 - #1514

        Return resultData
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

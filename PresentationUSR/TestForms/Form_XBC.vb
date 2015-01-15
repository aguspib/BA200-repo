
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports LIS.Biosystems.Ax00.LISCommunications

Public Class Form_XBC

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim resultData As New GlobalDataTO

        Try
            '' CODEPAGE
            'MessageBox.Show(System.Text.Encoding.Default.EncodingName)
            'MessageBox.Show(System.Text.Encoding.Default.CodePage)
            'MessageBox.Show(System.Text.Encoding.Default.WindowsCodePage)

            'Dim codePageString As String = "1252"
            'Dim cp As Integer = CInt(codePageString)
            'Dim enc As Encoding = Encoding.GetEncoding(cp)

            'Debug.WriteLine(String.Format("codePageString = '{0}' BodyName = '{1}' CodePage = {2} EncodingName = '{3}' HeaderName = '{4}' WindowsCodePage = {5}", _
            '                              codePageString, enc.BodyName, enc.CodePage, enc.EncodingName, enc.HeaderName, enc.WindowsCodePage))


            ' ****************** LIS TESTS *************************

            ' PROCESS ORDER TESTS
            'Dim myOrderTestsDelegate As New OrderTestsDelegate
            'resultData = myOrderTestsDelegate.LoadLISSavedWS(Nothing, IAx00MainMDI.ActiveAnalyzer, IAx00MainMDI.ActiveWorkSession, "", False)


            '' PROCESS LIS CONTROLS
            'Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate
            'Dim myWSDelegate As New WorkSessionsDelegate
            'Dim myOrderTestsDelegate As New OrderTestsDelegate
            'Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
            'Dim myWorkSessionResultDS As New WorkSessionResultDS
            'Dim pSavedWSID As Integer = 20
            'resultData = mySavedWSOrderTests.GetOrderTestsBySavedWSID(Nothing, pSavedWSID)
            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '    mySavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)
            'End If
            'resultData = myWSDelegate.GetOrderTestsForWS(Nothing, IAx00MainMDI.ActiveWorkSession)
            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
            'End If

            'resultData = myOrderTestsDelegate.ProcessLISControlOTs(Nothing, IAx00MainMDI.ActiveAnalyzer, myWorkSessionResultDS, mySavedWSOrderTestsDS, Nothing)


            '' ORDER DOWNLOAD - RERUNs
            'Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate
            'Dim myWSDelegate As New WorkSessionsDelegate
            'Dim myOrderTestsDelegate As New OrderTestsDelegate
            'Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
            'Dim myWorkSessionResultDS As New WorkSessionResultDS
            'Dim pSavedWSID As Integer = 5


            'resultData = mySavedWSOrderTests.GetOrderTestsBySavedWSID(Nothing, pSavedWSID)
            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '    mySavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)
            'End If
            '' Create SavedWSOrderTests ------------------------------------------------------------------
            'Dim mySavedDataTable As SavedWSOrderTestsDS.tparSavedWSOrderTestsDataTable
            'mySavedDataTable = DirectCast(mySavedWSOrderTestsDS.tparSavedWSOrderTests, SavedWSOrderTestsDS.tparSavedWSOrderTestsDataTable)
            'Dim mySavedRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow
            'mySavedRow = DirectCast(mySavedDataTable.NewRow, SavedWSOrderTestsDS.tparSavedWSOrderTestsRow)
            'mySavedRow.SampleClass = "PATIENT"
            'mySavedRow.SampleID = "#201303220001"
            'mySavedRow.PatientIDType = "AUTO"
            'mySavedRow.StatFlag = False
            'mySavedRow.TestType = "STD"
            'mySavedRow.TestID = 1
            'mySavedRow.SampleType = "SER"
            'mySavedRow.TubeType = "T13"
            'mySavedRow.ReplicatesNumber = 1
            'mySavedRow.CreationOrder = 1
            'mySavedRow.TestName = "ACID GLYCOPROTEI"
            'mySavedRow.FormulaText = ""
            'mySavedRow.ExternalQC = False
            'mySavedRow.SavedWSName = "sesA"
            'mySavedRow.CalcTestIDs = "3, 8"
            'mySavedRow.CalcTestNames = "BUN, GLOBULIN"
            'mySavedDataTable.Rows.Add(mySavedRow)
            'mySavedWSOrderTestsDS.tparSavedWSOrderTests.AcceptChanges()

            'resultData = myWSDelegate.GetOrderTestsForWS(Nothing, IAx00MainMDI.ActiveWorkSession)
            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
            'End If
            '' Create WorkSessions patients --------------------------------------------------------------
            'Dim myPatientsDataTable As WorkSessionResultDS.PatientsDataTable
            'myPatientsDataTable = DirectCast(myWorkSessionResultDS.Patients, WorkSessionResultDS.PatientsDataTable)
            'Dim myPatientsRow As WorkSessionResultDS.PatientsRow
            'myPatientsRow = DirectCast(myPatientsDataTable.NewRow(), WorkSessionResultDS.PatientsRow)
            'myPatientsRow.SampleClass = "PATIENT"
            'myPatientsRow.OrderID = "201303220001"
            'myPatientsRow.OrderTestID = 12
            'myPatientsRow.StatFlag = False
            'myPatientsRow.SampleID = "#201303220001"
            'myPatientsRow.SampleIDType = "AUTO"
            'myPatientsRow.TestType = "STD"
            'myPatientsRow.TestID = 1
            'myPatientsRow.TestName = "ACID GLYCOPROTEI"
            'myPatientsRow.SampleType = "SER"
            'If myPatientsRow.TestType = "STD" Or myPatientsRow.TestType = "ISE" Then
            '    myPatientsRow.NumReplicates = 1
            '    myPatientsRow.TubeType = "T13"
            'Else
            '    myPatientsRow.CalcTestFormula = ""
            'End If
            'myPatientsRow.CalcTestID = "5"
            'myPatientsRow.CalcTestName = "% TRF"
            'myPatientsRow.AwosID = ""
            'myPatientsRow.SpecimenID = "9999"
            'myPatientsRow.ESOrderID = ""
            'myPatientsRow.LISOrderID = ""
            'myPatientsRow.ESPatientID = ""
            'myPatientsRow.LISPatientID = ""
            'myPatientsRow.ExternalQC = False
            'myPatientsRow.Selected = False
            'myPatientsRow.OTStatus = "OPEN"
            'myPatientsRow.LISRequest = False
            'myPatientsRow.CreationOrder = 1
            'myPatientsDataTable.Rows.Add(myPatientsRow)
            'myWorkSessionResultDS.Patients.AcceptChanges()

            '' ---------------------------------------------------------------------------------
            'If (myWorkSessionResultDS.Patients.Rows.Count > 0) AndAlso (mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Count > 0) Then
            '    For Each wsRow As WorkSessionResultDS.PatientsRow In myWorkSessionResultDS.Patients.Rows
            '        For Each Row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
            '            If Row.SampleClass = "PATIENT" Then

            '                If wsRow.SampleID = Row.SampleID And _
            '                   wsRow.TestType = Row.TestType And _
            '                   wsRow.TestID = Row.TestID And _
            '                   wsRow.SampleType = Row.SampleType Then

            '                 resultData =    myOrderTestsDelegate.VerifyRerunOfManualPatientOT(wsRow, Row, "ANALYZER", Nothing, Nothing)
            '                    'resultData = myOrderTestsDelegate.VerifyRerunOfLISPatientOT(wsRow, Row, "ANALYZER", Nothing, Nothing)
            '                End If
            '            End If
            '        Next
            '    Next
            'End If

            '' ORDER DOWNLOAD - ADD
            'Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate
            'Dim myWSDelegate As New WorkSessionsDelegate
            'Dim myOrderTestsDelegate As New OrderTestsDelegate
            'Dim mySavedWSOrderTestsDS As SavedWSOrderTestsDS = Nothing
            'Dim myWorkSessionResultDS As WorkSessionResultDS = Nothing
            'Dim pSavedWSID As Integer = 1 ' TO CONSULT !

            'resultData = mySavedWSOrderTests.GetOrderTestsBySavedWSID(Nothing, pSavedWSID)
            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '    mySavedWSOrderTestsDS = DirectCast(resultData.SetDatos, SavedWSOrderTestsDS)
            'End If

            'resultData = myWSDelegate.GetOrderTestsForWS(Nothing, IAx00MainMDI.ActiveWorkSession)
            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '    myWorkSessionResultDS = DirectCast(resultData.SetDatos, WorkSessionResultDS)
            'End If

            'If (mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows.Count > 0) Then
            '    For Each Row As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow In mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows
            '        If Row.SampleClass = "PATIENT" Then
            '            myOrderTestsDelegate.AddPatientOrderTestsFromLIS(IAx00MainMDI.ActiveAnalyzer, Row, myWorkSessionResultDS)
            '        End If
            '    Next
            'End If



            ' UPLOAD RESULTS
            'Dim myExport As New ExportDelegate
            'Dim WorkSessionIDField As String = ""
            'Dim AnalyzerIDField As String = ""
            'Dim AnalyzerModelField As String = ""
            'WorkSessionIDField = IAx00MainMDI.ActiveWorkSession
            'AnalyzerIDField = IAx00MainMDI.ActiveAnalyzer
            'AnalyzerModelField = "A400" ' IAx00MainMDI.AnalyzerModel"
            'resultData = myExport.ExportToLISManualNEW(AnalyzerIDField, WorkSessionIDField, True)
            'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
            '    Dim myExportedExecutionsDS As New ExecutionsDS
            '    myExportedExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

            '    Dim myLISManager As ESWrapper
            '    Dim myParams As New SwParametersDelegate
            '    Dim myParametersDS As New ParametersDS
            '    Dim appNameForLISAttribute As String = "BAx00"
            '    Dim channelIdForLISAttribute As String = "1"

            '    'Read these parameters from database and update attributes appNameForLISAttribute and channelIdForLISAttribute
            '    ' Read application name for LIS parameter
            '    resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.APP_NAME_FOR_LIS.ToString, Nothing)
            '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
            '        myParametersDS = CType(resultData.SetDatos, ParametersDS)
            '        If myParametersDS.tfmwSwParameters.Rows.Count > 0 Then
            '            appNameForLISAttribute = myParametersDS.tfmwSwParameters.Item(0).ValueText
            '        End If

            '    End If
            '    ' Read channel ID for LIS parameter
            '    resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.CHANNELID_FOR_LIS.ToString, Nothing)
            '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
            '        myParametersDS = CType(resultData.SetDatos, ParametersDS)
            '        If myParametersDS.tfmwSwParameters.Rows.Count > 0 Then
            '            channelIdForLISAttribute = myParametersDS.tfmwSwParameters.Item(0).ValueText
            '        End If
            '    End If

            '    myLISManager = New ESWrapper(appNameForLISAttribute, channelIdForLISAttribute, AnalyzerModelField, AnalyzerIDField)


            '    Dim myTestMappDS As New AllTestsByTypeDS
            '    Dim myConfMappDS As New LISMappingsDS

            '    Dim testmapDlg As New AllTestByTypeDelegate
            '    resultData = testmapDlg.ReadAll(Nothing)
            '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
            '        myTestMappDS = CType(resultData.SetDatos, AllTestsByTypeDS)
            '    End If

            '    Dim mappDlg As New LISMappingsDelegate
            '    resultData = mappDlg.ReadAll(Nothing)
            '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
            '        myConfMappDS = CType(resultData.SetDatos, LISMappingsDS)
            '    End If

            '    'Get current WS results and result alarms
            '    Dim wsResultsDS As New ResultsDS
            '    Dim wsResultAlarmsDS As New ResultsDS

            '    If Not resultData.HasError Then
            '        'Get current WS results
            '        Dim myResults As New ResultsDelegate
            '        resultData = myResults.GetCompleteResults(Nothing, AnalyzerIDField, IAx00MainMDI.ActiveWorkSession)

            '        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
            '            wsResultsDS = CType(resultData.SetDatos, ResultsDS)
            '            resultData = myResults.GetResultAlarms(Nothing)
            '            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
            '                wsResultAlarmsDS = CType(resultData.SetDatos, ResultsDS)
            '            End If
            '        End If
            '    End If

            '    If Not resultData.HasError Then
            '        resultData = myLISManager.UploadOrdersResults(Nothing, myExportedExecutionsDS, False, myTestMappDS, myConfMappDS, wsResultsDS, wsResultAlarmsDS)
            '    End If

            'End If

            ' UPLOAD HISTORY RESULTS
            Dim myExport As New ExportDelegate
            Dim WorkSessionIDField As String = ""
            Dim AnalyzerIDField As String = ""
            Dim AnalyzerModelField As String = ""
            WorkSessionIDField = IAx00MainMDI.ActiveWorkSession
            AnalyzerIDField = IAx00MainMDI.ActiveAnalyzer
            AnalyzerModelField = "A400" ' IAx00MainMDI.AnalyzerModel"

            Dim myHisWSOTDelegate As New HisWSOrderTestsDelegate
            resultData = myHisWSOTDelegate.ReadAll(Nothing, AnalyzerIDField)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                Dim myHisWSData As HisWSOrderTestsDS = TryCast(resultData.SetDatos, HisWSOrderTestsDS)
                Dim HistOrderTestIDs As New List(Of Integer)
                Dim HistOrderTests As List(Of HisWSOrderTestsDS.thisWSOrderTestsRow) = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In myHisWSData.thisWSOrderTests).ToList
                For Each h As HisWSOrderTestsDS.thisWSOrderTestsRow In HistOrderTests
                    HistOrderTestIDs.Add(h.HistOrderTestID)
                Next
                resultData = myExport.ExportToLISManualFromHIST(HistOrderTestIDs)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    Dim myExportedExecutionsDS As New ExecutionsDS
                    myExportedExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                    Dim myLISManager As ESWrapper
                    Dim myParams As New SwParametersDelegate
                    Dim myParametersDS As New ParametersDS
                    Dim appNameForLISAttribute As String = "BAx00"
                    Dim channelIdForLISAttribute As String = "1"

                    'Read these parameters from database and update attributes appNameForLISAttribute and channelIdForLISAttribute
                    ' Read application name for LIS parameter
                    resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.APP_NAME_FOR_LIS.ToString, Nothing)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        myParametersDS = CType(resultData.SetDatos, ParametersDS)
                        If myParametersDS.tfmwSwParameters.Rows.Count > 0 Then
                            appNameForLISAttribute = myParametersDS.tfmwSwParameters.Item(0).ValueText
                        End If

                    End If
                    ' Read channel ID for LIS parameter
                    resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.CHANNELID_FOR_LIS.ToString, Nothing)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        myParametersDS = CType(resultData.SetDatos, ParametersDS)
                        If myParametersDS.tfmwSwParameters.Rows.Count > 0 Then
                            channelIdForLISAttribute = myParametersDS.tfmwSwParameters.Item(0).ValueText
                        End If
                    End If

                    myLISManager = New ESWrapper(appNameForLISAttribute, channelIdForLISAttribute, AnalyzerModelField, AnalyzerIDField)


                    Dim myTestMappDS As New AllTestsByTypeDS
                    Dim myConfMappDS As New LISMappingsDS

                    Dim testmapDlg As New AllTestByTypeDelegate
                    resultData = testmapDlg.ReadAll(Nothing)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        myTestMappDS = CType(resultData.SetDatos, AllTestsByTypeDS)
                    End If

                    Dim mappDlg As New LISMappingsDelegate
                    resultData = mappDlg.ReadAll(Nothing)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        myConfMappDS = CType(resultData.SetDatos, LISMappingsDS)
                    End If

                    'Get History Results
                    Dim wsResultsDS As New ResultsDS
                    Dim wsResultAlarmsDS As New ResultsDS

                    If Not resultData.HasError Then
                        'Get current WS results
                        Dim myResults As New ResultsDelegate
                        resultData = myResults.GetCompleteResults(Nothing, AnalyzerIDField, IAx00MainMDI.ActiveWorkSession)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            wsResultsDS = CType(resultData.SetDatos, ResultsDS)
                            resultData = myResults.GetResultAlarms(Nothing)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                wsResultAlarmsDS = CType(resultData.SetDatos, ResultsDS)
                            End If
                        End If
                    End If

                    If Not resultData.HasError Then
                        resultData = myLISManager.UploadOrdersResults(Nothing, myExportedExecutionsDS, True, myTestMappDS, myConfMappDS, wsResultsDS, wsResultAlarmsDS, Nothing)
                    End If

                End If
            End If

            '' SAVE XML MESSAGE RECEPTION
            'Dim xmlMessageDgt As New xmlMessagesDelegate

            'Dim myUtils As New Utilities
            'resultData = myUtils.GetNewGUID
            'Dim xmlMessage As New XmlDocument

            '' v1
            ''xmlMessage.Load("C:\Users\Xavier Badia\Desktop\En curs\LIS\XML Example - copia.xml")
            '' v2
            ''Dim xml As String = IO.File.ReadAllText("C:\Users\Xavier Badia\Desktop\En curs\LIS\XML Example.xml")
            ''xmlMessage.LoadXml(xml)
            '' v3
            'Dim stream As System.IO.StreamReader
            'stream = New System.IO.StreamReader("C:\Users\Xavier Badia\Desktop\En curs\LIS\XML Example.xml")
            'xmlMessage.Load(stream)
            '' fi v's

            'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
            '    Dim myMsgId As String = CType(resultData.SetDatos, String)
            '    resultData = xmlMessageDgt.AddMessage(Nothing, myMsgId, xmlMessage, "SENDING")
            'End If

            '' READ XML MESSAGE 
            ''resultData = xmlMessageDgt.ReadAll(Nothing)
            'resultData = xmlMessageDgt.ReadByStatus(Nothing, "DONE")
            'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
            '    'Dim myXmlMessages As New List(Of XMLMessagesTO)
            '    'myXmlMessages = DirectCast(resultData.SetDatos, List(Of XMLMessagesTO))

            '    ''Dim qxmlMsg As List(Of XMLMessagesDS.twksXMLMessagesRow)
            '    ''qxmlMsg = (From a In xmlMessageDS.twksXMLMessages _
            '    ''           Select a).ToList()

            '    ''Dim OutputXML As XmlDocument
            '    ''Dim XMLReader As XmlReader
            '    ''For Each xmlMsg As XMLMessagesDS.twksXMLMessagesRow In qxmlMsg
            '    ''    'OutputXML = xmlMsg.XMLMessage
            '    ''Next

            'End If

            'Dim myguid As String = "5ff2d425-1120-4885-9556-0a441de82f7a"
            'resultData = xmlMessageDgt.Read(Nothing, myguid)

            ' DELETE XML MESSAGE
            'Dim myguid As String = "8e891f9c-931a-4513-a193-e8daa182cbf7"
            'resultData = xmlMessageDgt.Delete(Nothing, myguid)
            'resultData = xmlMessageDgt.DeleteAll(Nothing)
            'resultData = xmlMessageDgt.DeleteByStatus(Nothing, "SENT")

            'IAx00MainMDI.ManageNewLISMessage("1", 1, Nothing)



            ' ****************** ISE TESTS *************************
            '' Test ADD ISETests
            'Dim myDelegate As New ISETestsDelegate
            'Dim myISETestsDS As New ISETestsDS
            'Dim myRow As DataRow = myISETestsDS.tparISETests.NewRow
            'myRow("ISETestID") = 5
            'myRow("ISE_ResultID") = "XX"
            'myRow("Name") = "XX+"
            'myRow("ShortName") = "XX+"
            'myRow("Units") = "xxLs"
            'myRow("ISE_Units") = "xxLs"
            'myRow("InUse") = False
            ''myRow("ActiveRangeType") = 
            ''myRow("TS_User") = 
            'myISETestsDS.tparISETests.Rows.Add(myRow)
            'resultData = myDelegate.Add(Nothing, myISETestsDS)

            '' Test UPDATE ISETests
            'Dim myDelegate As New ISETestsDelegate
            'Dim myISETestsDS As New ISETestsDS
            'Dim myRow As DataRow = myISETestsDS.tparISETests.NewRow
            'myRow("ISETestID") = 5
            'myRow("ISE_ResultID") = "AA"
            'myRow("Name") = "AA+"
            'myRow("ShortName") = "AA+"
            'myRow("Units") = "aaLs"
            'myRow("ISE_Units") = "aaLs"
            'myRow("InUse") = True
            ''myRow("ActiveRangeType") = 
            ''myRow("TS_User") = 
            'myISETestsDS.tparISETests.Rows.Add(myRow)
            'resultData = myDelegate.Modify(Nothing, myISETestsDS)

            '' Test READALL ISETests
            'Dim myDelegate As New ISETestsDelegate
            'resultData = myDelegate.GetList(Nothing)
            'If Not (resultData.HasError) Then
            '    Dim myISETestsDS As ISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)
            '    For Each dr As ISETestsDS.tparISETestsRow In myISETestsDS.tparISETests.Rows
            '        Debug.Print("ISETestID : " & dr("ISETestID").ToString)
            '        Debug.Print("ISE_ResultID : " & dr("ISE_ResultID").ToString)
            '        Debug.Print("Name : " & dr("Name").ToString)
            '        Debug.Print("ShortName : " & dr("ShortName").ToString)
            '        Debug.Print("Units : " & dr("Units").ToString)
            '        Debug.Print("ISE_Units : " & dr("ISE_Units").ToString)
            '        Debug.Print("InUse : " & dr("InUse").ToString)
            '    Next
            'End If

            '' Test DELETE ISETests & their Samples associated
            'Dim myDelegate As New ISETestsDelegate
            'Dim myISETestsDS As New ISETestsDS
            'Dim myRow As DataRow = myISETestsDS.tparISETests.NewRow
            'myRow("ISETestID") = 5
            'myISETestsDS.tparISETests.Rows.Add(myRow)
            'resultData = myDelegate.Delete(Nothing, myISETestsDS)


            ' ****************** ISE TEST SAMPLES *************************
            '' Test ADD ISETestSamples
            'Dim myDelegate As New ISETestSamplesDelegate
            'Dim myISETestSamplesDS As New ISETestSamplesDS
            'Dim myRow As DataRow = myISETestSamplesDS.tparISETestSamples.NewRow
            'myRow("ISETestID") = 5
            'myRow("SampleType") = "XXX"
            'myRow("SampleType_ResultID") = "WWW"
            'myRow("Decimals") = 2
            'myRow("ISE_Volume") = 500
            'myRow("ISE_DilutionFactor") = 4
            'myRow("ISE_RangeLower") = 1
            'myRow("ISE_RangeUpper") = 100
            ''myRow("ActiveRangeType") = 
            ''myRow("TS_User") = 
            'myISETestSamplesDS.tparISETestSamples.Rows.Add(myRow)
            'resultData = myDelegate.Add(Nothing, myISETestSamplesDS)

            '' Test UPDATE ISETestSamples
            'Dim myDelegate As New ISETestSamplesDelegate
            'Dim myISETestSamplesDS As New ISETestSamplesDS
            'Dim myRow As DataRow = myISETestSamplesDS.tparISETestSamples.NewRow
            'myRow("ISETestID") = 4
            'myRow("SampleType") = "XXX"
            'myRow("SampleType_ResultID") = "AAA"
            'myRow("Decimals") = 3
            'myRow("ISE_Volume") = 300
            'myRow("ISE_DilutionFactor") = 3
            'myRow("ISE_RangeLower") = 3
            'myRow("ISE_RangeUpper") = 300
            ''myRow("ActiveRangeType") = 
            ''myRow("TS_User") = 
            'myISETestSamplesDS.tparISETestSamples.Rows.Add(myRow)
            'resultData = myDelegate.Modify(Nothing, myISETestSamplesDS)

            '' Test READbyID ISETestSamples
            'Dim myDelegate As New ISETestSamplesDelegate
            'resultData = myDelegate.GetListByISETestID(Nothing, 3)
            'If Not (resultData.HasError) Then
            '    Dim myISETestSamplesDS As ISETestSamplesDS = DirectCast(resultData.SetDatos, ISETestSamplesDS)
            '    For Each dr As ISETestSamplesDS.tparISETestSamplesRow In myISETestSamplesDS.tparISETestSamples.Rows
            '        Debug.Print("SampleType : " & dr("SampleType").ToString)
            '        Debug.Print("SampleType_ResultID : " & dr("SampleType_ResultID").ToString)
            '        Debug.Print("Decimals : " & dr("Decimals").ToString)
            '        Debug.Print("ISE_Volume : " & dr("ISE_Volume").ToString)
            '        Debug.Print("ISE_DilutionFactor : " & dr("ISE_DilutionFactor").ToString)
            '        Debug.Print("ISE_RangeLower : " & dr("ISE_RangeLower").ToString)
            '        Debug.Print("ISE_RangeUpper : " & dr("ISE_RangeUpper").ToString)
            '        Debug.Print("ActiveRangeType : " & dr("ActiveRangeType").ToString)
            '        Debug.Print("TS_User : " & dr("TS_User").ToString)
            '    Next
            'End If


            ' ****************** ALARMS TESTS *************************
            '' Test ADD Alarms
            'Dim myDelegate As New AlarmsDelegate
            'Dim myAlarmsDS As New AlarmsDS
            'Dim myRow As DataRow = myAlarmsDS.tfmwAlarms.NewRow
            'myRow("AlarmID") = "XBC_TEST"
            'myRow("AlarmSource") = "XXX"
            'myRow("AlarmType") = "REMARK_XXX"
            'myRow("Name") = "XXX_XXX"
            ''myRow("NameResourceID") = ""
            'myRow("Description") = "xxx Description"
            'myRow("DescResourceID") = "XXX_XXX_XXX"
            'myRow("Solution") = ""
            ''myRow("SolResourceID") = 
            'myAlarmsDS.tfmwAlarms.Rows.Add(myRow)
            'resultData = myDelegate.Add(Nothing, myAlarmsDS)

            '' Test UPDATE Alarms
            'Dim myDelegate As New AlarmsDelegate
            'Dim myAlarmsDS As New AlarmsDS
            'Dim myRow As DataRow = myAlarmsDS.tfmwAlarms.NewRow
            'myRow("AlarmID") = "XBC_TEST"
            'myRow("AlarmSource") = "ZZZ"
            'myRow("AlarmType") = "REMARK_ZZZ"
            'myRow("Name") = "ZZZ_ZZZ"
            ''myRow("NameResourceID") = ""
            'myRow("Description") = "zzz Description"
            'myRow("DescResourceID") = "ZZZ_ZZZ_ZZZ"
            'myRow("Solution") = ""
            ''myRow("SolResourceID") = 
            'myAlarmsDS.tfmwAlarms.Rows.Add(myRow)
            'resultData = myDelegate.Update(Nothing, myAlarmsDS)

            '' Test READALL Alarms
            'Dim myDelegate As New AlarmsDelegate
            'resultData = myDelegate.Read(Nothing, "XBC_TEST")
            'If Not (resultData.HasError) Then
            '    Dim myAlarmsDS As AlarmsDS = DirectCast(resultData.SetDatos, AlarmsDS)
            '    For Each dr As AlarmsDS.tfmwAlarmsRow In myAlarmsDS.tfmwAlarms.Rows
            '        Debug.Print("AlarmID : " & dr("AlarmID").ToString)
            '        Debug.Print("AlarmSource : " & dr("AlarmSource").ToString)
            '        Debug.Print("AlarmType : " & dr("AlarmType").ToString)
            '        Debug.Print("Name : " & dr("Name").ToString)
            '        Debug.Print("NameResourceID : " & dr("NameResourceID").ToString)
            '        Debug.Print("Description : " & dr("Description").ToString)
            '        Debug.Print("DescResourceID : " & dr("DescResourceID").ToString)
            '        Debug.Print("Solution : " & dr("Solution").ToString)
            '        Debug.Print("SolResourceID : " & dr("SolResourceID").ToString)
            '    Next
            'End If

            '' Test DELETE Alarms
            'Dim myDelegate As New AlarmsDelegate
            'Dim myAlarmsDS As New AlarmsDS
            'Dim myRow As DataRow = myAlarmsDS.tfmwAlarms.NewRow
            'myRow("AlarmID") = "XBC_TEST"
            'myAlarmsDS.tfmwAlarms.Rows.Add(myRow)
            'resultData = myDelegate.Delete(Nothing, myAlarmsDS)

            '' Test LANGUAGE UPDATE Alarms
            'Dim myDelegate As New AlarmsDelegate
            'resultData = myDelegate.UpdateLanguageResource(Nothing, "SPA")



            If Not (resultData.HasError) Then
                MessageBox.Show("OK !")
            Else
                MessageBox.Show("ERROR [" & resultData.ErrorMessage)
            End If

        Catch ex As Exception
            MessageBox.Show("Err : " & ex.Message & vbCrLf & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class
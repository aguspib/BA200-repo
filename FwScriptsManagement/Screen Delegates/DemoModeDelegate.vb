Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class DemoModeDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate
            MyClass.ReportCountTimeout = 0
        End Sub
        Public Sub New()
            MyBase.New() 'SGM 20/01/2012
        End Sub
#End Region

#Region "Enumerations"
        Private Enum OPERATIONS
            _NONE
            READ_STRESS
            SEND_STRESS
            STOP_STRESS
        End Enum
#End Region

#Region "Declarations"
        Private MyResultsStressTest As StressDataTO
        Private CurrentOperation As OPERATIONS
        Private RecommendationsReport() As HISTORY_RECOMMENDATIONS
        Private ReportCountTimeout As Integer
#End Region

#Region "Attributes"
        Private TimerStatusModeStressAttr As Long   ' time between each request of state to the analyzer
        Private NumCyclesAttr As Long               ' Number of demo cycles defined
        Private StatusStressModeAttr As STRESS_STATUS
#End Region

#Region "Properties"
        'Public Property AnalyzerId() As String

        Public ReadOnly Property TimerStatusModeStress() As Long
            Get
                Return TimerStatusModeStressAttr
            End Get
        End Property

        Public ReadOnly Property NumCycles() As Long
            Get
                Return MyClass.NumCyclesAttr
            End Get
        End Property

        Public ReadOnly Property StatusStressMode() As STRESS_STATUS
            Get
                Return StatusStressModeAttr
            End Get
        End Property

#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 05/09/11</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles MyClass.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                ' XBC 05/05/2011 - timeout limit repetitions
                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    If MyClass.ReportCountTimeout = 0 Then
                        MyClass.ReportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        If MyClass.RecommendationsReport Is Nothing Then
                            ReDim MyClass.RecommendationsReport(0)
                        Else
                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                        End If
                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_COMM
                    End If

                    If pData.ToString = AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString Then
                        MyClass.StatusStressModeAttr = STRESS_STATUS.FINISHED_ERR
                    End If

                    Exit Sub
                End If
                ' XBC 05/05/2011 - timeout limit repetitions

                Select Case CurrentOperation

                    Case OPERATIONS.READ_STRESS
                        If pResponse = RESPONSE_TYPES.OK Then
                            ' Read Stress Mode values done !
                            myGlobal = ManageResultsStress()
                        End If
                End Select


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"
        '''' <summary>
        '''' Create the corresponding Script list according to the Screen Mode
        '''' </summary>
        '''' <param name="pMode">Screen mode</param>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created by: XBC 12/04/11
        '''' </remarks>
        'Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        ' Create the list of Scripts which are need to initialize this Adjustment
        '        Select Case pMode
        '            Case ADJUSTMENT_MODES.TESTING
        '                myResultData = Me.SendQueueForTESTING()

        '            Case ADJUSTMENT_MODES.TEST_EXITING
        '                myResultData = Me.SendQueueForTEST_EXITING()
        '        End Select
        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary>
        ''' Get the Demo Mode Test Parameters
        ''' </summary>
        ''' <param name="pAnalyzerModel">Identifier of the Analyzer model which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset</returns>
        ''' <remarks>Created by : XBC 14/05/2012</remarks>
        Public Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Try
                ' Timer to Request Status Stress
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_TIME_REQUEST_STRESS_MODE.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.TimerStatusModeStressAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Number of cycles for Demo Test
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_NUM_CYCLES_DEMO.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.NumCyclesAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.GetParameters", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Send High Level Demo Instruction 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 30/05/2011</remarks>
        Public Function SendDEMO_TEST() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try
                MyClass.StatusStressModeAttr = STRESS_STATUS.UNFINISHED

                CurrentOperation = OPERATIONS.SEND_STRESS

                myParams.Add(CInt(STRESS_TYPE.COMPLETE).ToString)
                myParams.Add(MyClass.NumCyclesAttr.ToString)
                myParams.Add(Now.Hour.ToString)
                myParams.Add(Now.Minute.ToString)
                myParams.Add(Now.Second.ToString)

                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SDMODE, True, Nothing, Nothing, "", myParams)

            Catch ex As Exception
                MyClass.StatusStressModeAttr = STRESS_STATUS.FINISHED_ERR
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.SendDEMO_TEST", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Send High Level Stress READ Instruction 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 14/05/2012</remarks>
        Public Function SendSDPOLL() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                CurrentOperation = OPERATIONS.READ_STRESS
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SDPOLL, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.SendSDPOLL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Send High Level Demo STOP Instruction 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 30/05/2011</remarks>
        Public Function SendDEMO_STOP() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try
                MyClass.StatusStressModeAttr = STRESS_STATUS.FINISHED_OK
                CurrentOperation = OPERATIONS.STOP_STRESS

                myParams.Add(CInt(STRESS_TYPE.STOP_SDMODE).ToString)
                myParams.Add(MyClass.NumCyclesAttr.ToString)
                myParams.Add(Now.Hour.ToString)
                myParams.Add(Now.Minute.ToString)
                myParams.Add(Now.Second.ToString)

                myResultData = Me.InsertReport("UTIL", "DEMO")
                If Not myResultData.HasError Then
                    myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SDMODE, True, Nothing, Nothing, "", myParams)
                End If

            Catch ex As Exception
                MyClass.StatusStressModeAttr = STRESS_STATUS.FINISHED_ERR
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.SendDEMO_STOP", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Method to decode the data information of the this screen from a String format source and obtain the data information easily legible
        ''' </summary>
        ''' <param name="pTask">task identifier</param>
        ''' <param name="pAction">task's action identifier</param>
        ''' <param name="pData">content data with the information to format</param>
        ''' <param name="pcurrentLanguage">language identifier to localize contents</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 06/09/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pcurrentLanguage As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Dim myUtility As New Utilities()
                Dim text1 As String
                Dim text As String = ""

                Dim j As Integer = 0
                ' Final Result
                If pData.Substring(j, 1) = "1" Then
                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DEMO_DONE", pcurrentLanguage)
                Else
                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                End If
                text += myUtility.FormatLineHistorics(text1)

                myResultData.SetDatos = text

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.DecodeDataReport", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Private Methods"
        ''' <summary> 
        ''' Routine called after receiving values for Stress Mode test obtained from the Instrument
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset</returns>
        ''' <remarks>Created by : XBC 14/05/2012</remarks>
        Private Function ManageResultsStress() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ReadStressModeData
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    MyClass.MyResultsStressTest = CType(myResultData.SetDatos, StressDataTO)

                    MyClass.StatusStressModeAttr = MyClass.MyResultsStressTest.Status

                Else
                    myResultData.HasError = True
                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.ManageResultsStress", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#Region "Historical reports"
        Public Function InsertReport(ByVal pTaskID As String, ByVal pActionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myHistoricalReportsDelegate As New HistoricalReportsDelegate
                Dim myHistoricReport As New SRVResultsServiceDS
                Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                myHistoricReportRow = myHistoricReport.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                myHistoricReportRow.TaskID = pTaskID
                myHistoricReportRow.ActionID = pActionID
                myHistoricReportRow.Data = MyClass.GenerateDataReport(myHistoricReportRow.TaskID, myHistoricReportRow.ActionID)
                myHistoricReportRow.AnalyzerID = AnalyzerId 'MyClass.AnalyzerIDAttr

                resultData = myHistoricalReportsDelegate.Add(Nothing, myHistoricReportRow)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    'Get the generated ID from the dataset returned 
                    Dim generatedID As Integer = -1
                    generatedID = DirectCast(resultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                    ' Insert recommendations if existing
                    If MyClass.RecommendationsReport IsNot Nothing Then
                        Dim myRecommendationsList As New SRVRecommendationsServiceDS
                        Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                        For i As Integer = 0 To MyClass.RecommendationsReport.Length - 1
                            myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                            myRecommendationsRow.ResultServiceID = generatedID
                            myRecommendationsRow.RecommendationID = CInt(MyClass.RecommendationsReport(i))
                            myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                        Next

                        resultData = myHistoricalReportsDelegate.AddRecommendations(Nothing, myRecommendationsList)
                        If resultData.HasError Then
                            resultData.HasError = True
                        End If
                        MyClass.RecommendationsReport = Nothing
                    End If
                Else
                    resultData.HasError = True
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.InsertReport", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Information of every test in this screen with its corresponding codification to be inserted in the common database of Historics
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 06/09/2011
        ''' 
        ''' Data Format : 
        ''' -----------
        ''' Mode operation done (1)
        ''' </remarks>
        Private Function GenerateDataReport(ByVal pTask As String, ByVal pAction As String) As String
            Dim returnValue As String = ""
            Try

                ' Demo completed
                returnValue = "1"

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DemoModeDelegate.GenerateDataReport", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function
#End Region

#End Region

    End Class


End Namespace

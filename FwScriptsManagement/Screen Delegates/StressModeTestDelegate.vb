Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class StressModeTestDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID)
            myFwScriptDelegate = pFwScriptsDelegate
            MyClass.ReportCountTimeout = 0
        End Sub

        Public Sub New()
            MyBase.New()
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
        Private CurrentOperation As OPERATIONS                  ' controls the operation which is currently executed in each time
        Private RecommendationsReport() As HISTORY_RECOMMENDATIONS
        Private ReportCountTimeout As Integer
        Private myGroupSeparator As String = SystemInfoManager.OSGroupSeparator 'RH 15/02/2012
#End Region

#Region "Attributes"
        Private StressTypeAttr As STRESS_TYPE
        Private TimeMachineCycleAttr As Integer     ' Cycle Time machine
        Private MaxCyclesAttr As Long               ' Maximum number of stress cycles allowed
        Private TimerStatusModeStressAttr As Long   ' time between each request of state to the analyzer
        Private NumCyclesAttr As Long               ' number of cycles defined by user
        Private HourStartStressAttr As Integer
        Private MinuteStartStressAttr As Integer
        Private SecondStartStressAttr As Integer
        'Private TimeStartStressTestAttr As DateTime ' Starting time of the stress test
        Private NumCompletedCyclesAttr As Long      ' Number of the cycles completed in the test
        Private TimeElapsedStressTestAttr As Long   ' Time elapsed since starting stress mode test
        Private NumResetsStressAttr As Integer      ' Number of resets accounted along stress mode test
        Private NumErrorsStressAttr As Integer      ' Number of errors accounted along stress mode test
        Private CyclesResetsStressAttr As List(Of Long) ' List of resets registered along stress mode test
        Private CodeErrorsStressAttr As List(Of String) ' List(Of STRESS_ERRORS)  ' List of errors registered along stress mode test    PDT !!!
        Private StatusStressModeAttr As STRESS_STATUS
        ' Language
        Private currentLanguageAttr As String

        Private AnalyzerIDAttr As String
#End Region

#Region "Properties"
        Public Property currentLanguage() As String
            Get
                Return MyClass.currentLanguageAttr
            End Get
            Set(ByVal value As String)
                MyClass.currentLanguageAttr = value
            End Set
        End Property

        Public Property StressType() As STRESS_TYPE
            Get
                Return StressTypeAttr
            End Get
            Set(ByVal value As STRESS_TYPE)
                StressTypeAttr = value
            End Set
        End Property

        Public Property TimeMachineCycle() As Integer
            Get
                Return TimeMachineCycleAttr
            End Get
            Set(ByVal value As Integer)
                TimeMachineCycleAttr = value
            End Set
        End Property

        Public Property MaxCycles() As Long
            Get
                Return MaxCyclesAttr
            End Get
            Set(ByVal value As Long)
                MaxCyclesAttr = value
            End Set
        End Property

        Public Property TimerStatusModeStress() As Long
            Get
                Return TimerStatusModeStressAttr
            End Get
            Set(ByVal value As Long)
                TimerStatusModeStressAttr = value
            End Set
        End Property

        Public Property NumCycles() As Long
            Get
                Return NumCyclesAttr
            End Get
            Set(ByVal value As Long)
                NumCyclesAttr = value
            End Set
        End Property

        'Public ReadOnly Property TimeStartStressTest() As DateTime
        '    Get
        '        Return TimeStartStressTestAttr
        '    End Get
        'End Property

        Public ReadOnly Property HourStartStress() As Integer
            Get
                Return HourStartStressAttr
            End Get
        End Property

        Public ReadOnly Property MinuteStartStress() As Integer
            Get
                Return MinuteStartStressAttr
            End Get
        End Property

        Public ReadOnly Property SecondStartStress() As Integer
            Get
                Return SecondStartStressAttr
            End Get
        End Property

        Public Property NumCompletedCycles() As Long
            Get
                Return NumCompletedCyclesAttr
            End Get
            Set(ByVal value As Long)
                NumCompletedCyclesAttr = value
            End Set
        End Property

        Public ReadOnly Property TimeElapsedStressTest() As Long
            Get
                Return TimeElapsedStressTestAttr
            End Get
        End Property

        Public ReadOnly Property NumResetsStress() As Integer
            Get
                Return MyClass.NumResetsStressAttr
            End Get
        End Property

        Public Property NumErrorsStress() As Integer
            Get
                Return MyClass.NumErrorsStressAttr
            End Get
            Set(ByVal value As Integer)
                NumErrorsStressAttr = value
            End Set
        End Property

        Public Property CyclesResetsStress() As List(Of Long)
            Get
                Return CyclesResetsStressAttr
            End Get
            Set(ByVal value As List(Of Long))
                CyclesResetsStressAttr = value
            End Set
        End Property

        Public ReadOnly Property CyclesResetsStressToString() As String
            Get
                Dim returnValue As String = ""

                If NumResetsStressAttr > 0 AndAlso CyclesResetsStressAttr.Count > 0 Then
                    For i As Integer = 0 To CyclesResetsStressAttr.Count - 1
                        returnValue += CyclesResetsStressAttr(i).ToString
                        If i < CyclesResetsStressAttr.Count Then
                            returnValue += " - "
                        End If
                    Next
                End If

                Return returnValue
            End Get
        End Property

        Public Property CodeErrorsStress() As List(Of String) '  STRESS_ERRORS) PDT !!!
            Get
                Return CodeErrorsStressAttr
            End Get
            Set(ByVal value As List(Of String)) ' STRESS_ERRORS))
                CodeErrorsStressAttr = value
            End Set
        End Property

        Public ReadOnly Property CodeErrorsStressToDS() As DataSet
            Get
                Dim returnValue As New DataSet
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Try
                    If NumErrorsStressAttr > 0 AndAlso CodeErrorsStressAttr.Count > 0 Then
                        Dim table As DataTable
                        Dim column As DataColumn
                        Dim row As DataRow
                        table = Nothing
                        column = Nothing
                        '// Create new DataTable.
                        table = New DataTable("ErrorsStress")
                        'creating own dataset to return
                        column = New DataColumn()
                        column.ColumnName = "Description"
                        table.Columns.Add(column)

                        returnValue.Tables.Add(table)

                        For i As Integer = 0 To CodeErrorsStressAttr.Count - 1
                            '// create new row
                            row = returnValue.Tables(0).NewRow()
                            Dim mytranslatedDesc As String
                            mytranslatedDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, CodeErrorsStressAttr(i), currentLanguage)
                            row(0) = mytranslatedDesc
                            returnValue.Tables(0).Rows.Add(row)
                        Next
                    End If
                Catch ex As Exception
                    returnValue = Nothing
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.Property CodeErrorsStressToDS", EventLogEntryType.Error, False)
                End Try
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property StatusStressMode() As STRESS_STATUS
            Get
                Return StatusStressModeAttr
            End Get
        End Property

        Public Property AnalyzerId() As String
            Get
                Return MyClass.AnalyzerIDAttr
            End Get
            Set(ByVal value As String)
                MyClass.AnalyzerIDAttr = value
            End Set
        End Property
#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 22/03/11</remarks>
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
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create the corresponding Script list according to the Screen Mode
        ''' </summary>
        ''' <param name="pMode">Screen mode</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XBC 22/03/11
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment
                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = MyBase.SendQueueForREADINGADJUSTMENTS()

                        'Case ADJUSTMENT_MODES.STRESS_READING
                        '    myResultData = MyClass.SendQueueForREADINGSTRESSSTATUS()

                        'Case ADJUSTMENT_MODES.TESTING
                        '    myResultData = MyClass.SendQueueForTESTING()

                        'Case ADJUSTMENT_MODES.TEST_EXITING
                        '    myResultData = MyClass.SendQueueForTEST_EXITING()
                End Select
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the Stress Mode Test Parameters
        ''' </summary>
        ''' <param name="pAnalyzerModel">Identifier of the Analyzer model which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset</returns>
        ''' <remarks>Created by : XBC 22/03/2011</remarks>
        Public Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Try
                ' Cycle Time machine
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_CYCLETIME_MACHINE.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.TimeMachineCycleAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Maximum cycles
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_CYCLES.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.MaxCyclesAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Timer to Request Status Stress
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_TIME_REQUEST_STRESS_MODE.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.TimerStatusModeStressAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.GetParameters", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the list of available single Elements to stressing
        ''' </summary>
        ''' <param name="pElement">element to stress</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset PreloadedMasterDataDS with the list of items in the
        '''          specified SubTable</returns>
        ''' <remarks>Created by : XBC 07/04/2011</remarks>
        Public Function ReadPartialElements(ByVal pElement As GlobalEnumerates.PreloadedMasterDataEnum) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                If (Not myResultData.HasError) Then
                    'Get the list of Movement Types as well as especified Arm
                    myResultData = myPreloadedMasterDataDelegate.GetList(Nothing, pElement)
                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.ReadPartialElements", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Send High Level Stress Instruction 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 23/05/2011</remarks>
        Public Function SendSDMODE() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try
                CurrentOperation = OPERATIONS.SEND_STRESS

                myParams.Add(CInt(MyClass.StressTypeAttr).ToString)
                myParams.Add(MyClass.NumCyclesAttr.ToString)
                myParams.Add(Now.Hour.ToString)
                myParams.Add(Now.Minute.ToString)
                myParams.Add(Now.Second.ToString)

                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SDMODE, True, Nothing, Nothing, "", myParams)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.SendSDMODE", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Send High Level Stress READ Instruction 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 23/05/2011</remarks>
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
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.SendSDPOLL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Send High Level Stress STOP Instruction 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 23/05/2011</remarks>
        Public Function SendSTRESS_STOP() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try
                ' registering the incidence in historical reports activity
                If MyClass.RecommendationsReport Is Nothing Then
                    ReDim MyClass.RecommendationsReport(0)
                Else
                    ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                End If
                MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.STRESS_STOP_BY_USER

                myParams.Add(CInt(STRESS_TYPE.STOP_SDMODE).ToString)
                myParams.Add(MyClass.NumCyclesAttr.ToString)
                myParams.Add(Now.Hour.ToString)
                myParams.Add(Now.Minute.ToString)
                myParams.Add(Now.Second.ToString)

                CurrentOperation = OPERATIONS.STOP_STRESS
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SDMODE, True, Nothing, Nothing, "", myParams)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.SendSTRESS_STOP", EventLogEntryType.Error, False)
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
        ''' <remarks>Created by XBC 31/08/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pcurrentLanguage As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Dim myUtility As New Utilities()
                Dim text1 As String
                Dim text As String = ""
                Dim StressTypeValue As STRESS_TYPE
                Dim Value As String
                Dim TimeValue As Integer

                'myResultData = MyClass.GetCultureInfo()
                'If myResultData.HasError Then
                '    Dim myLogAcciones As New ApplicationLogManager()
                '    myLogAcciones.CreateLogActivity(myResultData.ErrorMessage, "PhotometryAdjustmentDelegate.GenerateDataReport", EventLogEntryType.Error, False)
                '    Exit Try
                'End If

                Dim j As Integer = 0
                ' Final Result
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_TYPERES", pcurrentLanguage) + ": "
                StressTypeValue = CType(pData.Substring(j, 2), STRESS_TYPE)
                Select Case StressTypeValue
                    Case STRESS_TYPE.COMPLETE
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_COMPLETE", pcurrentLanguage)
                    Case STRESS_TYPE.SAMPLE_ARM_MH
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SampleArmH", pcurrentLanguage)
                    Case STRESS_TYPE.SAMPLE_ARM_MV
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SampleArmV", pcurrentLanguage)
                    Case STRESS_TYPE.REAGENT1_ARM_MH
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1H", pcurrentLanguage)
                    Case STRESS_TYPE.REAGENT1_ARM_MV
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1V", pcurrentLanguage)
                    Case STRESS_TYPE.REAGENT2_ARM_MH
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2H", pcurrentLanguage)
                    Case STRESS_TYPE.REAGENT2_ARM_MV
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2V", pcurrentLanguage)
                    Case STRESS_TYPE.MIXER1_ARM_MH
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer1H", pcurrentLanguage)
                    Case STRESS_TYPE.MIXER1_ARM_MV
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer1V", pcurrentLanguage)
                    Case STRESS_TYPE.MIXER2_ARM_MH
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer2H", pcurrentLanguage)
                    Case STRESS_TYPE.MIXER2_ARM_MV
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer2V", pcurrentLanguage)
                    Case STRESS_TYPE.SAMPLES_ROTOR
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_ROTOR_TYPES_SAMPLES", pcurrentLanguage)
                    Case STRESS_TYPE.REACTIONS_ROTOR
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REACT_ROTOR", pcurrentLanguage)
                    Case STRESS_TYPE.REAGENTS_ROTOR
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_ROTOR_TYPES_REAGENTS", pcurrentLanguage)
                    Case STRESS_TYPE.WASHING_STATION
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashStation", pcurrentLanguage)
                    Case STRESS_TYPE.SAMPLE_SYRINGE
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SamplesSyringe", pcurrentLanguage)
                    Case STRESS_TYPE.REAGENT1_SYRINGE
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1Syringe", pcurrentLanguage)
                    Case STRESS_TYPE.REAGENT2_SYRINGE
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2Syringe", pcurrentLanguage)
                End Select
                text += myUtility.FormatLineHistorics(text1)
                j += 2

                ' Cycles number programmed
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_CYCLES_PROG_RES", pcurrentLanguage) + ": "
                text1 += CSng(pData.Substring(j, 5)).ToString("##,##0")
                text += myUtility.FormatLineHistorics(text1)
                j += 5

                ' Total time programmed
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TIME_PROG", pcurrentLanguage) + ": "
                Value = pData.Substring(j, 8)
                If IsNumeric(Value) Then
                    TimeValue = CInt(Value)
                    text1 += myUtility.FormatToHHmmss(TimeValue)
                End If
                text += myUtility.FormatLineHistorics(text1)
                j += 8

                ' Cycles number completed
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_CYCLES_COMP_RES", pcurrentLanguage) + ": "
                text1 += CSng(pData.Substring(j, 5)).ToString("##,##0")
                text += myUtility.FormatLineHistorics(text1)
                j += 5

                ' Time completed
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TIME_COMP", pcurrentLanguage) + ": "
                Value = pData.Substring(j, 8)
                If IsNumeric(Value) Then
                    TimeValue = CInt(Value)
                    text1 += myUtility.FormatToHHmmss(TimeValue)
                End If
                text += myUtility.FormatLineHistorics(text1)
                j += 8

                ' Final Result
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FinalResult", pcurrentLanguage) + ": "
                If pData.Substring(j, 1) = "1" Then
                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                Else
                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                End If
                text += myUtility.FormatLineHistorics(text1)
                j += 1

                ' Number Resets detected
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NUM_CYCLES_RESETS", pcurrentLanguage) + ": "
                text1 += CSng(pData.Substring(j, 4)).ToString("#,##0")
                text += myUtility.FormatLineHistorics(text1)
                j += 4

                ' Number Errors detected
                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ERRNUM", pcurrentLanguage) + ": "
                text1 += CSng(pData.Substring(j, 4)).ToString("#,##0")
                text += myUtility.FormatLineHistorics(text1)

                ' XBC 13/09/2011 - put separator as a group thousands separator
                text = text.Replace(MyClass.myGroupSeparator.ToString, " ")

                myResultData.SetDatos = text

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.DecodeDataReport", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Private Methods"
        ''' <summary> 
        ''' Routine called after receiving values for Stress Mode test obtained from the Instrument
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset</returns>
        ''' <remarks>Created by : XBC 22/03/2011</remarks>
        Private Function ManageResultsStress() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ReadStressModeData
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    MyClass.MyResultsStressTest = CType(myResultData.SetDatos, StressDataTO)

                    MyClass.StressTypeAttr = MyClass.MyResultsStressTest.Type
                    MyClass.StatusStressModeAttr = MyClass.MyResultsStressTest.Status
                    MyClass.NumCyclesAttr = MyClass.MyResultsStressTest.NumCycles
                    MyClass.NumCompletedCyclesAttr = MyClass.MyResultsStressTest.NumCyclesCompleted
                    MyClass.TimeElapsedStressTestAttr = MyClass.TimeMachineCycleAttr * MyClass.MyResultsStressTest.NumCyclesCompleted
                    'MyClass.TimeStartStressTestAttr = MyClass.MyResultsStressTest.StartDatetime
                    MyClass.HourStartStressAttr = MyClass.MyResultsStressTest.StartHour
                    MyClass.MinuteStartStressAttr = MyClass.MyResultsStressTest.StartMinute
                    MyClass.SecondStartStressAttr = MyClass.MyResultsStressTest.StartSecond
                    MyClass.NumResetsStressAttr = MyClass.MyResultsStressTest.NumResets
                    MyClass.CyclesResetsStressAttr = MyClass.MyResultsStressTest.CyclesResets
                    MyClass.NumErrorsStressAttr = MyClass.MyResultsStressTest.NumErrors
                    ' MyClass.CodeErrorsStressAttr = MyClass.MyResultsStressTest.CodeErrors PDT !!!

                Else
                    myResultData.HasError = True
                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.ManageResultsStress", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        'RH 15/02/2012 Not needed. Removed.
        '''' <summary>
        '''' Control system info for separators formats
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 13/09/2011</remarks>
        'Private Function GetCultureInfo() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        myResultData = PCInfoReader.GetOSCultureInfo()
        '        Dim OSCultureInfo As New PCInfoReader.AX00PCOSCultureInfo
        '        If Not myResultData.HasError And Not myResultData Is Nothing Then
        '            OSCultureInfo = CType(myResultData.SetDatos, PCInfoReader.AX00PCOSCultureInfo)

        '            MyClass.myGroupSeparator = OSCultureInfo.GroupSeparator
        '        End If

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.GetCultureInfo", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

#End Region

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
                myHistoricReportRow.AnalyzerID = MyClass.AnalyzerIDAttr

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
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.InsertReport", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Information of every test in this screen with its corresponding codification to be inserted in the common database of Historics
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 31/08/2011
        ''' 
        ''' Data Format : 
        ''' -----------
        ''' Stress type (2)
        '''     1.Complete
        '''     2.Sample Arm Horizontal
        '''     3.Sample Arm Vertical
        '''     4.Reagent1 Arm Horizontal
        '''     5.Reagent1 Arm Vertical
        '''     6.Reagent2 Arm Horizontal
        '''     7.Reagent2 Arm Vertical
        '''     8.Mixer1 Arm Horizontal
        '''     9.Mixer1 Arm Vertical
        '''     10.Mixer2 Arm Horizontal
        '''     11.Mixer2 Arm Vertical
        '''     12.Samples Rotor
        '''     13.Reactions Rotor
        '''     14.Reagents Rotor
        '''     15.Washing Station
        '''     16.Sample Syringe
        '''     17.Reagent1 Syringe
        '''     18.Reagent2 Syringe
        ''' Cycles number programmed (5)
        ''' Total time programmed (8)
        ''' Cycles number completed (5)
        ''' Time completed (8)
        ''' Final Result (1) - NO=0 YES=1
        ''' Number Resets detected (4)
        ''' Number Errors detected (4)
        ''' </remarks>
        Private Function GenerateDataReport(ByVal pTask As String, ByVal pAction As String) As String
            Dim returnValue As String = ""
            Try

                returnValue = ""
                ' Stress type
                returnValue += CInt(MyClass.StressTypeAttr).ToString("00")

                ' Cycles number programmed
                returnValue += MyClass.NumCyclesAttr.ToString("00000")

                ' Total time programmed
                returnValue += (MyClass.NumCycles * MyClass.TimeMachineCycle).ToString("00000000")

                ' Cycles number completed
                returnValue += MyClass.NumCompletedCyclesAttr.ToString("00000")

                ' Time completed
                returnValue += MyClass.TimeElapsedStressTestAttr.ToString("00000000")

                ' Final Result
                If MyClass.NumResetsStressAttr > 0 Or MyClass.NumErrorsStressAttr > 0 Then
                    ' any dysfunction
                    returnValue += "2"
                Else
                    ' all is Ok
                    returnValue += "1"
                End If

                ' Number Resets detected
                returnValue += MyClass.NumResetsStressAttr.ToString("0000")

                ' Number Errors detected
                returnValue += MyClass.NumErrorsStressAttr.ToString("0000")

                ' Evalute possible resets and errors registered on stressing mode
                If MyClass.NumResetsStressAttr > 0 Then
                    ' registering the incidence in historical reports activity
                    If MyClass.RecommendationsReport Is Nothing Then
                        ReDim MyClass.RecommendationsReport(0)
                    Else
                        ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                    End If
                    MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.WARNING_STRESS_RESETS
                End If

                If MyClass.NumErrorsStressAttr > 0 Then
                    ' registering the incidence in historical reports activity
                    If MyClass.RecommendationsReport Is Nothing Then
                        ReDim MyClass.RecommendationsReport(0)
                    Else
                        ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                    End If
                    MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_STRESS
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "StressModeTestDelegate.GenerateDataReport", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function
#End Region

#Region "Simulate Mode"
        Public Sub SimulateTestStart()
            MyClass.StatusStressModeAttr = STRESS_STATUS.UNFINISHED
            MyClass.NumCompletedCyclesAttr = 1
            'MyClass.TimeStartStressTestAttr = DateTime.Now
            MyClass.HourStartStressAttr = DateTime.Now.Hour
            MyClass.MinuteStartStressAttr = DateTime.Now.Minute
            MyClass.SecondStartStressAttr = DateTime.Now.Second
            MyClass.TimeElapsedStressTestAttr = MyClass.TimeMachineCycleAttr * MyClass.NumCompletedCyclesAttr
            MyClass.NumResetsStressAttr = 0
            MyClass.NumErrorsStressAttr = 0
        End Sub

        Public Sub SimulateTestComplete()
            Dim Rnd As New Random()

            Dim value As Integer = Rnd.Next(0, 2)

            If value = 0 Then
                MyClass.StatusStressModeAttr = STRESS_STATUS.FINISHED_OK
                MyClass.NumCompletedCycles = 5
                'MyClass.TimeStartStressTestAttr = DateTime.Now
                MyClass.HourStartStressAttr = DateTime.Now.Hour
                MyClass.MinuteStartStressAttr = DateTime.Now.Minute
                MyClass.SecondStartStressAttr = DateTime.Now.Second
                MyClass.TimeElapsedStressTestAttr = MyClass.TimeMachineCycleAttr * MyClass.NumCompletedCyclesAttr
                MyClass.NumResetsStressAttr = 1
                MyClass.NumErrorsStressAttr = 0
            Else
                MyClass.StatusStressModeAttr = STRESS_STATUS.FINISHED_ERR
                MyClass.NumCompletedCycles = 3
                'MyClass.TimeStartStressTestAttr = DateTime.Now
                MyClass.HourStartStressAttr = DateTime.Now.Hour
                MyClass.MinuteStartStressAttr = DateTime.Now.Minute
                MyClass.SecondStartStressAttr = DateTime.Now.Second
                MyClass.TimeElapsedStressTestAttr = MyClass.TimeMachineCycleAttr * MyClass.NumCompletedCyclesAttr
                MyClass.NumResetsStressAttr = 1
                MyClass.NumErrorsStressAttr = 3

                Dim myResets As New List(Of Long)
                myResets.Add(2)
                MyClass.CyclesResetsStressAttr = myResets

                ' PDT !!!
                'Dim myErrors As New List(Of STRESS_ERRORS)
                'myErrors.Add(STRESS_ERRORS.ERROR1)
                'myErrors.Add(STRESS_ERRORS.ERROR2)
                'myErrors.Add(STRESS_ERRORS.ERROR3)
                'MyClass.CodeErrorsStressAttr = myErrors
                Dim myErrors As New List(Of String)
                myErrors.Add("ERROR1")
                myErrors.Add("ERROR2")
                myErrors.Add("ERROR3")
                MyClass.CodeErrorsStressAttr = myErrors

            End If
        End Sub
#End Region

    End Class

End Namespace
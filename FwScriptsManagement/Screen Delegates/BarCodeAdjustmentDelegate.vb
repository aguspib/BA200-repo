Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.App

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class BarCodeAdjustmentDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate

            MyClass.Initializations()
        End Sub

        Public Sub New()
            MyBase.New() 'SGM 20/01/2012
        End Sub
#End Region

#Region "Declarations"

        Public CurrentOperation As OPERATIONS                  ' controls the operation which is currently executed in each time
        Private RecommendationsList As New List(Of HISTORY_RECOMMENDATIONS)
        Private ReportCountTimeout As Integer
        Private RecommendationsReport() As HISTORY_RECOMMENDATIONS
        Private myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator 'RH 15/02/2012 Get the static version
        Private myGroupSeparator As String = SystemInfoManager.OSGroupSeparator 'RH 15/02/2012 Get the static version

#End Region

#Region "Attributes"
        ' Language
        Private currentLanguageAttr As String
        Private ReadSignalOkAttr As Integer

        Private SamplesRotorBCPositionAttr As String
        Private ReagentsRotorBCPositionAttr As String

        Private pAxisAdjustAttr As AXIS
        Private pMovAdjustAttr As MOVEMENT
        Private pValueAdjustAttr As String = ""

        ' Operations done
        Private LoadAdjDoneAttr As Boolean
        Private HomesDoneAttr As Boolean
        Private TestDoneAttr As Boolean
        Private TestModeDoneAttr As Boolean
        Private TestModeEndedAttr As Boolean

        Private NoneInstructionToSendAttr As Boolean

        ' Time expected for current operation
        Private CurrentTimeOperationAttr As Integer

        'Private AnalyzerIDAttr As String

        Private AdjustmentIDAttr As ADJUSTMENT_GROUPS
        Private AdjustmentBCPointAttr As Single
        Private BcResultsAttr As BarCodeResults()
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

        Public Property ReadSignalOk() As Integer
            Get
                Return MyClass.ReadSignalOkAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.ReadSignalOkAttr = value
            End Set
        End Property

        Public Property SamplesRotorBCPosition() As String
            Get
                Return MyClass.SamplesRotorBCPositionAttr
            End Get
            Set(ByVal value As String)
                MyClass.SamplesRotorBCPositionAttr = value
            End Set
        End Property

        Public Property ReagentsRotorBCPosition() As String
            Get
                Return MyClass.ReagentsRotorBCPositionAttr
            End Get
            Set(ByVal value As String)
                MyClass.ReagentsRotorBCPositionAttr = value
            End Set
        End Property

        Public Property pAxisAdjust() As AXIS
            Get
                Return Me.pAxisAdjustAttr
            End Get
            Set(ByVal value As AXIS)
                Me.pAxisAdjustAttr = value
            End Set
        End Property

        Public Property pMovAdjust() As MOVEMENT
            Get
                Return Me.pMovAdjustAttr
            End Get
            Set(ByVal value As MOVEMENT)
                Me.pMovAdjustAttr = value
            End Set
        End Property

        Public Property pValueAdjust() As String
            Get
                Return Me.pValueAdjustAttr
            End Get
            Set(ByVal value As String)
                Me.pValueAdjustAttr = value
            End Set
        End Property

        Public Property LoadAdjDone() As Boolean
            Get
                Return Me.LoadAdjDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.LoadAdjDoneAttr = value
            End Set
        End Property

        Public Property NoneInstructionToSend() As Boolean
            Get
                Return NoneInstructionToSendAttr
            End Get
            Set(ByVal value As Boolean)
                NoneInstructionToSendAttr = value
            End Set
        End Property

        Public Property HomesDone() As Boolean
            Get
                Return HomesDoneAttr
            End Get
            Set(ByVal value As Boolean)
                HomesDoneAttr = value
            End Set
        End Property

        Public Property TestDone() As Boolean
            Get
                Return MyClass.TestDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.TestDoneAttr = value
            End Set
        End Property

        Public Property TestModeDone() As Boolean
            Get
                Return MyClass.TestModeDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.TestModeDoneAttr = value
            End Set
        End Property

        Public Property TestModeEnded() As Boolean
            Get
                Return MyClass.TestModeEndedAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.TestModeEndedAttr = value
            End Set
        End Property

        Public ReadOnly Property CurrentTimeOperation() As Integer
            Get
                Return CurrentTimeOperationAttr
            End Get
        End Property

        'Public Property AnalyzerId() As String
        '    Get
        '        Return MyClass.AnalyzerIDAttr
        '    End Get
        '    Set(ByVal value As String)
        '        MyClass.AnalyzerIDAttr = value
        '    End Set
        'End Property

        Public Property AdjustmentID() As ADJUSTMENT_GROUPS
            Get
                Return MyClass.AdjustmentIDAttr
            End Get
            Set(ByVal value As ADJUSTMENT_GROUPS)
                MyClass.AdjustmentIDAttr = value
            End Set
        End Property

        Public WriteOnly Property AdjustmentBCPoint() As Single
            Set(ByVal value As Single)
                MyClass.AdjustmentBCPointAttr = value
            End Set
        End Property

        Public ReadOnly Property BcResultsCount() As Integer
            Get
                Dim returnValue As Integer

                If MyClass.BcResultsAttr Is Nothing Then
                    returnValue = 0
                Else
                    returnValue = UBound(MyClass.BcResultsAttr) + 1
                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property BcResults() As BarCodeResults()
            Get
                Return MyClass.BcResultsAttr
            End Get
        End Property

#End Region

#Region "Enumerations"
        Public Enum OPERATIONS
            NONE
            HOMES
            SAVE_ADJUSMENTS
            TEST
            TEST_MODE
            TEST_MODE_END
        End Enum
#End Region

#Region "Structures"
        Public Structure BarCodeResults
            Dim Position As Integer
            Dim Value As String
        End Structure
#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 15/12/2011</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles Me.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                ' timeout limit repetitions
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


                    Exit Sub
                End If
                ' timeout limit repetitions

                Select Case MyClass.CurrentOperation

                    Case OPERATIONS.SAVE_ADJUSMENTS
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                myGlobal = ManageSavingBC()
                                If Not myGlobal.HasError Then
                                    Me.LoadAdjDoneAttr = True
                                End If
                        End Select


                    Case OPERATIONS.HOMES
                        Select Case pResponse
                            Case RESPONSE_TYPES.OK
                                Me.HomesDoneAttr = True
                        End Select


                    Case OPERATIONS.TEST
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                CurrentTimeOperationAttr = AnalyzerController.Instance.Analyzer.MaxWaitTime '#REFACTORING
                            Case RESPONSE_TYPES.OK
                                Me.TestDoneAttr = True
                        End Select

                    Case OPERATIONS.TEST_MODE
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing to do
                            Case RESPONSE_TYPES.OK
                                Me.TestModeDoneAttr = True
                        End Select

                    Case OPERATIONS.TEST_MODE_END
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing to do
                            Case RESPONSE_TYPES.OK
                                Me.TestModeEndedAttr = True
                        End Select

                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create the corresponding Script list according to the Screen Mode
        ''' </summary>
        ''' <param name="pMode">Screen mode</param>
        ''' <param name="pAdjustmentGroup">Adjustment type</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 15/12/2011
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES, _
                                               Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment

                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = MyBase.SendQueueForREADINGADJUSTMENTS()

                    Case ADJUSTMENT_MODES.LOADING
                        myResultData = Me.SendQueueForLOADING()

                    Case ADJUSTMENT_MODES.TEST_EXITING
                        myResultData = Me.SendQueueForTEST_EXITING()

                    Case ADJUSTMENT_MODES.ADJUST_PREPARING
                        myResultData = Me.SendQueueForADJUST_PREPARING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.ADJUSTING
                        myResultData = Me.SendQueueForADJUSTING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.TESTING
                        myResultData = Me.SendQueueForTESTING(pAdjustmentGroup)

                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Sub Initializations()
            ' Pending of specified implementations...
        End Sub

        ''' <summary>
        ''' Get the BarCode Parameters
        ''' </summary>
        ''' <param name="pAnalyzerModel">Identifier of the Analyzer model which the data will be obtained</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 14/12/2011</remarks>
        Public Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Try
                ' Read Signal Quality recommendation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_BARCODE_READ_SIGNAL_OK.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReadSignalOkAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.GetParameters", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Load Adjustments High Level Instruction to save values into the instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 15/12/2011</remarks>
        Public Function SendLoad_Adjustments() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Me.CurrentOperation = OPERATIONS.SAVE_ADJUSMENTS
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, True, Nothing, Me.pValueAdjustAttr) '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendLOAD_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' High Level Instruction to request Barcode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 16/12/2011</remarks>
        Public Function SendBARCODE_REQUEST(ByVal pBarCodeDS As AnalyzerManagerDS, ByVal pOperation As OPERATIONS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Me.CurrentOperation = pOperation
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, pBarCodeDS, "") '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendBARCODE_REQUEST", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by XBC 16/12/2011</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                If pRefreshDS.RotorPositionsChanged.Rows.Count > 0 Then
                    For Each updatedRow As UIRefreshDS.RotorPositionsChangedRow In pRefreshDS.RotorPositionsChanged.Rows
                        If MyClass.CurrentOperation = OPERATIONS.TEST Then
                            If updatedRow.BarcodeStatus = "OK" Then
                                MyClass.BcResultsAdd(updatedRow.CellNumber, updatedRow.BarCodeInfo)
                            End If
                        Else
                            MyClass.BcResultsAttr = Nothing ' XBC 09/10/2012 - Correction - into test mode the value to read always is the first who overwrite the before read value
                            MyClass.BcResultsAdd(updatedRow.CellNumber, updatedRow.BarCodeInfo)
                        End If
                    Next
                End If

                If MyClass.CurrentOperation = OPERATIONS.TEST Then
                    ' Insert the new activity into Historic reports
                    myResultData = InsertReport("TEST", "BARCODE")
                End If

                MyClass.ScreenReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

        Public Sub BcResultsAdd(ByVal pPosition As Integer, ByVal pValue As String)
            Dim myResultData As New GlobalDataTO
            Try
                If MyClass.BcResultsAttr Is Nothing Then
                    ReDim MyClass.BcResultsAttr(0)
                Else
                    ReDim Preserve MyClass.BcResultsAttr(UBound(MyClass.BcResultsAttr) + 1)
                End If
                MyClass.BcResultsAttr(UBound(MyClass.BcResultsAttr)).Position = pPosition
                MyClass.BcResultsAttr(UBound(MyClass.BcResultsAttr)).Value = pValue

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.BcResultsAdd", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Method to decode the data information of the this screen from a String format source and obtain the data information easily legible
        ''' </summary>
        ''' <param name="pTask">task identifier</param>
        ''' <param name="pAction">task's action identifier</param>
        ''' <param name="pData">content data with the information to format</param>
        ''' <param name="pcurrentLanguage">language identifier to localize contents</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 20/12/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pcurrentLanguage As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                'Dim Utilities As New Utilities()
                Dim text1 As String
                Dim text As String = ""

                'myResultData = MyClass.GetCultureInfo()
                'If myResultData.HasError Then
                '    'Dim myLogAcciones As New ApplicationLogManager()
                '    GlobalBase.CreateLogActivity(myResultData.ErrorMessage, "BarCodeAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)
                '    Exit Try
                'End If

                Dim j As Integer = 0
                Select Case pTask
                    Case "ADJUST"

                        ' selected Rotor
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "GRID_SRV_ROTOR", pcurrentLanguage) + ": "
                        If pData.Substring(j, 1) = "1" Then
                            ' alignment...
                            text1 += Utilities.SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage)
                        Else
                            ' alignment...
                            text1 += Utilities.SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage)
                        End If
                        text += Utilities.FormatLineHistorics(text1)
                        j += 1

                        ' Barcode point value
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_BarCode", pcurrentLanguage) + ": "
                        ' alignment...
                        text1 += Utilities.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("##,##0").Length)
                        ' content
                        text1 += CSng(pData.Substring(j, 5)).ToString("##,##0")
                        text += Utilities.FormatLineHistorics(text1)
                        j += 5

                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUSTED", pcurrentLanguage) + ":"
                        ' alignment...
                        text1 += Utilities.SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage).Length)
                        ' content
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage)
                        text += Utilities.FormatLineHistorics(text1)

                    Case "TEST"

                        ' selected Rotor
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "GRID_SRV_ROTOR", pcurrentLanguage) + ": "
                        If pData.Substring(j, 1) = "1" Then
                            ' alignment...
                            text1 += Utilities.SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage)
                        Else
                            ' alignment...
                            text1 += Utilities.SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage)
                        End If
                        text += Utilities.FormatLineHistorics(text1)
                        j += 1

                        text += Environment.NewLine
                        ' detected tubes
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TubesDetected", pcurrentLanguage) + ": "
                        text += Utilities.FormatLineHistorics(text1)

                        Dim detectedTubes As Integer
                        If IsNumeric(pData.Substring(j, 3)) Then
                            detectedTubes = CInt(pData.Substring(j, 3))
                            j += 3

                            For i As Integer = 0 To detectedTubes - 1
                                text += Environment.NewLine
                                ' tube position
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TUBE_POSITION", pcurrentLanguage) + ": "
                                ' alignment...
                                text1 += Utilities.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 3).Length)
                                ' content
                                text1 += pData.Substring(j, 3)
                                text += Utilities.FormatLineHistorics(text1)
                                j += 3

                                ' barcode value (searching #%# character...)
                                Dim tmptext As String
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_BarCode", pcurrentLanguage) + ": "

                                tmptext = pData.Substring(j, pData.Length - j)
                                Dim k As Integer
                                k = tmptext.IndexOf("#%#")
                                ' alignment...
                                text1 += Utilities.SetSpaces(22 - text1.Length - 1 - tmptext.Substring(0, k).Length)
                                ' content
                                text1 += tmptext.Substring(0, k)
                                text += Utilities.FormatLineHistorics(text1)
                                j += k + 3

                            Next
                        End If


                End Select

                ' put separator as a group thousands separator
                text = text.Replace(MyClass.myGroupSeparator.ToString, " ")

                myResultData.SetDatos = text

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Creates the Script List for Screen Loading operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 30/30/2012</remarks>
        Private Function SendQueueForLOADING() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                ' Initializations 
                MyClass.BcResultsAttr = Nothing

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Script1
                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .FwScriptID = FwSCRIPTS_IDS.SWITCH_ON_BARCODE.ToString
                    .ParamList = Nothing
                End With

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForLOADING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Test exiting operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 30/30/2012</remarks>
        Private Function SendQueueForTEST_EXITING() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Script1
                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .FwScriptID = FwSCRIPTS_IDS.SWITCH_OFF_BARCODE.ToString
                    .ParamList = Nothing
                End With

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForTEST_EXITING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Adjust Preparing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 15/12/2010</remarks>
        Private Function SendQueueForADJUST_PREPARING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'get the pending Homes  
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDS As SRVPreliminaryHomesDS
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, pAdjustment.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

                    Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
                                    Where a.Done = False Select a).ToList

                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                        Dim myFwScript As New FwScriptQueueItem
                        myListFwScript.Add(myFwScript)
                    Next

                    Dim i As Integer = 0
                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                        'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                        If i = myListFwScript.Count - 1 Then
                            'Last index
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myFwScript1
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myFwScript1
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        Else
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myListFwScript(i + 1)
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myListFwScript(i + 1)
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        End If
                        i += 1
                    Next

                End If

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC

                        ' Mov ABS Reactions Rotor
                        ' Absolute positioning of the samples rotor to a predefined position (Barcode) 
                        With myFwScript1
                            .FwScriptID = FwSCRIPTS_IDS.SAMPLES_ABS_ROTOR.ToString
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(MyClass.SamplesRotorBCPositionAttr)
                        End With

                        'add to the queue list
                        If myListFwScript.Count > 0 Then
                            For i As Integer = 0 To myListFwScript.Count - 1
                                If i = 0 Then
                                    ' First Script
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                                Else
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                                End If
                            Next
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                        End If


                    Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC

                        ' Mov ABS Reactions Rotor
                        ' Absolute positioning of the samples rotor to a predefined position (Barcode) 
                        With myFwScript1
                            .FwScriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR.ToString
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(MyClass.ReagentsRotorBCPositionAttr)
                        End With

                        'add to the queue list
                        If myListFwScript.Count > 0 Then
                            For i As Integer = 0 To myListFwScript.Count - 1
                                If i = 0 Then
                                    ' First Script
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                                Else
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                                End If
                            Next
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                        End If



                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForADJUST_PREPARING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Adjusting operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 15/12/2011</remarks>
        Private Function SendQueueForADJUSTING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Script1
                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                    Select Case pAdjustment

                        Case ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                            Select Case pMovAdjust
                                Case MOVEMENT.HOME
                                    ' samples rotor Home
                                    .FwScriptID = FwSCRIPTS_IDS.SAMPLES_HOME_ROTOR.ToString
                                    .ParamList = Nothing
                                Case MOVEMENT.ABSOLUTE
                                    ' Mov ABS
                                    ' Absolute positioning of the samples rotor
                                    .FwScriptID = FwSCRIPTS_IDS.SAMPLES_ABS_ROTOR.ToString
                                    ' expects 1 param
                                    .ParamList = New List(Of String)
                                    .ParamList.Add(Me.pValueAdjustAttr)
                                Case MOVEMENT.RELATIVE
                                    ' Mov REL
                                    ' Relative positioning of the samples rotor for x steps
                                    .FwScriptID = FwSCRIPTS_IDS.SAMPLES_REL_ROTOR.ToString
                                    ' expects 1 param
                                    .ParamList = New List(Of String)
                                    .ParamList.Add(Me.pValueAdjustAttr)
                            End Select

                        Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                            Select Case pMovAdjust
                                Case MOVEMENT.HOME
                                    ' reagents rotor Home
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENTS_HOME_ROTOR.ToString
                                    .ParamList = Nothing
                                Case MOVEMENT.ABSOLUTE
                                    ' Mov ABS
                                    ' Absolute positioning of the reagents rotor
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR.ToString
                                    ' expects 1 param
                                    .ParamList = New List(Of String)
                                    .ParamList.Add(Me.pValueAdjustAttr)
                                Case MOVEMENT.RELATIVE
                                    ' Mov REL
                                    ' Relative positioning of the reagents rotor for x steps
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENTS_REL_ROTOR.ToString
                                    ' expects 1 param
                                    .ParamList = New List(Of String)
                                    .ParamList.Add(Me.pValueAdjustAttr)
                            End Select


                    End Select

                End With

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForADJUSTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Testing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 16/12/11</remarks>
        Private Function SendQueueForTESTING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                ' Initializations 
                MyClass.BcResultsAttr = Nothing

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'get the pending Homes
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDS As SRVPreliminaryHomesDS
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, pAdjustment.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

                    Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
                                    Where a.Done = False Select a).ToList


                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                        Dim myFwScript As New FwScriptQueueItem
                        myListFwScript.Add(myFwScript)
                    Next

                    Dim i As Integer = 0
                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList

                        'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                        If i = myListFwScript.Count - 1 Then
                            'Last index
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myFwScript1
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myFwScript1
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        Else
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myListFwScript(i + 1)
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myListFwScript(i + 1)
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        End If
                        i += 1

                    Next

                End If


                'Script1
                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .FwScriptID = FwSCRIPTS_IDS.SWITCH_ON_BARCODE.ToString
                    .ParamList = Nothing
                End With


                Me.CurrentOperation = OPERATIONS.HOMES

                'add to the queue list
                If myListFwScript.Count > 0 Then

                    For i As Integer = 0 To myListFwScript.Count - 1
                        If i = 0 Then
                            ' First Script
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                        End If
                    Next
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                Else
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                    'Else
                    '    MyClass.HomesDoneAttr = True
                    '    MyClass.NoneInstructionToSend = False
                End If


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary> 
        ''' Routine called after saving the CodeBar point adjustment 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 20/12/2011
        ''' </remarks>
        Private Function ManageSavingBC() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                ' Insert the new activity into Historic reports
                myResultData = InsertReport("ADJUST", "BARCODE")

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.ManageSavingBC", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#Region "Generic"

        'RH 15/92/2012 Not needed. Removed.
        '''' <summary>
        '''' Control system info for separators formats
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 20/12/2011</remarks>
        'Private Function GetCultureInfo() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        myResultData = PCInfoReader.GetOSCultureInfo()
        '        Dim OSCultureInfo As New PCInfoReader.AX00PCOSCultureInfo
        '        If Not myResultData.HasError And Not myResultData Is Nothing Then
        '            OSCultureInfo = CType(myResultData.SetDatos, PCInfoReader.AX00PCOSCultureInfo)

        '            MyClass.myDecimalSeparator = OSCultureInfo.DecimalSeparator
        '            MyClass.myGroupSeparator = OSCultureInfo.GroupSeparator
        '        End If

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.GetCultureInfo", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

#End Region

#Region "Historical reports"
        Public Function InsertReport(ByVal pTaskID As String, ByVal pActionID As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myHistoricalReportsDelegate As New HistoricalReportsDelegate
                Dim myHistoricReport As New SRVResultsServiceDS
                Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                myHistoricReportRow = myHistoricReport.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                myHistoricReportRow.TaskID = pTaskID
                myHistoricReportRow.ActionID = pActionID
                myHistoricReportRow.Data = GenerateDataReport(myHistoricReportRow.TaskID, myHistoricReportRow.ActionID)
                myHistoricReportRow.AnalyzerID = AnalyzerId

                myResultData = myHistoricalReportsDelegate.Add(Nothing, myHistoricReportRow)
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    'Get the generated ID from the dataset returned 
                    Dim generatedID As Integer = -1
                    generatedID = DirectCast(myResultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

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

                        myResultData = myHistoricalReportsDelegate.AddRecommendations(Nothing, myRecommendationsList)
                        If myResultData.HasError Then
                            myResultData.HasError = True
                        End If
                        MyClass.RecommendationsReport = Nothing
                    End If
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.InsertReport", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Information of every test in this screen with its corresponding codification to be inserted in the common database of Historics
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 20/12/2011
        ''' 
        ''' Data Format : 
        ''' -----------
        ''' Adjusting : 
        ''' Selected Rotor (1) - Sample=1 Reagent=2
        ''' BarCode point adjusted (5)
        ''' Testing :
        ''' Selected Rotor (1) - Sample=1 Reagent=2
        ''' Number of tube codebar detected (3)
        ''' - Following fields repeated for every BC Tube detected : -
        ''' Tube position (3)
        ''' BarCode value (x)
        ''' </remarks>
        Private Function GenerateDataReport(ByVal pTask As String, ByVal pAction As String) As String
            'Dim myResultData As New GlobalDataTO
            Dim returnValue As String = ""
            Try
                'myResultData = MyClass.GetCultureInfo()
                'If myResultData.HasError Then
                '    'Dim myLogAcciones As New ApplicationLogManager()
                '    GlobalBase.CreateLogActivity(myResultData.ErrorMessage, "BarCodeAdjustmentDelegate.GenerateDataReport", EventLogEntryType.Error, False)
                '    Exit Try
                'End If

                Select Case pTask
                    Case "ADJUST"

                        Select Case pAction
                            Case "BARCODE"

                                returnValue = ""
                                ' Selected rotor
                                If MyClass.AdjustmentIDAttr = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' BarCode point value
                                returnValue += MyClass.AdjustmentBCPointAttr.ToString("00000")

                        End Select


                    Case "TEST"

                        Select Case pAction
                            Case "BARCODE"

                                returnValue = ""
                                ' Selected rotor
                                If MyClass.AdjustmentIDAttr = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Number of tube codebar detected
                                If MyClass.BcResultsAttr Is Nothing Then
                                    returnValue += "0"
                                Else
                                    returnValue += MyClass.BcResultsAttr.Count.ToString("000")

                                    For i As Integer = 0 To MyClass.BcResultsAttr.Count - 1
                                        ' Tube position
                                        returnValue += MyClass.BcResultsAttr(i).Position.ToString("000")

                                        ' BarCode value
                                        returnValue += MyClass.BcResultsAttr(i).Value.ToString()
                                        ' Separator
                                        returnValue += "#%#"
                                    Next
                                End If

                        End Select
                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.GenerateDataReport", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function
#End Region

#End Region

    End Class

End Namespace

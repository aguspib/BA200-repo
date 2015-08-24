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
        Public Sub New(pAnalyzerID As String, pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate

            Initializations()
        End Sub

        Public Sub New()
            MyBase.New() 'SGM 20/01/2012
        End Sub
#End Region

#Region "Declarations"

        Public CurrentOperation As OPERATIONS                  ' controls the operation which is currently executed in each time
        Private _reportCountTimeout As Integer
        Private _recommendationsReport() As HISTORY_RECOMMENDATIONS
        Private ReadOnly _myGroupSeparator As String = SystemInfoManager.OSGroupSeparator 'RH 15/02/2012 Get the static version

#End Region

#Region "Attributes"
        ' Language
        Private _currentLanguageAttr As String
        Private _readSignalOkAttr As Integer

        Private _samplesRotorBcPositionAttr As String
        Private _reagentsRotorBcPositionAttr As String

        Private _pAxisAdjustAttr As AXIS
        Private _pMovAdjustAttr As MOVEMENT
        Private _pValueAdjustAttr As String = ""

        ' Operations done
        Private _loadAdjDoneAttr As Boolean
        Private _homesDoneAttr As Boolean
        Private _testDoneAttr As Boolean
        Private _testModeDoneAttr As Boolean
        Private _testModeEndedAttr As Boolean

        Private _noneInstructionToSendAttr As Boolean

        ' Time expected for current operation
        Private _currentTimeOperationAttr As Integer

        Private _adjustmentIDAttr As ADJUSTMENT_GROUPS
        Private _adjustmentBcPointAttr As Single
        Private _bcResultsAttr As BarCodeResults()
#End Region

#Region "Properties"
        Public Property CurrentLanguage As String
            Get
                Return _currentLanguageAttr
            End Get
            Set(value As String)
                _currentLanguageAttr = value
            End Set
        End Property

        Private _barcodeLaserEnabled As Boolean
        Public Property BarcodeLaserEnabled As Boolean
            Get
                Return _barcodeLaserEnabled
            End Get
            Set(value As Boolean)
                Dim myResultData As New GlobalDataTO
                Dim myFwScript1 As New FwScriptQueueItem
                Try
                    ' Initializations 
                    _bcResultsAttr = Nothing

                    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then myFwScriptDelegate.CurrentFwScriptsQueue.Clear()

                    'Script1
                    With myFwScript1
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing
                        .FwScriptID = If(value, FwSCRIPTS_IDS.SWITCH_ON_BARCODE.ToString, FwSCRIPTS_IDS.SWITCH_OFF_BARCODE.ToString)
                        .ParamList = Nothing
                    End With

                    'add to the queue list
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                    If Not myResultData.HasError Then
                        If NoneInstructionToSend Then
                            myResultData = myFwScriptDelegate.StartFwScriptQueue
                        End If
                    End If


                Catch ex As Exception
                    myResultData.HasError = True
                    myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                    myResultData.ErrorMessage = ex.Message

                    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                        myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                    End If

                    CreateLogActivity(ex)
                End Try
                _barcodeLaserEnabled = value
            End Set
        End Property


        Public Property ReadSignalOk As Integer
            Get
                Return _readSignalOkAttr
            End Get
            Set(value As Integer)
                _readSignalOkAttr = value
            End Set
        End Property

        Public Property SamplesRotorBcPosition As String
            Get
                Return _samplesRotorBcPositionAttr
            End Get
            Set(value As String)
                _samplesRotorBcPositionAttr = value
            End Set
        End Property

        Public Property ReagentsRotorBcPosition As String
            Get
                Return _reagentsRotorBcPositionAttr
            End Get
            Set(value As String)
                _reagentsRotorBcPositionAttr = value
            End Set
        End Property

        Public Property PAxisAdjust As AXIS
            Get
                Return _pAxisAdjustAttr
            End Get
            Set(value As AXIS)
                _pAxisAdjustAttr = value
            End Set
        End Property

        Public Property PMovAdjust As MOVEMENT
            Get
                Return _pMovAdjustAttr
            End Get
            Set(value As MOVEMENT)
                _pMovAdjustAttr = value
            End Set
        End Property

        Public Property PValueAdjust As String
            Get
                Return _pValueAdjustAttr
            End Get
            Set(value As String)
                _pValueAdjustAttr = value
            End Set
        End Property

        Public Property LoadAdjDone As Boolean
            Get
                Return _loadAdjDoneAttr
            End Get
            Set(value As Boolean)
                _loadAdjDoneAttr = value
            End Set
        End Property

        Public Property NoneInstructionToSend As Boolean
            Get
                Return _noneInstructionToSendAttr
            End Get
            Set(value As Boolean)
                _noneInstructionToSendAttr = value
            End Set
        End Property

        Public Property HomesDone As Boolean
            Get
                Return _homesDoneAttr
            End Get
            Set(value As Boolean)
                _homesDoneAttr = value
            End Set
        End Property

        Public Property TestDone As Boolean
            Get
                Return _testDoneAttr
            End Get
            Set(value As Boolean)
                _testDoneAttr = value
            End Set
        End Property

        Public Property TestModeDone As Boolean
            Get
                Return _testModeDoneAttr
            End Get
            Set(value As Boolean)
                _testModeDoneAttr = value
            End Set
        End Property

        Public Property TestModeEnded As Boolean
            Get
                Return _testModeEndedAttr
            End Get
            Set(value As Boolean)
                _testModeEndedAttr = value
            End Set
        End Property

        Public ReadOnly Property CurrentTimeOperation As Integer
            Get
                Return _currentTimeOperationAttr
            End Get
        End Property

        Public Property AdjustmentID As ADJUSTMENT_GROUPS
            Get
                Return _adjustmentIDAttr
            End Get
            Set(value As ADJUSTMENT_GROUPS)
                _adjustmentIDAttr = value
            End Set
        End Property

        Public WriteOnly Property AdjustmentBcPoint As Single
            Set(value As Single)
                _adjustmentBcPointAttr = value
            End Set
        End Property

        Public ReadOnly Property BcResultsCount As Integer
            Get
                Dim returnValue As Integer

                If _bcResultsAttr Is Nothing Then
                    returnValue = 0
                Else
                    returnValue = UBound(_bcResultsAttr) + 1
                End If

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property BcResults As BarCodeResults()
            Get
                Return _bcResultsAttr
            End Get
        End Property

#End Region

#Region "Enumerations"
        Public Enum Operations
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
        Private Sub ScreenReceptionLastFwScriptEvent(pResponse As RESPONSE_TYPES, pData As Object) Handles Me.ReceivedLastFwScriptEvent
            Try
                'manage special operations according to the screen characteristics
                ' timeout limit repetitions
                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    If _reportCountTimeout = 0 Then
                        _reportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        If _recommendationsReport Is Nothing Then
                            ReDim _recommendationsReport(0)
                        Else
                            ReDim Preserve _recommendationsReport(UBound(_recommendationsReport) + 1)
                        End If
                        _recommendationsReport(UBound(_recommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_COMM
                    End If


                    Exit Sub
                End If
                ' timeout limit repetitions

                Select Case CurrentOperation

                    Case Operations.SAVE_ADJUSMENTS
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                Dim myGlobal = ManageSavingBC()
                                If Not myGlobal.HasError Then
                                    _loadAdjDoneAttr = True
                                End If
                        End Select


                    Case Operations.HOMES
                        Select Case pResponse
                            Case RESPONSE_TYPES.OK
                                _homesDoneAttr = True
                        End Select


                    Case Operations.TEST
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                _currentTimeOperationAttr = AnalyzerController.Instance.Analyzer.MaxWaitTime '#REFACTORING
                            Case RESPONSE_TYPES.OK
                                _testDoneAttr = True
                        End Select

                    Case Operations.TEST_MODE
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing to do
                            Case RESPONSE_TYPES.OK
                                _testModeDoneAttr = True
                        End Select

                    Case Operations.TEST_MODE_END
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing to do
                            Case RESPONSE_TYPES.OK
                                _testModeEndedAttr = True
                        End Select

                End Select

            Catch ex As Exception
                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
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
        Public Function SendFwScriptsQueueList(pMode As ADJUSTMENT_MODES, _
                                               Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment

                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = SendQueueForREADINGADJUSTMENTS()

                    Case ADJUSTMENT_MODES.LOADING
                        myResultData = SendQueueForLOADING()

                    Case ADJUSTMENT_MODES.TEST_EXITING
                        myResultData = SendQueueForTEST_EXITING()

                    Case ADJUSTMENT_MODES.ADJUST_PREPARING
                        myResultData = SendQueueForADJUST_PREPARING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.ADJUSTING
                        myResultData = SendQueueForADJUSTING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.TESTING
                        myResultData = SendQueueForTESTING(pAdjustmentGroup)

                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
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
        Public Function GetParameters(pAnalyzerModel As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Try
                ' Read Signal Quality recommendation
                myResultData = myParams.ReadByParameterName(Nothing, SwParameters.SRV_BARCODE_READ_SIGNAL_OK.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    Dim myParametersDs = CType(myResultData.SetDatos, ParametersDS)
                    _readSignalOkAttr = CInt(myParametersDs.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.GetParameters", EventLogEntryType.Error, False)
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
                CurrentOperation = Operations.SAVE_ADJUSMENTS
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.LOADADJ, True, Nothing, _pValueAdjustAttr) '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendLOAD_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' High Level Instruction to request Barcode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 16/12/2011</remarks>
        Public Function SendBARCODE_REQUEST(pBarCodeDs As AnalyzerManagerDS, pOperation As Operations) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                CurrentOperation = pOperation
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, pBarCodeDS, "") '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendBARCODE_REQUEST", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDs"></param>
        ''' <remarks>Created by XBC 16/12/2011</remarks>
        Public Sub RefreshDelegate(pRefreshEventType As List(Of UI_RefreshEvents), pRefreshDs As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                If pRefreshDS.RotorPositionsChanged.Rows.Count > 0 Then
                    For Each updatedRow As UIRefreshDS.RotorPositionsChangedRow In pRefreshDS.RotorPositionsChanged.Rows
                        If CurrentOperation = Operations.TEST Then
                            If updatedRow.BarcodeStatus = "OK" Then
                                BcResultsAdd(updatedRow.CellNumber, updatedRow.BarCodeInfo)
                            End If
                        Else
                            _bcResultsAttr = Nothing ' XBC 09/10/2012 - Correction - into test mode the value to read always is the first who overwrite the before read value
                            BcResultsAdd(updatedRow.CellNumber, updatedRow.BarCodeInfo)
                        End If
                    Next
                End If

                If CurrentOperation = Operations.TEST Then
                    ' Insert the new activity into Historic reports
                    myResultData = InsertReport("TEST", "BARCODE")
                End If

                ScreenReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

        Public Sub BcResultsAdd(pPosition As Integer, pValue As String)
            Dim myResultData As New GlobalDataTO
            Try
                If _bcResultsAttr Is Nothing Then
                    ReDim _bcResultsAttr(0)
                Else
                    ReDim Preserve _bcResultsAttr(UBound(_bcResultsAttr) + 1)
                End If
                _bcResultsAttr(UBound(_bcResultsAttr)).Position = pPosition
                _bcResultsAttr(UBound(_bcResultsAttr)).Value = pValue

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.BcResultsAdd", EventLogEntryType.Error, False)
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
        Public Function DecodeDataReport(pTask As String, pAction As String, pData As String, pcurrentLanguage As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Dim text1 As String
                Dim text = ""
                Dim j = 0

                Select Case pTask
                    Case "ADJUST"

                        ' selected Rotor
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "GRID_SRV_ROTOR", pcurrentLanguage) + ": "
                        If pData.Substring(j, 1) = "1" Then
                            ' alignment...
                            text1 += SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage)
                        Else
                            ' alignment...
                            text1 += SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage)
                        End If
                        text += FormatLineHistorics(text1)
                        j += 1

                        ' Barcode point value
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_BarCode", pcurrentLanguage) + ": "
                        ' alignment...
                        text1 += SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("##,##0").Length)
                        ' content
                        text1 += CSng(pData.Substring(j, 5)).ToString("##,##0")
                        text += FormatLineHistorics(text1)
                        j += 5

                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUSTED", pcurrentLanguage) + ":"
                        ' alignment...
                        text1 += SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage).Length)
                        ' content
                        text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage)
                        text += FormatLineHistorics(text1)

                    Case "TEST"

                        ' selected Rotor
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "GRID_SRV_ROTOR", pcurrentLanguage) + ": "
                        If pData.Substring(j, 1) = "1" Then
                            ' alignment...
                            text1 += SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pcurrentLanguage)
                        Else
                            ' alignment...
                            text1 += SetSpaces(22 - text1.Length - 1 - myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage).Length)
                            ' content
                            text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pcurrentLanguage)
                        End If
                        text += FormatLineHistorics(text1)
                        j += 1

                        text += Environment.NewLine
                        ' detected tubes
                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TubesDetected", pcurrentLanguage) + ": "
                        text += FormatLineHistorics(text1)

                        Dim detectedTubes As Integer
                        If IsNumeric(pData.Substring(j, 3)) Then
                            detectedTubes = CInt(pData.Substring(j, 3))
                            j += 3

                            For i = 0 To detectedTubes - 1
                                text += Environment.NewLine
                                ' tube position
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TUBE_POSITION", pcurrentLanguage) + ": "
                                ' alignment...
                                text1 += SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 3).Length)
                                ' content
                                text1 += pData.Substring(j, 3)
                                text += FormatLineHistorics(text1)
                                j += 3

                                ' barcode value (searching #%# character...)
                                Dim tmptext As String
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_BarCode", pcurrentLanguage) + ": "

                                tmptext = pData.Substring(j, pData.Length - j)
                                Dim k As Integer
                                k = tmptext.IndexOf("#%#")
                                ' alignment...
                                text1 += SetSpaces(22 - text1.Length - 1 - tmptext.Substring(0, k).Length)
                                ' content
                                text1 += tmptext.Substring(0, k)
                                text += FormatLineHistorics(text1)
                                j += k + 3

                            Next
                        End If


                End Select

                ' put separator as a group thousands separator
                text = text.Replace(_myGroupSeparator.ToString, " ")

                myResultData.SetDatos = text

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)
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
        Private Function SendQueueForLoading() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                ' Initializations 
                _bcResultsAttr = Nothing

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
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForLOADING", EventLogEntryType.Error, False)
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
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForTEST_EXITING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Adjust Preparing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 15/12/2010</remarks>
        Private Function SendQueueForADJUST_PREPARING(pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'get the pending Homes  
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDs As SRVPreliminaryHomesDS
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, pAdjustment.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    myHomesDs = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

                    Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDs.srv_tadjPreliminaryHomes _
                                    Where a.Done = False Select a).ToList

                    For Each H In myPendingHomesList
                        Dim myFwScript As New FwScriptQueueItem
                        myListFwScript.Add(myFwScript)
                    Next

                    ' ReSharper disable once InconsistentNaming  'Delta expressions are better to avoid Cut&Paste operations in code: 
                    Dim SetScriptValues = Sub(script As FwScriptQueueItem, nextScriptToRun As FwScriptQueueItem, requiredHomeID As String)
                                              With script
                                                  .FwScriptID = requiredHomeID
                                                  .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                                  .EvaluateValue = 1
                                                  .NextOnResultOK = nextScriptToRun
                                                  .NextOnResultNG = Nothing
                                                  .NextOnTimeOut = nextScriptToRun
                                                  .NextOnError = Nothing
                                                  .ParamList = Nothing
                                              End With
                                          End Sub

                    Dim i = 0
                    For Each home As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList

                        'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                        If i = myListFwScript.Count - 1 Then
                            SetScriptValues(myListFwScript(i), myFwScript1, home.RequiredHomeID)
                        Else
                            SetScriptValues(myListFwScript(i), myListFwScript(i + 1), home.RequiredHomeID)
                        End If
                        i += 1
                    Next
                End If

                Dim scriptID As FwSCRIPTS_IDS, addRequiredScript = False

                Select Case pAdjustment

                    Case ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC
                        scriptID = FwSCRIPTS_IDS.SAMPLES_ABS_ROTOR
                        addRequiredScript = True

                    Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                        scriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR
                        addRequiredScript = True

                End Select

                If addRequiredScript Then
                    With myFwScript1
                        .FwScriptID = scriptID.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(_samplesRotorBcPositionAttr)
                    End With

                    For i = 0 To myListFwScript.Count - 1
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), myFwScriptDelegate.CurrentFwScriptsQueue.Any = False)
                    Next
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, myFwScriptDelegate.CurrentFwScriptsQueue.Any = False)

                    Dim switchOnBc As New FwScriptQueueItem With
                        {
                            .FwScriptID = FwSCRIPTS_IDS.SWITCH_ON_BARCODE.ToString,
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE,
                            .EvaluateValue = 1,
                            .NextOnResultOK = Nothing,
                            .NextOnResultNG = Nothing,
                            .NextOnTimeOut = Nothing,
                            .NextOnError = Nothing,
                            .ParamList = Nothing
                        }
                    myFwScript1.NextOnResultOK = switchOnBc
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(switchOnBc, myFwScriptDelegate.CurrentFwScriptsQueue.Any = False)
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForADJUST_PREPARING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Adjusting operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 15/12/2011</remarks>
        Private Function SendQueueForAdjusting(pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
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
                            Select Case PMovAdjust
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
                                    .ParamList.Add(_pValueAdjustAttr)
                                Case MOVEMENT.RELATIVE
                                    ' Mov REL
                                    ' Relative positioning of the samples rotor for x steps
                                    .FwScriptID = FwSCRIPTS_IDS.SAMPLES_REL_ROTOR.ToString
                                    ' expects 1 param
                                    .ParamList = New List(Of String)
                                    .ParamList.Add(_pValueAdjustAttr)
                            End Select

                        Case ADJUSTMENT_GROUPS.REAGENTS_ROTOR_BC
                            Select Case PMovAdjust
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
                                    .ParamList.Add(_pValueAdjustAttr)
                                Case MOVEMENT.RELATIVE
                                    ' Mov REL
                                    ' Relative positioning of the reagents rotor for x steps
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENTS_REL_ROTOR.ToString
                                    ' expects 1 param
                                    .ParamList = New List(Of String)
                                    .ParamList.Add(_pValueAdjustAttr)
                            End Select


                    End Select

                End With

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForADJUSTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Testing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 16/12/11</remarks>
        Private Function SendQueueForTesting(pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                ' Initializations 
                _bcResultsAttr = Nothing

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'get the pending Homes
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDs As SRVPreliminaryHomesDS
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, pAdjustment.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    myHomesDs = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

                    Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDs.srv_tadjPreliminaryHomes _
                                    Where a.Done = False Select a).ToList


                    For Each H In myPendingHomesList
                        Dim myFwScript As New FwScriptQueueItem
                        myListFwScript.Add(myFwScript)
                    Next

                    Dim i = 0
                    For Each h As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList

                        'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                        If i = myListFwScript.Count - 1 Then
                            'Last index
                            With myListFwScript(i)
                                .FwScriptID = h.RequiredHomeID.ToString
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
                                .FwScriptID = h.RequiredHomeID.ToString
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

                myListFwScript.Add(myFwScript1)
                CurrentOperation = Operations.HOMES

                'add to the queue list
                If myListFwScript.Count > 0 Then

                    For i = 0 To myListFwScript.Count - 1
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


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.SendQueueForTESTING", EventLogEntryType.Error, False)
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
        Private Function ManageSavingBc() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Insert the new activity into Historic reports
                myResultData = InsertReport("ADJUST", "BARCODE")

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.ManageSavingBC", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


#Region "Historical reports"
        Public Function InsertReport(pTaskID As String, pActionID As String) As GlobalDataTO
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
                    Dim generatedID = DirectCast(myResultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                    ' Insert recommendations if existing
                    If _recommendationsReport IsNot Nothing Then
                        Dim myRecommendationsList As New SRVRecommendationsServiceDS
                        Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                        For i = 0 To _recommendationsReport.Length - 1
                            myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                            myRecommendationsRow.ResultServiceID = generatedID
                            myRecommendationsRow.RecommendationID = CInt(_recommendationsReport(i))
                            myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                        Next

                        myResultData = myHistoricalReportsDelegate.AddRecommendations(Nothing, myRecommendationsList)
                        If myResultData.HasError Then
                            myResultData.HasError = True
                        End If
                        _recommendationsReport = Nothing
                    End If
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.InsertReport", EventLogEntryType.Error, False)
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
        Private Function GenerateDataReport(pTask As String, pAction As String) As String
            Dim returnValue = ""
            Try
                Select Case pTask
                    Case "ADJUST"

                        Select Case pAction
                            Case "BARCODE"

                                returnValue = ""
                                ' Selected rotor
                                If _adjustmentIDAttr = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' BarCode point value
                                returnValue += _adjustmentBcPointAttr.ToString("00000")

                        End Select


                    Case "TEST"

                        Select Case pAction
                            Case "BARCODE"

                                returnValue = ""
                                ' Selected rotor
                                If _adjustmentIDAttr = ADJUSTMENT_GROUPS.SAMPLES_ROTOR_BC Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Number of tube codebar detected
                                If _bcResultsAttr Is Nothing Then
                                    returnValue += "0"
                                Else
                                    returnValue += _bcResultsAttr.Count.ToString("000")

                                    For i = 0 To _bcResultsAttr.Count - 1
                                        ' Tube position
                                        returnValue += _bcResultsAttr(i).Position.ToString("000")

                                        ' BarCode value
                                        returnValue += _bcResultsAttr(i).Value.ToString()
                                        ' Separator
                                        returnValue += "#%#"
                                    Next
                                End If

                        End Select
                End Select

            Catch ex As Exception
                CreateLogActivity(ex.Message, "BarCodeAdjustmentDelegate.GenerateDataReport", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function
#End Region

#End Region

    End Class

End Namespace

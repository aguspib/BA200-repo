Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.BL


Namespace Biosystems.Ax00.FwScriptsManagement


    ''' <summary>
    ''' Delegate that manages the Scripts' sending and receiving
    ''' </summary>
    ''' <remarks>Created by SG 17/11/10</remarks>
    Public Class TankLevelsAdjustmentDelegate
        Inherits BaseFwScriptDelegate


#Region "Enumerations"
        Private Enum OPERATIONS
            NONE
            HOMES
            SAVE_ADJUSMENTS
            TANKS_TEST
        End Enum

        Public Enum HISTORY_AREAS
            NONE = 0
            SCALES = 1
            INTERMEDIATE = 2
        End Enum

        Public Enum HISTORY_RESULTS
            _ERROR = -1
            NOT_DONE = 0
            OK = 1
            NOK = 2
            CANCEL = 3
        End Enum
#End Region



#Region "Attributes"
        Private pValueAdjustAttr As String
        Private CurrentTestAttr As ADJUSTMENT_GROUPS    ' Test currently executed

        Private EmptyLcDoneAttr As Boolean = False
        Private EmptyLcDoingAttr As Boolean = False
        Private FillDwDoneAttr As Boolean = False
        Private FillDwDoingAttr As Boolean = False
        Private TransferDwLcDoneAttr As Boolean = False
        Private TransferDwLcDoingAttr As Boolean = False
        Private CurrentTimeOperationAttr As Integer                 ' Time expected for current operation
        'Private AnalyzerIdAttr As String = ""


        'HISTORY
        '***************************
        Private HistoryTaskAttr As PreloadedMasterDataEnum = Nothing
        Private HistoryAreaAttr As HISTORY_AREAS = HISTORY_AREAS.NONE

        'SCALES
        'adjustments
        Private WsMinAdjResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private WsMaxAdjResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HcMinAdjResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HcMaxAdjResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HisWsMinAdjValueAttr As Integer = -1
        Private HisWsMaxAdjValueAttr As Integer = -1
        Private HisHcMinAdjValueAttr As Integer = -1
        Private HisHcMaxAdjValueAttr As Integer = -1

        'tests
        Private WsMinTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private WsMaxTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HcMinTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HcMaxTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE

        'INTERMEDIATE TANKS
        'tests
        Private EmptyLcTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private FillDwTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private TransferDwLcTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE

#End Region

#Region "Declarations"
        Private CurrentOperation As OPERATIONS                  ' controls the operation which is currently executed in each time

        Private RecommendationsList As New List(Of HISTORY_RECOMMENDATIONS)
        Private ReportCountTimeout As Integer
#End Region

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            MyClass.myFwScriptDelegate = pFwScriptsDelegate
            MyClass.ReportCountTimeout = 0

        End Sub

        Public Sub New()
            MyBase.New() 'SGM 20/01/2012
            MyClass.ReportCountTimeout = 0
        End Sub
#End Region

#Region "Properties"

#Region "Private Properties"

       

#End Region

#Region "Public Properties"
        'Public Property AnalyzerId() As String
        '    Get
        '        Return AnalyzerIdAttr
        '    End Get
        '    Set(ByVal value As String)
        '        AnalyzerIdAttr = value
        '    End Set
        'End Property

        'used?????
        Public Property pValueAdjust() As String
            Get
                Return Me.pValueAdjustAttr
            End Get
            Set(ByVal value As String)
                Me.pValueAdjustAttr = value
            End Set
        End Property

        Public Property EmptyLcDone() As Boolean
            Get
                Return Me.EmptyLcDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.EmptyLcDoneAttr = value
            End Set
        End Property
        Public Property EmptyLcDoing() As Boolean
            Get
                Return Me.EmptyLcDoingAttr
            End Get
            Set(ByVal value As Boolean)
                Me.EmptyLcDoingAttr = value
            End Set
        End Property
        Public Property FillDwDone() As Boolean
            Get
                Return Me.FillDwDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.FillDwDoneAttr = value
            End Set
        End Property
        Public Property FillDwDoing() As Boolean
            Get
                Return Me.FillDwDoingAttr
            End Get
            Set(ByVal value As Boolean)
                Me.FillDwDoingAttr = value
            End Set
        End Property
        Public Property TransferDwLcDone() As Boolean
            Get
                Return Me.TransferDwLcDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.TransferDwLcDoneAttr = value
            End Set
        End Property
        Public Property TransferDwLcDoing() As Boolean
            Get
                Return Me.TransferDwLcDoingAttr
            End Get
            Set(ByVal value As Boolean)
                Me.TransferDwLcDoingAttr = value
            End Set
        End Property
        Public ReadOnly Property CurrentTimeOperation() As Integer
            Get
                Return CurrentTimeOperationAttr
            End Get
        End Property


        '**************HISTORY********************ç

        Public Property HistoryTask() As PreloadedMasterDataEnum
            Get
                Return HistoryTaskAttr
            End Get
            Set(ByVal value As PreloadedMasterDataEnum)
                HistoryTaskAttr = value
            End Set
        End Property

        Public Property HistoryArea() As HISTORY_AREAS
            Get
                Return HistoryAreaAttr
            End Get
            Set(ByVal value As HISTORY_AREAS)
                HistoryAreaAttr = value
            End Set
        End Property

       

        'SCALES
        'Adjustments
        Public Property WsMinAdjResult() As HISTORY_RESULTS
            Get
                Return WsMinAdjResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If WsMinAdjResultAttr <> value Then
                    WsMinAdjResultAttr = value
                End If
            End Set
        End Property
        Public Property WsMaxAdjResult() As HISTORY_RESULTS
            Get
                Return WsMaxAdjResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If WsMaxAdjResultAttr <> value Then
                    WsMaxAdjResultAttr = value
                End If
            End Set
        End Property
        Public Property HcMinAdjResult() As HISTORY_RESULTS
            Get
                Return HcMinAdjResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If HcMinAdjResultAttr <> value Then
                    HcMinAdjResultAttr = value
                End If
            End Set
        End Property
        Public Property HcMaxAdjResult() As HISTORY_RESULTS
            Get
                Return HcMaxAdjResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If HcMaxAdjResultAttr <> value Then
                    HcMaxAdjResultAttr = value
                End If
            End Set
        End Property

        Public Property HisWsMinAdjValue() As Integer
            Get
                Return HisWsMinAdjValueAttr
            End Get
            Set(ByVal value As Integer)
                HisWsMinAdjValueAttr = value
            End Set
        End Property
        Public Property HisWsMaxAdjValue() As Integer
            Get
                Return HisWsMaxAdjValueAttr
            End Get
            Set(ByVal value As Integer)
                HisWsMaxAdjValueAttr = value
            End Set
        End Property
        Public Property HisHcMinAdjValue() As Integer
            Get
                Return HisHcMinAdjValueAttr
            End Get
            Set(ByVal value As Integer)
                HisHcMinAdjValueAttr = value
            End Set
        End Property
        Public Property HisHcMaxAdjValue() As Integer
            Get
                Return HisHcMaxAdjValueAttr
            End Get
            Set(ByVal value As Integer)
                HisHcMaxAdjValueAttr = value
            End Set
        End Property

        'Tests
        Public Property WsMinTestResult() As HISTORY_RESULTS
            Get
                Return WsMinTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                WsMinTestResultAttr = value
            End Set
        End Property
        Public Property WsMaxTestResult() As HISTORY_RESULTS
            Get
                Return WsMaxTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                WsMaxTestResultAttr = value
            End Set
        End Property
        Public Property HcMinTestResult() As HISTORY_RESULTS
            Get
                Return HcMinTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HcMinTestResultAttr = value
            End Set
        End Property
        Public Property HcMaxTestResult() As HISTORY_RESULTS
            Get
                Return HcMaxTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HcMaxTestResultAttr = value
            End Set
        End Property


        'INTERMEDIATE TANKS
        Public Property EmptyTestResult() As HISTORY_RESULTS
            Get
                Return Me.EmptyLcTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                Me.EmptyLcTestResultAttr = value
            End Set
        End Property
        Public Property FillDwTestResult() As HISTORY_RESULTS
            Get
                Return Me.FillDwTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                Me.FillDwTestResultAttr = value
            End Set
        End Property
        Public Property TransferDwLcTestResult() As HISTORY_RESULTS
            Get
                Return Me.TransferDwLcTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                Me.TransferDwLcTestResultAttr = value
            End Set
        End Property

#End Region

#End Region



#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 18/05/11</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles Me.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    If MyClass.ReportCountTimeout = 0 Then
                        MyClass.ReportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        MyClass.UpdateRecommendationsList(HISTORY_RECOMMENDATIONS.ERR_COMM)
                    End If

                    Exit Sub

                End If

                Select Case CurrentOperation
                    Case OPERATIONS.TANKS_TEST

                        Select Case Me.CurrentTestAttr
                            Case ADJUSTMENT_GROUPS.TANKS_EMPTY_LC
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        CurrentTimeOperationAttr = myFwScriptDelegate.AnalyzerManager.MaxWaitTime
                                        Me.EmptyLcDoingAttr = True
                                    Case RESPONSE_TYPES.OK
                                        Me.EmptyLcDoneAttr = True
                                End Select

                            Case ADJUSTMENT_GROUPS.TANKS_FILL_DW
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        CurrentTimeOperationAttr = myFwScriptDelegate.AnalyzerManager.MaxWaitTime
                                        Me.FillDwDoingAttr = True
                                    Case RESPONSE_TYPES.OK
                                        Me.FillDwDoneAttr = True
                                End Select

                            Case ADJUSTMENT_GROUPS.TANKS_TRANSFER_DW_LC
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        CurrentTimeOperationAttr = myFwScriptDelegate.AnalyzerManager.MaxWaitTime
                                        Me.TransferDwLcDoingAttr = True
                                    Case RESPONSE_TYPES.OK
                                        Me.TransferDwLcDoneAttr = True
                                End Select

                        End Select

                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
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
        ''' Created by SG 17/11/10
        ''' Modified by XBC 04/01/11 - add pAdjustment parameter
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment

                Select Case pMode

                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = Me.SendQueueForREADINGADJUSTMENTS()

                    Case ADJUSTMENT_MODES.HOME_PREPARING
                        myResultData = Me.SendQueueForHOMING()

                        'Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST
                        '    myResultData = Me.SendQueueForEMPTY_LOW_CONTAMINATION

                        'Case ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST
                        '    myResultData = Me.SendQueueForFILL_DISTILLED_WATER

                        'Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST
                        '    myResultData = Me.SendQueueForTRANSFER_DW_LC

                    Case ADJUSTMENT_MODES.TEST_EXITING
                        myResultData = Me.SendQueueForTEST_EXITING()

                        'Case ADJUSTMENT_MODES.ADJUSTING
                        '    'myResultData = Me.SendQueueForADJUSTING(pAdjustmentGroup)

                        'Case ADJUSTMENT_MODES.ADJUST_EXITING
                        '    'myResultData = Me.SendQueueForADJUST_EXITING(pAdjustmentGroup)

                        'Case ADJUSTMENT_MODES.SAVING
                        '    myResultData = Me.SendQueueForSAVING()

                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Load Adjustments High Level Instruction to save values into the instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 18/05/2011</remarks>
        Public Function SendLoad_Adjustments(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Me.CurrentTestAttr = pAdjustment
                Me.CurrentOperation = OPERATIONS.SAVE_ADJUSMENTS
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, True, Nothing, Me.pValueAdjustAttr)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendLOAD_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Request for any part of the Tanks Test (3)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 18/05/2011</remarks>
        Public Function SendTanksTest_Adjustments(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Me.CurrentTestAttr = pAdjustment
                Me.CurrentOperation = OPERATIONS.TANKS_TEST
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.TANKS_TEST, True, Nothing, Me.pValueAdjustAttr)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendTANKSTEST_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Creates the Script List for Homing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 10/03/11</remarks>
        Private Function SendQueueForHOMING() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            'Dim myFwScript1 As New FwScriptQueueItem
            Try

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'sg 10/3/11
                'get the pending Homes
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDS As SRVPreliminaryHomesDS
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, ADJUSTMENT_GROUPS.INTERNAL_TANKS.ToString)
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
                                .NextOnResultOK = Nothing
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = Nothing
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
                    'If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                Else
                    'If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForHOMING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        '''' <summary>
        '''' Creates the Script List for  Internal tanks test
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 15/03/11</remarks>
        'Private Function SendQueueForEMPTY_LOW_CONTAMINATION() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myFwScript1 As New FwScriptQueueItem

        '        'Script1
        '        With myFwScript1
        '            .FwScriptID = FwSCRIPTS_IDS.TANKS_EMPTY_LC.ToString
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing
        '        End With

        '        '*************************************************************************************************************
        '        ''in case of specific handling the response
        '        '.ResponseEventHandling  = True 'enable event handling
        '        'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForEMPTY_LOW_CONTAMINATION", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for  Internal tanks test
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 15/03/11</remarks>
        'Private Function SendQueueForFILL_DISTILLED_WATER() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myFwScript1 As New FwScriptQueueItem

        '        'Script1
        '        With myFwScript1
        '            .FwScriptID = FwSCRIPTS_IDS.TANKS_FILL_DW.ToString
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing
        '        End With

        '        '*************************************************************************************************************
        '        ''in case of specific handling the response
        '        '.ResponseEventHandling  = True 'enable event handling
        '        'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForFILL_DISTILLED_WATER", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for  Internal tanks test
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 15/03/11</remarks>
        'Private Function SendQueueForTRANSFER_DW_LC() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myFwScript1 As New FwScriptQueueItem

        '        'Script1
        '        With myFwScript1
        '            .FwScriptID = FwSCRIPTS_IDS.TANKS_TRANSFER_DW_LC.ToString
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing
        '        End With

        '        '*************************************************************************************************************
        '        ''in case of specific handling the response
        '        '.ResponseEventHandling  = True 'enable event handling
        '        'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForTRANSFER_DW_LC", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary>
        ''' Creates the Script List for exiting Intermediate tanks
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 15/03/11</remarks>
        Private Function SendQueueForTEST_EXITING() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myFwScript1 As New FwScriptQueueItem

                'Script1
                With myFwScript1
                    .FwScriptID = FwSCRIPTS_IDS.TEST_EXIT.ToString
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = Nothing
                End With

                '*************************************************************************************************************
                ''in case of specific handling the response
                '.ResponseEventHandling  = True 'enable event handling
                'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForTESTEXITING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        '''' <summary>
        '''' Creates the Script List for Screen Saving operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/01/11</remarks>
        'Private Function SendQueueForSAVING() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Try
        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        With myFwScript1
        '            .FwScriptID = FwSCRIPTS_IDS.SAVE_ADJUSTMENTS.ToString
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            ' expects 1 param
        '            .ParamList = New List(Of String)
        '            .ParamList.Add(Me.pValueAdjustAttr)
        '        End With

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForSAVING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function






#End Region


#Region "History Data Report"

#Region "Public"

        ''' <summary>
        ''' Saves the History data to DB
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Public Function ManageHistoryResults() As GlobalDataTO

            Dim myResultData As New GlobalDataTO
            Dim myHistoryDelegate As New HistoricalReportsDelegate

            Try

                If MyClass.HistoryArea <> Nothing Then

                    Dim myHistoricReportDS As New SRVResultsServiceDS
                    Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                    Dim myTask As String = ""
                    Dim myAction As String = ""

                    Select Case MyClass.HistoryTask
                        Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES : myTask = "ADJUST"
                        Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES : myTask = "TEST"
                        Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES : myTask = "UTIL"
                    End Select

                    myAction = MyClass.HistoryArea.ToString

                    'Fill the data
                    myHistoricReportRow = myHistoricReportDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                    With myHistoricReportRow
                        .BeginEdit()
                        .TaskID = myTask
                        .ActionID = myAction
                        .Data = MyClass.GenerateResultData()
                        .AnalyzerID = MyClass.AnalyzerId
                        .EndEdit()
                    End With

                    'save to history
                    myResultData = myHistoryDelegate.Add(Nothing, myHistoricReportRow)


                    If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                        'Get the generated ID from the dataset returned 
                        Dim generatedID As Integer = -1
                        generatedID = DirectCast(myResultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                        'PDT!!
                        ' pending implementation for add possibles Recommendations usings in case of INCIDENCE!!!
                        If generatedID >= 0 Then
                            ' Insert recommendations if existing
                            If MyClass.RecommendationsList IsNot Nothing Then
                                Dim myRecommendationsList As New SRVRecommendationsServiceDS
                                Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                                For Each R As HISTORY_RECOMMENDATIONS In MyClass.RecommendationsList
                                    myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                                    myRecommendationsRow.ResultServiceID = generatedID
                                    myRecommendationsRow.RecommendationID = CInt(R)
                                    myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                                Next

                                myResultData = myHistoryDelegate.AddRecommendations(Nothing, myRecommendationsList)
                                If myResultData.HasError Then
                                    myResultData.HasError = True
                                End If
                                MyClass.RecommendationsList = Nothing
                            End If
                        End If

                    End If

                Else
                    myResultData.HasError = True
                    myResultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                End If



            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.ManageHistoryResults", EventLogEntryType.Error, False)
            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' Decodes the data from DB in order to display in History Screen
        ''' </summary>
        ''' <param name="pTask"></param>
        ''' <param name="pAction"></param>
        ''' <param name="pData"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pLanguageId As String) As GlobalDataTO

            Dim myResultData As New GlobalDataTO
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try

                Dim myUtility As New Utilities()
                Dim text1 As String = ""
                Dim text2 As String = ""
                Dim text3 As String = ""

                Dim myLines As List(Of List(Of String))
                Dim FinalText As String = ""

                Dim myTask As PreloadedMasterDataEnum
                Dim myArea As HISTORY_AREAS


                'set the task type
                Select Case pTask
                    Case "ADJUST" : myTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                    Case "TEST" : myTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                    Case "UTIL" : myTask = PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                End Select

                'set the area
                Select Case pAction
                    Case "SCALES" : myArea = HISTORY_AREAS.SCALES
                    Case "INTERMEDIATE" : myArea = HISTORY_AREAS.INTERMEDIATE
                End Select

                'obtain the connection
                myResultData = DAOBase.GetOpenDBConnection(Nothing)
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myResultData.SetDatos, SqlClient.SqlConnection)
                End If

                Select Case myTask
                    Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES

                        Select Case myArea
                            Case HISTORY_AREAS.SCALES

                                myLines = New List(Of List(Of String))

                                Dim myLine As List(Of String)
                                Dim myValue1 As String = ""
                                Dim myResult1 As HISTORY_RESULTS
                                Dim myValue2 As String = ""
                                Dim myResult2 As HISTORY_RESULTS

                                Dim myColWidth As Integer = 35
                                Dim myColSep As Integer = 3

                                '1st LINE (title)
                                '*****************************************************************
                                myLine = New List(Of String)
                                ' Washing Solution Tank
                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_WashingSolution", pLanguageId) + ":")
                                myLine.Add(text1)

                                ' High Contamination Tank
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_HighContaminationTank", pLanguageId) + ":")
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '2nd LINE (min level title)
                                '*****************************************************************
                                myLine = New List(Of String)
                                ' WS Minimum Level
                                myValue1 = MyClass.DecodeHistoryDataValue(pData, myTask, "WSMIN")
                                myResult1 = MyClass.DecodeHistoryDataResult(pData, myTask, "WSMIN")
                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_EMPTY_LEVEL", pLanguageId) + ":")
                                myLine.Add(text1)

                                ' HC Minimum Level
                                myValue2 = MyClass.DecodeHistoryDataValue(pData, myTask, "HCMIN")
                                myResult2 = MyClass.DecodeHistoryDataResult(pData, myTask, "HCMIN")
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_EMPTY_LEVEL", pLanguageId) + ":")
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '3rd LINE (min level value)
                                '*****************************************************************
                                myLine = New List(Of String)
                                ' WS Maximum Level
                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_Tests_Value", pLanguageId) + ": ")
                                text1 &= myValue1
                                myLine.Add(text1)

                                ' HC Maximum Level
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_Tests_Value", pLanguageId) + ": ")
                                text2 &= myValue2
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '4th LINE (min level adjusted)
                                '*****************************************************************
                                myLine = New List(Of String)
                                ' WS Minimum Level
                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ADJUSTED", pLanguageId) + ": ")
                                text1 &= MyClass.GetResultLanguageResource(dbConnection, myResult1, pLanguageId)
                                myLine.Add(text1)

                                ' HC Minimum Level
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ADJUSTED", pLanguageId) + ": ")
                                text2 &= MyClass.GetResultLanguageResource(dbConnection, myResult2, pLanguageId)
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '5th LINE (max level title)
                                '*****************************************************************
                                myLine = New List(Of String)
                                ' WS Maximum Level
                                myValue1 = MyClass.DecodeHistoryDataValue(pData, myTask, "WSMAX")
                                myResult1 = MyClass.DecodeHistoryDataResult(pData, myTask, "WSMAX")
                                text1 = MLRD.GetResourceText(dbConnection, "LBL_SRV_FULL_LEVEL", pLanguageId) + ":"
                                myLine.Add(text1)

                                ' HC Maximum Level
                                myValue2 = MyClass.DecodeHistoryDataValue(pData, myTask, "HCMAX")
                                myResult2 = MyClass.DecodeHistoryDataResult(pData, myTask, "HCMAX")
                                text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_FULL_LEVEL", pLanguageId) + ":"
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '6th LINE (max level value)
                                '*****************************************************************
                                myLine = New List(Of String)
                                ' WS Maximum Level
                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_Tests_Value", pLanguageId) + ": ")
                                text1 &= myValue1
                                myLine.Add(text1)

                                ' HC Maximum Level
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_Tests_Value", pLanguageId) + ": ")
                                text2 &= myValue2
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '7th LINE (max level adjusted)
                                '*****************************************************************
                                myLine = New List(Of String)
                                ' WS Maximum Level
                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ADJUSTED", pLanguageId) + ": ")
                                text1 &= MyClass.GetResultLanguageResource(dbConnection, myResult1, pLanguageId)
                                myLine.Add(text1)

                                ' HC Maximum Level
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ADJUSTED", pLanguageId) + ": ")
                                text2 &= MyClass.GetResultLanguageResource(dbConnection, myResult2, pLanguageId)
                                text2 &= vbCrLf
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                For Each Line As List(Of String) In myLines
                                    Dim newLine As Boolean = False
                                    Select Case myLines.IndexOf(Line) + 1
                                        Case 1 : newLine = True 'title
                                        Case 2 : newLine = False 'min level title
                                        Case 3 : newLine = False 'min level value
                                        Case 4 : newLine = True 'min level adjusted
                                        Case 5 : newLine = False 'max level title
                                        Case 6 : newLine = False 'max level value
                                        Case 7 : newLine = False 'max level adjusted
                                    End Select
                                    FinalText &= myUtility.FormatLineHistorics(Line, myColWidth, newLine)
                                Next

                                myResultData.SetDatos = FinalText




                            Case HISTORY_AREAS.INTERMEDIATE
                                'not adjustment implemented

                        End Select


                    Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES

                        Select Case myArea
                            Case HISTORY_AREAS.SCALES

                                'not applied


                            Case HISTORY_AREAS.INTERMEDIATE

                                myLines = New List(Of List(Of String))

                                Dim myLine As List(Of String)
                                Dim myResult1 As HISTORY_RESULTS
                                Dim myResult2 As HISTORY_RESULTS
                                Dim myResult3 As HISTORY_RESULTS

                                Dim myColWidth As Integer = 60
                                Dim myColSep As Integer = 0

                                '1st LINE (title)
                                '*****************************************************************
                                myLine = New List(Of String)

                                ' DW INPUT
                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_DW_INPUT", pLanguageId) + ": ")

                                Dim myResultNumber As String = ""
                                myResultNumber = pData.Substring(0, 1)
                                If IsNumeric(myResultNumber) Then
                                    Select Case CInt(myResultNumber)
                                        Case 0
                                            text1 &= " " & MLRD.GetResourceText(dbConnection, "LBL_SRV_FROM_EXTERNAL_TANK", pLanguageId)

                                        Case 1
                                            text1 &= " " & MLRD.GetResourceText(dbConnection, "LBL_SRV_FROM_EXTERNAL_SOURCE", pLanguageId)

                                    End Select
                                End If

                                myLine.Add(text1)

                                myLines.Add(myLine)



                                '2nd LINE (Empty LC)
                                '*****************************************************************
                                myLine = New List(Of String)
                                text1 = MLRD.GetResourceText(dbConnection, "LBL_SRV_TEST_LC_EMPTY", pLanguageId) + ":"
                                myLine.Add(text1)
                                myResult1 = MyClass.DecodeHistoryDataResult(pData, myTask, "EMPTYLC")
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_TESTED", pLanguageId) + ": ")
                                text2 &= MyClass.GetResultLanguageResource(dbConnection, myResult1, pLanguageId)
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '3rd LINE (Fill DW)
                                '*****************************************************************
                                myLine = New List(Of String)
                                text1 = MLRD.GetResourceText(dbConnection, "LBL_SRV_TEST_DW_FILL", pLanguageId) + ":"
                                myLine.Add(text1)
                                myResult2 = MyClass.DecodeHistoryDataResult(pData, myTask, "FILLDW")
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_TESTED", pLanguageId) + ": ")
                                text2 &= MyClass.GetResultLanguageResource(dbConnection, myResult2, pLanguageId)
                                myLine.Add(text2)

                                myLines.Add(myLine)


                                '4th LINE (TRANSFER)
                                '*****************************************************************
                                myLine = New List(Of String)
                                text1 = MLRD.GetResourceText(dbConnection, "LBL_SRV_TEST_TANKS_TRANSFER", pLanguageId) + ":"
                                myLine.Add(text1)
                                myResult3 = MyClass.DecodeHistoryDataResult(pData, myTask, "TRANSFER")
                                text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_TESTED", pLanguageId) + ": ")
                                text2 &= MyClass.GetResultLanguageResource(dbConnection, myResult3, pLanguageId)
                                myLine.Add(text2)

                                myLines.Add(myLine)


                                For Each Line As List(Of String) In myLines
                                    Dim newLine As Boolean = False
                                    Select Case myLines.IndexOf(Line) + 1
                                        Case 1 : newLine = True 'input
                                        Case 2 : newLine = False 'title & tested

                                    End Select
                                    FinalText &= myUtility.FormatLineHistorics(Line, myColWidth, newLine)
                                Next

                                myResultData.SetDatos = FinalText


                        End Select

                    Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                        'not applied


                End Select


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)

            Finally
                If Not dbConnection IsNot Nothing Then dbConnection.Close()
            End Try

            Return myResultData

        End Function

#End Region

#Region "Private"

        ''' <summary>
        ''' Reports the History data to DB. Encodes the result data to history
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GenerateResultData() As String

            Dim myData As String = ""

            Try

                If MyClass.HistoryArea <> Nothing Then

                    Select Case MyClass.HistoryArea
                        Case HISTORY_AREAS.SCALES
                            Select Case MyClass.HistoryTask
                                Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisWsMinAdjValue)
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.WsMinAdjResult))
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisWsMaxAdjValue)
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.WsMaxAdjResult))

                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisHcMinAdjValue)
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HcMinAdjResult))
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisHcMaxAdjValue)
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HcMaxAdjResult))



                                Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.WsMinTestResult))
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.WsMaxTestResult))
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HcMinTestResult))
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HcMaxTestResult))

                            End Select

                        Case HISTORY_AREAS.INTERMEDIATE
                            Select Case MyClass.HistoryTask
                                Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.EmptyTestResult))
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.FillDwTestResult))
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.TransferDwLcTestResult))

                            End Select

                    End Select



                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.GenerateResultData", EventLogEntryType.Error, False)
            End Try

            Return myData

        End Function

        ''' <summary>
        ''' Encodes History Result
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function EncodeHistoryResult(ByVal pValue As Integer) As String

            Dim res As String = ""

            Try
                If pValue >= 0 Then
                    res = pValue.ToString
                Else
                    res = "x"
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.EncodeHistoryResult", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Encodes History Value
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function EncodeHistoryValue(ByVal pValue As Integer) As String

            Dim res As String = "xxxxx"

            Try
                If pValue >= 0 Then
                    res = pValue.ToString("00000")
                Else
                    res = pValue.ToString("0000")
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.EncodeHistoryValue", EventLogEntryType.Error, False)
                res = "xxxxx"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Result
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pTaskType"></param>
        ''' <param name="pItem"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataResult(ByVal pData As String, ByVal pTaskType As PreloadedMasterDataEnum, ByVal pItem As String) As HISTORY_RESULTS

            Dim myResult As HISTORY_RESULTS

            Try
                Dim myIndex As Integer = -1

                Select Case pTaskType
                    Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                        Select Case pItem
                            Case "WSMIN" : myIndex = 5
                            Case "WSMAX" : myIndex = 11
                            Case "HCMIN" : myIndex = 17
                            Case "HCMAX" : myIndex = 23

                        End Select

                    Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                        Select Case pItem
                            Case "WSMIN" : myIndex = 0
                            Case "WSMAX" : myIndex = 1
                            Case "HCMIN" : myIndex = 2
                            Case "HCMAX" : myIndex = 3

                            Case "EMPTYLC" : myIndex = 0
                            Case "FILLDW" : myIndex = 1
                            Case "TRANSFER" : myIndex = 2

                        End Select

                    Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                        'not applied
                End Select

                If myIndex >= 0 Then

                    Dim myResultNumber As String = ""


                    myResultNumber = pData.Substring(myIndex, 1)

                    If IsNumeric(myResultNumber) Then
                        myResult = CType(CInt(myResultNumber), HISTORY_RESULTS)
                    ElseIf myResultNumber = "x" Then
                        myResult = CType(-1, HISTORY_RESULTS)
                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
            End Try

            Return myResult

        End Function

        ''' <summary>
        ''' Decodes History Value
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pTaskType"></param>
        ''' <param name="pItem"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataValue(ByVal pData As String, ByVal pTaskType As PreloadedMasterDataEnum, ByVal pItem As String, Optional ByVal pLanguageId As String = "") As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try
                Dim myIndex As Integer = -1

                Select Case pTaskType
                    Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                        Select Case pItem
                            Case "WSMIN" : myIndex = 0
                            Case "WSMAX" : myIndex = 6
                            Case "HCMIN" : myIndex = 12
                            Case "HCMAX" : myIndex = 18

                        End Select

                    Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                        'not applied
                    Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                        'not applied
                End Select

                If myIndex >= 0 Then

                    Dim myDataText As String = ""

                    myDataText = pData.Substring(myIndex, 5)

                    If IsNumeric(myDataText) Then

                        res = CInt(myDataText).ToString

                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Gets Result Text
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pResult"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GetResultLanguageResource(ByRef pdbConnection As SqlClient.SqlConnection, ByVal pResult As HISTORY_RESULTS, ByVal pLanguageId As String) As String

            Dim res As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Try
                Select Case pResult
                    Case HISTORY_RESULTS.OK
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_YES", pLanguageId)

                    Case HISTORY_RESULTS.NOK, HISTORY_RESULTS.NOT_DONE
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_NO", pLanguageId)

                    Case HISTORY_RESULTS.CANCEL
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_CANCELLED", pLanguageId)

                    Case HISTORY_RESULTS._ERROR
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_FAILED", pLanguageId)

                End Select
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.GetResultLanguageResource", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' registering the incidence in historical reports activity
        ''' </summary>
        ''' <remarks>Created by SGM 04/08/2011</remarks>
        Private Sub UpdateRecommendationsList(ByVal pRecommendationID As HISTORY_RECOMMENDATIONS)
            Try
                ' registering the incidence in historical reports activity

                'If MyClass.RecommendationsReport Is Nothing Then
                '    ReDim MyClass.RecommendationsReport(0)
                'Else
                '    ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                'End If
                'MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = pRecommendationID

                If MyClass.RecommendationsList Is Nothing Then
                    MyClass.RecommendationsList = New List(Of HISTORY_RECOMMENDATIONS)
                End If
                MyClass.RecommendationsList.Add(pRecommendationID)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.UpdateRecommendations", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region


#End Region


#Region "NOT USED"
        '''' <summary>
        '''' Creates the Script List for Screen Loading operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 17/11/10</remarks>
        'Private Function SendQueueForADJUST_PREPARING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myListFwScript As New List(Of FwScriptQueueItem)
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Dim myFwScript2 As New FwScriptQueueItem
        '    Dim myFwScript3 As New FwScriptQueueItem
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If


        '        Select Case pAdjustment
        '            Case ADJUSTMENT_GROUPS.WASHING_SOLUTION
        '                ' Mov ABS Rotor
        '                ' Absolute positioning of the Reactions Rotor to last adjustment position as origen well
        '                With myFwScript1
        '                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_REL_ROTOR.ToString
        '                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                    .EvaluateValue = 1
        '                    .NextOnResultOK = Nothing
        '                    .NextOnResultNG = Nothing
        '                    .NextOnTimeOut = Nothing
        '                    .NextOnError = Nothing
        '                    ' expects 1 param
        '                    .ParamList = New List(Of String)
        '                    '.ParamList.Add(Me.pValueRotorRELMovAttr)
        '                End With
        '                myListFwScript.Add(myFwScript1)

        '                'add to the queue list
        '                If myListFwScript.Count > 0 Then
        '                    For i As Integer = 0 To myListFwScript.Count - 1
        '                        If i = 0 Then
        '                            ' First Script
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                        Else
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                        End If
        '                    Next
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '                Else
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '                End If



        '        End Select


        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForADJUST_PREPARING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Loading operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 17/11/10</remarks>
        'Private Function SendQueueForADJUSTING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Try

        '        'NECESSARY?

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForADJUSTING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Exiting of Adjust operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/01/11</remarks>
        'Private Function SendQueueForADJUST_EXITING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Try
        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        ' Move to parking position the current arm
        '        'Script1
        '        With myFwScript1
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing

        '            Select Case pAdjustment
        '                Case ADJUSTMENT_GROUPS.WASHING_SOLUTION
        '                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString
        '                    ' expects 2 params
        '                    .ParamList = New List(Of String)
        '                    '.ParamList.Add(Me.pValueRotorABSMovAttr)


        '            End Select

        '        End With
        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForADJUST_EXITING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function



        '''' <summary>
        '''' Creates the Script List for Screen Testing operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/01/11</remarks>
        'Private Function SendQueueForTESTING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myListFwScript As New List(Of FwScriptQueueItem)
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Try
        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If



        '        'add to the queue list
        '        If myListFwScript.Count > 0 Then
        '            For i As Integer = 0 To myListFwScript.Count - 1
        '                If i = 0 Then
        '                    ' First Script
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                Else
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                End If
        '            Next
        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '        Else
        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '        End If


        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForTESTING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Exiting of Test operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 28/01/11</remarks>
        'Private Function SendQueueForTEST_EXITING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Try
        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'PENDING

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForTEST_EXITING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function
#End Region

#Region "To DELETE"
        '''' <summary>
        '''' Creates the Script List for Screen Loading operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 17/11/10</remarks>
        'Private Function SendQueueForLOADING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    'Dim myResultData As New GlobalDataTO
        '    'Try

        '    '    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '    '        myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '    '    End If


        '    '    ' PDT !!! - Es una prueba !!!

        '    '    'FOR TESTING
        '    '    '*************************************************************************************************
        '    '    Dim myFwScript1 As New FwScriptQueueItem
        '    '    Dim myFwScript2 As New FwScriptQueueItem

        '    '    'Script1
        '    '    With myFwScript1
        '    '        .FwScriptID = "action2"
        '    '        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '    '        .EvaluateValue = 1
        '    '        .NextOnResultOK = myFwScript2
        '    '        .NextOnResultNG = Nothing
        '    '        .NextOnTimeOut = Nothing
        '    '        .NextOnError = Nothing
        '    '        .ParamList = Nothing ' New List(Of String)

        '    '    End With

        '    '    'Script2
        '    '    With myFwScript2
        '    '        .FwScriptID = "action4"
        '    '        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '    '        .EvaluateValue = 1
        '    '        .NextOnResultOK = Nothing
        '    '        .NextOnResultNG = Nothing
        '    '        .NextOnTimeOut = Nothing
        '    '        .NextOnError = Nothing
        '    '        .ParamList = Nothing ' New List(Of String)
        '    '    End With

        '    '    '*************************************************************************************************************
        '    '    ''in case of specific handling the response
        '    '    '.ResponseEventHandling  = True 'enable event handling
        '    '    'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '    '    'add to the queue list
        '    '    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '    '    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
        '    '    ' PDT !!! - Es una prueba !!!

        '    'Catch ex As Exception
        '    '    myResultData.HasError = True
        '    '    myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '    '    myResultData.ErrorMessage = ex.Message

        '    '    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '    '        myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '    '    End If

        '    '    Dim myLogAcciones As New ApplicationLogManager()
        '    '    GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForLOADING", EventLogEntryType.Error, False)
        '    'End Try
        '    'Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Starting the Test operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/01/11</remarks>
        'Private Function SendQueueForTEST_BEGINING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If


        '        ' PDT !!! - Es una prueba !!!

        '        'FOR TESTING
        '        '*************************************************************************************************
        '        Dim myFwScript1 As New FwScriptQueueItem
        '        Dim myFwScript2 As New FwScriptQueueItem

        '        'Script1
        '        With myFwScript1
        '            .FwScriptID = "action2"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = myFwScript2
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)

        '        End With

        '        'Script2
        '        With myFwScript2
        '            .FwScriptID = "action4"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)
        '        End With

        '        '*************************************************************************************************************
        '        ''in case of specific handling the response
        '        '.ResponseEventHandling  = True 'enable event handling
        '        'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
        '        ' PDT !!! - Es una prueba !!!

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForTEST_BEGINING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Closing Screen operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/01/11</remarks>
        'Private Function SendQueueForCLOSING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If


        '        ' PDT !!! - Es una prueba !!!

        '        'FOR TESTING
        '        '*************************************************************************************************
        '        Dim myFwScript1 As New FwScriptQueueItem
        '        Dim myFwScript2 As New FwScriptQueueItem

        '        'Script1
        '        With myFwScript1
        '            .FwScriptID = "action2"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = myFwScript2
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)

        '        End With

        '        'Script2
        '        With myFwScript2
        '            .FwScriptID = "action4"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)
        '        End With

        '        '*************************************************************************************************************
        '        ''in case of specific handling the response
        '        '.ResponseEventHandling  = True 'enable event handling
        '        'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
        '        ' PDT !!! - Es una prueba !!!

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.SendQueueForCLOSING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function
#End Region

    End Class

End Namespace
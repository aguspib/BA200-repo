Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw

Namespace Biosystems.Ax00.FwScriptsManagement

    ''' <summary>
    ''' Delegate that manages the sending Scripts Queue List
    ''' </summary>
    ''' <remarks>Created by SG 17/11/10</remarks>
    Public Class SendFwScriptsDelegate

#Region "Declarations"
        'Communication's Application's Layer
        Private WithEvents myAnalyzer As AnalyzerManager

        'Item of the Script's Queue that is currently being processed by the Analyzer
        Private WithEvents CurrentFwScriptQueueItem As FwScriptQueueItem

        Private ActiveFwScriptsData As FwScriptsDataTO 'Scripts Data Set Against which the delegate's operations will be performed
#End Region

#Region "Attributes"
        Private CurrentFwScriptsQueueAttribute As List(Of FwScriptQueueItem)
        Private IsWaitingForResponseAttribute As Boolean
        'Private IsMonitorRequestedAttribute As Boolean

        ' XBC 09/05/2012
        Private INFOManagementEnabledAttribute As Boolean
#End Region

#Region "Constructor"
        Public Sub New(ByVal pAnalyzer As AnalyzerManager)
            Dim myGlobal As New GlobalDataTO
            myAnalyzer = pAnalyzer
            myGlobal = myAnalyzer.ReadFwScriptData()
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                ActiveFwScriptsData = CType(myGlobal.SetDatos, FwScriptsDataTO)
            End If

            ' XBC 09/05/2012
            MyClass.INFOManagementEnabledAttribute = True
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Scripts Data object against which the delegate works
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Public Property ActiveFwScriptsDO() As FwScriptsDataTO
            Get
                Return MyClass.ActiveFwScriptsData
            End Get
            Set(ByVal value As FwScriptsDataTO)
                MyClass.ActiveFwScriptsData = value
            End Set
        End Property

        ''' <summary>
        ''' List of Script Queue items that is currently being processed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Public Property CurrentFwScriptsQueue() As List(Of FwScriptQueueItem)
            Get
                Return CurrentFwScriptsQueueAttribute
            End Get
            Set(ByVal value As List(Of FwScriptQueueItem))
                CurrentFwScriptsQueueAttribute = value
                If value Is Nothing Then
                    CurrentFwScriptQueueItem = Nothing
                End If
            End Set
        End Property

        ''' <summary>
        ''' Defines if there is any response pending to be received
        ''' </summary>
        ''' <value></value>
        ''' <returns>boolean</returns>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Public Property IsWaitingForResponse() As Boolean
            Get
                Return IsWaitingForResponseAttribute
            End Get
            Set(ByVal value As Boolean)
                If IsWaitingForResponseAttribute <> value Then
                    IsWaitingForResponseAttribute = value
                End If
            End Set
        End Property

        'Public Property IsMonitorRequested() As Boolean
        '    Get
        '        Return IsMonitorRequestedAttribute
        '    End Get
        '    Set(ByVal value As Boolean)
        '        If Not IsMonitorRequestedAttribute And value Then
        '            'reset the scriptqueueitem
        '            'MyClass.CurrentFwScriptQueueItem = Nothing
        '        End If
        '        IsMonitorRequestedAttribute = value
        '    End Set
        'End Property

        Public Property AnalyzerManager() As AnalyzerManager
            Get
                Return myAnalyzer
            End Get
            Set(ByVal value As AnalyzerManager)
                myAnalyzer = value
            End Set
        End Property

        Public ReadOnly Property CurrentFwScriptItem() As FwScriptQueueItem
            Get
                Return CurrentFwScriptQueueItem
            End Get
        End Property

        Private CurrentFwScriptsQueueAbortedAttr As Boolean = False

        ''' <summary>
        ''' Abort the current Scripts Queue
        ''' </summary>
        ''' <remarks>Created by SGM 13/121/2011</remarks>
        Public WriteOnly Property CurrentFwScriptsQueueAborted() As Boolean
            Set(ByVal value As Boolean)
                CurrentFwScriptsQueueAbortedAttr = value
            End Set
        End Property


        ' XBC 09/05/2012
        Public Property INFOManagementEnabled() As Boolean
            Get
                Return MyClass.INFOManagementEnabledAttribute
            End Get
            Set(ByVal value As Boolean)
                MyClass.INFOManagementEnabledAttribute = value
            End Set
        End Property
#End Region

#Region "Communication Events"
        'event that manages the response of the Analyzer after sending a Script List
        Public Event LastFwScriptResponseEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) 'end of the scripts queue

        'event that manages every data received from the Analyzer
        Public Event DataReceivedEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) 'any data from analyzer

        'event that manages every response data from the Analyzer if the sender 
        '(Script Queue Item) settings allow it (ResponseEventHandling=true)
        Public Event FwScriptResponseEvent(ByVal sender As Object, ByVal e As System.EventArgs)

#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Adds a Script queue item to the list
        ''' </summary>
        ''' <param name="pFwScriptQueueItem"></param>
        ''' <param name="pFirstItem"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Public Function AddToFwScriptQueue(ByVal pFwScriptQueueItem As FwScriptQueueItem, ByVal pFirstItem As Boolean) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim createFirst As Boolean
            Try
                If pFwScriptQueueItem IsNot Nothing Then

                    createFirst = False
                    If Me.CurrentFwScriptsQueue Is Nothing Then
                        createFirst = True
                    Else
                        If Me.CurrentFwScriptsQueue.Count = 0 Then
                            createFirst = True
                        End If
                    End If

                    If createFirst Then
                        Me.CurrentFwScriptsQueue = New List(Of FwScriptQueueItem)
                        If pFirstItem Then
                            CurrentFwScriptQueueItem = pFwScriptQueueItem
                        End If
                    End If
                    Me.CurrentFwScriptsQueue.Add(pFwScriptQueueItem)
                Else
                    ' PDT !!! Controlar error
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                CurrentFwScriptsQueueAttribute.Clear()

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.AddToFwScriptQueue", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Set a Script Queue item as first
        ''' </summary>
        ''' <param name="pFwScriptQueueItem"></param>
        ''' <remarks>Created by SGM 18/10/2011</remarks>
        Public Sub SetQueueItemAsFirst(ByVal pFwScriptQueueItem As FwScriptQueueItem)
            Try
                If pFwScriptQueueItem IsNot Nothing Then
                    If Me.CurrentFwScriptsQueue IsNot Nothing Then
                        If Me.CurrentFwScriptsQueue.Contains(pFwScriptQueueItem) Then
                            CurrentFwScriptQueueItem = pFwScriptQueueItem
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.SetQueueItemAsFirst", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Starts the managing of the list of the Scripts' list sent
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SG 17/11/10
        ''' Modified by XBC 02/11/2011 - Activate ANSINF when Command operations have finished
        ''' Modified by XBC 19/10/2012 - Add previous condition to allow send command instructions to Analyzer and malfunctions management
        ''' </remarks>
        Public Function StartFwScriptQueue(Optional ByVal pFwScriptQueueItem As FwScriptQueueItem = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Try
                If myAnalyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then   ' XBC 19/10/2012
                    If Not IsWaitingForResponse Then
                        ' XBC 02/11/2011 - Deactivate ANSINF before Command operations have started
                        If INFOManagementEnabledAttribute Then
                            myResultData = MyClass.SEND_INFO_STOP()
                        End If

                        If Not myResultData.HasError Then
                            myResultData = Me.ManageFwScriptsQueueSending(pFwScriptQueueItem)
                        Else
                            ' XBC 19/10/2012
                            'Debug.Print("Captura Error 1 !")
                            myLogAcciones.CreateLogActivity("Send Info Stop Malfunction", "SendFwScriptsDelegate.StartFwScriptQueue", EventLogEntryType.Error, False)
                            ' XBC 19/10/2012
                        End If
                    End If
                Else
                    ' XBC 19/10/2012
                    myLogAcciones.CreateLogActivity("Try send Command when Analyzer no is on StandBy mode", "SendFwScriptsDelegate.StartFwScriptQueue", EventLogEntryType.Error, False)
                    myResultData.HasError = True
                    ' XBC 19/10/2012
                End If

                If myResultData.HasError Then
                    ' XBC 19/10/2012
                    'Debug.Print("Captura Error 2 !")
                    myLogAcciones.CreateLogActivity("Sending Fw Scripts Malfunction", "SendFwScriptsDelegate.StartFwScriptQueue", EventLogEntryType.Error, False)
                    ' XBC 19/10/2012
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.StartFwScriptQueue", EventLogEntryType.Error, False)
            End Try

            Return myResultData
        End Function

        ''' <summary>
        ''' Sends to the Communication's layer the Script ID in order to send the Script to the Analyzer
        ''' </summary>
        ''' <param name="pFwScriptQueueItem"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Public Function SendFwScript(ByVal pFwScriptQueueItem As FwScriptQueueItem) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                'SGM 15/03/11

                IsWaitingForResponse = True


                myResultData = myAnalyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND, _
                                                         True, _
                                                         Nothing, _
                                                         Nothing, _
                                                         pFwScriptQueueItem.FwScriptID, _
                                                         pFwScriptQueueItem.ParamList)


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.SendFwScript", EventLogEntryType.Error, False)
            End Try

            If myResultData.HasError Then
                IsWaitingForResponse = False
            End If

            Return myResultData
        End Function

        ''' <summary>
        ''' Sends to the Communication's layer the Script ID in order to send the Script to the Analyzer for testing instructions
        ''' </summary>
        ''' <param name="pInstructions"></param>
        ''' <param name="pParams"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 20/09/2011</remarks>
        Public Function SendFwScriptTest(ByVal pInstructions As List(Of InstructionTO), ByVal pParams As List(Of String)) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                IsWaitingForResponse = True

                myResultData = myAnalyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND, _
                                                         True, _
                                                         Nothing, _
                                                         pInstructions, _
                                                         "", _
                                                         pParams)


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.SendFwScriptTest", EventLogEntryType.Error, False)
            End Try

            IsWaitingForResponse = Not myResultData.HasError

            Return myResultData
        End Function

        ''' <summary>
        ''' Sends to the Communication's layer the Sensors 
        ''' </summary>
        ''' <param name="pFwScriptQueueItem"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 1/04/11</remarks>
        Public Function SendSensorsFwScript(ByVal pFwScriptQueueItem As FwScriptQueueItem) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                myResultData = myAnalyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND, _
                                                         True, _
                                                         Nothing, _
                                                         "", _
                                                         pFwScriptQueueItem.FwScriptID, _
                                                         pFwScriptQueueItem.ParamList)


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.SendSensorsFwScript", EventLogEntryType.Error, False)
            End Try

            Return myResultData
        End Function

        ''' <summary>
        ''' public procedure to send Start Info Refreshing Mode
        ''' </summary>
        ''' <remarks>Created by XBC 02/11/2011</remarks>
        Public Function SEND_INFO_START() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.IsWaitingForResponse = False

                If myAnalyzer.IsAutoInfoActivated Then Return myResultData

                myResultData = myAnalyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                         True, _
                                                         Nothing, _
                                                         GlobalEnumerates.Ax00InfoInstructionModes.STR)
                If Not myResultData.HasError Then
                    myAnalyzer.IsAutoInfoActivated = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.SEND_INFO_START", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' public procedure to send Stop Info Refreshing Mode
        ''' </summary>
        ''' <remarks>Created by XBC 02/11/2011</remarks>
        Public Function SEND_INFO_STOP() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                If Not myAnalyzer.IsAutoInfoActivated Then Return myResultData

                myResultData = myAnalyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                         True, _
                                                         Nothing, _
                                                         GlobalEnumerates.Ax00InfoInstructionModes.STP)

                If Not myResultData.HasError Then
                    myAnalyzer.IsAutoInfoActivated = False
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.SEND_INFO_STOP", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        '''' <summary>
        '''' Starts the managing of the list of the Scripts' list sent
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 17/11/10</remarks>
        'Public Function StartSensorsFwScriptQueue(Optional ByVal pFwScriptQueueItem As FwScriptQueueItem = Nothing) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        If IsMonitorRequestedAttribute Then
        '            myResultData = Me.ManageFwScriptsQueueSending(pFwScriptQueueItem)
        '        End If

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.StartMonitorFwScriptQueue", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary>
        ''' Stops the managing of the list of the Scripts
        ''' </summary>
        ''' <remarks>
        ''' Created by XBC 22/10/2012
        ''' </remarks>
        Public Sub StopFwScriptQueue()
            Dim myLogAcciones As New ApplicationLogManager()
            Try
                OmmitScriptResponseByAlarmAttr = True 'SGM 24/10/2012 
                If Not CurrentFwScriptsQueueAttribute Is Nothing Then
                    CurrentFwScriptsQueueAttribute.Clear()
                    CurrentFwScriptQueueItem = Nothing
                End If
                IsWaitingForResponse = False

                myLogAcciones.CreateLogActivity("Stop FW Script Queue", "SendFwScriptsDelegate.StopFwScriptQueue", EventLogEntryType.Information, False)

            Catch ex As Exception
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.StopFwScriptQueue", EventLogEntryType.Error, False)
            End Try

        End Sub

        'SGM 24/10/2012 
        Private OmmitScriptResponseByAlarmAttr As Boolean = False

        'SGM 24/10/2012 
        Private ReadOnly Property OmmitScriptResponseByAlarm() As Boolean
            Get
                Return OmmitScriptResponseByAlarmAttr
            End Get
        End Property



#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Manages the sending of the resultant next Script
        ''' </summary>
        ''' <param name="pFwScriptItem"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Private Function ManageFwScriptsQueueSending(Optional ByVal pFwScriptItem As FwScriptQueueItem = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                If CurrentFwScriptsQueueAttribute.Count > 0 Then

                    If pFwScriptItem IsNot Nothing Then
                        CurrentFwScriptQueueItem = pFwScriptItem
                    End If


                    If CurrentFwScriptQueueItem IsNot Nothing AndAlso CurrentFwScriptQueueItem.FwScriptID <> Nothing AndAlso CurrentFwScriptsQueueAttribute.Contains(CurrentFwScriptQueueItem) Then

                        myResultData = MyClass.SendFwScript(CurrentFwScriptQueueItem)

                    Else
                        myResultData.HasError = True
                        myResultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
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
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.ManageFwScriptsQueueSending", EventLogEntryType.Error, False)
            End Try
            Return myResultData

        End Function

        ''' <summary>
        ''' Determines which is the current Script to send according to the response's evaluation
        ''' </summary>
        ''' <param name="pResponseData"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SG 17/11/10
        ''' Modified by XBC 02/11/2011 - Activate ANSINF when Command operations have finished
        ''' </remarks>
        Private Function ManageFwScriptsQueueReceiving(ByVal pResponseData As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                If IsWaitingForResponse And CurrentFwScriptQueueItem Is Nothing Then
                    myResultData.HasError = True
                    myResultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                    Return myResultData
                End If

                If CurrentFwScriptQueueItem IsNot Nothing Then

                    IsWaitingForResponse = False

                    myResultData = EvaluateCurrentFwScriptResponse(pResponseData)

                    If Not myResultData.HasError And Not myResultData Is Nothing Then
                        Dim myResult As GlobalEnumerates.RESPONSE_TYPES = CType(myResultData.SetDatos, GlobalEnumerates.RESPONSE_TYPES)
                        CurrentFwScriptQueueItem.ResponseData = pResponseData
                        CurrentFwScriptQueueItem.Response = myResult

                        'if the script is not defined for handling the response it address to the next Script
                        If Not CurrentFwScriptQueueItem.ResponseEventHandling Then

                            Dim myNextFwScriptQueueItem As FwScriptQueueItem = Nothing

                            Select Case myResult
                                Case GlobalEnumerates.RESPONSE_TYPES.OK

                                    myNextFwScriptQueueItem = CurrentFwScriptQueueItem.NextOnResultOK

                                    'functionality for aborting a scripts queue
                                    'If Not MyClass.CurrentFwScriptsQueueAbortedAttr Then
                                    '    myNextFwScriptQueueItem = CurrentFwScriptQueueItem.NextOnResultOK
                                    'Else
                                    '    myNextFwScriptQueueItem = Nothing
                                    '    MyClass.CurrentFwScriptsQueueAbortedAttr = False
                                    'End If

                                Case GlobalEnumerates.RESPONSE_TYPES.NG
                                    myNextFwScriptQueueItem = CurrentFwScriptQueueItem.NextOnResultNG

                                Case GlobalEnumerates.RESPONSE_TYPES.TIMEOUT
                                    myNextFwScriptQueueItem = CurrentFwScriptQueueItem.NextOnTimeOut


                                Case GlobalEnumerates.RESPONSE_TYPES.EXCEPTION
                                    myNextFwScriptQueueItem = CurrentFwScriptQueueItem.NextOnError

                                Case RESPONSE_TYPES.START
                                    CurrentFwScriptQueueItem.TimeExpected = myAnalyzer.MaxWaitTime

                            End Select

                            If myNextFwScriptQueueItem IsNot Nothing Then
                                myResultData = ManageFwScriptsQueueSending(myNextFwScriptQueueItem)
                            Else
                                ' XBC 02/11/2011 - Activate ANSINF when Command operations have finished
                                If INFOManagementEnabledAttribute Then
                                    myResultData = MyClass.SEND_INFO_START()

                                End If
                                If Not myResultData.HasError Then
                                    ' XBC 02/11/2011 - Activate ANSINF when Command operations have finished

                                    RaiseEvent LastFwScriptResponseEvent(myResult, pResponseData)
                                    'SGM 02/02/2011 Clear only if not pending responses
                                    If Not IsWaitingForResponse Then
                                        CurrentFwScriptQueueItem = Nothing
                                        CurrentFwScriptsQueueAttribute.Clear()
                                    End If

                                End If
                            End If

                        End If
                    Else
                        myResultData.HasError = True
                        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If

                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.ManageFwScriptsQueueReceiving", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Evaluates the returned data with the expected data in the current Script
        ''' </summary>
        ''' <param name="pResponseData"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Private Function EvaluateCurrentFwScriptResponse(ByVal pResponseData As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                If CurrentFwScriptQueueItem IsNot Nothing Then

                    If CurrentFwScriptQueueItem.EvaluateType <> GlobalEnumerates.EVALUATE_TYPES.NONE Then
                        If pResponseData.Trim <> "" Then
                            Select Case CurrentFwScriptQueueItem.EvaluateType

                                Case GlobalEnumerates.EVALUATE_TYPES.TEXT_VALUE
                                    myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK

                                Case GlobalEnumerates.EVALUATE_TYPES.NUM_VALUE
                                    Dim myReceivedValue As Double = CType(pResponseData, Double)
                                    myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK

                                Case GlobalEnumerates.EVALUATE_TYPES.SENSOR_VALUE
                                    myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK

                                Case GlobalEnumerates.EVALUATE_TYPES.EQUAL
                                    Dim myReceivedValue As Double = CType(pResponseData, Double)
                                    Dim myEvalValue As Double = CType(CurrentFwScriptQueueItem.EvaluateValue, Double)
                                    If myReceivedValue = myEvalValue Then
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK
                                    Else
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.NG
                                    End If


                                Case GlobalEnumerates.EVALUATE_TYPES.HIGHER
                                    Dim myReceivedValue As Double = CType(pResponseData, Double)
                                    Dim myEvalValue As Double = CType(CurrentFwScriptQueueItem.EvaluateValue, Double)
                                    If myReceivedValue > myEvalValue Then
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK
                                    Else
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.NG
                                    End If


                                Case GlobalEnumerates.EVALUATE_TYPES.LOWER
                                    Dim myReceivedValue As Double = CType(pResponseData, Double)
                                    Dim myEvalValue As Double = CType(CurrentFwScriptQueueItem.EvaluateValue, Double)
                                    If myReceivedValue < myEvalValue Then
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK
                                    Else
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.NG
                                    End If

                                Case GlobalEnumerates.EVALUATE_TYPES.EQUAL_HIGHER
                                    Dim myReceivedValue As Double = CType(pResponseData, Double)
                                    Dim myEvalValue As Double = CType(CurrentFwScriptQueueItem.EvaluateValue, Double)
                                    If myReceivedValue >= myEvalValue Then
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK
                                    Else
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.NG
                                    End If

                                Case GlobalEnumerates.EVALUATE_TYPES.EQUAL_LOWER
                                    Dim myReceivedValue As Double = CType(pResponseData, Double)
                                    Dim myEvalValue As Double = CType(CurrentFwScriptQueueItem.EvaluateValue, Double)
                                    If myReceivedValue >= myEvalValue Then
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK
                                    Else
                                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.NG
                                    End If

                            End Select
                        Else
                            CurrentFwScriptQueueItem.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            myResultData.HasError = True
                            myResultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.EXCEPTION
                        End If
                    Else
                        myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.OK
                    End If

                Else
                    myResultData.HasError = True
                    myResultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                End If

            Catch ex As Exception
                If CurrentFwScriptQueueItem IsNot Nothing Then
                    CurrentFwScriptQueueItem.ErrorCode = ex.Message
                End If
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message
                myResultData.SetDatos = GlobalEnumerates.RESPONSE_TYPES.EXCEPTION

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.EvaluateCurrentFwScriptResponse", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' Event to Catch Reception Event from Communications layer
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <remarks>
        ''' Created by XBC 04/05/2011 - Separation between FwScripts Low level Instructions and High level Instructions
        ''' </remarks>
        Public Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                          ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), _
                                          ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Handles myAnalyzer.ReceptionEvent
            Try
                If pTreated Then
                    'Refresh button bar the Ax00 and the connection Status 
                    Select Case myAnalyzer.InstructionTypeReceived

                        Case AnalyzerManagerSwActionList.STATUS_RECEIVED
                            '
                            ' STATUS RECEIVED
                            '
                            Select Case myAnalyzer.AnalyzerCurrentAction

                                Case AnalyzerManagerAx00Actions.COMMAND_START

                                    If CurrentFwScriptQueueItem IsNot Nothing Then
                                        CurrentFwScriptQueueItem.TimeExpected = myAnalyzer.MaxWaitTime
                                    End If

                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.START, Nothing)

                                Case AnalyzerManagerAx00Actions.COMMAND_END

                                    ' XBC 22/10/2012 - Canceled (new alarms management)
                                    'If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                    '    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.EXCEPTION, Nothing)
                                    'Else
                                    '    'Dim myLogAcciones As New ApplicationLogManager()
                                    '    'myLogAcciones.CreateLogActivity("AnalyzerManagerAx00Actions.COMMAND_END [myAnalyzer.ReceptionEvent]", "SendFwScriptsDelegate.OnManageReceptionEvent", EventLogEntryType.Information, False)

                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)
                                    'End If

                                Case AnalyzerManagerAx00Actions.LOADADJ_START

                                    If CurrentFwScriptQueueItem IsNot Nothing Then
                                        CurrentFwScriptQueueItem.TimeExpected = myAnalyzer.MaxWaitTime
                                    End If

                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.START, Nothing)

                                Case AnalyzerManagerAx00Actions.LOADADJ_END

                                    ' XBC 22/10/2012 - Canceled (new alarms management)
                                    'If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                    '    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.EXCEPTION, Nothing)
                                    'Else
                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)
                                    'End If

                                    ' XBC 23/12/2011
                                Case AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED

                                    If CurrentFwScriptQueueItem IsNot Nothing Then
                                        CurrentFwScriptQueueItem.TimeExpected = myAnalyzer.MaxWaitTime
                                    End If

                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.START, Nothing)

                                    ' XBC 02/05/2012
                                Case AnalyzerManagerAx00Actions.STRESS_START

                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)


                                    '    ' XBC 04/06/2012
                                    'Case AnalyzerManagerAx00Actions.UTIL_START

                                    '    If CurrentFwScriptQueueItem IsNot Nothing Then
                                    '        CurrentFwScriptQueueItem.TimeExpected = myAnalyzer.MaxWaitTime
                                    '    End If

                                    '    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.START, Nothing)


                                    '    ' XBC 04/06/2012

                                    'Case AnalyzerManagerAx00Actions.UTIL_END

                                    '    If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                    '        RaiseEvent DataReceivedEvent(RESPONSE_TYPES.EXCEPTION, Nothing)
                                    '    Else
                                    '        RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)
                                    '    End If


                                    ' XBC 23/10/2012
                                Case AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_START

                                    If CurrentFwScriptQueueItem IsNot Nothing Then
                                        CurrentFwScriptQueueItem.TimeExpected = myAnalyzer.MaxWaitTime
                                    End If

                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.START, Nothing)


                                    ' XBC 23/10/2012

                                Case AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END

                                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)

                            End Select


                        Case AnalyzerManagerSwActionList.ADJUSTMENTS_RECEIVED
                            '
                            ' ADJUSTMENTS RECEIVED (high level)
                            '
                            RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)


                            'Case AnalyzerManagerSwActionList.ANSINF_RECEIVED   emplaced into MainMDI !
                            '    '
                            '    ' ANSINF RECEIVED (high level)
                            '    '
                            '    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)


                        Case AnalyzerManagerSwActionList.BASELINE_RECEIVED
                            '
                            ' BASELINE RECEIVED (high level)
                            '
                            RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)

                        Case AnalyzerManagerSwActionList.ANSSDM
                            '
                            ' STRESS RECEIVED (high level)
                            '
                            RaiseEvent DataReceivedEvent(RESPONSE_TYPES.OK, Nothing)

                            ' Comented by XBC 05/05/2011 
                            ' This case is replaced in the functionality way ManageFwCommandAnswer called from AnalyzerManager.ProcessFwCommandAnswerReceived
                            'Case AnalyzerManagerSwActionList.COMMAND_RECEIVED
                            '    '
                            '    ' COMMAND RECEIVED (low level)
                            '    '
                            '    ManageCommandReception(pInstructionReceived, True)

                    End Select

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.OnManageReceptionEvent", EventLogEntryType.Error, False)
            End Try
        End Sub

        Public Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles myAnalyzer.SendEvent
            Dim myResultData As New GlobalDataTO
            Try
                ' XBC 05/05/2011 - timeout limit repetitions
                If pInstructionSent.Contains(AnalyzerManagerSwActionList.TRYING_CONNECTION.ToString) Then
                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.TIMEOUT, AnalyzerManagerSwActionList.TRYING_CONNECTION)
                ElseIf pInstructionSent.Contains(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString) Then
                    RaiseEvent DataReceivedEvent(RESPONSE_TYPES.TIMEOUT, AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED)
                End If
                ' XBC 05/05/2011 - timeout limit repetitions

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.OnManageReceptionEvent", EventLogEntryType.Error, False)
            Finally
                If myResultData.HasError Then
                    ' PDT !!! pending to implement error answers !!!
                    RaiseEvent LastFwScriptResponseEvent(RESPONSE_TYPES.EXCEPTION, pInstructionSent)
                    CurrentFwScriptQueueItem = Nothing
                    CurrentFwScriptsQueueAttribute.Clear()
                End If
            End Try

        End Sub

        ''' <summary>
        ''' Manages the data received from the Analyzer
        ''' </summary>
        ''' <param name="pResponseData"></param>
        ''' <param name="pTreated"></param>
        ''' <remarks>
        ''' Created by SG 17/11/10
        ''' Modified by XBC 04/05/2011 - Separation between FwScripts Low level Instructions and High level Instructions
        ''' </remarks>
        Public Sub OnManageReceptionEvent(ByVal pResponseData As String, _
                                          ByVal pResponseValue As String, _
                                          ByVal pTreated As Boolean) Handles myAnalyzer.ReceptionFwScriptEvent
            Dim myResultData As New GlobalDataTO
            Try

                If IsWaitingForResponse Then
                    If pTreated Then '(1)

                        If myAnalyzer.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND_RECEIVED Then  '(2)

                            myResultData = MyClass.ManageFwScriptsQueueReceiving(pResponseValue)

                            'If Not myResultData.HasError And Not myResultData Is Nothing Then
                            '    Dim myLogAcciones As New ApplicationLogManager()
                            '    myLogAcciones.CreateLogActivity("Instruction received : " & pResponseData.ToString, _
                            '                                    "SendFwScriptsDelegate.OnManageReceptionEvent", EventLogEntryType.Information, False)
                            'End If

                        End If '(2)

                    Else
                        ' TEST !!! provant que vingui un error a les comunicacions !!!
                        myResultData = MyClass.ManageFwScriptsQueueReceiving("") ' PDT !!! - Això és una prova !!!
                    End If '(1)
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.OnManageReceptionEvent", EventLogEntryType.Error, False)
            Finally
                If myResultData.HasError Then
                    ' PDT !!! pending to implement error answers !!!
                    RaiseEvent LastFwScriptResponseEvent(RESPONSE_TYPES.EXCEPTION, pResponseData)
                    CurrentFwScriptQueueItem = Nothing
                    CurrentFwScriptsQueueAttribute.Clear()
                End If
            End Try
        End Sub

#End Region

#Region "FwScript Response Handling"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Private Sub OnFwScriptResponse(ByVal sender As Object, ByVal e As System.EventArgs) Handles CurrentFwScriptQueueItem.ResponseEvent
            Try
                'SGM 24/10/2012 
                If Not OmmitScriptResponseByAlarm Then
                    RaiseEvent FwScriptResponseEvent(sender, e)
                End If

                'RaiseEvent FwScriptResponseEvent(sender, e) SGM 24/10/2012

                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity("FwScript Response Handling [Handles CurrentFwScriptQueueItem.ResponseEvent]", "SendFwScriptsDelegate.OnFwScriptResponse", EventLogEntryType.Information, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.FwScriptResponseOK", EventLogEntryType.Error, False)
            End Try
        End Sub


#End Region

    End Class

End Namespace
Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.FwScriptsManagement

    ''' <summary>
    ''' Base class to include generical functionalities in the management of the Scripts
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 22/11/2010
    ''' Modified by XBC 12/09/2011 : 
    '''    Add IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    '''    Is need to implement a calling myScreenDelegate.Dispose() in each Form's screen that implements delegates which Inherit from BaseFwScriptDelegate
    ''' </remarks>
    Public Class BaseFwScriptDelegate
        Implements IDisposable

#Region "Constructor"
        Public Sub New()

        End Sub
        Public Sub New(ByVal pAnalyzerID As String) 'SGM 20/01/2012
            AnalyzerId = pAnalyzerID
        End Sub
#End Region
        

        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            Try
                MyClass.myFwScriptDelegate = Nothing
            Catch ex As Exception
                Throw ex
            Finally
                GC.SuppressFinalize(Me)
            End Try
        End Sub

#Region "Declarations"
        'delegate for sending/receiving to/from the Communications Layer
        Public WithEvents myFwScriptDelegate As SendFwScriptsDelegate

        Private pValueSensorsAttr As String

        'Protected Friend AnalyzerIdAttr As String = "" 'SGM 20/01/2012     'MANEL
#End Region

#Region "Properties"

        Public Property pValueSensors() As String
            Get
                Return Me.pValueSensorsAttr
            End Get
            Set(ByVal value As String)
                Me.pValueSensorsAttr = value
            End Set
        End Property

        'SGM 20/01/2012
        Public Property AnalyzerId() As String
#End Region

#Region "Communication Events"
        'event that manages the response of the Analyzer after sending a Script List
        Public Event ReceivedLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object)

        'event that manages every data received from the Analyzer
        Public Event DataReceived(ByVal pTreated As Boolean)

        'SGM 14/03/11
        Public Event ReceivedSensorDataEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object)
#End Region

#Region "Communication Event Handlers"
        ''' <summary>
        ''' manages the response of the Analyzer after sending a Script List
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Public Sub OnReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myFwScriptDelegate.LastFwScriptResponseEvent
            Try
                ' manage the entry data for Low level Instructions

                ' throw the event to the Presentation layer
                RaiseEvent ReceivedLastFwScriptEvent(pResponse, pData)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.OnReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' manages every data received from the Analyzer
        ''' </summary>
        ''' <param name="pResponse">types response</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Private Sub OnDataReceivedFromAnalyzer(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myFwScriptDelegate.DataReceivedEvent
            Try
                ' manage the entry data for High level Instructions

                ' throw the event to the Presentation layer
                RaiseEvent ReceivedLastFwScriptEvent(pResponse, pData)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.OnDataReceivedFromAnalyzer", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "Public Methods"
        'Public Function SendSensorsFwScriptsQueueList() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        ' Create the list of Scripts which are need to initialize this Adjustment
        '        If Not Me.myFwScriptDelegate.IsMonitorRequested Then
        '            Me.myFwScriptDelegate.IsMonitorRequested = True
        '            myResultData = MyClass.SendQueueForREADINGSENSORS
        '        Else
        '            myResultData = myResultData 'QUITAR
        '        End If


        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "SendMonitorFwScriptsQueueList.SendQueueForREADINGADJUSTMENTS", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary>
        ''' Creates the Script List for Reading Adjustments from Instruments operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 12/01/11
        ''' Modified by XBC 23/02/11 - Move function to Form Base according with his general functionality
        ''' </remarks>
        Protected Friend Function SendQueueForREADINGADJUSTMENTS() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myFwScript1 As New FwScriptQueueItem

                'Script1
                With myFwScript1
                    .FwScriptID = FwSCRIPTS_IDS.READ_ADJUSTMENTS.ToString
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
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendQueueForREADINGADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Sensors REading operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 25/02/11</remarks>
        Protected Friend Function SendQueueForREADINGSENSORS() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                With myFwScript1
                    .FwScriptID = FwSCRIPTS_IDS.READ_SENSORS.ToString
                    .EvaluateType = EVALUATE_TYPES.SENSOR_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    ' expects 1 param
                    .ParamList = New List(Of String)
                    .ParamList.Add(Me.pValueSensorsAttr)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendQueueForSAVING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#Region "Preliminary Homes"

        ''' <summary>
        ''' Inserts a copy of master data for a new Analyzer detected
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/01/2012</remarks>
        Public Function InsertNewAnalyzerPreliminaryHomes(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myPreliminaryDAO As New tadjPreliminaryHomesDAO
                resultData = myPreliminaryDAO.InsertAnalyzerPreliminaryHomes(pDBConnection, MyClass.AnalyzerId)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.InsertNewAnalyzerPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pGroupID"></param>
        '''' <returns></returns>
        '''' <remarks>SGM 02/02/2011</remarks>
        'Public Function GetPreliminaryHomesByGroupID(ByVal pAnalyzerID As String, ByVal pGroupID As String) As GlobalDataTO
        '    Dim resultdata As New GlobalDataTO
        '    Try
        '        Dim myHomesDAO As New tadjPreliminaryHomesDAO
        '        resultdata = myHomesDAO.GetPreliminaryHomes(Nothing, pAnalyzerID, pGroupID)

        '    Catch ex As Exception
        '        resultdata.HasError = True
        '        resultdata.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultdata.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "GetPreliminaryHomes", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultdata
        'End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 02/02/2011</remarks>
        Public Function GetAllPreliminaryHomes(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultdata As New GlobalDataTO
            Try
                Dim myHomesDAO As New tadjPreliminaryHomesDAO
                resultdata = myHomesDAO.GetAllPreliminaryHomes(pDBConnection, MyClass.AnalyzerId)

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "GetAllPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return resultdata
        End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pHomeID"></param>
        '''' <returns></returns>
        '''' <remarks>SGM 02/02/2011</remarks>
        'Public Function SetSpecificPreliminaryHomeAsDone(ByVal pHomeID As String) As GlobalDataTO
        '    Dim resultdata As New GlobalDataTO
        '    Try
        '        Dim myHomesDAO As New tadjPreliminaryHomesDAO
        '        resultdata = myHomesDAO.SetPreliminaryHomeAsDone(Nothing, pHomeID)

        '    Catch ex As Exception
        '        resultdata.HasError = True
        '        resultdata.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultdata.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "SetPreliminaryHomeAsDone", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultdata
        'End Function


        '''' <summary>
        '''' Reset the preliminary homes related to the current adjustment
        '''' </summary>
        '''' <param name="pAdjustment"></param>
        '''' <returns></returns>
        '''' <remarks>SGM 02/02/2011</remarks>
        'Public Function ResetPreliminaryHomes(ByVal pAnalyzerID As String, ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        Dim myHomes As New tadjPreliminaryHomesDAO

        '        myGlobal = myHomes.ResetPreliminaryHomesByGroupID(Nothing, pAnalyzerID, pAdjustment.ToString)

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ResetPreliminaryHomes", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 02/02/2011</remarks>
        Public Function ResetAllPreliminaryHomes(ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultdata As New GlobalDataTO
            Try
                Dim myHomesDAO As New tadjPreliminaryHomesDAO
                resultdata = myHomesDAO.ResetPreliminaryHomes(Nothing, pAnalyzerID)

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ResetPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return resultdata
        End Function


        ''' <summary>
        ''' get pending preliminary homes
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 02/02/2011</remarks>
        Public Function GetPendingPreliminaryHomes(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDS As SRVPreliminaryHomesDS
                Dim myPendingHomesList As New List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow)

                myGlobal = myHomes.GetPreliminaryHomesByAdjID(Nothing, MyClass.AnalyzerId, pAdjustment.ToString)
                If myGlobal IsNot Nothing AndAlso Not myGlobal.HasError Then
                    myHomesDS = CType(myGlobal.SetDatos, SRVPreliminaryHomesDS)

                    myPendingHomesList = (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
                                    Where a.AnalyzerID = MyClass.AnalyzerId And a.AdjustmentGroupID = pAdjustment.ToString And a.Done = False Select a).ToList

                End If

                myGlobal.AffectedRecords = myPendingHomesList.Count 'SGM 28/11/2012 - inform about there are pending homes
                myGlobal.SetDatos = myPendingHomesList

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "GetPendingPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        Protected Friend Function GetPendingPreliminaryHomeScripts(ByVal pAdjGroup As ADJUSTMENT_GROUPS, ByVal pNextScript As FwScriptQueueItem) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myAllHomeScriptList As New List(Of FwScriptQueueItem)
            Dim myAllHomeIds As New List(Of String)

            Try


                'Dim myHomeScriptList As New List(Of FwScriptQueueItem)

                'get the pending Homes  
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDS As SRVPreliminaryHomesDS

                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, MyClass.AnalyzerId, pAdjGroup.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

                    Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
                                    Where a.Done = False Select a).ToList

                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                        If Not myAllHomeIds.Contains(H.RequiredHomeID) Then
                            myAllHomeIds.Add(H.RequiredHomeID)
                            Dim myFwScript As New FwScriptQueueItem
                            myAllHomeScriptList.Add(myFwScript)
                        End If
                    Next

                End If


                Dim i As Integer = 0
                For Each H As String In myAllHomeIds
                    'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                    If i = myAllHomeScriptList.Count - 1 Then
                        'Last index
                        With myAllHomeScriptList(i)
                            .FwScriptID = H
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = pNextScript
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = pNextScript
                            .NextOnError = Nothing
                            .ParamList = Nothing
                        End With
                    Else
                        With myAllHomeScriptList(i)
                            .FwScriptID = H
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = myAllHomeScriptList(i + 1)
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = myAllHomeScriptList(i + 1)
                            .NextOnError = Nothing
                            .ParamList = Nothing
                        End With
                    End If
                    i += 1
                Next

                myResultData.SetDatos = myAllHomeScriptList

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.GetPendingPreliminaryHomeScripts", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Set the performed homes as done
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 02/02/2011</remarks>
        Public Function SetPreliminaryHomesAsDone(ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myPendingHomesList As New List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow)
                myGlobal = Me.GetPendingPreliminaryHomes(pAdjustmentGroup)
                If myGlobal IsNot Nothing AndAlso Not myGlobal.HasError Then
                    myPendingHomesList = CType(myGlobal.SetDatos, List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow))

                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList

                        myGlobal = myHomes.SetPreliminaryHomeAsDone(Nothing, AnalyzerId, H.RequiredHomeID)
                        If Not myGlobal.HasError Then

                        Else
                            Exit For
                        End If
                    Next

                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SetPreliminaryHomesAsDone", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Reset an specified Prelimimnary Home
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>XBC 12/11/2012
        ''' AG 01/10/2014 BA-1953 adapt method in order to receive a list of required homes not only 1</remarks>
        Public Function ResetSpecifiedPreliminaryHomes(ByVal pAnalyzerID As String, ByVal pListRequiredHomeID As List(Of String)) As GlobalDataTO
            'Public Function ResetSpecifiedPreliminaryHomes(ByVal pAnalyzerID As String, ByVal pRequiredHomeID As String) As GlobalDataTO
            Dim resultdata As New GlobalDataTO
            Try
                Dim myHomesDAO As New tadjPreliminaryHomesDAO
                'AG 01/10/2014 BA-1953
                'resultdata = myHomesDAO.ResetSpecifiedPreliminaryHomes(Nothing, pAnalyzerID, pRequiredHomeID)
                For Each item As String In pListRequiredHomeID
                    If Not resultdata.HasError Then
                        resultdata = myHomesDAO.ResetSpecifiedPreliminaryHomes(Nothing, pAnalyzerID, item)
                    Else
                        Exit For
                    End If
                Next
                'AG 01/10/2014 BA-1953

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.ResetSpecifiedPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return resultdata
        End Function


#End Region

        Public Function SendREAD_ADJUSTMENTS(ByVal pQueryMode As GlobalEnumerates.Ax00Adjustsments) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READADJ, True, Nothing, pQueryMode)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendREAD_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ' ''' <summary>
        ' ''' Creates the Script List for Homing operation
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks>Created by SG 10/03/11</remarks>
        'Protected Friend Function SendQueueForHOMING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myListFwScript As New List(Of FwScriptQueueItem)
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'get the pending Homes
        '        Dim myHomes As New tadjPreliminaryHomesDAO
        '        Dim myHomesDS As SRVPreliminaryHomesDS
        '        myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, MyClass.AnalyzerIdAttr, pAdjustment.ToString)

        '        If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then

        '            myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

        '            Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
        '                            (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
        '                            Where a.Done = False Select a).ToList

        '            For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
        '                Dim myFwScript As New FwScriptQueueItem
        '                myListFwScript.Add(myFwScript)
        '            Next


        '            With myFwScript1
        '                .FwScriptID = FwSCRIPTS_IDS.HOME_ALL_ARMS.ToString
        '                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                .EvaluateValue = 1
        '                .NextOnResultOK = Nothing
        '                .NextOnResultNG = Nothing
        '                .NextOnTimeOut = Nothing
        '                .NextOnError = Nothing
        '                .ParamList = Nothing
        '            End With


        '        End If

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
        '        myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendQueueForHOMING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary>
        ''' Request for Fw Information values
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 25/05/2011</remarks>
        Public Function SendPOLLFW(ByVal ID As POLL_IDs) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try


                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLFW, True, Nothing, ID)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendPOLLFW", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Function SendINFO_START() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                If myFwScriptDelegate.AnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                             True, _
                                                             Nothing, _
                                                             GlobalEnumerates.Ax00InfoInstructionModes.STR)
                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendINFO_START", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Function SendINFO_STOP() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                If myFwScriptDelegate.AnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                             True, _
                                                             Nothing, _
                                                             GlobalEnumerates.Ax00InfoInstructionModes.STP)
                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendINFO_STOP", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Request for Hw Information values
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 01/06/2011</remarks>
        Public Function SendPOLLHW(ByVal ID As POLL_IDs) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLHW, True, Nothing, ID)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendPOLLHW", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        '''' <summary>
        '''' Request for Fw Events enabling 'PDT to spec
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 10/06/2011</remarks>
        'Public Function SendENABLE_EVENTS() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENABLE_FW_EVENTS, True, Nothing, Nothing)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendENABLE_EVENTS", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Request for Fw Events disabling 'PDT to spec
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 10/06/2011</remarks>
        'Public Function SendDISABLE_EVENTS() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.DISABLE_FW_EVENTS, True, Nothing, Nothing)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendDISABLE_EVENTS", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        Public Overridable Function SendSOUND() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, True, Nothing, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendSOUND", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Overridable Function SendENDSOUND() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND, True, Nothing, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendENDSOUND", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pUTILCommand"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 04/10/2012</remarks>
        Public Overridable Function SendUTIL(ByVal pUTILCommand As UTILCommandTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.UTIL, True, Nothing, pUTILCommand)


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendUTIL", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

        'SGM 27/10/2011
#Region "Information Data Request"

        Public Function GetInformationDocument(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pAnalyzerModel As String, ByVal pAppPage As String, _
                                              Optional ByVal pLanguageID As String = "") As GlobalDataTO

            Dim myInfoDocsDS As New SRVInfoDocumentsDS
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myInfoDocsDAO As New tfmwInfoDocumentsDAO
                        resultData = myInfoDocsDAO.ReadDocumentPath(dbConnection, pAnalyzerModel, pAppPage, pLanguageID)

                        If (Not resultData.SetDatos Is Nothing) Then
                            myInfoDocsDS = CType(resultData.SetDatos, SRVInfoDocumentsDS)
                            If (myInfoDocsDS.srv_tfmwInfoDocuments.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.GetInformationDocument", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "FOR TESTING  FwScript Response Handling"

        ''' <summary>
        ''' Example for handling response for a specific Script 
        ''' This option is reserved for very special cases in which it is necessary to add 
        ''' a specific treatment after receiving some response
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>Created by SG 17/11/10</remarks>
        Private Sub onResponse(ByVal sender As Object, ByVal e As System.EventArgs) Handles myFwScriptDelegate.FwScriptResponseEvent
            Try
                Dim myCurrentQueueItem As FwScriptQueueItem = CType(sender, FwScriptQueueItem)

                Dim myNextQueueItem As FwScriptQueueItem = Nothing

                'manage needed operation
                Select Case myCurrentQueueItem.Response
                    Case RESPONSE_TYPES.OK
                        myNextQueueItem = myCurrentQueueItem.NextOnResultOK

                    Case RESPONSE_TYPES.NG
                        myNextQueueItem = myCurrentQueueItem.NextOnResultNG

                    Case RESPONSE_TYPES.TIMEOUT
                        myNextQueueItem = myCurrentQueueItem.NextOnTimeOut

                    Case RESPONSE_TYPES.EXCEPTION
                        myNextQueueItem = myCurrentQueueItem.NextOnError

                End Select

                If myNextQueueItem IsNot Nothing Then
                    Me.myFwScriptDelegate.StartFwScriptQueue(myNextQueueItem)
                Else
                    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                        myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                    End If
                    RaiseEvent ReceivedLastFwScriptEvent(myCurrentQueueItem.Response, myCurrentQueueItem.ResponseData)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.onResponse", EventLogEntryType.Error, False)
            End Try
        End Sub


#End Region

    End Class

End Namespace

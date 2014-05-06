Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class ChangeRotorDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate
            MyClass.CurrentOperation = OPERATIONS.NONE
            'MyClass.ReportCountTimeout = 0
        End Sub
        Public Sub New()
            MyBase.New() 'SGM 20/01/2012
        End Sub
#End Region

#Region "Enumerations"
        Private Enum OPERATIONS
            NONE
            NEW_ROTOR
            WASHING_STATION_UP
            WASHING_STATION_DOWN
        End Enum
#End Region

#Region "Declarations"
        Private CurrentOperation As OPERATIONS = OPERATIONS.NONE
        'Private RecommendationsList As New List(Of HISTORY_RECOMMENDATIONS)
        'Private ReportCountTimeout As Integer
#End Region

#Region "Attributes"
        Private AnalyzerIDAttr As String
        Private IsWashingStationUpAttr As Boolean = False
#End Region

#Region "Properties"
        Public Property AnalyzerId() As String
            Get
                Return MyClass.AnalyzerIDAttr
            End Get
            Set(ByVal value As String)
                MyClass.AnalyzerIDAttr = value
            End Set
        End Property

        Public Property IsWashingStationUp() As Boolean
            Get
                Return MyClass.IsWashingStationUpAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.IsWashingStationUpAttr = value
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
        ''' <remarks>Created by XBC 06/05/2011</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles Me.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics
                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    'If MyClass.ReportCountTimeout = 0 Then
                    '    MyClass.ReportCountTimeout += 1
                    '    ' registering the incidence in historical reports activity
                    '    MyClass.UpdateRecommendationsList(HISTORY_RECOMMENDATIONS.ERR_COMM)
                    'End If

                    Exit Sub
                End If

                Select Case CurrentOperation

                    Case OPERATIONS.WASHING_STATION_UP
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationUpAttr = True
                        End Select

                    Case OPERATIONS.WASHING_STATION_DOWN
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationUpAttr = False
                        End Select

                    Case OPERATIONS.NEW_ROTOR
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ''reset E:550 is informed flag
                                'myFwScriptDelegate.AnalyzerManager.IsServiceRotorMissingInformed = False

                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationUpAttr = False
                        End Select

                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ChangeRotorDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by XBC 05/12/2011</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.ScreenReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ChangeRotorDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"


        Public Function SendNEW_ROTOR() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try

                CurrentOperation = OPERATIONS.NEW_ROTOR
                
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NROTOR, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DemoModeDelegate.SendNEW_ROTOR", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Function SendWASH_STATION_CTRL(ByVal pAction As Ax00WashStationControlModes) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Select Case pAction
                    Case Ax00WashStationControlModes.UP
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_UP
                    Case Ax00WashStationControlModes.DOWN
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_DOWN
                End Select
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, pAction)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ChangeRotorDelegate.SendWASH_STATION_CTRL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function



#End Region

#Region "Private Methods"

        '''' <summary>
        '''' registering the incidence in historical reports activity
        '''' </summary>
        '''' <remarks>
        '''' Created by SGM 15/11/2012
        '''' </remarks>
        'Private Sub UpdateRecommendationsList(ByVal pRecommendationID As HISTORY_RECOMMENDATIONS)
        '    Try
        '        ' registering the incidence in historical reports activity

        '        'If MyClass.RecommendationsReport Is Nothing Then
        '        '    ReDim MyClass.RecommendationsReport(0)
        '        'Else
        '        '    ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
        '        'End If
        '        'MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = pRecommendationID

        '        If MyClass.RecommendationsList Is Nothing Then
        '            MyClass.RecommendationsList = New List(Of HISTORY_RECOMMENDATIONS)
        '        End If
        '        MyClass.RecommendationsList.Add(pRecommendationID)

        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ChangeRotorDelegate.UpdateRecommendations", EventLogEntryType.Error, False)
        '    End Try
        'End Sub

#End Region

    End Class


End Namespace

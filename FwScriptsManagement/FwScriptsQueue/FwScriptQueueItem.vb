
Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.CommunicationsSwFw

Namespace Biosystems.Ax00.FwScriptsManagement


    ''' <summary>
    ''' Object that define the element to be sent to the Analyzer
    ''' </summary>
    ''' <remarks>Created by SG 17/11/10</remarks>
    Public Class FwScriptQueueItem

#Region "Properties"

        'identifier of the Action Script
        Public Property FwScriptID() As String
            Get
                Return FwScriptIDAttr
            End Get
            Set(ByVal value As String)
                FwScriptIDAttr = value
            End Set
        End Property

        'overwrittable parameter value list
        Public Property ParamList() As List(Of String)
            Get
                Return ParamListAttr
            End Get
            Set(ByVal value As List(Of String))
                ParamListAttr = value
            End Set
        End Property

        'evaluating mode
        Public Property EvaluateType() As EVALUATE_TYPES
            Get
                Return EvaluateTypeAttr
            End Get
            Set(ByVal value As EVALUATE_TYPES)
                EvaluateTypeAttr = value
            End Set
        End Property

        'evaluating value for comparing to
        Public Property EvaluateValue() As Object
            Get
                Return EvaluateValueAttr
            End Get
            Set(ByVal value As Object)
                EvaluateValueAttr = value
            End Set
        End Property


        'execution flow
        'next script to send in case of timeout
        Public Property NextOnTimeOut() As FwScriptQueueItem
            Get
                Return NextOnTimeOutAttr
            End Get
            Set(ByVal value As FwScriptQueueItem)
                NextOnTimeOutAttr = value
            End Set
        End Property

        'next script to send in case of OK
        Public Property NextOnResultOK() As FwScriptQueueItem
            Get
                Return NextOnResultOKAttr
            End Get
            Set(ByVal value As FwScriptQueueItem)
                NextOnResultOKAttr = value
            End Set
        End Property

        'next script to send in case of NG
        Public Property NextOnResultNG() As FwScriptQueueItem
            Get
                Return NextOnResultNGAttr
            End Get
            Set(ByVal value As FwScriptQueueItem)
                NextOnResultNGAttr = value
            End Set
        End Property

        'next script to send in case of exception
        Public Property NextOnError() As FwScriptQueueItem
            Get
                Return NextOnErrorAttr
            End Get
            Set(ByVal value As FwScriptQueueItem)
                NextOnErrorAttr = value
            End Set
        End Property

        'handling enabling

        'enable handling of response FOR TESTING
        Public Property ResponseEventHandling() As Boolean
            Get
                Return ResponseEventHandlingAttr
            End Get
            Set(ByVal value As Boolean)
                ResponseEventHandlingAttr = value
            End Set
        End Property


        Public Property ErrorCode() As String
            Get
                Return ErrorCodeAttr
            End Get
            Set(ByVal value As String)
                ErrorCodeAttr = value
            End Set
        End Property

        Public Property ResponseData() As String
            Get
                Return ResponseDataAttr
            End Get
            Set(ByVal value As String)
                ResponseDataAttr = value
            End Set
        End Property


        Public Property Response() As RESPONSE_TYPES
            Get
                Return Me.ResponseAttr
            End Get
            Set(ByVal value As RESPONSE_TYPES)

                Me.ResponseAttr = value

                If Me.ResponseEventHandling Then
                    RaiseEvent ResponseEvent(Me, New System.EventArgs)
                End If

            End Set
        End Property

        Public Property TimeExpected() As Integer
            Get
                Return Me.TimeExpectedAttr
            End Get
            Set(ByVal value As Integer)
                Me.TimeExpectedAttr = value
            End Set
        End Property
#End Region

#Region "Events"
        Public Event ResponseEvent As EventHandler  'event of response FOR TESTING
#End Region

#Region "Attributes"
        Private FwScriptIDAttr As String                       'identifier of the Action Script

        Private ParamListAttr As List(Of String)            'overwrittable parameter value list

        Private EvaluateTypeAttr As EVALUATE_TYPES           'evaluating mode
        Private EvaluateValueAttr As Object                  'evaluating value for comparing to

        'execution flow
        Private NextOnTimeOutAttr As FwScriptQueueItem                 'next script to send in case of timeout
        Private NextOnResultOKAttr As FwScriptQueueItem                 'next script to send in case of result OK
        Private NextOnResultNGAttr As FwScriptQueueItem                 'next script to send in case of result NG
        Private NextOnErrorAttr As FwScriptQueueItem                'next script to send in case of exception

        'handling enabling
        Private ResponseEventHandlingAttr As Boolean               'enable handling of response


        Private ErrorCodeAttr As String
        Private ResponseAttr As RESPONSE_TYPES
        Private ResponseDataAttr As String

        ' XBC 04/05/2011
        Private TimeExpectedAttr As Integer
#End Region

#Region "Constructor"
        Public Sub New()

            ParamListAttr = New List(Of String)
            EvaluateTypeAttr = EVALUATE_TYPES.NONE
            EvaluateValueAttr = Nothing
            NextOnTimeOutAttr = Nothing
            NextOnResultOKAttr = Nothing
            NextOnResultNGAttr = Nothing
            NextOnErrorAttr = Nothing
            ResponseEventHandlingAttr = False
            ErrorCodeAttr = ""
            ResponseDataAttr = Nothing
            TimeExpectedAttr = 0
        End Sub

#End Region

    End Class



End Namespace
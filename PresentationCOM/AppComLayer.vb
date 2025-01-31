﻿Option Explicit On
Option Infer On

Imports System.Collections.Concurrent
Imports System.Windows.Forms
Imports Biosystems.Ax00.App.PresentationLayerListener
Imports Biosystems.Ax00.App.PresentationLayerListener.Requests

''' <summary>
''' This class is an implementation of the IPresentationLayerListener interface. This class can be used to attend requests from the application layer.
''' </summary>
Public Class AppComLayer
    Implements IPresentationLayerListener

    ''' <summary>
    ''' This method allows a request to be sent from any thread to the presentation layer
    ''' </summary>
    ''' <param name="request" >This is the request to be enqueued</param>
    ''' <remarks>This method is thread-safe and asynchronic</remarks>
    Public Sub QueueRequest(request As PresentationRequest) Implements IPresentationLayerListener.QueueRequest
        requestsQueue.Enqueue(request)
        _contextControl.BeginInvoke(Sub()
                                        DispatchQueue()
                                    End Sub)
    End Sub

    ''' <summary>
    ''' This method allows a request to be sent and processed from any thread to the presentation layer
    ''' </summary>
    ''' <param name="request" >This is the request to be sent and processed</param>
    ''' <remarks>This method is thread-safe</remarks>
    Public Sub InvokeSynchronizedRequest(request As PresentationRequest) Implements IPresentationLayerListener.InvokeSynchronizedRequest
        If _contextControl.InvokeRequired Then
            _contextControl.Invoke(Sub() InvokeSynchronizedRequest(request))
        Else
            Dispatch(request)
        End If
    End Sub

    Sub New(contextControl As Control)
        If contextControl.InvokeRequired Then
            contextControl.Invoke(Sub() _contextControl = New Control(""))
        Else
            _contextControl = New Control("")
        End If
    End Sub

#Region "Private"
    ''' <summary>
    ''' This is a concurrent queue that to synchroniously process requests
    ''' </summary>
    Private requestsQueue As New ConcurrentQueue(Of PresentationRequest)


    ''' <summary>
    ''' This is an internal control used to store thread execution context
    ''' </summary>
    Private _contextControl As Control 'System.Windows.Forms.Form

    Private Sub DispatchQueue()
        While requestsQueue.Count > 0
            Dim item As New PresentationRequest
            If requestsQueue.TryDequeue(item) Then
                Dispatch(item)
            End If
        End While
    End Sub

    Private Sub Dispatch(item As PresentationRequest)
        Select Case item.GetType
            Case GetType(Notify), GetType(YesNoQuestion), GetType(YesNoCancelQuestion)
                DispatchNotifyAndChildren(TryCast(item, Notify))

            Case Else


        End Select
    End Sub

    Private Sub DispatchNotifyAndChildren(item As Notify)
        Dim i2 = item
        Dim buttons As MsgBoxStyle
        Select Case item.GetType()
            Case GetType(Notify)
                buttons = MsgBoxStyle.Information
            Case GetType(YesNoQuestion)
                buttons = MsgBoxStyle.Question Or MsgBoxStyle.YesNo
            Case GetType(YesNoCancelQuestion)
                buttons = MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel
        End Select
        i2.Result = MsgBox(i2.Text, buttons, My.Application.Info.ProductName)
        If i2.OnAnswered IsNot Nothing Then i2.OnAnswered.Invoke(i2)

    End Sub

#End Region

End Class


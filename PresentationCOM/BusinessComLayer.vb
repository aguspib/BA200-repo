﻿Option Explicit On
Option Infer On

Imports System.Collections.Concurrent
Imports System.Windows.Forms
Imports Biosystems.Ax00.App.PresentationLayerListener
Imports Biosystems.Ax00.App.PresentationLayerListener.Requests

'Namespace Biosystems.Ax00.PresentationCOM
Public Class BusinessComLayer
    Implements IPresentationLayerListener

    Public Sub QueueRequest(request As PresentationRequest) Implements IPresentationLayerListener.QueueRequest
        requestsQueue.Enqueue(request)
        _managerForm.BeginInvoke(Sub()
                                     DispatchQueue()
                                 End Sub)
    End Sub

    Sub New(managerForm As Windows.Forms.Form)
        _managerForm = managerForm
    End Sub

#Region "Private"
    Dim requestsQueue As New ConcurrentQueue(Of PresentationRequest)
    Dim _managerForm As System.Windows.Forms.Form

    Sub DispatchQueue()
        While requestsQueue.Count > 0
            Dim item As New PresentationRequest
            If requestsQueue.TryDequeue(item) Then
                Dispatch(item)
            End If
        End While
    End Sub

    Sub Dispatch(item As PresentationRequest)
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

'End Namespace

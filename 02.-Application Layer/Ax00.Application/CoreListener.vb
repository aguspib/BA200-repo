Imports System.Collections.Concurrent
Imports System.Security.Permissions
Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.App.PresentationLayerListener.Requests

Public Class CoreListener
    Implements IAppLayerListener

    Public Sub QueueRequest(request As AppRequest) Implements IAppLayerListener.QueueRequest
        '_queue.Enqueue(request)
        Select Case request.GetType()
            Case GetType(AppRequest)
            Case GetType(AppYesNoQuestion)
                SendYesNo(DirectCast(request, AppYesNoQuestion))
        End Select

    End Sub

    Sub SendYesNo(request As AppYesNoQuestion)
        Dim item As New YesNoQuestion
        item.Text = request.Text
        item.OnAnswered =
            Sub(result As PresentationRequest)
                request.Result = item.Result
                If request.OnAnswered IsNot Nothing Then
                    request.OnAnswered.Invoke(request)
                End If
            End Sub

        AnalyzerController.PresentationLayerInterface.QueueRequest(item)
    End Sub
    'Private _queue As ConcurrentQueue(Of AppRequest)

End Class


Namespace Biosystems.Ax00.App.PresentationLayerListener
    Public Interface IPresentationLayerListener
        ''' <summary>
        ''' This method allows a request to be sent to the presentation layer
        ''' </summary>
        ''' <remarks></remarks>
        Sub QueueRequest(request As Requests.PresentationRequest)

        Sub InvokeSynchronizedRequest(request As Requests.PresentationRequest)

    End Interface

End Namespace
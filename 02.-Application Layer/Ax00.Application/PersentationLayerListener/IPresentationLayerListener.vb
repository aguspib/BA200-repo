
Namespace Biosystems.Ax00.App.PresentationLayerListener
    Public Interface IPresentationLayerListener
        ''' <summary>
        ''' This method allows a request to be sent from any thread to the presentation layer
        ''' </summary>
        ''' <remarks></remarks>
        Sub QueueRequest(request As Requests.PresentationRequest)

        ''' <summary>
        ''' This method allows a request to be sent and processed from any thread to the presentation layer.
        ''' The request will be immediatly attended, synchronously from the presentation layer.
        ''' </summary>
        ''' <param name="request"></param>
        ''' <remarks></remarks>
        Sub InvokeSynchronizedRequest(request As Requests.PresentationRequest)

    End Interface

End Namespace
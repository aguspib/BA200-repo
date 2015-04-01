Namespace Biosystems.Ax00.App.PresentationLayerListener.Requests

    Public Class Notify
        Inherits PresentationRequest
        Public Property Result As MsgBoxResult
        Public Property Text As String
        Public Property OnAnswered As Action(Of Notify)
    End Class

End Namespace

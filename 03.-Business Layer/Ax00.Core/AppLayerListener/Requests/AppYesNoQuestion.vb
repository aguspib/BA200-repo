Public Class AppNotify
    Inherits AppRequest
    Public Property Result As MsgBoxResult
    Public Property Text As String
    Public Property OnAnswered As Action(Of AppNotify)
End Class

Public Class AppYesNoQuestion
    Inherits AppNotify
End Class
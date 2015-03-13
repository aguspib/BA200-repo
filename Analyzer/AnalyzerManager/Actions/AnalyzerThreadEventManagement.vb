
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Entities
    Partial Public Class AnalyzerManager

        Public Sub ConnectionDoneReceptionEvent() Implements IAnalyzerManager.ConnectionDoneReceptionEvent
            RaiseEvent ReceptionEvent(CInt(GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE).ToString, True, myUI_RefreshEvent, myUI_RefreshDS, True)
        End Sub

        Public Sub ActionToSendEvent(Instruction As String) Implements IAnalyzerManager.ActionToSendEvent
            RaiseEvent SendEvent(Instruction)            
        End Sub
        Public Sub RunWellBaseLineWorker() Implements IAnalyzerManager.RunWellBaseLineWorker
            wellBaseLineWorker.RunWorkerAsync(bufferANSPHRReceived(0))
        End Sub


    End Class
End Namespace

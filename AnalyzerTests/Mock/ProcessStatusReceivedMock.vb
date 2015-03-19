Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.Tests.Mock
    Public Class ProcessStatusReceivedMock
        Inherits ProcessStatusReceived

        Public Sub New(ByRef analyzerMan As IAnalyzerManager)
            MyBase.New(analyzerMan)
        End Sub

    End Class
End Namespace

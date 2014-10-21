Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Public Class BA200AnalyzerEntity
        Inherits AnalyzerEntity

        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity, iseAnalyzer As IISEAnalyzerEntity)
            MyBase.New(assemblyName, analyzerModel, baseLine, iseAnalyzer)
        End Sub

    End Class

End Namespace

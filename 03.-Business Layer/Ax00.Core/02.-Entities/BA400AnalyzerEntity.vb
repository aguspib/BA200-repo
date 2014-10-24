Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Public Class BA400AnalyzerEntity
        Inherits AnalyzerEntity

        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            MyBase.New(assemblyName, analyzerModel, baseLine)
        End Sub

    End Class

End Namespace

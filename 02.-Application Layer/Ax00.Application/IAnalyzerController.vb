Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.App

    Public Interface IAnalyzerController

#Region "Properties"

        Property Analyzer As IAnalyzerEntity

#End Region

#Region "Public Methods"

        Function CreateAnalyzer(model As AnalyzerModelEnum, assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerEntity

#End Region

    End Interface

End Namespace

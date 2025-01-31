﻿Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.App

    Public Interface IAnalyzerFactory

        Function CreateAnalyzer(assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerManager

    End Interface

End Namespace


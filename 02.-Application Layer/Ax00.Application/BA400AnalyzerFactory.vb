Imports Biosystems.Ax00.Core
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.App

    Public Class BA400AnalyzerFactory
        Inherits AnalyzerFactory

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="assemblyName"></param>
        ''' <param name="analyzerModel"></param>
        ''' <param name="startingApplication"></param>
        ''' <param name="workSessionIDAttribute"></param>
        ''' <param name="analyzerIDAttribute"></param>
        ''' <param name="fwVersionAttribute"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function CreateAnalyzer(assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerManager

            Dim analyzer As IAnalyzerManager
            Dim baseLine As IBaseLineEntity
            Dim iseAnalyzer As ISEManager

            baseLine = New StaticBaseLineEntity()
            analyzer = New BA400AnalyzerEntity(assemblyName, analyzerModel, baseLine) With {.StartingApplication = startingApplication, _
                                                                            .ActiveWorkSession = workSessionIDAttribute, _
                                                                            .ActiveAnalyzer = analyzerIDAttribute, _
                                                                            .ActiveFwVersion = fwVersionAttribute}
            If analyzerIDAttribute = "" Then
                analyzer.ActiveAnalyzer = analyzer.GenericDefaultAnalyzer()
            End If

            If analyzer.Model = "" Then
                analyzer.Model = A400
            End If

                iseAnalyzer = New ISEManager(analyzer, analyzerIDAttribute, analyzerModel, False)
                analyzer.ISEAnalyzer = iseAnalyzer

                LoadAnalyzerConfiguration(analyzer)

                baseLine.BaseLineTypeForWellReject = analyzer.BaseLineTypeForWellReject 'AG 11/11/2014 BA-2065 - Inform the base line type for well rejection for this analyzer

                Return analyzer

        End Function

#End Region

    End Class

End Namespace


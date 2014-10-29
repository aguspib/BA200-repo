Imports Biosystems.Ax00.Core
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Calculations

Namespace Biosystems.Ax00.App

    Public Class BA400AnalyzerFactory
        Implements IAnalyzerFactory


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
        Function CreateAnalyzer(assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerEntity Implements IAnalyzerFactory.CreateAnalyzer

            Dim analyzer As IAnalyzerEntity
            Dim baseLine As IBaseLineEntity
            Dim iseAnalyzer As ISEAnalyzerEntity

            baseLine = New StaticBaseLineEntity()
            analyzer = New BA400AnalyzerEntity(assemblyName, analyzerModel, baseLine) With {.StartingApplication = startingApplication, _
                                                                            .ActiveWorkSession = workSessionIDAttribute, _
                                                                            .ActiveAnalyzer = analyzerIDAttribute, _
                                                                            .ActiveFwVersion = fwVersionAttribute}

            iseAnalyzer = New ISEAnalyzerEntity(analyzer, analyzerIDAttribute, analyzerModel, False)
            analyzer.ISEAnalyzer = iseAnalyzer

            'Delegates
            analyzer.Calculations = New CalculationsBA400Delegate()

            Return analyzer

        End Function

#End Region

    End Class

End Namespace


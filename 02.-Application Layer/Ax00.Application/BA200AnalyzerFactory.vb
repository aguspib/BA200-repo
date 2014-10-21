﻿Imports Biosystems.Ax00.Core
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities

Namespace Biosystems.Ax00.Application

    Public Class BA200AnalyzerFactory
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
        Public Function CreateAnalyzer(assemblyName As String, analyzerModel As String, startingApplication As Boolean, workSessionIDAttribute As String, analyzerIDAttribute As String, fwVersionAttribute As String) As IAnalyzerEntity Implements IAnalyzerFactory.CreateAnalyzer

            Dim analyzer As IAnalyzerEntity
            Dim baseLine As IBaseLineEntity
            Dim iseAnalyzer As ISEAnalyzerEntity

            baseLine = New DynamicBaseLineEntity()
            iseAnalyzer = New ISEAnalyzerEntity()

            analyzer = New BA200AnalyzerEntity(assemblyName, analyzerModel, baseLine, iseAnalyzer) With {.StartingApplication = startingApplication, _
                                                                            .ActiveWorkSession = workSessionIDAttribute, _
                                                                            .ActiveAnalyzer = analyzerIDAttribute, _
                                                                            .ActiveFwVersion = fwVersionAttribute}
            Return analyzer

        End Function

#End Region

    End Class

End Namespace


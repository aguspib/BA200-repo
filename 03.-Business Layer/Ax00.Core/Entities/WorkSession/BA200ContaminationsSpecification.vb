Imports System.Collections.Concurrent
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class BA200ContaminationsSpecification
        Inherits Ax00ContaminationsSpecification
        Implements IAnalyzerContaminationsSpecification

        Sub New(analyzer As IAnalyzerManager)
            MyBase.New(analyzer)

        End Sub

        Public Overrides Function CreateDispensing() As IDispensing
            Return New BA200Dispensing
        End Function


        Public Overrides Function GetAnalysisModeForReagent(reagentID As Integer) As AnalysisMode

            <ThreadStatic> Static testReagentsDataDS As TestReagentsDS

            Static catchedReagents As New ConcurrentDictionary(Of Integer, AnalysisMode)

            Dim byRefValue As AnalysisMode = AnalysisMode.MonoReactive 'Dummy value to be able to pass the var as reference to the TrygetValue
            If catchedReagents.TryGetValue(reagentID, byRefValue) = True Then Return byRefValue

            Try
                If testReagentsDataDS Is Nothing Then
                    testReagentsDataDS = GetAllReagents()
                End If

                Dim result = (From a In testReagentsDataDS.tparTestReagents
                              Where a.ReagentID = reagentID Select a.ReagentsNumber).First

                Dim mode = CType(result, AnalysisMode)
                catchedReagents.TryAdd(reagentID, mode)
                Return mode

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
                Throw
            End Try


        End Function

        Public Overrides Function AreAnalysisModesCompatible(current As AnalysisMode, expected As AnalysisMode) As Boolean
            If (expected = current OrElse expected = AnalysisMode.MonoReactive) Then
                Return True
            Else
                Return False
            End If

        End Function


        Private Function GetAllReagents() As TestReagentsDS

            Dim TestReagentsDAO As New tparTestReagentsDAO()
            Static testReagentsDataDS As TestReagentsDS
            Dim resultData As TypedGlobalDataTo(Of TestReagentsDS)

            If testReagentsDataDS Is Nothing Then
                resultData = TestReagentsDAO.GetAllReagents(Nothing)

                If Not resultData.HasError Then
                    testReagentsDataDS = resultData.SetDatos
                End If
            End If

            Return testReagentsDataDS
        End Function

    End Class
End Namespace

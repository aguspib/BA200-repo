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
        Implements IAnalyzerContaminationsSpecification





        Private myThread As Threading.Thread
        Sub New()

            AdditionalPredilutionSteps = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.PREDILUTION_CYCLES, AnalyzerModel).SetDatos

            Dim contaminationPersitance = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

            ContaminationsContextRange = New Range(Of Integer)(-contaminationPersitance, AdditionalPredilutionSteps + contaminationPersitance - 1)

            DispensesPerStep = 2    'The analyzer has R1 and R2

            AddHandler LinkLayer.ActivateProtocol, AddressOf Me.RetrieveInstructions

            myThread = Threading.Thread.CurrentThread

        End Sub

        Private Sub RetrieveInstructions(eventKind As GlobalEnumerates.AppLayerEventList, ByRef data As String)
            'We want to prevent this event listener from modifing data parameter that is passed ByRef, so we're adding a delegate passed ByValue.
            ProcessInstructionReceived(eventKind, data)
        End Sub
        Private Sub ProcessInstructionReceived(eventKind As GlobalEnumerates.AppLayerEventList, ByVal data As String)
            'If Threading.Thread.CurrentThread IsNot myThread Then
            '    Debug.WriteLine("Analyzer message fr om foreign thread: " & data)
            'Else
            '    Debug.WriteLine("Analyzer message from main thread: " & data)
            'End If
        End Sub

        Public Property ContaminationsContextRange As Range(Of Integer) Implements IAnalyzerContaminationsSpecification.ContaminationsContextRange

        Public Property DispensesPerStep As Integer Implements IAnalyzerContaminationsSpecification.DispensesPerStep

        Public Function CreateDispensing() As IDispensing Implements IAnalyzerContaminationsSpecification.CreateDispensing
            Return New BA200Dispensing
        End Function

        Public Property AdditionalPredilutionSteps As Integer Implements IAnalyzerContaminationsSpecification.AdditionalPredilutionSteps

        Public Function GetAnalysisModeForReagent(reagentID As Integer) As AnalysisMode Implements IAnalyzerContaminationsSpecification.GetAnalysisModeForReagent

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

        Public Function AreAnalysisModesCompatible(current As AnalysisMode, expected As AnalysisMode) As Boolean Implements IAnalyzerContaminationsSpecification.AreAnalysisModesCompatible
            If (expected = current OrElse expected = AnalysisMode.MonoReactive) Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Function RequiredAnalysisModeBetweenReactions(contaminator As AnalysisMode, contamined As AnalysisMode) As AnalysisMode Implements IAnalyzerContaminationsSpecification.RequiredAnalysisModeBetweenReactions
            If contaminator = AnalysisMode.BiReactive AndAlso contamined = AnalysisMode.BiReactive Then
                Return AnalysisMode.BiReactive
            Else
                Return AnalysisMode.MonoReactive
            End If
        End Function

        Public ReadOnly Property AnalyzerModel As String Implements IAnalyzerContaminationsSpecification.AnalyzerModel
            Get
                Return Enums.AnalyzerModelEnum.A200.ToString
            End Get
        End Property

#Region "Private members"
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

        Dim _currentContext As ContaminationsContext
#End Region

        Public Sub FillContextFromAnayzerData(instruction As IEnumerable(Of InstructionParameterTO)) Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData
            Dim analyzerFrame = New LAx00Frame(instruction)
            If analyzerFrame("R") = "1" Then
                Dim context = New ContaminationsContext(Me)
                context.FillContentsFromAnalyzer(analyzerFrame)
                _currentContext = context
                Debug.WriteLine("Context filled in running! ")
            End If
        End Sub



        Public ReadOnly Property CurrentRunningContext As IContaminationsContext Implements IAnalyzerContaminationsSpecification.CurrentRunningContext
            Get
                Return _currentContext
            End Get
        End Property

        Public ReadOnly Property HighContaminationPersistence As Integer Implements IAnalyzerContaminationsSpecification.HighContaminationPersistence
            Get
                <ThreadStatic> Static highContaminationPersitance As Integer = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos
                Return highContaminationPersitance
            End Get
        End Property

        Public Sub FillContextFromAnayzerData1(instruction As String) Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData
            Dim analyzerFrame = New LAx00Frame()
            Try
                analyzerFrame.ParseRawData(instruction)
                If analyzerFrame.KeysCollection.Contains("STATUS") AndAlso analyzerFrame("R") = "1" Then
                    Dim context = New ContaminationsContext(Me)
                    context.FillContentsFromAnalyzer(analyzerFrame)
                    _currentContext = context
                    Debug.WriteLine("Context filled in running! " & Mid(instruction, InStr(instruction, "R2B2:")))
                End If
            Catch ex As Exception
                Debug.WriteLine("EXCEPTION FILLING CONTEXT " & ex.Message)
                GlobalBase.CreateLogActivity(ex)
            End Try

        End Sub
    End Class
End Namespace

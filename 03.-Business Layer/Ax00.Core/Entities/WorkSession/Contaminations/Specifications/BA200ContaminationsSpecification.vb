
#Region "Imports"
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports System.Collections.Concurrent
Imports System.Windows.Forms
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing
Imports Biosystems.Ax00.Data

#End Region

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications
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

        Public Sub FillContextFromAnayzerData(ByVal instruction As String) 'Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData
            MyBase.FillContextFromAnayzerData(instruction)
            'Handle bireactive long lasting memory
            HandleBireactives()
        End Sub


        Private lastBireactives As LinkedList(Of IDispensing)

        Private Sub HandleBireactives()
            If lastBireactives Is Nothing Then
                LoadLastBireactivesFromDB()
            End If

            ' ReSharper disable once InconsistentNaming
            Const R1 As Integer = 1
            Const BEFOREONE As Integer = -1

            Dim stepToCheck = currentContext.Steps(BEFOREONE)
            Dim dispenseToCheck As IDispensing = Nothing
            Dim serializationRequired As Boolean = False

            If stepToCheck IsNot Nothing AndAlso stepToCheck.Dispensing(R1) IsNot Nothing Then
                dispenseToCheck = stepToCheck.Dispensing(R1)
            End If

            If dispenseToCheck IsNot Nothing Then
                Select Case dispenseToCheck.KindOfLiquid
                    Case KindOfDispensedLiquid.Washing
                        lastBireactives.AddLast(dispenseToCheck)
                        serializationRequired = True
                    Case KindOfDispensedLiquid.Reagent
                        If GetAnalysisModeForReagent(dispenseToCheck.R1ReagentID) = AnalysisMode.BiReactive Then
                            lastBireactives.AddLast(dispenseToCheck)
                            serializationRequired = True
                        End If
                End Select
            End If
            While lastBireactives.Count > HighContaminationPersistence
                lastBireactives.RemoveFirst()
                serializationRequired = True
            End While

            If serializationRequired Then
                SerializeLatestBireactives()
            End If
            'CurrentRunningContext.ActionRequiredForDispensing()
        End Sub

        Private Sub LoadLastBireactivesFromDB()
            Dim dao = New vWSExecutionsDAO
            Dim serializedData = dao.GetBireactivesContext()
            lastBireactives = New LinkedList(Of IDispensing)
            For Each R In serializedData
                Dim disp = CreateDispensing()
                disp.ExecutionID = R.ExecutionID
                disp.KindOfLiquid = CType(R.KindOfLiquid, KindOfDispensedLiquid)
                Select Case disp.KindOfLiquid
                    Case KindOfDispensedLiquid.Reagent
                        disp.R1ReagentID = R.R1ReagentID
                    Case KindOfDispensedLiquid.Washing
                        disp.WashingID = R.WashingID
                End Select
                lastBireactives.AddLast(disp)
            Next
        End Sub
        Private Sub SerializeLatestBireactives()
            If lastBireactives Is Nothing Then Return

            Dim dao = New vWSExecutionsDAO

            Dim table As New vWSExecutionsDS.twksWSBbireactiveContextDataTable
            For Each disp In lastBireactives
                table.AddtwksWSBbireactiveContextRow(
                    AnalyzerManager.GetCurrentAnalyzerManager.ActiveWorkSession,
                    AnalyzerManager.GetCurrentAnalyzerManager.ActiveAnalyzer,
                    disp.ExecutionID,
                    CByte(disp.KindOfLiquid),
                    CShort(disp.R1ReagentID),
                    disp.WashingID)
            Next

            dao.ClearBireactivesContext()
            dao.SaveBireactivesContext(table)

        End Sub

    End Class
End Namespace


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

                Dim resultList = (From a In testReagentsDataDS.tparTestReagents
                              Where a.ReagentID = reagentID Select a.ReagentsNumber)
                Dim result As AnalysisMode
                If resultList IsNot Nothing AndAlso resultList.Any Then
                    result = DirectCast(resultList.First(), AnalysisMode)
                Else
                    result = AnalysisMode.MonoReactive
                End If

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


        Private lastestBireactives As LinkedList(Of IDispensing)

        Private Sub HandleBireactives()
            Debug.WriteLine("HANDLING BIREACTIVES!")
            If currentContext Is Nothing Then Return
            If lastestBireactives Is Nothing Then
                LoadLastestBireactivesFromDB()
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
                        If lastestBireactives.Any AndAlso lastestBireactives.Last.Value.KindOfLiquid = KindOfDispensedLiquid.Washing AndAlso lastestBireactives.Last.Value.WashingID = dispenseToCheck.WashingID Then
                            'This dispense has been already added to the latest bireactvies list
                        Else
                            lastestBireactives.AddLast(dispenseToCheck)
                            serializationRequired = True
                        End If
                    Case KindOfDispensedLiquid.Reagent
                        If GetAnalysisModeForReagent(dispenseToCheck.R1ReagentID) = AnalysisMode.BiReactive Then

                            If lastestBireactives.Any AndAlso lastestBireactives.Last.Value.KindOfLiquid = KindOfDispensedLiquid.Reagent AndAlso lastestBireactives.Last.Value.ExecutionID = dispenseToCheck.ExecutionID Then
                                'This dispense has been already added to the latest bireactvies list
                            Else
                                lastestBireactives.AddLast(dispenseToCheck)
                                serializationRequired = True
                            End If
                        End If
                End Select
            End If
            While lastestBireactives.Count > HighContaminationPersistence
                lastestBireactives.RemoveFirst()
                serializationRequired = True
            End While

            If serializationRequired Then
                SerializeLatestBireactives()
            End If


        End Sub

        Private Sub LoadLastestBireactivesFromDB()
            Dim dao = New vWSExecutionsDAO
            Dim serializedData = dao.GetBireactivesContext()
            lastestBireactives = New LinkedList(Of IDispensing)
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
                lastestBireactives.AddLast(disp)
            Next
        End Sub
        Private Sub SerializeLatestBireactives()
            If lastestBireactives Is Nothing Then Return

            Dim dao = New vWSExecutionsDAO

            Dim table As New vWSExecutionsDS.twksWSBbireactiveContextDataTable
            For Each disp In lastestBireactives
                table.AddtwksWSBbireactiveContextRow(
                    AnalyzerManager.GetCurrentAnalyzerManager.ActiveWorkSession,
                    AnalyzerManager.GetCurrentAnalyzerManager.ActiveAnalyzer,
                    disp.ExecutionID,
                    CByte(disp.KindOfLiquid),
                    CShort(disp.R1ReagentID),
                    disp.WashingID)
            Next

            Dim serializationOfData As New Task(Sub()
                                                    dao.ClearBireactivesContext()
                                                    dao.SaveBireactivesContext(table)
                                                End Sub)
            serializationOfData.Start()

        End Sub

        Public Overrides Function GetActionRequiredInRunning(dispensing As IDispensing) As IActionRequiredForDispensing
            Dim result = currentContext.ActionRequiredForDispensing(dispensing)

            If (result.Action = IContaminationsAction.RequiredAction.GoAhead AndAlso
                lastestBireactives IsNot Nothing AndAlso
                lastestBireactives.Any) Then
                Dim auxContext As Context.Context = GetHistoricalBireactivesContext()
                result = auxContext.ActionRequiredForDispensing(dispensing)
            End If

            Return result

        End Function

        Private Function GetHistoricalBireactivesContext() As Context.Context

            Dim auxContext = New Context.Context(Me)
            auxContext.FillEmptyContextSteps()
            If lastestBireactives IsNot Nothing AndAlso lastestBireactives.Any Then
                If lastestBireactives.Last IsNot Nothing Then auxContext.Steps(-1)(1) = lastestBireactives.Last.Value
                If lastestBireactives.First IsNot Nothing Then auxContext.Steps(-2)(1) = lastestBireactives.First.Value
            End If
            Return auxContext
        End Function

        Protected Overrides Sub OnContextRequestProcessed()
            HandleBireactives()
            If lastestBireactives IsNot Nothing AndAlso lastestBireactives.Any Then
                Debug.WriteLine("Historic bireactives context:")
                Debug.WriteLine(GetHistoricalBireactivesContext.ToString)
            End If
        End Sub


    End Class
End Namespace

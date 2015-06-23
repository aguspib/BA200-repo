Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Core.Services.Enums
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports NUnit.Framework
Imports Telerik.JustMock

Namespace Biosystems.Ax00.Core.Services.Tests

    <TestFixture()> Public Class BaseLineServiceTests

        <Test()>
        Sub IntegrationTestHappyPath()
            CreateScenario()

            Dim result = BLService.StartService

            'result needs to be true if the service has been started
            NUnit.Framework.Assert.IsTrue(result)

            'service status needs to be set to running if it's listening and alive
            NUnit.Framework.Assert.AreEqual(BLService.Status, ServiceStatusEnum.Running)

            'We throw a Status instruction event, so the service updates its flags
            Analyzer.ThrowStatusEvent()


            'When the service gets the first Status alarm, it needs to update its status to check previous alarms
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.CheckPreviousAlarms)
            'No alarms in this test

            'When we have checked the previous alarms, the service needs to be on status "Conditional washing"
            Analyzer.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.ConditioningWashing)

            'We're in the middle of a washing process. it should still return a Washing status, as it hasn't finished!
            Analyzer.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.ConditioningWashing)

            'We finish the washing process
            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "END"

            Analyzer.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.StaticBaseLine)

            Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END"

            Analyzer.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.DynamicBaseLineFill)

            _mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "END"

            Analyzer.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.DynamicBaseLineFill)


            Analyzer.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.DynamicBaseLineFill)

        End Sub

        Shared BLService As BaseLineService

        Private Sub CreateScenario()


            StatusParameters.IsActive = False

            'Initialize Base line service:
            BLService = New BaseLineService(Analyzer, Mock.Create(Of IAnalyzerManagerFlagsDelegate))
            BLService.ReactRotorCRUD = Mock.Create(Of IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS))()

            'Initialize and set Analyzer settings:
            Analyzer.Connected = True
            Analyzer.ThrowStatusEvent()

            'Initialize injected dependencies:
            Dim RotorStatusSerializer = Mock.Create(Of IReactionsRotorStatusSerializer)()
            Dim BLDeleter = Mock.Create(Of IWSBLinesDelegateValuesDeleter)()
            BLService.ReactRotorStatusSerializer = RotorStatusSerializer
            BLService.BaselineValuesDeleter = BLDeleter

            Mock.Arrange(Function() RotorStatusSerializer.ChangeRotorPerformed(Arg.IsAny(Of SqlConnection), Arg.AnyString)).
                Returns(New GlobalDataTO With {.HasError = False})

            'BaselineValuesDeleter
            Mock.Arrange(Function() BLDeleter.DeleteBLinesValues(Arg.IsAny(Of SqlConnection), Arg.AnyString, Arg.AnyString, Arg.AnyString, Arg.AnyString)).
                Returns(New GlobalDataTO With {.HasError = False})

        End Sub

        Private ReadOnly Property Analyzer As ISuperAnalyzer
            Get
                Static _analyzer As ISuperAnalyzer = Nothing
                If _analyzer Is Nothing Then
                    _analyzer = Mock.Create(Of ISuperAnalyzer)()

                    'We add a method to send Status events:
                    Mock.Arrange(Sub() _analyzer.ThrowStatusEvent()).Raises(Sub() AddHandler _analyzer.ReceivedStatusInformationEventHandler, Nothing)

                    'We add a method to make a mock of the UpdateSessionFlags. We'll use an internal dictionary
                    Mock.Arrange(Sub() _analyzer.UpdateSessionFlags(Arg.AnyObject, Arg.IsAny(Of GlobalEnumerates.AnalyzerManagerFlags), Arg.AnyString)).DoInstead(
                        Sub(ByRef pFlagsDS As AnalyzerManagerFlagsDS, pFlagCode As GlobalEnumerates.AnalyzerManagerFlags, pNewValue As String)
                            UpdateSessionFlagsMock(pFlagsDS, pFlagCode, pNewValue)
                        End Sub
                    )

                    'We add a method to make a mock of the ManageAnalyzer GOD method. 
                    Mock.Arrange(Function() _analyzer.ManageAnalyzer(
                                     Arg.IsAny(Of GlobalEnumerates.AnalyzerManagerSwActionList),
                                     Arg.AnyBool, Arg.IsAny(Of List(Of InstructionParameterTO)),
                                     Arg.AnyObject, Arg.AnyString, Arg.IsAny(Of List(Of String)))) _
                             .Returns(
                                Function(pAction As GlobalEnumerates.AnalyzerManagerSwActionList, pSendingEvent As Boolean,
                                        pInstructionReceived As List(Of InstructionParameterTO), pSwAdditionalParameters As Object,
                                        pFwScriptId As String, pServiceParams As List(Of String)) As GlobalDataTO

                                    Return ManageAnalyzerMock(pAction, pSendingEvent, pInstructionReceived, pSwAdditionalParameters, pFwScriptId, pServiceParams)

                                End Function)


                    'This is the SessionFlag property getter mock:
                    Mock.Arrange(Function() _analyzer.SessionFlag(Arg.IsAny(Of GlobalEnumerates.AnalyzerManagerFlags))).Returns(
                        Function(param As GlobalEnumerates.AnalyzerManagerFlags) As String
                            If _mySessionFlags.ContainsKey(param.ToString) Then
                                Return _mySessionFlags(param.ToString)
                            Else
                                Return ""
                            End If
                        End Function
                        )


                    'We add a method to make a mock of the ManageAnalyzer GOD method. 
                    Mock.Arrange(Function() _analyzer.BaseLineTypeForCalculations()).Returns(GlobalEnumerates.BaseLineType.DYNAMIC)



                    'This is the SessionFlag property setter mock:
                    Mock.Arrange(Sub() _analyzer.SessionFlag(Arg.IsAny(Of GlobalEnumerates.AnalyzerManagerFlags)) = Arg.Matches(Of String)(Function(param As String) True)).DoInstead(
                        Sub(param As GlobalEnumerates.AnalyzerManagerFlags, value As String)
                            _mySessionFlags(param.ToString) = value
                        End Sub)




                End If
                Return _analyzer
            End Get
        End Property

        Private _mySessionFlags As New Dictionary(Of String, String)
        Const AnalyzerIDAttribute As String = "BA350-123456789000"

        Private Sub UpdateSessionFlagsMock(ByVal pFlagsDS As AnalyzerManagerFlagsDS, ByVal pFlagCode As GlobalEnumerates.AnalyzerManagerFlags, ByVal pNewValue As String)
            'Update dictionary flags variables
            _mySessionFlags(pFlagCode.ToString) = pNewValue

            'Add row into Dataset to Update
            Dim flagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
            flagRow = pFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
            With flagRow
                .BeginEdit()
                .AnalyzerID = AnalyzerIDAttribute
                .FlagID = pFlagCode.ToString
                If pNewValue <> "" Then
                    .Value = pNewValue
                Else
                    .SetValueNull()
                End If
                .EndEdit()
            End With
            pFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(flagRow)
            pFlagsDS.AcceptChanges()


        End Sub

        Function ManageAnalyzerMock(pAction As GlobalEnumerates.AnalyzerManagerSwActionList, pSendingEvent As Boolean, pInstructionReceived As List(Of InstructionParameterTO), pSwAdditionalParameters As Object, pFwScriptId As String, pServiceParams As List(Of String)) As GlobalDataTO
            Select Case pAction
                Case GlobalEnumerates.AnalyzerManagerSwActionList.WASH
                    _mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "END"
                Case GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT
                    _mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "END"
                    Analyzer.CurrentInstructionAction = GlobalEnumerates.InstructionActions.None
            End Select
            Return New GlobalDataTO With {.HasError = False}
        End Function

    End Class


    Public Interface ISuperAnalyzer
        Inherits IAnalyzerManager
        Sub ThrowStatusEvent()

    End Interface

End Namespace



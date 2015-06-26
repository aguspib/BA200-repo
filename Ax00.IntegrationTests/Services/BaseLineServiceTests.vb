Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Core.Services.Enums
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports NUnit.Framework
Imports Telerik.JustMock
Imports Telerik.JustMock.Helpers
Imports System.Data
Imports System.Globalization
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Types.AnalyzerSettingsDS

Namespace Biosystems.Ax00.Core.Services.Tests

    <TestFixture()> Public Class BaseLineServiceTests

#Region "Attributes"
        Private BLService As BaseLineService
        Private _mySessionFlags As New Dictionary(Of String, String)
        Const AnalyzerIDAttribute As String = "VA001-MOCKMODEL"
#End Region


        <Test()>
        Public Sub IntegrationTestHappyPath()
            Debug.WriteLine("Creating test scenario")
            CreateScenario()
            NUnit.Framework.Assert.AreEqual(BLService.Status, ServiceStatusEnum.NotYetStarted, "Service provided wrong start status. It should be NotYetStarted")

            Debug.WriteLine("Starting baseline service")
            Dim result = BLService.StartService
            NUnit.Framework.Assert.IsTrue(result, "Service was unable to start, or provided wrong start result. Expected TRUE")

            NUnit.Framework.Assert.AreEqual(BLService.Status, ServiceStatusEnum.Running, "Service status should be Running")

            'result needs to be true if the service has been started

            'We throw a Status instruction event, so the service updates its flags
            AnalyzerMock.ThrowStatusEvent()


            'When the service gets the first Status alarm, it needs to update its status to check previous alarms
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.CheckPreviousAlarms, "Service should be checking previous 551 and 560 alarms")
            'No alarms in this test

            'When we have checked the previous alarms, the service needs to be on status "Conditional washing"
            AnalyzerMock.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.ConditioningWashing, "Service should be doing a conditional washing")

            'We're in the middle of a washing process. it should still return a Washing status, as it hasn't finished!
            AnalyzerMock.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.ConditioningWashing, "Service should be doing a conditional washing")

            'We finish the washing process
            AnalyzerMock.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "END"

            'We perform the Static Base Line Step:
            AnalyzerMock.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.StaticBaseLine, "Service should be doing the Static baseline process")
            AnalyzerMock.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END"

            'We perform the Dynamic Base Line Fill Step:
            AnalyzerMock.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.DynamicBaseLineFill, "Service should be filling the rotor for the dynamic baseline")

            'We mmake our Analyzer manager mock finish filling the mocked rotor:
            AnalyzerMock.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "END"

            'We do the dynamic baseline read
            AnalyzerMock.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.DynamicBaseLineRead, "Service should be reading the rotor as part of the Dynamic Baseline")
            AnalyzerMock.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "END"

            AnalyzerMock.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.DynamicBaseLineEmpty, "Service should be emptying the rotor as part of the Dynamic Baseline")
            AnalyzerMock.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "END"

            AnalyzerMock.ThrowStatusEvent()
            NUnit.Framework.Assert.AreEqual(BLService.CurrentStep, BaseLineStepsEnum.Finalize, "Service should be finalized at this step")

            Const DateTimePrecission As Integer = 10 'in seconds
            'newBLDate.ToString(Globalization.CultureInfo.InvariantCulture)
            Dim setting = (From A In AnalyzerMock.AnalyzerSettings.tcfgAnalyzerSettings Where A.AnalyzerID = AnalyzerMock.ActiveAnalyzer And A.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.BL_DATETIME.ToString()).First()

            Dim mydate As DateTime = Now
            If DateTime.TryParse(setting.CurrentValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, mydate) Then
                If mydate < Now.AddSeconds(-DateTimePrecission) OrElse mydate > Now.AddSeconds(DateTimePrecission) Then
                    NUnit.Framework.Assert.Fail(String.Format("Baseline date was not properly calcaulted, or process took more than {0} seconds.", DateTimePrecission))
                End If
            End If
            Debug.WriteLine("Baseline creation time properly set")

            NUnit.Framework.Assert.AreEqual(BLService.Status, ServiceStatusEnum.EndSuccess, "Service should be returning an EndSuccess at this stage")
            Debug.WriteLine("All tested.")
            NUnit.Framework.Assert.Pass("BLService status: " & BLService.Status.ToString)
        End Sub

        Private Sub CreateScenario()

            StatusParameters.IsActive = False
            InstantiateBLService()

        End Sub

        Private Sub InstantiateBLService()

            'Initialize Base line service:
            BLService = New BaseLineService(AnalyzerMock, Mock.Create(Of IAnalyzerManagerFlagsDelegate))

            'Dependency injection of mocks:

            'Creation mocks to be injected:
            Dim rotorStatusSerializer = Mock.Create(Of IReactionsRotorStatusSerializer)()
            Mock.Arrange(Function() rotorStatusSerializer.ChangeRotorPerformed(Arg.IsAny(Of SqlConnection), Arg.AnyString)).
                Returns(New GlobalDataTO With {.HasError = False})


            Dim blDeleter = Mock.Create(Of IWSBLinesDelegateValuesDeleter)()
            Mock.Arrange(Function() blDeleter.DeleteBLinesValues(Arg.IsAny(Of SqlConnection), Arg.AnyString, Arg.AnyString, Arg.AnyString, Arg.AnyString)).
                Returns(New GlobalDataTO With {.HasError = False})

            'Injection:
            BLService.ReactRotorStatusSerializer = rotorStatusSerializer
            BLService.BaselineValuesDeleter = blDeleter
            BLService.ReactRotorCRUD = Mock.Create(Of IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS))()
            BLService.AnalyzerSettingsSaver = AddressOf SaverMock
            BLService.AnalyzerAlarmsManager = Mock.Create(Of IAnalyzerAlarms)()
        End Sub

        Public Shared Function SaverMock(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String,
                                         ByVal pAnalyzerSettings As AnalyzerSettingsDS, ByVal pSessionSettings As UserSettingDS) As GlobalDataTO
            Return New GlobalDataTO With {.HasError = False}
        End Function

        Private ReadOnly Property AnalyzerMock As ISuperAnalyzer
            Get
                Static _analyzer As ISuperAnalyzer = Nothing
                If _analyzer Is Nothing Then
                    _analyzer = Mock.Create(Of ISuperAnalyzer)()

                    'We add a method to send Status events:
                    Mock.Arrange(Sub() _analyzer.ThrowStatusEvent()).Raises(Sub() AddHandler _analyzer.ReceivedStatusInformationEventHandler, Nothing)

                    'We add a method to make a mock of the UpdateSessionFlags. We'll use an internal dictionary instead of a real database.
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


                    Mock.Arrange(Sub() _analyzer.SessionFlag(Arg.IsAny(Of GlobalEnumerates.AnalyzerManagerFlags)) = Arg.AnyString).DoInstead(
                        Sub(param As GlobalEnumerates.AnalyzerManagerFlags, value As String)
                            _mySessionFlags(param.ToString) = value
                        End Sub)

                    Mock.Arrange(Function() _analyzer.ActiveAnalyzer).Returns(AnalyzerIDAttribute)

                    'AnalyzerSettingsDS
                    Static settings As AnalyzerSettingsDS
                    If settings Is Nothing Then
                        settings = New AnalyzerSettingsDS

                        Dim row As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow = settings.tcfgAnalyzerSettings.NewRow
                        row.AnalyzerID = _analyzer.ActiveAnalyzer
                        row.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.BL_DATETIME.ToString()
                        row.CurrentValue = Now.ToString
                        settings.tcfgAnalyzerSettings.Rows.Add(row)
                        Mock.Arrange(Function() _analyzer.AnalyzerSettings).Returns(
                            Function() As AnalyzerSettingsDS
                                Return settings
                            End Function)

                    End If

                    _analyzer.Connected = True
                    _analyzer.ThrowStatusEvent()
                End If
                Return _analyzer
            End Get
        End Property

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
                    AnalyzerMock.CurrentInstructionAction = GlobalEnumerates.InstructionActions.None
            End Select
            Return New GlobalDataTO With {.HasError = False}
        End Function

    End Class

    'This interface is required in order to be able to simulate events being thrown by the mocked analyzerManager
    Public Interface ISuperAnalyzer
        Inherits IAnalyzerManager
        Sub ThrowStatusEvent()
    End Interface

End Namespace



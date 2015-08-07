Imports System.Data.SqlClient
Imports System.Runtime.CompilerServices
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports NUnit.Framework
Imports Telerik.JustMock


Namespace Biosystems.Ax00.Core.Entities.Tests

    <TestFixture()> Public Class Error560Tests

#Region "Scenario Functions"

        Private Sub InitAnalyzerScenario_NormalStatus(ByVal analyzerManager As IAnalyzerManager)

            analyzerManager.CommThreadsStarted = True
            GlobalConstants.REAL_DEVELOPMENT_MODE = 2
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "INI")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "INPROCESS")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "")
            analyzerManager.InstructionTypeSent = GlobalEnumerates.AppLayerEventList.STATE
            analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.STANDBY
        End Sub

        Private Sub MockingAnalyzerFlags()

            Dim analyzerManagerFlagsMock As ItcfgAnalyzerManagerFlags = Mock.Create(Of ItcfgAnalyzerManagerFlags)()
            Dim myAlarmsFlagsDS As AnalyzerManagerFlagsDS = New AnalyzerManagerFlagsDS()
            Mock.Arrange(Function() analyzerManagerFlagsMock.Update(Arg.IsAny(Of SqlConnection), Arg.IsAny(Of AnalyzerManagerFlagsDS))).Returns(New GlobalDataTO() With {.SetDatos = myAlarmsFlagsDS, .HasError = False})
            AnalyzerManagerFlagsDelegate.GettcfgAnalyzerManagerFlagsDAO = Function() analyzerManagerFlagsMock
        End Sub

        Private Sub MockingWSAlarms()

            Dim wsAlarmsMock As ItwksWSAnalyzerAlarms = Mock.Create(Of ItwksWSAnalyzerAlarms)()
            Dim myWsAlarmsDS As WSAnalyzerAlarmsDS = New WSAnalyzerAlarmsDS()
            Dim myWsAlarmsDS2 As WSAnalyzerAlarmsDS = New WSAnalyzerAlarmsDS()
            myWsAlarmsDS2.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow("FBLD_ROTOR_FULL", "A200", Date.Now, 1, "", "", True, Date.Now)
            Mock.Arrange(Function() wsAlarmsMock.GetAlarmsMonitor(Arg.IsAny(Of SqlConnection), Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myWsAlarmsDS, .HasError = False})
            Mock.Arrange(Function() wsAlarmsMock.GetAlarmsMonitor(Arg.IsAny(Of SqlConnection), Arg.IsAny(Of String), Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myWsAlarmsDS, .HasError = False})
            Mock.Arrange(Function() wsAlarmsMock.GetByAlarmID(Arg.IsAny(Of SqlConnection), "FBLD_ROTOR_FULL", Arg.IsAny(Of Date), Arg.IsAny(Of Date), Arg.IsAny(Of String), Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myWsAlarmsDS2, .HasError = False})
            Mock.Arrange(Function() wsAlarmsMock.GetByAlarmID(Arg.IsAny(Of SqlConnection), "UNKNOW_ROTOR_FULL", Arg.IsAny(Of Date), Arg.IsAny(Of Date), Arg.IsAny(Of String), Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myWsAlarmsDS, .HasError = False})
            Mock.Arrange(Function() wsAlarmsMock.Update(Arg.IsAny(Of SqlConnection), Arg.IsAny(Of WSAnalyzerAlarmsDS))).Returns(New GlobalDataTO() With {.SetDatos = myWsAlarmsDS, .HasError = False})
            WSAnalyzerAlarmsDelegate.GettwksWSAnalyzersAlarmsDAO = Function() wsAlarmsMock
        End Sub

        Private Sub MockingAnalyzerConfigs()

            Dim analyzerCfgMock As ItcfgAnalyzers = Mock.Create(Of ItcfgAnalyzers)()
            Dim myConfigDS As AnalyzersDS = New AnalyzersDS()
            myConfigDS.tcfgAnalyzers.AddtcfgAnalyzersRow("A200", "A200", 1, "1.50.2", False, True, False)
            Mock.Arrange(Function() analyzerCfgMock.ReadByAnalyzerActive(Arg.IsAny(Of SqlConnection))).Returns(New GlobalDataTO() With {.SetDatos = myConfigDS, .HasError = False})
            ExecutionsDelegate.GettcfgAnalyzersDAO = Function() analyzerCfgMock
        End Sub

        Private Sub MockingAlarms()

            Dim alarmsMock As ItfmwAlarms = Mock.Create(Of ItfmwAlarms)()
            Dim myAlarmsDS As AlarmsDS = New AlarmsDS()
            Mock.Arrange(Function() alarmsMock.ReadAll(Arg.IsAny(Of SqlConnection))).Returns(New GlobalDataTO() With {.SetDatos = myAlarmsDS, .HasError = False})
            AlarmsDelegate.GettfmwtfmwAlarmsDAO = Function() alarmsMock
        End Sub

        Private Sub GenerateMockingPointers()

            CreateLogActivityPointer = Sub(str As String, str2 As String, type As EventLogEntryType, sys As Boolean) Debug.Print("Mocked CreateLogActivity with value: " & str)
            GetSafeOpenDBConnectionPointer = Function(connection As SqlConnection) New TypedGlobalDataTo(Of SqlConnection) With {.SetDatos = New SqlConnection, .HasError = False}
            GetOpenDBTransactionPointer = Function(pDBConnection As SqlConnection) New GlobalDataTO() With {.SetDatos = New SqlConnection, .HasError = False}
            GetBeginTransactionPointer = Sub(pDBConnection As SqlConnection) Debug.Print("Mocked BeginTransaction")
            GetCommitTransactionPointer = Sub(pDBConnection As SqlConnection) Debug.Print("Mocked CommitTransaction")
            GetRollbackTransactionPointer = Sub(pDBConnection As SqlConnection) Debug.Print("Mocked RollbackTransaction")
        End Sub

        Private Sub MockingSwParameters()

            Dim swParametersMock As ItfmwSwParameters = Mock.Create(Of ItfmwSwParameters)()
            Dim myParamsDS As ParametersDS = New ParametersDS()
            myParamsDS.tfmwSwParameters.AddtfmwSwParametersRow(1, "STRD_VALUE", False, "", 300, "", "")
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "USB_REGISTRY_KEY", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "MAX_TIME_DEPOSIT_WARN", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "MAX_TIME_THERMO_REACTIONS_WARN", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "MAX_TIME_THERMO_FRIDGE_WARN", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "MAX_TIME_THERMO_ARM_REAGENTS_WARN", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "WAITING_TIME_OFF", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "SYSTEM_TIME_OFFSET", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "WAITING_TIME_DEFAULT", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "WAITING_TIME_FAST", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "WAITING_TIME_ISE_FAST", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "WAITING_TIME_ISE_OFFSET", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "PREDILUTION_CYCLES", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})
            Mock.Arrange(Function() swParametersMock.ReadByParameterName(Arg.IsAny(Of SqlConnection), "CONTAMIN_REAGENT_PERSIS", Arg.IsAny(Of String))).Returns(New GlobalDataTO() With {.SetDatos = myParamsDS, .HasError = False})

            SwParametersDelegate.GettfmwSwParametersDAO = Function() swParametersMock
        End Sub
#End Region

        ''' <summary>
        ''' Mocking Scenario For ManageAnalyzer Received Status
        ''' Executed BEFORE each test
        ''' </summary>
        ''' <remarks></remarks>
        <TestFixtureSetUp()> Public Sub Init()
            GenerateMockingPointers()
            MockingSwParameters()
            MockingAlarms()
            MockingAnalyzerConfigs()
            MockingWSAlarms()
            MockingAnalyzerFlags()
        End Sub

        <Test()> Public Sub GetInstructionBuilt_TestTool_OK()
            Dim instruction = New List(Of InstructionParameterTO)

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;

            instruction.Add(New InstructionParameterTO With {.InstructionType = "A", .ParameterIndex = 1, .ParameterValue = "A200"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "TYPE", .ParameterIndex = 2, .ParameterValue = "STATUS"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "S", .ParameterIndex = 3, .ParameterValue = "2"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "A", .ParameterIndex = 4, .ParameterValue = "46"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "T", .ParameterIndex = 5, .ParameterValue = "15"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "C", .ParameterIndex = 6, .ParameterValue = "20"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "W", .ParameterIndex = 7, .ParameterValue = "20"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "R", .ParameterIndex = 8, .ParameterValue = "0"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "E", .ParameterIndex = 9, .ParameterValue = "560"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "I", .ParameterIndex = 10, .ParameterValue = "0"})


            Dim auxIntruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            For Each o As InstructionParameterTO In instruction
                Dim item = auxIntruction.Find(Function(value As InstructionParameterTO)
                                                  Return value.ParameterIndex = o.ParameterIndex
                                              End Function)
                Assert.AreEqual(o.InstructionType, item.InstructionType)
                Assert.AreEqual(o.ParameterValue, item.ParameterValue)
            Next

            Assert.AreEqual(GetInstructionInString(auxIntruction), "A:A200;TYPE:STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;")
        End Sub

        <Test()> Public Sub ManageAnalyzerReceivedStatus_StatusErr560RetryWithReset_OK()

            Dim analyzerManager As IAnalyzerManager = New BA200AnalyzerEntity(String.Empty, String.Empty, Nothing)

            'Scenario
            InitAnalyzerScenario_NormalStatus(analyzerManager)

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;
            Dim instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            'preconditions
            Assert.AreEqual(analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "")
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

            'act : Not connected and Retry
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'result and precondition for reset
            Assert.IsTrue(analyzerManager.CanManageRetryAlarm)
            Assert.IsTrue(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 1)

            'act : Failed retry and Reset system
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'result
            Assert.IsFalse(analyzerManager.AnalyzerIsReady)
            Assert.IsFalse(analyzerManager.IsAlarmInfoRequested)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

        End Sub

        <Test()> Public Sub ManageAnalyzerReceivedStatus_StatusErr560Retry_OK()

            Dim analyzerManager As IAnalyzerManager = New BA200AnalyzerEntity(String.Empty, String.Empty, Nothing)

            'Scenario
            InitAnalyzerScenario_NormalStatus(analyzerManager)

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;
            Dim instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            'precondition
            Assert.AreEqual(analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "")
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

            'act : Not connected and Retry
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'result
            Assert.IsTrue(analyzerManager.CanManageRetryAlarm)
            Assert.IsTrue(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 1)

            'preconditions: A200;STATUS;S:2;A:47;T:15;C:20;W:20;R:0;E:0;I:0;
            instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:47;T:15;C:20;W:20;R:0;E:0;I:0")

            'act : Finish Flight reads without problems
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'result
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

        End Sub
    End Class

End Namespace



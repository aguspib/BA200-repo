Imports System.Reflection
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports NUnit.Framework
Imports Telerik.JustMock
Imports Telerik.JustMock.Helpers

Namespace Biosystems.Ax00.Core.Services.Tests

    <TestFixture()> Public Class BaseLineServiceTests

        <Test()>
        Sub IntegrationTest()
            CreateScenario()

            Dim result = BLService.StartService

            Analyzer.ThrowReceivedStatusInformation()
            'Assert.AreEqual(result, True) ', "BaseLine service could not be initialized")
            Analyzer.ThrowReceivedStatusInformation()

        End Sub

        Shared BLService As BaseLineService

        Private Sub CreateScenario()

            BLService = New BaseLineService(Analyzer, Mock.Create(Of IAnalyzerManagerFlagsDelegate))
            Analyzer.Connected = True
            Analyzer.ThrowReceivedStatusInformation()

        End Sub
        Private ReadOnly Property Analyzer As ISuperAnalyzer
            Get
                Static _analyzer As ISuperAnalyzer = Nothing
                If _analyzer Is Nothing Then
                    _analyzer = Mock.Create(Of ISuperAnalyzer)()
                    Mock.Arrange(Sub() _analyzer.ThrowReceivedStatusInformation()).Raises(Sub() AddHandler _analyzer.ReceivedStatusInformationEventHandler, Nothing)
                End If
                Return _analyzer
            End Get
        End Property
    End Class

    Public Interface ISuperAnalyzer
        Inherits IAnalyzerManager
        Sub ThrowReceivedStatusInformation()

    End Interface

End Namespace



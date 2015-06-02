Imports System.IO
Imports NUnit.Framework
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Telerik.JustMock
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing
Imports Biosystems.Ax00.Types
Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Tests
    <TestFixture()>
    Public Class ContextStepTest

        ''' <summary>
        ''' We instantiate our invented analyzer for the test
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()
            ContextTests.CreateSpecification()
        End Sub

        ''' <summary>
        ''' Test the ContextStep basic constructor
        ''' </summary>
        ''' <remarks></remarks>
        <Test()> Sub BasicConstructor()
            Dim ContextStep = New ContextStep(ContaminationsSpecification.DispensesPerStep)
            For i = 1 To ContaminationsSpecification.DispensesPerStep
                Assert.AreSame(ContextStep(i), Nothing)
            Next
            Assert.AreEqual(ContextStep.DispensingPerStep, ContaminationsSpecification.DispensesPerStep)
        End Sub

        ''' <summary>
        ''' We test the LINQ support of our ContextStep class. It acts like a collection of Dispenses.
        ''' </summary>
        ''' <remarks></remarks>
        <Test()> Sub LINQSupportTest()
            Try
                Dim contextStep = New ContextStep(ContaminationsSpecification.DispensesPerStep)

                'count using the LINQ engine:
                Dim i As Integer = contextStep.Count()

                'Check for correctness:
                Assert.AreEqual(i, ContaminationsSpecification.DispensesPerStep)
            Catch ex As Exception
                Assert.Fail(ex.Message)
            End Try
        End Sub

        'This is just a handy little shortcut function
        Private Function ContaminationsSpecification() As IAnalyzerContaminationsSpecification
            Return WSExecutionCreator.Instance.ContaminationsSpecification
        End Function

    End Class
End Namespace

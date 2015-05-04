Imports Biosystems.Ax00.Types
Imports NUnit.Framework

Namespace Tests
    <TestFixture()> Public Class DelegatesToCoreBusinesGlueTests

        <Test()> Public Sub CreateContaminationManagerTest()
            DelegatesToCoreBusinesGlue.CreateContaminationManager(Nothing, "BA200", 2, 2, Nothing, New List(Of ExecutionsDS.twksWSExecutionsRow))
            'Assert.Fail()
        End Sub
    End Class


End Namespace



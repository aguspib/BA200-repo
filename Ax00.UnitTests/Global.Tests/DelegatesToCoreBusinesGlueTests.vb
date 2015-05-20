Imports Biosystems.Ax00.Types
Imports NUnit.Framework


Namespace Tests

    <TestFixture()> Public Class DelegatesToCoreBusinesGlueTests

        <Test()> Public Sub CreateContaminationManagerTest()
            Dim obj = New DelegatesToCoreBusinesGlue.ContaminationManagerWrapper(False, Nothing, "BA200", 2, 2, Nothing, New List(Of ExecutionsDS.twksWSExecutionsRow))
            obj.ApplyOptimizations(Nothing, "BA200", New List(Of ExecutionsDS.twksWSExecutionsRow)())
            Assert.AreNotEqual(obj.bestResult(), Nothing)
            Assert.AreEqual(obj.bestResult().Count, 0)
            Assert.AreEqual(obj.currentContaminationNumber(), 0)
        End Sub
    End Class


End Namespace



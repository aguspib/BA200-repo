Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports NUnit.Framework


Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Tests

    <TestFixture()> Public Class InstrumentFrameParserTests

        <Test()> Public Sub ParseTest()
            Dim parser = New LAx00Frame
            parser.ParseRawData("STATUS;A:34;B:45;CDT;I:45")
            Assert.AreEqual(parser("A"), "34")
            Assert.AreEqual(parser("B"), "45")
            Assert.AreEqual(parser("CDT"), "")
            Assert.AreEqual(parser("I"), "45")
        End Sub

    End Class

End Namespace



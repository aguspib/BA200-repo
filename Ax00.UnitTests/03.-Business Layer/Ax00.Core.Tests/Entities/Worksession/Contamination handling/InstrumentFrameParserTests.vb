Imports NUnit.Framework


Namespace Tests

    <TestFixture()> Public Class InstrumentFrameParserTests

        <Test()> Public Sub ParseTest()
            Dim P = New InstrumentFrameParser
            P.ParseRawData("STATUS;A:34;B:45;CDT;I:45")
            Assert.AreEqual(P("A"), "34")
            Assert.AreEqual(P("B"), "45")
            Assert.AreEqual(P("CDT"), "")
            Assert.AreEqual(P("I"), "45")
        End Sub

    End Class


End Namespace



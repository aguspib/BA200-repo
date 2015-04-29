Imports System.Runtime.Remoting
Imports System.Text
Imports NUnit.Framework

Imports Biosystems.Ax00.CommunicationsSwFw


Namespace Biosystems.Ax00.CommunicationsSwFw.Tests

    <TestFixture()> Public Class LAX00InterpreterTests

        <Test()> Public Sub ReadTest()
            Dim inter = New LAX00Interpreter()
            Dim result = inter.Read("STATUS;A:45;B:56")
            Assert.AreEqual(result.HasError, False)
            Dim instructions = TryCast(result.SetDatos, Instructions)
            Assert.AreEqual(instructions.ParameterList.Count, 3)

            Dim i = instructions.ParameterList(0)
            Assert.AreEqual(i.ParameterID, "STATUS")
            Assert.AreEqual(i.ParameterValues, "")
            i = instructions.ParameterList(1)
            Assert.AreEqual(i.ParameterID, "A")
            Assert.AreEqual(i.ParameterValues, "45")
            i = instructions.ParameterList(2)
            Assert.AreEqual(i.ParameterID, "B")
            Assert.AreEqual(i.ParameterValues, "56")
        End Sub

        <Test()> Public Sub CompleteReadParsing()
            Dim Frame = "A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;"
            Dim inter = New LAX00Interpreter
            Dim result = inter.Read(Frame)
            Dim instructions = TryCast(result.SetDatos, Instructions)
            Dim SB As New StringBuilder
            For Each par In instructions.ParameterList
                SB.Append(par.ParameterID)
                If par.ParameterValues <> String.Empty Then
                    SB.Append(":"c)
                    SB.Append(par.ParameterValues)
                End If
                SB.Append(";"c)
            Next
            Assert.AreEqual(Frame, SB.ToString)
        End Sub

        <Test()> Public Sub CompleteWriteParsing()
            Dim Frame = "A200;ANSINF;GC:1;PC:1;RC:1;SC:1;HS:1;WS:1;SW:1765;WW:2043;PT:37.5;FS:0;FT:0.0;HT:45.0;R1T:36.9;R2T:36.9;IS:1;HSV:336;WSV:311;"
            Dim inter = New LAX00Interpreter
            Dim result = inter.Read(Frame)
            Dim instructions = TryCast(result.SetDatos, Instructions)

            Dim inter2 = New LAX00Interpreter
            Dim result2 = inter2.Write(instructions.ParameterList)
            Assert.AreEqual(result2.SetDatos, Frame)



        End Sub
    End Class


End Namespace



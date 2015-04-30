Imports System.Reflection
Imports NUnit.Framework


Namespace Tests

    <TestFixture()> Public Class DelegatesToCoreBusinesGlueTests

        <Test()> Public Sub CreateWSTest()


            Dim result = DelegatesToCoreBusinesGlue.CreateWS(Nothing, "BA200", "A1234", True)

        End Sub

    End Class


End Namespace



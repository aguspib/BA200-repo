Imports System.Reflection
Imports NUnit.Framework


Namespace Tests

    <TestFixture()> Public Class DelegatesToCoreBusinesGlueTests

        <Test()> Public Sub CreateWSTest()
            'This test ONLY evaluates the capacity of the glue code to locate the assembly and its contained types, as this is all the functionality it does


            Assert.AreNotSame(DelegatesToCoreBusinesGlue.BsCoreAssembly(), Nothing)
            Dim result = DelegatesToCoreBusinesGlue.CreateWS(Nothing, "A345", "A1234", True)
            Assert.AreNotSame(result, Nothing)
        End Sub

    End Class


End Namespace



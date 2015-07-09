Imports Biosystems.Ax00.Core.Interfaces
Imports NUnit.Framework
Imports Telerik.JustMock

Namespace Tests
    <TestFixture()> Public Class BaseLineEntityExpirationTests

        <Test()> Public Sub NewTest()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            Dim baseLineEntityExpiration = New BaseLineEntityExpiration(analyzerManager)

            ' baseLineEntityExpiration.IsBlExpired
        End Sub
    End Class


End Namespace



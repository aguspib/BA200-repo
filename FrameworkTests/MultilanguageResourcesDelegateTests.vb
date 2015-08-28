Imports NUnit.Framework

Imports Biosystems.Ax00.BL

Namespace Biosystems.Ax00.BL.Tests
    <TestFixture()> Public Class MultilanguageResourcesDelegateTests

        <Test()> Public Sub RegisterKeywordTest()
            '1.- Que se registra 2 o mas keywords con functiones que devuelven strings

        End Sub

        <Test()> Public Sub ParseKeywordsTest()
            '1.- Que se registra 2 o mas keywords con functiones que devuelven strings
            '2.- Que se parsea bien el texto

            'Ejemplo:
            ' "This is my:[[MODEL]]!"   ->Ha de ir y respetar el espaciado
            ' "This is my:[[Model]]"   ->Ha de ir y respetar el espaciado
            ' "[[ModeL]][[ModeL]]"   ->Ha de ir y respetar el espaciado
            '"This is [Model]]" '-> ha de fallar

            Assert.Fail()
        End Sub

        <Test()> Public Sub GetResourceTextTest()
            'Hacer leer un recurso que contenga [[]] y ver que se retorna parseado.
            Assert.Fail()
        End Sub

    End Class


End Namespace



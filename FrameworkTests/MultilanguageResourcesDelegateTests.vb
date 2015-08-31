Imports NUnit.Framework

Imports Biosystems.Ax00.BL


Namespace Biosystems.Ax00.BL.Tests

    <TestFixture()> Public Class MultilanguageResourcesDelegateTests
        'Dim MlRdObject As MultilanguageResourcesDelegate


        <Test()> Public Sub RegisterKeywordTest()

            '1.- Que se registra 2 o mas keywords con functiones que devuelven strings
            Dim mlRdObject As MultilanguageResourcesDelegate = New MultilanguageResourcesDelegate()

            MultilanguageResourcesDelegate.RegisterKeyword("MODEL", Function() "BA200")
            Assert.Contains("MODEL", MultilanguageResourcesDelegate.mlResourceDictionary.Keys)


            MultilanguageResourcesDelegate.RegisterKeyword("NUMPOS200", Function() "34")
            MultilanguageResourcesDelegate.RegisterKeyword("NUMPOS400", Function() "98")

            Assert.AreEqual(MultilanguageResourcesDelegate.mlResourceDictionary.Count, 3)

        End Sub

        <Test()> Public Sub ParseKeywordsTest()
            Dim strResult As String

            '1.- Que se registra 2 o mas keywords con functiones que devuelven strings
            '2.- Que se parsea bien el texto

            Dim MlRdObject As MultilanguageResourcesDelegate = New MultilanguageResourcesDelegate()
            'strResult = MultilanguageResourcesDelegate.ParseKeywords("Este es el analizador Modelo : [[MODEL]]")
            'Assert.


            'strResult = MultilanguageResourcesDelegate.ParseKeywords("[[Model]]")
            'strResult = MultilanguageResourcesDelegate.ParseKeywords("[[ModeL]][[ModeL]]")
            'strResult = MultilanguageResourcesDelegate.ParseKeywords("[Model]]")

            'strResult = MultilanguageResourcesDelegate.ParseKeywords("Este es el analizador Modelo : [[MODEL]")


            'Assert.That(Sub() strResult = MultilanguageResourcesDelegate.ParseKeywords("Este es el analizador Modelo : [[MODEL]"), _
            'Throws.InstanceOf(Of MultiLanguageParseException)())
            'Assert.Equals(strResult, "The Este es el analizador Modelo : [[MODEL] has a sintaxis error.")

            'Ejemplo:
            ' "This is my:[[MODEL]]!"   ->Ha de ir y respetar el espaciado
            ' "This is my:[[Model]]"   ->Ha de ir y respetar el espaciado
            ' "[[ModeL]][[ModeL]]"   ->Ha de ir y respetar el espaciado
            '"This is [Model]]" '-> ha de fallar

            'Assert.Fail()
        End Sub

        <Test()> Public Sub GetResourceTextTest()
            'Hacer leer un recurso que contenga [[]] y ver que se retorna parseado.
            Assert.Fail()
        End Sub

    End Class


End Namespace



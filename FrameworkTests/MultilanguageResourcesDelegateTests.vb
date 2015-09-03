Imports NUnit.Framework
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Telerik.JustMock


Namespace Biosystems.Ax00.BL.Tests

    <TestFixture()> Public Class MultilanguageResourcesDelegateTests


        <Test()> Public Sub ParseKeywordsTest()
            Dim strResult As String
            Dim MlRdObject As New MultilanguageResourcesDelegate

            CreateScenario()

            '1.- Que se registra 2 o mas keywords con functiones que devuelven strings

            MultilanguageResourcesDelegate.RegisterKeyword("MODEL", Function() "BA200")
            Assert.Contains("MODEL", MultilanguageResourcesDelegate.mlResourceDictionary.Keys)

            MultilanguageResourcesDelegate.RegisterKeyword("NUMPOS200", Function() "34")
            MultilanguageResourcesDelegate.RegisterKeyword("NUMPOS400", Function() "98")

            '2.- Que se parsea bien el texto

            'Palabra clave correcta
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo :[[MODEL]]")
            Assert.AreEqual(strResult, "Este es el analizador Modelo :BA200")

            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo :[[ MODEL]]")
            Assert.AreEqual(strResult, "Este es el analizador Modelo :BA200")

            'Mas de una Palabra clave
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [[ModeL]][[ModeL]]")
            Assert.AreEqual(strResult, "Este es el analizador Modelo : BA200BA200")

            'Solo un [ de apertura y para cerrar.
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [Model]")
            Assert.AreEqual(strResult, "Este es el analizador Modelo : [Model]")

            'Solo un [ de apertura.
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [Model]]")
            Assert.AreEqual(strResult, "Este es el analizador Modelo : [Model]]")

            'Solo un [ de apertura.
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [Model]] y este [[Model]]")
            Assert.AreEqual(strResult, "Este es el analizador Modelo : [Model]] y este BA200")

            ' Primera palabra clave correcta, segunda no existe en el diccionario.
            strResult = MlRdObject.ParseKeywords("Este es el analizador [[MODEL]]  y tiene [[Posiciones]] en el rotor de muestras.")
            Assert.AreEqual(strResult, "Este es el analizador BA200  y tiene Posiciones en el rotor de muestras.")

            'Dos palabras claves correctas.
            strResult = MlRdObject.ParseKeywords("Este es el analizador [[MODEL]]  y tiene [[NUMPOS200]] posiciones en el rotor de muestras.")
            Assert.AreEqual(strResult, "Este es el analizador BA200  y tiene 34 posiciones en el rotor de muestras.")

            'Solo un ] al finalizar.
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [[Model]")
            Assert.AreEqual(strResult, "Este es el analizador Modelo : Model")

            'Palabra clave correcta, y los dos ultimos caracteres son los claudators de apertura.
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [[Model]][[")
            Assert.AreEqual(strResult, "Este es el analizador Modelo : BA200[[")
            
            '3- Test escribir a  log y no.
            ErrorLogged = False
            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [[]]", False)
            Assert.AreEqual(strResult, "Este es el analizador Modelo : ")
            Assert.IsFalse(ErrorLogged)

            strResult = MlRdObject.ParseKeywords("Este es el analizador Modelo : [[]]", True)
            Assert.AreEqual(strResult, "Este es el analizador Modelo : ")
            Assert.IsTrue(ErrorLogged)

        End Sub

        Private ErrorLogged As Boolean = False

        Sub MyCreateLogActivity(param1 As String, param2 As String, myEvent As EventLogEntryType, flag As Boolean)
            ErrorLogged = True
            Console.WriteLine(param1)
        End Sub

        Sub CreateScenario()
            GlobalBase.CreateLogActivityPointer = AddressOf MyCreateLogActivity
        End Sub

    End Class


End Namespace



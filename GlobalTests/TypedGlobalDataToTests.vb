Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.Global.Tests

    <TestClass()> Public Class TypedGlobalDataToTests

        <TestMethod()> Public Sub GetCompatibleGlobalDataTOTest()
            Dim i = New TypedGlobalDataTo(Of String)
            i.SetDatos = "Hello world!"
            Dim j = i.GetCompatibleGlobalDataTO
            Assert.AreEqual(i.SetDatos, j.SetDatos)
        End Sub

        <TestMethod()> Public Sub GetCompatibleGlobalDataTOTestObject()
            Dim i = New TypedGlobalDataTo(Of Object)
            i.SetDatos = New List(Of String)
            Dim j = i.GetCompatibleGlobalDataTO
            Assert.AreEqual(i.SetDatos, j.SetDatos)
        End Sub

        <TestMethod()> Public Sub GetCompatibleGlobalDataTOCompleteList()
            Dim i = New TypedGlobalDataTo(Of List(Of String))
            i.SetDatos = New List(Of String)
            i.AffectedRecords = 34
            i.ErrorCode = "-34"
            i.ErrorMessage = "This is a very nasty error"
            i.HasError = True
            i.SetUserLevel = "Superman"
            Dim j = i.GetCompatibleGlobalDataTO
            Assert.AreEqual(i.SetDatos, j.SetDatos)
            Assert.AreEqual(i.AffectedRecords, j.AffectedRecords)
            Assert.AreEqual(i.ErrorCode, j.ErrorCode)
            Assert.AreEqual(i.ErrorMessage, j.ErrorMessage)
            Assert.AreEqual(i.HasError, j.HasError)
            Assert.AreEqual(i.SetUserLevel, j.SetUserLevel)
        End Sub

    End Class


End Namespace



Imports NUnit.Framework

Imports Biosystems.Ax00.CC


Namespace Biosystems.Ax00.CC.Tests

    <TestFixture()>
    Public Class RangedCollectionTests
        Const max = 10, min = 10

        <Test()>
        Public Sub NewTest()
            '1.- Test indexes creation
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            If RangedCol.WorkingRange.minimum <> min OrElse RangedCol.WorkingRange.maximum <> max Then
                Assert.Fail()
            Else
                Assert.Pass()
            End If

        End Sub

        <Test()> Public Sub NewTest1()
            Dim RangedCol As New RangedCollection(Of String)(New Range(Of Integer)(min, max))
            If RangedCol.WorkingRange.minimum <> min OrElse RangedCol.WorkingRange.maximum <> max Then
                Assert.Fail()
            Else
                Assert.Pass()
            End If

        End Sub

        <Test()> Public Sub AddTest()
            Dim RangedCol As New RangedCollection(Of String)(New Range(Of Integer)(min, max))
            RangedCol.Add("Hola")
            Assert.AreEqual(RangedCol.Count, 1)

        End Sub

        <Test()> Public Sub RemoveLastTest()
            Dim RangedCol As New RangedCollection(Of String)(New Range(Of Integer)(min, max))
            RangedCol.Add("Hola")
            Dim string2 = "Adiós"
            RangedCol.Add(string2)
            Dim last = RangedCol.RemoveLast
            Assert.AreSame(last, string2)
            Assert.AreEqual(RangedCol.Count, 1)
        End Sub

        <Test()> Public Sub RemoveOutOfRangeItemsTest()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            If RangedCol.WorkingRange.minimum <> min OrElse RangedCol.WorkingRange.maximum <> max Then
                Assert.Fail()
            Else
                Assert.Pass()
            End If

            '2.- Test Allow OutOfRange
            RangedCol.AllowOutOfRange = True
            For i = 1 To 1000
                RangedCol.Add("test " & 1)
            Next
            Assert.Equals(RangedCol.Count, 1000)

            '3.- Test Remove Out Of Range
            RangedCol.RemoveOutOfRangeItems()
            Assert.Equals(RangedCol.Count, max - min)

        End Sub

        <Test()> Public Sub RemoveFirstTest()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            For i = 1 To 20
                RangedCol.Add("Sample " & 1)
            Next

            Dim first = RangedCol(min), second = RangedCol(min + 1)
            Dim count = RangedCol.Count()
            Assert.AreSame(RangedCol.RemoveFirst, first)
            Assert.AreEqual(RangedCol.First, second)
            Assert.AreEqual(RangedCol.Count, count - 1)

        End Sub

        <Test()> Public Sub GetTypedEnumeratorTest()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            For i = 1 To 20
                RangedCol.Add("Sample " & 1)
            Next
            Dim counter = 0
            For Each cadena In RangedCol
                counter += 1
            Next

        End Sub

        <Test()> Public Sub AllowOutOfRange()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            For i = 1 To 1000
                RangedCol.Add("Sample " & 1)
            Next

            RangedCol.AllowOutOfRange = False
            RangedCol.Add("placeholder")
            Assert.AreEqual(RangedCol.Add("Can't fit, there's no more room!"), False)

        End Sub


    End Class


End Namespace



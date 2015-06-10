Imports NUnit.Framework

Imports Biosystems.Ax00.CC


Namespace Biosystems.Ax00.CC.Tests

    <TestFixture()>
    Public Class RangedCollectionTests
        Const max = 10, min = 10

        <Test()>
        Public Sub IntegersConstructorTest()
            '1.- Test indexes creation
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            If RangedCol.Range.minimum <> min OrElse RangedCol.Range.maximum <> max Then
                Assert.Fail()
            Else
                Assert.Pass()
            End If

        End Sub

        <Test()> Public Sub RangeConstructorTest()
            Dim RangedCol As New RangedCollection(Of String)(New Range(Of Integer)(min, max))
            If RangedCol.Range.minimum <> min OrElse RangedCol.Range.maximum <> max Then
                Assert.Fail()
            Else
                Assert.Pass()
            End If

        End Sub

        <Test()> Public Sub AddTest()
            Dim RangedCol As New RangedCollection(Of String)(New Range(Of Integer)(min, max))
            RangedCol.Append("Hola")
            Assert.AreEqual(RangedCol.Count, 1)

        End Sub

        <Test()> Public Sub RemoveLastTest()
            Dim RangedCol As New RangedCollection(Of String)(New Range(Of Integer)(min, max))
            RangedCol.Append("Hola")
            Dim string2 = "Adiós"
            RangedCol.Append(string2)
            Dim last = RangedCol.RemoveLast
            Assert.AreSame(last, string2)
            Assert.AreEqual(RangedCol.Count, 1)
        End Sub

        <Test()> Public Sub RemoveOutOfRangeItemsTest()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            If RangedCol.Range.minimum <> min OrElse RangedCol.Range.maximum <> max Then
                Assert.Fail()
            Else
                Assert.Pass()
            End If

            '2.- Test Allow OutOfRange
            RangedCol.AllowOutOfRange = True
            For i = 1 To 1000
                RangedCol.Append("test " & 1)
            Next
            Assert.Equals(RangedCol.Count, 1000)

            '3.- Test Remove Out Of Range
            RangedCol.RemoveOutOfRangeItems()
            Assert.Equals(RangedCol.Count, max - min)

        End Sub

        <Test()> Public Sub RemoveFirstTest()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            For i = 1 To 20
                RangedCol.Append("Sample " & 1)
            Next

            Dim first = RangedCol(min), second = RangedCol(min + 1)
            Dim count = RangedCol.Count()
            Assert.AreSame(RangedCol.RemoveFirst, first)
            Assert.AreEqual(RangedCol.First, second)
            Assert.AreEqual(RangedCol.Count, count - 1)

        End Sub

        <Test()> Public Sub GetTypedEnumeratorTest()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            Const items = 20
            For i = 1 To items
                RangedCol.Append("Sample " & 1)
            Next
            Dim counter = 0
            'forEach uses GetTypedEnumerator internally, so it's the way to test it
            For Each cadena In RangedCol
                counter += 1
            Next
            Assert.AreEqual(counter, items)
        End Sub

        <Test()> Public Sub AllowOutOfRange()
            Dim RangedCol As New RangedCollection(Of String)(min, max)
            For i = 1 To max - min + 10
                RangedCol.Append("Sample " & 1)
            Next

            RangedCol.AllowOutOfRange = False
            RangedCol.Append("placeholder")
            Assert.AreEqual(RangedCol.Append("Can't fit, there's no more room!"), False)

        End Sub


    End Class


End Namespace



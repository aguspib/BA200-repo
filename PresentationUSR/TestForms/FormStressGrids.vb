Imports System.Text

Public Class FormStressGrids
    Private myDataTable As DataTable
    Private myDummyDT As DataTable
    Private Shared random As New Random()
    Private Const legalCharacters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"

    ''' <summary>
    ''' The Typing Monkey Generates a random string with the given length.
    ''' </summary>
    ''' <param name="size">Size of the string</param>
    ''' <returns>Random string</returns>
    Public Function TypeAway(ByVal size As Integer) As String
        Dim builder As New StringBuilder()
        Dim ch As Char

        For i As Integer = 0 To size - 1
            ch = legalCharacters(random.[Next](0, legalCharacters.Length))
            builder.Append(ch)
        Next

        Return builder.ToString()
    End Function


    Private Sub PrepareDataTable()
        myDummyDT = New DataTable("DummyDT")
        'With myDummyDT
        '    .Columns.Add("DUMMY", GetType(Integer))
        '    .Columns.Add("DUMMY1", GetType(String))
        'End With
        'Dim row As DataRow = myDummyDT.NewRow()
        'row.SetField(0, 12345)
        'row.SetField(1, "Dummy values")
        'Try
        '    myDummyDT.Rows.Add(row)
        'Catch ex As Exception

        'End Try


        myDataTable = New DataTable("PagingDataTable")
        With myDataTable
            .Columns.Add("Id", GetType(Integer))
            .Columns.Add("Column1", GetType(String))
            .Columns.Add("Column2", GetType(String))
            .Columns.Add("Column3", GetType(String))
            .Columns.Add("Column4", GetType(String))
            .Columns.Add("Column5", GetType(String))
            .Columns.Add("Column6", GetType(String))
            .Columns.Add("Column7", GetType(String))
            .Columns.Add("Column8", GetType(String))
            .Columns.Add("Column9", GetType(String))
        End With
    End Sub


    Private Sub InsertRandomRowToDataTable(ByVal rowNum As Integer)
        Dim row As DataRow = myDataTable.NewRow()
        row.SetField(0, rowNum)
        row.SetField(1, TypeAway(1))
        row.SetField(2, TypeAway(1))
        row.SetField(3, TypeAway(3))
        row.SetField(4, TypeAway(4))
        row.SetField(5, TypeAway(5))
        row.SetField(6, TypeAway(6))
        row.SetField(7, TypeAway(7))
        row.SetField(8, TypeAway(8))
        row.SetField(9, TypeAway(9))

        Try
            myDataTable.Rows.Add(row)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub InsertRandomDataToDataTable(ByVal numRows As Integer)
        myDataTable.Clear()
        myDataTable.Dispose()
        myDataTable = Nothing
        PrepareDataTable()

        pbRegisters.Value = 0
        lblRegisters.Text = ""
        pbRegisters.Refresh()
        lblRegisters.Refresh()

        Dim oldTotal As Integer = myDataTable.Rows.Count
        If oldTotal < numRows Then
            Dim value As Integer
            For i As Integer = oldTotal To numRows - 1
                InsertRandomRowToDataTable(i)
                value = ((i + 1) * 100) \ numRows
                If i = oldTotal Or i = numRows - 1 Or value - pbRegisters.Value > 1 Then
                    pbRegisters.Value = value
                    pbRegisters.Refresh()
                    lblRegisters.Text = myDataTable.Rows.Count.ToString & "/" & numRows.ToString
                    lblRegisters.Refresh()
                End If
            Next
        ElseIf oldTotal > numRows Then
            Dim value As Integer
            For i As Integer = numRows To oldTotal - 1
                myDataTable.Rows.RemoveAt(myDataTable.Rows.Count - 1)

                value = ((i + 1) * 100) \ (oldTotal)
                If i = numRows Or i = oldTotal - 1 Or value - pbRegisters.Value > 1 Then
                    pbRegisters.Value = value
                    pbRegisters.Refresh()
                    lblRegisters.Text = myDataTable.Rows.Count.ToString & "/" & numRows.ToString
                    lblRegisters.Refresh()
                End If
            Next
        End If

    End Sub

    Public Sub New()
        PrepareDataTable()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        lblRegisters.Text = ""
        lblTime1.Text = ""
    End Sub


    Private Sub GenerateDataTable()
        'lblTime1.Text = ""
        'lblTime2.Text = ""

        lblAction.Text = "Creating DataTable..."
        lblAction.Refresh()
        InsertRandomDataToDataTable(numRows.Value)

        lblAction.Text = "Ready"
        lblAction.Refresh()
    End Sub

    Private Sub butBindGrid1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butBindGrid1.Click
        lblAction.Text = "Clearing data in grid (1)..."
        lblAction.Refresh()
        dxpGrid1.DataSource = myDummyDT
        dxpGrid1.Refresh()

        GenerateDataTable()

        Dim start_time As DateTime
        dxpGrid1.DataSource = myDummyDT
        dxpGrid1.Refresh()

        lblAction.Text = "Attaching data to grid (1)..."
        lblAction.Refresh()
        dxpGrid1.DataSource = Nothing
        start_time = Now
        dxpGrid1.DataSource = myDataTable
        dxpGrid1.Refresh()
        lblTime1.Text = (Now.Subtract(start_time).TotalSeconds * 1000).ToString("0000") & " ms"

        lblAction.Text = "Ready"
        lblAction.Refresh()
    End Sub

    Private Sub butBindGrid2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butBindGrid2.Click
        lblAction.Text = "Clearing data in grid (2)..."
        lblAction.Refresh()
        dxpGrid2.DataSource = myDummyDT
        dxpGrid2View.OptionsView.AllowCellMerge = True
        For i As Integer = 0 To dxpGrid2View.Columns.Count - 1
            If i = 1 Or i = 2 Or i = 3 Then
                dxpGrid2View.Columns(i).OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
            Else
                dxpGrid2View.Columns(i).OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            End If
        Next
        dxpGrid2.Refresh()

        GenerateDataTable()

        Dim start_time As DateTime
        dxpGrid2.DataSource = myDummyDT
        dxpGrid2.Refresh()

        lblAction.Text = "Attaching data to grid (2)..."
        lblAction.Refresh()
        dxpGrid2.DataSource = Nothing
        start_time = Now
        dxpGrid2.DataSource = myDataTable
        dxpGrid2.Refresh()
        lblTime2.Text = (Now.Subtract(start_time).TotalSeconds * 1000).ToString("0000") & " ms"

        lblAction.Text = "Ready"
        lblAction.Refresh()

    End Sub

    Private Sub butBindGrid3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butBindGrid3.Click
        lblAction.Text = "Clearing data in grid (3)..."
        lblAction.Refresh()
        dgrGrid3.DataSource = myDummyDT
        dgrGrid3.Refresh()

        GenerateDataTable()

        Dim start_time As DateTime
        dgrGrid3.DataSource = myDummyDT
        dgrGrid3.Refresh()

        lblAction.Text = "Attaching data to grid (3)..."
        lblAction.Refresh()
        dgrGrid3.DataSource = Nothing
        start_time = Now
        dgrGrid3.DataSource = myDataTable
        dgrGrid3.Refresh()
        lblTime3.Text = (Now.Subtract(start_time).TotalSeconds * 1000).ToString("0000") & " ms"

        lblAction.Text = "Ready"
        lblAction.Refresh()
    End Sub
End Class
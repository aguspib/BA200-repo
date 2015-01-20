Imports DevExpress.XtraReports.UI

Public Class GenericTableReport

    Private SaveProcessDuplicates As New Dictionary(Of String, DevExpress.XtraReports.UI.ValueSuppressType)

    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    'AJG
    Public Overloads Sub SetDataSource(ByVal aDataSource As IList)
        DataSource = aDataSource
    End Sub

    Public Overloads Sub SetDataSource(ByVal aDataSource As DataTable)
        DataSource = aDataSource
    End Sub

    Public Sub AddTableRowCells(ByVal Cells As XRTableCell())
        XrTableRowDetails.Cells.Clear()
        XrTableRowDetails.Cells.AddRange(Cells)
    End Sub

    Public Sub AddTableHeaderCells(ByVal Cells As XRTableCell())
        XrTableRowHeader.Cells.Clear()
        XrTableRowHeader.Cells.AddRange(Cells)
    End Sub

    Private Sub XrTableRowDetails_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableRowDetails.BeforePrint
        Dim Row As XRTableRow = TryCast(sender, XRTableRow)

        If Not TypeOf Row.Report.DataSource Is IList Then Return

        'Save Cell's ProcessDuplicates value
        For Each Cell As XRTableCell In Row.Cells
            SaveProcessDuplicates(Cell.Name) = Cell.ProcessDuplicates
        Next

        Dim Index As Integer = Row.Report.CurrentRowIndex

        If Index > 0 Then
            Dim theList As IList = CType(Row.Report.DataSource, IList)
            Dim theListRow As String
            Dim DataMember As String
            Dim FormatString As String
            Dim DrawBorder As Boolean = False


            For Each Cell As XRTableCell In Row.Cells
                If Cell.ProcessDuplicates = ValueSuppressType.Suppress Then
                    theListRow = theList(Index - 1).ToString()
                    DataMember = Cell.DataBindings.Item(0).DataMember
                    FormatString = String.Format("{0} = {1}", DataMember, Cell.Text)

                    If Not theListRow.Contains(FormatString) Then
                        DrawBorder = True
                        Exit For
                    End If
                End If
            Next

            If DrawBorder Then
                Row.Borders = DevExpress.XtraPrinting.BorderSide.Top

                For Each Cell As XRTableCell In Row.Cells
                    Cell.ProcessDuplicates = ValueSuppressType.Leave
                Next
            Else
                Row.Borders = DevExpress.XtraPrinting.BorderSide.None
            End If
        End If
    End Sub

    Private Sub XrTableRowDetails_AfterPrint(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XrTableRowDetails.AfterPrint
        Dim Row As XRTableRow = TryCast(sender, XRTableRow)

        'Restore Cell's ProcessDuplicates value
        For Each Cell As XRTableCell In Row.Cells
            Cell.ProcessDuplicates = SaveProcessDuplicates(Cell.Name)
        Next
    End Sub

    Private Sub XtraSubReportTable_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles MyBase.BeforePrint
        Dim Width As Integer = PageWidth - Margins.Left - Margins.Right
        Dim Margin As Single = 20.0!
        Dim Height As Single = 20.0!

        XrHeaderLabel.SizeF = New System.Drawing.SizeF(Width, Height)
        XrTableHeader.SizeF = New System.Drawing.SizeF(Width - Margin, Height)
        XrTableDetails.SizeF = New System.Drawing.SizeF(Width - Margin, Height)
    End Sub

End Class
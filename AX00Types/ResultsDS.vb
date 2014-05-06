Partial Class ResultsDS

    Partial Class vwksResultsDataTable

        Private Sub vwksResultsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.RelativeErrorCurveColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

    Partial Class ReportSampleDetailsDataTable

        'RH 05/01/2012
        'RH 16/05/2012 New field Remarks
        Public Overloads Function AddReportSampleDetailsRow(ByVal PatientID As String, ByVal TestName As String, ByVal SampleType As String, ByVal ReplicateNumber As String, ByVal ABSValue As String, ByVal CONC_Value As String, ByVal ReferenceRanges As String, ByVal Unit As String, ByVal ResultDate As String, ByVal Remarks As String) As ReportSampleDetailsRow
            Dim rowReportSampleDetailsRow As ReportSampleDetailsRow = CType(Me.NewRow, ReportSampleDetailsRow)
            Dim columnValuesArray() As Object = New Object() {PatientID, TestName, SampleType, ReplicateNumber, ABSValue, CONC_Value, ReferenceRanges, Unit, ResultDate, Remarks}

            rowReportSampleDetailsRow.ItemArray = columnValuesArray
            Me.Rows.Add(rowReportSampleDetailsRow)
            Return rowReportSampleDetailsRow
        End Function

    End Class

    Partial Class ReportTestDetailsDataTable

        'RH 12/01/2012
        Public Overloads Function AddReportTestDetailsRow(ByVal TestTypeTestID As String, ByVal SampleClass As String, ByVal Name As String, ByVal SampleType As String, ByVal ReplicateNumber As String, ByVal ABSValue As String, ByVal CONC_Value As String, ByVal MeasureUnit As String, ByVal CalibratorFactor As String, ByVal ResultDate As String) As ReportTestDetailsRow
            Dim rowReportTestDetailsRow As ReportTestDetailsRow = CType(Me.NewRow, ReportTestDetailsRow)
            Dim columnValuesArray() As Object = New Object() {TestTypeTestID, SampleClass, Name, SampleType, ReplicateNumber, ABSValue, CONC_Value, MeasureUnit, CalibratorFactor, ResultDate}

            rowReportTestDetailsRow.ItemArray = columnValuesArray
            Me.Rows.Add(rowReportTestDetailsRow)
            Return rowReportTestDetailsRow
        End Function

    End Class

End Class
Partial Class CalibratorsDS
    Partial Class tparCalibratorsDataTable

        Private Sub tparCalibratorsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.IsNewColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

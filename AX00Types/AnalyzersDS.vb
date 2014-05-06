Partial Class AnalyzersDS
    Partial Class tcfgAnalyzersDataTable

        Private Sub tcfgAnalyzersDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.AnalyzerModelColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

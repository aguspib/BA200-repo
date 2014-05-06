Partial Class WSRequiredElementsDS
    Partial Class twksWSRequiredElementsDataTable

        Private Sub twksWSRequiredElementsDataTable_ColumnChanging(sender As Object, e As DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.ElementFinishedColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

Partial Class AbsorbanceDS
    Partial Class twksAbsorbancesDataTable

        Private Sub twksAbsorbancesDataTable_twksAbsorbancesRowChanging(ByVal sender As System.Object, ByVal e As twksAbsorbancesRowChangeEvent) Handles Me.twksAbsorbancesRowChanging

        End Sub

        Private Sub twksAbsorbancesDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.DarkMainCountsColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

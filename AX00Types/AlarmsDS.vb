Partial Class AlarmsDS
    Partial Class tfmwAlarmsDataTable

        Private Sub tfmwAlarmsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.DescriptionColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

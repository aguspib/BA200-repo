

Partial Public Class TimeEstimationDS
    Partial Class TestTimeValuesDataTable

        Private Sub TestTimeValuesDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.OrderIDColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

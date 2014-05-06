

Partial Public Class WSPreparationsDS
    Partial Class twksWSPreparationsDataTable

        Private Sub twksWSPreparationsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.LAX00DataColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

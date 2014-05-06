

Partial Public Class OrderTestsDS
    Partial Class twksOrderTestsDataTable

        Private Sub twksOrderTestsDataTable_ColumnChanging(sender As Object, e As DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.ESOrderIDColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

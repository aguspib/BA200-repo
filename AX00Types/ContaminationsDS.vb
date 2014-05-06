

Partial Public Class ContaminationsDS
    Partial Class tparContaminationsDataTable

        Private Sub tparContaminationsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.TS_DateTimeColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

        Private Sub tparContaminationsDataTable_tparContaminationsRowChanging(ByVal sender As System.Object, ByVal e As tparContaminationsRowChangeEvent) Handles Me.tparContaminationsRowChanging

        End Sub

    End Class

End Class

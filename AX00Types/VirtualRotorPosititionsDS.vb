

Partial Public Class VirtualRotorPosititionsDS
    Partial Class tparVirtualRotorPosititionsDataTable

        Private Sub tparVirtualRotorPosititionsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.MultiTubeNumberColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

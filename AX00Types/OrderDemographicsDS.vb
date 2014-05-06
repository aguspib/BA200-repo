Partial Class OrderDemographicsDS
    Partial Class twksOrderDemographicsDataTable

        Private Sub twksOrderDemographicsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.OrderIDColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class

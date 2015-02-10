Partial Class ExecutionsDS


    Partial Class vwksWSExecutionsMonitorRow
        Implements IMonitorCurveResultsRow

        Public Property InterfaceMultiItemNumber As Integer Implements IMonitorCurveResultsRow.MultiItemNumber
            Get
                Return Me.MultiItemNumber()
            End Get
            Set(value As Integer)
                Me.MultiItemNumber = value
            End Set
        End Property


        Public Property InterfaceOrderTestID As Integer Implements IMonitorCurveResultsRow.OrderTestID
            Get
                Return Me.OrderTestID
            End Get
            Set(value As Integer)
                Me.OrderTestID = value
            End Set
        End Property

        Public Property InterfaceRerunNumber As Integer Implements IMonitorCurveResultsRow.RerunNumber
            Get
                Return Me.RerunNumber
            End Get
            Set(value As Integer)
                Me.RerunNumber = CShort(value)
            End Set
        End Property

    End Class

End Class

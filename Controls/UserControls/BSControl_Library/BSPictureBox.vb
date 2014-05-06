Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSPictureBox
#Region "Attributes"
        Private PositionNumberAttribute As Integer
#End Region

#Region "Properties"
        Public Property PositionNumber() As Integer
            Get
                Return PositionNumberAttribute
            End Get
            Set(ByVal value As Integer)
                PositionNumberAttribute = value
            End Set
        End Property

#End Region

        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(pe)

            'Add your custom paint code here
        End Sub

    End Class
End Namespace
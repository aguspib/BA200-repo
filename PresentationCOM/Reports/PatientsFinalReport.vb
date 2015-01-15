

Public Class PatientsFinalReport

    Public Sub SetHeaderLabel(ByVal aText As String)
        'XrHeaderLabel.Text = aText  ' EF 03/06/2014 (se anula titulo en cabecero)
    End Sub

    'Private Sub XrTableDetailsRow_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableDetailsRow.BeforePrint
    '    If Not String.IsNullOrEmpty(XrTableCellTestName.Text) Then
    '        XrTableDetailsRow.BackColor = Drawing.Color.WhiteSmoke
    '    Else
    '        XrTableDetailsRow.BackColor = Drawing.Color.Transparent
    '    End If
    'End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub PatientsFinalReport_DesignerLoaded(sender As Object, e As DevExpress.XtraReports.UserDesigner.DesignerLoadedEventArgs) Handles Me.DesignerLoaded

    End Sub
End Class
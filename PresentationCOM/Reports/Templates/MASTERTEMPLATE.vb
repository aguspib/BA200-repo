Imports System.ComponentModel.Design
Imports DevExpress.Charts.Native
Imports DevExpress.XtraReports.UI

Public Class MASTERTEMPLATE
    Private Sub XtraReport1_DesignerLoaded(ByVal sender As Object, ByVal e As DevExpress.XtraReports.UserDesigner.DesignerLoadedEventArgs) Handles Me.DesignerLoaded

        Try
            Dim designer As ComponentDesigner = DirectCast(e.DesignerHost.GetDesigner(DirectCast(sender, XtraReport)), ComponentDesigner)

            designer.Verbs.RemoveAt(1)
            designer.Verbs.RemoveAt(0)

            Dim numAct As Integer = designer.ActionLists.Count - 1
            For i = numAct To 0 Step -1
                designer.ActionLists.RemoveAt(i)
            Next

            ' Obtain the TopMargin band and remove it from the report. 
            'Dim band As Band = Bands.GetBandByType(GetType(TopMarginBand))
            'If Not (band Is Nothing) Then
            '    Bands.Remove(band)
            'End If

            '' Obtain the BottomMargin band and remove it from the report. 
            'band = Bands.GetBandByType(GetType(BottomMarginBand))
            'If Not (band Is Nothing) Then
            '    Bands.Remove(band)
            'End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub



    Private Sub XtraReport1_FilterControlProperties(ByVal sender As Object, ByVal e As DevExpress.XtraReports.UserDesigner.FilterControlPropertiesEventArgs) Handles Me.FilterControlProperties

        Try
            If e.Properties.Count > 0 Then
                e.Properties.Remove("DataBindings")
                e.Properties.Remove("Summary")
                e.Properties.Remove("FormattingRules")
                e.Properties.Remove("AnchorVertical")
                e.Properties.Remove("RunningBand")
                e.Properties.Remove("FormattingRules")
                e.Properties.Remove("Tag")
            End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

End Class